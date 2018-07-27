using MahApps.Metro.Controls;
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

namespace MeowsBetterParamEditor
{
    /// <summary>
    /// Interaction logic for QuickTextEntry.xaml
    /// </summary>
    public partial class QuickTextEntry : MetroWindow
    {
        public QuickTextEntry()
        {
            InitializeComponent();
        }

        private void KMS()
        {
            Close();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            KMS();
        }

        private void TextBoxEntry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                KMS();
        }
    }
}
