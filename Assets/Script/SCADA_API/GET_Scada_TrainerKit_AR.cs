using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

public class GET_Scada_TrainerKit_AR : MonoBehaviour
{
    [Header("Material References")]
    public Material buzzerLampMaterial;
    public Material greenLampMaterial;
    public Material redLampMaterial;
    public Material yellowLampMaterial;

    [Header("Button References")]
    public GameObject buzzerLampActiveButton;
    public GameObject buzzerLampInactiveButton;
    public GameObject greenLampActiveButton;
    public GameObject greenLampInactiveButton;
    public GameObject redLampActiveButton;
    public GameObject redLampInactiveButton;
    public GameObject yellowLampActiveButton;
    public GameObject yellowLampInactiveButton;

    [Header("Fan References")]
    public GameObject fanActiveButton;
    public GameObject fanInactiveButton;
    public GameObject fanObject; // 3D Fan object to rotate
    private bool isFanRotating = false;
    private Coroutine fanRotationCoroutine;
    private Coroutine pollingCoroutine;

    [Header("Display References")]
    public TMP_Text currentInjectorText;
    public TMP_Text thermoCopleText;

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

    private void OnEnable()
    {
        StartPolling();
    }

    private void OnDisable()
    {
        StopPolling();
    }

    private void StartPolling()
    {
        pollingCoroutine = StartCoroutine(PollingRoutine());
    }

    private IEnumerator PollingRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(MakeApiCall());
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator MakeApiCall()
    {
        string url = $"http://{ScadaConfigManager.Instance.GetIPAddress()}/WaWebService/Json/GetTagValue/Express";
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

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(url, jsonBody))
        {
            string credentials = System.Convert.ToBase64String(
                Encoding.ASCII.GetBytes("admin:")  // No password
            );

            webRequest.SetRequestHeader("Authorization", "Basic " + credentials);
            webRequest.SetRequestHeader("Content-Type", "application/json");

            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[{System.DateTime.Now:HH:mm:ss}] API Request Error: {webRequest.error}");
            }
            else
            {
                Debug.Log($"[{System.DateTime.Now:HH:mm:ss}] Received API Response: {webRequest.downloadHandler.text}");
                ParseAndHandleResponse(webRequest.downloadHandler.text);
            }
        }
    }

    private void ParseAndHandleResponse(string jsonResponse)
    {
        try
        {
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

                        case "Fan":
                            Debug.Log($"Fan Value: {value.Value}");
                            HandleFanState(value.Value == 1);
                            ToggleButtons(fanActiveButton, fanInactiveButton, value.Value == 1);
                            break;
                        case "currentInjector":
                            if (currentInjectorText != null)
                            {
                                currentInjectorText.text = $"{value.Value} mA";
                            }
                            break;
                        case "thermoCople":
                            if (thermoCopleText != null)
                            {
                                thermoCopleText.text = $"{value.Value} C°";
                            }
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

    private void HandleFanState(bool isOn)
    {
        if (fanObject != null)
        {
            if (isOn && !isFanRotating)
            {
                if (fanRotationCoroutine != null)
                {
                    StopCoroutine(fanRotationCoroutine);
                }
                fanRotationCoroutine = StartCoroutine(RotateFan());
                isFanRotating = true;
            }
            else if (!isOn && isFanRotating)
            {
                if (fanRotationCoroutine != null)
                {
                    StopCoroutine(fanRotationCoroutine);
                    fanRotationCoroutine = null;
                }
                isFanRotating = false;
            }
        }
    }

    private IEnumerator RotateFan()
    {
        while (isFanRotating)
        {
            fanObject.transform.Rotate(0, 0, 360 * Time.deltaTime); // Rotate around Z-axis
            yield return null;
        }
    }

    private void ToggleButtons(GameObject activeButton, GameObject inactiveButton, bool isActive)
    {
        if (activeButton != null)
        {
            activeButton.SetActive(isActive);
        }
        if (inactiveButton != null)
        {
            inactiveButton.SetActive(!isActive);
        }
    }

    private void SetMaterialEmission(Material material, bool enableEmission)
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

    private void StopPolling()
    {
        if (pollingCoroutine != null)
        {
            StopCoroutine(pollingCoroutine);
            pollingCoroutine = null;
        }

        if (fanRotationCoroutine != null)
        {
            StopCoroutine(fanRotationCoroutine);
            fanRotationCoroutine = null;
        }
        isFanRotating = false;
    }
}