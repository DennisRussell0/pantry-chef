import IngredientSelector from "./components/IngredientSelector";
import RecipeList from "./components/RecipeList";
import { useSelectedIngredients } from "./context/SelectedIngredientsContext";
import useRecipes from "./viewmodels/useRecipes";

const App: React.FC = () => {
  const { selectedIngredients } = useSelectedIngredients();
  const { recipes, loading, error, getMatchingRecipes } = useRecipes();

  const handleGetRecipes = () => {
    if (selectedIngredients.length > 0) {
      getMatchingRecipes(selectedIngredients);
    }
  };

  return (
    <div>
      <h1 className="p-1">Pantry Chef</h1>
      <IngredientSelector />
      <button 
        onClick={handleGetRecipes} 
        disabled={selectedIngredients.length === 0}
        className="cursor-pointer"
      >
        Get Matching Recipes
      </button>
      {loading && <p>Loading recipes...</p>}
      {error && <p>{error}</p>}
      <RecipeList recipes={recipes} />
    </div>
  );
};

export default App;