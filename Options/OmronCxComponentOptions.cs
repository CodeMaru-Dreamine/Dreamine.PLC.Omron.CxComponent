namespace Dreamine.PLC.Omron.CxComponent.Options;

/// <summary>
/// \if KO
/// <para>Omron CX-Compolet/CX-Component 연결 옵션을 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides Omron CX-Compolet/CX-Component connection options.</para>
/// \endif
/// </summary>
public sealed class OmronCxComponentOptions
{
    /// <summary>
    /// \if KO
    /// <para>CX-Compolet COM ProgID를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the CX-Compolet COM ProgID.</para>
    /// \endif
    /// </summary>
    public string ProgId { get; set; } = "OMRON.Compolet.CJ2Compolet";

    /// <summary>
    /// \if KO
    /// <para>상대 주소 또는 노드 이름 속성 값을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the peer-address or node-name property value.</para>
    /// \endif
    /// </summary>
    public string PeerAddress { get; set; } = "127.0.0.1";

    /// <summary>
    /// \if KO
    /// <para>상대 주소에 사용할 COM 속성 이름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the COM property name used for the peer address.</para>
    /// \endif
    /// </summary>
    public string PeerAddressPropertyName { get; set; } = "PeerAddress";

    /// <summary>
    /// \if KO
    /// <para>활성화 또는 연결 상태에 사용할 COM 속성 이름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the COM property name used for activation or connection state.</para>
    /// \endif
    /// </summary>
    public string ActivePropertyName { get; set; } = "Active";

    /// <summary>
    /// \if KO
    /// <para>변수 읽기에 사용할 COM 메서드 이름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the COM method name used to read a variable.</para>
    /// \endif
    /// </summary>
    public string ReadVariableMethodName { get; set; } = "ReadVariable";

    /// <summary>
    /// \if KO
    /// <para>변수 쓰기에 사용할 COM 메서드 이름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the COM method name used to write a variable.</para>
    /// \endif
    /// </summary>
    public string WriteVariableMethodName { get; set; } = "WriteVariable";
}
