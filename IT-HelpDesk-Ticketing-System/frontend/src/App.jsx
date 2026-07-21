import { BrowserRouter, Routes, Route } from "react-router-dom";

import Login from "./pages/Login";

import AdminDashboard from "./pages/AdminDashboard";
import ManagerDashboard from "./pages/ManagerDashboard";
import AgentDashboard from "./pages/AgentDashboard";
import EmployeeDashboard from "./pages/EmployeeDashboard";

import ProtectedRoute from "./routes/ProtectedRoute";
import ResetPassword from "./pages/ResetPassword";
import ForgotPassword from "./pages/ForgotPassword";

function App() {
    return (
        <BrowserRouter>

            <Routes>

                <Route path="/" element={<Login />} />
                <Route
    path="/reset-password"
    element={<ResetPassword />}

/>
<Route
    path="/forgot-password"
    element={<ForgotPassword />}
/>

                <Route
                    path="/admin"
                    element={
                        <ProtectedRoute allowedRole="Admin">
                            <AdminDashboard />
                        </ProtectedRoute>
                    }
                />

                <Route
    path="/manager"
    element={
        <ProtectedRoute allowedRole="Manager">
            <ManagerDashboard />
        </ProtectedRoute>
    }
/>
                    
                

                <Route
                    path="/agent"
                    element={
                        <ProtectedRoute allowedRole="IT Support Agent">
                            <AgentDashboard />
                        </ProtectedRoute>
                    }
                />

                <Route
                    path="/employee"
                    element={
                        <ProtectedRoute allowedRole="Employee">
                            <EmployeeDashboard />
                        </ProtectedRoute>
                    }
                />

            </Routes>

        </BrowserRouter>
    );
}

export default App;