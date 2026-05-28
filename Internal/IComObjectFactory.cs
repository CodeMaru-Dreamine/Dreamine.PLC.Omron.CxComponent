namespace Dreamine.PLC.Omron.CxComponent.Internal;

/// <summary>
/// Creates late-bound COM objects.
/// </summary>
public interface IComObjectFactory
{
    /// <summary>
    /// Creates a COM object from a ProgID.
    /// </summary>
    /// <param name="progId">The COM ProgID.</param>
    /// <returns>The created COM object.</returns>
    object Create(string progId);
}
