import { useNavigate } from "react-router-dom";

import TicketForm from "../components/TicketForm";

import { createTicket } from "../services/ticketService";
import Layout from "../components/Layout";


function CreateTicket() {

    const navigate = useNavigate();

    async function create(ticket) {

        await createTicket(ticket);

        alert("Ticket Created");

        navigate("/tickets");

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