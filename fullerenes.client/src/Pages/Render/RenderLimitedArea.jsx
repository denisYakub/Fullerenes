import React, { useEffect, useState } from "react";
import { Canvas } from "@react-three/fiber";
import { OrbitControls, PerspectiveCamera } from "@react-three/drei";
import * as THREE from "three";

export default function RenderLimitedArea({ areaType, fullereneType, center, params, fullerenes }) {
    const [radius, setRadius] = useState(1);
    const [position, setPosition] = useState([0, 0, 0]);
    const [selectedQuarter, setSelectedQuarter] = useState(0); // 0 - первая четверть, 1 - вторая и так далее
    const colors = ["red", "green", "blue"];

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

    // Функция для проверки попадания в одну из 8 частей
    const isInQuarter = (x, y, z) => {
        const halfRadius = radius / 2;
        switch (selectedQuarter) {
            case 0: // x >= 0, y >= 0, z >= 0
                return x >= 0 && y >= 0 && z >= 0;
            case 1: // x < 0, y >= 0, z >= 0
                return x < 0 && y >= 0 && z >= 0;
            case 2: // x >= 0, y < 0, z >= 0
                return x >= 0 && y < 0 && z >= 0;
            case 3: // x < 0, y < 0, z >= 0
                return x < 0 && y < 0 && z >= 0;
            case 4: // x >= 0, y >= 0, z < 0
                return x >= 0 && y >= 0 && z < 0;
            case 5: // x < 0, y >= 0, z < 0
                return x < 0 && y >= 0 && z < 0;
            case 6: // x >= 0, y < 0, z < 0
                return x >= 0 && y < 0 && z < 0;
            case 7: // x < 0, y < 0, z < 0
                return x < 0 && y < 0 && z < 0;
            default:
                return true;
        }
    };

    const renderSphere = () => {
        return (
            <mesh position={position}>
                <sphereGeometry args={[radius, 32, 32]} />
                <meshStandardMaterial color={"lightblue"} transparent={true} opacity={0.5} />
            </mesh>
        );
    };

    const renderFullerenes = () => {
        return fullerenes
            .filter((fullerene) => {
                const { center } = fullerene;
                return isInQuarter(center.x, center.y, center.z);
            })
            .map((fullerene, index) => {
                const { size, center, eulerAngles } = fullerene;
                return (
                    <mesh
                        key={index}
                        position={[center.x, center.y, center.z]}
                        rotation={[
                            eulerAngles.praecessioAngle,
                            eulerAngles.nutatioAngle,
                            eulerAngles.properRotationAngle,
                        ]}
                    >
                        <primitive attache="geometry" object={new THREE.IcosahedronGeometry(size)} />
                        <meshStandardMaterial color={colors[Math.floor(Math.random() * colors.length)]} />
                    </mesh>
                );
            });
    };

    return (
        <div>
            <div>
                <button onClick={() => setSelectedQuarter(0)}>Quarter 1</button>
                <button onClick={() => setSelectedQuarter(1)}>Quarter 2</button>
                <button onClick={() => setSelectedQuarter(2)}>Quarter 3</button>
                <button onClick={() => setSelectedQuarter(3)}>Quarter 4</button>
                <button onClick={() => setSelectedQuarter(4)}>Quarter 5</button>
                <button onClick={() => setSelectedQuarter(5)}>Quarter 6</button>
                <button onClick={() => setSelectedQuarter(6)}>Quarter 7</button>
                <button onClick={() => setSelectedQuarter(7)}>Quarter 8</button>
                <button onClick={() => setSelectedQuarter("all")}>All quarters</button>
            </div>
            <Canvas style={{ width: "100%", height: "70vh", border: "solid 1px" }} camera={{ position: [0, 0, 10] }}>
                <PerspectiveCamera makeDefault fov={90} position={[38.716473, 84.774315, -105.0208]} />
                <ambientLight intensity={0.5} />
                <pointLight position={[10, 10, 10]} />
                <OrbitControls />
                {areaType === 0 && renderSphere()} {/* Теперь отображаем только нужную часть сферы */}
                {fullereneType === 0 && renderFullerenes()} {/* Только те фуллерены, которые попадают в выбранную часть */}
            </Canvas>
        </div>
    );
}
