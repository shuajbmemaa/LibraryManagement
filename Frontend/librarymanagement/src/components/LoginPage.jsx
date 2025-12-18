import { useState } from "react";
import { login } from "../api/authApi";
import '../design/LoginPage.scss';
import { Link, useNavigate } from "react-router-dom";
import AuthPageWrapper from "./AuthPageWrapper";
import toast, {Toaster } from 'react-hot-toast'
import useLocalStorage from "use-local-storage";


export default function LoginPage() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const navigate = useNavigate();
    const [user, setUser] = useLocalStorage('user');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setLoading(true);

        try {
            const data = await login({ email, password });
            setUser(data);
            navigate('/dashboard')
        } catch (err) {
            setError("Email or password is incorrect");
        } finally {
            setLoading(false);
        }
    };

    return (
        <AuthPageWrapper>
            <Toaster />
            <form onSubmit={handleSubmit}>
                <>
                <h2>Login</h2>
                {error && <div className="error">{error}</div>}
                <input
                    type="email"
                    placeholder="Email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                />

                <input
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                />

                <div className="btns">
                
                <button type="submit" disabled={loading}>
                    {loading ? "Logging in..." : "Login"}
                </button>

                <Link to="/register" className="register-btn">
                    Register
                </Link>
                </div>
                </>
            </form>

        </AuthPageWrapper>
    );
}