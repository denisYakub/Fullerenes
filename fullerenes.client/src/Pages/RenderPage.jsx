import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom"; // Импортируем Link для создания ссылок

export default function RenderPage() {
    const [generations, setGenerations] = useState([]);

    useEffect(() => {
        const data = localStorage.getItem('generations');
        if (data) {
            setGenerations(JSON.parse(data));
        }
    }, []);

    return (
        <div>
            {generations.map((gen) => (
                <div key={gen.id}>
                    <h3>Generation #{gen.id}</h3>
                    {gen.superIds.map((superId) => (
                        <div key={superId}>
                            <Link to={`/render-series/${superId}`}>
                                Series #{superId}
                            </Link>
                        </div>
                    ))}
                </div>
            ))}
        </div>
    );
}
