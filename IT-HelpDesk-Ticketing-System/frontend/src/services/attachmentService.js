import axios from "axios";

const API_URL = "http://localhost:5232/api/TicketAttachment";

function authHeader() {
    return {
        headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`
        }
    };
}

export async function uploadAttachment(ticketId, file) {

    const formData = new FormData();

    formData.append("file", file);

    await axios.post(
        `${API_URL}/${ticketId}`,
        formData,
        {
            headers: {
                ...authHeader().headers,
                "Content-Type": "multipart/form-data"
            }
        }
    );
}

export async function getAttachments(ticketId) {

    const response = await axios.get(
        `${API_URL}/${ticketId}`,
        authHeader()
    );

    return response.data;
}

export async function deleteAttachment(id) {

    await axios.delete(
        `${API_URL}/${id}`,
        authHeader()
    );
}

export async function downloadAttachment(id) {

    const response = await axios.get(
        `${API_URL}/download/${id}`,
        {
            ...authHeader(),
            responseType: "blob"
        }
    );

    return response.data;
}