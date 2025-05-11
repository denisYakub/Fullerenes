import { Navigate } from 'react-router-dom';
import AuthorizePage from "../Pages/AuthorizePage"
import LogoutPage from "../Pages/LogoutPage"

function HomePage() {
    return (<AuthorizePage>
        <span><LogoutPage value="email">Logout</LogoutPage></span>
        <Navigate to="/input" />
    </AuthorizePage>);
}

export default HomePage;