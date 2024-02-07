using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveCam : MonoBehaviour
{
    public float _moveSpeed = 5f;


    public float _mouseSense = 0.05f;


    //public Vector3 _camOffset;
    public float _startY, _endY;
    public Vector3 _startPos;

    public Vector2 _limitZ = new Vector2();


    public bool isClick = false;

    void Start()
    {
        //_camOffset = transform.position;
    }


    void Update()
    {
#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())// 
            {
                isClick = true;
                _startY = Input.mousePosition.y;
                _endY = Input.mousePosition.y;
                _startPos = transform.position;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (isClick)
            {
                _endY = Input.mousePosition.y;
                transform.position = _startPos + new Vector3(0f, 0f, (_startY - _endY) * _mouseSense);
                if (transform.position.z > _limitZ.x)
                {
                    transform.position = new Vector3(0f, transform.position.y, _limitZ.x);
                }
                else if (transform.position.z < _limitZ.y)
                {
                    transform.position = new Vector3(0f, transform.position.y, _limitZ.y);
                }

            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isClick = false;
        }


        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += new Vector3(0f, 0f, _moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            transform.position -= new Vector3(0f, 0f, _moveSpeed * Time.deltaTime);
        }


        //#if UNITY_EDITOR
#elif !UNITY_EDITOR
  if (Input.touchCount > 0)
        {
            Touch _touch = Input.GetTouch(0);
            if (_touch.phase == TouchPhase.Began)
            {
                if (!EventSystem.current.IsPointerOverGameObject(_touch.fingerId))// 
                {
                   isClick = true;
                _startY = Input.mousePosition.y;
                _endY = Input.mousePosition.y;
                _startPos = transform.position;
                }
            }
            else if (_touch.phase == TouchPhase.Moved)
            {
                if (isClick)
                {
                     _endY = Input.mousePosition.y;
                transform.position = _startPos + new Vector3(0f, 0f, (_startY - _endY) * _mouseSense);
                if (transform.position.z > _limitZ.x)
                {
                    transform.position = new Vector3(0f, transform.position.y, _limitZ.x);
                }
                else if (transform.position.z < _limitZ.y)
                {
                    transform.position = new Vector3(0f, transform.position.y, _limitZ.y);
                }

                }
            }
            else if (_touch.phase == TouchPhase.Ended)
            {
                 isClick = false;
            }
        }


#endif
    }
}
