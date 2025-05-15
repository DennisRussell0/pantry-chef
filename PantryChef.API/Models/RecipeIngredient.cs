namespace PantryChef.API.Models;

// Represents the many-to-many relationship between Recipe and Ingredient
public class RecipeIngredient
{
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public string OriginalText { get; set; } = string.Empty;
}
