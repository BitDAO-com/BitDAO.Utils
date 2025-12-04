using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BitDAO.Utils.Calendar;

public class StandardTime
{
    public double JulianDay;
    public int Year, Month, Day, Hour, Minute, Second;
    public double TimezoneOffset;
    public double Latitude = 0.0;
    public double Longitude = 0.0;
    public StandardTimeType Type = StandardTimeType.Standard;

    #region 构造函数
    public StandardTime(double _julianDay,
                        double _timezoneOffset = 0.0,
                        double _latitude = 0.0,
                        double _longitude = 0.0,
                        StandardTimeType _type = StandardTimeType.Standard)
    {
        this.JulianDay = _julianDay;
        this.TimezoneOffset = _timezoneOffset;
        this.Latitude = _latitude;
        this.Longitude = _longitude;
        this.Type = _type;
        this.Update();
    }

    public StandardTime(int _year,
                        int _month,
                        int _day,
                        int _hour,
                        int _minute,
                        int _second,
                        double _timezoneOffset = 0.0,
                        double _latitude = 0.0,
                        double _longitude = 0.0,
                        StandardTimeType _type = StandardTimeType.Standard)
    {
        this.JulianDay = CalendarUtils.DateTime2JulianDay(_year, _month, _day, _hour, _minute, _second, _timezoneOffset);
        this.Year = _year;
        this.Month = _month;
        this.Day = _day;
        this.Hour = _hour;
        this.Minute = _minute;
        this.Second = _second;
        this.TimezoneOffset = _timezoneOffset;
        this.Latitude = _latitude;
        this.Longitude = _longitude;
        this.Type = _type;
    }

    public StandardTime(DateTime _datetime,
                        double _timezoneOffset = 0.0,
                        double _latitude = 0.0,
                        double _longitude = 0.0,
                        StandardTimeType _type = StandardTimeType.Standard) : this(_datetime.Year,
                                                                                   _datetime.Month,
                                                                                   _datetime.Day,
                                                                                   _datetime.Hour,
                                                                                   _datetime.Minute,
                                                                                   _datetime.Second,
                                                                                   _timezoneOffset,
                                                                                   _latitude,
                                                                                   _longitude,
                                                                                   _type)
    { }
    #endregion

    #region 日期更新和转换
    private void Update()
    {
        int[] _dateTime = CalendarUtils.JulianDay2DateTime(this.JulianDay, this.TimezoneOffset);
        this.Year = _dateTime[0];
        this.Month = _dateTime[1];
        this.Day = _dateTime[2];
        this.Hour = _dateTime[3];
        this.Minute = _dateTime[4];
        this.Second = _dateTime[5];
    }

    public double StandToMeanAdjustmentMinutes() => this.Longitude * 4.0 - this.TimezoneOffset * 60.0;
    public double MeanToTrueAdjustmentMinutes(bool _reverse = false)
    {
        if (!_reverse) { return AstronomyUtils.EquationOfTimeMinutes(this.JulianDay); }
        return AstronomyUtils.EquationOfTimeMinutes(this.JulianDay) * (_reverse ? -1 : 1);
    }

    public StandardTime ToStandardTime()
    {
        switch (this.Type)
        {
            case StandardTimeType.MeanSolar:
                {
                    StandardTime _time = this.AddMinutes(-this.StandToMeanAdjustmentMinutes());
                    _time.Type = StandardTimeType.Standard;
                    return _time;
                }
            case StandardTimeType.TrueSolar:
                {
                    StandardTime _mean = this.ToMeanSolarTime();
                    StandardTime _std = _mean.AddMinutes(-_mean.StandToMeanAdjustmentMinutes());
                    _std.Type = StandardTimeType.Standard;
                    return _std;
                }
        }
        return this.Clone();
    }

    public StandardTime ToMeanSolarTime()
    {
        switch (this.Type)
        {
            case StandardTimeType.Standard:
                {
                    StandardTime _time = this.AddMinutes(this.StandToMeanAdjustmentMinutes());
                    _time.Type = StandardTimeType.MeanSolar;
                    return _time;
                }
            case StandardTimeType.TrueSolar:
                {
                    // 初步估算：用 True 的 JD 算 EOT
                    double _eotGuess = this.MeanToTrueAdjustmentMinutes();
                    StandardTime _mean = this.AddMinutes(-_eotGuess);

                    // 迭代修正：用估算的 Mean 的 JD 重新算 EOT
                    double _eotCorrect = _mean.MeanToTrueAdjustmentMinutes();
                    // 差异大于 0.01 分钟才修正
                    if (Math.Abs(_eotCorrect - _eotGuess) > 0.01) { _mean = this.AddMinutes(-_eotCorrect); }

                    _mean.Type = StandardTimeType.MeanSolar;
                    return _mean;
                }
        }
        return this.Clone();
    }

    public StandardTime ToTrueSolarTime()
    {
        switch (this.Type)
        {
            case StandardTimeType.Standard:
                {
                    StandardTime _time = this.AddMinutes(this.StandToMeanAdjustmentMinutes());
                    _time = _time.AddMinutes(_time.MeanToTrueAdjustmentMinutes());
                    _time.Type = StandardTimeType.TrueSolar;
                    return _time;
                }
            case StandardTimeType.MeanSolar:
                {
                    StandardTime _time = this.AddMinutes(this.MeanToTrueAdjustmentMinutes());
                    _time.Type = StandardTimeType.TrueSolar;
                    return _time;
                }
        }
        return this.Clone();
    }
    #endregion

    #region 加减运算
    public StandardTime AddYears(int _years)
    {
        int _newYear = this.Year + _years;
        int _newDay = Math.Min(this.Day, CalendarUtils.DaysInMonth(_newYear, this.Month));
        return new StandardTime(_newYear, this.Month, _newDay,
                                this.Hour, this.Minute, this.Second,
                                this.TimezoneOffset, this.Latitude, this.Longitude, this.Type);
    }

    public StandardTime AddMonths(int _months)
    {
        int _totalMonths = this.Year * 12 + (this.Month - 1) + _months;

        // 使用地板除法处理负数
        int _newYear = (int)Math.Floor(_totalMonths / 12.0);
        int _newMonth = _totalMonths - _newYear * 12 + 1;

        int _newDay = Math.Min(this.Day, CalendarUtils.DaysInMonth(_newYear, _newMonth));

        return new StandardTime(_newYear, _newMonth, _newDay,
                                this.Hour, this.Minute, this.Second,
                                this.TimezoneOffset, this.Latitude, this.Longitude, this.Type);
    }

    public StandardTime AddDays(double _days) => new(this.JulianDay + _days, this.TimezoneOffset,
                                                     this.Latitude, this.Longitude, this.Type);

    public StandardTime AddHours(double _hours) => new(this.JulianDay + _hours / 24.0, this.TimezoneOffset,
                                                       this.Latitude, this.Longitude, this.Type);

    public StandardTime AddMinutes(double _minutes) => new(this.JulianDay + _minutes / 1440.0, this.TimezoneOffset,
                                                           this.Latitude, this.Longitude, this.Type);

    public StandardTime AddSeconds(double _seconds) => new(this.JulianDay + _seconds / 86400.0, this.TimezoneOffset,
                                                           this.Latitude, this.Longitude, this.Type);
    #endregion

    #region 辅助函数
    public int DayOfYear => (CalendarUtils.IsLeapYear(this.Year) ? CalendarUtils.DaysBeforeMonthLeap : CalendarUtils.DaysBeforeMonth)[this.Month] + this.Day;

    public StandardTime Clone() => new(this.JulianDay, this.TimezoneOffset, this.Latitude, this.Longitude, this.Type);

    public override string ToString() => $"{this.Year:D4}-{this.Month:D2}-{this.Day:D2} {this.Hour:D2}:{this.Minute:D2}:{this.Second:D2}";
    #endregion

    #region 新月
    public StandardTime NearestNewMoon() => NearestNewMoon(this.JulianDay, this.TimezoneOffset,
                                                           this.Latitude, this.Longitude, this.Type);
    public static StandardTime NearestNewMoon(double _julianDay, double _timezoneOffset = 0.0,
                                              double _latitude = 0.0, double _longitude = 0.0,
                                              StandardTimeType _type = StandardTimeType.Standard)
    {
        double _newMoon = AstronomyUtils.CalculateNewMoonJulianDay(_julianDay);
        StandardTime _time = new(_newMoon, _timezoneOffset, _latitude, _longitude, _type);
        return _time;
    }

    public StandardTime NextNewMoon() => NextNewMoon(this.JulianDay, this.TimezoneOffset,
                                                     this.Latitude, this.Longitude, this.Type);
    public static StandardTime NextNewMoon(double _julianDay, double _timezoneOffset = 0.0,
                                           double _latitude = 0.0, double _longitude = 0.0,
                                           StandardTimeType _type = StandardTimeType.Standard)
    {
        StandardTime _nearest = NearestNewMoon(_julianDay, _timezoneOffset, _latitude, _longitude, _type);
        double _nearestJulianDay = _nearest.JulianDay;
        if (_nearestJulianDay > _julianDay) { return _nearest; }

        return NearestNewMoon(_nearestJulianDay + CalendarUtils.SynodicMonth, _timezoneOffset, _latitude, _longitude, _type);
    }

    public StandardTime PrevNewMoon() => PrevNewMoon(this.JulianDay, this.TimezoneOffset,
                                                     this.Latitude, this.Longitude, this.Type);
    public static StandardTime PrevNewMoon(double _julianDay, double _timezoneOffset = 0.0,
                                           double _latitude = 0.0, double _longitude = 0.0,
                                           StandardTimeType _type = StandardTimeType.Standard)
    {
        StandardTime _nearest = NearestNewMoon(_julianDay, _timezoneOffset, _latitude, _longitude, _type);
        double _nearestJulianDay = _nearest.JulianDay;
        if (_nearestJulianDay < _julianDay) { return _nearest; }

        return NearestNewMoon(_nearestJulianDay - CalendarUtils.SynodicMonth, _timezoneOffset, _latitude, _longitude, _type);
    }
    #endregion

    #region 节气 农历
    public JieQiTime JieQi => NongLiUtils.JulianDay2JieQiTime(this.JulianDay, this);

    public NongLiTime NongLi => NongLiUtils.JulianDay2NongLiTime(this.JulianDay, this);
    #endregion

    #region 太阳/月亮，出/落/中
    public StandardTime CalculateSunRise(double _altitudeMeters = 0.0)
    {
        double _jd = this.JulianDay;
        // 海拔导致的地平线俯角修正：0.0293 * sqrt(h)
        double _dip = 0.0293 * Math.Sqrt(_altitudeMeters);
        double _standardAltitude = -0.8333333333 - _dip; // 使用更精确的 -50/60 度

        AstronomyUtils.SunMoonEventResult _result = AstronomyUtils.CalculateSunEvent(AstronomyUtils.SunMoonEventType.Rise,
                                                                                     _jd,
                                                                                     this.Latitude,
                                                                                     this.Longitude,
                                                                                     _standardAltitude);
        return new StandardTime(_result.JulianDay, this.TimezoneOffset, this.Latitude, this.Longitude, this.Type);
    }

    public StandardTime CalculateSunSet(double _altitudeMeters = 0.0)
    {
        double _jd = this.JulianDay;
        // 海拔导致的地平线俯角修正
        double _dip = 0.0293 * Math.Sqrt(_altitudeMeters);
        double _standardAltitude = -0.8333333333 - _dip;

        AstronomyUtils.SunMoonEventResult _result = AstronomyUtils.CalculateSunEvent(AstronomyUtils.SunMoonEventType.Set,
                                                                                     _jd,
                                                                                     this.Latitude,
                                                                                     this.Longitude,
                                                                                     _standardAltitude);
        return new StandardTime(_result.JulianDay, this.TimezoneOffset, this.Latitude, this.Longitude, this.Type);
    }

    public StandardTime CalculateSunTransit()
    {
        double _jd = this.JulianDay;
        AstronomyUtils.SunMoonEventResult _result = AstronomyUtils.CalculateSunEvent(AstronomyUtils.SunMoonEventType.Transit,
                                                                                     _jd,
                                                                                     this.Latitude,
                                                                                     this.Longitude);
        return new StandardTime(_result.JulianDay, this.TimezoneOffset, this.Latitude, this.Longitude, this.Type);
    }

    public StandardTime CalculateMoonRise(double _altitudeMeters = 0.0)
    {
        double _jd = this.JulianDay;
        // 海拔导致的地平线俯角修正
        double _dip = 0.0293 * Math.Sqrt(_altitudeMeters);
        // 传递给 AstronomyUtils 的“标准高度”现在只包含 Dip 部分的负值（几何地平线修正）
        // 视差、视半径、折射都在 CalculateMoonEvent 内部动态计算
        double _dipCorrection = -_dip;

        AstronomyUtils.SunMoonEventResult _result = AstronomyUtils.CalculateMoonEvent(AstronomyUtils.SunMoonEventType.Rise,
                                                                                     _jd,
                                                                                     this.Latitude,
                                                                                     this.Longitude,
                                                                                     _dipCorrection);
        return new StandardTime(_result.JulianDay, this.TimezoneOffset, this.Latitude, this.Longitude, this.Type);
    }

    public StandardTime CalculateMoonSet(double _altitudeMeters = 0.0)
    {
        double _jd = this.JulianDay;
        // 海拔导致的地平线俯角修正
        double _dip = 0.0293 * Math.Sqrt(_altitudeMeters);
        double _dipCorrection = -_dip;

        AstronomyUtils.SunMoonEventResult _result = AstronomyUtils.CalculateMoonEvent(AstronomyUtils.SunMoonEventType.Set,
                                                                                     _jd,
                                                                                     this.Latitude,
                                                                                     this.Longitude,
                                                                                     _dipCorrection);
        return new StandardTime(_result.JulianDay, this.TimezoneOffset, this.Latitude, this.Longitude, this.Type);
    }

    public StandardTime CalculateMoonTransit()
    {
        double _jd = this.JulianDay;
        AstronomyUtils.SunMoonEventResult _result = AstronomyUtils.CalculateMoonEvent(AstronomyUtils.SunMoonEventType.Transit,
                                                                                     _jd,
                                                                                     this.Latitude,
                                                                                     this.Longitude);
        return new StandardTime(_result.JulianDay, this.TimezoneOffset, this.Latitude, this.Longitude, this.Type);
    }
    #endregion

    #region 比较运算符
    public int CompareTo(StandardTime _other)
    {
        if (_other is null) { return 1; }
        double _jd1 = this.JulianDay;
        double _jd2 = _other.JulianDay;
        return _jd1 < _jd2 ? -1 : _jd1 > _jd2 ? 1 : 0;
    }

    public bool Equals(StandardTime _other)
    {
        if (_other is null) { return false; }
        double _eps = 1e-9;
        return Math.Abs(this.JulianDay - _other.JulianDay) < _eps;
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
        long _key = (long)Math.Round(this.JulianDay * _scale);
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
    #endregion

    #region 类型枚举
    public enum StandardTimeType
    {
        Standard,
        MeanSolar,
        TrueSolar,
    }
    #endregion
}
