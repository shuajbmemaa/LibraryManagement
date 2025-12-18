import React from "react";
import backgroundImage from '../assets/static/background.jpg'

export default function AuthPageWrapper ({children}) {
    return(
        <div className="login-wrapper">
            <div className="login-card">
                <div className="form-container">
                    {children}
                </div>
            </div>
        </div>
    )
}