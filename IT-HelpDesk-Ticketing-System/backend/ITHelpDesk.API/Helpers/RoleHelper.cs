namespace ITHelpDesk.API.Helpers;

public static class RoleHelper
{
    public static bool IsAdmin(string role)
        => role == "Admin";

    public static bool IsManager(string role)
        => role == "Manager";

    public static bool IsAgent(string role)
        => role == "IT Support Agent";

    public static bool IsEmployee(string role)
        => role == "Employee";
}