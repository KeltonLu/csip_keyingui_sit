//******************************************************************
//*  作    者：chenjingxian
//*  功能說明：他行自扣二Key
//*  創建日期：2009/10/21
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*Ares_jhun          2022/09/28         RQ-2022-019375-000     EDDA需求調整：查詢時新增電文P4_JCAA確認是否有正卡
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
using Framework.Common.Logging;

public partial class P010101060001 : PageBase
{
    #region 變數區
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private string gsTranCode = "";
    private EntityAuto_Pay_Popul beforeInfo;//*記錄Auto_Pay_Popul變更之前的記錄
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #endregion

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
            ReSetControls(true, false);
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"]; //*記錄網頁訊息
    }

    /// 作者 chenjingxian
    /// 創建日期：2009/10/21
    /// 修改日期：2009/10/21 
    /// <summary>
    /// 點選提交按鈕
    /// </summary>
    protected void btnHidenD_Click(object sender, EventArgs e)
    {
        DataSet dstInfo = null;
        Hashtable htEBill = (Hashtable)ViewState["vsP4JCF6Info"];

        string sAccNoBank = CommonFunction.GetSubString(txtAccNoBank.Text.Trim(), 0, 3);
        string sReceiveNumber = this.txtReceiveNumber.Text.Trim();
        string sUserId = this.txtUserId.Text.Trim().ToUpper();
        string sCompareOK;

        Hashtable htJCF6_G = (Hashtable)ViewState["HtgInfoJCF6"];
        Hashtable htJCDK_G = (Hashtable)ViewState["HtgInfoJCDK"];

        string sAuto_Pay_Setting = (htJCF6_G["CO_OWNER"].ToString().Trim() == "" && this.txtAccNo.Text.Trim() != "") ? "Y" : "N";
        string sCellP_Email_Setting = (((htJCDK_G["MOBILE_PHONE"].ToString().Trim() != this.txtMobilePhone.Text.Trim()) || (htJCDK_G["EMAIL"].ToString().Trim() != this.txtEmail.Text.Trim()))) ? "Y" : "N";
        string sE_Bill_Setting = ((htJCF6_G["OFF_PHONE_FLAG"].ToString().Trim() != this.txtEBill.Text.Trim())) ? "Y" : "N";
        string sOutputByTxt_Setting = "Y";
        string sAcct_NBR = htJCF6_G["ACCT_NBR"].ToString().Trim();

        //取得已壓上BatchNo  , 但未上送PCMC 者
        dstInfo = BROTHER_BANK_TEMP.SelectWithBatch(sReceiveNumber);


        //*查詢一Key資料
        DataTable dtblAchGet1Key = (DataTable)BROTHER_BANK_TEMP.SearchACH1Key(sUserId, sReceiveNumber);
        DataTable dtblAchGet1Keyadd = (DataTable)BRAuto_Pay_Popul.SearchQryACH1Key(sUserId, sReceiveNumber);

        if (dtblAchGet1Key == null || dtblAchGet1Key.Rows.Count < 1)
        {
            //* 顯示端末訊息
            base.strClientMsg = MessageHelper.GetMessage("01_01010600_018");
            return;
        }

        //
        //*查詢銀行代碼
        if (!CheckBankNo())
        {
            return;
        }
        //

        //*查詢二Key資料
        DataTable dtblAchGet2Key = (DataTable)BROTHER_BANK_TEMP.SearchACH2Key(sUserId, sReceiveNumber);
        DataTable dtblAchGet2Keyadd = (DataTable)BRAuto_Pay_Popul.SearchQryACH2Key(sUserId, sReceiveNumber);
        string sCaseClass = txtCaseClass.Text.Trim().Substring(0, 2);
        string sPopulEmpNo = txtPopulEmpNO.Text.Trim().PadLeft(8, '0');
        string sPopulNo = txtPopulNo.Text.Trim();

        if (dtblAchGet2Key != null && dtblAchGet2Key.Rows.Count > 0)
        {
            //*查詢出有二Key資料，則更新資料
            if (BROTHER_BANK_TEMP.UpdateKeyInfo(sAccNoBank, lblLongBankNo.Text.Trim(), this.txtAccNo.Text.Trim(), txtPayWay.Text.Trim(), txtAccID.Text.Trim().ToUpper(), txtBcycleCode.Text.Trim(), txtMobilePhone.Text.Trim(), txtEmail.Text.Trim(), txtEBill.Text.Trim(), lbBuildDateText.Text, sReceiveNumber, txtApplyType.Text.Trim().ToUpper(), txtTranCode.Text.Trim(), eAgentInfo.agent_id.ToString().Trim(), System.DateTime.Now.ToString("yyyyMMdd"), "2", sUserId) == false)
            {
                //* 顯示端末訊息
                base.strClientMsg = MessageHelper.GetMessage("01_01010600_014");
                return;
            }

            //*查詢出Auto_Pay_Popul有二Key資料，則更新資料,執行Auto_Pay_Popul的資料修改
            if (dtblAchGet2Keyadd != null && dtblAchGet2Keyadd.Rows.Count > 0)
            {
                if (BRAuto_Pay_Popul.UpdateKeyInfo(sReceiveNumber, sUserId, sCaseClass, sPopulNo, sPopulEmpNo, "2", System.DateTime.Now.ToString("yyyyMMdd"), eAgentInfo.agent_id.ToString().Trim(), "C", "N") == false)

                //* 顯示端末訊息
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01010600_014");
                    return;
                }
            }
            else
            {
                //*Auto_Pay_Popul沒有二Key資料，寫入二Key資料
                EntityAuto_Pay_Popul eBankadd = new EntityAuto_Pay_Popul();
                eBankadd.Receive_Number = sReceiveNumber;
                eBankadd.Cus_Id = sUserId;
                eBankadd.KeyIn_Flag = "2";
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
                    base.strClientMsg += MessageHelper.GetMessage("01_01010600_015");
                    return;
                }

            }
        }
        else
        {
            //查詢出無二Key資料，則新增資料
            EntityOTHER_BANK_TEMP eBank = new EntityOTHER_BANK_TEMP();
            eBank.Cus_ID = sUserId;
            //增加UserName 20130417 CasperShih
            eBank.Cus_Name = this.lblUserNameText.Text.Trim();
            eBank.Other_Bank_Code_S = sAccNoBank;
            eBank.Other_Bank_Code_L = lblLongBankNo.Text.Trim();
            eBank.Other_Bank_Acc_No = this.txtAccNo.Text.Trim();
            eBank.Other_Bank_Pay_Way = txtPayWay.Text.Trim();
            eBank.bcycle_code = txtBcycleCode.Text.Trim();
            eBank.Mobile_Phone = txtMobilePhone.Text.Trim();
            eBank.E_Mail = txtEmail.Text.Trim();
            eBank.E_Bill = txtEBill.Text.Trim();
            eBank.Build_Date = lbBuildDateText.Text;
            eBank.Receive_Number = sReceiveNumber;
            eBank.Other_Bank_Cus_ID = txtAccID.Text.Trim().ToUpper();
            eBank.Apply_Type = txtApplyType.Text.Trim().ToUpper();
            eBank.Deal_No = txtTranCode.Text.Trim();
            eBank.KeyIn_Flag = "2";
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
                base.strClientMsg = MessageHelper.GetMessage("01_01010600_015");
                return;
            }

            //*查詢出Auto_Pay_Popul有一Key資料，則更新資料,執行Auto_Pay_Popul的資料修改
            if (dtblAchGet2Keyadd.Rows.Count > 0)
            {
                if (BRAuto_Pay_Popul.UpdateKeyInfo(sReceiveNumber, sUserId, sCaseClass, sPopulNo, sPopulEmpNo, "2", System.DateTime.Now.ToString("yyyyMMdd"), eAgentInfo.agent_id.ToString().Trim(), "A", "N") == false)

                //* 顯示端末訊息
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01010600_014");
                    return;
                }
            }
            else
            {
                //*Auto_Pay_Popul沒有一Key資料，寫入一Key資料
                EntityAuto_Pay_Popul eBankadd = new EntityAuto_Pay_Popul();
                eBankadd.Receive_Number = sReceiveNumber;
                eBankadd.Cus_Id = sUserId;
                eBankadd.KeyIn_Flag = "2";
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
                    base.strClientMsg += MessageHelper.GetMessage("01_01010600_015");
                    return;
                }

            }
        }

        //比較一Key和二Key的資料是否相同
        if (Compare(dtblAchGet1Key))
        {
            //比較新增欄位一Key和二Key的資料是否相同
            if (CompareAdd(dtblAchGet1Keyadd))
            {
                BRAuto_Pay_Popul.Update2KeyInfo(sReceiveNumber, sUserId, sCaseClass, sPopulNo, sPopulEmpNo, "2", System.DateTime.Now.ToString("yyyyMMdd"), eAgentInfo.agent_id.ToString().Trim());
                beforeInfo = (EntityAuto_Pay_Popul)this.Session["beforeInfo"];
                this.CustomerLog_OtherBankTempAdd(beforeInfo);
            }
            else //*若新增欄位一Key與二Key資料不同，提示錯誤訊息
            {
                base.strClientMsg = MessageHelper.GetMessage("01_01010600_020");
                return;
            }

            sCompareOK = "Y";
            base.strClientMsg = MessageHelper.GetMessage("01_01010600_016");

            if (BROTHER_BANK_TEMP.Update2KeyInfo(sReceiveNumber, sUserId, sCompareOK, sAuto_Pay_Setting, sCellP_Email_Setting, sE_Bill_Setting, sOutputByTxt_Setting, sAcct_NBR))
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01010600_021");

                //記錄他行自扣異動欄位 add by yanglu 2012-10-09
                this.CustomerLog_OtherBankTemp(htJCF6_G, htJCDK_G);
            }
            else
            {
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("01_01010600_014");
                return;
            }

            ReSetControls(true, false);
            this.txtReceiveNumber.Text = "";
            this.txtUserId.Text = "";
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
        }
        else //*若一Key與二Key資料不同，提示錯誤訊息
        {
            sCompareOK = "N";
            base.strClientMsg = MessageHelper.GetMessage("01_01010600_020");
        }
    }

    //Add by Carolyn
    private bool Compare(DataTable dtblAchGet1Key)
    {
        int intCount = 0;
        bool blnSame = true;

        string sAccNoBank = CommonFunction.GetSubString(txtAccNoBank.Text.Trim(), 0, 3);
        string sAccNoBank_1KEY = CommonFunction.GetSubString(dtblAchGet1Key.Rows[0][0].ToString().Trim(), 0, 3);

        //*檢查銀行3碼代號
        if (sAccNoBank != sAccNoBank_1KEY)
        {
            //*銀行3碼一Key與二Key資料不同
            intCount++;
            this.txtAccNoBank.BackColor = Color.Red;
            base.sbRegScript.Append(BaseHelper.SetFocus("txtAccNoBank"));
            blnSame = false;
        }
        else
        {
            this.txtAccNoBank.BackColor = Color.White;
        }

        //*扣繳帳號
        GetCompareResult(this.txtAccNo, dtblAchGet1Key.Rows[0][1].ToString(), ref intCount, ref blnSame);

        //*繳款狀況
        GetCompareResult(this.txtPayWay, dtblAchGet1Key.Rows[0][2].ToString(), ref intCount, ref blnSame);

        //*帳戶ID
        GetCompareResult(this.txtAccID, dtblAchGet1Key.Rows[0][3].ToString(), ref intCount, ref blnSame);

        //*帳單週期
        GetCompareResult(this.txtBcycleCode, dtblAchGet1Key.Rows[0][4].ToString(), ref intCount, ref blnSame);

        //*行動電話
        GetCompareResult(this.txtMobilePhone, dtblAchGet1Key.Rows[0][5].ToString(), ref intCount, ref blnSame);

        //*e-mail
        GetCompareResult(this.txtEmail, dtblAchGet1Key.Rows[0][6].ToString(), ref intCount, ref blnSame);

        //*電子帳單
        GetCompareResult(this.txtEBill, dtblAchGet1Key.Rows[0][7].ToString(), ref intCount, ref blnSame);

        //*申請類別
        GetCompareResult(this.txtApplyType, dtblAchGet1Key.Rows[0][9].ToString(), ref intCount, ref blnSame);

        //*交易代號
        GetCompareResult(this.txtTranCode, dtblAchGet1Key.Rows[0][10].ToString(), ref intCount, ref blnSame);

        return blnSame;

    }

    private bool CompareAdd(DataTable dtblAchGet1Keyadd)
    {
        int intCount = 0;
        bool blnSame = true;

        string sCaseclass = txtCaseClass.Text.Trim().Substring(0, 2);
        string sCaseclass_1KEY = CommonFunction.GetSubString(dtblAchGet1Keyadd.Rows[0][2].ToString().Trim(), 0, 2);

        //*檢查案件類別
        if (sCaseclass != sCaseclass_1KEY)
        {
            //*案件類別一Key與二Key資料不同
            intCount++;
            this.txtCaseClass.BackColor = Color.Red;
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCaseClass"));
            blnSame = false;
        }
        else
        {
            this.txtCaseClass.BackColor = Color.White;
        }


        //*推廣代號
        GetCompareResult(this.txtPopulNo, dtblAchGet1Keyadd.Rows[0][1].ToString(), ref intCount, ref blnSame);

        //*推廣員編
        //GetCompareResult(this.txtPopulEmpNO, dtblAchGet1Keyadd.Rows[0][0].ToString(), ref intCount, ref blnSame);
        if (this.txtPopulEmpNO.Text.ToUpper().PadLeft(8, '0').Equals(NullToString(dtblAchGet1Keyadd.Rows[0][0].ToString()).ToUpper()))
        {
            txtPopulEmpNO.BackColor = Color.White;
        }
        else
        {
            if ((++intCount).Equals(1) && this.txtPopulEmpNO.Enabled)
            {
                base.sbRegScript.Append(BaseHelper.SetFocus(txtPopulEmpNO.ID));
            }
            txtPopulEmpNO.BackColor = Color.Red;
            blnSame = false;
        }
        return blnSame;

    }

    private bool GetCompareResult(CustTextBox txtBox, string strValue, ref int intCount, ref bool blnSame)
    {
        if (txtBox.Text.ToUpper().Trim() != NullToString(strValue).ToUpper().Trim())
        {
            intCount++;
            if (intCount == 1)
            {
                if (txtBox.Enabled == true)
                {
                    base.sbRegScript.Append(BaseHelper.SetFocus(txtBox.ID));
                }
            }
            txtBox.BackColor = Color.Red;
            blnSame = false;
        }
        else
        {
            txtBox.BackColor = Color.White;
        }
        return blnSame;
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
        DataSet dstInfo;//*記錄查詢結果

        //*進入查詢首先若新增的三個欄位有值，則存進session,若沒有值，存入空字符串，以便于比對
        DataTable dtblAchGet2Keyaddbefore = (DataTable)BRAuto_Pay_Popul.SearchQryACH2Key(this.txtUserId.Text.Trim().ToUpper(), this.txtReceiveNumber.Text.Trim());
        EntityAuto_Pay_Popul Before = new EntityAuto_Pay_Popul();
        if (dtblAchGet2Keyaddbefore != null && dtblAchGet2Keyaddbefore.Rows.Count > 0)
        {
            Before.Popul_No = dtblAchGet2Keyaddbefore.Rows[0][1].ToString().Trim();
            Before.Popul_EmpNo = dtblAchGet2Keyaddbefore.Rows[0][0].ToString().Trim();
            Before.Case_Class = SelectValue2(dtblAchGet2Keyaddbefore.Rows[0][2].ToString().Trim());
            Session["beforeInfo"] = Before;

        }
        else
        {
            Before.Popul_No = "";
            Before.Popul_EmpNo = "";
            Before.Case_Class = "";
            Session["beforeInfo"] = Before;
        }
        
        // 確認是否有正卡
        if (!checkCredit())
        {
            string[] strArray = new string[] { "01_01010700_011" };
            MessageHelper.ShowMessageWithParms(this, strArray[0], strArray);

            return;
        }

        Hashtable htOutputP4_JCF6 = new Hashtable();//*查詢第一卡人檔下行
        Hashtable htOutputP4_JCDK = new Hashtable();//*查詢第二卡人檔下行

        bool bIsReadOnly = false;
        string sReceiveNumber = this.txtReceiveNumber.Text.Trim();
        this.txtUserId.Text = this.txtUserId.Text.Trim().ToUpper();
        string sUserId = this.txtUserId.Text;

        ReSetControls(true, false);

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
                ViewState["gsTranCode"] = "598";
            }
            else
            {
                this.txtTranCode.Text = "851";
                gsTranCode = "851";
                ViewState["gsTranCode"] = "851";
            }

        }

        if (GetMainframeData(ref htOutputP4_JCF6, ref htOutputP4_JCDK))
        {
            //*　讀取 1 key 資料
            DataTable dtblAchGet1Key = (DataTable)BROTHER_BANK_TEMP.SearchQryACH1Key(sUserId, sReceiveNumber);
            if (dtblAchGet1Key == null || dtblAchGet1Key.Rows.Count < 1)
            {
                //無一Key資料
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                ReSetControls(false, false);
                base.strClientMsg = MessageHelper.GetMessage("01_01010600_018");
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010600_018") + "');");
                return;
            }
            //*比較一Key和二Key的User是否為同一人
            if (eAgentInfo.agent_id.ToString().Trim() == dtblAchGet1Key.Rows[0][14].ToString().Trim())
            {
                base.strClientMsg = MessageHelper.GetMessage("01_01010600_019");
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010600_019") + "');");
                ReSetControls(false, false);
                return;
            }

            lblUserNameText.Text = htOutputP4_JCF6["NAME_1"].ToString().Trim();

            ViewState["HtgInfoJCF6"] = htOutputP4_JCF6;
            ViewState["HtgInfoJCDK"] = htOutputP4_JCDK;

            this.Session["Acct_No_befor"] = CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 4, 26);

            //*查詢他行自扣資料 (Other_Bank_Temp) (已分配Batch_No,但尚未回貼主機)
            if (QueryWithBatch(sReceiveNumber, ref bIsReadOnly))
            {
                if (!bIsReadOnly)
                {
                    if (QueryWith2KEY(sReceiveNumber))
                    {
                        // lbBuildDateText.Text = System.DateTime.Now.ToString("yyyyMMdd");
                        //*顯示民國年月日
                        lbBuildDateText.Text = System.DateTime.Now.AddYears(-1911).ToString("yyyyMMdd").PadLeft(8, '0');
                        lbApplyCodeText.Text = "N";
                        SetControlsEnabledText(true);
                        base.strClientMsg = MessageHelper.GetMessage("01_01010600_005");
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
        string sAccNoBank = txtAccNoBank.Text.Trim();

        if (sAccNoBank != "")
        {
            DataTable dtblBANKINFO = (DataTable)BROTHER_BANK_TEMP.SearchBankInfo(sAccNoBank);
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
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010600_026") + "');");
                base.strClientMsg += MessageHelper.GetMessage("01_01010600_026");
            }
        }
        else
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010600_026") + "');");
            base.strClientMsg += MessageHelper.GetMessage("01_01010600_026");
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
                base.sbRegScript.Append("if(confirm('" + MessageHelper.GetMessage("01_01010600_022") + "')) {$('#btnHidenD').click();}else{document.getElementById('txtApplyType').focus();}");
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
        string strSysType = "";

        if (this.txtApplyType.Text.Trim().ToUpper() == "A")
        {
            this.txtTranCode.Text = "851";
            base.sbRegScript.Append(BaseHelper.SetFocus("txtTranCode"));

        }
        else if (this.txtApplyType.Text.Trim().ToUpper() == "D")
        {
            DataSet dstInfo = null;
            dstInfo = BRACH598DATA.Select(this.txtUserId.Text.Trim().ToUpper());
            if (dstInfo == null)
            {
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return;
            }

            if (dstInfo.Tables[0].Rows.Count > 0)
            {
                strSysType = "598";
            }


            if ((this.txtTranCode.Text.Trim() != strSysType) && strSysType.Length != 0)
            {

                base.sbRegScript.Append("if(confirm('" + MessageHelper.GetMessage("01_01010600_022") + "')) {document.getElementById('txtTranCode').value='" + strSysType + "' ;  document.getElementById('txtTranCode').focus(); }");

            }
        }
        else
        {
            ;
        }
    }

    /// 作者 chenjingxian
    /// 創建日期：2009/10/21
    /// 修改日期：2009/10/21 
    /// <summary>
    /// 點選檢核銀行代號按鈕，檢核交易代號
    /// </summary>
    protected void btnBankCheck_Click(object sender, EventArgs e)
    {

        lblLongBankNo.Text = "";
        lblLongBankName.Text = "";

        if (!CheckBankNo())
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtAccNoBank"));
            return;
        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtAccNoBank"));
    }

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
    /// <param name="strCaseNo">案件類別代號</param>
    /// <returns>案件類別代號+案件類別名稱</returns>
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
    private void ReSetControls(bool blnClearFields, bool blnEnabled)
    {
        if (blnClearFields)
        {
            SetControlsEnabled(blnEnabled);
        }
        else
        {
            SetControlsEnabledText(blnEnabled);
        }

        ReSetBackColor();
    }

    /// 作者 chenjingxian
    /// 創建日期：2009/10/22
    /// 修改日期：2009/10/22
    /// <summary>
    /// 將網頁中的輸入框字體顏色變為黑色
    /// </summary>
    /// <param name="blnEnabled">True可用，false不可用</param>
    private void ReSetBackColor()
    {
        this.txtAccNoBank.BackColor = Color.White;
        this.txtAccNo.BackColor = Color.White;
        this.txtPayWay.BackColor = Color.White;
        this.txtAccID.BackColor = Color.White;
        this.txtBcycleCodeText.BackColor = Color.LightGray;
        this.txtBcycleCode.BackColor = Color.LightGray;
        this.txtMobilePhone.BackColor = Color.White;
        this.txtEmail.BackColor = Color.LightGray;
        this.txtEBill.BackColor = Color.LightGray;
        this.txtApplyType.BackColor = Color.White;
        this.txtTranCode.BackColor = Color.White;
        this.txtCaseClass.BackColor = Color.White;
        this.txtPopulEmpNO.BackColor = Color.White;
        this.txtPopulNo.BackColor = Color.White;
    }

    //Add by Carolyn
    //查詢銀行代碼
    private bool CheckBankNo()
    {
        if (txtAccNoBank.Text.Trim() == "")
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01010500_017");
            return false;
        }
        else
        {
            //*查詢銀行代號資料
            DataTable dtblBANKINFO = (DataTable)BROTHER_BANK_TEMP.SearchBankInfo(txtAccNoBank.Text.Trim());

            if (dtblBANKINFO == null)
            {
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return false;
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
                return false;
            }
        }
        return true;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/11
    /// 修改日期：2009/11/11
    /// <summary>
    /// 將欄位值為NULL的轉換為空的字符串
    /// </summary>
    /// <param name="strValue">欄位值</param>
    /// <returns>為空的字符串</returns>
    private string NullToString(string strValue)
    {
        if (strValue == null)
        {
            return strValue = "";
        }
        return strValue;
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
    private bool QueryWith2KEY(string sReceiveNumber)
    {
        DataSet dstInfo;//*記錄查詢結果

        //*查詢二Key資料
        DataTable dtblAchGet2Key = (DataTable)BROTHER_BANK_TEMP.SearchACH2Key(this.txtUserId.Text.Trim().ToUpper(), sReceiveNumber);
        DataTable dtblAchGet2Keyadd = (DataTable)BRAuto_Pay_Popul.SearchQryACH2Key(this.txtUserId.Text.Trim().ToUpper(), sReceiveNumber);

        if (dtblAchGet2Key == null)
        {
            //* 顯示端末訊息
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
            return false;
        }

        if (dtblAchGet2Key.Rows.Count > 0)
        {
            //*查詢出有二Key資料，則將資料庫里的二Key資料付給畫面中的控件值
            txtAccNoBank.Text = SelectValue(dtblAchGet2Key.Rows[0][0].ToString().Trim());
            txtAccNoBankH.Text = txtAccNoBank.Text;
            txtAccNo.Text = dtblAchGet2Key.Rows[0][1].ToString().Trim();
            txtPayWay.Text = dtblAchGet2Key.Rows[0][2].ToString().Trim();
            txtAccID.Text = dtblAchGet2Key.Rows[0][3].ToString().Trim();
            txtBcycleCodeText.Text = dtblAchGet2Key.Rows[0][4].ToString().Trim();
            txtBcycleCode.Text = txtBcycleCodeText.Text;
            txtMobilePhone.Text = dtblAchGet2Key.Rows[0][5].ToString().Trim();
            txtEmail.Text = dtblAchGet2Key.Rows[0][6].ToString().Trim();
            txtEBill.Text = dtblAchGet2Key.Rows[0][7].ToString().Trim();
            txtApplyType.Text = dtblAchGet2Key.Rows[0][9].ToString().Trim();
            txtTranCode.Text = dtblAchGet2Key.Rows[0][10].ToString().Trim();
            if (dtblAchGet2Keyadd.Rows.Count > 0)
            {
                txtPopulEmpNO.Text = dtblAchGet2Keyadd.Rows[0][0].ToString().Trim();
                txtPopulNo.Text = dtblAchGet2Keyadd.Rows[0][1].ToString().Trim();
                txtCaseClass.Text = SelectValue2(dtblAchGet2Keyadd.Rows[0][2].ToString().Trim());
            }
            else
            {
                //this.txtCaseClass.Text = SelectValue2("03");
                this.txtCaseClass.Text = SelectValue2(txtReceiveNumber.Text.Substring(8, 2));
            }
        }
        else //*二Key無資料則顯示主機資料
        {

            Hashtable htJCF6 = (Hashtable)ViewState["HtgInfoJCF6"];
            Hashtable htJCDK = (Hashtable)ViewState["HtgInfoJCDK"];

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
        this.txtPopulEmpNO.Enabled = blnEnabled;
        this.txtPopulNo.Enabled = blnEnabled;
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

    private void CustomerLog_OtherBankTemp(Hashtable htJCF6_G, Hashtable htJCDK_G)
    {
        DataTable dtblUpdateData = new DataTable();
        dtblUpdateData.Columns.Add(Entitycustomer_log_OtherBankTemp.M_field_name);
        dtblUpdateData.Columns.Add(Entitycustomer_log_OtherBankTemp.M_before);
        dtblUpdateData.Columns.Add(Entitycustomer_log_OtherBankTemp.M_after);

        //銀行帳號
        if (txtAccNoBank.Text.Trim().Substring(0, 3) + "-" + txtAccNo.Text.Trim() != htJCF6_G["CO_OWNER"].ToString().Trim())
        {
            DataRow drowRow = dtblUpdateData.NewRow();
            drowRow[Entitycustomer_log_OtherBankTemp.M_field_name] = "銀行帳號";
            drowRow[Entitycustomer_log_OtherBankTemp.M_before] = htJCF6_G["CO_OWNER"].ToString();
            drowRow[Entitycustomer_log_OtherBankTemp.M_after] = txtAccNoBank.Text.Trim().Substring(0, 3) + "-" + txtAccNo.Text.Trim();
            dtblUpdateData.Rows.Add(drowRow);
        }

        //扣繳方式
        if (txtPayWay.Text.Trim() != htJCF6_G["CO_TAX_ID_TYPE"].ToString().Trim())
        {
            DataRow drowRow = dtblUpdateData.NewRow();
            drowRow[Entitycustomer_log_OtherBankTemp.M_field_name] = "扣繳方式";
            drowRow[Entitycustomer_log_OtherBankTemp.M_before] = htJCF6_G["CO_TAX_ID_TYPE"].ToString();
            drowRow[Entitycustomer_log_OtherBankTemp.M_after] = txtPayWay.Text.Trim();
            dtblUpdateData.Rows.Add(drowRow);
        }

        //帳戶ID
        if (txtAccID.Text.Trim() != htJCF6_G["DD_ID"].ToString().Trim())
        {
            DataRow drowRow = dtblUpdateData.NewRow();
            drowRow[Entitycustomer_log_OtherBankTemp.M_field_name] = "帳戶ID";
            drowRow[Entitycustomer_log_OtherBankTemp.M_before] = htJCF6_G["DD_ID"].ToString();
            drowRow[Entitycustomer_log_OtherBankTemp.M_after] = txtAccID.Text.Trim();
            dtblUpdateData.Rows.Add(drowRow);
        }

        //賬單周期
        if (txtBcycleCode.Text.Trim() != txtBcycleCodeText.Text.Trim())
        {
            DataRow drowRow = dtblUpdateData.NewRow();
            drowRow[Entitycustomer_log_OtherBankTemp.M_field_name] = "賬單周期";
            drowRow[Entitycustomer_log_OtherBankTemp.M_before] = txtBcycleCodeText.Text.ToString();
            drowRow[Entitycustomer_log_OtherBankTemp.M_after] = txtBcycleCode.Text.Trim();
            dtblUpdateData.Rows.Add(drowRow);
        }

        //行動電話
        if (txtMobilePhone.Text.Trim() != htJCDK_G["MOBILE_PHONE"].ToString().Trim())
        {
            DataRow drowRow = dtblUpdateData.NewRow();
            drowRow[Entitycustomer_log_OtherBankTemp.M_field_name] = "行動電話";
            drowRow[Entitycustomer_log_OtherBankTemp.M_before] = htJCDK_G["MOBILE_PHONE"].ToString().Trim();
            drowRow[Entitycustomer_log_OtherBankTemp.M_after] = txtMobilePhone.Text.Trim();
            dtblUpdateData.Rows.Add(drowRow);
        }

        //E-MAIL
        if (txtEmail.Text.Trim() != htJCDK_G["EMAIL"].ToString().Trim())
        {
            DataRow drowRow = dtblUpdateData.NewRow();
            drowRow[Entitycustomer_log_OtherBankTemp.M_field_name] = "E-MAIL";
            drowRow[Entitycustomer_log_OtherBankTemp.M_before] = htJCDK_G["EMAIL"].ToString().Trim();
            drowRow[Entitycustomer_log_OtherBankTemp.M_after] = txtEmail.Text.Trim();
            dtblUpdateData.Rows.Add(drowRow);
        }

        //電子賬單
        if (txtEBill.Text.Trim() != htJCF6_G["OFF_PHONE_FLAG"].ToString().Trim())
        {
            DataRow drowRow = dtblUpdateData.NewRow();
            drowRow[Entitycustomer_log_OtherBankTemp.M_field_name] = "電子賬單";
            drowRow[Entitycustomer_log_OtherBankTemp.M_before] = htJCF6_G["OFF_PHONE_FLAG"].ToString().Trim();
            drowRow[Entitycustomer_log_OtherBankTemp.M_after] = txtEBill.Text.Trim();
            dtblUpdateData.Rows.Add(drowRow);
        }

        //申請類別
        DataRow drowRow2 = dtblUpdateData.NewRow();
        drowRow2[Entitycustomer_log_OtherBankTemp.M_field_name] = "申請類別";
        drowRow2[Entitycustomer_log_OtherBankTemp.M_before] = "";
        drowRow2[Entitycustomer_log_OtherBankTemp.M_after] = txtApplyType.Text.ToString().Trim();
        dtblUpdateData.Rows.Add(drowRow2);

        //交易代碼
        if (txtTranCode.Text.Trim() != ViewState["gsTranCode"].ToString().Trim())
        {
            DataRow drowRow = dtblUpdateData.NewRow();
            drowRow[Entitycustomer_log_OtherBankTemp.M_field_name] = "交易代碼";
            drowRow[Entitycustomer_log_OtherBankTemp.M_before] = ViewState["gsTranCode"].ToString().Trim();
            drowRow[Entitycustomer_log_OtherBankTemp.M_after] = txtTranCode.Text.Trim();
            dtblUpdateData.Rows.Add(drowRow);
        }

        try
        {
            for (int i = 0; i < dtblUpdateData.Rows.Count; i++)
            {
                Entitycustomer_log_OtherBankTemp ecustomer_log_OtherBankTemp = new Entitycustomer_log_OtherBankTemp();
                ecustomer_log_OtherBankTemp.query_key = this.txtUserId.Text.Trim();
                ecustomer_log_OtherBankTemp.trans_id = ((structPageInfo)this.Session["PageInfo"]).strPageCode.ToString();
                ecustomer_log_OtherBankTemp.field_name = dtblUpdateData.Rows[i][Entitycustomer_log_OtherBankTemp.M_field_name].ToString();
                ecustomer_log_OtherBankTemp.before = dtblUpdateData.Rows[i][Entitycustomer_log_OtherBankTemp.M_before].ToString();
                ecustomer_log_OtherBankTemp.after = dtblUpdateData.Rows[i][Entitycustomer_log_OtherBankTemp.M_after].ToString();
                ecustomer_log_OtherBankTemp.user_id = eAgentInfo.agent_id;
                ecustomer_log_OtherBankTemp.mod_date = DateTime.Now.ToString("yyyyMMdd");
                ecustomer_log_OtherBankTemp.mod_time = DateTime.Now.ToString("HHmmss");
                ecustomer_log_OtherBankTemp.log_flag = "P4";
                ecustomer_log_OtherBankTemp.Receive_Number = this.txtReceiveNumber.Text.Trim();
                BRcustomer_log_OtherBankTemp.AddNewEntity(ecustomer_log_OtherBankTemp);
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
        }
    }
    
    private void CustomerLog_OtherBankTempAdd(EntityAuto_Pay_Popul beforeInfo)
    {
        DataTable dtblUpdateData = new DataTable();
        dtblUpdateData.Columns.Add(Entitycustomer_log_OtherBankTemp.M_field_name);
        dtblUpdateData.Columns.Add(Entitycustomer_log_OtherBankTemp.M_before);
        dtblUpdateData.Columns.Add(Entitycustomer_log_OtherBankTemp.M_after);

        //推廣員編
        if (txtPopulEmpNO.Text.Trim() != beforeInfo.Popul_EmpNo.ToString().Trim())
        {
            DataRow drowRow = dtblUpdateData.NewRow();
            drowRow[Entitycustomer_log_OtherBankTemp.M_field_name] = "推廣員編";
            drowRow[Entitycustomer_log_OtherBankTemp.M_before] = beforeInfo.Popul_EmpNo.ToString();
            drowRow[Entitycustomer_log_OtherBankTemp.M_after] = txtPopulEmpNO.Text.Trim().PadLeft(8, '0');
            dtblUpdateData.Rows.Add(drowRow);
        }

        //推廣代號
        if (txtPopulNo.Text.Trim() != beforeInfo.Popul_No.ToString().Trim())
        {
            DataRow drowRow = dtblUpdateData.NewRow();
            drowRow[Entitycustomer_log_OtherBankTemp.M_field_name] = "推廣代號";
            drowRow[Entitycustomer_log_OtherBankTemp.M_before] = beforeInfo.Popul_No.ToString();
            drowRow[Entitycustomer_log_OtherBankTemp.M_after] = txtPopulNo.Text.Trim();
            dtblUpdateData.Rows.Add(drowRow);
        }

        //案件類別
        if (txtCaseClass.Text.Trim().Substring(0, 2) != beforeInfo.Case_Class.ToString().Trim())
        {
            DataRow drowRow = dtblUpdateData.NewRow();
            drowRow[Entitycustomer_log_OtherBankTemp.M_field_name] = "案件類別";
            drowRow[Entitycustomer_log_OtherBankTemp.M_before] = beforeInfo.Case_Class.ToString();
            drowRow[Entitycustomer_log_OtherBankTemp.M_after] = txtCaseClass.Text.Trim();
            dtblUpdateData.Rows.Add(drowRow);
        }

        try
        {
            for (int i = 0; i < dtblUpdateData.Rows.Count; i++)
            {
                Entitycustomer_log_OtherBankTemp ecustomer_log_OtherBankTemp = new Entitycustomer_log_OtherBankTemp();
                ecustomer_log_OtherBankTemp.query_key = this.txtUserId.Text.Trim();
                ecustomer_log_OtherBankTemp.trans_id = ((structPageInfo)this.Session["PageInfo"]).strPageCode.ToString();
                ecustomer_log_OtherBankTemp.field_name = dtblUpdateData.Rows[i][Entitycustomer_log_OtherBankTemp.M_field_name].ToString();
                ecustomer_log_OtherBankTemp.before = dtblUpdateData.Rows[i][Entitycustomer_log_OtherBankTemp.M_before].ToString();
                ecustomer_log_OtherBankTemp.after = dtblUpdateData.Rows[i][Entitycustomer_log_OtherBankTemp.M_after].ToString();
                ecustomer_log_OtherBankTemp.user_id = eAgentInfo.agent_id;
                ecustomer_log_OtherBankTemp.mod_date = DateTime.Now.ToString("yyyyMMdd");
                ecustomer_log_OtherBankTemp.mod_time = DateTime.Now.ToString("HHmmss");
                ecustomer_log_OtherBankTemp.log_flag = "P4";
                ecustomer_log_OtherBankTemp.Receive_Number = this.txtReceiveNumber.Text.Trim();
                BRcustomer_log_OtherBankTemp.AddNewEntity(ecustomer_log_OtherBankTemp);
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
        }
    }
    
    /// <summary>
    /// Call 卡主機 確認是否有正卡
    /// </summary>
    /// <returns>true or false</returns>
    private bool checkCredit()
    {
        string strMsg = string.Empty;

        Hashtable htInputP4_JCAA = new Hashtable();
        string strAcct_NBR = CommonFunction.GetSubString(this.txtUserId.Text.Trim().ToUpper(), 0, 16);
        htInputP4_JCAA.Add("USER_ID", eAgentInfo.agent_id);
        htInputP4_JCAA.Add("ACCT_NBR", strAcct_NBR);
        htInputP4_JCAA["LINE_CNT"] = "0000";
        htInputP4_JCAA.Add("FUNCTION_CODE", "1");//1-透過身分證查詢 2-透過卡號查詢

        //*得到主機傳回信息
        Hashtable htResult = new Hashtable();

        if (CommonFunction.GetJCAAMainframeData(0, htInputP4_JCAA, ref htResult, eAgentInfo, ref strMsg) && !htResult.Contains("HtgMsg"))
        {
            //Host Message
            base.strHostMsg += htResult["HtgSuccess"] != null ? htResult["HtgSuccess"].ToString() : "P4_JCAA - 查詢成功";//*主機返回成功訊息 
            return true;
        }
        else
        {
            if (!htResult.Contains("HtgMsg"))
                base.strHostMsg += htResult["HtgSuccess"] != null ? htResult["HtgSuccess"].ToString() : "";//*主機返回查詢成功訊息
            else
                base.strHostMsg += htResult["HtgMsg"] != null ? htResult["HtgMsg"].ToString() : "";//*主機返回失敗訊息

            ////Client Message
            base.strClientMsg += strMsg;
            return false;
        }
    }
}
