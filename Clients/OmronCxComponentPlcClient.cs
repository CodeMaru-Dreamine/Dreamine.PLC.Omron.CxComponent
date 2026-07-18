using System.Globalization;
using System.Runtime.InteropServices;
using Dreamine.PLC.Abstractions.Devices;
using Dreamine.PLC.Abstractions.Results;
using Dreamine.PLC.Core.Clients;
using Dreamine.PLC.Omron.CxComponent.Devices;
using Dreamine.PLC.Omron.CxComponent.Internal;
using Dreamine.PLC.Omron.CxComponent.Options;

namespace Dreamine.PLC.Omron.CxComponent.Clients;

/// <summary>
/// \if KO
/// <para>Omron CX-Compolet/CX-Component COM 인터페이스를 사용하는 PLC 클라이언트를 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides a PLC client that uses the Omron CX-Compolet/CX-Component COM interface.</para>
/// \endif
/// </summary>
public sealed class OmronCxComponentPlcClient : PlcClientBase
{
    /// <summary>
    /// \if KO
    /// <para>options 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the options value.</para>
    /// \endif
    /// </summary>
    private readonly OmronCxComponentOptions _options;
    /// <summary>
    /// \if KO
    /// <para>factory 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the factory value.</para>
    /// \endif
    /// </summary>
    private readonly IComObjectFactory _factory;
    /// <summary>
    /// \if KO
    /// <para>component 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the component value.</para>
    /// \endif
    /// </summary>
    private object? _component;

    /// <summary>
    /// \if KO
    /// <para>기본 COM 개체 팩터리를 사용해 <see cref="OmronCxComponentPlcClient"/> 클래스의 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new instance of <see cref="OmronCxComponentPlcClient"/> using the default COM object factory.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>CX-Compolet 연결 및 메서드 설정입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The CX-Compolet connection and method settings.</para>
    /// \endif
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="options"/>가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="options"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    public OmronCxComponentPlcClient(OmronCxComponentOptions options)
        : this(options, new DefaultComObjectFactory())
    {
    }

    /// <summary>
    /// \if KO
    /// <para>지정한 COM 개체 팩터리를 사용해 <see cref="OmronCxComponentPlcClient"/> 클래스의 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new instance of <see cref="OmronCxComponentPlcClient"/> using the specified COM object factory.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>CX-Compolet 연결 및 메서드 설정입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The CX-Compolet connection and method settings.</para>
    /// \endif
    /// </param>
    /// <param name="factory">
    /// \if KO
    /// <para>후기 바인딩 COM 개체를 생성할 팩터리입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The factory used to create the late-bound COM object.</para>
    /// \endif
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="options"/> 또는 <paramref name="factory"/>가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="options"/> or <paramref name="factory"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    public OmronCxComponentPlcClient(OmronCxComponentOptions options, IComObjectFactory factory)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    /// <summary>
    /// \if KO
    /// <para>이 클라이언트가 사용하는 CX-Compolet 옵션을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the CX-Compolet options used by this client.</para>
    /// \endif
    /// </summary>
    public OmronCxComponentOptions Options => _options;

    /// <summary>
    /// \if KO
    /// <para>COM 개체를 생성하고 상대 주소와 활성 상태를 설정해 CX-Compolet 연결을 엽니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates the COM object and opens the CX-Compolet connection by configuring its peer address and active state.</para>
    /// \endif
    /// </summary>
    /// <param name="cancellationToken">
    /// \if KO
    /// <para>연결 작업을 취소하는 토큰입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A token that cancels the connection operation.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>연결 성공 결과를 포함하는 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task containing the successful connection result.</para>
    /// \endif
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// \if KO
    /// <para>취소 토큰이 취소된 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the cancellation token has been canceled.</para>
    /// \endif
    /// </exception>
    /// <exception cref="PlatformNotSupportedException">
    /// \if KO
    /// <para>기본 팩터리를 Windows가 아닌 플랫폼에서 사용할 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the default factory is used on a non-Windows platform.</para>
    /// \endif
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// \if KO
    /// <para>COM 개체 생성 또는 필수 활성 속성 설정이 실패할 때 발생할 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown when COM object creation or required active-property assignment fails.</para>
    /// \endif
    /// </exception>
    protected override Task<PlcResult> ConnectCoreAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _component = _factory.Create(_options.ProgId);
        TrySetProperty(_component, _options.PeerAddressPropertyName, _options.PeerAddress);
        ComInvoker.SetProperty(_component, _options.ActivePropertyName, true);

        return Task.FromResult(PlcResult.Success());
    }

    /// <summary>
    /// \if KO
    /// <para>가능한 경우 활성 속성을 해제하고 보유한 CX-Compolet COM 개체를 해제합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Deactivates the connection when supported and releases the owned CX-Compolet COM object.</para>
    /// \endif
    /// </summary>
    /// <param name="cancellationToken">
    /// \if KO
    /// <para>연결 해제 작업을 취소하는 토큰입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A token that cancels the disconnection operation.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>연결 해제 성공 결과를 포함하는 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task containing the successful disconnection result.</para>
    /// \endif
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// \if KO
    /// <para>취소 토큰이 취소된 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the cancellation token has been canceled.</para>
    /// \endif
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// \if KO
    /// <para>활성 속성 설정자가 존재하지만 내부 COM 오류가 발생할 때 발생할 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown when the active-property setter exists but raises an inner COM error.</para>
    /// \endif
    /// </exception>
    protected override Task<PlcResult> DisconnectCoreAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_component is null)
        {
            return Task.FromResult(PlcResult.Success());
        }

        TrySetProperty(_component, _options.ActivePropertyName, false);
        ReleaseComponent();
        return Task.FromResult(PlcResult.Success());
    }

    /// <summary>
    /// \if KO
    /// <para>연속된 PLC 비트 변수를 CX-Compolet 단건 호출로 읽습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Reads consecutive PLC bit variables using individual CX-Compolet calls.</para>
    /// \endif
    /// </summary>
    /// <param name="address">
    /// \if KO
    /// <para>읽기를 시작할 PLC 주소입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The PLC address at which to begin reading.</para>
    /// \endif
    /// </param>
    /// <param name="count">
    /// \if KO
    /// <para>읽을 비트 수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The number of bits to read.</para>
    /// \endif
    /// </param>
    /// <param name="cancellationToken">
    /// \if KO
    /// <para>읽기 작업을 취소하는 토큰입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A token that cancels the read operation.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>읽은 비트 배열 또는 포착된 호출 오류를 포함하는 결과 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task containing the bit array or a captured invocation error.</para>
    /// \endif
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// \if KO
    /// <para>클라이언트가 연결되지 않았을 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the client is not connected.</para>
    /// \endif
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// \if KO
    /// <para>취소 토큰이 취소된 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the cancellation token has been canceled.</para>
    /// \endif
    /// </exception>
    protected override Task<PlcResult<bool[]>> ReadBitsCoreAsync(
        PlcAddress address,
        int count,
        CancellationToken cancellationToken)
    {
        var component = RequireComponent();
        var values = new bool[count];

        try
        {
            for (var index = 0; index < count; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var raw = ComInvoker.Invoke(
                    component,
                    _options.ReadVariableMethodName,
                    OmronCxAddressNameFormatter.FormatOffset(address, index));

                values[index] = Convert.ToInt32(raw, CultureInfo.InvariantCulture) != 0;
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Task.FromResult(PlcResult<bool[]>.Failure($"CX-Compolet bit read failed. {ex.Message}"));
        }

        return Task.FromResult(PlcResult<bool[]>.Success(values));
    }

    /// <summary>
    /// \if KO
    /// <para>연속된 PLC 워드 변수를 CX-Compolet 단건 호출로 읽습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Reads consecutive PLC word variables using individual CX-Compolet calls.</para>
    /// \endif
    /// </summary>
    /// <param name="address">
    /// \if KO
    /// <para>읽기를 시작할 PLC 주소입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The PLC address at which to begin reading.</para>
    /// \endif
    /// </param>
    /// <param name="count">
    /// \if KO
    /// <para>읽을 워드 수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The number of words to read.</para>
    /// \endif
    /// </param>
    /// <param name="cancellationToken">
    /// \if KO
    /// <para>읽기 작업을 취소하는 토큰입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A token that cancels the read operation.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>읽은 워드 배열 또는 포착된 호출 오류를 포함하는 결과 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task containing the word array or a captured invocation error.</para>
    /// \endif
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// \if KO
    /// <para>클라이언트가 연결되지 않았을 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the client is not connected.</para>
    /// \endif
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// \if KO
    /// <para>취소 토큰이 취소된 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the cancellation token has been canceled.</para>
    /// \endif
    /// </exception>
    protected override Task<PlcResult<short[]>> ReadWordsCoreAsync(
        PlcAddress address,
        int count,
        CancellationToken cancellationToken)
    {
        var component = RequireComponent();
        var values = new short[count];

        try
        {
            for (var index = 0; index < count; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var raw = ComInvoker.Invoke(
                    component,
                    _options.ReadVariableMethodName,
                    OmronCxAddressNameFormatter.FormatOffset(address, index));

                values[index] = Convert.ToInt16(raw, CultureInfo.InvariantCulture);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Task.FromResult(PlcResult<short[]>.Failure($"CX-Compolet word read failed. {ex.Message}"));
        }

        return Task.FromResult(PlcResult<short[]>.Success(values));
    }

    /// <summary>
    /// \if KO
    /// <para>연속된 PLC 비트 변수를 CX-Compolet 단건 호출로 씁니다.</para>
    /// \endif
    /// \if EN
    /// <para>Writes consecutive PLC bit variables using individual CX-Compolet calls.</para>
    /// \endif
    /// </summary>
    /// <param name="address">
    /// \if KO
    /// <para>쓰기를 시작할 PLC 주소입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The PLC address at which to begin writing.</para>
    /// \endif
    /// </param>
    /// <param name="values">
    /// \if KO
    /// <para>쓸 비트 값입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The bit values to write.</para>
    /// \endif
    /// </param>
    /// <param name="cancellationToken">
    /// \if KO
    /// <para>쓰기 작업을 취소하는 토큰입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A token that cancels the write operation.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>성공 또는 포착된 호출 오류를 포함하는 결과 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task containing success or a captured invocation error.</para>
    /// \endif
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// \if KO
    /// <para>클라이언트가 연결되지 않았을 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the client is not connected.</para>
    /// \endif
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// \if KO
    /// <para>취소 토큰이 취소된 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the cancellation token has been canceled.</para>
    /// \endif
    /// </exception>
    protected override Task<PlcResult> WriteBitsCoreAsync(
        PlcAddress address,
        IReadOnlyList<bool> values,
        CancellationToken cancellationToken)
    {
        var component = RequireComponent();

        try
        {
            for (var index = 0; index < values.Count; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ComInvoker.Invoke(
                    component,
                    _options.WriteVariableMethodName,
                    OmronCxAddressNameFormatter.FormatOffset(address, index),
                    values[index] ? 1 : 0);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Task.FromResult(PlcResult.Failure($"CX-Compolet bit write failed. {ex.Message}"));
        }

        return Task.FromResult(PlcResult.Success());
    }

    /// <summary>
    /// \if KO
    /// <para>연속된 PLC 워드 변수를 CX-Compolet 단건 호출로 씁니다.</para>
    /// \endif
    /// \if EN
    /// <para>Writes consecutive PLC word variables using individual CX-Compolet calls.</para>
    /// \endif
    /// </summary>
    /// <param name="address">
    /// \if KO
    /// <para>쓰기를 시작할 PLC 주소입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The PLC address at which to begin writing.</para>
    /// \endif
    /// </param>
    /// <param name="values">
    /// \if KO
    /// <para>쓸 워드 값입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The word values to write.</para>
    /// \endif
    /// </param>
    /// <param name="cancellationToken">
    /// \if KO
    /// <para>쓰기 작업을 취소하는 토큰입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A token that cancels the write operation.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>성공 또는 포착된 호출 오류를 포함하는 결과 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task containing success or a captured invocation error.</para>
    /// \endif
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// \if KO
    /// <para>클라이언트가 연결되지 않았을 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the client is not connected.</para>
    /// \endif
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// \if KO
    /// <para>취소 토큰이 취소된 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the cancellation token has been canceled.</para>
    /// \endif
    /// </exception>
    protected override Task<PlcResult> WriteWordsCoreAsync(
        PlcAddress address,
        IReadOnlyList<short> values,
        CancellationToken cancellationToken)
    {
        var component = RequireComponent();

        try
        {
            for (var index = 0; index < values.Count; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ComInvoker.Invoke(
                    component,
                    _options.WriteVariableMethodName,
                    OmronCxAddressNameFormatter.FormatOffset(address, index),
                    values[index]);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Task.FromResult(PlcResult.Failure($"CX-Compolet word write failed. {ex.Message}"));
        }

        return Task.FromResult(PlcResult.Success());
    }

    /// <summary>
    /// \if KO
    /// <para>기본 PLC 클라이언트 자원을 비동기로 정리하고 CX-Compolet COM 개체를 해제합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Asynchronously disposes the base PLC client resources and releases the CX-Compolet COM object.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>비동기 정리 작업을 나타내는 값 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A value task representing the asynchronous disposal operation.</para>
    /// \endif
    /// </returns>
    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync().ConfigureAwait(false);
        ReleaseComponent();
    }

    /// <summary>
    /// \if KO
    /// <para>PLC 제품군에 따라 선택적인 COM 속성을 설정하며 속성이 없으면 무시합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Sets an optional COM property and ignores it when the PLC-family control does not expose it.</para>
    /// \endif
    /// </summary>
    /// <param name="target">
    /// \if KO
    /// <para>속성을 소유한 COM 대상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The COM target that owns the property.</para>
    /// \endif
    /// </param>
    /// <param name="name">
    /// \if KO
    /// <para>설정할 선택적 속성 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The optional property name to set.</para>
    /// \endif
    /// </param>
    /// <param name="value">
    /// \if KO
    /// <para>설정할 값입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The value to assign.</para>
    /// \endif
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// \if KO
    /// <para>속성 설정자가 존재하지만 내부 COM 오류가 발생할 때 발생할 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown when the property setter exists but raises an inner COM error.</para>
    /// \endif
    /// </exception>
    private static void TrySetProperty(object target, string name, object? value)
    {
        try
        {
            ComInvoker.SetProperty(target, name, value);
        }
        catch (MissingMethodException)
        {
            // CX-Compolet controls vary by PLC family; optional properties are intentionally ignored.
        }
    }

    /// <summary>
    /// \if KO
    /// <para>현재 연결된 CX-Compolet COM 개체를 반환합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Returns the currently connected CX-Compolet COM object.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>연결된 COM 개체입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The connected COM object.</para>
    /// \endif
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// \if KO
    /// <para>클라이언트가 연결되지 않았을 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the client is not connected.</para>
    /// \endif
    /// </exception>
    private object RequireComponent()
    {
        return _component ?? throw new InvalidOperationException("CX-Compolet is not connected.");
    }

    /// <summary>
    /// \if KO
    /// <para>보유한 CX-Compolet COM 개체의 참조를 최종 해제하고 연결 상태를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Finally releases the owned CX-Compolet COM reference and clears the connection state.</para>
    /// \endif
    /// </summary>
    private void ReleaseComponent()
    {
        if (_component is null)
        {
            return;
        }

        if (OperatingSystem.IsWindows() && Marshal.IsComObject(_component))
        {
            Marshal.FinalReleaseComObject(_component);
        }

        _component = null;
    }
}
