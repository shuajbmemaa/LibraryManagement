import React from "react";
import { BrowserRouter } from "react-router-dom";
import LoginPage from "./components/LoginPage";
import RegisterPage from "./components/RegisterPage";
import AllRoutes from "./Routes";
import { AIModule } from "./ai/AiModule";
import './App.scss'

function App() {
  return (
    <BrowserRouter>
      <AllRoutes />
      <AIModule />
    </BrowserRouter>
  )
}

export default App;