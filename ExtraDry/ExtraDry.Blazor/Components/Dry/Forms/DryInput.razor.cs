﻿namespace ExtraDry.Blazor.Forms;

/// <summary>
/// A single input field for a property of a model.  This is used by <see cref="DryForm{T}"/> to 
/// create all the inputs for a form.  For advanced use-cases, this component may be used directly 
/// and bound to any model, whether inside a <see cref="DryForm{T}"/> or not.
/// </summary>
public partial class DryInput<T> : OwningComponentBase, IDryInput<T>, IExtraDryComponent, IDisposable {

    /// <inheritdoc />
    [Parameter]
    public string CssClass { get; set; } = "";
    
    /// <inheritdoc />
    [Parameter, EditorRequired]
    public T? Model { get; set; }

    /// <inheritdoc />
    [Parameter]
    public PropertyDescription? Property { get; set; }

    /// <summary>
    /// If the <see cref="PropertyDescription"/> is not readily available, this can be used to 
    /// specify the name of the property to use.
    /// </summary>
    [Parameter]
    public string PropertyName { get; set; } = "";

    /// <inheritdoc />
    [Parameter]
    public EventCallback<ChangeEventArgs>? OnChange { get; set; }

    /// <inheritdoc />
    [CascadingParameter]
    public EditMode EditMode { get; set; } = EditMode.Create;

    /// <inheritdoc />
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? UnmatchedAttributes { get; set; }

    [Inject]
    private ILogger<DryInput<T>> Logger { get; set; } = null!;

    protected async override Task OnInitializedAsync()
    {
        Property ??= typeof(T).GetProperty(PropertyName) is PropertyInfo prop ? new PropertyDescription(prop) : null;
        if(Property?.Rules?.UpdateAction == RuleAction.Block) {
        }
        else if(Property?.HasTextRepresentation == false && Property?.HasDateTimeRepresentation == false && Property.HasNumericRepresentation == false) {
            await FetchLookupProviderOptions();
        }
    }

    private Dictionary<string, object>? LookupProviderOptions { get; set; }

    private List<object> LookupValues => LookupProviderOptions?.Values?.ToList() ?? new();

    private bool RulesAllowUpdate => Property?.Rules?.UpdateAction switch {
            RuleAction.Block => false,
            RuleAction.Ignore => false,
            _ => true,
        };

    private bool HasSetter => Property?.Property.CanWrite ?? false;

    private bool Editable => EditMode == EditMode.Create || EditMode == EditMode.Update && RulesAllowUpdate && HasSetter;

    private bool ReadOnly => !Editable;

    private string Value => Property?.DisplayValue(Model) ?? "";

    private string ValidationMessage { get; set; } = "";

    private bool Valid { get; set; } = true;

    private string CssClasses => DataConverter.JoinNonEmpty(" ", "field", SizeClass, Property?.DisplayClass, StateCss, ValidCss, CssClass);

    private string SizeClass => Property?.Size.ToString()?.ToLowerInvariant() ?? "";

    private bool ShowDescription { get; set; }

    private bool HasDescription => Property?.HasDescription ?? false;

    private async Task FetchLookupProviderOptions()
    {
        if(Property == null) {
            return;
        }
        var untypedOptionProvider = typeof(IOptionProvider<>);
        var propertyType = Property.Property.PropertyType;
        if(propertyType.IsAssignableTo(typeof(IList))) {
            propertyType = propertyType.GetGenericArguments().First();
        }
        var typedOptionProvider = untypedOptionProvider.MakeGenericType(propertyType);
        var optionProvider = ScopedServices.GetService(typedOptionProvider);
        if(optionProvider != null) {
            var method = typedOptionProvider.GetMethod("GetItemsAsync");
            var token = new CancellationTokenSource().Token;
            dynamic task = method!.Invoke(optionProvider, new object[] { token })!;
            var optList = (await task).Items as ICollection;
            var options = optList?.Cast<object>()?.ToList() ?? new();
            LookupProviderOptions = options
                .Select((e, i) => new { Key = i, Item = e })
                .ToDictionary(e => e.Key.ToString(CultureInfo.InvariantCulture), e => e.Item);
        }
        else {
            Logger.LogMissingOptionProvider(Property.Property.PropertyType.Name);
        }
    }

    private string TextDescription => Property?.Description ?? "";

    private string StateCss => (Editable, Property?.IsRequired) switch {
        (true, true) => "required",
        (true, false) => "optional",
        (_, _) => "readonly",
    };

    private string ValidCss => Valid ? " valid" : " invalid";

    private string HtmlDescription => TextDescription.Replace("-", "&#8209;"); // non-breaking-hyphen.

    private void ToggleDescription(MouseEventArgs args)
    {
        ShowDescription = !ShowDescription;
    }

    private async Task HandleChange(ChangeEventArgs args)
    {
        if(Property == null || Model == null) {
            return;
        }
        var value = args.Value;
        if(LookupProviderOptions != null && value is string strValue) {
            value = LookupProviderOptions[strValue];
        }
        Console.WriteLine($"Model: {Model} to Value: {value}");
        Property.SetValue(Model, value);
        Validate();
        var task = OnChange?.InvokeAsync(args);
        if(task != null) {
            await task;
        }
    }

    private async Task HandleClick(object selectValue)
    {
        if(Property == null || Model == null) {
            return;
        }
        var value = selectValue;
        //if(LookupProviderOptions != null && value is string strValue) {
        //    value = LookupProviderOptions[strValue];
        //}
        Console.WriteLine($"Model: {Model} to Value: {value}");
        Property.SetValue(Model, value);
        Validate();
        // Ignore that it's a physical click and treat like value change for listeners.
        var changeEventArgs = new ChangeEventArgs { Value = value };
        var task = OnChange?.InvokeAsync(changeEventArgs);
        if(task != null) {
            await task;
        }
        StateHasChanged();
    }

    private void Validate()
    {
        if(Property == null || Model == null) {
            return;
        }
        var validator = new DataValidator();
        if(validator.ValidateProperties(Model, Property.Property.Name)) {
            UpdateValidationUI(true, string.Empty);
        }
        else {
            UpdateValidationUI(false, string.Join("; ", validator.Errors.Select(e => e.ErrorMessage)));
        }
    }

    private Task ValidationChanged(ValidationEventArgs validation)
    {
        UpdateValidationUI(validation.IsValid, validation.Message);
        return Task.CompletedTask;
    }

    private void UpdateValidationUI(bool valid, string message)
    {
        ValidationMessage = message;
        Valid = valid;
        StateHasChanged();
    }

}
