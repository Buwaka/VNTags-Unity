using System;
using System.Collections.Generic;
using UnityEngine;
using VNTags;
using VNTags.Components;
using VNTags.Utility;

[Serializable]
public class VNPosition
{
    public Transform          Transform;
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
            if (characterVal.CharacterData == character)
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

    public static void HideCharacter(VNCharacterData character)
    {
        if (GetCharacter(character) is VNCharacterComponent charaComp)
        {
            charaComp.Hide();

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


    public static void ShowCharacter(VNCharacterData character, out GameObject obj, VNPosition position = null)
    {
        if (character == null)
        {
            Debug.LogError("VNCharacterPositionController: AddCharacter: Character is null, exiting");
            obj = null;
            return;
        }

        if (IsCharacterActive(character))
        {
            Debug.Log("VNCharacterPositionController: AddCharacter: Character is already active, exiting");
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
                Debug.LogError("VNCharacterPositionController: AddCharacter: Not enough ordered positions, exiting");
                obj = null;
                return;
            }
        }

        VNCharacterComponent charaComp;
        if (!_instance._characters.TryGetValue(character, out charaComp))
        {
            CreateCharacterComponent(character, out charaComp);
        }

        obj = charaComp.gameObject;

        obj.transform.position   = position.Transform.position;
        obj.transform.rotation   = position.Transform.rotation;
        obj.transform.localScale = new Vector3(_instance.GlobablSpriteScale, _instance.GlobablSpriteScale, _instance.GlobablSpriteScale);

        foreach (SpriteRenderer renderer in obj.GetComponentsInChildren<SpriteRenderer>(true))
        {
            renderer.sortingLayerID = position.Layer.id;
        }

        charaComp.Show();

        _instance._scenePositionComposition.Add(position, charaComp);
        _instance._characters.Add(character, charaComp);
        onCharacterAdded?.Invoke(charaComp, obj, position);
    }

    private static void CreateCharacterComponent(VNCharacterData character, out VNCharacterComponent charaComp)
    {
        var obj = new GameObject(character.Name);
        obj.transform.SetParent(_instance.transform);

        charaComp = obj.AddComponent<VNCharacterComponent>();
        charaComp.Init(character);
    }

    public static void MoveCharacter(VNCharacterData character, string position)
    {
        // todo
    }
}