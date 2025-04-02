import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import { Canvas } from "@react-three/fiber";
import { OrbitControls, PerspectiveCamera } from "@react-three/drei";

import RenderLimitedArea from "./RenderLimitedArea";
import RenderFullerene from "./RenderFullerene";

export default function RenderAction() {
    const { id } = useParams();
    const [limitedArea, setLimitedArea] = useState(null);
    const [series, setSeries] = useState(0);

    useEffect(() => {
        async function fetchData() {
            let data = JSON.parse(sessionStorage.getItem(id));

            if (!data) {
                const data = await fetch(`/api/Main/get-fullerenes-and-limited-area/${id}`, {
                    method: 'GET',
                    credentials: 'include',
                });
                sessionStorage.setItem(id, JSON.stringify(data));
            }
            
            setLimitedArea(data);
        }
        fetchData();
    }, [id]);

    const handleNextSeries = () => {
        if (limitedArea && limitedArea.fullerenes) {
            const maxSeries = Math.max(...limitedArea.fullerenes.map(f => f.series));
            setSeries((prev) => (prev < maxSeries ? prev + 1 : prev));
        }
    };

    const handlePrevSeries = () => {
        setSeries((prev) => (prev > 0 ? prev - 1 : prev));
    };

    return (
        <div className="Content-Page">
            <Canvas style={{width: "100%", height: "70vh", border: "solid 1px"}} camera={{ position: [0, 0, 10] }}>
                <PerspectiveCamera makeDefault fov={90} position={[38.716473, 84.774315, -105.0208]} />
                <ambientLight intensity={0.5} />
                <pointLight position={[10, 10, 10]} />
                <OrbitControls />

                {limitedArea && <RenderLimitedArea LimitedArea={limitedArea} />}

                {limitedArea?.fullerenes ? (
                    limitedArea.fullerenes.map(fullerene => (
                        fullerene.series === series && <RenderFullerene Fullerene={fullerene} key={fullerene.id} />
                    ))
                ) : console.log("No fullerenes to render")}
            </Canvas>

            <div className="Controll-Panel">
                <button onClick={handlePrevSeries}>Previous series</button>
                <span>Series: {series}</span>
                <button onClick={handleNextSeries}>Next series</button>
            </div>

            <div>
                <Link className="Link" to={`/render/${id}/testCollision/${series}`} state={{ LimitedArea: limitedArea }}>
                    <button>Make sure there are no intersections</button>
                </Link>
                <Link className="Link" to={`/render/${id}/getDotsHitsDependency/${series}`}>
                    <button>Draw a graph of the density of fullerenes in the regions</button>
                </Link>
            </div>
        </div>
    );
}