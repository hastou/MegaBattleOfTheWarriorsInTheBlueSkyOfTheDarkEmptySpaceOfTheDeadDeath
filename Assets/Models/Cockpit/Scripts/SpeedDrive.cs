//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Interactable that can be used to move in a circular motion
//
//=============================================================================

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Valve.VR.InteractionSystem;

//-------------------------------------------------------------------------
[RequireComponent(typeof(Interactable))]
public class SpeedDrive : MonoBehaviour
{
    public Collider childCollider = null;

    public bool hoverLock = false;

    public float MinSpeedHandleAngle = -10.0f;

    public float MaxSpeedHandleAngle = 10.0f;

    public float outAngle = 13;

    public float OutputSpeed = 0;

    private Quaternion start;

    private Vector3 worldPlaneNormal;

    private Vector3 lastHandProjected;

    private bool driving = false;

    Hand handHoverLocked = null;
    
    float AngleToScale(float angle, float min, float max)
    {
        return (angle - MinSpeedHandleAngle) * (max - min) / (MaxSpeedHandleAngle - MinSpeedHandleAngle) + min;
    }

    //-------------------------------------------------
    void Start()
    {
        if (childCollider == null)
        {
            childCollider = GetComponentInChildren<Collider>();
        }

        worldPlaneNormal = new Vector3(1.0f, 0.0f, 0.0f);

        if (transform.parent)
        {
            worldPlaneNormal = transform.parent.localToWorldMatrix.MultiplyVector(worldPlaneNormal).normalized;
        }

        outAngle = transform.localEulerAngles[0];

        UpdateGameObject();
    }


    //-------------------------------------------------
    void OnDisable()
    {
        if (handHoverLocked)
        {
            ControllerButtonHints.HideButtonHint(handHoverLocked, Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
            handHoverLocked.HoverUnlock(GetComponent<Interactable>());
            handHoverLocked = null;
        }
    }


    //-------------------------------------------------
    private IEnumerator HapticPulses(SteamVR_Controller.Device controller, float flMagnitude, int nCount)
    {
        if (controller != null)
        {
            int nRangeMax = (int)Util.RemapNumberClamped(flMagnitude, 0.0f, 1.0f, 100.0f, 900.0f);
            nCount = Mathf.Clamp(nCount, 1, 10);

            for (ushort i = 0; i < nCount; ++i)
            {
                ushort duration = (ushort)Random.Range(100, nRangeMax);
                controller.TriggerHapticPulse(duration);
                yield return new WaitForSeconds(.01f);
            }
        }
    }


    //-------------------------------------------------
    private void OnHandHoverBegin(Hand hand)
    {
        ControllerButtonHints.ShowButtonHint(hand, Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
    }


    //-------------------------------------------------
    private void OnHandHoverEnd(Hand hand)
    {
        ControllerButtonHints.HideButtonHint(hand, Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);

        if (driving && hand.GetStandardInteractionButton())
        {
            StartCoroutine(HapticPulses(hand.controller, 1.0f, 10));
        }

        driving = false;
        handHoverLocked = null;
    }


    //-------------------------------------------------
    private void HandHoverUpdate(Hand hand)
    {
        if (hand.GetStandardInteractionButtonDown())
        {
            // Trigger was just pressed
            lastHandProjected = ComputeToTransformProjected(hand.hoverSphereTransform);

            if (hoverLock)
            {
                hand.HoverLock(GetComponent<Interactable>());
                handHoverLocked = hand;
            }

            driving = true;

            ComputeAngle(hand);
            UpdateGameObject();

            ControllerButtonHints.HideButtonHint(hand, Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
        }
        else if (hand.GetStandardInteractionButtonUp())
        {
            // Trigger was just released
            if (hoverLock)
            {
                hand.HoverUnlock(GetComponent<Interactable>());
                handHoverLocked = null;
            }
        }
        else if (driving && hand.GetStandardInteractionButton() && hand.hoveringInteractable == GetComponent<Interactable>())
        {
            ComputeAngle(hand);
            UpdateGameObject();
        }
    }


    //-------------------------------------------------
    private Vector3 ComputeToTransformProjected(Transform xForm)
    {
        Vector3 toTransform = (xForm.localPosition - transform.localPosition).normalized;
        Vector3 toTransformProjected = new Vector3(0.0f, 0.0f, 0.0f);

        // Need a non-zero distance from the hand to the center of the CircularDrive
        if (toTransform.sqrMagnitude > 0.0f)
        {
            toTransformProjected = Vector3.ProjectOnPlane(toTransform, worldPlaneNormal).normalized;
        }

        return toTransformProjected;
    }

    //-------------------------------------------------
    // Updates the GameObject rotation
    //-------------------------------------------------
    private void UpdateGameObject()
    {
        transform.localRotation = Quaternion.AngleAxis(outAngle, worldPlaneNormal);
        OutputSpeed = AngleToScale(outAngle, 0, 100);
    }

    //-------------------------------------------------
    // Computes the angle to rotate the game object based on the change in the transform
    //-------------------------------------------------
    private void ComputeAngle(Hand hand)
    {
        Vector3 toHandProjected = ComputeToTransformProjected(hand.hoverSphereTransform);

        if (!toHandProjected.Equals(lastHandProjected))
        {
            float absAngleDelta = Vector3.Angle(lastHandProjected, toHandProjected);

            if (absAngleDelta > 0.0f)
            {
                Vector3 cross = Vector3.Cross(lastHandProjected, toHandProjected).normalized;
                float dot = Vector3.Dot(worldPlaneNormal, cross);

                float signedAngleDelta = absAngleDelta;

                if (dot < 0.0f)
                {
                    signedAngleDelta = -signedAngleDelta;
                }

                outAngle = Mathf.Clamp(outAngle + signedAngleDelta, MinSpeedHandleAngle, MaxSpeedHandleAngle); ;
                lastHandProjected = toHandProjected;
            }
        }
    }
}
