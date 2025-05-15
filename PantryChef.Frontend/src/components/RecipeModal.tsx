import type { Recipe } from "../types/Recipe";

interface RecipeModalProps {
  recipe: Recipe | null;
  onClose: () => void;
}

const RecipeModal: React.FC<RecipeModalProps> = ({ recipe, onClose }) => {
  if (!recipe) return null;

  return (
    <div
      className="fixed inset-0 bg-black/50 flex justify-center items-center z-50"
      onClick={onClose}
    >
      <div
        className="flex flex-col gap-6 bg-white dark:bg-[#1f1f1f] p-12 rounded-md max-w-[calc(100svw-70px)] md:w-2xl w-full relative max-h-[calc(100svh-70px)] overflow-y-auto"
        onClick={(e) => e.stopPropagation()}
      >
        <button
          className="absolute top-6 right-8 font-bold text-[#a0a0a0] text-2xl hover:text-[#FE5F55] transition-all duration-300 ease-in-out"
          onClick={onClose}
        >
          ×
        </button>
        <div className="flex flex-col items-center sm:items-left md:flex-row py-9 gap-6 border-b border-gray-300 dark:border-white/10 w-full">
            {recipe.imageName && (
                <img
                    src={`/images/${recipe.imageName}.jpg`}
                    alt={recipe.title}
                    className="rounded-md w-68 max-w-full h-auto object-contain"
                />
            )}
            <div className="flex flex-col gap-6 min-h-full justify-center w-full">
                <h2 className="text-4xl font-bold self-center md:self-start text-center md:text-start">{recipe.id} {recipe.title}</h2>
                <p className="text-[#FE5F55] text-lg">
                {recipe.matchingPercentage.toFixed(0)}% match
                </p>
            </div>
        </div>
        <h3 className="text-2xl font-bold text-[#FE5F55]">Ingredients Text</h3>
        <ul className="list-disc list-outside normal-case leading-7 ml-5">
          {Array.isArray(recipe.originalText) && recipe.originalText.length > 0 ? (
            recipe.originalText.map((txt, index) => (
              <li 
                key={index}
                className="px-1"
              >
                <span key={index}>
                  {txt}
                </span>
              </li>
            ))
          ) : (
            <li>No ingredients available</li>
          )}
        </ul>
        <h3 className="text-2xl font-bold text-[#FE5F55]">Ingredients</h3>
        <ul className="list-disc list-outside capitalize leading-7 ml-5">
          {Array.isArray(recipe.ingredients) && recipe.ingredients.length > 0 ? (
            recipe.ingredients.map((ingredient, index) => (
              <li 
                key={index}
                className="px-1"
              >
                <span>
                  {ingredient}
                </span>
              </li>
            ))
          ) : (
            <li>No ingredients available</li>
          )}
        </ul>
        <h3 className="text-2xl font-bold text-[#FE5F55]">Instructions</h3>
        <p className="leading-8 text-left sm:text-justify">{recipe.instructions}</p>
      </div>
    </div>
  );
};

export default RecipeModal;