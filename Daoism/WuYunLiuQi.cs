using System;

namespace BitDAO.Utils.Daoism;

public class WuYunLiuQi
{
    public static string[] Yun = ["木", "火", "土", "金", "水"];
    public static string[] Qi = ["厥阴风木", "少阴君火", "太阴湿土", "少阳相火", "阳明燥金", "太阳寒水"];
    public static int[] YunJie = [0, 4, 9, 14, 19];
    public static int[][] YunKe = [[16, 1172, 889, 605, 323],
                                   [40, 1196, 913, 629, 347],
                                   [64, 1220, 937, 653, 371],
                                   [88, 1244, 865, 581, 399]];

    /*
        #region GetByTime
        public static FiveYunSixQiForTime GetByTime(DateTime _date)
        {
            FiveYunSixQiForYear _year = GetByYear(_date.Year);
            int _days = _date.DayOfYear;
            if (_date.DayOfYear < _year.JieQi[0])
            {
                _year = GetByYear(_date.Year - 1);
                _days = (int)(_date - DateTime.Parse($"{_date.Year - 1}-01-01")).TotalDays;
            }
            int _ke = (_days - _year.JieQi[0]) * 8;

            NongLi.NongLi _nongli = NongLi.NongLi.FromYangLi(_date);
            BaZi _bazi = new(_nongli);

            FiveYunSixQiForTime _result = new();
            _result.Year = _year;
            _result.Solar = $"{_date:yyyy-MM-dd HH:mm:ss}";
            _result.Lunar = $"{_bazi.NianGanZhi}{_bazi.YueGanZhi}{_bazi.RiGanZhi}";

            for (int i = 0; i < _year.YunKe.Length; i++)
            {
                if (i == _year.YunKe.Length - 1 || (_ke >= _year.YunKe[i] && _ke < _year.YunKe[i + 1]))
                {
                    _result.ZhuYun = _year.ZhuYun[i];
                    _result.KeYun = _year.KeYun[i];
                    break;
                }
            }

            int _jieqi = 0;
            for (int i = 0; i < _year.JieQi.Length - 1; i++)
            {
                if (_days > _year.JieQi[i] && _days <= _year.JieQi[i + 1])
                {
                    _jieqi = i;
                }
            }

            Console.WriteLine($"{_jieqi} - {_days} {_year.JieQi[0]}");
            _result.ZhuQi = _year.ZhuQi[_jieqi / 4];
            _result.KeQi = _year.KeQi[_jieqi / 4];
            _result.JieQi = $"{JieQi[_jieqi]}:{_days - _year.JieQi[_jieqi]}";

            return _result;
        }
        public static FiveYunSixQiForTime GetByDate(string _ymd) => GetByTime(DateTime.Parse($"{_ymd} 22:30:00"));
        #endregion

        #region GetByYear
        public static FiveYunSixQiForYear GetByYear(int _year)
        {
            FiveYunSixQiForYear _result = new();
            _result.Year = _year.ToString();
            _result.JieQi = new int[25]; // TODO: 节气赋值

            DateTime _start = DateTime.Parse($"{_year}-01-01");
            YangLiDateTime _yangli = new(_year, 1, 1);
            BaZi _bazi = new(NongLi.NongLi.FromYangLi(_yangli));

            _result.YearGanZhi = _bazi.NianGanZhi;
            string _yearGan = _result.YearGanZhi[..1];
            _result.SuiYun = YearGan2SuiYun(_yearGan);

            _result.YunKe = new int[5];
            _result.ZhuYun = new string[5];
            _result.KeYun = new string[5];

            string _yunType = _result.SuiYun[1..];
            int _yunStart = Array.IndexOf(Yun, _result.SuiYun[..1]);
            int _ganStart = Array.IndexOf(Gan, _yearGan) % 4;

            for (int i = 0; i < 5; i++)
            {
                _result.YunKe[i] = _result.JieQi[YunJie[i]] * 8 + YunKe[_ganStart][i];

                string _type = i % 2 == 0 ? _yunType : (_yunType == "多" ? "少" : "多");
                _result.ZhuYun[i] = $"{Yun[i]}{_type}";

                int _index = _yunStart + i;
                _index = _index < 5 ? _index : (_index - 5);
                _result.KeYun[i] = $"{Yun[_index]}{_type}";
            }

            _result.SiTian = YearZhi2SuiQi(_result.YearGanZhi[1..])[0];
            _result.ZaiQuan = YearZhi2SuiQi(_result.YearGanZhi[1..])[1];
            _result.ZhuQi = new string[6];
            for (int i = 0; i < Qi.Length; i++)
            {
                _result.ZhuQi[i] = Qi[i == 2 ? 3 : (i == 3 ? 2 : i)];
            }

            _result.KeQi = new string[6];
            int _qiStart = Array.IndexOf(Qi, _result.SiTian);
            for (int i = 0; i < Qi.Length; i++)
            {
                int _index = _qiStart + i - 2;
                _result.KeQi[i] = Qi[_index < 0 ? _index + Qi.Length : (_index >= Qi.Length ? _index - Qi.Length : _index)];
            }

            return _result;
        }
        #endregion

        #region Zhi2Qi
        public static string Zhi2Qi(string _zhi)
        {
            int _index = Array.IndexOf(Zhi, _zhi);
            if (_index < -1) { return ""; }

            return Qi[(_index + 1) % 6];
        }
        #endregion

        #region Gan2Yun
        public static string TianGan2Yun(string _gan)
        {
            int _index = Array.IndexOf(Gan, _gan);
            if (_index < -1) { return ""; }

            return Yun[(_index + 2) % 5];
        }
        #endregion

        #region YearGan2SuiYun
        public static string YearGan2SuiYun(string _gan)
        {
            string _yun = TianGan2Yun(_gan);
            int _index = Array.IndexOf(Gan, _gan);
            string _yy = _index % 2 == 0 ? "多" : "少";

            return $"{_yun}{_yy}";
        }
        #endregion

        #region TianGan2ZhuYun
        public static string TianGan2SuiYun(string _gan)
        {
            string _yun = TianGan2Yun(_gan);
            int _index = Array.IndexOf(Gan, _gan);
            string _yy = _index % 2 == 0 ? "太过" : "不及";

            return $"{_yun}{_yy}";
        }
        #endregion

        #region YearZhi2SuiQi
        public static string[] YearZhi2SuiQi(string _zhi)
        {
            string _siTian = Zhi2Qi(_zhi);

            int _index = Array.IndexOf(Zhi, _zhi);
            _index += 3;
            _index = _index < Zhi.Length ? _index : _index - Zhi.Length;
            string _zaiQuan = Zhi2Qi(Zhi[_index]);

            return [_siTian, _zaiQuan];
        }
        #endregion
        */
}

#region FiveYunSixQiForTime
public class FiveYunSixQiForTime
{
    public string Solar;
    public string Lunar;

    public string SuiYun => Year.SuiYun;
    public string ZhuYun;
    public string KeYun;

    public string SiTian => Year.SiTian;
    public string ZaiQuan => Year.ZaiQuan;
    public string ZhuQi;
    public string KeQi;
    public string JieQi;

    public FiveYunSixQiForYear Year;

    public override string ToString() => $"{Solar} {Lunar} {JieQi}\n{SuiYun}, {ZhuYun}, {KeYun}\n{SiTian}/{ZaiQuan}, {ZhuQi}, {KeQi}";
}
#endregion

#region FiveYunSixQiForYear
public class FiveYunSixQiForYear
{
    public string Year;
    public string YearGanZhi;

    public string SuiYun;
    public int[] YunKe;
    public string[] ZhuYun;
    public string[] KeYun;

    public string SiTian;
    public string ZaiQuan;
    public string[] ZhuQi;
    public string[] KeQi;
    public int[] JieQi;
}
#endregion
