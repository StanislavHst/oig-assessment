using System.Net;
using System.Net.Http.Json;
using OIG.Assessment.Application.Commands.Users;
using OIG.Assessment.Application.Dtos;
using OIG.Assessment.Application.Queries.Users;

namespace OIG.Assessment.Blazor.Services;

public class UsersApiClient(HttpClient httpClient, CurrentUserState currentUserState)
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

    public async Task<(IReadOnlyList<UserDto> Users, HttpStatusCode? ErrorCode, string? ErrorMessage)> GetForSelectorAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            // цей endpoint не вимагає заголовка, але все одно приведемо HttpClient до єдиного стану
            ApplyCurrentUserHeader();

            var response = await _httpClient.GetAsync("api/users/selector", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<IReadOnlyList<UserDto>>(cancellationToken: cancellationToken)
                            ?? Array.Empty<UserDto>();
                return (users, null, null);
            }

            return (Array.Empty<UserDto>(), response.StatusCode,
                await response.Content.ReadAsStringAsync(cancellationToken));
        }
        catch (Exception ex)
        {
            return (Array.Empty<UserDto>(), null, ex.Message);
        }
    }

    public async Task<(IReadOnlyList<UserDto> Users, HttpStatusCode? ErrorCode, string? ErrorMessage)> SearchAsync(
        string? name,
        Guid? organizationId,
        CancellationToken cancellationToken = default)
    {
        var url = "api/users/search";
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
                var result = await response.Content.ReadFromJsonAsync<SearchUsersQueryResult>(cancellationToken: cancellationToken);
                return (result?.Users ?? Array.Empty<UserDto>(), null, null);
            }

            return (Array.Empty<UserDto>(), response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
        }
        catch (Exception ex)
        {
            return (Array.Empty<UserDto>(), null, ex.Message);
        }
    }

    public async Task<GetUserByIdQueryResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ApplyCurrentUserHeader();

        var response = await _httpClient.GetAsync($"api/users/{id}", cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<GetUserByIdQueryResult>(cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetPermissionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ApplyCurrentUserHeader();

        try
        {
            var response = await _httpClient.GetAsync($"api/users/{id}/permissions", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GetUserPermissionsQueryResult>(cancellationToken: cancellationToken);
                return result?.Permissions ?? Array.Empty<string>();
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
                return Array.Empty<string>();

            return Array.Empty<string>();
        }
        catch (HttpRequestException)
        {
            return Array.Empty<string>();
        }
    }

    public async Task<IReadOnlyList<RoleDtos>> GetAvailableRolesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ApplyCurrentUserHeader();

        var response = await _httpClient.GetFromJsonAsync<GetAvailableRolesForUserQueryResult>($"api/users/{id}/available-roles", cancellationToken);
        return response?.Roles ?? Array.Empty<RoleDtos>();
    }

    public async Task<(Guid? UserId, HttpStatusCode? ErrorCode, string? ErrorMessage)> CreateAsync(
        CreateUserCommandRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ApplyCurrentUserHeader();

            var response = await _httpClient.PostAsJsonAsync("api/users", request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CreateUserCommandResult>(cancellationToken: cancellationToken);
                return (result?.UserId, null, null);
            }

            return (null, response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
        }
        catch (Exception ex)
        {
            return (null, null, ex.Message);
        }
    }

    public async Task<string?> UpdateAsync(UpdateUserCommandRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            ApplyCurrentUserHeader();

            var response = await _httpClient.PutAsJsonAsync($"api/users/{request.UserId}", request, cancellationToken);
            if (response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<string?> AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            ApplyCurrentUserHeader();

            // Send an empty JSON body to satisfy HttpClient's requirement for non-null content
            using var response = await _httpClient.PostAsJsonAsync(
                $"api/users/{userId}/roles/{roleId}",
                new { },
                cancellationToken);
            if (response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<string?> DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            ApplyCurrentUserHeader();

            var response = await _httpClient.DeleteAsync($"api/users/{userId}", cancellationToken);
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

