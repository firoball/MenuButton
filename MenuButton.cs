using UnityEngine.UIElements;

namespace UI.Controls
{
    /// <summary>
    /// UnityEditor-independent replacement for UnityEditor.UIElements.ToolbarMenu.
    /// Behaves the same way: click opens a dropdown, populated via <see cref="menu"/>.
    /// Works in editor windows, UI Builder, and at runtime.
    /// </summary>
    [UxmlElement]
    public partial class MenuButton : VisualElement
    {
        public static readonly string ussClassName = "ui-menu-button";
        public static readonly string arrowUssClassName = ussClassName + "__arrow";

        private readonly Label label;

        [UxmlAttribute]
        public string text
        {
            get => label.text;
            set => label.text = value;
        }

        /// <summary>If true, the dropdown uses GenericDropdownMenu's own default appearance instead of MenuButton's color scheme.</summary>
        [UxmlAttribute]
        public bool useDefaultMenuAppearance
        {
            get => menu.useDefaultAppearance;
            set => menu.useDefaultAppearance = value;
        }

        /// <summary>Populate this like UnityEditor.UIElements.ToolbarMenu.menu (AppendAction / AppendSeparator).</summary>
        public MenuButtonMenu menu { get; }

        public MenuButton() : this(string.Empty) { }

        public MenuButton(string text, bool useDefaultMenuAppearance = false)
        {
            AddToClassList(ussClassName);

            label = new Label();
            label.RemoveFromClassList(Label.ussClassName);
            Add(label);
            this.text = text;

            var arrow = new VisualElement();
            arrow.AddToClassList(arrowUssClassName);
            Add(arrow);

            menu = new MenuButtonMenu(this, useDefaultMenuAppearance);
            MenuButtonStyle.Apply(this);

            focusable = true;
            RegisterCallback<PointerDownEvent>(OnPointerDown);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 0)
                return;

            Focus();
            menu.Show();
            evt.StopPropagation();
        }
    }
}
