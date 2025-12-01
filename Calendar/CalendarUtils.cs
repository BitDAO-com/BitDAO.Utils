using System;

namespace BitDAO.Utils.Calendar;

public class CalendarUtils
{
    public static bool IsLeapYear(int _year) => (_year % 4 == 0) && (_year % 100 != 0 || _year % 400 == 0);

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
}