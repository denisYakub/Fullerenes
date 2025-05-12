import AuthorizePage, { AuthorizedUser } from "../Pages/AuthorizePage"
import LogoutPage from "../Pages/LogoutPage"
import InputPage from "./InputPage";

function HomePage() {
    return (
        <AuthorizePage>
            <span><LogoutPage>Logout <AuthorizedUser value="email" /></LogoutPage></span>
            <InputPage />
        </AuthorizePage>
    );
}

export default HomePage;