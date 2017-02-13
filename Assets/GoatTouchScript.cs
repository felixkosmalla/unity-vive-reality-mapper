using UnityEngine;
using System.Collections;
using Leap.Unity;

public class GoatTouchScript : MonoBehaviour {


    public GameObject networkSyncer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}



    private bool IsHand(Collider other)
    {
        if (other.transform.parent && other.transform.parent.parent && other.transform.parent.parent.GetComponent<HandModel>())
            return true;
        else
            return false;
    }


    bool exploded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (IsHand(other))
        {
           // var audioPlayer = GameObject.Find("MyAudioPlayer").GetComponent<MyAudioNetwork>();
            //audioPlayer.RpcPlayLosingSound();


            networkSyncer.GetComponent<NetworkSyncerScript>().triggerAnimation("Goat", "Bleat");
        }
    }
}
