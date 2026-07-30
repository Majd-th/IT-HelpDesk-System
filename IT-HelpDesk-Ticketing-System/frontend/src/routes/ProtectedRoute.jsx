import { Navigate } from "react-router-dom";

function ProtectedRoute({
    children,
    allowedRole,
    allowedRoles
}) {
    const token = localStorage.getItem("token");
    const role = localStorage.getItem("role");

    if (!token) {
        return <Navigate to="/" replace />;
    }

    if (
        allowedRole &&
        role !== allowedRole
    ) {
        return <Navigate to="/tickets" replace />;
    }

    if (
        allowedRoles &&
        !allowedRoles.includes(role)
    ) {
        return <Navigate to="/tickets" replace />;
    }

    return children;
}

export default ProtectedRoute;