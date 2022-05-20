//******************************************************************
//*  作    者：趙呂梁

//*  功能說明：卡片資料異動-繳款異動資料

//*  創建日期：2009/09/02
//*  修改記錄：


//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
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
using CSIPCommonModel.EntityLayer;
using Framework.WebControls;
using Framework.Common.Message;
using CSIPKeyInGUI.BusinessRules;

public partial class P010102030001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
            CommonFunction.SetControlsEnabled(pnlText, false);
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"]; //*記錄網頁訊息
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/09/02
    /// 修改日期：2009/09/02 
    /// <summary>
    /// 查詢事件
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Account_Nbr = this.txtCardNo.Text;

        //20200109-修改SOC存入條件
        //log.Statement_Text = string.Format("AC_NO:{0}", log.Account_Nbr); //查詢條件內容: 用 | 區隔,要看文件確認
        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔

        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        //*添加上傳主機信息
        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT_NBR", this.txtCardNo.Text.Trim());
        htInput.Add("FUNCTION_CODE", "1");
        htInput.Add("LINE_CNT", "0000");

        //*得到主機傳回信息
        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCF7, htInput, false, "1", eAgentInfo);

        string strMsg = "";//*主機返回錯誤信息
        Hashtable htResult = new Hashtable();//*P4_JCAA返回電文信息的Hashtable
        string htReturnStr = "";

        this.lblNameText.Text = "";
        if (!htReturn.Contains("HtgMsg") && CommonFunction.GetJCAAMainframeData(this.txtCardNo.Text.Trim(), ref htResult, eAgentInfo, ref strMsg))
        {
            CommonFunction.SetControlsEnabled(pnlText, true);
            lblNameText.Text = htReturn["SHORT_NAME"].ToString();//*姓名
            txtDealFund.Text = htReturn["PYMT_FLAG"].ToString();//*繳款形式
            //*固定繳款額
            htReturnStr = htReturn["FIXED_PYMT_AMNT"].ToString();
            htReturnStr = htReturnStr == "" ? BRCommon.GetPadLeftString("0", 9, '0') : htReturnStr.Substring(0, htReturnStr.Length - 2); //將小數兩位去除
            htReturn["FIXED_PYMT_AMNT"] = htReturnStr;
            txtFixupDealFund.Text = double.Parse(htReturnStr).ToString();

            //*本期應繳
            htReturnStr = htReturn["CURR_DUE"].ToString();
            htReturnStr = htReturnStr == "" ? BRCommon.GetPadLeftString("0", 9, '0') : htReturnStr.Substring(0, htReturnStr.Length - 2); //將小數兩位去除
            htReturn["CURR_DUE"] = htReturnStr;
            txtThisDealFund.Text = double.Parse(htReturnStr).ToString();

            //*逾期一個月內
            htReturnStr = htReturn["PAST_DUE"].ToString();
            htReturnStr = htReturnStr == "" ? BRCommon.GetPadLeftString("0", 9, '0') : htReturnStr.Substring(0, htReturnStr.Length - 2); //將小數兩位去除
            htReturn["PAST_DUE"] = htReturnStr;
            txtDays1.Text = double.Parse(htReturnStr).ToString();

            //30Days
            htReturnStr = htReturn["30DAYS_DELQ"].ToString();
            htReturnStr = htReturnStr == "" ? BRCommon.GetPadLeftString("0", 9, '0') : htReturnStr.Substring(0, htReturnStr.Length - 2); //將小數兩位去除
            htReturn["30DAYS_DELQ"] = htReturnStr;
            txtDays2.Text = double.Parse(htReturnStr).ToString();

            //60Days
            htReturnStr = htReturn["60DAYS_DELQ"].ToString();
            htReturnStr = htReturnStr == "" ? BRCommon.GetPadLeftString("0", 9, '0') : htReturnStr.Substring(0, htReturnStr.Length - 2); //將小數兩位去除
            htReturn["60DAYS_DELQ"] = htReturnStr;
            txtDays3.Text = double.Parse(htReturnStr).ToString();

            //90Days
            htReturnStr = htReturn["90DAYS_DELQ"].ToString();
            htReturnStr = htReturnStr == "" ? BRCommon.GetPadLeftString("0", 9, '0') : htReturnStr.Substring(0, htReturnStr.Length - 2); //將小數兩位去除
            htReturn["90DAYS_DELQ"] = htReturnStr;
            txtDays4.Text = double.Parse(htReturnStr).ToString();

            //120Days
            htReturnStr = htReturn["120DAYS_DELQ"].ToString();
            htReturnStr = htReturnStr == "" ? BRCommon.GetPadLeftString("0", 9, '0') : htReturnStr.Substring(0, htReturnStr.Length - 2); //將小數兩位去除
            htReturn["120DAYS_DELQ"] = htReturnStr;
            txtDays5.Text = double.Parse(htReturnStr).ToString();

            //150Days
            htReturnStr = htReturn["150DAYS_DELQ"].ToString();
            htReturnStr = htReturnStr == "" ? BRCommon.GetPadLeftString("0", 9, '0') : htReturnStr.Substring(0, htReturnStr.Length - 2); //將小數兩位去除
            htReturn["150DAYS_DELQ"] = htReturnStr;
            txtDays6.Text = double.Parse(htReturnStr).ToString();

            //180Days
            htReturnStr = htReturn["180DAYS_DELQ"].ToString();
            htReturnStr = htReturnStr == "" ? BRCommon.GetPadLeftString("0", 9, '0') : htReturnStr.Substring(0, htReturnStr.Length - 2); //將小數兩位去除
            htReturn["180DAYS_DELQ"] = htReturnStr;
            txtDays7.Text = double.Parse(htReturnStr).ToString();

            //210Days
            htReturnStr = htReturn["210DAYS_DELQ"].ToString();
            htReturnStr = htReturnStr == "" ? BRCommon.GetPadLeftString("0", 9, '0') : htReturnStr.Substring(0, htReturnStr.Length - 2); //將小數兩位去除
            htReturn["210DAYS_DELQ"] = htReturnStr;
            txtDays8.Text = double.Parse(htReturnStr).ToString();

            ViewState["HtgInfo"] = htReturn;//*保存P4_JCF7主機查詢資料
            ViewState["TYPE"] = htResult["TYPE"].ToString();//*保存P4_JCAA主機查詢TYPE資料

            base.sbRegScript.Append(BaseHelper.SetFocus("txtDealFund"));
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_012");
            base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
        }
        else
        {
            CommonFunction.SetControlsEnabled(pnlText, false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));

            if (htReturn.Contains("HtgMsg"))//*查詢P4_JCF7失敗
            {
                //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
                etMstType = eMstType.Select;
                //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

                if (htReturn["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htReturn["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
                }
                else
                {
                    base.strClientMsg += htReturn["HtgMsg"].ToString();
                }
            }
            else//*查詢P4_JCAA失敗
            {
                base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息
                if (!htResult.Contains("HtgMsg"))
                {
                    base.strClientMsg += strMsg;
                    base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                }
                else
                {
                    //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
                    etMstType = eMstType.Select;
                    //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

                    if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                    {
                        base.strHostMsg += strMsg;
                        base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
                    }
                    else
                    {
                        base.strClientMsg += strMsg;
                    }
                }
            }
        }
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/09/02
    /// 修改日期：2009/09/02  
    /// <summary>
    /// 提交事件
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        Hashtable htJCF7Return = (Hashtable)ViewState["HtgInfo"];
        ArrayList arrayName = new ArrayList(new object[] { "ACCT_NBR", "PYMT_FLAG", "FIXED_PYMT_AMNT", "CURR_DUE", "PAST_DUE", "30DAYS_DELQ",
                                                                                            "60DAYS_DELQ","90DAYS_DELQ","120DAYS_DELQ","150DAYS_DELQ","180DAYS_DELQ","210DAYS_DELQ"});
        Hashtable htOutput = new Hashtable();
        MainFrameInfo.ChangeJCF7toPCTI(htJCF7Return, htOutput, arrayName, ViewState["TYPE"].ToString());

        DataTable dtblUpdate = CommonFunction.GetDataTable();
        //將相關數值欄位補滿 9 個0
        string strFIXED_PAYMENT = BRCommon.GetPadLeftString(this.txtFixupDealFund.Text.Trim(), 9, '0');
        string strCURR = BRCommon.GetPadLeftString(this.txtThisDealFund.Text.Trim(), 9, '0');
        string strXDAY = BRCommon.GetPadLeftString(this.txtDays1.Text.Trim(), 9, '0');
        string str30DAY = BRCommon.GetPadLeftString(this.txtDays2.Text.Trim(), 9, '0');
        string str60DAY = BRCommon.GetPadLeftString(this.txtDays3.Text.Trim(), 9, '0');
        string str90DAY = BRCommon.GetPadLeftString(this.txtDays4.Text.Trim(), 9, '0');
        string str120DAY = BRCommon.GetPadLeftString(this.txtDays5.Text.Trim(), 9, '0');
        string str150DAY = BRCommon.GetPadLeftString(this.txtDays6.Text.Trim(), 9, '0');
        string str180DAY = BRCommon.GetPadLeftString(this.txtDays7.Text.Trim(), 9, '0');
        string str210DAY = BRCommon.GetPadLeftString(this.txtDays8.Text.Trim(), 9, '0');

        //
        if (this.txtDealFund.Text.Trim() != htOutput["PAYMENT_TYPE"].ToString() || strFIXED_PAYMENT + "00" != htOutput["FIXED_PAYMENT"].ToString())
        {
            if (htOutput["PAYMENT_TYPE"].ToString() == "8" && this.txtDealFund.Text.Trim() != htOutput["PAYMENT_TYPE"].ToString() && strFIXED_PAYMENT + "00" != htOutput["FIXED_PAYMENT"].ToString())
            {
                //*比對繳款形式
                CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, this.txtDealFund.Text.Trim(), "PAYMENT_TYPE", BaseHelper.GetShowText("01_01020300_004"));

                //*比對固定繳款額
                CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, strFIXED_PAYMENT, "FIXED_PAYMENT", BaseHelper.GetShowText("01_01020300_005"));
            }
            else if (htOutput["PAYMENT_TYPE"].ToString() == "0" && this.txtDealFund.Text.Trim() != htOutput["PAYMENT_TYPE"].ToString() && strFIXED_PAYMENT != htOutput["FIXED_PAYMENT"].ToString())
            {
                //*比對繳款形式
                CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, this.txtDealFund.Text.Trim(), "PAYMENT_TYPE", BaseHelper.GetShowText("01_01020300_004"));

                //*比對固定繳款額
                CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, strFIXED_PAYMENT, "FIXED_PAYMENT", BaseHelper.GetShowText("01_01020300_005"));
            }
            else if (htOutput["PAYMENT_TYPE"].ToString() == "8" && this.txtDealFund.Text.Trim() == htOutput["PAYMENT_TYPE"].ToString() && strFIXED_PAYMENT != htOutput["FIXED_PAYMENT"].ToString())
            {
                //*比對繳款形式
                CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, this.txtDealFund.Text.Trim(), "PAYMENT_TYPE", BaseHelper.GetShowText("01_01020300_004"));

                //*比對固定繳款額
                CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, strFIXED_PAYMENT, "FIXED_PAYMENT", BaseHelper.GetShowText("01_01020300_005"));
            }
            else
            {
                htOutput.Remove("PAYMENT_TYPE");
                htOutput.Remove("FIXED_PAYMENT");
            }
        }
        else
        {
            htOutput.Remove("PAYMENT_TYPE");
            htOutput.Remove("FIXED_PAYMENT");
        }

        //*比對本期應繳
        CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, strCURR, "CURR", BaseHelper.GetShowText("01_01020300_006"));

        //*比對預期一個月內
        CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, strXDAY, "XDAY", BaseHelper.GetShowText("01_01020300_007"));

        //*比對30天
        CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, str30DAY, "30DAY", BaseHelper.GetShowText("01_01020300_008"));

        //*比對60天
        CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, str60DAY, "60DAY", BaseHelper.GetShowText("01_01020300_009"));

        //*比對90天
        CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, str90DAY, "90DAY", BaseHelper.GetShowText("01_01020300_010"));

        //*比對120天
        CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, str120DAY, "120DAY", BaseHelper.GetShowText("01_01020300_011"));

        //*比對150天
        CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, str150DAY, "150DAY", BaseHelper.GetShowText("01_01020300_012"));

        //*比對180天
        CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, str180DAY, "180DAY", BaseHelper.GetShowText("01_01020300_013"));

        //*比對210天
        CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, str210DAY, "210DAY", BaseHelper.GetShowText("01_01020300_014"));

        //*輸入內容與主機資料不同

        if (dtblUpdate.Rows.Count > 0)
        {
            htOutput.Add("FUNCTION_ID", "PCMH1");
            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_PCTI, htOutput, false, "2", eAgentInfo);
            //*異動成功
            if (!htResult.Contains("HtgMsg"))
            {
                if (!CommonFunction.InsertCustomerLog(dtblUpdate, eAgentInfo, this.txtCardNo.Text.Trim(), "P4", (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }
                if (BRTRANS_NUM.UpdateTransNum("B02") == false)
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_014");
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息  
                CommonFunction.SetControlsEnabled(pnlText, false);
                this.lblNameText.Text = "";
                this.txtCardNo.Text = "";
            }
            else
            {
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_027");
                }
                else
                {
                    base.strClientMsg += htResult["HtgMsg"].ToString();
                }
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_015");
        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
    }
    #endregion
}
