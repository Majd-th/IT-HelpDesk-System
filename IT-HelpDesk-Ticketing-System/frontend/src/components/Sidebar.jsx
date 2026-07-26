import { Link } from "react-router-dom";
import "./sidebar.css";
import { getDashboardRoute } from "../utils/routeHelpers";

function Sidebar() {
    const role = localStorage.getItem("role");

    return (
        <aside className="sidebar">

            <Link to={getDashboardRoute(role)}>
                Dashboard
            </Link>

            <Link to="/tickets">
                Tickets
            </Link>

            {(role === "Employee" || role === "Manager") && (
                <Link to="/tickets/create">
                    Create Ticket
                </Link>
            )}

            <Link to="/profile">
                Profile
            </Link>

            {role === "Admin" && (
                <Link to="/admin">
                    Admin Panel
                </Link>
            )}

        </aside>
    );
}

export default Sidebar;