using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    //定数設定
    float swipeSpeed = 0.1f;        //スワイプ速度
    float pinchSpeed = 1f;          //ピンチ速度

    //実行時取得
    GameObject objPanel;            //2Dパネル
    Vector2 clickPosition;          //今回クリック位置
    Vector2 prevClickPos0;          //前回クリック位置
    Vector2 touchEndPos0;           //初回タッチ
    Vector2 touchEndPos1;
    Vector2 previousTouchPos0;
    Vector2 previousTouchPos1;
    Camera mainCamera;
    float cameraYMinPosition = 5f;
    float cameraYMaxPosition = 30f;

    GameObject objTouch;
    Touch touch0;
    Touch touch1;

    void Awake()
    {
        objPanel = GameObject.Find("Canvas").transform.Find("Panel").gameObject;
        mainCamera = Camera.main;
    }

    void Start()
    {

    }

    void Update()
    {
        //シングルタッチ
        SingleTouch();
        //ダブルタッチ
        DoubleTouch();
    }

    //シングルタッチ
    void SingleTouch()
    {
        //タッチ開始
        if (Input.GetMouseButtonDown(0))
        {
            //タッチ座標を取得
            clickPosition = Input.mousePosition;

            //2D処理
            objTouch = GetTouchedCanvasObject(clickPosition);
            if (objTouch != null)
            {
                //Canvasに対する処理
                Debug.Log(objTouch.name);
            }

            //3D処理(2Dパネルが非アクティブのときに動作)
            if (!objPanel.activeSelf)
            {
                string[] layers = { "Default", "Water" };
                objTouch = GetTouchedObject(clickPosition, layers, QueryTriggerInteraction.Ignore);
                if (objTouch != null)
                {
                    //3Dオブジェクトを使った処理
                    Debug.Log(objTouch.name);
                }
            }

            prevClickPos0 = clickPosition;
        }
        //タッチ継続
        else if (Input.GetMouseButton(0))
        {
            //タッチ座標を取得
            clickPosition = Input.mousePosition;

            //3D処理(2Dパネルが非アクティブのときに動作)
            if (!objPanel.activeSelf)
            {
                string[] layers = { "Default" };
                objTouch = GetTouchedObject(clickPosition, layers, QueryTriggerInteraction.Ignore);
                if (objTouch != null)
                {
                    //3Dオブジェクトを使った処理
                    Debug.Log(objTouch.name);
                }
            }

            prevClickPos0 = clickPosition;
        }

        //タッチ終了
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("タッチ終了");
        }
    }

    //ダブルタッチ
    void DoubleTouch()
    {
        if (Input.touchCount == 2)
        {
            //3D処理(2Dパネルが非アクティブのときに動作)
            if (!objPanel.activeSelf)
            {
                //タッチ情報を取得
                touch0 = Input.GetTouch(0);
                touch1 = Input.GetTouch(1);

                //タッチ開始
                if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
                {
                    previousTouchPos0 = touch0.position;
                    previousTouchPos1 = touch1.position;
                }

                //タッチ継続
                if (touch0.phase == TouchPhase.Moved && touch1.phase == TouchPhase.Moved)
                {
                    //タッチ位置を取得
                    touchEndPos0 = touch0.position;
                    touchEndPos1 = touch1.position;

                    //前回タッチ位置との差分を取得
                    Vector2 delta0 = touchEndPos0 - previousTouchPos0;
                    Vector2 delta1 = touchEndPos1 - previousTouchPos1;

                    //カメラ位置を取得
                    Vector3 cameraPosition = mainCamera.transform.position;

                    //スワイプ操作の合計ベクトルを計算
                    Vector2 swipeDelta = (delta0 + delta1) * 0.5f;

                    //スワイプ操作の速度を取得
                    swipeDelta *= swipeSpeed * Time.deltaTime;

                    //スワイプ操作によるカメラ位置を更新(XとZ座標について上下限処理)
                    cameraPosition.x = Mathf.Clamp(cameraPosition.x - swipeDelta.x, 5f, 45f);
                    cameraPosition.z = Mathf.Clamp(cameraPosition.z - swipeDelta.y, -1f, 45f);
                    mainCamera.transform.position = cameraPosition;

                    //ピンチ操作
                    float prevMagnitude = (previousTouchPos0 - previousTouchPos1).magnitude;
                    float currentMagnitude = (touchEndPos0 - touchEndPos1).magnitude;
                    float difference = currentMagnitude - prevMagnitude;

                    //ピンチ操作の速度を取得
                    difference *= pinchSpeed * Time.deltaTime;

                    //ピンチ操作によるカメラ位置を更新(Y座標について上下限処理)
                    cameraPosition = mainCamera.transform.position;
                    cameraPosition.y = Mathf.Clamp(cameraPosition.y - difference, cameraYMinPosition, cameraYMaxPosition);
                    mainCamera.transform.position = cameraPosition;

                    previousTouchPos0 = touchEndPos0;
                    previousTouchPos1 = touchEndPos1;
                }
            }
        }
    }

    //タッチして3Dオブジェクトを検出
    GameObject GetTouchedObject(Vector3 touchPosition, string[] layerNames, QueryTriggerInteraction trigger)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        LayerMask layerMask = LayerMask.GetMask(layerNames);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, trigger))
        {
            return hit.collider.gameObject;
        }

        return null;
    }

    //タッチして2Dオブジェクトを検出
    GameObject GetTouchedCanvasObject(Vector3 touchPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touchPosition;

        // List<RaycastResult>を用意
        List<RaycastResult> results = new List<RaycastResult>();

        // Raycastを使用してCanvas上のUI要素を検出
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count > 0)
        {
            // 最前面のUI要素を取得
            return results[0].gameObject;
        }

        return null;
    }
}