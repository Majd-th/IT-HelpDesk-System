import api from "../api/axios";

export async function getProfile() {
    const response = await api.get("/Auth/me");

    return response.data;
}

export async function updateProfile(profile) {
    const response = await api.put(
        "/Auth/profile",
        profile
    );

    return response.data;
}

export async function changePassword(passwordData) {
    const response = await api.put(
        "/Auth/change-password",
        passwordData
    );

    return response.data;
}