namespace PantryChef.API.Models;

// Represents a recipe in the system
public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string ImageName { get; set; } = string.Empty;

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
