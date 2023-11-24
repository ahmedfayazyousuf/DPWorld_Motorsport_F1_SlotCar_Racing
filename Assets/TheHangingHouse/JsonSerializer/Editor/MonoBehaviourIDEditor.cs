using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TheHangingHouse.JsonSerializer;
using TheHangingHouse.Utility.Extensions;

[CustomEditor(typeof(MonoBehaviourID), true)]
[CanEditMultipleObjects]
public class MonoBehaviourIDEditor : Editor
{
    private MonoBehaviourID monoBehaviourID;

    private void OnEnable()
    {
        monoBehaviourID = (MonoBehaviourID)target;

        if (string.IsNullOrEmpty(monoBehaviourID.id) ||
            string.IsNullOrWhiteSpace(monoBehaviourID.id))
            monoBehaviourID.id = System.Guid.NewGuid().ToString();

        var sameId = Resources.FindObjectsOfTypeAll<MonoBehaviourID>().Filter(g => g.id.Equals(monoBehaviourID.id));
        if (sameId.Length > 1)
            for (var i = 0; i < sameId.Length; i++)
                Debug.LogWarning($"{{ {sameId[i].name} }} Has repeated id with another elemnet!");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (string.IsNullOrEmpty(monoBehaviourID.id))
            monoBehaviourID.id = System.Guid.NewGuid().ToString();
    }
}
