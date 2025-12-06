using System;
using BitDAO.Utils.Calendar;

namespace BitDAO.Utils.Daoism;

public class XiaoLiuRen
{
    public static string[] CalculateByDateTime(DateTime _time, bool _useNineStars = false)
    {
        BaZi _bazi = new(NongLiTime.FromStandardTime(new StandardTime(_time)));

        int _number1 = _bazi.NongLiTime.Yue;
        int _number2 = _bazi.NongLiTime.Ri;
        int _number3 = _bazi.ShiZhiIndex + 1;

        return CalculateByThreeNumber(_number1, _number2, _number3);
    }

    public static string[] CalculateByThreeAlphabet(char _word1, char _word2, char _word3, bool _useNineStars = false)
    {
        int _number1 = (int)Char.ToLower(_word1) - 96;
        int _number2 = (int)Char.ToLower(_word2) - 96;
        int _number3 = (int)Char.ToLower(_word3) - 96;

        return CalculateByThreeNumber(_number1, _number2, _number3);
    }

    public static string[] CalculateByThreeNumber(int _number1, int _number2, int _number3, bool _useNineStars = false)
    {
        if (_useNineStars)
        {
            int _result1 = (_number1 - 1) % 9;
            int _result2 = (_number1 + _number2 - 2) % 9;
            int _result3 = (_number1 + _number2 + _number3 - 3) % 9;

            //Console.WriteLine($"{_number1},{_number2},{_number3} -> {_result1},{_result2},{_result3}");

            return [LiuRen9Data[_result1][0], LiuRen9Data[_result2][0], LiuRen9Data[_result3][0]];
        }
        else
        {
            int _result1 = (_number1 - 1) % 6;
            int _result2 = (_number1 + _number2 - 2) % 6;
            int _result3 = (_number1 + _number2 + _number3 - 3) % 6;

            return [LiuRen6Data[_result1][0], LiuRen6Data[_result2][0], LiuRen6Data[_result3][0]];

        }
    }

    private static readonly string[][] LiuRen6Data = [
        ["大安", "木", "震", "正东", "长期,缓慢,稳定", "三清祖师"],
        ["留连", "木", "巽", "西南", "停止,反复,复杂", "文昌帝君"],
        ["速喜", "水", "离", "正南", "惊喜,快速,突然", "九天应元雷声普化天尊"],
        ["赤口", "金", "兑", "正西", "争斗,凶恶,伤害", "雷部诸将"],
        ["小吉", "火", "坎", "正北", "起步,不多,尚可", "真武大帝"],
        ["空亡", "土", "中", "正中", "失去,虚伪,空想", "玉皇大帝"]];

    private static readonly string[][] LiuRen9Data = [
        ["大安", "木", "震", "正东", "长期,缓慢,稳定", "三清祖师"],
        ["留连", "木", "巽", "西南", "停止,反复,复杂", "文昌帝君"],
        ["速喜", "水", "离", "正南", "惊喜,快速,突然", "九天应元雷声普化天尊"],
        ["赤口", "金", "兑", "正西", "争斗,凶恶,伤害", "雷部诸将"],
        ["小吉", "火", "坎", "正北", "起步,不多,尚可", "真武大帝"],
        ["空亡", "土", "中", "正中", "失去,虚伪,空想", "玉皇大帝"],
        ["病符", "土", "坤", "西南", "病态,异常,治疗", "后土娘娘"],
        ["桃花", "土", "艮", "东北", "欲望,牵绊,异性", "城隍神"],
        ["天德", "金", "乾", "西北", "贵人,上司,高远", "紫薇大帝"]];
}
