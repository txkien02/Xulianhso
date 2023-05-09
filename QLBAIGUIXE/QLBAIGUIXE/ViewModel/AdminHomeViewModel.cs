using QLBAIGUIXE.Model;
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
    public class AdminHomeViewModel: BaseViewModel
    {

        private UserControl _userControl { get; set; }
        public UserControl userControl { get => _userControl; set { _userControl = value; OnPropertyChanged(); } }

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
        private EmployeeUC _employeeUC { get; set; }
        public EmployeeUC employeeUC
        {
            get { return _employeeUC; }
            set
            {
                if (value == _employeeUC)
                    return;
                _employeeUC = value;
                OnPropertyChanged();
            }
        }
        private ParkingUC _parkingUC { get; set; }
        public ParkingUC parkingUC
        {
            get { return _parkingUC; }
            set
            {
                if (value == _parkingUC)
                    return;
                _parkingUC = value;
                OnPropertyChanged();
            }
        }
        private TrackHistoryUC _trackHistoryUC { get; set; }
        public TrackHistoryUC trackHistoryUC
        {
            get { return _trackHistoryUC; }
            set
            {
                if (value == _trackHistoryUC)
                    return;
                _trackHistoryUC = value;
                OnPropertyChanged();
            }
        }
        private userControlStatistical _userControlStatistical { get; set; }
        public userControlStatistical userControlStatistical
        {
            get { return _userControlStatistical; }
            set
            {
                if (value == _userControlStatistical)
                    return;
                _userControlStatistical = value;
                OnPropertyChanged();
            }
        }



        public ICommand ButtonOpenMenu_Click { get; set; } 
        public ICommand ButtonCloseMenu_Click { get; set; }
        public ICommand ListViewMenu_SelectionChanged { get; set; }


        public AdminHomeViewModel()

        {
            
            ButtonOpenMenu_Click = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                var value = (object[])p;
                Button ButtonOpenMenu = (Button)value[0];
                Button ButtonCloseMenu = (Button)value[1];

                ButtonCloseMenu.Visibility = Visibility.Visible;
                ButtonOpenMenu.Visibility = Visibility.Collapsed;
            });
            ButtonCloseMenu_Click = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                var value = (object[])p;
                Button ButtonOpenMenu = (Button)value[0];
                Button ButtonCloseMenu = (Button)value[1];

                ButtonCloseMenu.Visibility = Visibility.Collapsed;
                ButtonOpenMenu.Visibility = Visibility.Visible;
            });
            ListViewMenu_SelectionChanged = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                var value = (object[])p;
                ListView ListViewMenu = (ListView)value[0];
                Grid GridMain = (Grid)value[1];
                userControl = null;
                GridMain.Children.Clear();
                string sw ="";

                if (ListViewMenu.SelectedIndex != -1) 
                    sw = ((ListViewItem)((ListView)ListViewMenu).SelectedItem).Name;
                
                switch (sw)
                {
                    
                    case "ItemHome":
                        userControl = new UserControlHome();
                        GridMain.Children.Add(userControl);
                        break;
                    case "ItemCreate":
                        userControl = new EmployeeUC();
                        GridMain.Children.Add(userControl);
                        break;
                    
                    case "ItemParking":
                        userControl = new ParkingUC();
                        GridMain.Children.Add(userControl);
                        break;
                    case "ItemHistory":
                        userControl = new TrackHistoryUC();
                        GridMain.Children.Add(userControl);
                        break;
                    case "ItemCashMultiple":
                        userControl = new userControlStatistical();
                        GridMain.Children.Add(userControl);
                        break;

                    default:
                        userControl = new UserControlHome();
                        GridMain.Children.Add(userControl);
                        
                        break;
                }
            });
        }
    }
}
