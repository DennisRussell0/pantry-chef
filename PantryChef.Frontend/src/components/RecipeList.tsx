import type { Recipe } from "../types/Recipe";

const RecipeList: React.FC<{ recipes: Recipe[] }> = ({ recipes }) => {
  if (recipes.length === 0) {
    return <p>No matching recipes found.</p>;
  }

  return (
    <div>
      <h2>Matching Recipes:</h2>
      <ul>
        {recipes.map((recipe) => (
          <li key={recipe.id}>
            <h3>{recipe.title}</h3>
            <p>Matching Percentage: {recipe.matchingPercentage.toFixed(2)}%</p>
            <p>Ingredients: {recipe.ingredients.join(", ")}</p>
            {/*<p>Instructions: {recipe.instructions}</p>*/}
            {recipe.imageName && (
              <img
                src={`/images/${recipe.imageName}.jpg`}
                alt={recipe.title}
              />
            )}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default RecipeList;