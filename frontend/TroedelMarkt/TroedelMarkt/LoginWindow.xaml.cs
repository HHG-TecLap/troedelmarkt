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
    /// Interactionlogic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        /// <summary>
        /// The address of the server to connect to
        /// </summary>
        public string Adress { get; set; }
        /// <summary>
        /// The port of the server to connect to
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// The <see cref="HTTPManager"/> for hadeling the connection to the server
        /// </summary>
        public HTTPManager? httpManager { get; set; }

        /// <summary>
        /// Constructor for initialising the window
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent();//Starting Component
            //setting default data
            Adress = ""; 
            Port = "3080";
            //Preparing Variables for databindinig
            DataContext = Adress;
            DataContext = Port;
            
        }

        /// <summary>
        /// Funtion for handling a click on the login button
        /// </summary>
        private async void TbnLogin_Click(object sender, RoutedEventArgs e)
        {
            try 
            {//trying to bould a connection
                httpManager = await HTTPManager.NewAuthenticated(Adress,int.Parse(Port), PBoxPassword.Password);
                DialogResult = true;
            }
            catch(Exception ex) 
            {//giving feedback to the user, if building a connection failed
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
                    TBlockResponce.Text = "Es ist ein unbekannter Fehler aufgetreten";
                    MessageBox.Show($"{ex.Message},\n{ex.GetType()}");
                }
            }
        }
    }

    /// <summary>
    /// Class for validating ports
    /// </summary>
    partial class PortValidation : ValidationRule
    {
        public PortValidation() { }
        /// <summary>
        /// Function that validates the port
        /// </summary>
        /// <param name="value">The port to validate</param>
        /// <returns>The <see cref="ValidationResult"/></returns>
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
