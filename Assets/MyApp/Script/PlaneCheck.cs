using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

public class PlaneCheck : MonoBehaviour
{
    private RaycastHit _hit;

    [SerializeField]
    private GameObject _stage;

    [SerializeField]
    private GameObject _towerObj;

    private List<GameObject> _towerlist = new();

    [SerializeField]
    private Camera _smartCamera;

    private bool _setUp = true;
    private bool _gameMain = false;
    private int _towerCount = 0;

    private GameObject _stageCash;
    private float _spawnY = 0.5f;

    private Rigidbody _rigidbody;

    bool _stopPobj = true;

    private float _waitTime = 5f; // 待機時間

    private float _waitStartTime = 0f;
    private bool _isWaiting = false;


    private float _yPosition = 0f;
    private float _ypositionCash = 0;

    private GameObject _gameobject;

    private bool _setIsntatiatePosi = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_setUp)
        {
            StageSet();
        }
        else if (_gameMain)
        {
            GamePlay();
        }
    }


    /// <summary>
    /// ゲームのメイン処理になる部分
    /// </summary>
    private void GamePlay()
    {
        if (Input.touchCount > 0 && _gameMain)
        {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began && _stopPobj && _setIsntatiatePosi)
            {
                Vector3 _stagelocalPosition = _stageCash.transform.localPosition;
                Vector3 _newSpawPosition = new(_stagelocalPosition.x, _stagelocalPosition.y + _yPosition+0.4f, _stagelocalPosition.z);
                 _gameobject = Instantiate(_towerObj, _newSpawPosition, Quaternion.identity,_stage.transform);
                _towerlist.Add(_gameobject);
                _rigidbody = _gameobject.GetComponent<Rigidbody>();
                _stopPobj = false;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 _vec3MovePosition = _smartCamera.transform.TransformDirection(new Vector3(touch.deltaPosition.x / 1000, 0, 0));
                _gameobject.transform.localPosition += _vec3MovePosition;
            }

            if (touch.phase == TouchPhase.Ended && _gameMain)
            {
                
                _rigidbody.useGravity = true;
                _towerCount++;
                _waitStartTime = Time.time;
                 _stopPobj = false;     
                _setIsntatiatePosi = false;
                if (_rigidbody.velocity.magnitude > 0.0001)
                {
                    Debug.Log("StartGameWhile" + _rigidbody.velocity.magnitude);
                }
            }
            
        }

        if (!_stopPobj)
        {
            StartCoroutine(WaitTime());
            var velo = GetMaxVelocity();
            if (velo <= 0f)
            {
                _stopPobj = true;
                Debug.Log("StartGameStop" + velo);
            }
        }
       

        if(_stopPobj && !_setIsntatiatePosi)
        {
            
            float i  = UpdateMaxY();
            if(i < 0.4f)
            {
                _yPosition = i;
            }
            Debug.Log("StartGameYposi"+ _yPosition);
            _setIsntatiatePosi = true;
        }
    }

          
    private float UpdateMaxY()
    {
        float i = (_towerlist.Count > 0) ? _towerlist.Max(a => Mathf.Abs(a.transform.localPosition.y)) : float.MinValue;
            _ypositionCash = i;
            Debug.Log("StartGame" + i +"List"+ _towerlist.Count);      
        return i;
    }
    /// <summary>
    /// ゲームをするためのステージの設置場所を決める
    /// </summary>
    private void StageSet()
    {
        if (Input.touchCount > 0 && _setUp)
        {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began)
            {
                var ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out _hit))
                {
                    _stageCash = Instantiate(_stage, _hit.transform.position, Quaternion.identity);
                }
            }
            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 _Vec3StagePosition = _smartCamera.transform.TransformDirection(new Vector3(touch.deltaPosition.x / 1000, 0, 0));
                _stageCash.transform.localPosition += _Vec3StagePosition;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                _gameMain = true;
                _setUp = false;
            }

        }
    }

    /// <summary>
    /// 指定時間待つ
    /// </summary>
    /// <param name="_waitTime">待ちたい時間</param>
    private IEnumerator WaitTime()
    {
        yield return null;
        Debug.Log("StartGameStopCol");
    }


    /// <summary>
    /// リスト内にあるオブジェクト中で一番強いベロシティを返す
    /// </summary>
    /// <returns></returns>
    private float GetMaxVelocity()
    {
        float maxVelocity = 0f;
        foreach (GameObject tower in _towerlist)
        {
            Rigidbody towerRigidbody = tower.GetComponent<Rigidbody>();
            if (towerRigidbody != null)
            {
                float velocityMagnitude = towerRigidbody.velocity.magnitude;
                if (velocityMagnitude > maxVelocity)
                {
                    maxVelocity = velocityMagnitude;
                }
            }
        }

        return maxVelocity;
    }
}
