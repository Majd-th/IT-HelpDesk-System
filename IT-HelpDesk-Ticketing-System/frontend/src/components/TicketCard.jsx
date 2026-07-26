import AttachmentSection from "./AttachmentSection";

function TicketCard({

    ticket,

    role,

    onDelete

}){

    return(

        <div
            style={{
                border:"1px solid gray",
                marginBottom:15,
                padding:15
            }}
        >

            <h3>{ticket.title}</h3>

            <p>{ticket.description}</p>

            <p>{ticket.category}</p>

            <p>{ticket.priority}</p>

            <p>{ticket.status}</p>

            <p>{ticket.createdBy}</p>

            {(role==="Admin" || role==="Manager") && (

                <button
                    onClick={()=>onDelete(ticket.id)}
                >
                    Delete
                </button>

            )}

            <AttachmentSection
                ticketId={ticket.id}
            />

        </div>
    );
}

export default TicketCard;