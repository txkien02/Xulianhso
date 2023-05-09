namespace QLBAIGUIXE.Model
{

    public class DataProvider
    {
        public int Acc { get; set; }

        private static DataProvider _ins;
        public static DataProvider Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new DataProvider();
                return _ins;
            }
            set
            {
                _ins = value;
            }
        }
        public QLBAIXEEntities DB { get; set; }

        private DataProvider()
        {
            DB = new QLBAIXEEntities();
        }

        public void setAcc(int acc)
        {
            Acc = acc;
        }


        public string getLicensePlate { get; set; }

        public string getCode { get; set; }

        public void setdata(string PL, string code)
        {
            this.getLicensePlate = PL;
            this.getCode = code;
        }
    }
}