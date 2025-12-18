import { useState } from "react";
import { register } from "../api/authApi";
import { Link, useNavigate } from "react-router-dom";
import '../design/RegisterPage.css';
import AuthPageWrapper from "./AuthPageWrapper";

export default function RegisterPage() {
    const [name, setName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    const navigate = useNavigate();

    const handleRegister = async () => {
        setError("");
        setLoading(true);
        try {
            const data = await register({ name, email, password });
            localStorage.setItem("token", data.token);
            alert("Register successful!");
            navigate("/");
        } catch (err) {
            setError("Register failed. Please check your inputs.");
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <AuthPageWrapper>
        <form onSubmit={(e) => e.preventDefault()}>
            <h2>Register</h2>
            {error && <div className="error">{error}</div>}

            <input
                type="text"
                placeholder="Name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
            />
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

            <button onClick={handleRegister} disabled={loading}>
                {loading ? "Registering..." : "Register"}
            </button>

            <Link to="/" className="register-btn">
                Already have an account? Login
            </Link>
        </form>
    </AuthPageWrapper>
    );
}
