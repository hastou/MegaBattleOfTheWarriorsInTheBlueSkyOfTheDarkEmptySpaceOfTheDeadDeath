using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MySceneManager : MonoBehaviour {

    public Text MainScreenText;

    public Animator FadeAnim;
    public Image FadeImg;

    private string _sceneName;

	// Use this for initialization
	void Start () {

        _sceneName = SceneManager.GetActiveScene().name;

        //Debug.Log(_sceneName);

        if (_sceneName == "AsteroidField")
        {
            MainScreenText.text = "Passez par le téléporteur !";
        }
        else if (_sceneName == "deathstar")
        {
            InvokeRepeating("UpdateNumberOfTurrets", 0.0f, 1.0f);
        }
	}

    void UpdateNumberOfTurrets()
    {
        int remainingNumberOfTurrets = GameObject.FindGameObjectsWithTag("Turret").Length;
        if (remainingNumberOfTurrets == 0)
        {
            CancelInvoke();
            MainScreenText.text = "Mission terminée !";
            StartCoroutine("ChangeScene");
        }
        else MainScreenText.text = "Nombre de tourelles restant : " + remainingNumberOfTurrets;
    }

    IEnumerator ChangeScene()
    {
        FadeAnim.SetBool("fade", true);
        yield return new WaitUntil(() => FadeImg.color.a == 1);
        if (_sceneName == "AsteroidField")
        {
            SceneManager.LoadScene("deathstar");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerAllyTarget")
        {
            MainScreenText.text = "Mission terminée !";
            StartCoroutine("ChangeScene");
        }
    }

}
