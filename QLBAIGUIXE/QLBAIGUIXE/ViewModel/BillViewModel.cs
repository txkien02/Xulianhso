using QLBAIGUIXE.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QLBAIGUIXE.ViewModel
{
    public class BillViewModel:BaseViewModel
    {
        private string _LicensePlate { get; set; }
        public string LicensePlate { get => _LicensePlate; set { _LicensePlate = value;OnPropertyChanged(); } }
        private string _Code { get; set; }
        public string Code { get => _Code; set { _Code = value; OnPropertyChanged(); } }
        private int _Type { get; set; }
        public int Type { get => _Type; set { _Type = value; OnPropertyChanged(); } }
        
        private string _Time { get; set; }
        public string Time { get => _Time; set { _Time = value; OnPropertyChanged(); } }
        private SqlMoney _Price { get; set; }
        public SqlMoney Price { get => _Price; set { _Price = value;OnPropertyChanged(); } }
        public bool Ispayment { get; set; }

        public DateTime CheckInTime { get; set; }

        public ICommand CheckOutCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public BillViewModel()
        {
            Ispayment = false;
            this.LicensePlate = DataProvider.Ins.getLicensePlate;
            this.Code = DataProvider.Ins.getCode;

            var time = DataProvider.Ins.DB.INFOCARs;
            foreach(var item in time)
            {
                if (item.LicensePlate.Equals(LicensePlate))
                {
                    CheckInTime = (DateTime)item.CheckInTime;
                    Time = CheckInTime.ToString();
                    Type = item.Type;
                    break;
                }
            }
            this.Price = total();
            CheckOutCommand = new RelayCommand<Window>((p) =>
            {
                return true;
            }, (p) =>
            {
                CheckOut(p);
                
            });
            CancelCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { p.Close(); });


        }
        void CheckOut(Window p)
        {
            var ifcar = DataProvider.Ins.DB.INFOCARs.Where(x => x.LicensePlate == LicensePlate && x.CheckOutTime==null).FirstOrDefault();
            ifcar.CheckOutTime = DateTime.Now;
            var bill = new Model.Bill() { IdEMPLOYEE = 3, IdINFOCAR = ifcar.Id, Price = (decimal?)Price };
            var c = DataProvider.Ins.DB.PARKINGs.Where(x => x.IdINFOCAR == ifcar.Id).FirstOrDefault();
            DataProvider.Ins.DB.Bills.Add(bill);
            



            DataProvider.Ins.DB.PARKINGs.Remove(c);
            DataProvider.Ins.DB.SaveChanges();
            MessageBox.Show("Thanh toán thành công", "Thông báo");
            Ispayment = true;   
            p.Close();
            
        }
        public SqlMoney total()
        {
            TimeSpan time = DateTime.Now - CheckInTime;
            double diff = time.TotalMinutes;
            int Hour = 0;
            if ((int)diff % 60 == 0)
                Hour = (int)diff/60;
            else Hour = (int)diff/60+1;


            if ((int)Hour < 1)
                Hour = 1;
            
            if (Type == 1)
                return Hour * 4000;
            else return Hour * 2000;

        }
    }
}
