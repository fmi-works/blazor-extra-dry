using ExtraDry.Blazor.Components.Internal;
using ExtraDry.Core.Models;

namespace ExtraDry.Blazor;

/// <summary>
/// Generates a generic form for the creation and updating of a model.  Form layout is based on 
/// the attributes of the fields of the model, as well as the optional view model.
/// </summary>
public partial class DryForm<T> : ComponentBase, IExtraDryComponent {

    /// <inheritdoc />
    [Parameter]
    public string CssClass { get; set; } = string.Empty;

    /// <summary>
    /// The target model for the form that is being edited.
    /// </summary>
    [Parameter, EditorRequired]
    public T? Model { get; set; }

    [Parameter]
    public object? ViewModel { get; set; }

    /// <inheritdoc cref="Blazor.EditMode"/>
    [Parameter]
    public EditMode EditMode { get; set; } = EditMode.Update;

    /// <summary>
    /// Represents the number of fieldsets that are rendered in the first collection of fieldsets.
    /// The remainder are rendered in a second collection of fieldsets.  CSS styles can render 
    /// these two separately, e.g. my making the second set scrollable.
    /// </summary>
    [Parameter]
    public int FixedFieldsets { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? UnmatchedAttributes { get; set; }

    [Inject]
    private ILogger<DryForm<T>> Logger { get; set; } = null!;

    internal string GetEntityInfoCaption()
    {
        string caption = string.Empty;
        if(ViewModel is IViewModelCaption viewModelCaption) {
            caption = viewModelCaption.Caption(ViewModel);
        }
        return string.IsNullOrEmpty(caption) ? Description?.ModelDisplayName ?? string.Empty : caption;
    }

    protected override void OnParametersSet()
    {
        if(ViewModel == null) {
            Logger.LogConsoleError("DryForm requires a ViewModel");
            return;
        }
        Description ??= new ViewModelDescription(typeof(T), ViewModel);
        if(Model != null) {
            Description = new ViewModelDescription(Model.GetType(), ViewModel); // override the Description with the actual Description when we have the Model, which will account for polymorphism issues
            FormDescription = new FormDescription(Description, Model);
        }
    }

    internal string ModelNameSlug => Slug.ToSlug(FormDescription?.ViewModelDescription?.ModelDisplayName ?? "");

    internal ViewModelDescription? Description { get; set; }

    internal FormDescription? FormDescription { get; set; }

    private string CssClasses => DataConverter.JoinNonEmpty(" ", "dry-form", ModelNameSlug, CssClass);

    private List<string> AlertMessages { get; set; } = [];

    private Task ValidationChanged(ValidationEventArgs validation)
    {
        // Validation event args only returns one validation error at a time, but we're ready for it to increase if it ever needs to.
        // This also simplifies later logic.
        if(validation.IsValid) {
            ClientValidationErrors.Remove(validation.MemberName);
        }
        else {
            if(ClientValidationErrors.ContainsKey(validation.MemberName)) {
                ClientValidationErrors[validation.MemberName] = [validation.Message];
            }
            else {
                ClientValidationErrors.Add(validation.MemberName, [validation.Message]);
            }
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Persists a collection of client side validation errors, some of which model validation will not catch.
    /// For example, inputting 31/02/2024 will not update the model becuase the 31st Feb doesn't exist, so cannot be stored in a DateTime object.
    /// Values are a List to simplify the later logic with the model validator
    /// </summary>
    private Dictionary<string, List<string>> ClientValidationErrors { get; set; } = [];

    private void PreSubmissionCheck(CommandContext context)
    {
        if(context == CommandContext.Alternate) {
            // Only validate on Primary, Default or Danger calls.
            return;
        }
        if(Model is null) {
            // No point trying to validate something that doesn't exist.
            return;
        }

        var validator = new DataValidator();
        validator.ValidateObject(Model);
        if(validator.Errors.Count == 0 && ClientValidationErrors.Count == 0) {
            // Valid, continue.
            return;
        }

        // Else, get union of model and UI validation issues and throw.
        var union = new Dictionary<string, List<string>>(ClientValidationErrors);
        foreach(var error in validator.Errors) {
            foreach(var member in error.MemberNames) {
                if(union.TryGetValue(member, out List<string>? messages)) {
                    messages.Add(error.ErrorMessage ?? "Is Invalid");
                } else {
                    union.Add(member, [error.ErrorMessage ?? "Is Invalid"]);
                }
            }
        }

        var problem = new ProblemDetails() {
            Title = "One or more validation errors occurred.",
            Detail = "Client side validation"
        };
        problem.Extensions.Add("errors", union);
        // Add a value to tell the validation that this is a client side validation error, not server side (which would have a 400 status)
        problem.Extensions.Add("source", "client"); 

        throw new DryException(problem);
    }
}
