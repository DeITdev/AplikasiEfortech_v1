using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

public class SetScadaWaterPump : MonoBehaviour
{
    private const string PROJECT = "WaterPump";
    [SerializeField] private ScadaConfiguration scadaConfig;

    #region Pump Control Methods
    // Pump 1-1
    public void TurnOnPump1_1() => SendCommand("Pump1_1", 1);
    public void TurnOffPump1_1() => SendCommand("Pump1_1", 0);

    // Pump 1-2
    public void TurnOnPump1_2() => SendCommand("Pump1_2", 1);
    public void TurnOffPump1_2() => SendCommand("Pump1_2", 0);

    // Pump 2-1
    public void TurnOnPump2_1() => SendCommand("Pump2_1", 1);
    public void TurnOffPump2_1() => SendCommand("Pump2_1", 0);

    // Pump 2-2
    public void TurnOnPump2_2() => SendCommand("Pump2_2", 1);
    public void TurnOffPump2_2() => SendCommand("Pump2_2", 0);
    #endregion

    private void SendCommand(string deviceName, int value)
    {
        if (ValidateConfiguration() && ValidateDeviceName(deviceName))
        {
            StartCoroutine(SendScadaRequest(deviceName, value));
        }
    }

    private bool ValidateConfiguration()
    {
        if (scadaConfig == null)
        {
            Debug.LogError("ScadaConfiguration is not assigned!");
            return false;
        }

        if (string.IsNullOrEmpty(scadaConfig.IPAddress))
        {
            Debug.LogError("IP Address is not set in ScadaConfiguration!");
            return false;
        }

        return true;
    }

    private bool ValidateDeviceName(string deviceName)
    {
        // Only allow control of pump devices
        switch (deviceName)
        {
            case "Pump1_1":
            case "Pump1_2":
            case "Pump2_1":
            case "Pump2_2":
                return true;
            default:
                Debug.LogError($"Device {deviceName} is not controllable!");
                return false;
        }
    }

    private IEnumerator SendScadaRequest(string deviceName, int value)
    {
        string url = $"http://{scadaConfig.IPAddress}/WaWebService/Json/SetTagValue/{PROJECT}/{deviceName}/{value}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            string credentials = System.Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{scadaConfig.Username}:{scadaConfig.Password}")
            );

            webRequest.SetRequestHeader("Authorization", "Basic " + credentials);
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error sending request to {deviceName}: {webRequest.error}");
                Debug.LogError($"Response Code: {webRequest.responseCode}");
                if (webRequest.downloadHandler != null && !string.IsNullOrEmpty(webRequest.downloadHandler.text))
                {
                    Debug.LogError($"Error Response Body: {webRequest.downloadHandler.text}");
                }
            }
            else
            {
                Debug.Log($"Successfully set {deviceName} to {value}");
            }
        }
    }
}