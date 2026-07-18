namespace Dreamine.PLC.Omron.CxComponent.Internal;

/// <summary>
/// \if KO
/// <para>설치된 COM 개체를 ProgID로 생성합니다.</para>
/// \endif
/// \if EN
/// <para>Creates installed COM objects through their ProgID.</para>
/// \endif
/// </summary>
public sealed class DefaultComObjectFactory : IComObjectFactory
{
    /// <summary>
    /// \if KO
    /// <para>지정한 ProgID에서 CX-Compolet COM 개체를 생성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates a CX-Compolet COM object from the specified ProgID.</para>
    /// \endif
    /// </summary>
    /// <param name="progId">
    /// \if KO
    /// <para>생성할 COM 클래스의 ProgID입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The ProgID of the COM class to create.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>생성된 후기 바인딩 COM 개체입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The created late-bound COM object.</para>
    /// \endif
    /// </returns>
    /// <exception cref="PlatformNotSupportedException">
    /// \if KO
    /// <para>Windows가 아닌 플랫폼에서 호출할 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when called on a platform other than Windows.</para>
    /// \endif
    /// </exception>
    /// <exception cref="ArgumentException">
    /// \if KO
    /// <para><paramref name="progId"/>가 비어 있거나 공백일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="progId"/> is empty or whitespace.</para>
    /// \endif
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// \if KO
    /// <para>COM 클래스가 등록되지 않았거나 개체를 생성할 수 없을 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the COM class is not registered or cannot be created.</para>
    /// \endif
    /// </exception>
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

    /// <summary>
    /// \if KO
    /// <para>COM 등록 또는 생성 실패를 프로세스 비트 수 안내가 포함된 예외로 변환합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Converts a COM registration or creation failure into an exception with process-bitness guidance.</para>
    /// \endif
    /// </summary>
    /// <param name="progId">
    /// \if KO
    /// <para>생성에 실패한 COM ProgID입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The COM ProgID that could not be created.</para>
    /// \endif
    /// </param>
    /// <param name="innerException">
    /// \if KO
    /// <para>원래 발생한 예외이며 없으면 <see langword="null"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The original exception, or <see langword="null"/> when unavailable.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>진단 안내와 원본 예외를 포함한 예외입니다.</para>
    /// \endif
    /// \if EN
    /// <para>An exception containing diagnostic guidance and the original exception.</para>
    /// \endif
    /// </returns>
    private static InvalidOperationException CreateFriendlyException(string progId, Exception? innerException = null)
    {
        var bitness = Environment.Is64BitProcess ? "x64" : "x86";
        return new InvalidOperationException(
            $"CX-Compolet COM '{progId}' is not registered for the current {bitness} process. " +
            "Install/register Omron CX-Compolet for the same bitness, or run SampleSmart as x86 when only the 32-bit runtime is installed.",
            innerException);
    }
}
