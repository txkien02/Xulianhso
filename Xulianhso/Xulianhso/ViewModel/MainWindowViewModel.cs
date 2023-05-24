using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;




namespace Xulianhso.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private string _Picture_Src { get; set; }
        public string Picture_Src { get => _Picture_Src; set { _Picture_Src = value; OnPropertyChanged();}   }
        private string _Picture_LicensePlate { get; set; }
        public string Picture_LicensePlate { get => _Picture_LicensePlate; set { _Picture_LicensePlate = value; OnPropertyChanged(); } }

        private string _LicensePlate;
        public string LicensePlate { get => _LicensePlate; set { _LicensePlate = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _List;
        
        public ObservableCollection<string> List { get => _List; set { _List = value; OnPropertyChanged(); } }


        public ICommand Choose { get; set; }
        public ICommand ListBox_SelectionChanged { get; set; }
        private string _SelectedItem;
        public string SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();

            }
        }

        public MainWindowViewModel() {
            string imagePath = "./Asset/Image/img/imgroot.png"; 
            Picture_Src = imagePath;
            string imagePath2 = "./Asset/Image/img/imgplate.png"; // Replace with the correct path to your image
            Picture_LicensePlate = imagePath2;
            LicensePlate = "ENG 706-1";
            
            Choose = new RelayCommand<ListBox>((p) => { return true; },(p) =>
            {
                ChooseFolder();
                p.BorderThickness = new Thickness(1);
            });
            ListBox_SelectionChanged = new RelayCommand<object>((p) => { return true;}, (p) =>
            {
                string imagePathsrc = SelectedItem; // Replace with the correct path to your image
                Picture_Src = imagePathsrc;

            });
        }
        private void ChooseFolder()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Chọn thư mục";
            dialog.ValidateNames = false;
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = true;
            dialog.FileName = "Chọn thư mục";

            if (dialog.ShowDialog() == true)
            {
                string selectedPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                var imageFiles = Directory.GetFiles(selectedPath, "*.jpg");

                List = new ObservableCollection<string>(imageFiles);
            }
        }

        
    }
}
