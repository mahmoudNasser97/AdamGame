using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipCutScene : MonoBehaviour
{
    public GameObject cutscene;
    public GameObject skipButton; 

    private bool isCutscenePlaying = true;

    void Start()
    {
        cutscene.SetActive(true);
        skipButton.SetActive(true);

        skipButton.GetComponent<Button>().onClick.AddListener(SkipCutscene);
        StartCoroutine(InteractButton());
    }
    IEnumerator InteractButton()
    {
        yield return new WaitForSeconds(3f);
        skipButton.GetComponent<Button>().interactable = true;
    }
    public void SkipCutscene()
    {
        EndCutscene();
    }

    private void EndCutscene()
    {
        isCutscenePlaying = false;
        if (cutscene != null)
            cutscene.SetActive(false);

        skipButton.SetActive(false);
    }

}
