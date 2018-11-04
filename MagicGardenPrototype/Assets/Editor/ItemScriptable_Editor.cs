using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor (typeof(Item))]
[CanEditMultipleObjects]
public class ItemScriptable_Editor : Editor {

    SerializedObject targetItem;
    SerializedProperty icon;

    private void OnEnable()
    {
        targetItem = new SerializedObject(target);
        icon = targetItem.FindProperty("referenceIcon");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        // https://answers.unity.com/questions/626332/type-casting-serializedpropertyobjectreferencevalu.html

        Texture tempIcon = icon.objectReferenceValue as Texture;
        
        if (tempIcon != null)
            EditorGUI.DrawTextureTransparent(new Rect(new Vector2(20, 50), new Vector2(128,128)), tempIcon);

        DrawDefaultInspector();
    }
}
