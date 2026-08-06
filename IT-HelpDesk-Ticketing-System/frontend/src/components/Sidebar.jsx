import {
    useEffect,
    useState
} from "react";

import {
    NavLink,
    useNavigate
} from "react-router-dom";

import {
    getProfile
} from "../services/profileService";

import {
    getDashboardRoute
} from "../utils/routeHelpers";

import "./sidebar.css";

function Sidebar() {
    const navigate = useNavigate();

    const role =
        localStorage.getItem("role");

    const [profile, setProfile] =
        useState({
            firstName: "",
            lastName: "",
            email: "",
            role: role || ""
        });

    useEffect(() => {
        loadProfile();
    }, []);

    async function loadProfile() {
        try {
            const data =
                await getProfile();

            setProfile({
                firstName:
                    data.firstName || "",

                lastName:
                    data.lastName || "",

                email:
                    data.email || "",

                role:
                    data.role || role || ""
            });

            const fullName =
                `${data.firstName || ""} ` +
                `${data.lastName || ""}`;

            localStorage.setItem(
                "fullName",
                fullName.trim()
            );

            localStorage.setItem(
                "email",
                data.email || ""
            );
        } catch (error) {
            console.error(
                "Could not load sidebar profile:",
                error
            );

            setProfile({
                firstName:
                    localStorage
                        .getItem("fullName")
                        ?.split(" ")[0] || "",

                lastName: "",

                email:
                    localStorage
                        .getItem("email") || "",

                role: role || ""
            });
        }
    }

    function handleLogout() {
        localStorage.clear();

        navigate("/");
    }

    const canCreateTicket =
        role === "Employee" ||
        role === "Admin";

    const canManageAssignments =
        role === "Manager" ||
        role === "Admin";

    const isAgent =
        role === "IT Support Agent";

    const fullName =
        `${profile.firstName} ` +
        `${profile.lastName}`;

    const initials =
        (
            profile.firstName?.charAt(0) ||
            fullName.trim().charAt(0) ||
            "U"
        ).toUpperCase();

    function getLinkClass({
        isActive
    }) {
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

                {role === "Admin" && (
                    <NavLink
                        to="/admin/settings"
                        className={getLinkClass}
                    >
                        Admin Panel
                    </NavLink>
                )}
            </nav>

            <div className="sidebar-account">
                <NavLink
                    to="/profile"
                    className="sidebar-profile-link"
                >
                    <div className="sidebar-avatar">
                        {initials}
                    </div>

                    <div className="sidebar-profile-text">
                        <span className="sidebar-profile-email">
                            {profile.email ||
                                "Profile"}
                        </span>

                        <span className="sidebar-profile-role">
                            {profile.role}
                        </span>
                    </div>
                </NavLink>

                <button
                    type="button"
                    className="sidebar-logout-button"
                    onClick={handleLogout}
                >
                    Logout
                </button>
            </div>
        </aside>
    );
}

export default Sidebar;