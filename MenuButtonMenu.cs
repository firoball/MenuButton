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
        private static MenuButtonMenu openMenu;

        private readonly GenericDropdownMenu dropdownMenu = new GenericDropdownMenu();
        private readonly VisualElement anchor;
        private readonly VisualElement outerContainer;
        private readonly VisualElement menuRoot;
        private bool defaultAppearance;
        private bool globalHandlerRegistered;

        /// <summary>If true, uses GenericDropdownMenu's own default appearance instead of MenuButton's color scheme.</summary>
        public bool useDefaultAppearance
        {
            get => defaultAppearance;
            set
            {
                defaultAppearance = value;
                if (value)
                    MenuButtonStyle.Remove(outerContainer);
                else
                    MenuButtonStyle.Apply(outerContainer);
            }
        }

        internal MenuButtonMenu(VisualElement anchor, bool useDefaultAppearance = false)
        {
            this.anchor = anchor;

            outerContainer = dropdownMenu.contentContainer;
            while (outerContainer != null && !outerContainer.ClassListContains(GenericDropdownMenu.containerOuterUssClassName))
                outerContainer = outerContainer.parent;
            outerContainer ??= dropdownMenu.contentContainer;
            menuRoot = outerContainer.parent ?? outerContainer;

            menuRoot.RegisterCallback<DetachFromPanelEvent>(OnMenuDetached);
            this.useDefaultAppearance = useDefaultAppearance;
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
            if (openMenu != null && openMenu != this)
                openMenu.menuRoot.RemoveFromHierarchy();

            dropdownMenu.DropDown(anchor.worldBound, anchor, DropdownMenuSizeMode.Content);
            openMenu = this;

            if (!globalHandlerRegistered)
            {
                anchor.panel.visualTree.RegisterCallback<PointerDownEvent>(OnOutsidePointerDown, TrickleDown.TrickleDown);
                globalHandlerRegistered = true;
            }
        }

        // Closes this popup on any click outside it, regardless of what's clicked (another MenuButton,
        // a DropdownField, empty space) - not just other MenuButtons like the openMenu tracking above covers.
        private void OnOutsidePointerDown(PointerDownEvent evt)
        {
            if (!menuRoot.Contains(evt.target as VisualElement))
                menuRoot.RemoveFromHierarchy();
        }

        private void OnMenuDetached(DetachFromPanelEvent evt)
        {
            if (globalHandlerRegistered)
            {
                evt.originPanel?.visualTree.UnregisterCallback<PointerDownEvent>(OnOutsidePointerDown, TrickleDown.TrickleDown);
                globalHandlerRegistered = false;
            }
            if (openMenu == this)
                openMenu = null;
        }
    }
}
