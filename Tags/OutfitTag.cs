namespace VNTags
{
    public class OutfitTag : IVNTag
    {
        public  VNOutfit Outfit;
        
        public void Deserialize(VNTagLineContext context, params string[] parameters)
        {
            // todo
        }

        public string Serialize()
        {
            return Outfit != null ?  IVNTag.SerializeHelper(GetTagID(), Outfit.Name) : "";
        }

        public string GetTagID()
        {
            return "Outfit";
        }

        public void Execute(VNTagContext context, out bool isFinished)
        {
            // todo
            isFinished = true;
        }
    }
}