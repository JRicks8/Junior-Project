using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UIElements;
using static Interactable;

[CustomEditor(typeof(Interactable))]
public class InteractableObjectEditorDisplay : Editor
{
    Interactable script;

    SerializedProperty method;
    SerializedProperty interactionType;
    // Dialogue
    SerializedProperty dialogueText;
    SerializedProperty textBox;
    SerializedProperty textLocation;
    SerializedProperty textRotation;
    // Spawn Object
    SerializedProperty objectToSpawn;
    SerializedProperty locationToSpawn;
    // Move Object
    SerializedProperty moveType;
    SerializedProperty objectToMove;
    SerializedProperty origin;
    SerializedProperty destination;
    SerializedProperty moveTime;
    // Destroy Object
    SerializedProperty objectToDestroy;

    void OnEnable()
    {
        script = (Interactable)target;
        method = serializedObject.FindProperty("method");
        interactionType = serializedObject.FindProperty("interactionType");
        // Dialogue
        dialogueText = serializedObject.FindProperty("dialogueText");
        textBox = serializedObject.FindProperty("textBox");
        textLocation = serializedObject.FindProperty("textLocation");
        textRotation = serializedObject.FindProperty("textRotation");
        // Spawn Object
        objectToSpawn = serializedObject.FindProperty("objectToSpawn");
        locationToSpawn = serializedObject.FindProperty("locationToSpawn");
        // Move Object
        moveType = serializedObject.FindProperty("moveType");
        objectToMove = serializedObject.FindProperty("objectToMove");
        origin = serializedObject.FindProperty("origin");
        destination = serializedObject.FindProperty("destination");
        moveTime = serializedObject.FindProperty("moveTime");
        // Destroy Object
        objectToDestroy = serializedObject.FindProperty("objectToDestroy");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(method);
        EditorGUILayout.PropertyField(interactionType);

        if (script.GetInteractionType() == OnInteractType.Dialogue)
        {
            EditorGUILayout.PropertyField(dialogueText);
            EditorGUILayout.PropertyField(textBox);
            EditorGUILayout.PropertyField(textLocation);
            EditorGUILayout.PropertyField(textRotation);
        }
        else if (script.GetInteractionType() == OnInteractType.SpawnObject)
        {
            EditorGUILayout.PropertyField(objectToSpawn);
            EditorGUILayout.PropertyField(locationToSpawn);
        }
        else if (script.GetInteractionType() == OnInteractType.MoveObject)
        {
            EditorGUILayout.PropertyField(moveType);
            EditorGUILayout.PropertyField(objectToMove);
            EditorGUILayout.PropertyField(origin);
            EditorGUILayout.PropertyField(destination);
            EditorGUILayout.PropertyField(moveTime);
        }
        else if (script.GetInteractionType() == OnInteractType.DestroyObject)
        {
            EditorGUILayout.PropertyField(objectToDestroy);
        }

        serializedObject.ApplyModifiedProperties();
    }
}