namespace RhDev.SharePoint.Common.Taxonomy
{
    public class TermValueLabel
    {
        public string Value { get; }
        public int Lang { get; }
        public bool IsDefault { get; }
                
        public TermValueLabel(string value, int lang, bool isDefault)
        {
            Value = value;
            Lang = lang;
            IsDefault = isDefault;
        }
    }
}
