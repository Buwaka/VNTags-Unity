using System;
using System.Collections.Generic;
using UnityEngine;
using VNTags;
using VNTags.Components;
using VNTags.ScriptAnimations;
using VNTags.Utility;

[Serializable]
public class VNPosition
{
    public TransformData      Transform;
    public SortingLayerPicker Layer;
}

[Serializable]
public class VNNamedPosition : VNPosition
{
    public string Name;
}

public delegate bool CharacterAddedHandler(VNCharacterComponent character, GameObject obj, VNPosition position);

public class VNCharacterController : MonoBehaviour
{
    private static VNCharacterController _instance;

    public float             GlobablSpriteScale = 0.4f;
    public VNPosition[]      OrderedPositions;
    public VNNamedPosition[] NamedPositions;

    private readonly Dictionary<VNCharacterData, VNCharacterComponent> _characters = new();


    private readonly Dictionary<VNPosition, VNCharacterComponent> _scenePositionComposition = new();

    private CharacterAddedHandler _onCharacterAdded;

    // events
    public static CharacterAddedHandler onCharacterAdded
    {
        get { return _instance._onCharacterAdded; }
    }

    private void Start()
    {
        _instance = this;
    }

    public static bool IsCharacterActive(VNCharacterData character)
    {
        foreach (VNCharacterComponent characterVal in _instance._scenePositionComposition.Values)
        {
            if (characterVal.CharacterData == character && characterVal.isVisible)
            {
                return true;
            }
        }

        return false;
    }

    public static VNCharacterComponent GetCharacter(VNCharacterData character)
    {
        return _instance._characters.GetValueOrDefault(character, null);
    }

    public static void HideCharacter(VNCharacterData character, bool instant = false)
    {
        if (GetCharacter(character) is VNCharacterComponent charaComp)
        {
            if (instant)
            {
                charaComp.Hide();
            }
            else
            {
                ScriptAnimation defaultAnimation  = VNTagsConfig.GetConfig().DefaultExitAnimation;
                ScriptAnimation animationInstance = null;
                if (defaultAnimation != null)
                {
                    animationInstance = (ScriptAnimation)ScriptableObject.CreateInstance(defaultAnimation.GetType());
                }
                else
                {
                    Debug.LogWarning("VNCharacterPositionController: HideCharacter: Default exit animation not found");
                }
                charaComp.Hide(animationInstance);
            }

            charaComp.Reset();

            foreach (var pair in _instance._scenePositionComposition)
            {
                if (pair.Value.CharacterData == character)
                {
                    _instance._scenePositionComposition.Remove(pair.Key);
                    break;
                }
            }
        }
    }

    public static void HideAllCharacters(bool instant = false)
    {
        foreach (var character in _instance._characters)
        {
            HideCharacter(character.Key, instant);
        }
    }


    public static void ShowCharacter(VNCharacterData character, out GameObject obj, VNPosition position = null, bool instant = false)
    {
        if (character == null)
        {
            Debug.LogError("VNCharacterPositionController: ShowCharacter: Character is null, exiting");
            obj = null;
            return;
        }

        if (IsCharacterActive(character))
        {
            Debug.Log("VNCharacterPositionController: ShowCharacter: Character is already active, exiting");
            obj = null;
            return;
        }

        if (position == null)
        {
            foreach (VNPosition ordered in _instance.OrderedPositions)
            {
                if (!_instance._scenePositionComposition.ContainsKey(ordered))
                {
                    position = ordered;
                    break;
                }
            }

            if (position == null)
            {
                Debug.LogError("VNCharacterPositionController: ShowCharacter: Not enough ordered positions, exiting");
                obj = null;
                return;
            }
        }

        VNCharacterComponent charaComp;
        if (!_instance._characters.TryGetValue(character, out charaComp))
        {
            CreateCharacterComponent(character, out charaComp, position.Layer.Name);
        }

        obj = charaComp.gameObject;

        obj.transform.position   = position.Transform.Position;
        obj.transform.rotation   = Quaternion.Euler(position.Transform.Rotation);
        obj.transform.localScale = position.Transform.Scale * _instance.GlobablSpriteScale;
        // obj.transform.localScale = new Vector3(position.Transform.Scale.x * _instance.GlobablSpriteScale, 
        //                                        position.Transform.Scale.y * _instance.GlobablSpriteScale, 
        //                                        position.Transform.Scale.z * _instance.GlobablSpriteScale);

        foreach (SpriteRenderer renderer in obj.GetComponentsInChildren<SpriteRenderer>(true))
        {
            // renderer.sortingLayerID   = position.Layer.id;
            renderer.sortingLayerName = position.Layer.Name;
        }

        if (instant)
        {
            charaComp.Show();
        }
        else
        {
            ScriptAnimation defaultAnimation  = VNTagsConfig.GetConfig().DefaultEntranceAnimation;
            ScriptAnimation animationInstance = null;
            if (defaultAnimation != null)
            {
                animationInstance = (ScriptAnimation)ScriptableObject.CreateInstance(defaultAnimation.GetType());
            }
            else
            {
                Debug.LogWarning("VNCharacterPositionController: ShowCharacter: Default entrance animation not found");
            }

            charaComp.Show(true, animationInstance);
        }

        _instance._scenePositionComposition.Add(position, charaComp);
        _instance._characters.TryAdd(character, charaComp);
        onCharacterAdded?.Invoke(charaComp, obj, position);
    }

    private static void CreateCharacterComponent(VNCharacterData character, out VNCharacterComponent charaComp, string sortingLayerName)
    {
        var obj = new GameObject(character.Name);
        obj.transform.SetParent(_instance.transform);

        charaComp = obj.AddComponent<VNCharacterComponent>();
        charaComp.Init(character, sortingLayerName);
    }

    public static void MoveCharacter(VNCharacterData character, string position, bool instant)
    {
        // todo
    }
}