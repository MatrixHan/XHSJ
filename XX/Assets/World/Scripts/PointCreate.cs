using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ص㴴��
/// </summary>
public class PointCreate : MonoBehaviour {
    public WorldCreate world;

    /// <summary>
    /// ���ִ�
    /// </summary>
    [Tooltip("���ִ�")]
    public PointGameObject newHand;

    /// <summary>
    /// �޴�
    /// </summary>
    [Tooltip("�޴�")]
    public PointGameObject foolish;

    /// <summary>
    /// ����
    /// </summary>
    [Tooltip("����")]
    public PointGameObject[] pope;

    /// <summary>
    /// ���� ÿ7�����еĵ�һ���Ǵ����
    /// </summary>
    [Tooltip("����")]
    public PointGameObject[] city;

    /// <summary>
    /// �ż� -- ͻ��6+����4+ʯ��4+ҩ԰4
    /// </summary>
    [Tooltip("�ż� -- ��������")]
    public PointGameObject[] vestige;

    /// <summary>
    /// Ԫ������
    /// </summary>
    [Tooltip("Ԫ������")]
    public PointGameObject[] element;

    /// <summary>
    /// ������
    /// </summary>
    [Tooltip("������")]
    public PointGameObject[] transmit;

    /// <summary>
    /// ·��
    /// </summary>
    [Tooltip("·��")]
    public PointGameObject[] sign;

    int seed;
    private void Awake() {
        seed = (int)System.DateTime.Now.Ticks;
    }

    public void Init() {
        Create();
    }

    public void Create() {
        seed = (seed + 1) % short.MaxValue;
        Random.InitState(seed);
        string pope_path = Tools.SavePath("pope.data");
        string city_path = Tools.SavePath("city.data");
        string vestige_path = Tools.SavePath("vestige.data");
        string element_path = Tools.SavePath("element.data");
        string sign_path = Tools.SavePath("sign.data");

        if (Tools.FileExists(pope_path)) {
            byte[] byt = Tools.ReadAllBytes(pope_path);
            this.pope = Tools.DeserializeObject(byt) as PointGameObject[];
        } else {
            this.pope = new PointGameObject[0];
        }
        if (this.pope == null) {
            this.pope = new PointGameObject[0];
        }

        if (Tools.FileExists(city_path)) {
            byte[] byt = Tools.ReadAllBytes(city_path);
            this.city = Tools.DeserializeObject(byt) as PointGameObject[];
        } else {
            this.city = new PointGameObject[0];
        }
        if (this.city == null) {
            this.city = new PointGameObject[0];
        }

        if (Tools.FileExists(vestige_path)) {
            byte[] byt = Tools.ReadAllBytes(vestige_path);
            this.vestige = Tools.DeserializeObject(byt) as PointGameObject[];
        } else {
            this.vestige = new PointGameObject[0];
        }
        if (this.vestige == null) {
            this.vestige = new PointGameObject[0];
        }

        if (Tools.FileExists(element_path)) {
            byte[] byt = Tools.ReadAllBytes(element_path);
            this.element = Tools.DeserializeObject(byt) as PointGameObject[];
        } else {
            this.element = new PointGameObject[0];
        }
        if (this.element == null) {
            this.element = new PointGameObject[0];
        }

        if (Tools.FileExists(sign_path)) {
            byte[] byt = Tools.ReadAllBytes(sign_path);
            this.sign = Tools.DeserializeObject(byt) as PointGameObject[];
        } else {
            this.sign = new PointGameObject[0];
        }
        if (this.sign == null) {
            this.sign = new PointGameObject[0];
        }

        List<PointGameObject> pope = new List<PointGameObject>(this.pope);
        List<PointGameObject> city = new List<PointGameObject>(this.city);
        List<PointGameObject> vestige = new List<PointGameObject>(this.vestige);
        List<PointGameObject> element = new List<PointGameObject>(this.element);
        List<PointGameObject> sign = new List<PointGameObject>(this.sign);

        bool mod_pope = false;
        bool mod_city = false;
        bool mod_vestige = false;
        bool mod_element = false;
        bool mod_sign = false;

        int idx = 0;
        while (true) {
            idx++;
            if (idx > 100000) {
                Debug.LogErrorFormat("more 100000! {0}-{1}-{2}-{3}", pope.Count, city.Count, vestige.Count, element.Count);
                Invoke("Create", 1);
                return;
            }
            int x = Random.Range(0, world.size);
            int z = Random.Range(0, world.size);
            if (!CheckPos(x, z)) {
                continue;
            }
            WorldUnit unit = world.get_units(x, z);
            if (TryPope(x, z, unit, pope)) {
                mod_pope = true;
                continue;
            }
            if (TryCity(x, z, unit, city)) {
                mod_city = true;
                continue;
            }
            if (TryVestige(x, z, unit, vestige)) {
                mod_vestige = true;
                continue;
            }
            if (TryElement(x, z, unit, element)) {
                mod_element = true;
                continue;
            }
            if (TrySign(x, z, unit, sign)) {
                mod_sign = true;
                continue;
            }
            break;
        }

        if (mod_pope) {
            byte[] byt = Tools.SerializeObject(this.pope);
            Tools.WriteAllBytes(pope_path, byt);
        }
        if (mod_city) {
            byte[] byt = Tools.SerializeObject(this.city);
            Tools.WriteAllBytes(city_path, byt);
        }
        if (mod_vestige) {
            byte[] byt = Tools.SerializeObject(this.vestige);
            Tools.WriteAllBytes(vestige_path, byt);
        }
        if (mod_element) {
            byte[] byt = Tools.SerializeObject(this.element);
            Tools.WriteAllBytes(element_path, byt);
        }
        if (mod_sign) {
            byte[] byt = Tools.SerializeObject(this.sign);
            Tools.WriteAllBytes(sign_path, byt);
        }
    }

    bool TryPope(int x,int z, WorldUnit unit, List<PointGameObject>  pope)
    {
        int max = 12; // �������
        int min_dis = 10; // ÿ��֮����С����
        if (pope.Count >= max) {
            return false;
        }

        // �Ƿ���Խ���
        bool can = true;
        if (can) {
            can = (unit & WorldUnit.City) == WorldUnit.City;
        }
        if (can) {
            if (pope.Count < 6) {
                can = (unit & WorldUnit.Level1) == WorldUnit.Level1;
            } else {
                can = (unit & WorldUnit.Level2) == WorldUnit.Level2;
            }
        }

        if (can) {
            // �ڳ������� ������1������
            Vector3 position = new Vector3(x, 0, z);
            can = TryPos(position, min_dis);
            if (can) {
                pope.Add(new PointGameObject() { position = position,name = RandName.GetRandPopeName() });
                this.pope = pope.ToArray();
            }
        }
        return true;
    }

    bool TryCity(int x, int z, WorldUnit unit, List<PointGameObject> city) {
        int max = 14; // �������
        int min_dis = 8; // ÿ��֮����С����
        if (city.Count >= max) {
            return false;
        }

        // �Ƿ���Խ���
        bool can = true;
        if (can) {
            can = (unit & WorldUnit.City) == WorldUnit.City;
        }
        if (can) {
            if (city.Count < 7) {
                can = (unit & WorldUnit.Level1) == WorldUnit.Level1;
            } else {
                can = (unit & WorldUnit.Level2) == WorldUnit.Level2;
            }
        }

        if (can) {
            // �ڳ������� ������1������
            Vector3 position = new Vector3(x, 0, z);
            can = TryPos(position, min_dis);
            if (can) {
                city.Add(new PointGameObject() { position = position, name = RandName.GetRandCityName() + (city.Count % 7 == 0 ? "��" : "��") });
                this.city = city.ToArray();
            }
        }
        return true;
    }

    bool TryVestige(int x, int z, WorldUnit unit, List<PointGameObject> vestige) {
        // �ż� -- ͻ��6+����4+ʯ��4+ҩ԰4
        int max = 36; // �������
        int min_dis = 5; // ÿ��֮����С����
        if (vestige.Count >= max) {
            return false;
        }
        string name = "";
        // �Ƿ���Խ���
        bool can = true;

        int idx = vestige.Count % 18;
        if (idx < 10) {
            name = "�Ϲ��ż�";
        } else if (idx < 14) {
            name = "ʯ��";
        } else if (idx < 18) {
            name = "ҩ԰";
        }

        if (vestige.Count < 18) {
            if (vestige.Count < 5) {
                can = (unit & WorldUnit.Level1) == WorldUnit.Level1 && (unit & WorldUnit.City) == WorldUnit.City;
            } else if (vestige.Count < 6) {
                can = (unit & WorldUnit.Level1) == WorldUnit.Level1 && (unit & WorldUnit.Monster) == WorldUnit.Monster;
            } else{
                can = (unit & WorldUnit.Level1) == WorldUnit.Level1;
            }
            if (vestige.Count < 3) {
                name = "ͨ���ؾ�";
            } else if (vestige.Count < 6) {
                name = "�����ؾ�";
            } 
        } else {
            if (vestige.Count < 21) {
                name = "����������";
                can = (unit & WorldUnit.Level2) == WorldUnit.Level2 && (unit & WorldUnit.City) == WorldUnit.City;
            } else if (vestige.Count < 24) {
                name = "���Ʒ�ħ��";
                can = (unit & WorldUnit.Level2) == WorldUnit.Level2 && (unit & WorldUnit.Monster) == WorldUnit.Monster;
            } else {
                can = (unit & WorldUnit.Level2) == WorldUnit.Level2;
            }
        }

        if (can) {
            // �ڳ������� ������1������
            Vector3 position = new Vector3(x, 0, z);
            can = TryPos(position, min_dis);
            if (can) {
                vestige.Add(new PointGameObject() { position = position ,name  = name });
                this.vestige = vestige.ToArray();
            }
        }
        return true;
    }

    bool TryElement(int x, int z, WorldUnit unit, List<PointGameObject> element) {
        int max = 12; // �������
        int min_dis = 12; // ÿ��֮����С����
        if (element.Count >= max) {
            return false;
        }

        // �Ƿ���Խ���
        bool can = true;
        if (can) {
            can = (unit & WorldUnit.City) == WorldUnit.City;
        }
        if (can) {
            if (element.Count < 6) {
                can = (unit & WorldUnit.Level1) == WorldUnit.Level1;
            } else {
                can = (unit & WorldUnit.Level2) == WorldUnit.Level2;
            }
        }

        int idx = element.Count % 6;
        string name = "";
        switch (idx) {
            case 0:
                name = "����֮��";
                break;
            case 1:
                name = "����֮��";
                break;
            case 2:
                name = "�׷�֮��";
                break;
            case 3:
                name = "����֮��";
                break;
            case 4:
                name = "��ɳ֮��";
                break;
            case 5:
                name = "��֮��";
                break;
        }

        if (can) {
            // �ڳ������� ������1������
            Vector3 position = new Vector3(x, 0, z);
            can = TryPos(position, min_dis);
            if (can) {
                element.Add(new PointGameObject() { position = position, name =  name});
                this.element = element.ToArray();
            }
        }
        return true;
    }

    bool TrySign(int x, int z, WorldUnit unit, List<PointGameObject> sign) {
        int max = 6; // �������
        int min_dis = 6; // ÿ��֮����С����
        if (sign.Count >= max) {
            return false;
        }

        // �Ƿ���Խ���
        bool can = true;
        if (can) {
            can = (unit & WorldUnit.City) == WorldUnit.City;
        }
        if (can) {
            if (sign.Count < 3) {
                can = (unit & WorldUnit.Level1) == WorldUnit.Level1;
            } else {
                can = (unit & WorldUnit.Level2) == WorldUnit.Level2;
            }
        }

        if (can) {
            // �ڳ������� ������1������
            Vector3 position = new Vector3(x, 0, z);
            can = TryPos(position, min_dis);
            if (can) {
                sign.Add(new PointGameObject() { position = position, name = "·��" });
                this.sign = sign.ToArray();
            }
        }
        return true;
    }


    //����Ƿ�ɽ�������
    bool CheckPos(int x, int z) {
        for (int i = -4; i < 5; i++) {
            for (int j = -4; j < 5; j++) {
                int a = x + i;
                int b = z + j;
                if (a < 0 || a >= world.size || b < 0 || b >= world.size)
                    return false;
                WorldUnit unit = world.get_units(a, b);
                if ((unit & WorldUnit.Mountain) == WorldUnit.Mountain)
                    return false;
                if ((unit & WorldUnit.NewVillage) == WorldUnit.NewVillage)
                    return false;
                if ((unit & WorldUnit.Impede) == WorldUnit.Impede)
                    return false;
            }
        }
        return true;
    }
    bool TryPos(Vector3 position, int min_dis) {
        if (position.x < min_dis || position.x > (world.size - min_dis)) {
            return false;
        }
        if (position.z < min_dis || position.z > (world.size - min_dis)) {
            return false;
        }
        bool can = true;
        foreach (var item in pope) {
            if (Vector3.Distance(item.position, position) < min_dis) {
                can = false;
                break;
            }
        }
        foreach (var item in city) {
            if (Vector3.Distance(item.position, position) < min_dis) {
                can = false;
                break;
            }
        }
        foreach (var item in vestige) {
            if (Vector3.Distance(item.position, position) < min_dis) {
                can = false;
                break;
            }
        }
        foreach (var item in element) {
            if (Vector3.Distance(item.position, position) < min_dis) {
                can = false;
                break;
            }
        }
        foreach (var item in sign) {
            if (Vector3.Distance(item.position, position) < min_dis) {
                can = false;
                break;
            }
        }
        return can;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (!Application.isPlaying) {
            Gizmos.DrawIcon(newHand.position * world.scale + new Vector3(0, 3, 0), "city.png", true, new Color(1, 1, 1, 1));
            Gizmos.DrawIcon(foolish.position * world.scale + new Vector3(0, 3, 0), "city.png", true, new Color(1, 1, 1, 1));
            for (int i = 0; i < pope.Length; i++) {
                Gizmos.DrawIcon(pope[i].position * world.scale + new Vector3(0, 3, 0), "pope.png", true, new Color(1, 1, 1, 1));
            }
            for (int i = 0; i < city.Length; i++) {
                string icon = "city.png";
                if (i % 7 == 0) {
                    icon = "bigcity.png";
                }
                Gizmos.DrawIcon(city[i].position * world.scale + new Vector3(0, 3, 0), icon, true, new Color(1, 1, 1, 1));
            }
            for (int i = 0; i < vestige.Length; i++) {
                Gizmos.DrawIcon(vestige[i].position * world.scale + new Vector3(0, 3, 0), "vestige.png", true, new Color(1, 1, 1, 1));
            }
            for (int i = 0; i < element.Length; i++) {
                string icon = "fire.png";
                switch (i % 6) {
                    case 1:
                        icon = "soil.png";
                        break;
                    case 2:
                        icon = "thunder.png";
                        break;
                    case 3:
                        icon = "water.png";
                        break;
                    case 4:
                        icon = "wind.png";
                        break;
                    case 5:
                        icon = "wood.png";
                        break;
                }
                Gizmos.DrawIcon(element[i].position * world.scale + new Vector3(0, 3, 0), icon, true, new Color(1, 1, 1, 1));
            }
            for (int i = 0; i < transmit.Length; i++) {
                Gizmos.DrawIcon(transmit[i].position * world.scale + new Vector3(0, 3, 0), "transmit.png", true, new Color(1, 1, 1, 1));
            }
            for (int i = 0; i < sign.Length; i++) {
                Gizmos.DrawIcon(sign[i].position * world.scale + new Vector3(0, 3, 0), "sign.png", true, new Color(1, 1, 1, 1));
            }
        }
    }
#endif
}

