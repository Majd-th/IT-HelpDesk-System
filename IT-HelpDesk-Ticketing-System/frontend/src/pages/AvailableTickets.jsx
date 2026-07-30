import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

import Layout from "../components/Layout";

import {
    getAvailableTickets,
    requestTicket
} from "../services/assignmentService";

import "../assets/agent-tickets.css";

function AvailableTickets() {
    const [tickets, setTickets] = useState([]);
    const [notes, setNotes] = useState({});
    const [loading, setLoading] = useState(true);
    const [processingId, setProcessingId] =
        useState(null);

    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    useEffect(() => {
        loadTickets();
    }, []);

    async function loadTickets() {
        try {
            setLoading(true);
            setError("");

            const data = await getAvailableTickets();

            setTickets(data);
        } catch (requestError) {
            console.error(
                "Could not load available tickets:",
                requestError
            );

            setError(
                requestError.response?.data?.message ||
                "Could not load available tickets."
            );
        } finally {
            setLoading(false);
        }
    }

    async function handleRequest(ticket) {
        const confirmed = window.confirm(
            `Request ticket ${ticket.referenceNumber}?`
        );

        if (!confirmed) {
            return;
        }

        try {
            setProcessingId(ticket.id);
            setError("");
            setMessage("");

            const response = await requestTicket(
                ticket.id,
                notes[ticket.id] || ""
            );

            setMessage(
                response.message ||
                "Assignment request submitted."
            );

            setNotes((current) => ({
                ...current,
                [ticket.id]: ""
            }));

            await loadTickets();
        } catch (requestError) {
            console.error(
                "Could not request ticket:",
                requestError
            );

            setError(
                requestError.response?.data?.message ||
                "Could not request this ticket."
            );
        } finally {
            setProcessingId(null);
        }
    }

    function formatDate(date) {
        return date
            ? new Date(date).toLocaleString()
            : "—";
    }

    function priorityClass(priority) {
        switch (priority?.toLowerCase()) {
            case "critical":
                return "agent-priority critical";

            case "high":
                return "agent-priority high";

            case "medium":
                return "agent-priority medium";

            case "low":
                return "agent-priority low";

            default:
                return "agent-priority";
        }
    }

    return (
        <Layout>
            <div className="agent-ticket-page">
                <div className="agent-ticket-heading">
                    <div>
                        <h1>Available Tickets</h1>

                        <p>
                            View Open and unassigned tickets,
                            then request manager approval.
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

                {(message || error) && (
                    <div
                        className={
                            error
                                ? "agent-alert agent-error"
                                : "agent-alert agent-success"
                        }
                    >
                        {error || message}
                    </div>
                )}

                <div className="agent-summary-card">
                    <span>Available Tickets</span>
                    <strong>{tickets.length}</strong>
                </div>

                {loading ? (
                    <div className="agent-ticket-card">
                        <p>Loading available tickets...</p>
                    </div>
                ) : tickets.length === 0 ? (
                    <div className="agent-ticket-card">
                        <p>
                            No Open and unassigned tickets are
                            currently available.
                        </p>
                    </div>
                ) : (
                    <div className="agent-ticket-grid">
                        {tickets.map((ticket) => (
                            <article
                                key={ticket.id}
                                className="agent-ticket-card"
                            >
                                <div className="agent-ticket-card-header">
                                    <div>
                                        <span className="agent-reference">
                                            {
                                                ticket.referenceNumber
                                            }
                                        </span>

                                        <h2>{ticket.title}</h2>
                                    </div>

                                    <span
                                        className={priorityClass(
                                            ticket.priority
                                        )}
                                    >
                                        {ticket.priority}
                                    </span>
                                </div>

                                <p className="agent-description">
                                    {ticket.description}
                                </p>

                                <div className="agent-ticket-details">
                                    <div>
                                        <span>Category</span>
                                        <strong>
                                            {ticket.category}
                                        </strong>
                                    </div>

                                    <div>
                                        <span>Status</span>
                                        <strong>
                                            {ticket.status}
                                        </strong>
                                    </div>

                                    <div>
                                        <span>Created By</span>
                                        <strong>
                                            {ticket.createdBy}
                                        </strong>
                                    </div>

                                    <div>
                                        <span>Created</span>
                                        <strong>
                                            {formatDate(
                                                ticket.createdDate
                                            )}
                                        </strong>
                                    </div>
                                </div>

                                <div className="agent-request-area">
                                    <textarea
                                        placeholder="Optional message to the manager..."
                                        value={
                                            notes[ticket.id] || ""
                                        }
                                        onChange={(event) =>
                                            setNotes(
                                                (current) => ({
                                                    ...current,
                                                    [ticket.id]:
                                                        event.target
                                                            .value
                                                })
                                            )
                                        }
                                        disabled={
                                            ticket.hasPendingRequest
                                        }
                                    />

                                    <div className="agent-ticket-actions">
                                        <Link
                                            to={`/tickets/${ticket.id}`}
                                        >
                                            <button
                                                type="button"
                                                className="agent-secondary-button"
                                            >
                                                View Details
                                            </button>
                                        </Link>

                                        <button
                                            type="button"
                                            className="agent-primary-button"
                                            disabled={
                                                ticket.hasPendingRequest ||
                                                processingId ===
                                                    ticket.id
                                            }
                                            onClick={() =>
                                                handleRequest(
                                                    ticket
                                                )
                                            }
                                        >
                                            {ticket.hasPendingRequest
                                                ? "Request Pending"
                                                : processingId ===
                                                    ticket.id
                                                  ? "Submitting..."
                                                  : "Request Ticket"}
                                        </button>
                                    </div>
                                </div>
                            </article>
                        ))}
                    </div>
                )}
            </div>
        </Layout>
    );
}

export default AvailableTickets;