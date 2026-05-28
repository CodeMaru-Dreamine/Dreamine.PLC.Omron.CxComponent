using System.Globalization;
using Dreamine.PLC.Abstractions.Devices;

namespace Dreamine.PLC.Omron.CxComponent.Devices;

/// <summary>
/// Formats Dreamine PLC addresses for CX-Compolet variable/address strings.
/// </summary>
public static class OmronCxAddressNameFormatter
{
    /// <summary>
    /// Formats a PLC address for CX-Compolet.
    /// </summary>
    /// <param name="address">The PLC address.</param>
    /// <returns>The CX-Compolet address string.</returns>
    public static string Format(PlcAddress address)
    {
        var prefix = address.DeviceType switch
        {
            PlcDeviceType.D => "D",
            PlcDeviceType.M => "W",
            PlcDeviceType.X => "CIO",
            PlcDeviceType.Y => "CIO",
            PlcDeviceType.W => "W",
            PlcDeviceType.R => "H",
            _ => throw new NotSupportedException($"Unsupported CX-Compolet device type: {address.DeviceType}")
        };

        var offset = address.Offset.ToString(CultureInfo.InvariantCulture);
        return address.BitOffset.HasValue
            ? $"{prefix}{offset}.{address.BitOffset.Value}"
            : $"{prefix}{offset}";
    }

    /// <summary>
    /// Formats a PLC address after applying an offset delta.
    /// </summary>
    /// <param name="address">The start PLC address.</param>
    /// <param name="delta">The address delta.</param>
    /// <returns>The CX-Compolet address string.</returns>
    public static string FormatOffset(PlcAddress address, int delta)
    {
        return Format(address with { Offset = address.Offset + delta });
    }
}
