using QLBAIGUIXE.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace QLBAIGUIXE.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        private string _LicensePlate { get; set; }
        public string LicensePlate { get => _LicensePlate; set { _LicensePlate = value; OnPropertyChanged(); } }
        private string _Code { get; set; }
        public string Code { get => _Code; set { _Code = value; OnPropertyChanged(); } }
        private string _DisplayName { get; set; }
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }
        private string _UserName { get; set; }
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }
        private DateTime? _CheckOutTime;
        public DateTime? CheckOutTime { get => _CheckOutTime; set { _CheckOutTime = value; OnPropertyChanged(); } }
        private DateTime? _CheckInTime;
        public DateTime? CheckInTime { get => _CheckInTime; set { _CheckInTime = value; OnPropertyChanged(); } }
        private Decimal? _Price;
        public Decimal? Price
        {
            get => _Price; set { _Price = value; OnPropertyChanged(); }
        }
        private string _Search { get; set; }
        public string Search { get => _Search; set { _Search = value; OnPropertyChanged(); } }
        private ObservableCollection<Model.VIEWHYSTORY> _ViewHystory;
        public ObservableCollection<Model.VIEWHYSTORY> ViewHystory { get => _ViewHystory; set { _ViewHystory = value; OnPropertyChanged(); } }
        private Model.VIEWHYSTORY _SelectedViewHystory;
        public Model.VIEWHYSTORY SelectedViewHystory
        {
            get => _SelectedViewHystory;
            set
            {
                _SelectedViewHystory = value;
                OnPropertyChanged();
                if (SelectedViewHystory != null)
                {
                    LicensePlate = SelectedViewHystory.LicensePlate;
                    Code = SelectedViewHystory.Code;
                    DisplayName = SelectedViewHystory.DisplayName;
                    UserName = SelectedViewHystory.UserName;
                    CheckOutTime = SelectedViewHystory.CheckOutTime;
                    Price = SelectedViewHystory.Price;

                }


            }
        }

        public ICommand SearchCommand { get; set; }

        public HomeViewModel()
        {
            ViewHystory = new ObservableCollection<Model.VIEWHYSTORY>(DataProvider.Ins.DB.VIEWHYSTORies);

            SearchCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {


                ViewHystory = new ObservableCollection<Model.VIEWHYSTORY>(DataProvider.Ins.DB.VIEWHYSTORies.Where(x => x.LicensePlate.Contains(Search)));


            });
        }
    }
}
