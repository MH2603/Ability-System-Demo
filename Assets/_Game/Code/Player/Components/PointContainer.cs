using Foundation.EntitySystem;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MH
{
    public static class PointNames
    {
        public const string FirePoint = "FirePoint";
    }

    [Serializable]
    public class PointInfo
    {
        [field: SerializeField] public string PointName { get; private set; } 
        [field: SerializeField] public Transform Point { get; private set; } 

        public Vector3 GetPos()
        {
            return Point.position;  
        }

        public Vector3 GetDirection()
        {
            return Vector3.forward; 
        }
    }
    public class PointContainer : EntityComponent
    {
        [SerializeField] private PointInfo[] points;

        private Dictionary<string, PointInfo> pointMap;

        public override void ManualStart()
        {
            base.ManualStart();
            SetUpPointMap();

        }

        public PointInfo GetPoint(string namePoint)
        {
            if (!pointMap.ContainsKey(namePoint))
            {
                Debug.Log($" BUG Point : Not find point with name = {namePoint}");
                return null;
            }

            return pointMap[namePoint];  
        }


        private void SetUpPointMap()
        {
            pointMap = new();

            foreach (var point in points)
            {
                pointMap[point.PointName] = point;
            }
        }

    }

}
