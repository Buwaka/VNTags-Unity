namespace VNTags
{
    public class ExpressionTag : IVNTag
    {
        public VNExpression? Expression;
        
        public void Deserialize(VNTagLineContext context, params string[] parameters)
        {
            // todo
        }

        public string Serialize()
        {
            return Expression != null ?  IVNTag.SerializeHelper(GetTagID(), Expression.Name) : "";
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