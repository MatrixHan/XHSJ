using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAssets : MonoBehaviour {
    public static UIAssets instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    public Sprite[] itemColor;
    public Sprite[] bgColor;
    public GameObject[] grilPrefab;
    public GameObject[] boyPrefab;
    public GameObject[] ridePrefab;

    // 0-5 �ķ� �似/�鼼 ���� �� ��ͨ
    // 6-21 ����
    // 22-25 ��ָ
    public Sprite[] itemIcon;
}
