using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    //�萔�ݒ�
    float swipeSpeed = 0.1f;        //�X���C�v���x
    float pinchSpeed = 1f;          //�s���`���x

    //���s���擾
    GameObject objPanel;            //2D�p�l��
    Vector2 clickPosition;          //����N���b�N�ʒu
    Vector2 prevClickPos0;          //�O��N���b�N�ʒu
    Vector2 touchEndPos0;           //����^�b�`
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
        //�V���O���^�b�`
        SingleTouch();
        //�_�u���^�b�`
        DoubleTouch();
    }

    //�V���O���^�b�`
    void SingleTouch()
    {
        //�^�b�`�J�n
        if (Input.GetMouseButtonDown(0))
        {
            //�^�b�`���W���擾
            clickPosition = Input.mousePosition;

            //2D����
            objTouch = GetTouchedCanvasObject(clickPosition);
            if (objTouch != null)
            {
                //Canvas�ɑ΂��鏈��
                Debug.Log(objTouch.name);
            }

            //3D����(2D�p�l������A�N�e�B�u�̂Ƃ��ɓ���)
            if (!objPanel.activeSelf)
            {
                string[] layers = { "Default", "Water" };
                objTouch = GetTouchedObject(clickPosition, layers, QueryTriggerInteraction.Ignore);
                if (objTouch != null)
                {
                    //3D�I�u�W�F�N�g���g��������
                    Debug.Log(objTouch.name);
                }
            }

            prevClickPos0 = clickPosition;
        }
        //�^�b�`�p��
        else if (Input.GetMouseButton(0))
        {
            //�^�b�`���W���擾
            clickPosition = Input.mousePosition;

            //3D����(2D�p�l������A�N�e�B�u�̂Ƃ��ɓ���)
            if (!objPanel.activeSelf)
            {
                string[] layers = { "Default" };
                objTouch = GetTouchedObject(clickPosition, layers, QueryTriggerInteraction.Ignore);
                if (objTouch != null)
                {
                    //3D�I�u�W�F�N�g���g��������
                    Debug.Log(objTouch.name);
                }
            }

            prevClickPos0 = clickPosition;
        }

        //�^�b�`�I��
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("�^�b�`�I��");
        }
    }

    //�_�u���^�b�`
    void DoubleTouch()
    {
        if (Input.touchCount == 2)
        {
            //3D����(2D�p�l������A�N�e�B�u�̂Ƃ��ɓ���)
            if (!objPanel.activeSelf)
            {
                //�^�b�`�����擾
                touch0 = Input.GetTouch(0);
                touch1 = Input.GetTouch(1);

                //�^�b�`�J�n
                if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
                {
                    previousTouchPos0 = touch0.position;
                    previousTouchPos1 = touch1.position;
                }

                //�^�b�`�p��
                if (touch0.phase == TouchPhase.Moved && touch1.phase == TouchPhase.Moved)
                {
                    //�^�b�`�ʒu���擾
                    touchEndPos0 = touch0.position;
                    touchEndPos1 = touch1.position;

                    //�O��^�b�`�ʒu�Ƃ̍������擾
                    Vector2 delta0 = touchEndPos0 - previousTouchPos0;
                    Vector2 delta1 = touchEndPos1 - previousTouchPos1;

                    //�J�����ʒu���擾
                    Vector3 cameraPosition = mainCamera.transform.position;

                    //�X���C�v����̍��v�x�N�g�����v�Z
                    Vector2 swipeDelta = (delta0 + delta1) * 0.5f;

                    //�X���C�v����̑��x���擾
                    swipeDelta *= swipeSpeed * Time.deltaTime;

                    //�X���C�v����ɂ��J�����ʒu���X�V(X��Z���W�ɂ��ď㉺������)
                    cameraPosition.x = Mathf.Clamp(cameraPosition.x - swipeDelta.x, 5f, 45f);
                    cameraPosition.z = Mathf.Clamp(cameraPosition.z - swipeDelta.y, -1f, 45f);
                    mainCamera.transform.position = cameraPosition;

                    //�s���`����
                    float prevMagnitude = (previousTouchPos0 - previousTouchPos1).magnitude;
                    float currentMagnitude = (touchEndPos0 - touchEndPos1).magnitude;
                    float difference = currentMagnitude - prevMagnitude;

                    //�s���`����̑��x���擾
                    difference *= pinchSpeed * Time.deltaTime;

                    //�s���`����ɂ��J�����ʒu���X�V(Y���W�ɂ��ď㉺������)
                    cameraPosition = mainCamera.transform.position;
                    cameraPosition.y = Mathf.Clamp(cameraPosition.y - difference, cameraYMinPosition, cameraYMaxPosition);
                    mainCamera.transform.position = cameraPosition;

                    previousTouchPos0 = touchEndPos0;
                    previousTouchPos1 = touchEndPos1;
                }
            }
        }
    }

    //�^�b�`����3D�I�u�W�F�N�g�����o
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

    //�^�b�`����2D�I�u�W�F�N�g�����o
    GameObject GetTouchedCanvasObject(Vector3 touchPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touchPosition;

        // List<RaycastResult>��p��
        List<RaycastResult> results = new List<RaycastResult>();

        // Raycast���g�p����Canvas���UI�v�f�����o
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count > 0)
        {
            // �őO�ʂ�UI�v�f���擾
            return results[0].gameObject;
        }

        return null;
    }
}