using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class NetworkSyncerScript : NetworkBehaviour {

    //public GameObject sign;
    public GameObject timeSign;
    public GameObject lbPresents;
    public GameObject santaHat;
    public GameObject highScore;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    [ClientRpc]
    public void RpcUpdateHighscore(string v)
    {
        var mesh = highScore.GetComponent<TextMesh>();
        mesh.text = v;
    }

    public void updateHighscore(string v)
    {
        //var timeSignObject = GameObject.Find("TimeLeft");
        //Debug.LogWarning(timeSignObject.name);
        //Debug.LogWarning(timeSignObject.Equals(timeSign));
        var mesh = highScore.GetComponent<TextMesh>();
        mesh.text = v;
        RpcUpdateHighscore(v);
    }

    


    public void updateTime(string v)
    {
        //var timeSignObject = GameObject.Find("TimeLeft");
        //Debug.LogWarning(timeSignObject.name);
        //Debug.LogWarning(timeSignObject.Equals(timeSign));
        var mesh = timeSign.GetComponent<TextMesh>();
        mesh.text = v;
        RpcUpdateTime(v);
    }

    public void updatePresents(string v)
    {
        lbPresents.GetComponent<TextMesh>().text = v;
        RpcUpdatePresents(v);
    }

    [ClientRpc]
    public void RpcUpdatePresents(string v)
    {
        lbPresents.GetComponent<TextMesh>().text = v;
    }


    /// rcp class
    [ClientRpc]
    public void RpcUpdateTime(string v)
    {
        var timeSignObject = GameObject.Find("TimeLeft");
        var mesh = timeSign.GetComponent<TextMesh>();
        mesh.text = v;

    }

    internal void triggerSignAnimation(string v)
    {
        iTriggerSignAnimation(v);
        RpcTriggerSignAnimation(v);
       
    }

    [ClientRpc]
    private void RpcTriggerSignAnimation(string v)
    {
        iTriggerSignAnimation(v);
    }

    private void iTriggerSignAnimation(string v)
    {
        var anim = GameObject.Find("SignPivot").GetComponent<Animator>();
        anim.SetTrigger(v);
    }


    public void triggerAnimation(string game_object, string animation)
    {
        iTriggerAnimation(game_object, animation);
        RpcTriggerAnimation(game_object, animation);

    }

    [ClientRpc]
    private void RpcTriggerAnimation(string game_object, string animation)
    {
        iTriggerAnimation(game_object, animation);
    }

    private void iTriggerAnimation(string game_object,  string animation)
    {
        var anim = GameObject.Find(game_object).GetComponent<Animator>();
        anim.SetTrigger(animation);
    }

    [ClientRpc]
    internal void RpcSetSantaHatEnabled(bool v)
    {
       santaHat.SetActive(v);
    }

    [ClientRpc]
    internal void RpcSendSantaHatPosition(Vector3 localPosition, Quaternion localRotation)
    {

        santaHat.transform.localPosition = localPosition;
        santaHat.transform.localRotation = localRotation;
    }
}
