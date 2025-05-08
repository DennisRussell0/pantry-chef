import { createContext, useContext, useState } from "react";

type SelectedIngredientsContextType = {
  selectedIngredients: string[];
  addIngredient: (ingredient: string) => void;
  removeIngredient: (ingredient: string) => void;
  clearIngredients: () => void;
};

const SelectedIngredientsContext = createContext<SelectedIngredientsContextType | undefined>(undefined);

export const SelectedIngredientsProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [selectedIngredients, setSelectedIngredients] = useState<string[]>([]);

  const addIngredient = (ingredient: string) => {
    setSelectedIngredients((prev) => [...prev, ingredient]);
  };

  const removeIngredient = (ingredient: string) => {
    setSelectedIngredients((prev) => prev.filter((i) => i !== ingredient));
  };

  const clearIngredients = () => {
    setSelectedIngredients([]);
  };

  return (
    <SelectedIngredientsContext.Provider
      value={{ selectedIngredients, addIngredient, removeIngredient, clearIngredients }}
    >
      {children}
    </SelectedIngredientsContext.Provider>
  );
};

export const useSelectedIngredients = () => {
  const context = useContext(SelectedIngredientsContext);
  if (!context) {
    throw new Error("useSelectedIngredients must be used within a SelectedIngredientsProvider");
  }
  return context;
};