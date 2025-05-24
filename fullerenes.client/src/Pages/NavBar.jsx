import {Link} from "react-router-dom";

function NavBar() {
    return(
        <nav className="Nav-Menu">
            <Link to="/input">Create Area with fullerenes</Link>
            <Link to="/render">Results</Link>
        </nav>
    );
}

export default NavBar;