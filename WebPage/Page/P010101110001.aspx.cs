//******************************************************************
//*  作    者：chenjingxian
//*  功能說明：自扣案件終止作業
//*  創建日期：2009/10/02
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
//20160606 (U) by Tank
using System.Data.SqlClient;
using System.Web.Services;
using Framework.Common.Logging;

public partial class P010101110001 : PageBase
{
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            SetControlsText();
            LoadDropDownList();
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            radAuto.Checked = true;
            RsetControlsState(true, false, true);
        }
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }

    /// 修改日期：2009/09/18 
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

    /// <summary>
    /// 對頁面控件的單選按鈕Text賦值
    /// </summary>
    private void SetControlsText()
    {
        radAuto.Text = BaseHelper.GetShowText("01_01110001_010");
        radEnd.Text = BaseHelper.GetShowText("01_01110001_011");
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
        //*查詢前先清除畫面資料
        RsetControlsState(true, false, false);
        EntityAuto_Pay_Popul eAutoPayPopul = new EntityAuto_Pay_Popul();
        this.txtReceiveNumber.BackColor = Color.White;
        radAuto.Checked = true;
        Hashtable htInputP4_JCF6;
        Hashtable htOutputP4_JCF6;
        string strSql = "";
        string upperUserID = this.txtUserId.Text.Trim().ToUpper();
        string sUserId = CommonFunction.GetSubString(upperUserID, 0, 16);

        //*檢核輸入的收件編號和身分證號碼是否符合規則
        if (!CheckInPutInfo())
        {
            //return;//*失敗
        }

        #region 查詢第一卡人檔

        htInputP4_JCF6 = new Hashtable();
        htInputP4_JCF6.Add("ACCT_NBR", sUserId);
        htInputP4_JCF6.Add("FUNCTION_CODE", "1");
        htInputP4_JCF6.Add("LINE_CNT", "0000");

        htOutputP4_JCF6 = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCF6, htInputP4_JCF6, false, "1", eAgentInfo);

        if (htOutputP4_JCF6.Contains("HtgMsg"))
        {
            etMstType = eMstType.Select;
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));

            if (htOutputP4_JCF6["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg = htOutputP4_JCF6["HtgMsg"].ToString();
                base.strClientMsg = MessageHelper.GetMessage("01_01011000_016");
            }
            else
            {
                base.strClientMsg = htOutputP4_JCF6["HtgMsg"].ToString();
            }
            return;
        }

        #endregion

        if (htOutputP4_JCF6["NAME_1"].ToString().Trim() == "")
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01011000_018");
            return;
        }

        base.strHostMsg = htOutputP4_JCF6["HtgSuccess"].ToString();//*主機返回成功訊息
        ViewState["HtgInfoP4_JCF6"] = htOutputP4_JCF6;
        lblUserNameText.Text = htOutputP4_JCF6["NAME_1"].ToString().Trim();// 姓名
        lblDDIDText.Text = htOutputP4_JCF6["DD_ID"].ToString().Trim();     // 帳戶ID
        btnSubmit.Enabled = true;

        this.Session["Cus_name"] = htOutputP4_JCF6["NAME_1"].ToString().Trim();   // 姓名
        this.Session["pay_way"] = htOutputP4_JCF6["CO_TAX_ID_TYPE"].ToString();   // 扣繳方式
        this.Session["bcycle_code"] = htOutputP4_JCF6["BILLING_CYCLE"].ToString();// 帳單週期

        //*查詢出無資料，則將viewstate中的訊息顯示與網頁的欄位中
        //*銀行代號、扣繳帳號(銀行帳號)
        this.lblAccNoBankText.Text = SelectValue(CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 0, 3));
        this.lbPayWayText.Text = CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 4, 26);

        // 查詢案件類別
        DataTable dtblAchGet1Keyadd = (DataTable)BRAuto_Pay_Popul.SearchQryACH1Key(upperUserID, this.txtReceiveNumber.Text.Trim());
        if (dtblAchGet1Keyadd.Rows.Count > 0)
            txtCaseClass.Text = SelectValue2(dtblAchGet1Keyadd.Rows[0][2].ToString().Trim());
        else
            this.txtCaseClass.Text = SelectValue2(txtReceiveNumber.Text.Substring(8, 2));

        // 查詢自扣狀態
        strSql = "Popul_No,Popul_EmpNo,isEnd";
        DataSet dsPopul = BRAuto_Pay_Popul.SelectDetail(sUserId, strSql);
        string isEndState = "";

        if (dsPopul.Tables[0].Rows.Count > 0)
        {
            // 是否自扣終止
            isEndState = dsPopul.Tables[0].Rows[0][2].ToString().Trim();

            if (isEndState == "Y")
            {
                // 終止
                this.radEnd.Checked = true;
                this.radAuto.Checked = false;
            }
            else
            {
                // 自扣中 
                this.radAuto.Checked = true;
                this.radEnd.Checked = false;
            }
        }
        else
        {
            this.radAuto.Checked = true;
            this.radEnd.Checked = false;
        }

        if (isEndState != "")
            ViewState["radState"] = isEndState;
        else
            ViewState["radState"] = "";

        if (this.txtPopul.Text != "")
            ViewState["Popul"] = this.txtPopul.Text.Trim();
        else
            ViewState["Popul"] = "";

        if (this.txtPopulNumber.Text != "")
            ViewState["PopulNumber"] = this.txtPopulNumber.Text.Trim();
        else
            ViewState["PopulNumber"] = "";

        RsetControlsState(false, true, false);
    }

    /// <summary>
    /// 提交
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (txtCaseClass.Text == string.Empty)
        {
            MessageHelper.GetMessage("01_01010500_023");
            return;
        }

        //*檢核輸入的收件編號和身分證號碼是否符合規則
        if (!CheckInPutInfo())
        {
            return;//*失敗
        }

        DataTable dtblUpdateData = CommonFunction.GetDataTable(); // 異動customer_log記錄的Table
        string upperUserID = this.txtUserId.Text.Trim().ToUpper();// 身份證號碼
        string receiveNumber = this.txtReceiveNumber.Text.Trim(); // 收件編號
        string accNoBank = this.lblAccNoBankText.Text.Trim();     // 銀行帳號
        string sAccNoBank = CommonFunction.GetSubString(accNoBank, 0, 3);
        string accID = this.lblDDIDText.Text.Trim();         // 帳戶ID
        string caseClass = this.txtCaseClass.Text.Trim();    // 案件類別
        string popul = this.txtPopul.Text.Trim();            // 推廣代號
        string populNumber = this.txtPopulNumber.Text.Trim();// 推廣員編

        EntityAuto_Pay_Popul eAutoPayPopul = new EntityAuto_Pay_Popul();
        eAutoPayPopul.Receive_Number = receiveNumber;
        eAutoPayPopul.Cus_Id = this.txtUserId.Text.Trim();
        if (radEnd.Checked)
        {
            // 終止
            eAutoPayPopul.isEnd = "Y";
            eAutoPayPopul.FUNCTION_CODE = "D";
        }
        else
        {
            // 自扣中
            eAutoPayPopul.isEnd = "N";
            eAutoPayPopul.FUNCTION_CODE = "C";
        }

        //edit by mel 20141015 RQ-2014-014138-000KeyinGui CSIP 中信及郵局自扣 start
        if (this.radEnd.Checked && (sAccNoBank != "042" && sAccNoBank != "701") && accID == "")
        {
            // 他行自扣之帳戶ID無值,不可新增此筆自扣終止!!
            base.strClientMsg += MessageHelper.GetMessage("01_01010111001_001");
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010111001_001") + "');");
            return;
        }
        //edit by mel 20141015 RQ-2014-014138-000KeyinGui CSIP 中信及郵局自扣 end \

        eAutoPayPopul.Popul_No = popul;
        //20130926 Casper 推廣員編要左補0
        //20131003 Ralph 空白不補0
        eAutoPayPopul.Popul_EmpNo = populNumber.Equals(string.Empty) ? string.Empty : populNumber.PadLeft(8, '0');

        //*直接INSERT一筆新資料，Keyln_Flag註記為 0
        eAutoPayPopul.KeyIn_Flag = "0";
        eAutoPayPopul.Case_Class = caseClass.Substring(0, 2);
        eAutoPayPopul.User_Id = eAgentInfo.agent_id.ToString().Trim();
        eAutoPayPopul.mod_date = System.DateTime.Now;
        eAutoPayPopul.AccNoBank = sAccNoBank;
        eAutoPayPopul.Acc_No = this.lbPayWayText.Text;
        eAutoPayPopul.Acc_Id = accID;
        eAutoPayPopul.Cus_name = this.Session["Cus_name"].ToString();
        eAutoPayPopul.bcycle_code = this.Session["bcycle_code"].ToString();
        eAutoPayPopul.pay_way = this.Session["pay_way"].ToString();

        // 新增自扣終止新增欄位資料
        if (!BRAuto_Pay_Popul.AddEntity(eAutoPayPopul))
        {
            //*更新操作失敗
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }
        else
        {
            if (this.radAuto.Checked)
            {
                // 自扣中
                Logging.Log("設定", LogLayer.UI);
                this.InsertCustomerLog("推廣員編", string.Empty, popul.Equals(string.Empty) ? string.Empty : popul.PadLeft(8, '0'), eAgentInfo, upperUserID, "P4", sPageInfo, receiveNumber);
                this.InsertCustomerLog("推廣代號", string.Empty, populNumber, eAgentInfo, upperUserID, "P4", sPageInfo, receiveNumber);
                this.InsertCustomerLog("案件類別", string.Empty, caseClass, eAgentInfo, upperUserID, "P4", sPageInfo, receiveNumber);
            }
            else
            {
                // 終止
                Logging.Log("終止", LogLayer.UI);
                this.InsertCustomerLog("扣繳帳號", string.Format("{0}-{1}", accNoBank.Substring(0, 3), this.lbPayWayText.Text.Trim()), string.Empty, eAgentInfo, upperUserID, "P4", sPageInfo, receiveNumber);
                this.InsertCustomerLog("帳戶ID", accID, string.Empty, eAgentInfo, upperUserID, "P4", sPageInfo, receiveNumber);
            }

            string isEndState = (this.radEnd.Checked) ? "Y" : "N";

            if (ViewState["radState"].ToString().Trim() != isEndState)
            {
                // 自扣中
                if (this.radAuto.Checked && this.radEnd.Checked == false)
                {
                    DataRow drowRow = dtblUpdateData.NewRow();
                    drowRow[Entitycustomer_log_OtherBankTemp.M_field_name] = BaseHelper.GetShowText("01_01110001_009");
                    drowRow[Entitycustomer_log_OtherBankTemp.M_before] = ViewState["radState"].ToString();
                    drowRow[Entitycustomer_log_OtherBankTemp.M_after] = "N";
                    dtblUpdateData.Rows.Add(drowRow);
                }

                // 終止
                if (this.radEnd.Checked && this.radAuto.Checked == false)
                {
                    DataRow drowRow = dtblUpdateData.NewRow();
                    drowRow[Entitycustomer_log_OtherBankTemp.M_field_name] = BaseHelper.GetShowText("01_01110001_009");
                    drowRow[Entitycustomer_log_OtherBankTemp.M_before] = ViewState["radState"].ToString();
                    drowRow[Entitycustomer_log_OtherBankTemp.M_after] = "Y";
                    dtblUpdateData.Rows.Add(drowRow);
                }
            }

            if (ViewState["Popul"].ToString().Trim() != popul)
            {
                DataRow drowRow = dtblUpdateData.NewRow();
                drowRow[Entitycustomer_log_OtherBankTemp.M_field_name] = BaseHelper.GetShowText("01_01110001_013");
                drowRow[Entitycustomer_log_OtherBankTemp.M_before] = ViewState["Popul"].ToString();
                drowRow[Entitycustomer_log_OtherBankTemp.M_after] = popul;
                dtblUpdateData.Rows.Add(drowRow);
            }

            if (ViewState["PopulNumber"].ToString().Trim() != populNumber)
            {
                DataRow drowRow = dtblUpdateData.NewRow();
                drowRow[Entitycustomer_log_OtherBankTemp.M_field_name] = BaseHelper.GetShowText("01_01110001_014");
                drowRow[Entitycustomer_log_OtherBankTemp.M_before] = ViewState["PopulNumber"].ToString();
                drowRow[Entitycustomer_log_OtherBankTemp.M_after] = populNumber;
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
                    ecustomer_log_OtherBankTemp.Receive_Number = receiveNumber;
                    BRcustomer_log_OtherBankTemp.AddNewEntity(ecustomer_log_OtherBankTemp);
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.UI);
            }
        }

        // 如果是他行終止自扣，要執行寫入報送資料到OTHER_BANK_TEMP
        if (this.radEnd.Checked && (accNoBank != "042" && accNoBank != "701"))
        {
            OtherBank();
        }

        // 郵局自扣終止
        if (accNoBank == "701")
        {
            bool result = false;

            Hashtable htOutputP4_JCF6 = (Hashtable)ViewState["HtgInfoP4_JCF6"];
            string hostAccNo = htOutputP4_JCF6["CO_OWNER"].ToString().Trim();
            string accNo = "";
            if (hostAccNo != "")
                accNo = hostAccNo.Split('-')[1];// 郵局帳號
            string cusName = lblUserNameText.Text;            // 姓名
            string accType = (accNo.Length == 14) ? "P" : "G";// 帳戶別(P.存簿 G.劃撥)
            string accDeposit = accNo;                        // 儲金帳號(存簿：局帳號計14碼；劃撥：000000+8碼帳號)
            string cusNo = upperUserID;                       // 用戶編號
            string modDate = DateTime.Now.ToString("yyyyMMdd");
            // 新增PostOffice_Temp
            result = CSIPKeyInGUI.BusinessRules_new.BRPostOffice_Temp.InsertIntoPostOffice(upperUserID, receiveNumber, cusName, accNoBank, accNo, accID,
                                                                                            "2", accType, accDeposit, cusNo, eAgentInfo.agent_id, modDate);
            if (!result)
            {
                base.strClientMsg = MessageHelper.GetMessage("01_01010111001_002");
                return;
            }
        }

        RsetControlsState(true, false, true);
        base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
        this.radAuto.Checked = true;

        base.strClientMsg = MessageHelper.GetMessage("01_01011000_007");
    }

    private void RsetControlsState(bool bIsClear, bool blnEnabled, bool inputClear)
    {
        if (bIsClear)
        {
            this.lblAccNoBankText.Text = "";
            this.lbPayWayText.Text = "";
            this.txtPopul.Text = "";
            this.txtPopulNumber.Text = "";
            this.lblUserNameText.Text = "";
            btnSelect.Enabled = true;
            btnSubmit.Enabled = false;
            radAuto.Checked = true;
            radEnd.Checked = false;
            this.lblDDIDText.Text = "";
        }
        this.txtPopul.Enabled = blnEnabled;
        this.txtPopulNumber.Enabled = blnEnabled;

        if (inputClear)
        {
            this.txtReceiveNumber.Text = "";
            this.txtUserId.Text = "";
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
        dstInfo = BRAuto_Pay_Popul.Select(sReceiveNumber, strSql);

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

    //* 選擇"終止"時，清空"推廣員編"及"推廣代號"，並且不能輸入這兩個欄位
    protected void radEnd_CheckedChanged(Object sender, EventArgs e)
    {
        if (radEnd.Checked)
        {
            this.txtPopul.Text = "";
            this.txtPopulNumber.Text = "";
            RsetControlsState(false, false, false);
        }
        else
        {
            RsetControlsState(false, true, false);
        }
    }

    //* 寫入他行報送資料到OTHER_BANK_TEMP
    private void OtherBank()
    {
        Hashtable htJCF6 = (Hashtable)ViewState["HtgInfoP4_JCF6"];
        Hashtable htInputP4_JCDK;//*查詢第二卡人檔上行
        Hashtable htOutputP4_JCDK;//*查詢第二卡人檔下行

        string sBankNo = CommonFunction.GetSubString(htJCF6["CO_OWNER"].ToString(), 0, 3);
        string sLongBankNo = "";
        string sLongBankName = "";

        //*讀取第二卡人檔
        htInputP4_JCDK = new Hashtable();
        htInputP4_JCDK.Add("ACCT_NBR", this.txtUserId.Text.Trim());
        htInputP4_JCDK.Add("FUNCTION_CODE", "1");
        htInputP4_JCDK.Add("LINE_CNT", "0000");

        htOutputP4_JCDK = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCDK, htInputP4_JCDK, false, "1", eAgentInfo);

        if (htOutputP4_JCDK.Contains("HtgMsg"))
        {
            etMstType = eMstType.Select;

            //主機傳回訊息缺少欄位
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));

            if (htOutputP4_JCDK["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg = htOutputP4_JCDK["HtgMsg"].ToString();
                base.strClientMsg = MessageHelper.GetMessage("01_01011000_003");
            }
            else
            {
                base.strClientMsg = htOutputP4_JCDK["HtgMsg"].ToString();
            }
            return;
        }

        base.strHostMsg += htOutputP4_JCDK["HtgSuccess"].ToString();//*主機返回成功訊息

        //*查詢銀行代號資料
        DataTable dtblBANKINFO = (DataTable)BROTHER_BANK_TEMP.SearchBankInfo(sBankNo);

        if (dtblBANKINFO == null)
        {
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (dtblBANKINFO.Rows.Count > 0)
        {
            sLongBankNo = dtblBANKINFO.Rows[0][1].ToString().Trim();
            sLongBankName = dtblBANKINFO.Rows[0][2].ToString().Trim();
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01010500_017");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtAccNoBank"));
            return;
        }

        //判斷是否有 ACH 的資料 (Table:Ach598Data)
        DataSet dstInfo;//*記錄查詢結果
        string sTranCode = "";
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
                sTranCode = "598";
            }
            else
            {
                sTranCode = "851";
            }
        }

        //*寫入一Key資料
        EntityOTHER_BANK_TEMP eBank = new EntityOTHER_BANK_TEMP();
        eBank.Cus_ID = this.txtUserId.Text.Trim();
        eBank.Cus_Name = this.lblUserNameText.Text.Trim();
        eBank.Other_Bank_Code_S = sBankNo;
        eBank.Other_Bank_Code_L = sLongBankNo;
        eBank.Other_Bank_Acc_No = this.lbPayWayText.Text;
        eBank.Other_Bank_Pay_Way = htJCF6["CO_TAX_ID_TYPE"].ToString();//htJCF6["CO_TAX_ID_TYPE"].ToString();
        eBank.bcycle_code = htJCF6["BILLING_CYCLE"].ToString();//htJCF6["BILLING_CYCLE"].ToString()
        eBank.Mobile_Phone = htOutputP4_JCDK["MOBILE_PHONE"].ToString();//htJCDK["MOBILE_PHONE"].ToString()
        eBank.E_Mail = htOutputP4_JCDK["EMAIL"].ToString();//htJCDK["EMAIL"].ToString()
        eBank.E_Bill = htJCF6["OFF_PHONE_FLAG"].ToString();
        eBank.Build_Date = System.DateTime.Now.AddYears(-1911).ToString("yyyyMMdd").PadLeft(8, '0');
        eBank.Receive_Number = this.txtReceiveNumber.Text.Trim();
        eBank.Other_Bank_Cus_ID = htJCF6["DD_ID"].ToString();
        eBank.Apply_Type = "D";
        eBank.Deal_No = sTranCode;
        eBank.KeyIn_Flag = "1";
        //eBank.Upload_flag = "N";
        eBank.Upload_flag = "Y";
        eBank.user_id = eAgentInfo.agent_id.ToString().Trim();
        eBank.mod_date = System.DateTime.Now.ToString("yyyyMMdd");
        eBank.Oper_Flag = "0";

        eBank.Deal_S_No = "";
        eBank.Acc_No_D = "";
        eBank.Acc_No_L = "";
        eBank.Acc_No = "";
        eBank.Pay_Way = "";
        eBank.Acc_ID = "";
        eBank.ACH_Return_Code = "0";
        eBank.Pcmc_Return_Code = "ERROR:9";
        eBank.C1342_Return_Code = "";
        eBank.Batch_no = "";
        eBank.Pcmc_Upload_flag = "1";

        eBank.Acct_NBR = this.txtUserId.Text.Trim();
        eBank.Auto_Pay_Setting = "";
        eBank.CellP_Email_Setting = "";
        eBank.E_Bill_Setting = "";
        eBank.OutputByTXT_Setting = "Y";
        eBank.Ach_hold = "1";

        //20131007 Casper 終止件寫入他行時 Ach_Batch_Date時間要加上屬性代碼ACH_D_TYPE60MD的values
        DataTable ACH_date = new DataTable();
        if (BaseHelper.GetCommonProperty("01", "ACH_D_TYPE60MD", ref ACH_date))
        {
            for (int i = 0; i < ACH_date.Rows.Count; i++)
            {
                eBank.Ach_Batch_date = System.DateTime.Now.AddDays(Convert.ToInt32(ACH_date.Rows[i][1].ToString())).ToString("yyyyMMdd");
            }
        }


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

        //*寫入二Key資料
        eBank.KeyIn_Flag = "2";
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

    private bool InsertCustomerLog(DataTable dtblUpdate, EntityAGENT_INFO eAgentInfo, string strQueryKey, string strLogFlag, structPageInfo sPageInfo, string strReceiveNumber)
    {
        try
        {
            EntityCUSTOMER_LOG eCustomerLog = new EntityCUSTOMER_LOG();
            for (int i = 0; i < dtblUpdate.Rows.Count; i++)
            {
                eCustomerLog.query_key = strQueryKey;
                eCustomerLog.trans_id = sPageInfo.strPageCode.ToString();
                eCustomerLog.field_name = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_field_name].ToString();
                eCustomerLog.before = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_before].ToString();
                eCustomerLog.after = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_after].ToString();
                eCustomerLog.user_id = eAgentInfo.agent_id;
                eCustomerLog.mod_date = DateTime.Now.ToString("yyyyMMdd");
                eCustomerLog.mod_time = DateTime.Now.ToString("HHmmss");
                eCustomerLog.log_flag = strLogFlag;
                eCustomerLog.Receive_Number = strReceiveNumber;
                BRCustomer_Log.AddEntity(eCustomerLog);
            }
        }
        catch
        {
            return false;
        }
        return true;
    }

    private bool InsertCustomerLog(string field_name, string before, string after, EntityAGENT_INFO eAgentInfo, string strQueryKey, string strLogFlag, structPageInfo sPageInfo, string strReceiveNumber)
    {
        try
        {
            EntityCUSTOMER_LOG eCustomerLog = new EntityCUSTOMER_LOG();
            eCustomerLog.query_key = strQueryKey;
            eCustomerLog.trans_id = sPageInfo.strPageCode.ToString();
            eCustomerLog.field_name = field_name;
            eCustomerLog.before = before;
            eCustomerLog.after = after;
            eCustomerLog.user_id = eAgentInfo.agent_id;
            eCustomerLog.mod_date = DateTime.Now.ToString("yyyyMMdd");
            eCustomerLog.mod_time = DateTime.Now.ToString("HHmmss");
            eCustomerLog.log_flag = strLogFlag;
            eCustomerLog.Receive_Number = strReceiveNumber;
            BRCustomer_Log.AddEntity(eCustomerLog);
        }
        catch
        {
            return false;
        }
        return true;
    }

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
    /// 查詢自扣案件處理狀態
    /// </summary>
    /// <param name="query_key">身分證字號</param>
    /// <param name="strMsgID">返回的錯誤ID號</param>
    /// <returns>成功時：返回查詢結果；失敗時：null</returns>
    public static DataTable GetCustomerLog_Cnt(string strQueryKey)
    {
        try
        {
            #region 依據Request查詢資料庫

            String strDateTimeToday = DateTime.Now.ToString("yyyy-MM-dd") + " 19:00:00"; //今晚七點
            String strDateTimeYesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 19:00:00"; //昨晚七點

            //* 聲明SQL Command變量
            SqlCommand sqlcmSearchData = new SqlCommand();
            sqlcmSearchData.CommandType = CommandType.Text;

            string strQueryCmd = "Select Count(*) From (" +
                                    " Select a.Cus_ID" +
                                    " From auto_pay_status a with (nolock)" +
                                    " Inner join auto_pay b with (nolock) on a.receive_number=b.receive_number and a.Cus_ID=b.Cus_ID " +
                                    " Where a.IsCTCB in ('Y','S')" +
                                    " and b.KeyIn_Flag='2'" +
                                    " and a.isUpdateByTXT='Y'" +
                                    " and ((getDate()<'" + strDateTimeToday + "' and a.DateTime>'" + strDateTimeYesterday + "') or (getDate()>'" + strDateTimeToday + "' and a.DateTime>'" + strDateTimeToday + "')) " +
                                    " and b.Cus_ID like @Query_Key" +
                                    " and b.mod_date = (	Select max(b.mod_date)" +
                                    " 					    From auto_pay_status a with (nolock)" +
                                    " 					    Inner join auto_pay b with (nolock) on a.receive_number=b.receive_number and a.Cus_ID=b.Cus_ID " +
                                    " 					    Where a.IsCTCB in ('Y','S')" +
                                    " 					    and b.KeyIn_Flag='2'" +
                                    " 					    and a.isUpdateByTXT='Y'" +
                                    " 					    and b.Cus_ID like @Query_Key)" +
                                    " Union All" +
                                    " Select a.Other_Bank_Cus_ID" +
                                    " From Other_Bank_Temp a with (nolock)" +
                                    " Left join Ach_Rtn_Info b with (nolock) on a.ACH_Return_Code = b.ACH_Rtn_Code  " +
                                    " Where a.KeyIn_Flag='2' " +
                                    " and a.apply_type <>'D' " +
                                    " and a.ACH_Return_Code not in ('0','1')" +
                                    " and a.Other_Bank_Cus_ID like @Query_Key" +
                                    " and b.ACH_Rtn_Code=''" +
                                    " and a.mod_date = (	Select max(a.mod_date)" +
                                    " 					    From Other_Bank_Temp a with (nolock)" +
                                    " 					    Left join Ach_Rtn_Info b with (nolock) on a.ACH_Return_Code = b.ACH_Rtn_Code  " +
                                    " 					    Where a.KeyIn_Flag='2' " +
                                    " 					    and a.apply_type <>'D' " +
                                    " 					    and a.ACH_Return_Code not in ('0','1')" +
                                    " 					    and a.Other_Bank_Cus_ID like @Query_Key" +
                                    " 					    and b.ACH_Rtn_Code='')" +
                                " ) a";


            sqlcmSearchData.CommandText = strQueryCmd;

            //* 身分證號碼
            SqlParameter parmQueryKey = new SqlParameter("@Query_Key", "%" + strQueryKey + "%");
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
