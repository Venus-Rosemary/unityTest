using UnityEngine;
using System;
using System.Collections.Generic;

public class EventManager : Singleton<EventManager>
{
    private Dictionary<GameEventType, Action> eventDictionary = new Dictionary<GameEventType, Action>();
    private Dictionary<GameEventType, Action<object>> eventDictionaryWithParam = new Dictionary<GameEventType, Action<object>>();

    #region 无参数事件
    // 添加无参数事件监听
    public void AddListener(GameEventType eventType, Action listener)
    {
        if (eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] += listener;
        }
        else
        {
            eventDictionary.Add(eventType, listener);
        }
    }

    // 移除无参数事件监听
    public void RemoveListener(GameEventType eventType, Action listener)
    {
        if (eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] -= listener;
            if (eventDictionary[eventType] == null)
            {
                eventDictionary.Remove(eventType);
            }
        }
    }

    // 触发无参数事件
    public void TriggerEvent(GameEventType eventType)
    {
        if (eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType]?.Invoke();
        }
    }
    #endregion

    #region 带参数事件
    // 添加带参数事件监听
    public void AddListener<T>(GameEventType eventType, Action<T> listener)
    {
        if (eventDictionaryWithParam.ContainsKey(eventType))
        {
            eventDictionaryWithParam[eventType] += (obj) => listener((T)obj);
        }
        else
        {
            eventDictionaryWithParam.Add(eventType, (obj) => listener((T)obj));
        }
    }

    // 移除带参数事件监听
    public void RemoveListener<T>(GameEventType eventType, Action<T> listener)
    {
        if (eventDictionaryWithParam.ContainsKey(eventType))
        {
            eventDictionaryWithParam[eventType] -= (obj) => listener((T)obj);
            if (eventDictionaryWithParam[eventType] == null)
            {
                eventDictionaryWithParam.Remove(eventType);
            }
        }
    }

    // 触发带参数事件
    public void TriggerEvent<T>(GameEventType eventType, T parameter)
    {
        if (eventDictionaryWithParam.ContainsKey(eventType))
        {
            eventDictionaryWithParam[eventType]?.Invoke(parameter);
        }
    }
    #endregion

    // 清除所有事件
    public void ClearAllEvents()
    {
        eventDictionary.Clear();
        eventDictionaryWithParam.Clear();
    }

    #region 向后兼容的字符串方法
    // 以下方法保留用于向后兼容，但建议使用枚举版本
    public void AddListener(string eventName, Action listener)
    {
        AddListener((GameEventType)Enum.Parse(typeof(GameEventType), eventName), listener);
    }

    public void RemoveListener(string eventName, Action listener)
    {
        RemoveListener((GameEventType)Enum.Parse(typeof(GameEventType), eventName), listener);
    }

    public void TriggerEvent(string eventName)
    {
        TriggerEvent((GameEventType)Enum.Parse(typeof(GameEventType), eventName));
    }

    public void AddListener<T>(string eventName, Action<T> listener)
    {
        AddListener((GameEventType)Enum.Parse(typeof(GameEventType), eventName), listener);
    }

    public void RemoveListener<T>(string eventName, Action<T> listener)
    {
        RemoveListener((GameEventType)Enum.Parse(typeof(GameEventType), eventName), listener);
    }

    public void TriggerEvent<T>(string eventName, T parameter)
    {
        TriggerEvent((GameEventType)Enum.Parse(typeof(GameEventType), eventName), parameter);
    }
    #endregion
} 