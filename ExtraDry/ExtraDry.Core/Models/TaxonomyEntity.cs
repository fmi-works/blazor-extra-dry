﻿namespace ExtraDry.Core;

/// <summary>
/// For an entity that exists in a hierarchy, creates extensions for an EF model to model it.
/// This uses a Closure table that tracks all ancestors and descendents between entities.
/// </summary>
public abstract class TaxonomyEntity<T> where T : TaxonomyEntity<T>, ITaxonomyEntity, IResourceIdentifiers {

    /// <summary>
    /// Useful for creating a new `T` which is the direct child of a parent `T`.
    /// If parent is null, then the entity is set at the root of the taxonomy.
    /// </summary>
    public void SetParent(T? parent)
    {
        if(parent != null) {
            if(parent.Strata >= ((ITaxonomyEntity)this).Strata) {
                throw new InvalidOperationException("Parent must be at a higher level than current entity.");
            }
            Ancestors.Clear();
            Ancestors.AddRange(parent.Ancestors.Where(a => a.Uuid != parent.Uuid));
            Ancestors.Add(parent);
        }
        if(this is T self) {
            Ancestors.Add(self);
        }
    }

    /// <summary>
    /// The immediate parent for this entity in the taxonomy.  Use to infer the tree structure 
    /// from multiple entities, and to re-parent an entity during an `Update`.
    /// </summary>
    /// <remarks>
    /// This is usefully for trivially communicating the taxonomy through an API, but is not a 
    /// good pattern for database structures. It requires that the Ancestors are loaded and then 
    /// calculates it on the fly.
    /// Derived classes should override this and replace with a JsonConverter to a ResourceReference.
    /// </remarks>
    [NotMapped]
    [JsonIgnore]
    public virtual T? Parent { 
        get {
            parent ??= Ancestors?.OrderByDescending(e => e.Strata)?.Skip(1).FirstOrDefault(); // Skip 1 to bypass the self-reference.
            return parent;
        }
        set => parent = value; 
    }
    private T? parent;

    /// <summary>
    /// The set of all ancestors for this entity.
    /// </summary>
    /// <remarks>
    /// This works with `Descendants` and auto-creation of join tables in EF to create a tree Closure table.
    /// </remarks>
    [JsonIgnore]
    public List<T> Ancestors { get; set; } = new();

    /// <summary>
    /// The set of all descendants for this entity.
    /// </summary>
    /// <remarks>
    /// This works with `Ancestors` and auto-creation of join tables in EF to create a tree Closure table.
    /// </remarks>
    [JsonIgnore]
    public List<T> Descendants { get; set; } = new();

}
