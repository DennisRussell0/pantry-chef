using PantryChef.API.DTOs;
using PantryChef.API.Interfaces;
using PantryChef.API.Models;

namespace PantryChef.API.Services;

public class RecipeService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public async Task<IEnumerable<RecipeDto>> GetMatchingRecipesAsync(HashSet<string> userIngredients)
    {
        // Retrieve recipes with at least one matching ingredient
        var recipes = await _recipeRepository.GetMatchingRecipesAsync(userIngredients);

        // Calculate matching percentage and shape the data
        return recipes.Select(r =>
        {
            var totalIngredients = r.RecipeIngredients.Count;
            var matchingIngredients = r.RecipeIngredients.Count(ri => userIngredients.Contains(ri.Ingredient.Name.ToLower()));

            var matchingPercentage = totalIngredients > 0
                ? (double)matchingIngredients / totalIngredients * 100
                : 0;

            return new RecipeDto
            {
                Id = r.Id,
                Title = r.Title,
                MatchingPercentage = matchingPercentage,
                Ingredients = r.RecipeIngredients.Select(ri => ri.Ingredient.Name).ToList()
            };
        })
        .OrderByDescending(r => r.MatchingPercentage) // Sort by matching percentage
        .ToList();
    }
}