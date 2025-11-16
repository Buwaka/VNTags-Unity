namespace VNTags
{
    public interface IEditorTag
    {
        string GetValue();

        void SetValue(string value);

        VNTagID GetID();
    }
}