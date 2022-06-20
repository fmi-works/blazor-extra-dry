﻿namespace ExtraDry.Core;

/// <summary>
/// Represents a basic query to filter against a list of items.
/// </summary>
public class FilterQuery {

    public FilterQuery() { 
    }

    /// <summary>
    /// Forces the string comparisons in the query are built using an explicit string comparison.
    /// This _will_ break entity framework if attempted to be sent to a database.
    /// Only use this if the database is known to be in-memory.
    /// </summary>
    public FilterQuery(StringComparison forceInMemoryStringComparison)
    {
        ForceStringComparison = forceInMemoryStringComparison;
    }

    /// <summary>
    /// The entity specific text filter for the collection.
    /// </summary>
    public string? Filter { get; set; } 

    /// <summary>
    /// If the request would like sorted results, the name of the property to sort by.
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Indicates if the results are requested in ascending order by `Sort`.
    /// </summary>
    public bool Ascending { get; set; }

    public StringComparison? ForceStringComparison { get; private set; }

}
