import { BrowserRouter, Route, Routes } from "react-router-dom";

import "../src/Styles/Pages.css"

import NavBar from "./Pages/NavBar.jsx"
import InputPage from "./Pages/InputPage.jsx"
import RenderPage from "./Pages/RenderPage.jsx"
import RenderAction from "./Pages/RenderAction.jsx"
import TestAreaOnFullereneCollisionPage from "./Pages/TestAreaOnFullereneCollision.jsx"
import DotsHitsDependencyPage from "./Pages/DotsHitsDependencyPage.jsx"

export default function App() {
    return (<div className="Outer-Page">
        <div className="Inner-Page">
            <BrowserRouter>
                <NavBar />
                <Routes>
                    <Route path="/" element={<InputPage />} />
                    <Route path="/render" element={<RenderPage />} />
                    <Route path="/render/:id" element={<RenderAction />} />

                    <Route path="/render/:id/testCollision/:series" element={<TestAreaOnFullereneCollisionPage />} />
                    <Route path="/render/:id/getDotsHitsDependency/:series" element={<DotsHitsDependencyPage />} />
                </Routes>
            </BrowserRouter>
        </div>
    </div>);
}