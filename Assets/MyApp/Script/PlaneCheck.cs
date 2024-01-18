using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaneCheck : MonoBehaviour
{
    private RaycastHit _hit;

    [SerializeField]
    private GameObject _gameObject;

    private ARPlaneManager _arplaneManager;
    // Start is called before the first frame update
    void Start()
    {
        _arplaneManager = GetComponent<ARPlaneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            Debug.Log("Sample1");
                if (touch.phase == TouchPhase.Began)
                {
               
                    var ray = Camera.main.ScreenPointToRay(touch.position);
                 Debug.Log("Sample2");
                if (Physics.Raycast(ray, out _hit))
                    {
                        Instantiate(_gameObject, _hit.transform.position, _hit.transform.rotation);
                    Debug.Log("Sample3");
                    Debug.Log("Sample4"+_hit.collider.gameObject.name);
                    }
                }
            
        }

        /*if(_arplaneManager.trackables.count > 0)
        {
            Instantiate(gameObject);
        }*/
    }
}
