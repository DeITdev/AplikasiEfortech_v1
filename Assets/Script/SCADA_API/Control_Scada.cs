using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Control_Scada : MonoBehaviour
{
    public string scadaServerIP = "192.168.1.31"; // SCADA server IP address
    private string username = "admin";
    private string password = ""; // No password
    private string project = "TrainerKit";

    public void TurnOnYellowLamp()
    {
        StartCoroutine(SendScadaRequest("yellowLamp", 1));
    }

    public void TurnOffYellowLamp()
    {
        StartCoroutine(SendScadaRequest("yellowLamp", 0));
    }

    public void TurnOnGreenLamp()
    {
        StartCoroutine(SendScadaRequest("greenLamp", 1));
    }

    public void TurnOffGreenLamp()
    {
        StartCoroutine(SendScadaRequest("greenLamp", 0));
    }

    public void TurnOnRedLamp()
    {
        StartCoroutine(SendScadaRequest("redLamp", 1));
    }

    public void TurnOffRedLamp()
    {
        StartCoroutine(SendScadaRequest("redLamp", 0));
    }

    public void TurnOnBuzzerLamp()
    {
        StartCoroutine(SendScadaRequest("buzzerLamp", 1));
    }

    public void TurnOffBuzzerLamp()
    {
        StartCoroutine(SendScadaRequest("buzzerLamp", 0));
    }

    private IEnumerator SendScadaRequest(string nametag, int value)
    {
        string url = $"http://{scadaServerIP}/WaWebService/Json/SetTagValue/{project}/{nametag}/{value}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Add Basic Authentication header
            string credentials = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
            webRequest.SetRequestHeader("Authorization", "Basic " + credentials);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error sending request to {url}: {webRequest.error}");
            }
            else
            {
                Debug.Log($"Successfully sent request to {url}");
            }
        }
    }
}
