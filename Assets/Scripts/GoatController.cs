using UnityEngine;
using System.Collections;

public class GoatController : MonoBehaviour {


    public GameObject goatHead;
    public GameObject trackable;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        goatHead.transform.LookAt(trackable.transform);
	}
}
