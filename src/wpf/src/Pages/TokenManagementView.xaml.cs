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
using WPF_app.Models;

namespace WPF_app.Pages
{
    /// <summary>
    /// Interaction logic for TokenManagementView.xaml
    /// </summary>
    public partial class TokenManagementView : Window
    {
        public TokenManagementView()
        {
            InitializeComponent();
            DataContext = new TokenManagementViewModel();
        }
    }
}
