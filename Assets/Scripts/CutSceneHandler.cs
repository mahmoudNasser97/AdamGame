using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
namespace Opsive.UltimateCharacterController.Character
{
    using Opsive.Shared.Events;
    using Opsive.Shared.Game;
    using Opsive.UltimateCharacterController.Character.Abilities;
    using Opsive.UltimateCharacterController.Character.Abilities.Items;
    using Opsive.UltimateCharacterController.Game;
    using Opsive.UltimateCharacterController.Input;
    using Opsive.UltimateCharacterController.StateSystem;

    public class CutSceneHandler : MonoBehaviour
    {
        public VideoPlayer videoPlayer;
        public GameObject videoScreen;
        public UltimateCharacterLocomotionHandler playerController;
        private void Start()
        {
            if (videoScreen != null)
                videoScreen.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayCutscene();
                playerController.gameObject.SetActive(false);
            }
        }

        private void PlayCutscene()
        {
            if (videoPlayer != null)
            {
                if (videoScreen != null)
                    videoScreen.SetActive(true);

                videoPlayer.Play();

                videoPlayer.loopPointReached += OnVideoEnd;
            }
        }

        private void OnVideoEnd(VideoPlayer vp)
        {
            if (videoScreen != null)
                videoScreen.SetActive(false);

            vp.Stop();
            playerController.gameObject.SetActive(true);
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}
