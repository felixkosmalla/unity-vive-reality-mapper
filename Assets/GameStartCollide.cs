using UnityEngine;
using System.Collections;
using Leap.Unity;

public class GameStartCollide : MonoBehaviour {


    public GameObject spawner;

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

    private void OnTriggerEnter(Collider other)
    {
        if (IsHand(other))
        {
            spawner.GetComponent<PresentSpawnerBHV>().touchedRock();
          //  var theScript = transform.parent.gameObject.GetComponent<Explosion>();
           // theScript.explode();
            //transform.parent.gameObject.GetComponent<Explosion>().Rpcexplode();
            Debug.Log("Touched Rock!");
        }
    }
}
