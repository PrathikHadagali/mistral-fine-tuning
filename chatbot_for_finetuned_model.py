from fastapi import FastAPI
from pydantic import BaseModel
from transformers import AutoTokenizer, AutoModelForCausalLM, pipeline

MODEL_PATH = r"D:\AR\FineTuning_LLMs\finetuned_model"

# Load model + tokenizer
tokenizer = AutoTokenizer.from_pretrained(MODEL_PATH)
model = AutoModelForCausalLM.from_pretrained(MODEL_PATH)
chatbot = pipeline("text-generation", model=model, tokenizer=tokenizer)

# FastAPI app
app = FastAPI()

class Query(BaseModel):
    question: str

def ask_chatbot(question):
    prompt = f"User: {question}\nBot:"
    response = chatbot(prompt, max_length=250, temperature=0.7, top_p=0.9, do_sample=True)
    return response[0]["generated_text"].split("Bot:")[-1].strip()

@app.post("/chat")
async def chat(query: Query):
    answer = ask_chatbot(query.question)
    return {"answer": answer}
