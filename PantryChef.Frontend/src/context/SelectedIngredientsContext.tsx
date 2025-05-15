import { createContext, useContext, useState } from "react";
import type { SelectedIngredientsContextType } from "../types/SelectedIngredientsContextType";

// Create the context
const SelectedIngredientsContext = createContext<SelectedIngredientsContextType | undefined>(undefined);

// Provider component to manage selected ingredients
export const SelectedIngredientsProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [selectedIngredients, setSelectedIngredients] = useState<string[]>([]); // State to store selected ingredients

  // Add an ingredient to the list
  const addIngredient = (ingredient: string) => {
    setSelectedIngredients((prev) => [...prev, ingredient]);
  };

  // Remove an ingredient from the list
  const removeIngredient = (ingredient: string) => {
    setSelectedIngredients((prev) => prev.filter((i) => i !== ingredient));
  };

  // Clear all selected ingredients
  const clearIngredients = () => {
    setSelectedIngredients([]);
  };

  return (
    <SelectedIngredientsContext.Provider
      value={{ selectedIngredients, addIngredient, removeIngredient, clearIngredients }}
    >
      {children} {/* Render child components */}
    </SelectedIngredientsContext.Provider>
  );
};

// Hook to use the SelectedIngredientsContext
export const useSelectedIngredients = () => {
  const context = useContext(SelectedIngredientsContext);
  if (!context) {
    throw new Error("useSelectedIngredients must be used within a SelectedIngredientsProvider");
  }
  return context;
};