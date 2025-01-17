using UnityEngine;
using TMPro; // Make sure to include the TextMeshPro namespace

public class InputFieldController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField; // Reference to your InputField
    [SerializeField] private GameObject objectToEnable; // Reference to the GameObject you want to enable

    private void Start()
    {
        // Ensure the object is disabled at start
        objectToEnable.SetActive(false);

        // Add listener for when the text changes
        inputField.onValueChanged.AddListener(OnInputValueChanged);
    }

    private void OnInputValueChanged(string newText)
    {
        // Enable/disable the object based on whether there's at least one character
        objectToEnable.SetActive(newText.Length > 0);
    }

    // Optional: Clean up when the script is destroyed
    private void OnDestroy()
    {
        inputField.onValueChanged.RemoveListener(OnInputValueChanged);
    }
}