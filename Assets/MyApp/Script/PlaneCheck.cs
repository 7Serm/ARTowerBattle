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

    private float _waitTime = 5f; // 待機時間

    private float _waitStartTime = 0f;
    private bool _isWaiting = false;


    private float _yPosition = 0.4f;
    private float _ypositionCash = 0;
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
            if (touch.phase == TouchPhase.Began)
            {
                Vector3 _stagelocalPosition = _stageCash.transform.position;
                Vector3 _newSpawPosition = new(_stagelocalPosition.x, _stagelocalPosition.y + _yPosition, _stagelocalPosition.z);
                GameObject _gameObject = Instantiate(_towerObj, _newSpawPosition, Quaternion.identity);
                _towerlist.Add(_gameObject);
                _rigidbody = _gameObject.GetComponent<Rigidbody>();
                _stopPobj = false;
                Debug.Log("StartGame" + _stageCash.transform.position+"Stage");
                Debug.Log("StartGameY" + _newSpawPosition);
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
                _waitStartTime = Time.time;
              
            }
            
            var velo = GetMaxVelocity();
           /* Debug.Log("StartGameEND"+velo);
            Debug.Log("StartGameEND" +_towerlist.Count);*/

            while (velo > 0.01f)
            {
                velo = GetMaxVelocity();
                Debug.Log("StartGameStop");
            }
            //   Debug.Log("StartGameEND1" + velo);
            _yPosition = UpdateMaxY();
            Debug.Log("StartGameYPOSI" + _yPosition);

        }
    }
    private float UpdateMaxY()
    {
        float i = (_towerlist.Count > 0) ? _towerlist.Max(a => Mathf.Abs(a.transform.position.y)) : float.MinValue;

        if (_ypositionCash != _yPosition)
        {
            _ypositionCash = _yPosition;
            Debug.Log("StartGame" + _yPosition+"List");
        }

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
