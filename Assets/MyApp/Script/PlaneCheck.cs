using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private float _waitTime = 1f; // 待機時間

    private float _waitStartTime = 0f;
    private bool _isWaiting = false;
    private bool _firstloop = true;
    private int _yPosition;
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
        Vector3 _stagePosition = _stageCash.transform.position;
        Vector3 _newSpawPosition = new(_stagePosition.x, _stagePosition.y + _spawnY+_yPosition, _stagePosition.z);
        GameObject _gameObject = new();
        if (_stopPobj)
        {
            _gameObject = Instantiate(_towerObj, _newSpawPosition, Quaternion.identity);
            _towerlist.Add(_gameObject);
            _rigidbody = _gameObject.GetComponent<Rigidbody>();
            _stopPobj = false;
        }
        if (Input.touchCount > 0 && _gameMain)
        {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began)
            {
                var ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out _hit))
                {
                  
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 _vec3MovePosition = _smartCamera.transform.TransformDirection(new Vector3(touch.deltaPosition.x / 1000, 0, 0));
                _towerlist[_towerCount].transform.localPosition += _vec3MovePosition;

            }

            if (touch.phase == TouchPhase.Ended && _gameMain)
            {
                _rigidbody.useGravity = true;
                _towerCount++;               
              //  Debug.Log("StartGame");
                _isWaiting = true;
                _waitStartTime = Time.time;
                _firstloop = false;
            }
            if (_isWaiting)
            {
                float elapsedTime = Time.time - _waitStartTime;

                if (elapsedTime >= _waitTime)
                {
                 //   Debug.Log("StartGame" + _waitTime + " seconds.");
                    _isWaiting = false;
                    _stopPobj = true;
                }
            }
            if (!_firstloop && _stopPobj)
            {
                var maxY = (_towerlist.Count > 0) ? _towerlist.Max(a => Mathf.Abs(a.transform.localPosition.y)) : float.MinValue;
                _yPosition = Mathf.CeilToInt(maxY);
                Debug.Log("StartGame" + maxY);
            }
        }
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
                    _stageCash = Instantiate(_stage, _hit.transform.position, _hit.transform.rotation);
                }
            }
            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 _Vec3StagePosition = _smartCamera.transform.TransformDirection(new Vector3(touch.deltaPosition.x / 1000,0,0));
                _stageCash.transform.localPosition += _Vec3StagePosition;
            }

            if (touch.phase == TouchPhase.Ended)
            {

             //   Debug.Log("StartGameStage");
                _gameMain = true;
                _setUp = false;
            }
            
        }
    }

    /// <summary>
    /// 指定時間待つ
    /// </summary>
    /// <param name="_waitTime">待ちたい時間</param>
    private IEnumerator WaitTime(float _waitTime)
    {
        yield return new WaitForSeconds(_waitTime);
    }


    /// <summary>
    /// リスト内にあるオブジェクト中で一番強いベロシティを返す
    /// </summary>
    /// <returns></returns>
    private float GetMaxVelocity()
    {
        float _magnitude = 0f;
        foreach(GameObject _veloList in _towerlist)
        {
            var r = _veloList.GetComponent<Rigidbody>();
            var m = r.velocity.magnitude;
            if(_magnitude < m)
            {
                _magnitude = m;
            }
        }
        return _magnitude;
    }
}
