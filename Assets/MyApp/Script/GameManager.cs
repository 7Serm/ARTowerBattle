using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject _stage;
    [SerializeField] GameObject _towerobj;
    [SerializeField] Camera _smartcamera;

    private List<GameObject> _towerlist = new();
    private GameObject _stagecash;

    Vector3 _newSpawPosition;
    GameObject _gameobject;

    private void Start()
    {
        StartCoroutine(Main());
    }
    IEnumerator Main()
    {
        yield return  StageSet();
       
     //   yield return GameMain(); 
    }


   IEnumerator StageSet()
    {
        bool stageset = false;
        RaycastHit _hit;
        while (!stageset)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];
                if (touch.phase == TouchPhase.Began)
                {
                    var ray = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out _hit))
                    {
                        _stagecash = Instantiate(_stage, _hit.transform.position, Quaternion.identity);
                    }
                }
               /* if (touch.phase == TouchPhase.Moved)
                {
                    Vector3 _Vec3StagePosition = _smartcamera.transform.TransformDirection(new Vector3(touch.deltaPosition.x / 1000, 0, 0));
                    _stagecash.transform.localPosition += _Vec3StagePosition;
                }

                if (touch.phase == TouchPhase.Ended)
                {
                     stageset = true;
                }*/

            }
        }
        yield return null;
    }



    IEnumerator GameMain()
    {
        Rigidbody _rigidbody = null;
        bool _gamelife = true;
        
        var _yposition = 0f;
        while (_gamelife)
        {
           // GameObject _gameObject;
            Vector3 _stagelocalPosition = _stagecash.transform.position;
           _newSpawPosition  = new(_stagelocalPosition.x, _stagelocalPosition.y + _yposition, _stagelocalPosition.z);
            if (Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];
                if (touch.phase == TouchPhase.Began)
                {
                    
                     _gameobject = Instantiate(_towerobj, _newSpawPosition, Quaternion.identity);
                    
                    _rigidbody = _gameobject.GetComponent<Rigidbody>();
                    Debug.Log("StartGame" + _stagecash.transform.position + "Stage");
                    Debug.Log("StartGameY" + _newSpawPosition.y);
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    Vector3 _vec3MovePosition = _smartcamera.transform.TransformDirection(new Vector3(touch.deltaPosition.x / 1000, 0, 0));
                     _gameobject.transform.localPosition += _vec3MovePosition;

                }

                if (touch.phase == TouchPhase.Ended)
                {
                    _rigidbody.useGravity = true;
                    _towerlist.Add(_gameobject);
                    //  Debug.Log("StartGame");

                }
            }
        }

        yield return null;
    }
}
