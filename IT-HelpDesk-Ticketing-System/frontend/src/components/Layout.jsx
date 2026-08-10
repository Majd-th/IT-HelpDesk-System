import Sidebar from "./Sidebar";
import NotificationBell from
    "./NotificationBell";
import "../styles/Layout.css";
import NotificationRealtimeListener
    from "./NotificationRealtimeListener";

function Layout({ children }) {
    return (
        <div className="layout">
             <NotificationRealtimeListener/>
            <Sidebar />

            <div className="main">
                <header className="topbar">
                    <div className="topbar-title">
                        <h3>
                            IT Ticketing System
                        </h3>
                    </div>
                   

                    <div className="topbar-right">
                    
                              <NotificationBell />
                    
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