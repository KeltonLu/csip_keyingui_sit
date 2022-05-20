//******************************************************************
//*  作    者：蘇洺葳
//*  功能說明：郵局自扣刪除作業
//*  創建日期：2018/10/04
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
using System.Drawing;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.BusinessRules_new;
using CSIPKeyInGUI.EntityLayer;
using CSIPCommonModel.EntityLayer_new;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using CSIPCommonModel.EntityLayer;
public partial class P010214000001 : PageBase
{    /// <summary>
     /// Session變數集合
     /// </summary>
    private static EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ClearText();
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            this.btnDelete.Enabled = false;
        }

        base.strClientMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//*記錄網頁訊息
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtUserId.Text;

        //20200109-修改SOC存入條件
        //log.Statement_Text = string.Format("UserId:{0}", log.Customer_Id); //查詢條件內容: 用 | 區隔,要看文件確認
        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔

        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        ClearText();
        DataTable postOffice = null;

        try
        {
            // 查詢符合收件編號和身分證號碼的資料
            postOffice = BRPostOffice_Temp.GetPostOfficeDeleteItem(this.txtUserId.Text.Trim(), this.txtReceiveNumber.Text.Trim());
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (postOffice != null && postOffice.Rows.Count > 0)
        {
            lblAccNoText.Text = postOffice.Rows[0]["AccNo"].ToString().ToString();// 郵局帳號
            lblAccDepositText.Text = postOffice.Rows[0]["AccDeposit"].ToString();// 儲金帳號
            lblAccTypeText.Text = postOffice.Rows[0]["AccType"].ToString();// 帳戶別(P.存簿 G.劃撥)
            lblPayWayText.Text = postOffice.Rows[0]["Pay_Way"].ToString();// 扣款方式
            lblCusIDText.Text = postOffice.Rows[0]["Acc_ID"].ToString();// 扣款帳號所有人之ID
            lblBcycleCodeText.Text = postOffice.Rows[0]["bcycle_code"].ToString();// 帳單週期
            lblMobilePhoneText.Text = postOffice.Rows[0]["Mobile_Phone"].ToString();// 行動電話
            lblEMailText.Text = postOffice.Rows[0]["E_Mail"].ToString();// E-MAIL
            lblEBillText.Text = postOffice.Rows[0]["E_Bill"].ToString();// 電子帳單
            lblBuildDateText.Text = postOffice.Rows[0]["ModDate"].ToString();// 鍵檔日期
            lblApplyTypeText.Text = postOffice.Rows[0]["ApplyType"].ToString();// 申請類別
            lblRemarkText.Text = postOffice.Rows[0]["IsSendToPost"].ToString();// 處理註記
            lblRtnDateText.Text = postOffice.Rows[0]["ReturnDate"].ToString();// 回覆日期
            lblReturnCodeText.Text = postOffice.Rows[0]["PostRtnMsg"].ToString();// 郵局回覆碼
            lblCodeText.Text = postOffice.Rows[0]["IsUpload"].ToString();// 生效碼

            this.btnDelete.Enabled = true;
        }
        else
        {
            ClearText();
            this.btnDelete.Enabled = false;
            base.strClientMsg += MessageHelper.GetMessage("01_02140000_001");
            return;
        }
    }

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        base.sbRegScript.Append("if(confirm('" + MessageHelper.GetMessage("01_02140000_002") + "')) {$('#btnHiden').click();}else{document.getElementById('txtUserId').focus();}");
    }

    /// <summary>
    /// 確定刪除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnHiden_Click(object sender, EventArgs e)
    {
        if (BRPostOffice_Temp.DeletePostOfficeItem(this.txtUserId.Text.Trim(), this.txtReceiveNumber.Text.Trim()))
        {
            ClearAll();
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_02140000_004");
        }
    }

    /// <summary>
    /// 清空網頁中的所有欄位，【收件編號】按鈕為網頁焦點，【刪除】按鈕disable
    /// </summary>
    private void ClearAll()
    {
        ClearText();
        base.strClientMsg += MessageHelper.GetMessage("01_02140000_003");
        this.txtReceiveNumber.Text = "";
        this.txtUserId.Text = "";
        base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
        this.btnDelete.Enabled = false;
    }

    /// <summary>
    /// 清空文本
    /// </summary>
    private void ClearText()
    {
        this.lblAccNoText.Text = "";
        this.lblAccDepositText.Text = "";
        this.lblAccTypeText.Text = "";
        this.lblPayWayText.Text = "";
        this.lblCusIDText.Text = "";
        this.lblBcycleCodeText.Text = "";
        this.lblMobilePhoneText.Text = "";
        this.lblEMailText.Text = "";
        this.lblEBillText.Text = "";
        this.lblBuildDateText.Text = "";
        this.lblApplyTypeText.Text = "";
        this.lblRemarkText.Text = "";
        this.lblRtnDateText.Text = "";
        this.lblReturnCodeText.Text = "";
        this.lblCodeText.Text = "";
    }
}
