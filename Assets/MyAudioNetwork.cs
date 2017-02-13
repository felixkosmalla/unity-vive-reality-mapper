using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MyAudioNetwork : NetworkBehaviour {

    public AudioClip winningSound;
    public AudioClip tickSound;
    public AudioClip loseSound;

	// Use this for initialization
	void Start () {
	
	}
	

    [ClientRpc]
    public void RpcPlayWinningSound()
    {
        var audio = GetComponent<AudioSource>();
        audio.clip = winningSound;
        audio.Play();
    }

    [ClientRpc]
    public void RpcPlayLosingSound()
    {
        var audio = GetComponent<AudioSource>();
        audio.clip = loseSound;
        audio.Play();
    }

    [ClientRpc]
    public void RpcPlayTickSound()
    {
        var audio = GetComponent<AudioSource>();
       
        audio.clip = tickSound;
        audio.Play();
    }


    // Update is called once per frame
    void Update () {
	
	}
}
