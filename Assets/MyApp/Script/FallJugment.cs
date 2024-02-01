using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallJugment : MonoBehaviour
{
   private bool _falljudg = false;

    public bool Falljudg { get => _falljudg; set => _falljudg = value; }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("towerobj"))
        {
            _falljudg = true;
         //   Debug.Log("StartGameOVER");
        }
        
    }
}
