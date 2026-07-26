import { useEffect,useState } from "react";

import Layout from "../components/Layout";

import DashboardCard from "../components/DashboardCard";

import DashboardChart from "../components/DashboardChart";

import RecentTickets from "../components/RecentTickets";

import { getTickets } from "../services/ticketService";

import "../styles/Dashboard.css";

function AgentDashboard(){

    const [tickets,setTickets]=useState([]);

    useEffect(()=>{

        load();

    },[]);

    async function load(){

        setTickets(await getTickets());

    }

    return(

        <Layout>

            <div className="dashboard-container">

                <div className="dashboard-cards">

                    <DashboardCard

                        title="Open"

                        value={tickets.filter(t=>t.status==="Open").length}

                        color="#3498db"

                    />

                    <DashboardCard

                        title="Pending"

                        value={tickets.filter(t=>t.status==="Pending").length}

                        color="#f39c12"

                    />

                    <DashboardCard

                        title="Resolved"

                        value={tickets.filter(t=>t.status==="Resolved").length}

                        color="#2ecc71"

                    />

                    <DashboardCard

                        title="Critical"

                        value={tickets.filter(t=>t.priority==="Critical").length}

                        color="#e74c3c"

                    />

                </div>

                <DashboardChart/>

                <RecentTickets tickets={tickets}/>

            </div>

        </Layout>

    );

}
export default AgentDashboard;