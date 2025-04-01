import * as THREE from "three";

export default function RenderFullerene({ Fullerene }) {
    const colors = ["red", "green", "blue"];

    return (<mesh position={[Fullerene.center.x, Fullerene.center.y, Fullerene.center.z]}
        rotation={[Fullerene.eulerAngles.praecessioAngle, Fullerene.eulerAngles.nutatioAngle, Fullerene.eulerAngles.properRotationAngle]}
    >
        <primitive attache="geometry" object={new THREE.IcosahedronGeometry(Fullerene.size)} />
        <meshStandardMaterial color={colors[Math.floor(Math.random() * colors.length)]} />
    </mesh>);
}