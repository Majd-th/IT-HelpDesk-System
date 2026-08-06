import {
    useNavigate
} from "react-router-dom";

import TicketForm from
    "../components/TicketForm";

import Layout from
    "../components/Layout";

import {
    createTicket
} from "../services/ticketService";

function CreateTicket() {
    const navigate = useNavigate();

    async function create(ticket) {
        try {
            await createTicket(ticket);

            alert(
                "Ticket created successfully."
            );

            navigate("/tickets");
        } catch (error) {
            console.error(
                "Ticket creation failed:",
                error
            );

            alert(
                error.response?.data?.message ||
                "Could not create the ticket."
            );
        }
    }

    return (
        <Layout>
            <h1>Create Ticket</h1>

            <TicketForm
                onSubmit={create}
            />
        </Layout>
    );
}

export default CreateTicket;