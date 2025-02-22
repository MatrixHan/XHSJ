﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class MonoSingleton<T>:MonoBehaviour where T:MonoBehaviour {

    private static T _instance;
    public static T instance
    {
        get
        {
            if (null == _instance) {
                _instance = FindObjectOfType<T>();
                if (null == _instance) {
                    _instance = new GameObject().AddComponent<T>();
                    _instance.name = typeof(T).ToString();
                }
            }
            return _instance;
        }
    }
    private void Awake() {
        if (null == _instance) {
            _instance = this as T;
        } else {
            Destroy(this.gameObject);
        }
    }
}
