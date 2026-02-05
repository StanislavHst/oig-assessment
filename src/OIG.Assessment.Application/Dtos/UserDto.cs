namespace OIG.Assessment.Application.Dtos;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    Guid OrganizationId,
    string OrganizationName,
    IReadOnlyList<string> RoleNames);
