using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    }
