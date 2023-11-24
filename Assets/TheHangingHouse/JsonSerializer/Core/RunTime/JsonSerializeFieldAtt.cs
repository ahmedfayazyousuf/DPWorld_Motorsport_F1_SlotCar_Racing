using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheHangingHouse.JsonSerializer
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public sealed class JsonSerializeField : System.Attribute
    {
        public string DataName { get; set; } = BehaviourJsonSerializer.DEFAULT_DATA_NAME;
    }
}
