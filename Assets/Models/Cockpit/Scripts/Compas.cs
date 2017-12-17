using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compas : MonoBehaviour {

    public GameObject Stargate;

	// Update is called once per frame
	void Update () {
        transform.LookAt(Stargate.transform);
	}
}
