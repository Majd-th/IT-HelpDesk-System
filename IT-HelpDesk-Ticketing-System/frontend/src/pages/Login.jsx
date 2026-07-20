import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { login } from "../services/authService";

function Login() {

    const navigate = useNavigate();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

   
    const handleLogin = async (e) => {
    e.preventDefault();

    try {

        const result = await login(email, password);

        console.log(result);

        // Save JWT
        localStorage.setItem("token", result.token);

        // Decode JWT
        const payload = JSON.parse(atob(result.token.split(".")[1]));

        const role =
            payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

        console.log("Role:", role);

        // Save role
        localStorage.setItem("role", role);

        // Redirect based on role
        if (role === "Admin") {
            navigate("/admin");
        }
        else if (role === "Manager") {
            navigate("/manager");
        }
        else if (role === "IT Support Agent") {
            navigate("/agent");
        }
        else {
            navigate("/employee");
        }

    } catch (error) {
        console.error(error);
        alert("Invalid credentials");
    }
};
    return (
        <div>

            <h1>IT HelpDesk Login</h1>

            <form onSubmit={handleLogin}>

                <input
                    type="email"
                    placeholder="Email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />

                <br /><br />

                <input
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />

                <br /><br />

                <button type="submit">
                    Login
                </button>

            </form>

        </div>
    );
}

export default Login;