using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        private TraderView traderView;
        /// <summary>
        /// Constructor for initialising the MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent(); // initializing Window
            //setting default data and preparing databinding
            Transactions = new List<TransactionItem>();
            DataContext = Transactions;
            TraderIDs = new List<string>();
            DataContext = TraderIDs;
            Transactions.Add(new TransactionItem("", 0m));
            traderView = null;

            //showing login dialog
            LoginWindow lgin = new LoginWindow(); //Login 
            var result = lgin.ShowDialog();
            if (result != true)
            {
                System.Windows.Application.Current.Shutdown();
            }
            hTTPManager = lgin.httpManager;

            //updating data
            updateTraderList();
            updateSumm();
            //selecting firt Transactionitem
            LbTransactions.SelectedIndex = 0;
        }

        /// <summary>
        /// Function for handling clicks on the DeleteElementButton
        /// </summary>
        private void BtnDeleteElement_Click(object sender, RoutedEventArgs e)
        {

            if (LbTransactions.SelectedIndex != -1 && LbTransactions.SelectedIndex < Transactions.Count)
            {//removing transaction item
                Transactions.RemoveAt(LbTransactions.SelectedIndex);
            }   
            if (Transactions.Count < 1)
            {//adding new item if all have been deleted
                Transactions.Add(new TransactionItem("", 0m));
                LbTransactions.SelectedIndex = 0;
            }
            //updating data
            LbTransactions.Items.Refresh();
            updateSumm();
        }

        /// <summary>
        /// Function for handling clicks on the AddElementButton
        /// </summary>
        private void BtnAddElement_Click(object sender, RoutedEventArgs e)
        {
            if (LbTransactions.SelectedIndex == -1)
            {
                //decimal Value = 0m;
                //try
                //{
                //    Value = decimal.Parse(TBoxElementValue.Text, System.Globalization.CultureInfo.InvariantCulture);
                //}
                //catch (Exception ex) { }
                //creating new transactionItem
                Transactions.Add(new TransactionItem(CBTraderID.Text, 0m));
            }
            else
            {
                //creating and selecting new TransactionItem
                Transactions.Add(new TransactionItem("", 0m));
                LbTransactions.SelectedIndex = Transactions.Count;
            }
            //updating data, selecting item and setting focus
            LbTransactions.Items.Refresh();
            updateSumm();
            LbTransactions.SelectedIndex = Transactions.Count - 1;
            CBTraderID.Focus();
        }

        /// <summary>
        /// Function for handling click on the ExitTransactionButton
        /// </summary>
        private void BtnExitTransaction_Click(object sender, RoutedEventArgs e)
        {//asking for approval
            var result = MessageBox.Show("Sind Sie sicher?\nAlle eingetragenen Elemente werden entfernt.", "Elemente entfernen", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);
            if (result is MessageBoxResult.OK)
            {//deleting and updating data
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
        {//updataing summ 
            decimal summ = 0m;
            foreach (TransactionItem trans in Transactions)
            {
                summ += trans.Value;
            }
            //setting value
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
        private async void BtnMakeTransaction_Click(object sender, RoutedEventArgs e)
        {//asking for approval
            var result = MessageBox.Show("Sind Sie sicher!\nDie Transaktion kann nicht rückgängig gemacht werden.", "Transaktion durchführen", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk, MessageBoxResult.Cancel);
            if (result is MessageBoxResult.OK)
            {//if action is approved
                Transactions.RemoveAll(x => x.Trader == "" & x.Value == 0);//remove empty transactionItems
                foreach (TransactionItem trans in Transactions)
                {
                    if (TraderIDs.Find(x => x == trans.Trader) == null)
                    {//warning if traderIDs are not correct
                        MessageBox.Show("Einige Händler IDs sind nicht korrekt.\nBitte überprüfen sie die Eingabe", "Eingabe fehlerhaft", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                }
                try
                {//selling items
                    await hTTPManager.SellItems(Transactions);
                    Transactions.Clear();
                    Transactions.Add(new TransactionItem("", 0m));
                    LbTransactions.SelectedIndex = 0;
                    LbTransactions.Items.Refresh();
                    updateSumm();
                    if(traderView != null)
                    {
                        traderView.updateData();
                    }
                }
                catch (Exception ex)
                {//notefying of an error
                    MessageBox.Show($"Es ist ein Fehler aufgetreten\n{ex.Message}", "Bei Transaktion durchführen",MessageBoxButton.OK,MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Function for handling click of the TraderViewButton
        /// </summary>
        private void BtnTraderView_Click(object sender, RoutedEventArgs e)
        {//opening TraderView window
            if (traderView == null)
            {//when it has not been activated before
                traderView = new TraderView(hTTPManager);
                traderView.Owner = this;
                traderView.Show();
            }
            else if (traderView.active == false)
            {//reactivating
                traderView = new TraderView(hTTPManager);
                traderView.Owner = this;
                traderView.Show();
            }
            else
            {//focusing if already open
                traderView.WindowState = WindowState.Normal;
            }

        }

        /// <summary>
        /// Function for handling click of UpdateButton
        /// </summary>
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {//updating data
            updateTraderList();
            updateSumm();
        }

        /// <summary>
        /// Updating TraderList
        /// </summary>
        public async void updateTraderList()
        {//reloading trader list
            try
            {//getting traders from server
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
            if (e.Key == Key.Enter)
            {
                TBoxElementValue.Focus();
            }
        }

        /// <summary>
        /// Changes Focus to BTHAddElement on pressing Return in TBValue
        /// </summary>
        private void TBValue_keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnAddElement.Focus();
            }
        }

        /// <summary>
        /// Selects the text in the TbValueTextbox on getting focus
        /// </summary>
        private void TbValue_gotFocus(object sender, RoutedEventArgs e)
        {
            TBoxElementValue.SelectAll();
        }

        /// <summary>
        /// updates the TraderList when CBTraderID gets focus
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

        public TraderIDValidation() { }

        /// <summary>
        /// Method for validating the TraderID
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns>The result of the Validation</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {//checking for valid trader IDs
            Regex alphaNum = new Regex(pattern);
            string input = value as string;
            if (alphaNum.IsMatch(input))
            {
                return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "illegal input");
        }
    }
}
