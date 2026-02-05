namespace OIG.Assessment.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public Guid OrganizationId { get; set; }
    
    public Organization Organization { get; set; } = null!;
    
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
