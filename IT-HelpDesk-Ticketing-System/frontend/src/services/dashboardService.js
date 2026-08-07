import api from "../api/axios";

export async function getDashboardAnalytics(
    from,
    to
) {
    const response = await api.get(
        "/Dashboard/analytics",
        {
            params: {
                from,
                to
            }
        }
    );

    return response.data;
}