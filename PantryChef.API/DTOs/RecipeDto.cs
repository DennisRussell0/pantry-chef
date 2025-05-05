namespace PantryChef.API.DTOs;

public class RecipeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public double MatchingPercentage { get; set; }
    public List<string> Ingredients { get; set; } = new List<string>();
}