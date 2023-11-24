using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SerializableColor
{
    public float r, g, b, a;

    public SerializableColor(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public static implicit operator Color(SerializableColor serializableColor) => new Color(serializableColor.r, serializableColor.g, serializableColor.b, serializableColor.a);
    public static implicit operator SerializableColor(Color color) => new SerializableColor(color.r, color.g, color.b, color.a);
}
