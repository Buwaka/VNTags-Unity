using System;
using UnityEngine;

namespace VNTags.Tags
{
    /// <param name="nameOnly">
    ///     When true, only the character's name should be shown (e.g. in a name label) — the sprite must
    ///     not be shown or hidden, regardless of <paramref name="action" />.
    /// </param>
    public delegate bool CharacterHandler
        (VNTagContext context, VNCharacterData character, CharacterAction action, bool nameOnly = false);

    public enum CharacterAction
    {
        AddedToScene,
        RemovedFromScene
    }

    public class CharacterTag : VNTag
    {
        private CharacterAction _action = CharacterAction.AddedToScene;

        private VNCharacterData _character;

        /// <summary>
        ///     When true, this tag only updates the character's name label — it does not show or hide
        ///     their sprite, regardless of <see cref="_action" />. For a character speaking off-screen
        ///     (a phone call, narration) who should still get their name shown.
        /// </summary>
        private bool _nameOnly;

        public VNCharacterData Character
        {
            get { return _character; }
        }

        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if ((parameters == null) || (parameters.Length <= 0))
            {
                Debug.LogError("CharacterTag: Deserialize: No parameters provided '" + context + "'");
                return false;
            }

            _character = VNTagsConfig.GetConfig().GetCharacterByNameOrAlias(parameters[0]);

            if (Character == null)
            {
                Debug.Log("CharacterTag: Deserialize: Failed to find Character with name '" + parameters[0] + "', using only name instead " + context);
                _character = VNCharacterData.BlankCharacter(parameters[0]);
            }

            if (parameters.Length > 1)
            {
                if (CharacterAction.TryParse(parameters[1], true, out CharacterAction result))
                {
                    _action = result;
                }
                else
                {
                    Debug.LogWarning("CharacterTag: Deserialize: Failed to parse CharacterAction '"
                                   + parameters[1]
                                   + "', default action will be used, "
                                   + context);
                }
            }

            if (parameters.Length > 2)
            {
                if (bool.TryParse(parameters[2], out bool result))
                {
                    _nameOnly = result;
                }
                else
                {
                    Debug.LogWarning("CharacterTag: Deserialize: Failed to parse NameOnly '"
                                   + parameters[2]
                                   + "', default (false) will be used, "
                                   + context);
                }
            }

            return true;
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            return (Character == null) || Character.IsNone() ?
                SerializeHelper(GetTagName(), "", _action, _nameOnly) :
                SerializeHelper(GetTagName(), Character.Name, _action, _nameOnly);
        }

        public override string GetTagName()
        {
            return "Character";
        }

        protected override VNTagParameters Parameters(VNTagParameters currentParameters)
        {
            var characterParameter = new VNTagParameter(1,
                                                        "Character",
                                                        TypeCode.String,
                                                        "Character to add or remove from the scene",
                                                        false,
                                                        null,
                                                        VNTagsConfig.GetConfig().GetCharacterNames());
            var actionParameter = new VNTagParameter(2,
                                                     "Character Action",
                                                     TypeCode.String,
                                                     "enum value of 'add' or 'remove', will be passed along the onCharacterTag event (default is 'add')",
                                                     true,
                                                     typeof(CharacterAction));
            var nameOnlyParameter = new VNTagParameter(3,
                                                       "Name Only",
                                                       TypeCode.Boolean,
                                                       "If true, only the name label is updated — the sprite is not shown or hidden, "
                                                     + "regardless of Character Action (default false)",
                                                       true);

            currentParameters.DefaultParameter(characterParameter, _character?.Name!);
            currentParameters.DefaultParameter(actionParameter,   _action.ToString());
            currentParameters.DefaultParameter(nameOnlyParameter, _nameOnly.ToString());

            return currentParameters;
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onCharacterTag?.Invoke(context, Character, _action, _nameOnly));
        }

#if UNITY_EDITOR
        public void SetCharacter(VNCharacterData character)
        {
            _character = character;
        }

        public ref VNCharacterData GetCharacterRef()
        {
            return ref _character;
        }
#endif
    }
}