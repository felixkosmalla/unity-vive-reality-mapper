using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkPositionReceiver : NetworkBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    [ClientRpc]
    public void RpcsetPosition(Vector3 localPosition, Quaternion localRotation)
    {
        transform.localPosition = localPosition;
        transform.localRotation = localRotation;
    }
}
