using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LasersManager : MonoBehaviour {

    //Laser to be instanciated
    public GameObject Laser;
    //Laser spawn rate
    public float RateOfFire = 0.1f;
    //Distance between the laser spawn points
    public float LaserSpread = 0.8f;
    //Speed factor for the laser's rigidbody force
    public int LaserSpeed = 5000;
    //Time before the laser is destroyed
    public float LaserLifeTime = 2.5f;
    //Collider to be ignored
    public CapsuleCollider ShieldCollider;

    private float _initRateOfFire;
    private bool _isShooting = false;
    private AudioSource _laserSound;
    private float _laserSpawnSide = 1.0f;

    void Start()
    {
        _laserSound = GetComponent<AudioSource>();
        _initRateOfFire = RateOfFire;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) _isShooting = true;
        else if (Input.GetButtonUp("Fire1")) _isShooting = false;

        Shoot(_isShooting);
        RateOfFire -= Time.deltaTime;
    }

    private void Shoot(bool shoot)
    {
        if (shoot && RateOfFire <= 0.0f)
        {
            _laserSound.Play();

            //Alternate the laser spawning side
            Vector3 laserSpawnPoint = gameObject.transform.position;
            laserSpawnPoint += transform.right * LaserSpread * _laserSpawnSide;
            _laserSpawnSide *= (-1.0f);

            GameObject laser = Instantiate(Laser, laserSpawnPoint, gameObject.transform.rotation) as GameObject;
            //laser.GetComponent<Rigidbody>().velocity = transform.parent.transform.GetComponent<Rigidbody>().velocity;
            laser.GetComponent<Rigidbody>().AddForce(laser.transform.forward * LaserSpeed, ForceMode.Acceleration);

            //Ignore collisions with the shield around the cockpit
            Physics.IgnoreCollision(ShieldCollider, laser.GetComponent<Collider>());

            Destroy(laser, LaserLifeTime);
            RateOfFire = _initRateOfFire;
        }
    }
}
