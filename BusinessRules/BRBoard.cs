//******************************************************************
//*  作    者：占偉林
//*  功能說明：系統維護-公布欄
//*  創建日期：2009/10/16
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using CSIPCommonModel.BaseItem;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Utility;

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// 公布欄業務處理類
    /// </summary>
    public class BRBoard : CSIPCommonModel.BusinessRules.BRBase<EntityBoard>
    {
        #region 查詢公布欄訊息的sql
        //2021/03/17_Ares_Stanley-DB名稱改為變數
        private const string SEL_BOARD = @"select A.code,A.subject,A.content,B.USER_NAME,A.important_status " +
                    "from board A,(select distinct user_id,User_name from {0}.dbo.M_USER) as  B " +
                    "where A.user_id = B.USER_ID and A.board_status='Y' order by 1 desc";
        private const string SEL_BOARD_EDIT = @"select A.code,A.subject,A.content,B.USER_NAME,A.important_status,A.board_status " +
                    "from board A,(select distinct user_id,User_name from {0}.dbo.M_USER) as  B " +
                    "where A.user_id = B.USER_ID order by 1 desc";
        private const string UPD_BOARD = @"update board set subject = @subject,[content] = @content,"+
                    "board_status = @board_status,important_status = @important_status " +
                    "where code = @code";
        #endregion 

        /// <summary>
        /// 查詢公布欄訊息
        /// </summary>
        /// <returns></returns>
        public static DataTable SearchBoard(int intPageIndex, int intPageSize, 
                ref int intTotolCount, ref string strMsgID)
        {
            //2021/03/17_Ares_Stanley-DB名稱改為變數
            string strSql = string.Format(SEL_BOARD, UtilHelper.GetAppSettings("DB_CSIP"));
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = strSql;

            DataTable dtblBoard = new DataTable();
            try
            {
                dtblBoard = BRBoard.SearchOnDataSet(sqlcmd,intPageIndex,intPageSize,ref intTotolCount).Tables[0];
                return dtblBoard;
            }
            catch (Exception exp)
            {
                BRBoard.SaveLog(exp);
                strMsgID = "00_00000000_000";
                return null;
            }
        }
        
        /// <summary>
        /// 查詢公布欄訊息
        /// </summary>
        /// <returns></returns>
        public static DataTable SearchBoardEdit(int intPageIndex, int intPageSize,
                ref int intTotolCount, ref string strMsgID)
        {
            //2021/03/17_Ares_Stanley-DB名稱改為變數
            string strSql = string.Format(SEL_BOARD_EDIT, UtilHelper.GetAppSettings("DB_CSIP"));
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = strSql;

            DataTable dtblBoard = new DataTable();
            try
            {
                dtblBoard = BRBoard.SearchOnDataSet(sqlcmd, intPageIndex, intPageSize, ref intTotolCount).Tables[0];
                return dtblBoard;
            }
            catch (Exception exp)
            {
                BRBoard.SaveLog(exp);
                strMsgID = "00_00000000_000";
                return null;
            }
        }

        /// <summary>
        /// 以Code取對應的公布欄訊息
        /// </summary>
        /// <returns></returns>
        public static EntitySet<EntityBoard> SearchByCode(string strCode)
        {
            try
            {
                SqlHelper Sql = new SqlHelper();
                Sql.AddCondition(EntityBoard.M_code, Operator.Equal, DataTypeUtils.String, strCode);
                EntitySet<EntityBoard> esBoard = (EntitySet<EntityBoard>)BRBoard.Search(Sql.GetFilterCondition());
                return esBoard;
            }
            catch (Exception exp)
            {
                BRBoard.SaveLog(exp);
                return null;
            }
        }

        /// <summary>
        /// 刪除公布欄訊息
        /// </summary>
        /// <returns></returns>
        public static bool DeleteBoardByCode(string strCode)
        {
            SqlHelper Sql = new SqlHelper();
            Sql.AddCondition(EntityBoard.M_code, Operator.Equal, DataTypeUtils.String, strCode);
            EntityBoard eBoard = new EntityBoard();
            return BRBoard.DeleteEntityByCondition(eBoard, Sql.GetFilterCondition());
        }
       
        /// <summary>
        /// 添加公布欄訊息
        /// </summary>
        /// <param name="eBoard">公布欄對象</param>
        /// <param name="strMsgID">返回的消息ID</param>
        /// <returns>返回執行結果：True--添加成功；False--添加失敗</returns>
        public static bool Add(EntityBoard eBoard,ref string strMsgID)
        {
            //* 添加記錄
            if (!BRBoard.AddNewEntity(eBoard))
            {
                strMsgID = "01_04020000_009";
                return false;
            }
            else
            {
                strMsgID = "01_04020000_008";
                return true;
            }
        }

        /// <summary>
        /// 修改公布欄訊息
        /// </summary>
        /// <param name="eBoard">公布欄對象</param>
        /// <param name="strMsgID">返回的消息ID</param>
        /// <returns>返回執行結果：True--添加成功；False--添加失敗</returns>
        public static bool Update(EntityBoard eBoard, ref string strMsgID)
        {
            SqlCommand sqlcmdBoard = new SqlCommand();
            sqlcmdBoard.CommandText = UPD_BOARD;
            SqlParameter parmSubject = new SqlParameter("@"+EntityBoard.M_subject, eBoard.subject);
            sqlcmdBoard.Parameters.Add(parmSubject);
            SqlParameter parmContent = new SqlParameter("@"+EntityBoard.M_content, eBoard.content);
            sqlcmdBoard.Parameters.Add(parmContent);
            SqlParameter parmBoard_status = new SqlParameter("@"+EntityBoard.M_board_status, eBoard.board_status);
            sqlcmdBoard.Parameters.Add(parmBoard_status);
            SqlParameter parmImportant_status = new SqlParameter("@"+EntityBoard.M_important_status, eBoard.important_status);
            sqlcmdBoard.Parameters.Add(parmImportant_status);
            SqlParameter parmCode = new SqlParameter("@"+EntityBoard.M_code, eBoard.code);
            sqlcmdBoard.Parameters.Add(parmCode);

            //* 修改記錄
            if (!BRBoard.Update(sqlcmdBoard))
            {
                strMsgID = "01_04020000_011";
                return false;
            }
            else
            {
                strMsgID = "01_04020000_010";
                return true;
            }
        }
    }
}
