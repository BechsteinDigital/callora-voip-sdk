namespace CalloraVoipSdk.Modules;

/// <summary>
/// Generic module operation result used by SDK module facades.
/// </summary>
public readonly record struct ModuleOperationResult(bool IsSuccess, string? Message = null)
{
    public static ModuleOperationResult Success() => new(true, null);

    public static ModuleOperationResult Failure(string? message) => new(false, message);
}

/// <summary>
/// Thrown when a module is not available in the current runtime.
/// </summary>
public sealed class ModuleFeatureUnavailableException : InvalidOperationException
{
    public ModuleFeatureUnavailableException(string moduleId)
        : base($"Module '{moduleId}' is not available in this SDK runtime.")
    {
    }
}
