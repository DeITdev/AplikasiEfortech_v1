using UnityEngine;

[CreateAssetMenu(fileName = "ScadaConfig", menuName = "AR/SCADA Configuration")]
public class ScadaConfiguration : ScriptableObject
{
    [SerializeField] private string ipAddress;
    [SerializeField] private string username = "admin";
    [SerializeField] private string password = "";

    public string IPAddress
    {
        get => ipAddress;
        set => ipAddress = value;
    }

    public string Username
    {
        get => username;
        set => username = value;
    }

    public string Password
    {
        get => password;
        set => password = value;
    }
}