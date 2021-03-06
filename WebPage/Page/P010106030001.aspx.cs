//******************************************************************
//*  作    者：
//*  功能說明：一般簽單二Key
//*  創建日期：2014/08/27
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
//20160526 (U) by Tank, 卡號key滿自動跳下一欄，原被註解，解開使用
//20160720 (U) by Tank, 智仁:取消交易檢核日 B32代碼

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
using Framework.WebControls;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;
using Framework.Data.OM.Transaction;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using Framework.Common.Logging;
using CSIPKeyInGUI.BusinessRules;
using System.Data.SqlClient;
using Framework.Common.JavaScript;
using CSIPCommonModel.BaseItem;
using Framework.Data.OM;

public partial class Page_P010106030001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;

    /// <summary>
    /// 簽單類別(一般)
    /// </summary>
    private const string strSign_Type = "1";

    /// <summary>
    /// KeyIn_Flag(二KEY)
    /// </summary>
    private const string strKeyIn_Flag = "2";

    private string strBatch_Date;      //編批日期
    private string strReceive_Batch;   //收件批次
    private string strBatch_NO;        //批號
    private string strShop_ID;         //商店代號
    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {        
        if (!Page.IsPostBack)
        {
            #region 2014/09/22 一KEY鍵檔完,會跳回第一頁,但游標竟停在網址上,煩調整移至”編批日期” by Eric
            this.txtBatch_Date.Focus();
            #endregion

            InitDDL();
            Show();
            BindDropDownList();
            this.Session["OFF_FLAG"] = "";          //記錄內頁鍵檔狀態為新增或修改
            this.Session["OFF_FLAG_R"] = "";        //記錄內頁剔退鍵檔狀態為新增或修改
        }
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        SetControlsEnabled(false);
        SetButtonText();

        this.strBatch_Date = this.txtBatch_Date.Text;         //編批日期
        this.strReceive_Batch = this.txtReceive_Batch.Text;   //收件批次
        this.strBatch_NO = this.txtBatch_NO.Text;             //批號
        this.strShop_ID = this.txtShop_ID.Text;               //商店代號 
    }

    /// <summary>
    /// 資料查詢
    /// 修改紀錄:2021/02/01,03_Ares_Stanley-內頁建檔-資料未填滿時按ENTER觸發TAB鍵(跳至下一個項目)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void CustButton2_Click(object sender, EventArgs e)
    {
        #region 2014/9/23 編批日期鍵完,無法以TAB至下一個欄位(現在一定要用滑鼠點)
        //煩請修改為：取消出現日期選項框,改用人工key ,並用TAB移至下一個欄位 by Eric

        if (this.txtBatch_Date.Text.Length == 8)
        {
            DateTime timeValue;
            if (!DateTime.TryParse(string.Format("{0}/{1}/{2}", this.txtBatch_Date.Text.Substring(0, 4),
                this.txtBatch_Date.Text.Substring(4, 2), this.txtBatch_Date.Text.Substring(6, 2)), out timeValue))
            {
                jsBuilder.RegScript(this.UpdatePanel1, "alert('編批日期格式錯誤');$('#txtBatch_Date').focus();");
                return;
            }
        }
        else
        {
            jsBuilder.RegScript(this.UpdatePanel1, "alert('編批日期格式錯誤');$('#txtBatch_Date').focus();");
            return;
        }
        #endregion

        //重置修改狀態
        this.Session["OFF_FLAG"] = "";      //記錄內頁鍵檔狀態為新增或修改
        this.Session["OFF_FLAG_R"] = "";      //記錄內頁剔退鍵檔狀態為新增或修改
        SetButtonText();

        //清除內頁鍵檔及內頁剔退鍵檔輸入欄位
        CleanText();
        CleanText3();

        DataTable dt = new DataTable();

        string strMsgID = string.Empty;
        //設定全域變數
        this.strBatch_Date = this.txtBatch_Date.Text;         //編批日期
        this.strReceive_Batch = this.txtReceive_Batch.Text;   //收件批次
        this.strBatch_NO = this.txtBatch_NO.Text;             //批號
        this.strShop_ID = this.txtShop_ID.Text;               //商店代號

        #region 2014/9/12 一鍵及二鍵圴同一人也可KEY,必須擋不為同一人,此規範同現行CSIP,
        //於二KEY查詢時擋不可為同一個人,判斷最後修改人 by Eric
        DataSet ds_1key = BRArtificial_Signing_Primary.Select_Primary(strBatch_Date, strReceive_Batch, strBatch_NO, strShop_ID, strSign_Type, "1");

        if (ds_1key.Tables[0].Rows.Count > 0 && ds_1key.Tables[0].Rows[0]["Modify_User"] != null &&
            ds_1key.Tables[0].Rows[0]["Modify_User"].ToString().ToUpper().Trim().Equals(eAgentInfo.agent_id.ToUpper().Trim()))
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060200_033");
            return;
        }
        #endregion

        #region 2014/09/11 先判斷[編批日期],再判斷[收件批次],再判斷[批號],再判斷[商店代號] by Eric
        string expression = string.Format("Receive_Batch = '{0}' ", this.strReceive_Batch);  // where 條件

        //一般一Key匯入資料查詢 (sign_type = 1)
        //dt = BRArtificial_Signing_Batch_Data.Select_1KEY(strBatch_Date, strReceive_Batch, strBatch_NO, strShop_ID, strSign_Type);
        dt = BRArtificial_Signing_Batch_Data.Select_1KEY(strBatch_Date, strSign_Type);

        if (dt == null)
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060600_001");
            return;
        }
        else
        {
            if (dt.Rows.Count > 0)
            {
                DataRow[] foundRows;
                foundRows = dt.Select(expression);

                if (foundRows.Length > 0)
                {
                    expression += string.Format("and Batch_NO = '{0}' ", this.strBatch_NO);
                    foundRows = dt.Select(expression);

                    if (foundRows.Length > 0)
                    {
                        expression += string.Format("and Shop_ID = '{0}' ", this.strShop_ID);
                        foundRows = dt.Select(expression);

                        if (foundRows.Length > 0)
                        {
                            if (foundRows[0]["Process_Flag"].ToString().Equals("02"))
                            {
                                //該批已全部匯出並上傳成檔案
                                MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060200_002");
                                return;
                            }

                            txtReceive_Total_Count.Text = foundRows[0]["Receive_Total_Count"].ToString();
                            txtReceive_Total_AMT.Text = foundRows[0]["Receive_Total_AMT"].ToString();
                            txtlblShop_ID2.Text = foundRows[0]["Shop_ID"].ToString();
                        }
                        #region 判斷區塊
                        else
                        {
                            #region 商店代號不存在
                            MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060200_001");
                            jsBuilder.RegScript(this.txtReceive_Batch, "$('#txtShop_ID').val('');$('#txtShop_ID').focus();");
                            return;
                            #endregion
                        }
                    }
                    else
                    {
                        #region 批號不存在
                        MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060200_026");
                        jsBuilder.RegScript(this.txtReceive_Batch, "$('#txtBatch_NO').val('');$('#txtBatch_NO').focus();");
                        return;
                        #endregion
                    }
                }
                else
                {
                    #region 收件批次不存在
                    MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060200_025");
                    jsBuilder.RegScript(this.txtReceive_Batch, "$('#txtReceive_Batch').val('');$('#txtReceive_Batch').focus();");
                    return;
                    #endregion
                }
            }
            else
            {
                #region 編批日期不存在
                MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060200_024");
                jsBuilder.RegScript(this.txtBatch_Date, "$('#txtBatch_Date').val('');$('#txtBatch_Date').focus();");
                return;
                #endregion
            }
                        #endregion
        }
        #endregion

        DataSet ds = null;

        //一般一Key主檔資料查詢
        ds = BRArtificial_Signing_Primary.Select_Primary(strBatch_Date, strReceive_Batch, strBatch_NO, strShop_ID, strSign_Type, strKeyIn_Flag);

        if (ds == null)
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060600_001");
            return;
        }
        if (ds.Tables[0].Rows.Count == 0)
        {
            //若主檔無資料則新增
            if (BRArtificial_Signing_Primary.Insert_AS_Primary(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, strSign_Type, strKeyIn_Flag, eAgentInfo.agent_id, ref strMsgID))
            {
                //新增主檔資料同時抄寫一KEY剔退資料
                BRArtificial_Signing_Detail.Copy1Key_Detail(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, strSign_Type, "1", "1", eAgentInfo.agent_id, ref strMsgID);
                BindGridView1(this.txtBatch_Date.Text, txtShop_ID.Text, txtBatch_NO.Text, txtReceive_Batch.Text);

                this.pnl1.Visible = true;
                this.txtBatch_Date.Enabled = false;
                this.txtShop_ID.Enabled = false;
                this.txtBatch_NO.Enabled = false;
                this.txtReceive_Batch.Enabled = false;
                this.txtAMT.Attributes.Add("onkeyup", "CardNo(6)");
                //20160526 (U) by Tank, add event
                // 20220505 調整輸入信用卡號欄位，由4欄合併為1欄 By Kelton
                //this.txtCard_No1.Attributes.Add("onkeyup", "CardNo(1)");
                //this.txtCard_No2.Attributes.Add("onkeyup", "CardNo(2)");
                //this.txtCard_No3.Attributes.Add("onkeyup", "CardNo(3)");
                //this.txtCard_No4.Attributes.Add("onkeyup", "CardNo(4)");
                this.txtCard_No1.Attributes.Add("onkeyup", "CardNo(4)");
                this.txtAuth_Code.Attributes.Add("onkeyup", "CardNo(5)");            	
            	this.txtP1.Attributes.Add("onkeyup", "CardNo(7)");
            	this.txt_dpTran_Date.Attributes.Add("onkeyup", "CardNo(8)");
                //20210201 資料未填滿碼數按Enter會跳往下一格
                // 20220505 調整輸入信用卡號欄位，由4欄合併為1欄 By Kelton
                //this.txtCard_No1.Attributes.Add("onkeydown", "CardNo_Press(1)");
                //this.txtCard_No2.Attributes.Add("onkeydown", "CardNo_Press(2)");
                //this.txtCard_No3.Attributes.Add("onkeydown", "CardNo_Press(3)");
                //this.txtCard_No4.Attributes.Add("onkeydown", "CardNo_Press(4)");
                this.txtCard_No1.Attributes.Add("onkeydown", "CardNo_Press(4)");
                this.txt_dpTran_Date.Attributes.Add("onkeydown", "CardNo_Press(8)");
                this.txtAuth_Code.Attributes.Add("onkeydown", "CardNo_Press(5)");
                this.txtP1.Attributes.Add("onkeydown", "CardNo_Press(7)");
                this.txtAMT.Attributes.Add("onkeydown", "CardNo_Press(6)");
                //20210203 資料未填滿碼數按Enter會跳往下一格
                this.txtCard_No_1.Attributes.Add("onkeydown", "CardNo_Press(9)");
                this.txtAMT_1.Attributes.Add("onkeydown", "CardNo_Press(10)");
                this.ddlrt1.Attributes.Add("onkeydown", "CardNo_Press(11)");
                this.ddlReject_Reason.Attributes.Add("onkeydown", "CardNo_Press(12)");
            }
            else
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage(strMsgID);
            }
        }
        else
        {
            this.pnl1.Visible = true;
            this.txtBatch_Date.Enabled = false;
            this.txtShop_ID.Enabled = false;
            this.txtBatch_NO.Enabled = false;
            this.txtReceive_Batch.Enabled = false;

            #region 2014/09/11 內頁建檔部份，請改為key完後直接下一格 by Eric
            //20160526 (U) by Tank, 卡號key滿自動跳下一欄，原被註解，解開使用
            // 20220505 調整輸入信用卡號欄位，由4欄合併為1欄 By Kelton
            //this.txtCard_No1.Attributes.Add("onkeyup", "CardNo(1)");
            //this.txtCard_No2.Attributes.Add("onkeyup", "CardNo(2)");
            //this.txtCard_No3.Attributes.Add("onkeyup", "CardNo(3)");
            //this.txtCard_No4.Attributes.Add("onkeyup", "CardNo(4)");
            this.txtCard_No1.Attributes.Add("onkeyup", "CardNo(4)");
            this.txtAuth_Code.Attributes.Add("onkeyup", "CardNo(5)");
            this.txtAMT.Attributes.Add("onkeyup", "CardNo(6)");
            //20160526 (U) by Tank, add txtP1 event
            this.txtP1.Attributes.Add("onkeyup", "CardNo(7)");
            this.txt_dpTran_Date.Attributes.Add("onkeyup", "CardNo(8)");
            //20210201 資料未填滿碼數按Enter會跳往下一格
            // 20220505 調整輸入信用卡號欄位，由4欄合併為1欄 By Kelton
            //this.txtCard_No1.Attributes.Add("onkeydown", "CardNo_Press(1)");
            //this.txtCard_No2.Attributes.Add("onkeydown", "CardNo_Press(2)");
            //this.txtCard_No3.Attributes.Add("onkeydown", "CardNo_Press(3)");
            //this.txtCard_No4.Attributes.Add("onkeydown", "CardNo_Press(4)");
            this.txtCard_No1.Attributes.Add("onkeydown", "CardNo_Press(4)");
            this.txt_dpTran_Date.Attributes.Add("onkeydown", "CardNo_Press(8)");
            this.txtAuth_Code.Attributes.Add("onkeydown", "CardNo_Press(5)");
            this.txtP1.Attributes.Add("onkeydown", "CardNo_Press(7)");
            this.txtAMT.Attributes.Add("onkeydown", "CardNo_Press(6)");
            //20210203 資料未填滿碼數按Enter會跳往下一格
            this.txtCard_No_1.Attributes.Add("onkeydown", "CardNo_Press(9)");
            this.txtAMT_1.Attributes.Add("onkeydown", "CardNo_Press(10)");
            this.ddlrt1.Attributes.Add("onkeydown", "CardNo_Press(11)");
            this.ddlReject_Reason.Attributes.Add("onkeydown", "CardNo_Press(12)");
            #endregion

            BindGridView(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);
            BindGridView1(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);
        }
        #region 2014/09/22 按下[資料查詢]後, 游標請停[交易卡號]的第一個輸入框 by Eric
        base.sbRegScript.Append(BaseHelper.SetFocus("txtCard_No1"));
        #endregion
    }

    //內頁鍵檔 (確定新增/確定修改) 按鈕    
    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        #region 2014/9/23 編批日期鍵完,無法以TAB至下一個欄位(現在一定要用滑鼠點)
        //煩請修改為：取消出現日期選項框,改用人工key ,並用TAB移至下一個欄位 by Eric

        if (this.txt_dpTran_Date.Text.Length == 8)
        {
            DateTime timeValue;
            if (!DateTime.TryParse(string.Format("{0}/{1}/{2}", this.txt_dpTran_Date.Text.Substring(0, 4),
                this.txt_dpTran_Date.Text.Substring(4, 2), this.txt_dpTran_Date.Text.Substring(6, 2)), out timeValue))
            {
                jsBuilder.RegScript(this.UpdatePanel1, "alert('交易日期格式錯誤');$('#txt_dpTran_Date').focus();");
                return;
            }
        }
        else
        {
            jsBuilder.RegScript(this.UpdatePanel1, "alert('交易日期格式錯誤');$('#txt_dpTran_Date').focus();");
            return;
        }

        if (this.txtAuth_Code.Text.Length < 6)
        {
            jsBuilder.RegScript(this.txtAuth_Code, "$('#txtAuth_Code').focus();alert('授權號碼規則不符');");
            return;
        }

        #endregion

        #region 2014/9/18 預防js失效,金額不可為負數的第二道防線 by Eric
        if (this.txtAMT.Text.Contains("-"))
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060200_034");
            return;
        }
        #endregion

        string Shop_ID2 = this.txtlblShop_ID2.Text;
        // 20220505 調整輸入信用卡號欄位，由4欄合併為1欄 By Kelton
        //string Card_No = this.txtCard_No1.Text + this.txtCard_No2.Text + this.txtCard_No3.Text + this.txtCard_No4.Text;
        string Card_No = this.txtCard_No1.Text;
        string Auth_Code = this.txtAuth_Code.Text.ToUpper();
        string strAmt = this.txtAMT.Text;
        string Receipt_Type2 = this.ddlP1.SelectedValue.ToString();
        string Tran_Date = string.Format("{0}/{1}/{2}", this.txt_dpTran_Date.Text.Substring(0, 4),
                this.txt_dpTran_Date.Text.Substring(4, 2), this.txt_dpTran_Date.Text.Substring(6, 2));
        string SN = this.txtSN.Text;

        string strMsgID = string.Empty;

        //20140924  卡號及金額不可空白
        if (string.IsNullOrEmpty(Card_No) && string.IsNullOrEmpty(strAmt))
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, new string[] { "01_01060200_007", "01_01060200_006" });
            return;
        }
        if (string.IsNullOrEmpty(Card_No))
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060200_007");
            return;
        }
        if (string.IsNullOrEmpty(strAmt))
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060200_006");
            return;
        }

        if ((string)Session["OFF_FLAG"] != "1")
        {
            #region 新增資料

            string[] strMessage = new string[8];

            if (string.IsNullOrEmpty(Card_No))
            {
                strMessage[0] = "01_01060200_007";
            }
            if (Card_No.Length < 16)
            {
                strMessage[4] = "01_01060200_003";
                this.ddlReject_Reason.SelectedValue = "C40";
            }

            #region 請款
            if (ddlP1.SelectedValue == "40")
            {
                //交易日期檢核條件A
                if (string.IsNullOrEmpty(Tran_Date))
                {
                    strMessage[1] = "01_01060200_004";
                }
                else
                {
                    #region 交易日期檢核
                    DateTime STime = Convert.ToDateTime(DateTime.Now.Date.ToString("yyyy/MM/dd"));
                    DateTime ETime = Convert.ToDateTime(Tran_Date);
                    TimeSpan Total = STime.Subtract(ETime); //日期相減

                    string SysDate = DateTime.Now.Date.ToString("yyyy/MM/dd");

                    DateTime dtSysDate = Convert.ToDateTime(SysDate);
                    DateTime AddDays = GET_ADD11_DAY(Tran_Date);

                    //交易日期檢核條件B
                    if (Total.Days < 0)
                    {
                        strMessage[5] = "01_01060200_009";
                        this.ddlReject_Reason.SelectedValue = "B06";
                    }
                    //交易日期檢核條件E
                    if (Total.Days >= 90 && dtSysDate <= AddDays)
                    {
                        strMessage[6] = "01_01060200_010";
                        this.ddlReject_Reason.SelectedValue = "B29";
                    }
                    //交易日期檢核條件F
                    if (dtSysDate > AddDays)
                    {
                        strMessage[7] = "01_01060200_011";
                        this.ddlReject_Reason.SelectedValue = "B29";
                    }

                    #endregion
                }
            }
            #endregion
            #region 退款
            else
            {

                //交易日期檢核條件G
                if (string.IsNullOrEmpty(this.txt_dpTran_Date.Text))
                {
                    Tran_Date = DateTime.Now.Date.ToString("yyyy/MM/dd");
                }
                else
                {
                    Tran_Date = string.Format("{0}/{1}/{2}", this.txt_dpTran_Date.Text.Substring(0, 4),
                            this.txt_dpTran_Date.Text.Substring(4, 2), this.txt_dpTran_Date.Text.Substring(6, 2));
                }

                //交易日期檢核條件A
                if (string.IsNullOrEmpty(Tran_Date))
                {
                    strMessage[1] = "01_01060200_004";
                }
                else
                {
                    #region 交易日期檢核
                    DateTime STime = Convert.ToDateTime(DateTime.Now.Date.ToString("yyyy/MM/dd"));
                    DateTime ETime = Convert.ToDateTime(Tran_Date);
                    TimeSpan Total = STime.Subtract(ETime); //日期相減

                    string SysDate = DateTime.Now.Date.ToString("yyyy/MM/dd");

                    DateTime dtSysDate = Convert.ToDateTime(SysDate);
                    DateTime AddDays = GET_ADD11_DAY(Tran_Date);

                    #region 2014/9/11 系統日-交易日 >11個月 則顯示ERR訊息 , 新增交易日不可為未來日期 by Eric
                    AddDays = DateTime.Today.AddMonths(-11);

                    //交易日期不可為未來日
                    if (Total.Days < 0)
                    {
                        strMessage[6] = "01_01060200_009";
                        this.ddlReject_Reason.SelectedValue = "B06";
                    }

                    //不可超過11個月提出退款
                    //                    if (AddDays > dtSysDate)
                    if (AddDays > ETime)
                    {
                        strMessage[5] = "01_01060200_012";
                        this.ddlReject_Reason.SelectedValue = "H22";
                    }
                    #endregion

                    #endregion
                }
            }
            #endregion

            if (string.IsNullOrEmpty(Auth_Code))
            {
                strMessage[2] = "01_01060200_005";
            }

            if (string.IsNullOrEmpty(strAmt))
            {
                strMessage[3] = "01_01060200_006";
            }


            if (!string.IsNullOrEmpty(strMessage[0]) || !string.IsNullOrEmpty(strMessage[1]) ||
                !string.IsNullOrEmpty(strMessage[2]) || !string.IsNullOrEmpty(strMessage[3]) ||
                !string.IsNullOrEmpty(strMessage[4]) || !string.IsNullOrEmpty(strMessage[5]) ||
                !string.IsNullOrEmpty(strMessage[6]) || !string.IsNullOrEmpty(strMessage[7]))
            {
                ShowMessage(this.UpdatePanel1, strMessage);
                return;
            }
            else
            {

                DateTime STime = Convert.ToDateTime(DateTime.Now.Date.ToString("yyyy/MM/dd"));
                DateTime ETime = Convert.ToDateTime(Tran_Date);
                TimeSpan Total = STime.Subtract(ETime); //日期相減

                #region 2014/09/19 退款可接受11個月內，請修正:不可擋 by Eric
                //20160720 (U) by Tank, 智仁:取消交易檢核日 B32代碼
                //if (ddlP1.SelectedValue == "40")
                //{
                //    if (Total.Days >= 30 && Total.Days <= 89)
                //    {
                //        strMessage[0] = "01_01060200_008";
                //        ShowMessage2(this.UpdatePanel1, strMessage);
                //        this.ddlReject_Reason.SelectedValue = "B32";
                //        return;
                //    }
                //}
                #endregion

                #region GetArtificial_Signing_Detail SN 最大號
                int MaxNo = 0;
                DataTable dtMax = new DataTable();

                dtMax = BRArtificial_Signing_Detail.Select_1CheckMaxNo(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch, "0", strKeyIn_Flag, strSign_Type);

                if (string.IsNullOrEmpty(dtMax.Rows[0][0].ToString()))
                {
                    MaxNo = 1;
                }
                else
                {
                    MaxNo = int.Parse(dtMax.Rows[0][0].ToString()) + 1;
                }
                #endregion

                //20160526 (U) by Tank, insert 2Key先check資料
                DataTable dt1KeyData = Check2KeyData(MaxNo, strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);
                DataRow dr1KeyRow = dt1KeyData.Select("SN=" + MaxNo)[0];

                if (string.Compare(Card_No, dr1KeyRow["Card_No"].ToString()) != 0)//交易卡號
                {
                    jsBuilder.RegScript(this.UpdatePanel1, "alert('序號[" + MaxNo + "]-[交易卡號] 與一Key資料[" + dr1KeyRow["Card_No"].ToString() + "]不符!');$('#txtCard_No1').focus();");
                    return;
                }
                else if (string.Compare(Tran_Date.Replace("/",""), dr1KeyRow["Tran_Date"].ToString()) != 0)//交易日期
                {
                    jsBuilder.RegScript(this.UpdatePanel1, "alert('序號[" + MaxNo + "]-[交易日期] 與一Key資料[" + dr1KeyRow["Tran_Date"].ToString() + "]不符!');$('#txt_dpTran_Date').focus();");
                    return;
                }
                else if (string.Compare(Auth_Code, dr1KeyRow["Auth_Code"].ToString()) != 0)//授權號碼
                {
                    jsBuilder.RegScript(this.UpdatePanel1, "alert('序號[" + MaxNo + "]-[授權號碼] 與一Key資料["+ dr1KeyRow["Auth_Code"].ToString()+"]不符!');$('#txtAuth_Code').focus();");
                    return;
                }
                else if (string.Compare(Receipt_Type2, dr1KeyRow["Receipt_Type"].ToString()) != 0)//請退款
                {
                    jsBuilder.RegScript(this.UpdatePanel1, "alert('序號[" + MaxNo + "]-[請退款] 與一Key資料["+dr1KeyRow["Receipt_Type"].ToString()+"]不符!');$('#txtP1').focus();");
                    return;
                }
                else if (string.Compare(strAmt, dr1KeyRow["AMT"].ToString()) != 0)//金額
                {
                    jsBuilder.RegScript(this.UpdatePanel1, "alert('序號[" + MaxNo + "]-[金額] 與一Key資料[" + dr1KeyRow["AMT"].ToString() + "]不符!');$('#txtAMT').focus();");
                    return;
                }

                
                //TODO 塞入明細資料
                if (BRArtificial_Signing_Detail.Insert_Detail(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, MaxNo.ToString(), strSign_Type, "0", strKeyIn_Flag,
                    Card_No, Tran_Date, "", "", Auth_Code, strAmt, Receipt_Type2, "", eAgentInfo.agent_id, ref strMsgID))
                {
                    //資料有異動時平帳資料歸零
                    ClearBalance(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, eAgentInfo.agent_id);
                    InitialBalance();
                    InitialBGColor();
                    CleanText();
                }
                else
                {
                    MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
                }              
            }

            BindGridView(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);
            BindGridView1(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);

            base.strClientMsg += MessageHelper.GetMessage("01_01060200_016");

            #region 2014/09/22 按下[確定新增]後, 游標請停[交易卡號]的第一個輸入框 by Eric
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCard_No1"));
            #endregion

            return;

            #endregion 新增資料
        }
        else
        {
            #region 按下修改Button檢核
            string[] strMessage = new string[8];

            if (string.IsNullOrEmpty(Card_No))
            {
                strMessage[0] = "01_01060200_007";
            }
            if (Card_No.Length < 16)
            {
                strMessage[4] = "01_01060200_003";
                this.ddlReject_Reason.SelectedValue = "C40";
            }

            #region 請款
            if (ddlP1.SelectedValue == "40")
            {
                //交易日期檢核條件A
                if (string.IsNullOrEmpty(Tran_Date))
                {
                    strMessage[1] = "01_01060200_004";
                }
                else
                {
                    #region 交易日期檢核
                    DateTime STime = Convert.ToDateTime(DateTime.Now.Date.ToString("yyyy/MM/dd"));
                    DateTime ETime = Convert.ToDateTime(Tran_Date);
                    TimeSpan Total = STime.Subtract(ETime); //日期相減

                    string SysDate = DateTime.Now.Date.ToString("yyyy/MM/dd");

                    DateTime dtSysDate = Convert.ToDateTime(SysDate);
                    DateTime AddDays = GET_ADD11_DAY(Tran_Date);

                    //交易日期檢核條件B
                    if (Total.Days < 0)
                    {
                        strMessage[5] = "01_01060200_009";
                        this.ddlReject_Reason.SelectedValue = "B06";
                    }
                    //交易日期檢核條件E
                    if (Total.Days >= 90 && dtSysDate <= AddDays)
                    {
                        strMessage[6] = "01_01060200_010";
                        this.ddlReject_Reason.SelectedValue = "B29";
                    }
                    //交易日期檢核條件F
                    if (dtSysDate > AddDays)
                    {
                        strMessage[7] = "01_01060200_011";
                        this.ddlReject_Reason.SelectedValue = "B29";
                    }

                    #endregion
                }
            }
            #endregion
            #region 退款
            else
            {

                //交易日期檢核條件G
                if (string.IsNullOrEmpty(this.txt_dpTran_Date.Text))
                {
                    Tran_Date = DateTime.Now.Date.ToString("yyyy/MM/dd");
                }
                else
                {
                    Tran_Date = string.Format("{0}/{1}/{2}", this.txt_dpTran_Date.Text.Substring(0, 4),
                            this.txt_dpTran_Date.Text.Substring(4, 2), this.txt_dpTran_Date.Text.Substring(6, 2));
                }

                //交易日期檢核條件A
                if (string.IsNullOrEmpty(Tran_Date))
                {
                    strMessage[1] = "01_01060200_004";
                }
                else
                {
                    #region 交易日期檢核
                    DateTime STime = Convert.ToDateTime(DateTime.Now.Date.ToString("yyyy/MM/dd"));
                    DateTime ETime = Convert.ToDateTime(Tran_Date);
                    TimeSpan Total = STime.Subtract(ETime); //日期相減

                    string SysDate = DateTime.Now.Date.ToString("yyyy/MM/dd");

                    DateTime dtSysDate = Convert.ToDateTime(SysDate);
                    DateTime AddDays = GET_ADD11_DAY(Tran_Date);

                    #region 2014/9/11 系統日-交易日 >11個月 則顯示ERR訊息 , 新增交易日不可為未來日期 by Eric
                    AddDays = DateTime.Today.AddMonths(-11);

                    //交易日期不可為未來日
                    if (Total.Days < 0)
                    {
                        strMessage[6] = "01_01060200_009";
                        this.ddlReject_Reason.SelectedValue = "B06";
                    }

                    //不可超過11個月提出退款
                    //                    if (AddDays > dtSysDate)
                    if (AddDays > ETime)
                    {
                        strMessage[5] = "01_01060200_012";
                        this.ddlReject_Reason.SelectedValue = "H22";
                    }
                    #endregion

                    #endregion
                }
            }
            #endregion

            if (string.IsNullOrEmpty(Auth_Code))
            {
                strMessage[2] = "01_01060200_005";
            }

            if (string.IsNullOrEmpty(strAmt))
            {
                strMessage[3] = "01_01060200_006";
            }


            if (!string.IsNullOrEmpty(strMessage[0]) || !string.IsNullOrEmpty(strMessage[1]) ||
                !string.IsNullOrEmpty(strMessage[2]) || !string.IsNullOrEmpty(strMessage[3]) ||
                !string.IsNullOrEmpty(strMessage[4]) || !string.IsNullOrEmpty(strMessage[5]) ||
                !string.IsNullOrEmpty(strMessage[6]) || !string.IsNullOrEmpty(strMessage[7]))
            {
                ShowMessage(this.UpdatePanel1, strMessage);
                return;
            }
            else
            {

                DateTime STime = Convert.ToDateTime(DateTime.Now.Date.ToString("yyyy/MM/dd"));
                DateTime ETime = Convert.ToDateTime(Tran_Date);
                TimeSpan Total = STime.Subtract(ETime); //日期相減

                #region 2014/09/19 退款可接受11個月內，請修正:不可擋 by Eric
                //20160606 (U) by Tank, 智仁:取消
                //if (ddlP1.SelectedValue == "40")
                //{
                //    if (Total.Days >= 30 && Total.Days <= 89)
                //    {
                //        strMessage[0] = "01_01060200_008";
                //        //ShowMessage2(this.UpdatePanel1, strMessage);
                //        ShowMessageEdit(this.UpdatePanel1, strMessage);
                //        this.ddlReject_Reason.SelectedValue = "B32";
                //        return;
                //    }
                //}
                #endregion

                //TODO 修改明細內容按下Save後動作
                if (BRArtificial_Signing_Detail.Update_Detail(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, SN, strSign_Type, strKeyIn_Flag, Card_No,
                    Tran_Date, "", "", Auth_Code, strAmt, Receipt_Type2, eAgentInfo.agent_id, ref strMsgID))
                {
                    //資料有異動時平帳資料歸零
                    ClearBalance(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, eAgentInfo.agent_id);
                    InitialBalance();
                    InitialBGColor();
                }
                else
                {
                    MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
                }
            }

            BindGridView(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);
            BindGridView1(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);
            base.strClientMsg += MessageHelper.GetMessage(strMsgID);

            //不管更新成功或失敗都恢復為新增狀態
            this.Session["OFF_FLAG"] = "";
            SetButtonText();
            CleanText();

            #region 2014/09/22 按下[確定新增]後, 游標請停[交易卡號]的第一個輸入框 by Eric
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCard_No1"));
            #endregion

            return;
            #endregion
        }
    }

    //檢核錯誤存入剔退清單
    protected void CustButton3_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        string Shop_ID2 = this.txtlblShop_ID2.Text;
        // 20220505 調整輸入信用卡號欄位，由4欄合併為1欄 By Kelton
        //string Card_No = this.txtCard_No1.Text + this.txtCard_No2.Text + this.txtCard_No3.Text + this.txtCard_No4.Text;
        string Card_No = this.txtCard_No1.Text;
        string Auth_Code = this.txtAuth_Code.Text.ToUpper();
        string strAmt = this.txtAMT.Text;
        string Receipt_Type2 = this.ddlP1.SelectedValue.ToString();
        string Tran_Date = this.txt_dpTran_Date.Text;
        string SN = this.txtSN.Text;

        #region GetArtificial_Signing_Detail SN 最大號
        int MaxNo = 0;
        DataTable dtMax = new DataTable();

        dtMax = BRArtificial_Signing_Detail.Select_1CheckMaxNo(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch, "1", strKeyIn_Flag, strSign_Type);

        if (string.IsNullOrEmpty(dtMax.Rows[0][0].ToString()))
        {
            MaxNo = 1;
        }
        else
        {
            MaxNo = int.Parse(dtMax.Rows[0][0].ToString()) + 1;
        }
        #endregion


        //TODO 塞入明細資料
        if (BRArtificial_Signing_Detail.Insert_Detail(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, MaxNo.ToString(), strSign_Type, "1", strKeyIn_Flag,
                    Card_No, Tran_Date, "", "", Auth_Code, strAmt, Receipt_Type2, "", eAgentInfo.agent_id, ref strMsgID))
        {
            //資料有異動時平帳資料歸零
            ClearBalance(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, eAgentInfo.agent_id);
            InitialBalance();
            InitialBGColor();
            CleanText();
        }
        else
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
        }

        BindGridView(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);
        BindGridView1(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);

        this.Session["OFF_FLAG"] = "";
        SetButtonText();
        CleanText();

        base.strClientMsg += MessageHelper.GetMessage(strMsgID);
    }

    //30天~89天請款(日曆天),需附切結書！(視為正常件)
    protected void btnDubleConfirm_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        string Shop_ID2 = this.txtlblShop_ID2.Text;
        // 20220505 調整輸入信用卡號欄位，由4欄合併為1欄 By Kelton
        //string Card_No = this.txtCard_No1.Text + this.txtCard_No2.Text + this.txtCard_No3.Text + this.txtCard_No4.Text;
        string Card_No = this.txtCard_No1.Text;
        string Auth_Code = this.txtAuth_Code.Text.ToUpper();
        string strAmt = this.txtAMT.Text;
        string Receipt_Type2 = this.ddlP1.SelectedValue.ToString();
        string Tran_Date = this.txt_dpTran_Date.Text;

        #region GetArtificial_Signing_Detail SN 最大號
        int MaxNo = 0;
        DataTable dtMax = new DataTable();

        dtMax = BRArtificial_Signing_Detail.Select_1CheckMaxNo(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch, "0", strKeyIn_Flag, strSign_Type);

        if (string.IsNullOrEmpty(dtMax.Rows[0][0].ToString()))
        {
            MaxNo = 1;
        }
        else
        {
            MaxNo = int.Parse(dtMax.Rows[0][0].ToString()) + 1;
        }
        #endregion


        //TODO 塞入明細資料
        if (BRArtificial_Signing_Detail.Insert_Detail(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, MaxNo.ToString(), strSign_Type, "0", strKeyIn_Flag,
                    Card_No, Tran_Date, "", "", Auth_Code, strAmt, Receipt_Type2, "", eAgentInfo.agent_id, ref strMsgID))
        {
            //資料有異動時平帳資料歸零
            ClearBalance(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, eAgentInfo.agent_id);
            InitialBalance();
            InitialBGColor();
            CleanText();
        }
        else
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
        }

        BindGridView(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);
        BindGridView1(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);

        this.Session["OFF_FLAG"] = "";
        SetButtonText();

        base.strClientMsg += MessageHelper.GetMessage(strMsgID);
        return;
    }

    //30天~89天請款(日曆天),需附切結書！(視為正常件)--修改儲存
    protected void BtnEditSave_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        string Shop_ID2 = this.txtlblShop_ID2.Text;
        // 20220505 調整輸入信用卡號欄位，由4欄合併為1欄 By Kelton
        //string Card_No = this.txtCard_No1.Text + this.txtCard_No2.Text + this.txtCard_No3.Text + this.txtCard_No4.Text;
        string Card_No = this.txtCard_No1.Text;
        string Auth_Code = this.txtAuth_Code.Text.ToUpper();
        string strAmt = this.txtAMT.Text;
        string Receipt_Type2 = this.ddlP1.SelectedValue.ToString();
        string Tran_Date = this.txt_dpTran_Date.Text;
        string SN = this.txtSN.Text;

        //TODO 修改明細內容按下Save後動作
        if (BRArtificial_Signing_Detail.Update_Detail(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, SN, strSign_Type, strKeyIn_Flag,
                    Card_No, Tran_Date, "", "", Auth_Code, strAmt, Receipt_Type2, eAgentInfo.agent_id, ref strMsgID))
        {
            //資料有異動時平帳資料歸零
            ClearBalance(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, eAgentInfo.agent_id);
            InitialBalance();
            InitialBGColor();
            CleanText();
        }
        else
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
        }

        BindGridView(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);
        BindGridView1(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);

        this.Session["OFF_FLAG"] = "";
        SetButtonText();

        base.strClientMsg += MessageHelper.GetMessage(strMsgID);
        return;
    }

    protected void grvDetail1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CustLinkButton LbtnModify = (CustLinkButton)e.Row.FindControl("lbtnModify1");
            LbtnModify.Text = BaseHelper.GetShowText("01_01060200_021");

            CustLinkButton LbtnDelete = (CustLinkButton)e.Row.FindControl("lbtnDelete1");
            LbtnDelete.Text = BaseHelper.GetShowText("01_01060200_022");
        }
    }
    protected void grvDetail1_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (!Server.HtmlDecode(this.grvDetail1.Rows[e.NewEditIndex].Cells[4].Text).Trim().Equals(""))
        {
            this.ddlrt1.SelectByText(Server.HtmlDecode(this.grvDetail1.Rows[e.NewEditIndex].Cells[4].Text));
        }
        this.txtCard_No_1.Text = Server.HtmlDecode(this.grvDetail1.Rows[e.NewEditIndex].Cells[2].Text);
        this.txtAMT_1.Text = Server.HtmlDecode(this.grvDetail1.Rows[e.NewEditIndex].Cells[3].Text);
        if (!Server.HtmlDecode(this.grvDetail1.Rows[e.NewEditIndex].Cells[5].Text).Trim().Equals(""))
        {
            this.ddlReject_Reason.SelectByText(Server.HtmlDecode(this.grvDetail1.Rows[e.NewEditIndex].Cells[5].Text));
        }
        this.lblSNText.Text = Server.HtmlDecode(this.grvDetail1.Rows[e.NewEditIndex].Cells[1].Text);
        Session["OFF_FLAG"] = "1";
        btnReject.ShowID = "01_01060200_025";
    }
    protected void grvDetail1_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        string strMsgID = string.Empty;

        string strSN = Server.HtmlDecode(this.grvDetail1.Rows[e.RowIndex].Cells[1].Text);

        //TO DO 刪除明細資料
        if (BRArtificial_Signing_Detail.Delete_Detail(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, strSN, strSign_Type, "1", strKeyIn_Flag, ref strMsgID))
        {
            //資料有異動時平帳資料歸零
            ClearBalance(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, eAgentInfo.agent_id);
            InitialBalance();
            InitialBGColor();
        }
        else
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
        }

        BindGridView(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);
        BindGridView1(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);

        //刪除後清除修改FLAG
        this.Session["OFF_FLAG_R"] = "";
        SetButtonText();
        CleanText3();

        base.strClientMsg += MessageHelper.GetMessage(strMsgID);
    }

    protected void grvDetail0_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CustLinkButton LbtnModify = (CustLinkButton)e.Row.FindControl("lbtnModify");
            LbtnModify.Text = BaseHelper.GetShowText("01_01060200_021");

            CustLinkButton LbtnDelete = (CustLinkButton)e.Row.FindControl("lbtnDelete");
            LbtnDelete.Text = BaseHelper.GetShowText("01_01060200_022");
        }
    }
    protected void grvDetail0_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        string strMsgID = string.Empty;

        string strSN = Server.HtmlDecode(this.grvDetail0.Rows[e.RowIndex].Cells[1].Text);

        //TO DO 刪除明細資料
        if (BRArtificial_Signing_Detail.Delete_Detail(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, strSN, strSign_Type, "0", strKeyIn_Flag, ref strMsgID))
        {
            //資料有異動時平帳資料歸零
            ClearBalance(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, eAgentInfo.agent_id);
            InitialBalance();
            InitialBGColor();
        }
        else
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
        }

        BindGridView(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);
        BindGridView1(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);

        //刪除後清除修改FLAG
        this.Session["OFF_FLAG"] = "";
        SetButtonText();
        CleanText();

        base.strClientMsg += MessageHelper.GetMessage(strMsgID);
    }
    protected void grvDetail0_RowEditing(object sender, GridViewEditEventArgs e)
    {
        //TO DO 修改明細資料
        // 20220505 調整輸入信用卡號欄位，由4欄合併為1欄 By Kelton
        //this.txtCard_No1.Text = Server.HtmlDecode(this.grvDetail0.Rows[e.NewEditIndex].Cells[3].Text.Substring(0, 4));
        //this.txtCard_No2.Text = Server.HtmlDecode(this.grvDetail0.Rows[e.NewEditIndex].Cells[3].Text.Substring(4, 4));
        //this.txtCard_No3.Text = Server.HtmlDecode(this.grvDetail0.Rows[e.NewEditIndex].Cells[3].Text.Substring(8, 4));
        //this.txtCard_No4.Text = Server.HtmlDecode(this.grvDetail0.Rows[e.NewEditIndex].Cells[3].Text.Substring(12, 4));
        this.txtCard_No1.Text = Server.HtmlDecode(this.grvDetail0.Rows[e.NewEditIndex].Cells[3].Text.Substring(0, 16));
        this.txt_dpTran_Date.Text = Server.HtmlDecode(this.grvDetail0.Rows[e.NewEditIndex].Cells[4].Text);
        this.txtAuth_Code.Text = Server.HtmlDecode(this.grvDetail0.Rows[e.NewEditIndex].Cells[5].Text).ToUpper();
        this.txtAMT.Text = Server.HtmlDecode(this.grvDetail0.Rows[e.NewEditIndex].Cells[6].Text);
        if (!Server.HtmlDecode(this.grvDetail0.Rows[e.NewEditIndex].Cells[7].Text).Trim().Equals(""))
        {
            this.ddlP1.SelectByText(Server.HtmlDecode(this.grvDetail0.Rows[e.NewEditIndex].Cells[7].Text));
        }
        this.txtSN.Text = Server.HtmlDecode(this.grvDetail0.Rows[e.NewEditIndex].Cells[1].Text);
        Session["OFF_FLAG"] = "1";
        btnConfirm.ShowID = "01_01060200_025";
    }

    //剔退新增
    protected void btnReject_Click(object sender, EventArgs e)
    {
        string strCard_No = txtCard_No_1.Text;
        string strReject_Reason = ddlReject_Reason.SelectedValue.ToString();
        string strReceiptType = ddlrt1.SelectedValue.ToString();
        string strAMT = txtAMT_1.Text;
        string strSN = lblSNText.Text;

        string strMsgID = string.Empty;

        if ((string)Session["OFF_FLAG_R"] != "1")
        {
            #region 新增剔退資料

            #region GetArtificial_Signing_Detail SN 最大號
            int MaxNo = 0;
            DataTable dtMax = new DataTable();

            dtMax = BRArtificial_Signing_Detail.Select_1CheckMaxNo(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch, "1", strKeyIn_Flag, strSign_Type);

            if (string.IsNullOrEmpty(dtMax.Rows[0][0].ToString()))
            {
                MaxNo = 1;
            }
            else
            {
                MaxNo = int.Parse(dtMax.Rows[0][0].ToString()) + 1;
            }
            #endregion

            string[] strMessage = new string[3];

            if (string.IsNullOrEmpty(strCard_No))
            {
                strMessage[0] = "01_01060200_007";
            }

            if (string.IsNullOrEmpty(strAMT))
            {
                strMessage[1] = "01_01060200_006";
            }

            //20140924  剔退卡號不檢查是否為16碼
            //if (strCard_No.Length < 16)
            //{
            //    strMessage[2] = "01_01060200_003";
            //}

            if (!string.IsNullOrEmpty(strMessage[0]) || !string.IsNullOrEmpty(strMessage[1]) || !string.IsNullOrEmpty(strMessage[2]))
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, strMessage);
                return;
            }

            //TODO 塞入明細資料
            if (BRArtificial_Signing_Detail.Insert_Detail(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, MaxNo.ToString(), strSign_Type, "1", strKeyIn_Flag,
                    strCard_No, "", "", "", "", strAMT, strReceiptType, strReject_Reason, eAgentInfo.agent_id, ref strMsgID))
            {
                //資料有異動時平帳資料歸零
                ClearBalance(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, eAgentInfo.agent_id);
                InitialBalance();
                InitialBGColor();
                CleanText();
            }
            else
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
            }

            BindGridView(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);
            BindGridView1(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);

            base.strClientMsg += MessageHelper.GetMessage("01_01060200_016");
            CleanText3();
            return;

            #endregion 新增剔退資料
        }
        else // 使用者按下剔退清單中的修改,修改完畢儲存動作
        {
            #region 剔退修改
            string[] strMessage = new string[3];

            if (string.IsNullOrEmpty(strCard_No))
            {
                strMessage[0] = "01_01060200_007";
            }

            if (string.IsNullOrEmpty(strAMT))
            {
                strMessage[1] = "01_01060200_006";
            }

            //20140924  剔退卡號不檢查是否為16碼
            //if (strCard_No.Length < 16)
            //{
            //    strMessage[2] = "01_01060200_003";
            //}

            if (!string.IsNullOrEmpty(strMessage[0]) || !string.IsNullOrEmpty(strMessage[1]) || !string.IsNullOrEmpty(strMessage[2]))
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, strMessage);
                return;
            }

            //TODO 修改明細內容按下Save後動作
            if (BRArtificial_Signing_Detail.Update_Reject_Detail(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, strSN, strSign_Type, strKeyIn_Flag,
                strCard_No, strAMT, strReceiptType, strReject_Reason, eAgentInfo.agent_id, ref strMsgID))
            {
                //資料有異動時平帳資料歸零
                ClearBalance(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, eAgentInfo.agent_id);
                InitialBalance();
                InitialBGColor();
            }
            else
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
            }

            BindGridView(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);
            BindGridView1(strBatch_Date, strShop_ID, strBatch_NO, strReceive_Batch);

            base.strClientMsg += MessageHelper.GetMessage("01_01060200_017");

            this.Session["OFF_FLAG_R"] = "";
            SetButtonText();
            CleanText3();
            return;
            #endregion
        }
    }

    //第一次平帳
    protected void btnFirst_Click(object sender, EventArgs e)
    {
        this.btnFirst.Enabled = false;
        string strMsgID = string.Empty;

        this.txtBatchDateP1.Text = this.txtBatch_Date.Text;//編批日期
        this.txtShop_IDP1.Text = this.txtShop_ID.Text; //商店代號
        this.txtBatch_NOP1.Text = this.txtBatch_NO.Text;//批號
        this.txtReceive_BatchP1.Text = this.txtReceive_Batch.Text;//收件批次
        this.txtReceive_Total_CountP1.Text = this.txtReceive_Total_Count.Text;//收件-總筆數
        this.txtReceive_Total_AMTP1.Text = this.txtReceive_Total_AMT.Text;//收件-總金額

        DataTable dt1 = new DataTable();
        dt1 = BRArtificial_Signing_Detail.SelectBalanceData(this.txtBatchDateP1.Text, txtShop_ID.Text, txtBatch_NO.Text, txtReceive_Batch.Text, "1", "2");//取得筆數與金額

        if (dt1.Rows[0]["RowsCount"].ToString().Equals("0"))
        {
            //查無明細資料
            MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060200_023");
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_01060200_023");
            return;
        }

        this.txtKeyin_Success_Count_All.Text = dt1.Rows[0]["Keyin_Success_Count_All"].ToString();
        this.txtKeyin_Success_AMT_All.Text = dt1.Rows[0]["Keyin_Success_AMT_All"].ToString();
        this.txtKeyin_Success_Count_40.Text = dt1.Rows[0]["Keyin_Success_Count_40"].ToString();
        this.txtKeyin_Success_AMT_40.Text = dt1.Rows[0]["Keyin_Success_AMT_40"].ToString();
        this.txtKeyin_Success_Count_41.Text = dt1.Rows[0]["Keyin_Success_Count_41"].ToString();
        this.txtKeyin_Success_AMT_41.Text = dt1.Rows[0]["Keyin_Success_AMT_41"].ToString();
        this.txtKeyin_Reject_Count_All.Text = dt1.Rows[0]["Keyin_Reject_Count_All"].ToString();
        this.txtKeyin_Reject_AMT_All.Text = dt1.Rows[0]["Keyin_Reject_AMT_All"].ToString();
        this.txtKeyin_Reject_Count_40.Text = dt1.Rows[0]["Keyin_Reject_Count_40"].ToString();
        this.txtKeyin_Reject_AMT_40.Text = dt1.Rows[0]["Keyin_Reject_AMT_40"].ToString();
        this.txtKeyin_Reject_Count_41.Text = dt1.Rows[0]["Keyin_Reject_Count_41"].ToString();
        this.txtKeyin_Reject_AMT_41.Text = dt1.Rows[0]["Keyin_Reject_AMT_41"].ToString();

        this.txtFirst_Balance_Count.Text = Convert.ToString(int.Parse(txtReceive_Total_Count.Text) - int.Parse(txtKeyin_Success_Count_All.Text) - int.Parse(txtKeyin_Reject_Count_All.Text));//M
        this.txtFirst_Balance_AMT.Text = Convert.ToString(decimal.Parse(txtReceive_Total_AMT.Text) - decimal.Parse(txtKeyin_Success_AMT_All.Text) - decimal.Parse(txtKeyin_Reject_AMT_All.Text));//N

        if (txtFirst_Balance_Count.Text == "0" && Convert.ToDecimal(txtFirst_Balance_AMT.Text).Equals(0))
        {
            //btnSecond.Enabled = false;//不可執行第二次平帳
            btnBalanceOK.Enabled = true;
        }
        else
        {
            btnSecond.Enabled = true;//可執行第二次平帳
            btnFirst.Enabled = false;
        }
        //更新主檔第一次平帳動作
        if (!BRArtificial_Signing_Primary.Update_ASP_First_BalanceFlag(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, strSign_Type, strKeyIn_Flag, eAgentInfo.agent_id, "Y", ref strMsgID))
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
        }

        #region 2014/9/11 (M) 、(N)若不為0則顯示訊息 by Eric
        if (!string.IsNullOrEmpty(this.txtFirst_Balance_Count.Text.Trim()) &&
            !string.IsNullOrEmpty(this.txtFirst_Balance_AMT.Text.Trim()))
        {
            if (this.txtFirst_Balance_Count.Text.Trim() != "0" && this.txtFirst_Balance_AMT.Text.Trim() != "0")
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060200_027");
            }
            else if (this.txtFirst_Balance_Count.Text.Trim() != "0")
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060200_028");
            }
            else if (this.txtFirst_Balance_AMT.Text.Trim() != "0")
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060200_029");
            }
        }
        #endregion

        //* 顯示端末訊息
        base.strClientMsg += MessageHelper.GetMessage(strMsgID);
    }

    //第二次平帳
    protected void btnSecond_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        string[] strMessage = new string[3];

        if (string.IsNullOrEmpty(txtAdjust_Count.Text))
        {
            strMessage[0] = "01_01060200_013";
        }

        if (string.IsNullOrEmpty(txtAdjust_AMT.Text))
        {
            strMessage[1] = "01_01060200_014";
        }

        if (!string.IsNullOrEmpty(strMessage[0]) || !string.IsNullOrEmpty(strMessage[1]))
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, strMessage);
            return;
        }

        this.txtSecond_Balance_Count.Text = Convert.ToString(int.Parse(txtAdjust_Count.Text) - int.Parse(txtKeyin_Success_Count_All.Text) - int.Parse(txtKeyin_Reject_Count_All.Text));
        this.txtSecond_Balance_AMT.Text = Convert.ToString(decimal.Parse(txtAdjust_AMT.Text) - decimal.Parse(txtKeyin_Success_AMT_All.Text) - decimal.Parse(txtKeyin_Reject_AMT_All.Text));

        #region 2014/9/12 (Q)、(R) 若不為0則顯示訊息 by Eric
        if (this.txtSecond_Balance_Count.Text.Trim() != "0" &&
            Convert.ToDecimal(txtSecond_Balance_AMT.Text) != 0)
        {
            SecondMsg("01_01060200_030");
            return;
        }
        else if (this.txtSecond_Balance_Count.Text.Trim() != "0")
        {
            SecondMsg("01_01060200_031");
            return;
        }
        else if (Convert.ToDecimal(txtSecond_Balance_AMT.Text) != 0)
        {
            SecondMsg("01_01060200_032");
            return;
        }
        #endregion
        else
        {
            txtAdjust_Count.Enabled = false;
            txtAdjust_AMT.Enabled = false;
            btnSecond.Enabled = false;
            btnBalanceOK.Enabled = true;
        }

        //更新主檔第二次平帳動作
        if (!BRArtificial_Signing_Primary.Update_ASP_Second_BalanceFlag(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, strSign_Type, strKeyIn_Flag, eAgentInfo.agent_id, "Y", ref strMsgID))
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
        }
        //* 顯示端末訊息
        base.strClientMsg += MessageHelper.GetMessage(strMsgID);
    }

    //平帳完成提交
    protected void btnBalanceOK_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;

        bool isError = false;

        string[] strMessage = new string[2];
        //TODO 與一Key資料做比對

        #region 正常資料明細清單比對
        //查詢一KEY正常件資料
        DataTable dt1 = new DataTable();
        dt1 = BRArtificial_Signing_Detail.SelectDetailCase0(this.strBatch_Date, this.strShop_ID, this.strBatch_NO, this.strReceive_Batch, strSign_Type, "1");

        for (int i = 0; i < grvDetail0.Rows.Count; i++)
        {
            string strSN = Server.HtmlDecode(this.grvDetail0.Rows[i].Cells[1].Text);
            string strgvShop_ID = Server.HtmlDecode(this.grvDetail0.Rows[i].Cells[2].Text);
            string strCard_No = Server.HtmlDecode(this.grvDetail0.Rows[i].Cells[3].Text);
            string strTran_Date = Server.HtmlDecode(this.grvDetail0.Rows[i].Cells[4].Text);
            string strAuth_Code = Server.HtmlDecode(this.grvDetail0.Rows[i].Cells[5].Text).ToUpper();
            string strAMT = Server.HtmlDecode(this.grvDetail0.Rows[i].Cells[6].Text);
            string strNewReceipt_Type = Server.HtmlDecode(this.grvDetail0.Rows[i].Cells[7].Text);

            if (dt1.Select("SN=" + strSN).Length > 0)
            {
                DataRow dr = dt1.Select("SN=" + strSN)[0];

                if (string.Compare(strSN, dr["SN"].ToString()) != 0)//序號
                {
                    //this.grvDetail0.Rows[i].Cells[1].Style.Add("background-color", "Red");
                    this.grvDetail0.Rows[i].Cells[1].BackColor = Color.Red;
                    isError = true;
                }
                if (string.Compare(strgvShop_ID, dr["Shop_ID"].ToString()) != 0)//商店代號
                {
                    //this.grvDetail0.Rows[i].Cells[2].Style.Add("background-color", "Red");
                    this.grvDetail0.Rows[i].Cells[2].BackColor = Color.Red;
                    isError = true;
                }
                if (string.Compare(strCard_No, dr["Card_No"].ToString()) != 0)//交易卡號
                {
                    //this.grvDetail0.Rows[i].Cells[3].Style.Add("background-color", "Red");
                    this.grvDetail0.Rows[i].Cells[3].BackColor = Color.Red;
                    isError = true;
                }
                if (string.Compare(strTran_Date, dr["Tran_Date"].ToString()) != 0)//交易日期
                {
                    //this.grvDetail0.Rows[i].Cells[4].Style.Add("background-color", "Red");
                    this.grvDetail0.Rows[i].Cells[4].BackColor = Color.Red;
                    isError = true;
                }
                if (string.Compare(strAuth_Code, dr["Auth_Code"].ToString()) != 0)//授權號碼
                {
                    //this.grvDetail0.Rows[i].Cells[5].Style.Add("background-color", "Red");
                    this.grvDetail0.Rows[i].Cells[5].BackColor = Color.Red;
                    isError = true;
                }
                if (string.Compare(strAMT, dr["AMT"].ToString()) != 0)//金額
                {
                    //this.grvDetail0.Rows[i].Cells[6].Style.Add("background-color", "Red");
                    this.grvDetail0.Rows[i].Cells[6].BackColor = Color.Red;
                    isError = true;
                }
                if (string.Compare(strNewReceipt_Type, dr["NewReceipt_Type"].ToString()) != 0)//請退款
                {
                    //this.grvDetail0.Rows[i].Cells[7].Style.Add("background-color", "Red");
                    this.grvDetail0.Rows[i].Cells[7].BackColor = Color.Red;
                    isError = true;
                }
            }
            else
            {
                this.grvDetail0.Rows[i].Style.Add("background-color", "Red");
                isError = true;
            }
        }
        #endregion

        #region 剔退資料明細清單比對
        //查詢一KEY剔退件資料
        DataTable dt2 = new DataTable();
        dt2 = BRArtificial_Signing_Detail.SelectDetailCase1(this.strBatch_Date, this.strShop_ID, this.strBatch_NO, this.strReceive_Batch, strSign_Type, "1");
        for (int j = 0; j < grvDetail1.Rows.Count; j++)
        {
            string strSN2 = Server.HtmlDecode(this.grvDetail1.Rows[j].Cells[1].Text);
            string strCard_No2 = Server.HtmlDecode(this.grvDetail1.Rows[j].Cells[2].Text);
            string strAMT2 = Server.HtmlDecode(this.grvDetail1.Rows[j].Cells[3].Text);
            string strNewReceipt_Type2 = Server.HtmlDecode(this.grvDetail1.Rows[j].Cells[4].Text);
            string strReject_Reason = Server.HtmlDecode(this.grvDetail1.Rows[j].Cells[5].Text);

            if (dt2.Select("SN=" + strSN2).Length > 0)
            {
                DataRow dr2 = dt2.Select("SN=" + strSN2)[0];

                if (string.Compare(strSN2, dr2["SN"].ToString()) != 0)
                {
                    //this.grvDetail1.Rows[j].Cells[1].Style.Add("background-color", "Red");
                    this.grvDetail1.Rows[j].Cells[1].BackColor = Color.Red;
                    isError = true;
                }
                if (string.Compare(strCard_No2, dr2["Card_No"].ToString()) != 0)
                {
                    //this.grvDetail1.Rows[j].Cells[2].Style.Add("background-color", "Red");
                    this.grvDetail1.Rows[j].Cells[2].BackColor = Color.Red;
                    isError = true;
                }
                if (string.Compare(strAMT2, dr2["AMT"].ToString()) != 0)
                {
                    //this.grvDetail1.Rows[j].Cells[3].Style.Add("background-color", "Red");
                    this.grvDetail1.Rows[j].Cells[3].BackColor = Color.Red;
                    isError = true;
                }
                if (string.Compare(strNewReceipt_Type2, dr2["NewReceipt_Type"].ToString()) != 0)
                {
                    //this.grvDetail1.Rows[j].Cells[4].Style.Add("background-color", "Red");
                    this.grvDetail1.Rows[j].Cells[4].BackColor = Color.Red;
                    isError = true;
                }
                if (string.Compare(strReject_Reason, dr2["Reject_Reason"].ToString()) != 0)
                {
                    //this.grvDetail1.Rows[j].Cells[5].Style.Add("background-color", "Red");
                    this.grvDetail1.Rows[j].Cells[5].BackColor = Color.Red;
                    isError = true;
                }
            }
            else
            {
                this.grvDetail1.Rows[j].Style.Add("background-color", "Red");
                isError = true;
            }
        }
        #endregion

        #region 主檔資料比對

        //取出一key主檔資料
        DataSet ds = null;
        ds = BRArtificial_Signing_Primary.Select_Primary(strBatch_Date, strReceive_Batch, strBatch_NO, strShop_ID, strSign_Type, "1");
        //取出一key批次資料檔
        DataTable dtbd = new DataTable();
        dtbd = BRArtificial_Signing_Batch_Data.Select_1KEY(strBatch_Date, strReceive_Batch, strBatch_NO, strShop_ID, strSign_Type);

        if (ds.Tables[0].Rows.Count > 0)
        {
            if (string.Compare(this.txtBatchDateP1.Text, ds.Tables[0].Rows[0]["Batch_Date"].ToString()) != 0)
            {
                this.txtBatchDateP1.Style.Add("background-color", "Red");
                isError = true;
            }
            if (string.Compare(this.txtShop_IDP1.Text, ds.Tables[0].Rows[0]["Shop_ID"].ToString()) != 0)
            {
                this.txtShop_IDP1.Style.Add("background-color", "Red");
                isError = true;
            }
            if (string.Compare(this.txtBatch_NOP1.Text, ds.Tables[0].Rows[0]["Batch_NO"].ToString()) != 0)
            {
                this.txtBatch_NOP1.Style.Add("background-color", "Red");
                isError = true;
            }
            if (string.Compare(this.txtReceive_BatchP1.Text, ds.Tables[0].Rows[0]["Receive_Batch"].ToString()) != 0)
            {
                this.txtReceive_BatchP1.Style.Add("background-color", "Red");
                isError = true;
            }

            //if (string.Compare(this.txtReceive_Total_CountP1.Text, dtbd.Rows[0]["Receive_Total_Count"].ToString()) != 0)
            //{
            //    this.txtReceive_Total_CountP1.Style.Add("background-color", "Red");
            //    isError = true;
            //}
            //if (string.Compare(this.txtReceive_Total_AMTP1.Text, dtbd.Rows[0]["Receive_Total_AMT"].ToString()) != 0)
            //{
            //    this.txtReceive_Total_AMTP1.Style.Add("background-color", "Red");
            //    isError = true;
            //}

            if (string.Compare(this.txtKeyin_Success_Count_All.Text, ds.Tables[0].Rows[0]["Keyin_Success_Count_All"].ToString()) != 0)
            {
                //this.txtKeyin_Success_Count_All.Style.Add("background-color", "Red");
                this.txtKeyin_Success_Count_All.BackColor = Color.Red;
                isError = true;
            }
            if (string.Compare(this.txtKeyin_Success_AMT_All.Text, ds.Tables[0].Rows[0]["Keyin_Success_AMT_All"].ToString()) != 0)
            {
                //this.txtKeyin_Success_AMT_All.Style.Add("background-color", "Red");
                this.txtKeyin_Success_AMT_All.BackColor = Color.Red;
                isError = true;
            }
            if (string.Compare(this.txtKeyin_Success_Count_40.Text, ds.Tables[0].Rows[0]["Keyin_Success_Count_40"].ToString()) != 0)
            {
                //this.txtKeyin_Success_Count_40.Style.Add("background-color", "Red");
                this.txtKeyin_Success_Count_40.BackColor = Color.Red;
                isError = true;
            }
            if (string.Compare(this.txtKeyin_Success_AMT_40.Text, ds.Tables[0].Rows[0]["Keyin_Success_AMT_40"].ToString()) != 0)
            {
                //this.txtKeyin_Success_AMT_40.Style.Add("background-color", "Red");
                this.txtKeyin_Success_AMT_40.BackColor = Color.Red;
                isError = true;
            }
            if (string.Compare(this.txtKeyin_Success_Count_41.Text, ds.Tables[0].Rows[0]["Keyin_Success_Count_41"].ToString()) != 0)
            {
                //this.txtKeyin_Success_Count_41.Style.Add("background-color", "Red");
                this.txtKeyin_Success_Count_41.BackColor = Color.Red;
                isError = true;
            }
            if (string.Compare(this.txtKeyin_Success_AMT_41.Text, ds.Tables[0].Rows[0]["Keyin_Success_AMT_41"].ToString()) != 0)
            {
                //this.txtKeyin_Success_AMT_41.Style.Add("background-color", "Red");
                this.txtKeyin_Success_AMT_41.BackColor = Color.Red;
                isError = true;
            }
            if (string.Compare(this.txtKeyin_Reject_Count_All.Text, ds.Tables[0].Rows[0]["Keyin_Reject_Count_All"].ToString()) != 0)
            {
                //this.txtKeyin_Reject_Count_All.Style.Add("background-color", "Red");
                this.txtKeyin_Reject_Count_All.BackColor = Color.Red;
                isError = true;
            }
            if (string.Compare(this.txtKeyin_Reject_AMT_All.Text, ds.Tables[0].Rows[0]["Keyin_Reject_AMT_All"].ToString()) != 0)
            {
                //this.txtKeyin_Reject_AMT_All.Style.Add("background-color", "Red");
                this.txtKeyin_Reject_AMT_All.BackColor = Color.Red;
                isError = true;
            }
            if (string.Compare(this.txtKeyin_Reject_Count_40.Text, ds.Tables[0].Rows[0]["Keyin_Reject_Count_40"].ToString()) != 0)
            {
                //this.txtKeyin_Reject_Count_40.Style.Add("background-color", "Red");
                this.txtKeyin_Reject_Count_40.BackColor = Color.Red;
                isError = true;
            }
            if (string.Compare(this.txtKeyin_Reject_AMT_40.Text, ds.Tables[0].Rows[0]["Keyin_Reject_AMT_40"].ToString()) != 0)
            {
                //this.txtKeyin_Reject_AMT_40.Style.Add("background-color", "Red");
                this.txtKeyin_Reject_AMT_40.BackColor = Color.Red;
                isError = true;
            }
            if (string.Compare(this.txtKeyin_Reject_Count_41.Text, ds.Tables[0].Rows[0]["Keyin_Reject_Count_41"].ToString()) != 0)
            {
                //this.txtKeyin_Reject_Count_41.Style.Add("background-color", "Red");
                this.txtKeyin_Reject_Count_41.BackColor = Color.Red;
                isError = true;
            }
            if (string.Compare(this.txtKeyin_Reject_AMT_41.Text, ds.Tables[0].Rows[0]["Keyin_Reject_AMT_41"].ToString()) != 0)
            {
                //this.txtKeyin_Reject_AMT_41.Style.Add("background-color", "Red");
                this.txtKeyin_Reject_AMT_41.BackColor = Color.Red;
                isError = true;
            }
            if (string.Compare(this.txtFirst_Balance_Count.Text, ds.Tables[0].Rows[0]["First_Balance_Count"].ToString()) != 0)
            {
                //this.txtFirst_Balance_Count.Style.Add("background-color", "Red");
                this.txtFirst_Balance_Count.BackColor = Color.Red;
                isError = true;
            }
            if (string.Compare(this.txtFirst_Balance_AMT.Text, ds.Tables[0].Rows[0]["First_Balance_AMT"].ToString()) != 0)
            {
                //this.txtFirst_Balance_AMT.Style.Add("background-color", "Red");
                this.txtFirst_Balance_AMT.BackColor = Color.Red;
                isError = true;
            }

            if (string.Compare(string.IsNullOrEmpty(this.txtAdjust_Count.Text) ? "0" : this.txtAdjust_Count.Text, ds.Tables[0].Rows[0]["Adjust_Count"].ToString()) != 0)
            {
                //this.txtAdjust_Count.Style.Add("background-color", "Red");
                this.txtAdjust_Count.BackColor = Color.Red;
                isError = true;
            }

            decimal decAdjust_AMT = string.IsNullOrEmpty(this.txtAdjust_AMT.Text) ? 0 : Convert.ToDecimal(this.txtAdjust_AMT.Text);
            if (!decAdjust_AMT.Equals(Convert.ToDecimal(ds.Tables[0].Rows[0]["Adjust_AMT"].ToString())))
            {
                //this.txtAdjust_AMT.Style.Add("background-color", "Red");
                this.txtAdjust_AMT.BackColor = Color.Red;
                isError = true;
            }
            if (string.Compare(string.IsNullOrEmpty(this.txtSecond_Balance_Count.Text) ? "0" : this.txtSecond_Balance_Count.Text, ds.Tables[0].Rows[0]["Second_Balance_Count"].ToString()) != 0)
            {
                //this.txtSecond_Balance_Count.Style.Add("background-color", "Red");
                this.txtSecond_Balance_Count.BackColor = Color.Red;
                isError = true;
            }

            decimal decSecond_Balance_AMT = string.IsNullOrEmpty(this.txtSecond_Balance_AMT.Text) ? 0 : Convert.ToDecimal(this.txtSecond_Balance_AMT.Text);
            if (!decSecond_Balance_AMT.Equals(Convert.ToDecimal(ds.Tables[0].Rows[0]["Second_Balance_AMT"].ToString())))
            {
                //this.txtSecond_Balance_AMT.Style.Add("background-color", "Red");
                this.txtSecond_Balance_AMT.BackColor = Color.Red;
                isError = true;
            }
        }

        #endregion

        #region 比對一key與二key Count筆數
        if (dt1.Rows.Count > grvDetail0.Rows.Count)
        {
            strMessage[0] = "01_01060200_019";
        }
        if (dt2.Rows.Count > grvDetail1.Rows.Count)
        {
            strMessage[1] = "01_01060200_020";
        }

        if (!string.IsNullOrEmpty(strMessage[0]) || !string.IsNullOrEmpty(strMessage[1]))
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, strMessage);
            return;
        }
        #endregion

        //平帳資料寫入主檔
        if (BRArtificial_Signing_Primary.Update_ASP_Balance(strBatch_Date, strBatch_NO, this.strShop_ID, strReceive_Batch, strSign_Type, strKeyIn_Flag, eAgentInfo.agent_id,
            txtKeyin_Success_Count_All.Text, txtKeyin_Success_AMT_All.Text, txtKeyin_Success_Count_40.Text, txtKeyin_Success_AMT_40.Text,
            txtKeyin_Success_Count_41.Text, txtKeyin_Success_AMT_41.Text, txtKeyin_Reject_Count_All.Text, txtKeyin_Reject_AMT_All.Text,
            txtKeyin_Reject_Count_40.Text, txtKeyin_Reject_AMT_40.Text, txtKeyin_Reject_Count_41.Text, txtKeyin_Reject_AMT_41.Text, "Y", txtFirst_Balance_Count.Text,
            txtFirst_Balance_AMT.Text, txtAdjust_Count.Text, txtAdjust_AMT.Text, "Y", txtSecond_Balance_Count.Text, txtSecond_Balance_AMT.Text, "Y", ref strMsgID))
        {

            //寫入customer_log
            DataTable dtlog = CommonFunction.GetDataTable();
            CommonFunction.UpdateLog("", this.txtBatch_NO.Text, "批號", dtlog);
            CommonFunction.UpdateLog("", this.txtShop_ID.Text, "商店代號", dtlog);
            CommonFunction.InsertCustomerLog(dtlog, eAgentInfo, this.txtBatch_Date.Text.Replace("/", ""), "DB", (structPageInfo)Session["PageInfo"]);
        }
        else
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage(strMsgID);
            return;
        }

        //一key及二key資料比對若不一致則顯示警告訊息
        if (isError)
        {
            strMsgID = "01_01060500_027";
            MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage(strMsgID);
        }
        else
        {
            //比對成功更新比對註記 KeyIn_MatchFlag
            if (!BRArtificial_Signing_Primary.Update_ASP_KeyIn_MatchFlag(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, strSign_Type, strKeyIn_Flag, eAgentInfo.agent_id, "Y", ref strMsgID))
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage(strMsgID);
                return;
            }

            MessageHelper.ShowMessageAndGoto(this.UpdatePanel1, Request.Url.AbsoluteUri, "01_01060200_021");
        }
    }

    #endregion

    #region 方法區
    private void SetControlsEnabled(bool blnEnabled)
    {
        SetControlsEnabledText(blnEnabled);
    }

    private void SetControlsEnabledText(bool blnEnabled2)
    {
        this.txtReceive_Total_Count.Enabled = blnEnabled2;
        this.txtReceive_Total_AMT.Enabled = blnEnabled2;
        this.txtlblShop_ID2.Enabled = blnEnabled2;

        //欄位A~R
        this.txtBatchDateP1.Enabled = blnEnabled2;
        this.txtShop_IDP1.Enabled = blnEnabled2;
        this.txtBatch_NOP1.Enabled = blnEnabled2;
        this.txtReceive_BatchP1.Enabled = blnEnabled2;
        this.txtReceive_Total_CountP1.Enabled = blnEnabled2;
        this.txtReceive_Total_AMTP1.Enabled = blnEnabled2;
        this.txtKeyin_Success_Count_All.Enabled = blnEnabled2;
        this.txtKeyin_Success_AMT_All.Enabled = blnEnabled2;
        this.txtKeyin_Success_Count_40.Enabled = blnEnabled2;
        this.txtKeyin_Success_AMT_40.Enabled = blnEnabled2;
        this.txtKeyin_Success_Count_41.Enabled = blnEnabled2;
        this.txtKeyin_Success_AMT_41.Enabled = blnEnabled2;
        this.txtKeyin_Reject_Count_All.Enabled = blnEnabled2;
        this.txtKeyin_Reject_AMT_All.Enabled = blnEnabled2;
        this.txtKeyin_Reject_Count_40.Enabled = blnEnabled2;
        this.txtKeyin_Reject_AMT_40.Enabled = blnEnabled2;
        this.txtKeyin_Reject_Count_41.Enabled = blnEnabled2;
        this.txtKeyin_Reject_AMT_41.Enabled = blnEnabled2;
        this.txtFirst_Balance_Count.Enabled = blnEnabled2;
        this.txtFirst_Balance_AMT.Enabled = blnEnabled2;

        this.txtSecond_Balance_Count.Enabled = blnEnabled2;
        this.txtSecond_Balance_AMT.Enabled = blnEnabled2;

        this.btnBalanceOK.Enabled = blnEnabled2;

    }

    private void InitDDL()
    {
        ddlP1.Items.Add(new ListItem("請款", "40"));
        ddlP1.Items.Add(new ListItem("退款", "41"));
        ddlP1.SelectByText("請款");

        ddlrt1.Items.Add(new ListItem("請款", "40"));
        ddlrt1.Items.Add(new ListItem("退款", "41"));
        ddlrt1.SelectByText("請款");
    }

    /// <summary>
    /// 傳入多個MessageID一次用JS Alert出消息.
    /// 每筆消息會換行
    /// </summary>
    /// <param name="page">JS註冊用頁面元件(提示:一個元件只能注冊一次JS)</param>
    /// <param name="strMsgIDs">MessageID元組</param>
    public void ShowMessage(Control ctr, params string[] strMsgIDs)
    {
        string strMsg = MessageHelper.GetMessages(strMsgIDs);
        strMsg += "\\n\\n" + MessageHelper.GetMessage("01_01060200_035");
        base.sbRegScript.Append("if(confirm('" + strMsg + "')) {$('#CustButton3').click();}else{}");
    }

    /// <summary>
    /// 傳入多個MessageID一次用JS Alert出消息.
    /// 每筆消息會換行
    /// </summary>
    /// <param name="page">JS註冊用頁面元件(提示:一個元件只能注冊一次JS)</param>
    /// <param name="strMsgIDs">MessageID元組</param>
    public void ShowMessage2(Control ctr, params string[] strMsgIDs)
    {
        string strMsg = MessageHelper.GetMessages(strMsgIDs);
        strMsg += "\\n\\n" + MessageHelper.GetMessage("01_01060200_036");
        base.sbRegScript.Append("if(confirm('" + strMsg + "')) {$('#btnDubleConfirm').click();}else{$('#CustButton3').click();}");
    }

    /// <summary>
    /// 傳入多個MessageID一次用JS Alert出消息.
    /// 每筆消息會換行
    /// </summary>
    /// <param name="page">JS註冊用頁面元件(提示:一個元件只能注冊一次JS)</param>
    /// <param name="strMsgIDs">MessageID元組</param>
    public void ShowMessageEdit(Control ctr, params string[] strMsgIDs)
    {
        string strMsg = MessageHelper.GetMessages(strMsgIDs);
        strMsg += "\\n\\n" + MessageHelper.GetMessage("01_01060200_037");
        base.sbRegScript.Append("if(confirm('" + strMsg + "')) {$('#BtnEditSave').click();}else{}");
    }

    #region 算出系統日加11個月後天數
    //private int GET_ADD11_DAY(string DT)
    //{
    //    DateTime BF = new DateTime();
    //    BF = (DateTime.Parse(DT)).AddMonths(11);
    //    string NEW_DATE = BF.ToString("yyyy/MM/dd");
    //    DateTime dt1 = Convert.ToDateTime(NEW_DATE);
    //    DateTime dt2 = Convert.ToDateTime(DateTime.Now.Date.ToString("yyyy/MM/dd"));
    //    TimeSpan ts1 = new TimeSpan();
    //    ts1 = dt1.Subtract(dt2);
    //    return ts1.Days;
    //}
    private DateTime GET_ADD11_DAY(string DT)
    {
        DateTime BF = new DateTime();
        BF = (DateTime.Parse(DT)).AddMonths(11);
        //string NEW_DATE = BF.ToString("yyyy/MM/dd");
        return BF;
    }

    #endregion

    #region 剔退明細資料
    protected void BindGridView1(string Batch_Date, string Shop_ID, string Batch_No, string Receive_Batch)
    {
        DataTable dt = new DataTable();

        //dt = BRArtificial_Signing_Detail.Select2DetailCase1(Batch_Date, Shop_ID, Batch_No, Receive_Batch);
        dt = BRArtificial_Signing_Detail.SelectDetailCase1(Batch_Date, Shop_ID, Batch_No, Receive_Batch, "1", "2");

        try
        {
            //this.gp1.RecordCount = dt.Rows.Count;
            this.grvDetail1.DataSource = dt;
            this.grvDetail1.DataBind();

            if (this.grvDetail1.Rows.Count > 10)
            {
                this.Pl_gv2.Height = 325;
            }

        }
        catch
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
        }
    }
    #endregion

    #region 正常明細資料
    protected void BindGridView(string Batch_Date, string Shop_ID, string Batch_No, string Receive_Batch)
    {
        DataTable dt = new DataTable();

        //dt = BRArtificial_Signing_Detail.Select2DetailCase0(Batch_Date, Shop_ID, Batch_No, Receive_Batch);
        dt = BRArtificial_Signing_Detail.SelectDetailCase0(Batch_Date, Shop_ID, Batch_No, Receive_Batch, "1", "2");

        try
        {
            //this.gpList.RecordCount = dt.Rows.Count;
            this.grvDetail0.DataSource = dt;
            this.grvDetail0.DataBind();

            if (this.grvDetail0.Rows.Count > 10)
            {
                this.Pl_gv1.Height = 325;
            }

        }
        catch
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
        }
    }

    #endregion

    private void Show()
    {
        grvDetail0.Columns[0].HeaderText = BaseHelper.GetShowText("01_01060200_023");
        grvDetail0.Columns[1].HeaderText = BaseHelper.GetShowText("01_01060200_024");
        grvDetail0.Columns[2].HeaderText = BaseHelper.GetShowText("01_01060200_009");
        grvDetail0.Columns[3].HeaderText = BaseHelper.GetShowText("01_01060200_015");
        grvDetail0.Columns[4].HeaderText = BaseHelper.GetShowText("01_01060200_016");
        grvDetail0.Columns[5].HeaderText = BaseHelper.GetShowText("01_01060200_017");
        grvDetail0.Columns[6].HeaderText = BaseHelper.GetShowText("01_01060200_018");
        grvDetail0.Columns[7].HeaderText = BaseHelper.GetShowText("01_01060200_019");
        grvDetail1.Columns[0].HeaderText = BaseHelper.GetShowText("01_01060200_023");
        grvDetail1.Columns[1].HeaderText = BaseHelper.GetShowText("01_01060200_024");
        grvDetail1.Columns[2].HeaderText = BaseHelper.GetShowText("01_01060200_015");
        grvDetail1.Columns[3].HeaderText = BaseHelper.GetShowText("01_01060200_026");
        grvDetail1.Columns[4].HeaderText = BaseHelper.GetShowText("01_01060200_019");
        grvDetail1.Columns[5].HeaderText = BaseHelper.GetShowText("01_01060200_027");
    }

    /// <summary>
    /// 剔退原因DropDownList控件
    /// </summary>
    private void BindDropDownList()
    {
        try
        {
            //* 讀取公共屬性

            DataTable dtblProperty = null;
            if (!BaseHelper.GetCommonProperty("01", "AS_REJECTCODE", ref dtblProperty))
            {
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("01_03100000_001");
            }
            else
            {
                //* 將取得的【剔退原因】訊息顯示到報表種類DropDownList控件

                this.ddlReject_Reason.Items.Clear();

                #region 2014/09/17 剔退原因調整為原因碼為: 代碼+原因描述 by Eric
                this.ddlReject_Reason = AddRejectItemsL(this.ddlReject_Reason, dtblProperty);
                //this.ddlReject_Reason.DataSource = dtblProperty;
                //this.ddlReject_Reason.DataTextField = EntityM_PROPERTY_CODE.M_PROPERTY_NAME;
                //this.ddlReject_Reason.DataValueField = EntityM_PROPERTY_CODE.M_PROPERTY;
                //this.ddlReject_Reason.DataBind();
                #endregion

            }

        }

        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_03100000_001");
        }

    }

    #region 清除畫面二TextBox
    public void CleanText()
    {
        // 20220505 調整輸入信用卡號欄位，由4欄合併為1欄 By Kelton
        //this.txtCard_No1.Text = "";
        //this.txtCard_No2.Text = "";
        //this.txtCard_No3.Text = "";
        //this.txtCard_No4.Text = "";
        this.txtCard_No1.Text = "";
        this.txt_dpTran_Date.Text = "";
        this.txtAuth_Code.Text = "";
        this.txtAMT.Text = "";
        this.txtP1.Text = "";   //20160526 (U) by Tank, clern text

        Session["OFF_FLAG"] = "";
    }
    #endregion

    #region 清除畫面三TextBox
    public void CleanText3()
    {
        this.txtCard_No_1.Text = "";
        this.txtAMT_1.Text = "";

        Session["OFF_FLAG"] = "";
    }
    #endregion

    #region 清除第一次平帳畫面
    public void CleanViewFirst()
    {
        this.txtBatchDateP1.Text = "";
        this.txtShop_IDP1.Text = "";
        this.txtBatch_NOP1.Text = "";
        this.txtReceive_BatchP1.Text = "";
        this.txtReceive_Total_CountP1.Text = "";
        this.txtReceive_Total_AMTP1.Text = "";
        this.txtKeyin_Success_Count_All.Text = "";
        this.txtKeyin_Success_AMT_All.Text = "";
        this.txtKeyin_Success_Count_40.Text = "";
        this.txtKeyin_Success_AMT_40.Text = "";
        this.txtKeyin_Success_Count_41.Text = "";
        this.txtKeyin_Success_AMT_41.Text = "";
        this.txtKeyin_Reject_Count_All.Text = "";
        this.txtKeyin_Reject_AMT_All.Text = "";
        this.txtKeyin_Reject_Count_40.Text = "";
        this.txtKeyin_Reject_AMT_40.Text = "";
        this.txtKeyin_Reject_Count_41.Text = "";
        this.txtKeyin_Reject_AMT_41.Text = "";
        this.txtFirst_Balance_Count.Text = "";
        this.txtFirst_Balance_AMT.Text = "";

        this.btnFirst.Enabled = true;
    }
    #endregion

    /// <summary>
    /// 設定按鈕為新增或修改
    /// </summary>
    protected void SetButtonText()
    {
        //內頁鍵檔
        if (this.Session["OFF_FLAG"].ToString() == "1")
            btnConfirm.ShowID = "01_01060500_025";
        else
            btnConfirm.ShowID = "01_01060500_020";

        //內頁剔退鍵檔
        if (this.Session["OFF_FLAG_R"].ToString() == "1")
            btnReject.ShowID = "01_01060500_025";
        else
            btnReject.ShowID = "01_01060500_020";
    }

    /// <summary>
    /// 清除平帳資料
    /// </summary>
    protected void ClearBalance(string strBatch_Date, string strBatch_NO, string strShop_ID, string strReceive_Batch, string strModify_User)
    {
        string strMsgID = string.Empty;
        if (!BRArtificial_Signing_Primary.Update_ASP_Balance(strBatch_Date, strBatch_NO, strShop_ID, strReceive_Batch, strSign_Type, strKeyIn_Flag, strModify_User,
                            "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "N", "0", "0", "0", "0", "N", "0", "0", "N", ref strMsgID))
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
        }

        string strInitial = string.Empty;
        //第一次平帳資料
        this.txtKeyin_Success_Count_All.Text = strInitial;
        this.txtKeyin_Success_AMT_All.Text = strInitial;
        this.txtKeyin_Success_Count_40.Text = strInitial;
        this.txtKeyin_Success_AMT_40.Text = strInitial;
        this.txtKeyin_Success_Count_41.Text = strInitial;
        this.txtKeyin_Success_AMT_41.Text = strInitial;
        this.txtKeyin_Reject_Count_All.Text = strInitial;
        this.txtKeyin_Reject_AMT_All.Text = strInitial;
        this.txtKeyin_Reject_Count_40.Text = strInitial;
        this.txtKeyin_Reject_AMT_40.Text = strInitial;
        this.txtKeyin_Reject_Count_41.Text = strInitial;
        this.txtKeyin_Reject_AMT_41.Text = strInitial;
        this.txtFirst_Balance_Count.Text = strInitial;
        this.txtFirst_Balance_AMT.Text = strInitial;
        this.txtBatchDateP1.Text = strInitial;              //編批日期
        this.txtShop_IDP1.Text = strInitial;                //商店代號
        this.txtBatch_NOP1.Text = strInitial;               //批號
        this.txtReceive_BatchP1.Text = strInitial;          //收件批次
        this.txtReceive_Total_CountP1.Text = strInitial;    //收件-總筆數
        this.txtReceive_Total_AMTP1.Text = strInitial;      //收件-總金額

        //第二次平帳資料
        this.txtAdjust_Count.Text = strInitial;
        this.txtAdjust_AMT.Text = strInitial;
        this.txtSecond_Balance_Count.Text = strInitial;
        this.txtSecond_Balance_AMT.Text = strInitial;
    }

    /// <summary>
    /// 初始化平帳按鈕及輸入欄位
    /// </summary>
    protected void InitialBalance()
    {
        this.btnFirst.Enabled = true;
        this.btnSecond.Enabled = false;
        this.btnBalanceOK.Enabled = false;

        this.txtAdjust_Count.Enabled = true;
        this.txtAdjust_AMT.Enabled = true;
    }

    /// <summary>
    /// 初始化平帳欄位顏色
    /// </summary>
    protected void InitialBGColor()
    {
        Color c = Color.Empty;
        this.txtKeyin_Success_Count_All.BackColor = c;
        this.txtKeyin_Success_AMT_All.BackColor = c;
        this.txtKeyin_Success_Count_40.BackColor = c;
        this.txtKeyin_Success_AMT_40.BackColor = c;
        this.txtKeyin_Success_Count_41.BackColor = c;
        this.txtKeyin_Success_AMT_41.BackColor = c;
        this.txtKeyin_Reject_Count_All.BackColor = c;
        this.txtKeyin_Reject_AMT_All.BackColor = c;
        this.txtKeyin_Reject_Count_40.BackColor = c;
        this.txtKeyin_Reject_AMT_40.BackColor = c;
        this.txtKeyin_Reject_Count_41.BackColor = c;
        this.txtKeyin_Reject_AMT_41.BackColor = c;
        this.txtFirst_Balance_Count.BackColor = c;
        this.txtFirst_Balance_AMT.BackColor = c;

        this.txtAdjust_Count.BackColor = c;
        this.txtAdjust_AMT.BackColor = c;
        this.txtSecond_Balance_Count.BackColor = c;
        this.txtSecond_Balance_AMT.BackColor = c;
    }

    protected void SecondMsg(string msgNo)
    {
        btnBalanceOK.Enabled = false;
        MessageHelper.ShowMessage(this.UpdatePanel1, msgNo);
    }

    #endregion

    #region 2014/09/17 剔退原因調整為原因碼為: 代碼+原因描述 by Eric
    protected CustDropDownList AddRejectItemsL(CustDropDownList cddl, DataTable dt)
    {
        if (dt != null && dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string value = dt.Rows[i]["PROPERTY_CODE"] != null
                    ? dt.Rows[i]["PROPERTY_CODE"].ToString() : "";
                string text = dt.Rows[i]["PROPERTY_NAME"] != null
                    ? string.Format("{0} : {1}", value, dt.Rows[i]["PROPERTY_NAME"].ToString()) : "";

                cddl.Items.Add(new ListItem(text, value));
            }
        }
        return cddl;
    }
    #endregion


    //20160526 (U) by Tank, add Check2KeyData
    /// <summary>
    /// 檢查二Key資料是否與一Key資料一致
    /// </summary>
    public static System.Data.DataTable Check2KeyData(int intSN, string strBatch_Date, string strShop_ID, string strBatch_NO, string strReceive_Batch)
    {
        DataSet ds = null;
        SqlHelper sSql = new SqlHelper();

        SqlCommand sqlComm = new SqlCommand();
        try
        {
            //撈取1Key某筆資料明細
            sqlComm.CommandText = @"select SN,Card_No,Tran_Date,Auth_Code,AMT,Receipt_Type
                                from Artificial_Signing_Detail
                                where SN=@SN
                                and Batch_Date = @Batch_Date
                                and Shop_ID = @Shop_ID
                                and Batch_NO = @Batch_NO
                                and Receive_Batch = @Receive_Batch
                                and Case_Status = '0'
                                and KeyIn_Flag = '1' 
                                and Sign_Type = '1'";

            SqlParameter parmSN = new SqlParameter("@SN", intSN);
            SqlParameter parmBatch_Date = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
            SqlParameter parmShop_ID = new SqlParameter("@Shop_ID", strShop_ID);
            SqlParameter parmBatch_NO = new SqlParameter("@Batch_NO", strBatch_NO);
            SqlParameter parmReceive_Batch = new SqlParameter("@Receive_Batch", strReceive_Batch);

            sqlComm.Parameters.Add(parmSN);
            sqlComm.Parameters.Add(parmBatch_Date);
            sqlComm.Parameters.Add(parmShop_ID);
            sqlComm.Parameters.Add(parmBatch_NO);
            sqlComm.Parameters.Add(parmReceive_Batch);

            sqlComm.CommandType = CommandType.Text;
            ds = BRArtificial_Signing_Primary.SearchOnDataSet(sqlComm);
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
            throw ex;
        }

        return ds.Tables[0];
    }


}
