import IngredientSelector from "./components/IngredientSelector";
import RecipeList from "./components/RecipeList";
import { useSelectedIngredients } from "./context/SelectedIngredientsContext"; // Context for managing selected ingredients
import useRecipes from "./viewmodels/useRecipes"; // Custom hook for fetching and managing recipes

const App: React.FC = () => {
  const { selectedIngredients } = useSelectedIngredients(); // Access the list of selected ingredients
  const { recipes, loading, error, getMatchingRecipes } = useRecipes(); // Access recipes and related state

  // Function to fetch matching recipes based on selected ingredients
  const handleGetRecipes = () => {
    if (selectedIngredients.length > 0) {
      getMatchingRecipes(selectedIngredients); // Fetch recipes if ingredients are selected
    }
  };

  return (
    <div className="pt-3 tracking-wide flex flex-col gap-6">
      {/* Header */}
      <header className="px-9 border-b border-gray-200 dark:border-b-white/10 pb-3 flex">
        <h1 className="text-2xl text-[#FE5F55] font-black">Pantry Chef</h1>
      </header>

      {/* Main Content */}
      <div className="flex flex-col gap-18 pt-9 items-center w-full">
        <IngredientSelector /> {/* Component for ingredient selection */}
        
        {/* Button to fetch matching recipes */}
        <button 
          onClick={handleGetRecipes} 
          disabled={selectedIngredients.length === 0} // Disable button if no ingredients are selected
          className="self-center mx-9 w-fit text-xl rounded-full capitalize px-16 py-4 border bg-[#FE5F55] text-white border-transparent hover:scale-104 transition-all duration-300 ease-in-out"
        >
          Get Matching Recipes
        </button>

        {/* Loading and error messages */}
        {loading && <p className="text-[#a0a0a0] italic">Loading recipes...</p>}
        {error && <p>{error}</p>}

        {/* Recipe list */}
        <RecipeList recipes={recipes} />
      </div>
    </div>
  );
};

export default App;