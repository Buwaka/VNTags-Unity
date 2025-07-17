using System;

namespace VNTags
{
    /// <summary>
    /// This event announcer is the bridge between the VNTags system and the game specific implementation.
    /// This way you can freely implement how each event should be handled in your game,
    /// rather than relying on a default method inside of VNTags.
    /// The return value for each Handler will be used as the IsFinished value inside of IVNTag.Execute(),
    /// Essentially, the return value of the handler is whether the tag is finished or not,
    /// true for done, move to the next tag;
    /// false for not done, requiring another execute call before completion.
    ///
    /// how to subscribe a handler: VNTagEventAnnouncer.onDialogueChange += onDialogue;
    /// with onDialogue being a function with the same signature as DialogueHandler.
    /// </summary>
    public static class VNTagEventAnnouncer
    {
        public static CharacterHandler onCharacterChange;
        public static DialogueHandler onDialogueChange;
        public static ExpressionHandler onExpressionChange;
        public static OutfitHandler onOutfitChange;
    }
}