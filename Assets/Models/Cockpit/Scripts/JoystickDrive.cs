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
public class JoystickDrive : MonoBehaviour
{
    public LaserRaycast LaserGun;

    public Collider childCollider = null;

    public bool hoverLock = false;

    public float minAngle = -10.0f;

    public float maxAngle = 10.0f;

    public float outAngleX;
    public float outAngleZ;

    private Vector3 _worldPlaneNormalX;
    private Vector3 _worldPlaneNormalZ;

    private Vector3 _lastHandProjectedX;
    private Vector3 _lastHandProjectedZ;

    private bool _driving = false;

    Hand handHoverLocked = null;

    //-------------------------------------------------
    void Start()
    {
        if (childCollider == null)
        {
            childCollider = GetComponentInChildren<Collider>();
        }

        _worldPlaneNormalX = new Vector3(1.0f, 0.0f, 0.0f);
        _worldPlaneNormalZ = new Vector3(0.0f, 0.0f, 1.0f);

        if (transform.parent)
        {
            _worldPlaneNormalX = transform.parent.localToWorldMatrix.MultiplyVector(_worldPlaneNormalX).normalized;
            _worldPlaneNormalZ = transform.parent.localToWorldMatrix.MultiplyVector(_worldPlaneNormalZ).normalized;
        }

        outAngleX = transform.localEulerAngles[0];
        outAngleX = transform.localEulerAngles[2];

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

        if (_driving && hand.GetStandardInteractionButton())
        {
            StartCoroutine(HapticPulses(hand.controller, 1.0f, 10));
        }

        LaserGun.CanShoot = false;

        outAngleX = 0;
        outAngleZ = 0;
        UpdateGameObject();

        _driving = false;
        handHoverLocked = null;
    }


    //-------------------------------------------------
    private void HandHoverUpdate(Hand hand)
    {
        if (hand.GetStandardInteractionButtonDown())
        {
            // Trigger was just pressed
            _lastHandProjectedX = ComputeToTransformProjected(hand.hoverSphereTransform, true);
            _lastHandProjectedZ = ComputeToTransformProjected(hand.hoverSphereTransform, false);

            if (hoverLock)
            {
                hand.HoverLock(GetComponent<Interactable>());
                handHoverLocked = hand;
            }

            _driving = true;

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

            LaserGun.CanShoot = false;

            outAngleX = 0;
            outAngleZ = 0;
            UpdateGameObject();
        }
        else if (_driving && hand.GetStandardInteractionButton() && hand.hoveringInteractable == GetComponent<Interactable>())
        {
//            if (hand.controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad)) LaserGun.CanShoot = true;
//            else LaserGun.CanShoot = false;

            ComputeAngle(hand);
            UpdateGameObject();
        }
    }


    //-------------------------------------------------
    private Vector3 ComputeToTransformProjected(Transform xForm, bool xAxis)
    {
        Vector3 toTransform = (xForm.localPosition - transform.localPosition).normalized;
        Vector3 toTransformProjected = new Vector3(0.0f, 0.0f, 0.0f);

        // Need a non-zero distance from the hand to the center of the CircularDrive
        if (toTransform.sqrMagnitude > 0.0f)
        {
            toTransformProjected = (xAxis ? Vector3.ProjectOnPlane(toTransform, _worldPlaneNormalX).normalized : Vector3.ProjectOnPlane(toTransform, _worldPlaneNormalZ).normalized);
        }

        return toTransformProjected;
    }

    //-------------------------------------------------
    // Updates the GameObject rotation
    //-------------------------------------------------
    void UpdateGameObject()
    {
        transform.localRotation = Quaternion.AngleAxis(outAngleX, _worldPlaneNormalX) * Quaternion.AngleAxis(outAngleZ, _worldPlaneNormalZ);
    }

    //-------------------------------------------------
    // Computes the angle to rotate the game object based on the change in the transform
    //-------------------------------------------------
    private void ComputeAngle(Hand hand)
    {
        GetOutAngle(hand, true);
        GetOutAngle(hand, false);
    }

    void GetOutAngle(Hand hand, bool xAxis)
    {
        Vector3 toHandProjected = ComputeToTransformProjected(hand.hoverSphereTransform, xAxis);
        Vector3 lastHandProjected = (xAxis ? _lastHandProjectedX : _lastHandProjectedZ);

        float outAngle = (xAxis ? outAngleX : outAngleZ);

        if (!toHandProjected.Equals(lastHandProjected))
        {
            float absAngleDelta = Vector3.Angle(lastHandProjected, toHandProjected);

            if (absAngleDelta > 0.0f)
            {
                Vector3 cross = Vector3.Cross(lastHandProjected, toHandProjected).normalized;
                float dot = (xAxis ? Vector3.Dot(_worldPlaneNormalX, cross) : Vector3.Dot(_worldPlaneNormalZ, cross));

                float signedAngleDelta = absAngleDelta;

                if (dot < 0.0f)
                {
                    signedAngleDelta = -signedAngleDelta;
                }

                outAngle = Mathf.Clamp(outAngle + signedAngleDelta, minAngle, maxAngle);

                if (xAxis) _lastHandProjectedX = toHandProjected;
                else _lastHandProjectedZ = toHandProjected;
            }
        }

        if (xAxis) outAngleX = outAngle;
        else outAngleZ = outAngle;

    }
}
