using System;

namespace BitDAO.Utils.Calendar;

public class StandardTime
{
    public int Year, Month, Day, Hour, Minute, Second;
    public double TimezoneOffset;
    public const double SynodicMonth = 29.530588861;
    public const double Epsilon = 1e-6;    // 判断“同一时刻”的容差（天） ~0.086 秒

    public StandardTime(double _julianDay, double _timezoneOffset = 0.0)
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

        this.Year = _year;
        this.Month = _month;
        this.Day = _day;
        this.Hour = _hour;
        this.Minute = _minute;
        this.Second = _second;
        this.TimezoneOffset = _timezoneOffset;
    }

    public StandardTime(int _year, int _month, int _day, int _hour, int _minute, int _second, double _timezoneOffset = 0.0)
    {
        this.Year = _year;
        this.Month = _month;
        this.Day = _day;
        this.Hour = _hour;
        this.Minute = _minute;
        this.Second = _second;
        this.TimezoneOffset = _timezoneOffset;
    }

    public StandardTime(DateTime _datetime, double _timezoneOffset = 0.0)
        : this(_datetime.Year, _datetime.Month, _datetime.Day, _datetime.Hour, _datetime.Minute, _datetime.Second, _timezoneOffset) { }

    public StandardTime AddYears(int _years) => new(this.Year + _years, this.Month, this.Day, this.Hour, this.Minute, this.Second, this.TimezoneOffset);

    public StandardTime AddMonths(int _months)
    {
        int _totalMonths = this.Month + _months;
        int _newYear = this.Year + (_totalMonths - 1) / 12;
        int _newMonth = ((_totalMonths - 1) % 12) + 1;
        return new StandardTime(_newYear, _newMonth, this.Day, this.Hour, this.Minute, this.Second, this.TimezoneOffset);
    }

    public StandardTime AddDays(double _days) => this.AddSeconds((int)Math.Floor(_days * 86400.0));
    public StandardTime AddDays(int _days)
    {
        int _newYear = this.Year;
        int _newMonth = this.Month;
        int _newDay = this.Day + _days;

        if (_days >= 0)
        {
            while (true)
            {
                int _dim = CalendarUtils.DaysInMonth(_newYear, _newMonth);
                if (_newDay <= _dim) break;

                _newDay -= _dim;
                _newMonth++;
                if (_newMonth > 12) { _newMonth = 1; _newYear++; }
            }
        }
        else
        {
            while (_newDay <= 0)
            {
                _newMonth--;
                if (_newMonth < 1) { _newMonth = 12; _newYear--; }
                _newDay += CalendarUtils.DaysInMonth(_newYear, _newMonth);
            }
        }

        return new StandardTime(_newYear, _newMonth, _newDay, this.Hour, this.Minute, this.Second, this.TimezoneOffset);
    }

    public StandardTime AddHours(double _hours) => this.AddSeconds((int)Math.Floor(_hours * 3600.0));
    public StandardTime AddHours(int _hours)
    {
        int _totalHours = this.Hour + _hours;
        int _dayDelta = _totalHours / 24;
        int _newHour = _totalHours % 24;
        if (_newHour < 0) { _newHour += 24; _dayDelta -= 1; }

        StandardTime _date = this.AddDays(_dayDelta);
        _date.Hour = _newHour;
        return _date;
    }

    public StandardTime AddMinutes(double _minutes) => this.AddSeconds((int)Math.Floor(_minutes * 60.0));
    public StandardTime AddMinutes(int _minutes)
    {
        int _totalMinutes = this.Minute + _minutes;
        int _hourDelta = _totalMinutes / 60;
        int _newMinute = _totalMinutes % 60;
        if (_newMinute < 0) { _newMinute += 60; _hourDelta -= 1; }

        StandardTime _date = this.AddHours(_hourDelta);
        _date.Minute = _newMinute;
        return _date;
    }

    public StandardTime AddSeconds(double _seconds) => this.AddSeconds((int)Math.Floor(_seconds));
    public StandardTime AddSeconds(int _seconds)
    {
        int _totalSeconds = this.Second + _seconds;
        int _minuteDelta = _totalSeconds / 60;
        int _newSecond = _totalSeconds % 60;
        if (_newSecond < 0) { _newSecond += 60; _minuteDelta -= 1; }

        StandardTime _date = this.AddMinutes(_minuteDelta);
        _date.Second = _newSecond;
        return _date;
    }

    public StandardTime Clone() => new(this.Year, this.Month, this.Day, this.Hour, this.Minute, this.Second, this.TimezoneOffset);

    public int DayOfYear
    {
        get
        {
            int _dayOfYear = this.Day;
            for (int m = 1; m < this.Month; m++) { _dayOfYear += CalendarUtils.DaysInMonth(this.Year, m); }
            return _dayOfYear;
        }
    }

    public override string ToString() => $"{this.Year:D4}-{this.Month:D2}-{this.Day:D2} {this.Hour:D2}:{this.Minute:D2}:{this.Second:D2}";

    public double ToJulianDay()
    {
        long _year = this.Year;
        if (_year <= 0) { _year += 1; }

        long _month = this.Month;
        if (_month <= 2) { _year -= 1; _month += 12; }

        long _a = _year / 100;
        long _b = 2 - _a + _a / 4;

        double _julianDay = Math.Floor(365.25 * (_year + 4716))
                    + Math.Floor(30.6001 * (_month + 1))
                    + this.Day + _b - 1524.5;

        double _totalSeconds = this.Hour * 3600.0 + this.Minute * 60.0 + this.Second - this.TimezoneOffset * 3600.0;
        return _julianDay + _totalSeconds / 86400.0;
    }

    public long ToJulianSecond(bool _fromJ2000 = true)
    {
        double _jd = this.ToJulianDay();
        return _fromJ2000 ? (long)Math.Round((_jd - 2451545.0) * 86400.0) : (long)Math.Round(_jd * 86400.0);
    }

    public StandardTime NearestNewMoon() => NearestNewMoon(this.ToJulianDay());

    public static StandardTime NearestNewMoon(double _julianDay)
    {
        double _newMoon = AstronomyUtils.CalculateNewMoonJulianDay(_julianDay);
        StandardTime _time = new(_newMoon);
        //if (_time.Second >= 30) { _time = _time.AddMinutes(1); }
        //_time.Second = 0;
        return _time;
    }

    public static StandardTime NextNewMoon(double _julianDay) => NearestNewMoon(_julianDay + SynodicMonth);
    public StandardTime NextNewMoon() => NextNewMoon(this.ToJulianDay());

    public static StandardTime PrevNewMoon(double _julianDay) => NearestNewMoon(_julianDay - SynodicMonth);
    public StandardTime PrevNewMoon() => PrevNewMoon(this.ToJulianDay());

    public StandardTime Sunrise(double _latitudeDegree, double _longitudeDegree)
    {
        double _jd = this.ToJulianDay();
        AstronomyUtils.RiseSetResult _result = AstronomyUtils.FindSunRiseSet(_jd, _latitudeDegree, _longitudeDegree, true, -0.833);
        return new StandardTime(_result.JulianDay, this.TimezoneOffset);
    }

    public StandardTime Sunset(double _latitudeDegree, double _longitudeDegree)
    {
        double _jd = this.ToJulianDay();
        AstronomyUtils.RiseSetResult _result = AstronomyUtils.FindSunRiseSet(_jd, _latitudeDegree, _longitudeDegree, false, -0.833);
        return new StandardTime(_result.JulianDay, this.TimezoneOffset);
    }

    public int CompareTo(StandardTime _other)
    {
        if (_other is null) { return 1; }
        double _jd1 = this.ToJulianDay();
        double _jd2 = _other.ToJulianDay();
        return _jd1 < _jd2 ? -1 : _jd1 > _jd2 ? 1 : 0;
    }

    public bool Equals(StandardTime _other)
    {
        if (_other is null) { return false; }
        double _eps = 1e-9; // tolerance in days (~0.086 ms)
        return Math.Abs(this.ToJulianDay() - _other.ToJulianDay()) < _eps;
    }

    public override bool Equals(object _other)
    {
        if (_other is null) { return false; }
        if (_other is not StandardTime _otherTime) { return false; }
        return this.Equals(_otherTime);
    }

    public override int GetHashCode()
    {
        double _scale = 1e-9;
        long _key = (long)Math.Round(this.ToJulianDay() * _scale);
        return _key.GetHashCode();
    }

    public static bool operator ==(StandardTime _a, StandardTime _b)
    {
        if (ReferenceEquals(_a, _b)) { return true; }
        if (_a is null || _b is null) { return false; }
        return _a.Equals(_b);
    }

    public static bool operator !=(StandardTime _a, StandardTime _b) => !(_a == _b);

    public static bool operator <(StandardTime _a, StandardTime _b)
    {
        if (_a is null) { return _b is not null; }
        return _a.CompareTo(_b) < 0;
    }

    public static bool operator >(StandardTime _a, StandardTime _b)
    {
        if (_a is null) { return false; }
        return _a.CompareTo(_b) > 0;
    }

    public static bool operator <=(StandardTime _a, StandardTime _b) => !(_a > _b);

    public static bool operator >=(StandardTime _a, StandardTime _b) => !(_a < _b);
}
