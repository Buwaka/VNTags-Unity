namespace VNTags
{
    public class OutfitTag : IVNTag
    {
        public  VNOutfit Outfit;
        
        public void Init(string parameters, VNTagLineContext context)
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