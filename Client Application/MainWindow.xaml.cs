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

/*
    svcutil.exe http://localhost:7741/UnifiedWorkSolution/Loggers/LogReceiverService /config:LogService.config /out:LogServiceProxy.cs
 */

namespace Client_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            // quick test
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Error(new Exception(DateTime.Now.ToString()), "MainWindow_Exception");

            App.Logger.Error(new Exception(DateTime.Now.ToString()), "App_Exception");
        }
    }
}
