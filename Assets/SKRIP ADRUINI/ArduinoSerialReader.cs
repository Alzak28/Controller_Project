// Assets/Scripts/ArduinoSerialReader.cs
using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class ArduinoSerialReader : MonoBehaviour
{
    [Header("Serial")]
    public string portName = "COM9"; // ganti sesuai komputer Anda
    public int baudRate = 115200;

    [Header("Mapping")]
    public Transform target;     // object yang mau dikontrol
    public bool usePitchRoll = true; // true: pakai sudut; false: pakai aksel
    public float rotMultiplier = 1.0f; // besaran rotasi
    public float moveMultiplier = 2.0f; // kalau gerak posisi
    public float deadzone = 0.02f; // untuk aksel

    SerialPort _port;
    Thread _thread;
    volatile bool _running;

    // data terakhir dari Arduino
    volatile float _pitch, _roll, _ax, _ay, _az;

    void Start()
    {
        if (target == null) target = this.transform;

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
            Debug.LogError($"Serial open error: {e.Message}");
        }
    }

    void ReadLoop()
    {
        string buffer = "";
        while (_running && _port != null && _port.IsOpen)
        {
            try
            {
                string line = _port.ReadLine(); // menerima "pitch,roll,ax,ay,az"
                // sanitasi
                line = line.Trim();
                var parts = line.Split(',');
                if (parts.Length >= 5)
                {
                    float.TryParse(parts[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _pitch);
                    float.TryParse(parts[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _roll);
                    float.TryParse(parts[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _ax);
                    float.TryParse(parts[3], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _ay);
                    float.TryParse(parts[4], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _az);
                }
            }
            catch { /* timeout atau parse error: abaikan */ }
        }
    }

    void Update()
    {
        // contoh 1: kontrol rotasi object pakai pitch/roll
        if (usePitchRoll)
        {
            // misal pitch = rotasi X, roll = rotasi Z (atau Y sesuai kebutuhan)
            var euler = new Vector3(_pitch * rotMultiplier, 0f, -_roll * rotMultiplier);
            target.localRotation = Quaternion.Euler(euler);
        }
        else
        {
            // contoh 2: gerakkan object dengan akselerasi (miring = jalan)
            Vector3 input = new Vector3(_ax, 0f, _ay);

            // deadzone
            if (Mathf.Abs(input.x) < deadzone) input.x = 0;
            if (Mathf.Abs(input.z) < deadzone) input.z = 0;

            target.Translate(input * moveMultiplier * Time.deltaTime, Space.World);
        }
    }

    void OnDestroy()
    {
        _running = false;
        try { if (_thread != null && _thread.IsAlive) _thread.Join(200); } catch { }
        try { if (_port != null && _port.IsOpen) _port.Close(); } catch { }
    }
}
