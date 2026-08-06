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

function ManagerDashboard() {
    const [tickets, setTickets] =
        useState([]);

    useEffect(() => {
        load();
    }, []);

    async function load() {
        const data =
            await getTickets();

        setTickets(data);
    }

    return (
        <Layout>
            <div className="dashboard-container">
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

export default ManagerDashboard;