using UnityEngine;
using System.IO.Ports;
using System.Threading;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class ArduinoAccelFeeder : MonoBehaviour
{
    [Header("Serial")]
    public string portName = "COM3"; // Windows: COMx, macOS: /dev/tty.usbmodem*, Linux: /dev/ttyACM0 or /dev/ttyUSB0
    public int baudRate = 115200;

    [Header("Smoothing")]
    [Range(0f, 1f)] public float lowPassAlpha = 0.15f;

    SerialPort _port;
    Thread _thread;
    volatile bool _running;

    // data terakhir (thread-safe cukup volatile)
    volatile float _pitch, _roll, _ax, _ay, _az;
    Vector3 _fax = Vector3.zero; // filtered accel

    ArduinoAccelDevice _device;

    void OnEnable()
    {
        _device = ArduinoAccelDevice.CreateIfNeeded();

        try
        {
            _port = new SerialPort(portName, baudRate);
            _port.ReadTimeout = 50;
            _port.Open();

            _running = true;
            _thread = new Thread(ReadLoop);
            _thread.IsBackground = true;
            _thread.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ArduinoAccelFeeder] Failed to open {portName} : {e.Message}");
        }
    }

    void OnDisable()
    {
        _running = false;
        try { if (_thread != null && _thread.IsAlive) _thread.Join(200); } catch { }
        try { if (_port != null && _port.IsOpen) _port.Close(); } catch { }
    }

    void ReadLoop()
    {
        while (_running && _port != null && _port.IsOpen)
        {
            try
            {
                // format dari sketch Arduino: "pitch,roll,ax,ay,az\n"
                string line = _port.ReadLine().Trim();
                var p = line.Split(',');
                if (p.Length >= 5)
                {
                    if (float.TryParse(p[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var pi))
                        _pitch = pi;
                    if (float.TryParse(p[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var ro))
                        _roll = ro;
                    if (float.TryParse(p[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var ax))
                        _ax = ax;
                    if (float.TryParse(p[3], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var ay))
                        _ay = ay;
                    if (float.TryParse(p[4], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var az))
                        _az = az;
                }
            }
            catch { /* timeout/format error: abaikan */ }
        }
    }

    void Update()
    {
        if (_device == null) return;

        // low-pass di main thread
        var raw = new Vector3(_ax, _ay, _az);
        _fax = Vector3.Lerp(_fax, raw, lowPassAlpha);

        // Susun state dan kirim event ke Input System
        var state = new ArduinoAccelState
        {
            acceleration = _fax,
            pitch = _pitch,
            roll = _roll
        };

        // Queue event (delta/state event sama aja untuk kasus ini)
        unsafe
        {
            InputSystem.QueueStateEvent(_device, state);
        }

        // Penting supaya action diproses frame ini
        InputSystem.Update();
    }
}
