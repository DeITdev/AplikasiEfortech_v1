using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

public class GET_Scada_TrainerKit : MonoBehaviour
{
    [Header("Network Settings")]
    [SerializeField] private TMP_InputField ipAddressInput;  // Keep this for initial input
    private bool isPolling = false;
    private Coroutine pollingCoroutine;

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

    private void Start()
    {
        // Load saved IP if exists
        if (!string.IsNullOrEmpty(ScadaConfigManager.Instance.GetIPAddress()))
        {
            ipAddressInput.text = ScadaConfigManager.Instance.GetIPAddress();
        }
    }

    public void OnCheckButtonClick()
    {
        if (!isPolling)
        {
            if (string.IsNullOrEmpty(ipAddressInput.text))
            {
                Debug.LogError("IP Address is required!");
                return;
            }

            // Save IP to ScadaConfiguration
            ScadaConfigManager.Instance.SetIPAddress(ipAddressInput.text);

            isPolling = true;
            pollingCoroutine = StartCoroutine(StartPolling());
        }
        else
        {
            StopPolling();
        }
    }

    private IEnumerator StartPolling()
    {
        while (isPolling)
        {
            yield return StartCoroutine(MakeApiCall());
            yield return new WaitForSeconds(2f); // Poll every 2 seconds
        }
    }

    private IEnumerator MakeApiCall()
    {
        // Get IP from ScadaConfigManager
        string url = $"http://{ScadaConfigManager.Instance.GetIPAddress()}/WaWebService/Json/GetTagValue/Express";
        string jsonBody = "{\"Tags\":[{\"Name\":\"BuzzerLamp\"},{\"Name\":\"greenLamp\"},{\"Name\":\"redLamp\"},{\"Name\":\"yellowLamp\"}]}";

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(url, jsonBody))
        {
            // Use stored username and password from ScadaConfigManager
            string credentials = System.Convert.ToBase64String(
                Encoding.ASCII.GetBytes(
                    ScadaConfigManager.Instance.GetUsername() + ":" +
                    ScadaConfigManager.Instance.GetPassword()
                )
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
                Debug.LogError($"API Request Error: {webRequest.error}");
                StopPolling();
            }
            else
            {
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
                            Debug.Log($"BuzzerLamp: {value.Value}");
                            break;
                        case "greenLamp":
                            SetMaterialEmission(greenLampMaterial, value.Value == 1);
                            ToggleButtons(greenLampActiveButton, greenLampInactiveButton, value.Value == 1);
                            Debug.Log($"greenLamp: {value.Value}");
                            break;
                        case "redLamp":
                            SetMaterialEmission(redLampMaterial, value.Value == 1);
                            ToggleButtons(redLampActiveButton, redLampInactiveButton, value.Value == 1);
                            Debug.Log($"redLamp: {value.Value}");
                            break;
                        case "yellowLamp":
                            SetMaterialEmission(yellowLampMaterial, value.Value == 1);
                            ToggleButtons(yellowLampActiveButton, yellowLampInactiveButton, value.Value == 1);
                            Debug.Log($"yellowLamp: {value.Value}");
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

    public void StopPolling()
    {
        isPolling = false;
        if (pollingCoroutine != null)
        {
            StopCoroutine(pollingCoroutine);
            pollingCoroutine = null;
        }
    }

    private void OnDisable()
    {
        StopPolling();
    }
}