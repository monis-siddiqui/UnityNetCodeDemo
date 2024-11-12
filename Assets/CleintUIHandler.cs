using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClientUIHandler : MonoBehaviour
{
    NetworkDiscoveryHandler networkDiscoveryHandler;
    private Coroutine discoveryCoroutine;
    private float timeToReferesh = 5f;

    public Dictionary<string, ushort> discoveredServers;
    public GameObject referenceDiscoveredServer;
    public GameObject discoveredServerParent;

    public TMP_Text statusBar;

    // Start is called before the first frame update
    void Start()
    {
        networkDiscoveryHandler = GameObject.FindObjectOfType<NetworkDiscoveryHandler>();
        if (networkDiscoveryHandler == null)
            Debug.LogError("Unable to find NetworkDiscoveryHandler");
    }

    // Update is called once per frame
    void Update()
    {
        statusBar.text = networkDiscoveryHandler.statusText;
    }

    public void StartDiscovery()
    {
        if (discoveryCoroutine == null)
        {
            Debug.Log("Starting Discovery");
            discoveryCoroutine = StartCoroutine(DiscoveryRoutine());
        }
    }

    private void CheckServersFound() {
        discoveredServers = new Dictionary<string, ushort>();
        DestroyAllChildren(discoveredServerParent);
        foreach (var discoveredServer in networkDiscoveryHandler.discoveredServers){
            Debug.Log("Server found at address: " + discoveredServer.Key + " on port " + discoveredServer.Value.Port);
            AddDiscoveredServerToList(discoveredServer.Key.ToString(), discoveredServer.Value.Port);
        }
    }

    void DestroyAllChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddDiscoveredServerToList(string serverAddress,ushort serverPort) {
        discoveredServers.Add(serverAddress, serverPort);
        GameObject discoveredServerObj =  Instantiate(referenceDiscoveredServer);
        discoveredServerObj.transform.SetParent(discoveredServerParent.transform);
        discoveredServerObj.transform.localScale = Vector3.one;
        discoveredServerObj.GetComponent<DiscoveredServerHandler>().setServerDetails(serverAddress,serverPort);
    }

    public void StopDiscovery()
    {
        if (discoveryCoroutine != null)
        {
            Debug.Log("Stopping Discovery");
            StopCoroutine(discoveryCoroutine);
            discoveryCoroutine = null;
        }
        DestroyAllChildren(discoveredServerParent);
    }

    private IEnumerator DiscoveryRoutine()
    {
        while (true)
        {
            networkDiscoveryHandler.DiscoverServers();
            Invoke("CheckServersFound", 2F);
            yield return new WaitForSeconds(timeToReferesh);
        }
    }

    public void sendMessage() {
        MessageHandler messageHandler = GameObject.FindObjectOfType<MessageHandler>().GetComponent<MessageHandler>();
        messageHandler.sendMessage(0);
    }
}
