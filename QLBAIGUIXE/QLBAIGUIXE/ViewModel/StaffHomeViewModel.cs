using QLBAIGUIXE.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QLBAIGUIXE.ViewModel
{
    public class StaffHomeViewModel : BaseViewModel
    {
        private string _LicensePlate { get; set; }
        public string LicensePlate { get => _LicensePlate; set { _LicensePlate = value; OnPropertyChanged(); } }
        private string _Code { get; set; }
        public string Code { get => _Code; set { _Code = value; OnPropertyChanged(); } }
        private string _DisplayName { get; set; }
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }
        private string _Phone { get; set; }
        public string Phone { get => _Phone; set { _Phone = value; OnPropertyChanged(); } }

        private string _Search { get; set; }
        public string Search { get => _Search; set { _Search = value; OnPropertyChanged(); } }
        private string _MotoBike { get; set; }
        public string MotoBike { get => _MotoBike; set { _MotoBike = value; OnPropertyChanged(); } }

        private string _Car { get; set; }
        public string Car { get => _Car; set { _Car = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.VIEWPARKING> _ViewParking;
        public ObservableCollection<Model.VIEWPARKING> ViewParking { get => _ViewParking; set { _ViewParking = value; OnPropertyChanged(); } }
        private Model.VIEWPARKING _SelectedViewParking;
        public Model.VIEWPARKING SelectedViewParking
        {
            get => _SelectedViewParking;
            set
            {
                _SelectedViewParking = value;
                OnPropertyChanged();

            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand ClickCommand { get; set; }
        public ICommand SearchCommand { get; set; }


        private ObservableCollection<Model.INFOPARKING> _INFOPARKING;
        public ObservableCollection<Model.INFOPARKING> INFOPARKING { get => _INFOPARKING; set { _INFOPARKING = value; OnPropertyChanged(); } }

        private Model.INFOPARKING _SelectedInfoParking;
        public Model.INFOPARKING SelectedInfoParking
        {
            get => _SelectedInfoParking;
            set
            {
                _SelectedInfoParking = value;
                OnPropertyChanged();

            }
        }

        public StaffHomeViewModel()
        {
            INFOPARKING = new ObservableCollection<Model.INFOPARKING>(DataProvider.Ins.DB.INFOPARKINGs.Where(x => x.Status == true));
            ViewParking = new ObservableCollection<Model.VIEWPARKING>(DataProvider.Ins.DB.VIEWPARKINGs);
            Car = Count(1) + "/" + Capacity(1);
            MotoBike =Count(2) + "/" + Capacity(2);
            AddCommand = new RelayCommand<object>((p) =>
            {
                //if (string.IsNullOrEmpty(DisplayName) || string.IsNullOrEmpty(Phone) || string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(LicensePlate))
                if (string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(LicensePlate))
                    return false;

                if (SelectedInfoParking == null)
                    return false;
                var plicense = DataProvider.Ins.DB.INFOCARs.Where(x => x.LicensePlate == LicensePlate && x.CheckOutTime == null).Count();
                if (plicense > 0)
                    return false;

                return true;

            }, (p) =>
            {
                var custom = new Model.CUSTOMER() { DisplayName = DisplayName, Code = Code, Phone = Phone };
                DataProvider.Ins.DB.CUSTOMERs.Add(custom);
                var infocar = new Model.INFOCAR() { LicensePlate = LicensePlate, Type = SelectedInfoParking.Type, IdEMPLOYEE = DataProvider.Ins.Acc, IdCUSTOMER = custom.Id, CheckInTime = DateTime.Now };
                DataProvider.Ins.DB.INFOCARs.Add(infocar);
                var parking = new Model.PARKING() { IdINFOCAR = infocar.Id, Type = SelectedInfoParking.Type };
                DataProvider.Ins.DB.PARKINGs.Add(parking);
                DataProvider.Ins.DB.SaveChanges();
                var vp = new Model.VIEWPARKING() { Code = Code, LicensePlate = LicensePlate };
                updatecount();

                Code = "";
                LicensePlate = "";
                Phone = "";
                DisplayName = "";
                ViewParking.Add(vp);
                MessageBox.Show("Thêm thông tin thành công!", "Thông báo");
            });

            ClickCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            { OnOpenCheckOut(p); });

            SearchCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {

                ViewParking = new ObservableCollection<Model.VIEWPARKING>(DataProvider.Ins.DB.VIEWPARKINGs.Where(x => x.Code.Contains(Search)));


            });

            void OnOpenCheckOut(object commandParameter)
            {
                VIEWPARKING vp = commandParameter as VIEWPARKING;
                if (vp != null)
                {
                    DataProvider.Ins.setdata(vp.LicensePlate, vp.Code);
                    BillWindow billWindow = new BillWindow();
                    billWindow.ShowDialog();
                    var BillVM = billWindow.DataContext as BillViewModel;
                    if (BillVM.Ispayment)
                    {
                        ViewParking.Remove(vp);
                        updatecount();
                    }
                }
            }

            int Count(int Type)
            {
                return DataProvider.Ins.DB.VIEWPARKINGs.Where(x => x.Type == Type).Count();
            }

            int Capacity(int Type)
            {
                int c = 0;
                var capacity = DataProvider.Ins.DB.INFOPARKINGs.Where(x => x.Type == Type);
                foreach (var count in capacity)
                {
                    return c = count.Count;
                }
                return c;
            }

            void updatecount()
            {

                Car = Count(1) + "/" + Capacity(1);

                MotoBike = Count(2) + "/" + Capacity(2);
            }
        }

    }
}
