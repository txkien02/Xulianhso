using QLBAIGUIXE.ViewModel;
using System.Windows.Controls;

namespace QLBAIGUIXE.UserControlTeam1
{
    /// <summary>
    /// Interaction logic for TrackHistoryUC.xaml
    /// </summary>
    public partial class TrackHistoryUC : UserControl
    {
        public TrackHistoryVM Viewmodel { get; set; }
        public TrackHistoryUC()
        {
            InitializeComponent();
            this.DataContext = Viewmodel = new TrackHistoryVM();

        }
    }
}
