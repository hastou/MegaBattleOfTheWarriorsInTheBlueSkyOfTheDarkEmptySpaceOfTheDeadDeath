using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    private GameObject Target;
    public GameObject MovingBarrelPart;
    public GameObject FirstTurretMovingPart;
    public GameObject SecondTurretMovingPart;
    public GameObject Laser;
    public GameObject BarrelOut;
    public GameObject ExplosionEffect;
    public int LaserSpeed = 5000;
    public float LaserLifetime = 2.5f;

    public float FireRange = 100;

    public float TurretHealth = 100;

    private AudioSource laserSound;

    private Quaternion _originalRotation;
    private Quaternion _originalSecondPartRotation;
    public float CoolDown = 1f;
    private float _coolDownTimer;

    private SphereCollider _targetEnterTrigger;


    // Use this for initialization
    void Start()
    {
        _originalRotation = FirstTurretMovingPart.transform.rotation;
        _originalSecondPartRotation = SecondTurretMovingPart.transform.rotation;
        _targetEnterTrigger = gameObject.AddComponent<SphereCollider>();
        _targetEnterTrigger.radius = FireRange;
        _targetEnterTrigger.isTrigger = true;
        _coolDownTimer = CoolDown;
    }

    void LookAtAroundYAxis(Transform target)
    {
        Vector3 targetPos = FirstTurretMovingPart.transform.InverseTransformPoint(target.position);
        targetPos.y = 0;
        targetPos = FirstTurretMovingPart.transform.TransformPoint(targetPos);
        FirstTurretMovingPart.transform.LookAt(targetPos, FirstTurretMovingPart.transform.up);
    }

    void LookAtAroundXAxis(Transform target)
    {
        Vector3 targetDirection = target.position - SecondTurretMovingPart.transform.position;
        Quaternion rotation = Quaternion.LookRotation(targetDirection, SecondTurretMovingPart.transform.up);

        if (Quaternion.Angle(FirstTurretMovingPart.transform.rotation, rotation) <= 30)
        {
            SecondTurretMovingPart.transform.rotation = rotation;
//            SecondTurretMovingPart.transform.LookAt(targetPos, FirstTurretMovingPart.transform.up);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Target && !Target.GetComponentInParent<CockpitScript>().isCloakModeEngaged())
        {
            Debug.DrawLine(FirstTurretMovingPart.transform.position, FirstTurretMovingPart.transform.position + FirstTurretMovingPart.transform.forward * 100, Color.red);
            LookAtAroundYAxis(Target.transform);
            LookAtAroundXAxis(Target.transform);

            _coolDownTimer -= Time.deltaTime;
            if (_coolDownTimer < 0)
            {
                Fire();
                _coolDownTimer = CoolDown;
            }
        }
    }

    void Fire()
    {
        if (BarrelOut)
        {
            GameObject i = Instantiate(Laser, BarrelOut.transform.position, BarrelOut.transform.rotation);
            i.transform.localScale = new Vector3(1, 1, 1) * LaserSpeed/5000f;
            i.GetComponent<LineRenderer>().widthMultiplier = LaserSpeed/5000f;
            i.GetComponent<Rigidbody>().AddForce(i.transform.forward * LaserSpeed, ForceMode.Acceleration);
            i.layer = LayerMask.NameToLayer("Destroyer");
            Destroy(i, LaserLifetime);
            if (laserSound) laserSound.Play();
        }
    }


    public void Explode()
    {
        Instantiate(ExplosionEffect, transform.position, transform.rotation);
        Destroy(gameObject, 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAllyTarget"))
        {
            CockpitScript cockpitScript = other.gameObject.GetComponentInParent<CockpitScript>();
            if (cockpitScript)
            {
                Target = other.gameObject;
                cockpitScript.ShipDetected++;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Target)
        {
            if (Target.GetComponentInParent<CockpitScript>())
            {
                Target.GetComponentInParent<CockpitScript>().ShipDetected--;
            }
            Target = null;
        }
    }
}