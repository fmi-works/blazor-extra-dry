﻿using ExtraDry.Core.Models;
using System.Text.Json;

namespace ExtraDry.Blazor;

public partial class ValidationSummary : ComponentBase, IExtraDryComponent
{
    /// <inheritdoc />
    [Parameter]
    public string CssClass { get; set; } = string.Empty;

    /// <summary>
    /// If an exception was raised in the application, provides the details of the Exception.
    /// </summary>
    [Parameter]
    public Exception? Exception { get; set; }

    /// <summary>
    /// If ProblemDetails are available (per RFC7231) they are provided here.  This will be the 
    /// case with any RFC7231 APIs that return problem details on any 4xx response.
    /// </summary>
    [Parameter]
    public ProblemDetails? ProblemDetails { get; set; }

    /// <inheritdoc />
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? UnmatchedAttributes { get; set; }

    private string CssClasses => DataConverter.JoinNonEmpty(" ", "validation-summary", ErrorCss, CssClass);

    private string ErrorCss => IsValidationError ? $"status{ProblemDetails!.Status}": "unexpected-error";

    private bool IsValidationError => ProblemDetails != null && ProblemDetails.Status == (int)HttpStatusCode.BadRequest;

    public List<string> GetValidationMessages()
    {
        if(!IsValidationError) {
            // TODO: Propogate non validation exceptions to a higher ErrorBoundary?
            // Throwing the exception here doesn't get handled by DryErrorBoundary it goes unhandled and
            // the alert bar at bottom of the screen appears.
            // For now it's handled by the ValidationSummary to display a "A problem has occurred. Please try again" message
            // throw Exception;
            return new();
        }
        if(!ProblemDetails!.Extensions.TryGetValue("errors", out var errors)) { 
            return new(); 
        }

        var alertMessages = new List<string>();
        if(errors is not JsonElement) {
            return new();
        }
        var messages = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(errors.ToString() ?? string.Empty);
        if(messages == null || messages.Keys.Count == 0) {
            return new();
        }
        foreach(var group in messages.Keys) {
            foreach(var error in messages[group]) {
                alertMessages.Add($"{group} - {error}");
            }
        }

        return alertMessages;
    }
}
