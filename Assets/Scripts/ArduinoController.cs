using UnityEngine;
using System.IO.Ports;
using System;

public class ArduinoController : MonoBehaviour
{
    [Header("Serial Settings")]
    public string portName = ""; // kosongkan untuk autodetect
    public int baudRate = 115200;
    public bool debugRawData = false;

    [Header("Sensor Filtering & Calibration")]
    [Tooltip("Exponential smoothing factor (0..1). Nilai kecil = lebih halus. 0 = no smoothing.")]
    [Range(0f, 1f)] public float smoothing = 0.15f; // alpha untuk EMA
    [Tooltip("Deadzone: semua |value| < deadzone dianggap 0 (satuan G).")]
    public float deadzone = 0.05f;
    [Tooltip("Scale multiplier applied to the final (smoothed - calibrated) values.")]
    public float sensitivity = 1.0f;

    SerialPort serial;
    public float rawX, rawY, rawZ;
    public float X, Y, Z; // nilai akhir yang sudah di smooth & dikalibrasi

    // internal EMA state
    float smoothX, smoothY, smoothZ;
    bool firstSample = true;

    // calibration offsets (set by CalibrateZero)
    public float offsetX = 0f, offsetY = 0f, offsetZ = 0f;

    void Start()
    {
        string[] ports = SerialPort.GetPortNames();
        Debug.Log("Ports available: " + (ports.Length == 0 ? "(none)" : string.Join(", ", ports)));
        if (!string.IsNullOrEmpty(portName))
        {
            TryOpenPort(portName);
        }
        else
        {
            foreach (var p in ports)
            {
                if (TryOpenPort(p))
                {
                    portName = p;
                    break;
                }
            }
            if (serial == null)
                Debug.LogWarning("No serial port opened. Make sure Arduino is connected and Serial Monitor is closed.");
        }
    }

    bool TryOpenPort(string p)
    {
        try
        {
            serial = new SerialPort(p, baudRate);
            serial.ReadTimeout = 50;
            serial.Open();
            Debug.Log($"Opened serial {p} @ {baudRate}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to open {p}: {e.Message}");
            if (serial != null) { try { serial.Close(); } catch { } serial = null; }
            return false;
        }
    }

    void Update()
    {
        if (serial != null && serial.IsOpen)
        {
            try
            {
                string line = serial.ReadLine();
                if (debugRawData) Debug.Log("RAW: " + line);
                ParseData(line);
                ApplyFilteringAndCalibration();
            }
            catch (TimeoutException) { }
            catch (Exception ex) { Debug.LogWarning("Serial read error: " + ex.Message); }
        }
    }

    void ParseData(string data)
    {
        // expect "X:0.123;Y:-0.234;Z:0.987;"
        try
        {
            string[] parts = data.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in parts)
            {
                if (p.StartsWith("X:")) rawX = float.Parse(p.Substring(2));
                else if (p.StartsWith("Y:")) rawY = float.Parse(p.Substring(2));
                else if (p.StartsWith("Z:")) rawZ = float.Parse(p.Substring(2));
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("ParseData failed: " + e.Message + " raw='" + data + "'");
        }
    }

    void ApplyFilteringAndCalibration()
    {
        // first sample init
        if (firstSample)
        {
            smoothX = rawX; smoothY = rawY; smoothZ = rawZ;
            firstSample = false;
        }

        float a = Mathf.Clamp01(smoothing);
        // EMA: s = a * raw + (1-a) * s_prev => but a as alpha close to 0 => less reactive; using smoothing as alpha
        smoothX = a * rawX + (1f - a) * smoothX;
        smoothY = a * rawY + (1f - a) * smoothY;
        smoothZ = a * rawZ + (1f - a) * smoothZ;

        // Apply calibration offsets (zeroing) then apply deadzone and sensitivity
        float cx = (smoothX - offsetX) * sensitivity;
        float cy = (smoothY - offsetY) * sensitivity;
        float cz = (smoothZ - offsetZ) * sensitivity;

        X = Mathf.Abs(cx) < deadzone ? 0f : cx;
        Y = Mathf.Abs(cy) < deadzone ? 0f : cy;
        Z = Mathf.Abs(cz) < deadzone ? 0f : cz;
    }

    // public helper to calibrate current reading as zero
    public void CalibrateZero()
    {
        // set offsets to current smoothed values
        offsetX = smoothX;
        offsetY = smoothY;
        offsetZ = smoothZ;
        Debug.Log($"Calibrated offsets: {offsetX:F4}, {offsetY:F4}, {offsetZ:F4}");
    }

    void OnApplicationQuit() { ClosePort(); }
    void OnDisable() { ClosePort(); }
    void ClosePort()
    {
        if (serial != null)
        {
            try { if (serial.IsOpen) serial.Close(); serial.Dispose(); } catch { }
            serial = null;
            Debug.Log("Serial closed.");
        }
    }
}
