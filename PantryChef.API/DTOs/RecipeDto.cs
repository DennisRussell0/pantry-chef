namespace PantryChef.API.DTOs;

// Data Transfer Object for a Recipe
public class RecipeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public double MatchingPercentage { get; set; }
    public List<string> Ingredients { get; set; } = new List<string>();
    public List<string> OriginalText { get; set; } = new List<string>();
    public string Instructions { get; set; } = string.Empty;
    public string ImageName { get; set; } = string.Empty;
}