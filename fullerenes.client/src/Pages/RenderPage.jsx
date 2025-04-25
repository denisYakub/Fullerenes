import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom"; // Импортируем Link для создания ссылок

export default function RenderPage() {
    const [generations, setGenerations] = useState([]);

    useEffect(() => {
        const stored = JSON.parse(localStorage.getItem("generations") || "[]");

        // Keep only the highest series number per generationId
        const map = new Map();
        for (const { generationId, series } of stored) {
            if (!map.has(generationId) || map.get(generationId) < series) {
                map.set(generationId, series);
            }
        }

        const grouped = Array.from(map.entries()).map(([id, maxSeries]) => ({
            generationId: id,
            maxSeries,
        }));

        setGenerations(grouped);
    }, []);

    return (
        <div>
            <h2>Saved Generations</h2>
            {generations.map(({ generationId, maxSeries }) => (
                <div key={generationId}>
                    <h3>Generation #{generationId}</h3>
                    <ul>
                        {Array.from({ length: maxSeries + 1 }, (_, i) => (
                            <li key={i}>
                                {/* Создаем ссылку на RenderSeries с передачей seriesId и genId */}
                                <Link to={`/render-series/${i}/${generationId}`}>
                                    Series #{i}
                                </Link>
                            </li>
                        ))}
                    </ul>
                </div>
            ))}
        </div>
    );
}
