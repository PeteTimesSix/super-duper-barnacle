using UnityEngine;
using System.Collections;
using System;

public class Cursor3D : MonoBehaviour {

    public enum State { Hidden, OutZone, InZone, InZoneOverActivable }

    public State currentState = State.Hidden;

    public GameObject internalSphere;
    public GameObject externalSphere;
    public Light highlightLight;

    public Material internalOutZone;
    public Material internalInZone;
    public Material internalInZoneOverActivable;
    
    public Material externalOutZone;
    public Material externalInZone;
    public Material externalInZoneOverActivable;

    internal void UpdateStatus(State state)
    {
        currentState = state;
        switch (state)
        {
            case State.Hidden:
                setArtState(false);
                break;
            case State.OutZone:
                setArtState(true);
                internalSphere.GetComponent<Renderer>().material = internalOutZone;
                externalSphere.GetComponent<Renderer>().material = externalOutZone;
                highlightLight.color = Color.red;
                break;
            case State.InZone:
                setArtState(true);
                internalSphere.GetComponent<Renderer>().material = internalInZone;
                externalSphere.GetComponent<Renderer>().material = externalInZone;
                highlightLight.color = Color.yellow;
                break;
            case State.InZoneOverActivable:
                setArtState(true);
                internalSphere.GetComponent<Renderer>().material = internalInZoneOverActivable;
                externalSphere.GetComponent<Renderer>().material = externalInZoneOverActivable;
                highlightLight.color = Color.green;
                break;
        }
    }

    private void setArtState(bool state)
    {
        internalSphere.SetActive(state);
        externalSphere.SetActive(state);
        highlightLight.gameObject.SetActive(state);
    }
}
