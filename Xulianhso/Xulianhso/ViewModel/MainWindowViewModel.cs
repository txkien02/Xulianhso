using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace Xulianhso.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private BitmapSource _Picture_Src { get; set; }
        public BitmapSource Picture_Src { get => _Picture_Src; set { _Picture_Src = value; OnPropertyChanged();}   }
        private BitmapSource _Picture_LicensePlate { get; set; }
        public BitmapSource Picture_LicensePlate { get => _Picture_LicensePlate; set { _Picture_LicensePlate = value; OnPropertyChanged(); } }

        private string _LicensePlate;
        public string LicensePlate { get => _LicensePlate; set { _LicensePlate = value; OnPropertyChanged(); } }
        public MainWindowViewModel() {
            string imagePath = "./Asset/Image/img/imgroot.png"; // Replace with the correct path to your image
            Uri imageUri = new Uri(imagePath, UriKind.Relative);
            Picture_Src = new BitmapImage(imageUri);
            string imagePath2 = "./Asset/Image/img/imgplate.png"; // Replace with the correct path to your image
            Uri imageUri2 = new Uri(imagePath2, UriKind.Relative);
            Picture_LicensePlate = new BitmapImage(imageUri2);
            LicensePlate = "ENG 706-1";
        }
    }
}
