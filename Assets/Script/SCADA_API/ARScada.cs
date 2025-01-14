using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using System.Net.Sockets;

public class ARScada : MonoBehaviour
{
    private string username = "admin";
    private string password = ""; // No password

    public Material buzzerLampMaterial;
    public Material greenLampMaterial;
    public Material redLampMaterial;
    public Material yellowLampMaterial;

    // Optional button assignments
    public GameObject buzzerLampActiveButton;
    public GameObject buzzerLampInactiveButton;
    public GameObject greenLampActiveButton;
    public GameObject greenLampInactiveButton;
    public GameObject redLampActiveButton;
    public GameObject redLampInactiveButton;
    public GameObject yellowLampActiveButton;
    public GameObject yellowLampInactiveButton;

    public string scadaServerIP = "192.168.1.48"; // Replace with your server's IP or allow user input

    void Start()
    {
        string baseUrl = $"http://{scadaServerIP}/WaWebService/Json/GetTagValue/Express";
        StartCoroutine(CallAPIRepeatedly(baseUrl));
    }

    IEnumerator CallAPIRepeatedly(string baseUrl)
    {
        while (true)
        {
            yield return CallAPI(baseUrl);
            yield return new WaitForSeconds(2f); // Wait for 2 seconds
        }
    }

    IEnumerator CallAPI(string baseUrl)
    {
        string jsonBody = "{\"Tags\":[{\"Name\":\"BuzzerLamp\"},{\"Name\":\"greenLamp\"},{\"Name\":\"redLamp\"},{\"Name\":\"yellowLamp\"}]}";

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(baseUrl, jsonBody))
        {
            // Add headers for authorization
            string credentials = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
            webRequest.SetRequestHeader("Authorization", "Basic " + credentials);
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // Attach the body
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // Send the request
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"API Request Error: {webRequest.error}");
            }
            else
            {
                // Handle the response
                ParseAndHandleResponse(webRequest.downloadHandler.text);
            }
        }
    }

    void ParseAndHandleResponse(string jsonResponse)
    {
        try
        {
            // Parse the JSON response into the ResponseData class
            ResponseData parsedResponse = JsonUtility.FromJson<ResponseData>(jsonResponse);

            if (parsedResponse != null && parsedResponse.Values != null)
            {
                foreach (var value in parsedResponse.Values)
                {
                    switch (value.Name)
                    {
                        case "BuzzerLamp":
                            SetMaterialEmission(buzzerLampMaterial, value.Value == 1);
                            ToggleButtons(buzzerLampActiveButton, buzzerLampInactiveButton, value.Value == 1);
                            break;
                        case "greenLamp":
                            SetMaterialEmission(greenLampMaterial, value.Value == 1);
                            ToggleButtons(greenLampActiveButton, greenLampInactiveButton, value.Value == 1);
                            break;
                        case "redLamp":
                            SetMaterialEmission(redLampMaterial, value.Value == 1);
                            ToggleButtons(redLampActiveButton, redLampInactiveButton, value.Value == 1);
                            break;
                        case "yellowLamp":
                            SetMaterialEmission(yellowLampMaterial, value.Value == 1);
                            ToggleButtons(yellowLampActiveButton, yellowLampInactiveButton, value.Value == 1);
                            break;
                    }
                }
            }
            else
            {
                Debug.LogError("Parsed response is null or invalid.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error parsing API response: {ex.Message}");
        }
    }

    void ToggleButtons(GameObject activeButton, GameObject inactiveButton, bool isActive)
    {
        // Only toggle buttons if they are assigned
        if (activeButton != null)
        {
            activeButton.SetActive(isActive);
        }
        if (inactiveButton != null)
        {
            inactiveButton.SetActive(!isActive);
        }
    }

    void SetMaterialEmission(Material material, bool enableEmission)
    {
        if (material != null)
        {
            if (enableEmission)
            {
                material.EnableKeyword("_EMISSION");
            }
            else
            {
                material.DisableKeyword("_EMISSION");
            }
        }
    }

    public string GetLocalIPAddress()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            Debug.LogError("No IPv4 address found for the device.");
            return "No IPv4 Address Found";
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error retrieving IP address: " + ex.Message);
            return "Error Retrieving IP Address";
        }
    }

    [System.Serializable]
    public class Result
    {
        public int Ret;
        public int Total;
    }

    [System.Serializable]
    public class Values
    {
        public string Name;
        public int Value;
        public int Quality;
    }

    [System.Serializable]
    public class ResponseData
    {
        public Result Result;
        public List<Values> Values;
    }
}