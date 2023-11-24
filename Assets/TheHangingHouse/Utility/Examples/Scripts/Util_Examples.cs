using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheHangingHouse.Utility;

namespace TheHangingHouse.Utility.Examples
{
    public class Util_Examples : MonoBehaviour
    {
        void Start()
        {
            string txt = "5";
            var x = Util.Parse<int>(txt);
            var y = Util.Parse(txt, typeof(int));
            Debug.Log($"x = {x}, y = {y}");
            Debug.Log($"typeof(x) is {x.GetType()}, typeof(y) is {y.GetType()}");
        }
    }
}
