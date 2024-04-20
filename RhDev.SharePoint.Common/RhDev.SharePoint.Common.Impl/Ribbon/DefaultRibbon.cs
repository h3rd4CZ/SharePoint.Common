using FluentRibbon.Definitions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;

namespace RhDev.SharePoint.Common.Impl.Ribbon
{
    public class DefaultRibbon : RibbonBase
    {
        private const string CANCEL_ACTION = "Cancel";

        private string Url;
        private readonly bool _isSaveDisabled;

        public event EventHandler Cancel;
        public event EventHandler Save;

        public DefaultRibbon(Page page, string url, bool isSaveDisabled)
            : base(page)
        {
            ButtonLocalization.AddLocalization(CANCEL_ACTION, "Zavřít");
            Url = url;
            _isSaveDisabled = isSaveDisabled;
        }

        protected override TabDefinition GetTabDefinition()
        {
            var groups = new List<GroupDefinition>();

            var ribbonGroup = GetRibbonGroup();
            if (ribbonGroup != null && ribbonGroup.Controls.Any())
                groups.Add(ribbonGroup);

            var documentDetailTab = new TabDefinition
            {
                Id = "Action",
                Title = "Akce",
                Groups = groups.ToArray()
            };

            return documentDetailTab;
        }

        private GroupDefinition GetRibbonGroup()
        {
            var buttons = new List<ControlDefinition>();

            if(!_isSaveDisabled)
                AddButton(buttons, SAVE_ACTION, new Point(15, 1), ShowAlways, GetPostBackActionJavaScript(SAVE_ACTION));
            CreateAdditionalButtons(buttons);
            AddButton(buttons, CANCEL_ACTION, new Point(15, 15), ShowAlways, GetScriptForCloseButton(Url));

            return CreateGroup("Actions", "Akce", buttons);
        }

        protected virtual void CreateAdditionalButtons(List<ControlDefinition> buttons)
        {

        }

        protected override void ProcessPostBack(string eventTarget)
        {
            RaiseEventIfActionPostBack(eventTarget, CANCEL_ACTION, Cancel);
            RaiseEventIfActionPostBack(eventTarget, SAVE_ACTION, Save);
        }
    }
}
