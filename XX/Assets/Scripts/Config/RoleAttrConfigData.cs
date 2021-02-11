using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ������������
/// </summary>
public static class RoleAttrConfigData {
    public static Dictionary<int, RoleAttrConfig[]> dataList;
    static RoleAttrConfigData() {
        var data = Tools.ReadAllText("Config/roleAttr.txt").Split('\n');
        var attrdes = Tools.ReadAllText("Config/roleAttrDes.txt").Split('\n');
        int level = 0;
        Dictionary<int, RoleAttrConfig[]> all = new Dictionary<int, RoleAttrConfig[]>();
        List<RoleAttrConfig> levelData = null;
        int id = 0;
        foreach (var item in data) {
            if (string.IsNullOrWhiteSpace(item) || item.StartsWith("#")) {
                continue;
            }
            var line = item.Trim();
            var tmp = line.Split(':');

            switch (tmp[0]) {
                case "level":
                    if (levelData != null) {
                        all.Add(level, levelData.ToArray());
                    }
                    level = int.Parse(tmp[1]);
                    id = 0;
                    levelData = new List<RoleAttrConfig>();
                    break;
                default:
                    tmp = line.Split(',');
                    int progressMin = 0, progressMax = 0, randMin = 0, randMax = 0;
                    string randattr = tmp[1];
                    RoleAttrShowType typ = (RoleAttrShowType)int.Parse(tmp[2]);
                    string[] randattrlist = randattr.Split('-');
                    if (randattrlist.Length < 1) {
                        continue;
                    } else if (randattrlist.Length == 1) {
                        randMin = int.Parse(randattrlist[0]);
                        randMax = randMin;
                    } else {
                        randMin = int.Parse(randattrlist[0]);
                        randMax = int.Parse(randattrlist[1]);
                    }
                    if (typ == RoleAttrShowType.Progress || typ == RoleAttrShowType.RateProgress) {
                        string progressattr = tmp[3];
                        string[] progressattrlist = progressattr.Split('-');

                        progressMin = int.Parse(progressattrlist[0]);
                        progressMax = int.Parse(progressattrlist[1]);
                    }
                    string des = null;
                    if (id < attrdes.Length) {
                        des = string.Join("\n\n����", attrdes[id].Split('|'));
                    }
                    if (typ == RoleAttrShowType.Text) {
                        levelData.Add(new RoleAttrConfig(tmp[0], id,randMin, randMax, tmp[3], des));
                    } else {
                        levelData.Add(new RoleAttrConfig() {
                            name = tmp[0], id = id, progressMin = progressMin, progressMax = progressMax
                        , randMin = randMin, randMax = randMax, type = typ, describe = des});
                    }
                    id++;
                    break;
            }
        }
        all.Add(level, levelData.ToArray());
        dataList = all;
    }

    public static RoleAttrConfig[] GetAttrConfig(int level = 1) {
        int get_level = level;
        RoleAttrConfig[] result = null;
        foreach (var item in dataList) {
            if (result == null || (get_level <= item.Key && item.Key <= level)) {
                result = item.Value;
                get_level = item.Key;
            }
        }
        return result;
    }

    public static void GetRandomAttr(out int[] attribute, out int[] max_attribute, out RoleAttrConfig[] attribute_config) {
        attribute_config = GetAttrConfig();
        attribute = new int[attribute_config.Length];
        max_attribute = new int[attribute_config.Length];
        for (int i = 0; i < attribute_config.Length; i++) {
            RoleAttrConfig config = attribute_config[i];
            attribute_config[i] = config;
            int max = Random.Range(config.randMin, config.randMax + 1);
            int min = max;
            if (config.type == RoleAttrShowType.FixedMinMax) {
                // ���������
                min = config.randMin;
                max = config.randMax;
            }
            attribute[i] = min;
            max_attribute[i] = max;
        }
    }
}

/// <summary>
/// ��ɫ��������
/// </summary>
public struct RoleAttrConfig {
    public string name;
    public int id;
    public int progressMin;
    public int progressMax;
    public int randMin;
    public int randMax;
    public string param;
    public RoleAttrShowType type;
    static int[] charm_value;
    static string[] gril_showcharm;
    static string[] boy_showcharm;
    public string describe;

    public RoleAttrConfig(string name, int id, int randMin, int randMax, string param, string describe) {
        type = RoleAttrShowType.Text;
        this.name = name;
        this.id = id;
        this.randMin = randMin;
        this.randMax = randMax;
        this.param = param;
        progressMin = randMin;
        progressMax = randMax;
        this.param = param;
        string[] tmp = param.Split(';');
        int count = tmp.Length;
        charm_value = new int[count];
        gril_showcharm = new string[count];
        boy_showcharm = new string[count];
        this.describe = describe;
        for (int i = 0; i < count; i++) {
            string[] values = tmp[i].Split('-');
            charm_value[i] = int.Parse(values[0]);
            boy_showcharm[i] = values[1];
            if (values.Length == 3) {
                gril_showcharm[i] = values[2];
            } else {
                gril_showcharm[i] = values[1];
            }
        }
    }

    //��ȡ��������
    public static string GetValue(int value, Sex sex) {
        string[] showcharm;
        if (sex == Sex.Girl) {
            showcharm = gril_showcharm;
        } else {
            showcharm = boy_showcharm;
        }
        string show_name = showcharm[0];
        for (int i = 0; i < charm_value.Length; i++) {
            if (value > charm_value[i]) {
                show_name = showcharm[i];
            } else {
                break;
            }
        }
        return show_name;
    }
}

public enum RoleAttrShowType {
    /// <summary>
    /// Ĭ����ʾ
    /// </summary>
    None,
    /// <summary>
    /// ��ʾ��Сֵ xx/xxx
    /// </summary>
    MinMax,
    /// ��ʾ����
    /// </summary>
    Progress,
    /// <summary>
    /// ��ʾ�ı� ����,60,3,[0-99-����;100-199-����;200-299-����;300-499-Ѱ��;500-599-����;600-699-��-����;700-799-����-����;800-899-����-��
    /// </summary>
    Text,
    /// <summary>
    /// ���ִ��ٷֺ� ��ʾ����
    /// </summary>
    RateProgress,
    /// <summary>
    /// �̶���ʾ��Сֵ xx/xxx
    /// </summary>
    FixedMinMax,
}

public enum Attribute {

    /// <summary>
    /// ����
    /// </summary>
    life,

    /// <summary>
    /// ����
    /// </summary>
    mood,

    /// <summary>
    /// ����
    /// </summary>
    health,

    /// <summary>
    /// ����
    /// </summary>
    energy,

    /// <summary>
    /// ����
    /// </summary>
    hp,

    /// <summary>
    /// ����
    /// </summary>
    mp,

    /// <summary>
    /// ����
    /// </summary>
    spirit,

    /// <summary>
    /// ����
    /// </summary>
    lucky,

    /// <summary>
    /// ����
    /// </summary>
    intellectuality,

    /// <summary>
    /// ����
    /// </summary>
    fame,

    /// <summary>
    /// ����
    /// </summary>
    charm,

    /// <summary>
    /// ����
    /// </summary>
    attack,

    /// <summary>
    /// ����
    /// </summary>
    defend,

    /// <summary>
    /// ����
    /// </summary>
    stamina,

    /// <summary>
    /// ��������
    /// </summary>
    gongfa_resistance,

    /// <summary>
    /// �������
    /// </summary>
    linggen_resistance,

    /// <summary>
    /// ���� ����
    /// </summary>
    crit,

    /// <summary>
    /// ���� ����
    /// </summary>
    protection,

    /// <summary>
    /// ����
    /// </summary>
    speed,

    /// <summary>
    /// ��������
    /// </summary>
    crit_multiple,

    /// <summary>
    /// ��������
    /// </summary>
    protection_multiple,

    /// <summary>
    /// ����
    /// </summary>
    gongfa_knife,

    /// <summary>
    /// ǹ��
    /// </summary>
    gongfa_spear,

    /// <summary>
    /// ����
    /// </summary>
    gongfa_sword,

    /// <summary>
    /// ȭ��
    /// </summary>
    gongfa_fist,

    /// <summary>
    /// �Ʒ�
    /// </summary>
    gongfa_palm,

    /// <summary>
    /// ָ��
    /// </summary>
    gongfa_finger,

    /// <summary>
    /// �����
    /// </summary>
    linggen_fire,

    /// <summary>
    /// ˮ���
    /// </summary>
    linggen_water,

    /// <summary>
    /// �����
    /// </summary>
    linggen_thunder,

    /// <summary>
    /// �����
    /// </summary>
    linggen_wind,

    /// <summary>
    /// �����
    /// </summary>
    linggen_soil,

    /// <summary>
    /// ľ���
    /// </summary>
    linggen_wood,

    /// <summary>
    /// ����
    /// </summary>
    alchemy,

    /// <summary>
    /// ����
    /// </summary>
    forge,

    /// <summary>
    /// ��ˮ
    /// </summary>
    geomantic,

    /// <summary>
    /// ����
    /// </summary>
    juju,

    /// <summary>
    /// ҩ��
    /// </summary>
    medicinal,

    /// <summary>
    /// ���
    /// </summary>
    minerals,
    /// <summary>
    /// ����
    /// </summary>
    zhengdao,

    /// <summary>
    /// ħ��
    /// </summary>
    modao,
}