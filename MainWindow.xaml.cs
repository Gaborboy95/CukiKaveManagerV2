using CukiKaveManagerV2.UserControls;
using CukiKaveManagerV2.ws;
using CukiKaveManagerV2.ws.JSONObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace CukiKaveManagerV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Localization
        private string locale = "hu-HU";

        // SERVER URLS
        //public const string ADDRESS = "cukikave.herokuapp.com"; // This is easier to change
        public const string ADDRESS = "localhost"; // This is easier to change
        // Websocket URL
        const string WS_ADDRESS = "ws://"+ADDRESS; //MAIN SERVER
        
        // Product types
        public string[] productTypes = { "Torta", "Magyarország tortái", "Forróital", "Ital", "Sütemény" };
        // Login file
        const string HASH_FILE = "auth.hash";
        // Product list for easier management
        public Dictionary<int, flippable> Products = new Dictionary<int, flippable>();
        // This is for the dialogs
        public int currentPrompt = -1;
        // Websocket declaration
        public GenericWebSocket ws;

        public MainWindow()
        {
            InitializeComponent();

            // Set localization
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(locale);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(locale);

            LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(locale)));
        }

        public async Task<bool> ReloadUI()
        {
            ItemHolder.Children.Clear();
            var products = await cukiAPI.GetProducts();

            foreach (var _product in products)
            {
                // Create flippable
                var flp = new flippable(_product, this);

                // Add to containers
                ItemHolder.Children.Add(flp);
                Products.Add(_product.id, flp);

                // Add button click handlers
                deleteButton.Click += (object sndr, RoutedEventArgs arg) => flp.deleteConfirm(currentPrompt);
                updateConfirmButton.Click += (object sndr, RoutedEventArgs arg) => flp.updateConfirm(currentPrompt);
                updateRollbackButton.Click += (object sndr, RoutedEventArgs arg) => flp.rollbackChanges(currentPrompt);
            }

            loadingBar.Visibility = Visibility.Hidden;
            mainContent.Visibility = Visibility.Visible;
            return true;
        }

        private async void windowLoaded(object sender, RoutedEventArgs e)
        {
            string hash = null;
            if (File.Exists(HASH_FILE))
            {
                var file = File.ReadAllText(HASH_FILE);
                hash = file;
            }

            cukiAPI.hash = hash;

            // Combobox thing
            addProductType.Items.Clear();
            foreach (string option in productTypes)
                addProductType.Items.Add(option);

            ws = new GenericWebSocket(new Uri(WS_ADDRESS + "/" + hash));
            ws.OnWebSocketConnected += onWSConnected;
            await ws.Connect();
            new WebSocketHandler(ws, this);

        }

        private async void onWSConnected(object sender, EventArgs e)
        {
            await Dispatcher.InvokeAsync<Task<bool>>(ReloadUI);
        }

        private void openNewProductDialog(object sender, RoutedEventArgs e)
        {
            addNewProduct.IsOpen = true;
            addDatePick.SelectedDate = DateTime.Now;
        }

        private async void addNewItemClick(object sender, RoutedEventArgs e)
        {
            addNewProduct.IsOpen = false;
            int _price;
            if (addTitle.Text == "" || new TextRange(addDesc.Document.ContentStart, addDesc.Document.ContentEnd).Text == "" || !int.TryParse(addPrice.Text, out _price) || addProductType.SelectedItem == null )
            {
                inputMissing.IsOpen = true;
            }
            else
            {
                //Success, add the new item to the database
                Product newPrd = new Product(int.Parse(addPrice.Text), addTitle.Text, 999, new TextRange(addDesc.Document.ContentStart, addDesc.Document.ContentEnd).Text, addImagePath.Text, parseType(((ComboBoxItem)addProductType.SelectedItem).Content.ToString()), addDatePick.DisplayDate);

                await cukiAPI.SendNewProduct(newPrd);

            }
        }

        public string parseType(string _ps)
        {
            switch (_ps)
            {
                case "Torta":
                    return "cake";
                case "Magyarország tortái":
                    return "huncake";
                case "Forróital":
                    return "hotdrinks";
                case "Ital":
                    return "drinks";
                case "Sütemény":
                    return "bakedgoods";
                default:
                    return "parseerror";
            }
        }
    }
}
