import React from "react";
import ReactDOM from "react-dom/client";
import './index.css'
import App from "./App";
import { SelectedIngredientsProvider } from "./context/SelectedIngredientsContext";

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <SelectedIngredientsProvider>
      <App />
    </SelectedIngredientsProvider>
  </React.StrictMode>
);