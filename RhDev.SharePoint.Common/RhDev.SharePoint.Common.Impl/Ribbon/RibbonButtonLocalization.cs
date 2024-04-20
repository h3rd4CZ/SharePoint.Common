using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Impl.Ribbon
{
    public class RibbonButtonLocalization
    {
        private readonly Dictionary<string, string> buttonTitles = new Dictionary<string, string>();

        public void AddLocalization(string actionName, string buttonTitle)
        {
            buttonTitles[actionName] = buttonTitle;
        }

        public string GetLocalization(string actionName)
        {
            return buttonTitles[actionName];
        }
    }
}
