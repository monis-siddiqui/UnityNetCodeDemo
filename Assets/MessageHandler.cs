using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MessageHandler : NetworkBehaviour
{
    [SerializeField] private Text messageText;

    public Text inputFieldText;
    ulong cleintID;

    private void Start()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        messageText.text = "Client connected!";
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendMessageToServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        cleintID = rpcParams.Receive.SenderClientId;
        messageText.text = $"Client says: {message}";
        SendMessageToClientRpc($"Server Reply: {message}", rpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void SendMessageToClientRpc(string message, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            messageText.text = message;
        }
    }


public void sendMessage() {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Sending message to cleint");
            SendMessageToClientRpc("Server: " +inputFieldText.text, cleintID);
        }
        else {
            Debug.Log("Sending message to server");
            SendMessageToServerRpc("Client: "+inputFieldText.text);
        }
    }
}