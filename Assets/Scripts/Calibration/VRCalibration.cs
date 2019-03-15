using UnityEngine;
using System.Collections;
using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using System.IO;

public class VRCalibration : MonoBehaviour {

    

	[Header("General Settings")]
	[Tooltip("Filename of the calibration file")]
	public string filename;
    public bool load_on_start = false;

    [Header("Virtual Room Settings")]
    //[Tooltip("The parent GameObject of your 3D scanned room")]
	//public GameObject roomRig;
    [Tooltip("Add the fixpoints of your virtual room model")]
	public GameObject[] vr_room_spheres;

    [Header("Controller Settings")]
	[Tooltip("The Gameobject of your Vive Space")]
	public GameObject vive_rig;
	[Tooltip("These will be either added or loaded from the settings. No need to edit things manually here.")]
	public GameObject[] calibration_spheres;


    // mathnet vectors
    DenseVector[] vr_room_vectors = new DenseVector[3];
    DenseVector[] calibration_vectors = new DenseVector[3];


    public Material mat;




	public bool debug_mode = false;

    
    // Vive controllers
    public GameObject prefab;

    public int currentCalibrationIndex = -1;
    public Material highlightMaterial;
    public Material defaultMaterial;

    private LinkedList<GameObject> createdObjects = new LinkedList<GameObject>();



    private Transform original_parent;
    public  bool vive_controller_enabled = false;

    private GameObject parent_of_the_object_to_be_rotated;

	private LinkedList<GameObject> objects_to_be_deleted = new LinkedList<GameObject>();

    // Use this for initialization
    void Start () {

        
        parent_of_the_object_to_be_rotated = vive_rig;
        

        original_parent = parent_of_the_object_to_be_rotated.transform.parent;


        
        // initialize the calibration spheres array
		if (calibration_spheres == null || calibration_spheres.Length == 0) {
			calibration_spheres = new GameObject[vr_room_spheres.Length];
			increaseCalibrationIndex();

		}


        if (load_on_start)
        {
            loadFromCalibration();
        }




    }


	// this is called externally from the ViveControllerCalibration script
    internal void triggerPressed(Vector3 position)
    {

		// check we allow further calibration of the system
        if (!vive_controller_enabled)
        {
            return;
        }

        
        Debug.Log("trigger pressed.");

		// Create a calibration sphere at the position of the vive controller
        calibration_spheres[currentCalibrationIndex] = (GameObject) Instantiate(prefab,position, Quaternion.identity);

		// and add it to the list of calibration spheres
		// not that it is INHERENT that the order in which you add the calibration sphere (i.e. press the trigger) has to be the same as in the room spheres 
        createdObjects.AddFirst(calibration_spheres[currentCalibrationIndex]);

      
		        
        increaseCalibrationIndex();

		// if we added all calibration speheres, start the calibration
        if(currentCalibrationIndex == 0)
        {
           svd_matrix_algorithm();
        }

    }


    [ContextMenu("Save Calibration spheres!")]
    private void saveCalibrationSpheres()
    {


        // Write the string array to a new file named "WriteLines.txt".
        using (StreamWriter outputFile = new StreamWriter(filename+".json"))
        {
            for (int i = 0; i < calibration_spheres.Length; i++)
            {
                var position = calibration_spheres[i].transform.position;
                outputFile.WriteLine(JsonUtility.ToJson(position));
            }
        }

        

    }

    [ContextMenu("Load Calibration spheres")]
    private bool loadFromCalibration()
    {
        int counter = 0;
        string line;

        if (!File.Exists(filename + ".json"))
        {
            Debug.LogError("Calibration file not found");
            return false;
        }
            

        // Read the file and display it line by line.
        System.IO.StreamReader file =
           new System.IO.StreamReader(filename + ".json");
        while ((line = file.ReadLine()) != null)
        {
            // get the position
            var position = JsonUtility.FromJson<Vector3>(line);

			GameObject calibrationSphere = (GameObject)Instantiate(prefab, position, Quaternion.identity);
			calibrationSphere.name = "Calibration Sphere " + counter.ToString ();
			calibration_spheres [counter] = calibrationSphere;
			calibrationSphere.transform.parent = vive_rig.transform;

            createdObjects.AddFirst(calibration_spheres[currentCalibrationIndex]);



            Console.WriteLine(line);
            counter++;
        }

        file.Close();

        svd_matrix_algorithm();

        return true;
    }



    private void increaseCalibrationIndex()
    {
        if(currentCalibrationIndex == 0 && false)
        {
            // delete all
            foreach (GameObject g in createdObjects)
            {
                Destroy(g);

            }

            createdObjects.Clear();
        }


        currentCalibrationIndex++;
        if(currentCalibrationIndex >= vr_room_spheres.Length)
        {
            currentCalibrationIndex = 0;
        }

        // set the material of the spheres to default
        foreach(GameObject g in vr_room_spheres)
        {
            g.GetComponent<Renderer>().material = defaultMaterial;
            g.transform.localScale = Vector3.one * 0.05f;
        }

        vr_room_spheres[currentCalibrationIndex].GetComponent<Renderer>().material = highlightMaterial;
        vr_room_spheres[currentCalibrationIndex].transform.localScale =  Vector3.one * 0.5f;


    }


    [ContextMenu("Rotate the Vive Rig")]
    public void svd_matrix_algorithm()
    {

		// The actutal algorithm for finding the optimal rotation is based on this excelent article by
		// Nghia Ho
		// http://nghiaho.com/?page_id=671


		// delete the game objects which may still be there from previous calibrations
		foreach (GameObject go in objects_to_be_deleted) {
			Destroy (go);
		}
		objects_to_be_deleted.Clear ();



        // revert to original parent, otherwise there is strange behvior
        if(parent_of_the_object_to_be_rotated.transform.parent  != original_parent)
        {
            try
            {
                var old_parent = parent_of_the_object_to_be_rotated.transform.parent;
                parent_of_the_object_to_be_rotated.transform.parent = original_parent;
                Destroy(old_parent.gameObject);
            }
            catch(Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
        



        // convert game objects to dense vectors
		vr_room_vectors = convertPositions(vr_room_spheres);
		calibration_vectors = convertPositions(calibration_spheres);


		// calculate the centeroid of the room 
		DenseVector room_centeroid = getCenteroid(vr_room_vectors);
		var go_room_centroid = plot(room_centeroid, "Room centeroid");

        
		// calculate the centeroid of the calibration speheres (the spheres representing the physical positions)
        DenseVector calibration_centeroid = getCenteroid(calibration_vectors);
		var calibration_centeroid_go = plot(calibration_centeroid, "Calibration centeroid");


		// Recentering A and B to remove translational component
		var H = recenter(calibration_centeroid, calibration_vectors, room_centeroid, vr_room_vectors);

        var svdRes = H.Svd();

        var U = svdRes.U;
        var S = svdRes.S;
        var V = svdRes.VT;

        var R = V.Transpose() * U.Transpose();

        // Special reflection case
        if (R.Determinant() < 0)
        {
            // Correctly fix reflection case
            // See http://nghiaho.com/?page_id=671#comment-846413
            
            // Multiply 3rd column of V by -1
            var arr = V.ToArray();

            for(int i = 0; i < 3; i++)
            {
                arr[2,i] *= -1;
            }
            V = DenseMatrix.OfArray(arr);

            // Recompute R
            R = V.Transpose() * U.Transpose();

            print("determinant negative");
        }

        R = R.Transpose();

        // set pivot of the vive rig to the centeroid of the calibration spheres
		vive_rig.transform.parent = calibration_centeroid_go.transform;


       
        // calculate quaternion from matrix
        float w = (float)Math.Sqrt(1 + R.At(0, 0) + R.At(1, 1) + R.At(2, 2)) / 2;
        float x = (float)(R.At(2, 1) - R.At(1, 2)) / (4 * w);
        float y = (float)(R.At(0, 2) - R.At(2, 0)) / (4 * w);
        float z = (float)(R.At(1, 0) - R.At(0, 1)) / (4 * w);


        Quaternion q = new Quaternion(x, y, z, w);


   		// rotate the calibration centeroid and with this, also rotate the vive rig
		calibration_centeroid_go.transform.localRotation *=  q;


		// and the move it
        var diff = go_room_centroid.transform.position - calibration_centeroid_go.transform.position;

		calibration_centeroid_go.transform.position += diff;


		// disable the renderer for all calibration speheres
		if (!debug_mode) {
			foreach (GameObject spehere in calibration_spheres) {
				spehere.GetComponent<Renderer> ().enabled = false;
			}

			foreach (GameObject spehere in vr_room_spheres) {
				spehere.GetComponent<Renderer> ().enabled = false;
			}
			calibration_centeroid_go.GetComponent<Renderer> ().enabled = false;

			Destroy (go_room_centroid);
		}

       

    }

   

	private DenseMatrix recenter(DenseVector A_centeroid, DenseVector[] A_vectors, DenseVector B_centeroid, DenseVector[] B_Vectors)
    {
        var H = new DenseMatrix(3, 3);

		for(int i = 0; i < A_vectors.Length; i++)
        {
			var Pa = A_vectors[i];
			var Pb = B_Vectors[i];

			var A = Pa - A_centeroid;
			var B = Pb - B_centeroid;

            var temp = DenseMatrix.OfColumnVectors(B) * DenseMatrix.OfRowVectors(A);

            H += temp;
        }


        return H;
    }

	private GameObject plot(DenseVector position, string name)
    {
		
		GameObject visual = (GameObject)Instantiate(prefab, toUnityVector(position), Quaternion.identity);

       

	

		visual.name = name;

        

        visual.transform.localScale = Vector3.one * 0.3f;
        return visual;
   }

    private Vector3 toUnityVector(DenseVector wall_centeroid)
    {
        Vector3 v = new Vector3();
        v.x = (float)wall_centeroid[0];
        v.y = (float)wall_centeroid[1];
        v.z = (float) wall_centeroid[2];

        return v;
    }


    private Vector3 toUnityVector(Vector<double> t)
    {
        Vector3 v = new Vector3();
        v.x = (float)t[0];
        v.y = (float)t[1];
        v.z = (float)t[2];

        return v;
    }

    private DenseVector getCenteroid(DenseVector[] realVectors)
    {
        var res = DenseVector.Create(3, 0);

        foreach(DenseVector v in realVectors)
        {
            res += v;
        }

        return res / realVectors.Length;

    }

 
    private DenseVector[] convertPositions(GameObject[] wall_spheres)
    {
        DenseVector[] res = new DenseVector[wall_spheres.Length];

        for(int i = 0;  i < wall_spheres.Length; i++)
        {
            res[i] = toVectorDouble(wall_spheres[i].transform.position);
        }

        return res;


    }

    private DenseVector toVectorDouble(Vector3 position)
    {
        var res = new double[3];
        for(int i = 0; i < 3; i++)
        {
            res[i] = position[i];
        }
        return res;
    }








}
