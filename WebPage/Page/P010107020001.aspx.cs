//******************************************************************
//*  作    者：
//*  功能說明：上傳檔案(一般簽單及分期簽單)
//*  創建日期：2014/08/13
//*  修改紀錄：2021/04/01_Ares_Stanley-移除MicrosoftExcel
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************
//20160531 (U) by Tank, 簽單算總數的title移出來與分期共用
//20200721 (U) by Ray , 上傳主機/偽冒，分期簽單(空檔)-偽冒主機檔案內容清空
//20200806 (U) by Ray, strClentMsg 前面增加一般簽單/分期簽單
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
//using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;
using System.Text;
using System.Data.SqlClient;
using Framework.Common.IO;
using CSIPCommonModel.BusinessRules;
using DataTable = System.Data.DataTable;
using System.IO;
using Framework.Data;
using System.Windows.Forms;

public partial class P010107020001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    #endregion

    #region event
 
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            //* 綁定簽單類別DropDownList
            BindDropDownList();
        }
        base.strHostMsg += "";
        base.strClientMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
    }

    //產生檔案
    protected void btnExport_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
        string strBatchDate = this.dpBatchDate.Text;
        string strReceiveBatch = this.txtReceiveBatch.Text;
        string strSignType = this.dropSignType.SelectedValue;

        //檢查符合的[人工簽單主檔]中每一2key的資料否都已平帳(平帳註記=Y) ,且1/2key比對成功(KeyIn_MatchFlag=Y)
        if (!CSIPKeyInGUI.BusinessRules.BRASExport.CheckUploadData(strBatchDate, strReceiveBatch, strSignType, ref strMsgID))
        {
            //* 顯示未通過檢核訊息
            MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage(strMsgID);
            return;
        }

        //通過檢核執行產檔
        //20160531 (U) by Tank,從 BRASExport.cs 抓 ExportUploadFile 出來用
        //if (BRASExport.ExportUploadFile(strBatchDate, strReceiveBatch, strSignType, ref strMsgID, ref strServerPathFile))
        if (ExportUploadFile(strBatchDate, strReceiveBatch, strSignType, ref strMsgID, ref strServerPathFile))
        {
            //* 將[人工簽單批次資料檔] [處理註記]改成’02’己匯出成檔案,並依csip log機制寫入


            //* 顯示提示訊息：產檔成功
            string strFileName = string.Empty;
            string strLogKey = string.Empty;
            if (strSignType == "1")
            {
                //20141009  檔名更改為mpul
                //strFileName = "mupl.txt";
                //strLogKey = "mupl";
                strFileName = "mpul.txt";
                strLogKey = "mpul";
            }
            else
            {
                strFileName = "ipp" + strReceiveBatch + ".txt";
                strLogKey = "ipp";
            }
            this.Session["ServerFile"] = strServerPathFile;
            this.Session["ClientFile"] = strFileName;
            string urlString = @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);

            //* 依csip log機制寫入：交易名稱-人工簽單檔案匯出:[檔案名稱:XXXX]
            DataTable dtlog = CommonFunction.GetDataTable();
            CommonFunction.UpdateLog("", "檔案名稱：" + strFileName, "人工簽單檔案匯出", dtlog);
            CommonFunction.InsertCustomerLog(dtlog, eAgentInfo, strLogKey, "DB", (structPageInfo)Session["PageInfo"]);
        }
        //* 顯示產檔結果訊息
        MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
        //* 顯示端末訊息
        base.strClientMsg += MessageHelper.GetMessage(strMsgID);

        //*更新[人工簽單批次資料檔] [處理註記]改成"02"(己匯出成檔案)
        CSIPKeyInGUI.BusinessRules.BRASExport.Update_ASBD_Process_Flag(strBatchDate, strReceiveBatch, strSignType, "02", ref strMsgID);
        //* 顯示端末訊息
        base.strClientMsg += MessageHelper.GetMessage(strMsgID);
    }

    #endregion event

    /// <summary>
    /// 產生上傳檔案(一般簽單及分期簽單)
    /// </summary>
    /// <param name="strBatch_Date">編批日期</param>
    /// <param name="strReceive_Batch">收件批次</param>
    /// <param name="strSign_Type">簽單類別</param>
    /// <param name="strMsgID">返回的錯誤ID號</param>
    /// <param name="strPathFile">服務器端生成的TXT文檔路徑</param>
    public bool ExportUploadFile(string strBatch_Date, string strReceive_Batch, string strSign_Type, ref string strMsgID, ref string strPathFile)
    {
        try
        {
            StringBuilder sbCont = new StringBuilder();
            DataSet ds = getData_UploadFile(strBatch_Date, strReceive_Batch, strSign_Type, ref strMsgID);
            if (ds == null)
                return false;

            DataTable dtRow2 = ds.Tables["Row2Data"];
            DataTable dtRow3 = ds.Tables["Row3Data"];

            //* 檢查目錄，并刪除以前的文檔資料
            CSIPKeyInGUI.BusinessRules.BRExcel_File.CheckDirectory(ref strPathFile);

            //20160531 (U) by Tank, 簽單算總數的title移出來與分期共用
            //20140922 第一碼固定帶"0"，2-7碼日期請帶系統日(產檔當日)，格式：yyMMdd
            //西元年轉民國年 yyyMMdd 共7碼，不足左邊補0
            //string strBatch_Date_TW = BRASExport.ToTWDateString(DateTime.Parse(strBatch_Date));
            string strBatch_Date_TW = CSIPKeyInGUI.BusinessRules.BRASExport.ToTWDateString(DateTime.Now).Substring(1, 6);

            //封皮總筆數(查詢總筆數) 長度為5碼
            string strRowsCount = dtRow2.Rows.Count.ToString().PadLeft(5, '0');

            //20140925  帶上傳檔第一碼為2的加總
            //內頁總筆數(加總[人工簽單批次資料檔].[收件-總筆數]) 長度為8碼
            string strSumRTC = dtRow2.Compute("Sum(Receive_Total_Count)", "").ToString().PadLeft(8, '0');

            //20140930  總金額為上傳檔第一碼為2的加總NET值(請款加總 - 退款加總)
            //總金額(加總[人工簽單批次資料檔].[收件-總筆數]) 長度為12碼
            //string strSumRTA = Convert.ToInt64(dtRow2.Compute("Sum(Receive_Total_AMT)", "")).ToString().PadLeft(12, '0');
            Int64 i40 = Convert.ToInt64(dtRow3.Compute("Sum(AMT)", "Receipt_Type='40'") == DBNull.Value ? 0 : dtRow3.Compute("Sum(AMT)", "Receipt_Type='40'"));   //明細資料請款加總
            Int64 i41 = Convert.ToInt64(dtRow3.Compute("Sum(AMT)", "Receipt_Type='41'") == DBNull.Value ? 0 : dtRow3.Compute("Sum(AMT)", "Receipt_Type='41'"));   //明細資料退款加總
            //20141008  計算後取絕對值
            //string strSumRTA = Convert.ToString(i40 - i41).PadLeft(12, '0');
            string strSumRTA = Convert.ToString(Math.Abs(i40 - i41)).PadLeft(12, '0');

            //20140922 第一碼固定帶"0"，2-7碼日期請帶系統日(產檔當日)，格式：yyMMdd
            //表頭
            string strHead = "0" + strBatch_Date_TW + strReceive_Batch + strRowsCount + strSumRTC + strSumRTA + "00";
            sbCont.AppendLine(strHead.PadRight(60, ' '));

            if (strSign_Type == "1")
            {
                #region 一般簽單
                //內容
                foreach (DataRow dr2 in dtRow2.Rows)
                {
                    //第二列資料
                    //批號 長度為3碼
                    string strBatch_NO = dr2["Batch_NO"].ToString().PadLeft(3, '0');
                    //該批總筆數 長度為3碼
                    string strReceive_Total_Count = dr2["Receive_Total_Count"].ToString().PadLeft(3, '0');
                    //該批總金額 長度為9碼
                    string strReceive_Total_AMT = Convert.ToInt64(dr2["Receive_Total_AMT"]).ToString().PadLeft(9, '0');
                    //商店代號 長度為10碼
                    string strShop_ID = dr2["Shop_ID"].ToString().PadLeft(10, '0');

                    //組合第二列資料
                    string strRow2 = "100" + strBatch_NO + strReceive_Total_Count + strReceive_Total_AMT + "00" + strShop_ID;
                    sbCont.AppendLine(strRow2.PadRight(60, ' '));

                    //20141014  上傳檔序號另外編碼，由1開始連續累加
                    int iSN = 1;

                    //第三列資料 (明細)
                    foreach (DataRow dr3 in dtRow3.Select("Batch_NO=" + dr2["Batch_NO"].ToString()))
                    {
                        //序號 長度為3碼
                        //20141014  上傳檔序號另外編碼，由1開始連續累加
                        //string strSN = dr3["SN"].ToString().PadLeft(3, '0');
                        string strSN = iSN.ToString().PadLeft(3, '0');
                        iSN++;
                        //卡號 長度為16碼
                        string strCard_No = dr3["Card_No"].ToString().PadLeft(16, '0');
                        //交易金額 長度為7碼
                        string strAMT = Convert.ToInt64(dr3["AMT"]).ToString().PadLeft(7, '0');
                        //請退款 長度為2碼
                        string strReceipt_Type = dr3["Receipt_Type"].ToString().PadLeft(2, '0');
                        //交易日期(西元年:MMDDYY) 長度為6碼
                        string strTran_Date = DateTime.ParseExact(dr3["Tran_Date"].ToString(), "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToString("MMddyy");
                        //授權號碼 長度為6碼
                        string strAuth_Code = dr3["Auth_Code"].ToString().PadLeft(6, '0');

                        //組合第三列資料
                        string strRow3 = "200" + strBatch_NO + strSN + strCard_No + strAMT + "00" + strReceipt_Type + strTran_Date + strAuth_Code;
                        sbCont.AppendLine(strRow3.PadRight(60, ' '));
                    }
                }
                //20141009  檔名更改為mpul
                //strPathFile = strPathFile + @"\mupl_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                strPathFile = strPathFile + @"\mpul_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                #endregion 一般簽單
            }
            else
            {
                #region 分期簽單
                foreach (DataRow dr2 in dtRow2.Rows)
                {
                    //H(第一列)資料
                    //商店代號 長度為10碼
                    string strShop_ID = dr2["Shop_ID"].ToString().PadLeft(10, '0');
                    //批號 長度為5碼
                    string strBatch_NO = dr2["Batch_NO"].ToString().PadLeft(5, '0');
                    //該批總筆數 長度為5碼
                    string strReceive_Total_Count = dr2["Receive_Total_Count"].ToString().PadLeft(5, '0');
                    //該批總金額 長度為9碼
                    string strReceive_Total_AMT = Convert.ToInt64(dr2["Receive_Total_AMT"]).ToString().PadLeft(9, '0');

                    //組合第一列資料
                    string strRow2 = "H" + strShop_ID + strBatch_NO + strReceive_Total_Count + strReceive_Total_AMT + strBatch_Date.Replace("/", "") + "P";
                    sbCont.AppendLine(strRow2.PadRight(120, ' '));

                    //B(第二列)資料 (明細)
                    foreach (DataRow dr3 in dtRow3.Select("Shop_ID=" + dr2["Shop_ID"].ToString() + " AND Batch_NO=" + dr2["Batch_NO"].ToString()))
                    {
                        //卡號 長度為16碼
                        string strCard_No = dr3["Card_No"].ToString().PadLeft(16, '0');
                        //分期數 長度為2碼
                        string strINST = dr3["Installment_Periods"].ToString().Trim().PadLeft(2, '0');
                        //產品別 長度為3碼
                        string strProduct_Type = dr3["Product_Type"].ToString().PadLeft(3, '0');
                        //分期總價 長度為8碼
                        string strAMT = Convert.ToInt64(dr3["AMT"]).ToString().PadLeft(8, '0');
                        //交易日期(西元年:MMDDYY) 長度為8碼
                        string strTran_Date = dr3["Tran_Date"].ToString();
                        //授權號碼 長度為6碼
                        string strAuth_Code = dr3["Auth_Code"].ToString().PadLeft(6, '0');

                        //組合第二列資料
                        string strRow3 = "B" + strShop_ID + strBatch_Date.Replace("/", "") + strBatch_NO + "1" + strCard_No + strINST + strProduct_Type + strAMT
                                        + "0000" + strTran_Date + strAuth_Code + "00";
                        sbCont.AppendLine(strRow3.PadRight(120, ' '));
                    }
                }
                strPathFile = strPathFile + @"\ipp" + strReceive_Batch + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                #endregion 分期簽單
            }
            FileTools.Create(strPathFile, sbCont.ToString().Remove(sbCont.ToString().LastIndexOf("\r\n")));
            strMsgID = "01_01070200_005";
            return true;
        }
        catch (Exception ex)
        {
            strMsgID = "01_01070200_004";
            Logging.Log(ex, LogLayer.BusinessRule);
            throw ex;
        }
    }

    /// <summary>
    /// 產生上傳檔案(一般簽單及分期簽單)時，查詢資料
    /// </summary>
    /// <param name="strBatch_Date">編批日期</param>
    /// <param name="strReceive_Batch">收件批次</param>
    /// <param name="strSign_Type">簽單類別</param>
    /// <param name="strMsgID">返回的錯誤ID號</param>
    /// <returns>成功時：返回查詢結果；失敗時：null</returns>
    public static DataSet getData_UploadFile(string strBatch_Date, string strReceive_Batch, string strSign_Type, ref string strMsgID)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlcmSearchData = new SqlCommand();
            sqlcmSearchData.CommandType = CommandType.Text;

            if (strSign_Type == "1")
            {
                //一般簽單
                //* 查詢上傳檔案 一般簽單 資料
                //20141008 第二列總金額為NET值：(請款-退款)取絕對值
                //20140925 正常件才列出，並且以正常件重新計算總筆數及總金額
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmSearchData.CommandText = string.Format(@"--第二列為該[編批日期]，[收單批號]項下每一批號的資料
                                                SELECT Batch_NO, Shop_ID, count(*) AS Receive_Total_Count, 
                                                    ABS(SUM(CASE WHEN Receipt_Type = '40' THEN AMT ELSE -AMT END)) AS Receive_Total_AMT
                                                FROM (
	                                                SELECT Batch_NO, Shop_ID, AMT, Receipt_Type
	                                                FROM [{0}].[dbo].[Artificial_Signing_Detail]
	                                                WHERE KeyIn_Flag = '2'
		                                                AND Batch_Date = @Batch_Date
		                                                AND Receive_Batch = @Receive_Batch
		                                                AND Sign_Type = @Sign_Type
		                                                AND Case_Status = '0'
	                                                ) a
                                                GROUP BY Batch_NO, Shop_ID
                                                ORDER BY Batch_NO, Shop_ID;

                                                --第三列為第二列的明細資料
                                                SELECT Batch_NO, Shop_ID, SN, Card_No, AMT, Receipt_Type, Tran_Date, Auth_Code
                                                FROM [{0}].[dbo].[Artificial_Signing_Detail]
                                                WHERE KeyIn_Flag = '2'
	                                                AND Batch_Date = @Batch_Date
	                                                AND Receive_Batch = @Receive_Batch
	                                                AND Sign_Type = @Sign_Type
                                                    AND Case_Status = '0'
                                                ORDER BY Batch_NO, Shop_ID;", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            }
            else
            {
                //分期簽單
                //* 查詢上傳檔案 分期簽單 資料
                //20141008 第二列總金額為NET值：(請款-退款)取絕對值
                //20140925 正常件才列出，並且以正常件重新計算總筆數及總金額
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmSearchData.CommandText = string.Format(@"--第二列為該[編批日期]，[收單批號]項下每一批號的資料
                                                SELECT Batch_NO, Shop_ID, count(*) AS Receive_Total_Count, 
                                                    ABS(SUM(CASE WHEN Receipt_Type = '40' THEN AMT ELSE -AMT END)) AS Receive_Total_AMT
                                                FROM (
	                                                SELECT Batch_NO, Shop_ID, AMT, Receipt_Type
	                                                FROM [{0}].[dbo].[Artificial_Signing_Detail]
	                                                WHERE KeyIn_Flag = '2'
		                                                AND Batch_Date = @Batch_Date
		                                                AND Receive_Batch = @Receive_Batch
		                                                AND Sign_Type = @Sign_Type
		                                                AND Case_Status = '0'
	                                                ) a
                                                GROUP BY Batch_NO, Shop_ID
                                                ORDER BY Batch_NO, Shop_ID;

                                                --第三列為第二列的明細資料
                                                SELECT Batch_NO, Shop_ID, Card_No, AMT, Tran_Date, Installment_Periods, Product_Type, Auth_Code, Receipt_Type
                                                FROM [{0}].[dbo].[Artificial_Signing_Detail]
                                                WHERE KeyIn_Flag = '2'
	                                                AND Batch_Date = @Batch_Date
	                                                AND Receive_Batch = @Receive_Batch
	                                                AND Sign_Type = @Sign_Type
                                                    AND Case_Status = '0'
                                                ORDER BY Batch_NO, Shop_ID;", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            }
            //* 編批日期
            SqlParameter parmBatchDate = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
            sqlcmSearchData.Parameters.Add(parmBatchDate);
            //* 收件批次
            SqlParameter parmReceiveBatch = new SqlParameter("@Receive_Batch", strReceive_Batch);
            sqlcmSearchData.Parameters.Add(parmReceiveBatch);
            //* 簽單類別
            SqlParameter parmSignType = new SqlParameter("@Sign_Type", strSign_Type);
            sqlcmSearchData.Parameters.Add(parmSignType);

            //* 查詢數據
            DataSet dstSearchData = CSIPKeyInGUI.BusinessRules.BRASExport.SearchOnDataSet(sqlcmSearchData);
            if (null == dstSearchData)  //* 查詢數據失敗
            {
                strMsgID = "01_01070200_001";
                return null;
            }
            else
            {
                //* 查詢的數據不存在表示通過檢查回傳TRUE
                if (dstSearchData.Tables[0].Rows.Count == 0)
                {
                    strMsgID = "01_01070200_002";
                    return null;
                }
            }
            //* 查詢成功，有數據表示檢核失敗傳回FALSE及訊息
            dstSearchData.Tables[0].TableName = "Row2Data";
            dstSearchData.Tables[1].TableName = "Row3Data";
            strMsgID = "01_01070200_003";
            return dstSearchData;

            #endregion 依據Request查詢資料庫
        }
        catch (Exception exp)
        {
            CSIPKeyInGUI.BusinessRules.BRCompareRpt.SaveLog(exp);
            strMsgID = "01_01070200_001";
            return null;
        }
    }


    #region function
    /// <summary>
    /// 綁定簽單類別DropDownList控件
    /// </summary>
    private void BindDropDownList()
    {
        this.dropSignType.Items.Clear();
        this.dropSignType.Items.Add(new ListItem("一般簽單", "1"));
        this.dropSignType.Items.Add(new ListItem("分期簽單", "2"));
    }
    #endregion function
    /// <summary>
    /// 產生檔案並上傳FTP
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnExportFTP_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        string strServerPathFile = this.Server.MapPath(ConfigurationManager.AppSettings["ExportExcelFilePath"].ToString());
        string strBatchDate = this.dpBatchDate.Text;
        string strReceiveBatch = this.txtReceiveBatch.Text;
        string strSignType = this.dropSignType.SelectedValue;
        string MsgTMp = "";
        string txtMsg = "{0}:{1}:{2}:{3}";

        DataTable isUpdate = BRASExport.CheckBeforeUpload(strBatchDate, strReceiveBatch, strSignType, "02", ref strMsgID);
        DataTable isCount = BRASExport.confirmCount(strBatchDate, strReceiveBatch, strSignType, ref strMsgID);


        if (isCount == null)
        {
            base.strClientMsg += MessageHelper.GetMessage(strMsgID);
            return;
        }
        else
        {
            if (isCount.Rows[0][0].ToString() == "0")
            {
                isCount.Rows[0][1] = "0";
                isCount.Rows[0][2] = "0";
            }

            if (strSignType == "1")
            {
                MsgTMp = "是否產生一般簽單檔，並上傳主機及偽冒系統? \\n 一般：請款總筆數 :{0} ,請款總金額(元) :{1} ";
                txtMsg = string.Format(MsgTMp, isCount.Rows[0][1].ToString(), isCount.Rows[0][2].ToString());
            }
            else
            {
                MsgTMp = "是否產生分期簽單檔，並上傳主機及偽冒系統? \\n 分期：請款總筆數 :{0} ,請款總金額(元) :{1} ";
                txtMsg = string.Format(MsgTMp, isCount.Rows[0][1].ToString(), isCount.Rows[0][2].ToString());
            }


            if (isUpdate == null)
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
                return;
            }
            else if (isUpdate.Rows[0][0].ToString() == "0")
            {
                base.sbRegScript.Append(" if(confirm('" + txtMsg + "')) {$('#btnHidExportFTP').click(); }");
            }
            else
            {
                base.sbRegScript.Append("if(confirm('是否再次上傳檔案 ?')) {if(confirm('" + txtMsg + "')) {$('#btnHidExportFTP').click(); } }");
            }
        }
    }

    protected void btn_ExportFTP(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        string strServerPathFile = this.Server.MapPath(ConfigurationManager.AppSettings["ExportExcelFilePath"].ToString());
        string strBatchDate = this.dpBatchDate.Text;
        string strReceiveBatch = this.txtReceiveBatch.Text;
        string strSignType = this.dropSignType.SelectedValue;

        //檢查符合的[人工簽單主檔]中每一2key的資料否都已平帳(平帳註記=Y) ,且1/2key比對成功(KeyIn_MatchFlag=Y)
        if (!CSIPKeyInGUI.BusinessRules.BRASExport.CheckUploadData(strBatchDate, strReceiveBatch, strSignType, ref strMsgID))
        {
            //* 顯示未通過檢核訊息
            MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage(strMsgID);
            return;
        }

        //通過檢核執行產檔 並上傳FTP，與原流程相同

        // if (ExportUploadFile(strBatchDate, strReceiveBatch, strSignType, ref strMsgID, ref strServerPathFile))
        strMsgID = "01_01070200_004";

        //一次產出一般及分期，不管頁面設定
        if (ProcessJob(strBatchDate, strReceiveBatch, strSignType))

        {
            //* 將[人工簽單批次資料檔] [處理註記]改成’02’己匯出成檔案,並依csip log機制寫入

            strMsgID = "01_01070200_005";

        }


        //* 顯示產檔結果訊息
        MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
        //* 顯示端末訊息
        base.strClientMsg += MessageHelper.GetMessage(strMsgID);

        //*更新[人工簽單批次資料檔] [處理註記]改成"02"(己匯出成檔案)
        CSIPKeyInGUI.BusinessRules.BRASExport.Update_ASBD_Process_Flag(strBatchDate, strReceiveBatch, strSignType, "02", ref strMsgID);

        //* 顯示端末訊息
        base.strClientMsg += MessageHelper.GetMessage(strMsgID);

        // 20200806 (U) Ray, strClentMsg 前面增加一般簽單/分期簽單
        base.strClientMsg = this.dropSignType.SelectedItem.Text + base.strClientMsg;

        SendMail(strClientMsg, strClientMsg);

    }

    private void myDelFile(string sPath)
    {
        if (File.Exists(sPath))
        {
            File.Delete(sPath);
        }
    }
    /// <summary>
    /// 真正處理批次產檔及上傳FTP的地方     一次產出一般及分期
    /// </summary>
    /// <param name="strRunDate"></param>
    /// <param name="strRunBat"></param>
    /// <returns></returns>
    public bool ProcessJob(string strRunDate, string strRunBat, string strSignType)
    {
        bool result = true;
        string root = AppDomain.CurrentDomain.BaseDirectory + "\\" + ConfigurationManager.AppSettings["ExportExcelFilePath"].ToString();

        string strMsgID = "";
        string RPath = root;  //一般檔案產生後回傳檔名
        string FPath = root;  //一般檔案產生後回傳檔名  偽冒

        bool res = expFile(strRunDate, strRunBat, strSignType, ref strMsgID, ref RPath, ref FPath);
        if (!res)
        {
            result = false;
            // Logging.SaveLog(ELogLayer.UI, "產生檔案失敗 1批:" + res);
            Logging.Log("產生檔案失敗 1批:" + res,LogLayer.UI);
        }
        //發送 一般、分期主機
        bool isSend1 = SendToFTP("BaseToMain", RPath);
        //  發送 一般、分期偽冒
        bool isSend2 = SendToFTP("BaseToFake", FPath);
        if (!isSend1 || !isSend2)
        {
            result = false;
            // Logging.SaveLog(ELogLayer.UI, "上傳FTP失敗 主機:" + isSend1 + " 偽冒:" + isSend2);
            Logging.Log("上傳FTP失敗 主機:" + isSend1 + " 偽冒:" + isSend2,LogLayer.UI);
        }
        return result;
    }

    //依傳入批次產生一般及分期檔案
    private bool expFile(string strBatch_Date, string strReceive_Batch, string strSign_Type, ref string strMsgID, ref string strPathFile, ref string strFPathFile)
    {
        try
        {
            StringBuilder sbCont = new StringBuilder();
            DataSet ds = getData_UploadFile(strBatch_Date, strReceive_Batch, strSign_Type, ref strMsgID);

            string strBatch_Date_TW = BRASExport.ToTWDateString(DateTime.Now).Substring(1, 6);
            //要產出空檔
            if (ds == null)
            {
                //20140922 第一碼固定帶"0"，2-7碼日期請帶系統日(產檔當日)，格式：yyMMdd
                //表頭
                string strHeadN = "0" + strBatch_Date_TW + strReceive_Batch + "0".PadLeft(5, '0') + "0".PadLeft(8, '0') + "0".PadLeft(12, '0') + "00";
                sbCont.AppendLine(strHeadN.PadRight(60, ' '));
                strMsgID = "查無資料";
                string optStr = sbCont.ToString().Remove(sbCont.ToString().LastIndexOf("\r\n"));
                if (strSign_Type == "1")
                {
                    //一般主機名稱
                    strPathFile = strPathFile + @"\mpul.txt";
                    //偽冒主機名稱
                    strFPathFile = strFPathFile + @"\mpul" + strReceive_Batch + DateTime.Now.ToString("MMdd") + ".txt";
                }
                else
                {
                    //一般主機名稱
                    strPathFile = strPathFile + @"\ipp" + strReceive_Batch + ".txt";
                    //偽冒主機名稱
                    strFPathFile = strFPathFile + @"\ipp" + strReceive_Batch + DateTime.Now.ToString("MMdd") + ".txt";
                    //空檔清空內容即可
                    //2020.07.21 (U) Ray 取消清空內容
                    //optStr = "";
                }
                //增加寫入前清除舊檔
                myDelFile(strPathFile);
                myDelFile(strFPathFile);

                //寫入一般主機名稱
                FileTools.Create(strPathFile, optStr);
                //寫入偽冒主機名稱
                FileTools.Create(strFPathFile, optStr);

                return true;
            }

            DataTable dtRow2 = ds.Tables["Row2Data"];
            DataTable dtRow3 = ds.Tables["Row3Data"];

            //* 檢查目錄，并刪除以前的文檔資料
            //BRExcel_File.CheckDirectory(ref strPathFile);

            //20160531 (U) by Tank, 簽單算總數的title移出來與分期共用
            //20140922 第一碼固定帶"0"，2-7碼日期請帶系統日(產檔當日)，格式：yyMMdd
            //西元年轉民國年 yyyMMdd 共7碼，不足左邊補0
            //string strBatch_Date_TW = BRASExport.ToTWDateString(DateTime.Parse(strBatch_Date));


            //封皮總筆數(查詢總筆數) 長度為5碼
            string strRowsCount = dtRow2.Rows.Count.ToString().PadLeft(5, '0');

            //20140925  帶上傳檔第一碼為2的加總
            //內頁總筆數(加總[人工簽單批次資料檔].[收件-總筆數]) 長度為8碼
            string strSumRTC = dtRow2.Compute("Sum(Receive_Total_Count)", "").ToString().PadLeft(8, '0');

            //20140930  總金額為上傳檔第一碼為2的加總NET值(請款加總 - 退款加總)
            //總金額(加總[人工簽單批次資料檔].[收件-總筆數]) 長度為12碼
            //string strSumRTA = Convert.ToInt64(dtRow2.Compute("Sum(Receive_Total_AMT)", "")).ToString().PadLeft(12, '0');
            Int64 i40 = Convert.ToInt64(dtRow3.Compute("Sum(AMT)", "Receipt_Type='40'") == DBNull.Value ? 0 : dtRow3.Compute("Sum(AMT)", "Receipt_Type='40'"));   //明細資料請款加總
            Int64 i41 = Convert.ToInt64(dtRow3.Compute("Sum(AMT)", "Receipt_Type='41'") == DBNull.Value ? 0 : dtRow3.Compute("Sum(AMT)", "Receipt_Type='41'"));   //明細資料退款加總
                                                                                                                                                                  //20141008  計算後取絕對值
                                                                                                                                                                  //string strSumRTA = Convert.ToString(i40 - i41).PadLeft(12, '0');
            string strSumRTA = Convert.ToString(Math.Abs(i40 - i41)).PadLeft(12, '0');

            //20140922 第一碼固定帶"0"，2-7碼日期請帶系統日(產檔當日)，格式：yyMMdd
            //表頭
            string strHead = "0" + strBatch_Date_TW + strReceive_Batch + strRowsCount + strSumRTC + strSumRTA + "00";
            sbCont.AppendLine(strHead.PadRight(60, ' '));

            if (strSign_Type == "1")
            {
                #region 一般簽單
                //內容
                foreach (DataRow dr2 in dtRow2.Rows)
                {
                    //第二列資料
                    //批號 長度為3碼
                    string strBatch_NO = dr2["Batch_NO"].ToString().PadLeft(3, '0');
                    //該批總筆數 長度為3碼
                    string strReceive_Total_Count = dr2["Receive_Total_Count"].ToString().PadLeft(3, '0');
                    //該批總金額 長度為9碼
                    string strReceive_Total_AMT = Convert.ToInt64(dr2["Receive_Total_AMT"]).ToString().PadLeft(9, '0');
                    //商店代號 長度為10碼
                    string strShop_ID = dr2["Shop_ID"].ToString().PadLeft(10, '0');

                    //組合第二列資料
                    string strRow2 = "100" + strBatch_NO + strReceive_Total_Count + strReceive_Total_AMT + "00" + strShop_ID;
                    sbCont.AppendLine(strRow2.PadRight(60, ' '));

                    //20141014  上傳檔序號另外編碼，由1開始連續累加
                    int iSN = 1;

                    //第三列資料 (明細)
                    foreach (DataRow dr3 in dtRow3.Select("Batch_NO=" + dr2["Batch_NO"].ToString()))
                    {
                        //序號 長度為3碼
                        //20141014  上傳檔序號另外編碼，由1開始連續累加
                        //string strSN = dr3["SN"].ToString().PadLeft(3, '0');
                        string strSN = iSN.ToString().PadLeft(3, '0');
                        iSN++;
                        //卡號 長度為16碼
                        string strCard_No = dr3["Card_No"].ToString().PadLeft(16, '0');
                        //交易金額 長度為7碼
                        string strAMT = Convert.ToInt64(dr3["AMT"]).ToString().PadLeft(7, '0');
                        //請退款 長度為2碼
                        string strReceipt_Type = dr3["Receipt_Type"].ToString().PadLeft(2, '0');
                        //交易日期(西元年:MMDDYY) 長度為6碼
                        string strTran_Date = DateTime.ParseExact(dr3["Tran_Date"].ToString(), "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToString("MMddyy");
                        //授權號碼 長度為6碼
                        string strAuth_Code = dr3["Auth_Code"].ToString().PadLeft(6, '0');

                        //組合第三列資料
                        string strRow3 = "200" + strBatch_NO + strSN + strCard_No + strAMT + "00" + strReceipt_Type + strTran_Date + strAuth_Code;
                        sbCont.AppendLine(strRow3.PadRight(60, ' '));
                    }
                }
                //20141009  檔名更改為mpul
                //一般主機名稱
                strPathFile = strPathFile + @"\mpul.txt";
                //偽冒主機名稱
                strFPathFile = strFPathFile + @"\mpul" + strReceive_Batch + DateTime.Now.ToString("MMdd") + ".txt";
                #endregion 一般簽單
            }
            else
            {
                #region 分期簽單
                foreach (DataRow dr2 in dtRow2.Rows)
                {
                    //H(第一列)資料
                    //商店代號 長度為10碼
                    string strShop_ID = dr2["Shop_ID"].ToString().PadLeft(10, '0');
                    //批號 長度為5碼
                    string strBatch_NO = dr2["Batch_NO"].ToString().PadLeft(5, '0');
                    //該批總筆數 長度為5碼
                    string strReceive_Total_Count = dr2["Receive_Total_Count"].ToString().PadLeft(5, '0');
                    //該批總金額 長度為9碼
                    string strReceive_Total_AMT = Convert.ToInt64(dr2["Receive_Total_AMT"]).ToString().PadLeft(9, '0');

                    //組合第一列資料
                    string strRow2 = "H" + strShop_ID + strBatch_NO + strReceive_Total_Count + strReceive_Total_AMT + strBatch_Date.Replace("/", "") + "P";
                    sbCont.AppendLine(strRow2.PadRight(120, ' '));

                    //B(第二列)資料 (明細)
                    foreach (DataRow dr3 in dtRow3.Select("Shop_ID=" + dr2["Shop_ID"].ToString() + " AND Batch_NO=" + dr2["Batch_NO"].ToString()))
                    {
                        //卡號 長度為16碼
                        string strCard_No = dr3["Card_No"].ToString().PadLeft(16, '0');
                        //分期數 長度為2碼
                        string strINST = dr3["Installment_Periods"].ToString().Trim().PadLeft(2, '0');
                        //產品別 長度為3碼
                        string strProduct_Type = dr3["Product_Type"].ToString().PadLeft(3, '0');
                        //分期總價 長度為8碼
                        string strAMT = Convert.ToInt64(dr3["AMT"]).ToString().PadLeft(8, '0');
                        //交易日期(西元年:MMDDYY) 長度為8碼
                        string strTran_Date = dr3["Tran_Date"].ToString();
                        //授權號碼 長度為6碼
                        string strAuth_Code = dr3["Auth_Code"].ToString().PadLeft(6, '0');

                        //組合第二列資料
                        string strRow3 = "B" + strShop_ID + strBatch_Date.Replace("/", "") + strBatch_NO + "1" + strCard_No + strINST + strProduct_Type + strAMT
                                        + "0000" + strTran_Date + strAuth_Code + "00";
                        sbCont.AppendLine(strRow3.PadRight(120, ' '));
                    }
                }
                // strPathFile = strPathFile + @"\ipp" + strReceive_Batch + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                //一般主機名稱
                strPathFile = strPathFile + @"\ipp" + strReceive_Batch + ".txt";
                //偽冒主機名稱
                strFPathFile = strFPathFile + @"\ipp" + strReceive_Batch + DateTime.Now.ToString("MMdd") + ".txt";
                #endregion 分期簽單
            }
            //  FileTools.Create(strPathFile, sbCont.ToString().Remove(sbCont.ToString().LastIndexOf("\r\n")));
            //避免寫入錯誤
            string DataOpt = sbCont.ToString().Remove(sbCont.ToString().LastIndexOf("\r\n"));
            //增加寫入前清除舊檔
            myDelFile(strPathFile);
            myDelFile(strFPathFile);
            //寫入一般主機名稱
            FileTools.Create(strPathFile, DataOpt);
            //寫入偽冒主機名稱
            FileTools.Create(strFPathFile, DataOpt);
            strMsgID = "01_01070200_005";
            return true;
        }
        catch (Exception ex)
        {
            strMsgID = "01_01070200_004";
            // Logging.SaveLog(ELogLayer.BusinessRule, ex);
            Logging.Log(ex, LogLayer.BusinessRule);
        }
        return false;
    }



    /// <summary>
    ///  發送FTP
    /// </summary>  
    public bool SendToFTP(string jobID, string filePath)
    {

        bool result = true;
        string ftpIP = string.Empty;
        string ftpId = string.Empty;
        string ftpPwd = string.Empty;
        string ftpPath = string.Empty;
        string ZipPwd = string.Empty;

        bool isGet = GetFTPInfo2(jobID, ref ftpIP, ref ftpId, ref ftpPwd, ref ftpPath, ref ZipPwd);

        FTPFactory objFtp = new FTPFactory(ftpIP, "", ftpId, ftpPwd, "21", ftpPath, "Y");
        bool isSuccess = objFtp.Upload(ftpPath, Path.GetFileName(filePath), filePath);
        if (isSuccess)
        {
            result = true;
        }

        return result;
    }




    /// <summary>
    /// 取 FTP FileInfo
    /// </summary>
    /// <param name="ftpIp"></param>
    /// <param name="ftpId"></param>
    /// <param name="FtpPwd"></param>
    /// <param name="FtpPath"></param>
    /// <param name="ZipPwd"></param>
    /// <returns></returns>
    public static bool GetFTPInfo2(string jobID, ref string ftpIp, ref string ftpId, ref string FtpPwd, ref string FtpPath, ref string ZipPwd)
    {
        DataTable result = new DataTable();
        string sql = @"SELECT FtpIP, FtpUserName, FtpPwd, FtpPath, ZipPwd FROM [dbo].[tbl_FileInfo] WITH(NOLOCK) WHERE Job_ID = '" + jobID + "'";
        try
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;

            DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {
                ftpIp = resultSet.Tables[0].Rows[0][0].ToString();
                ftpId = resultSet.Tables[0].Rows[0][1].ToString();
                //測試用密碼不加密
                //FtpPwd = resultSet.Tables[0].Rows[0][2].ToString();
                //FtpPath = resultSet.Tables[0].Rows[0][3].ToString();
                //ZipPwd = resultSet.Tables[0].Rows[0][4].ToString();
                FtpPwd = RedirectHelper.GetDecryptString(resultSet.Tables[0].Rows[0][2].ToString());
                FtpPath = resultSet.Tables[0].Rows[0][3].ToString();
                ZipPwd = RedirectHelper.GetDecryptString(resultSet.Tables[0].Rows[0][4].ToString());
            }

            return true;
        }
        catch (Exception ex)
        {
            string ems = ex.Message;
            return false;
        }
    }

    /// <summary>
    /// 功能說明:發送mail
    /// 作    者:Tank
    /// 創建時間:2018/01/16
    /// 修改記錄:
    /// </summary>
    /// <param name="MerchName"></param>
    /// <param name="strBodyMsg"></param>
    /// <returns></returns>
    private bool SendMail(string resu, string strBodyMsg)
    {
        string strnMsg = string.Empty;
        string Subject = string.Format("【上傳主機/偽冒系統】檔案轉入結果通知 {0}", resu);
        try
        {
            string strMailUsers = GetMailUser();
            if (!string.IsNullOrEmpty(strMailUsers))
            {
                MailService.MailSenderBasic(strMailUsers, "", Subject, strBodyMsg, null);
                strnMsg = Subject + "，Mail通知成功！";
                // Logging.SaveLog(strnMsg, ELogType.Info);
                Logging.Log(strnMsg);
                return true;
            }
            else
            {
                // Logging.SaveLog(strnMsg, ELogType.Error);
                Logging.Log(strnMsg);

                strnMsg = Subject + "，Mail通知失敗！ 未取得寄送清單";
                return false;
            }
        }
        catch
        {
            // Logging.SaveLog(strnMsg, ELogType.Error);
            Logging.Log(strnMsg, LogState.Error, LogLayer.BusinessRule);

            strnMsg = Subject + "，Mail通知失敗！";
            return false;
        }
    }
    private string GetMailUser()
    {
        string strMailUsers = string.Empty;
        DataHelper dh = new DataHelper("Connection_System");
        DataHelper dh_CSIP = new DataHelper("Connection_CSIP");
        DataSet ds = new DataSet();
        ds = new DataSet();
        SqlCommand sqlcmd = new SqlCommand();
        try
        {
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandTimeout = 30000;
            sqlcmd.CommandText = @"select property_name from M_PROPERTY_CODE where function_key='01' and property_key='P010107020001'  and off_flag='1' order by SEQUENCE ";

            ds = dh_CSIP.ExecuteDataSet(sqlcmd);

            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string[] mailuser = ds.Tables[0].Rows[i][0].ToString().Split(':');  //林鴻揚:tank.lin@ctbcbank.com
                    strMailUsers += "," + mailuser[1].ToString();
                }
            }
            //修正當抓不到清單時長度問題會報錯
            if (strMailUsers.Length > 1)
            {
                return strMailUsers.Substring(1);
            }
        }
        catch (System.Exception ex)
        {
            // Logging.SaveLog(ex.Message, ELogType.Error);
            Logging.Log(ex, LogLayer.UI);

            // throw ex;
        }
        return "";
    }
}
