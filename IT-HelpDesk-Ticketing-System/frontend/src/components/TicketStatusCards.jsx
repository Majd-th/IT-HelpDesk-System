import DashboardCard from
    "./DashboardCard";

const statusCards = [
    {
        status: "Pending Review",
        title: "Pending Review",
        color: "#f59e0b"
    },
    {
        status: "Open",
        title: "Open",
        color: "#3498db"
    },
    {
        status: "Assigned",
        title: "Assigned",
        color: "#8b5cf6"
    },
    {
        status: "In Progress",
        title: "In Progress",
        color: "#0ea5e9"
    },
    {
        status: "Resolved",
        title: "Resolved",
        color: "#2ecc71"
    },
    {
        status: "Closed",
        title: "Closed",
        color: "#64748b"
    }
];

function TicketStatusCards({
    tickets
}) {
    return (
        <div className="dashboard-cards">
            {statusCards.map(card => (
                <DashboardCard
                    key={card.status}
                    title={card.title}
                    value={
                        tickets.filter(
                            ticket =>
                                ticket.status ===
                                card.status
                        ).length
                    }
                    color={card.color}
                />
            ))}
        </div>
    );
}

export default TicketStatusCards;