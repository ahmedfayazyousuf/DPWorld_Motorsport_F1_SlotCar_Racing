using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TheHangingHouse.Utility.Extensions;
using UnityEngine.EventSystems;

public class HelperFunctions
{
    [MenuItem("THH/Clone Material/Renderer")]
    public static void CloneSelectedRendererMaterial()
    {
        var selectedObjects = Selection.gameObjects;
        foreach(var selectedObject in selectedObjects)
        {
            var renderer = selectedObject.GetComponent<Renderer>();
            if (renderer != null && renderer.sharedMaterial != null)
            {
                Undo.RecordObject(renderer, $"Material Clone {renderer.name}");
                renderer.sharedMaterial = new Material(renderer.sharedMaterial);
                renderer.sharedMaterial.name = $"{renderer.sharedMaterial.name} ({selectedObject.name})";
            }
        }
    }

    [MenuItem("THH/Clone Material/Image")]
    public static void CloneSelectedImageMaterial()
    {
        var selectedImages = Selection.gameObjects;
        foreach (var selectedImage in selectedImages)
        {
            var image = selectedImage.GetComponent<Image>();
            if (image != null && image.material != null)
            {
                Undo.RecordObject(image, $"Material Clone {image.name}");
                image.material = new Material(image.material);
                image.material.name = $"{image.material.name} ({selectedImage.name})";
            }
        }
    }
}
