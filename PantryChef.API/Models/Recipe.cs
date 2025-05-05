namespace PantryChef.API.Models;

public class Recipe
{
    public int Id { get; set; }                                 // auto-increment in the database
    public string Title { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string ImageName { get; set; } = string.Empty;       // from Image_Name

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
