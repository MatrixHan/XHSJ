using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerateWorld {

    /// <summary>
    /// 地块数据
    /// </summary>
    [System.Serializable]    
    public struct SpaceData {
        public Vector pos;
        public Vector scale;
        public SpaceType type;
        public float angle;
        public bool useMeshScale;
        public short id;
        public short idx;

        public float min_x
        {
            get
            {
                return pos.x - scale.x * 0.5f - 1;
            }
        }
        public float max_x
        {
            get
            {
                return pos.x + scale.x * 0.5f + 1;
            }
        }
        public float min_z
        {
            get
            {
                return pos.z - scale.z * 0.5f - 1;
            }
        }
        public float max_z
        {
            get
            {
                return pos.z + scale.z * 0.5f + 1;
            }
        }


        public SpaceData(Vector3 pos, Vector3 scale, SpaceType type, float angle = 0, bool useMeshScale = false, short id = 0, short idx = 0) {
            this.pos = pos;
            this.scale = scale;
            this.type = type;
            this.angle = angle;
            this.useMeshScale = useMeshScale;
            this.id = id;
            this.idx = idx;
        }

        /// <summary>
        /// 是否可以看到空地
        /// </summary>
        /// <param name="pos">从哪里看过来</param>
        /// <param name="see_dis">视野距离</param>
        /// <returns></returns>
        public bool IsCanSee(Vector3 pos, int see_dis = 1000) {
            pos = pos + (this.pos - pos).normalized * see_dis;
            return IsTherein(pos, 1);
        }

        /// <summary>
        /// 位置是否交叉
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsOverlap(SpaceData data, int dis) {
            int min_dix = GetHypotenuse((int)data.scale.z, (int)data.scale.x) /2 + GetHypotenuse((int)scale.z, (int)scale.x) /2 + dis;
            return Vector3.Distance(data.pos, pos) < min_dix;
        }

        int GetHypotenuse(int x, int y) {
            return MapTools.IntegerSqrt(x * x + y * y);
        }

        /// <summary>
        /// 判断是否在这块区域内
        /// </summary>
        /// <param name="pos">需要判断的坐标</param>
        /// <returns>是否在这块区域内</returns>
        public bool IsTherein(Vector3 pos, int edge = 0) {
            if (pos.x > (min_x - edge) && pos.x < (max_x + edge) && pos.z > (min_z - edge) && pos.z < (max_z + edge)) {
                return true;
            }
            return false;
        }
    }
}