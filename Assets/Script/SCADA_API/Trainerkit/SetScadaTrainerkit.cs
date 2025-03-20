using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SetScadaTrainerkit : MonoBehaviour
{
    private const string PROJECT = "TrainerKit";
    [SerializeField] private ScadaConfiguration scadaConfig;

    #region Device Control Methods
    // Buzzer Lamp
    public void TurnOnBuzzerLamp() => SendCommand("BuzzerLamp", 1);
    public void TurnOffBuzzerLamp() => SendCommand("BuzzerLamp", 0);

    // Fan
    public void TurnOnFan() => SendCommand("Fan", 1);
    public void TurnOffFan() => SendCommand("Fan", 0);

    // Green Lamp
    public void TurnOnGreenLamp() => SendCommand("greenLamp", 1);
    public void TurnOffGreenLamp() => SendCommand("greenLamp", 0);

    // Red Lamp
    public void TurnOnRedLamp() => SendCommand("redLamp", 1);
    public void TurnOffRedLamp() => SendCommand("redLamp", 0);

    // Yellow Lamp
    public void TurnOnYellowLamp() => SendCommand("yellowLamp", 1);
    public void TurnOffYellowLamp() => SendCommand("yellowLamp", 0);
    #endregion

    private void SendCommand(string deviceName, int value)
    {
        if (ValidateConfiguration())
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
            }
        }
    }
}