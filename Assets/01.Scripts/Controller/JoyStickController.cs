using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum JoyStickMethod
{
    DoNotUse,
    Fixed,
    HardFixed,
    Follow,
    SlowFollow,
    RunningGame,
}
public class JoyStickController : MonoBehaviour
{
    [Header("조이스틱 방식")]
    [Tooltip("Fixed : 누른 위치에 고정\nHardFixed : 이미 고정 되어있음\nFollow : 원 밖으로 나가면 원이 따라옴\nSlowFollow : 원 밖으로 나가면 원이 천천히 따라옴\nRunningGame : 런 방식처럼 좌우 오프셋만 인식")]
    public JoyStickMethod joyStickMethod;
    [Header("조이스틱이 돌아다닐 수 있는 반경")]
    public float JoyStickBound;
    [Header("움직일 물체 (Rigidbody필요)")]
    public Rigidbody MoveObjectRig;

    public float Threshold = 10;


    public float Speed = 5;
    private float _curSpeed;
    public float XBound;
    public bool AutoRun;
    public float X_Sensitivity;
    public float X_Acceletor;

    public bool UseAccelerate;
    public float Accelerate = 100;


    private RectTransform _canvasRect;
    private RectTransform _joystick;
    private RectTransform _joystickHandle;
    [SerializeField]
    private Image _joystickImage;
    private Image _joystickHandleImage;


    private Vector3 _oriPos;
    private Vector3 newDir = Vector3.zero;
    private float _oriXPos;
    private int _walkableMask;

    [HideInInspector]
    public bool CanMove = false;
    private bool isButtonClick = false;
    private bool isMouseDown = false;

    public System.Action DownAction;
    public System.Action<Vector2> JoystickMoveAction;
    public System.Action UpAction;
    public void Awake()
    {
        _oriXPos = 0;
        _curSpeed = 0;
        _walkableMask = LayerMask.NameToLayer("Walkable");
        _canvasRect = GetComponent<RectTransform>();
        _joystick = _joystickImage.GetComponent<RectTransform>();
        _joystickHandle = _joystick.GetChild(0).GetComponent<RectTransform>();
        _joystickHandleImage = _joystickHandle.GetComponent<Image>();
        _joystickImage.enabled = false;
        _joystickHandleImage.enabled = false;

        AddDownEvent(() => isMouseDown = true);
        AddUpEvent(() => isMouseDown = false);

        switch (joyStickMethod)
        {
            case JoyStickMethod.DoNotUse:
                _joystick.gameObject.SetActive(false);
                break;
            case JoyStickMethod.Fixed:
                break;
            case JoyStickMethod.HardFixed:
                _joystickImage.enabled = true;
                _joystickHandleImage.enabled = true;
                break;
            case JoyStickMethod.Follow:
                break;
            case JoyStickMethod.SlowFollow:
                break;
            case JoyStickMethod.RunningGame:
                _joystickImage.enabled = false;
                _joystickHandleImage.enabled = false;
                break;
        }
        CanMove = true;
        isButtonClick = false;

        //Managers.Game.JoyStickController = this;
    }
    //JoyStickEditor 에서 활용함
    public void SetRigidBody()
    {
        MoveObjectRig.isKinematic = false;
        MoveObjectRig.mass = 100;
        MoveObjectRig.drag = 100;
        MoveObjectRig.angularDrag = 100;
        MoveObjectRig.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

    private void Update()
    {
        if (!CanMove) return;

        if (UseAccelerate)
        {
            if (!isMouseDown)
            {
                _curSpeed = Mathf.Max(0, _curSpeed - Accelerate * Time.deltaTime);
                CheckMoveDir();
            }
            MoveObjectRig.position += newDir * Time.deltaTime * _curSpeed;
        }


        switch (joyStickMethod)
        {
            case JoyStickMethod.DoNotUse:
                return;
            case JoyStickMethod.Fixed:
                if (Input.GetMouseButtonDown(0))
                {
                    // if (CheckButtonClick()) return;

                    _joystickImage.enabled = true;
                    _joystickHandleImage.enabled = true;

                    _joystick.anchoredPosition = Input.mousePosition * 2688f / Screen.height;
                    _joystickHandle.anchoredPosition = Vector2.zero;
                    _oriPos = _joystick.anchoredPosition;

                    DownAction?.Invoke();
                }
                else if (Input.GetMouseButton(0) && !isButtonClick)
                {
                    _joystickHandle.anchoredPosition = Input.mousePosition * 2688f / Screen.height - _oriPos;
                    if (_joystickHandle.anchoredPosition.magnitude > JoyStickBound)
                    {
                        _joystickHandle.anchoredPosition = _joystickHandle.anchoredPosition.normalized * JoyStickBound;
                    }

                    if (_joystickHandle.anchoredPosition.magnitude < Threshold) return;

                    JoystickMoveAction?.Invoke(_joystickHandle.anchoredPosition);

                    Vector3 dir = new Vector3(_joystickHandle.anchoredPosition.x, 0, _joystickHandle.anchoredPosition.y);

                    if (MoveObjectRig != null)
                    {
                        Move(dir);
                    }
                }
                else if (Input.GetMouseButtonUp(0) && !isButtonClick)
                {
                    _joystickImage.enabled = false;
                    _joystickHandleImage.enabled = false;
                    UpAction?.Invoke();
                }
                break;
            case JoyStickMethod.HardFixed:
                if (Input.GetMouseButtonDown(0))
                {
                    if (CheckButtonClick()) return;

                    DownAction?.Invoke();
                }
                else if (Input.GetMouseButton(0) && !isButtonClick)
                {
                    _joystickHandle.anchoredPosition = (Vector2)(Input.mousePosition * 2688f / Screen.height) - _joystick.anchoredPosition;
                    if (_joystickHandle.anchoredPosition.magnitude > JoyStickBound)
                    {
                        _joystickHandle.anchoredPosition = _joystickHandle.anchoredPosition.normalized * JoyStickBound;
                    }
                    if (_joystickHandle.anchoredPosition.magnitude < Threshold) return;

                    JoystickMoveAction?.Invoke(_joystickHandle.anchoredPosition);

                    Vector3 dir = new Vector3(_joystickHandle.anchoredPosition.x, 0, _joystickHandle.anchoredPosition.y);
                    if (MoveObjectRig != null)
                    {
                        Move(dir);
                    }
                }
                else if (Input.GetMouseButtonUp(0) && !isButtonClick)
                {
                    _joystickHandle.anchoredPosition = Vector2.zero;

                    UpAction?.Invoke();
                }
                break;
            case JoyStickMethod.Follow:
                if (Input.GetMouseButtonDown(0))
                {
                    if (CheckButtonClick()) return;

                    _joystickImage.enabled = true;
                    _joystickHandleImage.enabled = true;

                    _joystick.anchoredPosition = Input.mousePosition * 2688f / Screen.height;
                    _joystickHandle.anchoredPosition = Vector2.zero;
                    _oriPos = _joystick.anchoredPosition;

                    DownAction?.Invoke();
                }
                else if (Input.GetMouseButton(0) && !isButtonClick)
                {
                    _joystickHandle.anchoredPosition = Input.mousePosition * 2688f / Screen.height - _oriPos;
                    if (_joystickHandle.anchoredPosition.magnitude > JoyStickBound)
                    {
                        _joystick.anchoredPosition = (Vector2)_oriPos + _joystickHandle.anchoredPosition - JoyStickBound * _joystickHandle.anchoredPosition.normalized;
                        _joystickHandle.anchoredPosition = _joystickHandle.anchoredPosition.normalized * JoyStickBound;
                        _oriPos = _joystick.anchoredPosition;
                    }
                    if (_joystickHandle.anchoredPosition.magnitude < Threshold) return;

                    JoystickMoveAction?.Invoke(_joystickHandle.anchoredPosition);

                    Vector3 dir = new Vector3(_joystickHandle.anchoredPosition.x, 0, _joystickHandle.anchoredPosition.y);
                    if (MoveObjectRig != null)
                    {
                        Move(dir);
                    }
                }
                else if (Input.GetMouseButtonUp(0) && !isButtonClick)
                {
                    _joystickImage.enabled = false;
                    _joystickHandleImage.enabled = false;
                    UpAction?.Invoke();
                }
                break;
            case JoyStickMethod.SlowFollow:
                if (Input.GetMouseButtonDown(0))
                {
                    if (CheckButtonClick()) return;

                    _joystickImage.enabled = true;
                    _joystickHandleImage.enabled = true;

                    _joystick.anchoredPosition = Input.mousePosition * 2688f / Screen.height;
                    _joystickHandle.anchoredPosition = Vector2.zero;
                    _oriPos = _joystick.anchoredPosition;

                    DownAction?.Invoke();
                }
                else if (Input.GetMouseButton(0) && !isButtonClick)
                {
                    _joystickHandle.anchoredPosition = Input.mousePosition * 2688f / Screen.height - _oriPos;
                    if (_joystickHandle.anchoredPosition.magnitude > JoyStickBound)
                    {
                        _joystick.anchoredPosition = Vector3.Lerp(_joystick.anchoredPosition, (Vector2)_oriPos + _joystickHandle.anchoredPosition - JoyStickBound * _joystickHandle.anchoredPosition.normalized, Time.deltaTime);
                        _oriPos = _joystick.anchoredPosition;
                    }
                    if (_joystickHandle.anchoredPosition.magnitude < Threshold) return;

                    JoystickMoveAction?.Invoke(_joystickHandle.anchoredPosition);

                    Vector3 dir = new Vector3(_joystickHandle.anchoredPosition.x, 0, _joystickHandle.anchoredPosition.y);
                    if (MoveObjectRig != null)
                    {
                        Move(dir);
                    }
                }
                else if (Input.GetMouseButtonUp(0) && !isButtonClick)
                {
                    _joystickImage.enabled = false;
                    _joystickHandleImage.enabled = false;
                    UpAction?.Invoke();
                }
                break;
            case JoyStickMethod.RunningGame:
                if (MoveObjectRig == null) return;

                if (Input.GetMouseButtonDown(0))
                {
                    // if (CheckButtonClick()) return;

                    _oriXPos = Input.mousePosition.x;
                    _oriPos = MoveObjectRig.position;
                    DownAction?.Invoke();
                }
                else if (Input.GetMouseButton(0) && !isButtonClick)
                {
                    float _curXPos = Input.mousePosition.x;

                    if (_oriXPos == 0)
                        _oriXPos = Input.mousePosition.x;
                    float nextXPos = Mathf.Clamp(_oriPos.x + (_curXPos - _oriXPos) / _canvasRect.rect.width * X_Sensitivity, -XBound, XBound);

                    Vector3 nextPos = new Vector3(nextXPos, MoveObjectRig.position.y, MoveObjectRig.position.z);

                    MoveObjectRig.position = Vector3.Lerp(MoveObjectRig.position, nextPos, Time.deltaTime * X_Acceletor);

                    if (!AutoRun)
                    {
                        MoveObjectRig.MovePosition(MoveObjectRig.position + Vector3.forward * Speed * Time.deltaTime);
                    }
                    JoystickMoveAction?.Invoke(_joystickHandle.anchoredPosition);
                }
                else if (Input.GetMouseButtonUp(0) && !isButtonClick)
                {
                    _joystickImage.enabled = false;
                    _joystickHandleImage.enabled = false;
                    UpAction?.Invoke();
                }
                if (AutoRun)
                {
                    MoveObjectRig.MovePosition(MoveObjectRig.position + Vector3.forward * Speed * Time.deltaTime);
                }
                break;
        }
    }


    public void AddDownEvent(System.Action action)
    {
        DownAction -= action;
        DownAction += action;
    }
    public void AddMoveEvent(System.Action<Vector2> action)
    {
        JoystickMoveAction -= action;
        JoystickMoveAction += action;
    }
    public void AddUpEvent(System.Action action)
    {
        UpAction -= action;
        UpAction += action;
    }

    private bool CheckButtonClick()
    {
        //old
        // if (EventSystem.current?.currentSelectedGameObject?.GetComponent<Button>())
        // {
        //     isButtonClick = true;
        //     return true;
        // }
        // else
        // {
        //     isButtonClick = false;
        //     return false;
        // }

        isButtonClick = EventSystem.current.IsPointerOverGameObject();

        return isButtonClick;
    }

    private void Move(Vector3 dir)
    {
        newDir = dir.normalized;

        MoveObjectRig.rotation = Quaternion.LookRotation(newDir);

        CheckMoveDir();

        if (UseAccelerate)
        {
            _curSpeed = Mathf.Min(Speed, _curSpeed + Time.deltaTime * Accelerate);
        }
        else
        {
            MoveObjectRig.position += newDir * Time.deltaTime * Speed;
        }
    }
    private void CheckMoveDir()
    {
        bool CanForwardGo = Physics.Raycast(MoveObjectRig.position + Vector3.forward * Define.BOUND + Vector3.up * 50, Vector3.down, 100, 1 << _walkableMask);
        bool CanBackGo = Physics.Raycast(MoveObjectRig.position + Vector3.back * Define.BOUND + Vector3.up * 50, Vector3.down, 100, 1 << _walkableMask);
        bool CanRightGo = Physics.Raycast(MoveObjectRig.position + Vector3.right * Define.BOUND + Vector3.up * 50, Vector3.down, 100, 1 << _walkableMask);
        bool CanLeftGo = Physics.Raycast(MoveObjectRig.position + Vector3.left * Define.BOUND + Vector3.up * 50, Vector3.down, 100, 1 << _walkableMask);

        if (!CanForwardGo && newDir.z >= 0) newDir.z = 0;
        if (!CanBackGo && newDir.z < 0) newDir.z = 0;
        if (!CanRightGo && newDir.x >= 0) newDir.x = 0;
        if (!CanLeftGo && newDir.x < 0) newDir.x = 0;
    }
}
