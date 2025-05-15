// Type definition for the SelectedIngredientsContext
export type SelectedIngredientsContextType = {
    selectedIngredients: string[]; // List of selected ingredients
    addIngredient: (ingredient: string) => void; // Function to add an ingredient
    removeIngredient: (ingredient: string) => void; // Function to remove an ingredient
    clearIngredients: () => void; // Function to clear all selected ingredients
};