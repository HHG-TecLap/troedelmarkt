using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using TroedelMarkt;

namespace TroedelMarkt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<TransactionItem> Transactions { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Transactions= new List<TransactionItem>();
            DataContext = Transactions;
            for ( int i = 0; i<10; i++ )
            {
                Transactions.Add(new TransactionItem($"TraderNum{i}", i * 11m - i / 13m ));
            }
            updateSumm();
            LoginWindow lgin = new LoginWindow();
            var result = lgin.ShowDialog();
            if ( result != true) 
            {
                System.Windows.Application.Current.Shutdown();
            }
            
            
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
                    Value = decimal.Parse(TBoxElementValue.Text);
                }
                catch (Exception) { }
                Transactions.Add(new TransactionItem(TBoxTraderID.Text, 0m));
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

        private void BtnMakeTRansaction_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Sind Sie sicher!\nDie Transaktion kann nicht rückgängig gemacht werden.", "Transaktion durchführen", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk, MessageBoxResult.Cancel);
            if (result is MessageBoxResult.OK)
            {
                //API send request and validete / inform user when failed 
                throw new System.NotImplementedException();
            }
        }

        private void BtnTraderView_Click(object sender, RoutedEventArgs e)
        {
            Window1 wind = new Window1();
            wind.Owner= this;
            wind.Show();
        }
    }
}
