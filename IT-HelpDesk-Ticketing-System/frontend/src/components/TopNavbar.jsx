import "./navbar.css";

function TopNavbar() {

    const fullName =
        localStorage.getItem("fullName");

    const role =
        localStorage.getItem("role");

    return (

        <header className="navbar">

            <div className="navbar-title">

                IT Help Desk

            </div>

            <div className="navbar-right">

                <div className="user-info">

                    <span className="user-name">

                        {fullName}

                    </span>

                    <span className="user-role">

                        {role}

                    </span>

                </div>

            </div>

        </header>

    );

}

export default TopNavbar;