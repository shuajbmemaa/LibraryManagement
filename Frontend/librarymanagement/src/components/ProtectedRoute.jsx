import { Navigate } from "react-router-dom";
import useLocalStorage from "use-local-storage";

const ProtectedRoute = ({ children, adminOnly = false }) => {
    const [user] = useLocalStorage('user');

    if (!user) {
        return <Navigate to="/" />;
    }

    if (adminOnly && !user.isAdmin) {
        return <Navigate to="/dashboard" />;
    }

    return children;
};

export default ProtectedRoute;