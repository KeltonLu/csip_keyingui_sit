//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：卡人資料異動-異動地址

//*  創建日期：2009/09/29
//*  修改記錄：
//*  <author>            <time>            <TaskID>                <desc>
//* Joe              2021/07/23       RQ-2021-019992-000        增設帳單地址檢核(不可連續7個數字)
//*  Ares_Stanley        2021/11/01        20200031-CSIP EOS       理專十誡，JCDK改JCBG，註解P4D環境相關程式。
//*******************************************************************
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CSIPKeyInGUI.BusinessRules;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;
using Framework.Data.OM.Collections;
using CSIPKeyInGUI.EntityLayer;
using System.Drawing;
using Framework.Common.JavaScript;
using System.Linq;
using System.Collections.Generic;
using CSIPCommonModel.BusinessRules_new;
using Framework.Common.Logging;
using System.Text.RegularExpressions;

public partial class P010101010001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息

    /// <summary>
    /// 住家電話(主機值)
    /// </summary>
    private string strOHomeTel = "";

    /// <summary>
    /// 公司電話（主機值）
    /// </summary>
    private string strOOfficeTel = "";

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUserId"));
            CommonFunction.SetControlsEnabled(pnlText, false);
        }
        this.txtBill.ForeColor = Color.Black;
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/29
    /// 修改日期：2009/09/29
    /// <summary>
    /// 查詢事件
    /// </summary>
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

        CommonFunction.SetControlsEnabled(pnlText, false);
        this.lblNameText.Text = "";

        //*得到主機傳回信息
        Hashtable htReturnOne = new Hashtable();
        Hashtable htReturnP4_JCF6 = new Hashtable();
        //Hashtable htReturnP4D_JCF6 = new Hashtable();
        Hashtable htReturnP4_JCBG = new Hashtable();

        bool bReturnP4_JCF6 = false;
        //bool bReturnP4D_JCF6 = false;
        bool bReturnP4_JCBG = false;

        string lblTempEmailStatusText = string.Empty;

        ViewState["flagP4_JCF6"] = "0";
        //ViewState["flagP4D_JCF6"] = "0";
        ViewState["flagP4_JCBG"] = "0";

        bReturnP4_JCF6 = GetMainframeData(HtgType.P4_JCF6, ref htReturnP4_JCF6, "1");
        //bReturnP4D_JCF6 = GetMainframeData(HtgType.P4D_JCF6, ref htReturnP4D_JCF6, "12");

        if (bReturnP4_JCF6)
        {
            ViewState["flagP4_JCF6"] = "1";

            base.strClientMsg = MessageHelper.GetMessage("01_00000000_012");
            base.strHostMsg = htReturnP4_JCF6["HtgSuccess"].ToString();

            bReturnP4_JCBG = GetMainframeData(HtgType.P4_JCBG, ref htReturnP4_JCBG, "1");
            if (bReturnP4_JCBG)
            {
                ViewState["flagP4_JCBG"] = "1";
                base.strHostMsg += htReturnP4_JCBG["HtgSuccess"].ToString();
            }
            else
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_037");
                base.strHostMsg += htReturnP4_JCBG["HtgMsg"].ToString();
            }

        }
        else
        {
            base.strClientMsg = MessageHelper.GetMessage("01_00000000_035");
            base.strHostMsg = htReturnP4_JCF6["HtgMsg"].ToString();
        }

        //Start 2021/11/01  理專十誡，註解P4D環境相關程式 by  Ares_Stanley
        //if (bReturnP4D_JCF6)
        //{
        //    ViewState["flagP4D_JCF6"] = "1";
        //    base.strClientMsg = MessageHelper.GetMessage("01_00000000_012");
        //    base.strHostMsg += htReturnP4D_JCF6["HtgSuccess"].ToString();
        //}
        //else
        //{
        //    if (!bReturnP4_JCF6)
        //    {
        //        base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
        //    }
        //    base.strHostMsg += htReturnP4D_JCF6["HtgMsg"].ToString();
        //}
        //End 2021/11/01  理專十誡，註解P4D環境相關程式 by  Ares_Stanley


        if (bReturnP4_JCF6)
        {

            htReturnOne = htReturnP4_JCF6;
        }
        else
        {
            //Start 2021/11/01  理專十誡，註解P4D環境相關程式 by  Ares_Stanley
            //if (bReturnP4D_JCF6)
            //{
            //    htReturnOne = htReturnP4D_JCF6;

            //}
            //End 2021/11/01  理專十誡，註解P4D環境相關程式 by  Ares_Stanley
        }

        //*查詢主機第一卡人檔P4、第二卡人檔P4、PCMCP4

        //if (bReturnP4_JCF6 || bReturnP4D_JCF6) //2021/11/01  理專十誡，註解P4D環境相關程式 by  Ares_Stanley
        if (bReturnP4_JCF6)
        {
            ViewState["HtgInfoOne"] = htReturnOne;//*保存主機查詢資料
            ViewState["HtgInfoOneP4"] = htReturnP4_JCF6;//*保存主機查詢資料P4

            //2021/11/01  理專十誡，註解P4D環境相關程式 by  Ares_Stanley
            /*ViewState["HtgInfoOneP4D"] = htReturnP4D_JCF6;*///*保存主機查詢資料P4D
            ViewState["HtgInfoTwo"] = htReturnP4_JCBG;

            CommonFunction.SetControlsEnabled(pnlText, true);
            lblNameText.Text = htReturnOne["NAME_1"].ToString();//*姓名
            txtCityName.Text = htReturnOne["CITY"].ToString();//*城市
            txtMerchantAddOne.Text = htReturnOne["ADDR_1"].ToString();//*帳單地址1
            txtMerchantAddTwo.Text = htReturnOne["ADDR_2"].ToString();//*帳單地址2
            txtCompanyName.Text = htReturnOne["EMPLOYER"].ToString();//*公司名稱
            txtHomeTel1.Text = CommonFunction.GetSubString(htReturnOne["HOME_PHONE"].ToString().Trim(), 0, 3);//*住家電話1
            txtHomeTel2.Text = CommonFunction.GetSubString(htReturnOne["HOME_PHONE"].ToString().Trim(), 4, 14);//*住家電話2
            txtCompanyTel1.Text = CommonFunction.GetSubString(htReturnOne["OFFICE_PHONE"].ToString().Trim(), 0, 3);//*公司電話1
            txtCompanyTel2.Text = CommonFunction.GetSubString(htReturnOne["OFFICE_PHONE"].ToString().Trim(), 4, 14);//*公司電話2
            txtPostalDistrict.Text = htReturnOne["MANAGE_ZIP_CODE"].ToString();//*管理郵區
            txtZipCode.Text = htReturnOne["ZIP"].ToString();//*郵遞區號
            txtRemarkOne.Text = htReturnOne["MEMO_1"].ToString();//*註記一
            txtRemarkTwo.Text = htReturnOne["MEMO_2"].ToString();//*註記二
            txtBill.Text = htReturnOne["OFF_PHONE_FLAG"].ToString();

            //2021/11/01  理專十誡，註解P4D環境相關程式 by  Ares_Stanley
            //if (bReturnP4_JCDK && bReturnP4_JCF6)
            if (bReturnP4_JCBG && bReturnP4_JCF6)
            {

                txtRegTel1.Text = CommonFunction.GetSubString(htReturnP4_JCBG["TEL_PERM"].ToString(), 0, 3).Trim();//*戶籍電話1
                txtRegTel2.Text = CommonFunction.GetSubString(htReturnP4_JCBG["TEL_PERM"].ToString(), 4, 14).Trim(); //*戶籍電話2
                txtMobile.Text = htReturnP4_JCBG["MOBILE_PHONE"].ToString();//*移動電話
                txtEmail.Text = htReturnP4_JCBG["EMAIL"].ToString();//*Email
                txtTempEmail.Text = htReturnP4_JCBG["TEMP_E_MAIL"].ToString().Trim();//*暫存Email
                switch (htReturnP4_JCBG["TEMP_E_MAIL_STATUS"].ToString())
                {
                    case "P":
                        lblTempEmailStatusText = "P-待驗證";
                        break;
                    case "Y":
                        lblTempEmailStatusText = "Y-已驗證";
                        break;
                    case "X":
                        lblTempEmailStatusText = "X-已過期未驗證";
                        break;
                    default:
                        lblTempEmailStatusText = "";
                        break;
                }
                lblTempEmailStatus.Text = lblTempEmailStatusText;
            }
            else
            {
                SetControlsEnabled(false);
            }

            base.sbRegScript.Append(BaseHelper.SetFocus("txtPostalDistrict"));
        }
        else
        {
            CommonFunction.SetEnabled(pnlText, false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUserId"));
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/29
    /// 修改日期：2009/09/29
    /// <summary>
    /// 提交事件
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        //*  2021/07/23   Joe  增設帳單地址檢核(不可連續7個數字)
        string norrowMerchantAddOne = ToNarrow(this.txtMerchantAddOne.Text);
        string norrowMerchantAddTwo = ToNarrow(this.txtMerchantAddTwo.Text);

        if (Regex.IsMatch(norrowMerchantAddOne, "[0-9]{7}") || Regex.IsMatch(norrowMerchantAddTwo, "[0-9]{7}"))
        {
            base.sbRegScript.Append("alert('帳單地址內容不可連續7個數字，請重新輸入');");
            return;
        }


        EntitySet<EntitySZIP> SZIPSet = null;
        try
        {
            SZIPSet = BRSZIP.SelectEntitySet(txtCityName.Text.Trim());
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        //清空上一次USER選擇是否發送Email驗證的值
        this.hidSendMailVerifyFlag.Text = "";

        if (SZIPSet != null && SZIPSet.Count > 0)
        {
            txtZipCode.Text = SZIPSet.GetEntity(0).zip_code;
            if (txtPostalDistrict.Text.Trim() != "013" && txtPostalDistrict.Text.Trim() != "006" && txtPostalDistrict.Text.Trim() != "001")
            {
                txtPostalDistrict.Text = SZIPSet.GetEntity(0).zip_code;
            }

            btnCheckEsbHtgStatus_Click(sender, e);

        }
        else
        {
            base.sbRegScript.Append("if(confirm('" + MessageHelper.GetMessage("01_01010100_008") + "')) {$('#btnCheckEsbHtgStatus').click();}else{document.getElementById('txtCityName').focus();}");
        }
    }

    /// 作者 Ares Stanley
    /// 創建日期：2021/10/26
    /// 修改日期：2001/10/26
    /// <summary>
    /// 檢核資料是否需送驗證
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCheckEsbHtgStatus_Click(object sender, EventArgs e)
    {
        Hashtable htReturnP4_JCBG = new Hashtable();
        string strflagP4_JCBG = ViewState["flagP4_JCBG"].ToString();
        string oriEBill = this.txtBill.Text.Trim();
        string validSelValue = string.Empty;
        string blockMsg = string.Empty;
        bool useRTDS = false;
        bool askUser = false;

        if (strflagP4_JCBG == "1")
        {
            //取JCBG值
            CommonFunction.GetViewStateHt(ViewState["HtgInfoTwo"], ref htReturnP4_JCBG);

            GetEsbHtgStatus(htReturnP4_JCBG["EMAIL"].ToString().Trim(), htReturnP4_JCBG["TEMP_E_MAIL"].ToString().Trim(), this.txtTempEmail.Text.Trim(), htReturnP4_JCBG["TEMP_E_MAIL_STATUS"].ToString().Trim(), ref useRTDS, ref validSelValue, ref blockMsg, ref askUser);

            if (!string.IsNullOrEmpty(blockMsg))
            {
                base.sbRegScript.Append(string.Format("alert('{0}');", blockMsg));
                return;
            }

            if (askUser)
            {
                base.sbRegScript.Append("openDialog();");
                return;
            }
        }

        btnHiden_Click(sender, e);
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/29
    /// 修改日期：2009/09/29
    /// <summary>
    /// 提交事件
    /// </summary>
    protected void btnHiden_Click(object sender, EventArgs e)
    {
        Hashtable htReturnP4_JCF6 = new Hashtable();
        //2021/11/01  理專十誡，註解P4D環境相關程式 by  Ares_Stanley
        //Hashtable htReturnP4D_JCF6 = new Hashtable();
        Hashtable htReturnP4_JCBG = new Hashtable();

        Hashtable htReturnP4_JCBGViewState = new Hashtable();
        DataTable dtblUpdate = CommonFunction.GetDataTable();
        Hashtable htOneCard = new Hashtable();
        Hashtable htOutputP4 = new Hashtable();
        //2021/11/01  理專十誡，註解P4D環境相關程式 by  Ares_Stanley
        //Hashtable htOutputP4D = new Hashtable();

        DataTable dtCustCheckInfo = new DataTable();
        DataTable dtReplyCustCheckInfo = new DataTable();
        DataTable dtESBStatus = new DataTable();
        DataTable dtJCBGStatus = new DataTable();

        bool bSucc = true;
        bool sameFlag = false;
        bool usePCTIToUpdateEBill = true;
        bool useRTDS = false;
        bool askUser = false;

        string strflagP4_JCF6 = ViewState["flagP4_JCF6"].ToString();

        //2021/11/01  理專十誡，註解P4D環境相關程式 by  Ares_Stanley
        //string strflagP4D_JCF6 = ViewState["flagP4D_JCF6"].ToString();
        string strflagP4_JCBG = ViewState["flagP4_JCBG"].ToString();
        string ESBResult = string.Empty;
        string RTDSCheckTime = string.Empty;
        string oriEBill = this.txtBill.Text.Trim();
        string validSelValue = string.Empty;
        string blockMsg = string.Empty;
        string errorMsg = string.Empty;
        string rtnHtgMsgType = string.Empty;
        string rtnHtgMsg = string.Empty;

        string TransactionID_Search = "CSIP" + DateTime.Now.ToString("yyyyMMddHHmmssff");
        string TransactionID_Edit = "CSIP" + DateTime.Now.ToString("yyyyMMddHHmmssff");

        if (strflagP4_JCF6 == "1")
        {
            //查詢P4環境成功
            htReturnP4_JCF6 = (Hashtable)ViewState["HtgInfoOneP4"];

            CommonFunction.GetViewStateHt(ViewState["HtgInfoTwo"], ref htReturnP4_JCBG);

            htOneCard = (Hashtable)htReturnP4_JCF6.Clone();//*得到主機第一卡人檔P4信息

            //*檢核電子帳單
            if (this.txtBill.Text.Trim() != htReturnP4_JCF6["OFF_PHONE_FLAG"].ToString())
            {
                if (htReturnP4_JCF6["OFF_PHONE_FLAG"].ToString() != "0" && htReturnP4_JCF6["OFF_PHONE_FLAG"].ToString() != "2")
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01010100_009") + htReturnP4_JCF6["OFF_PHONE_FLAG"].ToString() + MessageHelper.GetMessage("01_01010100_011");
                    this.txtBill.Text = htReturnP4_JCF6["OFF_PHONE_FLAG"].ToString();
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtBill"));
                    this.txtBill.ForeColor = Color.Red;
                    return;
                }
            }
        }

        if (!GetAllCodeMsg(ref dtCustCheckInfo, ref dtReplyCustCheckInfo, ref dtESBStatus, ref dtJCBGStatus))
        {
            base.sbRegScript.Append("alert('取得電文代碼訊息時發生錯誤！');");
            return;
        }

        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG userInfo = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);

        if (strflagP4_JCBG == "1")
        {
            CommonFunction.GetViewStateHt(ViewState["HtgInfoTwo"], ref htReturnP4_JCBG);

            GetEsbHtgStatus(htReturnP4_JCBG["EMAIL"].ToString().Trim(), htReturnP4_JCBG["TEMP_E_MAIL"].ToString().Trim(), this.txtTempEmail.Text.Trim(), htReturnP4_JCBG["TEMP_E_MAIL_STATUS"].ToString().Trim(), ref useRTDS, ref validSelValue, ref blockMsg, ref askUser);

            if (!string.IsNullOrEmpty(this.hidSendMailVerifyFlag.Text))
            {
                if (this.hidSendMailVerifyFlag.Text == "T")
                {
                    useRTDS = true;
                    validSelValue = "B";
                }
                if (this.hidSendMailVerifyFlag.Text == "F")
                {
                    useRTDS = false;
                    validSelValue = "";
                }
            }
        }
        //取JCBG值


        //查詢ESB_RTDS(未異動EMail或VALID_SEL為空或B則不打RTDS電文)
        if (useRTDS)
        {
            ebPhneEMailWhoRegnInqRqREQHDR emailWhoRegnInqRqHDR = new ebPhneEMailWhoRegnInqRqREQHDR();
            emailWhoRegnInqRqHDR.UserId = userInfo.Login_Account_Nbr;
            emailWhoRegnInqRqHDR.TrnNum = TransactionID_Search;

            ebPhneEMailWhoRegnInqRqREQBDY emailWhoRegnInqRqBDY = new ebPhneEMailWhoRegnInqRqREQBDY();
            //emailWhoRegnInqRqBDY.MobileNo = this.txtMobile.Text.Trim();  //不帶Mobile
            emailWhoRegnInqRqBDY.Email = this.txtTempEmail.Text.Trim();
            //emailWhoRegnInqRqBDY.OtpNo = ""; //畫面無otp欄位
            emailWhoRegnInqRqBDY.CustId = this.txtUserId.Text.Trim();
            emailWhoRegnInqRqBDY.EmailAllStatus = "Y";

            BRESBCustCheckInfo custCheckInfo = new BRESBCustCheckInfo();
            custCheckInfo.TransactionID = TransactionID_Search;
            custCheckInfo.REQHDR = emailWhoRegnInqRqHDR;
            custCheckInfo.REQBDY = emailWhoRegnInqRqBDY;

            //打RTDS電文查詢多人共用FLAG
            RTDSCheckTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ESBResult = ConntoESB.ConnESB(custCheckInfo);
            sameFlag = custCheckInfo.SameFlag == "T" ? true : false;

            #region Alert ESB電文錯誤
            if (custCheckInfo.ConnStatus == "F")
            {
                //ESB 錯誤
                errorMsg = GetErrorMsg(custCheckInfo.ErrorCode, custCheckInfo.ErrorMessage, dtESBStatus);
                base.sbRegScript.Append(string.Format("alert('ESB電文錯誤代碼：{0}，錯誤訊息：{1}，請重新操作。');", custCheckInfo.ErrorCode, errorMsg));
                bSucc = false;
                this.hidSendMailVerifyFlag.Text = "";
                return;
            }

            if (custCheckInfo.RspCode != "0000" && custCheckInfo.RspCode != "C001")
            {
                //custCheckInfo 錯誤
                errorMsg = GetErrorMsg(custCheckInfo.RspCode, custCheckInfo.RspMsg, dtCustCheckInfo);
                base.sbRegScript.Append(string.Format("alert('RTDS電文錯誤代碼：{0}，錯誤訊息：{1}，請重新操作。');", custCheckInfo.RspCode, errorMsg));
                bSucc = false;
                this.hidSendMailVerifyFlag.Text = "";
                return;
            }
            #endregion

            //打RTDS電文修改replyCustCheckInfo
            TransactionID_Edit = "CSIP" + DateTime.Now.ToString("yyyyMMddHHmmssff");

            cpCstmrEmailUpdRcrdMemoAddRqREQHDR emailUpdRcrdMemoAddRqHDR = new cpCstmrEmailUpdRcrdMemoAddRqREQHDR();
            emailUpdRcrdMemoAddRqHDR.UserID = userInfo.Login_Account_Nbr;
            emailUpdRcrdMemoAddRqHDR.TrnNum = TransactionID_Edit;

            cpCstmrEmailUpdRcrdMemoAddRqREQBDY emailUpdRcrdMemoAddRqBDY = new cpCstmrEmailUpdRcrdMemoAddRqREQBDY();
            emailUpdRcrdMemoAddRqBDY.TxnSeqNo = TransactionID_Edit;
            emailUpdRcrdMemoAddRqBDY.ApplyCustId = this.txtUserId.Text.Trim();
            emailUpdRcrdMemoAddRqBDY.Email = this.txtTempEmail.Text.Trim();
            //emailUpdRcrdMemoAddRq.OtpNo = ""; //畫面無otp欄位
            //emailUpdRcrdMemoAddRqBDY.MobileNo = this.txtMobile.Text.Trim(); //不帶Mobile
            emailUpdRcrdMemoAddRqBDY.CheckType = "email";
            emailUpdRcrdMemoAddRqBDY.ReplyCheckReason = sameFlag ? "" : "N-無共用";
            emailUpdRcrdMemoAddRqBDY.EmpId = userInfo.Login_Account_Nbr;
            emailUpdRcrdMemoAddRqBDY.BranchNo = userInfo.Branch_Nbr;

            cpCstmrEmailUpdRcrdMemoAddRqREQBDYReplyCustCheckInfoList replyCustCheckInfoList = new cpCstmrEmailUpdRcrdMemoAddRqREQBDYReplyCustCheckInfoList();
            replyCustCheckInfoList.CheckTime = RTDSCheckTime;

            BRESBReplyCustCheckInfo replyCustCheckInfo = new BRESBReplyCustCheckInfo();
            replyCustCheckInfo.TransactionID = TransactionID_Edit;
            replyCustCheckInfo.REQHDR = emailUpdRcrdMemoAddRqHDR;
            replyCustCheckInfo.REQBDY = emailUpdRcrdMemoAddRqBDY;
            replyCustCheckInfo.REQBDY.ReplyCustCheckInfoList = replyCustCheckInfoList;

            //打RTDS電文修改
            ESBResult = ConntoESB.ConnESB(replyCustCheckInfo);

            # region Alert ESB電文錯誤
            if (replyCustCheckInfo.ConnStatus == "F")
            {
                //ESB 錯誤
                errorMsg = GetErrorMsg(replyCustCheckInfo.ErrorCode, replyCustCheckInfo.ErrorMessage, dtESBStatus);
                base.sbRegScript.Append(string.Format("alert('ESB電文錯誤代碼：{0}，錯誤訊息：{1}，請重新操作。');", custCheckInfo.ErrorCode, errorMsg));
                bSucc = false;
                this.hidSendMailVerifyFlag.Text = "";
                return;
            }

            if (replyCustCheckInfo.RspCode != "0000")
            {
                //replyCustCheckInfo 錯誤
                errorMsg = GetErrorMsg(replyCustCheckInfo.RspCode, replyCustCheckInfo.RspMsg, dtReplyCustCheckInfo);
                base.sbRegScript.Append(string.Format("alert('RTDS電文錯誤代碼：{0}，錯誤訊息：{1}，請重新操作。');", custCheckInfo.RspCode, errorMsg));
                bSucc = false;
                this.hidSendMailVerifyFlag.Text = "";
                return;
            }
            #endregion
        }

        ArrayList arrayName = new ArrayList(new object[] { "ACCT_NBR", "CITY", "ADDR_1", "ADDR_2", "EMPLOYER", "HOME_PHONE", "OFFICE_PHONE", "MANAGE_ZIP_CODE", "ZIP", "MEMO_1", "MEMO_2", "OFF_PHONE_FLAG" });
        string[] eBillList = { "3", "5", "6", "7" };

        if (strflagP4_JCF6 == "1")
        {
            //查詢P4環境成功
            htReturnP4_JCF6 = (Hashtable)ViewState["HtgInfoOneP4"];

            CommonFunction.GetViewStateHt(ViewState["HtgInfoTwo"], ref htReturnP4_JCBG);

            htOneCard = (Hashtable)htReturnP4_JCF6.Clone();//*得到主機第一卡人檔P4信息

            if (strflagP4_JCBG == "1")
            {
                if (validSelValue == "B" && !eBillList.Contains(htReturnP4_JCF6["OFF_PHONE_FLAG"].ToString()) && eBillList.Contains(this.txtBill.Text.Trim()))
                {
                    usePCTIToUpdateEBill = false;
                }
            }

            MainFrameInfo.ChangeJCF6toPCTI(htReturnP4_JCF6, htOutputP4, arrayName);
            UpdateHtgInfo(htOutputP4, dtblUpdate);
            htOutputP4.Add("FUNCTION_ID", "PCMC1");

            if (UpdateP4DOrP4(htOutputP4, HtgType.P4_PCTI, dtblUpdate, "P4", usePCTIToUpdateEBill, strflagP4_JCBG == "1", htReturnP4_JCF6, ref rtnHtgMsgType, ref rtnHtgMsg))
            {
                if (strflagP4_JCBG == "1")
                {
                    UpdateJCBGP4(htReturnP4_JCBG, !usePCTIToUpdateEBill, TransactionID_Edit, sameFlag, validSelValue, htReturnP4_JCF6, ref rtnHtgMsgType, ref rtnHtgMsg);
                    if (!string.IsNullOrEmpty(rtnHtgMsgType))
                    {
                        base.sbRegScript.Append(string.Format("alert('HTG錯誤代碼：{0}，錯誤訊息：{1}');", rtnHtgMsgType, GetErrorMsg(rtnHtgMsgType, "", dtJCBGStatus)));
                        bSucc = false;
                    }
                }
            }
            else
            {
                bSucc = false;
                if (!string.IsNullOrEmpty(rtnHtgMsgType))
                {
                    base.sbRegScript.Append(string.Format("alert('HTG錯誤代碼：{0}，錯誤訊息：{1}');", rtnHtgMsgType, GetErrorMsg(rtnHtgMsgType, "", dtJCBGStatus)));
                }
            }
        }

        //Start 2021/11/01  理專十誡，註解P4D環境相關程式 by  Ares_Stanley
        //if (strflagP4D_JCF6 == "1")
        //{
        //    htReturnP4D_JCF6 = (Hashtable)ViewState["HtgInfoOneP4D"];

        //    if (this.txtBill.Text.Trim() != htReturnP4D_JCF6["OFF_PHONE_FLAG"].ToString())
        //    {
        //        if (htReturnP4D_JCF6["OFF_PHONE_FLAG"].ToString() != "0" && htReturnP4D_JCF6["OFF_PHONE_FLAG"].ToString() != "2")
        //        {
        //            base.strClientMsg += MessageHelper.GetMessage("01_01010100_010") + htReturnP4D_JCF6["OFF_PHONE_FLAG"].ToString() + MessageHelper.GetMessage("01_01010100_011");
        //            this.txtBill.Text = htReturnP4D_JCF6["OFF_PHONE_FLAG"].ToString();
        //            base.sbRegScript.Append(BaseHelper.SetFocus("txtBill"));
        //            this.txtBill.ForeColor = Color.Red;
        //            return;
        //        }
        //    }
        //    MainFrameInfo.ChangeJCF6toPCTI(htReturnP4D_JCF6, htOutputP4D, arrayName);
        //    UpdateHtgInfo(htOutputP4D, dtblUpdate);
        //    htOutputP4D.Add("FUNCTION_ID", "PCMC1");

        //    //*P4D、P4環境下異動主機資料
        //    if (!UpdateP4DOrP4(htOutputP4D, HtgType.P4D_PCTI, dtblUpdate, "P4D", usePCTIToUpdateEBill, strflagP4_JCBG == "1", ref rtnHtgMsgType, ref rtnHtgMsg))
        //    {
        //        bSucc = false;
        //        if (!string.IsNullOrEmpty(rtnHtgMsgType))
        //        {
        //            jsBuilder.RegScript(this.Page, string.Format("alert('HTG錯誤代碼：{0}，錯誤訊息：{1}')", rtnHtgMsgType, GetErrorMsg(rtnHtgMsgType, "", dtJCBGStatus)));
        //        }
        //    }
        //}
        //End 2021/11/01  理專十誡，註解P4D環境相關程式 by  Ares_Stanley

        if (bSucc == false)
        {
            this.hidSendMailVerifyFlag.Text = "";
            CommonFunction.SetEnabled(pnlText, false);//*將網頁中的提交按鈕和輸入框disable           
        }
        else
        {
            if (strflagP4_JCBG != "1")
            {
                base.sbRegScript.Append(string.Format("alert('請注意，由於第二卡人檔電文JCBG異常，系統未透過PCTI或JCBG異動電子帳單欄位！JCBG訊息：{0}');", htReturnP4_JCBG["HtgMsg"].ToString().Trim()));
            }
            this.txtUserId.Text = "";
            this.lblNameText.Text = "";
            this.lblTempEmailStatus.Text = "";
            this.hidSendMailVerifyFlag.Text = "";
            CommonFunction.SetControlsEnabled(pnlText, false);
        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtUserId"));
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/30
    /// 修改日期：2009/09/30
    /// <summary>
    /// 修改主機資料
    /// </summary>
    /// <param name="htReturn">主機返回信息的HashTable</param>
    /// <param name="dtblUpdate">修改主機信息的DataTable</param>
    private void UpdateHtgInfo(Hashtable htReturn, DataTable dtblUpdate)
    {
        //*（主機）住家電話
        strOHomeTel = htReturn["TELEPHONE"].ToString().Trim();
        //*（主機）公司電話
        strOOfficeTel = htReturn["WORK_PHONE"].ToString().Trim();

        if (strOHomeTel.Trim() == "-")
        {
            strOHomeTel = "";
        }

        if (strOOfficeTel.Trim() == "-")
        {
            strOOfficeTel = "";
        }

        //*比對城市名
        CommonFunction.ContrastDataEdit(htReturn, dtblUpdate, this.txtCityName.Text.Trim(), "CITY", BaseHelper.GetShowText("01_01010100_005"));

        //*比對帳單地址1
        CommonFunction.ContrastDataEdit(htReturn, dtblUpdate, this.txtMerchantAddOne.Text.Trim(), "ADDR_LINE_1", BaseHelper.GetShowText("01_01010100_006"));

        //*比對帳單地址2
        CommonFunction.ContrastDataEdit(htReturn, dtblUpdate, this.txtMerchantAddTwo.Text.Trim(), "ADDR_LINE_2", BaseHelper.GetShowText("01_01010100_007"));

        //*比對公司名稱
        CommonFunction.ContrastDataEdit(htReturn, dtblUpdate, this.txtCompanyName.Text.Trim(), "OWNER", BaseHelper.GetShowText("01_01010100_008"));

        //*比對住家電話
        if (CommonFunction.ContrastData(htReturn, this.txtHomeTel1.Text.Trim().PadRight(3, ' ') + "-" + this.txtHomeTel2.Text.Trim(), "TELEPHONE") != 0)
        {
            //*（輸入）住家電話
            string strMHomeTel = htReturn["TELEPHONE"].ToString();

            if (strMHomeTel.Trim() == "-")
            {
                strMHomeTel = "";
            }

            CommonFunction.UpdateLog(strOHomeTel, strMHomeTel, BaseHelper.GetShowText("01_01010100_009"), dtblUpdate);
        }
        else
        {
            htReturn.Remove("TELEPHONE");
        }

        //*比對公司電話一
        if (CommonFunction.ContrastData(htReturn, this.txtCompanyTel1.Text.Trim().PadRight(3, ' ') + "-" + this.txtCompanyTel2.Text.Trim(), "WORK_PHONE") != 0)
        {
            //*（輸入）公司電話
            string strMOfficeTel = htReturn["WORK_PHONE"].ToString();

            if (strMOfficeTel.Trim() == "-")
            {
                strMOfficeTel = "";
            }

            CommonFunction.UpdateLog(strOOfficeTel, strMOfficeTel, BaseHelper.GetShowText("01_01010100_010"), dtblUpdate);
        }
        else
        {
            htReturn.Remove("WORK_PHONE");
        }

        //*比對管理郵區
        CommonFunction.ContrastDataEdit(htReturn, dtblUpdate, this.txtPostalDistrict.Text.Trim(), "MAIL_IND", BaseHelper.GetShowText("01_01010100_011"));

        //*比對郵遞區號
        CommonFunction.ContrastDataEdit(htReturn, dtblUpdate, this.txtZipCode.Text.Trim(), "ZIP", BaseHelper.GetShowText("01_01010100_012"));

        //*比對註記一
        CommonFunction.ContrastDataEdit(htReturn, dtblUpdate, this.txtRemarkOne.Text.Trim(), "MEMO_1", BaseHelper.GetShowText("01_01010100_013"));

        //*比對註記二
        CommonFunction.ContrastDataEdit(htReturn, dtblUpdate, this.txtRemarkTwo.Text.Trim(), "MEMO_2", BaseHelper.GetShowText("01_01010100_014"));

        //*電子賬單
        CommonFunction.ContrastDataEdit(htReturn, dtblUpdate, this.txtBill.Text.Trim(), "MAIL", BaseHelper.GetShowText("01_01010100_019"));

    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/30
    /// 修改日期：2009/09/30
    /// <summary>
    /// 修改P4D或P4環境主機信息
    /// </summary>
    /// <param name="htReturn">主機返回信息的HashTable</param>
    /// <param name="type">電文類型</param>
    /// <param name="dtblUpdate">修改主機欄位信息的DataTable</param>
    /// <param name="strLogFlag">操作標識</param>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateP4DOrP4(Hashtable htReturn, HtgType type, DataTable dtblUpdate, string strLogFlag, bool usePCTIToUpdateEBill, bool isJCBGWork, Hashtable htReturnP4_JCF6, ref string rtnHtgMsgType, ref string rtnHtgMsg)
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        //20211026 使用JCBG上送Ebill時, PCTI只送P4, 不送P4D, JCBG異常時不送Ebill欄位 by Ares Stanley
        if (strLogFlag == "P4" && isJCBGWork == false)
        {
            htReturn.Remove("MAIL");
        }
        
        //20211111 使用JCBG上送新Ebill資料時, 使用PCTI上送舊資料 by Ares Stanley
        if (strLogFlag == "P4" && usePCTIToUpdateEBill == false)
        {
            htReturn["MAIL"] = htReturnP4_JCF6["OFF_PHONE_FLAG"].ToString();
        }

        if (dtblUpdate.Rows.Count > 0)
        {
            string strType = "";
            if (strLogFlag == "P4D")
            {
                strType = "22";//*異動P4D環境
            }
            else
            {
                strType = "2";
            }

            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(type, htReturn, false, strType, eAgentInfo);
            if (!htResult.Contains("HtgMsg"))
            {
                base.strClientMsg += type.ToString() + MessageHelper.GetMessage("01_00000000_014");
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                UpdateDataBase(dtblUpdate, strLogFlag);
                return true;
            }
            else
            {
                if (htResult["MESSAGE_TYPE"] != null)
                    rtnHtgMsgType = htResult["MESSAGE_TYPE"].ToString().Trim();
                rtnHtgMsg = htResult["HtgMsg"].ToString().Trim();
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();

                    if (strLogFlag == "P4D")
                    {
                        base.strClientMsg += MessageHelper.GetMessage("01_01010100_003");
                    }
                    else
                    {
                        base.strClientMsg += MessageHelper.GetMessage("01_01010100_004");
                    }
                }
                else
                {
                    base.strClientMsg += htResult["HtgMsg"].ToString();
                }
                return false;
            }
        }
        else
        {
            if (strLogFlag == "P4D")
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01010100_001");
            }
            else
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01010100_002");
            }
            return true;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/30
    /// 修改日期：2009/09/30 
    /// <summary>
    /// 在P4環境下異動第二卡人檔主機資料
    /// </summary>
    /// <param name="htReturn">第二卡人檔主機資料返回信息的HashTable</param>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateJCDKP4(Hashtable htReturn)
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        DataTable dtblUpdate = CommonFunction.GetDataTable();

        //*比對戶籍電話
        string strTelPrem = this.txtRegTel1.Text.Trim().PadRight(3, ' ') + "-" + this.txtRegTel2.Text.Trim().PadRight(14, ' ');
        CommonFunction.ContrastData(htReturn, dtblUpdate, strTelPrem, "TEL_PERM", BaseHelper.GetShowText("01_01010100_016"));

        //*比對行動電話
        CommonFunction.ContrastData(htReturn, dtblUpdate, this.txtMobile.Text.Trim(), "MOBILE_PHONE", BaseHelper.GetShowText("01_01010100_017"));

        //*比對Email
        CommonFunction.ContrastData(htReturn, dtblUpdate, this.txtEmail.Text.Trim(), "EMAIL", BaseHelper.GetShowText("01_01010100_018"));

        if (dtblUpdate.Rows.Count > 0)
        {
            htReturn["FUNCTION_CODE"] = "2";
            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCDK, htReturn, false, "2", eAgentInfo);
            if (!htResult.Contains("HtgMsg"))
            {
                base.strClientMsg += HtgType.P4_JCDK.ToString() + MessageHelper.GetMessage("01_00000000_014");
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息 
                UpdateDataBase(dtblUpdate, "P4");
                return true;
            }
            else
            {
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_038");
                }
                else
                {
                    base.strClientMsg += htResult["HtgMsg"].ToString();
                }
                return false;
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01010100_005");
            return true;
        }
    }

    /// 作者 Ares Stanley
    /// 創建日期：2021/10/15
    /// 修改日期：2021/10/15 
    /// <summary>
    /// 在P4環境下異動JCBG
    /// </summary>
    /// <param name="htReturn">JCBG返回信息的HashTable</param>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateJCBGP4(Hashtable htReturn, bool updateEBill, string TransactionID, bool sameFlag, string validSel, Hashtable htReturnP4_JCF6, ref string rtnHtgMsgType, ref string rtnHtgMsg)
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        DataTable dtblUpdate = CommonFunction.GetDataTable();
        string[] comparArr = new string[] { "TEL_PERM", "MOBILE_PHONE", "EMAIL", "TEMP_E_MAIL", "VALID_SEL", "SAME_FLAG", "VERIFY_SEQ_NO", "USER_ID", "LINE_CNT", "ACCT_NBR", "EBILL_FLAG" };
        string strTelPrem = this.txtRegTel1.Text.Trim().PadRight(3, ' ') + "-" + this.txtRegTel2.Text.Trim().PadRight(14, ' ');
        //*若須進行EMAIL驗證則一律上送JCBG，否則需比對是否異動
        if (validSel == "")
        {
            //*比對戶籍電話
            if (CommonFunction.ContrastData(htReturn, dtblUpdate, strTelPrem, "TEL_PERM", BaseHelper.GetShowText("01_01010100_016")) == 0)
                htReturn.Remove("TEL_PERM");

            //*比對行動電話
            if (CommonFunction.ContrastData(htReturn, dtblUpdate, this.txtMobile.Text.Trim(), "MOBILE_PHONE", BaseHelper.GetShowText("01_01010100_017")) == 0)
                htReturn.Remove("MOBILE_PHONE");

            //*比對Email
            if (CommonFunction.ContrastData(htReturn, dtblUpdate, this.txtEmail.Text.Trim(), "EMAIL", BaseHelper.GetShowText("01_01010100_018")) == 0)
                htReturn.Remove("EMAIL");

            //*比對TempEmail
            if (CommonFunction.ContrastData(htReturn, dtblUpdate, this.txtTempEmail.Text.Trim(), "TEMP_E_MAIL", BaseHelper.GetShowText("01_01010100_022")) == 0)
                htReturn.Remove("TEMP_E_EMAIL");
        }
        else
        {
            //Tel
            DataRow drowRowTel = dtblUpdate.NewRow();
            drowRowTel[EntityCUSTOMER_LOG.M_field_name] = BaseHelper.GetShowText("01_01010100_016");
            drowRowTel[EntityCUSTOMER_LOG.M_before] = htReturn["TEL_PERM"].ToString();
            drowRowTel[EntityCUSTOMER_LOG.M_after] = strTelPrem;
            dtblUpdate.Rows.Add(drowRowTel);
            htReturn["TEL_PERM"] = strTelPrem;

            //MobilePhone
            DataRow drowRowMobile = dtblUpdate.NewRow();
            drowRowMobile[EntityCUSTOMER_LOG.M_field_name] = BaseHelper.GetShowText("01_01010100_017");
            drowRowMobile[EntityCUSTOMER_LOG.M_before] = htReturn["MOBILE_PHONE"].ToString();
            drowRowMobile[EntityCUSTOMER_LOG.M_after] = this.txtMobile.Text.Trim();
            dtblUpdate.Rows.Add(drowRowMobile);
            htReturn["MOBILE_PHONE"] = this.txtMobile.Text.Trim();

            //Email
            DataRow drowRowEmail = dtblUpdate.NewRow();
            drowRowEmail[EntityCUSTOMER_LOG.M_field_name] = BaseHelper.GetShowText("01_01010100_018");
            drowRowEmail[EntityCUSTOMER_LOG.M_before] = htReturn["EMAIL"].ToString();
            drowRowEmail[EntityCUSTOMER_LOG.M_after] = this.txtEmail.Text.Trim();
            dtblUpdate.Rows.Add(drowRowEmail);
            htReturn["EMAIL"] = this.txtEmail.Text.Trim();

            //TempEmail
            DataRow drowRowTempEmail = dtblUpdate.NewRow();
            drowRowTempEmail[EntityCUSTOMER_LOG.M_field_name] = BaseHelper.GetShowText("01_01010100_022");
            drowRowTempEmail[EntityCUSTOMER_LOG.M_before] = htReturn["TEMP_E_MAIL"].ToString();
            drowRowTempEmail[EntityCUSTOMER_LOG.M_after] = this.txtTempEmail.Text.Trim();
            dtblUpdate.Rows.Add(drowRowTempEmail);
            htReturn["TEMP_E_MAIL"] = this.txtTempEmail.Text.Trim();
        }

        htReturn["VALID_SEL"] = validSel;

        //*比對EBill
        if (!updateEBill)
        {
            htReturn.Remove("EBILL_FLAG");
        }
        else
        {
            if (CommonFunction.ContrastData(htReturnP4_JCF6, dtblUpdate, this.txtBill.Text.Trim(), "OFF_PHONE_FLAG", BaseHelper.GetShowText("01_01010100_019")) == 0)
            {
                htReturn.Remove("EBILL_FLAG");
            }
            else
            {
                htReturn["EBILL_FLAG"] = this.txtBill.Text.Trim();
            }
                
        }

        //是否共用
        htReturn["SAME_FLAG"] = sameFlag ? "Y" : "N";

        //平台提供序號
        htReturn["VERIFY_SEQ_NO"] = TransactionID;


        //移除不修改欄位
        Hashtable htReturnClone = (Hashtable)htReturn.Clone();
        foreach (DictionaryEntry htValue in htReturn)
        {
            if (!comparArr.Contains(htValue.Key))
            {
                htReturnClone.Remove(htValue.Key);
            }
        }

        //未修改資料補_
        fillUnderscoreToJCBG(htReturnClone);

        if (dtblUpdate.Rows.Count > 0)
        {
            htReturnClone["FUNCTION_CODE"] = "C";
            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCBG, htReturnClone, false, "2", eAgentInfo);
            if (!htResult.Contains("HtgMsg"))
            {
                base.strClientMsg += HtgType.P4_JCBG.ToString() + MessageHelper.GetMessage("01_00000000_014");
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息 
                // 經USER確認，若上送資料修改前後無差異則不寫入LOG by Ares Stanley 20211115
                for (int i = dtblUpdate.Rows.Count - 1; i >= 0; i--)
                {
                    if (dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_before].ToString().Trim() == dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_after].ToString().Trim())
                    {
                        dtblUpdate.Rows[i].Delete();
                    }
                }
                if (dtblUpdate.Rows.Count > 0)
                {
                    UpdateDataBase(dtblUpdate, "P4");
                }
                return true;
            }
            else
            {
                if (htResult["MESSAGE_TYPE"] != null)
                    rtnHtgMsgType = htResult["MESSAGE_TYPE"].ToString().Trim();
                rtnHtgMsg = htResult["HtgMsg"].ToString().Trim();
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_038");
                }
                else
                {
                    base.strClientMsg += htResult["HtgMsg"].ToString();
                }
                return false;
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01010100_005");
            return true;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/30
    /// 修改日期：2009/09/30 
    /// <summary>
    /// 得到主機資料
    /// </summary>
    /// <param name="htResult">主機返回的HashTable</param>
    /// <returns>true成功， false失敗</returns>
    private bool GetMainframeData(HtgType type, ref Hashtable htResult, string strType)
    {

        //*添加上傳主機信息
        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT_NBR", CommonFunction.GetSubString(this.txtUserId.Text.Trim().ToUpper(), 0, 16));
        htInput.Add("FUNCTION_CODE", "1");
        htInput.Add("LINE_CNT", "0000");

        //*得到主機傳回信息
        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(type, htInput, false, strType, eAgentInfo);

        if (!htReturn.Contains("HtgMsg"))
        {
            htReturn["ACCT_NBR"] = htInput["ACCT_NBR"]; //* for_xml_test  
            htReturn["FUNCTION_CODE"] = "1";
            htReturn["LINE_CNT"] = "0000";
            htReturn["MESSAGE_TYPE"] = "";
            htResult = htReturn;
            return true;
        }
        else
        {
            htResult = htReturn;
            return false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/30
    /// 修改日期：2009/09/30 
    /// <summary>
    /// 更新資料庫信息
    /// </summary>
    /// <param name="dtblUpdate">修改主機信息的DataTable</param>
    /// <param name="strLogFlag">操作標識</param>
    private void UpdateDataBase(DataTable dtblUpdate, string strLogFlag)
    {
        if (!BRTRANS_NUM.UpdateTransNum("A01"))
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            }
        }
        if (!CommonFunction.InsertCustomerLog(dtblUpdate, eAgentInfo, this.txtUserId.Text.Trim().ToUpper(), strLogFlag, (structPageInfo)Session["PageInfo"]))
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            }
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/30
    /// 修改日期：2009/09/30
    /// <summary>
    /// 設置控件是否可用
    /// </summary>
    /// <param name="blnEnabled">True可用，false不可用</param>
    private void SetControlsEnabled(bool blnEnabled)
    {
        this.txtRegTel1.Text = "";
        this.txtRegTel2.Text = "";
        this.txtMobile.Text = "";
        this.txtEmail.Text = "";
        this.txtRegTel1.Enabled = blnEnabled;
        this.txtRegTel2.Enabled = blnEnabled;
        this.txtMobile.Enabled = blnEnabled;
        this.txtEmail.Enabled = blnEnabled;
    }

    /// 作者 Ares Stanley
    /// 創建日期：2021/10/08
    /// 修改日期：2021/10/08
    /// <summary>
    /// 未修改欄位補_
    /// </summary>
    /// <param name="htInput"></param>
    private void fillUnderscoreToJCBG(Hashtable htInput)
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();
        dict.Add("CON_RPT", 2);
        dict.Add("MOBILE_PHONE", 10);
        dict.Add("EMAIL", 50);
        dict.Add("TITLE", 12);
        dict.Add("E_SERVICE_CODE", 4);
        dict.Add("FAX_NO", 18);
        dict.Add("PERM_ZIP", 5);
        dict.Add("PERM_CITY", 14);
        dict.Add("PERM_ADDR_1", 30);
        dict.Add("PERM_ADDR_2", 30);
        dict.Add("INFORMANT_NAME_1", 10);
        dict.Add("INFORMANT_TEL_NO_1", 18);
        dict.Add("INFORMANT_NAME_2", 10);
        dict.Add("INFORMANT_TEL_NO_2", 18);
        dict.Add("TEL_PERM", 18);
        dict.Add("STUS_ID", 2);
        dict.Add("STUS_TELO", 2);
        dict.Add("STUS_TELH", 2);
        dict.Add("STUS_TELP", 2);
        dict.Add("STUS_FAX", 2);
        dict.Add("STUS_MOBIL", 2);
        dict.Add("STUS_ADDB", 2);
        dict.Add("STUS_ADDP", 2);
        dict.Add("STUS_EMAIL", 2);
        dict.Add("GRADE_SCHOOL", 22);
        dict.Add("GRADE_SCHOOL_LOCATE", 10);
        dict.Add("OVERSEA_PHONE", 18);
        dict.Add("SMS_NP_FLAG", 1);
        dict.Add("VALID_SEL", 1);
        dict.Add("TEMP_E_MAIL", 50);
        dict.Add("TEMP_E_MAIL_STATUS", 1);
        dict.Add("TMAL_CREATE_DATE", 8);
        dict.Add("SAME_FLAG", 1);
        dict.Add("EBILL_FLAG", 1);
        dict.Add("VERIFY_SEQ_NO", 20);
        string[] withUnderscore = { "VALID_SEL", "TEMP_E_MAIL", "TEMP_E_MAIL_STATUS", "TMAL_CREATE_DATE", "SAME_FLAG", "EBILL_FLAG", "VERIFY_SEQ_NO" };
        foreach (KeyValuePair<string, int> col in dict)
        {
            if (htInput[col.Key] == null || htInput[col.Key].ToString().Trim() == "")
            {
                if (htInput[col.Key] == null)
                {
                    htInput.Add(col.Key, "");
                }

                if (withUnderscore.Contains(col.Key.ToString()))
                {
                    continue;
                }

                htInput[col.Key] = new String('_', col.Value);
            }
        }
    }

    /// 作者 Ares Stanley
    /// 創建日期：2021/10/22
    /// 修改日期：2021/10/22
    /// <summary>
    /// 判斷是否要打RTDS，判斷VALID_SEL值
    /// </summary>
    /// <param name="Email">原始Email</param>
    /// <param name="oriTempEmail">原始待驗證Email</param>
    /// <param name="curTempEmail">待驗證Email</param>
    /// <param name="tempEmailStatus">原始待驗證Email狀態</param>
    /// <param name="useRTDS">使否打RTDS</param>
    /// <param name="ValidSelValue">VALID_SEL所帶的值</param>
    /// <param name="blockMsg">阻擋USER繼續進行的原因</param>
    public static void GetEsbHtgStatus(string Email, string oriTempEmail, string curTempEmail, string tempEmailStatus, ref bool useRTDS, ref string ValidSelValue, ref string blockMsg, ref bool askUser)
    {
        #region 不進行Email驗證
        //待驗證Email與原始待驗證Email相同且待驗證Email狀態不為X，不進行Email驗證
        if ((oriTempEmail == curTempEmail) && tempEmailStatus != "X")
        {
            useRTDS = false;
            ValidSelValue = "";
            return;
        }
        #endregion

        #region 阻擋User
        //待驗證Email為空且原始待驗證Email不為空時，跳出提示訊息「不可清空待驗證EMAIL」，阻擋USER
        if (string.IsNullOrEmpty(curTempEmail) && !string.IsNullOrEmpty(oriTempEmail))
        {
            blockMsg = MessageHelper.GetMessage("01_01010100_015");
            return;
        }

        //原始Email與待驗證Email不為空且待驗證Email與原始Email相同且待驗證Email狀態不為Y時，跳出提示訊息「不可將待驗證Email改為與Email相同」，阻擋USER
        if (!string.IsNullOrEmpty(curTempEmail) && !string.IsNullOrEmpty(Email) && curTempEmail == Email && tempEmailStatus == "Y")
        {
            useRTDS = false;
            ValidSelValue = "";
            return;
        }

        //原始Email與待驗證Email不為空且待驗證Email與原始Email相同且待驗證Email狀態不為Y時，跳出提示訊息「不可將待驗證Email改為與Email相同」，阻擋USER
        if (!string.IsNullOrEmpty(curTempEmail) && !string.IsNullOrEmpty(Email) && curTempEmail == Email && tempEmailStatus != "Y")
        {
            blockMsg = MessageHelper.GetMessage("01_01010100_016");
            return;
        }
        #endregion

        #region 進行Email驗證
        //待驗證Email與原始待驗證Email不同且待驗證Email狀態不為X，進行Email驗證
        if ((oriTempEmail != curTempEmail) && tempEmailStatus != "X")
        {
            useRTDS = true;
            ValidSelValue = "B";
            return;
        }

        //待驗證Email與原始待驗證Email不同且待驗證Email狀態為X，進行EMAIL驗證
        if ((oriTempEmail != curTempEmail) && tempEmailStatus == "X")
        {
            useRTDS = true;
            ValidSelValue = "B";
            return;
        }
        #endregion

        #region 讓User選擇
        //待驗證Email與原始待驗證Email相同且待驗證Email狀態為X，給USER選擇是否進行EMAIL驗證
        if ((oriTempEmail == curTempEmail) && tempEmailStatus == "X")
        {
            askUser = true;
            return;
        }
        #endregion

    }

    /// 作者 Ares Stanley
    /// 創建日期：2021/10/22
    /// 修改日期：2021/10/22
    /// <summary>
    /// 取得所有代碼訊息
    /// </summary>
    /// <param name="dtCustCheckInfo"></param>
    /// <param name="dtReplyCustCheckInfo"></param>
    /// <param name="dtESBStatus"></param>
    /// <param name="dtJCBGStatus"></param>
    /// <returns></returns>
    public static bool GetAllCodeMsg(ref DataTable dtCustCheckInfo, ref DataTable dtReplyCustCheckInfo, ref DataTable dtESBStatus, ref DataTable dtJCBGStatus)
    {
        try
        {
            string[] strPropertyKey = new string[] { };
            //取得custCheckInfo代碼訊息
            strPropertyKey = new string[] { "custCheckInfo" };
            if (!BRM_PROPERTY_CODE.GetEnableProperty("01", strPropertyKey, ref dtCustCheckInfo))
                return false;

            //取得replyCustCheckInfo代碼訊息
            strPropertyKey = new string[] { "replyCustCheckInfo" };
            if (!BRM_PROPERTY_CODE.GetEnableProperty("01", strPropertyKey, ref dtReplyCustCheckInfo))
                return false;

            //取得ESB代碼訊息
            strPropertyKey = new string[] { "ESB" };
            if (!BRM_PROPERTY_CODE.GetEnableProperty("01", strPropertyKey, ref dtESBStatus))
                return false;

            //取得JCBG代碼訊息
            strPropertyKey = new string[] { "P4_JCBG" };
            if (!BRM_PROPERTY_CODE.GetEnableProperty("01", strPropertyKey, ref dtJCBGStatus))
                return false;
            return true;
        }
        catch (Exception ex)
        {
            Logging.Log(string.Format("取得電文代碼訊息時發生錯誤，錯誤訊息：{0}", ex), LogState.Info, LogLayer.Util);
            return false;
        }
    }

    /// 作者 Ares Stanley
    /// 創建日期：2021/10/26
    /// 修改日期：2021/10/26
    /// <summary>
    /// 從屬性維護取得錯誤訊息
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="errorMsg"></param>
    /// <param name="dtSource"></param>
    /// <returns></returns>
    public static string GetErrorMsg(string errorCode, string errorMsg, DataTable dtSource)
    {
        string result = string.Empty;
        DataRow[] propertyArr = new DataRow[] { };

        propertyArr = dtSource.Select(string.Format("PROPERTY_CODE=" + "'{0}'", errorCode));

        if (propertyArr.Length == 1)
        {
            result = propertyArr[0]["PROPERTY_NAME"].ToString().Trim();
        }
        else
        {
            if (!string.IsNullOrEmpty(errorMsg))
            {
                result = errorMsg;
            }
            else
            {
                result = errorCode;
            }
        }

        return result;
    }
}
