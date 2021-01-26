using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

namespace GenerateWorld {
    /// <summary>
    /// ��ͼ����
    /// </summary>
    public class MapData {
        GenerateMap generate; // ���ɵ�ͼ����
        public string data_dir;
        SpaceData[] city_data; // ���г���ռ��
        SpaceData[] forest_data; // ����ɭ��ռ��
        SpaceData[] ground_data; // ��������ռ��
        SpaceData[] trigger_data; // ��������ռ��
        SpaceData[][] city_datas; // ���г�����ϸ����



        List<Thread> threads = new List<Thread>();
        List<FileTools> fileTools = new List<FileTools>();
        public float progress;

        public MapData(GenerateMap generate, int seed, int size) {
            data_dir = string.Format("map_{0}_{1}", seed, size);
            if (!Directory.Exists(data_dir)) {
                Directory.CreateDirectory(data_dir);
            }
            this.generate = generate;
        }


        public bool HasCityData(out SpaceData[] result) {
            return HasData(ref city_data, "city_data", out result);
        }
        public void SetCityData(SpaceData[] data) {
            city_data = data;
            SetData(data, "city_data");
        }

        public bool HasForestData(out SpaceData[] result) {
            return HasData(ref forest_data, "forest_data", out result);
        }
        public void SetForestData(SpaceData[] data) {
            forest_data = data;
            SetData(data, "forest_data");
        }

        public bool HasGroundData(out SpaceData[] result, out SpaceData[] triggers) {
            bool a = HasData(ref ground_data, "ground_data", out result);
            bool b = HasData(ref trigger_data, "trigger_data", out triggers);
            return a && b;
        }
        public void SetGroundData(SpaceData[] data, SpaceData[] triggers) {
            ground_data = data;
            SetData(data, "ground_data");
            SetData(triggers, "trigger_data");
        }




        private bool HasData(ref SpaceData[] data, string file_name, out SpaceData[] result) {
            try {
                if (data != null) {
                    result = data;
                    return true;
                } else {
                    // ��ȡ�ļ�
                    string path = Path.Combine(data_dir, file_name);
                    if (File.Exists(path)) {
                        FileTools file_tool = new FileReadTools(path);
                        fileTools.Add(file_tool);
                        byte[] bytes = null;
                        try {
                            while (true) {
                                Thread.Sleep(10);
                                if (file_tool == null) {
                                    result = null;
                                    return false;
                                }
                                lock (file_tool) {
                                    progress = file_tool.progress;
                                    if (file_tool.isdone) {
                                        bytes = file_tool.result;
                                        fileTools.Remove(file_tool);
                                        break;
                                    }
                                }
                            }
                            using (MemoryStream ms = new MemoryStream(bytes)) {
                                // //�Զ����Ƹ�ʽ��������������Ӷ���ͼ�����л��ͷ����л���
                                IFormatter formatter = new BinaryFormatter();
                                //���ַ����Զ����ƷŽ�memStream��
                                data = (SpaceData[])formatter.Deserialize(ms);
                            }
                            result = data;
                            return true;
                        } catch (System.Exception e) {
                            lock (fileTools) {
                                if (fileTools.Contains(file_tool)) {
                                    fileTools.Remove(file_tool);
                                }
                            }
                            Debug.LogError(e.Message + "\n" + e.StackTrace);
                        }
                    }
                }
            } catch (System.Exception e) {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
            }
            result = null;
            return false;
        }

        private void SetData(SpaceData[] data, string file_name) {
            Thread thread = new Thread(()=> {
                byte[] bytes;
                using (MemoryStream ms = new MemoryStream()) {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(ms, data);
                    bytes = ms.GetBuffer();
                }
                string path = Path.Combine(data_dir, file_name);
                if (File.Exists(path)) {
                    File.Delete(path);
                }
                // д���ļ�
                FileTools file_tool = new FileWriteTools(path, bytes);
                fileTools.Add(file_tool);
                while (true) {
                    Thread.Sleep(10);
                    if (file_tool == null) {
                        return;
                    }
                    lock (file_tool) {
                        if (file_tool.isdone) {
                            lock (fileTools) {
                                if (fileTools.Contains(file_tool)) {
                                    fileTools.Remove(file_tool);
                                }
                            }
                            return;
                        }
                    }
                }
            });
            thread.Start();
            threads.Add(thread);
        }

        ~MapData() {
            foreach (var item in fileTools) {
                item.Close();
            }
            fileTools = null;
            foreach (var item in threads) {
                item.Abort();
            }
            threads = null;
        }
    }
}