using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client_Application.GenericControls
{
    //[TemplatePart(Name = ToolPopup.PopupBorderKey, Type = typeof(System.Windows.Controls.Border))]

    [System.Windows.Markup.ContentPropertyAttribute("Text")]
    public class SpeechBubblePopup : System.Windows.Controls.Primitives.Popup
    {
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(System.Windows.Media.Brush), typeof(SpeechBubblePopup),
            new FrameworkPropertyMetadata(System.Windows.Media.Brushes.Red, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground", typeof(System.Windows.Media.Brush), typeof(SpeechBubblePopup),
            new FrameworkPropertyMetadata(System.Windows.Media.Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty FlowDirectionFromOriginProperty = DependencyProperty.Register(
            "FlowDirectionFromOrigin", typeof(System.Windows.FlowDirection), typeof(SpeechBubblePopup),
            new FrameworkPropertyMetadata(System.Windows.FlowDirection.LeftToRight, FrameworkPropertyMetadataOptions.AffectsArrange |
                                                                                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(SpeechBubblePopup),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                                                FrameworkPropertyMetadataOptions.AffectsRender));

        private const string BorderKey = "PART_Border";
        private const string PolygonKey = "PART_Polygon";
        private const string StackPanelKey = "PART_StackPanel";
        private const string TextBlockKey = "PART_TextBlock";

        System.Windows.Controls.TextBlock _textBlock = null;



        static SpeechBubblePopup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpeechBubblePopup), new FrameworkPropertyMetadata(typeof(SpeechBubblePopup)));
        }

        public SpeechBubblePopup()
        {
            this.Opened += Popup_Opened;
        }


        [System.ComponentModel.Bindable(false)]
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.ReadOnly(true)]
        new public UIElement Child
        {
            get { return base.Child; }
            set { base.Child = value; }
        }

        [System.ComponentModel.Bindable(true)]
        public System.Windows.Media.Brush Background
        {
            get { return GetValue(BackgroundProperty) as System.Windows.Media.Brush; }
            set { SetValue(BackgroundProperty, value); }
        }

        [System.ComponentModel.Bindable(true)]
        public System.Windows.Media.Brush Foreground
        {
            get { return GetValue(ForegroundProperty) as System.Windows.Media.Brush; }
            set { SetValue(ForegroundProperty, value); }
        }

        [System.ComponentModel.Bindable(true)]
        public System.Windows.FlowDirection FlowDirectionFromOrigin
        {
            get { return (System.Windows.FlowDirection)GetValue(FlowDirectionFromOriginProperty); }
            set { SetValue(FlowDirectionFromOriginProperty, value); }
        }

        [System.ComponentModel.Bindable(true)]
        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }


        private void Popup_Opened(object sender, EventArgs e)
        {
            System.Windows.Controls.Primitives.Popup popup = sender as System.Windows.Controls.Primitives.Popup;
            if (popup == null || popup.Child == null)
                return;

            if (_textBlock == null)
                _textBlock = FindVisualChild<System.Windows.Controls.TextBlock>(popup.Child, TextBlockKey);

            if (popup.PlacementTarget == null || _textBlock == null)
                return;


            Point relativePoint = _textBlock.TranslatePoint(new Point(0, 0), (UIElement)popup.PlacementTarget);

            if (relativePoint.X < 0)
            {
                FlowDirectionFromOrigin = System.Windows.FlowDirection.RightToLeft;
                // TODO: find a way to make the binding work
                (popup.Child as StackPanel).FlowDirection = System.Windows.FlowDirection.RightToLeft;
            }
            else
            {
                FlowDirectionFromOrigin = System.Windows.FlowDirection.LeftToRight;
                // TODO: find a way to make the binding work
                (popup.Child as StackPanel).FlowDirection = System.Windows.FlowDirection.LeftToRight;
            }
        }


        public T FindVisualChild<T>(DependencyObject dependencyObject, string key) where T : DependencyObject
        {
            if (dependencyObject != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, i);
                    if (child != null && child is T && key.Equals((string)((T)child).GetValue(NameProperty)))
                        return (T)child;

                    T childItem = FindVisualChild<T>(child, key);
                    if (childItem != null)
                        return childItem;
                }
            }
            return null;
        }
    }
}
