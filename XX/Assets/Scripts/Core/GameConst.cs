using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GameConst {
    public const long oneSecondTicks = 864000000000;
    public const long oneDayTicks = 864000000000;
    public const int max_color = 6;
    public const int max_role_level = 12;
    public const int max_item_level = max_role_level / 3;
    public static Dictionary<ItemType, string> itemTypeName = new Dictionary<ItemType, string>() 
    {
        { ItemType.Other, "����"},
        { ItemType.Remedy, "��ҩ"},
        { ItemType.Gongfa, "�ؼ�"},
        { ItemType.Equip, "װ��"},
        { ItemType.Material, "����"},
        { ItemType.Toy, "���"},
    };

    public static string[] item_color_str = new string[] { "E6D6BEFF", "3FBD8CFF", "397CB8FF", "A73BBDFF", "DEB731FF", "FC7425FF", "E73E47FF", };
    public static Color[] item_color = new Color[] {
        new Color(0xE6 / 255f, 0xD6 / 255f, 0xBE / 255f),
        new Color(0x3F / 255f, 0xBD / 255f, 0x8C / 255f),
        new Color(0x39 / 255f, 0x7C / 255f, 0xB8 / 255f),
        new Color(0xA7 / 255f, 0x3B / 255f, 0xBD / 255f),
        new Color(0xDE / 255f, 0xB7 / 255f, 0x31 / 255f),
        new Color(0xFC / 255f, 0x74 / 255f, 0x25 / 255f),
        new Color(0xE7 / 255f, 0x3E / 255f, 0x47 / 255f),
    };

    // ���ܵ��������������������������ѧ70%������80%��С��90%�����100%��Բ��110%������120%��
    public static string[] attr_level_name = new string[] { "զ��", "���", "ͨ��", "С��", "�鶯", "��ͨ", "���", "Բ��", "����", "����", "���",
    "һǧ", "��ǧ", "��ǧ", "��ǧ", "��ǧ", "��ǧ", "��ǧ", "��ǧ", "��ǧ", "һ��", "ʮ��", };

    public static Dictionary<ItemSubType, string> itemSubTypeName = new Dictionary<ItemSubType, string>()
    {
        { ItemSubType.None, ""},
        { ItemSubType.Heart, "�ķ�"},
        { ItemSubType.Body, "��"},
        { ItemSubType.Attack, "�似/�鼼"},
        { ItemSubType.Magic, "��ͨ"},
        { ItemSubType.Skill, "����"},
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
