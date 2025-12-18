import api from "./axios";

export const askAi = async (question) => {
    const res = await api.post("/AiQuery/ollama-query", { question });
    return res.data;
};