using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

public class DatabaseController : MonoBehaviour
{
    [System.Serializable]

    public class GameState
    {
        public string Ip;
        public int PathNr;
        public string Cell1;
        public string Cell2;
        public string Cell3;
        public string Cell4;
        public string Cell5;
        public string Cell6;
        public string Cell7;
        public string Cell8;
        public string Cell9;
        public string Cell10;
    }
    private string _ip;
    private int _pathNr;
    private List<Vector3> _cells;

    private GameState _data = new GameState();

    public void Start()
    {
        _ip = GetIP();
        DontDestroyOnLoad(gameObject);
    }

    public void sendData(List<Vector3> t_cells)
    {
        
        _data = new GameState
        {
            Ip = _ip,
            Cell1 = t_cells[0].ToString(),
            Cell2 = t_cells[1].ToString(),
            Cell3 = t_cells[2].ToString(),
            Cell4 = t_cells[3].ToString(),
            Cell5 = t_cells[4].ToString(),
            Cell6 = t_cells[5].ToString(),
            Cell7 = t_cells[6].ToString(),
            Cell8 = t_cells[7].ToString(),
            Cell9 = t_cells[8].ToString(),
            Cell10 = t_cells[9].ToString()
        };

        postAfterGame();
    }

    public void postAfterGame()
    {
        Debug.Log("Start Sending Data");

        string jsonData = JsonUtility.ToJson(_data);
        StartCoroutine(PostMethod(jsonData));

        Debug.Log("Finished Sending Data");

    }

    public IEnumerator PostMethod(string jsonData)
    {
        string url = "https://geovalepaths.anvil.app/_/api/paths";

        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;

            request.SetRequestHeader("Content-Type", "application/json");

            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (!request.isNetworkError && request.responseCode == (int)HttpStatusCode.OK)
            {
                Debug.Log("Data successfully sent to the server");

            }
            else
            {
                Debug.Log("Error sending data to the server: Error " + request.responseCode);
            }
        }

    }

    private string GetIP()
    {
        string ipAddress = "";

        // Grabs all network interfaces
        NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (NetworkInterface iface in interfaces)
        {
            if (iface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 &&
                iface.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
            {
                continue;
            }

            // Get IP properties
            IPInterfaceProperties ipProps = iface.GetIPProperties();

            // Loops through unicast IP addresses
            foreach (UnicastIPAddressInformation ip in ipProps.UnicastAddresses)
            {
                // Skip loopback and link-local addresses
                if (ip.Address.AddressFamily != AddressFamily.InterNetwork ||
                    IPAddress.IsLoopback(ip.Address) ||
                    ip.Address.ToString().StartsWith("169.254."))
                {
                    continue;
                }
                ipAddress = ip.Address.ToString();
                break;
            }

            if (!string.IsNullOrEmpty(ipAddress))
            {
                break;
            }
        }

        return ipAddress;
    }
}
