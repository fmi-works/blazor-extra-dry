﻿namespace Sample.Data.Services;

public class RegionService {

    public RegionService(SampleContext sampleContext, RuleEngine ruleEngine)
    {
        database = sampleContext;
        rules = ruleEngine;
    }

    public async Task<FilteredCollection<Region>> ListAsync(FilterQuery query)
    {
        return await database.Regions
            .Include(e => e.Parent)
            .QueryWith(query)
            .ToFilteredCollectionAsync();
    }

    public async Task<FilteredCollection<Region>> ListChildrenAsync(string code)
    {
        throw new NotImplementedException();
        //return await database.Regions
        //    .QueryWith(new(), e => e.Ancestors.Any(f => f.Slug == code && (int)f.Level + 1 == (int)e.Level))
        //    .ToFilteredCollectionAsync();
    }

    public async Task CreateAsync(Region item)
    {
        Region? parent = null;
        if(item.Level != RegionLevel.Global) {
            if(item.Parent == null) {
                throw new ArgumentException("A region must have a parent if it is not at the global level.");
            }
            parent = await TryRetrieveAsync(item.Parent.Slug);
        }
        item.SetParent(parent);

        database.Regions.Add(item);

        await database.SaveChangesAsync();

        await database.Database.ExecuteSqlRawAsync(string.Format(AddToEndOfChildren, parent?.Id.ToString() ?? "null", item.Id ));
    }

    public async Task<Region?> TryRetrieveAsync(string code)
    {
        return await database.Regions
            .Include(e => e.Parent)
            .FirstOrDefaultAsync(e => e.Slug == code);
    }

    public async Task<Region> RetrieveAsync(string code)
    {
        var result = await TryRetrieveAsync(code)
            ?? throw new ArgumentOutOfRangeException(nameof(code));
        return result;
    }

    /// <summary>
    /// Updates the provided Region. If allowMove is set to true it also allows this Region to be reparented.
    /// </summary>
    /// <param name="slug">The slug that is used to identify the region to update</param>
    /// <param name="item">The item to update</param>
    /// <param name="allowMove">If false (default), any parent changes will be ignored. If true, will attempt to reparent the provided item to the provided parent.</param>
    /// <returns></returns>
    public async Task UpdateAsync(string slug, Region item, bool allowMove = false)
    {        
        var existing = await RetrieveAsync(slug);
        await database.Database.BeginTransactionAsync();
        try {
            if(allowMove && existing.Parent != null && item.Parent != null && existing.Parent.Slug != item.Parent.Slug) {
                var newParent = await RetrieveAsync(item.Parent.Slug);
                existing.Parent = newParent;

            }
            await rules.UpdateAsync(item, existing);
            await database.SaveChangesAsync();

            existing.SetParent(existing.Parent);

            await database.Database.CommitTransactionAsync();
        }
        catch {
            await database.Database.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task DeleteAsync(string code)
    {
        var existing = await RetrieveAsync(code);
        await rules.DeleteAsync(existing, () => database.Regions.Remove(existing), () => database.SaveChangesAsync());
    }

    public async Task RestoreAsync(string code)
    {
        var existing = await RetrieveAsync(code);
        await rules.RestoreAsync(existing);
        await database.SaveChangesAsync();
    }

    private readonly SampleContext database;

    private readonly RuleEngine rules;

    const string AddToEndOfChildren =
    @"update regions
set AncestorList =
	coalesce(
	(select top 1 AncestorList from Regions where Id = {0}).GetDescendant(
		nullif((select top 1 AncestorList from Regions where parentid = {0} order by AncestorList desc), CAST('/' as hierarchyid))
		, NULL
	), CAST('/' as hierarchyid))
 where id = {1}";

}
