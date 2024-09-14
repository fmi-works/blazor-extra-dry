namespace ExtraDry.Blazor;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "DRY1500:Extra DRY Blazor components should have an interface.", Justification = "Pending decision on fate of component.")]
public partial class DryButtonBar : ComponentBase {

    /// <summary>
    /// The information, retrieved through reflection, about the method to execute, along with display attributes.
    /// Use `CommandInfo` constructors to create this command if using `DryButton` directly.
    /// </summary>
    [Parameter]
    public IList<CommandInfo> Commands { get; set; } = [];

    /// <summary>
    /// The target (method argument) for the command.
    /// If not set, then the target will be inferred from the active `SelectionSet`.
    /// </summary>
    [Parameter]
    public object? Target { get; set; }

    [Parameter]
    public object? ViewModel { get; set; }

    /// <summary>
    /// Filter the view model's commands by this category.
    /// </summary>
    [Parameter]
    public string? Category { get; set; }

    /// <summary>
    /// A function that is called before the click action of a button is executed. Used for pre-execution validation.
    /// </summary>
    [Parameter]
    public Action<CommandContext>? PreClickCheck { get; set; }

    private ViewModelDescription? Description { get; set; }

    protected override void OnParametersSet()
    {
        if(!Commands.Any() && ViewModel != null && Description == null) {
            Description = new ViewModelDescription(ViewModel);
            Commands = Description.Commands;
        }
    }

    private IEnumerable<CommandInfo> SelectCommands(CommandContext context) => Commands
        .Where(e => e.Context == context)
        .Where(e => Category == null || Category == e.Category);
}
