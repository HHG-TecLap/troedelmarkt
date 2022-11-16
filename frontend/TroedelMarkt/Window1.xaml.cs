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

namespace TroedelMarkt
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        List<Trader> Traders { get; set; }
        public Window1()
        {
            InitializeComponent();
            Traders= new List<Trader>();
            for ( int i = 0; i<10; i++)
            {
                Traders.Add(new Trader($"TraderNums: {i}",$"Name{i}", i + 100m + i / 3m,i/3m+i*2m, i/10m, (i + 100m + i / 3m)*i/10m));
            }
            DataContext = Traders;
            updateStatistics();
        }

        private void BtnAddTdr_Click(object sender, RoutedEventArgs e)
        { // API update database
            if ( TBoxTraderID.Text != "")
            {
                Traders.Add(new Trader(TBoxTraderID.Text, "", 0m, 0m, 0m, 0m));
                TBoxTraderID.Text = "";
            }
            DGTrader.Items.Refresh();
            updateStatistics();
        }

        private void BtnDelTdr_Click(object sender, RoutedEventArgs e)
        {
            if (DGTrader.SelectedIndex != -1)
            { //API update database
                var result = MessageBox.Show("Diese aktion kann nicht rückgängig gemacht werden!", "Händler löschen", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    foreach (var tdr in DGTrader.SelectedItems)
                        Traders.Remove(tdr as Trader);
                    DGTrader.Items.Refresh();
                }
            }
            updateStatistics();
        }

        private void updateStatistics()
        {
            decimal summ = 0;
            decimal provSumm = 0;
            foreach( Trader tdr in Traders)
            {
                summ += tdr.Balance;
                provSumm += tdr.Provision;
            }
            TBlockSumm.Text = $"Summe: {summ.ToString("C")}";
            TBlockProvSumm.Text = $"Provisions Summe: {provSumm.ToString("C")}";
        }

        private void DGSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            updateStatistics();
        }

        private void DGCellEditEnd(object sender, DataGridCellEditEndingEventArgs e)
        {
            TBlockDebug.Text = "edditEnded"; //API update database
            
        }
    }
}
