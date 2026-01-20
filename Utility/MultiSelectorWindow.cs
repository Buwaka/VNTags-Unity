using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VNTags.Utility
{
#if UNITY_EDITOR
    /// <summary>
    ///     this is AI generated and doesn't actually work fully,
    /// </summary>
    public class MultiSelectorWindow : EditorWindow
    {
        private Type               _baseType;
        private List<Type>         _foundTypes;
        private Vector2            _scrollPosition;
        private List<bool>         _selectionStates;
        private SerializedProperty _typesProperty;

        private void OnDestroy()
        {
            if (_typesProperty != null)
            {
                ApplySelection();
            }
        }

        private void OnGUI()
        {
            if (_foundTypes == null || _baseType == null)
            {
                EditorGUILayout.HelpBox("Window not initialized. Please open it from a valid property drawer.", MessageType.Error);
                return;
            }

            EditorGUILayout.LabelField($"Select types inheriting from {_baseType.Name}", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            for (int i = 0; i < _foundTypes.Count; i++)
            {
                _selectionStates[i] = EditorGUILayout.ToggleLeft(new GUIContent(_foundTypes[i].Name), _selectionStates[i]);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
            if (GUILayout.Button("Apply Selection"))
            {
                ApplySelection();
                Close();
            }
        }

        public static void OpenWindow(SerializedProperty typesProperty, Type baseType)
        {
            var window = GetWindow<MultiSelectorWindow>("Select Types");
            window.Initialize(typesProperty, baseType);
        }

        private void Initialize(SerializedProperty typesProperty, Type baseType)
        {
            _typesProperty = typesProperty;
            _baseType      = baseType;
            _foundTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && _baseType.IsAssignableFrom(type))
                .ToList();

            // Initialize selection states based on current property values
            var currentTypes = ((IEnumerable<Type>)typesProperty.managedReferenceValue).ToList();
            _selectionStates = _foundTypes.Select(type => currentTypes.Contains(type)).ToList();
        }

        private void ApplySelection()
        {
            var selectedTypes = _foundTypes.Where((t, i) => _selectionStates[i]).ToArray();
            _typesProperty.managedReferenceValue = selectedTypes;
            _typesProperty.serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}