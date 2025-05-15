import { useState } from "react";
import { fetchMatchingRecipes } from "../services/api"; // API function to fetch matching recipes
import type { Recipe } from "../types/Recipe"; // Type definition for a Recipe

// Custom hook for managing recipes and fetching matching recipes
const useRecipes = () => {
  const [recipes, setRecipes] = useState<Recipe[]>([]); // State to store the list of recipes
  const [loading, setLoading] = useState<boolean>(false); // State to track loading status
  const [error, setError] = useState<string | null>(null); // State to store error messages

  // Function to fetch matching recipes based on selected ingredients
  const getMatchingRecipes = async (selectedIngredients: string[]) => {
    setLoading(true); // Set loading to true while fetching
    setError(null); // Clear any previous errors
    try {
      const data = await fetchMatchingRecipes(selectedIngredients); // Fetch recipes from the API
      setRecipes(data); // Update the recipes state with the fetched data
    } catch (err) {
      setError("Failed to fetch matching recipes. Please try again later.");
    } finally {
      setLoading(false); // Set loading to false after fetching
    }
  };

  return { recipes, loading, error, getMatchingRecipes }; // Return state and function for use in components
};

export default useRecipes;