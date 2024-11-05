using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

[RequireComponent(typeof(ExampleNetworkDiscovery))]
[RequireComponent(typeof(NetworkManager))]
public class NetworkDiscoveryHandler : MonoBehaviour
{
    [SerializeField] private Text messageText;

    [SerializeField, HideInInspector]
    ExampleNetworkDiscovery m_Discovery;

    NetworkManager m_NetworkManager;

    Dictionary<IPAddress, DiscoveryResponseData> discoveredServers = new Dictionary<IPAddress, DiscoveryResponseData>();


    void Awake()
    {
        m_Discovery = GetComponent<ExampleNetworkDiscovery>();
        m_NetworkManager = GetComponent<NetworkManager>();

        if (m_Discovery != null) {
            m_Discovery.OnServerFound.AddListener(OnServerFound);
        }
    }


    void OnServerFound(IPEndPoint sender, DiscoveryResponseData response)
    {
        Debug.Log("Discovered Server");
        discoveredServers[sender.Address] = response;
        UnityTransport transport = (UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport;
        transport.SetConnectionData(sender.Address.ToString(), response.Port);
        m_NetworkManager.StartClient();
    }

    public void StartHost(bool isClient)
    {
        var networkManager = NetworkManager.Singleton;
        if (networkManager == null) return;

        if (!isClient)
        {
            // Start server

            networkManager.StartServer();
            messageText.text = "Server Started";
            Debug.Log("Is Server " + networkManager.IsServer);
            StartServerBroadcast();
        }
        else
        {
            
            // Start client with discovery
            DiscoverAndConnect();
        }
    }

    private void DiscoverAndConnect()
    {
            m_Discovery.StartClient();
            m_Discovery.ClientBroadcast(new DiscoveryBroadcastData());
    }

    private void StartServerBroadcast() {
        m_Discovery.StartServer();
    }

    private void StopServerBroadcast() {
        m_Discovery.StopDiscovery();
    }

}