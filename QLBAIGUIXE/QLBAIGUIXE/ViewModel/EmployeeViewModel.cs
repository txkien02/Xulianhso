using QLBAIGUIXE.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace QLBAIGUIXE.ViewModel
{
    public class EmployeeViewModel : BaseViewModel
    {
        private ObservableCollection<EMPLOYEE> _List;
        public ObservableCollection<EMPLOYEE> List
        {
            get => _List; set { _List = value; OnPropertyChanged(); }
        }
        //Thuoc tinh bang nhan vien


        private string _UserName;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }

        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }

        private string _IdRole;
        public string IdRole { get => _IdRole; set { _IdRole = value; OnPropertyChanged(); } }

        private EMPLOYEE _SelectedItem;
        public EMPLOYEE SelectedItem
        {
            get => _SelectedItem; set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    UserName = SelectedItem.UserName;
                    DisplayName = SelectedItem.DisplayName;
                    IdRole = SelectedItem.IdRole;
                }
            }
        }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public EmployeeViewModel()
        {
            List = new ObservableCollection<EMPLOYEE>
                (DataProvider.Ins.DB.EMPLOYEEs.Where(x => x.Status == true));
            //(DataProvider.Ins.DB.EMPLOYEEs);
            //(DataProvider.Ins.DB.EMPLOYEEs.Where(x=>x.IdRole.Equals("1") || x.IdRole.Equals("0")));
            //thêm
            AddCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(DisplayName))
                    return false;

                var displayList = DataProvider.Ins.DB.EMPLOYEEs.Where(x => x.DisplayName == DisplayName);
                if (displayList == null || IdRole == null || displayList.Count() != 0)
                    return false;
                return true;

            }, (p) =>
            {
                string passEncode = MD5Hash(Base64Encode(UserName));
                var EMPLOYEE = new EMPLOYEE()
                {
                    UserName = UserName,
                    Password = passEncode,
                    DisplayName = DisplayName,
                    IdRole = IdRole,
                    Status = true
                };

                DataProvider.Ins.DB.EMPLOYEEs.Add(EMPLOYEE);
                DataProvider.Ins.DB.SaveChanges();

                List.Add(EMPLOYEE);
                MessageBox.Show("Thêm nhân viên thành công!", "Thông báo");
            });
            //sửa
            EditCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;

                var displayList = DataProvider.Ins.DB.EMPLOYEEs.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;

                return false;

            }, (p) =>
            {
                var EMPLOYEE = DataProvider.Ins.DB.EMPLOYEEs.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();

                EMPLOYEE.UserName = UserName;
                EMPLOYEE.DisplayName = DisplayName;
                EMPLOYEE.IdRole = IdRole;

                DataProvider.Ins.DB.SaveChanges();

                SelectedItem.DisplayName = DisplayName;
                SelectedItem.UserName = UserName;
                MessageBox.Show("Sửa nhân viên thành công!", "Thông báo");

            });
            //xóa
            DeleteCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;

                var displayList = DataProvider.Ins.DB.EMPLOYEEs.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;

                return false;

            }, (p) =>
            {
                var EMPLOYEE = DataProvider.Ins.DB.EMPLOYEEs.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                EMPLOYEE.UserName = UserName;

                EMPLOYEE.DisplayName = DisplayName;
                EMPLOYEE.IdRole = IdRole;
                EMPLOYEE.Status = false;

                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này?", "Thông báo",
                    MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    DataProvider.Ins.DB.SaveChanges();
                    if (!EMPLOYEE.Status == true)
                    {
                        List.Remove(EMPLOYEE);
                    }
                    else
                    {
                        SelectedItem.DisplayName = DisplayName;
                        SelectedItem.UserName = UserName;
                    }

                    MessageBox.Show("Xóa nhân viên thành công!", "Thông báo");
                }

            });
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
