using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class GetScadaWaterPump : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private ScadaConfiguration scadaConfig;
    [SerializeField] private ScadaDataStorageWaterPump dataStorage;
    [SerializeField] private bool autoStartPolling = false;
    private bool isPollingEnabled = false;
    private Coroutine pollingCoroutine;

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

    private void Start()
    {
        if (autoStartPolling) StartPollingData();
    }

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
        if (dataStorage == null)
        {
            Debug.LogError("SCADA Data Storage is not assigned!");
            return false;
        }
        return true;
    }

    private void UpdatePollingState()
    {
        if (isPollingEnabled && !string.IsNullOrEmpty(scadaConfig?.IPAddress)) StartPolling();
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
        while (isPollingEnabled && !string.IsNullOrEmpty(scadaConfig?.IPAddress))
        {
            yield return StartCoroutine(MakeApiCall());
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator MakeApiCall()
    {
        string url = $"http://{scadaConfig.IPAddress}/WaWebService/Json/GetTagValue/express";
        string jsonBody = "{\"Tags\":[" +
            "{\"Name\":\"Flow_1\"}," +
            "{\"Name\":\"Flow_2\"}," +
            "{\"Name\":\"Level_1\"}," +
            "{\"Name\":\"Level_2\"}," +
            "{\"Name\":\"Pressure_1\"}," +
            "{\"Name\":\"Pressure_2\"}," +
            "{\"Name\":\"Pressure_3\"}," +
            "{\"Name\":\"Pressure_4\"}," +
            "{\"Name\":\"Temp1_1\"}," +
            "{\"Name\":\"Temp1_2\"}," +
            "{\"Name\":\"Temp2_1\"}," +
            "{\"Name\":\"Temp2_2\"}," +
            "{\"Name\":\"Temp_Vibration\"}," +
            "{\"Name\":\"X_Axis\"}," +
            "{\"Name\":\"Y_Axis\"}," +
            "{\"Name\":\"Z_Axis\"}," +
            "{\"Name\":\"Pump1_1\"}," +
            "{\"Name\":\"Pump1_2\"}," +
            "{\"Name\":\"Pump2_1\"}," +
            "{\"Name\":\"Pump2_2\"}," +
            "{\"Name\":\"Status1_1\"}," +
            "{\"Name\":\"Status1_2\"}," +
            "{\"Name\":\"Status2_1\"}," +
            "{\"Name\":\"Status2_2\"}" +
            "]}";

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody));
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Basic " + System.Convert.ToBase64String(Encoding.ASCII.GetBytes($"{scadaConfig.Username}:{scadaConfig.Password}")));

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
                switch (value.Name)
                {
                    case "Flow_1":
                        if (dataStorage.Flow1 != value.Value)
                        {
                            dataStorage.Flow1 = value.Value;
                            dataChanged = true;
                        }
                        break;
                    case "Flow_2":
                        if (dataStorage.Flow2 != value.Value)
                        {
                            dataStorage.Flow2 = value.Value;
                            dataChanged = true;
                        }
                        break;
                    case "Level_1":
                        if (dataStorage.Level1 != value.Value)
                        {
                            dataStorage.Level1 = value.Value;
                            dataChanged = true;
                        }
                        break;
                    case "Level_2":
                        if (dataStorage.Level2 != value.Value)
                        {
                            dataStorage.Level2 = value.Value;
                            dataChanged = true;
                        }
                        break;
                    case "Pressure_1":
                        if (dataStorage.Pressure1 != value.Value)
                        {
                            dataStorage.Pressure1 = value.Value;
                            dataChanged = true;
                        }
                        break;
                    case "Pressure_2":
                        if (dataStorage.Pressure2 != value.Value)
                        {
                            dataStorage.Pressure2 = value.Value;
                            dataChanged = true;
                        }
                        break;
                    case "Pressure_3":
                        if (dataStorage.Pressure3 != value.Value)
                        {
                            dataStorage.Pressure3 = value.Value;
                            dataChanged = true;
                        }
                        break;
                    case "Pressure_4":
                        if (dataStorage.Pressure4 != value.Value)
                        {
                            dataStorage.Pressure4 = value.Value;
                            dataChanged = true;
                        }
                        break;
                    case "Temp1_1":
                        if (dataStorage.Temp1_1 != value.Value)
                        {
                            dataStorage.Temp1_1 = value.Value;
                            dataChanged = true;
                        }
                        break;
                    case "Temp1_2":
                        if (dataStorage.Temp1_2 != value.Value)
                        {
                            dataStorage.Temp1_2 = value.Value;
                            dataChanged = true;
                        }
                        break;
                    case "Temp2_1":
                        if (dataStorage.Temp2_1 != value.Value)
                        {
                            dataStorage.Temp2_1 = value.Value;
                            dataChanged = true;
                        }
                        break;
                    case "Temp2_2":
                        if (dataStorage.Temp2_2 != value.Value)
                        {
                            dataStorage.Temp2_2 = value.Value;
                            dataChanged = true;
                        }
                        break;
                    case "Temp_Vibration":
                        if (dataStorage.Temp_Vibration != value.Value)
                        {
                            dataStorage.Temp_Vibration = value.Value;
                            dataChanged = true;
                        }
                        break;
                    case "X_Axis":
                        if (dataStorage.X_Axis != value.Value)
                        {
                            dataStorage.X_Axis = value.Value;
                            dataChanged = true;
                        }
                        break;
                    case "Y_Axis":
                        if (dataStorage.Y_Axis != value.Value)
                        {
                            dataStorage.Y_Axis = value.Value;
                            dataChanged = true;
                        }
                        break;
                    case "Z_Axis":
                        if (dataStorage.Z_Axis != value.Value)
                        {
                            dataStorage.Z_Axis = value.Value;
                            dataChanged = true;
                        }
                        break;
                    case "Pump1_1":
                        bool newPump1_1State = Mathf.Approximately(value.Value, 1f);
                        if (dataStorage.Pump1_1 != newPump1_1State)
                        {
                            dataStorage.Pump1_1 = newPump1_1State;
                            dataChanged = true;
                        }
                        break;
                    case "Pump1_2":
                        bool newPump1_2State = Mathf.Approximately(value.Value, 1f);
                        if (dataStorage.Pump1_2 != newPump1_2State)
                        {
                            dataStorage.Pump1_2 = newPump1_2State;
                            dataChanged = true;
                        }
                        break;
                    case "Pump2_1":
                        bool newPump2_1State = Mathf.Approximately(value.Value, 1f);
                        if (dataStorage.Pump2_1 != newPump2_1State)
                        {
                            dataStorage.Pump2_1 = newPump2_1State;
                            dataChanged = true;
                        }
                        break;
                    case "Pump2_2":
                        bool newPump2_2State = Mathf.Approximately(value.Value, 1f);
                        if (dataStorage.Pump2_2 != newPump2_2State)
                        {
                            dataStorage.Pump2_2 = newPump2_2State;
                            dataChanged = true;
                        }
                        break;
                    case "Status1_1":
                        bool newStatus1_1 = Mathf.Approximately(value.Value, 1f);
                        if (dataStorage.Status1_1 != newStatus1_1)
                        {
                            dataStorage.Status1_1 = newStatus1_1;
                            dataChanged = true;
                        }
                        break;
                    case "Status1_2":
                        bool newStatus1_2 = Mathf.Approximately(value.Value, 1f);
                        if (dataStorage.Status1_2 != newStatus1_2)
                        {
                            dataStorage.Status1_2 = newStatus1_2;
                            dataChanged = true;
                        }
                        break;
                    case "Status2_1":
                        bool newStatus2_1 = Mathf.Approximately(value.Value, 1f);
                        if (dataStorage.Status2_1 != newStatus2_1)
                        {
                            dataStorage.Status2_1 = newStatus2_1;
                            dataChanged = true;
                        }
                        break;
                    case "Status2_2":
                        bool newStatus2_2 = Mathf.Approximately(value.Value, 1f);
                        if (dataStorage.Status2_2 != newStatus2_2)
                        {
                            dataStorage.Status2_2 = newStatus2_2;
                            dataChanged = true;
                        }
                        break;
                    default:
                        Debug.LogWarning($"Unknown SCADA tag name: {value.Name}");
                        break;
                }
            }

            if (dataChanged)
            {
                dataStorage.NotifyDataUpdated();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to parse SCADA response: {ex.Message}");
            Debug.LogError($"Response content: {jsonResponse}");
        }
    }
}