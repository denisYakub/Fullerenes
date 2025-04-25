import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import RenderLimitedArea from "../Render/RenderLimitedArea"

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
                    <RenderLimitedArea
                        areaType={data.areaType} fullereneType={data.fullereneType}
                        center={data.center} params={data.params}
                        fullerenes={data.fullerenes}
                />
            )}
        </div>
    );
}
