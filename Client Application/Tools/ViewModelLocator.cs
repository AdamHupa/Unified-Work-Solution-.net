using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using System.ComponentModel;
using System.Windows;

namespace Client_Application.Tools
{
    /// <example>
    /// view_models:ViewModelLocator.AutoHookedUpViewModel="True"
    /// </example>
    public static class ViewModelLocator
    {
        private static readonly DependencyProperty AutoWireViewModelProperty = DependencyProperty.RegisterAttached(// nameof()
            "AutoWireViewModel", typeof(bool), typeof(ViewModelLocator),
            new PropertyMetadata(false, AutoHookedUpViewModelChanged));

        private static readonly Autofac.IContainer _iocRegistry = null;
        

        static ViewModelLocator()
        {
            if (App.Current.Resources.Contains(App.GlobalAutofacRegistry))
                _iocRegistry = App.Current.Resources[App.GlobalAutofacRegistry] as Autofac.IContainer;

            if (_iocRegistry == null)
                throw new TypeInitializationException(typeof(ViewModelLocator).FullName, null);
        }


        public static Autofac.IContainer DefaultRegistry { get { return _iocRegistry; } }
        

        public static bool GetAutoHookedUpViewModel(DependencyObject DependencyObject)
        {
            return (bool)DependencyObject.GetValue(AutoWireViewModelProperty);
        }

        public static void SetAutoHookedUpViewModel(DependencyObject DependencyObject, bool value)
        {
            DependencyObject.SetValue(AutoWireViewModelProperty, value);
        }

        private static void AutoHookedUpViewModelChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(dependencyObject))
                return;

            string viewTypeName = dependencyObject.GetType().FullName;
            string viewModelTypeName = viewTypeName.Replace(".Views.", ".ViewModels.") + "Model";

            Type viewModelType = Type.GetType(viewModelTypeName);
            if (viewModelType == null)
                return;

            
            //object viewModel = Activator.CreateInstance(viewModelType);

            //_iocRegistry.Register(viewModelType);
            //object viewModel = _iocRegistry.RegisteredEntry(viewModelType);
            
            object viewModel = null;
            if (!_iocRegistry.TryResolve(viewModelType, out viewModel))
                return;


            // view-viewModel hook up
            FrameworkElement view = dependencyObject as FrameworkElement;
            if (view != null)
                view.DataContext = viewModel;
        }
    }
}
