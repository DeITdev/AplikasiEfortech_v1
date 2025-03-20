using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class GetScadaTrainerkit : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private ScadaConfiguration scadaConfig;
    [SerializeField] private ScadaDataStorageTrainerkit dataStorageTrainerkit;
    [SerializeField] private bool autoStartPolling = false;
    private bool isPollingEnabled = false;
    private Coroutine pollingCoroutine;

    private void Start()
    {
        // Auto-start polling if enabled
        if (autoStartPolling)
        {
            StartPollingData();
        }
    }

    // Public methods to control polling
    public void StartPollingData()
    {
        if (!isPollingEnabled)
        {
            isPollingEnabled = true;
            UpdatePollingState();
            Debug.Log("SCADA polling started");
        }
    }

    public void StopPollingData()
    {
        if (isPollingEnabled)
        {
            isPollingEnabled = false;
            UpdatePollingState();
            Debug.Log("SCADA polling stopped");
        }
    }

    public bool IsPolling()
    {
        return isPollingEnabled;
    }

    [System.Serializable]
    private class Result
    {
        public int Ret;
        public int Total;
    }

    [System.Serializable]
    private class Values
    {
        public string Name;
        public float Value;
        public int Quality;
    }

    [System.Serializable]
    private class ResponseData
    {
        public Result Result;
        public List<Values> Values;
    }

    private void OnEnable()
    {
        if (ValidateComponents()) UpdatePollingState();
    }

    private void OnDisable()
    {
        StopPolling();
    }

    private bool ValidateComponents()
    {
        if (scadaConfig == null)
        {
            Debug.LogError("SCADA Configuration is not assigned!");
            return false;
        }
        if (dataStorageTrainerkit == null)
        {
            Debug.LogError("SCADA Data Storage is not assigned!");
            return false;
        }
        return true;
    }

    public void UpdateOnce()
    {
        if (!string.IsNullOrEmpty(scadaConfig.IPAddress))
            StartCoroutine(MakeApiCall());
        else
            Debug.LogError("Cannot update SCADA - IP address is empty!");
    }

    private void UpdatePollingState()
    {
        if (isPollingEnabled && !string.IsNullOrEmpty(scadaConfig.IPAddress)) StartPolling();
        else StopPolling();
    }

    private void StartPolling()
    {
        StopPolling();
        pollingCoroutine = StartCoroutine(PollingRoutine());
    }

    private void StopPolling()
    {
        if (pollingCoroutine != null)
        {
            StopCoroutine(pollingCoroutine);
            pollingCoroutine = null;
        }
    }

    private IEnumerator PollingRoutine()
    {
        while (isPollingEnabled && !string.IsNullOrEmpty(scadaConfig.IPAddress))
        {
            yield return StartCoroutine(MakeApiCall());
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator MakeApiCall()
    {
        string url = $"http://{scadaConfig.IPAddress}/WaWebService/Json/GetTagValue/express";
        string jsonBody = "{\"Tags\":[" +
            "{\"Name\":\"BuzzerLamp\"}," +
            "{\"Name\":\"currentInjector\"}," +
            "{\"Name\":\"Fan\"}," +
            "{\"Name\":\"greenButton\"}," +
            "{\"Name\":\"greenLamp\"}," +
            "{\"Name\":\"redButton\"}," +
            "{\"Name\":\"redLamp\"}," +
            "{\"Name\":\"switch1\"}," +
            "{\"Name\":\"switch2\"}," +
            "{\"Name\":\"thermoCople\"}," +
            "{\"Name\":\"yellowLamp\"}" +
            "]}";

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody));
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Basic " + System.Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:")));

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                ParseAndStoreResponse(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"SCADA API Request Error: {webRequest.error}");
                Debug.LogError($"Response Code: {webRequest.responseCode}");
                if (webRequest.downloadHandler != null && !string.IsNullOrEmpty(webRequest.downloadHandler.text))
                {
                    Debug.LogError($"Error Response Body: {webRequest.downloadHandler.text}");
                }
            }
        }
    }

    private void ParseAndStoreResponse(string jsonResponse)
    {
        try
        {
            var response = JsonUtility.FromJson<ResponseData>(jsonResponse);
            if (response?.Values == null)
            {
                Debug.LogError("SCADA Response or Values is null!");
                return;
            }

            bool dataChanged = false;
            foreach (var value in response.Values)
            {
                bool newBoolValue = Mathf.Approximately(value.Value, 1f);
                switch (value.Name)
                {
                    case "BuzzerLamp":
                        if (dataStorageTrainerkit.BuzzerLamp != newBoolValue)
                        {
                            dataStorageTrainerkit.BuzzerLamp = newBoolValue;
                            dataChanged = true;
                        }
                        break;
                    case "currentInjector":
                        int newCurrentValue = Mathf.RoundToInt(value.Value);
                        if (dataStorageTrainerkit.CurrentInjector != newCurrentValue)
                        {
                            dataStorageTrainerkit.CurrentInjector = newCurrentValue;
                            dataChanged = true;
                        }
                        break;
                    case "Fan":
                        if (dataStorageTrainerkit.Fan != newBoolValue)
                        {
                            dataStorageTrainerkit.Fan = newBoolValue;
                            dataChanged = true;
                        }
                        break;
                    case "greenButton":
                        if (dataStorageTrainerkit.GreenButton != newBoolValue)
                        {
                            dataStorageTrainerkit.GreenButton = newBoolValue;
                            dataChanged = true;
                        }
                        break;
                    case "greenLamp":
                        if (dataStorageTrainerkit.GreenLamp != newBoolValue)
                        {
                            dataStorageTrainerkit.GreenLamp = newBoolValue;
                            dataChanged = true;
                        }
                        break;
                    case "redButton":
                        if (dataStorageTrainerkit.RedButton != newBoolValue)
                        {
                            dataStorageTrainerkit.RedButton = newBoolValue;
                            dataChanged = true;
                        }
                        break;
                    case "redLamp":
                        if (dataStorageTrainerkit.RedLamp != newBoolValue)
                        {
                            dataStorageTrainerkit.RedLamp = newBoolValue;
                            dataChanged = true;
                        }
                        break;
                    case "switch1":
                        if (dataStorageTrainerkit.Switch1 != newBoolValue)
                        {
                            dataStorageTrainerkit.Switch1 = newBoolValue;
                            dataChanged = true;
                        }
                        break;
                    case "switch2":
                        if (dataStorageTrainerkit.Switch2 != newBoolValue)
                        {
                            dataStorageTrainerkit.Switch2 = newBoolValue;
                            dataChanged = true;
                        }
                        break;
                    case "thermoCople":
                        int newThermoValue = Mathf.RoundToInt(value.Value);
                        if (dataStorageTrainerkit.ThermoCople != newThermoValue)
                        {
                            dataStorageTrainerkit.ThermoCople = newThermoValue;
                            dataChanged = true;
                        }
                        break;
                    case "yellowLamp":
                        if (dataStorageTrainerkit.YellowLamp != newBoolValue)
                        {
                            dataStorageTrainerkit.YellowLamp = newBoolValue;
                            dataChanged = true;
                        }
                        break;
                    default:
                        Debug.LogWarning($"Unknown SCADA tag name: {value.Name}");
                        break;
                }
            }

            if (dataChanged) dataStorageTrainerkit.NotifyDataUpdated();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to parse SCADA response: {ex.Message}");
            Debug.LogError($"Response content: {jsonResponse}");
        }
    }
}