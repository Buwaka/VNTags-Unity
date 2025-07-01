namespace VNTags
{
    public class DialogueNameTag : IVNTag
    {
        public string GetTagID()
        {
            return "DialogueName";
        }

        public void Execute(VNTagContext context, out bool isFinished)
        {
            //todo
            isFinished = true;
        }
    }
}