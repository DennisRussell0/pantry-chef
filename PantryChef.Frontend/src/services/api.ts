const API_BASE_URL = "http://localhost:5093/api";

// Fetch all ingredients from the API
export const fetchIngredients = async () => {
  const response = await fetch(`${API_BASE_URL}/Ingredients`); // API endpoint for ingredients
  if (!response.ok) {
    throw new Error("Failed to fetch ingredients");
  }
  return response.json();
};

// Fetch matching recipes based on selected ingredients
export const fetchMatchingRecipes = async (ingredients: string[], page: number = 1, pageSize: number = 10) => {
  const query = ingredients.join(","); // Join ingredients into a comma-separated string
  const response = await fetch(`${API_BASE_URL}/recipes/match?ingredients=${query}&page=${page}&pageSize=${pageSize}`); // API endpoint for matching recipes
  if (!response.ok) {
    throw new Error("Failed to fetch matching recipes");
  }
  return response.json();
};