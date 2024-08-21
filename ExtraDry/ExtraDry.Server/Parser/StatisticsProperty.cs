﻿using System.Reflection;

namespace ExtraDry.Server.Internal;

/// <summary>
/// Represents information about a property on an Entity that is decorated with StatisticsAttribute
/// in the ModelDescription.
/// </summary>
internal class StatisticsProperty(
    PropertyInfo property, 
    string externalName, 
    Stats stats)
{
    public PropertyInfo Property { get; } = property;

    public Stats Stats { get; } = stats;

    public string ExternalName { get; } = externalName;
}
