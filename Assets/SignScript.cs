using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SignScript : NetworkBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    [ClientRpc]
    public void RpcStartAnimation()
    {
        startAnimation();
    }

    public void startAnimation()
    {
        GetComponent<Animator>().Play("sign");
    }

    [ClientRpc]
    public void RpcStopAnimation()
    {
        stopAnimation();
    }

    public void stopAnimation()
    {
        GetComponent<Animator>().Stop();
    }

    // Update is called once per frame
    void Update () {
	
	}
}
