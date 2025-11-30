using System;

namespace BitDAO.Utils.Calendar;

public class MeanSolarTime : StandardTime
{
    public StandardTime StandardTime;
    public StandardTime LocalMeanSolarTime;
    public double Longitude;

    public MeanSolarTime(DateTime _datetime, double _timezoneOffset, double _longitude)
        : base(_datetime, _timezoneOffset)
    {
        this.Longitude = _longitude;
        this.Calculate();
    }

    public MeanSolarTime(int _year, int _month, int _day, int _hour, int _minute, int _second, double _timezoneOffset, double _longitude)
        : base(_year, _month, _day, _hour, _minute, _second, _timezoneOffset)
    {
        this.Longitude = _longitude;
        this.Calculate();
    }

    private void Calculate()
    {
        double _centralMeridian = base.TimezoneOffset * 15.0;
        double _longitudeDifference = this.Longitude - _centralMeridian;
        double _timeAdjustmentMinutes = _longitudeDifference * 4.0;

        this.LocalMeanSolarTime = base.Clone();
        this.LocalMeanSolarTime = base.AddMinutes(_timeAdjustmentMinutes);
    }
}