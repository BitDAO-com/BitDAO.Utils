using System;

namespace BitDAO.Utils.Calendar;

public class TrueSolarTime : MeanSolarTime
{
    public StandardTime LocalTrueSolarTime;

    public TrueSolarTime(int _year, int _month, int _day, int _hour, int _minute, int _second, double _timezoneOffset, double _longitude)
        : base(_year, _month, _day, _hour, _minute, _second, _timezoneOffset, _longitude)
    {
        this.Calculate();
    }

    public TrueSolarTime(DateTime _datetime, double _timezoneOffset, double _longitude)
        : base(_datetime, _timezoneOffset, _longitude)
    {
        this.Calculate();
    }

    private void Calculate()
    {
        double _b_deg = 360 * (base.DayOfYear - 81) / 365.0;
        double _b_rad = _b_deg * Math.PI / 180.0;
        double _eot = 9.87 * Math.Sin(2 * _b_rad) - 7.53 * Math.Cos(_b_rad) - 1.5 * Math.Sin(_b_rad);

        this.LocalTrueSolarTime = base.LocalMeanSolarTime.AddMinutes(_eot);
    }
}
