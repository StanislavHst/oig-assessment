using MediatR;
using Microsoft.AspNetCore.Mvc;
using OIG.Assessment.Api.Permissions;
using OIG.Assessment.Application.Commands.Roles;
using OIG.Assessment.Application.Queries.Roles;

namespace OIG.Assessment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [Permission(Domain.Common.Permissions.ViewRolesList)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRoleByIdQueryRequest(id), cancellationToken);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("search")]
    [Permission(Domain.Common.Permissions.ViewRolesList)]
    public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] Guid? organizationId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new SearchRolesQueryRequest(name, organizationId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("permissions/available")]
    [Permission(Domain.Common.Permissions.ViewRolesList)]
    public async Task<IActionResult> GetAvailablePermissions(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAvailablePermissionsQueryRequest(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Permission(Domain.Common.Permissions.AddRole)]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommandRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.RoleId }, result);
    }

    [HttpPut("{id:guid}")]
    [Permission(Domain.Common.Permissions.EditRole)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleCommandRequest request, CancellationToken cancellationToken)
    {
        if (id != request.RoleId)
            return BadRequest("Route id does not match body RoleId.");

        await mediator.Send(request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Permission(Domain.Common.Permissions.DeleteRole)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteRoleCommandRequest(id), cancellationToken);
        return NoContent();
    }
}

