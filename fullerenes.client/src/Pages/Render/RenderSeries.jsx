import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import RenderLimitedArea from "../Render/RenderLimitedArea"
import PhiDistributionChart from "../PhiDistributionPage";

export default function RenderSeries() {
    const { superId } = useParams();
    const [loading, setLoading] = useState(true);
    const [data, setData] = useState(null);
    const [phis, setPhis] = useState(5);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch(`/api/Main/get-series-of-generation/${superId}`,
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
    }, [superId]);

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

            <div style={{ marginTop: '1rem' }}>
                <label htmlFor="phis-input">Number of layers : </label>
                <input
                    id="phis-input"
                    type="number"
                    min="1"
                    value={phis}
                    onChange={(e) => setPhis(Number(e.target.value))}
                    style={{ width: '60px', marginLeft: '10px' }}
                />
            </div>

            <PhiDistributionChart phis={phis} superId={superId} />
        </div>
    );
}
