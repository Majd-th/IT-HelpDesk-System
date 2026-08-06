import Sidebar from "./Sidebar";

import "../styles/Layout.css";

function Layout({ children }) {
    return (
        <div className="layout">
            <Sidebar />

            <div className="main">
                <header className="topbar">
                    <div className="topbar-title">
                        <h3>
                            IT Ticketing System
                        </h3>
                    </div>

                    <div className="topbar-right">
                        <button
                            type="button"
                            className="notification"
                            aria-label="Notifications"
                        >
                            🔔
                        </button>
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