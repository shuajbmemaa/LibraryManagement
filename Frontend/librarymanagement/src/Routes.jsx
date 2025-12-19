import React from "react";
import { Routes, Route, Navigate} from "react-router-dom";
import LoginPage from "./components/LoginPage";
import RegisterPage from "./components/RegisterPage";
import useLocalStorage from 'use-local-storage'
import Dashboard from "./components/Dashboard";
import ProtectedRoute from "./components/ProtectedRoute";

export default function AllRoutes () {
    const [user, setUser] = useLocalStorage('user');
    
    return (
        <Routes>
            {!user ?
            (
                <>
                    <Route path="/" element={<LoginPage />} />
                    <Route path="/register" element={<RegisterPage />} />
                    <Route path="/*" element={<Navigate to="/" />} />
                </>
            ) : (
                <>
                    <Route path="/dashboard" element={
                        <ProtectedRoute>
                            <Dashboard />
                        </ProtectedRoute>
                    } />
                    <Route path="/*" element={<Navigate to="/dashboard" />} />
                </>
            )}
        </Routes>
    )
}