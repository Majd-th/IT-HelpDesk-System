import { NavLink } from "react-router-dom";

import { getDashboardRoute } from "../utils/routeHelpers";

import "./sidebar.css";

function Sidebar() {
    const role = localStorage.getItem("role");

    const canCreateTicket =
        role === "Employee" ||
        role === "Admin";

    const canManageAssignments =
        role === "Manager" ||
        role === "Admin";

    const isAgent =
        role === "IT Support Agent";

    function getLinkClass({ isActive }) {
        return isActive
            ? "menu-link active"
            : "menu-link";
    }

    return (
        <aside className="sidebar">
            <div className="logo">
                IT HelpDesk
            </div>

            <nav className="menu">
                <NavLink
                    to={getDashboardRoute(role)}
                    className={getLinkClass}
                >
                    Dashboard
                </NavLink>

                <NavLink
                    to="/tickets"
                    className={getLinkClass}
                >
                    Tickets
                </NavLink>

                {canCreateTicket && (
                    <NavLink
                        to="/tickets/new"
                        className={getLinkClass}
                    >
                        Create Ticket
                    </NavLink>
                )}

                {canManageAssignments && (
                    <NavLink
                        to="/assignments"
                        className={getLinkClass}
                    >
                        Assignment Management
                    </NavLink>
                )}

                {isAgent && (
                    <>
                        <NavLink
                            to="/agent/available-tickets"
                            className={getLinkClass}
                        >
                            Available Tickets
                        </NavLink>

                        <NavLink
                            to="/agent/my-tickets"
                            className={getLinkClass}
                        >
                            My Assigned Tickets
                        </NavLink>

                        <NavLink
                            to="/agent/ticket-history"
                            className={getLinkClass}
                        >
                            Ticket History
                        </NavLink>
                    </>
                )}

                <NavLink
                    to="/profile"
                    className={getLinkClass}
                >
                    Profile
                </NavLink>

                {role === "Admin" && (
                    <NavLink
                        to="/admin/settings"
                        className={getLinkClass}
                    >
                        Admin Panel
                    </NavLink>
                )}
            </nav>
        </aside>
    );
}

export default Sidebar;