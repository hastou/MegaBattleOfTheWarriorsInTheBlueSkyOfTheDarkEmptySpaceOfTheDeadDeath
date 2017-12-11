using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CockpitScript : MonoBehaviour
{
    public bool UseXboxController;
    public Slider SpeedSlider;
    public Transform SpeedHandle;
    public float Speed = 1;
    private float _currentSpeedHandleAngle;
    private float _maxSpeedHandleAngle = 110f, _minSpeedHandleAngle = 13f;

    public List<Light> Lights;
    public int ShipDetected;

    public Transform Joystick;

    private AudioSource _motorSound;

    public float MaxVolume = 1.0f, MinVolume = 0.6f;
    public float MaxPitch = 1.0f, MinPitch = 0.6f;

    private Quaternion _quaternionOriginalAngle;

    private Rigidbody _rigidbody;

    private ShieldManager _shieldManager;
    
    public float turnspeed = 10;

    // Use this for initialization
    void Start()
    {
        SpeedSlider.minValue = MinVolume;
        SpeedSlider.maxValue = MaxVolume;
        SpeedSlider.value = MinVolume;

        _motorSound = GetComponent<AudioSource>();
        _motorSound.pitch = MinPitch;
        _motorSound.volume = MinVolume;

        ChangeLightsColor(new Color(250, 250, 250, 255), 1.0f);
        ShipDetected = 0;
        _currentSpeedHandleAngle = _minSpeedHandleAngle;
        _quaternionOriginalAngle = SpeedHandle.transform.localRotation;

        _rigidbody = GetComponent<Rigidbody>();

        _shieldManager = GetComponentInChildren<ShieldManager>();
    }

    public bool isCloakModeEngaged()
    {
        return _shieldManager.CloakModeOn;
    }

    // Update is called once per frame
    void Update()
    {
        _currentSpeedHandleAngle = Quaternion.Angle(_quaternionOriginalAngle, SpeedHandle.transform.localRotation) +
                                   _minSpeedHandleAngle;

        float newAudio = AngleToScale(_currentSpeedHandleAngle, MinVolume, MaxVolume);
        float newPitch = AngleToScale(_currentSpeedHandleAngle, MinPitch, MaxPitch);

        _motorSound.pitch = newPitch;
        _motorSound.volume = newAudio;
        SpeedSlider.value = newAudio;

        if (ShipDetected > 0)
        {
            ChangeLightsColor(new Color(250, 50, 50, 255), 2.0f);
        }
        else if (ShipDetected == 0)
        {
            ChangeLightsColor(new Color(250, 250, 250, 255), 1.0f);
        }
    }

    void ChangeLightsColor(Color newColor, float newIntensity)
    {
        foreach (Light currentLight in Lights)
        {
            currentLight.color = newColor / 255;
            currentLight.intensity = newIntensity;
        }
    }

    float AngleToScale(float angle, float min, float max)
    {
        return (angle - _minSpeedHandleAngle) * (max - min) / (_maxSpeedHandleAngle - _minSpeedHandleAngle) + min;
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (UseXboxController)
        {
            _rigidbody.AddForce(transform.forward * Input.GetAxisRaw("Speed") * Speed * 100);
            _rigidbody.AddRelativeTorque(Input.GetAxisRaw("Pitch") * turnspeed, Input.GetAxisRaw("Yaw") * turnspeed, Input.GetAxisRaw("Roll") * turnspeed, ForceMode.Acceleration);
        }
        else
        {
            _rigidbody.AddForce(transform.forward * SpeedHandle.GetComponent<SpeedDrive>().OutputSpeed * Speed);

            float joystickAngleX = Joystick.GetComponent<JoystickDrive>().outAngleX;
            float joystickAngleZ = Joystick.GetComponent<JoystickDrive>().outAngleZ;

            //Dead zone of 1 angle where the ship does not move
            if (joystickAngleX < 1 && joystickAngleX > -1) joystickAngleX = 0;
            else
            {
                if (joystickAngleX < 1) joystickAngleX -= 1;
                else if (joystickAngleX > -1) joystickAngleX += 1;
            }
            if (joystickAngleZ < 1 && joystickAngleZ > -1) joystickAngleZ = 0;
            else
            {
                if (joystickAngleZ < 1) joystickAngleZ -= 1;
                else if (joystickAngleZ > -1) joystickAngleZ += 1;
            }

            _rigidbody.AddRelativeTorque(joystickAngleX * turnspeed / 3, 0, joystickAngleZ * turnspeed / 3, ForceMode.Acceleration);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        
        if (other.gameObject.CompareTag("Laser"))
        {
            //If the shield has health, deal damage
            if (_shieldManager.ShieldHealth > 0)
            {
                _shieldManager.ImpactShieldSound.Play();
                _shieldManager.ShieldHealth -= 10.0f;
            }
            //Else ?
            else
            {
                OnDeath();
            }
        }

        else if (other.gameObject.CompareTag("Asteroid") || other.gameObject.CompareTag("Enemyship"))
        {
            //If the shield has health, deal damage
            if (_shieldManager.ShieldHealth > 0)
            {
                _shieldManager.ImpactShieldSound.Play();
                _shieldManager.ShieldHealth -= 20.0f;
            }
            //Else ?
            else
            {
                OnDeath();
            }
        }
    }

    void OnDeath()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}