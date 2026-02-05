using System;

namespace OIG.Assessment.Api.Permissions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class PermissionAttribute : Attribute
{
    public string Permission { get; }

    public PermissionAttribute(string permission)
    {
        Permission = permission;
    }
}

