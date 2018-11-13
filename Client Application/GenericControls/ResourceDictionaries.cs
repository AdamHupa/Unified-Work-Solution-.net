using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace Client_Application.GenericControls
{
    public static /*partial*/ class ResourceDictionaries
    {
        public static Dictionary<string, Uri> GenericBases = new Dictionary<string, Uri>()
        {
            // referenced assembly name may contain spaces

            { "Default", new Uri("/Client Application;component/GenericControls/ResourceDictionary.xaml", UriKind.RelativeOrAbsolute) },

            // add here
        };
    }

    public static class ResourceKeys
    {
        public static readonly ComponentResourceKey ChessBoardPatternBase =
            new ComponentResourceKey(typeof(ChessBoardPattern), ResourceIds.ChessBoardPatternBase);

        public static readonly ComponentResourceKey DraggablePopupThumbBase =
            new ComponentResourceKey(typeof(System.Windows.Controls.Primitives.Thumb), ResourceIds.DraggablePopupThumbBase);

        public static readonly ComponentResourceKey ResizablePopupThumbBase =
            new ComponentResourceKey(typeof(System.Windows.Controls.Primitives.Thumb), ResourceIds.ResizablePopupThumbBase);

        public static readonly ComponentResourceKey SpeechBubblePopupBase =
            new ComponentResourceKey(typeof(SpeechBubblePopup), ResourceIds.SpeechBubblePopupBase);

        public static readonly ComponentResourceKey ToolPopupBase =
            new ComponentResourceKey(typeof(ToolPopup), ResourceIds.ToolPopupBase);

    }

    public static class ResourceIds
    {
        public const string ChessBoardPatternBase = "ChessBoardPatternBase";

        public const string DraggablePopupThumbBase = "DraggablePopupThumbBase";
        public const string ResizablePopupThumbBase = "ResizablePopupThumbBase";

        public const string SpeechBubblePopupBase = "SpeechBubblePopupBase";

        public const string ToolPopupBase = "ToolPopupBaseSetting";
        
    }
}
