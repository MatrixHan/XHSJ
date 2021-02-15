using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ������������
/// </summary>
public static class MessageData {
    public static string[] dataList;
    static MessageData() {
        dataList = Tools.ReadAllText("Config/MessageData.txt").Split('\n');
    }

    public static string GetMessage(int id) {
        return dataList[id - 1];
    }
}