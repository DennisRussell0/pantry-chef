export interface Recipe {
    id: number;
    title: string;
    matchingPercentage: number;
    ingredients: string[];
    instructions: string;
    imageName: string;
}