using System;
using System.Collections.Generic;
using System.Linq;


namespace BitDAO.Utils.Daoism;

public class ShiErZhangSheng
{
    private static readonly string[] YangGan = ["甲", "丙", "戊", "庚", "壬"];

    private static readonly Dictionary<string, string[]> Data = new()
    {
        // 甲木（阳）: 长生在亥，顺行
        {"甲", new[] { "亥", "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌" }},
        // 乙木（阴）: 长生在午，逆行
        { "乙", new[] { "午", "巳", "辰", "卯", "寅", "丑", "子", "亥", "戌", "酉", "申", "未" } },
        // 丙火（阳）: 长生在寅，顺行
        { "丙", new[] { "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥", "子", "丑" } },
        // 丁火（阴）: 长生在酉，逆行
        { "丁", new[] { "酉", "申", "未", "午", "巳", "辰", "卯", "寅", "丑", "子", "亥", "戌" } },
        // 戊土（阳）: 长生在寅，顺行（同丙火）
        { "戊", new[] { "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥", "子", "丑" } },
        // 己土（阴）: 长生在酉，逆行（同丁火）
        { "己", new[] { "酉", "申", "未", "午", "巳", "辰", "卯", "寅", "丑", "子", "亥", "戌" } },
        // 庚金（阳）: 长生在巳，顺行
        { "庚", new[] { "巳", "午", "未", "申", "酉", "戌", "亥", "子", "丑", "寅", "卯", "辰" } },
        // 辛金（阴）: 长生在子，逆行
        { "辛", new[] { "子", "亥", "戌", "酉", "申", "未", "午", "巳", "辰", "卯", "寅", "丑" } },
        // 壬水（阳）: 长生在申，顺行
        { "壬", new[] { "申", "酉", "戌", "亥", "子", "丑", "寅", "卯", "辰", "巳", "午", "未" } },
        // 癸水（阴）: 长生在卯，逆行
        { "癸", new[] { "卯", "寅", "丑", "子", "亥", "戌", "酉", "申", "未", "午", "巳", "辰" } }
    };

    public static string GetByGanZhi(string _gan, string _zhi) => DaoismUtils.ZhangSheng[GetIndexByGanZhi(_gan, _zhi)];
    public static int GetIndexByGanZhi(string _ganzhi) => GetIndexByGanZhi(_ganzhi[..1], _ganzhi[1..2]);
    public static int GetIndexByGanZhi(string _gan, string _zhi) => Array.IndexOf(Data[_gan], _zhi);
}