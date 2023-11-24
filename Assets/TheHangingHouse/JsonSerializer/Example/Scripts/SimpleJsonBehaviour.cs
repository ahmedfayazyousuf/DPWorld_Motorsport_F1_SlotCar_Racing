using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheHangingHouse.JsonSerializer;

namespace TheHangingHouse.JsonSerializer.Example
{
    public class SimpleJsonBehaviour : MonoBehaviourID
    {
        [JsonSerializeField] public string playerName;
        [JsonSerializeField] public Point point;

        private new void Awake()
        {
            base.Awake();

            Debug.LogFormat("Player Name: {0}", playerName);
            Debug.LogFormat("Point: {0}", point);
        }
    }

    [System.Serializable]
    public class Point
    {
        public float x;
        public float y;

        public override string ToString()
        {
            return $"x: {x}, y: {y}";
        }
    }
}
