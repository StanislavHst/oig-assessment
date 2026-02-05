namespace OIG.Assessment.Application.Dtos;

public record OrganizationDto(
    Guid Id,
    string Name,
    Guid? ParentId);

public record OrganizationHierarchyItemDto(
    Guid Id,
    string Name,
    Guid? ParentId,
    IReadOnlyList<OrganizationHierarchyItemDto> Children);

