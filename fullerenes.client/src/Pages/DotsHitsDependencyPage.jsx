import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend } from 'recharts';

export default function DotsHitsDependencyPage() {
    const { id, series } = useParams();
    const [numberOfLayers, setNumberOfLayers] = useState(5);
    const [numberOfDots, setNumberOfDots] = useState(1_000);
    const [excess, setExcess] = useState(0);
    const [density, setDensity] = useState();
    const [loading, setLoading] = useState(false);

    // Обработчики для изменения значений
    const handleLayersChange = (e) => setNumberOfLayers(e.target.value);
    const handleDotsChange = (e) => setNumberOfDots(e.target.value);
    const handleExcessChange = (e) => setExcess(e.target.value);

    const handleSubmit = async () => {
        setLoading(true);

        const data = await fetch(`/api/Main/create-density-of-fullerenes-in-layers/${id}/${series}/${numberOfLayers}/${numberOfDots}/${excess}`, {
            method: 'POST',
            credentials: 'include',
            headers: { "Content-Type": "application/json" },
            body: {}
        });
        console.log(data);

        const densityData = Object.entries(data.item1).map(([key, value]) => ({
            layer: key,
            density: value
        }));

        setDensity(densityData);

        setExcess(data.item2);
        console.log(excess);

        setLoading(false);
    };

    useEffect(() => {
        handleSubmit();
    }, [id, series]);

    return (
        <div>
            <h2>Dots in Fullerene vs Dots in Layer</h2>

            <div>
                <label>
                    Number of Layers:
                    <input
                        type="number"
                        value={numberOfLayers}
                        onChange={handleLayersChange}
                    />
                </label>
                <label>
                    Number of Dots:
                    <input
                        type="number"
                        value={numberOfDots}
                        onChange={handleDotsChange}
                    />
                </label>
                <label>
                    Excess:
                    <input
                        type="number"
                        value={excess}
                        onChange={handleExcessChange}
                    />
                </label>
                <button
                    onClick={handleSubmit}
                    disabled={loading}
                >
                    {loading ? "Checking..." : "Submit"}
                </button>
            </div>

            <div className="Dependency-Chart">
                <LineChart width={700} height={300} data={density} >
                    <Line type="monotone" dataKey="density" name="density in layer" stroke="green" />
                    <Tooltip />
                    <Legend />
                    <CartesianGrid stroke="#ccc" />
                    <XAxis dataKey="layer" />
                    <YAxis />
                </LineChart>
            </div>
        </div>
    );
}