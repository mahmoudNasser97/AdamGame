using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageChanger : MonoBehaviour
{
    string arLanguage = "Ar";
    string enLanguage = "En";

    void Start()
    {

    }
    void Update()
    {
        Debug.Log(PlayerPrefs.GetString("Language") + "Language");
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
    public void SetArabicLanguage()
    {
        //if (PlayerPrefs.GetString("Language") == enLanguage)
        //{
            DialogueManager.SetLanguage("Ar");
        //}
    }
    public void SetEnglishLanguage()
    {
        //if (PlayerPrefs.GetString("Language") == arLanguage)
        //{
            DialogueManager.SetLanguage("En");
        //}
    }
}
