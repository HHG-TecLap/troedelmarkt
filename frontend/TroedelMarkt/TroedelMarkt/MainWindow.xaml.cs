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
        public List<TransactionItem> Transactions { get; set; } //Epty at programm start( still filled for debug)
        public List<string> TraderIDs { get; set; } //needs to be filld with proper API data (currently silled for debug)

        public HTTPManager hTTPManager { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            Transactions = new List<TransactionItem>();
            DataContext = Transactions;
            TraderIDs = new List<string>();
            DataContext = TraderIDs;
            for ( int i = 0; i<10; i++ )
            {
                Transactions.Add(new TransactionItem($"TraderNum{i}", i * 11m - i / 13m ));
            }

            LoginWindow lgin = new LoginWindow(); //Login 
            var result = lgin.ShowDialog();
            if (result != true)
            {
                System.Windows.Application.Current.Shutdown();
            }
            hTTPManager = lgin.httpManager;
            

            updateTraderList();
            updateSumm();
        }

        private void BtnDeleteElement_Click(object sender, RoutedEventArgs e)
        {
            if(LbTransactions.SelectedIndex != -1)
                Transactions.RemoveAt(LbTransactions.SelectedIndex);
            LbTransactions.Items.Refresh();
            updateSumm();
        }

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
                Transactions.Add(new TransactionItem(null, 0m)); 
            }
            LbTransactions.Items.Refresh();
            updateSumm();
            LbTransactions.SelectedIndex = Transactions.Count - 1;
        }

        private void BtnExitTransaction_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Sind Sie sicher?\nAlle eingetragenen Elemente werden entfernt.", "Elemente entfernen", MessageBoxButton.OKCancel, MessageBoxImage.Warning,MessageBoxResult.Cancel);
            if (result is MessageBoxResult.OK) 
            {
                Transactions.Clear();
                LbTransactions.Items.Refresh();
                updateSumm();
            }
            
        }

        private void updateSumm()
        {
            decimal summ = 0m;
            foreach (TransactionItem trans in  Transactions ){
                summ += trans.Value;
            }
            TBlockSumm.Text = $"Summe: {summ.ToString("c")}";
        }

        private void LbTransctionsCange(object sender, SelectionChangedEventArgs e)
        {
            updateSumm();
        }

        private void TbUpdateBinding(object sender, TextChangedEventArgs e)
        {
            (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
            updateSumm();
        }

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
                    updateTraderList();
                    updateSumm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Es ist ein Fehler aufgetreten\n{ex.Message}");
                }
            }
        }

        private void BtnTraderView_Click(object sender, RoutedEventArgs e)
        {
            Window1 wind = new Window1(hTTPManager);
            wind.Owner= this;
            wind.Show();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            updateTraderList();
            updateSumm();
        }

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
    }
    partial class TraderIDValidation : ValidationRule
    {
        public string pattern { get; set; }
        public TraderIDValidation() { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex alphaNum = new Regex(pattern);//
            string input = value as string;
            if (alphaNum.IsMatch(input))
            {
                return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "illegal input");
        }
    }
}
