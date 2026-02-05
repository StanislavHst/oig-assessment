namespace OIG.Assessment.Application.Dtos;

public record RoleDtos(Guid Id, string Name, IReadOnlyList<string> Permissions);

public record RoleListItemDto(
    Guid Id,
    string Name,
    Guid OrganizationId,
    string OrganizationName,
    IReadOnlyList<string> Permissions);
