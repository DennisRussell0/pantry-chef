const API_BASE_URL = "http://localhost:5093/api";

export const fetchIngredients = async () => {
  const response = await fetch(`${API_BASE_URL}/Ingredients`);
  if (!response.ok) {
    throw new Error("Failed to fetch ingredients");
  }
  return response.json();
};

export const fetchMatchingRecipes = async (ingredients: string[], page: number = 1, pageSize: number = 10) => {
  const query = ingredients.join(",");
  const response = await fetch(`${API_BASE_URL}/recipes/match?ingredients=${query}&page=${page}&pageSize=${pageSize}`);
  if (!response.ok) {
    throw new Error("Failed to fetch matching recipes");
  }
  return response.json();
};