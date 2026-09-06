using UnityEngine.UIElements;

namespace Ui.Controls
{
    /// <summary>
    /// UnityEditor-independent replacement for UnityEditor.UIElements.ToolbarMenu.
    /// Behaves the same way: click opens a dropdown, populated via <see cref="menu"/>.
    /// Works in editor windows, UI Builder, and at runtime.
    /// </summary>
    [UxmlElement]
    public partial class MenuButton : TextElement
    {
        public static readonly string ussClassName = "ui-menu-button";
        public static readonly string arrowUssClassName = ussClassName + "__arrow";

        /// <summary>Populate this like UnityEditor.UIElements.ToolbarMenu.menu (AppendAction / AppendSeparator).</summary>
        public MenuButtonMenu menu { get; }

        public MenuButton() : this(string.Empty) { }

        public MenuButton(string text)
        {
            AddToClassList(ussClassName);
            this.text = text;

            var arrow = new VisualElement();
            arrow.AddToClassList(arrowUssClassName);
            Add(arrow);

            menu = new MenuButtonMenu(this);
            MenuButtonStyle.Apply(this);

            RegisterCallback<PointerDownEvent>(OnPointerDown);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 0)
                return;

            menu.Show();
            evt.StopPropagation();
        }
    }
}
