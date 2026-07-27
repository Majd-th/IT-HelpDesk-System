import axios from "axios";

import api from "../api/axios";
const API_URL = "http://localhost:5232/api/Ticket";

function authHeader() {
    return {
        headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`
        }
    };
}

// ======================
// GET ALL
// ======================

export async function getTickets() {

    const response = await axios.get(
        API_URL,
        authHeader()
    );

    return response.data;
}

// ======================
// GET BY ID
// ======================

export async function getTicketById(id) {

    const response = await axios.get(
        `${API_URL}/${id}`,
        authHeader()
    );

    return response.data;
}

// ======================
// CREATE
// ======================

export async function createTicket(ticket) {

    const response = await axios.post(
        API_URL,
        ticket,
        authHeader()
    );

    return response.data;
}

// ======================
// UPDATE
// ======================

export async function updateTicket(id, ticket) {

console.log(ticket);
    const response = await axios.put(
        `${API_URL}/${id}`,
        ticket,
        authHeader()
    );

    return response.data;
}

// ======================
// DELETE
// ======================

export async function deleteTicket(id) {

    const response = await axios.delete(
        `${API_URL}/${id}`,
        authHeader()
    );

    return response.data;
}

// ======================
// FILTER
// ======================

export async function filterTickets(filters) {

    const params = new URLSearchParams();

    if (filters.search)
        params.append("search", filters.search);

    if (filters.categoryId)
        params.append("categoryId", filters.categoryId);

    if (filters.priorityId)
        params.append("priorityId", filters.priorityId);

    if (filters.statusId)
        params.append("statusId", filters.statusId);

    if (filters.createdAfter)
        params.append("createdAfter", filters.createdAfter);

    if (filters.createdBefore)
        params.append("createdBefore", filters.createdBefore);

    const response = await axios.get(
        `${API_URL}/filter?${params.toString()}`,
        authHeader()
    );

    return response.data;
}



export async function getActivityLog(id){

    const response = await api.get(`/Ticket/${id}/activity`);

    return response.data;

}

   