using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GenerateWorld {

    public class GenerateMap : MonoBehaviour {



        public float sea_altitude = -0.5f;
        public float ground_altitude = 0f;
        public float city_altitude = 0.05f;


        /// <summary>
        /// ��ͼ��Ե��С
        /// </summary>
        public int map_edge = 100;

        /// <summary>
        /// ��ͼ�Ĵ�С
        /// </summary>
        public int map_size = 10000;

        /// <summary>
        /// ���еĴ�С
        /// </summary>
        public int city_size = 200;

        /// <summary>
        /// ���еľ���
        /// </summary>
        public int city_dis = 200;

        /// <summary>
        /// ���еĴ�С�������
        /// </summary>
        public int city_random = 100;

        /// <summary>
        /// ���е�����
        /// </summary>
        public int city_count = 3;

        /// <summary>
        /// ���صĴ�С
        /// </summary>
        public int field_size = 15;

        /// <summary>
        /// ���صľ���
        /// </summary>
        public int field_dis = 5;

        /// <summary>
        /// ���صĴ�С�������
        /// </summary>
        public int field_random = 10;

        /// <summary>
        /// ���ص�����
        /// </summary>
        public int field_count = 100;

        /// <summary>
        /// �յصĴ�С
        /// </summary>
        public int ground_size = 100;

        /// <summary>
        /// ������ˮ��obj
        /// </summary>
        public StaticObjData waterObj;

        /// <summary>
        /// �����������obj
        /// </summary>
        public StaticObjData seaObj;

        /// <summary>
        /// �����������obj
        /// </summary>
        public StaticObjData groundObj;

        /// <summary>
        /// ��������������obj
        /// </summary>
        public StaticObjData city_groundObj;

        /// <summary>
        /// ������ɭ�ֵ�obj
        /// </summary>
        public StaticObjData forestObj;

        /// <summary>
        /// ��������·��obj
        /// </summary>
        public StaticObjData wayObj;

        /// <summary>
        /// ������ǽ��obj
        /// </summary>
        public StaticObjData wallObj;

        /// <summary>
        /// ������ǽ�ڵ��obj
        /// </summary>
        public StaticObjData wallNodeObj;

        /// <summary>
        /// ����������obj
        /// </summary>
        public StaticObjsData[] treeObjs;

        /// <summary>
        /// ���������ӵ�obj
        /// </summary>
        public StaticObjsData[] houseObjs;

        /// <summary>
        /// ��������Ҫ�̵��obj
        /// </summary>
        public StaticObjsData[] shopObjs;

        /// <summary>
        /// ��������չ�̵��obj
        /// </summary>
        public StaticObjsData[] exshopObjs;

        /// <summary>
        /// ���ڵ���װ�����obj
        /// </summary>
        public StaticObjsData[] decorateObjs;

        /// <summary>
        /// �������
        /// </summary>
        public int seed = 100;

        /// <summary>
        /// ���ܶ�
        /// </summary>
        public int tree_density = 5;
        /// <summary>
        /// ����װ�����ܶ�
        /// </summary>
        public int decorate_density = 20;

        List<SpaceData> grounds;
        List<SpaceData> walls;
        List<SpaceData> ways;
        List<SpaceData> houses;
        List<SpaceData> decorates;
        List<SpaceData> doors;
        List<SpaceData> allData;

        private List<GameObject> all_obj;
        public MapData map_data;

        private void Awake() {
            seaObj.Init();
            groundObj.Init();
            city_groundObj.Init();
            forestObj.Init();
            wallObj.Init();
            wallNodeObj.Init();
            wayObj.Init();
            waterObj.Init();
            foreach (var item in shopObjs) {
                item.Init();
            }
            foreach (var item in houseObjs) {
                item.Init();
            }
        }

        public void GenerateWorld() {
            DateTime start_time = DateTime.Now;
            if (seed < 0) {
                seed = (int)start_time.Second;
            }
            Random.InitState(seed);
            grounds = new List<SpaceData>();
            walls = new List<SpaceData>();
            ways = new List<SpaceData>();
            houses = new List<SpaceData>();
            decorates = new List<SpaceData>();
            doors = new List<SpaceData>();

            grounds.AddRange(CreateSpacePos(city_count, city_size, city_random, city_dis, SpaceType.City));
            grounds.AddRange(CreateSpacePos(field_count, field_size, field_random, field_dis, SpaceType.Forest));
            grounds.AddRange(CreateGround(grounds.ToArray()));
            BuildCity();
            BuildDecorate();

            allData = new List<SpaceData>(grounds.Count + walls.Count + ways.Count + houses.Count + decorates.Count);
            allData.AddRange(grounds);
            allData.AddRange(walls);
            allData.AddRange(ways);
            allData.AddRange(houses);
            allData.AddRange(decorates);
            allData.AddRange(doors);

            int map_size = this.map_size / 2 + map_edge;
            Vector3 map_pos = new Vector3(map_size, sea_altitude, map_size);
            Vector3 sea_scale = new Vector3(this.map_size * 8 + 1000, 1, this.map_size * 8 + 1000);
            allData.Add(new SpaceData(map_pos, sea_scale, SpaceType.Sea, useMeshScale: true));
            Vector3 water_scale = new Vector3(this.map_size + map_edge * 2, 1, this.map_size + map_edge * 2);
            allData.Add(new SpaceData(map_pos+new Vector3(0,0.05f,0), water_scale, SpaceType.Water, useMeshScale:true));

            map_data = new MapData() { allData = allData.ToArray() };

            CreateWorldGameObject();

            DateTime end_time = DateTime.Now;
            Debug.Log("���������ܻ���ʱ�䣺" + (end_time - start_time).TotalMilliseconds);
        }

        private void BuildDecorate() {
            List<SpaceData> tmp_trees = new List<SpaceData>();
            int min_map_pos = map_edge - ground_size;
            int max_map_pos = map_size + map_edge + ground_size;

            for (int x1 = min_map_pos; x1 < max_map_pos; x1++) {
                for (int y1 = min_map_pos; y1 < max_map_pos; y1++) {
                    float x = x1 + Random.Range(-0.38f, 0.38f);
                    float z = y1 + Random.Range(-0.38f, 0.38f);
                    float max_dis = default;
                    Vector3 center = default;
                    CreateType create = CreateType.Node;
                    foreach (SpaceData item in grounds) {
                        if (item.IsTherein(new Vector3(x, 0, z), 2)) {
                            if (item.type == SpaceType.City) {
                                create = CreateType.City;
                                break;
                            } else if(item.type == SpaceType.Forest) {
                                create = CreateType.Tree;
                                center = item.pos;
                                max_dis = Mathf.Max(item.scale.x, item.scale.z) / 2.5f;
                                break;
                            } else if (item.type == SpaceType.Ground) {
                                if (item.IsTherein(new Vector3(x, 0, z), -space_edge)) {
                                    create = CreateType.Decorate;
                                    break;
                                }
                            }
                        }
                    }

                    if (create != CreateType.Node) {
                        int ran_create_id = Random.Range(0, 100);
                        if (ran_create_id < tree_density) {
                            if (create == CreateType.Tree && create != CreateType.City) {
                                Vector3 pos = new Vector3(x, 1, z);
                                float center_dis = Vector3.Distance(pos, center);
                                bool can_create = center_dis < max_dis;
                                // ������Χ50�ײ�������
                                if (can_create) {
                                    foreach (var item in doors) {
                                        if (Vector3.Distance(item.pos, pos) < space_size) {
                                            can_create = false;
                                            break;
                                        }
                                    }
                                }
                                if (can_create) {
                                    float tree_size = Random.Range(0.75f, 1.25f);
                                    if (center_dis < max_dis * 0.5f) {
                                        tree_size *= Random.Range(1f, 1.25f);
                                    } else if (center_dis < max_dis * 0.25f) {
                                        tree_size *= Random.Range(1.25f, 1.5f);
                                    } else if (center_dis < max_dis * 0.1f) {
                                        tree_size *= Random.Range(1.5f, 2f);
                                    }
                                    int ran = Random.Range(0, treeObjs[0].objs.Length);
                                    SpaceData tree = new SpaceData(pos, new Vector3(tree_size, tree_size, tree_size), SpaceType.Tree, angle: Random.Range(0f, 360f), id: 0, idx: ran);
                                    foreach (var item in tmp_trees) {
                                        if (item.IsOverlap(tree, 1)){
                                            can_create = false;
                                        }
                                    }
                                    if (can_create) {
                                        tmp_trees.Add(tree);
                                    }
                                }
                            }
                        } else if (ran_create_id < decorate_density) {
                            var pos = new Vector3(x, 1, z);
                            bool can_create = true;
                            if (create == CreateType.City) {
                                can_create = false;
                            }
                            if (can_create) {
                                int ran = Random.Range(0, decorateObjs[0].objs.Length);
                                float decorate_size = Random.Range(0.2f, 0.5f);
                                SpaceData decorate = new SpaceData(pos, new Vector3(decorate_size, decorate_size, decorate_size), SpaceType.Decorate, angle: Random.Range(0f, 360f), id: 0, idx: ran);
                                decorates.Add(decorate);
                            }
                        }
                    }
                }
            }
            decorates.AddRange(tmp_trees);
        }


        int house_width = 10;
        int house_length = 8;
        // ÿ��������50*50  �߽���10��  ��·3�׿� Χǽ��3�� ���ſ�5��
        int space_size = 50;
        int space_edge = 10;
        int way_width = 2;
        int wall_width = 2;
        int wallnode_size = 3;
        int door_width = 4;
        private void BuildCity() {
            List<SpaceData> tmp_shops = new List<SpaceData>();
            int half_wall_width = (int)(wall_width * 0.5f);

            DateTime start_time = DateTime.Now;

            foreach (SpaceData item in grounds) {
                
                if (item.type == SpaceType.City) {
                    SpaceData city = item;

                    Vector3 pos = item.pos;
                    float width = item.scale.x;
                    float length = item.scale.z;
                    float min_x = item.min_x;
                    float max_x = item.max_x;
                    float min_y = item.min_z;
                    float max_y = item.max_z;

                    // ����ͳһ��������
                    // �����·  
                    // �����һ���㣬Ȼ�����һ����������·���������߽�����·
                    int way_node_min_x = (int)(min_x + space_size + space_edge);
                    int way_node_min_y = (int)(min_y + space_size + space_edge);
                    int way_node_max_x = (int)(max_x - space_size - space_edge);
                    int way_node_max_y = (int)(max_y - space_size - space_edge);
                    // �����������ĵ�
                    Vector3 city_center;
                    if (width > ((space_size + space_edge) * 0.5f) && length > ((space_size + space_edge) * 0.5f)) {
                        city_center = new Vector3(Random.Range(way_node_min_x, way_node_max_x),0, Random.Range(way_node_min_y, way_node_max_y));
                        Debug.Log(city_center);
                    } else {
                        city_center = new Vector3(min_x + (min_y - min_x) * 0.5f,0, max_x + (max_y - max_x) * 0.5f);
                    }

                    // ��ǽ
                    Vector3 east_wall_pos = new Vector3(max_x - half_wall_width, 1, pos.z);
                    float east_wall_width = wall_width;
                    float east_wall_length = length;
                    SpaceData east_wall = new SpaceData(east_wall_pos, new Vector3(east_wall_width, 3, east_wall_length) , SpaceType.Wall, useMeshScale:true);
                    // ��ǽ
                    Vector3 west_wall_pos = new Vector3(min_x + half_wall_width, 1, pos.z);
                    float west_wall_width = wall_width;
                    float west_wall_length = length;
                    SpaceData west_wall = new SpaceData(west_wall_pos, new Vector3(west_wall_width,3, west_wall_length), SpaceType.Wall, useMeshScale: true);
                    //��ǽ
                    Vector3 north_wall_pos = new Vector3(pos.x, 1, max_y - half_wall_width);
                    float north_wall_width = width;
                    float north_wall_length = wall_width;
                    SpaceData north_wall = new SpaceData(north_wall_pos, new Vector3(north_wall_width,3, north_wall_length), SpaceType.Wall, useMeshScale: true);
                    // ��ǽ1
                    float south1_wall_width = (int)(max_x - city_center.x - door_width);
                    float south1_wall_length = wall_width;
                    Vector3 south1_wall_pos = new Vector3(max_x - south1_wall_width * 0.5f, 1, min_y + half_wall_width);
                    SpaceData south1_wall = new SpaceData(south1_wall_pos, new Vector3(south1_wall_width,3, south1_wall_length), SpaceType.Wall, useMeshScale: true);
                    // ��ǽ2
                    float south2_wall_width = (int)(city_center.x - min_x - door_width);
                    float south2_wall_length = wall_width;
                    Vector3 south2_wall_pos = new Vector3(min_x + south2_wall_width * 0.5f, 1, min_y + half_wall_width);
                    SpaceData south2_wall = new SpaceData(south2_wall_pos, new Vector3(south2_wall_width,3, south2_wall_length), SpaceType.Wall, useMeshScale: true);

                    walls.Add(east_wall);
                    walls.Add(west_wall);
                    walls.Add(north_wall);
                    walls.Add(south1_wall);
                    walls.Add(south2_wall);

                    // Χǽ����
                    walls.Add(new SpaceData(new Vector3(min_x + half_wall_width, 1, min_y + half_wall_width), new Vector3(wallnode_size,5, wallnode_size), SpaceType.WallNode, useMeshScale: true));
                    walls.Add(new SpaceData(new Vector3(min_x + half_wall_width, 1, max_y - half_wall_width), new Vector3(wallnode_size, 5, wallnode_size), SpaceType.WallNode, useMeshScale: true));
                    walls.Add(new SpaceData(new Vector3(max_x - half_wall_width, 1, min_y + half_wall_width), new Vector3(wallnode_size, 5, wallnode_size), SpaceType.WallNode, useMeshScale: true));
                    walls.Add(new SpaceData(new Vector3(max_x - half_wall_width, 1, max_y - half_wall_width), new Vector3(wallnode_size, 5, wallnode_size), SpaceType.WallNode, useMeshScale: true));
                    walls.Add(new SpaceData(new Vector3(min_x + south2_wall_width - half_wall_width, 1, min_y + half_wall_width), new Vector3(wallnode_size * 2, 5, wallnode_size * 2), SpaceType.WallNode, useMeshScale: true));
                    walls.Add(new SpaceData(new Vector3(max_x - south1_wall_width + half_wall_width, 1, min_y + half_wall_width), new Vector3(wallnode_size * 2, 5, wallnode_size * 2), SpaceType.WallNode, useMeshScale: true));

                    var door = new SpaceData(new Vector3(city_center.x, 0, min_y), new Vector3(door_width, 1, 1), SpaceType.Door);
                    // ��
                    doors.Add(door);

                    //���ɵ�
                    List<SpaceData> ways = new List<SpaceData>();
                    int main_way_vertical_width = door_width;
                    int main_way_vertical_length = (int)(max_y - min_y - space_edge);

                    int main_way_horizontal_width = (int)(max_x - min_x - space_edge * 2);
                    int main_way_horizontal_length = door_width;

                    Vector3 main_vertical_way_pos = new Vector3(city_center.x, 1, min_y + main_way_vertical_length * 0.5f);
                    Vector3 main_horizontal_way_pos = new Vector3(min_x + main_way_horizontal_width * 0.5f + space_edge, 1, city_center.z);

                    SpaceData main_vertical_way = new SpaceData(main_vertical_way_pos, new Vector3(main_way_vertical_width, 0.08f, main_way_vertical_length), SpaceType.Way, useMeshScale:true);
                    SpaceData main_horizontal_way = new SpaceData(main_horizontal_way_pos, new Vector3(main_way_horizontal_width, 0.08f, main_way_horizontal_length), SpaceType.Way, useMeshScale: true);
                    ways.Add(main_vertical_way);
                    ways.Add(main_horizontal_way);
                    // ��ӱ����е�·
                    this.ways.AddRange(ways);


                    // ���ĵ������ɵ����ߴ����̵�
                    int shop_idx = house_width;
                    float house_offset =  house_width * 0.5f+way_width;
                    float main_way_right_x = city_center.x + house_offset;
                    float main_way_left_x = city_center.x - house_offset;
                    float main_way_up_y = city_center.z + house_offset;
                    float main_way_down_y = city_center.z - house_offset;
                    while (true) {
                        int up_y = (int)(city_center.z + shop_idx);
                        int down_y = (int)(city_center.z - shop_idx);
                        int right_x = (int)(city_center.x + shop_idx);
                        int left_x = (int)(city_center.x - shop_idx);

                        if (up_y > (main_vertical_way.max_z) && down_y < (main_vertical_way.min_z) && right_x > (main_horizontal_way.max_x) && left_x < (main_horizontal_way.min_x)) {
                            break;
                        }
                        //�����
                        if (up_y < (main_vertical_way.max_z) && Vector3.Distance(door.pos, new Vector3(main_way_right_x, 1, up_y)) > space_edge) {
                            tmp_shops.Add(BuildHouse(new Vector3(main_way_right_x, 1, up_y), Direction.West, SpaceType.Shop));
                            tmp_shops.Add(BuildHouse(new Vector3(main_way_left_x, 1, up_y), Direction.East, SpaceType.Shop));
                        }
                        //�����
                        if (down_y > (main_vertical_way.min_z) && Vector3.Distance(door.pos, new Vector3(main_way_right_x, 1, down_y)) > space_edge) {
                            tmp_shops.Add(BuildHouse(new Vector3(main_way_right_x, 1, down_y), Direction.West, SpaceType.Shop));
                            tmp_shops.Add(BuildHouse(new Vector3(main_way_left_x, 1, down_y), Direction.East, SpaceType.Shop));
                        }


                        right_x += house_width;
                        left_x -= house_width;
                        // �ұߵ�
                        if (right_x < (main_horizontal_way.max_x) && Vector3.Distance(door.pos, new Vector3(right_x, 1, main_way_up_y)) > space_edge) {
                            tmp_shops.Add(BuildHouse(new Vector3(right_x, 1, main_way_up_y), Direction.South, SpaceType.Shop));
                            tmp_shops.Add(BuildHouse(new Vector3(right_x, 1, main_way_down_y), Direction.North, SpaceType.Shop));
                        }
                        // ��ߵ�
                        if (left_x > (main_horizontal_way.min_x) && Vector3.Distance(door.pos, new Vector3(left_x, 1, main_way_up_y)) > space_edge) {
                            tmp_shops.Add(BuildHouse(new Vector3(left_x, 1, main_way_up_y), Direction.South, SpaceType.Shop));
                            tmp_shops.Add(BuildHouse(new Vector3(left_x, 1, main_way_down_y), Direction.North, SpaceType.Shop));
                        }
                        shop_idx += house_width;
                    }

                    // ������
                    float try_count = city.scale.x * city.scale.z / 50;
                    for (int i = 0; i < try_count; i++) {
                        int x = (int)Random.Range(city.min_x + space_edge, city.max_x - space_edge);
                        int y = (int)Random.Range(city.min_z + space_edge, city.max_z - space_edge);
                        bool can_build = true;
                        SpaceData h = BuildHouse(new Vector3(x, 1, y), x < city_center.x ? Direction.East : Direction.West, SpaceType.House);
                        foreach (SpaceData shop in tmp_shops) {
                            if (shop.IsOverlap(h, 8)) {
                                can_build = false;
                            }
                        }
                        if (can_build) {
                            foreach (SpaceData house in houses) {
                                if (house.IsOverlap(h, 7)) {
                                    can_build = false;
                                }
                            }
                        }
                        if (can_build && Vector3.Distance(door.pos, h.pos) > space_edge * 2 && Vector3.Distance(city_center, h.pos) > space_edge * 2) {
                            houses.Add(h);
                        }
                    };

                    Debug.Log("������ " + houses.Count + "/" + try_count);
                }
            }
            houses.AddRange(tmp_shops);

            DateTime end_time = DateTime.Now;
            Debug.Log("���ɳ��л���ʱ�䣺" + (end_time - start_time).TotalMilliseconds);
        }

        /// <summary>
        /// ���ɷ���
        /// </summary>
        /// <param name="pos">λ��</param>
        /// <param name="dir">����</param>
        private SpaceData BuildHouse( Vector3 pos, Direction dir, SpaceType typ) {
            StaticObjsData[] objs;
            switch (typ) {
                case SpaceType.House:
                    objs = houseObjs;
                    break;
                case SpaceType.Shop:
                    objs = shopObjs;
                    break;
                default:
                    objs = null;
                    break;
            }
            int id = objs == null ? 0 : Random.Range(0, objs.Length);
            int idx = objs == null ? 0 : Random.Range(0, objs[id].objs.Length);

            var size = Random.Range(0.9f, 1.1f);
            float angle = Random.Range(-5f, 5f);
            switch (dir) {
                case Direction.East:
                    angle += 90;
                    break;
                case Direction.West:
                    angle += 270;
                    break;
                case Direction.South:
                    angle += 180;
                    break;
            }
            return (new SpaceData(pos, new Vector3(size, size, size), typ, angle: angle, id:id, idx:idx));
        }

        private void CreateWorldGameObject() {
            DateTime start_time = DateTime.Now;
            if (all_obj != null) {
                foreach (GameObject item in all_obj) {
                    Destroy(item);
                }
            }
            all_obj = new List<GameObject>();

            foreach (SpaceData space_data in allData) {
                StaticObj static_obj = null;
                switch (space_data.type) {
                    case SpaceType.Forest:
                        break;
                    case SpaceType.Ground:
                        static_obj = groundObj.obj;
                        break;
                    case SpaceType.City:
                        static_obj = city_groundObj.obj;
                        break;
                    case SpaceType.House:
                        static_obj = houseObjs[space_data.id].objs[space_data.idx];
                        break;
                    case SpaceType.Shop:
                        static_obj = shopObjs[space_data.id].objs[space_data.idx];
                        break;
                    case SpaceType.Way:
                        static_obj = wayObj.obj;
                        break;
                    case SpaceType.Wall:
                        static_obj = wallObj.obj;
                        break;
                    case SpaceType.WallNode:
                        static_obj = wallNodeObj.obj;
                        break;
                    case SpaceType.Tree:
                        static_obj = treeObjs[space_data.id].objs[space_data.idx];
                        break;
                    case SpaceType.Decorate:
                        static_obj = decorateObjs[space_data.id].objs[space_data.idx];
                        break;
                    case SpaceType.Sea:
                        static_obj =  seaObj.obj;
                        break;
                    case SpaceType.Water:
                        static_obj = waterObj.obj;
                        Debug.LogError("ˮ size="+ static_obj.size + " scale=" + static_obj.scale);
                        break;
                    default:
                        break;
                }
                if (static_obj != null) {
                    GameObject obj = Instantiate(static_obj.prefab, new Vector3(space_data.pos.x, space_data.pos.y, space_data.pos.z), Quaternion.Euler(0, space_data.angle, 0));
                    if (space_data.useMeshScale) {
#if UNITY_EDITOR
                        if (static_obj.scale == default) {
                            Debug.LogError("�����obj���Ѿ�����Init������ʼ��:" + static_obj);
                        }
#endif
                            obj.transform.localScale =new Vector3(space_data.scale.x * static_obj.scale.x, space_data.scale.y * static_obj.scale.y, space_data.scale.z * static_obj.scale.z);
                    } else {
                        obj.transform.localScale = space_data.scale;
                    }
                    all_obj.Add(obj);
                }
            }


            //GameObject sea = Instantiate(seaObj.obj.prefab);
            //Transform sea_tf = sea.transform;
            //sea_tf.localScale = new Vector3(map_size * map_size, seaObj.obj.scale.y, map_size * map_size);
            //int map_pos = map_size / 2 + map_edge;
            //sea_tf.position = new Vector3(map_pos, sea_altitude, map_pos);
            //all_obj.Add(sea);

            DateTime end_time = DateTime.Now;
            Debug.Log("����ر���ʱ�䣺" + (end_time - start_time).TotalMilliseconds);
        }

        private SpaceData[] CreateGround(SpaceData[] grounds) {
            List<SpaceData> result = new List<SpaceData>();
            foreach (var item in grounds) {
                SpaceData data = new SpaceData(item.pos - new Vector3(0, 0.05f, 0), new Vector3(item.scale.x + ground_size, 1, item.scale.z + ground_size), SpaceType.Ground, useMeshScale: true);
                result.Add(data);
            }
            return result.ToArray();
        }

        private SpaceData[] CreateSpacePos(int count, int size, int rand, int dis, SpaceType typ) {
            List<SpaceData> rand_pos = new List<SpaceData>();
            int idx = 0;
            // ��������100�Σ������ͼ̫С�޷������㹻�������
            int max = count + 100;
            // ��С���λ��
            int min_pos = size / 2 + map_edge;
            // ������λ��
            int max_pos = map_size - size / 2 + map_edge;
            // ��С����
            int min_dis = size + dis;
            while (idx < max) {
                idx++;
                int pos_x = Random.Range(min_pos, max_pos);
                int pos_y = Random.Range(min_pos, max_pos);
                Vector2 pos = new Vector2(pos_x, pos_y);
                int width;
                int height;
                if (idx > max / 10) {
                    width = size - rand;
                    height = size - rand;
                } else if (idx > max / 2) {
                    width = Random.Range(size - rand, size);
                    height = Random.Range(size - rand, size);
                } else {
                    width = Random.Range(size, size + rand);
                    height = Random.Range(size, size + rand);
                }
                SpaceData data = new SpaceData(new Vector3(pos_x, 0.05f, pos_y), new Vector3(Random.Range(size - rand, size + rand), 1, Random.Range(size - rand, size + rand)), typ, useMeshScale:true);
                bool can_pos = true;
                foreach (SpaceData p in rand_pos) {
                    if (p.IsOverlap(data, dis)) {
                        // ����̫������Ҫ�������
                        can_pos = false;
                        break;
                    }
                }

                if (!can_pos) {
                    continue;
                }

                rand_pos.Add(data);
                if (rand_pos.Count >= count) {
                    // �Ѿ���������㹻��ĳ�������
                    break;
                }
            }
            return rand_pos.ToArray();
        }

        private void Start() {
            GenerateWorld();
        }

#if UNITY_EDITOR
        private void OnGUI() {
            if (seed > 0) {
                if (GUI.Button(new Rect(10, 10, 30, 30), "<")) {
                    seed--;
                    GenerateWorld();
                }
            }
            if (seed < int.MaxValue) {
                if (GUI.Button(new Rect(40, 10, 30, 30), ">")) {
                    seed++;
                    GenerateWorld();
                }
            }
            GUI.Label(new Rect(10, 40, 100, 30), "count:"+ allData.Count);
        }
#endif
    }
}