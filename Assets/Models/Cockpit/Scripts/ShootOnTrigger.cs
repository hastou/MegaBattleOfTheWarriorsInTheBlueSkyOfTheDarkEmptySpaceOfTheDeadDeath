using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ShootOnTrigger : MonoBehaviour
{
    public List<GameObject> lasers;
	public GameObject target;
	private bool followTarget = false;

    // Update is called once per frame
    void Update () {
		if (followTarget)
		{
            foreach (GameObject laser in lasers)
            {
                float ytheta = Mathf.Atan(laser.transform.localPosition.x / target.transform.localPosition.z) * Mathf.Rad2Deg;
                float xtheta = Mathf.Atan(laser.transform.localPosition.y / target.transform.localPosition.z) * Mathf.Rad2Deg;
                laser.transform.eulerAngles = new Vector3(xtheta, -ytheta, 0);
                laser.SendMessage("Shoot", true);
            }
        }
    }

	private void OnTriggerEnter(Collider other)
	{
        if (other.gameObject.name == "Target")
        {
            followTarget = true;
            target = other.gameObject;
        }
	}

	private void OnTriggerExit(Collider other)
	{
        if (other.gameObject.name == "Target") followTarget = false;
	}
}
