using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiscoveredServerHandler : MonoBehaviour
{

    public TMP_Text displayName;
    private string servername;
    private ushort port;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setServerDetails(string serverName, ushort port) {
        this.servername = serverName;
        this.port = port;
        displayName.text = serverName + ":" + port;
    }

    public void ConnectToServer()
    {
        if (!this.servername.Equals(""))
            GameObject.FindAnyObjectByType<NetworkDiscoveryHandler>().ConnectToServer(this.servername, this.port);
        else
            Debug.Log("No Server To connect");
    }


}
