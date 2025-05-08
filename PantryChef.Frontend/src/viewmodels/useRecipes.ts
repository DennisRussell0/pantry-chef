import { useState } from "react";
import { fetchMatchingRecipes } from "../services/api";
import type { Recipe } from "../types/Recipe";

const useRecipes = () => {
  const [recipes, setRecipes] = useState<Recipe[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const getMatchingRecipes = async (selectedIngredients: string[]) => {
    setLoading(true);
    setError(null);
    try {
      const data = await fetchMatchingRecipes(selectedIngredients);
      setRecipes(data);
    } catch (err) {
      setError("Failed to fetch matching recipes. Please try again later.");
    } finally {
      setLoading(false);
    }
  };

  return { recipes, loading, error, getMatchingRecipes };
};

export default useRecipes;