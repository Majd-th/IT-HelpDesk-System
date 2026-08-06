import {
    useEffect,
    useState
} from "react";

import Layout from
    "../components/Layout";

import TicketStatusCards from
    "../components/TicketStatusCards";

import DashboardChart from
    "../components/DashboardChart";

import RecentTickets from
    "../components/RecentTickets";

import {
    getTickets
} from "../services/ticketService";

import "../styles/Dashboard.css";

function EmployeeDashboard() {
    const [tickets, setTickets] =
        useState([]);

    const [loading, setLoading] =
        useState(true);

    const [error, setError] =
        useState("");

    useEffect(() => {
        loadTickets();
    }, []);

    async function loadTickets() {
        try {
            setLoading(true);
            setError("");

            const data =
                await getTickets();

            setTickets(data);
        } catch (requestError) {
            console.error(
                "Dashboard loading failed:",
                requestError
            );

            setError(
                requestError.response
                    ?.data?.message ||
                "Could not load dashboard tickets."
            );
        } finally {
            setLoading(false);
        }
    }

    if (loading) {
        return (
            <Layout>
                <p>
                    Loading dashboard...
                </p>
            </Layout>
        );
    }

    return (
        <Layout>
            <div className="dashboard-container">
                {error && (
                    <p className="error-message">
                        {error}
                    </p>
                )}

                {/*
                    Use all tickets returned by the backend.

                    This includes:
                    - Employee's own tickets
                    - Other resolved/closed tickets
                      that contain solutions
                */}
                <TicketStatusCards
                    tickets={tickets}
                />

                <DashboardChart />

                <RecentTickets
                    tickets={tickets}
                />
            </div>
        </Layout>
    );
}

export default EmployeeDashboard;