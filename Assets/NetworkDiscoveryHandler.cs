using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;

public class NetworkDiscoveryHandler : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private int networkDiscoveryPort=8001;
    private UdpClient udpClient;


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
            BroadcastServer();
        }
        else
        {
            
            // Start client with discovery
            DiscoverAndConnect();
        }
    }

    private void DiscoverAndConnect()
    {
        ListenForServer();
    }

    //This Method Listens To Server Broadcasted accross network
    private async void ListenForServer()
    {

        udpClient = new UdpClient(networkDiscoveryPort);
        Debug.Log("Listening for discovery server at " + networkDiscoveryPort);
        while (true)
        {
            UdpReceiveResult receivedResults = await udpClient.ReceiveAsync();
            string message = Encoding.UTF8.GetString(receivedResults.Buffer);

            if (message == "UnityNetcodeServer")
            {
                string serverAddress = receivedResults.RemoteEndPoint.Address.ToString();
                Debug.Log($"Server discovered at {serverAddress}");
                messageText.text = $"Server discovered at {serverAddress}";
                ConnectToServer(serverAddress);
                break; // Optional: Stop listening after connecting to a server
            }
        }
    }

    //This Method Broadcasts Network Discovery on Network
    private async void BroadcastServer()
    {
        udpClient = new UdpClient();
        udpClient.EnableBroadcast = true;
        while (true)
        {
            Debug.Log("Broadcasting server at Network Discovery at" + IPAddress.Broadcast +":"+ networkDiscoveryPort);
            var serverMessage = Encoding.UTF8.GetBytes("UnityNetcodeServer");
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, networkDiscoveryPort);

            await udpClient.SendAsync(serverMessage, serverMessage.Length, endPoint);
            await Task.Delay(1000); // Broadcast every second
        }
    }

    private void ConnectToServer(string serverAddress)
    {

        if (NetworkManager.Singleton != null)
        {
            // Configure server address
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (transport != null)
            {
                transport.SetConnectionData(serverAddress, 7777); // Use the appropriate port
            }

            // Start the client connection
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client started and connected to server at: " + serverAddress);
            Debug.Log("Is Cleint " + NetworkManager.Singleton.IsClient);
        }
        else
        {
            Debug.LogError("NetworkManager instance not found!");
        }
    }





    private void OnDestroy()
    {
        udpClient.Close();
    }



}