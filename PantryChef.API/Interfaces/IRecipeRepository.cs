using PantryChef.API.Models;

namespace PantryChef.API.Interfaces
{
    public interface IRecipeRepository
    {
        Task<IEnumerable<Recipe>> GetMatchingRecipesAsync(HashSet<string> userIngredients);
    }
}