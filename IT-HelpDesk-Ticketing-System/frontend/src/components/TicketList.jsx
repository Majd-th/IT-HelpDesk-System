import { Link } from "react-router-dom";

function TicketList({
    tickets,
    onDelete
}) {
    const role =
        localStorage.getItem("role");

    const currentUserId =
        Number(
            localStorage.getItem("userId")
        );

    function getStatusClass(status) {
        const statusClasses = {
            "Pending Review":
                "status-pending-review",

            "Open":
                "status-open",

            "Assigned":
                "status-assigned",

            "In Progress":
                "status-in-progress",

            "Escalated":
                "status-escalated",

            "Rejected":
                "status-rejected",

            "Canceled":
                "status-canceled",

            "Resolved":
                "status-resolved",

            "Closed":
                "status-closed",

            "Reopened":
                "status-reopened"
        };

        return (
            statusClasses[status] ||
            "status-default"
        );
    }

    function getPriorityClass(priority) {
        const priorityClasses = {
            Critical:
                "priority-critical",

            High:
                "priority-high",

            Medium:
                "priority-medium",

            Low:
                "priority-low"
        };

        return (
            priorityClasses[priority] ||
            ""
        );
    }

    if (
        !tickets ||
        tickets.length === 0
    ) {
        return (
            <div className="ticket-card">
                <p>No tickets found.</p>
            </div>
        );
    }

    return (
        <div className="ticket-card">
            <table className="ticket-table">
                <thead>
                    <tr>
                        <th>Reference</th>
                        <th>Title</th>
                        <th>Category</th>
                        <th>Priority</th>
                        <th>Status</th>
                        <th>Created By</th>
                        <th>Actions</th>
                    </tr>
                </thead>

                <tbody>
                    {tickets.map(ticket => {
                        const isOwner =
                            Number(
                                ticket.createdByUserId
                            ) === currentUserId;

                        const isPendingReview =
                            ticket.status ===
                            "Pending Review";

                        /*
                         * Employee:
                         * Can edit only their own
                         * Pending Review ticket.
                         *
                         * Manager/Admin:
                         * Can edit Pending Review tickets.
                         *
                         * Agent:
                         * Does not use the normal
                         * edit page.
                         */
                        const canEdit =
                            (
                                role === "Employee"
                                &&
                                isOwner
                                &&
                                isPendingReview
                            )
                            ||
                            (
                                (
                                    role === "Manager"
                                    ||
                                    role === "Admin"
                                )
                                &&
                                isPendingReview
                            );

                        /*
                         * Only Admin permanently deletes.
                         */
                        const canDelete =
                            role === "Admin";

                        return (
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
                                    <span
                                        className={
                                            getPriorityClass(
                                                ticket.priority
                                            )
                                        }
                                    >
                                        {
                                            ticket.priority
                                        }
                                    </span>
                                </td>

                                <td>
                                    <span
                                        className={
                                            `status-badge ${getStatusClass(
                                                ticket.status
                                            )}`
                                        }
                                    >
                                        {
                                            ticket.status
                                        }
                                    </span>
                                </td>

                                <td>
                                    {ticket.createdBy}
                                </td>

                                <td>
                                    <Link
                                        to={
                                            `/tickets/${ticket.id}`
                                        }
                                    >
                                        <button
                                            type="button"
                                            className="action-btn view-btn"
                                        >
                                            View
                                        </button>
                                    </Link>

                                    {canEdit && (
                                        <Link
                                            to={
                                                `/tickets/edit/${ticket.id}`
                                            }
                                        >
                                            <button
                                                type="button"
                                                className="action-btn edit-btn"
                                            >
                                                Edit
                                            </button>
                                        </Link>
                                    )}

                                    {canDelete && (
                                        <button
                                            type="button"
                                            className="action-btn delete-btn"
                                            onClick={() =>
                                                onDelete(
                                                    ticket.id
                                                )
                                            }
                                        >
                                            Delete
                                        </button>
                                    )}
                                </td>
                            </tr>
                        );
                    })}
                </tbody>
            </table>
        </div>
    );
}

export default TicketList;