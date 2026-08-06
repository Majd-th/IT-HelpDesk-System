import {
    useEffect,
    useState
} from "react";

import {
    Link,
    useParams
} from "react-router-dom";

import Layout from
    "../components/Layout";

import AttachmentSection from
    "../components/AttachmentSection";

import {
    getTicketById,
    getActivityLog,
    startWork,
    resolveTicket,
    closeTicket
} from "../services/ticketService";

import "../assets/tickets.css";

function TicketDetails() {
    const { id } = useParams();

    const [ticket, setTicket] =
        useState(null);

    const [activity, setActivity] =
        useState([]);

    const [loading, setLoading] =
        useState(true);

    const [
        actionLoading,
        setActionLoading
    ] = useState(false);

    const [
        startDescription,
        setStartDescription
    ] = useState("");

    const [solution, setSolution] =
        useState("");

    const [
        workDescription,
        setWorkDescription
    ] = useState("");

    const [
        closeReason,
        setCloseReason
    ] = useState("");

    const [message, setMessage] =
        useState("");

    const [error, setError] =
        useState("");

    useEffect(() => {
        load();
    }, [id]);

    async function load() {
        try {
            setLoading(true);
            setError("");

            const [
                ticketData,
                activityData
            ] = await Promise.all([
                getTicketById(id),
                getActivityLog(id)
            ]);

            setTicket(ticketData);
            setActivity(activityData);
        } catch (requestError) {
            console.error(
                "Could not load ticket:",
                requestError
            );

            setError(
                requestError.response
                    ?.data?.message ||
                "Could not load the ticket."
            );
        } finally {
            setLoading(false);
        }
    }

    // =====================================================
    // AGENT: START WORK
    // Assigned / Reopened -> In Progress
    // =====================================================

    async function handleStartWork() {
        try {
            setActionLoading(true);
            setMessage("");
            setError("");

            const response =
                await startWork(
                    id,
                    startDescription
                );

            setStartDescription("");

            setMessage(
                response.message ||
                "Work started successfully."
            );

            await load();
        } catch (requestError) {
            console.error(
                "Start Work failed:",
                requestError
            );

            setError(
                requestError.response
                    ?.data?.message ||
                "Could not start work."
            );
        } finally {
            setActionLoading(false);
        }
    }

    // =====================================================
    // AGENT: RESOLVE TICKET
    // In Progress -> Resolved
    // =====================================================

    async function handleResolve(event) {
        event.preventDefault();

        if (
            solution.trim().length < 10
        ) {
            setError(
                "The solution must contain at least 10 characters."
            );

            return;
        }

        try {
            setActionLoading(true);
            setMessage("");
            setError("");

            const response =
                await resolveTicket(
                    id,
                    solution,
                    workDescription
                );

            setSolution("");
            setWorkDescription("");

            setMessage(
                response.message ||
                "Ticket resolved successfully."
            );

            await load();
        } catch (requestError) {
            console.error(
                "Resolve failed:",
                requestError
            );

            setError(
                requestError.response
                    ?.data?.message ||
                "Could not resolve the ticket."
            );
        } finally {
            setActionLoading(false);
        }
    }

    // =====================================================
    // EMPLOYEE OWNER / MANAGER / ADMIN: CLOSE TICKET
    // Resolved -> Closed
    // =====================================================

    async function handleCloseTicket(event) {
        event.preventDefault();

        if (
            closeReason.trim().length < 5
        ) {
            setError(
                "The closing reason must contain at least 5 characters."
            );

            return;
        }

        try {
            setActionLoading(true);
            setMessage("");
            setError("");

            const response =
                await closeTicket(
                    id,
                    closeReason
                );

            setCloseReason("");

            setMessage(
                response.message ||
                "Ticket closed successfully."
            );

            await load();
        } catch (requestError) {
            console.error(
                "Close ticket failed:",
                requestError
            );

            setError(
                requestError.response
                    ?.data?.message ||
                "Could not close the ticket."
            );
        } finally {
            setActionLoading(false);
        }
    }

    if (loading) {
        return (
            <Layout>
                <h2>
                    Loading ticket...
                </h2>
            </Layout>
        );
    }

    if (!ticket) {
        return (
            <Layout>
                <h2>
                    Ticket not found.
                </h2>

                {error && (
                    <p className="ticket-action-error">
                        {error}
                    </p>
                )}
            </Layout>
        );
    }

    // =====================================================
    // CURRENT USER
    // =====================================================

    const role =
        localStorage.getItem("role");

    const currentUserId =
        Number(
            localStorage.getItem(
                "userId"
            )
        );

    const isOwner =
        Number(
            ticket.createdByUserId
        ) === currentUserId;

    const isAssignedAgent =
        role === "IT Support Agent"
        &&
        Number(
            ticket.assignedToUserId
        ) === currentUserId;

    // =====================================================
    // PERMISSIONS
    // =====================================================

    /*
     * A Pending Review ticket can be edited by:
     * - Employee who created it
     * - Manager
     * - Admin
     */
    const canEditTicket =
        ticket.status ===
            "Pending Review"
        &&
        (
            role === "Admin"
            ||
            role === "Manager"
            ||
            (
                role === "Employee"
                &&
                isOwner
            )
        );

    /*
     * Start Work is available only to the
     * assigned Agent.
     */
    const canStartWork =
        isAssignedAgent
        &&
        (
            ticket.status ===
                "Assigned"
            ||
            ticket.status ===
                "Reopened"
        );

    /*
     * Resolve is available only to the
     * assigned Agent while In Progress.
     */
    const canResolve =
        isAssignedAgent
        &&
        ticket.status ===
            "In Progress";

    /*
     * A Resolved ticket can be closed by:
     * - Employee owner
     * - Manager
     * - Admin
     */
    const canCloseTicket =
        ticket.status ===
            "Resolved"
        &&
        (
            role === "Admin"
            ||
            role === "Manager"
            ||
            (
                role === "Employee"
                &&
                isOwner
            )
        );

    return (
        <Layout>
            <div className="ticket-details-actions">
                <Link to="/tickets">
                    ← Back to Tickets
                </Link>

                {canEditTicket && (
                    <Link
                        to={
                            `/tickets/edit/${ticket.id}`
                        }
                    >
                        <button
                            type="button"
                            className="action-btn edit-btn"
                        >
                            Edit Ticket
                        </button>
                    </Link>
                )}
            </div>

            <br />

            {(message || error) && (
                <div
                    className={
                        error
                            ? "ticket-action-message ticket-action-error"
                            : "ticket-action-message ticket-action-success"
                    }
                >
                    {error || message}
                </div>
            )}

            <div className="page-card">
                <h1 className="page-title">
                    {ticket.title}
                </h1>

                <div className="detail-grid">
                    <div className="detail-box">
                        <h4>Reference</h4>

                        <p>
                            {ticket.referenceNumber}
                        </p>
                    </div>

                    <div className="detail-box">
                        <h4>Status</h4>

                        <p>
                            {ticket.status}
                        </p>
                    </div>

                    <div className="detail-box">
                        <h4>Priority</h4>

                        <p>
                            {ticket.priority}
                        </p>
                    </div>

                    <div className="detail-box">
                        <h4>Category</h4>

                        <p>
                            {ticket.category}
                        </p>
                    </div>

                    <div className="detail-box">
                        <h4>Created By</h4>

                        <p>
                            {ticket.createdBy}
                        </p>
                    </div>

                    <div className="detail-box">
                        <h4>Assigned To</h4>

                        <p>
                            {ticket.assignedTo ||
                                "Not assigned"}
                        </p>
                    </div>

                    <div className="detail-box">
                        <h4>Created</h4>

                        <p>
                            {new Date(
                                ticket.createdDate
                            ).toLocaleString()}
                        </p>
                    </div>

                    {ticket.resolvedDate && (
                        <div className="detail-box">
                            <h4>Resolved</h4>

                            <p>
                                {new Date(
                                    ticket.resolvedDate
                                ).toLocaleString()}
                            </p>
                        </div>
                    )}

                    {ticket.closedDate && (
                        <div className="detail-box">
                            <h4>Closed</h4>

                            <p>
                                {new Date(
                                    ticket.closedDate
                                ).toLocaleString()}
                            </p>
                        </div>
                    )}

                    <div className="detail-box full-width">
                        <h4>Description</h4>

                        <p>
                            {ticket.description}
                        </p>
                    </div>

                    <div className="detail-box full-width">
                        <h4>Solution</h4>

                        <p>
                            {ticket.solution ||
                                "No solution yet."}
                        </p>
                    </div>
                </div>

                {/* =========================================
                    START WORK
                ========================================== */}

                {canStartWork && (
                    <section className="ticket-action-panel">
                        <h2>
                            Start Work
                        </h2>

                        <p>
                            Add an optional note,
                            then start the work timer.
                        </p>

                        <div className="form-group">
                            <label htmlFor="startDescription">
                                Starting Note
                            </label>

                            <textarea
                                id="startDescription"
                                value={
                                    startDescription
                                }
                                onChange={event =>
                                    setStartDescription(
                                        event.target.value
                                    )
                                }
                                placeholder="Example: Starting printer diagnostics."
                            />
                        </div>

                        <button
                            type="button"
                            className="start-work-button"
                            onClick={
                                handleStartWork
                            }
                            disabled={
                                actionLoading
                            }
                        >
                            {actionLoading
                                ? "Starting..."
                                : "Start Work"}
                        </button>
                    </section>
                )}

                {/* =========================================
                    RESOLVE TICKET
                ========================================== */}

                {canResolve && (
                    <section className="ticket-action-panel">
                        <h2>
                            Resolve Ticket
                        </h2>

                        <p>
                            Enter the final solution
                            before resolving the ticket.
                        </p>

                        <form
                            onSubmit={
                                handleResolve
                            }
                        >
                            <div className="form-group">
                                <label htmlFor="solution">
                                    Final Solution
                                </label>

                                <textarea
                                    id="solution"
                                    value={solution}
                                    onChange={event =>
                                        setSolution(
                                            event.target.value
                                        )
                                    }
                                    placeholder="Explain exactly how the issue was solved."
                                    required
                                />
                            </div>

                            <div className="form-group">
                                <label htmlFor="workDescription">
                                    Work Description
                                </label>

                                <textarea
                                    id="workDescription"
                                    value={
                                        workDescription
                                    }
                                    onChange={event =>
                                        setWorkDescription(
                                            event.target.value
                                        )
                                    }
                                    placeholder="Optional summary of the work performed."
                                />
                            </div>

                            <button
                                type="submit"
                                className="resolve-ticket-button"
                                disabled={
                                    actionLoading
                                }
                            >
                                {actionLoading
                                    ? "Resolving..."
                                    : "Resolve Ticket"}
                            </button>
                        </form>
                    </section>
                )}

                {/* =========================================
                    CLOSE TICKET
                ========================================== */}

                {canCloseTicket && (
                    <section className="ticket-action-panel">
                        <h2>
                            Close Ticket
                        </h2>

                        <p>
                            Confirm that the solution
                            worked and provide a closing
                            reason.
                        </p>

                        <form
                            onSubmit={
                                handleCloseTicket
                            }
                        >
                            <div className="form-group">
                                <label htmlFor="closeReason">
                                    Closing Reason
                                </label>

                                <textarea
                                    id="closeReason"
                                    value={
                                        closeReason
                                    }
                                    onChange={event =>
                                        setCloseReason(
                                            event.target.value
                                        )
                                    }
                                    placeholder="Example: The employee confirmed that the issue is fixed."
                                    required
                                />
                            </div>

                            <button
                                type="submit"
                                className="close-ticket-button"
                                disabled={
                                    actionLoading
                                }
                            >
                                {actionLoading
                                    ? "Closing..."
                                    : "Close Ticket"}
                            </button>
                        </form>
                    </section>
                )}

                <AttachmentSection
                    ticketId={ticket.id}
                />

                <div className="timeline">
                    <h2>
                        Activity Timeline
                    </h2>

                    {activity.length === 0 && (
                        <p>
                            No activity recorded.
                        </p>
                    )}

                    {activity.map(
                        (log, index) => (
                            <div
                                key={
                                    `${log.createdDate}-${index}`
                                }
                                className="timeline-item"
                            >
                                <strong>
                                    {log.action}
                                </strong>

                                <br />

                                <span>
                                    {log.user}
                                </span>

                                {log.description && (
                                    <>
                                        <br />

                                        <span>
                                            {log.description}
                                        </span>
                                    </>
                                )}

                                <br />

                                <small>
                                    {new Date(
                                        log.createdDate
                                    ).toLocaleString()}
                                </small>
                            </div>
                        )
                    )}
                </div>
            </div>
        </Layout>
    );
}

export default TicketDetails;