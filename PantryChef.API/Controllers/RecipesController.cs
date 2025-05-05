using Microsoft.AspNetCore.Mvc;
using PantryChef.API.DTOs;
using PantryChef.API.Services;

namespace PantryChef.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly RecipeService _recipeService;

    public RecipesController(RecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    // GET: api/recipes/match
    [HttpGet("match")]
    public async Task<ActionResult<IEnumerable<RecipeDto>>> GetMatchingRecipes(
        [FromQuery] string ingredients, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        var userIngredients = ingredients.Split(',').Select(i => i.Trim().ToLower()).ToHashSet();

        var matchingRecipes = await _recipeService.GetMatchingRecipesAsync(userIngredients);

        // Apply pagination
        var paginatedRecipes = matchingRecipes
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(paginatedRecipes);
    }
}