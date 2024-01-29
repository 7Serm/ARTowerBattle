using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaneCheck : MonoBehaviour
{
    private RaycastHit _hit;

    [SerializeField]
    private GameObject _stage;

    [SerializeField]
    private GameObject _towerObj;

    private List<GameObject>_towerlist = new();

    [SerializeField]
    private Camera _smartCamera;

    private bool _setUp = true;
    private bool _gameMain = false;
    private int  _towerCount = 0;

    private GameObject _stageCash;
    private float _spawnY = 0.5f;

    private new Rigidbody _rigidbody;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        StageSet();
        GamePlay();
    }

    private void GamePlay()
    {
         Vector3 _stagePosition = _stageCash.transform.position;
         Vector3  _newSpawPosition = new Vector3(_stagePosition.x,_stagePosition.y + _spawnY ,_stagePosition.z);
        if (Input.touchCount > 0 && _gameMain == true)
        {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began)
            {
                var ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out _hit))
                {
                    GameObject newgame = Instantiate(_towerObj ,_newSpawPosition,Quaternion.identity);
                    _rigidbody = newgame.GetComponent<Rigidbody>();
                    _towerlist.Add(newgame);
                    Debug.Log("StartGame");
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 _vec3MovePosition = _smartCamera.transform.TransformDirection(new Vector3(touch.deltaPosition.x / 1000, 0, 0));
                _towerlist[_towerCount].transform.localPosition += _vec3MovePosition;
            }

            if(touch.phase == TouchPhase.Ended)
            {
                _rigidbody.useGravity = true;
                _towerCount++;
            }

        }
    }

    private void StageSet()
    {
        if(Input.touchCount > 0  && _setUp == true)
        {
            Touch touch = Input.touches[0];
            if(touch.phase == TouchPhase.Began)
            {
                var ray = Camera.main.ScreenPointToRay(touch.position);
                if(Physics.Raycast(ray, out _hit))
                {
                  _stageCash = Instantiate(_stage,_hit.transform.position, _hit.transform.rotation);
                }
                Debug.Log("Start");
                _gameMain = true;
                _setUp = false;
            }
        }
      
    }
}
