import { useState } from "react";
import { useParams, useLocation } from "react-router-dom";

export default function TestAreaOnFullereneCollisionPage() {
    const { series } = useParams();
    const [result, setResult] = useState(null);
    const [loading, setLoading] = useState(false);
    const location = useLocation();
    const limitedArea = location.state.LimitedArea;

    const transformData = (data) => {
        return {
            center: data.center,
            parameters: [data.radius],
            fullerenes: data.fullerenes
                .filter(f => f.series == series)
                .map(f => ({
                    center: f.center,
                    eulerAngles: f.eulerAngles,
                    size: f.size
                }))
        };
    };

    const handleTestCollision = async () => {
        setLoading(true);
        console.log(limitedArea);
        const requestData = transformData(limitedArea);
        console.log(requestData);

        const hasCollision = await fetch(`/api/Main/run-tests-on-fullerenes-collision-in-limited-area/Sphere/Icosaider`, {
            method: 'POST',
            credentials: 'include',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(requestData)
        });
        setResult(hasCollision);
        setLoading(false);
    };

    return (
        <div className="p-4 max-w-md mx-auto text-center">
            <h2 className="text-xl font-bold mb-4">Checking fullerene intersections</h2>
            <button className="px-4 py-2 bg-blue-500 text-white rounded" onClick={handleTestCollision} disabled={loading}>
                {loading ? "Checking..." : "Run test"}
            </button>
            {result !== null && (
                <p className={`mt-4 text-lg font-bold ${result ? "text-red-500" : "text-green-500"}`}>
                    {result ? "Intersections detected!" : "There are no intersections!"}
                </p>
            )}
        </div>
    );
}