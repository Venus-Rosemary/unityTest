using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateModel : Singleton<RotateModel>
{
    [Header("游戏是否开始")]
    private bool isCanGameStart;

    [Header("InputSystem")]
    private CommonControls controls;

    [Header("要旋转的物体")]
    public Transform modelTransform;

    [Header("旋转参数")]
    [Tooltip("旋转灵敏度：鼠标移动多少像素对应360度旋转")]
    public float rotationSensitivity = 5.0f;
    [Tooltip("摩擦力系数 (0 - 1)")]
    [Range(0.90f, 0.999f)]
    public float friction = 0.98f;
    [Tooltip("最小角速度，用于调整旋转停止手感")]
    public float minAngularVelocity = 1.0f;


    [Header("是否处于旋转状态")]
    [SerializeField]
    private bool isRotate;

    [Header("鼠标是否按下(用于调试查看)")]
    [SerializeField]
    private bool isHold;
    [Header("当前角速度(用于调试查看)")]
    [SerializeField]
    private float currentAngularVelocity;

    [Header("记录鼠标按下的起点")]
    private Vector3 inputStartPoint;

    [Header("记录旋转圈数")]
    public float rotationNumber;
    private float totalAccumulatedAngle = 0f; //累积的总角度

    [Header("剩余旋转次数")]
    public int remainingAttempts;

    private float lastInputDelta;
    private const float MaxAngularVelocity = 21600f;
    private const float DragToVelocityFactor = 0.1f;


    void Awake()
    {
        controls = new CommonControls();
        if (modelTransform == null)
            modelTransform = transform.GetChild(0).gameObject.transform;
    }

    private void Start()
    {
        EventManager.Instance.AddListener(GameEventType.GameStart, Initialization);
        Initialization();
    }

    void Update()
    {
        if (!isCanGameStart) return;
        UseInputRoate();
    }


    private void OnEnable()
    {
        //启用并注册Tap事件,鼠标按下和抬起
        controls.UI.Enable();
        controls.UI.Tap.performed += OnTapStart;
        controls.UI.Tap.canceled += OnTapEnd;
    }

    private void OnDisable()
    {
        //取消注册并禁用
        controls.UI.Tap.performed -= OnTapStart;
        controls.UI.Tap.canceled -= OnTapEnd;
        controls.UI.Disable();
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(GameEventType.GameStart, Initialization);
    }

    #region 事件内容
    //鼠标或手指按下触发事件
    private void OnTapStart(InputAction.CallbackContext context)
    {
        //inputStartPoint = Mouse.current.position.ReadValue();
        inputStartPoint = controls.UI.Position.ReadValue<Vector2>();

        lastInputDelta = 0;

    }
    //鼠标或手指抬起触发事件
    private void OnTapEnd(InputAction.CallbackContext context)
    {
        //（这段可以不要）
        //float throwVelocity = currentAngularVelocity +(lastInputDelta / rotationSensitivity) * 360 * 0.1f;
        //currentAngularVelocity = Mathf.Clamp(throwVelocity, -21600f, 21600f);

        if (isCanGameStart)
        {
            if (remainingAttempts <= 0)
            {
                remainingAttempts = 0;
            }
            else
            {
                remainingAttempts -= 1;
            }
        }
    }
    #endregion

    //旋转方法
    private void UseInputRoate()
    {
        //检测是否按下
        isHold = controls.UI.Tap.ReadValue<float>() > 0;

        if (isHold && remainingAttempts!=0)
        {
            //获取当前鼠标屏幕位置
            Vector2 currentInputPos = controls.UI.Position.ReadValue<Vector2>();
            //水平移动差值
            float deltaInput = currentInputPos.x - inputStartPoint.x;

            if (deltaInput >= 10 || deltaInput <= -10)
            {
                //更新数据
                inputStartPoint = controls.UI.Position.ReadValue<Vector2>();

                lastInputDelta = deltaInput;

                //在update中是自传的速度
                float targetAngle = currentAngularVelocity + (deltaInput / rotationSensitivity) * 360 * DragToVelocityFactor;

                //平滑旋转
                float currentAngle = modelTransform.eulerAngles.z;
                float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime / 0.2f);
                //应用旋转
                modelTransform.rotation = Quaternion.Euler(0, 0, newAngle);

                //限制
                currentAngularVelocity = Mathf.Clamp(targetAngle, -MaxAngularVelocity, MaxAngularVelocity);
            }
            else
            {
                lastInputDelta = 0;
            }
        }
        SpeedReduction();
    }

    //降速
    private void SpeedReduction()
    {
        //如果当前有速度
        if (Mathf.Abs(currentAngularVelocity) > minAngularVelocity)
        {
            isRotate = true;

            //摩擦力
            currentAngularVelocity *= friction;

            float deltaRotation = currentAngularVelocity * Time.deltaTime;
            totalAccumulatedAngle += Mathf.Abs(deltaRotation);
            rotationNumber = totalAccumulatedAngle / 360f;
            modelTransform.Rotate(0, 0, deltaRotation);
        }
        else
        {
            isRotate = false;
            currentAngularVelocity = 0;
        }
    }

    //初始化
    public void Initialization()
    {
        rotationNumber = 0;
        totalAccumulatedAngle = 0f;
        remainingAttempts = 5;
    }

    public float GetCurrentAngularVelocity() => currentAngularVelocity;
    public bool GetIsRotate() => isRotate;
    public void SetIsCanGameStart(bool isStart)
    {
        isCanGameStart = isStart;
    }
}
