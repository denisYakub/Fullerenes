import React, { useEffect, useState } from "react";
import { Canvas } from "@react-three/fiber";
import { OrbitControls } from "@react-three/drei";

export default function RenderLimitedArea({ areaType, center, params }) {
    const [radius, setRadius] = useState(1); // По умолчанию радиус сферы 1
    const [position, setPosition] = useState([0, 0, 0]); // Центр (x, y, z)

    useEffect(() => {
        if (center) {
            setPosition([center.x, center.y, center.z]);
        }

        if (params && params.length > 0) {
            const radiusParam = params.find((p) => p.item1 === "Radius");
            if (radiusParam) {
                setRadius(parseFloat(radiusParam.item2));
            }
        }
    }, [center, params]);

    const renderSphere = () => {
        return (
            <mesh position={position}>
                <sphereGeometry args={[radius, 32, 32]} />
                <meshStandardMaterial color={"skyblue"} />
            </mesh>
        );
    };

    return (
        <div>
            <Canvas>
                <ambientLight intensity={0.5} />
                <pointLight position={[10, 10, 10]} />
                <OrbitControls />
                {areaType === 0 && renderSphere()} {/* Если areaType === 0, рисуем сферу */}
                {/* Здесь можно добавить другие фигуры для других areaType */}
            </Canvas>
        </div>
    );
}
