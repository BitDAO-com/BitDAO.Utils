using System;

namespace BitDAO.Utils.Calendar;

public class CalendarUtils
{
    public static double SynodicMonth = 29.530588861;

    public static bool IsLeapYear(int _year) => (_year % 4 == 0) && (_year % 100 != 0 || _year % 400 == 0);

    public static readonly int[] DaysBeforeMonth = [0, 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334];
    public static readonly int[] DaysBeforeMonthLeap = [0, 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335];

    public static int DaysInMonth(int _year, int _month)
    {
        return _month switch
        {
            1 => 31,
            2 => IsLeapYear(_year) ? 29 : 28,
            3 => 31,
            4 => 30,
            5 => 31,
            6 => 30,
            7 => 31,
            8 => 31,
            9 => 30,
            10 => 31,
            11 => 30,
            12 => 31,
            _ => throw new ArgumentOutOfRangeException(nameof(_month), "Month must be 1..12")
        };
    }

    public static double DateTime2JulianDay(int _year, int _month, int _day,
                                            int _hour, int _minute, int _second,
                                            double _timezoneOffset = 0.0)
    {
        if (_year <= 0) { _year += 1; }
        if (_month <= 2) { _year -= 1; _month += 12; }

        long _a = _year / 100;
        long _b = 2 - _a + _a / 4;

        double _julianDay = Math.Floor(365.25 * (_year + 4716))
                          + Math.Floor(30.6001 * (_month + 1))
                          + _day + _b - 1524.5;

        double _totalSeconds = _hour * 3600.0 + _minute * 60.0 + _second - _timezoneOffset * 3600.0;
        return _julianDay + _totalSeconds / 86400.0;
    }

    public static int[] JulianDay2DateTime(double _julianDay, double _timezoneOffset = 0.0)
    {
        _julianDay += _timezoneOffset / 24.0;

        double _zf = _julianDay + 0.5;
        int _z = (int)Math.Floor(_zf);  // 整数部分
        double _f = _zf - _z;           // 小数部分

        int _a = _z;
        // 修复: 增加格里高利历修正 (JD >= 2299161 对应 1582年10月15日之后)
        if (_z >= 2299161)
        {
            int _alpha = (int)((_z - 1867216.25) / 36524.25);
            _a = _z + 1 + _alpha - (_alpha / 4);
        }

        int _b = _a + 1524;
        int _c = (int)((_b - 122.1) / 365.25);
        int _d = (int)(365.25 * _c);
        int _e = (int)((_b - _d) / 30.6001);

        double _dayDouble = _b - _d - Math.Floor(30.6001 * _e) + _f;

        int _month = (_e < 14) ? (_e - 1) : (_e - 13);
        int _year = (_month > 2) ? (_c - 4716) : (_c - 4715);
        int _day = (int)Math.Floor(_dayDouble);

        double _fd = _dayDouble - Math.Floor(_dayDouble);
        double _totalSeconds = _fd * 86400.0;

        int _hour = (int)(_totalSeconds / 3600.0);
        _totalSeconds -= _hour * 3600.0;

        int _minute = (int)(_totalSeconds / 60.0);
        _totalSeconds -= _minute * 60.0;

        int _second = (int)Math.Round(_totalSeconds);

        if (_second >= 60) { _second -= 60; _minute++; }
        if (_minute >= 60) { _minute -= 60; _hour++; }
        if (_hour >= 24) { _hour -= 24; _day++; }

        return [_year, _month, _day, _hour, _minute, _second];
    }
}