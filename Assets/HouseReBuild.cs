using RayFire;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseReBuild : MonoBehaviour
{
    [SerializeField] RayfireShatter RayfireShatter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RayfireShatter.previewScale--;
        if (Input.GetKey(KeyCode.P))
        {
            PlayerInput();
        }
    }
    public void PlayerInput()
    {
        RayfireShatter.previewScale--;
    }
}
