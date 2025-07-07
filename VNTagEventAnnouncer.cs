using System;

namespace VNTags
{
    public static class VNTagEventAnnouncer
    {
        public static CharacterHandler onCharacterChange;
        public static DialogueHandler onDialogueChange;
        public static ExpressionHandler onExpressionChange;
        public static OutfitHandler onOutfitChange;
    }
}