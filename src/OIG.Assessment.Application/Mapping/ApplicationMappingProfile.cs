using AutoMapper;
using OIG.Assessment.Application.Dtos;
using OIG.Assessment.Application.Queries.Roles;
using OIG.Assessment.Application.Queries.Users;
using OIG.Assessment.Domain.Entities;

namespace OIG.Assessment.Application.Mapping;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<Role, GetRoleByIdQueryResult>()
            .ForCtorParam("OrganizationName", o => o.MapFrom(s => s.Organization.Name))
            .ForCtorParam("Permissions", o => o.MapFrom(s => s.Permissions.ToList()));

        CreateMap<Role, RoleListItemDto>()
            .ForCtorParam("OrganizationName", o => o.MapFrom(s => s.Organization.Name))
            .ForCtorParam("Permissions", o => o.MapFrom(s => s.Permissions.ToList()));

        CreateMap<Role, RoleDtos>()
            .ForCtorParam("Permissions", o => o.MapFrom(s => s.Permissions.ToList()));

        CreateMap<User, GetUserByIdQueryResult>()
            .ForCtorParam("OrganizationName", o => o.MapFrom(s => s.Organization.Name))
            .ForCtorParam("Roles", o => o.MapFrom(s => s.Roles));

        CreateMap<User, UserDto>()
            .ForCtorParam("OrganizationName", o => o.MapFrom(s => s.Organization.Name))
            .ForCtorParam("RoleNames", o => o.MapFrom(s => s.Roles.Select(r => r.Name).ToList()));
    }
}
