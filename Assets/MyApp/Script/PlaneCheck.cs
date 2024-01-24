using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaneCheck : MonoBehaviour
{
    private RaycastHit _hit;

    [SerializeField]
    private GameObject _stage;

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
            if (touch.phase == TouchPhase.Began)
            {
                var ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out _hit))
                {
                    Instantiate(_stage, _hit.transform.position, _hit.transform.rotation);
                }
            }

            if(touch.phase == TouchPhase.Moved)
            {
                _stage.transform.position = touch.deltaPosition;
            }

        }

        /*if(_arplaneManager.trackables.count > 0)
        {
            Instantiate(gameObject);
        }*/
    }
}
