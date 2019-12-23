using CukiKaveManagerV2.ws;
using CukiKaveManagerV2.ws.JSONObjects;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media.Imaging;


namespace CukiKaveManagerV2.UserControls
{
    /// <summary>
    /// Interaction logic for flippable.xaml
    /// </summary>
    public partial class flippable : UserControl
    {
        // These don't need to be accessible
        private string locale = "hu-HU";
        private Product orgProduct;
        private MainWindow mainWindow;
        // List for the combobox so it works properly
        private string[] productTypes = { "Torta", "Magyarország tortái", "Forróital", "Ital", "Sütemény" };
        // This needs to be accessible by the main window
        public int id;

        public flippable(Product _prd, MainWindow _mainWindow)
        {
            InitializeComponent();

            // Date picker
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(locale);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(locale);

            // Set id
            id = _prd.id;

            // Set the content and the mainWindow
            setContents(_prd);
            mainWindow = _mainWindow;
        }

        public void updateContents(Product _prd) => setContents(_prd);

        private string parseToComboType(string _inType)
        {
            switch (_inType)
            {
                case "cake":
                    return "Torta";
                case "huncake":
                    return "Magyarország tortái";
                case "hotdrinks":
                    return "Forróital";
                case "drinks":
                    return "Ital";
                case "bakedgoods":
                    return "Sütemény";
                default:
                    return "parseerror";
            }
        }

        public void setContents(Product _prd)
        {
            // Titles
            frontTitle.Text = _prd.name;
            title.Text = _prd.name;
            // Price
            price.Text = _prd.price.ToString();
            // Description
            desc.Document.Blocks.Clear();
            desc.Document.Blocks.Add(new Paragraph(new Run(_prd.description)));
            // Image textbox
            imagePath.Text = _prd.image;
            // Date picker
            datePick.SelectedDate = _prd.added;

            // ComboBox setup
            _prd.type = parseToComboType(_prd.type);
            productType.Items.Clear();

            foreach (string item in productTypes)
                productType.Items.Add(item);

            productType.SelectedItem = _prd.type;
            
            // Make bitmapimage for the front page
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(@"http://localhost/uploads/" + _prd.image, UriKind.Absolute);
            bmp.EndInit();
            frontImg.Source = bmp;

            // Set orgProduct equal to _prd
            orgProduct = _prd;
        }

        public void rollbackChanges(int currentPrompt)
        {
            // Make sure that the currentPrompt is the current flippable
            if(currentPrompt == id)
            {
                // Do some funky business
                orgProduct.type = mainWindow.parseType(orgProduct.type);
                // Set the content
                setContents(orgProduct);
            }
        }

        public void deleteButton(object sender, System.Windows.RoutedEventArgs e)
        {
            // Open delete confirm dialog
            mainWindow.deleteConfirm.IsOpen = true;
            mainWindow.currentPrompt = id;
        }

        public async void deleteConfirm(int idIn)
        {
            // Make sure that the item that is to be deleted is this
            if (idIn == id)
            {
                // Hide the dialog and send a message to the ws, and also hide the current flippable since it was deleted
                mainWindow.currentPrompt = -1;
                mainWindow.deleteConfirm.IsOpen = false;
                if (await cukiAPI.DeleteProduct(id))
                {
                    this.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void modifyClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            // Create a product to compare them
            Product _prd = new Product(int.Parse(price.Text), title.Text, id, new TextRange(desc.Document.ContentStart, desc.Document.ContentEnd).Text, imagePath.Text, productType.SelectedItem.ToString(), datePick.DisplayDate);
            _prd.description = _prd.description.Remove(_prd.description.Length - 2);
            
            // Don't use the product variable because it doesn't work
            if (_prd.id == orgProduct.id && _prd.name == orgProduct.name && _prd.description == orgProduct.description && _prd.image == orgProduct.image && _prd.price == orgProduct.price && _prd.type == orgProduct.type && _prd.added == orgProduct.added)
            {
                thisFlipper.IsFlipped = false;
            }
            else
            {
                // Changed, display prompt to check if user really wants to change the data
                // Debug info
                /*MessageBox.Show("Mod | Org\n" +
                    $"{_prd.id.ToString()} | {orgProduct.id.ToString()} | {_prd.id == orgProduct.id}\n" +
                    $"{_prd.name} | {orgProduct.name}  | {_prd.name == orgProduct.name}\n" +
                    $"{_prd.description} | {orgProduct.description} | {_prd.description == orgProduct.description}\n" +
                    $"{_prd.image} | {orgProduct.image} | {_prd.image == orgProduct.image}\n" +
                    $"{_prd.price.ToString()} | {orgProduct.price.ToString()} | {_prd.price == orgProduct.price}\n" +
                    $"{_prd.type} | {orgProduct.type} | {_prd.type == orgProduct.type}\n" +
                    $"{_prd.added.ToString()} | {orgProduct.added.ToString()} | {_prd.added == orgProduct.added}");
                    */

                mainWindow.updateConfirm.IsOpen = true;
                mainWindow.currentPrompt = id;
            }
        }
        
        public async void updateConfirm(int inId)
        {
            // Make sure that the right flippable is being updated
            if (inId == id)
            {
                // Create a product from the current context
                Product _prd = new Product(int.Parse(price.Text), title.Text, id, new TextRange(desc.Document.ContentStart, desc.Document.ContentEnd).Text, imagePath.Text, productType.SelectedItem.ToString(), datePick.DisplayDate);
                _prd.description = _prd.description.Remove(_prd.description.Length - 2);
                _prd.type = mainWindow.parseType(_prd.type);

                // Hide the Dialog and send a message to the ws server
                mainWindow.currentPrompt = -1;
                mainWindow.updateConfirm.IsOpen = false;
                if(await cukiAPI.UpdateProduct(_prd))
                {
                    
                }
            }
        }
    }
}
