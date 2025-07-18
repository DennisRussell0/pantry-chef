using System.Globalization;
using CsvHelper;
using PantryChef.API.Data;
using PantryChef.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace PantryChef.API.Services;

// Service for seeding the database with data from a CSV file
public class CsvSeeder
{
    private readonly PantryChefDbContext _dbContext;
    private readonly ILogger<CsvSeeder> _logger;
    private readonly IngredientParser _ingredientParser;
    private readonly string _csvPath = Path.Combine("..", "Assets", "data.csv");

    public CsvSeeder(PantryChefDbContext dbContext, ILogger<CsvSeeder> logger, IngredientParser ingredientParser)
    {
        _dbContext = dbContext;
        _logger = logger;
        _ingredientParser = ingredientParser;
    }

    // Removes all existing data to ensure clean seeding
    private async Task ClearDatabaseAsync()
    {
        _dbContext.Recipes.RemoveRange(_dbContext.Recipes); // Remove all recipes
        _dbContext.Ingredients.RemoveRange(_dbContext.Ingredients); // Remove all ingredients
        await _dbContext.SaveChangesAsync();

        // Reset ID sequences for PostgreSQL
        await _dbContext.Database.ExecuteSqlRawAsync("ALTER SEQUENCE \"Ingredients_Id_seq\" RESTART WITH 1");
        await _dbContext.Database.ExecuteSqlRawAsync("ALTER SEQUENCE \"Recipes_Id_seq\" RESTART WITH 1");
    }

    // Main method to seed database from CSV file
    public async Task SeedAsync()
    {
        await ClearDatabaseAsync(); // Clear the database before seeding

        using var reader = new StreamReader(_csvPath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        // Configure CSV reader to ignore these "recipes"
        var unwantedTitles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Fabric-Covered Vase Centerpiece",
            "Napkin Rings and a Decorated Vase from Corrugated Cardboard",
        };

        // Read all records from the CSV file
        var records = csv.GetRecords<CsvRecipe>().ToList();

        // Cache existing ingredients to avoid duplication
        var existingIngredients = await _dbContext.Ingredients
            .ToDictionaryAsync(i => i.Name, StringComparer.OrdinalIgnoreCase);

        var recipes = new List<Recipe>();

        foreach (var record in records)
        {
            // Skip unwanted "recipes" based on their titles
            if (unwantedTitles.Contains(record.Title.Trim()))
            {
                _logger.LogInformation("Skipping unwanted recipe: {Title}", record.Title);
                continue;
            }

            // Step 1: Remove enclosing square brackets
            var cleanedIngredients = record.Cleaned_Ingredients.Trim('[', ']');

            // Step 2: Split on commas outside of quotes
            var ingredients = Regex.Split(cleanedIngredients, @",(?=(?:[^']*'[^']*')*[^']*$)")
                .Select(i => i.Trim().Trim('\''))
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .ToList();

            // Parse ingredients asynchronously
            var recipeIngredients = new List<RecipeIngredient>();
            foreach (var i in ingredients)
            {
                var trimmed = i.Trim();

                // Explicitly skip unwanted entries
                if (string.IsNullOrWhiteSpace(trimmed) ||
                    trimmed.Equals("divided", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Equals("to taste", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var coreName = await _ingredientParser.ExtractCoreIngredientSmartAsync(trimmed);

                // Check if the ingredient already exists in the database
                if (!existingIngredients.TryGetValue(coreName, out var ingredient))
                {
                    ingredient = new Ingredient { Name = coreName };
                    existingIngredients[coreName] = ingredient;
                    _dbContext.Ingredients.Add(ingredient);
                }

                var recipeIngredient = new RecipeIngredient
                {
                    Ingredient = ingredient,
                    OriginalText = trimmed
                };
                recipeIngredients.Add(recipeIngredient);
            }

            // Avoid duplicate RecipeIngredient entries for the same ingredient
            var uniqueRecipeIngredients = recipeIngredients
                .GroupBy(ri => ri.Ingredient.Name)
                .Select(g => g.First())
                .ToList();

            var recipe = new Recipe
            {
                Title = record.Title,
                Instructions = record.Instructions,
                ImageName = record.Image_Name,
                RecipeIngredients = uniqueRecipeIngredients
            };

            recipes.Add(recipe);
        }

        // Batch insert recipes
        await _dbContext.Recipes.AddRangeAsync(recipes);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Database seeded with {Count} recipes.", recipes.Count);
    }

    // Represents a single row in the CSV file
    private class CsvRecipe
    {
        public string Title { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public string Cleaned_Ingredients { get; set; } = string.Empty;
        public string Image_Name { get; set; } = string.Empty;
    }
}