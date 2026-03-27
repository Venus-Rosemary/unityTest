using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("按钮管理")]
    public Button startButton;
    public Button restartButton;
    public Button quitButton;

    [Header("界面管理")]
    public GameObject startB;
    public GameObject endF;

    [Header("圈数设置")]
    public TMP_Text turnsNumberText;
    public Slider turnsSlider;

    [Header("转速设置")]
    public TMP_Text rotateSpeedText;

    [Header("剩余次数")]
    public TMP_Text remainingAttemptsText;

    public TMP_Text testT;

    void Start()
    {
        Initialization();
    }

    void Update()
    {
        //测试用
        //Vector2 currentPos = RotateModel.Instance.controls.UI.Position.ReadValue<Vector2>();
        //testT.text = $"位置：{currentPos},startP:{RotateModel.Instance.inputStartPoint},bool:{RotateModel.Instance.isHold}";

        if (RotateModel.Instance.GetCurrentAngularVelocity() >= 6 || RotateModel.Instance.GetCurrentAngularVelocity() <= -6)
        {
            rotateSpeedText.text = 
                $"{Mathf.Abs(Mathf.FloorToInt(RotateModel.Instance.GetCurrentAngularVelocity() / 360f * 60f))}/min";
        }
        else
        {
            rotateSpeedText.text =
                $"0/min";
        }

        turnsNumberText.text = $"{Mathf.FloorToInt(RotateModel.Instance.rotationNumber)}圈";
        turnsSlider.value = (RotateModel.Instance.rotationNumber % 100F)/100F;

        remainingAttemptsText.text = $"{RotateModel.Instance.remainingAttempts}次";

        CheckEnd();
    }

    //初始化
    public void Initialization()
    {
        OnRestart();
        rotateSpeedText.text = $"0/min";
    }

    //结束检测
    private void CheckEnd()
    {
        if (RotateModel.Instance.remainingAttempts == 0 
            && RotateModel.Instance.GetCurrentAngularVelocity() == 0)
        {
            endF.gameObject.SetActive(true);
            RotateModel.Instance.SetIsCanGameStart(false);
        }
    }

    //按钮事件注册
    private void OnEnable()
    {
        startButton.onClick.AddListener(OnGameStart);
        restartButton.onClick.AddListener(OnRestart);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveAllListeners();
        restartButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }

    //开始按钮上的事件
    private void OnGameStart()
    {
        StartCoroutine(StartEffect());
    }
    IEnumerator StartEffect()
    {
        startButton.interactable = false;
        yield return new WaitForSeconds(0.5f);
        startButton.interactable = true;
        startB.gameObject.SetActive(false);

        RotateModel.Instance.SetIsCanGameStart(true);
        EventManager.Instance.TriggerEvent(GameEventType.GameStart);
    }

    //重新开始的按钮事件
    private void OnRestart()
    {
        EventManager.Instance.TriggerEvent(GameEventType.GameStart);
        startB.gameObject.SetActive(true);
        endF.gameObject.SetActive(false);
    }

    //退出
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
