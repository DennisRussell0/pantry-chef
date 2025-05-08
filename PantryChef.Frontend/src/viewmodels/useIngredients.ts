import { useEffect, useState } from "react";
import { fetchIngredients } from "../services/api";
import type { Ingredient } from "../types/Ingredient";

const useIngredients = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    const loadIngredients = async () => {
      try {
        const data = await fetchIngredients();
        setIngredients(data);
      } catch (err) {
        setError("Failed to load ingredients. Please try again later.");
      } finally {
        setLoading(false);
      }
    };

    loadIngredients();
  }, []);

  return { ingredients, error, loading };
};

export default useIngredients;