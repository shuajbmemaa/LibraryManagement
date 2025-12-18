import api from "./axios";

export const getAllBooks = async (filter = {}) => {
    const res = await api.get("/book/getAllBooks", { params: filter });
    return res.data;
};

export const getBookById = async (id) => {
    const res = await api.get(`/book/${id}`);
    return res.data;
};

export const createBook = async (dto) => {
    const res = await api.post("/book", dto);
    return res.data;
};

export const updateBook = async (id, dto) => {
    const res = await api.put(`/book/${id}`, dto);
    return res.data;
};

export const deleteBook = async (id) => {
    const res = await api.delete(`/book/${id}`);
    return res.data;
};
