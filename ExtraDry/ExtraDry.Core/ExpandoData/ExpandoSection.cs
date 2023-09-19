﻿namespace ExtraDry.Core;

/// <summary>
/// An Expando Section is used to divide custom fields into manageable parts.
/// </summary>
public class ExpandoSection {
    /// <summary>
    /// Title of the section for user display.
    /// </summary>
    public string Title { get; set; } = "Custom Fields";

    /// <summary>
    /// A list of fields contained in the section.
    /// </summary>
    public List<ExpandoField> Fields { get; set; } = new();

    /// <summary>
    /// Section order.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Section State.
    /// </summary>
    public ExpandoState State { get; set; } = ExpandoState.Draft;

}
