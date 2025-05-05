using PantryChef.API.Models;

namespace PantryChef.API.Interfaces
{
    public interface IRecipeRepository
    {
        Task<IEnumerable<Recipe>> GetAllRecipesAsync();
        Task<Recipe?> GetRecipeByIdAsync(int id);
        Task<IEnumerable<Recipe>> GetMatchingRecipesAsync(HashSet<string> userIngredients);
        Task AddRecipeAsync(Recipe recipe);
        Task DeleteRecipeAsync(int id);
        Task SaveChangesAsync();
    }
}