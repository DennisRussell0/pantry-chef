import { useRef, useState } from "react";
import useIngredients from "../viewmodels/useIngredients"; // Custom hook for fetching ingredients
import { useSelectedIngredients } from "../context/SelectedIngredientsContext"; // Context for managing selected ingredients

const IngredientSelector: React.FC = () => {
  const { ingredients, error, loading } = useIngredients(); // Fetch ingredients and related state
  const { selectedIngredients, addIngredient, removeIngredient, clearIngredients } = useSelectedIngredients(); // Manage selected ingredients
  const [searchQuery, setSearchQuery] = useState<string>(""); // State for search input
  const inputRef = useRef<HTMLInputElement>(null); // Reference to the search input field

  // Filter ingredients based on search input
  const filteredIngredients = ingredients.filter((ingredient) =>
    ingredient.name.toLowerCase().includes(searchQuery.toLowerCase())
  );

  // Toggle ingredient selection (add or remove)
  const toggleIngredient = (ingredientName: string) => {
    if (selectedIngredients.includes(ingredientName)) {
      removeIngredient(ingredientName); // Remove ingredient if already selected
    } else {
      addIngredient(ingredientName); // Add ingredient if not selected
    }

    // Clear and refocus the search input after clicking
    setSearchQuery("");
    setTimeout(() => inputRef.current?.focus(), 0);
  };

  if (loading) return <p className="text-[#a0a0a0] italic">Loading ingredients...</p>;
  if (error) return <p>{error}</p>;

  return (
    <div className="px-9 flex flex-col gap-9 items-center text-center">
      <h2 className="text-4xl font-bold text-[#FE5F55]">Search & Select Ingredients</h2>

      {/* Search input with reset (×) button */}
      <div className="relative w-fit text-lg">
        <input
          ref={inputRef}
          type="text"
          placeholder="Search Ingredients..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)} // Update search query
          name="search"
          className="rounded-md p-2 border border-gray-300 dark:border-white/30 bg-gray-50 dark:bg-inherit focus:bg-white dark:focus:bg-white/5 focus:outline-none focus:border-[#FE5F55] text-[#2b2b2b] dark:text-[#ffffff] hover:border-[#FE5F55] transition-all duration-300 ease-in-out"
        />
        {searchQuery && (
          <button
            onClick={() => {
              setSearchQuery(""); // Clear search query
              inputRef.current?.focus(); // Refocus input field
            }}
            className="absolute right-2 top-1/2 transform -translate-y-1/2 text-[#a0a0a0] hover:text-[#FE5F55] transition-all duration-300 ease-in-out"
            aria-label="Clear search"
          >
            ×
          </button>
        )}
      </div>

      {/* Ingredient buttons (shown only when searching) */}
      {searchQuery && (
        <div className="flex flex-wrap gap-3 text-md justify-center">
          {filteredIngredients.map((ingredient) => (
            <button
              key={ingredient.id}
              onClick={() => toggleIngredient(ingredient.name)} // Toggle ingredient selection
              className={`rounded-full capitalize px-4 py-1 border dark:text-white hover:border-[#FE5F55] transition-all duration-300 ease-in-out ${
                selectedIngredients.includes(ingredient.name)
                  ? "bg-[#FE5F55] border-transparent text-white hover:bg-white hover:text-[#2b2b2b] dark:hover:bg-white/5 dark:hover:text-white transition-all duration-300 ease-in-out"
                  : "dark:bg-white/5 text-[#2b2b2b] border-gray-300 dark:border-white/10"
              }`}
            >
              {ingredient.name} {selectedIngredients.includes(ingredient.name)}
            </button>
          ))}
        </div>
      )}

      {/* Selected ingredients */}
      <div className="flex flex-col gap-9 pt-6 w-full">
        <h3 className="text-3xl text-[#FE5F55]">Selected Ingredients</h3>
        {selectedIngredients.length === 0 ? (
          <p className="text-[#a0a0a0] italic">No ingredients selected</p>
        ) : (
          <ul className="capitalize flex gap-3 justify-center flex-wrap">
            {selectedIngredients.map((ingredient) => (
              <li 
                key={ingredient}
                className="flex gap-2 items-center rounded-full capitalize px-3 pl-4 py-1 border border-gray-300 dark:border-white/10 dark:bg-white/5"
              >
                <span>{ingredient}</span>
                <button
                  onClick={() => removeIngredient(ingredient)} // Remove ingredient
                  aria-label={`Remove ${ingredient}`}
                  className="font-bold text-[#FE5F55] text-xl translate-y-[-1px]"
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
            onClick={clearIngredients} // Clear all selected ingredients
            className="mt-6 w-fit self-center text-lg rounded-full capitalize px-8 py-2 border text-[#2b2b2b] dark:text-white dark:bg-white/5 border-gray-300 dark:border-white/10 hover:border-[#FE5F55] transition-all duration-300 ease-in-out"
          >
            Reset
          </button>
        )}
      </div>
    </div>
  );
};

export default IngredientSelector;
