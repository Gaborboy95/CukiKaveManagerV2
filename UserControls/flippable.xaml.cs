using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CukiKaveManagerV2.UserControls
{
    /// <summary>
    /// Interaction logic for flippable.xaml
    /// </summary>
    public partial class flippable : UserControl
    {

        public string Title { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }

        public flippable(string _title, int _price, string _desc)
        {
            InitializeComponent();

            Title = _title;
            Price = _price;
            Description = _desc;
        }
    }
}
