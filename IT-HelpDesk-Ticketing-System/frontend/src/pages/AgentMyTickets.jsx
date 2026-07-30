import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

import Layout from "../components/Layout";

import {
    getMyAssignedTickets
} from "../services/assignmentService";

import "../assets/agent-tickets.css";

function AgentMyTickets() {
    const [tickets, setTickets] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        loadTickets();
    }, []);

    async function loadTickets() {
        try {
            setLoading(true);
            setError("");

            const data =
                await getMyAssignedTickets();

            setTickets(data);
        } catch (requestError) {
            console.error(
                "Could not load assigned tickets:",
                requestError
            );

            setError(
                requestError.response?.data?.message ||
                "Could not load your assigned tickets."
            );
        } finally {
            setLoading(false);
        }
    }

    function formatDate(date) {
        return date
            ? new Date(date).toLocaleString()
            : "—";
    }

    function statusClass(status) {
        switch (status?.toLowerCase()) {
            case "open":
                return "agent-status open";

            case "in progress":
                return "agent-status progress";

            case "pending":
                return "agent-status pending";

            default:
                return "agent-status";
        }
    }

    return (
        <Layout>
            <div className="agent-ticket-page">
                <div className="agent-ticket-heading">
                    <div>
                        <h1>My Assigned Tickets</h1>

                        <p>
                            View and work on tickets currently
                            assigned to you.
                        </p>
                    </div>

                    <button
                        type="button"
                        className="agent-refresh-button"
                        onClick={loadTickets}
                    >
                        Refresh
                    </button>
                </div>

                {error && (
                    <div className="agent-alert agent-error">
                        {error}
                    </div>
                )}

                <div className="agent-summary-card">
                    <span>Active Assigned Tickets</span>
                    <strong>{tickets.length}</strong>
                </div>

                {loading ? (
                    <div className="agent-ticket-card">
                        <p>Loading assigned tickets...</p>
                    </div>
                ) : tickets.length === 0 ? (
                    <div className="agent-ticket-card">
                        <p>
                            You currently have no active
                            assigned tickets.
                        </p>
                    </div>
                ) : (
                    <div className="agent-table-card">
                        <table className="agent-ticket-table">
                            <thead>
                                <tr>
                                    <th>Reference</th>
                                    <th>Title</th>
                                    <th>Category</th>
                                    <th>Priority</th>
                                    <th>Status</th>
                                    <th>Created By</th>
                                    <th>Created</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>

                            <tbody>
                                {tickets.map((ticket) => (
                                    <tr key={ticket.id}>
                                        <td>
                                            {
                                                ticket.referenceNumber
                                            }
                                        </td>

                                        <td>{ticket.title}</td>
                                        <td>{ticket.category}</td>
                                        <td>{ticket.priority}</td>

                                        <td>
                                            <span
                                                className={statusClass(
                                                    ticket.status
                                                )}
                                            >
                                                {ticket.status}
                                            </span>
                                        </td>

                                        <td>
                                            {ticket.createdBy}
                                        </td>

                                        <td>
                                            {formatDate(
                                                ticket.createdDate
                                            )}
                                        </td>

                                        <td>
                                            <Link
                                                to={`/tickets/${ticket.id}`}
                                            >
                                                <button
                                                    type="button"
                                                    className="agent-secondary-button"
                                                >
                                                    View
                                                </button>
                                            </Link>

                                            <Link
                                                to={`/tickets/edit/${ticket.id}`}
                                            >
                                                <button
                                                    type="button"
                                                    className="agent-primary-button"
                                                >
                                                    Update
                                                </button>
                                            </Link>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </Layout>
    );
}

export default AgentMyTickets;