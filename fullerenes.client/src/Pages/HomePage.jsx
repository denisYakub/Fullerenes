import { useEffect } from "react";

export default function HomePage() {
    useEffect(() => {
        fetch("/ping-auth", {
            credentials: "include",
        }).then(res => {
            if (res.status === 401) {
                window.location.href = `/Identity/Account/Login`;
            } else {
                return res.json();
            }
        });
    });

    return (
        <div>
            Main page
        </div>
    );
}
