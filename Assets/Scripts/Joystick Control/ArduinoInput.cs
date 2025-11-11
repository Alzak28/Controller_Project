using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports; // Wajib ada untuk Serial

public class ArduinoInput : MonoBehaviour
{
    // Variabel publik ini akan menyimpan data accelerometer
    public Vector3 accelData;

    // Atur port dan baud rate Anda di Inspector
    public string portName = "COM3"; // GANTI INI!
    public int baudRate = 9600;

    private SerialPort serialPort;

    void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 100; // Timeout agar tidak 'freeze'
            serialPort.Open();
            Debug.Log("Port Serial " + portName + " terbuka!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error membuka port serial: " + e.Message);
        }
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                // Baca satu baris data (misal: "2.50,1.10,9.80")
                string dataString = serialPort.ReadLine();

                // Pecah string berdasarkan koma
                string[] values = dataString.Split(',');

                // Pastikan kita dapat 3 nilai (X, Y, Z)
                if (values.Length == 3)
                {
                    // Ubah string menjadi float (angka)
                    float x = float.Parse(values[0]);
                    float y = float.Parse(values[1]);
                    float z = float.Parse(values[2]);

                    // Simpan ke variabel Vector3
                    accelData = new Vector3(x, y, z);

                    // (Opsional) Cetak ke console Unity
                    // Debug.Log("X: " + x + " Y: " + y + " Z: " + z);
                }
            }
            catch (System.TimeoutException)
            {
                // Ini wajar terjadi jika data tidak sempat terbaca
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error membaca data: " + e.Message);
            }
        }
    }

    // Pastikan port ditutup saat aplikasi berhenti
    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Port Serial ditutup.");
        }
    }
}