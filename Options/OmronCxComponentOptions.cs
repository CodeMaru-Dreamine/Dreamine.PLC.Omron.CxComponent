namespace Dreamine.PLC.Omron.CxComponent.Options;

/// <summary>
/// Provides Omron CX-Compolet/CX-Component connection options.
/// </summary>
public sealed class OmronCxComponentOptions
{
    /// <summary>
    /// Gets or sets the CX-Compolet COM ProgID.
    /// </summary>
    public string ProgId { get; set; } = "OMRON.Compolet.CJ2Compolet";

    /// <summary>
    /// Gets or sets the peer address or node name property value.
    /// </summary>
    public string PeerAddress { get; set; } = "127.0.0.1";

    /// <summary>
    /// Gets or sets the property name used for the peer address.
    /// </summary>
    public string PeerAddressPropertyName { get; set; } = "PeerAddress";

    /// <summary>
    /// Gets or sets the active/connection property name.
    /// </summary>
    public string ActivePropertyName { get; set; } = "Active";

    /// <summary>
    /// Gets or sets the read variable method name.
    /// </summary>
    public string ReadVariableMethodName { get; set; } = "ReadVariable";

    /// <summary>
    /// Gets or sets the write variable method name.
    /// </summary>
    public string WriteVariableMethodName { get; set; } = "WriteVariable";
}
