export function getUser() {

    const token = localStorage.getItem("token");

    if (!token) return null;

    const payload = JSON.parse(atob(token.split(".")[1]));

    return {
        id: payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
        firstName: payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"],
        lastName: payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"],
        email: payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"],
        role: payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
    };
}