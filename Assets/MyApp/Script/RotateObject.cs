using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] GameObject _gameObject;



    // Update is called once per frame
    void Update()
    {
        _gameObject.transform.Rotate(new Vector3(0, 50, 0) * Time.deltaTime);
    }
}
