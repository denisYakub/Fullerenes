import {Link} from "react-router-dom";

function NavBar() {
    return(
        <nav className="Nav-Menu">
            <Link to="/input">Create Area with fullerenes</Link>
            <Link to="/render">Results</Link>
            <a href={`/Identity/Account/Logout`}>Logout</a>
        </nav>
    );
}

export default NavBar;