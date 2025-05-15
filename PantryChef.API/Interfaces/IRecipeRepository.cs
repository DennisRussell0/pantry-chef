using PantryChef.API.Models;

namespace PantryChef.API.Interfaces;

// Interface defining the contract for the Recipe repository
public interface IRecipeRepository
{
    // Fetch recipes that match the user's selected ingredients
    Task<IEnumerable<Recipe>> GetMatchingRecipesAsync(HashSet<string> userIngredients);
}