import React, { useEffect, useState } from 'react';
import { LineChart, Label, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';

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
                console.log(result.value);

                const { q, i } = result.value;

                const combined = q.map((qValue, index) => ({
                    q: qValue,
                    I: i[index]
                }));

                setData(combined);

            } catch (error) {
                console.error('Ошибка при загрузке данных:', error);
            }
        }

        fetchData();
    }, [qMin, qMax, qNum, superId]);

    return (
        <ResponsiveContainer width="100%" height={300}>
            <LineChart data={data}>
                <CartesianGrid stroke="#eee" strokeDasharray="5 5" />
                <XAxis dataKey="q" />
                <YAxis />
                <Tooltip />
                <Line type="monotone" dataKey="I" stroke="#8884d8" name="Интенсивность I(q)" dot={false} />
            </LineChart>
        </ResponsiveContainer>
    );
}