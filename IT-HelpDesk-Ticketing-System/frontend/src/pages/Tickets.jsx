import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

import Layout from "../components/Layout";
import TicketFilter from "../components/TicketFilter";
import TicketList from "../components/TicketList";

import {
    getTickets,
    deleteTicket,
    filterTickets
} from "../services/ticketService";

import "../assets/tickets.css";

function Tickets() {

    const [tickets,setTickets]=useState([]);

    useEffect(()=>{
        loadTickets();
    },[]);

    async function loadTickets(){
        const data=await getTickets();
        setTickets(data);
    }

    async function handleDelete(id){

        if(!window.confirm("Delete this ticket?"))
            return;

        await deleteTicket(id);

        loadTickets();
    }

    async function handleFilter(filters){

        const data=await filterTickets(filters);

        setTickets(data);
    }

    return(

        <Layout>

            <div className="ticket-page">

                <div className="ticket-header">

                    <h1>Tickets</h1>

                    <Link to="/tickets/new">

                        <button className="create-btn">

                            + Create Ticket

                        </button>

                    </Link>

                </div>

                <TicketFilter
                    onFilter={handleFilter}
                />

                <br/>

                <TicketList
                    tickets={tickets}
                    onDelete={handleDelete}
                />

            </div>

        </Layout>

    );

}

export default Tickets;