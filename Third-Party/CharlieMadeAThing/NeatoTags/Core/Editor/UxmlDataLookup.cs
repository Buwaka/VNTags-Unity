namespace CharlieMadeAThing.NeatoTags.Core.Editor
{
    public static class UxmlDataLookup
    {

        public static string TaggerUxml
        {
            get
            {
                return $"{TagAssetCreation.GetUxmlDirectory()}/Tagger.uxml";
            }
        }
        public static string ButtonTagUxml
        {
            get
            {
                return $"{TagAssetCreation.GetUxmlDirectory()}/buttonTag.uxml";
            }
        }
        public static string ButtonTagWithXUxml
        {
            get
            {
                return $"{TagAssetCreation.GetUxmlDirectory()}/buttonTagWithX.uxml";
            }
        }
        public static string NeatoTagUxml
        {
            get
            {
                return $"{TagAssetCreation.GetUxmlDirectory()}/NeatoTag.uxml";
            }
        }
        public static string NeatTagManagerUxml
        {
            get
            {
                return $"{TagAssetCreation.GetUxmlDirectory()}/NeatoTagManager.uxml";
            }
        }
    }
}