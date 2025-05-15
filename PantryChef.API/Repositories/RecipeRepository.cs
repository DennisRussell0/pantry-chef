using Microsoft.EntityFrameworkCore;
using PantryChef.API.Data;
using PantryChef.API.Interfaces;
using PantryChef.API.Models;

namespace PantryChef.API.Repositories;

// Repository for accessing Recipe data
public class RecipeRepository : IRecipeRepository
{
    private readonly PantryChefDbContext _dbContext;

    public RecipeRepository(PantryChefDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Fetch recipes that match the user's selected ingredients
    public async Task<IEnumerable<Recipe>> GetMatchingRecipesAsync(HashSet<string> userIngredients)
    {
        // Convert userIngredients to lowercase for case-insensitive matching
        var lowerCaseIngredients = userIngredients.Select(i => i.ToLower()).ToHashSet();

        // Query the database for recipes with at least one matching ingredient
        return await _dbContext.Recipes
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .Where(r => r.RecipeIngredients.Any(ri => lowerCaseIngredients.Contains(ri.Ingredient.Name.ToLower())))
            .ToListAsync();
    }
}