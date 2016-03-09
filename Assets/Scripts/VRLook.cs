using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


[Serializable]
public class VRLook
{
    public Cursor3D cursor3D;

    public LayerMask raycastMask;

    public float lookRotationEdge = 45f;
    public float lookRotationCutoff = 90f;
    public float lookSensitivity = 2f;
    public float xSensitivity = 0.1f;
    public float ySensitivity = 0.1f;
    public float bufferZone = 0.5f;
    public float pastBufferZone = 0.75f;
    public float resetTimeStart = 1.5f;
    public float resetTime = 0.5f;
    public float hideAfterReset = 0.5f;
    public float rayCastMult = 10f;
    //public float MinimumX = -90F;
    //public float MaximumX = 90F;
    public bool lockCursor = true;

    //private Quaternion m_CharacterTargetRot;
    //private Quaternion m_CameraTargetRot;
    private bool m_cursorIsLocked = true;
    private float xBuffer = 0;
    private float yBuffer = 0;
    private float timeSinceMovement = 0;


    /*public void Init(Transform character, Transform camera)
    {
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camera.localRotation;
    }*/


    public void LookRotation(Transform character, float delta)
    {
        float xRot = CrossPlatformInputManager.GetAxis("Mouse X") * xSensitivity;
        float yRot = CrossPlatformInputManager.GetAxis("Mouse Y") * ySensitivity;
        if (xRot != 0 & yRot != 0)
        {
            timeSinceMovement = 0;
        }
        else
        {
            timeSinceMovement += delta;
        }

        xBuffer += xRot;
        xRot = xBuffer;
        if (xBuffer > pastBufferZone) xBuffer = pastBufferZone;
        if (xBuffer < -pastBufferZone) xBuffer = -pastBufferZone;
        if (xBuffer > 0)
        {
            if (xBuffer > bufferZone) xRot -= bufferZone;
            else xRot = 0;
        }
        else
        {
            if (xBuffer < -bufferZone) xRot += bufferZone;
            else xRot = 0;
        }
        xRot = xRot / (pastBufferZone - bufferZone);

        yBuffer += yRot;
        yRot = yBuffer;
        if (yBuffer > pastBufferZone) yBuffer = pastBufferZone;
        if (yBuffer < -pastBufferZone) yBuffer = -pastBufferZone;
        if (yBuffer > 0)
        {
            if (yBuffer > bufferZone) yRot -= bufferZone;
            else yRot = 0;
        }
        else
        {
            if (yBuffer < -bufferZone) yRot += bufferZone;
            else yRot = 0;
        }
        yRot = yRot / (pastBufferZone - bufferZone);


        float camAngle = ControllablesManager.Singleton.mainCameraActual.transform.eulerAngles.y;
        float bodyAngle = character.eulerAngles.y;
        //float angleDiff = bodyAngle - camAngle;
        float angleDiffWrapped = Mathf.DeltaAngle(bodyAngle, camAngle);
        if (angleDiffWrapped < 0)
        {
            angleDiffWrapped += lookRotationEdge;
            if (angleDiffWrapped > 0) angleDiffWrapped = 0;
        }
        else
        {
            angleDiffWrapped -= lookRotationEdge;
            if (angleDiffWrapped < 0) angleDiffWrapped = 0;
        }
        if (angleDiffWrapped < -lookRotationCutoff)
        {
            angleDiffWrapped = -lookRotationCutoff;
        }
        if (angleDiffWrapped > lookRotationCutoff)
        {
            angleDiffWrapped = lookRotationCutoff;
        }
        xRot += (angleDiffWrapped / (lookRotationCutoff - lookRotationEdge)) * lookSensitivity;
        xRot += CrossPlatformInputManager.GetAxis("SpinOnX") * xSensitivity;
        //float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

        Quaternion m_CharacterTargetRot = character.localRotation * Quaternion.Euler(0f, xRot, 0f);
        //m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        //if (clampVerticalRotation)
        //    m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

        character.localRotation = m_CharacterTargetRot;
        //camera.localRotation = m_CameraTargetRot;
        //MonoBehaviour.print("x:" + xBuffer + " y:"+yBuffer);

        UpdateCursorLock();
        Position3DCursor();
    }

    private void Position3DCursor()
    {
        RaycastHit hit;

        float xBufferClamped = xBuffer / bufferZone;
        float yBufferClamped = yBuffer / bufferZone;
        
        Vector3 rotatedForward = ControllablesManager.Singleton.mainCameraActual.rotation *  Quaternion.Euler(-yBufferClamped * rayCastMult, xBufferClamped * rayCastMult, 0) * Vector3.forward;
        bool didHit = Physics.Raycast(ControllablesManager.Singleton.mainCameraActual.position, rotatedForward, out hit, 9999, raycastMask);
        bool hitTargetOverride = false;
        if (didHit)
        {
            if (cursor3D != null)
                cursor3D.transform.position = hit.point;

            IInteractable interactable = hit.transform.gameObject.GetComponent<IInteractable>();
            hitTargetOverride = interactable != null;
        }

        if (timeSinceMovement > resetTime + hideAfterReset & hideAfterReset >= 0 & !hitTargetOverride)
        {
            cursor3D.UpdateStatus(Cursor3D.State.Hidden);
            xBuffer = 0;
            yBuffer = 0;
        }
        else
        {
            if (timeSinceMovement > resetTime)
            {
                xBuffer = Mathf.Lerp(xBuffer, 0, (timeSinceMovement - resetTimeStart) / resetTime);
                yBuffer = Mathf.Lerp(yBuffer, 0, (timeSinceMovement - resetTimeStart) / resetTime);
            }

            if (xBufferClamped < 1 & yBufferClamped < 1 & xBufferClamped > -1 & yBufferClamped > -1)
            {
                if (didHit & hitTargetOverride)
                {
                    cursor3D.UpdateStatus(cursor3D.currentState = Cursor3D.State.InZoneOverActivable);
                }
                else
                {
                    cursor3D.UpdateStatus(cursor3D.currentState = Cursor3D.State.InZone);
                }
            }
            else
                cursor3D.UpdateStatus(cursor3D.currentState = Cursor3D.State.OutZone);
        }

    }


    public void SetCursorLock(bool value)
    {
        lockCursor = value;
        if (!lockCursor)
        {//we force unlock the cursor if the user disable the cursor locking helper
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock()
    {
        //if the user set "lockCursor" we check & properly lock the cursos
        if (lockCursor)
            InternalLockUpdate();
    }

    private void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            m_cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_cursorIsLocked = true;
        }

        if (m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /*Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }*/

}
