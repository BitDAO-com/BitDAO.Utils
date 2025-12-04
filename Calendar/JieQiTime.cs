using System;

namespace BitDAO.Utils.Calendar;

public class JieQiTime : StandardTime
{
    public int Index;
    public JieQiTime(double _julianDay, int _index, StandardTime _standardTime = null) : base(_julianDay)
    {
        this.Index = _index;
        if (_standardTime != null)
        {
            this.TimezoneOffset = _standardTime.TimezoneOffset;
            this.Latitude = _standardTime.Latitude;
            this.Longitude = _standardTime.Longitude;
            this.Type = _standardTime.Type;
        }
    }

    public JieQiTime(double _julianDay, int _index, double _timezoneOffset = 0.0,
                     double _latitude = 0.0, double _longitude = 0.0,
                     StandardTimeType _type = StandardTimeType.Standard)
                     : base(_julianDay, _timezoneOffset,
                            _latitude, _longitude, _type)
    {
        this.Index = _index;
    }

    public JieQiType JieQiType => (JieQiType)(this.Index % 2);

    public JieQiTime Next() => Move(+1);
    public JieQiTime Prev() => Move(-1);
    public JieQiTime NextJie() => Move(+1, JieQiType.Jie);
    public JieQiTime PrevJie() => Move(-1, JieQiType.Jie);
    public JieQiTime NextQi() => Move(+1, JieQiType.Qi);
    public JieQiTime PrevQi() => Move(-1, JieQiType.Qi);

    private JieQiTime Move(int _step, JieQiType? _targetType = null)
    {
        int _candidateIndex = (this.Index + _step) % 24;
        if (_candidateIndex < 0) { _candidateIndex += 24; }

        if (_targetType.HasValue)
        {
            int _parity = (int)_targetType.Value; // Jie=0, Qi=1
            if ((_candidateIndex % 2) != _parity)
            {
                int _delta = _step >= 0 ? 1 : -1;
                _candidateIndex = (_candidateIndex + _delta + 24) % 24;
            }
        }

        // 0°=春分，对应 index=3；立春=0 => 315°
        double _targetLongitude = ((_candidateIndex + 21) % 24) * 15.0;

        double _start = this.JulianDay + (_step >= 0 ? 1.0 : -40.0);
        double _end = this.JulianDay + (_step >= 0 ? 40.0 : -1.0);

        double _jd = NongLiUtils.BisectSolarLongitude(_targetLongitude, _start, _end);
        return new JieQiTime(_jd, _candidateIndex, this);
    }
}

public enum JieQiType { Jie = 0, Qi = 1 }
