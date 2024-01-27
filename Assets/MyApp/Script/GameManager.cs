using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
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
�@�@private int  _returnY = 0;
    private int _cashY = 0;

    private Rigidbody  _rigitbody;

    private GameObject  _gameObj;

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

    /// <summary>
    /// �Q�[���̃��C�����W�b�N����
    /// </summary>
    private void GamePlay()
    {
        Vector3 _stagePosition = _stageCash.transform.position;
        Vector3 _newSpawPosition = new (_stagePosition.x, _stagePosition.y + _spawnY+_returnY, _stagePosition.z);

        bool _stopObj = true;
        bool _spawnready = true;
        if (Input.touchCount > 0 && _gameMain == true)
        {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began)
            {
                var ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out _hit))
                {
                    _gameObj = Instantiate(_towerObj, _newSpawPosition, Quaternion.identity);
                    _rigitbody = _gameObj.GetComponent<Rigidbody>();

                    Debug.Log("StartGame");
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 _vec3MovePosition = _smartCamera.transform.TransformDirection(new Vector3(touch.deltaPosition.x / 1000, 0, 0));
                _towerlist[_towerCount].transform.localPosition += _vec3MovePosition;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                _towerlist.Add(_gameObj);
                _rigitbody.useGravity = true;
                _towerCount++;
                _stopObj = false;

                while(_rigitbody.velocity.magnitude < 0.001F){}
            }

            var _objVelo = GetMaxVelocity();
            while (_objVelo > 0.001F)
            {
                _objVelo = GetMaxVelocity();
               
            }

            _returnY = MaxY();
            _stopObj = true;
            _spawnready = true;
        }
    }

    /// <summary>
    ///�Q�[�����n�߂�ۂɃX�e�[�W�̐ݒu���s���֐�
    /// </summary>
    private void StageSet()
    {
        if (Input.touchCount > 0 && _setUp == true)
        {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began)
            {
                var ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out _hit))
                {
                    _stageCash = Instantiate(_stage, _hit.transform.position, _hit.transform.rotation);
                }
                Debug.Log("Start");
                _gameMain = true;
                _setUp = false;
            }
        }
    }

    /// <summary>
    /// ���X�g���ɂ���I�u�W�F�N�g�̃x���V�e�B��Ԃ��֐�
    /// ������g���Č��݂̏�Ԃ̊Ď����s���i�^���[�������Ă���̂��A�~�܂��Ă���̂��j
    /// </summary>
    /// <returns></returns>
    private float GetMaxVelocity()
    {
        float magnitude = 0f;

        foreach (GameObject go in _towerlist)
        {
            var r = go.GetComponent<Rigidbody>();
            var m = r.velocity.magnitude;
            if (magnitude < m)
            {
                magnitude = m;
            }
        }
        return magnitude;
    }

    /// <summary>
    /// ���X�g�̒��ň��Y����ԍ������̂�T�����̒l��Ԃ�
    /// </summary>
    /// <returns></returns>
    private int MaxY()
    {       
        // �~�܂����� Y ���W����ԏ�̃Q�[���I�u�W�F�N�g��T������
        var _maxY = (_towerlist.Count > 0) ?_towerlist.Max(a => Mathf.Abs(a.transform.position.y)) : float.MinValue;
       int  i = Mathf.CeilToInt(_maxY);

        return i;
    }
}
