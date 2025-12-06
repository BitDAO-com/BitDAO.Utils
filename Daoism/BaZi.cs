using System;
using System.Collections.Generic;
using System.Linq;
using BitDAO.Utils.Calendar;

namespace BitDAO.Utils.Daoism;

public class BaZi
{
    public enum BaZiGender { Male, Female }

    public NongLiTime NongLiTime;
    public BaZiGender Gender;


    public int NianGanIndex;
    public string NianGan => DaoismUtils.Gan[this.NianGanIndex];
    public int NianZhiIndex;
    public string NianZhi => DaoismUtils.Zhi[this.NianZhiIndex];
    public string NianGanZhi => $"{NianGan}{NianZhi}";
    public int YueGanIndex;
    public string YueGan => DaoismUtils.Gan[this.YueGanIndex];
    public int YueZhiIndex;
    public string YueZhi => DaoismUtils.Zhi[this.YueZhiIndex];
    public string YueGanZhi => $"{YueGan}{YueZhi}";
    public int RiGanIndex;
    public string RiGan => DaoismUtils.Gan[this.RiGanIndex];
    public int RiZhiIndex;
    public string RiZhi => DaoismUtils.Zhi[this.RiZhiIndex];
    public string RiGanZhi => $"{RiGan}{RiZhi}";
    public int ShiGanIndex;
    public string ShiGan => DaoismUtils.Gan[this.ShiGanIndex];
    public int ShiZhiIndex;
    public string ShiZhi => DaoismUtils.Zhi[this.ShiZhiIndex];
    public string ShiGanZhi => $"{ShiGan}{ShiZhi}";
    public string NianWuXing => $"{DaoismUtils.GanWuXing[NianGan]}{DaoismUtils.ZhiWuXing[NianZhi]}";
    public string YueWuXing => $"{DaoismUtils.GanWuXing[YueGan]}{DaoismUtils.ZhiWuXing[YueZhi]}";
    public string RiWuXing => $"{DaoismUtils.GanWuXing[RiGan]}{DaoismUtils.ZhiWuXing[RiZhi]}";
    public string ShiWuXing => $"{DaoismUtils.GanWuXing[ShiGan]}{DaoismUtils.ZhiWuXing[ShiZhi]}";
    public string[] NianCangGan => DaoismUtils.ZhiCangGan[NianZhi];
    public string[] YueCangGan => DaoismUtils.ZhiCangGan[YueZhi];
    public string[] RiCangGan => DaoismUtils.ZhiCangGan[RiZhi];
    public string[] ShiCangGan => DaoismUtils.ZhiCangGan[ShiZhi];
    public string[] NianCangGanWuXing => CalculateGanWuXing(NianCangGan);
    public string[] YueCangGanWuXing => CalculateGanWuXing(YueCangGan);
    public string[] RiCangGanWuXing => CalculateGanWuXing(RiCangGan);
    public string[] ShiCangGanWuXing => CalculateGanWuXing(ShiCangGan);
    public string NianXingYun => ShiErZhangSheng.GetByGanZhi(this.RiGan, this.NianZhi);
    public string YueXingYun => ShiErZhangSheng.GetByGanZhi(this.RiGan, this.YueZhi);
    public string RiXingYun => ShiErZhangSheng.GetByGanZhi(this.RiGan, this.RiZhi);
    public string ShiXingYun => ShiErZhangSheng.GetByGanZhi(this.RiGan, this.ShiZhi);
    public string NianZiZuo => ShiErZhangSheng.GetByGanZhi(this.NianGan, this.NianZhi);
    public string YueZiZuo => ShiErZhangSheng.GetByGanZhi(this.YueGan, this.YueZhi);
    public string RiZiZuo => ShiErZhangSheng.GetByGanZhi(this.RiGan, this.RiZhi);
    public string ShiZiZuo => ShiErZhangSheng.GetByGanZhi(this.ShiGan, this.ShiZhi);
    public string NianNaYin => DaoismUtils.NaYin[this.NianGanZhi];
    public string YueNaYin => DaoismUtils.NaYin[this.YueGanZhi];
    public string RiNaYin => DaoismUtils.NaYin[this.RiGanZhi];
    public string ShiNaYin => DaoismUtils.NaYin[this.ShiGanZhi];
    public string NianShiShenGan => DaoismUtils.ShiShen[$"{RiGan}{NianGan}"];
    public string YueShiShenGan => DaoismUtils.ShiShen[$"{RiGan}{YueGan}"];
    public string RiShiShenGan = "日主";
    public string ShiShiShenGan => DaoismUtils.ShiShen[$"{RiGan}{ShiGan}"];
    public string[] NianShiShenZhi => CalculateShiShenZhi(NianZhi, RiGan);
    public string[] YueShiShenZhi => CalculateShiShenZhi(YueZhi, RiGan);
    public string[] RiShiShenZhi => CalculateShiShenZhi(RiZhi, RiGan);
    public string[] ShiShiShenZhi => CalculateShiShenZhi(ShiZhi, RiGan);
    public string NianKong => CalculateXunKong(NianGanIndex, NianZhiIndex);
    public string RiKong => CalculateXunKong(RiGanIndex, RiZhiIndex);
    private WuXing wuxing = null;
    public WuXing WuXing { get { if (this.wuxing == null) { wuxing = new WuXing(this); } return this.wuxing; } }
    private ShenSha shensha = null;
    public ShenSha ShenSha { get { if (this.shensha == null) { shensha = ShenSha.FromBaZi(this); } return this.shensha; } }
    private string taiYuan = "";
    public string TaiYuan { get { if (this.taiYuan == "") { this.taiYuan = this.CalculateTaiYuan(); } return this.taiYuan; } }
    private string mingGong = "";
    public string MingGong { get { if (this.mingGong == "") { this.mingGong = this.CalculateMingGong(); } return this.mingGong; } }
    private string shenGong = "";
    public string ShenGong { get { if (this.shenGong == "") { this.shenGong = this.CalculateShenGong(); } return this.shenGong; } }


    public BaZi(NongLiTime _nongliTime, BaZiGender _gender = BaZiGender.Male)
    {
        NongLiTime = _nongliTime;
        Gender = _gender;

        int _nianGanIndex = (_nongliTime.Nian - 4) % 10;
        int _nianZhiIndex = (_nongliTime.Nian - 4) % 12;
        if (_nianGanIndex < 0) { _nianGanIndex += 10; }
        if (_nianZhiIndex < 0) { _nianZhiIndex += 12; }
        int _nianGanOffset = _nianGanIndex;
        int _nianZhiOffset = _nianZhiIndex;

        JieQiTime _currentJieQi = _nongliTime.StandardTime.JieQi;
        JieQiTime _lichun = _currentJieQi;
        double _liChunJd = _lichun.JulianDay;
        while (_lichun.Index != 0) { _lichun = _lichun.PrevJie(); _liChunJd = _lichun.JulianDay; }

        if (_nongliTime.Nian == _nongliTime.StandardTime.Year)
        {
            // 农历年等于公历年，需判断立春前后
            if (_nongliTime.StandardTime.JulianDay < _liChunJd)
            {
                _nianGanOffset--;
                _nianZhiOffset--;
            }
        }
        else if (_nongliTime.Nian < _nongliTime.StandardTime.Year)
        {
            // 农历年小于公历年，一定是立春之后
            if (_nongliTime.StandardTime.JulianDay >= _liChunJd)
            {
                _nianGanOffset++;
                _nianZhiOffset++;
            }
        }
        _nianGanIndex = (_nianGanOffset < 0 ? _nianGanOffset + 10 : _nianGanOffset) % 10;
        _nianZhiIndex = (_nianZhiOffset < 0 ? _nianZhiOffset + 12 : _nianZhiOffset) % 12;
        this.NianGanIndex = _nianGanIndex;
        this.NianZhiIndex = _nianZhiIndex;

        JieQiTime _monthJieQi = _currentJieQi.Index % 2 == 0 ? _currentJieQi : _currentJieQi.PrevJie();
        int _monthOrderFromLiChun = _monthJieQi.Index / 2; // 0=寅月(立春)
        int _firstMonthGanIndex = (_nianGanIndex * 2 + 2) % 10; // 甲/己年起丙寅
        int _yueGanIndex = (_firstMonthGanIndex + _monthOrderFromLiChun) % 10;
        int _yueZhiIndex = (2 + _monthOrderFromLiChun) % 12;
        this.YueGanIndex = _yueGanIndex;
        this.YueZhiIndex = _yueZhiIndex;

        double _localJulianDay = _nongliTime.StandardTime.JulianDay + _nongliTime.StandardTime.TimezoneOffset / 24.0;
        int _dayNumber = (int)Math.Floor(_localJulianDay + 0.5);
        int _riIndex = (_dayNumber + 49) % 60; // 0=甲子
        if (_riIndex < 0) { _riIndex += 60; }
        int _riGanIndex = _riIndex % 10;
        int _riZhiIndex = _riIndex % 12;
        this.RiGanIndex = _riGanIndex;
        this.RiZhiIndex = _riZhiIndex;

        int _totalMinutes = _nongliTime.StandardTime.Hour * 60 + _nongliTime.StandardTime.Minute;
        int _shiZhiIndex = (_totalMinutes + 60) / 120 % 12; // 23:00-00:59 归子时
        int _shiGanIndex = (_riGanIndex % 5 * 2 + _shiZhiIndex) % 10;
        this.ShiGanIndex = _shiGanIndex;
        this.ShiZhiIndex = _shiZhiIndex;
    }

    public override string ToString()
    {
        return $"{NianGan}{NianZhi} {YueGan}{YueZhi} {RiGan}{RiZhi} {ShiGan}{ShiZhi}";
    }


    private static string[] CalculateGanWuXing(string[] _gans)
    {
        string[] _result = new string[_gans.Length];
        for (int i = 0; i < _gans.Length; i++) { _result[i] = DaoismUtils.GanWuXing[_gans[i]]; }
        return _result;
    }

    private static string[] CalculateShiShenZhi(string _zhi, string _riGan)
    {
        string[] _cangGan = DaoismUtils.ZhiCangSiShen[_zhi];
        List<string> _list = new(_cangGan.Length);
        _list.AddRange(_cangGan.Select(_gan => DaoismUtils.ShiShen[$"{_riGan}{_gan}"]));
        return [.. _list];
    }

    /// <summary>
    /// 获取干支所在旬对应的旬空(空亡)
    /// </summary>
    /// <returns>旬空(空亡)</returns>
    public static string CalculateXunKong(int _ganIndex, int _zhiIndex)
    {
        int _diff = _ganIndex - _zhiIndex;
        if (_diff < 0) { _diff += 12; }
        return DaoismUtils.XunKong[_diff / 2];
    }

    private string CalculateTaiYuan()
    {
        int _ganIndex = this.YueGanIndex + 1;
        int _zhiIndex = this.YueZhiIndex + 3;
        _ganIndex -= _ganIndex >= 10 ? 10 : 0;
        _zhiIndex -= _zhiIndex >= 12 ? 12 : 0;
        return $"{DaoismUtils.Gan[_ganIndex]}{DaoismUtils.Zhi[_zhiIndex]}";
    }

    private string CalculateMingGong()
    {
        int _jieQiIndex = this.NongLiTime.StandardTime.JieQi.Index;

        int _zhiIndex1 = _jieQiIndex / 2 + 1;
        int _zhiIndex2 = this.ShiZhiIndex + 1;
        int _zhiIndex = 14 - _zhiIndex1 + (4 - _zhiIndex2);
        _zhiIndex -= _zhiIndex > 12 ? 12 : 0;
        _zhiIndex += _zhiIndex <= 0 ? 12 : 0;

        int _ganIndex1 = Array.IndexOf(DaoismUtils.Gan, DaoismUtils.WuHuDun[NianGan]) + 1;
        int _ganIndex2 = (_zhiIndex - 3 + 12) % 12;
        int _ganIndex = _ganIndex1 + _ganIndex2 + 10;
        _ganIndex %= 10;

        return $"{DaoismUtils.Gan[_ganIndex - 1]}{DaoismUtils.Zhi[_zhiIndex - 1]}";
    }

    private string CalculateShenGong()
    {
        int _jieQiIndex = this.NongLiTime.StandardTime.JieQi.Index;

        int _zhiIndex1 = _jieQiIndex / 2 + 1;
        int _zhiIndex2 = this.ShiZhiIndex + 1;
        int _zhiIndex = _zhiIndex1 + (_zhiIndex2 - 10);
        _zhiIndex %= 12;
        _zhiIndex -= _zhiIndex > 12 ? 12 : 0;
        _zhiIndex += _zhiIndex <= 0 ? 12 : 0;

        int _ganIndex1 = Array.IndexOf(DaoismUtils.Gan, DaoismUtils.WuHuDun[NianGan]) + 1;
        int _ganIndex2 = (_zhiIndex - 3 + 12) % 12;
        int _ganIndex = _ganIndex1 + _ganIndex2 + 10;
        _ganIndex %= 10;
        _ganIndex -= _ganIndex > 10 ? 10 : 0;
        _ganIndex += _ganIndex <= 0 ? 10 : 0;

        return $"{DaoismUtils.Gan[_ganIndex - 1]}{DaoismUtils.Zhi[_zhiIndex - 1]}";
    }
}
