using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.IO;

public class PresentSpawnerBHV : NetworkBehaviour {



    public int round_time = 60;
    public int min_presents = 20;
    public GameObject presentPrefab;

    public GameObject roomRig;

    public GameObject[] spawningPositions;
    public GameObject startGameLight;
    public GameObject santaHead;

    public GameObject networkSyncer;

    

    

    private int sphereCounter = 0;
    private int presentCounter = 0;
    private int explodedPresents = 0;

    public enum XmasGameState  { Waiting_for_player, Playing, Won, Lost  };

    public XmasGameState currentState = XmasGameState.Waiting_for_player;

    
    // Use this for initialization
    void Start () {
        loadHighscore();

        Invoke("updateHighcore", 1000);
	}

    private int[] highscore = new int[4];

    private void loadHighscore()
    {
        int counter = 0;
        string line;

        if (!File.Exists("highscore.txt"))
        {
            Debug.LogError("Highscore file not found");
            return;
        }


        // Read the file and display it line by line.
        System.IO.StreamReader file =
           new System.IO.StreamReader("highscore.txt");
        while ((line = file.ReadLine()) != null)
        {
            highscore[counter] = Convert.ToInt32(line);
            counter++;
        }

        file.Close();

        updateHighscore();
    }

    private string updateHighscore()
    {
        // convert interger array to sting
        string hs = "";

        for(int i = 0; i < highscore.Length; i++)
        {
            hs += highscore[i].ToString() + "\n";
        }

        getNetworkSync().updateHighscore(hs);

        getNetworkSync().triggerAnimation("HighScorePivot", "Spin");

        return hs;

    }

    private NetworkSyncerScript getNetworkSync()
    {
        return networkSyncer.GetComponent<NetworkSyncerScript>();
    }

    public void spawnPresents(int count)
    {
        for (int i = 0; i < count; i++)
        {
            presentCounter++;
            Invoke("spawnPresent", UnityEngine.Random.value / 2f);
        }

        updateExplodedPresentCount();

    }

    private void updateExplodedPresentCount()
    {
        getNetworkSync().updatePresents(explodedPresents.ToString() + " / " + min_presents);
    }

    public void spawnPresent(){
        // generate random position
        var numChildren = spawningPositions.Length;

        var childIndex = (int)UnityEngine.Random.Range(0, numChildren);

        sphereCounter++;
        if(sphereCounter >= numChildren)
        {
            sphereCounter = 0;
        }


        if(childIndex == numChildren)
        {
            childIndex--;
        }

        var sphere = spawningPositions[sphereCounter];


        var randomPositionInUnitSphere = UnityEngine.Random.insideUnitSphere * sphere.transform.localScale.x * 0.2f;

        var newPosition = sphere.transform.localPosition + randomPositionInUnitSphere;



	

		var present = (GameObject)Instantiate (presentPrefab, Vector3.zero, Quaternion.identity, roomRig.transform);
        //Instantiate()
        spawnedPresents.AddFirst(present);


		NetworkServer.Spawn(present);
        present.transform.SetParent(roomRig.transform);
        present.transform.localPosition = newPosition;

        // get the explosion script
        present.GetComponent<Explosion>().RpcAssignParent(newPosition);


    }

    internal bool presentExploded()
    {
        
       // first check if we are still palying
       if(currentState != XmasGameState.Playing)
        {
            return false;
        }



        presentCounter--;
        explodedPresents++;
        if(presentCounter == 0)
        {
            this.allPresentsGone();
        }
        updateExplodedPresentCount();


        // create a new present
        if(presentCounter <= 5)
        {
            presentCounter++;
            Invoke("spawnPresent", UnityEngine.Random.value);
        }

        if(explodedPresents > min_presents)
        {
            networkSyncer.GetComponent<NetworkSyncerScript>().RpcSetSantaHatEnabled(true);
        }


        return true;

    }

    private void allPresentsGone()
    {
        if(currentState == XmasGameState.Playing)
        {
            stopCurrentRound(true);
        }


  


    }

    internal void touchedRock()
    {
        if(currentState == XmasGameState.Waiting_for_player)
        {
            
            startNewRound();
            
        }
        
    }

    private void startNewRound()
    {
        explodedPresents = 0;
        spawnPresents(10);
        currentState = XmasGameState.Playing;

        


        networkSyncer.GetComponent<NetworkSyncerScript>().triggerSignAnimation("round_start");
        networkSyncer.GetComponent<NetworkSyncerScript>().triggerAnimation("TimeSignPivot", "round_start");


        seconds_left = round_time;
        this.startTimer();
    }

    private void startTimer()
    {
        InvokeRepeating("timerTick", 0, 1);
    }
    private int seconds_left = 30;
    private LinkedList<GameObject> spawnedPresents = new LinkedList<GameObject>();

    private void timerTick()
    {
        seconds_left--;
        var audioPlayer = GameObject.Find("MyAudioPlayer").GetComponent<MyAudioNetwork>();
        audioPlayer.RpcPlayTickSound();

        networkSyncer.GetComponent<NetworkSyncerScript>().updateTime(seconds_left.ToString());

        
        
        if(seconds_left == 0)
        {
            

            CancelInvoke("timerTick");


            stopCurrentRound(explodedPresents >= min_presents);


        }


    }

    private void resetGame()
    {
        // clean up presents that might still be there
        foreach(GameObject go in spawnedPresents)
        {
            Destroy(go);
        }



        spawnedPresents.Clear();

        presentCounter = 0;
        explodedPresents = 0;


        currentState = XmasGameState.Waiting_for_player;
        networkSyncer.GetComponent<NetworkSyncerScript>().triggerSignAnimation("round_stop");
        networkSyncer.GetComponent<NetworkSyncerScript>().triggerAnimation("TimeSignPivot", "round_stop");
        networkSyncer.GetComponent<NetworkSyncerScript>().RpcSetSantaHatEnabled(false);
    }

    private void stopCurrentRound(bool won)
    {
        CancelInvoke("timerTick");
        var audioPlayer = GameObject.Find("MyAudioPlayer").GetComponent<MyAudioNetwork>();
        if (won)
        {
            currentState = XmasGameState.Won;
            getNetworkSync().updateTime("You won!");
            

            audioPlayer.RpcPlayWinningSound();
        }
        else
        {
            currentState = XmasGameState.Lost;
            getNetworkSync().updateTime("Try again!");
            audioPlayer.RpcPlayLosingSound();
        }


        // try to explode the presents that are still left
        foreach(GameObject go in spawnedPresents)
        {
            try
            {
                var explosion = go.GetComponent<Explosion>();

                explosion.explode();

            }catch(Exception e)
            {

            }
        }


        updateHighscorePoints(explodedPresents);
        updateHighscore();



        Invoke("resetGame", 10);

        //        Invoke("resetGame", 5);

    }

    private bool updateHighscorePoints(int explodedPresents)
    {
        int[] newArray = new int[highscore.Length + 1];

        

        for(int i = 0; i <highscore.Length; i++)
        {
            newArray[i] = highscore[i];
        
        }
        newArray[highscore.Length] = explodedPresents;

        
        Array.Sort(newArray);
        Array.Reverse(newArray);

        bool hasChanged = false;

        for (int i = 0; i < highscore.Length; i++)
        {
            if(highscore[i] != newArray[i])
            {
                hasChanged = true;
            }

            highscore[i] = newArray[i];

        }



        saveHighscore();
        return hasChanged;
    }

    private void saveHighscore()
    {
        // Write the string array to a new file named "WriteLines.txt".
        using (StreamWriter outputFile = new StreamWriter("highscore.txt"))
        {
            for (int i = 0; i < highscore.Length; i++)
            {
                
                outputFile.WriteLine(highscore[i].ToString());
            }
        }
    }

    // Update is called once per frame
    void Update () {
        

		if(Input.GetKeyDown("p"))
			spawnPresents(15);

        if(presentCounter == 0)
        {
            var licht = startGameLight.GetComponent<Light>();
            var min = 0.1f;
            var max = 1.23f;
            var si = Mathf.Sin( Time.time);
            licht.intensity = map(si, -1f, 1f, min, max);
        }else
        {
            var licht = startGameLight.GetComponent<Light>();
            licht.intensity = 0;
        }
	}


    public void onHeadPosition(Transform t)
    {
        


        if (!Network.isServer)
        {
            return;
        }


        santaHead.transform.position = t.position;
        santaHead.transform.rotation = t.rotation;
        santaHead.GetComponent<Renderer>().enabled = false;
        try
        {
            networkSyncer.GetComponent<NetworkSyncerScript>().RpcSetSantaHatEnabled(true);
            networkSyncer.GetComponent<NetworkSyncerScript>().RpcSendSantaHatPosition(santaHead.transform.localPosition, santaHead.transform.localRotation);

            //santaHead.GetComponent<NetworkPositionReceiver>().RpcsetPosition(santaHead.transform.localPosition, santaHead.transform.localRotation);

        }
        catch (Exception e)
        {

        }
        
    }

	float uniformMap(float x,   float out_min, float out_max)
	{
		return (x) * (out_max - out_min)  + out_min;
	}

    float map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
}
