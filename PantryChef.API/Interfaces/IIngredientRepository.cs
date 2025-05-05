using PantryChef.API.Models;

namespace PantryChef.API.Interfaces
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<Ingredient>> GetAllIngredientsAsync();
        Task<Ingredient?> GetIngredientByNameAsync(string name);
        Task AddIngredientAsync(Ingredient ingredient);
        Task SaveChangesAsync();
    }
}