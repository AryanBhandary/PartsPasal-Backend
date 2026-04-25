namespace PartsPasal.Application.DTOs.Staff;

public class StaffDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string SpecializedSkill { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
}
