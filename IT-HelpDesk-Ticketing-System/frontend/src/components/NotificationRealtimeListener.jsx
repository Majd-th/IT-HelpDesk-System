import {
    useEffect
} from "react";

import {
    useQueryClient
} from "@tanstack/react-query";

import * as signalR
    from "@microsoft/signalr";


function NotificationRealtimeListener() {
    const queryClient =
        useQueryClient();

    useEffect(() => {
        const token =
            localStorage.getItem(
                "token"
            );

        if (!token) {
            return;
        }

        let disposed = false;

        const connection =
            new signalR
                .HubConnectionBuilder()
                .withUrl(
                    "http://localhost:5232/hubs/notifications",
                    {
                        accessTokenFactory:
                            () =>
                                localStorage.getItem(
                                    "token"
                                ) || ""
                    }
                )
                .withAutomaticReconnect()
                .build();

        connection.on(
            "NotificationReceived",
            async () => {
                await Promise.all([
                    queryClient
                        .invalidateQueries({
                            queryKey: [
                                "notifications"
                            ]
                        }),

                    queryClient
                        .invalidateQueries({
                            queryKey: [
                                "notification-unread-count"
                            ]
                        })
                ]);
            }
        );

        async function startConnection() {
            try {
                await connection.start();

                if (!disposed) {
                    console.log(
                        "SignalR notifications connected."
                    );
                }
            } catch (error) {
                if (!disposed) {
                    console.error(
                        "SignalR connection failed:",
                        error
                    );
                }
            }
        }

        startConnection();

        return () => {
            disposed = true;

            connection.off(
                "NotificationReceived"
            );

            connection
                .stop()
                .catch(() => {});
        };
    }, [queryClient]);

    return null;
}

export default
    NotificationRealtimeListener;