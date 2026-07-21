import { useState } from "react";
import axios from "axios";

function ForgotPassword() {

    const [email, setEmail] = useState("");

    const handleSubmit = async (e) => {

        e.preventDefault();

        try {

            await axios.post(
                "http://localhost:5232/api/auth/forgot-password",
                { email }
            );

            alert("If the email exists, a reset link has been sent.");

        } catch {

            alert("Something went wrong.");

        }

    };

    return (

        <div>

            <h1>Forgot Password</h1>

            <form onSubmit={handleSubmit}>

                <input
                    type="email"
                    placeholder="Enter your email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />

                <br /><br />

                <button type="submit">
                    Send Reset Link
                </button>

            </form>

        </div>

    );

}

export default ForgotPassword;