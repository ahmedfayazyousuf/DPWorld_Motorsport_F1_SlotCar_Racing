using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheHangingHouse.JsonSerializer;

namespace TheHangingHouse.JsonSerializer.Example
{
    public class ArraySerializingExample : MonoBehaviourID
    {
        [JsonSerializeField] public float[] numbers;
        [JsonSerializeField] public Person[] persons;
        [JsonSerializeField] public List<bool> bools;
        [JsonSerializeField] public float gameDuration;
    }

    [System.Serializable]
    public class Person
    {
        public string name;
        public float age;
    }
}