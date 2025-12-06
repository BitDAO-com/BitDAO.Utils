using System;
using System.Collections.Generic;
using System.Linq;

namespace BitDAO.Utils.Daoism;

public class ShenSha
{
    public BaZi BaZi;
    public string[] Nian;
    public string[] Yue;
    public string[] Ri;
    public string[] Shi;

    public string ToText()
    {
        string _text = "";
        _text += $"年柱: {string.Join(", ", Nian)}\n";
        _text += $"月柱: {string.Join(", ", Yue)}\n";
        _text += $"日柱: {string.Join(", ", Ri)}\n";
        _text += $"时柱: {string.Join(", ", Shi)}\n";
        return _text;
    }

    #region GetLiuYun
    public string[] GetLiuYun(string _ganzhi) => GetLiuYun(_ganzhi[..1], _ganzhi[1..]);
    public string[] GetLiuYun(string _gan, string _zhi)
    {
        string _ganzhi = $"{_gan}{_zhi}";
        IList<string> _result = new List<string>();

        if (TianYiGuiRenDict[this.BaZi.NianGan].Contains(_zhi) || TianYiGuiRenDict[this.BaZi.RiGan].Contains(_zhi)) { _result.Add("天乙贵人"); }
        if (TaiJiGuiRenDict[this.BaZi.NianGan].Contains(_zhi) || TaiJiGuiRenDict[this.BaZi.RiGan].Contains(_zhi)) { _result.Add("太极贵人"); }
        if (WenChangGuiRenDict[this.BaZi.NianGan].Contains(_zhi) || WenChangGuiRenDict[this.BaZi.RiGan].Contains(_zhi)) { _result.Add("文昌贵人"); }
        if (TianDeGuiRenDict[this.BaZi.YueZhi].Contains(_gan) || TianDeGuiRenDict[this.BaZi.YueZhi].Contains(_zhi)) { _result.Add("天德贵人"); }
        if (YueDeGuiRenDict[this.BaZi.YueZhi].Contains(_gan)) { _result.Add("月德贵人"); }
        if (FuXingGuiRenDict[this.BaZi.NianGan].Contains(_zhi) || FuXingGuiRenDict[this.BaZi.RiGan].Contains(_zhi)) { _result.Add("福星贵人"); }
        if (DeXiuGuiRenDict[this.BaZi.YueZhi].Contains(_gan)) { _result.Add("德秀贵人"); }
        if (TianChuGuiRenDict[this.BaZi.NianGan].Contains(_zhi) || TianChuGuiRenDict[this.BaZi.RiGan].Contains(_zhi)) { _result.Add("天厨贵人"); }
        if (GuoYinGuiRenDict[this.BaZi.NianGan].Contains(_zhi) || GuoYinGuiRenDict[this.BaZi.RiGan].Contains(_zhi)) { _result.Add("国印贵人"); }
        if (TianDeHeDict[this.BaZi.YueZhi].Contains(_gan) || TianDeHeDict[this.BaZi.YueZhi].Contains(_zhi)) { _result.Add("天德合"); }
        if (YueDeHeDict[this.BaZi.YueZhi].Contains(_gan)) { _result.Add("月德合"); }
        if ((TianLuoDiWangDict.TryGetValue(this.BaZi.NianZhi, out string[] _tldw1) && _tldw1.Contains(_zhi)) || (TianLuoDiWangDict.TryGetValue(this.BaZi.RiZhi, out string[] _tldw2) && _tldw2.Contains(_zhi))) { _result.Add("天罗地网"); }
        if (TianLuoDict.TryGetValue(this.BaZi.NianNaYin[^1..], out string[] _tianluo) && _tianluo.Contains(_zhi)) { _result.Add("天罗"); }
        if (DiWangDict.TryGetValue(this.BaZi.NianNaYin[^1..], out string[] _diwang) && _diwang.Contains(_zhi)) { _result.Add("地网"); }
        if (GouJiaoShaDict[this.BaZi.NianZhi].Contains(_zhi)) { _result.Add("勾绞煞"); }
        if (HongYanShaDict[this.BaZi.RiGan].Contains(_zhi)) { _result.Add("红艳煞"); }
        if (HuaGaiDict[this.BaZi.NianZhi].Contains(_zhi) || HuaGaiDict[this.BaZi.RiZhi].Contains(_zhi)) { _result.Add("华盖"); }
        if (YiMaDict[this.BaZi.NianZhi].Contains(_zhi) || YiMaDict[this.BaZi.RiZhi].Contains(_zhi)) { _result.Add("驿马"); }
        if (LuShengDict[this.BaZi.RiGan].Contains(_zhi)) { _result.Add("禄神"); }
        if (JiangXingDict[this.BaZi.NianZhi].Contains(_zhi) || JiangXingDict[this.BaZi.RiZhi].Contains(_zhi)) { _result.Add("将星"); }
        if (XueTangDict[this.BaZi.NianNaYin[^1..]].Contains(_zhi)) { _result.Add(ZhengXueTangDict[this.BaZi.NianNaYin[^1..]].Contains(_ganzhi) ? "正学堂" : "学堂"); }
        if (CiGuanDict[this.BaZi.NianNaYin[^1..]].Contains(_zhi)) { _result.Add(ZhengCiGuanDict[this.BaZi.NianNaYin[^1..]].Contains(_ganzhi) ? "正词馆" : "词馆"); }
        if (TianYiDict[this.BaZi.YueZhi].Contains(_zhi)) { _result.Add("天医"); }
        if (JinYuDict[this.BaZi.NianGan].Contains(_zhi) || JinYuDict[this.BaZi.RiGan].Contains(_zhi)) { _result.Add("金舆"); }
        if (TaoHuaDict[this.BaZi.NianZhi].Contains(_zhi) || TaoHuaDict[this.BaZi.RiZhi].Contains(_zhi)) { _result.Add("桃花"); }
        if (HongLuanDict[this.BaZi.NianZhi].Contains(_zhi)) { _result.Add("红鸾"); }
        if (TianXiDict[this.BaZi.NianZhi].Contains(_zhi)) { _result.Add("天喜"); }
        if (SangMenDict[this.BaZi.NianZhi].Contains(_zhi)) { _result.Add("丧门"); }
        if (DiaoKeDict[this.BaZi.NianZhi].Contains(_zhi)) { _result.Add("吊客"); }
        if (PiMaDict[this.BaZi.NianZhi].Contains(_zhi)) { _result.Add("披麻"); }
        if (YuanChenDict[this.BaZi.NianZhi].Contains(_zhi)) { _result.Add("元辰"); }
        if (GuChenDict[this.BaZi.NianZhi].Contains(_zhi)) { _result.Add("孤辰"); }
        if (GuaSuDict[this.BaZi.NianZhi].Contains(_zhi)) { _result.Add("寡宿"); }
        if (LiuXiaDict[this.BaZi.RiGan].Contains(_zhi)) { _result.Add("流霞"); }
        if (WangShenDict[this.BaZi.NianZhi].Contains(_zhi) || WangShenDict[this.BaZi.RiZhi].Contains(_zhi)) { _result.Add("亡神"); }
        if (JieShaDict[this.BaZi.NianZhi].Contains(_zhi) || JieShaDict[this.BaZi.RiZhi].Contains(_zhi)) { _result.Add("劫煞"); }
        if (ZaiShaDict[this.BaZi.NianZhi].Contains(_zhi)) { _result.Add("灾煞"); }
        if (XueRenDict[this.BaZi.YueZhi].Contains(_zhi)) { _result.Add("血刃"); }
        if (FeiRenDict[this.BaZi.RiGan].Contains(_zhi)) { _result.Add("飞刃"); }
        if (YangRenDict[this.BaZi.RiGan].Contains(_zhi)) { _result.Add("羊刃"); }
        if (KongWangNianDict[this.BaZi.NianGanZhi].Contains(_zhi) || KongWangNianDict[this.BaZi.RiGanZhi].Contains(_zhi)) { _result.Add("空亡"); }

        return [.. _result];
    }
    #endregion

    #region FromBaZi
    public static ShenSha FromBaZi(BaZi _bazi)
    {
        IList<string> _nian = [];
        IList<string> _yue = [];
        IList<string> _ri = [];
        IList<string> _shi = [];

        if (TianYiGuiRenDict[_bazi.NianGan].Contains(_bazi.NianZhi) || TianYiGuiRenDict[_bazi.RiGan].Contains(_bazi.NianZhi)) { _nian.Add("天乙贵人"); }
        if (TianYiGuiRenDict[_bazi.NianGan].Contains(_bazi.YueZhi) || TianYiGuiRenDict[_bazi.RiGan].Contains(_bazi.YueZhi)) { _yue.Add("天乙贵人"); }
        if (TianYiGuiRenDict[_bazi.NianGan].Contains(_bazi.RiZhi) || TianYiGuiRenDict[_bazi.RiGan].Contains(_bazi.RiZhi)) { _ri.Add("天乙贵人"); }
        if (TianYiGuiRenDict[_bazi.NianGan].Contains(_bazi.ShiZhi) || TianYiGuiRenDict[_bazi.RiGan].Contains(_bazi.ShiZhi)) { _shi.Add("天乙贵人"); }

        if (TaiJiGuiRenDict[_bazi.NianGan].Contains(_bazi.NianZhi) || TaiJiGuiRenDict[_bazi.RiGan].Contains(_bazi.NianZhi)) { _nian.Add("太极贵人"); }
        if (TaiJiGuiRenDict[_bazi.NianGan].Contains(_bazi.YueZhi) || TaiJiGuiRenDict[_bazi.RiGan].Contains(_bazi.YueZhi)) { _yue.Add("太极贵人"); }
        if (TaiJiGuiRenDict[_bazi.NianGan].Contains(_bazi.RiZhi) || TaiJiGuiRenDict[_bazi.RiGan].Contains(_bazi.RiZhi)) { _ri.Add("太极贵人"); }
        if (TaiJiGuiRenDict[_bazi.NianGan].Contains(_bazi.ShiZhi) || TaiJiGuiRenDict[_bazi.RiGan].Contains(_bazi.ShiZhi)) { _shi.Add("太极贵人"); }

        if (WenChangGuiRenDict[_bazi.NianGan].Contains(_bazi.NianZhi) || WenChangGuiRenDict[_bazi.RiGan].Contains(_bazi.NianZhi)) { _nian.Add("文昌贵人"); }
        if (WenChangGuiRenDict[_bazi.NianGan].Contains(_bazi.YueZhi) || WenChangGuiRenDict[_bazi.RiGan].Contains(_bazi.YueZhi)) { _yue.Add("文昌贵人"); }
        if (WenChangGuiRenDict[_bazi.NianGan].Contains(_bazi.RiZhi) || WenChangGuiRenDict[_bazi.RiGan].Contains(_bazi.RiZhi)) { _ri.Add("文昌贵人"); }
        if (WenChangGuiRenDict[_bazi.NianGan].Contains(_bazi.ShiZhi) || WenChangGuiRenDict[_bazi.RiGan].Contains(_bazi.ShiZhi)) { _shi.Add("文昌贵人"); }

        if (TianDeGuiRenDict[_bazi.YueZhi].Contains(_bazi.NianGan) || TianDeGuiRenDict[_bazi.YueZhi].Contains(_bazi.NianZhi)) { _nian.Add("天德贵人"); }
        if (TianDeGuiRenDict[_bazi.YueZhi].Contains(_bazi.YueGan) || TianDeGuiRenDict[_bazi.YueZhi].Contains(_bazi.YueZhi)) { _yue.Add("天德贵人"); }
        if (TianDeGuiRenDict[_bazi.YueZhi].Contains(_bazi.RiGan) || TianDeGuiRenDict[_bazi.YueZhi].Contains(_bazi.RiZhi)) { _ri.Add("天德贵人"); }
        if (TianDeGuiRenDict[_bazi.YueZhi].Contains(_bazi.ShiGan) || TianDeGuiRenDict[_bazi.YueZhi].Contains(_bazi.ShiZhi)) { _shi.Add("天德贵人"); }

        if (YueDeGuiRenDict[_bazi.YueZhi].Contains(_bazi.NianGan)) { _nian.Add("月德贵人"); }
        if (YueDeGuiRenDict[_bazi.YueZhi].Contains(_bazi.YueGan)) { _yue.Add("月德贵人"); }
        if (YueDeGuiRenDict[_bazi.YueZhi].Contains(_bazi.RiGan)) { _ri.Add("月德贵人"); }
        if (YueDeGuiRenDict[_bazi.YueZhi].Contains(_bazi.ShiGan)) { _shi.Add("月德贵人"); }

        if (TianShangSanQiDict == $"{_bazi.NianGan}{_bazi.YueGan}{_bazi.RiGan}") { _nian.Add("天上三奇"); _yue.Add("天上三奇"); _ri.Add("天上三奇"); }
        if (TianShangSanQiDict == $"{_bazi.YueGan}{_bazi.RiGan}{_bazi.ShiGan}") { _yue.Add("天上三奇"); _ri.Add("天上三奇"); _shi.Add("天上三奇"); }
        if (DiShangSanQiDict == $"{_bazi.NianGan}{_bazi.YueGan}{_bazi.RiGan}") { _nian.Add("地支三奇"); _yue.Add("地支三奇"); _ri.Add("地支三奇"); }
        if (DiShangSanQiDict == $"{_bazi.YueGan}{_bazi.RiGan}{_bazi.ShiGan}") { _yue.Add("地支三奇"); _ri.Add("地支三奇"); _shi.Add("地支三奇"); }
        if (RenZhongSanQiDict == $"{_bazi.NianGan}{_bazi.YueGan}{_bazi.RiGan}") { _nian.Add("人中三奇"); _yue.Add("人中三奇"); _ri.Add("人中三奇"); }
        if (RenZhongSanQiDict == $"{_bazi.YueGan}{_bazi.RiGan}{_bazi.ShiGan}") { _yue.Add("人中三奇"); _ri.Add("人中三奇"); _shi.Add("人中三奇"); }

        if (FuXingGuiRenDict[_bazi.NianGan].Contains(_bazi.NianZhi) || FuXingGuiRenDict[_bazi.RiGan].Contains(_bazi.NianZhi)) { _nian.Add("福星贵人"); }
        if (FuXingGuiRenDict[_bazi.NianGan].Contains(_bazi.YueZhi) || FuXingGuiRenDict[_bazi.RiGan].Contains(_bazi.YueZhi)) { _yue.Add("福星贵人"); }
        if (FuXingGuiRenDict[_bazi.NianGan].Contains(_bazi.RiZhi) || FuXingGuiRenDict[_bazi.RiGan].Contains(_bazi.RiZhi)) { _ri.Add("福星贵人"); }
        if (FuXingGuiRenDict[_bazi.NianGan].Contains(_bazi.ShiZhi) || FuXingGuiRenDict[_bazi.RiGan].Contains(_bazi.ShiZhi)) { _shi.Add("福星贵人"); }

        if (DeXiuGuiRenDict[_bazi.YueZhi].Contains(_bazi.NianGan)) { _nian.Add("德秀贵人"); }
        if (DeXiuGuiRenDict[_bazi.YueZhi].Contains(_bazi.YueGan)) { _yue.Add("德秀贵人"); }
        if (DeXiuGuiRenDict[_bazi.YueZhi].Contains(_bazi.RiGan)) { _ri.Add("德秀贵人"); }
        if (DeXiuGuiRenDict[_bazi.YueZhi].Contains(_bazi.ShiGan)) { _shi.Add("德秀贵人"); }

        if (TianChuGuiRenDict[_bazi.NianGan].Contains(_bazi.NianZhi) || TianChuGuiRenDict[_bazi.RiGan].Contains(_bazi.NianZhi)) { _nian.Add("天厨贵人"); }
        if (TianChuGuiRenDict[_bazi.NianGan].Contains(_bazi.YueZhi) || TianChuGuiRenDict[_bazi.RiGan].Contains(_bazi.YueZhi)) { _yue.Add("天厨贵人"); }
        if (TianChuGuiRenDict[_bazi.NianGan].Contains(_bazi.RiZhi) || TianChuGuiRenDict[_bazi.RiGan].Contains(_bazi.RiZhi)) { _ri.Add("天厨贵人"); }
        if (TianChuGuiRenDict[_bazi.NianGan].Contains(_bazi.ShiZhi) || TianChuGuiRenDict[_bazi.RiGan].Contains(_bazi.ShiZhi)) { _shi.Add("天厨贵人"); }

        if (GuoYinGuiRenDict[_bazi.NianGan].Contains(_bazi.NianZhi) || GuoYinGuiRenDict[_bazi.RiGan].Contains(_bazi.NianZhi)) { _nian.Add("国印贵人"); }
        if (GuoYinGuiRenDict[_bazi.NianGan].Contains(_bazi.YueZhi) || GuoYinGuiRenDict[_bazi.RiGan].Contains(_bazi.YueZhi)) { _yue.Add("国印贵人"); }
        if (GuoYinGuiRenDict[_bazi.NianGan].Contains(_bazi.RiZhi) || GuoYinGuiRenDict[_bazi.RiGan].Contains(_bazi.RiZhi)) { _ri.Add("国印贵人"); }
        if (GuoYinGuiRenDict[_bazi.NianGan].Contains(_bazi.ShiZhi) || GuoYinGuiRenDict[_bazi.RiGan].Contains(_bazi.ShiZhi)) { _shi.Add("国印贵人"); }

        if (TianDeHeDict[_bazi.YueZhi].Contains(_bazi.NianGan) || TianDeHeDict[_bazi.YueZhi].Contains(_bazi.NianZhi)) { _nian.Add("天德合"); }
        if (TianDeHeDict[_bazi.YueZhi].Contains(_bazi.YueGan) || TianDeHeDict[_bazi.YueZhi].Contains(_bazi.YueZhi)) { _yue.Add("天德合"); }
        if (TianDeHeDict[_bazi.YueZhi].Contains(_bazi.RiGan) || TianDeHeDict[_bazi.YueZhi].Contains(_bazi.RiZhi)) { _ri.Add("天德合"); }
        if (TianDeHeDict[_bazi.YueZhi].Contains(_bazi.ShiGan) || TianDeHeDict[_bazi.YueZhi].Contains(_bazi.ShiZhi)) { _shi.Add("天德合"); }

        if (YueDeHeDict[_bazi.YueZhi].Contains(_bazi.NianGan)) { _nian.Add("月德合"); }
        if (YueDeHeDict[_bazi.YueZhi].Contains(_bazi.YueGan)) { _yue.Add("月德合"); }
        if (YueDeHeDict[_bazi.YueZhi].Contains(_bazi.RiGan)) { _ri.Add("月德合"); }
        if (YueDeHeDict[_bazi.YueZhi].Contains(_bazi.ShiGan)) { _shi.Add("月德合"); }

        if (TianLuoDiWangDict.TryGetValue(_bazi.RiZhi, out string[] _tldw1) && _tldw1.Contains(_bazi.NianZhi)) { _nian.Add("天罗地网"); }
        if ((TianLuoDiWangDict.TryGetValue(_bazi.NianZhi, out string[] _tldw2) && _tldw2.Contains(_bazi.YueZhi)) || (TianLuoDiWangDict.TryGetValue(_bazi.RiZhi, out string[] _tldw3) && _tldw3.Contains(_bazi.YueZhi))) { _yue.Add("天罗地网"); }
        if (TianLuoDiWangDict.TryGetValue(_bazi.NianZhi, out string[] _tldw4) && _tldw4.Contains(_bazi.RiZhi)) { _nian.Add("天罗地网"); }
        if ((TianLuoDiWangDict.TryGetValue(_bazi.NianZhi, out string[] _tldw5) && _tldw5.Contains(_bazi.ShiZhi)) || (TianLuoDiWangDict.TryGetValue(_bazi.RiZhi, out string[] _tldw6) && _tldw6.Contains(_bazi.ShiZhi))) { _yue.Add("天罗地网"); }

        if (TianLuoDict.TryGetValue(_bazi.NianNaYin[^1..], out string[] _tianluo) && _tianluo.Contains(_bazi.RiZhi)) { _ri.Add("天罗"); }
        if (DiWangDict.TryGetValue(_bazi.NianNaYin[^1..], out string[] _diwang) && _diwang.Contains(_bazi.RiZhi)) { _ri.Add("地网"); }

        if (ShiEBaDaiDict.Contains(_bazi.RiGanZhi)) { _ri.Add("十恶大败"); }

        if (YinChaYangCuoDict.Contains(_bazi.RiGanZhi)) { _ri.Add("阴差阳错"); }

        if (TianSheDict[_bazi.YueZhi].Contains(_bazi.RiGanZhi)) { _ri.Add("天赦"); }

        if (ShiLingDict.Contains(_bazi.RiGanZhi)) { _ri.Add("十灵"); }

        if (LiuXiuDict.Contains(_bazi.RiGanZhi)) { _ri.Add("六秀"); }

        if (SiFeiDict[_bazi.YueZhi].Contains(_bazi.RiGanZhi)) { _ri.Add("四废"); }

        if (GuLuanShaDict.Contains(_bazi.RiGanZhi)) { _ri.Add("孤鸾煞"); }

        if (GouJiaoShaDict[_bazi.NianZhi].Contains(_bazi.YueZhi)) { _yue.Add("勾绞煞"); }
        if (GouJiaoShaDict[_bazi.NianZhi].Contains(_bazi.RiZhi)) { _ri.Add("勾绞煞"); }
        if (GouJiaoShaDict[_bazi.NianZhi].Contains(_bazi.ShiZhi)) { _shi.Add("勾绞煞"); }

        if (HongYanShaDict[_bazi.RiGan].Contains(_bazi.NianZhi)) { _nian.Add("红艳煞"); }
        if (HongYanShaDict[_bazi.RiGan].Contains(_bazi.YueZhi)) { _yue.Add("红艳煞"); }
        if (HongYanShaDict[_bazi.RiGan].Contains(_bazi.RiZhi)) { _ri.Add("红艳煞"); }
        if (HongYanShaDict[_bazi.RiGan].Contains(_bazi.ShiZhi)) { _shi.Add("红艳煞"); }

        if (TongZiShaNianDict[_bazi.NianNaYin[^1..]].Contains(_bazi.RiZhi) || TongZiShaYueDict[_bazi.YueZhi].Contains(_bazi.RiZhi)) { _ri.Add("童子煞"); }
        if (TongZiShaNianDict[_bazi.NianNaYin[^1..]].Contains(_bazi.ShiZhi) || TongZiShaYueDict[_bazi.YueZhi].Contains(_bazi.ShiZhi)) { _shi.Add("童子煞"); }

        if (HuaGaiDict[_bazi.RiZhi].Contains(_bazi.NianZhi)) { _nian.Add("华盖"); }
        if (HuaGaiDict[_bazi.NianZhi].Contains(_bazi.YueZhi) || HuaGaiDict[_bazi.RiZhi].Contains(_bazi.YueZhi)) { _yue.Add("华盖"); }
        if (HuaGaiDict[_bazi.NianZhi].Contains(_bazi.RiZhi)) { _ri.Add("华盖"); }
        if (HuaGaiDict[_bazi.NianZhi].Contains(_bazi.ShiZhi) || HuaGaiDict[_bazi.RiZhi].Contains(_bazi.ShiZhi)) { _shi.Add("华盖"); }

        if (YiMaDict[_bazi.RiZhi].Contains(_bazi.NianZhi)) { _nian.Add("驿马"); }
        if (YiMaDict[_bazi.NianZhi].Contains(_bazi.YueZhi) || YiMaDict[_bazi.RiZhi].Contains(_bazi.YueZhi)) { _yue.Add("驿马"); }
        if (YiMaDict[_bazi.NianZhi].Contains(_bazi.RiZhi)) { _ri.Add("驿马"); }
        if (YiMaDict[_bazi.NianZhi].Contains(_bazi.ShiZhi) || YiMaDict[_bazi.RiZhi].Contains(_bazi.ShiZhi)) { _shi.Add("驿马"); }

        if (LuShengDict[_bazi.RiGan].Contains(_bazi.NianZhi)) { _nian.Add("禄神"); }
        if (LuShengDict[_bazi.RiGan].Contains(_bazi.YueZhi)) { _yue.Add("禄神"); }
        if (LuShengDict[_bazi.RiGan].Contains(_bazi.RiZhi)) { _ri.Add("禄神"); }
        if (LuShengDict[_bazi.RiGan].Contains(_bazi.ShiZhi)) { _shi.Add("禄神"); }

        if (JiangXingDict[_bazi.RiZhi].Contains(_bazi.NianZhi)) { _nian.Add("将星"); }
        if (JiangXingDict[_bazi.NianZhi].Contains(_bazi.YueZhi) || JiangXingDict[_bazi.RiZhi].Contains(_bazi.YueZhi)) { _yue.Add("将星"); }
        if (JiangXingDict[_bazi.NianZhi].Contains(_bazi.RiZhi)) { _ri.Add("将星"); }
        if (JiangXingDict[_bazi.NianZhi].Contains(_bazi.ShiZhi) || JiangXingDict[_bazi.RiZhi].Contains(_bazi.ShiZhi)) { _shi.Add("将星"); }

        if (XueTangDict[_bazi.NianNaYin[^1..]].Contains(_bazi.YueZhi)) { _yue.Add(ZhengXueTangDict[_bazi.NianNaYin[^1..]].Contains(_bazi.YueGanZhi) ? "正学堂" : "学堂"); }
        if (XueTangDict[_bazi.NianNaYin[^1..]].Contains(_bazi.RiZhi)) { _ri.Add(ZhengXueTangDict[_bazi.NianNaYin[^1..]].Contains(_bazi.RiGanZhi) ? "正学堂" : "学堂"); }
        if (XueTangDict[_bazi.NianNaYin[^1..]].Contains(_bazi.ShiZhi)) { _shi.Add(ZhengXueTangDict[_bazi.NianNaYin[^1..]].Contains(_bazi.ShiGanZhi) ? "正学堂" : "学堂"); }

        if (CiGuanDict[_bazi.NianNaYin[^1..]].Contains(_bazi.YueZhi)) { _yue.Add(ZhengCiGuanDict[_bazi.NianNaYin[^1..]].Contains(_bazi.YueGanZhi) ? "正词馆" : "词馆"); }
        if (CiGuanDict[_bazi.NianNaYin[^1..]].Contains(_bazi.RiZhi)) { _ri.Add(ZhengCiGuanDict[_bazi.NianNaYin[^1..]].Contains(_bazi.RiGanZhi) ? "正词馆" : "词馆"); }
        if (CiGuanDict[_bazi.NianNaYin[^1..]].Contains(_bazi.ShiZhi)) { _shi.Add(ZhengCiGuanDict[_bazi.NianNaYin[^1..]].Contains(_bazi.ShiGanZhi) ? "正词馆" : "词馆"); }

        if (TianYiDict[_bazi.YueZhi].Contains(_bazi.NianZhi)) { _nian.Add("天医"); }
        if (TianYiDict[_bazi.YueZhi].Contains(_bazi.YueZhi)) { _yue.Add("天医"); }
        if (TianYiDict[_bazi.YueZhi].Contains(_bazi.RiZhi)) { _ri.Add("天医"); }
        if (TianYiDict[_bazi.YueZhi].Contains(_bazi.ShiZhi)) { _shi.Add("天医"); }

        if (JinYuDict[_bazi.NianGan].Contains(_bazi.NianZhi) || JinYuDict[_bazi.RiGan].Contains(_bazi.NianZhi)) { _nian.Add("金舆"); }
        if (JinYuDict[_bazi.NianGan].Contains(_bazi.YueZhi) || JinYuDict[_bazi.RiGan].Contains(_bazi.YueZhi)) { _yue.Add("金舆"); }
        if (JinYuDict[_bazi.NianGan].Contains(_bazi.RiZhi) || JinYuDict[_bazi.RiGan].Contains(_bazi.RiZhi)) { _ri.Add("金舆"); }
        if (JinYuDict[_bazi.NianGan].Contains(_bazi.ShiZhi) || JinYuDict[_bazi.RiGan].Contains(_bazi.ShiZhi)) { _shi.Add("金舆"); }

        if (TaoHuaDict[_bazi.RiZhi].Contains(_bazi.NianZhi)) { _nian.Add("桃花"); }
        if (TaoHuaDict[_bazi.NianZhi].Contains(_bazi.YueZhi) || TaoHuaDict[_bazi.RiZhi].Contains(_bazi.YueZhi)) { _yue.Add("桃花"); }
        if (TaoHuaDict[_bazi.NianZhi].Contains(_bazi.RiZhi)) { _ri.Add("桃花"); }
        if (TaoHuaDict[_bazi.NianZhi].Contains(_bazi.ShiZhi) || TaoHuaDict[_bazi.RiZhi].Contains(_bazi.ShiZhi)) { _shi.Add("桃花"); }

        if (HongLuanDict[_bazi.NianZhi].Contains(_bazi.YueZhi)) { _yue.Add("红鸾"); }
        if (HongLuanDict[_bazi.NianZhi].Contains(_bazi.RiZhi)) { _ri.Add("红鸾"); }
        if (HongLuanDict[_bazi.NianZhi].Contains(_bazi.ShiZhi)) { _shi.Add("红鸾"); }

        if (TianXiDict[_bazi.NianZhi].Contains(_bazi.YueZhi)) { _yue.Add("天喜"); }
        if (TianXiDict[_bazi.NianZhi].Contains(_bazi.RiZhi)) { _ri.Add("天喜"); }
        if (TianXiDict[_bazi.NianZhi].Contains(_bazi.ShiZhi)) { _shi.Add("天喜"); }

        if (JinShenDict.Contains(_bazi.RiGanZhi)) { _ri.Add("金神"); }
        if (JinShenDict.Contains(_bazi.ShiGanZhi)) { _shi.Add("金神"); }

        if (GongLuDict.TryGetValue(_bazi.ShiGanZhi, out string[] _gonglu) && _gonglu.Contains(_bazi.RiGanZhi)) { _ri.Add("拱禄"); }

        if (TianZhuanDict[_bazi.YueZhi].Contains(_bazi.RiGanZhi)) { _ri.Add("天转"); }

        if (DiZhuanDict[_bazi.YueZhi].Contains(_bazi.RiGanZhi)) { _ri.Add("地转"); }

        if (SangMenDict[_bazi.NianZhi].Contains(_bazi.YueZhi)) { _yue.Add("丧门"); }
        if (SangMenDict[_bazi.NianZhi].Contains(_bazi.RiZhi)) { _ri.Add("丧门"); }
        if (SangMenDict[_bazi.NianZhi].Contains(_bazi.ShiZhi)) { _shi.Add("丧门"); }

        if (DiaoKeDict[_bazi.NianZhi].Contains(_bazi.YueZhi)) { _yue.Add("吊客"); }
        if (DiaoKeDict[_bazi.NianZhi].Contains(_bazi.RiZhi)) { _ri.Add("吊客"); }
        if (DiaoKeDict[_bazi.NianZhi].Contains(_bazi.ShiZhi)) { _shi.Add("吊客"); }

        if (PiMaDict[_bazi.NianZhi].Contains(_bazi.YueZhi)) { _yue.Add("披麻"); }
        if (PiMaDict[_bazi.NianZhi].Contains(_bazi.RiZhi)) { _ri.Add("披麻"); }
        if (PiMaDict[_bazi.NianZhi].Contains(_bazi.ShiZhi)) { _shi.Add("披麻"); }

        if (BaZhuanDict.Contains(_bazi.RiGanZhi)) { _ri.Add("八专"); }

        if (JiuChouDict.Contains(_bazi.RiGanZhi)) { _ri.Add("九丑"); }

        if (YuanChenDict[_bazi.NianZhi].Contains(_bazi.YueZhi)) { _yue.Add("元辰"); }
        if (YuanChenDict[_bazi.NianZhi].Contains(_bazi.RiZhi)) { _ri.Add("元辰"); }
        if (YuanChenDict[_bazi.NianZhi].Contains(_bazi.ShiZhi)) { _shi.Add("元辰"); }

        if (KuiGangDict.Contains(_bazi.RiGanZhi)) { _ri.Add("魁罡"); }

        if (GuChenDict[_bazi.NianZhi].Contains(_bazi.YueZhi)) { _yue.Add("孤辰"); }
        if (GuChenDict[_bazi.NianZhi].Contains(_bazi.RiZhi)) { _ri.Add("孤辰"); }
        if (GuChenDict[_bazi.NianZhi].Contains(_bazi.ShiZhi)) { _shi.Add("孤辰"); }

        if (GuaSuDict[_bazi.NianZhi].Contains(_bazi.YueZhi)) { _yue.Add("寡宿"); }
        if (GuaSuDict[_bazi.NianZhi].Contains(_bazi.RiZhi)) { _ri.Add("寡宿"); }
        if (GuaSuDict[_bazi.NianZhi].Contains(_bazi.ShiZhi)) { _shi.Add("寡宿"); }

        if (LiuXiaDict[_bazi.RiGan].Contains(_bazi.NianZhi)) { _nian.Add("流霞"); }
        if (LiuXiaDict[_bazi.RiGan].Contains(_bazi.YueZhi)) { _yue.Add("流霞"); }
        if (LiuXiaDict[_bazi.RiGan].Contains(_bazi.RiZhi)) { _ri.Add("流霞"); }
        if (LiuXiaDict[_bazi.RiGan].Contains(_bazi.ShiZhi)) { _shi.Add("流霞"); }

        if (WangShenDict[_bazi.RiZhi].Contains(_bazi.NianZhi)) { _nian.Add("亡神"); }
        if (WangShenDict[_bazi.NianZhi].Contains(_bazi.YueZhi) || WangShenDict[_bazi.RiZhi].Contains(_bazi.YueZhi)) { _yue.Add("亡神"); }
        if (WangShenDict[_bazi.NianZhi].Contains(_bazi.RiZhi)) { _ri.Add("亡神"); }
        if (WangShenDict[_bazi.NianZhi].Contains(_bazi.ShiZhi) || WangShenDict[_bazi.RiZhi].Contains(_bazi.ShiZhi)) { _shi.Add("亡神"); }

        if (JieShaDict[_bazi.RiZhi].Contains(_bazi.NianZhi)) { _nian.Add("劫煞"); }
        if (JieShaDict[_bazi.NianZhi].Contains(_bazi.YueZhi) || JieShaDict[_bazi.RiZhi].Contains(_bazi.YueZhi)) { _yue.Add("劫煞"); }
        if (JieShaDict[_bazi.NianZhi].Contains(_bazi.RiZhi)) { _ri.Add("劫煞"); }
        if (JieShaDict[_bazi.NianZhi].Contains(_bazi.ShiZhi) || JieShaDict[_bazi.RiZhi].Contains(_bazi.ShiZhi)) { _shi.Add("劫煞"); }

        if (ZaiShaDict[_bazi.NianZhi].Contains(_bazi.NianZhi)) { _yue.Add("灾煞"); }
        if (ZaiShaDict[_bazi.NianZhi].Contains(_bazi.YueZhi)) { _yue.Add("灾煞"); }
        if (ZaiShaDict[_bazi.NianZhi].Contains(_bazi.RiZhi)) { _ri.Add("灾煞"); }
        if (ZaiShaDict[_bazi.NianZhi].Contains(_bazi.ShiZhi)) { _shi.Add("灾煞"); }

        if (XueRenDict[_bazi.YueZhi].Contains(_bazi.NianZhi)) { _nian.Add("血刃"); }
        if (XueRenDict[_bazi.YueZhi].Contains(_bazi.YueZhi)) { _yue.Add("血刃"); }
        if (XueRenDict[_bazi.YueZhi].Contains(_bazi.RiZhi)) { _ri.Add("血刃"); }
        if (XueRenDict[_bazi.YueZhi].Contains(_bazi.ShiZhi)) { _shi.Add("血刃"); }

        if (FeiRenDict[_bazi.RiGan].Contains(_bazi.NianZhi)) { _nian.Add("飞刃"); }
        if (FeiRenDict[_bazi.RiGan].Contains(_bazi.YueZhi)) { _yue.Add("飞刃"); }
        if (FeiRenDict[_bazi.RiGan].Contains(_bazi.RiZhi)) { _ri.Add("飞刃"); }
        if (FeiRenDict[_bazi.RiGan].Contains(_bazi.ShiZhi)) { _shi.Add("飞刃"); }

        if (YangRenDict[_bazi.RiGan].Contains(_bazi.NianZhi)) { _nian.Add("羊刃"); }
        if (YangRenDict[_bazi.RiGan].Contains(_bazi.YueZhi)) { _yue.Add("羊刃"); }
        if (YangRenDict[_bazi.RiGan].Contains(_bazi.RiZhi)) { _ri.Add("羊刃"); }
        if (YangRenDict[_bazi.RiGan].Contains(_bazi.ShiZhi)) { _shi.Add("羊刃"); }

        if (KongWangNianDict[_bazi.RiGanZhi].Contains(_bazi.NianZhi)) { _nian.Add("空亡"); }
        if (KongWangNianDict[_bazi.NianGanZhi].Contains(_bazi.YueZhi) || KongWangNianDict[_bazi.RiGanZhi].Contains(_bazi.YueZhi)) { _yue.Add("空亡"); }
        if (KongWangNianDict[_bazi.NianGanZhi].Contains(_bazi.RiZhi)) { _ri.Add("空亡"); }
        if (KongWangNianDict[_bazi.NianGanZhi].Contains(_bazi.ShiZhi) || KongWangNianDict[_bazi.RiGanZhi].Contains(_bazi.ShiZhi)) { _shi.Add("空亡"); }

        return new ShenSha()
        {
            BaZi = _bazi,
            Nian = [.. _nian],
            Yue = [.. _yue],
            Ri = [.. _ri],
            Shi = [.. _shi]
        };
    }
    #endregion

    #region 字典
    #region 天乙贵人
    public static Dictionary<string, string[]> TianYiGuiRenDict = new()
    {
        //年日干 - 四柱、流运
        //甲戊庚牛羊，乙己鼠猴乡，丙丁猪鸡位，壬癸蛇兔藏，
        //六辛逢虎马，此是贵人方。辰戌魁罡位，贵人不到场。
        { "甲", [ "丑", "未" ] },
        { "乙", [ "子", "申" ] },
        { "丙", [ "亥", "酉" ] },
        { "丁", [ "亥", "酉" ] },
        { "戊", [ "丑", "未" ] },
        { "己", [ "子", "申" ] },
        { "庚", [ "丑", "未" ] },
        { "辛", [ "寅", "午" ] },
        { "壬", [ "卯", "巳" ] },
        { "癸", [ "卯", "巳" ] }
    };
    #endregion

    #region 太极贵人
    public static Dictionary<string, string[]> TaiJiGuiRenDict = new()
    {
        //年日干 - 四柱、流运
        //甲乙生人子午中，丙丁鸡兔定亨通，戊己两干临四季，庚辛寅亥禄丰隆，
        //壬癸巳申偏喜美，值此应当福气钟，更须贵格来相扶，候封万户到三公。
        { "甲", [ "子", "午" ] },
        { "乙", [ "子", "午" ] },
        { "丙", [ "卯", "酉" ] },
        { "丁", [ "卯", "酉" ] },
        { "戊", [ "辰", "戌", "丑", "未" ] },
        { "己", [ "辰", "戌", "丑", "未" ] },
        { "庚", [ "寅", "亥" ] },
        { "辛", [ "寅", "亥" ] },
        { "壬", [ "巳", "申" ] },
        { "癸", [ "巳", "申" ] }
    };
    #endregion

    #region 文昌贵人
    public static readonly Dictionary<string, string[]> WenChangGuiRenDict = new()
    {
        //年日干 - 四柱、流运
        //甲-巳，乙-午，丙-申，丁-酉，戊-申，己-酉，庚-亥，辛-子，壬-寅，癸-卯
        { "甲", [ "巳" ] },
        { "乙", [ "午" ] },
        { "丙", [ "申" ] },
        { "丁", [ "酉" ] },
        { "戊", [ "申" ] },
        { "己", [ "酉" ] },
        { "庚", [ "亥" ] },
        { "辛", [ "子" ] },
        { "壬", [ "寅" ] },
        { "癸", [ "卯" ] }
    };
    #endregion

    #region 天德贵人
    public static readonly Dictionary<string, string[]> TianDeGuiRenDict = new()
    {
        //月支 - 四柱、流运
        //丑-庚，寅-丁，辰-壬，巳-辛，未-甲，申-癸，戌-丙，亥-乙
        //子-巳，卯-申，午-亥，酉-寅
        { "丑", [ "庚" ] },
        { "寅", [ "丁" ] },
        { "辰", [ "壬" ] },
        { "巳", [ "辛" ] },
        { "未", [ "甲" ] },
        { "申", [ "癸" ] },
        { "戌", [ "丙" ] },
        { "亥", [ "乙" ] },
        { "子", [ "巳" ] },
        { "卯", [ "申" ] },
        { "午", [ "亥" ] },
        { "酉", [ "寅" ] }
    };
    #endregion

    #region 月德贵人
    public static readonly Dictionary<string, string[]> YueDeGuiRenDict = new()
    {
        //月支 - 四柱、流运
        //子-壬，丑-庚，寅-丙，卯-甲，辰-壬，巳-庚，午-丙，未-甲，申-壬，酉-庚，戌-丙，亥-甲
        { "子", [ "壬" ] },
        { "丑", [ "庚" ] },
        { "寅", [ "丙" ] },
        { "卯", [ "甲" ] },
        { "辰", [ "壬" ] },
        { "巳", [ "庚" ] },
        { "午", [ "丙" ] },
        { "未", [ "甲" ] },
        { "申", [ "壬" ] },
        { "酉", [ "庚" ] },
        { "戌", [ "丙" ] },
        { "亥", [ "甲" ] }
    };
    #endregion

    #region 三奇贵人
    public static readonly string TianShangSanQiDict = "乙丙丁";
    public static readonly string DiShangSanQiDict = "甲戊庚";
    public static readonly string RenZhongSanQiDict = "壬癸辛";
    #endregion

    #region 福星贵人
    public static readonly Dictionary<string, string[]> FuXingGuiRenDict = new()
    {
        //年日干 - 四柱、流运
        //甲-寅子，乙-卯丑，丙-寅子，丁-亥，戊-申，己-未，庚-午，辛-巳，壬-辰，癸-卯丑
        { "甲", [ "寅", "子" ] },
        { "乙", [ "卯", "丑" ] },
        { "丙", [ "寅", "子" ] },
        { "丁", [ "亥" ] },
        { "戊", [ "申" ] },
        { "己", [ "未" ] },
        { "庚", [ "午" ] },
        { "辛", [ "巳" ] },
        { "壬", [ "辰" ] },
        { "癸", [ "卯", "丑" ] }
    };
    #endregion

    #region 德秀贵人
    public static Dictionary<string, string[]> DeXiuGuiRenDict = new()
    {
        //月支 - 四柱、流运
        //寅-丙丁戊癸，午-丙丁戊癸，戌-丙丁戊癸，申-壬癸戊己丙辛甲，子-壬癸戊己丙辛甲，辰-壬癸戊己丙辛甲，
        //巳-乙庚辛，酉-乙庚辛，丑-乙庚辛，亥-甲乙丁壬，卯-甲乙丁壬，未-甲乙丁壬
        { "寅", [ "丙", "丁", "戊", "癸" ] },
        { "午", [ "丙", "丁", "戊", "癸" ] },
        { "戌", [ "丙", "丁", "戊", "癸" ] },
        { "申", [ "壬", "癸", "戊", "己", "丙", "辛", "甲" ] },
        { "子", [ "壬", "癸", "戊", "己", "丙", "辛", "甲" ] },
        { "辰", [ "壬", "癸", "戊", "己", "丙", "辛", "甲" ] },
        { "巳", [ "乙", "庚", "辛" ] },
        { "酉", [ "乙", "庚", "辛" ] },
        { "丑", [ "乙", "庚", "辛" ] },
        { "亥", [ "甲", "乙", "丁", "壬" ] },
        { "卯", [ "甲", "乙", "丁", "壬" ] },
        { "未", [ "甲", "乙", "丁", "壬" ] }
    };
    #endregion

    #region 天厨贵人
    public static readonly Dictionary<string, string[]> TianChuGuiRenDict = new()
    {
        //年日干 - 四柱、流运
        //甲-巳，乙-午，丙-巳，丁-午，戊-申，己-酉，庚-亥，辛-子，壬-寅，癸-卯
        { "甲", [ "巳" ] },
        { "乙", [ "午" ] },
        { "丙", [ "巳" ] },
        { "丁", [ "午" ] },
        { "戊", [ "申" ] },
        { "己", [ "酉" ] },
        { "庚", [ "亥" ] },
        { "辛", [ "子" ] },
        { "壬", [ "寅" ] },
        { "癸", [ "卯" ] }
    };
    #endregion

    #region 国印贵人
    public static readonly Dictionary<string, string[]> GuoYinGuiRenDict = new()
    {
        //年日干 - 四柱、流运
        //甲-戌，乙-亥，丙-丑，丁-寅，戊-丑，己-寅，庚-辰，辛-巳，壬-未，癸-申
        { "甲", [ "戌" ] },
        { "乙", [ "亥" ] },
        { "丙", [ "丑" ] },
        { "丁", [ "寅" ] },
        { "戊", [ "丑" ] },
        { "己", [ "寅" ] },
        { "庚", [ "辰" ] },
        { "辛", [ "巳" ] },
        { "壬", [ "未" ] },
        { "癸", [ "申" ] }
    };
    #endregion

    #region 天德合
    public static readonly Dictionary<string, string[]> TianDeHeDict = new()
    {
        //月支 - 四柱、流运
        //丑-乙，寅-壬，辰-丁，巳-丙，未-己，申-戊，戌-辛，亥-庚
        //子-申，卯-巳，午-寅，酉-亥
        { "丑", [ "乙" ] },
        { "寅", [ "壬" ] },
        { "辰", [ "丁" ] },
        { "巳", [ "丙" ] },
        { "未", [ "己" ] },
        { "申", [ "戊" ] },
        { "戌", [ "辛" ] },
        { "亥", [ "庚" ] },
        { "子", [ "申" ] },
        { "卯", [ "巳" ] },
        { "午", [ "寅" ] },
        { "酉", [ "亥" ] }
    };
    #endregion

    #region 月德合
    public static readonly Dictionary<string, string[]> YueDeHeDict = new()
    {
        //月支 - 四柱、流运
        //子-丁，丑-乙，寅-辛，卯-己，辰-丁，巳-乙，午-辛，未-己，申-丁，酉-乙，戌-辛，亥-己
        { "子", [ "丁" ] },
        { "丑", [ "乙" ] },
        { "寅", [ "辛" ] },
        { "卯", [ "己" ] },
        { "辰", [ "丁" ] },
        { "巳", [ "乙" ] },
        { "午", [ "辛" ] },
        { "未", [ "己" ] },
        { "申", [ "丁" ] },
        { "酉", [ "乙" ] },
        { "戌", [ "辛" ] },
        { "亥", [ "己" ] }
    };
    #endregion

    #region 天罗地网    
    public static readonly Dictionary<string, string[]> TianLuoDiWangDict = new()
    {
        //年支 - 月、日、时、流运
        //日支 - 年、月、时、流运
        //辰-巳，巳-辰，戌-亥，亥-戌
        { "辰", [ "巳" ] },
        { "巳", [ "辰" ] },
        { "戌", [ "亥" ] },
        { "亥", [ "戌" ] }
    };
    #endregion

    #region 天罗
    public static readonly Dictionary<string, string[]> TianLuoDict = new()
    {
        //年纳音 - 日、流运
        //火-戌、亥
        { "火", [ "戌", "亥" ] }
    };
    #endregion

    #region 地网
    public static readonly Dictionary<string, string[]> DiWangDict = new()
    {
        //年纳音 - 日、流运
        //水-辰、巳，土-辰、巳
        { "水", [ "辰", "巳" ] },
        { "土", [ "辰", "巳" ] }
    };
    #endregion

    #region 十恶大败
    public static readonly string[] ShiEBaDaiDict =
    [
        //日柱
        //壬申，庚辰，辛巳，丁亥，己丑，丙申，戊戌，甲辰，乙巳，癸亥
        "壬申",
        "庚辰",
        "辛巳",
        "丁亥",
        "己丑",
        "丙申",
        "戊戌",
        "甲辰",
        "乙巳",
        "癸亥"
    ];
    #endregion

    #region 阴差阳错
    public static readonly string[] YinChaYangCuoDict =
    [
        //日柱
        //丙子，丁丑，戊寅，辛卯，壬辰，丙午，丁未，戊申，辛酉，壬戌，癸亥，癸巳
        "丙子",
        "丁丑",
        "戊寅",
        "辛卯",
        "壬辰",
        "丙午",
        "丁未",
        "戊申",
        "辛酉",
        "壬戌",
        "癸亥",
        "癸巳"
    ];
    #endregion

    #region 天赦
    public static readonly Dictionary<string, string[]> TianSheDict = new()
    {
        //月支 - 日
        //寅-戊寅，卯-戊寅，辰-戊寅，巳-甲午，午-甲午，未-甲午，申-戊申，酉-戊申，戌-戊申，亥-甲子，子-甲子，丑-甲子
        { "寅", [ "戊寅" ] },
        { "卯", [ "戊寅" ] },
        { "辰", [ "戊寅" ] },
        { "巳", [ "甲午" ] },
        { "午", [ "甲午" ] },
        { "未", [ "甲午" ] },
        { "申", [ "戊申" ] },
        { "酉", [ "戊申" ] },
        { "戌", [ "戊申" ] },
        { "亥", [ "甲子" ] },
        { "子", [ "甲子" ] },
        { "丑", [ "甲子" ] }
    };
    #endregion

    #region 十灵
    public static readonly string[] ShiLingDict =
    [
        //日柱
        //乙亥，癸未，庚寅，丁酉，壬寅，甲辰，庚戌，辛亥，丙辰，戊午
        "乙亥",
        "癸未",
        "庚寅",
        "丁酉",
        "壬寅",
        "甲辰",
        "庚戌",
        "辛亥",
        "丙辰",
        "戊午"
    ];
    #endregion

    #region 六秀
    public static readonly string[] LiuXiuDict =
    [
        //日柱
        //戊子，己丑，丙午，丁未，戊午，己未
        "戊子",
        "己丑",
        "丙午",
        "丁未",
        "戊午",
        "己未"
    ];
    #endregion

    #region 四废
    public static readonly Dictionary<string, string[]> SiFeiDict = new()
    {
        //月支 - 日
        //寅-庚申，卯-庚申，辰-庚申，巳-壬子，午-壬子，未-壬子，申-甲寅，酉-甲寅，戌-甲寅，亥-丙午，子-丙午，丑-丙午
        //寅-辛酉，卯-辛酉，辰-辛酉，巳-癸亥，午-癸亥，未-癸亥，申-乙卯，酉-乙卯，戌-乙卯，亥-丁巳，子-丁巳，丑-丁巳
        { "寅", [ "庚申", "辛酉" ] },
        { "卯", [ "庚申", "辛酉" ] },
        { "辰", [ "庚申", "辛酉" ] },
        { "巳", [ "壬子", "癸亥" ] },
        { "午", [ "壬子", "癸亥" ] },
        { "未", [ "壬子", "癸亥" ] },
        { "申", [ "甲寅", "乙卯" ] },
        { "酉", [ "甲寅", "乙卯" ] },
        { "戌", [ "甲寅", "乙卯" ] },
        { "亥", [ "丙午", "丁巳" ] },
        { "子", [ "丙午", "丁巳" ] },
        { "丑", [ "丙午", "丁巳" ] }
    };
    #endregion

    #region 孤鸾煞
    public static readonly string[] GuLuanShaDict =
    [
        //日柱
        //丁巳，乙巳，丙午，戊申，辛亥，壬子，甲寅，戊午
        "丁巳",
        "乙巳",
        "丙午",
        "戊申",
        "辛亥",
        "壬子",
        "甲寅",
        "戊午"
    ];
    #endregion

    #region 勾绞煞
    public static readonly Dictionary<string, string[]> GouJiaoShaDict = new()
    {
        //年支 - 月、日、时、流运
        //子-卯，丑-辰，寅-巳，卯-午，辰-未，巳-申，午-酉，未-戌，申-亥，酉-子，戌-丑，亥-寅
        { "子", [ "卯" ] },
        { "丑", [ "辰" ] },
        { "寅", [ "巳" ] },
        { "卯", [ "午" ] },
        { "辰", [ "未" ] },
        { "巳", [ "申" ] },
        { "午", [ "酉" ] },
        { "未", [ "戌" ] },
        { "申", [ "亥" ] },
        { "酉", [ "子" ] },
        { "戌", [ "丑" ] },
        { "亥", [ "寅" ] }
    };
    #endregion

    #region 红艳煞
    public static readonly Dictionary<string, string[]> HongYanShaDict = new()
    {
        //日干 - 四柱、流运
        //甲-午，乙-午，丙-寅，丁-未，戊-辰，己-辰，庚-戌，辛-酉，壬-子，癸-申
        { "甲", [ "午" ] },
        { "乙", [ "午" ] },
        { "丙", [ "寅" ] },
        { "丁", [ "未" ] },
        { "戊", [ "辰" ] },
        { "己", [ "辰" ] },
        { "庚", [ "戌" ] },
        { "辛", [ "酉" ] },
        { "壬", [ "子" ] },
        { "癸", [ "申" ] }
    };
    #endregion

    #region 童子煞
    public static readonly Dictionary<string, string[]> TongZiShaYueDict = new()
    {
        //月支 - 日、时
        //子-卯未辰，丑-卯未辰，寅-寅子，卯-寅子，辰-寅子，巳-卯未辰，午-卯未辰，未-卯未辰，申-寅子，酉-寅子，戌-寅子，亥-卯未辰
        { "子", [ "卯", "未", "辰" ] },
        { "丑", [ "卯", "未", "辰" ] },
        { "寅", [ "寅", "子" ] },
        { "卯", [ "寅", "子" ] },
        { "辰", [ "寅", "子" ] },
        { "巳", [ "卯", "未", "辰" ] },
        { "午", [ "卯", "未", "辰" ] },
        { "未", [ "卯", "未", "辰" ] },
        { "申", [ "寅", "子" ] },
        { "酉", [ "寅", "子" ] },
        { "戌", [ "寅", "子" ] },
        { "亥", [ "卯", "未", "辰" ] }
    };
    public static readonly Dictionary<string, string[]> TongZiShaNianDict = new()
    {
        //年五行 - 日、时
        //金-午、卯，木-午、卯，水-酉、戌，火-酉、戌，土-辰、巳
        { "金", [ "午", "卯" ] },
        { "木", [ "午", "卯" ] },
        { "水", [ "酉", "戌" ] },
        { "土", [ "辰", "巳" ] },
        { "火", [ "酉", "戌" ] }
    };
    #endregion

    #region 华盖
    public static readonly Dictionary<string, string[]> HuaGaiDict = new()
    {
        //年支 - 月、日、时、流运
        //日支 - 年、月、时、流运
        //子-辰，丑-丑，寅-戌，卯-未，辰-辰，巳-丑，午-戌，未-未，申-辰，酉-丑，戌-戌，亥-未
        { "子", [ "辰" ] },
        { "丑", [ "丑" ] },
        { "寅", [ "戌" ] },
        { "卯", [ "未" ] },
        { "辰", [ "辰" ] },
        { "巳", [ "丑" ] },
        { "午", [ "戌" ] },
        { "未", [ "未" ] },
        { "申", [ "辰" ] },
        { "酉", [ "丑" ] },
        { "戌", [ "戌" ] },
        { "亥", [ "未" ] }
    };
    #endregion

    #region 驿马
    public static readonly Dictionary<string, string[]> YiMaDict = new()
    {
        //年支 - 月、日、时、流运
        //日支 - 年、月、时、流运
        //子-寅，丑-亥，寅-申，卯-巳，辰-寅，巳-亥，午-申，未-巳，申-寅，酉-亥，戌-申，亥-巳
        { "子", [ "寅" ] },
        { "丑", [ "亥" ] },
        { "寅", [ "申" ] },
        { "卯", [ "巳" ] },
        { "辰", [ "寅" ] },
        { "巳", [ "亥" ] },
        { "午", [ "申" ] },
        { "未", [ "巳" ] },
        { "申", [ "寅" ] },
        { "酉", [ "亥" ] },
        { "戌", [ "申" ] },
        { "亥", [ "巳" ] }
    };
    #endregion

    #region 禄神
    public static readonly Dictionary<string, string[]> LuShengDict = new()
    {
        //日干 - 四柱、流运
        //甲-寅，乙-卯，丙-巳，丁-午，戊-巳，己-午，庚-申，辛-酉，壬-亥，癸-子
        { "甲", [ "寅" ] },
        { "乙", [ "卯" ] },
        { "丙", [ "巳" ] },
        { "丁", [ "午" ] },
        { "戊", [ "巳" ] },
        { "己", [ "午" ] },
        { "庚", [ "申" ] },
        { "辛", [ "酉" ] },
        { "壬", [ "亥" ] },
        { "癸", [ "子" ] }
    };
    #endregion

    #region 将星
    public static readonly Dictionary<string, string[]> JiangXingDict = new()
    {
        //年支 - 月、日、时、流运
        //日支 - 年、月、时、流运
        //子-子，丑-酉，寅-午，卯-卯，辰-子，巳-酉，午-午，未-卯，申-子，酉-酉，戌-午，亥-卯
        { "子", [ "子" ] },
        { "丑", [ "酉" ] },
        { "寅", [ "午" ] },
        { "卯", [ "卯" ] },
        { "辰", [ "子" ] },
        { "巳", [ "酉" ] },
        { "午", [ "午" ] },
        { "未", [ "卯" ] },
        { "申", [ "子" ] },
        { "酉", [ "酉" ] },
        { "戌", [ "午" ] },
        { "亥", [ "卯" ] }
    };
    #endregion

    #region 学堂
    public static readonly Dictionary<string, string[]> XueTangDict = new()
    {
        //年五行 - 月、日、时、流运
        //金-巳，木-亥，水-申，土-申，火-寅
        { "金", [ "巳" ] },
        { "木", [ "亥" ] },
        { "水", [ "申" ] },
        { "土", [ "申" ] },
        { "火", [ "寅" ] }
    };
    #endregion

    #region 正学堂
    public static readonly Dictionary<string, string[]> ZhengXueTangDict = new()
    {
        //年五行 - 月、日、时、流运
        //金-辛巳，木-己亥，水-甲申，土-戊申，火-丙寅
        { "金", [ "辛巳" ] },
        { "木", [ "己亥" ] },
        { "水", [ "甲申" ] },
        { "土", [ "戊申" ] },
        { "火", [ "丙寅" ] }
    };
    #endregion

    #region 词馆
    public static readonly Dictionary<string, string[]> CiGuanDict = new()
    {
        //年五行 - 月、日、时、流运
        //金-申，木-寅，水-亥，土-亥，火-巳
        { "金", [ "申" ] },
        { "木", [ "寅" ] },
        { "水", [ "亥" ] },
        { "土", [ "亥" ] },
        { "火", [ "巳" ] }
    };
    #endregion

    #region 正词馆
    public static readonly Dictionary<string, string[]> ZhengCiGuanDict = new()
    {
        //年五行 - 月、日、时、流运
        //金-壬申，木-庚寅，水-癸亥，土-丁亥，火-乙巳
        { "金", [ "壬申" ] },
        { "木", [ "庚寅" ] },
        { "水", [ "癸亥" ] },
        { "土", [ "丁亥" ] },
        { "火", [ "乙巳" ] }
    };
    #endregion

    #region 天医
    public static readonly Dictionary<string, string[]> TianYiDict = new()
    {
        //月支 - 四柱、流运
        //子-亥，丑-子，寅-丑，卯-寅，辰-卯，巳-辰，午-巳，未-午，申-未，酉-申，戌-酉，亥-戌
        { "子", [ "亥" ] },
        { "丑", [ "子" ] },
        { "寅", [ "丑" ] },
        { "卯", [ "寅" ] },
        { "辰", [ "卯" ] },
        { "巳", [ "辰" ] },
        { "午", [ "巳" ] },
        { "未", [ "午" ] },
        { "申", [ "未" ] },
        { "酉", [ "申" ] },
        { "戌", [ "酉" ] },
        { "亥", [ "戌" ] }
    };
    #endregion

    #region 金舆
    public static readonly Dictionary<string, string[]> JinYuDict = new()
    {
        //年日干 - 四柱、流运
        //甲-辰，乙-巳，丙-未，丁-申，戊-未，己-申，庚-戌，辛-亥，壬-丑，癸-寅
        { "甲", [ "辰" ] },
        { "乙", [ "巳" ] },
        { "丙", [ "未" ] },
        { "丁", [ "申" ] },
        { "戊", [ "未" ] },
        { "己", [ "申" ] },
        { "庚", [ "戌" ] },
        { "辛", [ "亥" ] },
        { "壬", [ "丑" ] },
        { "癸", [ "寅" ] }
    };
    #endregion

    #region 桃花
    public static readonly Dictionary<string, string[]> TaoHuaDict = new()
    {
        //年支 - 月、日、时、流运
        //日支 - 年、月、时、流运
        //子-酉，丑-午，寅-卯，卯-子，辰-酉，巳-午，午-卯，未-子，申-酉，酉-午，戌-卯，亥-子
        { "子", [ "酉" ] },
        { "丑", [ "午" ] },
        { "寅", [ "卯" ] },
        { "卯", [ "子" ] },
        { "辰", [ "酉" ] },
        { "巳", [ "午" ] },
        { "午", [ "卯" ] },
        { "未", [ "子" ] },
        { "申", [ "酉" ] },
        { "酉", [ "午" ] },
        { "戌", [ "卯" ] },
        { "亥", [ "子" ] }
    };
    #endregion

    #region 红鸾
    public static readonly Dictionary<string, string[]> HongLuanDict = new()
    {
        //年支 - 月、日、时、流运
        //子-卯，丑-寅，寅-丑，卯-子，辰-亥，巳-戌，午-酉，未-申，申-未，酉-午，戌-巳，亥-辰
        { "子", [ "卯" ] },
        { "丑", [ "寅" ] },
        { "寅", [ "丑" ] },
        { "卯", [ "子" ] },
        { "辰", [ "亥" ] },
        { "巳", [ "戌" ] },
        { "午", [ "酉" ] },
        { "未", [ "申" ] },
        { "申", [ "未" ] },
        { "酉", [ "午" ] },
        { "戌", [ "巳" ] },
        { "亥", [ "辰" ] }
    };
    #endregion

    #region 天喜
    public static readonly Dictionary<string, string[]> TianXiDict = new()
    {
        //年支 - 月、日、时、流运
        //子-酉，丑-申，寅-未，卯-午，辰-巳，巳-辰，午-卯，未-寅，申-丑，酉-子，戌-亥，亥-戌
        { "子", [ "酉" ] },
        { "丑", [ "申" ] },
        { "寅", [ "未" ] },
        { "卯", [ "午" ] },
        { "辰", [ "巳" ] },
        { "巳", [ "辰" ] },
        { "午", [ "卯" ] },
        { "未", [ "寅" ] },
        { "申", [ "丑" ] },
        { "酉", [ "子" ] },
        { "戌", [ "亥" ] },
        { "亥", [ "戌" ] }
    };
    #endregion

    #region 金神
    public static readonly string[] JinShenDict =
    [
        //日柱，时柱
        //乙丑，癸酉，己巳
        "乙丑",
        "癸酉",
        "己巳"
    ];
    #endregion

    #region 拱禄
    public static readonly Dictionary<string, string[]> GongLuDict = new()
    {
        //时干支 - 日干支
        //癸丑-癸亥，癸亥-癸丑，丁未-丁巳，己巳-己未，戊午-戊辰
        { "癸丑", [ "癸亥" ] },
        { "癸亥", [ "癸丑" ] },
        { "丁未", [ "丁巳" ] },
        { "己巳", [ "己未" ] },
        { "戊午", [ "戊辰" ] }
    };
    #endregion

    #region 天转
    public static readonly Dictionary<string, string[]> TianZhuanDict = new()
    {
        //月支 - 日
        //寅-乙卯，卯-乙卯，辰-乙卯，巳-丙午，午-丙午，未-丙午，申-辛酉，酉-辛酉，戌-辛酉，亥-壬子，子-壬子，丑-壬子
        { "寅", [ "乙卯" ] },
        { "卯", [ "乙卯" ] },
        { "辰", [ "乙卯" ] },
        { "巳", [ "丙午" ] },
        { "午", [ "丙午" ] },
        { "未", [ "丙午" ] },
        { "申", [ "辛酉" ] },
        { "酉", [ "辛酉" ] },
        { "戌", [ "辛酉" ] },
        { "亥", [ "壬子" ] },
        { "子", [ "壬子" ] },
        { "丑", [ "壬子" ] }
    };
    #endregion

    #region 地转
    public static readonly Dictionary<string, string[]> DiZhuanDict = new()
    {
        //月支 - 日
        //寅-辛卯，卯-辛卯，辰-辛卯，巳-戊午，午-戊午，未-戊午，申-癸酉，酉-癸酉，戌-癸酉，亥-丙子，子-丙子，丑-丙子
        { "寅", [ "辛卯" ] },
        { "卯", [ "辛卯" ] },
        { "辰", [ "辛卯" ] },
        { "巳", [ "戊午" ] },
        { "午", [ "戊午" ] },
        { "未", [ "戊午" ] },
        { "申", [ "癸酉" ] },
        { "酉", [ "癸酉" ] },
        { "戌", [ "癸酉" ] },
        { "亥", [ "丙子" ] },
        { "子", [ "丙子" ] },
        { "丑", [ "丙子" ] }
    };
    #endregion

    #region 丧门
    public static readonly Dictionary<string, string[]> SangMenDict = new()
    {
        //年支 - 月、日、时、流运
        //子-寅，丑-卯，寅-辰，卯-巳，辰-午，巳-未，午-申，未-酉，申-戌，酉-亥，戌-子，亥-丑
        { "子", [ "寅" ] },
        { "丑", [ "卯" ] },
        { "寅", [ "辰" ] },
        { "卯", [ "巳" ] },
        { "辰", [ "午" ] },
        { "巳", [ "未" ] },
        { "午", [ "申" ] },
        { "未", [ "酉" ] },
        { "申", [ "戌" ] },
        { "酉", [ "亥" ] },
        { "戌", [ "子" ] },
        { "亥", [ "丑" ] }
    };
    #endregion

    #region 吊客
    public static readonly Dictionary<string, string[]> DiaoKeDict = new()
    {
        //年支 - 月、日、时、流运
        //子-戌，丑-亥，寅-子，卯-丑，辰-寅，巳-卯，午-辰，未-巳，申-午，酉-未，戌-申，亥-酉
        { "子", [ "戌" ] },
        { "丑", [ "亥" ] },
        { "寅", [ "子" ] },
        { "卯", [ "丑" ] },
        { "辰", [ "寅" ] },
        { "巳", [ "卯" ] },
        { "午", [ "辰" ] },
        { "未", [ "巳" ] },
        { "申", [ "午" ] },
        { "酉", [ "未" ] },
        { "戌", [ "申" ] },
        { "亥", [ "酉" ] }
    };
    #endregion

    #region 披麻
    public static readonly Dictionary<string, string[]> PiMaDict = new()
    {
        //年支 - 月、日、时、流运
        //子-酉，丑-戌，寅-亥，卯-子，辰-丑，巳-寅，午-卯，未-辰，申-巳，酉-午，戌-未，亥-申
        { "子", [ "酉" ] },
        { "丑", [ "戌" ] },
        { "寅", [ "亥" ] },
        { "卯", [ "子" ] },
        { "辰", [ "丑" ] },
        { "巳", [ "寅" ] },
        { "午", [ "卯" ] },
        { "未", [ "辰" ] },
        { "申", [ "巳" ] },
        { "酉", [ "午" ] },
        { "戌", [ "未" ] },
        { "亥", [ "申" ] }
    };
    #endregion

    #region 八专
    public static readonly string[] BaZhuanDict =
    [
        //日柱
        //戊戌，丁未，癸丑，甲寅，乙卯，己未，庚申，辛酉
        "戊戌",
        "丁未",
        "癸丑",
        "甲寅",
        "乙卯",
        "己未",
        "庚申",
        "辛酉"
    ];
    #endregion

    #region 九丑
    public static readonly string[] JiuChouDict =
    [
        //日柱
        //己卯，壬午，戊子，辛卯，丁酉，己酉，壬子，戊午，辛酉
        "己卯",
        "壬午",
        "戊子",
        "辛卯",
        "丁酉",
        "己酉",
        "壬子",
        "戊午",
        "辛酉"
    ];
    #endregion

    #region 元辰
    public static readonly Dictionary<string, string[]> YuanChenDict = new()
    {
        //年支 - 月、日、时、流运
        //子-未，丑-申，寅-酉，卯-戌，辰-亥，巳-子，午-丑，未-寅，申-卯，酉-辰，戌-巳，亥-午
        //子-巳，丑-午，寅-未，卯-申，辰-酉，巳-戌，午-亥，未-子，申-丑，酉-寅，戌-卯，亥-辰
        { "子", [ "未", "巳" ] },
        { "丑", [ "申", "午" ] },
        { "寅", [ "酉", "未" ] },
        { "卯", [ "戌", "申" ] },
        { "辰", [ "亥", "酉" ] },
        { "巳", [ "子", "戌" ] },
        { "午", [ "丑", "亥" ] },
        { "未", [ "寅", "子" ] },
        { "申", [ "卯", "丑" ] },
        { "酉", [ "辰", "寅" ] },
        { "戌", [ "巳", "卯" ] },
        { "亥", [ "午", "辰" ] }
    };
    #endregion

    #region 魁罡
    public static readonly string[] KuiGangDict =
    [
        //日柱
        //庚辰，壬辰，戊戌，庚戌
        "庚辰",
        "壬辰",
        "戊戌",
        "庚戌"
    ];
    #endregion

    #region 孤辰
    public static readonly Dictionary<string, string[]> GuChenDict = new()
    {
        //年支 - 月、日、时、流运
        //子-寅，丑-寅，寅-巳，卯-巳，辰-巳，巳-申，午-申，未-申，申-亥，酉-亥，戌-亥，亥-寅
        { "子", [ "寅" ] },
        { "丑", [ "寅" ] },
        { "寅", [ "巳" ] },
        { "卯", [ "巳" ] },
        { "辰", [ "巳" ] },
        { "巳", [ "申" ] },
        { "午", [ "申" ] },
        { "未", [ "申" ] },
        { "申", [ "亥" ] },
        { "酉", [ "亥" ] },
        { "戌", [ "亥" ] },
        { "亥", [ "寅" ] }
    };
    #endregion

    #region 寡宿
    public static readonly Dictionary<string, string[]> GuaSuDict = new()
    {
        //年支 - 月、日、时、流运
        //子-戌，丑-戌，寅-丑，卯-丑，辰-丑，巳-辰，午-辰，未-辰，申-未，酉-未，戌-未，亥-戌
        { "子", [ "戌" ] },
        { "丑", [ "戌" ] },
        { "寅", [ "丑" ] },
        { "卯", [ "丑" ] },
        { "辰", [ "丑" ] },
        { "巳", [ "辰" ] },
        { "午", [ "辰" ] },
        { "未", [ "辰" ] },
        { "申", [ "未" ] },
        { "酉", [ "未" ] },
        { "戌", [ "未" ] },
        { "亥", [ "戌" ] }
    };
    #endregion

    #region 流霞
    public static readonly Dictionary<string, string[]> LiuXiaDict = new()
    {
        //日干 - 四柱、流运
        //甲-酉，乙-戌，丙-未，丁-申，戊-巳，己-午，庚-辰，辛-卯，壬-亥，癸-寅
        { "甲", [ "酉" ] },
        { "乙", [ "戌" ] },
        { "丙", [ "未" ] },
        { "丁", [ "申" ] },
        { "戊", [ "巳" ] },
        { "己", [ "午" ] },
        { "庚", [ "辰" ] },
        { "辛", [ "卯" ] },
        { "壬", [ "亥" ] },
        { "癸", [ "寅" ] }
    };
    #endregion

    #region 亡神
    public static readonly Dictionary<string, string[]> WangShenDict = new()
    {
        //年支 - 月、日、时、流运
        //日支 - 年、月、时、流运
        //子-亥，丑-申，寅-巳，卯-寅，辰-亥，巳-申，午-巳，未-寅，申-亥，酉-申，戌-巳，亥-寅
        { "子", [ "亥" ] },
        { "丑", [ "申" ] },
        { "寅", [ "巳" ] },
        { "卯", [ "寅" ] },
        { "辰", [ "亥" ] },
        { "巳", [ "申" ] },
        { "午", [ "巳" ] },
        { "未", [ "寅" ] },
        { "申", [ "亥" ] },
        { "酉", [ "申" ] },
        { "戌", [ "巳" ] },
        { "亥", [ "寅" ] }
    };
    #endregion

    #region 劫煞
    public static readonly Dictionary<string, string[]> JieShaDict = new()
    {
        //年支 - 月、日、时、流运
        //日支 - 年、月、时、流运
        //子-巳，丑-寅，寅-亥，卯-申，辰-巳，巳-寅，午-亥，未-申，申-巳，酉-寅，戌-亥，亥-申
        { "子", [ "巳" ] },
        { "丑", [ "寅" ] },
        { "寅", [ "亥" ] },
        { "卯", [ "申" ] },
        { "辰", [ "巳" ] },
        { "巳", [ "寅" ] },
        { "午", [ "亥" ] },
        { "未", [ "申" ] },
        { "申", [ "巳" ] },
        { "酉", [ "寅" ] },
        { "戌", [ "亥" ] },
        { "亥", [ "申" ] }
    };
    #endregion

    #region 灾煞
    public static readonly Dictionary<string, string[]> ZaiShaDict = new()
    {
        //年支 - 月、日、时、流运
        //子-午，丑-卯，寅-子，卯-酉，辰-午，巳-卯，午-子，未-酉，申-午，酉-卯，戌-子，亥-酉
        { "子", [ "午" ] },
        { "丑", [ "卯" ] },
        { "寅", [ "子" ] },
        { "卯", [ "酉" ] },
        { "辰", [ "午" ] },
        { "巳", [ "卯" ] },
        { "午", [ "子" ] },
        { "未", [ "酉" ] },
        { "申", [ "午" ] },
        { "酉", [ "卯" ] },
        { "戌", [ "子" ] },
        { "亥", [ "酉" ] }
    };
    #endregion

    #region 血刃
    public static readonly Dictionary<string, string[]> XueRenDict = new()
    {
        //月支 - 四柱、流运
        //子-午，丑-子，寅-丑，卯-未，辰-寅，巳-申，午-卯，未-酉，申-辰，酉-戌，戌-巳，亥-亥
        { "子", [ "午" ] },
        { "丑", [ "子" ] },
        { "寅", [ "丑" ] },
        { "卯", [ "未" ] },
        { "辰", [ "寅" ] },
        { "巳", [ "申" ] },
        { "午", [ "卯" ] },
        { "未", [ "酉" ] },
        { "申", [ "辰" ] },
        { "酉", [ "戌" ] },
        { "戌", [ "巳" ] },
        { "亥", [ "亥" ] }
    };
    #endregion

    #region 飞刃
    public static readonly Dictionary<string, string[]> FeiRenDict = new()
    {
        //日干 - 四柱、流运
        //甲-酉，乙-申，丙-子，丁-亥，戊-子，己-亥，庚-卯，辛-寅，壬-午，癸-巳
        { "甲", [ "酉" ] },
        { "乙", [ "申" ] },
        { "丙", [ "子" ] },
        { "丁", [ "亥" ] },
        { "戊", [ "子" ] },
        { "己", [ "亥" ] },
        { "庚", [ "卯" ] },
        { "辛", [ "寅" ] },
        { "壬", [ "午" ] },
        { "癸", [ "巳" ] }
    };
    #endregion

    #region 羊刃
    public static readonly Dictionary<string, string[]> YangRenDict = new()
    {
        //日干 - 四柱、流运
        //甲-卯，乙-寅，丙-午，丁-巳，戊-午，己-巳，庚-酉，辛-申，壬-子，癸-亥
        { "甲", [ "卯" ] },
        { "乙", [ "寅" ] },
        { "丙", [ "午" ] },
        { "丁", [ "巳" ] },
        { "戊", [ "午" ] },
        { "己", [ "巳" ] },
        { "庚", [ "酉" ] },
        { "辛", [ "申" ] },
        { "壬", [ "子" ] },
        { "癸", [ "亥" ] }
    };
    #endregion

    #region 空亡
    //年干 - 月、日、时、流运
    //日干 - 年、月、时、流运
    //甲子---乙丑---丙寅---丁卯---戊辰---己巳---庚午---辛未---壬申---癸酉 (遇)戌 亥
    //甲戌---乙亥---丙子---丁丑---戊寅---己卯---庚辰---辛巳---壬午---癸未 (遇)申 酉
    //甲申---乙酉---丙戌---丁亥---戊子---己丑---庚寅---辛卯---壬辰---癸巳 (遇)午 未
    //甲午---乙未---丙申---丁酉---戊戌---己亥---庚子---辛丑---壬寅---癸卯 (遇)辰 巳
    //甲辰---乙巳---丙午---丁未---戊申---己酉---庚戌---辛亥---壬子---癸丑 (遇)寅 卯
    //甲寅---乙卯---丙辰---丁巳---戊午---己未---庚申---辛酉---壬戌---癸亥 (遇)子 丑
    public static readonly Dictionary<string, string[]> KongWangNianDict = new()
    {
        // 戌组
        { "甲子", [ "戌","亥" ] }, { "乙丑", [ "戌","亥" ] }, { "丙寅", [ "戌","亥" ] }, { "丁卯", [ "戌","亥" ] }, { "戊辰", [ "戌","亥" ] },
        { "己巳", [ "戌","亥" ] }, { "庚午", [ "戌","亥" ] }, { "辛未", [ "戌","亥" ] }, { "壬申", [ "戌","亥" ] }, { "癸酉", [ "戌","亥" ] },
    
        // 申组
        { "甲戌", [ "申" ,"酉"] }, { "乙亥", [ "申" ,"酉"] }, { "丙子", [ "申" ,"酉"] }, { "丁丑", [ "申" ,"酉"] }, { "戊寅", [ "申" ,"酉"] },
        { "己卯", [ "申" ,"酉"] }, { "庚辰", [ "申" ,"酉"] }, { "辛巳", [ "申" ,"酉"] }, { "壬午", [ "申" ,"酉"] }, { "癸未", [ "申" ,"酉"] },
    
        // 午组
        { "甲申", [ "午" ,"未"] }, { "乙酉", [ "午" ,"未"] }, { "丙戌", [ "午" ,"未"] }, { "丁亥", [ "午" ,"未"] }, { "戊子", [ "午" ,"未"] },
        { "己丑", [ "午" ,"未"] }, { "庚寅", [ "午" ,"未"] }, { "辛卯", [ "午" ,"未"] }, { "壬辰", [ "午" ,"未"] }, { "癸巳", [ "午" ,"未"] },
    
        // 辰组
        { "甲午", [ "辰","巳" ] }, { "乙未", [ "辰","巳" ] }, { "丙申", [ "辰","巳" ] }, { "丁酉", [ "辰","巳" ] }, { "戊戌", [ "辰","巳" ] },
        { "己亥", [ "辰","巳" ] }, { "庚子", [ "辰","巳" ] }, { "辛丑", [ "辰","巳" ] }, { "壬寅", [ "辰","巳" ] }, { "癸卯", [ "辰","巳" ] },
    
        // 寅组
        { "甲辰", [ "寅","卯" ] }, { "乙巳", [ "寅","卯" ] }, { "丙午", [ "寅","卯" ] }, { "丁未", [ "寅","卯" ] }, { "戊申", [ "寅","卯" ] },
        { "己酉", [ "寅","卯" ] }, { "庚戌", [ "寅","卯" ] }, { "辛亥", [ "寅","卯" ] }, { "壬子", [ "寅","卯" ] }, { "癸丑", [ "寅","卯" ] },
    
        // 子组
        { "甲寅", [ "子","丑" ] }, { "乙卯", [ "子","丑" ] }, { "丙辰", [ "子","丑" ] }, { "丁巳", [ "子","丑" ] }, { "戊午", [ "子","丑" ] },
        { "己未", [ "子","丑" ] }, { "庚申", [ "子","丑" ] }, { "辛酉", [ "子","丑" ] }, { "壬戌", [ "子","丑" ] }, { "癸亥", [ "子","丑" ] }
    };
    #endregion
    #endregion
}
