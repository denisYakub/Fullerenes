import { BrowserRouter, Route, Routes } from "react-router-dom";

import "../src/Styles/Pages.css"
import "../src/Styles/Loader.css"

import NavBar from "./Pages/NavBar.jsx"
import InputPage from "./Pages/InputPage.jsx"
import RenderPage from "./Pages/RenderPage.jsx"
import RenderSeries from "./Pages/Render/RenderSeries.jsx"
import LoginPage from "./Pages/LoginPage";
import HomePage from "./Pages/HomePage";
import RegisterPage from "./Pages/RegisterPage";

export default function App() {
    return (<div className="Outer-Page">
        <div className="Inner-Page">
            <BrowserRouter>
                <NavBar />
                <Routes>
                    <Route path="/" element={<HomePage />} />
                    <Route path="/login" element={<LoginPage />} />
                    <Route path="/register" element={<RegisterPage />} />

                    <Route path="/input" element={<InputPage />} />
                    <Route path="/render" element={<RenderPage />} />
                    <Route path="/render-series/:superId" element={<RenderSeries />} />
                 </Routes>
            </BrowserRouter>
        </div>
    </div>);
}