using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheHangingHouse.JsonSerializer
{
    public class MonoBehaviourID : MonoBehaviour
    {
        public string id;

        protected void Awake()
        {

        }

        [ContextMenu("Generate Random Id")]
        public void GenerateID()
        {
            id = System.Guid.NewGuid().ToString();
        }

        protected void OnValidate()
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                id = System.Guid.NewGuid().ToString();
        }
    }
}
