using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(SFX2DSource))]
[CanEditMultipleObjects]
public class SFX2DSourceEditor : Editor
{
    private AudioManager audioManagerRef; 
    private string[] soundNames;
    private int selectedIndex = 0;

    private void OnEnable()
    {
        FindAudioManagerReference();
        UpdateSoundList();
    }

    public override void OnInspectorGUI()
    {
        SFX2DSource script = (SFX2DSource)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Настройки Звука", EditorStyles.boldLabel);

        if (soundNames != null && soundNames.Length > 0)
        {
            selectedIndex = System.Array.IndexOf(soundNames, script.soundName);
            
            if (selectedIndex < 0) 
            {
                selectedIndex = 0;
                if (!string.IsNullOrEmpty(script.soundName))
                {
                    EditorGUILayout.HelpBox($"Звук '{script.soundName}' не найден в списке! Выберите новый.", MessageType.Error);
                }
            }

            int newIndex = EditorGUILayout.Popup("Выберите звук", selectedIndex, soundNames);
            
            if (newIndex >= 0 && newIndex < soundNames.Length)
            {
                script.soundName = soundNames[newIndex];
                selectedIndex = newIndex;
            }

            if (audioManagerRef != null)
            {
                bool isSceneObject = !EditorUtility.IsPersistent(audioManagerRef.gameObject);
                string sourceStatus = isSceneObject ? "Со сцены" : "Из префаба";
                EditorGUILayout.HelpBox($"Список загружен: {sourceStatus}", MessageType.None);
            }
        }
        else
        {
            script.soundName = EditorGUILayout.TextField("Название звука (Manual)", script.soundName);
            
            EditorGUILayout.HelpBox("AudioManager не найден ни на сцене, ни в префабах! \n" +
                                    "Убедитесь, что у вас есть префаб с именем 'AudioManager'.", MessageType.Warning);
            
            if (GUILayout.Button("Попробовать найти снова"))
            {
                 FindAudioManagerReference();
                 UpdateSoundList();
            }
        }

        EditorGUILayout.Space();
        
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "m_Script", "soundName");
        serializedObject.ApplyModifiedProperties();
    }

    private void FindAudioManagerReference()
    {
        audioManagerRef = GameObject.FindFirstObjectByType<AudioManager>();

        if (audioManagerRef == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab AudioManager"); 
            
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                audioManagerRef = AssetDatabase.LoadAssetAtPath<AudioManager>(path);
            }
        }
    }

    private void UpdateSoundList()
    {
        if (audioManagerRef != null && audioManagerRef.sounds != null)
        {
            soundNames = audioManagerRef.sounds.Select(s => s.name).ToArray();
        }
        else
        {
            soundNames = null;
        }
    }
}