using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using QLBAIGUIXE.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace QLBAIGUIXE.ViewModel
{
    public class StatisticalViewModel : BaseViewModel
    {
        private string[] _Labels { get; set; }
        public string[] Labels { get => _Labels; set { _Labels = value; OnPropertyChanged(); } }
        private Func<double, string> _YFormatter { get; set; }
        public Func<double, string> YFormatter { get => _YFormatter; set { _YFormatter = value; OnPropertyChanged(); } }
        public int CountCar { get; set; }
        public int CountBike { get; set; }
        private string _Turnover { get; set; }
        public string Turnover { get => _Turnover; set { _Turnover = value; OnPropertyChanged(); } }
        private string _Amount { get; set; }
        public string Amount { get => _Amount; set { _Amount = value; OnPropertyChanged(); } }
        private string _Avg { get; set; }
        public string Avg { get => _Avg; set { _Avg = value; OnPropertyChanged(); } }

        private ObservableCollection<Filter> _FilterList;
        public ObservableCollection<Filter> FilterList { get => _FilterList; set { _FilterList = value; OnPropertyChanged(); } }
        private Filter _SelectedFilter;
        public Filter SelectedFilter
        {
            get => _SelectedFilter;
            set
            {
                _SelectedFilter = value;
                OnPropertyChanged();

            }
        }
        private ObservableCollection<VIEWHYSTORY> _List;
        public ObservableCollection<VIEWHYSTORY> List
        {
            get => _List; set { _List = value; OnPropertyChanged(); }
        }

        private SeriesCollection _SeriesCollection { get; set; }
        public SeriesCollection SeriesCollection
        {
            get => _SeriesCollection;
            set
            {
                _SeriesCollection = value;
                OnPropertyChanged();

            }
        }
        private SeriesCollection _SeriesCollection1 { get; set; }
        public SeriesCollection SeriesCollection1
        {
            get => _SeriesCollection1;
            set
            {
                _SeriesCollection1 = value;
                OnPropertyChanged();

            }
        }

        public ICommand FilterChangeCommand { get; set; }

        public StatisticalViewModel()
        {
            List = new ObservableCollection<VIEWHYSTORY>(DataProvider.Ins.DB.VIEWHYSTORies.Where(x => ((DateTime)x.CheckOutTime).Month == DateTime.Now.Month));
            YFormatter = value => value.ToString();
            Labels = getLabels(List);
            FilterList = new ObservableCollection<Filter>();
            FilterList.Add(new Filter("Tháng này", 1));
            FilterList.Add(new Filter("Tháng trước ", 2));
            Load();
            Load1();
            FilterChangeCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {

                if (SelectedFilter.Type == 1)
                {
                    List = new ObservableCollection<VIEWHYSTORY>(DataProvider.Ins.DB.VIEWHYSTORies.Where(x => ((DateTime)x.CheckOutTime).Month == DateTime.Now.Month));
                    Load();
                    Load1();
                }
                else
                {
                    List = new ObservableCollection<VIEWHYSTORY>(DataProvider.Ins.DB.VIEWHYSTORies.Where(x => ((DateTime)x.CheckOutTime).Month == DateTime.Now.Month - 1));
                    Load();
                    Load1();
                }
            });
        }
        //Load () : set vaule cho List của tháng hiện tại
        public void Load()
        {
            getProperties(List);
            SeriesCollection = new SeriesCollection{
                new PieSeries
                {
                    Title = "Ô tô",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(getVaule(1)) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Xe máy",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(getVaule(2)) },
                    DataLabels = true
                }
            };
        }
        //Load1 () : set vaule cho List của tháng trước
        public void Load1()
        {
            getProperties(List);
            var myArray = getVaule1(1, List);
            var myArray1 = getVaule1(2, List);
            SeriesCollection1 = new SeriesCollection{
                new LineSeries
                {
                    Title = "Ô tô",
                    Values = myArray.AsChartValues()

                },
                new LineSeries
                {
                    Title = "Xe máy",
                    Values = myArray1.AsChartValues()
                }
            };
        }

        public void getProperties(ObservableCollection<VIEWHYSTORY> List)
        {
            Amount = List.Count + " Xe";
            int n = 0;
            decimal turnover = 0;
            foreach (var item in List)
            {
                if (((DateTime)item.CheckOutTime).Month == DateTime.Now.Month)
                    n = DateTime.Now.Day;
                else n = getday(List);
                break;
            }
            foreach (var item in List)
            {
                turnover += (decimal)(item.Price);
            }
            Turnover = (int)turnover + " VND";
            Avg = ((int)turnover / n) + " VND";

        }//set giá trị của Amount, Turnover, Avg

        public string[] getLabels(ObservableCollection<VIEWHYSTORY> List)
        {
            int n = getday(List);
            string[] count;
            if (n == 28)
                count = new string[28];
            else if (n == 29)
                count = new string[29];
            else if (n == 30)
                count = new string[30];
            else count = new string[31];
            for (int i = 0; i < n; i++)
            {
                count[i] = "Ngày " + (i + 1).ToString();
            }
            return count;
        }//labels điểu đồ đường

        public int getVaule(int Type)
        {
            int count = 0;

            foreach (var item in List)
            {
                if (item.Type == Type)
                    count++;
            }
            return count;
        }//vaule biểu đồ tròn
        public int[] getVaule1(int Type, ObservableCollection<VIEWHYSTORY> List)
        {
            int n = getday(List);
            int[] count;
            if (n == 28)
                count = new int[28];
            else if (n == 29)
                count = new int[29];
            else if (n == 30)
                count = new int[30];
            else count = new int[31];


            foreach (var item in List)
            {
                if (item.Type == Type)
                    count[((DateTime)item.CheckOutTime).Day - 1] += (int)item.Price;

            }
            return count;

        }//vaule biểu đồ đường
        //getday(): tính sô ngày trong tháng
        public int getday(ObservableCollection<VIEWHYSTORY> List)
        {
            int n = 0;
            foreach (var item in List)
            {
                int y = ((DateTime)item.CheckOutTime).Year;
                int m = ((DateTime)item.CheckOutTime).Month;
                n = DateTime.DaysInMonth(y, m);
            }
            return n;
        }
    }
    public class Filter
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public Filter(string Name, int type)
        {
            this.Name = Name;
            Type = type;
        }
    }

}
