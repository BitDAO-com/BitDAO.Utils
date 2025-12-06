using System;

namespace BitDAO.Utils.Calendar;

public class NongLiTime
{
    public int Nian, Yue, Ri, Shi, Ke;
    public bool IsLeapMonth;
    public StandardTime StandardTime;

    public NongLiTime(int _nian, int _yue, int _ri,
                      int _shi, int _ke, bool _isLeapMonth,
                      StandardTime _standardTime)
    {
        Nian = _nian;
        Yue = _yue;
        Ri = _ri;
        Shi = _shi;
        Ke = _ke;
        IsLeapMonth = _isLeapMonth;
        StandardTime = _standardTime;
    }

    public static NongLiTime FromStandardTime(StandardTime _standardTime) => NongLiUtils.JulianDay2NongLiTime(_standardTime.JulianDay, _standardTime);

    public string NianName => $"{this.Nian}年";

    public string YueName => $"{(this.IsLeapMonth ? "闰" : "")}{NongLiUtils.Yue[this.Yue - 1]}月";

    public string RiName => $"{NongLiUtils.Ri[this.Ri]}日";

    public string ShiName => $"{NongLiUtils.Shi[this.Shi]}时";

    public string KeName => $"{NongLiUtils.Ke[this.Ke]}刻";

    public override string ToString()
    {
        return $"{NianName}{YueName}{RiName}{ShiName}{KeName}";
    }
}
