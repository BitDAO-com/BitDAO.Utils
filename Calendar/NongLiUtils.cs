using System;
using System.Collections.Generic;

namespace BitDAO.Utils.Calendar;

public class NongLiUtils
{
    public static readonly string[] Jie = ["立春", "惊蛰", "清明", "立夏",
                                           "芒种", "小暑", "立秋", "白露",
                                           "寒露", "立冬", "大雪", "小寒"];
    public static readonly string[] Qi = ["雨水", "春分", "谷雨", "小满",
                                          "夏至", "大暑", "处暑", "秋分",
                                          "霜降", "小雪", "冬至", "大寒"];
    public static readonly string[] JieQi = ["立春", "雨水", "惊蛰", "春分",
                                             "清明", "谷雨", "立夏", "小满",
                                             "芒种", "夏至", "小暑", "大暑",
                                             "立秋", "处暑", "白露", "秋分",
                                             "寒露", "霜降", "立冬", "小雪",
                                             "大雪", "冬至", "小寒", "大寒"];

    public static readonly string[] Yue = ["正", "二", "三", "四", "五", "六",
                                           "七", "八", "九", "十", "冬", "腊"];

    public static readonly string[] Ri = ["初一", "初二", "初三", "初四", "初五",
                                          "初六", "初七", "初八", "初九", "初十",
                                          "十一", "十二", "十三", "十四", "十五",
                                          "十六", "十七", "十八", "十九", "二十",
                                          "廿一", "廿二", "廿三", "廿四", "廿五",
                                          "廿六", "廿七", "廿八", "廿九", "三十"];
    public static readonly string[] Shi = ["子", "丑", "寅", "卯", "辰", "巳",
                                           "午", "未", "申", "酉", "戌", "亥"];
    public static readonly string[] Ke = ["初初", "初一", "初二", "初三",
                                          "正初", "正一", "正二", "正三"];


    #region 儒略日转农历
    public static NongLiTime JulianDay2NongLiTime(double _julianDay, StandardTime _standardTime = null)
    {
        double _timezoneOffset = _standardTime != null ? _standardTime.TimezoneOffset : 0.0;

        // 最近冬至（索引21，属“气”）
        JieQiTime _solstice = JulianDay2JieQiTime(_julianDay, _standardTime);
        int _guard = 0;
        while (_solstice.Index != 21 && _guard++ < 30) { _solstice = _solstice.PrevQi(); }

        // 冬至所在朔月 = 农历十一月（需取冬至当日或之前的朔）
        double _month11StartJD = AstronomyUtils.CalculateNewMoonJulianDay(_solstice.JulianDay);
        if (_month11StartJD > _solstice.JulianDay)
        {
            _month11StartJD = AstronomyUtils.CalculateNewMoonJulianDay(_month11StartJD - CalendarUtils.SynodicMonth);
        }

        // 从十一月起生成朔序列，覆盖 14 个月（最多出现闰月）
        List<double> _moons = [_month11StartJD];
        for (int i = 0; i < 15; i++)
        {
            double _guess = _moons[^1] + CalendarUtils.SynodicMonth;
            double _next = AstronomyUtils.CalculateNewMoonJulianDay(_guess);
            if (_next <= _moons[^1]) { _next = _moons[^1] + CalendarUtils.SynodicMonth; }
            _moons.Add(_next);
        }

        int _monthCount = _moons.Count - 1;
        int[] _monthNumbers = new int[_monthCount];
        bool[] _isLeap = new bool[_monthCount];

        int _monthNo = 11;
        bool _leapAssigned = false;

        for (int i = 0; i < _monthCount; i++)
        {
            _monthNumbers[i] = _monthNo;
            bool _hasZhongQi = ContainsZhongQi(_moons[i], _moons[i + 1]);
            _isLeap[i] = !_hasZhongQi && !_leapAssigned;
            if (_isLeap[i]) { _leapAssigned = true; }

            _monthNo = (_monthNo % 12) + 1;
        }

        // 当前所在月（按本地日期判定）
        double _offsetDays = _timezoneOffset / 24.0;
        int _currentMonthIndex = 0;
        for (int i = 0; i < _monthCount; i++)
        {
            int _monthStartDate = (int)Math.Floor(_moons[i] + 0.5 + _offsetDays);       // 本地日期编号
            int _nextMonthDate = (int)Math.Floor(_moons[i + 1] + 0.5 + _offsetDays);
            int _currentDate = (int)Math.Floor(_julianDay + 0.5 + _offsetDays);
            if (_currentDate >= _monthStartDate && _currentDate < _nextMonthDate)
            {
                _currentMonthIndex = i;
                break;
            }
        }

        int _monthStartDateLocal = (int)Math.Floor(_moons[_currentMonthIndex] + 0.5 + _offsetDays);
        int _currentDateLocal = (int)Math.Floor(_julianDay + 0.5 + _offsetDays);
        int _ri = _currentDateLocal - _monthStartDateLocal;
        int _yue = _monthNumbers[_currentMonthIndex];
        bool _isLeapMonth = _isLeap[_currentMonthIndex];

        // 春节（首个非闰正月）与年份
        double _cnyJD = double.NaN;
        for (int i = 0; i < _monthCount; i++)
        {
            if (_monthNumbers[i] == 1 && !_isLeap[i]) { _cnyJD = _moons[i]; break; }
        }

        int _cnyYear = double.IsNaN(_cnyJD)
                        ? CalendarUtils.JulianDay2DateTime(_julianDay, _timezoneOffset)[0]
                        : CalendarUtils.JulianDay2DateTime(_cnyJD, _timezoneOffset)[0];

        int _nian = (!double.IsNaN(_cnyJD) && _julianDay < _cnyJD) ? _cnyYear - 1 : _cnyYear;

        // 本地时辰/刻：子时跨日，按 23:00-01:00 归子
        int[] _dt = CalendarUtils.JulianDay2DateTime(_julianDay, _timezoneOffset);
        int _totalMinutes = _dt[3] * 60 + _dt[4];
        int _shiIndex = (_totalMinutes + 60) / 120 % 12; // 0=子,1=丑...
        int _minutesInShichen = (_totalMinutes + 60) % 120;
        int _ke = (int)Math.Floor((_minutesInShichen * 60 + _dt[5]) / 900.0); // 0..7 (每刻15分钟)

        return new NongLiTime(_nian, _yue, _ri, _shiIndex, _ke, _isLeapMonth, _standardTime);
    }
    #endregion

    #region JulianDay2JieQiTime 儒略日转节气
    public static JieQiTime JulianDay2JieQiTime(double _julianDay, StandardTime _standardTime = null)
    {
        double _lambdaNow = AstronomyUtils.NormalizeDegrees(AstronomyUtils.GetSunEclipticLongitude(_julianDay));
        double _targetLongitude = Math.Floor(_lambdaNow / 15.0) * 15.0; // 上一个节气边界
        double _jdBoundary = PrevSolarLongitudeBoundary(_julianDay, _targetLongitude);

        int _k = (int)(_targetLongitude / 15.0); // 0..23，对应 0°=春分
        int _jieQiIndex = (_k + 3) % 24;         // 偏移到立春=0

        return new JieQiTime(_jdBoundary, _jieQiIndex, _standardTime);
    }
    #endregion

    #region ContainsZhongQi 包含中气
    public static bool ContainsZhongQi(double _startJulianDay, double _endJulianDay)
    {
        JieQiTime _jieqi = JulianDay2JieQiTime(_startJulianDay);
        if (_jieqi.Index % 2 == 1
            && _jieqi.JulianDay >= _startJulianDay
            && _jieqi.JulianDay < _endJulianDay) { return true; }

        _jieqi = _jieqi.NextQi();

        return _jieqi.JulianDay >= _startJulianDay && _jieqi.JulianDay < _endJulianDay;
    }
    #endregion

    #region SolarLongitudeDifference 节气黄经
    /// <summary>
    /// 太阳视黄经与目标黄经的差值，规范到 -180°..+180°（度）
    /// </summary>
    public static double SolarLongitudeDifference(double _julianDay, double _targetLongitude)
    {
        double _lambda = AstronomyUtils.GetSunEclipticLongitude(_julianDay);
        return AstronomyUtils.Angle2Degree180(_lambda - _targetLongitude);
    }
    #endregion

    #region BisectSolarLongitude 二分法求节气时刻
    /// <summary>
    /// 在给定区间内用二分法求太阳视黄经 = 目标黄经的时刻
    /// </summary>
    public static double BisectSolarLongitude(double _targetLongitude, double _leftJulianDay, double _rightJulianDay, int _iterations = 50)
    {
        double _fLeft = SolarLongitudeDifference(_leftJulianDay, _targetLongitude);
        double _fRight = SolarLongitudeDifference(_rightJulianDay, _targetLongitude);

        for (int i = 0; i < _iterations; i++)
        {
            double _mid = 0.5 * (_leftJulianDay + _rightJulianDay);
            double _fMid = SolarLongitudeDifference(_mid, _targetLongitude);
            if (Math.Abs(_fMid) < 1e-8) { return _mid; }

            if (_fLeft * _fMid <= 0.0)
            {
                _rightJulianDay = _mid;
                _fRight = _fMid;
            }
            else
            {
                _leftJulianDay = _mid;
                _fLeft = _fMid;
            }

            if (Math.Abs(_rightJulianDay - _leftJulianDay) < 1e-8) { break; }
        }

        return 0.5 * (_leftJulianDay + _rightJulianDay);
    }
    #endregion

    #region NextSolarLongitudeBoundary 向后搜索节气时刻
    /// <summary>
    /// 从指定时刻向前搜索，找到太阳视黄经跨越目标黄经的上一个时刻
    /// </summary>
    public static double PrevSolarLongitudeBoundary(double _currentJulianDay,
                                                            double _targetLongitude,
                                                            int _maxDays = 60,
                                                            int _bisectIterations = 50)
    {
        double _jd1 = _currentJulianDay;
        double _f1 = SolarLongitudeDifference(_jd1, _targetLongitude);

        for (int i = 0; i < _maxDays; i++)
        {
            double _jd2 = _jd1 - 1.0;
            double _f2 = SolarLongitudeDifference(_jd2, _targetLongitude);

            if (Math.Abs(_f1) < 1e-8) { return _jd1; }
            if (_f1 * _f2 <= 0.0) { return BisectSolarLongitude(_targetLongitude, _jd2, _jd1, _bisectIterations); }

            _jd1 = _jd2;
            _f1 = _f2;
        }

        return _currentJulianDay; // fallback
    }
    #endregion
}
