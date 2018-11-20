using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics; // for: PresentationTraceSources, Debugger
using System.Windows.Data; // for: IValueConverter
using System.Windows.Markup; // for: MarkupExtension

// detecting invalid binding expression in XAML:
// 
// in Window tag: xmlns:diagnostics="clr-namespace:System.Diagnostics;assembly=WindowsBase"
// in data binding expression: <TextBlock Text="{Binding Id, diagnostics:PresentationTraceSources.TraceLevel=High}" />

namespace Client_Application.Views.Tools
{
    /// <summary>
    /// Dummy converter to debug the binding values.
    /// </summary>
    /// <remarks>
    /// Append the converter into project's global namespace (Application.Resources tag)
    /// </remarks>
    /// <example>
    /// <Label Content="{Binding ElementName=txtBox1, Path=Text, Converter={StaticResource DebugDummyConverter}}" />
    /// </example>
    public class DebugDummyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debugger.Break();
            return value;
            //return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debugger.Break();
            return value;
            //return Binding.DoNothing;
        }
    }

    /// <summary>
    /// Markup extension to debug data bindings.
    /// </summary>
    public class DebugBindingExtension : MarkupExtension
    {
        /// <summary>
        /// Creates an instance of the DebugDummyConverter.
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new DebugDummyConverter();
        }
    }

    /// <summary>
    /// Listener for Data Binding issues.
    /// </summary>
    /// <remarks>
    /// Add using System.Diagnostics and the following line to the OnStartup method of App class:
    /// PresentationTraceSources.Refresh();
    /// //PresentationTraceSources.DataBindingSource.Listeners.Add(new ConsoleTraceListener());
    /// PresentationTraceSources.DataBindingSource.Listeners.Add(new DebugTraceListener());
    /// PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning | SourceLevels.Error;
    /// </remarks>
    public class DebugTraceListener : TraceListener
    {
        public override void Write(string message)
        {

        }

        public override void WriteLine(string message)
        {
            Debugger.Break();
        }
    }
}
