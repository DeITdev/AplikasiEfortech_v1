using UnityEngine;
using System;
[CreateAssetMenu(fileName = "ScadaDataWaterPump", menuName = "AR/SCADA Data Storage WaterPump")]
public class ScadaDataStorageWaterPump : ScriptableObject
{
    // Sensor readings
    public float Flow1 = 0f;
    public float Flow2 = 0f;
    public float Level1 = 0f;
    public float Level2 = 0f;
    public float Pressure1 = 0f;
    public float Pressure2 = 0f;
    public float Pressure3 = 0f;
    public float Pressure4 = 0f;
    public float Temp1_1 = 0f;
    public float Temp1_2 = 0f;
    public float Temp2_1 = 0f;
    public float Temp2_2 = 0f;
    // Vibration data fields
    public float Temp_Vibration = 0f;
    public float X_Axis = 0f;
    public float Y_Axis = 0f;
    public float Z_Axis = 0f;
    // Pump states
    public bool Pump1_1 = false;
    public bool Pump1_2 = false;
    public bool Pump2_1 = false;
    public bool Pump2_2 = false;
    // Status indicators
    public bool Status1_1 = false;
    public bool Status1_2 = false;
    public bool Status2_1 = false;
    public bool Status2_2 = false;
    public event Action OnDataUpdated;
    private void OnEnable()
    {
        ResetToDefaults();
    }
    private void ResetToDefaults()
    {
        // Reset all sensor readings
        Flow1 = 0f;
        Flow2 = 0f;
        Level1 = 0f;
        Level2 = 0f;
        Pressure1 = 0f;
        Pressure2 = 0f;
        Pressure3 = 0f;
        Pressure4 = 0f;
        Temp1_1 = 0f;
        Temp1_2 = 0f;
        Temp2_1 = 0f;
        Temp2_2 = 0f;
        // Reset new data fields
        Temp_Vibration = 0f;
        X_Axis = 0f;
        Y_Axis = 0f;
        Z_Axis = 0f;
        // Reset pump states
        Pump1_1 = false;
        Pump1_2 = false;
        Pump2_1 = false;
        Pump2_2 = false;
        // Reset status indicators
        Status1_1 = false;
        Status1_2 = false;
        Status2_1 = false;
        Status2_2 = false;
        NotifyDataUpdated();
    }
    public void NotifyDataUpdated()
    {
        OnDataUpdated?.Invoke();
    }
    public void Reset()
    {
        ResetToDefaults();
    }
}