namespace VNTags
{
    public class ExpressionTag : IVNTag
    {
        public VNExpression? Expression;
        
        public void Deserialize(string parameters, VNTagLineContext context)
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