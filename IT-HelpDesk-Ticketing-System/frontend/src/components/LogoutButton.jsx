import { useNavigate } from "react-router-dom";

function LogoutButton() {

    const navigate = useNavigate();

    const logout = () => {

        localStorage.removeItem("token");

        navigate("/");

    };

    return (

        <button onClick={logout}>
            Logout
        </button>

    );
}

export default LogoutButton;