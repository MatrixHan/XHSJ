using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GenerateWorld {
    public enum CreateType {
        Node,
        Tree,
        Decorate,
        City,
    }

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

        public List<SpaceData> all_pos;

        private List<GameObject> all_obj;

        private void Awake() {
            seaObj.Init();
            groundObj.Init();
            city_groundObj.Init();
            forestObj.Init();
        }

        public void Generate() {
            DateTime start_time = DateTime.Now;

            if (all_obj != null) {
                foreach (GameObject item in all_obj) {
                    Destroy(item);
                }
            }

            if (seed < 0) {
                seed = (int)start_time.Second;
            }
            Random.InitState(seed);

            all_pos = new List<SpaceData>();
            all_pos.AddRange(CreatePos(city_count, city_size, city_random, city_dis, SpaceType.City));
            all_pos.AddRange(CreatePos(field_count, field_size, field_random, field_dis, SpaceType.Forest));



            all_obj = new List<GameObject>();
            int idx = 0;

            GameObject sea = Instantiate(seaObj.obj.prefab);
            Transform sea_tf = sea.transform;
            sea_tf.localScale = new Vector3(map_size * map_size, seaObj.obj.size.y, map_size * map_size);
            int map_pos = map_size / 2 + map_edge;
            sea_tf.position = new Vector3(map_pos, sea_altitude, map_pos);
            all_obj.Add( sea);

            foreach (SpaceData item in all_pos) {
                GameObject obj;
                int width = item.width;
                int height = item.height;
                if (item.type == SpaceType.City) {
                    obj = Instantiate(city_groundObj.obj.prefab);
                    Transform obj_tf = obj.transform;
                    obj_tf.localScale = new Vector3(width * city_groundObj.obj.size.x, city_groundObj.obj.size.y, height * city_groundObj.obj.size.z);
                    obj_tf.position = new Vector3(item.pos.x, city_altitude, item.pos.y);
                    all_obj.Add(obj);
                } else {
                    //obj = Instantiate(forestObj.obj.prefab);
                    //width = width * 3;
                    //height = height * 3;
                    //ground_height = -0.1f;
                    //Transform obj_tf = obj.transform;
                    //obj_tf.localScale = new Vector3(width * forestObj.obj.size.x, forestObj.obj.size.y, height * forestObj.obj.size.z);
                    //obj_tf.position = new Vector3(item.pos.x, ground_height, item.pos.y);
                    //all_obj.Add( obj;
                }


                GameObject ground = Instantiate(groundObj.obj.prefab);
                Transform ground_tf = ground.transform;
                ground_tf.localScale = new Vector3((item.width + ground_size) * groundObj.obj.size.x, groundObj.obj.size.y, (item.height + ground_size) * groundObj.obj.size.z);
                ground_tf.position = new Vector3(item.pos.x, ground_altitude, item.pos.y);
                all_obj.Add(ground);
            }



            DateTime a_time = DateTime.Now;
            Debug.Log("����ر���ʱ�䣺" + (a_time - start_time).TotalMilliseconds);

            int min_map_pos = map_edge - ground_size;
            int max_map_pos = map_size + map_edge + ground_size;
            for (int x1 = min_map_pos; x1 < max_map_pos; x1++) {
                for (int y1 = min_map_pos; y1 < max_map_pos; y1++) {
                    float x = x1 + Random.Range(-0.38f, 0.38f);
                    float y = y1 + Random.Range(-0.38f, 0.38f);
                    CreateType create = CreateType.Node;
                    foreach (SpaceData item in all_pos) {
                        if (item.IsTherein(new Vector2(x, y))) {
                            if (item.type == SpaceType.City) {
                                create = CreateType.City;
                                break;
                            } else {
                                create = CreateType.Tree;
                                break;
                            }
                        }
                    }

                    if (create == CreateType.Node) {
                        int ground_dege = ground_size / 2 - 2;
                        foreach (SpaceData item in all_pos) {
                            if (item.IsTherein(new Vector2(x, y), ground_dege)) {
                                create = CreateType.Decorate;
                                break;
                            }
                        }
                    }

                    if (create != CreateType.City && create != CreateType.Node) {
                        int ran_create_id = Random.Range(0, 100);
                        if (ran_create_id < 5) {
                            if (create == CreateType.Tree) {
                                int ran = Random.Range(0, treeObjs[0].objs.Length);
                                GameObject prefab = treeObjs[0].objs[ran].prefab;
                                Vector3 size = treeObjs[0].objs[ran].size;
                                GameObject obj = Instantiate(prefab);
                                Transform obj_tf = obj.transform;
                                float tree_size = Random.Range(0.5f, 1.5f);
                                obj_tf.localScale = new Vector3(tree_size, tree_size + Random.Range(-0.2f, 0.2f), tree_size);
                                obj_tf.position = new Vector3(x, 1, y);
                                obj_tf.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                                all_obj.Add(obj);
                            }
                        } else if (ran_create_id < 15) {
                            int ran = Random.Range(0, decorateObjs[0].objs.Length);
                            GameObject prefab = decorateObjs[0].objs[ran].prefab;
                            Vector3 size = decorateObjs[0].objs[ran].size;
                            GameObject obj = Instantiate(prefab);
                            Transform obj_tf = obj.transform;
                            float tree_size = Random.Range(0.05f, 0.2f);
                            obj_tf.localScale = new Vector3(tree_size, tree_size + Random.Range(-0.02f, 0.02f), tree_size);
                            obj_tf.position = new Vector3(x, 1, y);
                            obj_tf.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                            all_obj.Add(obj);
                    } else {

                    }
                }
                }
            }



            DateTime end_time = DateTime.Now;
            Debug.Log("���������ܻ���ʱ�䣺" + (end_time - start_time).TotalMilliseconds);
        }

        private SpaceData[] CreatePos(int count, int size, int rand, int dis, SpaceType typ) {
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
                SpaceData data = new SpaceData(new Vector2(pos_x, pos_y), Random.Range(size - rand, size + rand), Random.Range(size - rand, size + rand), typ);
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
            Generate();
        }

#if UNITY_EDITOR
        private void OnGUI() {
            if (seed > 0) {
                if (GUI.Button(new Rect(10, 10, 30, 30), "<")) {
                    seed--;
                    Generate();
                }
            }
            if (seed < int.MaxValue) {
                if (GUI.Button(new Rect(40, 10, 30, 30), ">")) {
                    seed++;
                    Generate();
                }
            }
            GUI.Label(new Rect(10, 40, 100, 30), "count:"+ all_pos.Count);
        }
#endif
    }
}