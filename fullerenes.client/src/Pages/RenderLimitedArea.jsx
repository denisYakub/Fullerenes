import { Sphere } from "@react-three/drei";

export default function RenderLimitedArea({ LimitedArea }) {

    return (<mesh position={[LimitedArea.center.x, LimitedArea.center.y, LimitedArea.center.z]}>
        {LimitedArea.radius ? (
            <Sphere args={[LimitedArea.radius, 32, 32]}>
                <meshPhysicalMaterial
                    transparent
                    opacity={0.3}
                    roughness={0.1}
                    transmission={1}
                    color="lightblue"
                />
            </Sphere>
        ) : null }
    </mesh>);
}