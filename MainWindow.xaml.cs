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
        private string locale = "hu-HU";
        //const string WS_ADDRESS = "ws://localhost"; //DEBUG LOCALHOST
        const string WS_ADDRESS = "ws://cukikave.herokuapp.com"; //MAIN SERVER
        const string HASH_FILE = "auth.hash";
        public Dictionary<int, flippable> Products = new Dictionary<int, flippable>();
        public int currentPrompt = -1;

        public GenericWebSocket ws;

        public MainWindow()
        {
            InitializeComponent();

            
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
                var flp = new flippable(_product, this);
                ItemHolder.Children.Add(flp);
                Products.Add(_product.id, flp);
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

            ws = new GenericWebSocket(new Uri(WS_ADDRESS + "/" + hash));
            ws.OnWebSocketConnected += onWSConnected;
            await ws.Connect();
            new WebSocketHandler(ws, this);

            //Add handler for clicking on the add new product button
            confirmAddNew.Click += ConfirmAddNew_Click;
        }

        private void ConfirmAddNew_Click(object sender, RoutedEventArgs e)
        {
            
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
