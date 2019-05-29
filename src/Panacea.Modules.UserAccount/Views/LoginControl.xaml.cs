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
using System.Windows.Threading;

namespace Panacea.Modules.UserAccount.Views
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        public LoginControl()
        {
            InitializeComponent();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;

            if (tabs.SelectedIndex == 0)
            {
                Dispatcher.BeginInvoke(
                DispatcherPriority.ContextIdle,
                new Action(delegate ()
                {
                    txtdob.Focus();
                })
            );
            }
            else if (tabs.SelectedIndex == 1)
            {
                Dispatcher.BeginInvoke(
                DispatcherPriority.ContextIdle,
                new Action(delegate ()
                {
                    userid.Focus();
                }));
            }
        }

    }
}
