using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TroedelMarkt;

namespace TroedelMarkt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// List of <see cref="TransactionItem"/> to sell
        /// </summary>
        public List<TransactionItem> Transactions { get; set; }
        /// <summary>
        /// List of <see cref="Trader.TraderID"/>s 
        /// </summary>
        public List<string> TraderIDs { get; set; } 
        /// <summary>
        /// <see cref="HTTPManager"/> for handling communication with the server
        /// </summary>
        public HTTPManager hTTPManager { get; set; }
        /// <summary>
        /// The <see cref="TraderView"/> window
        /// </summary>
        private TraderView wind1;
        /// <summary>
        /// Constructor for initialising the MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Transactions = new List<TransactionItem>();
            DataContext = Transactions;
            TraderIDs = new List<string>();
            DataContext = TraderIDs;


            LoginWindow lgin = new LoginWindow(); //Login 
            var result = lgin.ShowDialog();
            if (result != true)
            {
                System.Windows.Application.Current.Shutdown();
            }
            hTTPManager = lgin.httpManager;

            Transactions.Add(new TransactionItem("", 0m));
            updateTraderList();
            updateSumm();
            LbTransactions.SelectedIndex = 0;

            wind1 = null;
        }

        /// <summary>
        /// Function for handling clicks on the DeleteElementButton
        /// </summary>
        private void BtnDeleteElement_Click(object sender, RoutedEventArgs e)
        {
            if(LbTransactions.SelectedIndex != -1 && LbTransactions.SelectedIndex < Transactions.Count)
                Transactions.RemoveAt(LbTransactions.SelectedIndex);
            if(Transactions.Count < 1)
            {
                Transactions.Add(new TransactionItem("", 0m));
                LbTransactions.SelectedIndex = 0;
            }
            LbTransactions.Items.Refresh();
            updateSumm();
        }

        /// <summary>
        /// Function for handling clicks on the AddElementButton
        /// </summary>
        private void BtnAddElement_Click(object sender, RoutedEventArgs e)
        {
            if(LbTransactions.SelectedIndex == -1)
            {
                decimal Value = 0m;
                try
                {
                    Value = decimal.Parse(TBoxElementValue.Text,System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception ex) { }
                Transactions.Add(new TransactionItem(CBTraderID.Text, 0m));
            }
            else 
            { 
                Transactions.Add(new TransactionItem("", 0m)); 
                LbTransactions.SelectedIndex = Transactions.Count;
            }
            LbTransactions.Items.Refresh();
            updateSumm();
            LbTransactions.SelectedIndex = Transactions.Count - 1;
            CBTraderID.Focus();
        }

        /// <summary>
        /// Function for handling click on the ExitTransactionButton
        /// </summary>
        private void BtnExitTransaction_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Sind Sie sicher?\nAlle eingetragenen Elemente werden entfernt.", "Elemente entfernen", MessageBoxButton.OKCancel, MessageBoxImage.Warning,MessageBoxResult.Cancel);
            if (result is MessageBoxResult.OK) 
            {
                Transactions.Clear();
                Transactions.Add(new TransactionItem("", 0m));
                LbTransactions.SelectedIndex = 0;
                LbTransactions.Items.Refresh();
                updateSumm();
            }
            
        }

        /// <summary>
        /// Updating the summ of all <see cref="TransactionItem"/>s
        /// </summary>
        private void updateSumm()
        {
            decimal summ = 0m;
            foreach (TransactionItem trans in  Transactions ){
                summ += trans.Value;
            }
            TBlockSumm.Text = $"Summe: {summ.ToString("c")}";
        }

        /// <summary>
        /// handling changes of the <see cref="TransactionItem"/> selection
        /// </summary>
        private void LbTransctionsCange(object sender, SelectionChangedEventArgs e)
        {
            updateSumm();
        }

        /// <summary>
        /// Function for handling changes of the value of the ElementValueTextbox
        /// </summary>
        private void TbUpdateBinding(object sender, TextChangedEventArgs e)
        {
            //(sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
            updateSumm();
        }

        /// <summary>
        /// Function for handling click of Make TransactionButton
        /// </summary>
        private async void BtnMakeTRansaction_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Sind Sie sicher!\nDie Transaktion kann nicht rückgängig gemacht werden.", "Transaktion durchführen", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk, MessageBoxResult.Cancel);
            if (result is MessageBoxResult.OK)
            {
                foreach (TransactionItem trans in Transactions)
                {
                    if (TraderIDs.Find(x => x == trans.Trader) == null)
                    {
                        MessageBox.Show("Einige Händler IDs sind nicht korrekt.\nBitte überprüfen sie die Eingabe", "Eingabe fehlerhaft", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                }
                try
                {
                    await hTTPManager.SellItems(Transactions);
                    Transactions.Clear();
                    Transactions.Add(new TransactionItem("", 0m));
                    LbTransactions.SelectedIndex = 0;
                    LbTransactions.Items.Refresh();
                    updateSumm();
                    wind1.updateData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Es ist ein Fehler aufgetreten\n{ex.Message}","Bei Transaktion durchführen");
                }
            }
        }

        /// <summary>
        /// Function for handling click of the TraderViewButton
        /// </summary>
        private void BtnTraderView_Click(object sender, RoutedEventArgs e)
        {
            if(wind1 == null)
            {
                wind1 = new TraderView(hTTPManager);
                wind1.Owner = this;
                wind1.Show();
            }
            else if (wind1.active == false)
            {
                wind1 = new TraderView(hTTPManager);
                wind1.Owner = this;
                wind1.Show();
            }
            else
            {
                wind1.WindowState= WindowState.Normal;
            }
            
        }

        /// <summary>
        /// Function for handling click of UpdateButton
        /// </summary>
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            updateTraderList();
            updateSumm();
        }

        /// <summary>
        /// Updating TraderList
        /// </summary>
        public async void updateTraderList()
        {
            try
            {
                List<Trader> tdrs = new List<Trader>(await hTTPManager.GetAllTraders());
                TraderIDs.Clear();
                foreach (Trader tdr in tdrs)
                {
                    TraderIDs.Add(tdr.TraderID);
                }
                TraderIDs.Sort();
                CBTraderID.Items.Refresh();
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// Changes Focus to TBoxElementValue on pressing Return in CBTraderID
        /// </summary>
        private void CB_keyDown(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.Enter)
            {
                TBoxElementValue.Focus();
            }
        }

        /// <summary>
        /// Changes Focus to BTHAddElement on pressing Return in TBValue
        /// </summary>
        private void TBValue_keyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                BtnAddElement.Focus();
            }
        }


        private void TbValue_gotFocus(object sender, RoutedEventArgs e)
        {
            TBoxElementValue.SelectAll();
        }

        /// <summary>
        /// updates the TraderList CBTraderID gets focus
        /// </summary>
        private void CBTraderID_gotFocus(object sender, RoutedEventArgs e)
        {
            updateTraderList();
        }
    }
    /// <summary>
    /// A Class used to validate TraderIDs
    /// </summary>
    partial class TraderIDValidation : ValidationRule
    {
        /// <summary>
        /// The regex pattern the ID is validated with
        /// </summary>
        public string pattern { get; set; }
        //public bool checkIDExists { get; set; }
        //public TraderIDValidationWrapper Wrapper { get; set; }
        
        /// <summary>
        /// Method for validating the TraderID
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns>The result of the Validation</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex alphaNum = new Regex(pattern);
            string input = value as string;
            if (alphaNum.IsMatch(input))
            {
                /*if(checkIDExists)
                {
                    if(Wrapper.TraderIDs.Find(t => t == input) == null)
                    {
                        return new ValidationResult(false, "Trader ID not found");
                    }
                }*/
                return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "illegal input");
        }
    }
    /*public class TraderIDValidationWrapper : DependencyObject
    {
        public DependencyProperty TraderIDsProperty = DependencyProperty.Register("TraderIDs", typeof(List<string>), typeof(TraderIDValidationWrapper));
        public List<string> TraderIDs
        {
            get { return (List<string>)GetValue(TraderIDsProperty); }
            set { SetValue(TraderIDsProperty, value);}
        }
    }*/
}
