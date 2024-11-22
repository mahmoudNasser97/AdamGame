using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNumbers : MonoBehaviour
{
    void Update()
    {
        //if(this.gameObject.active)
        //{           
         //   StartCoroutine(HideNumber());
       // }
    }
    public void EnableNumbers()
    {
        this.gameObject.SetActive(true);
        StartCoroutine(HideNumber());
    }
    IEnumerator HideNumber()
    {
        yield return new WaitForSeconds(2f);
        this.gameObject.SetActive(false);
    }
}
