using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Leap.Unity;

public class Explosion : NetworkBehaviour  {

	public ParticleSystem particleSystem;
	public GameObject present;

    public GameObject designatedParent;


    public AudioClip spawnSound;
    public AudioClip explodeSound;

	private float y0;
	private float someRandom;
	private float someMoreRandom;

	// Use this for initialization
	void Start () {
		y0 = transform.localPosition.y;
		someRandom = Random.value;
		someMoreRandom = Random.value / 10f;

        var audioPlayer = GetComponent<AudioSource>();
        audioPlayer.clip = spawnSound;
        audioPlayer.time = 0.0f;
        audioPlayer.Play();
    }


    public void explode()
    {
        localExplosion();
        Rpcexplode();
    }

    public void localExplosion()
    {
        var audioPlayer = GetComponent<AudioSource>();
        audioPlayer.clip = explodeSound;
        audioPlayer.time = 0.0f;
        audioPlayer.Play();

        particleSystem.Play();
        Destroy(present);
        Destroy(gameObject, particleSystem.duration);
    }

	[ClientRpc]
	public void Rpcexplode(){
        localExplosion();
	}

    [ClientRpc]
    public void RpcAssignParent(Vector3 position)
    {
        transform.parent = GameObject.Find("RoomRig").transform;
        transform.localPosition = position;

        y0 = transform.localPosition.y;
        someRandom = Random.value;
        someMoreRandom = Random.value / 10f;
    }

    // Update is called once per frame
    //[ServerCallback]
    void Update () {

		if (Input.GetKeyDown ("space")) {
            explode();
		}

		transform.Rotate(0f,6.0f*(10.0f-someRandom)*Time.deltaTime,0f);

		var oldPos = transform.localPosition; 
		oldPos.y = y0+0.01f*Mathf.Sin((1-someMoreRandom)*Time.time + someRandom);

		transform.localPosition = oldPos;


	}


  
}
