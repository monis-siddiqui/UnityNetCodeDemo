using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerUIHandler : MonoBehaviour
{

    public List<ulong> clients;
    public GameObject clientListParent;
    public GameObject clientObjRef;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void onClientAdd(ulong clientId) {
        if (clients == null) {
            clients = new List<ulong>();
        }
        clients.Add(clientId);
        GameObject connectedClient = Instantiate(clientObjRef);
        connectedClient.transform.gameObject.name = clientId+"";
        connectedClient.transform.SetParent(clientListParent.transform);
        connectedClient.GetComponent<ConnectedClientHandler>().setClientDetails(clientId);
    }

    public void onClientDisconnect(ulong clientId) {
        clients.Remove(clientId);
        Destroy(clientListParent.transform.Find(clientId + "").gameObject);
    }
}
