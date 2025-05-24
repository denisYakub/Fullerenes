import React, { useEffect, useState } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';

export default function IntenceOptPage({ qMin, qMax, qNum, superId }) {
    const [data, setData] = useState([]);

    useEffect(() => {
        async function fetchData() {
            try {
                const response = await fetch(`/api/Main/get-intens-opt-series/${qMin}/${qMax}/${qNum}/${superId}`,
                    {
                        method: 'GET'
                    }
                );

                const result = await response.json();
                console.log(result);

                const chartData = result.map(([q, i]) => ({
                    q,
                    I: result.item2[i]
                }));

                setData(chartData);

            } catch (error) {
                console.error('Ошибка при загрузке данных:', error);
            }
        }

        fetchData();
    }, [qMin, qMax, qNum, superId]);

    return (
        <ResponsiveContainer width="100%" height={300}>
            <LineChart data={data}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="index" label={{ value: 'q', position: 'insideBottom', offset: -5 }} />
                <YAxis label={{ value: 'I', angle: -90, position: 'insideLeft' }} />
                <Tooltip />
                <Line type="monotone" dataKey="value" stroke="#8884d8" strokeWidth={2} dot={false} />
            </LineChart>
        </ResponsiveContainer>
    );
}