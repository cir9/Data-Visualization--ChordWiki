using Data_ChordWiki;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


public static class Utils
{


    static readonly char[] InvalidChars = Path.GetInvalidFileNameChars();

    //static readonly char[] FullPunctuations = "：，。；？".ToCharArray();


    /// <summary>
    /// 处理文件名称
    /// </summary>
    /// <param name="fileNameFormat">文件格式</param>
    /// <returns>返回合法的文件名</returns>
    public static string ParseStringToFileName(string fileNameFormat)
    {
        foreach (char c in InvalidChars)
            fileNameFormat = fileNameFormat.Replace(c.ToString(), "_");

        //foreach (char c in FullPunctuations)
        //    fileNameFormat = fileNameFormat.Replace(c.ToString(), "_");

        //去掉空格.
        //while (fileNameFormat.Contains(" ") == true)
        //    fileNameFormat = fileNameFormat.Replace(" ", "");

        //替换特殊字符.
        fileNameFormat = fileNameFormat.Replace("\t\n", "");

        //处理合法的文件名.
        StringBuilder rBuilder = new StringBuilder(fileNameFormat);
        foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
            rBuilder.Replace(rInvalidChar.ToString(), string.Empty);

        fileNameFormat = rBuilder.ToString();


        if (fileNameFormat.Length > 240)
            fileNameFormat = fileNameFormat[..240];

        return fileNameFormat;
    }

    public static TEnum ToEnum<TEnum>(this string str) where TEnum : struct
    {
        return Enum.Parse<TEnum>(str);
    }

    public static int GetCount(this Regex regex, string text)
    {
        return regex.Matches(text).Count;
    }
    public static string GetMatchAt(this Regex regex, string text, int index)
    {
        var match = regex.Match(text);
        if (!match.Success) return "";

        return regex.Match(text).Groups[index].Value;
    }


    public static V GetOrDefault<K, V>(this Dictionary<K, V> dict, K key, V defalutValue) where K: notnull
    {
        if (dict.TryGetValue(key, out V? result))
            return result;
        return defalutValue;

    }


    public static string ToDBC(this string input)
    {
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++) {
            if (c[i] == 12288) {
                c[i] = (char)32;
                continue;
            }
            if (c[i] > 65280 && c[i] < 65375)
                c[i] = (char)(c[i] - 65248);
        }
        return new string(c);
    }

}





public struct MonthTime
{
    public int month;
    public int year;

    public MonthTime(int month, int year)
    {
        this.month = month;
        this.year = year;
    }

    public MonthTime(string str)
    {
        if (str.Length == 0) {
            month = DateTime.Now.Month;
            year = DateTime.Now.Year;
            return;
        }

        int code = int.Parse(str);
        month = code % 100;
        year = code / 100;
    }

    public MonthTime Next() 
    {
        if (month >= 12) return new MonthTime(1, year + 1);
        return new MonthTime(month + 1, year);
    }

    public static bool operator !=(MonthTime left, MonthTime right)
    {
        return left.month != right.month || left.year != right.year;
    }

    public static bool operator ==(MonthTime left, MonthTime right)
    {
        return left.month == right.month && left.year == right.year;
    }

    public static bool operator <(MonthTime left, MonthTime right)
    {
        return left.GetHashCode() < right.GetHashCode();
    }

    public static bool operator >(MonthTime left, MonthTime right)
    {
        return left.GetHashCode() > right.GetHashCode();
    }

    public static bool operator <=(MonthTime left, MonthTime right)
    {
        return left.GetHashCode() <= right.GetHashCode();
    }

    public static bool operator >=(MonthTime left, MonthTime right)
    {
        return left.GetHashCode() >= right.GetHashCode();
    }


    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj?.GetHashCode() == GetHashCode();
    }

    public override int GetHashCode()
    {
        return year * 12 + month;
    }
}