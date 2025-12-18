import api from "./axios";

export const getAllUsers = async () => {
    const res = await api.get("/user");
    return res.data;
};

export const deleteUser = async (id) => {
    const res = await api.delete(`/user/${id}`);
    return res.data;
};

export const createAdminUser = async (dto) => {
    const res = await api.post("/user/create", dto);
    return res.data;
};