import api from "../api/axios";

export async function getNotifications() {
    const response =
        await api.get(
            "/Notification"
        );

    return response.data;
}

export async function getUnreadCount() {
    const response =
        await api.get(
            "/Notification/unread-count"
        );

    return response.data;
}

export async function markNotificationAsRead(
    notificationId
) {
    const response =
        await api.put(
            `/Notification/${notificationId}/read`
        );

    return response.data;
}

export async function markAllNotificationsAsRead() {
    const response =
        await api.put(
            "/Notification/read-all"
        );

    return response.data;
}