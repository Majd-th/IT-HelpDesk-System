import {
    useEffect,
    useState
} from "react";

import Layout from
    "../components/Layout";

import {
    getUnassignedTickets,
    getAgentWorkloads,
    getPendingRequests,
    assignTicket,
    reassignTicket,
    reviewAssignmentRequest,
    getAssignmentHistory,
    publishTicket
} from "../services/assignmentService";

import "../assets/assignment-management.css";

function AssignmentManagement() {
    const [activeSection, setActiveSection] =
        useState("unassigned");

    const [tickets, setTickets] = useState([]);
    const [workloads, setWorkloads] = useState([]);
    const [requests, setRequests] = useState([]);
    const [history, setHistory] = useState([]);

    const [selectedTicket, setSelectedTicket] =
        useState(null);

    const [selectedAgentId, setSelectedAgentId] =
        useState("");

    const [assignmentNotes, setAssignmentNotes] =
        useState("");

    const [reassignTicketId, setReassignTicketId] =
        useState("");

    const [reassignAgentId, setReassignAgentId] =
        useState("");

    const [reassignNotes, setReassignNotes] =
        useState("");

    const [historyTicketId, setHistoryTicketId] =
        useState("");

    const [reviewNotes, setReviewNotes] =
        useState({});

    const [loading, setLoading] = useState(true);
    const [processing, setProcessing] =
        useState(false);

    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    useEffect(() => {
        loadAssignmentData();
    }, []);

    async function loadAssignmentData() {
        try {
            setLoading(true);
            setError("");

            const [
                ticketData,
                workloadData,
                requestData
            ] = await Promise.all([
                getUnassignedTickets(),
                getAgentWorkloads(),
                getPendingRequests()
            ]);

            setTickets(ticketData);
            setWorkloads(workloadData);
            setRequests(requestData);
        } catch (requestError) {
            console.error(
                "Could not load assignment data:",
                requestError
            );

            setError(
                requestError.response?.data?.message ||
                "Could not load assignment information."
            );
        } finally {
            setLoading(false);
        }
    }
    async function handlePublish(ticket) {
    const notes =
        window.prompt(
            `Why are you publishing ${ticket.referenceNumber}?`
        );

    if (notes === null) {
        return;
    }

    if (notes.trim().length < 5) {
        alert(
            "Publishing notes must contain at least 5 characters."
        );

        return;
    }

    const confirmed =
        window.confirm(
            `Publish ${ticket.referenceNumber} for IT Agents to request?`
        );

    if (!confirmed) {
        return;
    }

    try {
        setProcessing(true);
        setError("");
        setMessage("");

        const response =
            await publishTicket(
                ticket.id,
                notes
            );

        setMessage(
            response.message ||
            "Ticket published successfully."
        );

        /*
         * Reload the information without refreshing
         * the entire browser page.
         */
        await loadAssignmentData();
    } catch (requestError) {
        console.error(
            "Publish failed:",
            requestError
        );

        setError(
            requestError.response?.data?.message ||
            "Could not publish the ticket."
        );
    } finally {
        setProcessing(false);
    }
}

    function openAssignForm(ticket) {
        setSelectedTicket(ticket);
        setSelectedAgentId("");
        setAssignmentNotes("");
        setMessage("");
        setError("");
    }

    function closeAssignForm() {
        setSelectedTicket(null);
        setSelectedAgentId("");
        setAssignmentNotes("");
    }

    async function handleAssign(event) {
        event.preventDefault();

        if (!selectedTicket) {
            setError("Select a ticket.");
            return;
        }

        if (!selectedAgentId) {
            setError("Select an IT Support Agent.");
            return;
        }

        try {
            setProcessing(true);
            setError("");
            setMessage("");

            const response = await assignTicket(
                selectedTicket.id,
                selectedAgentId,
                assignmentNotes
            );

            setMessage(
                response.message ||
                "Ticket assigned successfully."
            );

            closeAssignForm();
            await loadAssignmentData();
        } catch (requestError) {
            console.error(
                "Assignment failed:",
                requestError
            );

            setError(
                requestError.response?.data?.message ||
                "Could not assign the ticket."
            );
        } finally {
            setProcessing(false);
        }
    }

    async function handleReassign(event) {
        event.preventDefault();

        if (!reassignTicketId.trim()) {
            setError("Enter the ticket ID.");
            return;
        }

        if (!reassignAgentId) {
            setError("Select the new agent.");
            return;
        }

        try {
            setProcessing(true);
            setError("");
            setMessage("");

            const response = await reassignTicket(
                reassignTicketId,
                reassignAgentId,
                reassignNotes
            );

            setMessage(
                response.message ||
                "Ticket reassigned successfully."
            );

            setReassignTicketId("");
            setReassignAgentId("");
            setReassignNotes("");

            await loadAssignmentData();
        } catch (requestError) {
            console.error(
                "Reassignment failed:",
                requestError
            );

            setError(
                requestError.response?.data?.message ||
                "Could not reassign the ticket."
            );
        } finally {
            setProcessing(false);
        }
    }

    async function handleReview(
        assignmentId,
        approved
    ) {
        const action =
            approved ? "approve" : "reject";

        const confirmed = window.confirm(
            `Are you sure you want to ${action} this request?`
        );

        if (!confirmed) {
            return;
        }

        try {
            setProcessing(true);
            setError("");
            setMessage("");

            const response =
                await reviewAssignmentRequest(
                    assignmentId,
                    approved,
                    reviewNotes[assignmentId] || ""
                );

            setMessage(
                response.message ||
                `Request ${approved ? "approved" : "rejected"}.`
            );

            setReviewNotes((current) => {
                const updated = { ...current };

                delete updated[assignmentId];

                return updated;
            });

            await loadAssignmentData();
        } catch (requestError) {
            console.error(
                "Request review failed:",
                requestError
            );

            setError(
                requestError.response?.data?.message ||
                "Could not review the request."
            );
        } finally {
            setProcessing(false);
        }
    }

    async function handleLoadHistory(event) {
        event.preventDefault();

        if (!historyTicketId.trim()) {
            setError("Enter a ticket ID.");
            return;
        }

        try {
            setProcessing(true);
            setError("");
            setMessage("");

            const data = await getAssignmentHistory(
                historyTicketId
            );

            setHistory(data);
        } catch (requestError) {
            console.error(
                "Could not load assignment history:",
                requestError
            );

            setError(
                requestError.response?.data?.message ||
                "Could not load assignment history."
            );
        } finally {
            setProcessing(false);
        }
    }

    function formatDate(date) {
        if (!date) {
            return "—";
        }

        return new Date(date).toLocaleString();
    }

    if (loading) {
        return (
            <Layout>
                <p>Loading assignment management...</p>
            </Layout>
        );
    }

    return (
        <Layout>
            <div className="assignment-page">
                <div className="assignment-heading">
                    <div>
                        <h1>Assignment Management</h1>

                        <p>
                            Assign tickets, review requests,
                            and monitor agent workload.
                        </p>
                    </div>

                    <button
                        type="button"
                        className="assignment-refresh-button"
                        onClick={loadAssignmentData}
                    >
                        Refresh
                    </button>
                </div>

                {(message || error) && (
                    <div
                        className={
                            error
                                ? "assignment-alert assignment-error"
                                : "assignment-alert assignment-success"
                        }
                    >
                        {error || message}
                    </div>
                )}

                <div className="assignment-summary-grid">
                    <div className="assignment-summary-card">
                        <span>Unassigned Tickets</span>
                        <strong>{tickets.length}</strong>
                    </div>

                    <div className="assignment-summary-card">
                        <span>Active Agents</span>
                        <strong>{workloads.length}</strong>
                    </div>

                    <div className="assignment-summary-card">
                        <span>Pending Requests</span>
                        <strong>{requests.length}</strong>
                    </div>

                    <div className="assignment-summary-card">
                        <span>Fully Loaded Agents</span>

                        <strong>
                            {
                                workloads.filter(
                                    (agent) =>
                                        agent.isFullyLoaded
                                ).length
                            }
                        </strong>
                    </div>
                </div>

                <div className="assignment-tabs">
                    <button
                        type="button"
                        className={
                            activeSection === "unassigned"
                                ? "assignment-tab active"
                                : "assignment-tab"
                        }
                        onClick={() =>
                            setActiveSection("unassigned")
                        }
                    >
                        Unassigned Tickets
                    </button>

                    <button
                        type="button"
                        className={
                            activeSection === "workload"
                                ? "assignment-tab active"
                                : "assignment-tab"
                        }
                        onClick={() =>
                            setActiveSection("workload")
                        }
                    >
                        Agent Workload
                    </button>

                    <button
                        type="button"
                        className={
                            activeSection === "requests"
                                ? "assignment-tab active"
                                : "assignment-tab"
                        }
                        onClick={() =>
                            setActiveSection("requests")
                        }
                    >
                        Pending Requests
                    </button>

                    <button
                        type="button"
                        className={
                            activeSection === "reassign"
                                ? "assignment-tab active"
                                : "assignment-tab"
                        }
                        onClick={() =>
                            setActiveSection("reassign")
                        }
                    >
                        Reassign Ticket
                    </button>

                    <button
                        type="button"
                        className={
                            activeSection === "history"
                                ? "assignment-tab active"
                                : "assignment-tab"
                        }
                        onClick={() =>
                            setActiveSection("history")
                        }
                    >
                        History
                    </button>
                </div>

                {activeSection === "unassigned" && (
                    <section className="assignment-card">
                        <div className="assignment-section-heading">
                            <div>
                                <h2>Unassigned  Tickets</h2>
<p>
    Review Pending Review tickets or assign available
    tickets to agents.
</p>
                            </div>
                        </div>

                        {tickets.length === 0 ? (
                            <p>No unassigned tickets found.</p>
                        ) : (
                            <div className="assignment-table-wrapper">
                                <table className="assignment-table">
                                    <thead>
                                        <tr>
                                            <th>Reference</th>
                                            <th>Title</th>
                                            <th>Category</th>
                                            <th>Priority</th>
                                            <th>Created By</th>
                                            <th>Created</th>
                                            <th>Action</th>
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
                                                <td>{ticket.createdBy}</td>

                                                <td>
                                                    {formatDate(
                                                        ticket.createdDate
                                                    )}
                                                </td>

                                               <td className="assignment-actions">
    <button
        type="button"
        className="assignment-button"
        onClick={() =>
            openAssignForm(ticket)
        }
        disabled={processing}
    >
        Assign
    </button>

    {ticket.status === "Pending Review" && (
        <button
            type="button"
            className="publish-button"
            onClick={() =>
                handlePublish(ticket)
            }
            disabled={processing}
        >
            Publish
        </button>
    )}

                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        )}
                    </section>
                )}

                {activeSection === "workload" && (
                    <section className="assignment-card">
                        <div className="assignment-section-heading">
                            <div>
                                <h2>Agent Workload</h2>

                                <p>
                                    Agents with fewer active tickets
                                    appear first.
                                </p>
                            </div>
                        </div>

                        {workloads.length === 0 ? (
                            <p> No tickets are currently awaiting assignment.</p>
                        ) : (
                            <div className="assignment-workload-grid">
                                {workloads.map((agent) => (
                                    <div
                                        key={agent.agentId}
                                        className="assignment-agent-card"
                                    >
                                        <div>
                                            <h3>{agent.fullName}</h3>
                                            <p>{agent.email}</p>
                                        </div>

                                        <div className="assignment-agent-count">
                                            <strong>
                                                {
                                                    agent.activeTicketCount
                                                }
                                            </strong>

                                            <span>
                                                Active tickets
                                            </span>
                                        </div>

                                        <span
                                            className={
                                                agent.isFullyLoaded
                                                    ? "assignment-load-status full"
                                                    : "assignment-load-status available"
                                            }
                                        >
                                            {agent.isFullyLoaded
                                                ? "Fully loaded"
                                                : "Available"}
                                        </span>
                                    </div>
                                ))}
                            </div>
                        )}
                    </section>
                )}

                {activeSection === "requests" && (
                    <section className="assignment-card">
                        <div className="assignment-section-heading">
                            <div>
                                <h2>Pending Agent Requests</h2>

                                <p>
                                    Approve or reject agents who
                                    requested an available ticket.
                                </p>
                            </div>
                        </div>

                        {requests.length === 0 ? (
                            <p>No pending requests found.</p>
                        ) : (
                            <div className="assignment-request-list">
                                {requests.map((request) => (
                                    <article
                                        key={request.id}
                                        className="assignment-request-card"
                                    >
                                        <div className="assignment-request-content">
                                            <h3>
                                                {
                                                    request.ticketReference
                                                }{" "}
                                                — {request.ticketTitle}
                                            </h3>

                                            <p>
                                                Requested by:{" "}
                                                <strong>
                                                    {
                                                        request.assignedToUser
                                                    }
                                                </strong>
                                            </p>

                                            <p>
                                                Requested:{" "}
                                                {formatDate(
                                                    request.assignedDate
                                                )}
                                            </p>

                                            {request.notes && (
                                                <p>
                                                    Notes:{" "}
                                                    {request.notes}
                                                </p>
                                            )}
                                        </div>

                                        <textarea
                                            placeholder="Manager review notes..."
                                            value={
                                                reviewNotes[
                                                    request.id
                                                ] || ""
                                            }
                                            onChange={(event) =>
                                                setReviewNotes(
                                                    (current) => ({
                                                        ...current,
                                                        [request.id]:
                                                            event
                                                                .target
                                                                .value
                                                    })
                                                )
                                            }
                                        />

                                        <div className="assignment-request-actions">
                                            <button
                                                type="button"
                                                className="assignment-approve-button"
                                                disabled={processing}
                                                onClick={() =>
                                                    handleReview(
                                                        request.id,
                                                        true
                                                    )
                                                }
                                            >
                                                Approve
                                            </button>

                                            <button
                                                type="button"
                                                className="assignment-reject-button"
                                                disabled={processing}
                                                onClick={() =>
                                                    handleReview(
                                                        request.id,
                                                        false
                                                    )
                                                }
                                            >
                                                Reject
                                            </button>
                                        </div>
                                    </article>
                                ))}
                            </div>
                        )}
                    </section>
                )}

                {activeSection === "reassign" && (
                    <section className="assignment-card">
                        <div className="assignment-section-heading">
                            <div>
                                <h2>Reassign Ticket</h2>

                                <p>
                                    Move an active ticket to another
                                    IT Support Agent.
                                </p>
                            </div>
                        </div>

                        <form
                            className="assignment-form"
                            onSubmit={handleReassign}
                        >
                            <div className="assignment-form-group">
                                <label htmlFor="reassignTicketId">
                                    Ticket ID
                                </label>

                                <input
                                    id="reassignTicketId"
                                    type="number"
                                    min="1"
                                    value={reassignTicketId}
                                    onChange={(event) =>
                                        setReassignTicketId(
                                            event.target.value
                                        )
                                    }
                                />
                            </div>

                            <div className="assignment-form-group">
                                <label htmlFor="reassignAgentId">
                                    New agent
                                </label>

                                <select
                                    id="reassignAgentId"
                                    value={reassignAgentId}
                                    onChange={(event) =>
                                        setReassignAgentId(
                                            event.target.value
                                        )
                                    }
                                >
                                    <option value="">
                                        Select agent
                                    </option>

                                    {workloads.map((agent) => (
                                        <option
                                            key={agent.agentId}
                                            value={agent.agentId}
                                            disabled={
                                                agent.isFullyLoaded
                                            }
                                        >
                                            {agent.fullName} —{" "}
                                            {
                                                agent.activeTicketCount
                                            }{" "}
                                            active
                                            {agent.isFullyLoaded
                                                ? " — Fully loaded"
                                                : ""}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div className="assignment-form-group full-width">
                                <label htmlFor="reassignNotes">
                                    Notes
                                </label>

                                <textarea
                                    id="reassignNotes"
                                    value={reassignNotes}
                                    onChange={(event) =>
                                        setReassignNotes(
                                            event.target.value
                                        )
                                    }
                                    placeholder="Reason for reassignment..."
                                />
                            </div>

                            <button
                                type="submit"
                                className="assignment-primary-button"
                                disabled={processing}
                            >
                                {processing
                                    ? "Processing..."
                                    : "Reassign Ticket"}
                            </button>
                        </form>
                    </section>
                )}

                {activeSection === "history" && (
                    <section className="assignment-card">
                        <div className="assignment-section-heading">
                            <div>
                                <h2>Assignment History</h2>

                                <p>
                                    Search assignment history by
                                    ticket ID.
                                </p>
                            </div>
                        </div>

                        <form
                            className="assignment-history-search"
                            onSubmit={handleLoadHistory}
                        >
                            <input
                                type="number"
                                min="1"
                                placeholder="Enter ticket ID"
                                value={historyTicketId}
                                onChange={(event) =>
                                    setHistoryTicketId(
                                        event.target.value
                                    )
                                }
                            />

                            <button
                                type="submit"
                                disabled={processing}
                            >
                                Load History
                            </button>
                        </form>

                        {history.length > 0 && (
                            <div className="assignment-table-wrapper">
                                <table className="assignment-table">
                                    <thead>
                                        <tr>
                                            <th>Agent</th>
                                            <th>Assigned By</th>
                                            <th>Type</th>
                                            <th>Approval</th>
                                            <th>Assigned</th>
                                            <th>Unassigned</th>
                                            <th>Active</th>
                                            <th>Notes</th>
                                        </tr>
                                    </thead>

                                    <tbody>
                                        {history.map((item) => (
                                            <tr key={item.id}>
                                                <td>
                                                    {
                                                        item.assignedToUser
                                                    }
                                                </td>

                                                <td>
                                                    {
                                                        item.assignedByUser
                                                    }
                                                </td>

                                                <td>
                                                    {
                                                        item.assignmentType
                                                    }
                                                </td>

                                                <td>
                                                    {
                                                        item.approvalStatus
                                                    }
                                                </td>

                                                <td>
                                                    {formatDate(
                                                        item.assignedDate
                                                    )}
                                                </td>

                                                <td>
                                                    {formatDate(
                                                        item.unassignedDate
                                                    )}
                                                </td>

                                                <td>
                                                    {item.isActive
                                                        ? "Yes"
                                                        : "No"}
                                                </td>

                                                <td>
                                                    {item.notes || "—"}
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        )}
                    </section>
                )}

                {selectedTicket && (
                    <div
                        className="assignment-modal-backdrop"
                        onClick={closeAssignForm}
                    >
                        <div
                            className="assignment-modal"
                            onClick={(event) =>
                                event.stopPropagation()
                            }
                        >
                            <div className="assignment-modal-header">
                                <div>
                                    <h2>Assign Ticket</h2>

                                    <p>
                                        {
                                            selectedTicket.referenceNumber
                                        }{" "}
                                        — {selectedTicket.title}
                                    </p>
                                </div>

                                <button
                                    type="button"
                                    onClick={closeAssignForm}
                                >
                                    ×
                                </button>
                            </div>

                            <form onSubmit={handleAssign}>
                                <div className="assignment-form-group">
                                    <label htmlFor="assignmentAgent">
                                        IT Support Agent
                                    </label>

                                    <select
                                        id="assignmentAgent"
                                        value={selectedAgentId}
                                        onChange={(event) =>
                                            setSelectedAgentId(
                                                event.target.value
                                            )
                                        }
                                    >
                                        <option value="">
                                            Select agent
                                        </option>

                                        {workloads.map((agent) => (
                                            <option
                                                key={agent.agentId}
                                                value={agent.agentId}
                                                disabled={
                                                    agent.isFullyLoaded
                                                }
                                            >
                                                {agent.fullName} —{" "}
                                                {
                                                    agent.activeTicketCount
                                                }{" "}
                                                active
                                                {agent.isFullyLoaded
                                                    ? " — Fully loaded"
                                                    : ""}
                                            </option>
                                        ))}
                                    </select>
                                </div>

                                <div className="assignment-form-group">
                                    <label htmlFor="assignmentNotes">
                                        Assignment notes
                                    </label>

                                    <textarea
                                        id="assignmentNotes"
                                        value={assignmentNotes}
                                        onChange={(event) =>
                                            setAssignmentNotes(
                                                event.target.value
                                            )
                                        }
                                    />
                                </div>

                                <div className="assignment-modal-actions">
                                    <button
                                        type="button"
                                        className="assignment-secondary-button"
                                        onClick={closeAssignForm}
                                    >
                                        Cancel
                                    </button>

                                    <button
                                        type="submit"
                                        className="assignment-primary-button"
                                        disabled={processing}
                                    >
                                        {processing
                                            ? "Assigning..."
                                            : "Assign Ticket"}
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                )}
            </div>
        </Layout>
    );
}

export default AssignmentManagement;