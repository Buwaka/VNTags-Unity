namespace VNTags
{
    public class CostumeTag : IVNTag
    {
        public void Init(string parameters)
        {
            // todo
        }

        public string GetTagID()
        {
            return "Expression";
        }

        public void Execute(VNTagContext context, out bool isFinished)
        {
            // todo
            isFinished = true;
        }
    }
}