namespace OIG.Assessment.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public Guid OrganizationId { get; set; }
    
    public Organization Organization { get; set; } = null!;
    
    public ICollection<string> Permissions { get; set; } = new List<string>();
    
    public ICollection<User> Users { get; set; } = new List<User>();
    
    public bool ValidatePermissions()
    {
        return Permissions.All(Common.Permissions.IsValid);
    }
}
