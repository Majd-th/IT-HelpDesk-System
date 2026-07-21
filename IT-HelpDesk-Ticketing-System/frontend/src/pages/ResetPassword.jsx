import { useState } from "react";
import { useSearchParams } from "react-router-dom";
import axios from "axios";

function ResetPassword() {

    const [searchParams] = useSearchParams();

    const token = searchParams.get("token");

    const [newPassword, setNewPassword] = useState("");

    const handleSubmit = async (e) => {

        e.preventDefault();

        try {

            await axios.post("http://localhost:5232/api/auth/reset-password", {

                token,

                newPassword

            });

            alert("Password reset successfully!");

        }
        catch {

            alert("Invalid or expired reset link.");

        }

    };

    return (

        <div>

            <h1>Reset Password</h1>

            <form onSubmit={handleSubmit}>

                <input
                    type="password"
                    placeholder="New Password"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                />

                <br /><br />

                <button type="submit">
                    Reset Password
                </button>

            </form>

        </div>

    );

}

export default ResetPassword;