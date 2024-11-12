using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MessageHandler : NetworkBehaviour
{
    [SerializeField] private TMP_Text messageText;

    public TMP_Text inputFieldText;
    List<ulong> cleintIDs;

    private void Start()
    {
        //if (IsServer)
        //{
        //    Debug.Log("Registering Callbacks");
        //    NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        //    NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        //}
        //else {
        //    Debug.Log("Nope");
        //}

    }

    public void RegisterCallBacks() {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnEnable()
    {
        messageText = GameObject.FindGameObjectWithTag("ReplyMessage").GetComponent<TMP_Text>();
        inputFieldText = GameObject.FindGameObjectWithTag("InputMessage").GetComponent<TMP_Text>();
    }

    private void OnClientDisconnected(ulong clientId) {
        cleintIDs.Remove(clientId);
        GameObject.FindObjectOfType<ServerUIHandler>().GetComponent<ServerUIHandler>().onClientDisconnect(clientId);
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log("On Client Connected " + clientId);
        if (cleintIDs == null)
            cleintIDs = new List<ulong>();
        cleintIDs.Add(clientId);
        GameObject.FindObjectOfType<ServerUIHandler>().GetComponent<ServerUIHandler>().onClientAdd(clientId);
        messageText.text = "Client connected!";
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendMessageToServerRpc(string message, ServerRpcParams rpcParams = default)         //Main Method
    {
        messageText.text = $"Client says: {message}";
        SendMessageToClientRpc($"Server Reply: {message}", rpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    public void SendMessageToClientRpc(string message, ulong clientId)   //Main Method, Use it in your implementation
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            messageText.text = message;
        }
    }


public void sendMessage(ulong clientId) {           //Demo Method for my Implementation
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Sending message to cleint");
            SendMessageToClientRpc("Server: " +inputFieldText.text, clientId);
        }
        else {
            Debug.Log("Sending message to server");
            SendMessageToServerRpc("Client: "+inputFieldText.text);
        }
    }
}