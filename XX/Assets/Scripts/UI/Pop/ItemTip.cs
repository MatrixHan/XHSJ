﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemTip : MonoBehaviour {
    public Text item_name;
    public Text item_type;
    public Text item_sub_type;
    public Text item_need_lv;
    public Image item_bg;
    public Image item_color;
    public Image item_icon;
    public GameObject item_use;

    public void ShowTip(ItemData item) {
        gameObject.SetActive(true);
        UpdateItem(item);
    }

    private void UpdateItem(ItemData item) {
        ItemStaticData staticData = GameData.instance.item_static_data[item.static_id];

        item_name.text = staticData.name;
        item_type.text = GameConst.itemTypeName[staticData.type];
        item_sub_type.text = GameConst.itemSubTypeName[staticData.sub_ype];
        item_need_lv.text = LevelConfigData.GetBigName(staticData.level);

        string[] array = staticData.des.Split('\n');
        for (int i = 1; i < transform.childCount; i++) {
            if (i > array.Length) {
                transform.GetChild(i).gameObject.SetActive(false);
            } else {
                GameObject obj = transform.GetChild(i).gameObject;
                obj.SetActive(true);
                obj.GetComponent<Text>().text = array[i - 1];
            }
        }
        //item_attr.text = GetItemAttrDes(staticData);
        item_bg.sprite = UIAssets.instance.bgColor[staticData.color];
        item_color.sprite = UIAssets.instance.itemColor[staticData.color];
        item_icon.sprite = UIAssets.instance.itemIcon[staticData.icon];
        item_use.SetActive(RoleData.mainRole.ItemIsEquip(item.id));
    }
    /*
     * <color=#E28225FF>装备后可获得以下属性属性</color>
背包上限：<color=#20C123FF>+12</color>
     * 
     */
    private string GetItemAttrDes(ItemStaticData staticData) {
        if (staticData.attributes == null || staticData.attributes.Length < 1) {
            return null;
        }
        RoleAttrConfig[] attribute_config = RoleAttrConfigData.GetAttrConfig();
        StringBuilder myString = new StringBuilder("<color=#E28225FF>装备后可获得以下属性</color>");
        for (int i = 0; i < staticData.attributes.Length; i++) {
            myString.AppendLine();
            myString.Append(attribute_config[(int)staticData.attributes[i]].name);
            myString.AppendFormat("<color=#20C123FF>+{0}</color>", staticData.attr_values[i]);
        }
        return myString.ToString();
    }
}
