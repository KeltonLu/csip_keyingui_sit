//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：卡片資料異動-狀況碼資料

//*  創建日期：2009/08/31
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
using Framework.Common.Message;
using CSIPKeyInGUI.BusinessRules;

public partial class P010102020001 : PageBase
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
    /// 創建日期：2009/08/31
    /// 修改日期：2009/08/31 
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
        lblNameText.Text = "";
        Hashtable htResult = new Hashtable();//*P4_JCAA返回電文信息的Hashtable
        if (!htReturn.Contains("HtgMsg") && CommonFunction.GetJCAAMainframeData(this.txtCardNo.Text.Trim(), ref htResult, eAgentInfo, ref strMsg))
        {
            htReturn["ACCT_NBR"] = htInput["ACCT_NBR"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值
            htResult["ACCT_NBR"] = htInput["ACCT_NBR"];//* for_xml_test 
            CommonFunction.SetControlsEnabled(pnlText, true);
            lblNameText.Text = htReturn["SHORT_NAME"].ToString();//*姓名
            txtStatusCode.Text = htReturn["CHGOFF_STATUS_FLAG"].ToString();//*狀況碼
            ViewState["HtgInfo"] = htReturn;//*保存P4_JCF7主機查詢資料
            ViewState["TYPE"] = htResult["TYPE"].ToString();//*保存P4_JCAA主機查詢TYPE資料
            base.sbRegScript.Append(BaseHelper.SetFocus("txtStatusCode"));
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
    /// 創建日期：2009/08/31
    /// 修改日期：2009/08/31 
    /// <summary>
    /// 提交事件
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        ArrayList arrayName = new ArrayList(new object[] { "ACCT_NBR", "CHGOFF_STATUS_FLAG" });

        Hashtable htOutput = new Hashtable();
        MainFrameInfo.ChangeJCF7toPCTI((Hashtable)ViewState["HtgInfo"], htOutput, arrayName, ViewState["TYPE"].ToString());
        DataTable dtblUpdate = CommonFunction.GetDataTable();

        //*比對狀況碼
        CommonFunction.ContrastDataEdit(htOutput, dtblUpdate, this.txtStatusCode.Text.Trim(), "STATUS_FLAG", BaseHelper.GetShowText("01_01020200_004"));

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

                if (BRTRANS_NUM.UpdateTransNum("B05") == false)
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
                lblNameText.Text = "";
            }
            else
            {
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_016");
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
