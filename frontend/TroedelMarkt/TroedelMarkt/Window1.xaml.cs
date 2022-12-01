using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public List<Trader> Traders { get; set; } //Traders get pullt from API
        public string newTraderID { get; set; }
        public HTTPManager hTTPManager { get; set; }

        public bool active { get; set; }
        
        public Window1( HTTPManager httmMan)
        {
            InitializeComponent();
            Traders = new List<Trader>();
            newTraderID = "";
            DataContext = Traders;
            DataContext = newTraderID;
            hTTPManager = httmMan;
            updateData();
            active= true;

        }

        private async void BtnAddTdr_Click(object sender, RoutedEventArgs e)
        { // API update database
            Regex alphanum = new Regex(@"^[a-zA-Z0-9]*$");
            if ( newTraderID != "" && alphanum.IsMatch(TBoxTraderID.Text))
            {
                if (Traders.Find(t => t.TraderID == newTraderID) == null)
                {
                    try 
                    {
                        await hTTPManager.CreateNewTrader(newTraderID, "",null,null);
                        TBoxTraderID.Text = "";
                    }
                    catch (Exception ex){ MessageBox.Show($"Es ist ein fehler aufgetreten.\n{ex.Message}","Händler hinzufügen",MessageBoxButton.OK,MessageBoxImage.Warning); }
                }
                else
                {
                    MessageBox.Show("Die ID ist bereits vorhanden", "Ungültige ID",MessageBoxButton.OK,MessageBoxImage.Error);
                }
            }
            updateData();
        }

        private async void BtnDelTdr_Click(object sender, RoutedEventArgs e)
        {
            if (DGTrader.SelectedIndex != -1)
            { //API update database
                var result = MessageBox.Show($"Diese Aktion kann nicht rückgängig gemacht werden!\nEs werden {DGTrader.SelectedItems.Count} Händler unwiederuflich gelöscht.",
                    "Händler löschen", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    foreach (Trader tdr in DGTrader.SelectedItems) {
                        try
                        {
                            await hTTPManager.DeleteTrader(tdr.TraderID);
                        }
                        catch (Exception ex) 
                        {
                            if (ex is DeletionOrderException)
                            {
                                result = MessageBox.Show($"Der Händler {tdr.TraderID} kann nicht geslöscht werden, da seine Bilanz nicht 0 ist.", "Löschen fehlgeschlagen", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                                if( result == MessageBoxResult.Cancel) 
                                {
                                    return;
                                }
                            }
                            
                        }
                    }
                }
            }
            updateData();
            DGTrader.Items.Refresh();
            updateStatistics();
        }

        private void DGSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            updateData();
        }

        /// <summary>
        /// Sends canges to API and updates local data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DGCellEditEnd(object sender, DataGridCellEditEndingEventArgs e)
        {
            BtnUpdateTraders.IsEnabled = true;
            e.Row.Background = Brushes.Chartreuse;
        }

        private async void ExportCSV_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveDia = new Microsoft.Win32.SaveFileDialog();
            saveDia.FileName = "Export";
            saveDia.AddExtension = true;
            saveDia.DefaultExt = ".csv";
            saveDia.Filter = "Comma-separated values (.csv) |*.csv";
            var result = saveDia.ShowDialog(this);
            if(result == true)
            {
                try
                {
                    FileStream file = File.Create(saveDia.FileName);
                    var stream = await hTTPManager.ExportCsv();
                    stream.CopyTo(file);
                    file.Close();
                }
                catch (Exception ex)
                { 
                    MessageBox.Show($"Es ist ein Fehler aufgetreten.\n{ex.Message}","Daten exportieren",MessageBoxButton.OK,MessageBoxImage.Information);
                }
            }
        }

        private void BtnUpdateData_Click(object sender, RoutedEventArgs e)
        {
            updateData();
        }

        /// <summary>
        /// Gets data from API  and updates Traders and statistics.
        /// <seealso cref="updateStatistics"/>
        /// </summary>
        public async void updateData()
        {
            try
            {
                Traders.Clear();
                Traders.AddRange(await hTTPManager.GetAllTraders());
                DGTrader.Items.Refresh();
                updateStatistics();
                BtnUpdateTraders.IsEnabled = false;
            }
            catch (Exception ex) { MessageBox.Show($"Es ist eiin Fehler aufgetreten\n{ex.Message}","Händler aktualisieren",MessageBoxButton.OK,MessageBoxImage.Information); }
        }

        /// <summary>
        ///Updates statistics shown on screen 
        /// </summary>
        private void updateStatistics()
        {
            decimal summ = 0;
            decimal provSumm = 0;
            foreach (Trader tdr in Traders)
            {
                summ += tdr.Balance;
                provSumm += tdr.Provision;
            }
            TBlockSumm.Text = $"Summe: {summ.ToString("C")}";
            TBlockProvSumm.Text = $"Provisions Summe: {provSumm.ToString("C")}";
        }

        private async void BtnUpdateTraders_Click(object sender, RoutedEventArgs e)
        {
            if (DGTrader.SelectedIndex != -1)
            { //API update database
                var result = MessageBox.Show($"Es werden {DGTrader.SelectedItems.Count} Händler aktualisiert.",
                    "Händler löschen", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                if (result == MessageBoxResult.OK)
                {
                    foreach (Trader tdr in DGTrader.SelectedItems)
                    {
                        try
                        {
                            await hTTPManager.UpdateTrader(tdr);
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                    DGTrader.Items.Refresh();
                }
            }
            updateData();
            DGTrader.Items.Refresh();
            updateStatistics();
            BtnUpdateTraders.IsEnabled = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            active = false;
        }
    }
}
