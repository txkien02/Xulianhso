using QLBAIGUIXE.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QLBAIGUIXE.ViewModel
{
    public class ParkingViewModel : BaseViewModel
    {
        private ObservableCollection<INFOPARKING> _List;
        public ObservableCollection<INFOPARKING> List
        {
            get => _List; set { _List = value; OnPropertyChanged(); }
        }
        private int _Type;
        public int Type { get => _Type; set { _Type = value; OnPropertyChanged(); } }

        private string _Name;
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }

        private int _Count;
        public int Count { get => _Count; set { _Count = value; OnPropertyChanged(); } }

        private bool _Status;
        public bool Status { get => _Status; set { _Status = value; OnPropertyChanged(); } }
        private INFOPARKING _SelectedItem;
        public INFOPARKING SelectedItem
        {
            get => _SelectedItem; set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    Type = SelectedItem.Type;
                    Name = SelectedItem.Name;
                    Count = SelectedItem.Count;

                }
            }
        }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ParkingViewModel()
        {
            List = new ObservableCollection<INFOPARKING>
                (DataProvider.Ins.DB.INFOPARKINGs.Where(x => x.Status == true));
            //(DataProvider.Ins.DB.EMPLOYEEs);
            //(DataProvider.Ins.DB.EMPLOYEEs.Where(x=>x.IdRole.Equals("1") || x.IdRole.Equals("0")));
            //thêm
            AddCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(Name) || Count == 0)
                    return false;

                var displayList = DataProvider.Ins.DB.INFOPARKINGs.Where(x => x.Name == Name);
                if (displayList == null || displayList.Count() != 0)
                    return false;
                return true;
            }, (p) =>
            {
                var typeList = DataProvider.Ins.DB.INFOPARKINGs.Where(x => x.Type == Type);
                if (typeList.Count() != 0)
                {
                    MessageBox.Show("Trùng mã bãi, vui lòng nhập mã khác!", "Thông báo");
                }
                else
                {
                    var INFOPARKING = new INFOPARKING()
                    {
                        Type = Type,
                        Name = Name,
                        Count = Count,
                        Status = true
                    };

                    DataProvider.Ins.DB.INFOPARKINGs.Add(INFOPARKING);
                    DataProvider.Ins.DB.SaveChanges();

                    List.Add(INFOPARKING);
                    MessageBox.Show("Thêm bãi gửi thành công!", "Thông báo");
                }
            });
            //sửa
            EditCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;

                var displayList = DataProvider.Ins.DB.INFOPARKINGs.Where(x => x.Type == SelectedItem.Type);
                if (displayList != null && displayList.Count() != 0)
                    return true;

                return false;

            }, (p) =>
            {
                var INFOPARKING = DataProvider.Ins.DB.INFOPARKINGs.Where(x => x.Type == SelectedItem.Type).SingleOrDefault();

                INFOPARKING.Name = Name;
                INFOPARKING.Count = Count;

                DataProvider.Ins.DB.SaveChanges();

                SelectedItem.Name = Name;
                SelectedItem.Count = Count;
                MessageBox.Show("Sửa bãi gửi xe thành công!", "Thông báo");

            });
            //xóa
            DeleteCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;

                var displayList = DataProvider.Ins.DB.INFOPARKINGs.Where(x => x.Type == SelectedItem.Type);
                if (displayList != null && displayList.Count() != 0)
                    return true;

                return false;

            }, (p) =>
            {
                var INFOPARKING = DataProvider.Ins.DB.INFOPARKINGs.Where(x => x.Type == SelectedItem.Type).SingleOrDefault();

                INFOPARKING.Status = false;

                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa bãi gửi xe này?", "Thông báo",
                    MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    DataProvider.Ins.DB.SaveChanges();
                    if (!INFOPARKING.Status == true)
                    {
                        List.Remove(INFOPARKING);
                    }
                    MessageBox.Show("Xóa bãi gửi này thành công!", "Thông báo");
                    Type = 0;
                    Name = "";
                    Count = 0;
                }

            });
        }
    }
}
