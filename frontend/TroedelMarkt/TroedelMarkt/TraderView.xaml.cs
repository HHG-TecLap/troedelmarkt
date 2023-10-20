using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        /// The <see cref="HTTPManager"/> for handling communication with the server
        /// </summary>
        public HTTPManager hTTPManager { get; set; }
        /// <summary>
        /// Wether the Window is active or not 
        /// </summary>
        public bool active { get; set; }

        /// <summary>
        /// Constructir for initialising the TraderView
        /// </summary>
        /// <param name="httpMan">The <see cref="HTTPManager"/> for cummunication with the server</param>
        public TraderView(HTTPManager httpMan)
        {
            InitializeComponent();//Initialising Window
            //setting default data
            Traders = new List<Trader>();
            newTraderID = "";
            //preparing variables for databinding
            DataContext = Traders;
            DataContext = newTraderID;
            //getting the HttpManager
            hTTPManager = httpMan;
            //Updating data from server
            updateData();
            //mark as active
            active = true;
        }

        /// <summary>
        /// Function for handling clicks of the AddTraderButton
        /// </summary>
        private async void BtnAddTdr_Click(object sender, RoutedEventArgs e)
        {
            Regex alphanum = new Regex(@"^[a-zA-Z0-9]*$");
            if (newTraderID != "" && alphanum.IsMatch(TBoxTraderID.Text))
            {//chacking, wether the traderID is alphanumeric
                if (Traders.Find(t => t.TraderID == newTraderID) == null)
                {//chacking, wehter the ID already exists
                    try
                    {//trying to create the new Trader
                        await hTTPManager.CreateNewTrader(newTraderID, "", null);
                        TBoxTraderID.Text = "";
                    }
                    catch (Exception ex)
                    { //Showing warning if creation failed
                        MessageBox.Show($"Es ist ein fehler aufgetreten.\n{ex.Message}", "Händler hinzufügen", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {//Showing warning if the ID already exists
                    MessageBox.Show("Die ID ist bereits vorhanden", "Ungültige ID", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            //updating data 
            updateData();
        }

        /// <summary>
        /// Function for hadeling clicks on the DeleteTraderButton
        /// </summary>
        private async void BtnDelTdr_Click(object sender, RoutedEventArgs e)
        {
            if (DGTrader.SelectedIndex != -1)
            { //if a trader is selected show a warning
                var result = MessageBox.Show($"Diese Aktion kann nicht rückgängig gemacht werden!\nEs werden {DGTrader.SelectedItems.Count} Händler unwiederuflich gelöscht.",
                    "Händler löschen", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {//if deletion is aproved tryes to delete every selected trader
                    foreach (Trader tdr in DGTrader.SelectedItems)
                    {
                        try
                        {
                            await hTTPManager.DeleteTrader(tdr.TraderID);
                        }
                        catch (Exception ex)
                        {
                            if (ex is DeletionOrderException)
                            {//shpwes a warning when the trader can not be deleted
                                result = MessageBox.Show($"Der Händler {tdr.TraderID} kann nicht geslöscht werden, da seine Bilanz nicht 0 ist.", "Löschen fehlgeschlagen", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                                if (result == MessageBoxResult.Cancel)
                                {
                                    return;
                                }
                            }

                        }
                    }
                }
            }
            //Updating data and refreshing databinding
            updateData();
            DGTrader.Items.Refresh();
            updateStatistics();
        }

        /// <summary>
        /// Function for handling SelectionChanged events on the Trader Datagrid
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
            //enabeling Button for syncong changes
            BtnUpdateTraders.IsEnabled = true;
            //marking changed Treaders visualy
            e.Row.Background = Brushes.Chartreuse;
        }

        /// <summary>
        /// Function for handling of Exporting data as CSV
        /// </summary>
        private async void ExportCSV_Click(object sender, RoutedEventArgs e)
        {
            //getting path to save the file to
            Microsoft.Win32.SaveFileDialog saveDia = new Microsoft.Win32.SaveFileDialog();
            saveDia.FileName = "Export";
            saveDia.AddExtension = true;
            saveDia.DefaultExt = ".csv";
            saveDia.Filter = "Comma-separated values (.csv) |*.csv";
            var result = saveDia.ShowDialog(this);
            if (result == true)
            {//when a path was given
                try
                {//Saving the file
                    FileStream file = File.Create(saveDia.FileName);
                    var stream = await hTTPManager.ExportCsv();
                    stream.CopyTo(file);
                    file.Close();
                }
                catch (Exception ex)
                {//notefying of an error
                    MessageBox.Show($"Es ist ein Fehler aufgetreten.\n{ex.Message}", "Daten exportieren", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// Function for handling clicks on the UpdateDataButton
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
            {//updating traderlists and refreshing databindungs
                Traders.Clear();
                Traders.AddRange(await hTTPManager.GetAllTraders());
                DGTrader.Items.Refresh();
                updateStatistics();
                BtnUpdateTraders.IsEnabled = false;
            }
            catch (Exception ex) 
            { //showing a warning when an error ocurres
                MessageBox.Show($"Es ist eiin Fehler aufgetreten\n{ex.Message}", "Händler aktualisieren", MessageBoxButton.OK, MessageBoxImage.Information); 
            }
        }

        /// <summary>
        ///Updates statistics shown on screen 
        /// </summary>
        private void updateStatistics()
        {
            //aclculating statistics
            decimal summ = 0;
            decimal provSumm = 0;
            foreach (Trader tdr in Traders)
            {
                summ += tdr.Balance;
                provSumm += tdr.Provision;
            }
            //setting statistics to textFields
            TBlockSumm.Text = $"Summe: {summ.ToString("C")}";
            TBlockProvSumm.Text = $"Provisions Summe: {provSumm.ToString("C")}";
        }

        /// <summary>
        /// Function for handling Clocks on the UpdateTRadersButton
        /// </summary>
        private async void BtnUpdateTraders_Click(object sender, RoutedEventArgs e)
        {
            if (DGTrader.SelectedIndex != -1)
            { //asking for confirmation on updatng traders
                var result = MessageBox.Show($"Es werden {DGTrader.SelectedItems.Count} Händler aktualisiert.",
                    "Händler löschen", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                if (result == MessageBoxResult.OK)
                {//updating changed traders
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
            //refreshing data
            updateData();
            DGTrader.Items.Refresh();
            updateStatistics();
            BtnUpdateTraders.IsEnabled = false;
        }

        /// <summary>
        /// Function for handling the closing of the window
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //marking the window as inactive
            active = false;
        }
    }
}
