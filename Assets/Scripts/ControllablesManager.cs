using UnityEngine;
using System.Collections;
using System;

public class ControllablesManager : MonoBehaviour {

    public static ControllablesManager Singleton { get; set; }

    public Transform mainCameraRig;
    public Transform mainCameraActual;

    public GameObject defaultController;

    private IControllable activeController;
    public  IControllable ActiveController { get { return activeController; }
        set
        {
            if(activeController != null)
                activeController.TransferControlTo(value);
            IControllable temp = activeController;
            activeController = value;
            activeController.ReceiveControlFrom(temp);
            AttachMainCamera(activeController.getCameraAnchor());
        }
    }

    private void AttachMainCamera(Transform anchor)
    {
        mainCameraRig.position = anchor.position;
        mainCameraRig.rotation = anchor.rotation;
        mainCameraRig.parent = anchor;
        //mainCameraRig.localScale = anchor.lossyScale * (1f / anchor.lossyScale.magnitude);
    }

    // Use this for initialization
    void Start () {
        Singleton = this;
        ActiveController = defaultController.GetComponent<IControllable>();
	}
	
}
