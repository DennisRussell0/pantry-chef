using PantryChef.API.DTOs;
using PantryChef.API.Interfaces;

namespace PantryChef.API.Services;

// Service for handling recipe-related business logic
public class RecipeService
{
    private readonly IRecipeRepository _recipeRepository;

    // Constructor to inject the RecipeRepository dependency
    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    // Fetch recipes that match the user's selected ingredients
    public async Task<IEnumerable<RecipeDto>> GetMatchingRecipesAsync(HashSet<string> userIngredients)
    {
        // Always assume these ingredients are available
        userIngredients.Add("salt");
        userIngredients.Add("black pepper");
        userIngredients.Add("olive oil");
        userIngredients.Add("vegetable oil");
        userIngredients.Add("water");

        // Retrieve recipes with at least one matching ingredient
        var recipes = await _recipeRepository.GetMatchingRecipesAsync(userIngredients);

        // Calculate matching percentage and map to RecipeDto
        return recipes.Select(r =>
        {
            var totalIngredients = r.RecipeIngredients.Count; // Total ingredients in the recipe
            var matchingIngredients = r.RecipeIngredients.Count(ri => userIngredients.Contains(ri.Ingredient.Name.ToLower())); // Count of matching ingredients

            // Calculate the percentage of matching ingredients
            var matchingPercentage = totalIngredients > 0
                ? (double)matchingIngredients / totalIngredients * 100
                : 0;

            // Map the Recipe entity to a RecipeDto
            return new RecipeDto
            {
                Id = r.Id,
                Title = r.Title,
                MatchingPercentage = matchingPercentage,
                Ingredients = r.RecipeIngredients.Select(ri => ri.Ingredient.Name).ToList(),
                OriginalText = r.RecipeIngredients.Select(ri => ri.OriginalText).ToList(),
                Instructions = r.Instructions,
                ImageName = r.ImageName,
            };
        })
        .OrderByDescending(r => r.MatchingPercentage) // Sort recipes by matching percentage in descending order
        .ToList();
    }
}