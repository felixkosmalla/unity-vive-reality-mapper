using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.VR;

public class ModeControllerScript : MonoBehaviour {
    public  string server_address = "192.168.0.106";
    public GameObject networkController;
    public GameObject canvas;



	[Header("Calibration Controllers")]
	public GameObject roomAliveCalibration;
	public GameObject riftCalibration;

	[Header("betaCube Mode")]
	public GameObject projectionRig;
	public GameObject RaWallModel;
    public GameObject raLight;

	[Header("Rift Mode")]
	public GameObject vrRig;
    public GameObject riftLight;




	private NetworkManager networkManager;


	private enum GameMode
	{
		None,
		Rift,
		betaCube
	}

	private GameMode currentMode = GameMode.None;


	// Use this for initialization
	void Start () {
		var bla = System.Environment.GetCommandLineArgs ();
        Debug.Log(VRSettings.loadedDeviceName);

        // get the network controller
        this.networkManager = networkController.GetComponent<NetworkManager>();
	}
	
	// Update is called once per frame
	void Update () {


        if (Input.GetKeyDown("s"))
        {
            NetworkServer.SpawnObjects();
        }
	
		if (currentMode == GameMode.None) {
			if(Input.GetKeyDown("r")){
				currentMode = GameMode.Rift;
				networkManager.networkPort = 8881;
				networkManager.StartServer ();


               


                this.setupRiftMode ();

				this.postSetup ();
			}


			if(Input.GetKeyDown("b")){
				currentMode = GameMode.betaCube;
				networkManager.networkAddress = server_address;
				networkManager.networkPort = 8881;
				networkManager.StartClient ();


				this.setupBetaCubeMode ();

				this.postSetup ();
			}


		}

		if (Input.GetKeyDown ("q")) {
			currentMode = GameMode.None;
            canvas.SetActive(true);
		}


	}

	public void setupBetaCubeMode(){
		// do the rotation for the beta cube
		roomAliveCalibration.GetComponent<VRCalibration>().svd_matrix_algorithm();

		// disable the rift rig
		vrRig.SetActive(false);

		// hide the calibration wall
		RaWallModel.SetActive(false);

        raLight.SetActive(true);
        riftLight.SetActive(false);
    }


	public void setupRiftMode(){
		// disable the camera of the projeciton
		projectionRig.SetActive(false);
        raLight.SetActive(false);
        riftLight.SetActive(true);


	}

	private void postSetup(){
        // todo hide the network manager
        canvas.SetActive(false);
	}
}
