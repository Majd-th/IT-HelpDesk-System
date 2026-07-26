    import { useEffect, useState } from "react";
    import { useNavigate, useParams } from "react-router-dom";
import AttachmentSection from "../components/AttachmentSection";
    import TicketForm from "../components/TicketForm";
import Layout from "../components/Layout";
    import {
        getTicketById,
        updateTicket
    } from "../services/ticketService";

    function EditTicket() {

        const { id } = useParams();

        const navigate = useNavigate();

        const [ticket, setTicket] = useState(null);

        useEffect(() => {

            loadTicket();

        }, []);

        async function loadTicket() {

            const data = await getTicketById(id);

            setTicket(data);
        }

        async function handleUpdate(updatedTicket) {

            await updateTicket(id, updatedTicket);

            alert("Ticket updated!");

            navigate("/tickets");
        }

        if (!ticket)
            return <h2>Loading...</h2>;
return (
    <Layout>
        <h1>Edit Ticket</h1>

        <TicketForm
            initialValues={ticket}
            onSubmit={handleUpdate}
        />

        <hr />

        <AttachmentSection ticketId={ticket.id} />
    </Layout>
);
    }

    export default EditTicket;