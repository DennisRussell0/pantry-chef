using System.Globalization;
using CsvHelper;
using PantryChef.API.Data;
using PantryChef.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace PantryChef.API.Services;

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
        // Remove all data from the Recipes and Ingredients tables
        _dbContext.Recipes.RemoveRange(_dbContext.Recipes);
        _dbContext.Ingredients.RemoveRange(_dbContext.Ingredients);
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

        // Read all records from the CSV file
        var records = csv.GetRecords<CsvRecipe>().ToList();

        // Cache existing ingredients to avoid duplication
        var existingIngredients = await _dbContext.Ingredients
            .ToDictionaryAsync(i => i.Name, StringComparer.OrdinalIgnoreCase);

        var recipes = new List<Recipe>();

        foreach (var record in records)
        {
            // Step 1: Remove enclosing square brackets
            var cleanedIngredients = record.Cleaned_Ingredients.Trim('[', ']');

            // Step 2: Split on commas outside of quotes
            var ingredients = Regex.Split(cleanedIngredients, @",(?=(?:[^']*'[^']*')*[^']*$)")
                .Select(i => i.Trim().Trim('\'')) // Trim whitespace and surrounding single quotes
                .Where(i => !string.IsNullOrWhiteSpace(i)) // Remove empty entries
                .ToList();

            // Create a new Recipe object for each record
            var recipe = new Recipe
            {
                Title = record.Title,
                Instructions = record.Instructions,
                ImageName = record.Image_Name,
                RecipeIngredients = ingredients
                    .Select(i =>
                    {
                        var trimmed = i.Trim();
                        var coreName = _ingredientParser.ExtractCoreIngredient(trimmed);

                        // Check if the ingredient already exists in the database
                        if (!existingIngredients.TryGetValue(coreName, out var ingredient))
                        {
                            // Create a new ingredient and add it to the dictionary
                            ingredient = new Ingredient { Name = coreName };
                            existingIngredients[coreName] = ingredient;

                            // Add the new ingredient to the DbContext
                            _dbContext.Ingredients.Add(ingredient);
                        }

                        return new RecipeIngredient
                        {
                            Ingredient = ingredient,
                            OriginalText = trimmed
                        };
                    })
                    .GroupBy(ri => ri.Ingredient.Name) // Avoid duplicate RecipeIngredient entries
                    .Select(g => g.First()) // Take the first unique RecipeIngredient
                    .ToList()
            };

            recipes.Add(recipe); // Add the recipe to the list
        }

        // Batch insert recipes
        await _dbContext.Recipes.AddRangeAsync(recipes);
        // Save changes to the database
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
