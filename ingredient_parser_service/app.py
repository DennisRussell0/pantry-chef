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

def clean_noun_chunk(chunk):
    chunk = re.sub(r"\b\d+([\/\.\d]*)?\b", "", chunk)
    words = [w for w in chunk.strip().split() if w.lower() not in stopwords and not w.isdigit()]
    return " ".join(words).strip()

def get_best_match(text, threshold=0.85):
    text = text.lower()
    cleaned_text = clean_noun_chunk(text).lower()
    for ing in common_ingredients:
        if ing.lower() == cleaned_text:
            return ing, 1.0

    query_emb = sbert.encode([text])[0]
    sims = ingredient_embeddings @ query_emb / (np.linalg.norm(ingredient_embeddings, axis=1) * np.linalg.norm(query_emb) + 1e-8)
    idx = int(np.argmax(sims))
    best_match = common_ingredients[idx]
    similarity = float(sims[idx])
    if similarity >= threshold:
        return best_match, similarity

    doc = nlp(text)
    noun_chunks = [clean_noun_chunk(chunk.text.lower()) for chunk in doc.noun_chunks]
    noun_chunks = [nc for nc in noun_chunks if nc]
    if noun_chunks:
        return max(noun_chunks, key=len), similarity

    return clean_noun_chunk(text), similarity

app = Flask(__name__)

@app.route("/parse-ingredient", methods=["POST"])
def parse_ingredient():
    data = request.get_json()
    text = data.get("ingredient", "")
    best_match, similarity = get_best_match(text)
    return jsonify({"core_ingredient": best_match, "similarity": similarity})

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5005)