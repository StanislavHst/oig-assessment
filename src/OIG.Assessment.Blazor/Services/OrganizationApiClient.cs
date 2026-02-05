using System.Net;
using System.Net.Http.Json;
using OIG.Assessment.Application.Commands.Organizations;
using OIG.Assessment.Application.Dtos;
using OIG.Assessment.Application.Queries.Organizations;

namespace OIG.Assessment.Blazor.Services;

public class OrganizationApiClient(HttpClient httpClient, CurrentUserState currentUserState)
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

    public async Task<IReadOnlyList<OrganizationHierarchyItemDto>> GetTreeAsync(CancellationToken cancellationToken = default)
    {
        ApplyCurrentUserHeader();

        var response = await _httpClient.GetFromJsonAsync<GetOrganizationTreeQueryResult>("api/organizations/tree", cancellationToken);
        return response?.Organizations ?? Array.Empty<OrganizationHierarchyItemDto>();
    }

    public async Task<GetOrganizationByIdQueryResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ApplyCurrentUserHeader();

        var response = await _httpClient.GetAsync($"api/organizations/{id}", cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<GetOrganizationByIdQueryResult>(cancellationToken: cancellationToken);
    }

    public async Task<GetOrganizationHierarchyQueryResult?> GetHierarchyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ApplyCurrentUserHeader();

        var response = await _httpClient.GetAsync($"api/organizations/{id}/hierarchy", cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<GetOrganizationHierarchyQueryResult>(cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<OrganizationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        ApplyCurrentUserHeader();

        var response = await _httpClient.GetFromJsonAsync<SearchOrganizationsQueryResult>("api/organizations/search", cancellationToken);
        return response?.Organizations ?? Array.Empty<OrganizationDto>();
    }

    public async Task<string?> UpdateAsync(UpdateOrganizationCommandRequest request, CancellationToken cancellationToken = default)
    {
        ApplyCurrentUserHeader();

        var response = await _httpClient.PutAsJsonAsync($"api/organizations/{request.OrganizationId}", request, cancellationToken);
        if (response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<string?> CreateAsync(CreateOrganizationCommandRequest request, CancellationToken cancellationToken = default)
    {
        ApplyCurrentUserHeader();

        var response = await _httpClient.PostAsJsonAsync("api/organizations", request, cancellationToken);
        if (response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<string?> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ApplyCurrentUserHeader();

        var response = await _httpClient.DeleteAsync($"api/organizations/{id}", cancellationToken);
        if (response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<string?> CreateChildAsync(string name, Guid parentId, CancellationToken cancellationToken = default)
    {
        var request = new CreateOrganizationCommandRequest(name, parentId);
        ApplyCurrentUserHeader();

        var response = await _httpClient.PostAsJsonAsync("api/organizations", request, cancellationToken);
        if (response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}

