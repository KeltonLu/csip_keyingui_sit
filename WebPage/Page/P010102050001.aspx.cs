//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：卡片資料異動-優惠碼

//*  創建日期：2009/09/08
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
using Framework.Common.Message;
using CSIPKeyInGUI.BusinessRules;
using CSIPCommonModel.EntityLayer;

public partial class P010102050001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #endregion

    /// <summary>
    /// 頁面加載事件
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CommonFunction.SetControlsEnabled(pnlText, false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)Session["Agent"];
        sPageInfo = (structPageInfo)this.Session["PageInfo"]; //*記錄網頁訊息
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/08
    /// 修改日期：2009/09/08 
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
        Hashtable htResult = new Hashtable();//*P4_JCAA主機傳回信息

        this.lblNameText.Text = "";
        if (!htReturn.Contains("HtgMsg") && CommonFunction.GetJCAAMainframeData(this.txtCardNo.Text.Trim(), ref htResult, eAgentInfo, ref strMsg))
        {
            ViewState["HtgInfo"] = htReturn;//*保存P4_JCDH主機查詢資料
            ViewState["TYPE"] = htResult["TYPE"].ToString();//*保存P4_JCAA主機查詢TYPE資料
            htReturn["ACCT_NBR"] = htInput["ACCT_NBR"];//* for_xml_test        
            CommonFunction.SetControlsEnabled(pnlText, true);
            lblNameText.Text = htReturn["SHORT_NAME"].ToString();
            txtFavourableCode.Text = htReturn["USER_CODE"].ToString();//*優惠碼
            txtCardType.Text = htReturn["USER_CODE_2"].ToString();//*卡人類別
            base.sbRegScript.Append(BaseHelper.SetFocus("txtFavourableCode"));
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_012");
            base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
        }
        else
        {
            CommonFunction.SetControlsEnabled(pnlText, false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));

            if (htReturn.Contains("HtgMsg"))//*查詢P4_JCDH失敗
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
    /// 創建日期：2009/09/08
    /// 修改日期：2009/09/08 
    /// <summary>
    /// 提交事件
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        Hashtable htReturn = (Hashtable)ViewState["HtgInfo"];
        ArrayList arrayName = new ArrayList(new object[] { "ACCT_NBR", "USER_CODE", "USER_CODE_2" });//*P4_JCDH欄位名稱
        Hashtable htOutput = new Hashtable();
        MainFrameInfo.ChangeJCF7toPCTI(htReturn, htOutput, arrayName, ViewState["TYPE"].ToString());
        DataTable dtblUpdate = CommonFunction.GetDataTable();

        //*比對優惠碼
        CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, this.txtFavourableCode.Text.Trim(), "CHPM_CODE", BaseHelper.GetShowText("01_01020500_004"));
        //*比對卡人類別
        CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, this.txtCardType.Text.Trim(), "CATEGORY", BaseHelper.GetShowText("01_01020500_005"));

        if (dtblUpdate.Rows.Count > 0)
        {
            htOutput.Add("FUNCTION_ID", "PCMH1");
            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_PCTI, htOutput, false, "2", eAgentInfo);
            if (!htResult.Contains("HtgMsg"))
            {
                //*資料庫中寫入Log
                if (!CommonFunction.InsertCustomerLog(dtblUpdate, eAgentInfo, this.txtCardNo.Text.Trim(), "P4", (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }
                if (BRTRANS_NUM.UpdateTransNum("B04") == false)
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_014");
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                CommonFunction.SetControlsEnabled(pnlText, false);
                this.txtCardNo.Text = "";
                this.lblNameText.Text = "";
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
}
