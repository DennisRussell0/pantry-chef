using Microsoft.EntityFrameworkCore;
using PantryChef.API.Data;
using PantryChef.API.Interfaces;
using PantryChef.API.Models;

namespace PantryChef.API.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly PantryChefDbContext _dbContext;

        public RecipeRepository(PantryChefDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
        {
            return await _dbContext.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();
        }

        public async Task<Recipe?> GetRecipeByIdAsync(int id)
        {
            return await _dbContext.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

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

        public async Task AddRecipeAsync(Recipe recipe)
        {
            await _dbContext.Recipes.AddAsync(recipe);
        }

        public async Task DeleteRecipeAsync(int id)
        {
            var recipe = await GetRecipeByIdAsync(id);
            if (recipe != null)
            {
                _dbContext.Recipes.Remove(recipe);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}