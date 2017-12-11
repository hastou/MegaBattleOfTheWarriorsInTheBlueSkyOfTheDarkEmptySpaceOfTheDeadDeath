using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMusic : MonoBehaviour
{
    private bool isNextSceneLoading = false;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
//        if (Input.GetButtonDown("Fire1") && isNextSceneLoading == false)
//        {
//            StartCoroutine("FadeOutAndLoadLevel");
//        }
    }

    public void StartTitleMusic()
    {
        GetComponent<AudioSource>().Play();
    }

    public IEnumerator FadeOutAndLoadLevel()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        while (audioSource.pitch > 1)
        {
            audioSource.pitch -= 0.001f;
            yield return new WaitForSeconds(0.001f);
        }
        
        isNextSceneLoading = true;
        
        AsyncOperation async = Application.LoadLevelAsync("AsteroidField");
        yield return async;

        
        Debug.Log("Loading complete");
        
        while (audioSource.volume > 0f)
        {
            audioSource.volume -= 0.0005f;
            yield return new WaitForSeconds(0.001f);
        }
        DestroyImmediate(gameObject);
    }
}