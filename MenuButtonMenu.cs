using System;
using UnityEngine.UIElements;

namespace UI.Controls
{
    /// <summary>
    /// Populates and shows the dropdown for a <see cref="MenuButton"/>. API mirrors
    /// UnityEditor.UIElements.DropdownMenu (AppendAction / AppendSeparator) but is backed
    /// by the runtime-safe GenericDropdownMenu, so it has no UnityEditor dependency.
    /// </summary>
    public class MenuButtonMenu
    {
        private readonly GenericDropdownMenu dropdownMenu = new GenericDropdownMenu();
        private readonly VisualElement anchor;

        internal MenuButtonMenu(VisualElement anchor)
        {
            this.anchor = anchor;
        }

        public void AppendAction(string actionName, Action<DropdownMenuAction> action,
            DropdownMenuAction.Status status = DropdownMenuAction.Status.Normal, object userData = null)
        {
            AppendAction(actionName, action, _ => status, userData);
        }

        public void AppendAction(string actionName, Action<DropdownMenuAction> action,
            Func<DropdownMenuAction, DropdownMenuAction.Status> statusCallback, object userData = null)
        {
            var menuAction = new DropdownMenuAction(actionName, action, statusCallback, userData);
            var status = statusCallback != null ? statusCallback(menuAction) : DropdownMenuAction.Status.Normal;

            if ((status & DropdownMenuAction.Status.Hidden) != 0)
                return;

            bool isChecked = (status & DropdownMenuAction.Status.Checked) != 0;
            bool isDisabled = (status & DropdownMenuAction.Status.Disabled) != 0;

            if (isDisabled)
                dropdownMenu.AddDisabledItem(actionName, isChecked);
            else
                dropdownMenu.AddItem(actionName, isChecked, () => menuAction.Execute());
        }

        public void AppendSeparator(string subMenuPath = null)
        {
            dropdownMenu.AddSeparator(subMenuPath ?? string.Empty);
        }

        internal void Show()
        {
            dropdownMenu.DropDown(anchor.worldBound, anchor, true);
        }
    }
}
