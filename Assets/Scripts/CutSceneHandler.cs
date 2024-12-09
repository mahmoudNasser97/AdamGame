using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

    public class CutSceneHandler : MonoBehaviour
    {
        public VideoPlayer videoPlayer;
        public GameObject videoScreen;
        public BoxCollider cutSceneTrigger;
    public GameObject cameraObject;
    public Canvas playerControls;
    public BoxCollider questCollider;
    public AudioSource rainSFX;
    public AudioSource backGroundSFX;
    public bool endScene=false;
    public Button skipButton;
    private void Start()
    {
        if (videoScreen != null)
            videoScreen.SetActive(false);
        StartCoroutine(InteractButton());
    }

    IEnumerator InteractButton()
    { 
        yield return new WaitForSeconds(3f);
        skipButton.interactable = true;
    }

    private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayCutscene();
            cameraObject.SetActive(true);
            playerControls.enabled = false;
            rainSFX.Stop();
            backGroundSFX.Stop();

                //playerController.gameObject.SetActive(false);
                //cutSceneTrigger.gameObject.SetActive(false);

            }
        }

        private void PlayCutscene()
        {
            if (videoPlayer != null)
            {
                if (videoScreen != null)
                    videoScreen.SetActive(true);

                videoPlayer.Play();
            skipButton.gameObject.SetActive(true);
                videoPlayer.loopPointReached += OnVideoEnd;
            }
        }

    public void SkipCutScene()
    {
        OnVideoEnd(videoPlayer);

    }
        private void OnVideoEnd(VideoPlayer vp)
        {
            if (videoScreen != null)
                videoScreen.SetActive(false);

            vp.Stop();
        //playerController.gameObject.SetActive(true);
        cameraObject.SetActive(false);
        playerControls.enabled = true;
        Destroy(questCollider.gameObject);
        rainSFX.Play();
        backGroundSFX.Play();
        cutSceneTrigger.gameObject.SetActive(false);
            videoPlayer.loopPointReached -= OnVideoEnd;
        skipButton.gameObject.SetActive(false);
        if(endScene==true)
        {
            //LoadScene("MainMenu");
        }
        }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
