namespace ExtraDry.Blazor;
public partial class DryStaticFilter<TItem> : ComponentBase, IExtraDryComponent
{
    public DryStaticFilter()
    {
        ViewModelDescription = new ViewModelDescription(typeof(TItem), this);
    }

    /// <inheritdoc cref="IExtraDryComponent.CssClass "/>
    [Parameter]
    public string CssClass { get; set; } = string.Empty;

    /// <inheritdoc cref="IExtraDryComponent.UnmatchedAttributes" />
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? UnmatchedAttributes { get; set; }

    /// <inheritdoc cref="DryPageQueryView.PageQueryBuilder" />
    [CascadingParameter]
    public QueryBuilder? QueryBuilder { get; set; }

    public void SetInitialLevel(int level)
    {
        QueryBuilder?.Level.SetInitialLevel(level);
    }

    private ViewModelDescription ViewModelDescription { get; set; }

    private string CssClasses => DataConverter.JoinNonEmpty(" ", "dry-static-filter", CssClass);
}
