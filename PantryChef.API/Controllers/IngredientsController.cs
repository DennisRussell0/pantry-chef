using Microsoft.AspNetCore.Mvc;
using PantryChef.API.Interfaces;
using PantryChef.API.Models;

namespace PantryChef.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientsController : ControllerBase
{
    private readonly IIngredientRepository _ingredientRepository;

    // Constructor to inject the ingredient repository
    public IngredientsController(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    // GET: api/ingredients
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ingredient>>> GetIngredients()
    {
        // Retrieve all ingredients from the repository
        var ingredients = await _ingredientRepository.GetAllIngredientsAsync();
        return Ok(ingredients);
    }
}