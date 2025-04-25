import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";

export default function RenderSeries() {
    const { seriesId, genId } = useParams();
    const [loading, setLoading] = useState(true);
    const [data, setData] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch(`/api/Main/get-series-of-generation/${seriesId}/${genId}`,
                    {
                        method: "GET",
                        redirect: "follow",
                    }
                );

                if (!response.ok) {
                    throw new Error("Error fetching data");
                }

                const result = await response.json();
                setData(result);
                setLoading(false);
            } catch (error) {
                console.error(error);
                setLoading(false);
                alert("Error loading data");
            }
        };

        fetchData();
    }, [seriesId, genId]);

    return (
        <div>
            {loading ? (
                <div className="loader"></div>  // Спиннер
            ) : (
                <div>
                    <h2>Series {seriesId} of Generation {genId}</h2>
                    <div className="data-container">
                        {/* Выводим данные в виде таблицы */}
                        <table>
                            <thead>
                                <tr>
                                    <th>Key</th>
                                    <th>Value</th>
                                </tr>
                            </thead>
                            <tbody>
                                {Object.entries(data).map(([key, value]) => (
                                    <tr key={key}>
                                        <td>{key}</td>
                                        <td>{JSON.stringify(value, null, 2)}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>
            )}
        </div>
    );
}
