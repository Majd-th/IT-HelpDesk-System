import { Link } from "react-router-dom";

function TicketList({ tickets, onDelete }) {
    const role = localStorage.getItem("role");
    const currentUserId = Number(localStorage.getItem("userId"));

    function statusClass(status) {
    switch (status?.toLowerCase()) {
        case "open":
            return "status status-open";

        case "in progress":
            return "status status-in-progress";

        case "pending":
            return "status status-pending";

        case "resolved":
            return "status status-resolved";

        case "closed":
            return "status status-closed";

        default:
            return "status";
    }
}

    function priorityClass(priority) {
        switch (priority) {
            case "Critical":
                return "priority-critical";

            case "High":
                return "priority-high";

            case "Medium":
                return "priority-medium";

            case "Low":
                return "priority-low";

            default:
                return "";
        }
    }

    if (!tickets || tickets.length === 0) {
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
                    {tickets.map((ticket) => {
                        const isOwner =
                            ticket.createdByUserId === currentUserId;
const isOpen =
    ticket.status?.toLowerCase() === "open";

                      const canEdit =
    role === "Admin" ||
    role === "IT Support Agent" ||
    (
        role === "Employee" &&
        isOwner &&
        isOpen
    );

const canDelete =
    role === "Admin" ||
    (
        role === "Employee" &&
        isOwner &&
        isOpen
    );

                        return (
                            <tr key={ticket.id}>
                                <td>{ticket.referenceNumber}</td>

                                <td>{ticket.title}</td>

                                <td>{ticket.category}</td>

                                <td>
                                    <span
                                        className={priorityClass(
                                            ticket.priority
                                        )}
                                    >
                                        {ticket.priority}
                                    </span>
                                </td>

                                <td>
                                    <span
                                        className={statusClass(
                                            ticket.status
                                        )}
                                    >
                                        {ticket.status}
                                    </span>
                                </td>

                                <td>{ticket.createdBy}</td>

                                <td>
                                    <Link to={`/tickets/${ticket.id}`}>
                                        <button className="action-btn view-btn">
                                            View
                                        </button>
                                    </Link>

                                    {canEdit && (
                                        <Link
                                            to={`/tickets/edit/${ticket.id}`}
                                        >
                                            <button className="action-btn edit-btn">
                                                Edit
                                            </button>
                                        </Link>
                                    )}

                                    {canDelete && (
                                        <button
                                            className="action-btn delete-btn"
                                            onClick={() =>
                                                onDelete(ticket.id)
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