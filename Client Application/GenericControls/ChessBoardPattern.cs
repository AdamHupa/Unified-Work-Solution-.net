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
    public class ChessBoardPattern : UserControl
    {
        public static readonly DependencyProperty IsCentralizedProperty = DependencyProperty.Register(
            "IsCentralized", typeof(bool), typeof(ChessBoardPattern),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TransformBrushProperty = DependencyProperty.Register(
            "TransformBrush", typeof(System.Windows.Media.Transform), typeof(ChessBoardPattern),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BlackSquaresProperty = DependencyProperty.Register(
            "BlackSquares", typeof(System.Windows.Media.Brush), typeof(ChessBoardPattern),
            new FrameworkPropertyMetadata(System.Windows.Media.Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty WhiteSquaresProperty = DependencyProperty.Register(
            "WhiteSquares", typeof(System.Windows.Media.Brush), typeof(ChessBoardPattern),
            new FrameworkPropertyMetadata(System.Windows.Media.Brushes.DarkGray, FrameworkPropertyMetadataOptions.AffectsRender));


        static ChessBoardPattern()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChessBoardPattern), new FrameworkPropertyMetadata(typeof(ChessBoardPattern)));
        }


        public bool IsCentralized
        {
            get { return (bool)GetValue(IsCentralizedProperty); }
            set { SetValue(IsCentralizedProperty, value); }
        }

        public System.Windows.Media.Transform TransformBrush
        {
            get { return GetValue(TransformBrushProperty) as System.Windows.Media.Transform; }
            set { SetValue(TransformBrushProperty, value); }
        }

        public System.Windows.Media.Brush BlackSquares
        {
            get { return GetValue(BlackSquaresProperty) as System.Windows.Media.Brush; }
            set { SetValue(BlackSquaresProperty, value); }
        }

        public System.Windows.Media.Brush WhiteSquares
        {
            get { return GetValue(WhiteSquaresProperty) as System.Windows.Media.Brush; }
            set { SetValue(WhiteSquaresProperty, value); }
        }
    }
}
