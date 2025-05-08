using System.Text.RegularExpressions;

namespace PantryChef.API.Services;

public class IngredientParser
{
    // Set of common stop words (units, descriptors, etc.) to be excluded from ingredient parsing
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        // Mål og tilberedningsord
        "cup", "cups", "tablespoon", "tablespoons", "tbsp", "tbsp.",
        "teaspoon", "teaspoons", "tsp", "tsp.", "pound", "pounds", "ounce",
        "ounces", "gram", "grams", "kilogram", "kilograms", "pinch", "dash",
        "slice", "slices", "piece", "pieces", "finely", "coarsely", "roughly",
        "chopped", "sliced", "diced", "minced", "thinly", "lightly", "beaten",
        "halved", "lengthwise", "seeded", "unpeeled", "boneless", "unripe",
        "shaved", "peeled", "crushed", "grated", "julienne", "batons",
        
        // Tilstand og præcision
        "room", "temperature", "melted", "divided", "plus", "more", "about", "such", "as", "to", "taste",
        "optional", "small", "medium", "large", "needed", "not", "peeled", "preferably",
        "homemade", "as", "needed", "maldon", "persian", "with", "seeds", "like", "amy’s",
        
        // Retnings- og forberedelsesord
        "for", "serving", "to", "serve", "brushing", "dusting", "finishing", "into",
        "cut", "off", "crosswise", "wedges", "cubes", "dice", "thin",
        "strips", "chunks", "on", "bias", "a", "diagonal", "deep", "diagonal", "crosswise",
        "diagonal", "through", "stem", "until", "water", "trimmed", "tops",
        
        // Diverse ubrugelige ord
        "patted", "dry", "kernels", "removed", "strings", "removed", "if", "and", "or", "the",
        "below", "quartered", "leaves", "separated", "tip", "skinless", "parts", "only",
        "juice", "of", "runs", "clear", "to", "serve", "a", "your", "from", "with", "in",
        "bought", "store bought", "homemade", "new", "skin", "shelling", "packed", "rind",
        "legs", "stems", "tender", "stems", "tender", "herbs", "root", "end", "root", "ends", "pale", "green",
        "separately", "seasoning", "graham", "crackers", "knife", "food", "mill",
        "paste", "your cider", "your", "bowl", "bite", "size", "needed"
    };

    // Set of common ingredient keywords for recognition
    private static readonly HashSet<string> CommonIngredients = new(StringComparer.OrdinalIgnoreCase)
    {
        // Grundingredienser
        "salt", "pepper", "butter", "oil", "olive oil", "flour", "sugar", "milk", "cream",
        "egg", "cheese", "bread", "onion", "garlic", "vinegar", "wine", "broth",
        
        // Krydderier og smagsgivere
        "paprika", "cinnamon", "nutmeg", "clove", "allspice", "cayenne", "coriander",
        "coriander seeds", "cumin", "cumin seeds", "turmeric", "mustard", "mustard seeds",
        "ginger", "garam masala", "adobo sauce", "cardamom", "curry powder", "dill",
        "tahini", "sauce tamari", "gochujang", "doenjang", "soy sauce", "fish sauce",
        
        // Urter
        "parsley", "rosemary", "thyme", "basil", "oregano", "sage", "mint", "tarragon",
        "chives", "cilantro", "beni cilantro", "wild rocket", "green lettuce", "swiss chard",
        "marjoram", "scallions", "leeks", "shallots", "fennel bulbs", "gem lettuce",
        "trimmed watercress",
        
        // Kød, fisk og æg
        "chicken", "beef", "pork", "turkey", "ham", "lamb", "salmon", "cod", "snapper",
        "halibut", "shrimp", "chorizo", "bacon", "brisket", "stew meat", "eggs",
        
        // Grøntsager og frugt
        "potatoes", "sweet potatoes", "idaho potatoes", "gold potatoes", "carrot", "celery",
        "parsnips", "squash", "zucchini", "tomato", "beets", "spinach", "cucumbers",
        "avocados", "green beans", "green cabbage", "broccolini", "mangoes", "apples",
        "lemon", "lime", "orange", "grapes", "pineapple", "papaya", "berries", "chiles",
        "thai chiles", "red chiles", "green chiles", "serrano chile", "jalapeño", "habanero",
        
        // Bælgfrugter, korn, nødder
        "lentils", "red lentils", "chana dal", "moong dal", "masoor dal", "urad dal",
        "pinto beans", "black beans", "chickpeas", "peas", "lima beans", "shelling beans",
        "white rice", "sushi rice", "grain rice", "farro", "quinoa", "freekeh", "barley",
        "oats", "pumpkin seeds", "sesame seeds", "almonds", "cashews", "walnuts",
        "pecans", "hazelnuts", "pine nuts", "cocoa powder", "chocolate chips",
        
        // Andre
        "miso", "mayonnaise", "honey", "maple syrup", "vanilla extract", "apple cider",
        "apple", "raisins", "currants", "dates", "apricots", "tortillas", "macaroni",
        "loaf", "bread", "baking powder", "baking soda", "yogurt", "manchego", "parmesan",
        "cheddar", "fashioned oats", "tea", "rum", "bourbon", "scotch", "mezcal", "gin",
        "amaro averna", "grand marnier", "cynar", "fino sherry", "amontillado sherry",
        "rose harissa", "dashi", "grain mustard", "anchovy", "anchovy fillet"
    };

    // Extracts the core ingredient from a given raw ingredient string
    public string ExtractCoreIngredient(string ingredient)
    {
        if (string.IsNullOrWhiteSpace(ingredient))
            return string.Empty;

        // 1. Remove quantities, fractions and measurements (e.g., "1 1/2", "¾", "2 tbsp")
        ingredient = Regex.Replace(ingredient, @"\b\d+(\.\d+)?\s*[\w¼½¾⅓⅔⅛⅜⅝⅞]*\b", "", RegexOptions.IgnoreCase);

        // 2. Remove content inside parentheses
        ingredient = Regex.Replace(ingredient, @"\([^)]*\)", "");

        // 3. Remove symbols and punctuation
        ingredient = Regex.Replace(ingredient, @"[""\[\]\.,/\\:;!?']", " ");
        ingredient = Regex.Replace(ingredient, @"\s{2,}", " ").Trim();

        // 4. Tokenize and filter out stop words and non-letter entries
        var words = ingredient
            .Split(new[] { ' ', ',', '-', '(', ')' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(w =>
                !StopWords.Contains(w, StringComparer.OrdinalIgnoreCase) &&
                Regex.IsMatch(w, @"[a-zA-Z]") // must contain letters
            )
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