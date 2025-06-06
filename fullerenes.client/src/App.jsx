import { BrowserRouter, Route, Routes } from "react-router-dom";

import "../src/Styles/Pages.css"
import "../src/Styles/Loader.css"

import NavBar from "./Pages/NavBar.jsx"
import InputPage from "./Pages/InputPage.jsx"
import RenderPage from "./Pages/RenderPage.jsx"
import RenderSeries from "./Pages/Render/RenderSeries.jsx"
import HomePage from "./Pages/HomePage";

export default function App() {
    return (<div className="Outer-Page">
        <div className="Inner-Page">
            <BrowserRouter>
                <NavBar />
                <Routes>
                    <Route path="/" element={<HomePage />} />
                    <Route path="/input" element={<InputPage />} />
                    <Route path="/render" element={<RenderPage />} />
                    <Route path="/render-series/:superId" element={<RenderSeries />} />
                 </Routes>
            </BrowserRouter>
        </div>
    </div>);
}