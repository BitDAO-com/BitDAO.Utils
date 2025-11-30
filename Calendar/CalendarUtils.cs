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

    public static double Degree2Radian(double _degree) => Math.PI / 180.0 * _degree;

    public static double Angle2Degree360(double _degree)
    {
        _degree %= 360.0;
        if (_degree < 0) { _degree += 360.0; }
        return _degree;
    }

    private static double Angle2Degree180(double _degree)
    {
        _degree %= 360.0;
        if (_degree > 180.0) _degree -= 360.0;
        if (_degree <= -180.0) _degree += 360.0;
        return _degree;
    }

    public static double GetSunEclipticLongitude(double _julianDay)
    {
        // 儒略世纪（相对于 J2000.0）
        double _t = (_julianDay - 2451545.0) / 36525.0;
        double _t2 = _t * _t;
        double _t3 = _t2 * _t;

        // 太阳几何平均黄经 L0（度）
        double _l0 = 280.46646 + 36000.76983 * _t + 0.0003032 * _t2;

        // 太阳平近点角 M（度）
        double _m = 357.52911 + 35999.05029 * _t - 0.0001537 * _t2 - _t3 / 24490000.0;

        // 地球轨道离心率 e（如果后续只用黄经，可以不显式用 e）
        double _e = 0.016708634 - 0.000042037 * _t - 0.0000001267 * _t2;
        // 角度转弧度
        double _m_rad = Degree2Radian(_m);

        // 太阳中心差 C（度）
        double _c = (1.914602 - 0.004817 * _t - 0.000014 * _t2) * Math.Sin(_m_rad)
                  + (0.019993 - 0.000101 * _t) * Math.Sin(2.0 * _m_rad)
                  + 0.000289 * Math.Sin(3.0 * _m_rad);

        // 太阳真黄经（几何） true longitude
        double _trueLong = _l0 + _c;

        // 岁差/章动修正，得到视黄经
        double _omega = 125.04 - 1934.136 * _t;
        double _omegaRadian = Degree2Radian(_omega);

        double _lambdaApparent = _trueLong - 0.00569 - 0.00478 * Math.Sin(_omegaRadian);

        // 规范化到 [0, 360)
        return Angle2Degree360(_lambdaApparent);
    }

    public static double GetMoonEclipticLongitude(double _julianDay)
    {
        // 1. 儒略世纪（相对于 J2000.0）
        double _t = (_julianDay - 2451545.0) / 36525.0;
        double _t2 = _t * _t;
        double _t3 = _t2 * _t;
        double _t4 = _t3 * _t;

        // 2. 月亮平黄经 L'（度）
        double _l_prime = 218.3164477 + 481267.88123421 * _t - 0.0015786 * _t2 + _t3 / 538841.0 - _t4 / 65194000.0;

        // 3. 伸距 D
        double _d = 297.8501921 + 445267.1114034 * _t - 0.0018819 * _t2 + _t3 / 545868.0 - _t4 / 113065000.0;

        // 4. 太阳平近点角 M
        double _m = 357.5291092 + 35999.0502909 * _t - 0.0001536 * _t2 + _t3 / 24490000.0;

        // 5. 月亮平近点角 M'
        double _m_prime = 134.9633964 + 477198.8675055 * _t + 0.0087414 * _t2 + _t3 / 69699.0 - _t4 / 14712000.0;

        // 6. 月亮纬度参数 F
        double _f = 93.2720950 + 483202.0175233 * _t - 0.0036539 * _t2 - _t3 / 3526000.0 + _t4 / 863310000.0;

        // 归一并转弧度
        _d = Degree2Radian(Angle2Degree360(_d));
        _m = Degree2Radian(Angle2Degree360(_m));
        _m_prime = Degree2Radian(Angle2Degree360(_m_prime));
        _f = Degree2Radian(Angle2Degree360(_f));

        // 7. 使用 Meeus 表 47.A 的项来求扰动 Δλ
        //    Δλ = Σ (l_i * sin( D_i*D + M_i*M + Mp_i*M' + F_i*F )) / 1e6
        double _sumL = 0.0;

        for (int i = 0; i < MoonLongitudeData.GetLength(0); i++)
        {
            int _c_d = MoonLongitudeData[i, 0];
            int _c_m = MoonLongitudeData[i, 1];
            int _c_m_prime = MoonLongitudeData[i, 2];
            int _c_f = MoonLongitudeData[i, 3];
            int _l = MoonLongitudeData[i, 4];   // 单位: 10^-6 度

            double _arg = _c_d * _d + _c_m * _m + _c_m_prime * _m_prime + _c_f * _f;
            _sumL += _l * Math.Sin(_arg);
        }

        double _deltaLambda = _sumL / 1000000.0; // 转成度
        double _lambda = _l_prime + _deltaLambda;

        // 可选：加上章动 Δψ，修正为“视黄经”，类似太阳那一步
        // 这里只返回地心黄经（度）
        return Angle2Degree360(_lambda);
    }

    /// <summary>
    /// f(t) = λ_moon(jd) - λ_sun(jd)，范围 -180°~+180°
    /// </summary>
    public static double LunarMinusSolarLongitude(double _julianDay)
    {
        double _lambdaMoon = Angle2Degree360(GetMoonEclipticLongitude(_julianDay));
        double _lambdaSun = Angle2Degree360(GetSunEclipticLongitude(_julianDay));

        double _diff = _lambdaMoon - _lambdaSun;
        return Angle2Degree180(_diff);
    }

    /// <summary>
    /// f'(t) 数值求导
    /// </summary>
    public static double DerivativeLunarMinusSolar(double _julianDay, double _diff)
    {
        double _f1 = LunarMinusSolarLongitude(_julianDay + _diff);
        double _f2 = LunarMinusSolarLongitude(_julianDay - _diff);
        return (_f1 - _f2) / (2.0 * _diff);   // 度/天
    }

    public static double CalculateNewMoonJulianDay(double _julianDay)
    {
        double _dataStep = 0.0001;  // 数值导数步长，约 8.64 秒
        int _maxIter = 10;          // 最多迭代 10 次
        double _tol = 1e-7;       // 收敛阈值：~0.01 秒

        for (int i = 0; i < _maxIter; i++)
        {
            // 1) 计算 f(t) = λ_moon - λ_sun
            double _f = LunarMinusSolarLongitude(_julianDay);

            // 2) 计算 f'(t) 数值导数
            double _f_prime = DerivativeLunarMinusSolar(_julianDay, _dataStep);

            // 3) Newton 步长：delta = f / f'
            double delta = _f / _f_prime;   // 单位：天

            // 4) 更新时间
            _julianDay -= delta;

            // 5) 收敛判断
            if (Math.Abs(delta) < _tol) { break; }
        }

        return _julianDay;  // 收敛后的朔时刻 JD
    }

    private static int[,] MoonLongitudeData = new int[,]
    {
        // D,  M,  M', F,    l (10^-6 deg)
        {0,  0,  1,  0, 6288774, -20905355},
        {2,  0, -1,  0, 1274027,  -3699111},
        {2,  0,  0,  0,  658314,  -2955968},
        {0,  0,  2,  0,  213618,   -569925},
        {0,  1,  0,  0, -185116,     48888},
        {0,  0,  0,  2, -114332,     -3149},
        {2,  0, -2,  0,   58793,    246158},
        {2, -1, -1,  0,   57066,   -152138},
        {2,  0,  1,  0,   53322,   -170733},
        {2, -1,  0,  0,   45758,   -204586},
        {0,  1, -1,  0,  -40923,   -129620},
        {1,  0,  0,  0,  -34720,    108743},
        {0,  1,  1,  0,  -30383,    104755},
        {2,  0,  0, -2,   15327,     10321},
        {0,  0,  1,  2,  -12528,         0},
        {0,  0,  1, -2,   10980,     79661},
        {4,  0, -1,  0,   10675,    -34782},
        {0,  0,  3,  0,   10034,    -23210},
        {4,  0, -2,  0,    8548,    -21636},
        {2,  1, -1,  0,   -7888,     24208},
        {2,  1,  0,  0,   -6766,     30824},
        {1,  0, -1,  0,   -5163,     -8379},
        {1,  1,  0,  0,    4987,    -16675},
        {2, -1,  1,  0,    4036,    -12831},
        {2,  0,  2,  0,    3994,    -10445},
        {4,  0,  0,  0,    3861,    -11650},
        {2,  0, -3,  0,    3665,     14403},
        {0,  1, -2,  0,   -2689,     -7003},
        {2,  0, -1,  2,   -2602,         0},
        {2, -1, -2,  0,    2390,     10056},
        {1,  0,  1,  0,   -2348,      6322},
        {2, -2,  0,  0,    2236,     -9884},
        {0,  1,  2,  0,   -2120,      5751},
        {0,  2,  0,  0,   -2069,         0},
        {2, -2, -1,  0,    2048,     -4950},
        {2,  0,  1, -2,   -1773,      4130},
        {2,  0,  0,  2,   -1595,         0},
        {4, -1, -1,  0,    1215,     -3958},
        {0,  0,  2,  2,   -1110,         0},
        {3,  0, -1,  0,    -892,      3258},
        {2,  1,  1,  0,    -810,      2616},
        {4, -1, -2,  0,     759,     -1897},
        {0,  2, -1,  0,    -713,     -2117},
        {2,  2, -1,  0,    -700,      2354},
        {2,  1, -2,  0,     691,         0},
        {2, -1,  0, -2,     596,         0},
        {4,  0,  1,  0,     549,     -1423},
        {0,  0,  4,  0,     537,     -1117},
        {4, -1,  0,  0,     520,     -1571},
        {1,  0, -2,  0,    -487,     -1739},
        {2,  1,  0, -2,    -399,         0},
        {0,  0,  2, -2,    -381,     -4421},
        {1,  1,  1,  0,     351,         0},
        {3,  0, -2,  0,    -340,         0},
        {4,  0, -3,  0,     330,         0},
        {2, -1,  2,  0,     327,         0},
        {0,  2,  1,  0,    -323,      1165},
        {1,  1, -1,  0,     299,         0},
        {2,  0,  3,  0,     294,         0},
        {2,  0, -1, -2,       0,      8752},
    };
}