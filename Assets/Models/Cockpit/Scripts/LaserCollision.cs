using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCollision : MonoBehaviour
{
    //The particle to be instanciated on collision
    public GameObject Particle;

    void OnCollisionEnter(Collision other)
    {
        //On collision, instantiate the particle at contact point 0
        GameObject particle = Instantiate(Particle, other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal));
        particle.transform.SetParent(gameObject.transform.parent, true);

        //Destroy the laser
        Destroy(gameObject);
    }
}