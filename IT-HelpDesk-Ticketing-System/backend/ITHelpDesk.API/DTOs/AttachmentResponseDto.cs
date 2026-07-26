namespace ITHelpDesk.API.DTOs;

public class AttachmentResponseDto
{
    public int Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime UploadedDate { get; set; }

    public string UploadedBy { get; set; } = string.Empty;
}