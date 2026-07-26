import { Link } from "react-router-dom";

function TicketList({ tickets, onDelete }) {

    function statusClass(status){

        switch(status){

            case "Open":
                return "status status-open";

            case "Pending":
                return "status status-pending";

            case "Resolved":
                return "status status-resolved";

            case "Closed":
                return "status status-closed";

            default:
                return "status";
        }

    }

    function priorityClass(priority){

        switch(priority){

            case "High":
                return "priority-high";

            case "Medium":
                return "priority-medium";

            default:
                return "priority-low";

        }

    }

    return(

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

                    {tickets.map(ticket=>(

                        <tr key={ticket.id}>

                            <td>{ticket.referenceNumber}</td>

                            <td>{ticket.title}</td>

                            <td>{ticket.category}</td>

                            <td>

                                <span className={priorityClass(ticket.priority)}>

                                    {ticket.priority}

                                </span>

                            </td>

                            <td>

                                <span className={statusClass(ticket.status)}>

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

                                <Link to={`/tickets/edit/${ticket.id}`}>

                                    <button className="action-btn edit-btn">

                                        Edit

                                    </button>

                                </Link>

                                <button

                                    className="action-btn delete-btn"

                                    onClick={()=>onDelete(ticket.id)}

                                >

                                    Delete

                                </button>

                            </td>

                        </tr>

                    ))}

                </tbody>

            </table>

        </div>

    );

}

export default TicketList;