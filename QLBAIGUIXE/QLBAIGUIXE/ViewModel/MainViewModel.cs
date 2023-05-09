using QLBAIGUIXE.UserControlTeam1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QLBAIGUIXE.ViewModel
{
    public class MainViewModel:BaseViewModel
    {
        private UserControl _userControl { get; set; }  
        public UserControl userControl { get => _userControl;set { _userControl = value; OnPropertyChanged(); } }


        private UserControlStaff _userControlStaff { get; set; }
        public UserControlStaff userControlStaff
        {
            get { return _userControlStaff; }
            set
            {
                if (value == _userControlStaff)
                    return;
                _userControlStaff = value;
                OnPropertyChanged();
            }
        }

        private UserControlAdmin _userControlAdmin { get; set; }
        public UserControlAdmin userControlAdmin
        {
            get { return _userControlAdmin; }
            set
            {
                if (value == _userControlAdmin)
                    return;
                _userControlAdmin = value;
                OnPropertyChanged();
            }
        }


        public bool Isloaded = false;
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand LoadedGridMainCommand { get; set; }
        


        public MainViewModel()
        {
            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) => {
                    Isloaded = true;
                    if (p == null)
                        return;
                    p.Hide();
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.ShowDialog();

                    if (loginWindow.DataContext == null)
                        return;
                    var loginVM = loginWindow.DataContext as LoginViewModel;

                    if (loginVM.IsLogin)
                    {
                        p.Show();
                        if (loginVM.IdRole.Equals("1"))
                            userControl = new UserControlStaff();
                        else userControl = new UserControlAdmin();
                    }
                    else
                    {
                        p.Close();
                    }
            });
            LoadedGridMainCommand = new RelayCommand<Grid>((p) => { return true; }, (p) => {

                if (userControl == null)
                    return;
                p.Children.Add(userControl);

            });

        }

    }
}
