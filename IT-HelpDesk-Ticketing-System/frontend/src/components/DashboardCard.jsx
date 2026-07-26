function DashboardCard({

    title,
    value,
    color

}){

    return(

        <div
            className="dashboard-card"
            style={{borderLeft:`6px solid ${color}`}}
        >

            <h4>{title}</h4>

            <h1>{value}</h1>

        </div>

    );

}

export default DashboardCard;