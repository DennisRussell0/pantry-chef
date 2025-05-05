using Microsoft.EntityFrameworkCore;
using PantryChef.API.Data;
using PantryChef.API.Interfaces;
using PantryChef.API.Models;

namespace PantryChef.API.Repositories
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly PantryChefDbContext _dbContext;

        public IngredientRepository(PantryChefDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Ingredient>> GetAllIngredientsAsync()
        {
            return await _dbContext.Ingredients.ToListAsync();
        }

        public async Task<Ingredient?> GetIngredientByNameAsync(string name)
        {
            return await _dbContext.Ingredients.FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());
        }

        public async Task AddIngredientAsync(Ingredient ingredient)
        {
            await _dbContext.Ingredients.AddAsync(ingredient);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}