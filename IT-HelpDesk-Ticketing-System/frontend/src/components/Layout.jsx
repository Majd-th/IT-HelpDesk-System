import { NavLink } from "react-router-dom";

import "../styles/Layout.css";

function Layout({children}){
    const role = localStorage.getItem("role");

let dashboardLink = "/employee";

if (role === "Admin")
    dashboardLink = "/admin";

else if (role === "Manager")
    dashboardLink = "/manager";

else if (role === "IT Support Agent")
    dashboardLink = "/agent";

    

    const fullName=localStorage.getItem("fullName");

    return (

<div className="layout">

    <aside className="sidebar">

        <div className="logo">

            IT HelpDesk

        </div>

        <nav className="menu">

          <NavLink to={dashboardLink}>Dashboard</NavLink>

            <NavLink to="/tickets">Tickets</NavLink>
{role !== "IT Support Agent" && (
    <NavLink to="/tickets/new">
        Create Ticket
    </NavLink>
)}

            <NavLink to="/profile">Profile</NavLink>

        </nav>

    </aside>

    <div className="main">

        <header className="topbar">

            <div>

                <h3>

                    {role}

                </h3>

            </div>

            <div className="topbar-right">

                <button className="notification">

                    🔔

                </button>

                <div className="user-menu">

                    👤 {fullName}

                    <button
                        onClick={()=>{
                            localStorage.clear();
                            window.location="/";
                        }}
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