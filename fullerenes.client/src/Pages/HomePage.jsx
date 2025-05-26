import { useEffect } from "react";

export default function HomePage() {
    const target = import.meta.env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${import.meta.env.ASPNETCORE_HTTPS_PORT}` :
        import.meta.env.ASPNETCORE_URLS ? import.meta.env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7245';

    useEffect(() => {
        fetch("/ping-auth", {
            credentials: "include",
        }).then(res => {
            if (res.status === 401) {
                window.location.href = `${target}/Identity/Account/Login`;
            } else {
                return res.json();
            }
        });
    });

    return (
        <div>
            Hello world!
            <a href={`${target}/Identity/Account/Logout`}>Logout</a>
        </div>
    );
}
