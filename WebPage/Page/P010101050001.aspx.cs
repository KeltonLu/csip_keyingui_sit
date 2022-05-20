//******************************************************************
//*  作    者：chenjingxian
//*  功能說明：他行自扣一Key
//*  創建日期：2009/10/21
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
//20160606 (U) by Tank, add 查詢近60天內"其他作業 記錄查詢"看有無資料

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
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Common.JavaScript;
using Framework.Data;
using Framework.WebControls;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Transactions;
using CSIPCommonModel.EntityLayer;
using System.Text;
//20160606 (U) by Tank
using System.Data.SqlClient;
using System.Web.Services;

public partial class P010101050001 : PageBase
{

    #region 變數區
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息 
    private string gsTranCode = ""; //交易代號
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #endregion

    #region 事件區


    /// 作者 chenjingxian
    /// 創建日期：2009/10/21
    /// 修改日期：2009/10/21 
    /// <summary>
    /// 加載網頁
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LoadDropDownList();
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            SetControlsEnabled(false);
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"]; //*記錄網頁訊息
        SetColorBlack();//*使網頁中的輸入框字體顏色變    
    }


    /// 作者 chenjingxian
    /// 創建日期：2009/10/21
    /// 修改日期：2009/10/21 
    /// <summary>
    /// 異動一Key資料
    /// </summary>

    protected void btnHidenD_Click(object sender, EventArgs e)
    {
        DataSet dstInfo = null;
        Hashtable htEBill = (Hashtable)ViewState["HtgInfoJCF6"];

        lblLongBankNo.Text = "";
        lblLongBankName.Text = "";
        string sReceiveNumber = this.txtReceiveNumber.Text.Trim();
        string sUserId = this.txtUserId.Text.Trim().ToUpper();

        if (txtAccNoBank.Text.Trim() != "")
        {
            //*查詢銀行代號資料
            DataTable dtblBANKINFO = (DataTable)BROTHER_BANK_TEMP.SearchBankInfo(txtAccNoBank.Text.Trim());

            if (dtblBANKINFO == null)
            {
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return;
            }

            if (dtblBANKINFO.Rows.Count > 0)
            {
                txtAccNoBank.Text = SelectValue(dtblBANKINFO.Rows[0][0].ToString().Trim());
                lblLongBankNo.Text = dtblBANKINFO.Rows[0][1].ToString().Trim();
                lblLongBankName.Text = dtblBANKINFO.Rows[0][2].ToString().Trim();
            }
            else
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01010500_017");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtAccNoBank"));
                return;
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01010500_017");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtAccNoBank"));
            return;
        }

        //*檢核他行帳單週期是否為05/10/15/20/27週期(可隨系統維護中的資料維護中的帳單週期變動)，若輸入非系統設定的週期則出”帳單週期錯誤”之錯誤訊息
        dstInfo = BaseHelper.CheckCommonPropertySet("01", "15", this.txtBcycleCode.Text.Trim());

        if (dstInfo == null)
        {
            //*查詢資料庫時發生錯誤
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            SetControlsEnabled(false);
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }
        else
        {
            if (dstInfo.Tables[0].Rows.Count == 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01010500_020");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                return;
            }
        }

        //*檢核銀行代號+銀行帳號欄位與原主機帶出資料一樣，則出”帳號與主機相同不需處理”之錯誤訊息
        if ((htEBill["CO_OWNER"].ToString()) == (CommonFunction.GetSubString(this.txtAccNoBank.Text.Trim(), 0, 3) + "-" + this.txtAccNo.Text.ToString()))
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01010500_021");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            return;
        }

        //*若收件編號對應的資料已經分配了batch_no,但未完成回貼主機動作（Pcmc_Upload_flag<>'1'），不允許修改，只能查看此筆資料。
        dstInfo = BROTHER_BANK_TEMP.SelectWithBatch(sReceiveNumber);

        if (dstInfo == null)
        {
            //*查詢資料庫時發生錯誤
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            SetControlsEnabled(false);
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (dstInfo.Tables[0].Rows.Count > 0)
        {
            strClientMsg += MessageHelper.GetMessage("01_01010500_009");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            return;
        }

        //*輸入的收件編號是否已經有完成的ACH 流程的資料
        DataTable dtblAch = (DataTable)BROTHER_BANK_TEMP.SearchACH(sReceiveNumber);
        if (dtblAch == null)
        {
            //* 顯示端末訊息
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            SetControlsEnabled(false);
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }
        else
        {
            if (dtblAch.Rows.Count > 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01010500_010");
                return;
            }
        }

        //*檢核電子帳單    
        if (this.txtEBill.Text.Trim() != htEBill["OFF_PHONE_FLAG"].ToString())
        {
            if (htEBill["OFF_PHONE_FLAG"].ToString() != "0" && htEBill["OFF_PHONE_FLAG"].ToString() != "2")
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01010500_013") + htEBill["OFF_PHONE_FLAG"].ToString() + MessageHelper.GetMessage("01_01010500_019");
                this.txtEBill.Text = htEBill["OFF_PHONE_FLAG"].ToString();
                base.sbRegScript.Append(BaseHelper.SetFocus("txtEBill"));
                return;
            }
        }

        //*查詢一Key資料
        DataTable dtblAchGet1Key = (DataTable)BROTHER_BANK_TEMP.SearchACH1Key(sUserId, sReceiveNumber);
        DataTable dtblAchGet1Keyadd = (DataTable)BRAuto_Pay_Popul.SearchQryACH1Key(sUserId, sReceiveNumber);
        if (dtblAchGet1Key == null)
        {
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        string sBank_Code_S = CommonFunction.GetSubString(txtAccNoBank.Text.Trim(), 0, 3);
        string sLongBankNo = lblLongBankNo.Text.Trim();
        string sAccNo = this.txtAccNo.Text.Trim();
        string sPayWay = txtPayWay.Text.Trim();
        string sAccID = txtAccID.Text.Trim().ToUpper();
        string sBcycleCode = txtBcycleCode.Text.Trim();
        string sMobilePhone = txtMobilePhone.Text.Trim();
        string sEmail = txtEmail.Text.Trim();
        string sEBill = txtEBill.Text.Trim();
        string sBuildDate = lbBuildDateText.Text;
        string sApplyType = txtApplyType.Text.Trim().ToUpper();
        string sTranCode = txtTranCode.Text.Trim();
        //*Auto_Pay_Popul表中需要添加的字段
        string sCaseClass = txtCaseClass.Text.Trim().Substring(0, 2);
        string sPopulEmpNo = txtPopulEmpNO.Text.Trim().PadLeft(8, '0');
        string sPopulNo = txtPopulNo.Text.Trim();

        if (dtblAchGet1Key.Rows.Count > 0)
        {
            //*查詢出有一Key資料，則更新資料
            if (BROTHER_BANK_TEMP.UpdateKeyInfo(sBank_Code_S, sLongBankNo, sAccNo, sPayWay, sAccID, sBcycleCode, sMobilePhone, sEmail, sEBill, sBuildDate, sReceiveNumber, sApplyType, sTranCode, eAgentInfo.agent_id.ToString().Trim(), System.DateTime.Now.ToString("yyyyMMdd"), "1", sUserId) == false)
            {
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("01_01010500_014");
                return;
            }
            //*查詢出Auto_Pay_Popul有一Key資料，則更新資料,執行Auto_Pay_Popul的資料修改
            if (dtblAchGet1Keyadd.Rows.Count > 0)
            {
                if (BRAuto_Pay_Popul.UpdateKeyInfo(sReceiveNumber, sUserId, sCaseClass, sPopulNo, sPopulEmpNo, "1", System.DateTime.Now.ToString("yyyyMMdd"), eAgentInfo.agent_id.ToString().Trim(), "C", "N") == false)

                //* 顯示端末訊息
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01010500_014");
                    return;
                }
            }
            else
            {
                //*Auto_Pay_Popul沒有一Key資料，寫入一Key資料
                EntityAuto_Pay_Popul eBankadd = new EntityAuto_Pay_Popul();
                eBankadd.Receive_Number = sReceiveNumber;
                eBankadd.Cus_Id = sUserId;
                eBankadd.KeyIn_Flag = "1";
                eBankadd.Popul_EmpNo = sPopulEmpNo;
                eBankadd.Popul_No = sPopulNo;
                eBankadd.Case_Class = sCaseClass;
                eBankadd.User_Id = eAgentInfo.agent_id.ToString().Trim();
                eBankadd.mod_date = System.DateTime.Now;
                eBankadd.isEnd = "N";
                if (this.Session["Acct_No_befor"] == null || this.Session["Acct_No_befor"].ToString() == "")
                {
                    eBankadd.FUNCTION_CODE = "A";
                }
                else
                {
                    eBankadd.FUNCTION_CODE = "C";
                }
                try
                {
                    BRAuto_Pay_Popul.AddEntity(eBankadd);
                }
                catch
                {
                    //*寫入資料失敗
                    base.strClientMsg += MessageHelper.GetMessage("01_01010500_015");
                    return;
                }

            }

        }
        else
        {
            //*一Key無資料則新增資料
            //*沒有一Key資料，寫入一Key資料
            EntityOTHER_BANK_TEMP eBank = new EntityOTHER_BANK_TEMP();
            eBank.Cus_ID = sUserId;
            //增加UserName 20130417 CasperShih
            eBank.Cus_Name = this.lblUserNameText.Text.Trim();
            eBank.Other_Bank_Code_S = sBank_Code_S;
            eBank.Other_Bank_Code_L = sLongBankNo;
            eBank.Other_Bank_Acc_No = sAccNo;
            eBank.Other_Bank_Pay_Way = sPayWay;
            eBank.bcycle_code = sBcycleCode;
            eBank.Mobile_Phone = sMobilePhone;
            eBank.E_Mail = sEmail;
            eBank.E_Bill = sEBill;
            eBank.Build_Date = sBuildDate;
            eBank.Receive_Number = sReceiveNumber;
            eBank.Other_Bank_Cus_ID = sAccID;
            eBank.Apply_Type = sApplyType;
            eBank.Deal_No = sTranCode;
            eBank.KeyIn_Flag = "1";
            eBank.Upload_flag = "N";
            eBank.user_id = eAgentInfo.agent_id.ToString().Trim();
            eBank.mod_date = System.DateTime.Now.ToString("yyyyMMdd");
            eBank.Oper_Flag = "0";

            eBank.Deal_S_No = "";
            eBank.Acc_No_D = "";
            eBank.Acc_No_L = "";
            eBank.Acc_No = "";
            eBank.Pay_Way = "";
            eBank.Acc_ID = "";
            eBank.ACH_Return_Code = "";
            eBank.Pcmc_Return_Code = "";
            eBank.C1342_Return_Code = "";
            eBank.Batch_no = "";
            eBank.Pcmc_Upload_flag = "";
            eBank.Ach_hold = "0";

            try
            {
                BROTHER_BANK_TEMP.AddEntity(eBank);
            }
            catch
            {
                //*寫入資料失敗
                base.strClientMsg += MessageHelper.GetMessage("01_01010500_015");
                return;
            }


            //*查詢出Auto_Pay_Popul有一Key資料，則更新資料,執行Auto_Pay_Popul的資料修改
            if (dtblAchGet1Keyadd.Rows.Count > 0)
            {
                if (BRAuto_Pay_Popul.UpdateKeyInfo(sReceiveNumber, sUserId, sCaseClass, sPopulNo, sPopulEmpNo, "1", System.DateTime.Now.ToString("yyyyMMdd"), eAgentInfo.agent_id.ToString().Trim(), "A", "N") == false)

                //* 顯示端末訊息
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01010500_014");
                    return;
                }
            }
            else
            {
                //*Auto_Pay_Popul沒有一Key資料，寫入一Key資料
                EntityAuto_Pay_Popul eBankadd = new EntityAuto_Pay_Popul();
                eBankadd.Receive_Number = sReceiveNumber;
                eBankadd.Cus_Id = sUserId;
                eBankadd.KeyIn_Flag = "1";
                eBankadd.Popul_EmpNo = sPopulEmpNo;
                eBankadd.Popul_No = sPopulNo;
                eBankadd.Case_Class = sCaseClass;
                eBankadd.User_Id = eAgentInfo.agent_id.ToString().Trim();
                eBankadd.mod_date = System.DateTime.Now;
                eBankadd.isEnd = "N";
                if (this.Session["Acct_No_befor"] == null || this.Session["Acct_No_befor"].ToString() == "")
                {
                    eBankadd.FUNCTION_CODE = "A";
                }
                else
                {
                    eBankadd.FUNCTION_CODE = "C";
                }
                try
                {
                    BRAuto_Pay_Popul.AddEntity(eBankadd);
                }
                catch
                {
                    //*寫入資料失敗
                    base.strClientMsg += MessageHelper.GetMessage("01_01010500_015");
                    return;
                }

            }
        }

        if (BROTHER_BANK_TEMP.UpdateKeyInfo(sReceiveNumber, sUserId) == false)
        {
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_01010500_014");
            //return false;
        }

        SetControlsEnabled(false);
        this.txtReceiveNumber.Text = "";
        this.txtUserId.Text = "";

        base.strClientMsg += MessageHelper.GetMessage("01_01010500_016");
        base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
    }

    /// 作者 chenjingxian
    /// 創建日期：2009/10/21
    /// 修改日期：2009/10/21 
    /// <summary>
    /// 點選查詢按鈕
    /// </summary>
    /// 
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
        Hashtable htOutputP4_JCF6 = new Hashtable();//*查詢第一卡人檔下行
        Hashtable htOutputP4_JCDK = new Hashtable();//*查詢第二卡人檔下行

        bool bIsReadOnly = false;
        string sReceiveNumber = this.txtReceiveNumber.Text.Trim();
        this.txtUserId.Text = this.txtUserId.Text.Trim().ToUpper();

        //*檢核輸入的收件編號和身分證號碼是否符合規則
        //20131029 Casper 收編驗證改成彈出警示訊息 不在阻擋
        if (!CheckInPutInfo())
        {
            //*失敗
            //return;         

        }

        SetControlsEnabled(false);

        DataSet dstInfo;//*記錄查詢結果

        //判斷是否有 ACH 的資料 (Table:Ach598Data)
        dstInfo = BRACH598DATA.Select(this.txtUserId.Text);

        if (dstInfo == null)
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
            return;
        }
        else
        {
            if (dstInfo.Tables[0].Rows.Count > 0)
            {
                this.txtTranCode.Text = "598";
                gsTranCode = "598";
            }
            else
            {
                this.txtTranCode.Text = "851";
                gsTranCode = "851";
            }

        }

        if (GetMainframeData(ref htOutputP4_JCF6, ref htOutputP4_JCDK))
        {
            lblUserNameText.Text = htOutputP4_JCF6["NAME_1"].ToString().Trim();
            //若姓名為空則為VD不能進行ACH作業
            if (lblUserNameText.Text.Trim().Length == 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01010500_012");
                return;
            }

            ViewState["HtgInfoJCF6"] = htOutputP4_JCF6;
            ViewState["HtgInfoJCDK"] = htOutputP4_JCDK;

            this.Session["Acct_No_befor"] = CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 4, 26);

            //*查詢他行自扣資料 (Other_Bank_Temp) (已分配Batch_No,但尚未回貼主機)
            if (QueryWithBatch(sReceiveNumber, ref bIsReadOnly))
            {
                if (!bIsReadOnly)
                {
                    if (QueryWith1KEY(sReceiveNumber))
                    {
                        // lbBuildDateText.Text = System.DateTime.Now.ToString("yyyyMMdd");
                        //*顯示民國年月日
                        lbBuildDateText.Text = System.DateTime.Now.AddYears(-1911).ToString("yyyyMMdd").PadLeft(8, '0');
                        lbApplyCodeText.Text = "N";
                        SetControlsEnabledText(true);
                        base.strClientMsg = MessageHelper.GetMessage("01_01010500_005");
                        base.sbRegScript.Append(BaseHelper.SetFocus("txtAccNoBank"));

                    }
                    else
                    {
                        SetControlsEnabled(false);
                        base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                        return;
                    }
                }
            }
            else
            {
                SetControlsEnabled(false);
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                return;
            }
        }
        else
        {
            SetControlsEnabled(false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            return;
        }

    }

    /// <summary>
    /// 檢核收件編號和身分證號輸入是否符合規則
    /// </summary>
    /// <returns>檢核成功或失敗</returns>
    private bool CheckInPutInfo()
    {
        string strSql = ""; //*查詢資料庫的顯示內容
        DataSet dstInfo;//*記錄查詢結果
        string sReceiveNumber = this.txtReceiveNumber.Text.Trim();

        /*若收件編號已被使用，不能再被使用*/
        strSql = "Receive_Number,Cus_ID";
        dstInfo = BROTHER_BANK_TEMP.Select1(sReceiveNumber, strSql);

        if (dstInfo == null)
        {
            //*查詢資料庫時發生錯誤
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
            return false;
        }

        if (dstInfo.Tables[0].Rows.Count > 0)
        {
            string dtbUserId = dstInfo.Tables[0].Rows[0][1].ToString().Trim();
            //*此收件編號已被使用，不能再被使用
            base.strClientMsg = BaseHelper.GetShowText("01_01110001_002") + ": 此收件編號已被 " + dtbUserId + " 使用,請確認後再繼續操作~";
            base.sbRegScript.Append("alert('" + BaseHelper.GetShowText("01_01110001_002") + ": 此收件編號已被 " + dtbUserId + " 使用,請確認後再繼續操作~');");

            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            this.txtReceiveNumber.BackColor = Color.Red;

            return false;
        }

        return true;
    }


    /// 作者 chenjingxian
    /// 創建日期：2009/10/21
    /// 修改日期：2009/10/21 
    /// <summary>
    /// 銀行訊息下拉框內容發生變化，銀行7碼和銀行名稱也改變
    /// </summary>
    protected void dropAccNo_TextChanged(object sender, EventArgs e)
    {
        lblLongBankNo.Text = "";
        lblLongBankName.Text = "";

        if (dropAccNo.SelectedItem.Text.Trim() != "")
        {
            DataTable dtblBANKINFO = (DataTable)BROTHER_BANK_TEMP.SearchBankInfo(dropAccNo.SelectedItem.Text.Trim());
            if (dtblBANKINFO == null)
            {
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return;
            }

            if (dtblBANKINFO.Rows.Count > 0)
            {
                lblLongBankNo.Text = dtblBANKINFO.Rows[0][1].ToString().Trim();
                lblLongBankName.Text = dtblBANKINFO.Rows[0][2].ToString().Trim();
            }
            base.sbRegScript.Append(BaseHelper.SetFocus("txtAccNoBank"));
            txtAccNoBank.Text = dropAccNo.SelectedItem.Text;
        }
    }


    /// 作者 chenjingxian
    /// 創建日期：2009/10/21
    /// 修改日期：2009/10/21 
    /// <summary>
    /// 隱藏按鈕點選事件，更改網頁中的銀行訊息
    /// </summary>
    protected void btnHiden_Click(object sender, EventArgs e)
    {
        lblLongBankNo.Text = "";
        lblLongBankName.Text = "";

        if (txtAccNoBank.Text.Trim() != "")
        {
            DataTable dtblBANKINFO = (DataTable)BROTHER_BANK_TEMP.SearchBankInfo(txtAccNoBank.Text.Trim());
            if (dtblBANKINFO == null)
            {
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return;
            }

            if (dtblBANKINFO.Rows.Count > 0)
            {
                txtAccNoBank.Text = SelectValue(dtblBANKINFO.Rows[0][0].ToString().Trim());
                lblLongBankNo.Text = dtblBANKINFO.Rows[0][1].ToString().Trim();
                lblLongBankName.Text = dtblBANKINFO.Rows[0][2].ToString().Trim();

            }
            else
            {
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010500_022") + "');");
                base.strClientMsg += MessageHelper.GetMessage("01_01010500_022");

            }
        }
        else
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010500_022") + "');");
            base.strClientMsg += MessageHelper.GetMessage("01_01010500_022");

        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtAccNoBank"));

    }

    /// 作者 chenjingxian
    /// 創建日期：2009/10/21
    /// 修改日期：2009/10/21 
    /// <summary>
    /// 隱藏按鈕點選事件，更改網頁中的銀行訊息
    /// </summary>
    protected void btnHidenC_Click(object sender, EventArgs e)
    {
        lblLongBankNo.Text = "";
        lblLongBankName.Text = "";
        if (txtAccNoBank.Text.Trim() != "")
        {
            DataTable dtblBANKINFO = (DataTable)BROTHER_BANK_TEMP.SearchBankInfo(txtAccNoBank.Text.Trim());
            if (dtblBANKINFO == null)
            {
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return;
            }

            if (dtblBANKINFO.Rows.Count > 0)
            {
                // txtAccNoBank.Text = dtblBANKINFO.Rows[0][0].ToString().Trim();
                txtAccNoBank.Text = SelectValue(dtblBANKINFO.Rows[0][0].ToString().Trim());
                lblLongBankNo.Text = dtblBANKINFO.Rows[0][1].ToString().Trim();
                lblLongBankName.Text = dtblBANKINFO.Rows[0][2].ToString().Trim();
            }
        }
        if (txtHiden.Text.Trim().Length != 0)
        {
            base.sbRegScript.Append(BaseHelper.SetFocus(txtHiden.Text.Trim()));
        }
    }

    /// 作者 chenjingxian
    /// 創建日期：2009/10/21
    /// 修改日期：2009/10/21 
    /// <summary>
    /// 點選提交按鈕
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        this.txtApplyType.Text = this.txtApplyType.Text.Trim().ToUpper();
        string sApplyType = this.txtApplyType.Text;
        if (txtCaseClass.Text == string.Empty)
        {
            MessageHelper.GetMessage("01_01010500_023");
            return;
        }

        if (sApplyType == "D" || sApplyType == "O")
        {

            if ((this.txtTranCode.Text.Trim() != gsTranCode) && gsTranCode.Length != 0)
            {
                base.sbRegScript.Append("if(confirm('" + MessageHelper.GetMessage("01_01010500_018") + "')) {$('#btnHidenD').click();}else{document.getElementById('txtApplyType').focus();}");

            }
            else
            {
                btnHidenD_Click(sender, e);
            }
        }
        else
        {
            if (sApplyType == "A")
            {
                this.txtTranCode.Text = "851";
            }
            btnHidenD_Click(sender, e);
        }
    }

    /// 作者 chenjingxian
    /// 創建日期：2009/10/21
    /// 修改日期：2009/10/21 
    /// <summary>
    /// 申請類別下拉選項發生變化，改變交易代號內容
    /// </summary>
    protected void dropApplyType_TextChanged(object sender, EventArgs e)
    {
        if (dropApplyType.Text.Trim().ToUpper() == "A")
        {
            this.txtTranCode.Text = "851";
        }

        base.sbRegScript.Append(BaseHelper.SetFocus("txtApplyType"));

    }

    /// 作者 chenjingxian
    /// 創建日期：2009/10/21
    /// 修改日期：2009/10/21 
    /// <summary>
    /// 點選檢核交易代號按鈕，檢核交易代號
    /// </summary>
    protected void btnCheck_Click(object sender, EventArgs e)
    {
        if (this.txtApplyType.Text.Trim().ToUpper() == "A")
        {
            this.txtTranCode.Text = "851";
            base.sbRegScript.Append(BaseHelper.SetFocus("txtTranCode"));
        }
        else if (this.txtApplyType.Text.Trim().ToUpper() == "D")
        {

            if ((this.txtTranCode.Text.Trim() != gsTranCode) && gsTranCode.Length != 0)
            {
                base.sbRegScript.Append("if(confirm('" + MessageHelper.GetMessage("01_01010500_018") + "')) {document.getElementById('txtTranCode').value='" + gsTranCode + "' ;  document.getElementById('txtTranCode').focus(); }");
            }
        }
        else
        {
            ;
        }
    }



    /// 作者 chenjingxian
    /// 創建日期：2009/10/21
    /// 修改日期：2009/10/21 ; 2021/02/08_Ares_Stanley-調整下拉選單
    /// <summary>
    /// 點選檢核銀行代號按鈕，檢核交易代號
    /// </summary>
    protected void btnBankCheck_Click(object sender, EventArgs e)
    {
        lblLongBankNo.Text = "";
        lblLongBankName.Text = "";
        txtAccNoBank.Text = txtAccNoBankH.Text;
        if (txtAccNoBank.Text.Trim() != "")
        {
            DataTable dtblBANKINFO = (DataTable)BROTHER_BANK_TEMP.SearchBankInfo(txtAccNoBank.Text.Trim());
            if (dtblBANKINFO == null)
            {
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return;
            }

            if (dtblBANKINFO.Rows.Count > 0)
            {
                txtAccNoBank.Text = SelectValue(dtblBANKINFO.Rows[0][0].ToString().Trim());
                lblLongBankNo.Text = dtblBANKINFO.Rows[0][1].ToString().Trim();
                lblLongBankName.Text = dtblBANKINFO.Rows[0][2].ToString().Trim();

            }
            else
            {
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010500_022") + "');");
                base.strClientMsg += MessageHelper.GetMessage("01_01010500_022");

            }
        }
        else
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010500_022") + "');");
            base.strClientMsg += MessageHelper.GetMessage("01_01010500_022");

        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtAccNoBank"));
    }
    #endregion

    #region 方法區

    /// <summary>
    /// 查找顯示下拉列表的Text
    /// </summary>
    /// <param name="strAccNoBank">銀行3碼</param>
    /// <returns>銀行3碼+銀行名稱</returns>
    private string SelectValue(string strAccNoBank)
    {
        ListItem listItem = dropAccNo.Items.FindByValue(strAccNoBank);

        if (listItem != null)
        {
            return listItem.Text;
        }
        return strAccNoBank;
    }

    /// <summary>
    /// 查找顯示下拉列表的Text
    /// </summary>
    /// <param name="strCaseNo">案件類別代碼</param>
    /// <returns>案件類別代碼+案件類別名稱</returns>
    private string SelectValue2(string strCaseNo)
    {
        ListItem listItem = dropCaseClass.Items.FindByValue(strCaseNo);

        if (listItem != null)
        {
            return listItem.Text;
        }
        return strCaseNo;
    }


    /// 作者 chenjingxian
    /// 創建日期：2009/10/21
    /// 修改日期：2009/10/21 
    /// <summary>
    /// 加載下拉列表
    /// </summary>
    private void LoadDropDownList()
    {
        DataTable dtblResult = new DataTable();//*記錄查詢Table結果
        if (BaseHelper.GetCommonProperty("01", "17", ref dtblResult))
        {
            for (int i = 0; i < dtblResult.Rows.Count; i++)
            {
                ListItem listItem = new ListItem();
                listItem.Text = dtblResult.Rows[i][0].ToString() + " " + dtblResult.Rows[i][1].ToString();
                listItem.Value = dtblResult.Rows[i][0].ToString();
                this.dropAccNo.Items.Add(listItem);
            }
        }

        dtblResult.Clear();

        if (BaseHelper.GetCommonProperty("01", "15", ref dtblResult))
        {
            for (int i = 0; i < dtblResult.Rows.Count; i++)
            {
                ListItem listItem = new ListItem();
                listItem.Text = dtblResult.Rows[i][1].ToString();
                listItem.Value = dtblResult.Rows[i][0].ToString();
                this.dropBcycleCode.Items.Add(listItem);
            }
        }

        dtblResult.Clear();

        if (BaseHelper.GetCommonProperty("01", "19", ref dtblResult))
        {
            for (int i = 0; i < dtblResult.Rows.Count; i++)
            {
                ListItem listItem = new ListItem();
                listItem.Text = dtblResult.Rows[i][1].ToString();
                listItem.Value = dtblResult.Rows[i][0].ToString();
                this.dropApplyType.Items.Add(listItem);
            }
        }

        dtblResult.Clear();

        if (BaseHelper.GetCommonProperty("01", "20", ref dtblResult))
        {
            for (int i = 0; i < dtblResult.Rows.Count; i++)
            {
                ListItem listItem = new ListItem();
                listItem.Text = dtblResult.Rows[i][1].ToString();
                listItem.Value = dtblResult.Rows[i][0].ToString();
                this.dropTranCode.Items.Add(listItem);
            }
        }

        dtblResult.Clear();

        if (BaseHelper.GetCommonProperty("01", "24", ref dtblResult))
        {
            for (int i = 0; i < dtblResult.Rows.Count; i++)
            {
                ListItem listItem = new ListItem();
                listItem.Text = dtblResult.Rows[i][0].ToString() + "-" + dtblResult.Rows[i][1].ToString();
                listItem.Value = dtblResult.Rows[i][0].ToString();
                this.dropCaseClass.Items.Add(listItem);
            }
        }
    }

    /// 作者 chenjingxian
    /// 創建日期：2009/10/22
    /// 修改日期：2009/10/22
    /// <summary>
    /// 設置控件是否可用
    /// </summary>
    /// <param name="blnEnabled">True可用，false不可用</param>
    private void SetControlsEnabled(bool blnEnabled)
    {
        this.lblUserNameText.Text = "";
        this.txtAccNoBank.Text = "";
        this.lblLongBankNo.Text = "";
        this.lblLongBankName.Text = "";
        this.txtAccNo.Text = "";
        this.txtPayWay.Text = "";
        this.txtAccID.Text = "";
        this.txtBcycleCodeText.Text = "";
        this.txtBcycleCode.Text = "";
        this.txtMobilePhone.Text = "";
        this.txtEmail.Text = "";
        this.txtEBill.Text = "";
        this.lbBuildDateText.Text = "";
        this.txtApplyType.Text = "";
        this.txtTranCode.Text = "";
        this.lbNoteText.Text = "";
        this.lbReturnDate.Text = "";
        this.lbAchText.Text = "";
        this.lbApplyCodeText.Text = "";
        this.txtCaseClass.Text = "";
        this.txtPopulEmpNO.Text = "";
        this.txtPopulNo.Text = "";

        SetControlsEnabledText(blnEnabled);
    }


    /// 作者 chenjingxian
    /// 創建日期：2009/10/22
    /// 修改日期：2009/10/22
    /// <summary>
    /// 將網頁中的輸入框字體顏色變為黑色
    /// </summary>
    /// <param name="blnEnabled">True可用，false不可用</param>
    private void SetColorBlack()
    {
        this.txtReceiveNumber.ForeColor = Color.Black;
        this.txtUserId.ForeColor = Color.Black;
        this.txtAccNoBank.ForeColor = Color.Black;
        this.txtAccNo.ForeColor = Color.Black;
        this.txtPayWay.ForeColor = Color.Black;
        this.txtAccID.ForeColor = Color.Black;
        this.txtBcycleCodeText.ForeColor = Color.Black;
        this.txtBcycleCode.ForeColor = Color.Black;
        this.txtMobilePhone.ForeColor = Color.Black;
        this.txtEmail.ForeColor = Color.Black;
        this.txtEBill.ForeColor = Color.Black;
        this.txtApplyType.ForeColor = Color.Black;
        this.txtTranCode.ForeColor = Color.Black;
        this.txtCaseClass.ForeColor = Color.Black;
        this.txtPopulEmpNO.ForeColor = Color.Black;
        this.txtPopulNo.ForeColor = Color.Black;
    }

    /// 作者 chenjingxian
    /// 創建日期：2009/10/22
    /// 修改日期：2009/10/22
    /// <summary>
    /// 設置控件是否可用
    /// </summary>
    /// <param name="blnEnabled">True可用，false不可用</param>
    private void SetControlsEnabledText(bool blnEnabled)
    {
        this.dropAccNo.Enabled = blnEnabled;
        this.txtAccNoBank.Enabled = blnEnabled;
        this.txtAccNo.Enabled = blnEnabled;
        this.txtPayWay.Enabled = blnEnabled;
        this.txtAccID.Enabled = blnEnabled;
        //this.txtBcycleCodeText.Enabled = blnEnabled;
        this.txtBcycleCode.Enabled = blnEnabled;
        this.dropBcycleCode.Enabled = blnEnabled;
        this.txtMobilePhone.Enabled = blnEnabled;
        this.txtEmail.Enabled = blnEnabled;
        this.txtEBill.Enabled = blnEnabled;
        this.txtApplyType.Enabled = blnEnabled;
        this.txtTranCode.Enabled = blnEnabled;
        this.btnSubmit.Enabled = blnEnabled;
        this.dropApplyType.Enabled = blnEnabled;
        this.dropTranCode.Enabled = blnEnabled;
        this.btnBankCheck.Enabled = blnEnabled;
        this.btnCheck.Enabled = blnEnabled;
        //this.dropCaseClass.Enabled = blnEnabled;
        //this.txtCaseClass.Enabled = blnEnabled;
        this.txtPopulEmpNO.Enabled = blnEnabled;
        this.txtPopulNo.Enabled = blnEnabled;
    }

    //Add by Carolyn
    private bool QueryWithBatch(string sReceiveNumber, ref bool bIsReadOnly)
    {
        DataSet dstInfo;//*記錄查詢結果

        dstInfo = BROTHER_BANK_TEMP.SelectWithBatch(sReceiveNumber);
        if (dstInfo == null)//*查詢資料庫時發生錯誤
        {
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            SetControlsEnabled(false);
            return false;
        }

        //*已壓上Batch_NO , 不允許修改，只能查看此筆資料
        if (dstInfo.Tables[0].Rows.Count > 0)
        {
            bIsReadOnly = true;
            dstInfo = null;

            dstInfo = BROTHER_BANK_TEMP.SelectDistBatch(this.txtReceiveNumber.Text.Trim());

            if (dstInfo == null)
            {
                //*查詢資料庫時發生錯誤
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                SetControlsEnabled(false);
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return false;
            }

            if (dstInfo.Tables[0].Rows.Count < 1)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01010500_009");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                return false;
            }

            //*將查詢出的訊息顯示與網頁中
            txtAccNoBank.Text = SelectValue(dstInfo.Tables[0].Rows[0][0].ToString());
            txtAccNo.Text = dstInfo.Tables[0].Rows[0][1].ToString();
            txtPayWay.Text = dstInfo.Tables[0].Rows[0][2].ToString();
            txtAccID.Text = dstInfo.Tables[0].Rows[0][3].ToString();
            txtBcycleCodeText.Text = dstInfo.Tables[0].Rows[0][4].ToString();
            txtBcycleCode.Text = dstInfo.Tables[0].Rows[0][4].ToString();
            txtMobilePhone.Text = dstInfo.Tables[0].Rows[0][5].ToString();
            txtEmail.Text = dstInfo.Tables[0].Rows[0][6].ToString();
            txtEBill.Text = dstInfo.Tables[0].Rows[0][7].ToString();
            lbBuildDateText.Text = dstInfo.Tables[0].Rows[0][8].ToString();
            txtApplyType.Text = dstInfo.Tables[0].Rows[0][9].ToString();
            txtTranCode.Text = dstInfo.Tables[0].Rows[0][10].ToString();

            EntitySet<EntityACH_RTN_INFO> eAchRtnInfoSet = null;
            try
            {
                //*查詢ACH中文回覆訊息
                eAchRtnInfoSet = BRACH_RTN_INFO.SelectEntitySet(dstInfo.Tables[0].Rows[0][11].ToString().Trim());
            }
            catch
            {
                base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
                return false;
            }

            if (eAchRtnInfoSet != null && eAchRtnInfoSet.Count > 0)
            {
                //*將ACH中文回覆訊息顯示與網頁中
                lbAchText.Text = eAchRtnInfoSet.GetEntity(0).Ach_Rtn_Msg.Trim();
            }

            EntitySet<EntityBATCH> eBatchSet = null;
            try
            {
                //*查詢此筆資料是否送出P02
                eBatchSet = BRBATCH.SelectEntitySet(dstInfo.Tables[0].Rows[0][12].ToString().Trim());
            }
            catch
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return false;
            }
            if (eBatchSet != null && eBatchSet.Count > 0)
            {
                if (eBatchSet.GetEntity(0).P02_flag.Trim() == "Y")
                {
                    //*已送P02
                    lbNoteText.Text = "*";
                }

                lbReturnDate.Text = eBatchSet.GetEntity(0).R02DateReceive.Trim();

                if (!(eBatchSet.GetEntity(0).R02_flag == "2" || eBatchSet.GetEntity(0).R02_flag == "3"))
                {
                    //*若未收到R02回覆，清空ACH中文訊息
                    lbAchText.Text = "";
                }
            }
            else
            {
                lbAchText.Text = "";
            }

            lbApplyCodeText.Text = "N";
            dstInfo = BaseHelper.GetCommonPropertySet("01", "16", CommonFunction.GetSubString(this.txtAccNoBank.Text.Trim(), 0, 3));
            if (dstInfo == null)
            {
                //*查詢資料庫時發生錯誤
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return false;
            }
            else
            {
                if (dstInfo.Tables[0].Rows.Count > 0)
                {
                    this.lblLongBankNo.Text = dstInfo.Tables[0].Rows[0][0].ToString().Trim();
                }
            }

            dstInfo = BaseHelper.GetCommonPropertySet("01", "17", CommonFunction.GetSubString(this.txtAccNoBank.Text.Trim(), 0, 3));

            if (dstInfo == null)
            {
                //*查詢資料庫時發生錯誤
                base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
                return false;

            }
            else
            {
                if (dstInfo.Tables[0].Rows.Count > 0)
                {
                    this.lblLongBankName.Text = dstInfo.Tables[0].Rows[0][0].ToString().Trim();
                }
            }

            base.strClientMsg = MessageHelper.GetMessage("01_01010500_009");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
        }
        else
        {
            bIsReadOnly = false;
        }
        return true;

    }

    //Add by Carolyn
    private bool QueryWith1KEY(string sReceiveNumber)
    {
        DataSet dstInfo;//*記錄查詢結果
        //*輸入的收件編號是否已經有完成的ACH 流程的資料 (已上傳主機)
        DataTable dtblAch = (DataTable)BROTHER_BANK_TEMP.SearchACH(sReceiveNumber);
        if (dtblAch == null)
        {
            //* 顯示端末訊息
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
            return false;
        }
        else
        {
            if (dtblAch.Rows.Count > 0) //表示該收件編號已被使用,不能再被使用
            {
                base.strClientMsg = MessageHelper.GetMessage("01_01010500_010");
                return false;
            }
        }

        //*輸入的收件編號是否有還未分配批次號的資料，若有，比較資料檔里收件編號對應的身分證號是否和畫面中輸入的身分證號一樣
        DataTable dtblAchGet = (DataTable)BROTHER_BANK_TEMP.SearchACHGET(sReceiveNumber);
        if (dtblAchGet == null)
        {
            //* 顯示端末訊息
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
            return false;
        }
        else
        {
            if (dtblAchGet.Rows.Count > 0)
            {
                string dtbUserId = dtblAchGet.Rows[0][1].ToString();
                if (this.txtUserId.Text.Trim().ToUpper() != dtbUserId) //收件編號重複
                {
                    base.strClientMsg = MessageHelper.GetMessage("01_01010500_011") + ": 此收件編號已被 " + dtbUserId + " 使用";
                    base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010500_011") + ": 此收件編號已被 " + dtbUserId + " 使用');");
                    this.txtReceiveNumber.ForeColor = Color.Red;
                    return false;
                }
            }
        }

        //*查詢一Key資料
        DataTable dtblAchGet1Key = (DataTable)BROTHER_BANK_TEMP.SearchQryACH1Key(this.txtUserId.Text.Trim().ToUpper(), sReceiveNumber);
        DataTable dtblAchGet1Keyadd = (DataTable)BRAuto_Pay_Popul.SearchQryACH1Key(this.txtUserId.Text.Trim().ToUpper(), sReceiveNumber);
        if (dtblAchGet1Key == null)
        {
            //* 顯示端末訊息
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
            return false;
        }

        if (dtblAchGet1Key.Rows.Count > 0)
        {
            //*查詢出有一Key資料，則將資料庫里的一Key資料付給畫面中的控件值
            if (dtblAchGet1Key.Rows[0][0].ToString().Trim().Length == 3)
            {
                txtAccNoBank.Text = SelectValue(dtblAchGet1Key.Rows[0][0].ToString().Trim());
                txtAccNoBankH.Text = txtAccNoBankH.Text;
                txtAccNo.Text = dtblAchGet1Key.Rows[0][1].ToString().Trim();
            }
            txtPayWay.Text = dtblAchGet1Key.Rows[0][2].ToString().Trim();
            txtAccID.Text = dtblAchGet1Key.Rows[0][3].ToString().Trim();
            txtBcycleCodeText.Text = dtblAchGet1Key.Rows[0][4].ToString().Trim();
            txtBcycleCode.Text = txtBcycleCodeText.Text;
            txtMobilePhone.Text = dtblAchGet1Key.Rows[0][5].ToString().Trim();
            txtEmail.Text = dtblAchGet1Key.Rows[0][6].ToString().Trim();
            txtEBill.Text = dtblAchGet1Key.Rows[0][7].ToString().Trim();
            txtApplyType.Text = dtblAchGet1Key.Rows[0][9].ToString().Trim();
            txtTranCode.Text = dtblAchGet1Key.Rows[0][10].ToString().Trim();
            if (dtblAchGet1Keyadd.Rows.Count > 0)
            {
                txtCaseClass.Text = SelectValue2(dtblAchGet1Keyadd.Rows[0][2].ToString().Trim());
                txtPopulEmpNO.Text = dtblAchGet1Keyadd.Rows[0][0].ToString().Trim();
                txtPopulNo.Text = dtblAchGet1Keyadd.Rows[0][1].ToString().Trim();
            }
            else
            {
                //this.txtCaseClass.Text = SelectValue2("03");
                this.txtCaseClass.Text = SelectValue2(txtReceiveNumber.Text.Substring(8, 2));
            }
        }
        else
        {

            Hashtable htJCF6 = (Hashtable)ViewState["HtgInfoJCF6"];
            Hashtable htJCDK = (Hashtable)ViewState["HtgInfoJCDK"];

            //*一Key無資料則顯示主機資料
            txtAccNoBank.Text = SelectValue(CommonFunction.GetSubString(htJCF6["CO_OWNER"].ToString(), 0, 3));
            txtAccNoBankH.Text = txtAccNoBankH.Text;
            txtAccNo.Text = CommonFunction.GetSubString(htJCF6["CO_OWNER"].ToString(), 4, 26);
            txtPayWay.Text = htJCF6["CO_TAX_ID_TYPE"].ToString();
            txtAccID.Text = htJCF6["DD_ID"].ToString();
            txtBcycleCodeText.Text = htJCF6["BILLING_CYCLE"].ToString();
            txtBcycleCode.Text = txtBcycleCodeText.Text;
            txtMobilePhone.Text = htJCDK["MOBILE_PHONE"].ToString();
            txtEmail.Text = htJCDK["EMAIL"].ToString();
            txtEBill.Text = htJCF6["OFF_PHONE_FLAG"].ToString();

            //*案件類別
            //this.txtCaseClass.Text = SelectValue2("03");
            this.txtCaseClass.Text = SelectValue2(txtReceiveNumber.Text.Substring(8, 2));
        }

        dstInfo = BaseHelper.GetCommonPropertySet("01", "16", CommonFunction.GetSubString(txtAccNoBank.Text.Trim(), 0, 3));
        if (dstInfo == null)
        {
            //*查詢資料庫時發生錯誤
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
            return false;
        }
        else
        {
            if (dstInfo.Tables[0].Rows.Count > 0)
            {
                this.lblLongBankNo.Text = dstInfo.Tables[0].Rows[0][0].ToString().Trim();
            }
        }

        dstInfo = BaseHelper.GetCommonPropertySet("01", "17", CommonFunction.GetSubString(txtAccNoBank.Text.Trim(), 0, 3));

        if (dstInfo == null)
        {
            //*查詢資料庫時發生錯誤
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
            return false;
        }
        else
        {
            if (dstInfo.Tables[0].Rows.Count > 0)
            {
                this.lblLongBankName.Text = dstInfo.Tables[0].Rows[0][0].ToString().Trim();
            }
        }

        return true;
    }


    //Add by Carolyn
    private bool GetMainframeData(ref Hashtable htOutputP4_JCF6, ref Hashtable htOutputP4_JCDK)
    {
        Hashtable htInputP4_JCF6 = new Hashtable();
        Hashtable htInputP4_JCDK = new Hashtable();

        string sUserId = CommonFunction.GetSubString(this.txtUserId.Text.Trim().ToUpper(), 0, 16);

        //*讀取第一卡人檔
        htInputP4_JCF6.Add("ACCT_NBR", sUserId);
        htInputP4_JCF6.Add("FUNCTION_CODE", "1");
        htInputP4_JCF6.Add("LINE_CNT", "0000");
        htOutputP4_JCF6 = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCF6, htInputP4_JCF6, false, "1", eAgentInfo);
        if (htOutputP4_JCF6.Contains("HtgMsg"))
        {
            etMstType = eMstType.Select;
            base.strHostMsg = htOutputP4_JCF6["HtgMsg"].ToString();
            base.strClientMsg = MessageHelper.GetMessage("01_00000000_026");
            return false;
        }
        base.strHostMsg = htOutputP4_JCF6["HtgSuccess"].ToString();//*主機返回成功訊息

        //*讀取第二卡人檔
        htInputP4_JCDK.Add("ACCT_NBR", sUserId);
        htInputP4_JCDK.Add("FUNCTION_CODE", "1");
        htInputP4_JCDK.Add("LINE_CNT", "0000");
        htOutputP4_JCDK = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCDK, htInputP4_JCDK, false, "1", eAgentInfo);
        if (htOutputP4_JCDK.Contains("HtgMsg"))
        {
            etMstType = eMstType.Select;
            base.strHostMsg += htOutputP4_JCDK["HtgMsg"].ToString();
            base.strClientMsg = MessageHelper.GetMessage("01_00000000_026");
            return false;
        }
        base.strHostMsg += htOutputP4_JCDK["HtgSuccess"].ToString();//*主機返回成功訊息

        return true;
    }

    #endregion

    //20160606 (U) by Tank
    [WebMethod(EnableSession = false)]
    public static int funGetDataCnt(string KeyValue)
    {
        DataTable dt = GetCustomerLog_Cnt(KeyValue);
        int dataCount = int.Parse(dt.Rows[0].ItemArray[0].ToString());
        return dataCount;
    }

    //20160606 (U) by Tank
    /// <summary>
    /// 查詢近60天內"其他作業 記錄查詢"看有無資料
    /// </summary>
    /// <param name="query_key">身分證字號</param>
    /// <param name="strMsgID">返回的錯誤ID號</param>
    /// <returns>成功時：返回查詢結果；失敗時：null</returns>
    public static DataTable GetCustomerLog_Cnt(string strQueryKey)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlcmSearchData = new SqlCommand();
            sqlcmSearchData.CommandType = CommandType.Text;

            string strQueryCmd = @"Select count(*) 
                                   From (select query_key, 
                                            case trans_id 
                                            when 'A01' then '01010100' 
                                            when 'A02' then '01010800' 
                                            when 'A04' then '01010400' 
                                            when 'A03' then '01010400' 
                                            when 'A11' then '01010200' 
                                            when 'A06' then '01010300' 
                                            when 'A14' then '01010600' 
                                            when 'B01' then '01020100' 
                                            when 'B02' then '01020300' 
                                            when 'B05' then '01020200' 
                                            when 'B04' then '01020500' 
                                            when 'B10' then '01020700' 
                                            when 'B14' then '01021300' 
                                            when 'B12' then '01020900' 
                                            when 'C02' then '01030200' 
                                            when 'D01' then '01030200' 
                                            else trans_id 
                                            end as trans_id, 
                                            field_name,before,after,user_id,mod_date,mod_time,log_flag 
                                            From customer_log with (nolock) 
                                            Union all 
                                            select query_key, 
                                            case trans_id 
                                            when 'A01' then '01010100' 
                                            when 'A02' then '01010800' 
                                            when 'A04' then '01010400' 
                                            when 'A03' then '01010400' 
                                            when 'A11' then '01010200' 
                                            when 'A06' then '01010300' 
                                            when 'A14' then '01010600' 
                                            when 'B01' then '01020100' 
                                            when 'B02' then '01020300' 
                                            when 'B05' then '01020200' 
                                            when 'B04' then '01020500' 
                                            when 'B10' then '01020700' 
                                            when 'B14' then '01021300' 
                                            when 'B12' then '01020900' 
                                            when 'C02' then '01030200' 
                                            when 'D01' then '01030200' 
                                            else trans_id 
                                            end as trans_id, 
                                            field_name,before,after,user_id,mod_date,mod_time,log_flag 
                                            From customer_log_OtherBankTemp with (nolock) 
                                       ) A 
                                   where A.trans_id='01011100' 
                                   and A.field_name='是否自扣終止' 
                                   and A.after='Y' 
                                   and A.query_key = '{0}' 
                                   and A.mod_date > convert(varchar,dateadd(d,-60,getdate()),112)";    //查近期60天內有無資料

            sqlcmSearchData.CommandText = string.Format(strQueryCmd, strQueryKey);

            //* 身分證號碼
            SqlParameter parmQueryKey = new SqlParameter("@Query_Key", strQueryKey);
            sqlcmSearchData.Parameters.Add(parmQueryKey);

            //* 查詢數據
            DataSet dstSearchData = BRASExport.SearchOnDataSet(sqlcmSearchData);
            if (null == dstSearchData)  //* 查詢數據失敗
            {
                return null;
            }
            else
            {
                //* 查詢的數據不存在時
                if (dstSearchData.Tables[0].Rows.Count == 0)
                {
                    return null;
                }
            }
            //* 查詢成功
            return dstSearchData.Tables[0];

            #endregion 依據Request查詢資料庫
        }
        catch (Exception exp)
        {
            BRCompareRpt.SaveLog(exp);
            return null;
        }
    }
}
