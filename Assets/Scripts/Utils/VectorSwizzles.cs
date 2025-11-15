using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JetEngine
{
    public static class VectorSwizzles
    {
        #region Vector2 -> Vector2 Swizzles
        public static Vector2 SwizzleXX(this Vector2 v)
        {
            return new Vector2(v.x, v.x);
        }

        //No change; effectively deep copies v
        public static Vector2 SwizzleXY(this Vector2 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2 SwizzleYX(this Vector2 v)
        {
            return new Vector2(v.y, v.x);
        }

        public static Vector2 SwizzleYY(this Vector2 v)
        {
            return new Vector2(v.y, v.y);
        }
        #endregion

        #region Vector3 -> Vector2 Swizzles
        public static Vector2 SwizzleXX(this Vector3 v)
        {
            return new Vector2(v.x, v.x);
        }

        public static Vector2 SwizzleXY(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2 SwizzleXZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }
        public static Vector2 SwizzleYY(this Vector3 v)
        {
            return new Vector2(v.y, v.y);
        }

        public static Vector2 SwizzleYX(this Vector3 v)
        {
            return new Vector2(v.y, v.x);
        }

        public static Vector2 SwizzleYZ(this Vector3 v)
        {
            return new Vector2(v.y, v.z);
        }

        public static Vector2 SwizzleZZ(this Vector3 v)
        {
            return new Vector2(v.z, v.z);
        }

        public static Vector2 SwizzleZX(this Vector3 v)
        {
            return new Vector2(v.z, v.x);
        }

        public static Vector2 SwizzleZY(this Vector3 v)
        {
            return new Vector2(v.z, v.y);
        }

        #endregion
    }
}