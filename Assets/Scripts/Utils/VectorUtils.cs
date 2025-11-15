using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JetEngine
{
    public static class VectorUtils
    {
        # region Vector3 -> Vector2 conversions

        //Creates a new Vector2 with this.x as the new Vector's x and this.y
        //  as the new Vector's y
        public static Vector2 GetXY(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        //Creates a new Vector2 with this.x as the new Vector's x and this.z
        //  as the new Vector's y
        public static Vector2 GetXZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        //Creates a new Vector2 with this.y as the new Vector's x and this.z
        //  as the new Vector's y
        public static Vector2 GetYZ(this Vector3 v)
        {
            return new Vector2(v.y, v.z);
        }

        #endregion

        #region Vector2 -> Vector3 conversions

        //Creates a new Vector3 with this.x as the new Vector's x and this.y
        //  as the new Vector's y. Sets the new Vector's z to 0f
        public static Vector3 ToVec3XY(this Vector2 v)
        {
            return new Vector3(v.x, v.y, 0f);
        }

        //Creates a new Vector3 with this.x as the new Vector's x and this.y
        //  as the new Vector's z. Sets the new Vector's y to 0f
        public static Vector3 ToVec3XZ(this Vector2 v)
        {
            return new Vector3(v.x, 0f, v.y);
        }

        //Creates a new Vector3 with this.x as the new Vector's y and this.y
        //  as the new Vector's z. Sets the new Vector's x to 0f
        public static Vector3 ToVec3YZ(this Vector2 v)
        {
            return new Vector3(0f, v.x, v.y);
        }

        #endregion

        #region VectorX to VectorXInt conversions
        public static Vector2Int ToVector2Int(this Vector2 v)
        {
            return new Vector2Int((int)v.x, (int)v.y);
        }

        public static Vector3Int ToVector3Int(this Vector3 v)
        {
            return new Vector3Int((int)v.x, (int)v.y, (int)v.z);
        }
        #endregion

        //Unity does not like changing just one component on transform vectors, so these handle making the copy and updating just the specified field
        #region Vector2 Update Functions
        //Result: new Vector2(_newX, v.y)
        public static Vector2 NewX(this Vector2 v, float _newX)
        {
            return new Vector2(_newX, v.y);
        }

        //Result: new Vector2(v.x + _addX, v.y)
        public static Vector2 AddX(this Vector2 v, float _addX)
        {
            return new Vector2(v.x + _addX, v.y);
        }

        //Result: new Vector2(v.x - _subX, v.y)
        public static Vector2 SubX(this Vector2 v, float _subX)
        {
            return new Vector2(v.x - _subX, v.y);
        }

        //Result: new Vector2(v.x * _mulX, v.y)
        public static Vector2 MulX(this Vector2 v, float _mulX)
        {
            return new Vector2(v.x * _mulX, v.y);
        }

        //Result: new Vector2(v.x / _divX, v.y)
        public static Vector2 DivX(this Vector2 v, float _divX)
        {
            return new Vector2(v.x / _divX, v.y);
        }

        //Result: new Vector2(v.x, _newY)
        public static Vector2 NewY(this Vector2 v, float _newY)
        {
            return new Vector2(v.x, _newY);
        }

        //Result: new Vector2(v.x, v.y + _addY)
        public static Vector2 AddY(this Vector2 v, float _addY)
        {
            return new Vector2(v.x, v.y + _addY);
        }

        //Result: new Vector2(v.x, v.y - _subY)
        public static Vector2 SubY(this Vector2 v, float _subY)
        {
            return new Vector2(v.x, v.y - _subY);
        }

        //Result: new Vector2(v.x, v.y * _mulY)
        public static Vector2 MulY(this Vector2 v, float _mulY)
        {
            return new Vector2(v.x, v.y * _mulY);
        }

        //Result: new Vector2(v.x, v.y / _divY)
        public static Vector2 DivY(this Vector2 v, float _divY)
        {
            return new Vector2(v.x, v.y / _divY);
        }
        #endregion

        //Unity does not like changing just one or two components on transform vectors, so these handle making the copy and updating just the specified field
        #region Vector3 Update Functions
        #region Single Component Updates
        //Result: new Vector3(_newX, v.y, v.z)
        public static Vector3 NewX(this Vector3 v, float _newX)
        {
            return new Vector3(_newX, v.y, v.z);
        }

        //Result: new Vector3(v.x + _addX, v.y, v.z)
        public static Vector3 AddX(this Vector3 v, float _addX)
        {
            return new Vector3(v.x + _addX, v.y, v.z);
        }

        //Result: new Vector3(v.x - _subX, v.y, v.z)
        public static Vector3 SubX(this Vector3 v, float _subX)
        {
            return new Vector3(v.x - _subX, v.y, v.z);
        }

        //Result: new Vector3(v.x * _mulX, v.y, v.z)
        public static Vector3 MulX(this Vector3 v, float _mulX)
        {
            return new Vector3(v.x * _mulX, v.y, v.z);
        }

        //Result: new Vector3(v.x / _divX, v.y, v.z)
        public static Vector3 DivX(this Vector3 v, float _divX)
        {
            return new Vector3(v.x / _divX, v.y, v.z);
        }

        //Result: new Vector3(v.x, _newY, v.z)
        public static Vector3 NewY(this Vector3 v, float _newY)
        {
            return new Vector3(v.x, _newY, v.z);
        }

        //Result: new Vector3(v.x, v.y + _addY, v.z)
        public static Vector3 AddY(this Vector3 v, float _addY)
        {
            return new Vector3(v.x, v.y + _addY, v.z);
        }

        //Result: new Vector3(v.x, v.y - _subY, v.z)
        public static Vector3 SubY(this Vector3 v, float _subY)
        {
            return new Vector3(v.x, v.y - _subY, v.z);
        }

        //Result: new Vector3(v.x, v.y * _mulY, v.z)
        public static Vector3 MulY(this Vector3 v, float _mulY)
        {
            return new Vector3(v.x, v.y * _mulY, v.z);
        }

        //Result: new Vector3(v.x, v.y / _divY, v.z)
        public static Vector3 DivY(this Vector3 v, float _divY)
        {
            return new Vector3(v.x, v.y / _divY, v.z);
        }

        //Result: new Vector3(v.x, v.y, _newZ)
        public static Vector3 NewZ(this Vector3 v, float _newZ)
        {
            return new Vector3(v.x, v.y, _newZ);
        }

        //Result: new Vector3(v.x, v.y, v.z + _addZ)
        public static Vector3 AddZ(this Vector3 v, float _addZ)
        {
            return new Vector3(v.x, v.y, v.z + _addZ);
        }

        //Result: new Vector3(v.x, v.y, v.z - _subZ)
        public static Vector3 SubZ(this Vector3 v, float _subZ)
        {
            return new Vector3(v.x, v.y, v.z - _subZ);
        }

        //Result: new Vector3(v.x, v.y, v.z * _mulZ)
        public static Vector3 MulZ(this Vector3 v, float _mulZ)
        {
            return new Vector3(v.x, v.y, v.z * _mulZ);
        }

        //Result: new Vector3(v.x, v.y, v.z / _divZ)
        public static Vector3 DivZ(this Vector3 v, float _divZ)
        {
            return new Vector3(v.x, v.y, v.z / _divZ);
        }
        #endregion

        #region Double Component Updates
        //Result: new Vector3(_newX, _newY, v.z)
        public static Vector3 NewXY(this Vector3 v, float _newX, float _newY)
        {
            return new Vector3(_newX, _newY, v.z);
        }

        //Result: new Vector3(_newV.x, _newV.y, v.z)
        public static Vector3 NewXY(this Vector3 v, Vector2 _newV)
        {
            return new Vector3(_newV.x, _newV.y, v.z);
        }

        //Result: new Vector3(v.x + _addX, v.y + _addY, v.z)
        public static Vector3 AddXY(this Vector3 v, float _addX, float _addY)
        {
            return new Vector3(v.x + _addX, v.y + _addY, v.z);
        }

        //Result: new Vector3(v.x - _subX, v.y - _subY, v.z)
        public static Vector3 SubXY(this Vector3 v, float _subX, float _subY)
        {
            return new Vector3(v.x - _subX, v.y - _subY, v.z);
        }

        //Result: new Vector3(v.x * _mulX, v.y * _mulY, v.z)
        public static Vector3 MulXY(this Vector3 v, float _mulX, float _mulY)
        {
            return new Vector3(v.x * _mulX, v.y * _mulY, v.z);
        }

        //Result: new Vector3(v.x / _divX, v.y / _divY, v.z)
        public static Vector3 DivXY(this Vector3 v, float _divX, float _divY)
        {
            return new Vector3(v.x / _divX, v.y / _divY, v.z);
        }

        //Result: new Vector3(_newX, v.y, _newZ)
        public static Vector3 NewXZ(this Vector3 v, float _newX, float _newZ)
        {
            return new Vector3(_newX, v.y, _newZ);
        }

        //Result: new Vector3(v.x + _addX, v.y, v.z + _addZ)
        public static Vector3 AddXZ(this Vector3 v, float _addX, float _addZ)
        {
            return new Vector3(v.x + _addX, v.y, v.z + _addZ);
        }

        //Result: new Vector3(v.x - _subX, v.y, v.z - _subZ)
        public static Vector3 SubXZ(this Vector3 v, float _subX, float _subZ)
        {
            return new Vector3(v.x - _subX, v.y, v.z - _subZ);
        }

        //Result: new Vector3(v.x * _mulX, v.y, v.z * _mulZ)
        public static Vector3 MulXZ(this Vector3 v, float _mulX, float _mulZ)
        {
            return new Vector3(v.x * _mulX, v.y, v.z * _mulZ);
        }

        //Result: new Vector3(v.x / _divX, v.y, v.z / _divZ)
        public static Vector3 DivXZ(this Vector3 v, float _divX, float _divZ)
        {
            return new Vector3(v.x / _divX, v.y, v.z / _divZ);
        }

        //Result: new Vector3(v.x, _newY, _newZ)
        public static Vector3 NewYZ(this Vector3 v, float _newY, float _newZ)
        {
            return new Vector3(v.x, _newY, _newZ);
        }

        //Result: new Vector3(v.x, v.y + _addY, v.z + _addZ)
        public static Vector3 AddYZ(this Vector3 v, float _addY, float _addZ)
        {
            return new Vector3(v.x, v.y + _addY, v.z + _addZ);
        }

        //Result: new Vector3(v.x, v.y - _subY, v.z - _subZ)
        public static Vector3 SubYZ(this Vector3 v, float _subY, float _subZ)
        {
            return new Vector3(v.x, v.y - _subY, v.z - _subZ);
        }

        //Result: new Vector3(v.x, v.y * _mulY, v.z * _mulZ)
        public static Vector3 MulYZ(this Vector3 v, float _mulY, float _mulZ)
        {
            return new Vector3(v.x, v.y * _mulY, v.z * _mulZ);
        }

        //Result: new Vector3(v.x, v.y / _divY, v.z / _divZ)
        public static Vector3 DivYZ(this Vector3 v, float _divY, float _divZ)
        {
            return new Vector3(v.x, v.y / _divY, v.z / _divZ);
        }
        #endregion
        #endregion
    }
}