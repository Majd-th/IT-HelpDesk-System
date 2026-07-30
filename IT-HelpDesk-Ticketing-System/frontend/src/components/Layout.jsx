import { useNavigate } from "react-router-dom";

import Sidebar from "./Sidebar";

import "../styles/Layout.css";

function Layout({ children }) {
    const navigate = useNavigate();

    const role = localStorage.getItem("role");
    const fullName = localStorage.getItem("fullName");

    function handleLogout() {
        localStorage.clear();
        navigate("/");
    }

    return (
        <div className="layout">
            <Sidebar />

            <div className="main">
                <header className="topbar">
                    <div className="topbar-title">
                        <h3>IT Ticketing System</h3>
                    </div>

                    <div className="topbar-right">
                        <button
                            type="button"
                            className="notification"
                            aria-label="Notifications"
                        >
                            🔔
                        </button>

                        <div className="user-information">
                            <div className="user-details">
                                <span className="user-name">
                                    {fullName || "User"}
                                </span>

                                <span className="user-role">
                                    {role || ""}
                                </span>
                            </div>

                            <button
                                type="button"
                                className="logout-button"
                                onClick={handleLogout}
                            >
                                Logout
                            </button>
                        </div>
                    </div>
                </header>

                <main className="content">
                    {children}
                </main>
            </div>
        </div>
    );
}

export default Layout;