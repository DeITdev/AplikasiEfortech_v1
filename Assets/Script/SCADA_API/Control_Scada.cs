using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Control_Scada : MonoBehaviour
{
    private string project = "TrainerKit";
    private string username;
    private string password;

    private void Start()
    {
        // Get credentials from ScadaConfigManager
        username = ScadaConfigManager.Instance.GetUsername();
        password = ScadaConfigManager.Instance.GetPassword();
    }

    public void TurnOnYellowLamp()
    {
        if (ValidateIPAddress())
            StartCoroutine(SendScadaRequest("yellowLamp", 1));
    }

    public void TurnOffYellowLamp()
    {
        if (ValidateIPAddress())
            StartCoroutine(SendScadaRequest("yellowLamp", 0));
    }

    public void TurnOnGreenLamp()
    {
        if (ValidateIPAddress())
            StartCoroutine(SendScadaRequest("greenLamp", 1));
    }

    public void TurnOffGreenLamp()
    {
        if (ValidateIPAddress())
            StartCoroutine(SendScadaRequest("greenLamp", 0));
    }

    public void TurnOnRedLamp()
    {
        if (ValidateIPAddress())
            StartCoroutine(SendScadaRequest("redLamp", 1));
    }

    public void TurnOffRedLamp()
    {
        if (ValidateIPAddress())
            StartCoroutine(SendScadaRequest("redLamp", 0));
    }

    public void TurnOnBuzzerLamp()
    {
        if (ValidateIPAddress())
            StartCoroutine(SendScadaRequest("buzzerLamp", 1));
    }

    public void TurnOffBuzzerLamp()
    {
        if (ValidateIPAddress())
            StartCoroutine(SendScadaRequest("buzzerLamp", 0));
    }

    private bool ValidateIPAddress()
    {
        // Check IP from ScadaConfigManager
        string storedIP = ScadaConfigManager.Instance.GetIPAddress();
        if (string.IsNullOrEmpty(storedIP))
        {
            Debug.LogError("IP Address is not set in configuration!");
            return false;
        }
        return true;
    }

    private IEnumerator SendScadaRequest(string nametag, int value)
    {
        // Use the stored IP from ScadaConfigManager
        string ip = ScadaConfigManager.Instance.GetIPAddress();
        string url = $"http://{ip}/WaWebService/Json/SetTagValue/{project}/{nametag}/{value}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            string credentials = System.Convert.ToBase64String(
                Encoding.ASCII.GetBytes(username + ":" + password)
            );
            webRequest.SetRequestHeader("Authorization", "Basic " + credentials);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error sending request to {url}: {webRequest.error}");
            }
            else
            {
                Debug.Log($"Successfully sent request to {url}");
                Debug.Log($"Response: {webRequest.downloadHandler.text}");
            }
        }
    }
}