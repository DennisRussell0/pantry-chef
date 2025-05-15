using PantryChef.API.Models;

namespace PantryChef.API.Interfaces;

// Interface defining the contract for the Ingredient repository
public interface IIngredientRepository
{
    Task<IEnumerable<Ingredient>> GetAllIngredientsAsync(); // Fetch all ingredients
    Task<Ingredient?> GetIngredientByNameAsync(string name); // Fetch a specific ingredient by name
    Task AddIngredientAsync(Ingredient ingredient); // Add a new ingredient to the database
    Task SaveChangesAsync(); // Save changes to the database
}