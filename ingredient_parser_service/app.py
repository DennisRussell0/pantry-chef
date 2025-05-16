from flask import Flask, request, jsonify
from sentence_transformers import SentenceTransformer
import numpy as np
import spacy
import re

sbert = SentenceTransformer('all-MiniLM-L6-v2')
nlp = spacy.load("en_core_web_lg")

with open("stopwords.txt", encoding="utf-8") as f:
    stopwords = set(line.strip().lower() for line in f if line.strip())

with open("common_ingredients.txt", encoding="utf-8") as f:
    common_ingredients = [line.strip() for line in f if line.strip()]

ingredient_embeddings = sbert.encode(common_ingredients)

def get_best_match(text):
    text = text.lower()
    query_emb = sbert.encode([text])[0]
    sims = ingredient_embeddings @ query_emb / (np.linalg.norm(ingredient_embeddings, axis=1) * np.linalg.norm(query_emb) + 1e-8)
    idx = int(np.argmax(sims))
    best_match = common_ingredients[idx]
    similarity = float(sims[idx])

    return best_match, similarity

app = Flask(__name__)

@app.route("/parse-ingredient", methods=["POST"])
def parse_ingredient():
    data = request.get_json()
    text = data.get("ingredient", "")
    best_match, similarity = get_best_match(text)
    return jsonify({"core_ingredient": best_match, "similarity": similarity})

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5005)