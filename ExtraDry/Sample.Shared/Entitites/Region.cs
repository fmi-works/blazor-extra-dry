﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample.Shared;

/// <summary>
/// Represents a single geo-political region in a taxonomy of geo-political regions.
/// </summary>
[DeleteRule(DeleteAction.Recycle, nameof(DeleteStatus), DeleteStatus.Recycled, DeleteStatus.Live)]
[Index(nameof(Uuid), IsUnique = true)]
public partial class Region : TaxonomyEntity<Region>, ITaxonomyEntity, IValidatableObject {

    /// <inheritdoc cref="ITaxonomyEntity.Id"/>
    [Key]
    [JsonIgnore]
    public int Id { get; set; }

    /// <inheritdoc cref="IResourceIdentifiers.Uuid" />
    [Sort(SortType.Sortable)] // test a normally excempt rule being included in the sort.
    public Guid Uuid { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The level for this region inside a taxonomy of regions.
    /// </summary>
    [Display(Name = "Level", ShortName = "Level")]
    [Filter]
    public RegionLevel Level { get; set; }

    [Filter]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonConverter(typeof(ResourceReferenceConverter<Region>))]
    public override Region? Parent { get => base.Parent; set => base.Parent = value; }

    [JsonIgnore]
    public HierarchyId? AncestorList { get; set; } = HierarchyId.GetRoot();

    /// <summary>
    /// The strata for the entity in the taxonomy, 0 is root, each level adds 1.
    /// </summary>
    [JsonIgnore]
    public int Strata => (int)Level;

    /// <summary>
    /// The code for the region, using the ISO-3166 standard.
    /// Use alpha-2 codes for country, then country specific codes.  E.g. "AU", then "AU-QLD", then "AU-QLD-Brisbane".
    /// </summary>
    [Required, StringLength(32)]
    [Display(ShortName = "Code")]
    [Filter]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// The short name of the country or region, such as 'Australia', or 'USA'.
    /// </summary>
    [Required, StringLength(32)]
    [Display(ShortName = "Title")]
    [Filter]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The full name of the country or region, such as 'Commonwealth of Australia', or 'United States of America'.
    /// </summary>
    /// <remarks>
    /// Limited to 100 characters based on full names of countries which, in English, max at 59 characters per ISO.
    /// </remarks>
    [Required, StringLength(100)]
    [DefaultValue("Description")]
    public string Description { get; set; } = string.Empty;

    [NotMapped]
    public string Caption => $"Region {Slug}";

    [Required]
    [Display(Name = "Status", ShortName = "Status")]
    public RegionStatus Status { get; set; } = RegionStatus.Active;

    [Sort(SortType.NotSortable)] // test for a valid field being suppressed from sorting.
    public DeleteStatus IsDeleted { get; set; }

    [JsonIgnore]
    public VersionInfo Version { get; set; } = new();

    /// <summary>
    /// Validates the code is in the right ISO-3166 format based on the level of the region.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        var codeRegex = Level switch {
            RegionLevel.Country => CountryRegex(),
            RegionLevel.Division => DivisionRegex(),
            _ => SubdivisionRegex(),
        };
        if(Level != RegionLevel.Global && !codeRegex.IsMatch(Slug)) {
            results.Add(new ValidationResult("Code must follow ISO-3166 naming scheme, e.g. 'AU', 'AU-QLD', 'AU-QLD-Brisbane'."));
        }
        return results;
    }

    [GeneratedRegex(@"^\w{2}$", RegexOptions.Compiled)]
    private partial Regex CountryRegex();

    [GeneratedRegex(@"^\w{2}-\w{2,4}$", RegexOptions.Compiled)]
    private partial Regex DivisionRegex();

    [GeneratedRegex(@"^\w{2}-\w{2,4}-\w{2,20}$", RegexOptions.Compiled)]
    private partial Regex SubdivisionRegex();

    public override bool Equals(object? obj)
    {
        var obj1 = obj as Region;
        return obj1?.Slug == Slug;
    }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DeleteStatus
{
    Live,
    Recycled, 
}
