using UnityEngine;
using System.Collections;

public class Positionublisher : MonoBehaviour {


    public GameObject positionReceiver;

	// Use this for initialization
	void Start () {
	
	}



    private int counter = 0;
	// Update is called once per frame
	void Update () {
        counter++;

        if(counter % 5 == 0)
        {
            positionReceiver.GetComponent<PresentSpawnerBHV>().onHeadPosition(transform);
        }
        
	}
}
