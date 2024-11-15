using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageChanger : MonoBehaviour
{
    string arLanguage = "Ar";
    string enLanguage = "En";
    // Start is called before the first frame update
    void Start()
    {
        DialogueManager.SetLanguage("En");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(PlayerPrefs.GetString("Language") + "Language");
    }
    public void ChangeLang()
    {
        if (PlayerPrefs.GetString("Language") == arLanguage)
        {
            DialogueManager.SetLanguage("En");

        }
        else if (PlayerPrefs.GetString("Language") == enLanguage)
        {
            DialogueManager.SetLanguage("Ar");
        }
        Debug.Log("Language Changeeeeddd");
    }
    public void ChangeIntoArabic()
    {
        DialogueManager.SetLanguage("Ar");
    }
    public void ChangeIntoEnglish()
    {
        DialogueManager.SetLanguage("En");
    }
}
