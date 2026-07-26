namespace ITHelpDesk.API.DTOs;

public class ActivityLogResponseDto
{
    public string User { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
}