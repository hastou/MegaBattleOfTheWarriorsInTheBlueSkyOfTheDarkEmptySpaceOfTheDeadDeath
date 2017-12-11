using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldManager : MonoBehaviour {

    public Button ShieldModeButton;
    public RawImage CloakMode, ShieldMode;

    public Image CloakStatus, ShieldStatus;

    public float ShieldHealth = 100;

    private MeshRenderer _shieldMR;

    public bool CloakModeOn;

    private AudioSource _toggleShieldSound;

    public AudioSource ImpactShieldSound;

    private bool _coroutineRunning;

    void Start()
    {
        _coroutineRunning = false;
        _shieldMR = GetComponent<MeshRenderer>();
        _toggleShieldSound = GetComponents<AudioSource>()[0];
        ImpactShieldSound = GetComponents<AudioSource>()[1];
        CloakModeOn = false;

        //Update the shield values every 0.1s
        InvokeRepeating("UpdateShieldStatus", 0.0f, 0.1f);
    }

    void UpdateShieldStatus()
    {
        if (ShieldHealth < 100.0f) ShieldHealth = Mathf.Min(100.0f, ShieldHealth + 0.2f);
        ShieldStatus.fillAmount = ShieldHealth / 100.0f;

        if (CloakModeOn)
        {
            if (CloakStatus.fillAmount > 0.0f) CloakStatus.fillAmount -= 0.007f;
            else ToggleMode();
        }
        else
        {
            if (CloakStatus.fillAmount < 1.0f) CloakStatus.fillAmount += 0.007f;
        }
    }

    //When the image is clicked, switch between cloak and shield mode
	public void ToggleMode()
    {
        //If the coroutine is not running
        if (!_coroutineRunning)
        {
            CloakModeOn = !CloakModeOn;
            _toggleShieldSound.Play();
            _coroutineRunning = true;
            StartCoroutine("ChangeShieldIcon");
        } 
    }

    IEnumerator ChangeShieldIcon()
    {
        float startAlpha = 0.7f;

        for (float f = 0; f <= startAlpha; f += 0.005f)
        {
            //New shield icon alpha
            Color newColor = CloakMode.color;
            newColor.a = (CloakModeOn) ?  f : 0.7f - f;
            CloakMode.color = newColor;

            //New cloak icon alpha
            newColor = ShieldMode.color;
            newColor.a = (CloakModeOn) ? 0.7f - f : f;
            ShieldMode.color = newColor;

            //New shield : add color modification
            newColor = _shieldMR.material.color;
            newColor.a = (f > startAlpha/2 ? (0.7f - f) : f);
            _shieldMR.material.color = newColor;

            yield return null;
        }

        //Coroutine can be started again
        _coroutineRunning = false;
    }
}
