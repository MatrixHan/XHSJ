using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class SettingWindow : BaseWindow {
    public Dropdown dropdown;
    public Toggle windowType;
    public Toggle mouseType;
    public GameObject[] pages;
    private void Awake() {
        var v = SettingData.instance;
    }

    bool initok = false;
    private void Start() {
        UpdateUI();
    }

    private void OnEnable() {
        if (!initok)
            return;

        string sub_show = MainUI.instance.sub_show == "" ? "system" : MainUI.instance.sub_show;
        pages[0].SetActive(sub_show == "system");
        pages[1].SetActive(sub_show == "key");

        UpdateUI();
    }

    private void UpdateUI() {
        initok = false;
        int idx = GetResolutionOptions();
        dropdown.value = idx;
        windowType.isOn = !SettingData.instance.fullScreen;
        mouseType.isOn = Cursor.lockState == CursorLockMode.Confined;
        initok = true;
    }

    public void SetResolution() {
        if (!initok)
            return;

        int width = ResolutionList[dropdown.value].x;
        int height = ResolutionList[dropdown.value].y;
        var r = Screen.currentResolution;
        if (width != r.width || height != r.height) {
            Screen.SetResolution(width, height, SettingData.instance.fullScreen);

        }

        MessageWindow.CheckMessage(51, 80, () => {
            SettingData.instance.resolutionl_width = width;
            SettingData.instance.resolutionl_height = height;
        }, () => {
            Screen.SetResolution(SettingData.instance.resolutionl_width, SettingData.instance.resolutionl_height, SettingData.instance.fullScreen);
            initok = false;
            int idx = GetResolutionOptions();
            dropdown.value = idx;
            initok = true;
        }, 0, 10);

    }

    Vector2Int[] ResolutionList;
    public void SetResolutionOptions() {
        GetResolutionOptions();
    }
    /// <summary>
    /// ���÷ֱ���ѡ��
    /// </summary>
    public int GetResolutionOptions() {
        initok = false;
        int idx = dropdown.value;
        List<Vector2Int> list = new List<Vector2Int>();
        List<OptionData> options = new List<OptionData>();
        Resolution[] resolutions = Screen.resolutions;
        Vector2Int cur_resolution = new Vector2Int(SettingData.instance.resolutionl_width, SettingData.instance.resolutionl_height);
        for (int i = resolutions.Length - 1; i >= 0; i--) {
            Resolution item = resolutions[i];
            if (item.width >= 1024 && item.height >= 768) {
                Vector2Int resolution = new Vector2Int(item.width, item.height);
                if (!list.Contains(resolution)) {
                    if (item.width == cur_resolution.x && item.height == cur_resolution.y) {
                        idx = list.Count;
                    }
                    options.Add(new OptionData(item.width + "��" + item.height));
                    list.Add(resolution);
                }
            }
        }

        if (!list.Contains(cur_resolution)) {
            idx = list.Count;
            options.Add(new OptionData(cur_resolution.x + "��" + cur_resolution.y));
            list.Add(cur_resolution);
        }

        ResolutionList = list.ToArray();
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.value = idx;
        initok = true;
        return idx;
    }

    public void SetWindowType() {
        if (!initok)
            return;
        SettingData.instance.fullScreen = !windowType.isOn;
        Screen.SetResolution(SettingData.instance.resolutionl_width, SettingData.instance.resolutionl_height, SettingData.instance.fullScreen);
    }

    public void SetMouseType() {
        if (!initok)
            return;
        if (mouseType.isOn) {
            SettingData.instance.cursorLockMode = CursorLockMode.Confined;
        } else {
            SettingData.instance.cursorLockMode = CursorLockMode.None;
        }
        Cursor.lockState = SettingData.instance.cursorLockMode;
    }

    public override void ClickClose() {
        SettingData.SaveSetting();
        base.ClickClose();
    }
}

[System.Serializable]
public class SettingData {
    static string data_path = "config/setting.data";
    public static SettingData _instance;
    public static SettingData instance
    {
        get
        {
            if (_instance == null) {
                Rest();
            }
            return _instance;
        }
    }

    public static void Rest() {
        bool has_data = Tools.FileExists(data_path);
        if (!has_data) {
            _instance = new SettingData();
        } else {
            byte[] byt = Tools.ReadAllBytes(data_path);
            SettingData setting_data = Tools.DeserializeObject<SettingData>(byt);
            _instance = setting_data;
        }
        if (_instance.resolutionl_width == 0 || _instance.resolutionl_height == 0) {
            Resolution resolutionl = Screen.resolutions[Screen.resolutions.Length - 1];
            _instance.resolutionl_width = resolutionl.width;
            _instance.resolutionl_height = resolutionl.height;
            _instance.fullScreen = true;
            _instance.cursorLockMode = CursorLockMode.None;
            SaveSetting();
        }
        Screen.SetResolution(_instance.resolutionl_width, _instance.resolutionl_height, _instance.fullScreen);
        Cursor.lockState = _instance.cursorLockMode;
    }

    public static void SaveSetting() {
        byte[] byt = Tools.SerializeObject(_instance);
        Tools.WriteAllBytes(data_path, byt);
    }


    public int resolutionl_width;
    public int resolutionl_height;
    public bool fullScreen;
    public CursorLockMode cursorLockMode;
    public float volume = 50;
    public float sound_effect = 50;
    public float music = 50;


    // �������ݼ�
    public SettingStruct[] worldShortcutKeys = new SettingStruct[]{
        // �� W
        new SettingStruct(){
            type = "move",
            param1 = "up",
            param2 = "",
            keyCode = KeyCode.W
        },
        // �� S
        new SettingStruct(){
            type = "move",
            param1 = "down",
            param2 = "",
            keyCode = KeyCode.S
        },
        // �� A
        new SettingStruct(){
            type = "move",
            param1 = "left",
            param2 = "",
            keyCode = KeyCode.A
        },
        // �� D
        new SettingStruct(){
            type = "move",
            param1 = "right",
            param2 = "",
            keyCode = KeyCode.D
        },

        // �������� I
        new SettingStruct(){
            type = "uiwindow",
            param1 = "RoleWindow",
            param2 = "role",
            keyCode = KeyCode.I
        },

        // ���� X
        new SettingStruct(){
            type = "uiwindow",
            param1 = "RoleWindow",
            param2 = "skill",
            keyCode = KeyCode.X
        },

        // ���� B
        new SettingStruct(){
            type = "uiwindow",
            param1 = "RoleWindow",
            param2 = "bag",
            keyCode = KeyCode.B
        },
        // ���� L
        new SettingStruct(){
            type = "uiwindow",
            param1 = "XXXXX",
            param2 = "",
            keyCode = KeyCode.L
        },
        // �ż� H
        new SettingStruct(){
            type = "uiwindow",
            param1 = "XXXXX",
            param2 = "",
            keyCode = KeyCode.H
        },
        // ���¼� N
        new SettingStruct(){
            type = "uiwindow",
            param1 = "XXXXX",
            param2 = "",
            keyCode = KeyCode.N
        },
        // С��ͼ M
        new SettingStruct(){
            type = "uiwindow",
            param1 = "XXXXX",
            param2 = "",
            keyCode = KeyCode.M
        },
        // ���� O
        new SettingStruct(){
            type = "uiwindow",
            param1 = "RoleWindow",
            param2 = "artistry",
            keyCode = KeyCode.O
        },
        // �������� Z
        new SettingStruct(){
            type = "uiwindow",
            param1 = "XXXXX",
            param2 = "",
            keyCode = KeyCode.Z
        },
        // ������� J
        new SettingStruct(){
            type = "uiwindow",
            param1 = "XXXXX",
            param2 = "",
            keyCode = KeyCode.J
        },
    };

    // ս����ݼ�
    public SettingStruct[] battleShortcutKeys = new SettingStruct[]{
        // �� W
        new SettingStruct(){
            type = "move",
            param1 = "up",
            param2 = "",
            keyCode = KeyCode.W
        },
        // �� S
        new SettingStruct(){
            type = "move",
            param1 = "down",
            param2 = "",
            keyCode = KeyCode.S
        },
        // �� A
        new SettingStruct(){
            type = "move",
            param1 = "left",
            param2 = "",
            keyCode = KeyCode.A
        },
        // �� D
        new SettingStruct(){
            type = "move",
            param1 = "right",
            param2 = "",
            keyCode = KeyCode.D
        },
        // �似/�鼼 ���
        new SettingStruct(){
            type = "battle",
            param1 = "attack",
            param2 = "",
            keyCode = KeyCode.Mouse0
        },
        // ���� �Ҽ�
        new SettingStruct(){
            type = "battle",
            param1 = "skill",
            param2 = "",
            keyCode = KeyCode.Mouse1
        },
        // �� �ո�
        new SettingStruct(){
            type = "battle",
            param1 = "body",
            param2 = "",
            keyCode = KeyCode.Space
        },
        // ��ͨ R
        new SettingStruct(){
            type = "battle",
            param1 = "magic",
            param2 = "",
            keyCode = KeyCode.R
        },
        // ����1 1
        new SettingStruct(){
            type = "item",
            param1 = "1",
            param2 = "",
            keyCode = KeyCode.Alpha1
        },
        // ����2 2
        new SettingStruct(){
            type = "item",
            param1 = "2",
            param2 = "",
            keyCode = KeyCode.Alpha2
        },
        // ����3 3
        new SettingStruct(){
            type = "item",
            param1 = "3",
            param2 = "",
            keyCode = KeyCode.Alpha3
        },
        // ����4 4
        new SettingStruct(){
            type = "item",
            param1 = "4",
            param2 = "",
            keyCode = KeyCode.Alpha4
        },
        // ����5 5
        new SettingStruct(){
            type = "item",
            param1 = "5",
            param2 = "",
            keyCode = KeyCode.Alpha5
        },
        // ���� E
        new SettingStruct(){
            type = "action",
            param1 = "talk",
            param2 = "",
            keyCode = KeyCode.Alpha6
        },
    };
}


[System.Serializable]
public struct SettingStruct {
    public string type;
    public string param1;
    public string param2;
    public KeyCode keyCode;

    
}