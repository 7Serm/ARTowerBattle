using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class GameControl : MonoBehaviour
{
    [SerializeField] GameObject _stage;
    [SerializeField] List <GameObject> _towerobj;
    [SerializeField] Camera _smartcamera;
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] GameObject _uiCanvas;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _audioClip;
    [SerializeField] GameObject _restartButton;

    private FallJugment fallJugment;
    private List<GameObject> _towerlist = new();
    private GameObject _stagecash;
    private GameObject _canvascash;
    private GameObject _towerobjcash;
    private int _score = 0;
    bool _stageset = false;
    Rigidbody _rigidbody;
    private TextMeshProUGUI _textUI;
    private void Start()
    {
        StartCoroutine(StageSet());
    }




    IEnumerator StageSet()
    {
        while (!_stageset)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];
                if (touch.phase == TouchPhase.Began)
                {
                    var ray = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out RaycastHit _hit))
                    {
                        Vector3 _pvector = new(_hit.transform.position.x, _hit.transform.position.y + 0.2f, _hit.transform.position.z);
                        _stagecash = Instantiate(_stage, _pvector, Quaternion.identity);
                        Vector3 _pUIvrctor = new(_stagecash.transform.position.x + -0.3f, _stagecash.transform.position.y + 1.2f, _stagecash.transform.position.z);
                        _canvascash = Instantiate(_uiCanvas, _pUIvrctor,Quaternion.identity);                     
                    }
                }
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector3 _Vec3StagePosition = _smartcamera.transform.TransformDirection(new Vector3(touch.deltaPosition.x / 1000, 0, 0));
                    _stagecash.transform.localPosition += _Vec3StagePosition;
                    _canvascash.transform.localPosition += _Vec3StagePosition;
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    _stageset = true;
                    yield return null;
                    var obj = GameObject.Find("Score");
                    _textUI = obj.GetComponent<TextMeshProUGUI>();
                }
               
            }
            yield return null;
        }
        planeManager.requestedDetectionMode = PlaneDetectionMode.None;
        Transform _childtrans = _stagecash.transform.GetChild(1);
        fallJugment = _childtrans.GetComponent<FallJugment>();
        StartCoroutine(GameMain());
        yield return null;
    }



    IEnumerator GameMain()
    {
        float _ypositon = 0f;
        bool _spawnready = true;
        bool _stopobj = true;
        bool _tapoff = false;
        Quaternion[] quaternions = new Quaternion[7];
        quaternions[0] = Quaternion.identity;
        quaternions[1] = Quaternion.Euler(0, 0, 90);
        quaternions[2] = Quaternion.Euler(0, 0, 120);
        quaternions[3] = Quaternion.Euler(0, 0, 210);

        
        while (!fallJugment.Falljudg)
        {
            Vector3 _stagelocalPosition = _stagecash.transform.localPosition;
            Vector3 _newobjposition = new(_stagelocalPosition.x, _stagelocalPosition.y + _ypositon + 0.4f, _stagelocalPosition.z);
            if (_spawnready)
            {
                int i = Random.Range(0, 4);
                int f = Random.Range(0, 4);
                _towerobjcash = Instantiate(_towerobj[i], _newobjposition, quaternions[f], _stagecash.transform);
                _audioSource.PlayOneShot(_audioClip);
                _spawnready = false;
                _stopobj = false;
                _tapoff = false;
                yield return null;
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];
                if (touch.phase == TouchPhase.Began)
                {
                    _rigidbody = _towerobjcash.GetComponent<Rigidbody>();
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    Vector3 _vec3MovePosition = _smartcamera.transform.TransformDirection(new Vector3(touch.deltaPosition.x / 1000, 0, 0));
                    _towerobjcash.transform.localPosition += _vec3MovePosition;
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    _rigidbody.useGravity = true;
                    _spawnready = true;
                    _towerlist.Add(_towerobjcash);
                    _towerobjcash = null;
                    _tapoff = true;
                    _score++;
                    while (_rigidbody.velocity.magnitude < 0.00001)
                    {
                        yield return null;
                    }
                    yield return null;
                }
            }

            var objvelo = GetMaxVelocity();
            while (objvelo > 0.00001)
            {
                objvelo = GetMaxVelocity();
                // Debug.Log("StartGame" + objvelo);
                _stopobj = true;
                yield return null;
            }

            if (fallJugment.Falljudg) break;

            var yposicash = UpdateMaxY();
            if (_stopobj && _tapoff)
            {
                _ypositon = yposicash;
            }

            _textUI.text = _score.ToString();
            yield return null;
        }

        StartCoroutine(Result());
        yield return null;
    }


    IEnumerator Result()
    {
        _restartButton.SetActive(true);
        yield return null;
    }
    private float UpdateMaxY()
    {
        float i = (_towerlist.Count > 0) ? _towerlist.Max(a => Mathf.Abs(a.transform.localPosition.y)) : float.MinValue;
        // Debug.Log("StartGame" + i + "List" + _towerlist.Count);
        return i;
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
