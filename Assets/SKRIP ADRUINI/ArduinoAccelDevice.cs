// Requires: Unity Input System (com.unity.inputsystem)
// Edit > Project Settings > Player > Active Input Handling: Input System Package

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using Unity.Collections;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

// 1) State struct buat event data yang kita kirim
// Format bebas, tapi pakai FourCC biar jelas
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ArduinoAccelState : IInputStateTypeInfo
{
    public static FourCC Format => new FourCC('A', 'C', 'C', 'L');
    public FourCC format => Format;

    // Vector3 acceleration (ax, ay, az) dalam "g"
    public Vector3 acceleration;

    // Sudut (derajat)
    public float pitch;
    public float roll;
}

// 2) Definisi device di Input System
[InputControlLayout(stateType = typeof(ArduinoAccelState), displayName = "Arduino Accelerometer")]
public class ArduinoAccelDevice : InputDevice
{
    public Vector3Control acceleration { get; private set; }
    public AxisControl pitch { get; private set; }
    public AxisControl roll { get; private set; }

    protected override void FinishSetup()
    {
        base.FinishSetup();
        acceleration = GetChildControl<Vector3Control>("acceleration");
        pitch = GetChildControl<AxisControl>("pitch");
        roll = GetChildControl<AxisControl>("roll");
    }

    // Helper untuk buat device secara programatik
    public static ArduinoAccelDevice CreateIfNeeded()
    {
        var existing = InputSystem.GetDevice<ArduinoAccelDevice>();
        if (existing != null) return existing;

        InputSystem.RegisterLayout<ArduinoAccelDevice>(
            matches: new InputDeviceMatcher()
                .WithInterface("ArduinoAccel")); // interface arbitrary

        // Buat device
        var dev = InputSystem.AddDevice<ArduinoAccelDevice>("Arduino Accelerometer");
        return dev;
    }
}
