using MediatR;
using Microsoft.AspNetCore.Mvc;
using OIG.Assessment.Api.Permissions;
using OIG.Assessment.Application.Commands.Organizations;
using OIG.Assessment.Application.Permissions;
using OIG.Assessment.Application.Queries.Organizations;
using OIG.Assessment.Application.Services;

namespace OIG.Assessment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController(IMediator mediator, IPermissionChecker permissionChecker, ICurrentUserContext currentUserContext) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrganizationByIdQueryRequest(id), cancellationToken);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("{id:guid}/hierarchy")]
    public async Task<IActionResult> GetHierarchy(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrganizationHierarchyQueryRequest(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetTree(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrganizationTreeQueryRequest(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] Guid? parentId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new SearchOrganizationsQueryRequest(name, parentId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Permission(Domain.Common.Permissions.AddOrganization)]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationCommandRequest request, CancellationToken cancellationToken)
    {
        if (!request.ParentId.HasValue)
        {
            var userId = currentUserContext.GetCurrentUserId();
            if (userId == null)
                return Unauthorized("Current user is not specified. Provide user id via 'X-Demo-UserId' header or 'currentUserId' query parameter.");

            var hasRootPermission = await permissionChecker.HasPermissionAsync(userId.Value, Domain.Common.Permissions.AddRootOrganization, cancellationToken);
            if (!hasRootPermission)
                return Forbid($"Permission '{Domain.Common.Permissions.AddRootOrganization}' is required to create root organizations.");
        }

        var result = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.OrganizationId }, result);
    }

    [HttpPut("{id:guid}")]
    [Permission(Domain.Common.Permissions.EditOrganization)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrganizationCommandRequest request, CancellationToken cancellationToken)
    {
        if (id != request.OrganizationId)
            return BadRequest("Route id does not match body OrganizationId.");

        await mediator.Send(request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Permission(Domain.Common.Permissions.DeleteOrganization)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteOrganizationCommandRequest(id), cancellationToken);
        return NoContent();
    }
}
