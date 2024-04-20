using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using FluentRibbon;
using FluentRibbon.Definitions;
using FluentRibbon.Definitions.Controls;
using FluentRibbon.Libraries;
using RhDev.SharePoint.Common.Utils;

namespace RhDev.SharePoint.Common.Impl.Ribbon
{
    public abstract class RibbonBase
    {
        public const string CLOSE_ACTION = "Close";
        public const string SAVE_ACTION = "Save";

        protected static Func<ButtonDisplay> ShowAlways { get; } = () => ButtonDisplay.Show;

        protected Page Page { get; private set; }

        protected SectionDesignation CurrentSectionDesignation
        {
            get { throw new NotImplementedException(); }
        }
        
        protected RibbonButtonLocalization ButtonLocalization { get; private set; }

        protected RibbonBase(Page page)
        {
            Page = page;
            InitializeLocalization();
        }

        private void InitializeLocalization()
        {
            ButtonLocalization = new RibbonButtonLocalization();
            ButtonLocalization.AddLocalization(SAVE_ACTION, "Uložit");
            ButtonLocalization.AddLocalization(CLOSE_ACTION, "Zavřít");
        }

        public void AddToPage()
        {
            var tabDefinition = GetTabDefinition();

            if (tabDefinition != null && tabDefinition.Groups.Any())
                RibbonController.Current.AddRibbonTabToPage(tabDefinition, Page, true);
        }

        protected abstract TabDefinition GetTabDefinition();

        public void ProcessPostBack()
        {
            ProcessPostBack(false);
        }

        public void ProcessPostBack(bool forbiddenAction)
        {
            if (forbiddenAction)
                return;

            string eventTarget = Page.Request["__EVENTTARGET"];

            TracePostBackEventTarget(eventTarget);

            ProcessPostBack(eventTarget);
        }

        private static void TracePostBackEventTarget(string eventTarget)
        {

        }

        protected abstract void ProcessPostBack(string eventTarget);



        protected string GetPostBackActionJavaScript(string action)
        {
            return GetPostBackActionJavaScript(GetActionSource(), action);
        }

        private string GetActionSource()
        {
            return GetType().Name;
        }

        public static string GetPostBackActionJavaScript(string actionSource, string action)
        {
            const string disableRibbonScript = "SP.Ribbon.PageManager.get_instance().get_ribbon().set_enabled(false);";
            return String.Format("{0} __doPostBack('{1}','');", disableRibbonScript, GetPostBackActionName(actionSource, action));
        }

        protected string GetPostBackActionJavaScriptConfirm(string action, string confirmText)
        {
            Guard.StringNotNullOrWhiteSpace(confirmText, nameof(confirmText));

            var pbAction = GetPostBackActionJavaScript(GetActionSource(), action);
                        
            var challengeAction = $"if (confirm('{confirmText}')){{{pbAction}}}";

            return challengeAction;
        }

        private string GetPostBackActionName(string action)
        {
            return GetPostBackActionName(GetActionSource(), action);
        }

        private static string GetPostBackActionName(string actionSource, string action)
        {
            return String.Format("RhDevSOLUTION_{0}_{1}", actionSource, action);
        }

        private bool IsPostBackAction(string eventTarget, string action)
        {
            string actionName = GetPostBackActionName(action);
            return String.Equals(eventTarget, actionName, StringComparison.Ordinal);
        }

        protected bool RaiseEventIfActionPostBack(string eventTarget, string action, EventHandler eventHandler)
        {
            return RaiseEventIfActionPostBack(eventTarget, action, eventHandler, EventArgs.Empty);
        }

        private bool RaiseEventIfActionPostBack(string eventTarget, string action, EventHandler eventHandler, EventArgs eventArgs)
        {
            if (IsPostBackAction(eventTarget, action) && eventHandler != null)
            {
                eventHandler(this, eventArgs);
                return true;
            }

            return false;
        }

        protected enum ButtonDisplay
        {
            Show,
            Hide,
            Disable
        }
        
        protected void AddButtonSaveTask(IList<ControlDefinition> controls, string actionName, Point imageCoordinates, Func<ButtonDisplay> displayPredicate, string onClickScript)
        {
            ButtonDisplay buttonDisplay = ButtonDisplay.Show;

            if (displayPredicate != null)
            {
                buttonDisplay = displayPredicate();

                if (buttonDisplay == ButtonDisplay.Hide)
                    return;
            }
            
            var buttonDefinition = new ButtonDefinition
            {
                Id = actionName,
                Title = ButtonLocalization.GetLocalization(actionName),
                CommandJavaScript = onClickScript,
                CommandEnableJavaScript = (buttonDisplay.Equals(ButtonDisplay.Disable)) ? "false" : "true"
            };

            controls.Add(buttonDefinition);
        }

        protected void AddButton(IList<ControlDefinition> controls, string actionName, Point imageCoordinates, Func<ButtonDisplay> displayPredicate)
        {
            string onClickScript = GetPostBackActionJavaScript(actionName);
            AddButton(controls, actionName, imageCoordinates, displayPredicate, onClickScript);
        }

        protected void AddButton(IList<ControlDefinition> controls, string actionName, Point imageCoordinates, Func<ButtonDisplay> displayPredicate, string onClickScript)
        {
            ButtonDisplay buttonDisplay = ButtonDisplay.Show;

            if (displayPredicate != null)
            {
                buttonDisplay = displayPredicate();

                if (buttonDisplay == ButtonDisplay.Hide)
                    return;
            }

            var buttonDefinition = new ButtonDefinition
            {
                Id = actionName,
                Title = ButtonLocalization.GetLocalization(actionName),
                CommandJavaScript = onClickScript,
                CommandEnableJavaScript = (buttonDisplay.Equals(ButtonDisplay.Disable)) ? "false" : "true",
                Image = ImageLibrary.GetStandardImage(imageCoordinates.X, imageCoordinates.Y, 1029),
            };            

            controls.Add(buttonDefinition);
        }

        protected static GroupDefinition CreateGroup(string name, string title, List<ControlDefinition> buttons)
        {
            return new GroupDefinition
            {
                Id = name,
                Title = title,
                Template = GroupTemplateLibrary.SimpleTemplate,
                Controls = buttons.ToArray()
            };
        }

        protected string GetScriptForCloseButton(string redirectUrl)
        {
            return String.Format("javascript:window.location.href = '{0}'; return false;", redirectUrl);
        }
    }
}
