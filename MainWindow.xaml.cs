using CukiKaveManagerV2.UserControls;
using CukiKaveManagerV2.ws;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CukiKaveManagerV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string locale = "hu-HU";
        const string WS_ADDRESS = @"ws://localhost";
        const string HASH_FILE = "auth.hash";
        public Dictionary<int, flippable> Products = new Dictionary<int, flippable>();

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
                var flp = new flippable(_product);
                ItemHolder.Children.Add(flp);
                Products.Add(_product.id, flp);
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
        }

        private async void onWSConnected(object? sender, EventArgs e)
        {
            await Dispatcher.InvokeAsync<Task<bool>>(ReloadUI);
        }
    }
}
