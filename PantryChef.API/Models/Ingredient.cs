namespace PantryChef.API.Models;

// Represents an ingredient in the system
public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
