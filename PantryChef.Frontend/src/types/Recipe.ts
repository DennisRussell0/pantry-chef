// Type definition for a Recipe object
export interface Recipe {
    id: number;
    title: string;
    matchingPercentage: number;
    ingredients: string[];
    originalText: string[];
    instructions: string;
    imageName: string;
}