import { BrowserRouter, Routes, Route } from "react-router-dom";

import Login from "./pages/Login";

import AdminDashboard from "./pages/AdminDashboard";
import ManagerDashboard from "./pages/ManagerDashboard";
import AgentDashboard from "./pages/AgentDashboard";
import EmployeeDashboard from "./pages/EmployeeDashboard";

import ProtectedRoute from "./routes/ProtectedRoute";

import ResetPassword from "./pages/ResetPassword";
import ForgotPassword from "./pages/ForgotPassword";

import Tickets from "./pages/Tickets";

import CreateTicket from "./pages/CreateTicket";
import EditTicket from "./pages/EditTicket";
import TicketDetails from "./pages/TicketDetails";
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

    <Route
    path="/tickets"
    element={
        <ProtectedRoute>
            <Tickets />
        </ProtectedRoute>
    }
/>

<Route
    path="/tickets/new"
    element={
        <ProtectedRoute>
            <CreateTicket />
        </ProtectedRoute>
    }
/>

<Route
    path="/tickets/:id"
    element={
        <ProtectedRoute>
            <TicketDetails />
        </ProtectedRoute>
    }
/>

<Route
    path="/tickets/edit/:id"
    element={
        <ProtectedRoute>
            <EditTicket />
        </ProtectedRoute>
    }
/>
    </Routes>

</BrowserRouter>
        
    );
}

export default App;