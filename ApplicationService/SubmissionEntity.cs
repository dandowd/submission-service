namespace ApplicationService;

public class SubmissionEntity
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? DOB { get; set; }
    public string? SSN { get; set; }
    public SubmissionStatus Status { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
}
