import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

import Layout from "../components/Layout";
import TicketFilter from "../components/TicketFilter";
import TicketList from "../components/TicketList";

import {
    getTickets,
    deleteTicket,
    filterTickets
} from "../services/ticketService";

import "../assets/tickets.css";

function Tickets() {
    const [tickets, setTickets] = useState([]);
    const [loading, setLoading] = useState(true);

    const role = localStorage.getItem("role");

    const canCreateTicket =
        role === "Employee" ||
        role === "Admin";

    useEffect(() => {
        loadTickets();
    }, []);

    async function loadTickets() {
        try {
            setLoading(true);

            const data = await getTickets();

            setTickets(data);
        } catch (error) {
            console.error("Could not load tickets:", error);
            alert("Could not load tickets.");
        } finally {
            setLoading(false);
        }
    }

    async function handleDelete(id) {
        if (!window.confirm("Delete this ticket?")) {
            return;
        }

        try {
            await deleteTicket(id);
            await loadTickets();
        } catch (error) {
            console.error("Delete failed:", error);

            alert(
                error.response?.data?.message ||
                "You are not allowed to delete this ticket."
            );
        }
    }

    async function handleFilter(filters) {
        try {
            setLoading(true);

            const data = await filterTickets(filters);

            setTickets(data);
        } catch (error) {
            console.error("Filter failed:", error);
            alert("Could not filter tickets.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <Layout>
            <div className="ticket-page">
                <div className="ticket-header">
                    <h1>Tickets</h1>

                    {canCreateTicket && (
                        <Link to="/tickets/new">
                            <button className="create-btn">
                                + Create Ticket
                            </button>
                        </Link>
                    )}
                </div>

                <TicketFilter
                    onFilter={handleFilter}
                    onReset={loadTickets}
                />

                <br />

                {loading ? (
                    <p>Loading tickets...</p>
                ) : (
                    <TicketList
                        tickets={tickets}
                        onDelete={handleDelete}
                    />
                )}
            </div>
        </Layout>
    );
}

export default Tickets;