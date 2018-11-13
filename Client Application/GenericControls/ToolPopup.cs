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
    
    [System.Windows.Markup.ContentPropertyAttribute("ToolContent")]
    public class ToolPopup : System.Windows.Controls.Primitives.Popup
    {
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(System.Windows.Media.Brush), typeof(ToolPopup),
            new FrameworkPropertyMetadata(System.Windows.Media.Brushes.DarkGray, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground", typeof(System.Windows.Media.Brush), typeof(ToolPopup),
            new FrameworkPropertyMetadata(System.Windows.Media.Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ToolContentProperty = DependencyProperty.Register( // readonly == constant ??
            "ToolContent", typeof(UIElement), typeof(ToolPopup),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                                                FrameworkPropertyMetadataOptions.AffectsRender));

        private const string ContentPresenterKey = "PART_ContentPresenter";
        private const string PopupBorderKey = "PART_PopupBorder";
        private const string DraggableThumbKey = "PART_DraggableThumb";
        private const string ResizableThumbKey = "PART_ResizableThumb";

        private EventHandler _windowRepositionHandler;
        private SizeChangedEventHandler _windowResizeHandler;
        private MouseButtonEventHandler _windowMouseDownHandler;


        static ToolPopup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolPopup), new FrameworkPropertyMetadata(typeof(ToolPopup)));
        }

        public ToolPopup()
        {
            //// set default style and settings
            //SetResourceReference(StyleProperty, typeof(ToolPopup));

            // on window mouse down - close popup after lose focus inside parent window
            _windowMouseDownHandler = delegate(object sender, MouseButtonEventArgs e)
            {
                if (this.IsMouseOver == false)
                {
                    if (this.PlacementTarget == null || !this.PlacementTarget.IsMouseOver)
                        this.IsOpen = false;
                }
            };
            // on window reposition - automatic popup reposition
            _windowRepositionHandler = delegate(object sender, EventArgs args)
            {
                double offset = this.HorizontalOffset;

                this.HorizontalOffset = offset + 1;
                this.HorizontalOffset = offset;
            };
            // on window resize - automatic popup reposition
            _windowResizeHandler = delegate(object sender, SizeChangedEventArgs e)
            {
                double offset = this.HorizontalOffset;

                this.HorizontalOffset = offset + 1;
                this.HorizontalOffset = offset;
            };
            
            // set default behaviour
            this.Closed += Popup_Closed;
            this.Loaded += Popup_Loaded;
            this.Unloaded += Popup_Unloaded;
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
        public UIElement ToolContent
        {
            get { return GetValue(ToolContentProperty) as UIElement; }
            set { SetValue(ToolContentProperty, value); }
        }


        #region Popup methods

        private void Popup_Closed(object sender, EventArgs e)
        {
            System.Windows.Controls.Primitives.Popup popup = sender as System.Windows.Controls.Primitives.Popup;
            if (popup == null)
                return;

            popup.ClearValue(System.Windows.Controls.Primitives.Popup.HorizontalOffsetProperty);
            popup.ClearValue(System.Windows.Controls.Primitives.Popup.VerticalOffsetProperty);
        }

        private void Popup_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Primitives.Popup popup = sender as System.Windows.Controls.Primitives.Popup;
            if (popup == null)
                return;

            Window window = Window.GetWindow(popup);
            if (window == null)
                return;

            // add reposition listeners
            window.LocationChanged += _windowRepositionHandler;
            window.SizeChanged += _windowResizeHandler;
            // add window localised click listener
            window.PreviewMouseDown += _windowMouseDownHandler;

            // add drag listener
            System.Windows.Controls.Primitives.Thumb draggableThumb =
                FindVisualChild<System.Windows.Controls.Primitives.Thumb>(popup.Child, DraggableThumbKey);
            if (draggableThumb != null)
                draggableThumb.DragDelta += DraggableThumb_DragDelta;

            //// add resize listener
            //System.Windows.Controls.Primitives.Thumb resizableThumb =
            //    FindVisualChild<System.Windows.Controls.Primitives.Thumb>(popup.Child, ResizableThumbKey);
            //if (resizableThumb != null)
            //    resizableThumb.DragDelta += ResizableThumb_DragDelta;
        }

        private void Popup_Unloaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Primitives.Popup popup = sender as System.Windows.Controls.Primitives.Popup;
            if (popup == null)
                return;

            Window window = Window.GetWindow(popup);
            if (window == null)
                return;

            window.LocationChanged -= _windowRepositionHandler;
            window.SizeChanged -= _windowResizeHandler;
            window.PreviewMouseDown -= _windowMouseDownHandler;
        }
        #endregion


        #region Popup.Child methods

        private void DraggableThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            this.HorizontalOffset += e.HorizontalChange;
            this.VerticalOffset += e.VerticalChange;
        }

        //private void ResizableThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        //{
        //    e.HorizontalChange e.VerticalChange;
        //}
        #endregion


        public T FindVisualChild<T>(DependencyObject dependencyObject, string key) where T : DependencyObject
        {
            if (dependencyObject != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, i);
                    if (child != null && child is T && key.Equals( (string)((T)child).GetValue(NameProperty) ))
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
