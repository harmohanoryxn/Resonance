using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cloudmaster.WCS.Ward.Model;
using Cloudmaster.WCS.Model;

namespace Cloudmaster.WCS.Ward.Controls
{
    /// <summary>
    /// Interaction logic for EnterLogin.xaml
    /// </summary>
    public partial class EnterLogin : UserControl
    {
        public EnterLogin()
        {
            InitializeComponent();
        }

        public void EnableDisableControls()
        {
            bool isValidLength = (passwordBox.Password.Count() == 4);

            btn0.IsEnabled = !isValidLength;
            btn1.IsEnabled = !isValidLength;
            btn2.IsEnabled = !isValidLength;
            btn3.IsEnabled = !isValidLength;
            btn4.IsEnabled = !isValidLength;
            btn5.IsEnabled = !isValidLength;
            btn6.IsEnabled = !isValidLength;
            btn7.IsEnabled = !isValidLength;
            btn8.IsEnabled = !isValidLength;
            btn9.IsEnabled = !isValidLength;

            if (isValidLength)
            {
                if (WardModel.Instance.SecurityViewModel.Login(passwordBox.Password))
                {
                    passwordBox.Password = string.Empty;

                    EnableDisableControls();
                }
            }
        }

        private void btn0_Click(object sender, RoutedEventArgs e)
        {
            passwordBox.Password += "0";

            EnableDisableControls();
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            passwordBox.Password += "1";

            EnableDisableControls();
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            passwordBox.Password += "2";

            EnableDisableControls();
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            passwordBox.Password += "3";

            EnableDisableControls();
        }

        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            passwordBox.Password += "4";

            EnableDisableControls();
        }

        private void btn5_Click(object sender, RoutedEventArgs e)
        {
            passwordBox.Password += "5";

            EnableDisableControls();

        }

        private void btn6_Click(object sender, RoutedEventArgs e)
        {
            passwordBox.Password += "6";

            EnableDisableControls();
        }

        private void btn7_Click(object sender, RoutedEventArgs e)
        {
            passwordBox.Password += "7";
            EnableDisableControls();

        }

        private void btn8_Click(object sender, RoutedEventArgs e)
        {
            passwordBox.Password += "8";

            EnableDisableControls();
        }

        private void btn9_Click(object sender, RoutedEventArgs e)
        {
            passwordBox.Password += "9";

            EnableDisableControls();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            passwordBox.Password = string.Empty;

            WardModel.Instance.SecurityViewModel.Message = SecurityViewModel.DefaultMessage;

            EnableDisableControls();
        }
    }
}
