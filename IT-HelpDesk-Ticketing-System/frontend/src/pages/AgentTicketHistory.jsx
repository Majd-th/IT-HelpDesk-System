import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";

import Layout from "../components/Layout";

import {
    getMyTicketHistory
} from "../services/assignmentService";

import "../assets/agent-tickets.css";

function AgentTicketHistory() {
    const [tickets, setTickets] = useState([]);
    const [search, setSearch] = useState("");
    const [status, setStatus] = useState("");
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        loadHistory();
    }, []);

    async function loadHistory() {
        try {
            setLoading(true);
            setError("");

            const data = await getMyTicketHistory();

            setTickets(data);
        } catch (requestError) {
            console.error(
                "Could not load ticket history:",
                requestError
            );

            setError(
                requestError.response?.data?.message ||
                "Could not load your ticket history."
            );
        } finally {
            setLoading(false);
        }
    }

    const filteredTickets = useMemo(() => {
        const normalizedSearch =
            search.trim().toLowerCase();

        return tickets.filter((ticket) => {
            const matchesSearch =
                !normalizedSearch ||
                ticket.referenceNumber
                    ?.toLowerCase()
                    .includes(normalizedSearch) ||
                ticket.title
                    ?.toLowerCase()
                    .includes(normalizedSearch) ||
                ticket.category
                    ?.toLowerCase()
                    .includes(normalizedSearch);

            const matchesStatus =
                !status ||
                ticket.status === status;

            return matchesSearch && matchesStatus;
        });
    }, [tickets, search, status]);

    function formatDate(date) {
        return date
            ? new Date(date).toLocaleString()
            : "—";
    }

    function statusClass(ticketStatus) {
        switch (ticketStatus?.toLowerCase()) {
            case "resolved":
                return "agent-status resolved";

            case "closed":
                return "agent-status closed";

            default:
                return "agent-status";
        }
    }

    return (
        <Layout>
            <div className="agent-ticket-page">
                <div className="agent-ticket-heading">
                    <div>
                        <h1>Ticket History</h1>

                        <p>
                            View your Resolved and Closed
                            tickets.
                        </p>
                    </div>

                    <button
                        type="button"
                        className="agent-refresh-button"
                        onClick={loadHistory}
                    >
                        Refresh
                    </button>
                </div>

                {error && (
                    <div className="agent-alert agent-error">
                        {error}
                    </div>
                )}

                <div className="agent-history-filters">
                    <input
                        type="search"
                        placeholder="Search reference, title, or category..."
                        value={search}
                        onChange={(event) =>
                            setSearch(event.target.value)
                        }
                    />

                    <select
                        value={status}
                        onChange={(event) =>
                            setStatus(event.target.value)
                        }
                    >
                        <option value="">
                            All completed statuses
                        </option>

                        <option value="Resolved">
                            Resolved
                        </option>

                        <option value="Closed">
                            Closed
                        </option>
                    </select>

                    <button
                        type="button"
                        className="agent-secondary-button"
                        onClick={() => {
                            setSearch("");
                            setStatus("");
                        }}
                    >
                        Clear
                    </button>
                </div>

                <div className="agent-summary-card">
                    <span>History Results</span>
                    <strong>
                        {filteredTickets.length}
                    </strong>
                </div>

                {loading ? (
                    <div className="agent-ticket-card">
                        <p>Loading ticket history...</p>
                    </div>
                ) : filteredTickets.length === 0 ? (
                    <div className="agent-ticket-card">
                        <p>
                            No Resolved or Closed tickets were
                            found.
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
                                    <th>Action</th>
                                </tr>
                            </thead>

                            <tbody>
                                {filteredTickets.map(
                                    (ticket) => (
                                        <tr key={ticket.id}>
                                            <td>
                                                {
                                                    ticket.referenceNumber
                                                }
                                            </td>

                                            <td>
                                                {ticket.title}
                                            </td>

                                            <td>
                                                {ticket.category}
                                            </td>

                                            <td>
                                                {ticket.priority}
                                            </td>

                                            <td>
                                                <span
                                                    className={statusClass(
                                                        ticket.status
                                                    )}
                                                >
                                                    {
                                                        ticket.status
                                                    }
                                                </span>
                                            </td>

                                            <td>
                                                {
                                                    ticket.createdBy
                                                }
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
                                            </td>
                                        </tr>
                                    )
                                )}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </Layout>
    );
}

export default AgentTicketHistory;