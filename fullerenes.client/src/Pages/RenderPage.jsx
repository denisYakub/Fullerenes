import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

export default function RenderPage() {
    const [ids, setIds] = useState([]);
    const navigate = useNavigate();

    useEffect(() => {
        const storedIds = localStorage.getItem("ids");
        if (storedIds) {
            setIds(JSON.parse(storedIds));
        }
    }, []);

    return (
        <div className="Content-Page">
            <h2>IDs of created Limited Area</h2>
            <ul className="List-Ids">
                {ids.length > 0 ? (
                    ids.map((id, index) => (
                        <li key={index} onClick={() => navigate(`/render/${id}`)}>
                            {id}
                        </li>
                    ))
                ) : (
                    <li className="Sub-Text">Nothing to render</li>
                )}
            </ul>
        </div>
    );
}