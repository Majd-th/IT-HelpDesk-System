import api from "../api/axios";

export async function getAttachments(ticketId) {
    const response = await api.get(
        `/TicketAttachment/ticket/${ticketId}`
    );

    return response.data;
}

export async function uploadAttachment(ticketId, file) {
    const formData = new FormData();

    formData.append("file", file);

    const response = await api.post(
        `/TicketAttachment/${ticketId}`,
        formData,
        {
            headers: {
                "Content-Type": undefined
            }
        }
    );

    return response.data;
}

export async function downloadAttachment(
    attachmentId,
    fileName
) {
    const response = await api.get(
        `/TicketAttachment/download/${attachmentId}`,
        {
            responseType: "blob"
        }
    );

    const blobUrl =
        window.URL.createObjectURL(
            response.data
        );

    const link =
        document.createElement("a");

    link.href = blobUrl;
    link.download =
        fileName || "attachment";

    document.body.appendChild(link);
    link.click();
    link.remove();

    window.URL.revokeObjectURL(blobUrl);
}

export async function deleteAttachment(
    attachmentId
) {
    await api.delete(
        `/TicketAttachment/${attachmentId}`
    );
}