using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DrawGameLevel : EditorWindow {

    enum DrawMode {
        /// <summary>
        /// �����
        /// </summary>
        Point,
        /// <summary>
        /// ���λ���
        /// </summary>
        Rect,
    }
    enum DrawType {
        /// <summary>
        /// ���
        /// </summary>
        Add,
        /// <summary>
        /// �Ƴ�
        /// </summary>
        Remove,
    }

    [MenuItem("Maps/Creater %M")]//�����ݼ�
    public static void OpenMapCreate() {
        DrawGameLevel window = EditorWindow.GetWindow<DrawGameLevel>("��ͼ�༭��");
        window.Show();
        window.minSize = new Vector2(400, 800);//����������С
        window.maxSize = new Vector2(400, 1200);
    }

    private void Awake() {
        defStyle = new GUIStyle();
        selStyle = new GUIStyle();
    }

    private void OnEnable() {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDestroy() {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private bool _drag = false;
    Vector3 downPoint;
    private void OnSceneGUI(SceneView sceneVie) {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
            //���
            OnMouseEvent(Event.current.type);
        } else if (Event.current.type == EventType.MouseUp && Event.current.button == 0) {
            //̧��
            if (!_drag) {
                OnMouseEvent(Event.current.type);
            }
            _drag = false;
        } else if (Event.current.type == EventType.MouseDrag && Event.current.button == 0) {
            //�϶�
            OnMouseEvent(Event.current.type);
            _drag = true;
        }
    }

    private void OnMouseEvent(EventType type) {
        if (world == null) {
            Debug.Log("û�е�ͼ����");
            return;
        }
        //��ȡ�������
        Vector2 mousePos = Event.current.mousePosition;
        //��������ԭ��������,����Ļ�ռ�ԭ������,��ת��
        mousePos.y = Camera.current.pixelHeight - mousePos.y;
        Ray ray = Camera.current.ScreenPointToRay(mousePos);
        RaycastHit rh;
        
        if (Physics.Raycast(ray, out rh)) {
            //�ж��Ƿ��䵽��plane,�ǵĻ����в�������
            //Debug.Log("�䵽 " + rh.point);

            if (drawMode == DrawMode.Rect) {
                switch (type) {
                    case EventType.MouseDown:
                        downPoint = rh.point;
                        break;
                    case EventType.MouseUp:

                        break;
                    case EventType.MouseDrag:
                        int start_x = (int)(Mathf.Min(downPoint.x, rh.point.x) / world.scale);
                        int start_z = (int)(Mathf.Min(downPoint.z, rh.point.z) / world.scale);

                        int end_x = (int)(Mathf.Max(downPoint.x, rh.point.x) / world.scale);
                        int end_z = (int)(Mathf.Max(downPoint.z, rh.point.z) / world.scale);

                        for (int x = start_x; x <= end_x; x++) {
                            for (int z = start_z; z <= end_z; z++) {
                                ModUnit(x, z);
                            }
                        }
                        break;
                }
            } else {
                switch (type) {
                    case EventType.MouseDrag:
                        int x = (int)(Mathf.Floor(rh.point.x / world.scale));
                        int z = (int)(Mathf.Floor(rh.point.z / world.scale));
                        ModUnit(x, z);
                        break;
                }
            }
        } else {
            //Debug.Log("û������");
        }
    }

    private void ModUnit(int x,int z) {
        if (unitMode == WorldUnit.None) {
            world.set_units(x, z, WorldUnit.None);
        } else {
            var unit = world.get_units(x, z);
            if (drawType == DrawType.Add) {
                if ((unit & unitMode) != unitMode) {
                    world.set_units(x, z, unit |= unitMode);
                } 
            } else {
                if ((unit & unitMode) == unitMode) {
                    world.set_units(x, z, unit -= unitMode);
                }
            }
        }
    }

    private int _select = 0;
    private Texture[] _items = new Texture[12];
    WorldUnit unitMode = WorldUnit.None;
    DrawMode drawMode = DrawMode.Point;
    DrawType drawType = DrawType.Add;
    WorldCreate world;
    GUIStyle selStyle;
    GUIStyle defStyle;
    int size;
    void OnGUI() {

        if (world == null) {
            world = FindObjectOfType<WorldCreate>();
            if (world == null) {
                GUILayout.Label("û�е�ͼ����");
                return;
            }
        }
        if (!world.has_units()) {
            world.Init();
            this.size = world.size;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("��ͼ��С��");
        if (int.TryParse(GUILayout.TextField(this.size.ToString()), out int size)) {
            this.size = size;
        }
        GUILayout.Space(40);
        if (GUILayout.Button("��ʼ��")) {
            world.size = size;
            world.units = new int[size * size];
            world.Init();
        }
        GUILayout.EndHorizontal();

        Color def = GUI.color;

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        if (drawMode == DrawMode.Point) {
            GUI.color = Color.red;
        } else {
            GUI.color = def;
        }
        if (GUILayout.Button("�����")) {
            drawMode = DrawMode.Point;
        }
        if (drawMode == DrawMode.Rect) {
            GUI.color = Color.red;
        } else {
            GUI.color = def;
        }
        if (GUILayout.Button("���λ���")) {
            drawMode = DrawMode.Rect;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);


        GUILayout.BeginHorizontal();
        if (drawType == DrawType.Add) {
            GUI.color = Color.red;
        } else {
            GUI.color = def;
        }
        if (GUILayout.Button("����ģʽ")) {
            drawType = DrawType.Add;
        }
        if (drawType == DrawType.Remove) {
            GUI.color = Color.red;
        } else {
            GUI.color = def;
        }
        if (GUILayout.Button("�Ƴ�ģʽ")) {
            drawType = DrawType.Remove;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);



        if (unitMode == WorldUnit.None) {
            GUI.color = Color.red;
        } else {
            GUI.color = def;
        }
        if (GUILayout.Button("���ƿյ�")) {
            unitMode = WorldUnit.None;
        }
        if (unitMode == WorldUnit.Impede) {
            GUI.color = Color.red;
        } else {
            GUI.color = def;
        }
        if (GUILayout.Button("�����ϰ�")) {
            unitMode = WorldUnit.Impede;
        }
        if (unitMode == WorldUnit.Monster) {
            GUI.color = Color.red;
        } else {
            GUI.color = def;
        }
        if (GUILayout.Button("����Ұ��")) {
            unitMode = WorldUnit.Monster;
        }
        if (unitMode == WorldUnit.City) {
            GUI.color = Color.red;
        } else {
            GUI.color = def;
        }
        if (GUILayout.Button("���Ƴ���")) {
            unitMode = WorldUnit.City;
        }
        if (unitMode == WorldUnit.Mountain) {
            GUI.color = Color.red;
        } else {
            GUI.color = def;
        }
        if (GUILayout.Button("���Ƹ�ɽ")) {
            unitMode = WorldUnit.Mountain;
        }
        if (unitMode == WorldUnit.NewVillage) {
            GUI.color = Color.red;
        } else {
            GUI.color = def;
        }
        if (GUILayout.Button("�������ִ�")) {
            unitMode = WorldUnit.NewVillage;
        }


        if (unitMode == WorldUnit.Level1) {
            GUI.color = Color.red;
        } else {
            GUI.color = def;
        }
        if (GUILayout.Button("����һ������")) {
            unitMode = WorldUnit.Level1;
        }
        if (unitMode == WorldUnit.Level2) {
            GUI.color = Color.red;
        } else {
            GUI.color = def;
        }
        if (GUILayout.Button("���ƶ�������")) {
            unitMode = WorldUnit.Level2;
        }
        if (unitMode == WorldUnit.Level3) {
            GUI.color = Color.red;
        } else {
            GUI.color = def;
        }
        if (GUILayout.Button("������������")) {
            unitMode = WorldUnit.Level3;
        }
        if (unitMode == WorldUnit.Level4) {
            GUI.color = Color.red;
        } else {
            GUI.color = def;
        }
        if (GUILayout.Button("�����ļ�����")) {
            unitMode = WorldUnit.Level4;
        }
        if (unitMode == WorldUnit.Level5) {
            GUI.color = Color.red;
        } else {
            GUI.color = def;
        }
        if (GUILayout.Button("�����弶����")) {
            unitMode = WorldUnit.Level5;
        }
        GUI.color = def;
        //EditorGUILayout.BeginHorizontal("box");
        //int sizeY = 100 * Mathf.CeilToInt(_items.Length / 4f);
        //_select = GUI.SelectionGrid(new Rect(new Vector2(0, 155), new Vector2(100 * 4, sizeY)), _select, _items, 4);//���Ը���gridѡ���,��Ҫ������ͼ����_items}

        GUILayout.Space(20);
        GUILayout.Label("��ͼ��������"+ world.units_count());


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("��ȡ����")) {
            byte[] byt = Tools.ReadAllBytes("config/map.data");
            world.units = Tools.DeserializeObject(byt) as int[];
            world.size = (int)Mathf.Sqrt(world.units_count());
            world.Init();
        }
        if (GUILayout.Button("��������")) {
            byte[] byt = Tools.SerializeObject(world.units);
            Tools.WriteAllBytes("config/map.data", byt);
        }
        GUILayout.EndHorizontal();


        GUILayout.Space(20);
        if (GUILayout.Button("���ɵص�")) {
            PointCreate point = FindObjectOfType<PointCreate>();
            if (point) {
                point.Create();
            } else {
                Debug.LogError("�Ҳ����ݵ�������");
            }
        }


        GUILayout.Space(20);
        if (GUILayout.Button("����ɽ��")) {
            GameObjectCreate obj = FindObjectOfType<GameObjectCreate>();
            if (obj) {
                obj.CreateWorld();
            } else {
                Debug.LogError("�Ҳ�������������");
            }
        }
    }

  






}
