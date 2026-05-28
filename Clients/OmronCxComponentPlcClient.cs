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
/// Provides an Omron CX-Compolet/CX-Component PLC client.
/// </summary>
public sealed class OmronCxComponentPlcClient : PlcClientBase
{
    private readonly OmronCxComponentOptions _options;
    private readonly IComObjectFactory _factory;
    private object? _component;

    /// <summary>
    /// Initializes a new instance of the <see cref="OmronCxComponentPlcClient"/> class.
    /// </summary>
    /// <param name="options">The CX-Compolet options.</param>
    public OmronCxComponentPlcClient(OmronCxComponentOptions options)
        : this(options, new DefaultComObjectFactory())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OmronCxComponentPlcClient"/> class.
    /// </summary>
    /// <param name="options">The CX-Compolet options.</param>
    /// <param name="factory">The COM object factory.</param>
    public OmronCxComponentPlcClient(OmronCxComponentOptions options, IComObjectFactory factory)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    /// <summary>
    /// Gets the CX-Compolet options.
    /// </summary>
    public OmronCxComponentOptions Options => _options;

    /// <inheritdoc />
    protected override Task<PlcResult> ConnectCoreAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _component = _factory.Create(_options.ProgId);
        TrySetProperty(_component, _options.PeerAddressPropertyName, _options.PeerAddress);
        ComInvoker.SetProperty(_component, _options.ActivePropertyName, true);

        return Task.FromResult(PlcResult.Success());
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    protected override Task<PlcResult<bool[]>> ReadBitsCoreAsync(
        PlcAddress address,
        int count,
        CancellationToken cancellationToken)
    {
        var component = RequireComponent();
        var values = new bool[count];

        for (var index = 0; index < count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var raw = ComInvoker.Invoke(
                component,
                _options.ReadVariableMethodName,
                OmronCxAddressNameFormatter.FormatOffset(address, index));

            values[index] = Convert.ToInt32(raw, CultureInfo.InvariantCulture) != 0;
        }

        return Task.FromResult(PlcResult<bool[]>.Success(values));
    }

    /// <inheritdoc />
    protected override Task<PlcResult<short[]>> ReadWordsCoreAsync(
        PlcAddress address,
        int count,
        CancellationToken cancellationToken)
    {
        var component = RequireComponent();
        var values = new short[count];

        for (var index = 0; index < count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var raw = ComInvoker.Invoke(
                component,
                _options.ReadVariableMethodName,
                OmronCxAddressNameFormatter.FormatOffset(address, index));

            values[index] = Convert.ToInt16(raw, CultureInfo.InvariantCulture);
        }

        return Task.FromResult(PlcResult<short[]>.Success(values));
    }

    /// <inheritdoc />
    protected override Task<PlcResult> WriteBitsCoreAsync(
        PlcAddress address,
        IReadOnlyList<bool> values,
        CancellationToken cancellationToken)
    {
        var component = RequireComponent();

        for (var index = 0; index < values.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ComInvoker.Invoke(
                component,
                _options.WriteVariableMethodName,
                OmronCxAddressNameFormatter.FormatOffset(address, index),
                values[index] ? 1 : 0);
        }

        return Task.FromResult(PlcResult.Success());
    }

    /// <inheritdoc />
    protected override Task<PlcResult> WriteWordsCoreAsync(
        PlcAddress address,
        IReadOnlyList<short> values,
        CancellationToken cancellationToken)
    {
        var component = RequireComponent();

        for (var index = 0; index < values.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ComInvoker.Invoke(
                component,
                _options.WriteVariableMethodName,
                OmronCxAddressNameFormatter.FormatOffset(address, index),
                values[index]);
        }

        return Task.FromResult(PlcResult.Success());
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync().ConfigureAwait(false);
        ReleaseComponent();
    }

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

    private object RequireComponent()
    {
        return _component ?? throw new InvalidOperationException("CX-Compolet is not connected.");
    }

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
