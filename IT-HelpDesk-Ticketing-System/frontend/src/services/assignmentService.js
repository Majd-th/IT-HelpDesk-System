import api from "../api/axios";

export async function getUnassignedTickets() {
    const response = await api.get(
        "/TicketAssignment/unassigned"
    );

    return response.data;
}

export async function getAgentWorkloads() {
    const response = await api.get(
        "/TicketAssignment/agents/workload"
    );

    return response.data;
}

export async function assignTicket(
    ticketId,
    agentId,
    notes
) {
    const response = await api.post(
        `/TicketAssignment/${ticketId}/assign`,
        {
            agentId: Number(agentId),
            notes: notes?.trim() || null
        }
    );

    return response.data;
}

export async function reassignTicket(
    ticketId,
    newAgentId,
    notes
) {
    const response = await api.put(
        `/TicketAssignment/${ticketId}/reassign`,
        {
            newAgentId: Number(newAgentId),
            notes: notes?.trim() || null
        }
    );

    return response.data;
}

export async function getPendingRequests() {
    const response = await api.get(
        "/TicketAssignment/requests"
    );

    return response.data;
}

export async function reviewAssignmentRequest(
    assignmentId,
    approved,
    notes
) {
    const response = await api.put(
        `/TicketAssignment/requests/${assignmentId}/review`,
        {
            approved,
            notes: notes?.trim() || null
        }
    );

    return response.data;
}

export async function getAssignmentHistory(ticketId) {
    const response = await api.get(
        `/TicketAssignment/${ticketId}/history`
    );

    return response.data;
}
export async function getAvailableTickets() {
    const response = await api.get(
        "/TicketAssignment/available"
    );

    return response.data;
}

export async function requestTicket(ticketId, notes) {
    const response = await api.post(
        `/TicketAssignment/${ticketId}/request`,
        {
            notes: notes?.trim() || null
        }
    );

    return response.data;
}

export async function getMyAssignedTickets() {
    const response = await api.get(
        "/TicketAssignment/my-tickets"
    );

    return response.data;
}

export async function getMyTicketHistory() {
    const response = await api.get(
        "/TicketAssignment/my-history"
    );

    return response.data;
}