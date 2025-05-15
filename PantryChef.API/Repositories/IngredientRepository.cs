using Microsoft.EntityFrameworkCore;
using PantryChef.API.Data;
using PantryChef.API.Interfaces;
using PantryChef.API.Models;

namespace PantryChef.API.Repositories;

// Repository for accessing Ingredient data
public class IngredientRepository : IIngredientRepository
{
    private readonly PantryChefDbContext _dbContext;

    public IngredientRepository(PantryChefDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Fetch all ingredients from the database
    public async Task<IEnumerable<Ingredient>> GetAllIngredientsAsync()
    {
        return await _dbContext.Ingredients.ToListAsync();
    }

    // Fetch a specific ingredient by name
    public async Task<Ingredient?> GetIngredientByNameAsync(string name)
    {
        return await _dbContext.Ingredients.FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());
    }

    // Add a new ingredient to the database
    public async Task AddIngredientAsync(Ingredient ingredient)
    {
        await _dbContext.Ingredients.AddAsync(ingredient);
    }

    // Save changes to the database
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}