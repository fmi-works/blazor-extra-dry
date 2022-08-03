﻿#nullable enable

namespace ExtraDry.Blazor;

/// <summary>
/// A flexi alternative to a select control.  Creates a semantic HTML control
/// with extended capabilities for generating single and multiple select 
/// controls on mobile and desktop platforms.  Includes list management and
/// filtering.
/// </summary>
/// <typeparam name="T">The type for items in the select list.</typeparam>
public partial class FlexiSelect<T> : ComponentBase {

    /// <inheritdoc cref="MiniDialog.CssClass" />
    [Parameter]
    public string CssClass { get; set; } = string.Empty;

    /// <inheritdoc cref="FlexiSelectForm{T}.MultiSelect" />
    [Parameter]
    public bool MultiSelect { get; set; }

    /// <inheritdoc cref="FlexiSelectForm{T}.Data" />
    [Parameter]
    public IEnumerable<T>? Data { get; set; }

    /// <inheritdoc cref="MiniDialog.LoseFocusAction" />
    [Parameter]
    public MiniDialogAction LoseFocusAction { get; set; } = MiniDialogAction.Disabled;

    /// <summary>
    /// Event that is fired when the button is clicked and the flexi select has
    /// been displayed.  Will be followed with OnSubmit or OnCancel when user
    /// is finished with dialog.
    /// </summary>    
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <inheritdoc cref="MiniDialog.OnSubmit" />
    [Parameter]
    public EventCallback<DialogEventArgs> OnSubmit { get; set; }
  
    /// <inheritdoc cref="MiniDialog.OnCancel" />
    [Parameter]
    public EventCallback<DialogEventArgs> OnCancel { get; set; }

    /// <inheritdoc cref="MiniDialog.AnimationDuration" />
    [Parameter]
    public int AnimationDuration { get; set; } = 100;

    [Parameter]
    public T? Value { get; set; }

    [Parameter]
    public EventCallback<T?> ValueChanged { get; set; }

    private async Task DoClick(MouseEventArgs args)
    {
        await MiniDialog.Show();
        await OnClick.InvokeAsync(args);
    }

    private MiniDialog MiniDialog { get; set; } = null!;


}
