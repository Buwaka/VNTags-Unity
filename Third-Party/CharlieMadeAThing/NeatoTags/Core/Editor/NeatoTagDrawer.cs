using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CharlieMadeAThing.NeatoTags.Core.Editor
{
    /// <summary>
    ///     The drawer for tags. This is mainly used when a tag is clicked in the project or when displaying a tag in the tag
    ///     manager.
    /// </summary>
    [CustomEditor(typeof(NeatoTag))]
    public class NeatoTagDrawer : UnityEditor.Editor
    {
        private const           string             TagButtonBoxName = "tagButtonBox";
        private const           string             TagColorName     = "tagColor";
        private const           string             CommentFieldName = "commentField";
        private static readonly List<TaggerDrawer> s_taggerDrawers  = new();
        private                 Button             _button;
        private                 ColorField         _colorField;
        private                 TextField          _commentField;
        private                 NeatoTag           _neatoTag;

        private VisualElement      _root;
        private VisualElement      _tagButtonBox;
        private VisualTreeAsset    _tagButtonTemplate;
        private SerializedProperty PropertyColor   { get; set; }
        private SerializedProperty PropertyComment { get; set; }


        private void OnEnable()
        {
            _root = new VisualElement();

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlDataLookup.NeatoTagUxml);
            visualTree.CloneTree(_root);
            if (!visualTree)
            {
                Debug.LogError("Failed to load tag UXML template.");
                return;
            }

            _tagButtonBox = _root.Q<VisualElement>(TagButtonBoxName);
            _tagButtonTemplate =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlDataLookup.ButtonTagUxml);
            NeatoTagAssetModificationProcessor.RegisterNeatoTagDrawer(this);
        }

        private void OnDisable()
        {
            NeatoTagAssetModificationProcessor.UnregisterNeatoTagDrawer(this);
            _colorField.UnregisterValueChangedCallback(UpdateTagIconVisual);
        }

        public override VisualElement CreateInspectorGUI()
        {
            _neatoTag = target as NeatoTag;
            FindProperties();
            _colorField = _root.Q<ColorField>(TagColorName);
            _colorField.BindProperty(PropertyColor);
            _colorField.RegisterValueChangedCallback(UpdateTagIconVisual);
            _colorField.showAlpha    = false;
            PropertyColor.colorValue = _colorField.value;

            _button                       = _tagButtonTemplate.Instantiate().Q<Button>();
            _button.text                  = target.name;
            _button.style.backgroundColor = PropertyColor.colorValue;
            _button.name                  = "tagIcon";
            _tagButtonBox.Add(_button);

            _commentField = _root.Q<TextField>(CommentFieldName);
            _commentField.BindProperty(PropertyComment);

            return _root;
        }


        private void FindProperties()
        {
            PropertyColor   = serializedObject.FindProperty("color");
            PropertyComment = serializedObject.FindProperty("comment");
        }

        public void UpdateTagButtonText()
        {
            if (_neatoTag != null && _button != null)
            {
                _button.text = _neatoTag.name;
            }
        }


        private void UpdateTagIconVisual(ChangeEvent<Color> evt)
        {
            PropertyColor.colorValue      = evt.newValue;
            _button.style.backgroundColor = PropertyColor.colorValue;
            _button.style.color           = TaggerDrawer.GetTextColorBasedOnBackground(PropertyColor.colorValue);
            foreach (TaggerDrawer taggerDrawer in s_taggerDrawers)
            {
                if (!taggerDrawer) continue;
                taggerDrawer.PopulateButtons();
            }
        }

        public static void RegisterTaggerDrawer(TaggerDrawer taggerDrawer)
        {
            if (s_taggerDrawers.Contains(taggerDrawer))
            {
                return;
            }

            s_taggerDrawers.Add(taggerDrawer);
        }
    }
}