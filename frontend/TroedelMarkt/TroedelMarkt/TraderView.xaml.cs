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
using System.Xml.Linq;

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
            InitializeComponent();//Initialising Window
            //setting default data
            Traders = new List<Trader>();
            newTraderID = "";
            //preparing variables for databinding
            DataContext = Traders;
            DataContext = newTraderID;
            //getting the HttpManager
            hTTPManager = httmMan;
            //Updating data from server
            updateData();
            //mark as active
            active = true;
        }

        /// <summary>
        /// Function for handeling clicks of the AddTraderButton
        /// </summary>
        private async void BtnAddTdr_Click(object sender, RoutedEventArgs e)
        {
            //Checking for alphanumeric Trader ISs
            Regex alphanum = new Regex(@"^[a-zA-Z0-9]*$");
            if ( newTraderID != "" && alphanum.IsMatch(TBoxTraderID.Text))
            {//Checking for existing IDs
                if (Traders.Find(t => t.TraderID == newTraderID) == null)
                {
                    try 
                    {//adding trader
                        await hTTPManager.CreateNewTrader(newTraderID, "",null,null);
                        TBoxTraderID.Text = "";
                    }
                    catch (Exception ex)
                    { //Showing warning if creation failed
                        if (ex.GetType() == typeof(DuplicateException))
                        {
                            MessageBox.Show("Die ID ist bereits vorhanden", "Ungültige ID", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            MessageBox.Show($"Es ist ein fehler aufgetreten.\n{ex.Message}", "Händler hinzufügen", MessageBoxButton.OK, MessageBoxImage.Warning);
                        } 
                    }
                }
                else
                {//Showing warning if the ID already exists
                    MessageBox.Show("Die ID ist bereits vorhanden", "Ungültige ID",MessageBoxButton.OK,MessageBoxImage.Error);
                }
            }
            //updating data
            updateData();
        }

        /// <summary>
        /// Function for hadeling clicks on the DeleteTraderButton
        /// </summary>
        private async void BtnDelTdr_Click(object sender, RoutedEventArgs e)
        {//if a trader is selected show a warning
            if (DGTrader.SelectedIndex != -1)
            { //API update database
                var result = MessageBox.Show($"Diese Aktion kann nicht rückgängig gemacht werden!\nEs werden {DGTrader.SelectedItems.Count} Händler unwiederuflich gelöscht.",
                    "Händler löschen", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {//if deletion is aproved tryes to delete every selected trader
                    foreach (Trader tdr in DGTrader.SelectedItems) {
                        try
                        {
                            await hTTPManager.DeleteTrader(tdr.TraderID);
                        }
                        catch (Exception ex)
                        {//showes a warning when the trader can not be deleted
                            if (ex is DeletionOrderException)
                            {
                                result = MessageBox.Show($"Der Händler {tdr.TraderID} kann nicht geslöscht werden, da seine Bilanz nicht 0€ ist.", "Löschen fehlgeschlagen", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                                if( result == MessageBoxResult.Cancel) 
                                {
                                    return;
                                }
                            }
                            else if(ex is NotFoundException)
                            {
                                result = MessageBox.Show($"Der Händler {tdr.TraderID} kann nicht geslöscht werden, da er nicht existiert.", "Löschen fehlgeschlagen", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                                if (result == MessageBoxResult.Cancel)
                                {
                                    return;
                                }
                            }
                            else
                            {
                                result = MessageBox.Show($"Der Händler {tdr.TraderID} kann nicht geslöscht werden.\n {ex.Message}", "Löschen fehlgeschlagen", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                                if (result == MessageBoxResult.Cancel)
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            //Updating data
            updateData();
            DGTrader.Items.Refresh();
        }

        /// <summary>
        /// Function for handeling SelectionChanged events on the Trader Datagrid
        /// </summary>
        private void DGSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            updateData();
        }

        /// <summary>
        /// /// Function for visualising changed data in the Trader Datagrid
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
        {//setting up dialog for getting file path
            Microsoft.Win32.SaveFileDialog saveDia = new Microsoft.Win32.SaveFileDialog();
            saveDia.FileName = "Export";
            saveDia.AddExtension = true;
            saveDia.DefaultExt = ".csv";
            saveDia.Filter = "Comma-separated values (.csv) |*.csv";
            var hasReturnedPath = saveDia.ShowDialog(this);
            if(hasReturnedPath.GetValueOrDefault())
            {//when a path was given
                try
                {//saving file
                    FileStream file = File.Create(saveDia.FileName);
                    var stream = await hTTPManager.ExportCsv();
                    stream.CopyTo(file);
                    file.Close();
                }
                catch (Exception ex)
                { //showing an error message
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
        {//updating traderlists and refreshing databindungs
            try
            {
                Traders.Clear();
                Traders.AddRange(await hTTPManager.GetAllTraders());
                DGTrader.Items.Refresh();
                updateStatistics();
                BtnUpdateTraders.IsEnabled = false;
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Es ist ein Fehler aufgetreten\n{ex.Message}","Händler aktualisieren",MessageBoxButton.OK,MessageBoxImage.Information); 
            }
        }

        /// <summary>
        ///Updates statistics shown on screen 
        /// </summary>
        private void updateStatistics()
        {//calculating statistics
            decimal summ = 0;
            decimal provSumm = 0;
            foreach (Trader tdr in Traders)
            {
                summ += tdr.Balance;
                provSumm += tdr.Provision;
            }
            //setting Values
            TBlockSumm.Text = $"Summe: {summ.ToString("C")}";
            TBlockProvSumm.Text = $"Provisions Summe: {provSumm.ToString("C")}";
        }

        /// <summary>
        /// Function for handeling Clocks on the UpdateTRadersButton
        /// </summary>
        private async void BtnUpdateTraders_Click(object sender, RoutedEventArgs e)
        {
            if (DGTrader.SelectedIndex != -1)
            { //asking for confirmation on updatng traders
                var result = MessageBox.Show($"Es werden {DGTrader.SelectedItems.Count} Händler aktualisiert.",
                    "Händler aktualisieren", MessageBoxButton.OKCancel, MessageBoxImage.Information);
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
                            if(ex is NotFoundException)
                            {
                                MessageBox.Show($"Der Händler {tdr.TraderID} kann nicht aktualisiert werden, da er nicht existiert.", "Aktualisieren fehlgeschlagen", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                            }
                            else
                            {
                                MessageBox.Show($"Der Händler {tdr.TraderID} kann nicht aktualisiert werden.\n{ex.Message}", "Aktualisieren fehlgeschlagen", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                            }
                        }
                    }
                    DGTrader.Items.Refresh();
                }
            }
            updateData();
            DGTrader.Items.Refresh();
            BtnUpdateTraders.IsEnabled = false;
        }

        /// <summary>
        /// Function for handeling the closing of the window
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //marking the window as inactive
            active = false;
        }
    }
}
