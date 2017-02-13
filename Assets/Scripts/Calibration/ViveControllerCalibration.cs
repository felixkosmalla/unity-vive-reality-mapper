using UnityEngine;
using System.Collections;

public class ViveControllerCalibration : MonoBehaviour {

    public GameObject calibrationController;
    public GameObject controllerModel;

    private bool triggerButtonDown = false;

    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    private SteamVR_Controller.Device controller
    {

        get
        {
            return SteamVR_Controller.Input((int)trackedObj.index);

        }

    }

    private SteamVR_TrackedObject trackedObj;

    void Start()
    {
        Debug.Log("controller initialized");
        trackedObj = GetComponent< SteamVR_TrackedObject>();

        
        
        Debug.Log("tracked object index" + (trackedObj.index));

    }

    private bool triggerWasUp = true;

    void Update()
    {

        

        if (controller == null)
        {

            Debug.Log("Controller not initialized");

            return;

        }

        triggerButtonDown = controller.GetPressDown(triggerButton | Valve.VR.EVRButtonId.k_EButton_DPad_Left);


        if (controller.GetPress(triggerButton))
        {
            if (triggerWasUp)
            {
                triggerWasUp = false;
                controller.TriggerHapticPulse(2500,Valve.VR.EVRButtonId.k_EButton_Axis0);
                controller.TriggerHapticPulse(2500, Valve.VR.EVRButtonId.k_EButton_Axis1);

                Debug.Log("trigger button");
                Vector3 Bstart = controllerModel.transform.position;

                // get the script on the other side
                VRCalibration vrc = calibrationController.GetComponent<VRCalibration>();
                vrc.triggerPressed(Bstart);
            }
        }else
        {
            triggerWasUp = true;
        }

       // Debug.Log(controller.GetPress(triggerButton));
        //Debug.Log(triggerButtonDown);
        if (triggerButtonDown)
        {
           

        }

    }
}
