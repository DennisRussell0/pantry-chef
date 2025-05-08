import { useRef, useState } from "react";
import useIngredients from "../viewmodels/useIngredients";
import { useSelectedIngredients } from "../context/SelectedIngredientsContext";

const IngredientSelector: React.FC = () => {
  const { ingredients, error, loading } = useIngredients();
  const { selectedIngredients, addIngredient, removeIngredient, clearIngredients } = useSelectedIngredients();
  const [searchQuery, setSearchQuery] = useState<string>("");
  const inputRef = useRef<HTMLInputElement>(null);

  // Filter ingredients based on search input
  const filteredIngredients = ingredients.filter((ingredient) =>
    ingredient.name.toLowerCase().includes(searchQuery.toLowerCase())
  );

  // Toggle ingredient selection
  const toggleIngredient = (ingredientName: string) => {
    if (selectedIngredients.includes(ingredientName)) {
      removeIngredient(ingredientName);
    } else {
      addIngredient(ingredientName);
    }

    // Clear and refocus the search input after clicking
    setSearchQuery("");
    setTimeout(() => inputRef.current?.focus(), 0);
  };

  if (loading) return <p>Loading ingredients...</p>;
  if (error) return <p>{error}</p>;

  return (
    <div className="flex flex-col gap-1 p-1">
      <h2>Search and Select Ingredients:</h2>

      {/* Search input with reset (×) button */}
      <div className="relative w-fit">
        <input
          ref={inputRef}
          type="text"
          placeholder="Search Ingredients..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          name="search"
          className="p-2 border border-gray-500 outline-none"
        />
        {searchQuery && (
          <button
            onClick={() => {
              setSearchQuery("");
              inputRef.current?.focus();
            }}
            className="absolute right-2 top-1/2 transform -translate-y-1/2 text-gray-500"
            aria-label="Clear search"
          >
            ×
          </button>
        )}
      </div>

      {/* Ingredient buttons (shown only when searching) */}
      {searchQuery && (
        <div className="flex flex-wrap gap-1">
          {filteredIngredients.map((ingredient) => (
            <button
              key={ingredient.id}
              onClick={() => toggleIngredient(ingredient.name)}
              className={`cursor-pointer capitalize px-2 py-1 border text-sm ${
                selectedIngredients.includes(ingredient.name)
                  ? "bg-gray-400 text-white"
                  : "bg-white text-black border-gray-500"
              }`}
            >
              {ingredient.name} {selectedIngredients.includes(ingredient.name)}
            </button>
          ))}
        </div>
      )}

      {/* Selected ingredients */}
      <div className="flex flex-col gap-1">
        <h3>Selected Ingredients:</h3>
        {selectedIngredients.length === 0 ? (
          <p>No ingredients selected</p>
        ) : (
          <ul className="capitalize">
            {selectedIngredients.map((ingredient) => (
              <li 
                key={ingredient}
                className="flex gap-1"
              >
                <span>{ingredient}</span>
                <button
                  onClick={() => removeIngredient(ingredient)}
                  aria-label={`Remove ${ingredient}`}
                  className="cursor-pointer"
                >
                  ×
                </button>
              </li>
            ))}
          </ul>
        )}

        {/* Clear all button */}
        {selectedIngredients.length > 0 && (
          <button
            onClick={clearIngredients}
            className="cursor-pointer self-start"
          >
            Reset
          </button>
        )}
      </div>
    </div>
  );
};

export default IngredientSelector;
