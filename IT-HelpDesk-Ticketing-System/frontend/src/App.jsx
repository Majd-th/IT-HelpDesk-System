import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import Profile from "./pages/Profile";
import Login from "./pages/Login";
import ForgotPassword from "./pages/ForgotPassword";
import ResetPassword from "./pages/ResetPassword";
import AdminPanel from "./pages/AdminPanel";
import AdminDashboard from "./pages/AdminDashboard";
import ManagerDashboard from "./pages/ManagerDashboard";
import AgentDashboard from "./pages/AgentDashboard";
import EmployeeDashboard from "./pages/EmployeeDashboard";
import AssignmentManagement from "./pages/AssignmentManagement";
import AvailableTickets from "./pages/AvailableTickets";
import AgentMyTickets from "./pages/AgentMyTickets";
import AgentTicketHistory from "./pages/AgentTicketHistory";
import Tickets from "./pages/Tickets";
import CreateTicket from "./pages/CreateTicket";
import EditTicket from "./pages/EditTicket";
import TicketDetails from "./pages/TicketDetails";
import NotificationsPage from
    "./pages/NotificationsPage";

//import Profile from "./pages/Profile";
//  import AdminPanel from "./pages/AdminPanel";

import ProtectedRoute from "./routes/ProtectedRoute";

function App() {
    return (
        <BrowserRouter>
            <Routes>
                {/* Public routes */}
                <Route path="/" element={<Login />} />

                <Route
                    path="/forgot-password"
                    element={<ForgotPassword />}
                />

                <Route
                    path="/reset-password"
                    element={<ResetPassword />}
                />

                {/* Role dashboards */}
                <Route
                    path="/admin/dashboard"
                    element={
                        <ProtectedRoute allowedRole="Admin">
                            <AdminDashboard />
                        </ProtectedRoute>
                    }
                />
<Route
    path="/assignments"
    element={
        <ProtectedRoute
            allowedRoles={["Manager", "Admin"]}
        >
            <AssignmentManagement />
        </ProtectedRoute>
    }
/>

<Route
    path="/agent/available-tickets"
    element={
        <ProtectedRoute
            allowedRole="IT Support Agent"
        >
            <AvailableTickets />
        </ProtectedRoute>
    }
/>

<Route
    path="/agent/my-tickets"
    element={
        <ProtectedRoute
            allowedRole="IT Support Agent"
        >
            <AgentMyTickets />
        </ProtectedRoute>
    }
/>

<Route
    path="/agent/ticket-history"
    element={
        <ProtectedRoute
            allowedRole="IT Support Agent"
        >
            <AgentTicketHistory />
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

                {/* Ticket routes */}
                <Route
                    path="/tickets"
                    element={
                        <ProtectedRoute>
                            <Tickets />
                        </ProtectedRoute>
                    }
                />
<Route
    path="/admin/settings"
    element={
        <ProtectedRoute allowedRole="Admin">
            <AdminPanel />
        </ProtectedRoute>
    }
/>
                <Route
                    path="/tickets/new"
                    element={
                        <ProtectedRoute
                            allowedRoles={["Employee", "Admin"]}
                        >
                            <CreateTicket />
                        </ProtectedRoute>
                    }
                />
<Route
    path="/profile"
    element={
        <ProtectedRoute>
            <Profile />
        </ProtectedRoute>
    }
/><Route
    path="/notifications"
    element={
        <NotificationsPage />
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

                <Route
                    path="/tickets/:id"
                    element={
                        <ProtectedRoute>
                            <TicketDetails />
                        </ProtectedRoute>
                    }
                />


                {/* Redirect old admin route */}
                <Route
                    path="/admin"
                    element={<Navigate to="/admin/dashboard" replace />}
                />

                {/* Unknown routes */}
                <Route
                    path="*"
                    element={<Navigate to="/" replace />}
                />
            </Routes>
        </BrowserRouter>
    );
}

export default App;