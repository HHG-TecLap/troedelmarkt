using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TroedelMarkt
{
    /// <summary>
    /// Interaktionslogik für LoginWindow.xaml
    /// </summary>
    
    public partial class LoginWindow : Window
    {
        public string Adress { get; set; }
        public string Port { get; set; }
        public HTTPManager? httpManager { get; set; }
        public LoginWindow()
        {
            InitializeComponent();
            Adress = "";
            Port = "3080";
            DataContext = Adress;
            DataContext = Port;
            
        }

        private async void TbnLogin_Click(object sender, RoutedEventArgs e)
        {//Databinding for adress with regex
            try 
            {
                httpManager = await HTTPManager.NewAuthenticated(Adress,int.Parse(Port), PBoxPassword.Password);
                DialogResult = true;
            }
            catch(Exception ex) 
            { 
                if(ex is UnauthorizedException)
                {
                    TBlockResponce.Text = "Passwort falsch";
                }
                else if (ex is NotFoundException)
                {
                    TBlockResponce.Text = "Adresse oder Port falsch";
                }
                else
                {
                    TBlockResponce.Text = $"Es ist ein unbekannter Fehler aufgetreten\n{ex.Message},\n{ex.GetType()}";
                    MessageBox.Show($"{ex.Message},\n{ex.GetType()}");
                }
            }
        }
    }

    partial class PortValidation : ValidationRule
    {
        public PortValidation() { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                int input = int.Parse(value as string);
                if (0 <= input && input <= 65535)
                {
                    return ValidationResult.ValidResult;
                }
            }
            catch
            {
                
            }
            return new ValidationResult(false, "illegal input");
        }
    }
}
