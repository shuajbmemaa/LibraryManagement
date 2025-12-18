import api from "./axios";
import useLocalStorage from "use-local-storage";
import { useNavigate } from "react-router-dom";

export const login = async (dto) => {
    const res = await api.post("/account/login", dto);
    return res.data;
};

export const register = async (dto) => {
    const res = await api.post("/account/register", dto);
    return res.data;
};

export function logout() {
    const [user, setUser] = useLocalStorage('user')
    const navigate = useNavigate();
    setUser(undefined);
    navigate('/')
}