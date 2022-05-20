//******************************************************************
//*  功能說明：系統公共方法類
//*  創建日期：2021/03/26
//*  修改記錄：


//*<author>            <time>            <TaskID>                <desc>
// ARES_LUKE           2021/03/26        20200031-CSIP EOS       因CI/CD單元測試，故將 KEYIN_GUI的系統公共方法類轉移至此FN
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CSIPKeyInGUI.EntityLayer;

namespace CSIPKeyInGUI.BusinessRules
{
    public class BRCommon 
    {
        /// <summary>
        /// 專案代號:20200031-CSIP EOS
        /// 功能說明:SQL Injection
        /// 作    者:Ares Luke 
        /// 修改時間:2021/03/09
        /// </summary>
        public static String EncodeForSQL(String str, Int32 layer = 1)
        {
            List<String> oldList = new List<String>();
            List<String> newList = new List<String>();
            List<String> typeList = new List<String>();
            if (layer == -1)
            {
                return str;
            }
            if (layer >= 0)
                AddReplaceList(
                    new String[] { "'" },
                    new String[] { "''" },
                    new String[] { "" },
                    ref oldList, ref newList, ref typeList);
            if (layer >= 1)
                AddReplaceList(
                    new String[] {
                    ";", ",", "?", "<", ">",
                    "(", ")", "@", "--", "=",
                    "+", "*", "&", "#", "%",
                    "$" },
                    new String[] {
                    "", "", "", "", "",
                    "", "", "", "", "",
                    "", "", "", "", "",
                    "" },
                    new String[] {
                    "", "", "", "", "",
                    "", "", "", "", "",
                    "", "", "", "", "",
                    "" },
                    ref oldList, ref newList, ref typeList);
            if (layer >= 2)
                AddReplaceList(
                    new String[] {
                    "select", "insert", "delete from", "count", "drop table",
                    "truncate", "asc", "mid", "char", "xp_cmdshell",
                    "exec master", "net localgroup administrators", "and", "net user", "or",
                    "net", "delete", "drop", "script", "update",
                    "chr", "master", "declare", "exec" },
                    new String[] {
                    "", "", "", "", "",
                    "", "", "", "", "",
                    "", "", "", "", "",
                    "", "", "", "", "",
                    "", "", "", "" },
                    new String[] {
                    "1", "1", "1", "1", "1",
                    "1", "1", "1", "1", "1",
                    "1", "1", "1", "1", "1",
                    "1", "1", "1", "1", "1",
                    "1", "1", "1", "1" },
                    ref oldList, ref newList, ref typeList);
            for (int i = 0; i < oldList.Count && i < newList.Count && i < typeList.Count; i++)
                str = (typeList[i] == "" ? str.Replace(oldList[i], newList[i]) : Regex.Replace(str, oldList[i], newList[i], RegexOptions.IgnoreCase));
            return str;
        }

        private static void AddReplaceList(String[] oldArr, String[] newArr, String[] typeArr, ref List<String> oldList, ref List<String> newList, ref List<String> typeList)
        {
            oldList.AddRange(oldArr);
            newList.AddRange(newArr);
            typeList.AddRange(typeArr);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static string GetNum(string strSource)
        {
            string strRet = "0";
            Double dNum = 0;

            try
            {
                dNum = Convert.ToDouble(strSource) * 1000;
                strRet = dNum.ToString();
                return strRet;

            }
            catch
            {
                return "0";
            }

        }


        /// <summary>
        /// 按分割長度用指定字符串分割字符串
        /// </summary>
        /// <param name="strValue">字符串</param>
        /// <param name="startIndex">分割的長度</param>
        /// <param name="strInsertValue">分割符號</param>
        /// <returns>新的字符串</returns>
        public static string InsertAppointString(string strValue, int startIndex, string strInsertValue)
        {
            int intCount = strValue.Length;
            for (int i = 0; i < intCount / startIndex; i++)
            {
                if (i == 0 && intCount > startIndex)
                {
                    strValue = strValue.Insert(startIndex, strInsertValue);
                }
                else
                {
                    strValue = strValue.Insert(i * startIndex + i, strInsertValue);
                }
            }
            return strValue = strValue.Trim();
        }

        /// <summary>
        /// 字符串不為空左補字符
        /// </summary>
        /// <param name="strValue">字符串</param>
        /// <param name="intPadCount">數量</param>
        /// <param name="chType">左補的字符</param>
        /// <returns>字符串</returns>
        public static string GetPadLeftString(string strValue, int intPadCount, char chType)
        {
            if (strValue.Trim() != "" && strValue.Trim().ToUpper() != "X")
            {
                return strValue.PadLeft(intPadCount, chType);
            }
            return strValue;
        }


        /// <summary>
        /// 字符串不為空右補字符
        /// </summary>
        /// <param name="strValue">字符串</param>
        /// <param name="intPadCount">數量</param>
        /// <param name="chType">左補的字符</param>
        /// <returns>字符串</returns>
        public static string GetPadRightString(string strValue, int intPadCount, char chType)
        {
            if (strValue.Trim() != "" && strValue.Trim().ToUpper() != "X")
            {
                return strValue.PadRight(intPadCount, chType);
            }
            return strValue;
        }


        /// <summary>
        /// 將字符串半角转全角
        /// </summary>
        /// <param name="strInput">字符串</param>
        /// <returns>全角</returns>
        public static string ChangeToSBC(string strInput)
        {
            //*半角转全角
            char[] c = strInput.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] >= 33 && c[i] <= 126)
                {
                    c[i] = (char)(c[i] + 65248);
                }
                else if (c[i] == 32)
                {
                    c[i] = (char)12288;
                }
            }
            return new string(c);
        }

    }
}
