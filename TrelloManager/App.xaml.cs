using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Core;

namespace TrelloManagerv2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Unhandled exception occurred: \n" + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            File.AppendAllText(Constants.Errorfile, $"{DateTime.Now} -  {e.Exception.Message}.{Environment.NewLine}");
            File.AppendAllText(Constants.Errorfile, $"{e.Exception.StackTrace} {Environment.NewLine}");

        }
    }
}
