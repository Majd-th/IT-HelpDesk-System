import {
    useMutation,
    useQuery,
    useQueryClient
} from "@tanstack/react-query";

import {
    useNavigate
} from "react-router-dom";

import Layout from
    "../components/Layout";

import {
    getNotifications,
    markAllNotificationsAsRead,
    markNotificationAsRead
} from "../services/notificationService";

import "../assets/notifications.css";

function NotificationsPage() {
    const navigate =
        useNavigate();

    const queryClient =
        useQueryClient();

    const {
        data: notifications = [],
        isLoading,
        error
    } = useQuery({
        queryKey: [
            "notifications"
        ],

        queryFn:
            getNotifications
    });

    const markReadMutation =
        useMutation({
            mutationFn:
                markNotificationAsRead,

            onSuccess: () => {
                queryClient
                    .invalidateQueries({
                        queryKey: [
                            "notifications"
                        ]
                    });

                queryClient
                    .invalidateQueries({
                        queryKey: [
                            "notification-unread-count"
                        ]
                    });
            }
        });

    const markAllMutation =
        useMutation({
            mutationFn:
                markAllNotificationsAsRead,

            onSuccess: () => {
                queryClient
                    .invalidateQueries({
                        queryKey: [
                            "notifications"
                        ]
                    });

                queryClient
                    .invalidateQueries({
                        queryKey: [
                            "notification-unread-count"
                        ]
                    });
            }
        });

    async function handleNotificationClick(
        notification
    ) {
        try {
            if (
                !notification.isRead
            ) {
                await markReadMutation
                    .mutateAsync(
                        notification.id
                    );
            }

            if (
                notification.ticketId
            ) {
                navigate(
                    `/tickets/${notification.ticketId}`
                );
            }
        } catch (requestError) {
            console.error(
                "Could not open notification:",
                requestError
            );
        }
    }

    function formatDate(date) {
        if (!date) {
            return "";
        }

        return new Date(
            date
        ).toLocaleString();
    }

    if (isLoading) {
        return (
            <Layout>
                <p>
                    Loading notifications...
                </p>
            </Layout>
        );
    }

    if (error) {
        return (
            <Layout>
                <div className="notification-error">
                    Could not load notifications.
                </div>
            </Layout>
        );
    }

    const unreadCount =
        notifications.filter(
            notification =>
                !notification.isRead
        ).length;

    return (
        <Layout>
            <div className="notifications-page">

                <div className="notifications-header">
                    <div>
                        <h1>
                            Notifications
                        </h1>

                        <p>
                            {unreadCount}
                            {" "}
                            unread notification
                            {unreadCount === 1
                                ? ""
                                : "s"}
                        </p>
                    </div>

                    {unreadCount > 0 && (
                        <button
                            type="button"
                            className="mark-all-read-button"
                            disabled={
                                markAllMutation
                                    .isPending
                            }
                            onClick={() =>
                                markAllMutation
                                    .mutate()
                            }
                        >
                            {markAllMutation
                                .isPending
                                ? "Marking..."
                                : "Mark All as Read"}
                        </button>
                    )}
                </div>

                {notifications.length === 0 ? (
                    <div className="notifications-empty">
                        <span>
                            🔔
                        </span>

                        <h2>
                            No notifications
                        </h2>

                        <p>
                            New ticket activity
                            will appear here.
                        </p>
                    </div>
                ) : (
                    <div className="notifications-list">

                        {notifications.map(
                            notification => (
                                <button
                                    key={
                                        notification.id
                                    }
                                    type="button"
                                    className={
                                        notification.isRead
                                            ? "notification-item read"
                                            : "notification-item unread"
                                    }
                                    onClick={() =>
                                        handleNotificationClick(
                                            notification
                                        )
                                    }
                                >
                                    <div className="notification-status-area">
                                        {!notification.isRead && (
                                            <span className="notification-unread-dot" />
                                        )}
                                    </div>

                                    <div className="notification-content">
                                        <div className="notification-title-row">
                                            <h3>
                                                {
                                                    notification.title
                                                }
                                            </h3>

                                            <span>
                                                {formatDate(
                                                    notification.createdDate
                                                )}
                                            </span>
                                        </div>

                                        <p>
                                            {
                                                notification.message
                                            }
                                        </p>

                                        {notification.ticketReference && (
                                            <span className="notification-ticket-reference">
                                                {
                                                    notification.ticketReference
                                                }
                                            </span>
                                        )}

                                        <span className="notification-type">
                                            {
                                                notification.type
                                            }
                                        </span>
                                    </div>
                                </button>
                            )
                        )}
                    </div>
                )}
            </div>
        </Layout>
    );
}

export default NotificationsPage;