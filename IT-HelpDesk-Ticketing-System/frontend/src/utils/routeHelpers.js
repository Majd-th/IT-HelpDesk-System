export function getDashboardRoute(role) {
    switch (role) {
        case "Admin":
            return "/admin/dashboard";

        case "Manager":
            return "/manager";

        case "IT Support Agent":
            return "/agent";

        case "Employee":
            return "/employee";

        default:
            return "/";
    }
}