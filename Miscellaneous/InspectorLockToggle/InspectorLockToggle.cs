using System;
using System.Reflection;
using UnityEditor;

public static class InspectorLockToggle
{
    [MenuItem("Leeboro/Tools/Toggle Lock &q")]
    static void ToggleWindowLock() // Inspector must be inspecting something to be locked
    {
        EditorWindow windowToBeLocked = EditorWindow.mouseOverWindow; // "EditorWindow.focusedWindow" can be used instead

        if (windowToBeLocked != null && windowToBeLocked.GetType().Name == "InspectorWindow")
        {
            Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow");
            PropertyInfo propertyInfo = type.GetProperty("isLocked");
            bool value = (bool)propertyInfo.GetValue(windowToBeLocked, null);
            propertyInfo.SetValue(windowToBeLocked, !value, null);
            windowToBeLocked.Repaint();
        }
        else if (windowToBeLocked != null && windowToBeLocked.GetType().Name == "ProjectBrowser")
        {
            Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.ProjectBrowser");
            PropertyInfo propertyInfo = type.GetProperty("isLocked", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            bool value = (bool)propertyInfo.GetValue(windowToBeLocked, null);
            propertyInfo.SetValue(windowToBeLocked, !value, null);
            windowToBeLocked.Repaint();
        }
        else if (windowToBeLocked != null && windowToBeLocked.GetType().Name == "SceneHierarchyWindow")
        {
            Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.SceneHierarchyWindow");

            FieldInfo fieldInfo = type.GetField("m_SceneHierarchy", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo propertyInfo = fieldInfo.FieldType.GetProperty("isLocked",
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            object value = fieldInfo.GetValue(windowToBeLocked);
            bool value2 = (bool)propertyInfo.GetValue(value);
            propertyInfo.SetValue(value, !value2, null);
            windowToBeLocked.Repaint();
        }
    }
}
