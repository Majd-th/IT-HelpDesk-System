import api from "../api/axios";

// ======================
// GET ROLE-SPECIFIC LIST
// ======================

export async function getTickets() {
    const response =
        await api.get("/Ticket");

    return response.data;
}

// ======================
// GET ONE TICKET
// ======================

export async function getTicketById(id) {
    const response =
        await api.get(
            `/Ticket/${id}`
        );

    return response.data;
}

// ======================
// CREATE
// ======================

export async function createTicket(
    ticket
) {
    const response =
        await api.post(
            "/Ticket",
            ticket
        );

    return response.data;
}

// ======================
// UPDATE DETAILS
// ======================

export async function updateTicket(
    id,
    ticket
) {
    const response =
        await api.put(
            `/Ticket/${id}`,
            ticket
        );

    return response.data;
}

// ======================
// ADMIN DELETE
// ======================

export async function deleteTicket(id) {
    const response =
        await api.delete(
            `/Ticket/${id}`
        );

    return response.data;
}

// ======================
// FILTER
// ======================

export async function filterTickets(
    filters
) {
    const params = {};

    if (filters.search?.trim()) {
        params.search =
            filters.search.trim();
    }

    if (filters.categoryId) {
        params.categoryId =
            filters.categoryId;
    }

    if (filters.priorityId) {
        params.priorityId =
            filters.priorityId;
    }

    if (filters.statusId) {
        params.statusId =
            filters.statusId;
    }

    if (filters.createdAfter) {
        params.createdAfter =
            filters.createdAfter;
    }

    if (filters.createdBefore) {
        params.createdBefore =
            filters.createdBefore;
    }

    const response =
        await api.get(
            "/Ticket/filter",
            {
                params
            }
        );

    return response.data;
}// ======================
// CLOSE TICKET
// ======================

export async function closeTicket(
    id,
    reason
) {
    const response =
        await api.put(
            `/Ticket/${id}/close`,
            {
                reason:
                    reason.trim()
            }
        );

    return response.data;
}

// ======================
// ACTIVITY
// ======================

export async function getActivityLog(
    id
) {
    const response =
        await api.get(
            `/Ticket/${id}/activity`
        );

    return response.data;
}

// ======================
// AGENT START WORK
// ======================

export async function startWork(
    id,
    description
) {
    const response =
        await api.put(
            `/Ticket/${id}/start-work`,
            {
                description:
                    description.trim() ||
                    null
            }
        );

    return response.data;
}

// ======================
// AGENT RESOLVE
// ======================

export async function resolveTicket(
    id,
    solution,
    workDescription
) {
    const response =
        await api.put(
            `/Ticket/${id}/resolve`,
            {
                solution:
                    solution.trim(),

                workDescription:
                    workDescription.trim() ||
                    null
            }
        );

    return response.data;
}