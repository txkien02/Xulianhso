using QLBAIGUIXE.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QLBAIGUIXE.UserControlTeam1
{
    /// <summary>
    /// Interaction logic for ParkingUC.xaml
    /// </summary>
    public partial class ParkingUC : UserControl
    {
        public ParkingViewModel Viewmodel { get; set; }
        public ParkingUC()
        {
            InitializeComponent();
            this.DataContext = Viewmodel = new ParkingViewModel();
        }
    }
}
