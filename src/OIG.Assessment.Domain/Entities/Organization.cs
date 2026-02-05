namespace OIG.Assessment.Domain.Entities;

public class Organization : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public Organization? Parent { get; set; }
    public ICollection<Organization> Children { get; set; } = new List<Organization>();
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
