using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VNTags.Utility
{
    public static class SubAssetExtension
    {
        /// <summary>
        ///     About Object serialization
        ///     Reference types need to inherit from MonoBehavior or ScriptableObject,
        ///     This makes it possible to save instances of the class as assets.
        ///     This is essential because otherwise reference objects won't be saved or serialized.
        ///     [Serializable] classes will be fine since they act more like glorified structs.
        /// </summary>
        /// the purpose of these functions is to create and manage custom editor/property fields that generate assets as needed.
        /// This is achieved by creating sub-assets and attaching them to the parent object,
        /// this sub-object will then be referenced as the value of the field.
        /// These object references work using Unity's GUID system
        /// 
        /// Text serialized asset example:
        /*
         *  Transitions:
          - name: Time
            alias: ...
            prefab: ...
            Tags: {fileID: 8697949849457358854}

            ...

         * --- !u!114 &8697949849457358854
            MonoBehaviour:
            ...
              m_Script: {fileID: 11500000, guid: e1aadd7d3e380094eb6f9b8e09d7ba16, type: 3}
              m_Name: 'Character: 3146671180'
         */
        private static SubAssetIndex GetIndex(SerializedProperty property)
        {
            if (property == null)
            {
                Debug.LogError("SubAssetExtension: UpdateSubAsset: property is null");
                return null;
            }

            string path = AssetDatabase.GetAssetPath(property.serializedObject.targetObject);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("SubAssetExtension: GetIndex: property has no valid path, " + property);
                return null;
            }

            var allAssets = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (Object asset in allAssets)
            {
                if (AssetDatabase.IsSubAsset(asset) && asset is SubAssetIndex index)
                {
                    return index;
                }
            }

            return null;
        }

        private static bool HasIndex(SerializedProperty property)
        {
            return GetIndex(property) != null;
        }

        /// <summary>
        ///     Retrieves SubAsset based on an Identifier
        /// </summary>
        /// <param name="property">the main asset to add the sub-asset ti</param>
        /// <typeparam name="T">the expected type of the subAsset</typeparam>
        public static T GetSubAsset<T>(SerializedProperty property)
            where T : Object
        {
            SubAssetIndex index = GetIndex(property);

            if (index == null || index.Entries == null)
            {
                return null;
            }

            foreach ((string path, Object reference) entry in index.Entries)
            {
                if (entry.path.Equals(property.propertyPath))
                {
                    return (T)entry.reference;
                }
            }

            return null;
        }

        /// <summary>
        ///     Updates a sub-asset, creates it if necessary
        /// </summary>
        /// <param name="property">the main asset to add the sub-asset ti</param>
        /// <param name="newValue">the new value of the sub-asset</param>
        /// <returns>true if success, false if fail</returns>
        public static void UpdateSubAsset(SerializedProperty property, Object newValue)
        {
            var  asset = GetSubAsset<Object>(property);
            bool found = asset != null;

            if (!found)
            {
                SubAssetIndex index = GetIndex(property);
                if (index == null)
                {
                    index = ScriptableObject.CreateInstance<SubAssetIndex>();
                    AssetDatabase.AddObjectToAsset(index, property.serializedObject.targetObject);
                }

                AssetDatabase.AddObjectToAsset(newValue, property.serializedObject.targetObject);
                index.AddEntry(property.propertyPath, newValue);
            }

            property.objectReferenceValue = newValue;
            property.serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssetIfDirty(property.serializedObject.targetObject);
        }
    }
}