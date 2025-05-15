using System.Text.RegularExpressions;
// NEW START
using System.Text.Json;
using System.Threading.Tasks;
// NEW END

namespace PantryChef.API.Services;

// Service for parsing raw ingredient strings into core ingredients
public class IngredientParser
{
    // NEW START
    private readonly IngredientMicroserviceClient? _microserviceClient;
    private const double SimilarityThreshold = 0.75; // Adjust as needed

    public IngredientParser() { }

    public IngredientParser(IngredientMicroserviceClient microserviceClient)
    {
        _microserviceClient = microserviceClient;
    }
    // NEW END
    
    // Set of common stop words (units, descriptors, etc.) to exclude during parsing
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        // Measurement units and preparation terms
        "cup", "cups", "tablespoon", "tablespoons", "tbsp", "tbsp.",
        "teaspoon", "teaspoons", "tsp", "tsp.", "pound", "pounds", "ounce",
        "ounces", "gram", "grams", "lb", "kilogram", "kilograms", "pinch", "dash",
        "slice", "slices", "piece", "pieces", "finely", "coarsely", "roughly",
        "chopped", "sliced", "diced", "minced", "thinly", "lightly", "beaten",
        "halved", "lengthwise", "seeded", "unpeeled", "boneless", "unripe",
        "shaved", "peeled", "crushed", "grated", "julienne", "batons",

        // Miscellaneous descriptors
        "room", "temperature", "melted", "divided", "plus", "more", "about", "such", "as", "to", "taste",
        "optional", "small", "medium", "large", "needed", "not", "peeled", "preferably", "to taste",
        "homemade", "as", "needed", "maldon", "persian", "with", "seeds", "like", "amy’s",

        // Preparation and serving terms
        "for", "serving", "to", "serve", "brushing", "dusting", "finishing", "into",
        "cut", "off", "crosswise", "wedges", "cubes", "dice", "thin",
        "strips", "chunks", "on", "bias", "a", "diagonal", "deep", "diagonal", "crosswise",
        "diagonal", "through", "stem", "until", "trimmed", "tops",

        // Miscellaneous filler words
        "patted", "dry", "kernels", "removed", "strings", "removed", "if", "and", "or", "the",
        "below", "quartered", "leaves", "separated", "tip", "skinless", "parts", "only", "of", "runs",
        "clear", "to", "serve", "a", "your", "from", "with", "in",
        "bought", "store bought", "homemade", "new", "skin", "shelling", "packed", "rind",
        "legs", "stems", "tender", "stems", "tender", "herbs", "root", "end", "root", "ends", "pale", "green",
        "separately", "seasoning", "graham", "crackers", "knife", "food", "mill", "oil-packed",
        "paste", "your cider", "your", "bowl", "bite", "size", "needed", "cheesecloth",
        "beefeater", "button",
    };

    // Set of common ingredient keywords for recognition
    private static readonly HashSet<string> CommonIngredients = new(StringComparer.OrdinalIgnoreCase)
    {
        "black pepper", "feta", "buttermilk", "olive oil", "vegetable oil", "flour", "sugar", "cream",
        "eggs", "bread", "onion", "garlic", "broth", "butternut squash", "whole chicken",
        "paprika", "cinnamon", "nutmeg", "allspice", "cayenne", "coriander", "tomatoes", "pickles",
        "parsley", "rosemary", "thyme", "basil", "oregano", "sage", "mint", "tarragon", "rib eye",
        "ground beef", "pork", "turkey", "ham", "lamb", "salmon", "cod", "shrimp", "sweet potatoes",
        "potatoes", "carrot", "celery", "tomato", "spinach", "cucumber", "avocado", "gin", "duck fat",
        "red-wine vinegar", "rice vinegar", "red wine vinegar", "balsamic vinegar", "strip steak", "duck",
        "lentils", "beans", "rice", "quinoa", "barley", "oats", "almonds", "cashews", "potato", "white wine vinegar",
        "miso", "mayonnaise", "honey", "maple syrup", "vanilla extract", "chocolate chips", "jalapeno",
        "lemon", "vermouth", "water", "ice", "green olives", "green olive", "black olives", "shishito peppers",
        "beef rib roast", "beef tenderloin", "beef bones", "bacon", "buns", "mustard", "lettuce", "white vinegar",
        "beef tallow", "egg", "beef dripping", "salsa", "tonic", "club soda", "sparkling water", "seltzer", 
        "cola", "ginger ale", "ginger beer", "lemon juice", "lime juice", "orange juice", "cranberry juice", 
        "pineapple juice", "grenadine", "bitters", "simple syrup", "agave syrup", "triple sec", "cointreau", 
        "curaçao", "whiskey", "bourbon", "rye", "rum", "white rum", "dark rum", "tequila", "mezcal", "brandy", 
        "cognac", "prosecco", "champagne", "beer", "stout", "lager", "red wine", "white wine", "rosé wine",    
        "port", "sherry", "amaretto", "absinthe", "sambuca", "liqueur", "coffee liqueur", "irish cream", 
        "peach schnapps", "baking powder", "baking soda", "yeast", "cornstarch", "corn starch", "cocoa powder", 
        "chocolate", "vanilla", "vanilla bean", "brown sugar", "powdered sugar", "confectioners sugar", 
        "molasses", "peanut butter", "jam", "jelly", "preserves", "ketchup", "soy sauce", "fish sauce", 
        "oyster sauce", "hoisin sauce", "sriracha", "hot sauce", "chili flakes", "chili powder", "rice wine",
        "curry powder", "turmeric", "ginger", "scallion", "green onion", "shallot", "zucchini", 
        "eggplant", "bell pepper", "red pepper", "yellow pepper", "mushrooms", "kale", "arugula", "cabbage", 
        "broccoli", "cauliflower", "peas", "corn", "apple", "banana", "strawberry", "blueberry", "raspberry", 
        "grape", "pear", "peach", "plum", "apricot", "coconut", "almond milk", "soy milk", "oat milk", "coconut milk",
        "peppers", "pepper", "chicken thighs", "chicken", "white-wine vinegar", "apple cider vinegar",
        "champagne vinegar", "cider vinegar", "vinegar", "leeks", "leek", "cantaloupe", "beets", "butter", "milk",
        "horseradish", "radishes", "salt", "parmesan", "mozzarella", "cheddar", "cream cheese", "ricotta",
        "goat cheese", "blue cheese", "yogurt", "cottage cheese", "cheese",
    };

    // NEW START
    public async Task<string> ExtractCoreIngredientSmartAsync(string ingredient)
    {
        if (_microserviceClient != null)
        {
            var (coreIngredient, similarity) = await _microserviceClient.GetCoreIngredientWithScoreAsync(ingredient);
            if (similarity >= SimilarityThreshold && !string.IsNullOrWhiteSpace(coreIngredient))
                return coreIngredient;
        }
        // Fallback to original logic
        return ExtractCoreIngredient(ingredient);
    }
    // NEW END

    // Extracts the core ingredient from a given raw ingredient string
    public string ExtractCoreIngredient(string ingredient)
    {
        if (string.IsNullOrWhiteSpace(ingredient))
            return string.Empty;

        // 1. Remove quantities, fractions, and measurements
        ingredient = Regex.Replace(ingredient, @"\b\d+(\.\d+)?\s*[\w¼½¾⅓⅔⅛⅜⅝⅞]*\b", "", RegexOptions.IgnoreCase);

        // 2. Remove content inside parentheses
        ingredient = Regex.Replace(ingredient, @"\([^)]*\)", "");

        // 3. Remove symbols and punctuation
        ingredient = Regex.Replace(ingredient, @"[""\[\]\.,/\\:;!?\*']", " ");
        ingredient = Regex.Replace(ingredient, @"\s{2,}", " ").Trim();

        // 4. Tokenize and filter out stop words
        var words = ingredient
            .Split(new[] { ' ', ',', '-', '(', ')' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(w => !StopWords.Contains(w, StringComparer.OrdinalIgnoreCase) && Regex.IsMatch(w, @"[a-zA-Z]"))
            .ToList();

        if (words.Count == 0)
            return string.Empty;

        // 5. Attempt full string match against common ingredients
        var joined = string.Join(" ", words).ToLowerInvariant();
        var match = CommonIngredients.FirstOrDefault(ci => joined.Contains(ci.ToLowerInvariant()));
        if (match != null) return match;

        // 6. Fallback: try matching individual words
        var originalWords = ingredient.ToLowerInvariant().Split(new[] { ' ', ',', '-', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
        var wordMatch = CommonIngredients.FirstOrDefault(ci => originalWords.Contains(ci.ToLowerInvariant()));
        if (wordMatch != null) return wordMatch;

        // 7. Final fallback: return the last 1–2 words as best guess
        return string.Join(" ", words.TakeLast(Math.Min(2, words.Count))).ToLowerInvariant();
    }
}