using UnityEngine;
using System.Collections;

public class GoatAudio : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
        //Debug.Log("PLAYING!");
    }
}
