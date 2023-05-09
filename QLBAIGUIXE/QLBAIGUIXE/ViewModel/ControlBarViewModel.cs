using QLBAIGUIXE.Model;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace QLBAIGUIXE.ViewModel
{
    public class ControlBarViewModel : BaseViewModel
    {
        #region commands
        public ICommand CloseWindowCommand { get; set; }
        public ICommand MaximizeWindowCommand { get; set; }
        public ICommand MinimizeWindowCommand { get; set; }
        public ICommand MouseMoveWindowCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        #endregion


        private string _DisplayName { get; set; }
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }

        private Visibility _IsAccount { get; set; }
        public Visibility IsAccount { get => _IsAccount; set { _IsAccount = value; OnPropertyChanged(); } }




        public ControlBarViewModel()
        {
            IsAccount = Visibility.Hidden;

            LoadedCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {

                if (p == null)
                    return;
                if (DataProvider.Ins.Acc != 0)
                {
                    IsAccount = Visibility.Visible;
                    var acc = DataProvider.Ins.DB.EMPLOYEEs.Where(x => x.Id == DataProvider.Ins.Acc);
                    foreach (var item in acc)
                    {
                        DisplayName = item.DisplayName;
                    }
                }
                else
                {
                    IsAccount = Visibility.Hidden;
                }
            });

            CloseWindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement window = GetWindowParent(p);
                var w = window as Window;
                if (w != null)
                {
                    var result = MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Thông báo",
                    MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        w.Close();
                    }
                }
            }
            );
            MaximizeWindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement window = GetWindowParent(p);
                var w = window as Window;
                if (w != null)
                {
                    if (w.WindowState != WindowState.Maximized)
                        w.WindowState = WindowState.Maximized;
                    else
                        w.WindowState = WindowState.Normal;
                }
            }
            );
            MinimizeWindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement window = GetWindowParent(p);
                var w = window as Window;
                if (w != null)
                {
                    if (w.WindowState != WindowState.Minimized)
                        w.WindowState = WindowState.Minimized;
                    else
                        w.WindowState = WindowState.Maximized;
                }
            }
            );
            MouseMoveWindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement window = GetWindowParent(p);
                var w = window as Window;
                if (w != null)
                {
                    w.DragMove();
                }
            }
           );
            LogoutCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement window = GetWindowParent(p);
                var w = window as MainWindow;
                if (w != null)
                {
                    DataProvider.Ins.Acc = -1;
                    DataProvider.Ins.setdata(null, null);
                    var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất không?", "Thông báo",
                    MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        w.Close();
                    }
                    Application.Current.MainWindow = null;
                }
            }
            );
        }

        FrameworkElement GetWindowParent(UserControl p)
        {
            FrameworkElement parent = p;

            while (parent.Parent != null)
            {
                parent = parent.Parent as FrameworkElement;
            }

            return parent;
        }


    }
}