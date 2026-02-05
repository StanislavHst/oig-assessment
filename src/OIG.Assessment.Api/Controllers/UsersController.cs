using MediatR;
using Microsoft.AspNetCore.Mvc;
using OIG.Assessment.Api.Permissions;
using OIG.Assessment.Application.Commands.Users;
using OIG.Assessment.Application.Dtos;
using OIG.Assessment.Application.Queries.Users;

namespace OIG.Assessment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [Permission(Domain.Common.Permissions.ViewUserList)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserByIdQueryRequest(id), cancellationToken);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("{id:guid}/permissions")]
    [Permission(Domain.Common.Permissions.ViewUserList)]
    public async Task<IActionResult> GetPermissions(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserPermissionsQueryRequest(id), cancellationToken);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("{id:guid}/available-roles")]
    [Permission(Domain.Common.Permissions.ViewRolesList)]
    public async Task<IActionResult> GetAvailableRoles(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAvailableRolesForUserQueryRequest(id), cancellationToken);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("search")]
    [Permission(Domain.Common.Permissions.ViewUserList)]
    public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] Guid? organizationId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new SearchUsersQueryRequest(name, organizationId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("selector")]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetForSelector(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUsersForSelectorQueryRequest(), cancellationToken);
        return Ok(result.Users);
    }

    [HttpPost]
    [Permission(Domain.Common.Permissions.AddUser)]
    public async Task<IActionResult> Create([FromBody] CreateUserCommandRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.UserId }, result);
    }

    [HttpPut("{id:guid}")]
    [Permission(Domain.Common.Permissions.EditUser)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommandRequest request, CancellationToken cancellationToken)
    {
        if (id != request.UserId)
            return BadRequest("Route id does not match body UserId.");

        await mediator.Send(request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Permission(Domain.Common.Permissions.DeleteUser)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteUserCommandRequest(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{userId:guid}/roles/{roleId:guid}")]
    public async Task<IActionResult> AssignRole(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        await mediator.Send(new AssignRoleToUserCommandRequest(userId, roleId), cancellationToken);
        return NoContent();
    }
}

