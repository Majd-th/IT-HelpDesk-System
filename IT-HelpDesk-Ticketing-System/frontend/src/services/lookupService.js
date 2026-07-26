import axios from "axios";

const API = "http://localhost:5232/api";

function authHeader(){

    return{

        headers:{
            Authorization:`Bearer ${localStorage.getItem("token")}`
        }

    };

}

export async function getCategories(){

    const response = await axios.get(

        `${API}/Category`,
        authHeader()

    );

    return response.data;

}

export async function getPriorities(){

    const response = await axios.get(

        `${API}/Priority`,
        authHeader()

    );

    return response.data;

}

export async function getStatuses(){

    const response = await axios.get(

        `${API}/Status`,
        authHeader()

    );

    return response.data;

}