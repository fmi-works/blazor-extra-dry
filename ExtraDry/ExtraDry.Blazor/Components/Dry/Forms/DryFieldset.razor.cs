namespace ExtraDry.Blazor.Forms;

/// <summary>
/// A representation of a fieldset in a form, typically rendered automatically by an enclosing
/// <see cref="DryForm{T}"/>.  This component may be used to group a set of properties together on
/// a form with a legend.  It may also be used to group a set of child items in a list, where each
/// has their own small form or rendering.
/// </summary>
public partial class DryFieldset<T> : ComponentBase, IExtraDryComponent {

    /// <inheritdoc />
    [Parameter]
    public string CssClass { get; set; } = string.Empty;

    /// <inheritdoc />
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? UnmatchedAttributes { get; set; }

    /// <summary>
    /// Cascading parameter so the model can be internal and not exposed to users.
    /// </summary>
    [CascadingParameter]
    internal DryForm<T> Form { get; set; } = null!;

    /// <summary>
    /// Cascading parameter so the model can be internal and not exposed to users.
    /// </summary>
    [CascadingParameter]
    internal FormFieldset FormFieldset { get; set; } = null!;

    /// <summary>
    /// Event that is raised when the input is validated using internal rules. Does not check
    /// global rules that might be set on the model using data annotations.
    /// </summary>
    [Parameter]
    public EventCallback<ValidationEventArgs> OnValidationChanged { get; set; }

    private string CssClasses => DataConverter.JoinNonEmpty(" ", "dry-fieldset", Form.ModelNameSlug, FormFieldset.CssClass, CssClass);

    private CommandInfo AddNewCommand =>
        new(this, MethodInfoHelper.GetMethodInfo<DryFieldset<T>>(e => e.AddDefaultElementToList(Array.Empty<int>()))) {
            Arguments = CommandArguments.Single,
            Context = CommandContext.Alternate
        };

    [Command(Name = "Add New", Icon = "plus")]
    private void AddDefaultElementToList(IList items)
    {
        var type = items.GetType().SingleGenericType();
        var instance = type.CreateDefaultInstance();
        items.Add(instance);
        if(Form.Description != null && Form.Model != null) {
            Form.FormDescription = new FormDescription(Form.Description, Form.Model); // re-build description to add/remove UI elements.
        }
        StateHasChanged();
    }

    private void DeleteItem(object? items, object item)
    {
        Console.WriteLine("DeleteItem");
        ArgumentNullException.ThrowIfNull(items);
        if(items is IList list) {
            Console.WriteLine("  A list");
            list.Remove(item);
            if(Form.Description != null && Form.Model != null) {
                Form.FormDescription = new FormDescription(Form.Description, Form.Model); // re-build description to add/remove UI elements.
            }
            StateHasChanged();
        }
        else {
            Console.WriteLine("  Not a list: " + items.GetType().ToString());
        }
    }

}
