using UnityEngine;
using TMPro;

public class ScadaVisualUpdaterWaterPump : MonoBehaviour
{
    [Header("Data Reference")]
    [SerializeField] private ScadaDataStorageWaterPump dataStorage;

    [Header("Analog Display References")]
    [SerializeField] private TMP_Text[] flow1Text;
    [SerializeField] private TMP_Text[] flow2Text;
    [SerializeField] private TMP_Text[] level1Text;
    [SerializeField] private TMP_Text[] level2Text;
    [SerializeField] private TMP_Text[] pressure1Text;
    [SerializeField] private TMP_Text[] pressure2Text;
    [SerializeField] private TMP_Text[] pressure3Text;
    [SerializeField] private TMP_Text[] pressure4Text;
    [SerializeField] private TMP_Text[] temp1_1Text;
    [SerializeField] private TMP_Text[] temp1_2Text;
    [SerializeField] private TMP_Text[] temp2_1Text;
    [SerializeField] private TMP_Text[] temp2_2Text;

    [Header("Vibration Data Combined Display")]
    [SerializeField] private TMP_Text[] vibrationDataText;

    [Header("Pump Indicator GameObjects")]
    [SerializeField] private GameObject Pump_1_Green_ON;
    [SerializeField] private GameObject Pump_1_Red_OFF;
    [SerializeField] private GameObject Pump_2_Green_ON;
    [SerializeField] private GameObject Pump_2_Red_OFF;

    [Header("Pump Button References")]
    [SerializeField] private GameObject pump1_1ActiveButton;
    [SerializeField] private GameObject pump1_1InactiveButton;
    [SerializeField] private GameObject pump1_2ActiveButton;
    [SerializeField] private GameObject pump1_2InactiveButton;
    [SerializeField] private GameObject pump2_1ActiveButton;
    [SerializeField] private GameObject pump2_1InactiveButton;
    [SerializeField] private GameObject pump2_2ActiveButton;
    [SerializeField] private GameObject pump2_2InactiveButton;

    private void OnEnable()
    {
        if (dataStorage != null)
        {
            dataStorage.OnDataUpdated += UpdateVisuals;
        }
        else
        {
            Debug.LogError("ScadaDataStorage is not assigned!");
        }
    }

    private void OnDisable()
    {
        if (dataStorage != null)
        {
            dataStorage.OnDataUpdated -= UpdateVisuals;
        }
    }

    private void UpdateVisuals()
    {
        // Update Analog Displays
        UpdateAnalogDisplayArray(flow1Text, dataStorage.Flow1, "L/s");
        UpdateAnalogDisplayArray(flow2Text, dataStorage.Flow2, "L/s");
        UpdateAnalogDisplayArray(level1Text, dataStorage.Level1, "cm");
        UpdateAnalogDisplayArray(level2Text, dataStorage.Level2, "cm");
        UpdateAnalogDisplayArray(pressure1Text, dataStorage.Pressure1, "kg/cm²");
        UpdateAnalogDisplayArray(pressure2Text, dataStorage.Pressure2, "kg/cm²");
        UpdateAnalogDisplayArray(pressure3Text, dataStorage.Pressure3, "kg/cm²");
        UpdateAnalogDisplayArray(pressure4Text, dataStorage.Pressure4, "kg/cm²");
        UpdateAnalogDisplayArray(temp1_1Text, dataStorage.Temp1_1, "°C");
        UpdateAnalogDisplayArray(temp1_2Text, dataStorage.Temp1_2, "°C");
        UpdateAnalogDisplayArray(temp2_1Text, dataStorage.Temp2_1, "°C");
        UpdateAnalogDisplayArray(temp2_2Text, dataStorage.Temp2_2, "°C");

        // Update Combined Vibration Data Display
        UpdateVibrationDataDisplay();

        // Update Pump Indicators
        // Update pump 1 status indicators (Green ON / Red OFF)
        UpdatePumpStatusIndicator(Pump_1_Green_ON, Pump_1_Red_OFF, dataStorage.Pump1_1 || dataStorage.Pump1_2);

        // Update pump 2 status indicators (Green ON / Red OFF)
        UpdatePumpStatusIndicator(Pump_2_Green_ON, Pump_2_Red_OFF, dataStorage.Pump2_1 || dataStorage.Pump2_2);

        // Update Pump Buttons (individual pump controls)
        UpdatePumpButtonControls(pump1_1ActiveButton, pump1_1InactiveButton, dataStorage.Pump1_1);
        UpdatePumpButtonControls(pump1_2ActiveButton, pump1_2InactiveButton, dataStorage.Pump1_2);
        UpdatePumpButtonControls(pump2_1ActiveButton, pump2_1InactiveButton, dataStorage.Pump2_1);
        UpdatePumpButtonControls(pump2_2ActiveButton, pump2_2InactiveButton, dataStorage.Pump2_2);
    }

    private void UpdateVibrationDataDisplay()
    {
        if (vibrationDataText != null)
        {
            string vibrationInfo = string.Format(
                "Temp Vibration = {0:F2} °C\n" +
                "X-Axis = {1:F2} mm/s\n" +
                "Y-Axis = {2:F2} mm/s\n" +
                "Z-Axis = {3:F2} mm/s",
                dataStorage.Temp_Vibration,
                dataStorage.X_Axis,
                dataStorage.Y_Axis,
                dataStorage.Z_Axis
            );

            foreach (var display in vibrationDataText)
            {
                if (display != null)
                {
                    display.text = vibrationInfo;
                }
            }
        }
    }

    private void UpdateAnalogDisplayArray(TMP_Text[] displays, float value, string unit)
    {
        if (displays != null)
        {
            foreach (var display in displays)
            {
                if (display != null)
                {
                    display.text = $"{value:F2} {unit}";
                }
            }
        }
    }

    private void UpdatePumpStatusIndicator(GameObject greenOnIndicator, GameObject redOffIndicator, bool isActive)
    {
        // Update indicator visibility
        if (greenOnIndicator != null)
        {
            greenOnIndicator.SetActive(isActive);
        }

        if (redOffIndicator != null)
        {
            redOffIndicator.SetActive(!isActive);
        }
    }

    private void UpdatePumpButtonControls(GameObject activeButton, GameObject inactiveButton, bool isActive)
    {
        // Update button visibility
        if (activeButton != null)
        {
            activeButton.SetActive(isActive);
        }

        if (inactiveButton != null)
        {
            inactiveButton.SetActive(!isActive);
        }
    }
}