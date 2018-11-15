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
    public enum OperatingMode : byte
    {
        DesignMode = 0,
        RunTimeMode
    }

    
    /// <remarks>Use as View resource.</remarks>
    public class SpecialisedLocator
    {
        private static readonly Autofac.IContainer _iocRegistry = null;
        private static bool _isInDesignMode = InDesignMode();

        private string _targetType = "";
        private string _designModeToken = "";
        private string _runTimeToken = "";

        
        static SpecialisedLocator()
        {
            if (App.Current.Resources.Contains(App.GlobalAutofacRegistry))
                _iocRegistry = App.Current.Resources[App.GlobalAutofacRegistry] as Autofac.IContainer;

            if (_iocRegistry == null)
                throw new TypeInitializationException(typeof(SpecialisedLocator).FullName, null);
        }


        public static bool IsInDesignMode
        {
            get { return _isInDesignMode; }
        }

        public string DesignModeToken
        {
            set { _designModeToken = value; }
        }

        public string RunTimeToken
        {
            set { _runTimeToken = value; }
        }

        public string TargetType
        {
            set { _targetType = value; }
        }

        public object ViewModelInstance
        {
            get { return GetInstance(); }
        }


        private object GetInstance()
        {
            if (String.IsNullOrWhiteSpace(_targetType))
                return null;

            Type viewModelType = Type.GetType(_targetType);
            if (viewModelType == null) 
                return null;


            string tokenString = _isInDesignMode ? _designModeToken : _runTimeToken;
            object viewModel = null;

            if (tokenString != null)
                _iocRegistry.TryResolveNamed(tokenString, viewModelType, out viewModel);
            else
                _iocRegistry.TryResolveKeyed(_isInDesignMode ? OperatingMode.DesignMode : OperatingMode.RunTimeMode, viewModelType, out viewModel);

            return viewModel;
        }

        /// <summary>
        /// Determines if the application's code works in Design Mode.
        /// </summary>
        /// <returns>true, if in Design Mode.</returns>
        private static bool InDesignMode()
        {
            DependencyProperty property = DesignerProperties.IsInDesignModeProperty;
            bool result = (bool)DependencyPropertyDescriptor.FromProperty(property, typeof(FrameworkElement)).Metadata.DefaultValue;

            // panic mode
            if (!result && System.Diagnostics.Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal))
                result = true;
            return result;
        }
    }
}
