import { useEffect,useState } from "react";
import { useParams,Link } from "react-router-dom";

import Layout from "../components/Layout";
import AttachmentSection from "../components/AttachmentSection";

import {

    getTicketById,
    getActivityLog

} from "../services/ticketService";

import "../assets/tickets.css";

function TicketDetails(){

    const {id}=useParams();

    const [ticket,setTicket]=useState(null);

    const [activity,setActivity]=useState([]);

    useEffect(()=>{

        load();

    },[id]);

    async function load(){

        const t=await getTicketById(id);

        setTicket(t);

        const logs=await getActivityLog(id);

        setActivity(logs);

    }

    if(!ticket)

        return <h2>Loading...</h2>;

    return(

        <Layout>

            <Link to="/tickets">

                ← Back to Tickets

            </Link>

            <br/><br/>

            <div className="page-card">

                <h1 className="page-title">

                    {ticket.title}

                </h1>

                <div className="detail-grid">

                    <div className="detail-box">

                        <h4>Reference</h4>

                        {ticket.referenceNumber}

                    </div>

                    <div className="detail-box">

                        <h4>Status</h4>

                        {ticket.status}

                    </div>

                    <div className="detail-box">

                        <h4>Priority</h4>

                        {ticket.priority}

                    </div>

                    <div className="detail-box">

                        <h4>Category</h4>

                        {ticket.category}

                    </div>

                    <div className="detail-box">

                        <h4>Created By</h4>

                        {ticket.createdBy}

                    </div>

                    <div className="detail-box">

                        <h4>Created</h4>

                        {ticket.createdDate}

                    </div>

                    <div className="detail-box full-width">

                        <h4>Description</h4>

                        {ticket.description}

                    </div>

                    <div className="detail-box full-width">

                        <h4>Solution</h4>

                        {ticket.solution || "No solution yet."}

                    </div>

                </div>

                <br/>

                <AttachmentSection

                    ticketId={ticket.id}

                />

                <div className="timeline">

                    <h2>

                        Activity Timeline

                    </h2>

                    {

                        activity.map(log=>(

                            <div

                                key={log.createdDate}

                                className="timeline-item"

                            >

                                <strong>

                                    {log.action}

                                </strong>

                                <br/>

                                {log.user}

                                <br/>

                                <small>

                                    {log.createdDate}

                                </small>

                            </div>

                        ))

                    }

                </div>

            </div>

        </Layout>

    );

}

export default TicketDetails;