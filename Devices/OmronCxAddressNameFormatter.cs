using System.Globalization;
using Dreamine.PLC.Abstractions.Devices;

namespace Dreamine.PLC.Omron.CxComponent.Devices;

/// <summary>
/// \if KO
/// <para>Dreamine PLC 주소를 CX-Compolet 변수 또는 주소 문자열로 변환합니다.</para>
/// \endif
/// \if EN
/// <para>Formats Dreamine PLC addresses as CX-Compolet variable or address strings.</para>
/// \endif
/// </summary>
public static class OmronCxAddressNameFormatter
{
    /// <summary>
    /// \if KO
    /// <para>PLC 주소를 CX-Compolet 주소 문자열로 변환합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Formats a PLC address as a CX-Compolet address string.</para>
    /// \endif
    /// </summary>
    /// <param name="address">
    /// \if KO
    /// <para>변환할 PLC 주소입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The PLC address to format.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>CX-Compolet 주소 문자열입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The CX-Compolet address string.</para>
    /// \endif
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// \if KO
    /// <para>주소의 장치 형식을 CX-Compolet이 지원하지 않을 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the address uses a device type unsupported by CX-Compolet.</para>
    /// \endif
    /// </exception>
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
    /// \if KO
    /// <para>오프셋 증분을 적용한 PLC 주소를 CX-Compolet 주소 문자열로 변환합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Formats a PLC address as a CX-Compolet address string after applying an offset delta.</para>
    /// \endif
    /// </summary>
    /// <param name="address">
    /// \if KO
    /// <para>시작 PLC 주소입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The starting PLC address.</para>
    /// \endif
    /// </param>
    /// <param name="delta">
    /// \if KO
    /// <para>주소 오프셋에 더할 증분입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The delta to add to the address offset.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>증분이 적용된 CX-Compolet 주소 문자열입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The CX-Compolet address string with the delta applied.</para>
    /// \endif
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// \if KO
    /// <para>주소의 장치 형식을 CX-Compolet이 지원하지 않을 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the address uses a device type unsupported by CX-Compolet.</para>
    /// \endif
    /// </exception>
    public static string FormatOffset(PlcAddress address, int delta)
    {
        return Format(address with { Offset = address.Offset + delta });
    }
}
