using MauiAssignment.Models;
using MauiAssignment.Services;
using Syncfusion.Maui.Core.Carousel;
using System.Xml.Linq;
using ThirdParty.Json.LitJson;
using Syncfusion.Maui.DataGrid;
using Microsoft.Maui.Controls;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Syncfusion.Maui.Core.Converters;

namespace MauiAssignment
{
    public partial class MainPage : ContentPage
    {
        private readonly MongoDBService _mongoService;
        private readonly IConfiguration _configuration;

        public MainPage()
        {
            _mongoService = new MongoDBService(Common.DatabaseConnection.ConnectionString, Common.DatabaseConnection.DatabaseName, Common.DatabaseConnection.CollectionName);
            InitializeComponent();
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            try
            {
                string xmlFilePath = Common.DatabaseConnection.xmlFilePath;
                var common = new Common.Common();
                var xmlData = common.LoadXmlfile(xmlFilePath);
                var jsonData = common.ConvertXmlToJsonfile(xmlData);

                await _mongoService.SaveJsonToMongoAsync(jsonData);
                var viewModel = await _mongoService.GetAllDocumentsAsync();
                dataGrid.ItemsSource = viewModel;
                dataGrid.AutoExpandGroups = true;

                UpdateStatus("Added completed successfully!", Colors.Green);
                clear();
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error Add data: {ex.Message}", Colors.Red);
            }
        }

        private void dataGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedRows.Count > 0)
            {
                var selectedItem = dataGrid.SelectedRows[0];
                txtMachineKeyValue.Text = GetPropertyValue(selectedItem, "MachineKey");
                txtStemKeyValue.Text = GetPropertyValue(selectedItem, "StemKey");
                txtStemNumber.Text = GetPropertyValue(selectedItem, "StemNumber");
                txtreceiverPosition.Text = GetPropertyValue(selectedItem, "receiverPosition");
                txtcoordinateReferenceSystem.Text = GetPropertyValue(selectedItem, "coordinateReferenceSystem");
                txtLatitude.Text = GetPropertyValue(selectedItem, "Latitude");
                txtLongitude.Text = GetPropertyValue(selectedItem, "Longitude");
                txtAltitude.Text = GetPropertyValue(selectedItem, "Altitude");
            }

        }

        private string GetPropertyValue(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            return property?.GetValue(obj)?.ToString() ?? string.Empty;
        }

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtStemNumber.Text))
                {
                    UpdateStatus("Please select a row in the grid.", Colors.Red);
                    return;
                }

                StemData stemData = new StemData();
                stemData = CreateStemDataFromInput();
                await _mongoService.UpdateJsonToMongoAsync(stemData);
                UpdateStatus("Update completed successfully!", Colors.Green);
                var viewModel = await _mongoService.GetAllDocumentsAsync();
                dataGrid.ItemsSource = viewModel;
                dataGrid.AutoExpandGroups = true;
                clear();
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error updating data: {ex.Message}", Colors.Red);
            }
        }
           
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtStemNumber.Text))
                {
                    UpdateStatus("Please select a row in the grid.", Colors.Red);
                    return;
                }

                StemData stemData = new StemData();
                stemData = CreateStemDataFromInput();
                await _mongoService.DeleteStemAsync(stemData);
                UpdateStatus("Delete completed successfully!", Colors.Green);
                var viewModel = await _mongoService.GetAllDocumentsAsync();
                dataGrid.ItemsSource = viewModel;
                dataGrid.AutoExpandGroups = true;
                clear();
            }
            catch (Exception ex)
            {

                UpdateStatus($"Error updating data: {ex.Message}", Colors.Red);
            }
        }
        private async void OnClearClicked(object sender, EventArgs e)
        {
           try
            {
                dataGrid.ItemsSource = null;
                dataGrid.AutoExpandGroups = true;
                StatusLabel.Text = "";
                clear();
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error Clear data: {ex.Message}", Colors.Red);
            }
        }
        private StemData CreateStemDataFromInput()
        {
            return new StemData
            {
                MachineKey = txtMachineKeyValue.Text,
                StemKey = txtStemKeyValue.Text,
                StemNumber = txtStemNumber.Text,
                receiverPosition = txtreceiverPosition.Text,
                coordinateReferenceSystem = txtcoordinateReferenceSystem.Text,
                Latitude = Convert.ToDecimal(txtLatitude.Text),
                Longitude = Convert.ToDecimal(txtLongitude.Text),
                Altitude = Convert.ToDecimal(txtAltitude.Text)
            };
        }

        private void UpdateStatus(string message, Color color)
        {
            StatusLabel.Text = message;
            StatusLabel.TextColor = color;
        }
        void clear()
        {
            txtMachineKeyValue.Text = ""; 
            txtStemKeyValue.Text = "";
            txtStemNumber.Text = ""; ;
            txtreceiverPosition.Text = ""; 
            txtcoordinateReferenceSystem.Text = ""; 
            txtLatitude.Text = ""; 
            txtLongitude.Text = ""; 
            txtAltitude.Text = ""; 
        }

    }

}