using System.Net;
using System.Net.Http.Json;
using OIG.Assessment.Application.Commands.Roles;
using OIG.Assessment.Application.Dtos;
using OIG.Assessment.Application.Queries.Roles;

namespace OIG.Assessment.Blazor.Services;

public class RolesApiClient(HttpClient httpClient, CurrentUserState currentUserState)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly CurrentUserState _currentUserState = currentUserState;

    private void ApplyCurrentUserHeader()
    {
        _httpClient.DefaultRequestHeaders.Remove("X-Demo-UserId");
        if (_currentUserState.CurrentUserId.HasValue)
        {
            _httpClient.DefaultRequestHeaders.Add("X-Demo-UserId", _currentUserState.CurrentUserId.Value.ToString());
        }
    }

    public async Task<(IReadOnlyList<RoleListItemDto> Roles, HttpStatusCode? ErrorCode, string? ErrorMessage)> SearchAsync(
        string? name,
        Guid? organizationId,
        CancellationToken cancellationToken = default)
    {
        var url = "api/roles/search";
        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(name))
            query.Add($"name={Uri.EscapeDataString(name)}");
        if (organizationId.HasValue && organizationId.Value != Guid.Empty)
            query.Add($"organizationId={organizationId.Value}");

        if (query.Count > 0)
            url += "?" + string.Join("&", query);

        try
        {
            ApplyCurrentUserHeader();

            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<SearchRolesQueryResult>(cancellationToken: cancellationToken);
                return (result?.Roles ?? Array.Empty<RoleListItemDto>(), null, null);
            }

            return (Array.Empty<RoleListItemDto>(), response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
        }
        catch (Exception ex)
        {
            return (Array.Empty<RoleListItemDto>(), null, ex.Message);
        }
    }

    public async Task<(Guid? RoleId, HttpStatusCode? ErrorCode, string? ErrorMessage)> CreateAsync(
        CreateRoleCommandRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ApplyCurrentUserHeader();

            var response = await _httpClient.PostAsJsonAsync("api/roles", request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CreateRoleCommandResult>(cancellationToken: cancellationToken);
                return (result?.RoleId, null, null);
            }

            return (null, response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
        }
        catch (Exception ex)
        {
            return (null, null, ex.Message);
        }
    }

    public async Task<GetRoleByIdQueryResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ApplyCurrentUserHeader();

        var response = await _httpClient.GetAsync($"api/roles/{id}", cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<GetRoleByIdQueryResult>(cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetAvailablePermissionsAsync(CancellationToken cancellationToken = default)
    {
        ApplyCurrentUserHeader();

        var response = await _httpClient.GetFromJsonAsync<GetAvailablePermissionsQueryResult>("api/roles/permissions/available", cancellationToken);
        return response?.Permissions ?? Array.Empty<string>();
    }

    public async Task<string?> UpdateAsync(UpdateRoleCommandRequest request, CancellationToken cancellationToken = default)
    {
        ApplyCurrentUserHeader();

        var response = await _httpClient.PutAsJsonAsync($"api/roles/{request.RoleId}", request, cancellationToken);
        if (response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<string?> DeleteAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            ApplyCurrentUserHeader();

            var response = await _httpClient.DeleteAsync($"api/roles/{roleId}", cancellationToken);
            if (response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}

