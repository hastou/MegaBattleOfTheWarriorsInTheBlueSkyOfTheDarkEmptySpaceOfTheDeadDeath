using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastRenderer : MonoBehaviour {

    public float LaserRange;
    public Transform RaycastOrigin;
	
	// Update is called once per frame
	void Update () {
        Vector3 lineOrigin = RaycastOrigin.position;
        Debug.DrawRay(lineOrigin, RaycastOrigin.forward * LaserRange, Color.green);
	}
}
