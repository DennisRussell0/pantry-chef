import { useState } from "react";
import type { Recipe } from "../types/Recipe";
import RecipeModal from "./RecipeModal";

const RecipeList: React.FC<{ recipes: Recipe[] }> = ({ recipes }) => {
  const [selectedRecipe, setSelectedRecipe] = useState<Recipe | null>(null);

  const handleRecipeClick = (recipe: Recipe) => {
    setSelectedRecipe(recipe);
  };

  const closeModal = () => {
    setSelectedRecipe(null);
  };

  if (recipes.length === 0) {
    return <p className="text-[#a0a0a0] italic">No matching recipes found</p>;
  }

  return (
    <div className="w-full px-9 py-15 flex flex-col gap-9 bg-gray-100 dark:bg-inherit border-t border-transparent dark:border-white/10 items-center">
      <h2 className="text-5xl font-bold text-[#FE5F55] text-center">Matching Recipes</h2>
      <ul className="py-9 grid grid-cols-1 gap-9 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 2xl:grid-cols-5 m-auto">
        {recipes.map((recipe) => (
          <li 
            key={recipe.id}
            className="flex flex-col gap-6 p-6 border rounded-md border-transparent max-w-min cursor-pointer bg-white dark:bg-white/5 hover:border-[#FE5F55] transition-all duration-300 ease-in-out"
            onClick={() => handleRecipeClick(recipe)}
          >
            {recipe.imageName && (
              <img
                src={`/images/${recipe.imageName}.jpg`}
                alt={recipe.title}
                className="rounded-md max-w-68"
              />
            )}
            <div className="flex flex-col gap-9 h-full justify-between min-h-">
              <h3 className="text-2xl font-bold">{recipe.title}</h3>
              <p className="text-[#FE5F55] self-end text-sm">{recipe.matchingPercentage.toFixed(0)}% match</p>
            </div>
          </li>
        ))}
      </ul>
      {selectedRecipe && (
        <RecipeModal recipe={selectedRecipe} onClose={closeModal} />
      )}
    </div>
  );
};

export default RecipeList;