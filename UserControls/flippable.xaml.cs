using CukiKaveManagerV2.ws;
using CukiKaveManagerV2.ws.JSONObjects;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;


namespace CukiKaveManagerV2.UserControls
{
    /// <summary>
    /// Interaction logic for flippable.xaml
    /// </summary>
    public partial class flippable : UserControl
    {
        private string locale = "hu-HU";
        private int id;
        private Product orgProduct;
        public flippable(Product _prd)
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(locale);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(locale);

            id = _prd.id;

            setContents(_prd);
        }

        public void updateContents(Product _prd) => setContents(_prd);

        public void setContents(Product _prd)
        {
            frontTitle.Text = _prd.name;
            title.Text = _prd.name;
            price.Text = _prd.price.ToString();
            desc.Document.Blocks.Add(new Paragraph(new Run(_prd.description)));
            imagePath.Text = _prd.image;
            datePick.SelectedDate = _prd.added;
            timePick.SelectedTime = _prd.added;

            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(@"http://localhost/uploads/" + _prd.image, UriKind.Absolute);
            bmp.EndInit();
            frontImg.Source = bmp;
            orgProduct = _prd;
        }

        private async void deleteButton(object sender, System.Windows.RoutedEventArgs e)
        {
            if (await cukiAPI.DeleteProduct(id))
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void modifyClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            Product _prd = new Product(int.Parse(price.Text), title.Text, id, new TextRange(desc.Document.ContentStart, desc.Document.ContentEnd).Text, imagePath.Text, "test",new DateTime(datePick.DisplayDate.Year, datePick.DisplayDate.Month, datePick.DisplayDate.Day, ((DateTime)timePick.SelectedTime).Hour, ((DateTime)timePick.SelectedTime).Minute, ((DateTime)timePick.SelectedTime).Second));
            if (_prd != orgProduct)
            {
                // Changed, display prompt to check if user really wants to change the data
            }
            else
            {
                thisFlipper.IsFlipped = false;
            }
        }
    }
}
