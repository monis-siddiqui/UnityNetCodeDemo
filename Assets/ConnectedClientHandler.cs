using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConnectedClientHandler : MonoBehaviour
{
    public ulong clientID;
    public TMP_Text displayText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SendMessageToClient() {
        MessageHandler messageHandler = GameObject.FindObjectOfType<MessageHandler>().GetComponent<MessageHandler>();
        messageHandler.sendMessage(clientID);
    }

    public void setClientDetails(ulong clientId) {
        this.clientID = clientId;
        displayText.text = "Send message to client :" + clientID;
    }
}
