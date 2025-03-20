using UnityEngine;
using TMPro;

public class IP_AddressInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipAddressInput;
    [SerializeField] private ScadaConfiguration scadaConfig;

    private void Awake()
    {
        // Add listener for when the input field value changes
        if (ipAddressInput != null)
        {
            ipAddressInput.onEndEdit.AddListener(OnInputFieldEndEdit);
        }
        else
        {
            Debug.LogError("Input Field is not assigned!");
        }
    }

    private void Start()
    {
        if (scadaConfig == null)
        {
            Debug.LogError("ScadaConfiguration is not assigned!");
            return;
        }

        // Load saved IP address if it exists
        if (!string.IsNullOrEmpty(scadaConfig.IPAddress))
        {
            ipAddressInput.text = scadaConfig.IPAddress;
        }
    }

    private void OnDestroy()
    {
        // Remove listener when object is destroyed
        if (ipAddressInput != null)
        {
            ipAddressInput.onEndEdit.RemoveListener(OnInputFieldEndEdit);
        }
    }

    // Called when user finishes editing the input field
    private void OnInputFieldEndEdit(string value)
    {
        if (scadaConfig != null && !string.IsNullOrEmpty(value))
        {
            scadaConfig.IPAddress = value;
        }
    }

    // Button click handler
    public void OnCheckButtonClick()
    {
        if (scadaConfig == null)
        {
            Debug.LogError("ScadaConfiguration is not assigned!");
            return;
        }

        if (string.IsNullOrEmpty(ipAddressInput.text))
        {
            Debug.LogError("IP Address is required!");
            return;
        }

        // Save IP address to scriptable object
        scadaConfig.IPAddress = ipAddressInput.text;
        Debug.Log($"IP Address saved: {scadaConfig.IPAddress}"); // Added for verification
    }
}