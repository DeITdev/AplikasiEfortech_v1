using UnityEngine;
using TMPro;

public class ScadaVisualUpdaterTrainerkit : MonoBehaviour
{
    [System.Serializable]
    public class StatusReferences
    {
        [Header("Button Status")]
        public GameObject redButtonActiveButton;
        public GameObject redButtonInactiveButton;
        public GameObject greenButtonActiveButton;
        public GameObject greenButtonInactiveButton;

        [Header("Switch Status")]
        public GameObject switch1ActiveButton;
        public GameObject switch1InactiveButton;
        public GameObject switch2ActiveButton;
        public GameObject switch2InactiveButton;
    }

    [Header("Data Reference")]
    [SerializeField] private ScadaDataStorageTrainerkit dataStorageTrainerkit;

    [Header("Material References")]
    [SerializeField] private Material buzzerLampMaterial;
    [SerializeField] private Material greenLampMaterial;
    [SerializeField] private Material redLampMaterial;
    [SerializeField] private Material yellowLampMaterial;

    [Header("Button References")]
    [SerializeField] private GameObject buzzerLampActiveButton;
    [SerializeField] private GameObject buzzerLampInactiveButton;
    [SerializeField] private GameObject greenLampActiveButton;
    [SerializeField] private GameObject greenLampInactiveButton;
    [SerializeField] private GameObject redLampActiveButton;
    [SerializeField] private GameObject redLampInactiveButton;
    [SerializeField] private GameObject yellowLampActiveButton;
    [SerializeField] private GameObject yellowLampInactiveButton;

    [Header("Status References (Optional)")]
    [SerializeField] private bool useStatusReferences = false;
    [SerializeField] private StatusReferences statusReferences;

    [Header("Fan References")]
    [SerializeField] private GameObject fanActiveButton;
    [SerializeField] private GameObject fanInactiveButton;
    [SerializeField] private GameObject fanObject;
    private float currentRotationSpeed = 0f;
    private const float MAX_ROTATION_SPEED = 360f;
    private const float ACCELERATION = 180f;
    private const float DECELERATION = 180f;

    [Header("Display References")]
    [SerializeField] private TMP_Text currentInjectorText;
    [SerializeField] private TMP_Text thermoCopleText;

    private void OnEnable()
    {
        if (dataStorageTrainerkit != null)
        {
            dataStorageTrainerkit.OnDataUpdated += UpdateVisuals;
        }
        else
        {
            Debug.LogError("ScadaDataStorage is not assigned!");
        }
    }

    private void OnDisable()
    {
        if (dataStorageTrainerkit != null)
        {
            dataStorageTrainerkit.OnDataUpdated -= UpdateVisuals;
        }
    }

    private void Update()
    {
        if (fanObject != null && dataStorageTrainerkit != null)
        {
            // Handle rotation speed changes
            if (dataStorageTrainerkit.Fan)
            {
                // Accelerate
                currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, MAX_ROTATION_SPEED,
                    ACCELERATION * Time.deltaTime);
            }
            else
            {
                // Decelerate
                currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, 0f,
                    DECELERATION * Time.deltaTime);
            }

            // Apply rotation if we have any speed
            if (currentRotationSpeed > 0)
            {
                fanObject.transform.Rotate(0, 0, -currentRotationSpeed * Time.deltaTime);
            }
        }
    }

    private void UpdateVisuals()
    {
        // Update Buzzer Lamp
        SetMaterialEmission(buzzerLampMaterial, dataStorageTrainerkit.BuzzerLamp);
        ToggleButtons(buzzerLampActiveButton, buzzerLampInactiveButton, dataStorageTrainerkit.BuzzerLamp);

        // Update Green Lamp
        SetMaterialEmission(greenLampMaterial, dataStorageTrainerkit.GreenLamp);
        ToggleButtons(greenLampActiveButton, greenLampInactiveButton, dataStorageTrainerkit.GreenLamp);

        // Update Red Lamp
        SetMaterialEmission(redLampMaterial, dataStorageTrainerkit.RedLamp);
        ToggleButtons(redLampActiveButton, redLampInactiveButton, dataStorageTrainerkit.RedLamp);

        // Update Yellow Lamp
        SetMaterialEmission(yellowLampMaterial, dataStorageTrainerkit.YellowLamp);
        ToggleButtons(yellowLampActiveButton, yellowLampInactiveButton, dataStorageTrainerkit.YellowLamp);

        // Update Status References only if enabled
        if (useStatusReferences && statusReferences != null)
        {
            // Update Button Status
            ToggleButtons(statusReferences.redButtonActiveButton,
                         statusReferences.redButtonInactiveButton,
                         dataStorageTrainerkit.RedButton);
            ToggleButtons(statusReferences.greenButtonActiveButton,
                         statusReferences.greenButtonInactiveButton,
                         dataStorageTrainerkit.GreenButton);

            // Update Switch Status
            ToggleButtons(statusReferences.switch1ActiveButton,
                         statusReferences.switch1InactiveButton,
                         dataStorageTrainerkit.Switch1);
            ToggleButtons(statusReferences.switch2ActiveButton,
                         statusReferences.switch2InactiveButton,
                         dataStorageTrainerkit.Switch2);
        }

        // Update Fan buttons
        ToggleButtons(fanActiveButton, fanInactiveButton, dataStorageTrainerkit.Fan);

        // Update Display Text
        if (currentInjectorText != null)
        {
            currentInjectorText.text = $"{dataStorageTrainerkit.CurrentInjector} mA";
        }

        if (thermoCopleText != null)
        {
            thermoCopleText.text = $"{dataStorageTrainerkit.ThermoCople} C°";
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
}