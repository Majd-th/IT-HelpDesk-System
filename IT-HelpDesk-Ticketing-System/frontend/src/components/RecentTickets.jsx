import { Link } from "react-router-dom";

function RecentTickets({tickets=[]}){

    return(

        <div className="dashboard-card">

            <h2>

                Recent Tickets

            </h2>

            <table>

                <thead>

                    <tr>

                        <th>Reference</th>

                        <th>Title</th>

                        <th>Status</th>

                        <th></th>

                    </tr>

                </thead>

                <tbody>

                    {

                        tickets.slice(0,5).map(ticket=>(

                            <tr key={ticket.id}>

                                <td>

                                    {ticket.referenceNumber}

                                </td>

                                <td>

                                    {ticket.title}

                                </td>

                                <td>

                                    {ticket.status}

                                </td>

                                <td>

                                    <Link to={`/tickets/${ticket.id}`}>

                                        View

                                    </Link>

                                </td>

                            </tr>

                        ))

                    }

                </tbody>

            </table>

        </div>

    );

}

export default RecentTickets;