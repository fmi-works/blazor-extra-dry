using ExtraDry.Core;
using ExtraDry.Server;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sample.Components.Api.Security;
using Sample.Components.Api.Services;

namespace Sample.Components.Api.Controllers;

/// <summary>
/// Endpoints for managing tenants, typically called by Admins.
/// </summary>
[ApiController]
[SkipStatusCodePages]
[ApiExceptionStatusCodes]
public class TenantController(
    CustomerService tenants)
{
    /// <summary>
    /// Get a list of all Tenants.
    /// </summary>
    [HttpGet("/tenants")]
    [Authorize(Policies.Admin)]
    [Produces("application/json")]
    public async Task<PagedCollection<Customer>> ListTenants([FromQuery] PageQuery query)
    {
        return await tenants.ListTenantsAsync(query);
    }

    /// <summary>
    /// Create a new Tenant.
    /// </summary>
    /// <remarks>
    /// Typically called by agents to create a tenant when events are received indicating new
    /// clients.
    /// </remarks>
    [HttpPost("/tenants")]
    [Authorize(Policies.AdminOrAgent)]
    [Consumes("application/json"), Produces("application/json")]
    public async Task<ResourceReference<Customer>> CreateTenant(Customer exemplar)
    {
        var tenant = await tenants.CreateTenantAsync(exemplar);
        return new ResourceReference<Customer>(tenant);
    }

    /// <summary>
    /// Retrieve a tenant by its Slug.
    /// </summary>
    [HttpGet("/tenants/{slug}")]
    [Authorize(Policies.AdminOrAgent)]
    [Produces("application/json")]
    public async Task<Customer> RetrieveTenant(string slug)
    {
        return await tenants.RetrieveTenantAsync(slug);
    }

    /// <summary>
    /// Update an existing tenant.
    /// </summary>
    [HttpPut("/tenants/{slug}")]
    [Authorize(Policies.AdminOrAgent)]
    [Consumes("application/json"), Produces("application/json")]
    public async Task<ResourceReference<Customer>> UpdateTenant(string slug, Customer exemplar)
    {
        if(slug != exemplar.Slug) {
            throw new ArgumentException("Slug mismatch", nameof(slug));
        }
        var tenant = await tenants.UpdateTenantAsync(exemplar);
        return new ResourceReference<Customer>(tenant);
    }

    /// <summary>
    /// Delete an existing tenant.
    /// </summary>
    [HttpDelete("/tenants/{slug}")]
    [Authorize(Policies.AdminOrAgent)]
    public async Task DeleteTenant(string slug)
    {
        await tenants.DeleteTenantAsync(slug);
    }
}
