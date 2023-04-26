using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;
/// <summary>
/// Handles sending path data to the database
/// </summary>
public class DatabaseController : MonoBehaviour
{
    [System.Serializable]

    public class GameState
    {
        public string ip;
        public int pathNr;
        public string cell1;
        public string cell2;
        public string cell3;
        public string cell4;
        public string cell5;
        public string cell6;
        public string cell7;
        public string cell8;
        public string cell9;
        public string cell10;
    }
    private string _ip;
    private int _pathNr;
    private List<Vector3> _cells;

    GameState data = new GameState();

    public void Start()
    {
        _ip = GetIP();
        DontDestroyOnLoad(gameObject);
    }

    public void sendData(List<Vector3> t_cells)
    {
        
        data = new GameState
        {
            ip = _ip,
            cell1 = t_cells[0].ToString(),
            cell2 = t_cells[1].ToString(),
            cell3 = t_cells[2].ToString(),
            cell4 = t_cells[3].ToString(),
            cell5 = t_cells[4].ToString(),
            cell6 = t_cells[5].ToString(),
            cell7 = t_cells[6].ToString(),
            cell8 = t_cells[7].ToString(),
            cell9 = t_cells[8].ToString(),
            cell10 = t_cells[9].ToString()
        };

        postAfterGame();
    }

    public void postAfterGame()
    {
        Debug.Log("Start Sending Data");

        string jsonData = JsonUtility.ToJson(data);
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

    string GetIP()
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
