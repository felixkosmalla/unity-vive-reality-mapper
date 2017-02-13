using UnityEngine;
using System.Collections;


public class TimeSignScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void changeText(string text)
    {
        iChangeText(text);
        
    }


    private void iChangeText(string text)
    {
        transform.GetChild(0).GetComponent<TextMesh>().text = text;
    }

}
