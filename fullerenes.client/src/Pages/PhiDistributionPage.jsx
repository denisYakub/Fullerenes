import React, { useEffect, useState } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';

export default function PhiDistributionChart({ phis, superId }) {
    const [data, setData] = useState([]);

    useEffect(() => {
        async function fetchData() {
            try {
                const response = await fetch(`/api/Main/get-phis-of-generation-series/${phis}/${superId}`,
                    {
                    method: 'GET',
                    redirect: "follow",
                    }
                );
                const result = await response.json();

                console.log(result)
                // Преобразуем в формат, подходящий для Recharts
                const chartData = result.map((value, index) => ({
                    index,
                    value
                }));
                setData(chartData);
            } catch (error) {
                console.error('Ошибка при загрузке данных:', error);
            }
        }

        fetchData();
    }, [phis, superId]);

    return (
        <ResponsiveContainer width="100%" height={300}>
            <LineChart data={data}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="index" label={{ value: 'Layer', position: 'insideBottom', offset: -5 }} />
                <YAxis label={{ value: 'Phi', angle: -90, position: 'insideLeft' }} />
                <Tooltip />
                <Line type="monotone" dataKey="value" stroke="#8884d8" strokeWidth={2} dot={false} />
            </LineChart>
        </ResponsiveContainer>
    );
}
