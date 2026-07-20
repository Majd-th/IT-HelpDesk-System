import { getUser } from "../services/tokenService";
import LogoutButton from "../components/LogoutButton";

function ManagerDashboard() {

    const user = getUser();

    return (
        <div>

            <h1>Manager Dashboard</h1>

            

            <hr />

            <h2>Welcome, {user.firstName} {user.lastName}</h2>

            <p><strong>Email:</strong> {user.email}</p>

            <p><strong>Role:</strong> {user.role}</p>
<LogoutButton />
        </div>
    );
}

export default ManagerDashboard;