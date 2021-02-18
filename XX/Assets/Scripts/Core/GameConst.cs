using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GameConst {
    public static int max_color = 6;
    public static Dictionary<ItemType, string> itemTypeName = new Dictionary<ItemType, string>() 
    {
        { ItemType.Other, "����"},
        { ItemType.Remedy, "��ҩ"},
        { ItemType.Gongfa, "�ؼ�"},
        { ItemType.Equip, "װ��"},
        { ItemType.Material, "����"},
        { ItemType.Toy, "���"},
    };

    public static Dictionary<ItemSubType, string> itemSubTypeName = new Dictionary<ItemSubType, string>()
    {
        { ItemSubType.None, ""},
        { ItemSubType.Heart, "�ķ�"},
        { ItemSubType.Body, "��"},
        { ItemSubType.Attack, "�似"},
        { ItemSubType.Magic, "�鼼"},
        { ItemSubType.Skill, "����"},
        { ItemSubType.Witchcraft, "��ͨ"},
        { ItemSubType.Ring, "��ָ"},
        { ItemSubType.Ride, "����"},
        { ItemSubType.recoverRemedy, "�ָ���ҩ"},
        { ItemSubType.buffRemedy, "���浤ҩ"},
        { ItemSubType.aptitudesRemedy, "���ʵ�ҩ"},
        { ItemSubType.otherRemedy, "������ҩ"},
    };

    public static UIShortcutKey[] uIShortcutKeys = new UIShortcutKey[]{
        new UIShortcutKey(){
            type = "uiwindow",
            param1 = "RoleWindow",
            param2 = "role",
            keyCode = KeyCode.I
        },
        new UIShortcutKey(){
            type = "uiwindow",
            param1 = "RoleWindow",
            param2 = "skill",
            keyCode = KeyCode.X
        },
        new UIShortcutKey(){
            type = "uiwindow",
            param1 = "RoleWindow",
            param2 = "artistry",
            keyCode = KeyCode.O
        },
        new UIShortcutKey(){
            type = "uiwindow",
            param1 = "RoleWindow",
            param2 = "bag",
            keyCode = KeyCode.B
        },
        new UIShortcutKey(){
            type = "occupy",
            keyCode = KeyCode.Escape
        },
    };
}
