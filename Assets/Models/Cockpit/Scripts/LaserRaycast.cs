using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRaycast : MonoBehaviour
{
    public Transform RaycastOrigin;
    public float RateOfFire;
    public float LaserRange;
    public float LaserSpread;
    public float Damage;

    public bool CanShoot;

    private WaitForSeconds _laserDuration = new WaitForSeconds(0.05f);
    private LineRenderer _laserLine;
    private AudioSource _laserSound;
    private float _initRateOfFire;
    private float _laserSpawnSide = 1.0f;

    // Use this for initialization
    void Start()
    {
        _laserLine = GetComponent<LineRenderer>();
        _laserSound = GetComponent<AudioSource>();
        _initRateOfFire = RateOfFire;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) CanShoot = true;
        else if (Input.GetButtonUp("Fire1")) CanShoot = false;

        Shoot(CanShoot);
        RateOfFire -= Time.deltaTime;
    }

    private void Shoot(bool shoot)
    {
        if (!_laserLine.enabled && shoot && RateOfFire <= 0.0f)
        {
            _laserSound.Play();

            _laserLine.enabled = true;

            StartCoroutine("LaserShot");

            //Alternate the laser spawning side
            Vector3 laserSpawnPoint = transform.position;
            laserSpawnPoint += transform.right * LaserSpread * _laserSpawnSide;
            _laserSpawnSide *= (-1.0f);

            Vector3 raycastOrigin = RaycastOrigin.transform.position;
            RaycastHit hit;

            _laserLine.SetPosition(0, laserSpawnPoint);

            if (Physics.Raycast(raycastOrigin, RaycastOrigin.transform.forward, out hit, LaserRange))
            {
                _laserLine.SetPosition(1, hit.point);

                if (hit.collider.gameObject.tag == "Turret")
                {
                    TurretScript turretScript = hit.collider.GetComponent<TurretScript>();
                    if (turretScript.TurretHealth > 0) turretScript.TurretHealth -= Damage;
                    else
                    {
                        turretScript.Explode();
                    }
                }
            }
            else _laserLine.SetPosition(1, raycastOrigin + (transform.forward * LaserRange));

            RateOfFire = _initRateOfFire;
        }
    }

    IEnumerator LaserShot()
    {
        float startWidth = 4.0f;

        for (float w = startWidth; w > 0; w -= 0.5f)
        {
            //New shield icon alpha
            _laserLine.startWidth = w;
            _laserLine.endWidth = w;

            yield return null;
        }

        _laserLine.enabled = false;
    }
}