using UnityEngine;

public class ScadaConfigManager : MonoBehaviour
{
    [SerializeField] private ScadaConfiguration scadaConfig;

    public static ScadaConfigManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string GetIPAddress()
    {
        return scadaConfig.IPAddress;
    }

    public void SetIPAddress(string ip)
    {
        scadaConfig.IPAddress = ip;
    }

    public string GetUsername()
    {
        return scadaConfig.Username;
    }

    public string GetPassword()
    {
        return scadaConfig.Password;
    }
}