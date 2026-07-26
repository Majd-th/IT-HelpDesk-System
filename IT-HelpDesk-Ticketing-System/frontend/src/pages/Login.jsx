import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { login } from "../services/authService";
import "../styles/Login.css";

function Login() {

    const navigate=useNavigate();

    const[email,setEmail]=useState("");

    const[password,setPassword]=useState("");

    async function handleLogin(e){

        e.preventDefault();

        try{

            const result=await login(email,password);

            localStorage.setItem("token",result.token);

            const payload=JSON.parse(atob(result.token.split(".")[1]));

            const role=payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
const fullName =
payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];

localStorage.setItem("fullName", fullName);
            const name=payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];

            localStorage.setItem("role",role);

            localStorage.setItem("fullName",name);

            if(role==="Admin")
                navigate("/admin");

            else if(role==="Manager")
                navigate("/manager");

            else if(role==="IT Support Agent")
                navigate("/agent");

            else
                navigate("/employee");

        }

        catch{

            alert("Invalid credentials");

        }

    }

    return(

        <div className="login-page">

            <div className="login-card">

                <h1>

                    IT HelpDesk

                </h1>

                <p>

                    Sign in to your account

                </p>

                <form onSubmit={handleLogin}>

                    <input

                        type="email"

                        placeholder="Email"

                        value={email}

                        onChange={(e)=>setEmail(e.target.value)}

                    />

                    <input

                        type="password"

                        placeholder="Password"

                        value={password}

                        onChange={(e)=>setPassword(e.target.value)}

                    />

                    <button>

                        Login

                    </button>

                </form>

                <Link to="/forgot-password">

                    Forgot Password?

                </Link>

            </div>

        </div>

    );

}

export default Login;