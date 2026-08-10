import {
    useQuery
} from "@tanstack/react-query";

import {
    useNavigate
} from "react-router-dom";

import {
    getUnreadCount
} from "../services/notificationService";

import "../assets/notifications.css";

function NotificationBell() {
    const navigate =
        useNavigate();

    const {
        data
    } = useQuery({
        queryKey: [
            "notification-unread-count"
        ],

        queryFn:
            getUnreadCount,

        /*
         * Temporary polling.
         *
         * SignalR will replace this later
         * with real-time notifications.
         */
        refetchInterval:
            30000,

        refetchOnWindowFocus:
            true
    });

    const unreadCount =
        data?.unreadCount ?? 0;

    return (
        <button
            type="button"
            className="notification-bell"
            onClick={() =>
                navigate(
                    "/notifications"
                )
            }
            title="Notifications"
        >
            <span className="notification-bell-icon">
                🔔
            </span>

            {unreadCount > 0 && (
                <span className="notification-badge">
                    {unreadCount > 99
                        ? "99+"
                        : unreadCount}
                </span>
            )}
        </button>
    );
}

export default NotificationBell;