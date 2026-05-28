namespace Dreamine.PLC.Omron.CxComponent.Internal;

/// <summary>
/// Creates installed COM objects through their ProgID.
/// </summary>
public sealed class DefaultComObjectFactory : IComObjectFactory
{
    /// <inheritdoc />
    public object Create(string progId)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("CX-Compolet controls are supported on Windows only.");
        }

        if (string.IsNullOrWhiteSpace(progId))
        {
            throw new ArgumentException("COM ProgID must not be empty.", nameof(progId));
        }

        Type? type;
        try
        {
            type = Type.GetTypeFromProgID(progId, throwOnError: false);
        }
        catch (Exception ex)
        {
            throw CreateFriendlyException(progId, ex);
        }

        if (type is null)
        {
            throw CreateFriendlyException(progId);
        }

        try
        {
            return Activator.CreateInstance(type)
                ?? throw new InvalidOperationException($"Failed to create CX-Compolet COM object: {progId}");
        }
        catch (Exception ex)
        {
            throw CreateFriendlyException(progId, ex);
        }
    }

    private static InvalidOperationException CreateFriendlyException(string progId, Exception? innerException = null)
    {
        var bitness = Environment.Is64BitProcess ? "x64" : "x86";
        return new InvalidOperationException(
            $"CX-Compolet COM '{progId}' is not registered for the current {bitness} process. " +
            "Install/register Omron CX-Compolet for the same bitness, or run SampleSmart as x86 when only the 32-bit runtime is installed.",
            innerException);
    }
}
