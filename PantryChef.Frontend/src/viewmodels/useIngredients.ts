import { useEffect, useState } from "react";
import { fetchIngredients } from "../services/api"; // API function to fetch ingredients
import type { Ingredient } from "../types/Ingredient"; // Type definition for an Ingredient

// Custom hook for managing ingredients and fetching them from the API
const useIngredients = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]); // State to store the list of ingredients
  const [error, setError] = useState<string | null>(null); // State to store error messages
  const [loading, setLoading] = useState<boolean>(true); // State to track loading status

  // Fetch ingredients when the component using this hook mounts
  useEffect(() => {
    const loadIngredients = async () => {
      try {
        const data = await fetchIngredients(); // Fetch ingredients from the API
        setIngredients(data); // Update the ingredients state with the fetched data
      } catch (err) {
        setError("Failed to load ingredients. Please try again later.");
      } finally {
        setLoading(false); // Set loading to false after fetching
      }
    };

    loadIngredients();
  }, []); // Empty dependency array ensures this runs only once on mount

  return { ingredients, error, loading }; // Return state for use in components
};

export default useIngredients;