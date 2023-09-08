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
    /// Interactionligic for TraderView.xaml
    /// </summary>
    public partial class TraderView : Window
    {
        /// <summary>
        /// The list of <see cref="Trader"/>s
        /// </summary>
        public List<Trader> Traders { get; set; } //Traders get pullt from API
        /// <summary>
        /// The <see cref="Trader.TraderID"/> for creating a new <see cref="Trader"/>
        /// </summary>
        public string newTraderID { get; set; }
        /// <summary>
        /// The <see cref="HTTPManager"/> for handeling communication with the server
        /// </summary>
        public HTTPManager hTTPManager { get; set; }
        /// <summary>
        /// Wether the Window is active or not 
        /// </summary>
        public bool active { get; set; }
        
        /// <summary>
        /// Constructir for initialising the TraderView
        /// </summary>
        /// <param name="httmMan">The <see cref="HTTPManager"/> for cummunication with the server</param>
        public TraderView( HTTPManager httmMan)
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

        /// <summary>
        /// Function for handeling clicks of the AddTraderButton
        /// </summary>
        private async void BtnAddTdr_Click(object sender, RoutedEventArgs e)
        { // API update database
            Regex alphanum = new Regex(@"^[a-zA-Z0-9]*$");
            if ( newTraderID != "" && alphanum.IsMatch(TBoxTraderID.Text))
            {
                if (Traders.Find(t => t.TraderID == newTraderID) == null)
                {
                    try 
                    {
                        await hTTPManager.CreateNewTrader(newTraderID, "",null);
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

        /// <summary>
        /// Function for hadeling clicks on the DeleteTraderButton
        /// </summary>
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

        /// <summary>
        /// Function for handeling SelectionChanged events on the Trader Datagrid
        /// </summary>
        private void DGSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            updateData();
        }

        /// <summary>
        /// Function for visualising changed data in the Trader Datagrid
        /// </summary>
        private void DGCellEditEnd(object sender, DataGridCellEditEndingEventArgs e)
        {
            BtnUpdateTraders.IsEnabled = true;
            e.Row.Background = Brushes.Chartreuse;
        }

        /// <summary>
        /// Function for handeling of Exporting data as CSV
        /// </summary>
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

        /// <summary>
        /// Function for handeling clicks on the UpdateDataButton
        /// </summary>
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

        /// <summary>
        /// Function for handeling Clocks on the UpdateTRadersButton
        /// </summary>
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

        /// <summary>
        /// Function for handeling the closing of the window
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            active = false;
        }
    }
}
