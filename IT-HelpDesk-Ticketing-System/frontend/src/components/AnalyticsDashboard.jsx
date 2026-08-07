import {
    useState
} from "react";

import {
    useQuery
} from "@tanstack/react-query";

import {
    Bar,
    BarChart,
    CartesianGrid,
    Legend,
    Line,
    LineChart,
    ResponsiveContainer,
    Tooltip,
    XAxis,
    YAxis
} from "recharts";

import DashboardCard from
    "./DashboardCard";

import {
    getDashboardAnalytics
} from "../services/dashboardService";

import "../styles/AnalyticsDashboard.css";

function formatDateForInput(date) {
    const year =
        date.getFullYear();

    const month =
        String(
            date.getMonth() + 1
        ).padStart(2, "0");

    const day =
        String(
            date.getDate()
        ).padStart(2, "0");

    return `${year}-${month}-${day}`;
}

function createDefaultDates() {
    const today =
        new Date();

    const thirtyDaysAgo =
        new Date();

    thirtyDaysAgo.setDate(
        today.getDate() - 29
    );

    return {
        from:
            formatDateForInput(
                thirtyDaysAgo
            ),

        to:
            formatDateForInput(
                today
            )
    };
}

function formatHours(hours) {
    const value =
        Number(hours || 0);

    return `${value.toFixed(2)} h`;
}

function getRoleCards(analytics) {
    if (!analytics) {
        return [];
    }

    if (
        analytics.role ===
        "Employee"
    ) {
        return [
            {
                title: "My Tickets",
                value:
                    analytics.totalTickets,
                color: "#2563eb"
            },
            {
                title: "Pending Review",
                value:
                    analytics
                        .pendingReviewTickets,
                color: "#f59e0b"
            },
            {
                title: "Assigned",
                value:
                    analytics
                        .assignedTickets,
                color: "#8b5cf6"
            },
            {
                title: "In Progress",
                value:
                    analytics
                        .inProgressTickets,
                color: "#0ea5e9"
            },
            {
                title: "Resolved",
                value:
                    analytics
                        .resolvedTickets,
                color: "#16a34a"
            },
            {
                title: "Closed",
                value:
                    analytics
                        .closedTickets,
                color: "#64748b"
            }
        ];
    }

    if (
        analytics.role ===
        "IT Support Agent"
    ) {
        return [
            {
                title: "My Tickets",
                value:
                    analytics.totalTickets,
                color: "#2563eb"
            },
            {
                title: "Assigned",
                value:
                    analytics
                        .assignedTickets,
                color: "#8b5cf6"
            },
            {
                title: "In Progress",
                value:
                    analytics
                        .inProgressTickets,
                color: "#0ea5e9"
            },
            {
                title: "Resolved",
                value:
                    analytics
                        .resolvedTickets,
                color: "#16a34a"
            },
            {
                title:
                    "Available Open",
                value:
                    analytics
                        .availableOpenTickets,
                color: "#0891b2"
            },
            {
                title:
                    "Average Resolution",
                value:
                    formatHours(
                        analytics
                            .averageResolutionHours
                    ),
                color: "#475569"
            }
        ];
    }

    /*
     * Manager and Admin receive
     * system-wide statistics.
     */
    return [
        {
            title: "Total Tickets",
            value:
                analytics.totalTickets,
            color: "#2563eb"
        },
        {
            title: "Pending Review",
            value:
                analytics
                    .pendingReviewTickets,
            color: "#f59e0b"
        },
        {
            title: "Open",
            value:
                analytics.openTickets,
            color: "#0891b2"
        },
        {
            title: "Assigned",
            value:
                analytics.assignedTickets,
            color: "#8b5cf6"
        },
        {
            title: "In Progress",
            value:
                analytics
                    .inProgressTickets,
            color: "#0ea5e9"
        },
        {
            title: "Resolved",
            value:
                analytics.resolvedTickets,
            color: "#16a34a"
        },
        {
            title: "Closed",
            value:
                analytics.closedTickets,
            color: "#64748b"
        },
        {
            title:
                "Pending Requests",
            value:
                analytics
                    .pendingAssignmentRequests,
            color: "#dc2626"
        },
        {
            title: "Active Agents",
            value:
                analytics.activeAgents,
            color: "#0f766e"
        },
        {
            title:
                "Average Resolution",
            value:
                formatHours(
                    analytics
                        .averageResolutionHours
                ),
            color: "#475569"
        }
    ];
}

function EmptyChart({
    message
}) {
    return (
        <div className="analytics-empty-chart">
            {message}
        </div>
    );
}

function AnalyticsDashboard() {
    const defaults =
        createDefaultDates();

    const [dateInputs, setDateInputs] =
        useState(defaults);

    const [
        appliedDates,
        setAppliedDates
    ] = useState(defaults);

    const [
        validationError,
        setValidationError
    ] = useState("");

    const {
        data: analytics,
        isLoading,
        isFetching,
        error,
        refetch
    } = useQuery({
        queryKey: [
            "dashboard-analytics",
            appliedDates.from,
            appliedDates.to
        ],

        queryFn: () =>
            getDashboardAnalytics(
                appliedDates.from,
                appliedDates.to
            )
    });

    function handleDateChange(event) {
        const {
            name,
            value
        } = event.target;

        setDateInputs(current => ({
            ...current,
            [name]: value
        }));
    }

    function handleApplyFilter(event) {
        event.preventDefault();

        setValidationError("");

        if (
            !dateInputs.from ||
            !dateInputs.to
        ) {
            setValidationError(
                "Choose both dates."
            );

            return;
        }

        if (
            dateInputs.from >
            dateInputs.to
        ) {
            setValidationError(
                "The From date cannot be after the To date."
            );

            return;
        }

        setAppliedDates({
            from: dateInputs.from,
            to: dateInputs.to
        });
    }

    function handleLast30Days() {
        const dates =
            createDefaultDates();

        setDateInputs(dates);
        setAppliedDates(dates);
        setValidationError("");
    }

    if (isLoading) {
        return (
            <div className="analytics-loading">
                Loading dashboard analytics...
            </div>
        );
    }

    if (error) {
        return (
            <div className="analytics-error-panel">
                <h2>
                    Dashboard could not be loaded
                </h2>

                <p>
                    {error.response?.data
                        ?.message ||
                        "Could not load dashboard analytics."}
                </p>

                <button
                    type="button"
                    onClick={() =>
                        refetch()
                    }
                >
                    Try Again
                </button>
            </div>
        );
    }

    const cards =
        getRoleCards(analytics);

    const hasTrendData =
        analytics.trend?.some(
            item =>
                item.created > 0 ||
                item.resolved > 0 ||
                item.closed > 0
        );

    return (
        <div className="analytics-dashboard">
            <div className="analytics-header">
                <div>
                    <h1>
                        {analytics.role}
                        {" "}
                        Dashboard
                    </h1>

                    <p>
                        Analytics from{" "}
                        <strong>
                            {appliedDates.from}
                        </strong>{" "}
                        to{" "}
                        <strong>
                            {appliedDates.to}
                        </strong>
                    </p>
                </div>

                {isFetching && (
                    <span className="analytics-refreshing">
                        Updating...
                    </span>
                )}
            </div>

            <form
                className="analytics-date-filter"
                onSubmit={
                    handleApplyFilter
                }
            >
                <div className="analytics-date-field">
                    <label htmlFor="dashboardFrom">
                        From
                    </label>

                    <input
                        id="dashboardFrom"
                        type="date"
                        name="from"
                        value={
                            dateInputs.from
                        }
                        onChange={
                            handleDateChange
                        }
                    />
                </div>

                <div className="analytics-date-field">
                    <label htmlFor="dashboardTo">
                        To
                    </label>

                    <input
                        id="dashboardTo"
                        type="date"
                        name="to"
                        value={
                            dateInputs.to
                        }
                        onChange={
                            handleDateChange
                        }
                    />
                </div>

                <button
                    type="submit"
                    className="analytics-filter-button"
                >
                    Apply Filter
                </button>

                <button
                    type="button"
                    className="analytics-secondary-button"
                    onClick={
                        handleLast30Days
                    }
                >
                    Last 30 Days
                </button>
            </form>

            {validationError && (
                <div className="analytics-validation-error">
                    {validationError}
                </div>
            )}

            <section className="analytics-kpi-grid">
                {cards.map(card => (
                    <DashboardCard
                        key={card.title}
                        title={card.title}
                        value={card.value}
                        color={card.color}
                    />
                ))}
            </section>

            <section className="analytics-chart-grid">
                <article className="analytics-chart-card analytics-chart-wide">
                    <div className="analytics-chart-heading">
                        <div>
                            <h2>
                                Ticket Activity Trend
                            </h2>

                            <p>
                                Created, resolved,
                                and closed tickets
                                during the selected
                                period.
                            </p>
                        </div>
                    </div>

                    {!hasTrendData ? (
                        <EmptyChart
                            message="No ticket activity exists during this period."
                        />
                    ) : (
                        <div className="analytics-chart-container">
                            <ResponsiveContainer
                                width="100%"
                                height="100%"
                            >
                                <LineChart
                                    data={
                                        analytics.trend
                                    }
                                    margin={{
                                        top: 10,
                                        right: 20,
                                        left: 0,
                                        bottom: 10
                                    }}
                                >
                                    <CartesianGrid
                                        strokeDasharray="3 3"
                                    />

                                    <XAxis
                                        dataKey="period"
                                        minTickGap={28}
                                    />

                                    <YAxis
                                        allowDecimals={
                                            false
                                        }
                                    />

                                    <Tooltip />

                                    <Legend />

                                    <Line
                                        type="monotone"
                                        dataKey="created"
                                        name="Created"
                                        stroke="#2563eb"
                                        strokeWidth={2}
                                        activeDot={{
                                            r: 5
                                        }}
                                    />

                                    <Line
                                        type="monotone"
                                        dataKey="resolved"
                                        name="Resolved"
                                        stroke="#16a34a"
                                        strokeWidth={2}
                                    />

                                    <Line
                                        type="monotone"
                                        dataKey="closed"
                                        name="Closed"
                                        stroke="#64748b"
                                        strokeWidth={2}
                                    />
                                </LineChart>
                            </ResponsiveContainer>
                        </div>
                    )}
                </article>

                <article className="analytics-chart-card">
                    <div className="analytics-chart-heading">
                        <div>
                            <h2>
                                Tickets by Status
                            </h2>

                            <p>
                                Current ticket status
                                breakdown.
                            </p>
                        </div>
                    </div>

                    {analytics.statusBreakdown
                        ?.length === 0 ? (
                        <EmptyChart
                            message="No status data exists."
                        />
                    ) : (
                        <div className="analytics-chart-container">
                            <ResponsiveContainer
                                width="100%"
                                height="100%"
                            >
                                <BarChart
                                    data={
                                        analytics
                                            .statusBreakdown
                                    }
                                    margin={{
                                        top: 10,
                                        right: 10,
                                        left: 0,
                                        bottom: 35
                                    }}
                                >
                                    <CartesianGrid
                                        strokeDasharray="3 3"
                                    />

                                    <XAxis
                                        dataKey="name"
                                        angle={-20}
                                        textAnchor="end"
                                        interval={0}
                                    />

                                    <YAxis
                                        allowDecimals={
                                            false
                                        }
                                    />

                                    <Tooltip />

                                    <Bar
                                        dataKey="count"
                                        name="Tickets"
                                        fill="#8b5cf6"
                                        radius={[
                                            6,
                                            6,
                                            0,
                                            0
                                        ]}
                                    />
                                </BarChart>
                            </ResponsiveContainer>
                        </div>
                    )}
                </article>

                <article className="analytics-chart-card">
                    <div className="analytics-chart-heading">
                        <div>
                            <h2>
                                Tickets by Priority
                            </h2>

                            <p>
                                Distribution of ticket
                                urgency.
                            </p>
                        </div>
                    </div>

                    {analytics.priorityBreakdown
                        ?.length === 0 ? (
                        <EmptyChart
                            message="No priority data exists."
                        />
                    ) : (
                        <div className="analytics-chart-container">
                            <ResponsiveContainer
                                width="100%"
                                height="100%"
                            >
                                <BarChart
                                    data={
                                        analytics
                                            .priorityBreakdown
                                    }
                                    margin={{
                                        top: 10,
                                        right: 10,
                                        left: 0,
                                        bottom: 10
                                    }}
                                >
                                    <CartesianGrid
                                        strokeDasharray="3 3"
                                    />

                                    <XAxis
                                        dataKey="name"
                                    />

                                    <YAxis
                                        allowDecimals={
                                            false
                                        }
                                    />

                                    <Tooltip />

                                    <Bar
                                        dataKey="count"
                                        name="Tickets"
                                        fill="#f59e0b"
                                        radius={[
                                            6,
                                            6,
                                            0,
                                            0
                                        ]}
                                    />
                                </BarChart>
                            </ResponsiveContainer>
                        </div>
                    )}
                </article>

                <article className="analytics-chart-card analytics-chart-wide">
                    <div className="analytics-chart-heading">
                        <div>
                            <h2>
                                Tickets by Category
                            </h2>

                            <p>
                                Most common support
                                request categories.
                            </p>
                        </div>
                    </div>

                    {analytics.categoryBreakdown
                        ?.length === 0 ? (
                        <EmptyChart
                            message="No category data exists."
                        />
                    ) : (
                        <div className="analytics-chart-container">
                            <ResponsiveContainer
                                width="100%"
                                height="100%"
                            >
                                <BarChart
                                    data={
                                        analytics
                                            .categoryBreakdown
                                    }
                                    margin={{
                                        top: 10,
                                        right: 15,
                                        left: 0,
                                        bottom: 45
                                    }}
                                >
                                    <CartesianGrid
                                        strokeDasharray="3 3"
                                    />

                                    <XAxis
                                        dataKey="name"
                                        angle={-20}
                                        textAnchor="end"
                                        interval={0}
                                    />

                                    <YAxis
                                        allowDecimals={
                                            false
                                        }
                                    />

                                    <Tooltip />

                                    <Bar
                                        dataKey="count"
                                        name="Tickets"
                                        fill="#0ea5e9"
                                        radius={[
                                            6,
                                            6,
                                            0,
                                            0
                                        ]}
                                    />
                                </BarChart>
                            </ResponsiveContainer>
                        </div>
                    )}
                </article>
            </section>
        </div>
    );
}

export default AnalyticsDashboard;