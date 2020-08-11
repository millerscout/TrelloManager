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
using System.Windows.Shapes;
using Core;

namespace TrelloManager
{
    /// <summary>
    /// Interaction logic for UserAccess.xaml
    /// </summary>
    public partial class UserAccess : Window
    {
        public UserAccess()
        {
            InitializeComponent();

            txtKey.Text = LoadService.Config.Key;
            txtToken.Text = LoadService.Config.Token;


        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            LoadService.Config =
            new TrelloConfig
            {
                Key = txtKey.Text,
                Token = txtToken.Text
            };

            LoadService.SaveConfig();

        }
    }
}
