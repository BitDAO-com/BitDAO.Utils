using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace BitDAO.Utils.Daoism;

public class WuXing
{
    public BaZi BaZi;
    public Dictionary<string, decimal> Scores;
    public IList<string[]> ScoreLog = [];
    public WuXing(BaZi _bazi) { this.BaZi = _bazi; this.Scores = this.GetScores(); }

    public Dictionary<string, decimal> GetNumbers(bool _includeCangGan = false)
    {
        Dictionary<string, decimal> _result = new();
        for (int i = 0; i < DaoismUtils.WuXing.Length; i++) { _result.Add(DaoismUtils.WuXing[i], 0); }
        _result[this.BaZi.NianWuXing[..1]] += 1;
        _result[this.BaZi.YueWuXing[..1]] += 1;
        _result[this.BaZi.RiWuXing[..1]] += 1;
        _result[this.BaZi.ShiWuXing[..1]] += 1;

        if (!_includeCangGan)
        {
            _result[this.BaZi.NianWuXing[1..2]] += 1;
            _result[this.BaZi.YueWuXing[1..2]] += 1;
            _result[this.BaZi.RiWuXing[1..2]] += 1;
            _result[this.BaZi.ShiWuXing[1..2]] += 1;
        }
        else
        {
            _result = GetNumbersAtCangGan(this.BaZi.NianCangGanWuXing, _result);
            _result = GetNumbersAtCangGan(this.BaZi.YueCangGanWuXing, _result);
            _result = GetNumbersAtCangGan(this.BaZi.RiCangGanWuXing, _result);
            _result = GetNumbersAtCangGan(this.BaZi.ShiCangGanWuXing, _result);
        }

        return _result;
    }

    public static Dictionary<string, decimal> GetNumbersAtCangGan(string[] _wuxing, Dictionary<string, decimal> _result)
    {
        if (_wuxing.Length == 1)
        {
            _result[_wuxing[0]] += 1M;
        }
        else if (_wuxing.Length == 2)
        {
            _result[_wuxing[0]] += 0.7M;
            _result[_wuxing[1]] += 0.3M;
        }
        else if (_wuxing.Length == 3)
        {
            _result[_wuxing[0]] += 0.6M;
            _result[_wuxing[1]] += 0.3M;
            _result[_wuxing[2]] += 0.1M;
        }

        return _result;
    }

    #region GetScores
    public Dictionary<string, decimal> GetScores()
    {
        Dictionary<string, decimal> _ganScores = [];
        foreach (string _wuxing in DaoismUtils.WuXing) { _ganScores.Add(_wuxing, 0); }

        (string _nianGanWuXing, decimal _nianGanScore) = GetGanScore(this.BaZi.NianGan);
        _ganScores[_nianGanWuXing] += _nianGanScore;

        (string _yueGanWuXing, decimal _yueGanScore) = GetGanScore(this.BaZi.YueGan);
        _ganScores[_yueGanWuXing] += _yueGanScore;

        (string _riGanWuXing, decimal _riGanScore) = GetGanScore(this.BaZi.RiGan);
        _ganScores[_riGanWuXing] += _riGanScore;

        (string _shiGanWuXing, decimal _shiGanScore) = GetGanScore(this.BaZi.ShiGan);
        _ganScores[_shiGanWuXing] += _shiGanScore;

        Dictionary<string, decimal> _zhiScores = [];
        foreach (string _wuxing in DaoismUtils.WuXing) { _zhiScores.Add(_wuxing, 0); }

        Dictionary<string, decimal> _nianZhiScores = GetZhiScore(this.BaZi.NianZhi);
        foreach (string _wuxing in DaoismUtils.WuXing) { _zhiScores[_wuxing] += _nianZhiScores[_wuxing]; }

        Dictionary<string, decimal> _yueZhiScores = GetZhiScore(this.BaZi.YueZhi);
        foreach (string _wuxing in DaoismUtils.WuXing) { _zhiScores[_wuxing] += _yueZhiScores[_wuxing]; }

        Dictionary<string, decimal> _riZhiScores = GetZhiScore(this.BaZi.RiZhi);
        foreach (string _wuxing in DaoismUtils.WuXing) { _zhiScores[_wuxing] += _riZhiScores[_wuxing]; }

        Dictionary<string, decimal> _shiZhiScores = GetZhiScore(this.BaZi.ShiZhi);
        foreach (string _wuxing in DaoismUtils.WuXing) { _zhiScores[_wuxing] += _shiZhiScores[_wuxing]; }

        string[] _zhiStatus = WuXingZhiStatus[this.BaZi.YueZhi];
        for (int i = 0; i < _zhiStatus.Length; i++)
        {
            decimal _rate = YueLingRate[i];
            string _wuxing = _zhiStatus[i];
            decimal _score = _zhiScores[_wuxing] * _rate;
            this.ScoreLog.Add(["支", this.BaZi.YueZhi, _wuxing, "月令", $"{_zhiScores[_wuxing]} x {_rate} = {_score:0.0000}"]);
            _zhiScores[_wuxing] += _score;
        }

        foreach (string _wuxing in DaoismUtils.WuXing) { _ganScores[_wuxing] += _zhiScores[_wuxing]; }

        return _ganScores;
    }
    #endregion

    #region GetZhiScore
    private Dictionary<string, decimal> GetZhiScore(string _zhi)
    {
        Dictionary<string, decimal> _zhiScores = [];
        foreach (string _wuxing in DaoismUtils.WuXing) { _zhiScores.Add(_wuxing, 0); }

        int[] _zhiQi = WuXingZhiQi[_zhi];
        int _qiCount = _zhiQi[0];
        decimal[] _qiRate = ZhiQiRate[_qiCount];

        for (int i = 0; i < _qiCount; i++)
        {
            string _wuxing = DaoismUtils.WuXing[_zhiQi[i + 1]];
            decimal _score = _qiRate[i] * ZhiBase;
            string _type = i switch { 0 => "本气", 1 => "中气", 2 => "余气", _ => "" };
            this.ScoreLog.Add(["支", _zhi, _wuxing, _type, $"{ZhiBase} x {_qiRate[i]} = {_score:0.0000}"]);
            _zhiScores[_wuxing] += _score;
        }

        return _zhiScores;
    }
    #endregion

    #region GetGanScore
    public (string, decimal) GetGanScore(string _gan)
    {
        string _wuxing = DaoismUtils.GanWuXing[_gan];
        decimal _score = GanBase;
        this.ScoreLog.Add(["干", _gan, _wuxing, "基础", $"{_score:0.0000}"]);

        int[] _relation = WuXingGanGen[_gan];
        decimal _rate1 = GanGenRate[_relation[this.BaZi.NianZhiIndex]];

        decimal _rate2 = GanGenRate[_relation[this.BaZi.YueZhiIndex]];
        _rate1 = _rate1 > _rate2 ? _rate1 : _rate2;

        _rate2 = GanGenRate[_relation[this.BaZi.RiZhiIndex]];
        _rate1 = _rate1 > _rate2 ? _rate1 : _rate2;

        _rate2 = GanGenRate[_relation[this.BaZi.ShiZhiIndex]];
        _rate1 = _rate1 > _rate2 ? _rate1 : _rate2;

        decimal _genPlus = GanBase * _rate1;
        this.ScoreLog.Add(["干", _gan, _wuxing, "根气", $"{GanBase} x {_rate1:0.0000} = {_genPlus:0.0000}"]);
        _score += _genPlus;

        return (_wuxing, _score);
    }
    #endregion

    #region ToString
    public override string ToString()
    {
        string _text = $"";
        decimal _tong = 0M;
        foreach (string _wuxing in WuXingTong[DaoismUtils.GanWuXing[this.BaZi.RiGan]])
        {
            _text += $"{_wuxing}: {this.Scores[_wuxing]}  ";
            _tong += this.Scores[_wuxing];
        }
        _text += $"同类得分: {_tong}\n";

        decimal _yi = 0M;
        foreach (string _wuxing in WuXingYi[DaoismUtils.GanWuXing[this.BaZi.RiGan]])
        {
            _text += $"{_wuxing}: {this.Scores[_wuxing]}  ";
            _yi += this.Scores[_wuxing];
        }
        _text += $"异类得分: {_yi}\n";
        _text += $"相差: {_tong - _yi}  比率: {(_tong - _yi) / _tong:0.0000}\n";
        return _text;
    }
    #endregion

    #region ScoresLog
    public string ScoresLog
    {
        get
        {
            string _text = $"{this.BaZi}\n";
            foreach (string[] _log in this.ScoreLog)
            {
                _text += $"{_log[0]} {_log[1]} {_log[2]} {_log[3]} {_log[4]}\n";
            }
            return _text;
        }
    }
    #endregion

    public static readonly decimal GanBase = 5M;
    public static readonly decimal ZhiBase = 15M;

    // 天干对应根气的权重（0=无根，1=主气根（同阴阳），2=主气根（异阴阳），3=中气根（同阴阳），4=中气根（异阴阳），5=余气根（同阴阳），6=余气根（异阴阳））
    //public static readonly decimal[] GanGenRate = [0, 0M, 0M, 0M, 0M, 0M, 0.0M];
    //public static readonly decimal[] GanGenRate = [0, 0.5M, 0.25M, 0.25M, 0.125M, 0.125M, 0.0625M];
    public static readonly decimal[] GanGenRate = [0, 0.8M, 0.4M, 0.4M, 0.2M, 0.2M, 0.1M];
    public static readonly decimal[] YueLingRate = [0.5M, 0.25M, 0.0M, -0.25M, -0.5M];



    #region ZhiQiRate
    public static readonly Dictionary<int, decimal[]> ZhiQiRate = new()
    {
        {1,[1M]},
        {2,[0.7M,0.3M]},
        {3,[0.6M,0.3M,0.1M]}
    };
    #endregion


    #region WuXingZhiStatus
    public static readonly Dictionary<string, string[]> WuXingZhiStatus = new()
    {
        // 春季
        { "寅", new[] { "木", "火", "水", "金", "土" } },  // 木旺 火相 水休 金囚 土死
        { "卯", new[] { "木", "火", "水", "金", "土" } },  // 同上
        // 夏季
        { "巳", new[] { "火", "土", "木", "水", "金" } },  // 火旺 土相 木休 水囚 金死
        { "午", new[] { "火", "土", "木", "水", "金" } },  // 同上
        // 秋季
        { "申", new[] { "金", "水", "土", "火", "木" } },  // 金旺 水相 土休 火囚 木死
        { "酉", new[] { "金", "水", "土", "火", "木" } },  // 同上
        // 冬季
        { "亥", new[] { "水", "木", "金", "土", "火" } },  // 水旺 木相 金休 土囚 火死
        { "子", new[] { "水", "木", "金", "土", "火" } },  // 同上
        // 四季末（土旺）
        { "辰", new[] { "土", "金", "火", "木", "水" } },  // 土旺 金相 火休 木囚 水死
        { "未", new[] { "土", "金", "火", "木", "水" } },  // 同上
        { "戌", new[] { "土", "金", "火", "木", "水" } },  // 同上
        { "丑", new[] { "土", "金", "火", "木", "水" } }   // 同上
    };
    #endregion

    #region WuXingZhiQi
    public static readonly Dictionary<string, int[]> WuXingZhiQi = new()
    {
        // 地支 | 木火土金水
        { "子", new int[] { 1, 4 } },           // 本气：癸水
        { "丑", new int[] { 3, 2, 4, 3} },      // 本气：己土，中气：癸水，余气：辛金
        { "寅", new int[] { 3, 0, 1, 2} },      // 本气：甲木，中气：丙火，余气：戊土
        { "卯", new int[] { 1, 0 } },           // 本气：乙木
        { "辰", new int[] { 3, 2, 0, 4 } },     // 本气：戊土，中气：乙木，余气：癸水
        { "巳", new int[] { 3, 1, 3, 2 } },     // 本气：丙火，中气：庚金，余气：戊土
        { "午", new int[] { 2, 1, 2 } },        // 本气：丁火，中气：己土
        { "未", new int[] { 3, 2, 1, 0 } },     // 本气：己土，中气：丁火，余气：乙木
        { "申", new int[] { 3, 3, 4, 2 } },     // 本气：庚金，中气：壬水，余气：戊土
        { "酉", new int[] { 1, 3 } },           // 本气：辛金
        { "戌", new int[] { 3, 2, 3, 1 } },     // 本气：戊土，中气：辛金，余气：丁火
        { "亥", new int[] { 2, 4, 0 } }         // 本气：壬水，中气：甲木
    };
    #endregion

    #region GanWuXingGenMap
    public static readonly Dictionary<string, int[]> WuXingGanGen = new()
    {
        // 数值含义：0=无根，1=主气根（同阴阳），2=主气根（异阴阳）
        //         3=中气根（同阴阳），4=中气根（异阴阳）
        //         5=余气根（同阴阳），6=余气根（异阴阳）

        ["甲"] = [
            0, // 子（无木）
            0, // 丑（无木）
            1, // 寅（主气甲木，同阳）
            2, // 卯（主气乙木，异阴）
            6, // 辰（余气乙木，异阴）
            0, // 巳（无木）
            0, // 午（无木）
            6, // 未（余气乙木，异阴）
            0, // 申（无木）
            0, // 酉（无木）
            0, // 戌（无木）
            3  // 亥（中气甲木，同阳）
        ],
        ["乙"] = [
            0, // 子（无木）
            0, // 丑（无木）
            2, // 寅（主气甲木，异阳）
            1, // 卯（主气乙木，同阴）
            3, // 辰（中气乙木，同阴）
            0, // 巳（无木）
            0, // 午（无木）
            5, // 未（余气乙木，同阴）
            0, // 申（无木）
            0, // 酉（无木）
            0, // 戌（无木）
            4  // 亥（中气甲木，异阳）
        ],
        ["丙"] = [
            0, // 子（无火）
            0, // 丑（无火）
            0, // 寅（无火）
            0, // 卯（无火）
            0, // 辰（无火）
            1, // 巳（主气丙火，同阳）
            2, // 午（主气丁火，异阴）
            4, // 未（中气丁火，异阴）
            0, // 申（无火）
            0, // 酉（无火）
            6, // 戌（余气丁火，异阴）
            0  // 亥（无火）
        ],
        ["丁"] = [
            0, // 子（无火）
            0, // 丑（无火）
            0, // 寅（无火）
            0, // 卯（无火）
            0, // 辰（无火）
            2, // 巳（主气丙火，异阳）
            1, // 午（主气丁火，同阴）
            3, // 未（中气丁火，同阴）
            0, // 申（无火）
            0, // 酉（无火）
            5, // 戌（余气丁火，同阴）
            0  // 亥（无火）
        ],
        ["戊"] = [
            0, // 子（无土）
            5, // 丑（余气己土，异阴）
            6, // 寅（余气戊土，同阳）
            0, // 卯（无土）
            1, // 辰（主气戊土，同阳）
            6, // 巳（余气戊土，同阳）
            0, // 午（无土）
            0, // 未（无土）
            0, // 申（无土）
            0, // 酉（无土）
            1, // 戌（主气戊土，同阳）
            0  // 亥（无土）
        ],
        ["己"] = [
            0, // 子（无土）
            3, // 丑（主气己土，同阴）
            0, // 寅（无土）
            0, // 卯（无土）
            2, // 辰（主气戊土，异阳）
            0, // 巳（无土）
            0, // 午（无土）
            1, // 未（主气己土，同阴）
            0, // 申（无土）
            0, // 酉（无土）
            2, // 戌（主气戊土，异阳）
            0  // 亥（无土）
        ],
        ["庚"] = [
            0, // 子（无金）
            6, // 丑（余气辛金，异阴）
            0, // 寅（无金）
            0, // 卯（无金）
            0, // 辰（无金）
            4, // 巳（中气庚金，同阳）
            0, // 午（无金）
            0, // 未（无金）
            1, // 申（主气庚金，同阳）
            2, // 酉（主气辛金，异阴）
            5, // 戌（余气辛金，异阴）
            0  // 亥（无金）
        ],
        ["辛"] = [
            0, // 子（无金）
            5, // 丑（余气辛金，同阴）
            0, // 寅（无金）
            0, // 卯（无金）
            0, // 辰（无金）
            6, // 巳（中气庚金，异阳）
            0, // 午（无金）
            0, // 未（无金）
            2, // 申（主气庚金，异阳）
            1, // 酉（主气辛金，同阴）
            3, // 戌（中气辛金，同阴）
            0  // 亥（无金）
        ],
        ["壬"] = [
            2, // 子（主气癸水，异阴）
            4, // 丑（中气癸水，异阴）
            0, // 寅（无水）
            0, // 卯（无水）
            6, // 辰（余气癸水，异阴）
            0, // 巳（无水）
            0, // 午（无水）
            0, // 未（无水）
            3, // 申（中气壬水，同阳）
            0, // 酉（无水）
            0, // 戌（无水）
            1  // 亥（主气壬水，同阳）
        ],
        ["癸"] = [
            1, // 子（主气癸水，同阴）
            3, // 丑（中气癸水，同阴）
            0, // 寅（无水）
            0, // 卯（无水）
            5, // 辰（余气癸水，同阴）
            0, // 巳（无水）
            0, // 午（无水）
            0, // 未（无水）
            4, // 申（中气壬水，异阳）
            0, // 酉（无水）
            0, // 戌（无水）
            2  // 亥（主气壬水，异阳）
        ]
    };
    #endregion

    #region WuXingTong
    public static readonly Dictionary<string, string[]> WuXingTong = new()
    {
        {"木", ["木","水"]},
        {"火", ["火","木"]},
        {"土", ["土","火"]},
        {"金", ["金","土"]},
        {"水", ["水","金"]}
    };
    #endregion

    #region WuXingYi
    public static readonly Dictionary<string, string[]> WuXingYi = new()
    {
        {"木", ["金","火","土"]},
        {"火", ["水","土","金"]},
        {"土", ["木","金","水"]},
        {"金", ["火","水","木"]},
        {"水", ["土","木","火"]}
    };
    #endregion
}