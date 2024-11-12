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
    //[SerializeField] private Text messageText;

    public string statusText="Waiting for connection...";

    [SerializeField, HideInInspector]
    ExampleNetworkDiscovery m_Discovery;

    NetworkManager m_NetworkManager;

    public Dictionary<IPAddress, DiscoveryResponseData> discoveredServers = new Dictionary<IPAddress, DiscoveryResponseData>();


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
        //UnityTransport transport = (UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport;
        //transport.SetConnectionData(sender.Address.ToString(), response.Port);
        //m_NetworkManager.StartClient();
    }

    //Client Method
    public void ConnectToServer(string serverAddress,ushort port)
    {
        UnityTransport transport = (UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport;
        transport.SetConnectionData(serverAddress, port);
        if (m_NetworkManager.StartClient()) {
            Debug.Log("Connected to Server:");
            statusText = "Connected to Server " + serverAddress + ":" + port;
        }
    }

    //Pass true for client and false for server
    public void StartHost(bool isClient)
    {
        var networkManager = NetworkManager.Singleton;
        if (networkManager == null) return;

        if (!isClient)
        {
            // Start server

            bool isStarted = networkManager.StartServer();
            if (isStarted) {
                Debug.Log("Server has started.");
                Debug.Log("Is Server " + networkManager.IsServer);
                StartServerBroadcast();
                RegisterClientCallBacks();
            }
        }
        else
        {
            Debug.Log("Starting Client");
            // Start client with discovery
            DiscoverAndConnect();
        }
    }

    public void RegisterClientCallBacks() {
        GameObject.FindGameObjectWithTag("MessageHandler").GetComponent<MessageHandler>().RegisterCallBacks();
    }

    private void DiscoverAndConnect()
    {
            m_Discovery.StartClient();
            m_Discovery.ClientBroadcast(new DiscoveryBroadcastData());
    }

    public void DiscoverServers()
    {
        Debug.Log("Running Discover Service Call");
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