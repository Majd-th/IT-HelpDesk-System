export function getDashboardRoute(role) {
    switch (role) {
        case "Employee":
            return "/employee";

        case "IT Support Agent":
            return "/it-agent";

        case "Manager":
            return "/manager";

        case "Admin":
            return "/admin";

        default:
            return "/";
    }
}