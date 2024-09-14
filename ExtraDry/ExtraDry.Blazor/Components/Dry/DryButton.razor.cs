using ExtraDry.Blazor.Components.Internal;

namespace ExtraDry.Blazor;

/// <summary>
/// An Extra Dry button executes a command, typically from a ViewModel, with a Model as the argument.
/// This is typically used from within `DryCommandBar` or `DryForm`, but can be used directly.
/// If using manually, populate ViewModel and MethodName, and optionally Model.
/// </summary>
public partial class DryButton : ComponentBase, IExtraDryComponent {

    /// <inheritdoc />
    [Parameter]
    public string CssClass { get; set; } = string.Empty;

    /// <summary>
    /// A link to the ViewModel that contains the method to call on the button click.  Typically
    /// `this`.  When used, must also be partnered with `MethodName` being set.
    /// </summary>
    [Parameter]
    public object? ViewModel { get; set; }

    /// <summary>
    /// When defining the method using the ViewModel parameter, the method name to call.  The
    /// method should either take no parameters, a single Model or an enumeration of Models.
    /// </summary>
    [Parameter]
    public string? MethodName { get; set; }

    /// <summary>
    /// A CommandInfo object that defines the view model and method.  This parameter is mutually
    /// exclusive from the ViewModel/MethodName.  Use CommandInfo constructors to create this 
    /// command if using DryButton directly, typically this parameter is only used if the caller
    /// already has the CommandInfo from a ViewModelDescription.
    /// </summary>
    [Parameter]
    public CommandInfo? Command { get; set; }

    /// <summary>
    /// The information, retrieved through reflection, about the method to execute, along with 
    /// display attributes.  
    /// </summary>
    //[Parameter]
    private CommandInfo? ResolvedCommand { get; set; }

    /// <summary>
    /// The optional argument for the command if the command takes one.  Typically the model or
    /// models for the command are determined based on the context of the page and this is not set
    /// directly.  When unset, the active `SelectionSet` will have one or models that will 
    /// determine the arguments for the method.
    /// </summary>
    [Parameter]
    public object? Model { get; set; }

    /// <summary>
    /// If both an icon and a caption are available (as defined in the `CommandAttribute` on the method), then display as an Icon only.
    /// </summary>
    [Parameter]
    public bool IconOnly { get; set; }

    /// <summary>
    /// Set to enable/disable the button. Set to `null` (the default) to use the existence of Models
    /// in the `SelectionSet` to determine if the button should be enabled.
    /// </summary>
    [Parameter]
    public bool? Enabled { get; set; }

    /// <inheritdoc />
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? UnmatchedAttributes { get; set; }

    /// <summary>
    /// A function that is called before the click action is executed. Used for pre-execution validation.
    /// </summary>
    [Parameter]
    public Action<CommandContext>? PreClickCheck { get; set; }

    [CascadingParameter]
    protected SelectionSet? Selection { get; set; }

    [Inject]
    protected ILogger<DryButton> Logger { get; set; } = null!;

    private string CssClasses => DataConverter.JoinNonEmpty(" ", CssClass, ResolvedCommand?.DisplayClass);

    private SelectionSet? selection;

    private bool HasIcon => !string.IsNullOrWhiteSpace(ResolvedCommand?.Icon);

    private bool ShowCaption => !string.IsNullOrWhiteSpace(ResolvedCommand?.Caption) && !(HasIcon && IconOnly);

    private string ButtonCaption => ShowCaption ? ResolvedCommand?.Caption! : string.Empty;

    protected override void OnParametersSet()
    {
        if(Command is not null) {
            if(ViewModel is not null || MethodName is not null) {
                Logger.LogConsoleWarning("Command should be used mutually exclusively with ViewModel and MethodName in DryButton.  The Command property takes precedence and the ViewModel and MethodName will be ignored.");
            }
            ResolvedCommand = Command;
        }
        else if(ViewModel is not null || MethodName is not null) {
            if(ViewModel is null || MethodName is null) {
                Logger.LogConsoleError("When using ViewModel and MethodName to define a command, both must be set.  Button command can not be resolved.");
            }
            else {
                ResolvedCommand = new CommandInfo(ViewModel, MethodName);
            }
        }
        else {
            Logger.LogConsoleError("DryButton must define the command by either providing the Command parameter or both the ViewModel and the MethodName parameters.");
        }
    }

    protected override void OnInitialized()
    {
        if(Model is null) {
            selection = Selection ?? SelectionSet.Lookup(ResolvedCommand?.ViewModel ?? this);
            if(selection != null) {
                selection.Changed += SelectionChanged;
                UpdateDisabled();
            }
        }
    }

    private void SelectionChanged(object? sender, SelectionSetChangedEventArgs args)
    {
        UpdateDisabled();
    }

    private void UpdateDisabled()
    {
        if(ResolvedCommand is null) {
            SetEnabled(false);
        }
        else if(selection is null) {
            SetEnabled(true);
        }
        else if(ResolvedCommand.Arguments == CommandArguments.Single) {
            SetEnabled(selection.Single());
        }
        else if(ResolvedCommand.Arguments == CommandArguments.Multiple) {
            SetEnabled(selection.Any());
        }
    }

    private void SetEnabled(bool enabled)
    {
        var newDisabled = !enabled;
        if(disabled != newDisabled) {
            disabled = newDisabled;
            StateHasChanged();
        }
    }
    private bool disabled;

    private async Task DoClick(MouseEventArgs args)
    {
        if(ResolvedCommand is null) {
            return;
        }
        PreClickCheck?.Invoke(ResolvedCommand.Context);
        if(Model != null) {
            await ResolvedCommand.ExecuteAsync(Model);
        }
        else if(ResolvedCommand.Arguments == CommandArguments.Multiple && (selection?.Any() ?? false)) {
            await ResolvedCommand.ExecuteAsync(selection.Items);
        }
        else if(ResolvedCommand.Arguments == CommandArguments.Single && (selection?.Any() ?? false)) {
            await ResolvedCommand.ExecuteAsync(selection.Items.First());
        }
        else if(ResolvedCommand.Arguments == CommandArguments.None) {
            Console.WriteLine("No Args");
            await ResolvedCommand.ExecuteAsync();
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        if(selection != null) {
            selection.Changed -= SelectionChanged;
        }
    }

}
