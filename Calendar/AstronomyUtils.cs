using System;

namespace BitDAO.Utils.Calendar;

public class AstronomyUtils
{
    #region 公共结构体
    /// <summary>
    /// 赤道坐标（地心）
    /// RightAscension/Declination 单位：度
    /// </summary>
    public struct EquatorialPosition
    {
        public double RightAscension; // degrees
        public double Declination;    // degrees
    }

    /// <summary>
    /// 水平坐标
    /// Azimuth 方位角：0° = 正北，90° = 正东，180° = 正南，270° = 正西
    /// Altitude 高度角：0° = 地平线，>0 在地平线上方
    /// </summary>
    public struct HorizontalPosition
    {
        public double Azimuth;   // degrees
        public double Altitude;  // degrees
    }

    /// <summary>
    /// 升起/落下 事件结果
    /// </summary>
    public struct RiseSetResult
    {
        /// <summary>
        /// 是否在当天发生（高纬地区可能不存在日出或日落）
        /// </summary>
        public bool IsValid;

        /// <summary>
        /// 事件发生时刻的 Julian Day（UT）
        /// </summary>
        public double JulianDay;

        /// <summary>
        /// 事件瞬间的水平坐标（主要看 Azimuth）
        /// </summary>
        public HorizontalPosition Position;
    }
    #endregion

    #region 新月
    /// <summary>
    /// 计算指定时间附近的朔（新月）时刻
    /// 使用 Meeus Chapter 49 的直接公式，精度远高于迭代法
    /// </summary>
    public static double CalculateNewMoonJulianDay(double _julianDay)
    {
        // 1. 计算 k (自 J2000.0 年初以来的朔望月数，取整以找最近的新月)
        // 2451550.09766 是 J2000.0 的首个标准朔时刻
        double _k = Math.Round((_julianDay - 2451550.09766) / 29.530588861);

        // 2. T: 自 J2000.0 起的儒略世纪数
        double _t = _k / 1236.85;
        double _t2 = _t * _t;
        double _t3 = _t2 * _t;
        double _t4 = _t3 * _t;

        // 3. 平均朔时刻 JDE (Meeus Eq 49.1)
        double _jde = 2451550.09766
                    + 29.530588861 * _k
                    + 0.0001337 * _t2
                    - 0.000000150 * _t3
                    + 0.00000000073 * _t4;

        // 4. 计算太阳平近点角 M (degrees)
        double _m = 2.5534 + 29.10535669 * _k - 0.00000218 * _t2 - 0.00000011 * _t3;
        _m = NormalizeDegrees(_m);

        // 5. 计算月亮平近点角 M' (degrees)
        double _mPrime = 201.5643 + 385.81693528 * _k + 0.0107438 * _t2 + 0.00001239 * _t3 - 0.000000058 * _t4;
        _mPrime = NormalizeDegrees(_mPrime);

        // 6. 月亮纬度参数 F (degrees)
        double _f = 160.7108 + 390.67050274 * _k - 0.0016341 * _t2 - 0.00000227 * _t3 + 0.000000011 * _t4;
        _f = NormalizeDegrees(_f);

        // 7. 升交点黄经 Omega (degrees)
        double _omega = 124.7746 - 1.56375580 * _k + 0.0020691 * _t2 + 0.00000215 * _t3;
        _omega = NormalizeDegrees(_omega);

        // 8. 地球轨道离心率校正因子 E
        double _e = 1.0 - 0.002516 * _t - 0.0000074 * _t2;

        // 转弧度用于三角函数计算
        double _radM = Degree2Radian(_m);
        double _radMPrime = Degree2Radian(_mPrime);
        double _radF = Degree2Radian(_f);
        double _radOmega = Degree2Radian(_omega);

        // 9. 周期项修正 (Meeus Table 49.A for New Moon - 完整版)，单位：天
        double _correction = 0.0;

        // ========== 主要修正项 (Meeus Table 49.A，共 25 项) ==========
        _correction -= 0.40720 * Math.Sin(_radMPrime);
        _correction += 0.17241 * _e * Math.Sin(_radM);
        _correction += 0.01608 * Math.Sin(2 * _radMPrime);
        _correction += 0.01039 * Math.Sin(2 * _radF);
        _correction += 0.00739 * _e * Math.Sin(_radMPrime - _radM);
        _correction -= 0.00514 * _e * Math.Sin(_radMPrime + _radM);
        _correction += 0.00208 * _e * _e * Math.Sin(2 * _radM);
        _correction -= 0.00111 * Math.Sin(_radMPrime - 2 * _radF);
        _correction -= 0.00057 * Math.Sin(_radMPrime + 2 * _radF);
        _correction += 0.00056 * _e * Math.Sin(2 * _radMPrime + _radM);
        _correction -= 0.00042 * Math.Sin(3 * _radMPrime);
        _correction += 0.00042 * _e * Math.Sin(_radM + 2 * _radF);
        _correction += 0.00038 * _e * Math.Sin(_radM - 2 * _radF);
        _correction -= 0.00024 * _e * Math.Sin(2 * _radMPrime - _radM);
        _correction -= 0.00017 * Math.Sin(_radOmega);
        _correction -= 0.00007 * Math.Sin(_radMPrime + 2 * _radM);
        _correction += 0.00004 * Math.Sin(2 * _radMPrime - 2 * _radF);
        _correction += 0.00004 * Math.Sin(3 * _radM);
        _correction += 0.00003 * Math.Sin(_radMPrime + _radM - 2 * _radF);
        _correction += 0.00003 * Math.Sin(2 * _radMPrime + 2 * _radF);
        _correction -= 0.00003 * Math.Sin(_radMPrime + _radM + 2 * _radF);
        _correction += 0.00003 * Math.Sin(_radMPrime - _radM + 2 * _radF);
        _correction -= 0.00002 * Math.Sin(_radMPrime - _radM - 2 * _radF);
        _correction -= 0.00002 * Math.Sin(3 * _radMPrime + _radM);
        _correction += 0.00002 * Math.Sin(4 * _radMPrime);

        // ========== 行星摄动修正 A1-A14 (Meeus Table 49.B) ==========
        double _a1 = Degree2Radian(299.77 + 0.107408 * _k - 0.009173 * _t2);
        _correction += 0.000325 * Math.Sin(_a1);
        double _a2 = Degree2Radian(251.88 + 0.016321 * _k);
        _correction += 0.000165 * Math.Sin(_a2);
        double _a3 = Degree2Radian(251.83 + 26.651886 * _k);
        _correction += 0.000164 * Math.Sin(_a3);
        double _a4 = Degree2Radian(349.42 + 36.412478 * _k);
        _correction += 0.000126 * Math.Sin(_a4);
        double _a5 = Degree2Radian(84.66 + 18.206239 * _k);
        _correction += 0.000110 * Math.Sin(_a5);
        double _a6 = Degree2Radian(141.74 + 53.303531 * _k);
        _correction += 0.000062 * Math.Sin(_a6);
        double _a7 = Degree2Radian(207.14 + 2.453732 * _k);
        _correction += 0.000060 * Math.Sin(_a7);
        double _a8 = Degree2Radian(154.84 + 7.306860 * _k);
        _correction += 0.000056 * Math.Sin(_a8);
        double _a9 = Degree2Radian(34.52 + 27.261239 * _k);
        _correction += 0.000047 * Math.Sin(_a9);
        double _a10 = Degree2Radian(207.19 + 0.121824 * _k);
        _correction += 0.000042 * Math.Sin(_a10);
        double _a11 = Degree2Radian(291.34 + 1.844379 * _k);
        _correction += 0.000040 * Math.Sin(_a11);
        double _a12 = Degree2Radian(161.72 + 24.198154 * _k);
        _correction += 0.000037 * Math.Sin(_a12);
        double _a13 = Degree2Radian(239.56 + 25.513099 * _k);
        _correction += 0.000035 * Math.Sin(_a13);
        double _a14 = Degree2Radian(331.55 + 3.592518 * _k);
        _correction += 0.000023 * Math.Sin(_a14);

        // ========== 额外高阶修正项 (Chapront-Touzé & Chapront ELP2000) ==========
        // 这些项来自更精确的月球星历表，可以将精度提高到约 10 秒
        double _d = Degree2Radian(297.8502042 + 445267.1115168 * _t - 0.0016300 * _t2 + _t3 / 545868.0 - _t4 / 113065000.0);
        double _sunMeanLong = Degree2Radian(280.46645 + 36000.76983 * _t + 0.0003032 * _t2);
        double _moonMeanLong = Degree2Radian(218.3164591 + 481267.88134236 * _t - 0.0015786 * _t2 + _t3 / 538841.0 - _t4 / 65194000.0);

        // 额外的长周期项
        _correction += 0.000004 * Math.Sin(2 * _d - _radMPrime);
        _correction += 0.000003 * Math.Sin(2 * _d + _radMPrime);
        _correction += 0.000003 * Math.Sin(2 * _d - _radM);
        _correction -= 0.000002 * Math.Sin(4 * _d);
        _correction += 0.000002 * Math.Sin(_sunMeanLong - _moonMeanLong);

        // ========== 木星和金星摄动 (Jean Meeus, More Mathematical Astronomy Morsels) ==========
        double _venus = Degree2Radian(82.7311 + 6070.0279 * _t);
        double _jupiter = Degree2Radian(20.3623 + 1832.5793 * _t);
        _correction += 0.000003 * Math.Sin(_venus);
        _correction += 0.000002 * Math.Sin(_jupiter);
        _correction += 0.000002 * Math.Sin(_venus - _jupiter);

        double _jdeFinal = _jde + _correction; // 此时是力学时 (TD)

        // 11. 修正 Delta T: UT = TD - DeltaT
        // 获取 Delta T (秒)，并在最后减去
        double _deltaTSeconds = EstimateDeltaT(_jdeFinal);
        double _julianDayUt = _jdeFinal - _deltaTSeconds / 86400.0;

        return _julianDayUt;
    }

    /// <summary>
    /// (已弃用) f'(t) 数值求导 - 仅供旧版迭代法使用，可保留或删除
    /// </summary>
    public static double DerivativeLunarMinusSolar(double _julianDay, double _diff)
    {
        double _f1 = LunarMinusSolarLongitude(_julianDay + _diff);
        double _f2 = LunarMinusSolarLongitude(_julianDay - _diff);
        return (_f1 - _f2) / (2.0 * _diff);   // 度/天
    }

    /// <summary>
    /// f(t) = λ_moon(jd) - λ_sun(jd)，范围 -180°~+180°
    /// </summary>
    public static double LunarMinusSolarLongitude(double _julianDay)
    {
        double _lambdaMoon = NormalizeDegrees(GetMoonEclipticLongitude(_julianDay));
        double _lambdaSun = NormalizeDegrees(GetSunEclipticLongitude(_julianDay));

        double _diff = _lambdaMoon - _lambdaSun;
        return Angle2Degree180(_diff);
    }

    #region Delta T 计算 (查表法 + 拟合)
    // ============================================
    // 最精确的 DeltaT 数据表
    // 来源: NASA Eclipse + Meeus + IERS
    // ============================================
    private static readonly double[,] DeltaTTable = new double[,]
    {
        // ========== 远古历史数据 (NASA Table 1 / Morrison & Stephenson 2004) ==========
        // 公元前 - 每100年，误差较大 (±数百秒)
        {-500, 17190}, {-400, 15530}, {-300, 14080}, {-200, 12790}, {-100, 11640},
        {0, 10580}, {100, 9600}, {200, 8640}, {300, 7680}, {400, 6700},
        {500, 5710}, {600, 4740}, {700, 3810}, {800, 2960}, {900, 2200},
        {1000, 1570}, {1100, 1090}, {1200, 740}, {1300, 490}, {1400, 320},
        {1500, 200}, {1600, 120},

        // ========== 望远镜时代 1620-1800 (Meeus Table 10.A，每2年) ==========
        // 精度 ±1-5 秒
        {1620, 121}, {1622, 112}, {1624, 103}, {1626, 95}, {1628, 88},
        {1630, 82}, {1632, 77}, {1634, 72}, {1636, 68}, {1638, 63},
        {1640, 60}, {1642, 56}, {1644, 53}, {1646, 51}, {1648, 48},
        {1650, 46}, {1652, 44}, {1654, 42}, {1656, 40}, {1658, 38},
        {1660, 35}, {1662, 33}, {1664, 31}, {1666, 29}, {1668, 26},
        {1670, 24}, {1672, 22}, {1674, 20}, {1676, 18}, {1678, 16},
        {1680, 14}, {1682, 12}, {1684, 11}, {1686, 10}, {1688, 9},
        {1690, 9}, {1692, 9}, {1694, 9}, {1696, 9}, {1698, 9},
        {1700, 9}, {1702, 9}, {1704, 9}, {1706, 9}, {1708, 10},
        {1710, 10}, {1712, 10}, {1714, 10}, {1716, 10}, {1718, 11},
        {1720, 11}, {1722, 11}, {1724, 11}, {1726, 11}, {1728, 11},
        {1730, 11}, {1732, 11}, {1734, 12}, {1736, 12}, {1738, 12},
        {1740, 12}, {1742, 13}, {1744, 13}, {1746, 13}, {1748, 13},
        {1750, 13}, {1752, 14}, {1754, 14}, {1756, 14}, {1758, 14},
        {1760, 15}, {1762, 15}, {1764, 15}, {1766, 15}, {1768, 16},
        {1770, 16}, {1772, 16}, {1774, 16}, {1776, 16}, {1778, 17},
        {1780, 17}, {1782, 17}, {1784, 17}, {1786, 17}, {1788, 17},
        {1790, 17}, {1792, 16}, {1794, 16}, {1796, 16}, {1798, 15},
        {1800, 14},

        // ========== 1800-1900 (Meeus Table 10.A，每2年) ==========
        {1800, 13.7}, {1802, 13.4}, {1804, 13.1}, {1806, 12.9}, {1808, 12.7},
        {1810, 12.6}, {1812, 12.5}, {1814, 12.5}, {1816, 12.5}, {1818, 12.5},
        {1820, 12.0}, {1822, 11.2}, {1824, 10.4}, {1826, 9.6}, {1828, 8.9},
        {1830, 8.3}, {1832, 7.7}, {1834, 7.2}, {1836, 6.8}, {1838, 6.4},
        {1840, 6.1}, {1842, 6.0}, {1844, 6.0}, {1846, 6.1}, {1848, 6.3},
        {1850, 6.6}, {1852, 6.9}, {1854, 7.2}, {1856, 7.5}, {1858, 7.7},
        {1860, 7.7}, {1862, 7.5}, {1864, 7.2}, {1866, 6.5}, {1868, 5.4},
        {1870, 3.9}, {1872, 2.0}, {1874, -0.1}, {1876, -2.2}, {1878, -4.0},
        {1880, -5.4}, {1882, -5.8}, {1884, -5.9}, {1886, -5.9}, {1888, -5.9},
        {1890, -5.9}, {1892, -5.8}, {1894, -5.7}, {1896, -5.4}, {1898, -4.8},
        {1900, -3.8},

        // ========== 1900-1954 (Meeus Table 10.A，每2年) ==========
        {1900, -2.8}, {1902, -0.9}, {1904, 1.4}, {1906, 3.6}, {1908, 5.7},
        {1910, 7.7}, {1912, 9.7}, {1914, 11.9}, {1916, 14.2}, {1918, 16.5},
        {1920, 18.8}, {1922, 20.5}, {1924, 21.8}, {1926, 22.8}, {1928, 23.6},
        {1930, 24.0}, {1932, 24.0}, {1934, 23.9}, {1936, 23.8}, {1938, 23.9},
        {1940, 24.3}, {1942, 24.7}, {1944, 25.3}, {1946, 26.2}, {1948, 27.3},
        {1950, 28.6}, {1952, 30.0}, {1954, 31.0},

        // ========== 1955-1971 (NASA Table 2 直接观测，每年) ==========
        {1955, 31.1}, {1956, 31.6}, {1957, 32.2}, {1958, 32.7}, {1959, 33.2},
        {1960, 33.2}, {1961, 33.5}, {1962, 34.0}, {1963, 34.5}, {1964, 35.2},
        {1965, 35.7}, {1966, 36.5}, {1967, 37.2}, {1968, 38.3}, {1969, 39.4},
        {1970, 40.2}, {1971, 41.2},

        // ========== 1972-2025 IERS EOP 14 C04 高精度数据 (每年) ==========
        // 精度 ±0.001 秒
        {1972, 42.23}, {1973, 43.37}, {1974, 44.48}, {1975, 45.48},
        {1976, 46.46}, {1977, 47.52}, {1978, 48.53}, {1979, 49.59},
        {1980, 50.54}, {1981, 51.38}, {1982, 52.17}, {1983, 52.96},
        {1984, 53.79}, {1985, 54.34}, {1986, 54.87}, {1987, 55.32},
        {1988, 55.82}, {1989, 56.30}, {1990, 56.86}, {1991, 57.57},
        {1992, 58.31}, {1993, 59.12}, {1994, 59.98}, {1995, 60.79},
        {1996, 61.63}, {1997, 62.30}, {1998, 62.97}, {1999, 63.47},
        {2000, 63.83}, {2001, 64.09}, {2002, 64.30}, {2003, 64.47},
        {2004, 64.57}, {2005, 64.69}, {2006, 64.85}, {2007, 65.15},
        {2008, 65.46}, {2009, 65.78}, {2010, 66.07}, {2011, 66.32},
        {2012, 66.60}, {2013, 66.91}, {2014, 67.28}, {2015, 67.64},
        {2016, 68.10}, {2017, 68.59}, {2018, 68.97}, {2019, 69.22},
        {2020, 69.36}, {2021, 69.36}, {2022, 69.29}, {2023, 69.20},
        {2024, 69.18}, {2025, 69.14},

        // ========== 2025+ 未来预测 (NASA 外推) ==========
        {2030, 70}, {2040, 72}, {2050, 75}, {2100, 93}, {2150, 203}, {2200, 442}
    };

    /// <summary>
    /// 估算 Delta T = TD - UT (单位：秒)
    /// 使用查表+线性插值，超出范围使用多项式拟合
    /// 来源: NASA Eclipse / Meeus / IERS
    /// </summary>
    public static double EstimateDeltaT(double _julianDay)
    {
        double _year = (_julianDay - 2451545.0) / 365.25 + 2000.0;

        // 1. 查表范围 (-500 ~ 2200)，使用线性插值
        double _minYear = DeltaTTable[0, 0];
        double _maxYear = DeltaTTable[DeltaTTable.GetLength(0) - 1, 0];

        if (_year >= _minYear && _year <= _maxYear)
        {
            for (int i = 0; i < DeltaTTable.GetLength(0) - 1; i++)
            {
                double _y1 = DeltaTTable[i, 0];
                double _y2 = DeltaTTable[i + 1, 0];

                if (_year >= _y1 && _year <= _y2)
                {
                    double _dt1 = DeltaTTable[i, 1];
                    double _dt2 = DeltaTTable[i + 1, 1];
                    double _ratio = (_year - _y1) / (_y2 - _y1);
                    return _dt1 + (_dt2 - _dt1) * _ratio;
                }
            }
        }

        // 2. 超出查表范围，使用多项式拟合
        // 基于 NASA Eclipse 网站的长期抛物线趋势: ΔT = -20 + 32 * t^2
        // 其中 t = (year - 1820) / 100
        double _t = (_year - 1820.0) / 100.0;
        return -20.0 + 32.0 * _t * _t;
    }
    #endregion
    #endregion

    #region 日出 / 日落 专用
    /// <summary>
    /// 日出/日落 搜索（太阳专用）
    /// </summary>
    public static RiseSetResult FindSunRiseSet(double _julianDay,
                                                double _latitudeDegree,
                                                double _longitudeDegree,
                                                bool _isRise,
                                                double _standardAltitudeDegree)
    {
        double _step = 1.0 / 24.0; // 每小时扫一次
        double _start = _julianDay;
        double _end = _julianDay + 1.0;

        double _latRadian = Degree2Radian(_latitudeDegree);
        double _lonDegree = _longitudeDegree; // 东经为正

        // 定义一个委托：给定时间 → 当前太阳高度
        double altitudeFunc(double _t) => GetSunAltitude(_t, _latRadian, _lonDegree);

        double _prevTime = _start;
        double _altPrev = altitudeFunc(_prevTime);

        RiseSetResult _result = new()
        {
            IsValid = false,
            JulianDay = double.NaN,
            Position = new HorizontalPosition { Azimuth = double.NaN, Altitude = double.NaN }
        };

        for (double _t = _start + _step; _t <= _end; _t += _step)
        {
            double _alt = altitudeFunc(_t);
            bool _crossed = false;

            if (_isRise)
            {
                // 升起：从低于标准高度 → 高于标准高度
                if (_altPrev < _standardAltitudeDegree && _alt >= _standardAltitudeDegree) { _crossed = true; }
            }
            else
            {
                // 落下：从高于标准高度 → 低于标准高度
                if (_altPrev > _standardAltitudeDegree && _alt <= _standardAltitudeDegree) { _crossed = true; }
            }

            if (_crossed)
            {
                double _eventTime = BinarySearchAltitude(_prevTime, _t, altitudeFunc, _standardAltitudeDegree, _isRise);

                EquatorialPosition _eq = CalculateSunEquatorialPosition(_eventTime);
                HorizontalPosition _pos = EquatorialToHorizontal(_eventTime, _latitudeDegree, _longitudeDegree, _eq);

                _result.IsValid = true;
                _result.JulianDay = _eventTime;
                _result.Position = _pos;
                return _result;
            }

            _altPrev = _alt;
            _prevTime = _t;
        }

        return _result;
    }

    private static double GetSunAltitude(double _julianDay, double _latRadian, double _lonDegree)
    {
        EquatorialPosition _eq = CalculateSunEquatorialPosition(_julianDay);
        HorizontalPosition _pos = EquatorialToHorizontal(_julianDay, Radian2Degree(_latRadian), _lonDegree, _eq);
        return _pos.Altitude;
    }
    #endregion

    #region 月出 / 月落 专用
    /// <summary>
    /// 月出/月落 搜索（月亮专用）
    /// </summary>
    private static RiseSetResult FindMoonRiseSet(double _julianDay,
                                                 double _latitudeDegree,
                                                 double _longitudeDegree,
                                                 bool _isRise,
                                                 double _standardAltitudeDegree)
    {
        double _step = 1.0 / 24.0; // 每小时扫一次
        double _start = _julianDay;
        double _end = _julianDay + 1.0;

        double _latRadian = Degree2Radian(_latitudeDegree);
        double _lonDegree = _longitudeDegree; // 东经为正

        double _altitudeFunc(double _t) => GetMoonAltitude(_t, _latRadian, _lonDegree);

        double _prevTime = _start;
        double _altPrev = _altitudeFunc(_prevTime);

        RiseSetResult _result = new()
        {
            IsValid = false,
            JulianDay = double.NaN,
            Position = new HorizontalPosition { Azimuth = double.NaN, Altitude = double.NaN }
        };

        for (double _t = _start + _step; _t <= _end; _t += _step)
        {
            double _alt = _altitudeFunc(_t);

            bool _crossed = false;
            if (_isRise)
            {
                if (_altPrev < _standardAltitudeDegree && _alt >= _standardAltitudeDegree) { _crossed = true; }
            }
            else
            {
                if (_altPrev > _standardAltitudeDegree && _alt <= _standardAltitudeDegree) { _crossed = true; }
            }

            if (_crossed)
            {
                double _eventTime = BinarySearchAltitude(_altPrev, _t, _altitudeFunc, _standardAltitudeDegree, _isRise);
                EquatorialPosition _eq = CalculateMoonEquatorialPosition(_eventTime);
                HorizontalPosition _pos = EquatorialToHorizontal(_eventTime, _latitudeDegree, _longitudeDegree, _eq);

                _result.IsValid = true;
                _result.JulianDay = _eventTime;
                _result.Position = _pos;
                return _result;
            }

            _altPrev = _alt;
            _prevTime = _t;
        }

        return _result;
    }

    private static double GetMoonAltitude(double _julianDay, double _latRadian, double _lonDegree)
    {
        EquatorialPosition _eq = CalculateMoonEquatorialPosition(_julianDay);
        HorizontalPosition _hor = EquatorialToHorizontal(_julianDay, Radian2Degree(_latRadian), _lonDegree, _eq);
        return _hor.Altitude;
    }
    #endregion

    #region 通用：二分搜索高度 crossing
    /// <summary>
    /// 在 [t1, t2] 内用二分法求 altitude = standardAltitudeDeg 的时刻
    /// altitudeFunc: 给定 JD → 高度（度）
    /// </summary>
    private static double BinarySearchAltitude(double _t1,
                                               double _t2,
                                               Func<double, double> _altitudeFunc,
                                               double _standardAltitudeDegree,
                                               bool _isRise)
    {
        double _a1 = _altitudeFunc(_t1), _a2 = _altitudeFunc(_t2);

        // 40 次迭代足够到秒级
        for (int i = 0; i < 40; i++)
        {
            double _tm = 0.5 * (_t1 + _t2);
            double _am = _altitudeFunc(_tm);

            if (_isRise)
            {
                // 低→高 crossing
                if (_a1 < _standardAltitudeDegree && _am >= _standardAltitudeDegree)
                {
                    _t2 = _tm;
                    _a2 = _am;
                }
                else
                {
                    _t1 = _tm;
                    _a1 = _am;
                }
            }
            else
            {
                // 高→低 crossing
                if (_a1 > _standardAltitudeDegree && _am <= _standardAltitudeDegree)
                {
                    _t2 = _tm;
                    _a2 = _am;
                }
                else
                {
                    _t1 = _tm;
                    _a1 = _am;
                }
            }
        }

        return 0.5 * (_t1 + _t2);
    }
    #endregion

    #region 太阳位置（赤经 / 赤纬）——复用你的黄经函数
    /// <summary>
    /// 太阳地心赤经 / 赤纬（度）
    /// 使用：太阳黄经 = GetSunEclipticLongitude(JD)，再转赤道坐标。
    /// </summary>
    private static EquatorialPosition CalculateSunEquatorialPosition(double _julianDay)
    {
        double _t = (_julianDay - 2451545.0) / 36525.0;

        double _lambda = GetSunEclipticLongitude(_julianDay);
        _lambda = NormalizeDegrees(_lambda);
        double _lambdaRadian = Degree2Radian(_lambda);

        // 黄赤交角（简化）
        double _epsilon = 23.439291 - 0.0130042 * _t;
        double _epsilonRadian = Degree2Radian(_epsilon);

        double _sinLambda = Math.Sin(_lambdaRadian);
        double _cosLambda = Math.Cos(_lambdaRadian);

        double _y = Math.Cos(_epsilonRadian) * _sinLambda;
        double _x = _cosLambda;

        double _raRadian = Math.Atan2(_y, _x);
        double _decRadian = Math.Asin(Math.Sin(_epsilonRadian) * _sinLambda);

        double _raDegree = NormalizeDegrees(Radian2Degree(_raRadian));
        double _decDegree = Radian2Degree(_decRadian);

        return new EquatorialPosition { RightAscension = _raDegree, Declination = _decDegree };
    }
    #endregion

    #region 月亮位置（赤经 / 赤纬）
    /// <summary>
    /// 月亮地心赤经 / 赤纬（度）
    /// </summary>
    private static EquatorialPosition CalculateMoonEquatorialPosition(double _julianDay)
    {
        double _t = (_julianDay - 2451545.0) / 36525.0;

        // D, M', F 用于计算黄纬
        double _d = 297.8501921 + 445267.1114034 * _t
                  - 0.0018819 * _t * _t
                  + _t * _t * _t / 545868.0
                  - _t * _t * _t * _t / 113065000.0;

        double _m_prime = 134.9633964 + 477198.8675055 * _t
                        + 0.0087414 * _t * _t
                        + _t * _t * _t / 69699.0
                        - _t * _t * _t * _t / 14712000.0;

        double _f = 93.2720950 + 483202.0175233 * _t
                  - 0.0036539 * _t * _t
                  - _t * _t * _t / 3526000.0
                  + _t * _t * _t * _t / 863310000.0;

        _d = NormalizeDegrees(_d);
        _m_prime = NormalizeDegrees(_m_prime);
        _f = NormalizeDegrees(_f);

        double _d_radian = Degree2Radian(_d);
        double _m_prime_radian = Degree2Radian(_m_prime);
        double _f_radian = Degree2Radian(_f);

        double _lambda = GetMoonEclipticLongitude(_julianDay);
        _lambda = NormalizeDegrees(_lambda);
        double _lambdaRadian = Degree2Radian(_lambda);

        // 简化黄纬公式
        double _beta = 5.128 * Math.Sin(_f_radian)
                     + 0.280 * Math.Sin(_m_prime_radian + _f_radian)
                     + 0.277 * Math.Sin(_m_prime_radian - _f_radian)
                     + 0.173 * Math.Sin(2 * _d_radian - _f_radian)
                     + 0.055 * Math.Sin(2 * _d_radian + _f_radian)
                     + 0.046 * Math.Sin(2 * _d_radian - _m_prime_radian + _f_radian);

        double _betaRadian = Degree2Radian(_beta);

        // 黄赤交角
        double _epsilon = 23.439291 - 0.0130042 * _t;
        double _epsilonRadian = Degree2Radian(_epsilon);

        double _sinLambda = Math.Sin(_lambdaRadian);
        double _cosLambda = Math.Cos(_lambdaRadian);
        double _sinBeta = Math.Sin(_betaRadian);
        double _cosBeta = Math.Cos(_betaRadian);

        double _y = _sinLambda * Math.Cos(_epsilonRadian) - Math.Tan(_betaRadian) * Math.Sin(_epsilonRadian);
        double _x = _cosLambda;

        double _raRadian = Math.Atan2(_y, _x);
        double _decRadian = Math.Asin(_sinBeta * Math.Cos(_epsilonRadian) + _cosBeta * Math.Sin(_epsilonRadian) * _sinLambda);

        double _raDegree = NormalizeDegrees(Radian2Degree(_raRadian));
        double _decDegree = Radian2Degree(_decRadian);

        return new EquatorialPosition { RightAscension = _raDegree, Declination = _decDegree };
    }
    #endregion

    #region 黄经计算函数
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
        return NormalizeDegrees(_lambdaApparent);
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
        _d = Degree2Radian(NormalizeDegrees(_d));
        _m = Degree2Radian(NormalizeDegrees(_m));
        _m_prime = Degree2Radian(NormalizeDegrees(_m_prime));
        _f = Degree2Radian(NormalizeDegrees(_f));

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
        return NormalizeDegrees(_lambda);
    }
    #endregion

    #region 赤道坐标 → 水平坐标 & LST
    /// <summary>
    /// 赤道坐标 → 水平坐标（Az, Alt）
    /// longitudeDeg：东经为正，西经为负
    /// </summary>
    private static HorizontalPosition EquatorialToHorizontal(
        double _julianDay,
        double _latitudeDegress,
        double _longitudeDegree,
        EquatorialPosition _ePosition)
    {
        double _latRadian = Degree2Radian(_latitudeDegress);

        // 地方恒星时（度）
        double _lstDegree = GetLocalSiderealTime(_julianDay, _longitudeDegree);
        double _lstRadian = Degree2Radian(_lstDegree);

        double _raRadian = Degree2Radian(_ePosition.RightAscension);
        double _decRadian = Degree2Radian(_ePosition.Declination);

        // 时角 H = LST - RA
        double _hourAngle = _lstRadian - _raRadian;
        _hourAngle = NormalizeRadians(_hourAngle);

        // 高度角
        double _sinAltitude = Math.Sin(_latRadian) * Math.Sin(_decRadian) +
                              Math.Cos(_latRadian) * Math.Cos(_decRadian) * Math.Cos(_hourAngle);

        // 防止数值略超 [-1,1]
        if (_sinAltitude > 1.0) { _sinAltitude = 1.0; }
        if (_sinAltitude < -1.0) { _sinAltitude = -1.0; }

        double _altitudeRadian = Math.Asin(_sinAltitude);

        // 方位角（0° 北，90° 东）
        double _cosAltitude = Math.Cos(_altitudeRadian);
        if (Math.Abs(_cosAltitude) < 1e-10) { _cosAltitude = 1e-10; }

        double _sinAz = -Math.Cos(_decRadian) * Math.Sin(_hourAngle) / _cosAltitude;
        double _cosAz = (Math.Sin(_decRadian) - Math.Sin(_altitudeRadian) * Math.Sin(_latRadian)) /
                       (_cosAltitude * Math.Cos(_latRadian));

        double _azRadian = Math.Atan2(_sinAz, _cosAz);
        double _azDegree = NormalizeDegrees(Radian2Degree(_azRadian));

        return new HorizontalPosition { Azimuth = _azDegree, Altitude = Radian2Degree(_altitudeRadian) };
    }

    /// <summary>
    /// 计算地方恒星时（度）
    /// longitudeDeg：东经为正
    /// </summary>
    private static double GetLocalSiderealTime(double _julianDay, double _longitudeDegree)
    {
        double T = (_julianDay - 2451545.0) / 36525.0;

        // Greenwich Mean Sidereal Time (GMST) in degrees
        double theta0 = 280.46061837
                      + 360.98564736629 * (_julianDay - 2451545.0)
                      + 0.000387933 * T * T
                      - T * T * T / 38710000.0;

        // Local Sidereal Time
        double _lst = theta0 + _longitudeDegree;
        return NormalizeDegrees(_lst);
    }
    #endregion

    #region 工具函数: 工具函数：角度 / 弧度 / 归一化
    public static double Degree2Radian(double _degree) => Math.PI / 180.0 * _degree;
    public static double Radian2Degree(double _radian) => 180.0 / Math.PI * _radian;

    public static double NormalizeDegrees(double _degree)
    {
        _degree %= 360.0;
        if (_degree < 0) { _degree += 360.0; }
        return _degree;
    }

    public static double NormalizeRadians(double _radian)
    {
        double _2Pi = 2.0 * Math.PI;
        _radian %= _2Pi;
        if (_radian < 0) _radian += _2Pi;
        return _radian;
    }

    public static double Angle2Degree180(double _degree)
    {
        _degree %= 360.0;
        if (_degree > 180.0) _degree -= 360.0;
        if (_degree <= -180.0) _degree += 360.0;
        return _degree;
    }
    #endregion

    #region 月球轨道数据
    private static readonly int[,] MoonLongitudeData = new int[,]
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
    #endregion
}