using System.Collections;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using LitJson;
using Network;
using Robot;
using UnityEngine.UI;

/// <summary>
/// 网络传输协议类型
/// </summary>
public enum NetworkProtocol
{
    TCP,
    UDP
}
/// <summary>
/// Display window of PC camera
/// Responsible for receiving, decoding, and displaying data
/// </summary>
public class RemoteCameraWindow : MonoBehaviour
{
    public RawImage RemoteCameraImage;
    //private TcpListener _tcpListener;
    //private TcpClient _client;
    //private NetworkStream _stream;
    private Texture2D _texture;
    public Texture2D Texture => _texture;
    //private byte[] _imageBuffer;
    //private CancellationTokenSource _receiveImageTs = null;
    //private Task _imageReceiveTask;
    public NetworkProtocol _networkProtocol = NetworkProtocol.UDP;

    private int _resolutionWidth = 2160;
    private int _resolutionHeight = 2160 / 2 * 4 / 3;
    private int _videoFps = 60;
    private int _bitrate = 40 * 1024 * 1024;

    public CustomButton listenBtn;

    private void Awake()
    {
        transform.position = Camera.main.transform.position;
        transform.rotation = Camera.main.transform.rotation;
    }

    public void StartListen(int width, int height, int fps, int bitrate, int port,
        NetworkProtocol protocol = NetworkProtocol.UDP)
    {
        _resolutionWidth = width;
        _resolutionHeight = height;
        _videoFps = fps;
        _bitrate = bitrate;
        _networkProtocol = protocol;

        StartCoroutine(OnStartListen(port));
    }

    private void OnDisable()
    {
        if (_networkProtocol == NetworkProtocol.TCP)
        {
            MediaDecoder.release();
        }
        else if (_networkProtocol == NetworkProtocol.UDP)
        {
            MediaDecoderUDP.release();
        }
        Debug.Log("RemoteCameraWindow OnDisable");
        TcpHandler.SendFunctionValue("StopReceivePcCamera", "");
    }

    public void OnCloseBtn()
    {
        // Reset listen button
        listenBtn.SetOn(false);
        // send close event to server
        NetworkCommander.Instance.CloseCamera();
        gameObject.SetActive(false);
    }

    public IEnumerator OnStartListen(int port)
    {
        Debug.Log("StartListen port:" + port + " protocol:" + _networkProtocol);

        _texture = new Texture2D(_resolutionWidth, _resolutionHeight, TextureFormat.RGB24, false, false);
        RemoteCameraImage.texture = _texture;
        yield return null;

        if (_networkProtocol == NetworkProtocol.TCP)
        {
            MediaDecoder.initialize((int)_texture.GetNativeTexturePtr(), _resolutionWidth, _resolutionHeight);
            MediaDecoder.startServer(port, false);
        }
        else if (_networkProtocol == NetworkProtocol.UDP)
        {
            MediaDecoderUDP.initialize((int)_texture.GetNativeTexturePtr(), _resolutionWidth, _resolutionHeight);
            MediaDecoderUDP.startServer(port, false);
        }
        yield return null;

        JsonData cameraParam = new JsonData();
        cameraParam["ip"] = Utils.GetLocalIPv4();
        cameraParam["port"] = port;
        cameraParam["width"] = _resolutionWidth;
        cameraParam["height"] = _resolutionHeight;
        cameraParam["fps"] = _videoFps;
        cameraParam["bitrate"] = _bitrate;
        //cameraParam["protocol"] = _networkProtocol.ToString();
        TcpHandler.SendFunctionValue("StartReceivePcCamera", cameraParam.ToJson());
    }

    private void LateUpdate()
    {
        //Keep the window facing the camera at all times
        if (Camera.main != null)
        {
            transform.position = Camera.main.transform.position;
            transform.rotation = Camera.main.transform.rotation;
        }
    }

    private void Update()
    {
        if (_texture != null)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (_networkProtocol == NetworkProtocol.TCP)
                {
                    if (MediaDecoder.isUpdateFrame())
                    {
                        MediaDecoder.updateTexture();
                        GL.InvalidateState();
                    }
                }
                else if (_networkProtocol == NetworkProtocol.UDP)
                {
                    if (MediaDecoderUDP.isUpdateFrame())
                    {
                        MediaDecoderUDP.updateTexture();
                        GL.InvalidateState();
                    }
                }
            }
        }
    }
}