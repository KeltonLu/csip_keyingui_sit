//******************************************************************
//*  作    者：chenjingxian
//*  功能說明：中信及郵局 自扣二KEY
//*  創建日期：2009/09/23
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
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.Data;
using Framework.WebControls;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Transactions;
using CSIPCommonModel.EntityLayer;

public partial class P010101080001 : PageBase
{
    #region 變數區
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息
    private EntityAuto_Pay_Popul beforeInfo;//*記錄Auto_Pay_Popul變更之前的記錄
    #endregion

    #region 事件區
    /// 作者 chenjingxian
    /// 創建日期：2009/09/23
    /// 修改日期：2009/09/23
    /// <summary>
    /// 加載網頁
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            SetControlsText();
            LoadDropDownList();
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            ReSetControls(true, false);
        }

        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }

    /// 作者 chenjingxian
    /// 創建日期：2009/09/23
    /// 修改日期：2009/09/23 
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
        //*進入查詢首先若新增的三個欄位有值，則存進session,若沒有值，存入空字符串，以便于比對
        DataTable dtblAchGet2Keyaddbefore = (DataTable)BRAuto_Pay_Popul.SearchQryACH2Key(this.txtUserId.Text.Trim().ToUpper(), this.txtReceiveNumber.Text.Trim());
        EntityAuto_Pay_Popul Before = new EntityAuto_Pay_Popul();

        if (dtblAchGet2Keyaddbefore != null && dtblAchGet2Keyaddbefore.Rows.Count > 0)
        {
            Before.Popul_No = dtblAchGet2Keyaddbefore.Rows[0][1].ToString().Trim();                // 推廣代號
            Before.Popul_EmpNo = dtblAchGet2Keyaddbefore.Rows[0][0].ToString().Trim();             // 推廣員編
            Before.Case_Class = SelectValue2(dtblAchGet2Keyaddbefore.Rows[0][2].ToString().Trim());// 案件類別
            Session["beforeInfo"] = Before;
        }
        else
        {
            Before.Popul_No = "";
            Before.Popul_EmpNo = "";
            Before.Case_Class = "";
            Session["beforeInfo"] = Before;
        }

        Hashtable htInputP4_JCF6; // 查詢第一卡人檔上行
        Hashtable htOutputP4_JCF6;// 查詢第一卡人檔下行
        Hashtable htInputP4_JCDK; // 查詢第二卡人檔上行
        Hashtable htOutputP4_JCDK;// 查詢第二卡人檔下行

        string strSql = "";
        string upperUserID = this.txtUserId.Text.Trim().ToUpper();
        string sUserId = CommonFunction.GetSubString(upperUserID, 0, 16);
        string sReceiveNumber = this.txtReceiveNumber.Text.Trim();

        ReSetControls(true, false);

        // 檢核輸入的收件編號和身分證號碼是否符合規則
        if (!CheckInPutInfo())
        {
            return;// 失敗
        }

        #region 獲取主機資料

        // 讀取第一卡人檔
        htInputP4_JCF6 = new Hashtable();
        htInputP4_JCF6.Add("ACCT_NBR", sUserId);
        htInputP4_JCF6.Add("FUNCTION_CODE", "1");
        htInputP4_JCF6.Add("LINE_CNT", "0000");

        htOutputP4_JCF6 = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCF6, htInputP4_JCF6, false, "1", eAgentInfo);

        if (htOutputP4_JCF6.Contains("HtgMsg"))
        {
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            etMstType = eMstType.Select;
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

            // 主機傳回訊息缺少欄位
            ReSetControls(false, false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));

            // 若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            if (htOutputP4_JCF6["HtgMsgFlag"].ToString() == "0")
            {
                base.strHostMsg = htOutputP4_JCF6["HtgMsg"].ToString();
                base.strClientMsg = MessageHelper.GetMessage("01_01010800_016");
            }
            else
            {
                base.strClientMsg = htOutputP4_JCF6["HtgMsg"].ToString();
            }

            return;
        }

        if (htOutputP4_JCF6["NAME_1"].ToString().Trim() == "")
        {
            base.strClientMsg = MessageHelper.GetMessage("01_01010800_017");
            return;
        }

        base.strHostMsg = htOutputP4_JCF6["HtgSuccess"].ToString();// 主機返回成功訊息

        ViewState["HtgInfoP4_JCF6"] = htOutputP4_JCF6;

        // 讀取第二卡人檔
        htInputP4_JCDK = new Hashtable();
        htInputP4_JCDK.Add("ACCT_NBR", sUserId);
        htInputP4_JCDK.Add("FUNCTION_CODE", "1");
        htInputP4_JCDK.Add("LINE_CNT", "0000");

        htOutputP4_JCDK = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCDK, htInputP4_JCDK, false, "1", eAgentInfo);

        if (htOutputP4_JCDK.Contains("HtgMsg"))
        {
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            etMstType = eMstType.Select;
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

            // 主機傳回訊息缺少欄位
            ReSetControls(false, false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));

            // 若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            if (htOutputP4_JCDK["HtgMsgFlag"].ToString() == "0")
            {
                base.strHostMsg = htOutputP4_JCDK["HtgMsg"].ToString();
                base.strClientMsg = MessageHelper.GetMessage("01_01010800_003");
            }
            else
            {
                base.strClientMsg = htOutputP4_JCDK["HtgMsg"].ToString();
            }

            return;
        }

        base.strHostMsg += htOutputP4_JCDK["HtgSuccess"].ToString();// 主機返回成功訊息
        htOutputP4_JCDK["ACCT_NBR"] = htInputP4_JCDK["ACCT_NBR"];// for_xml_test 
        htOutputP4_JCDK["MESSAGE_TYPE"] = "";
        htOutputP4_JCDK["FUNCTION_CODE"] = "2";
        htOutputP4_JCDK["LINE_CNT"] = "0000";
        ViewState["HtgInfoP4_JCDK"] = htOutputP4_JCDK;

        // 確認是否有正卡
        if (!checkCredit())
        {
            string[] strArray = new string[] { "01_01010700_011" };
            MessageHelper.ShowMessageWithParms(this, strArray[0], strArray);

            return;
        }

        #endregion 獲取主機資料結束

        #region 查詢一Key、二Key資料
        // 查詢一Key資料
        strSql = "Acc_No, Pay_Way, Acc_ID, bcycle_code, Mobile_Phone, E_Mail, E_Bill, Receive_Number, user_id";
        //DataSet dstInfoKey1 = BRAUTO_PAY.SelectDetail(upperUserID, "0", "Y", "1", strSql);
        DataSet dstInfoKey1 = CSIPKeyInGUI.BusinessRules_new.BRAuto_pay.SelectDetail(upperUserID, sReceiveNumber, "0", "Y", "1", strSql);

        if (dstInfoKey1 == null)
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            ReSetControls(false, false);
            base.strClientMsg = BaseHelper.GetShowText("00_00000000_000");
            return;
        }

        if (dstInfoKey1.Tables[0].Rows.Count < 1)
        {
            // 若一Key無資料,提示一Key後才能二Key
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUserId"));
            ReSetControls(false, false);
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010800_008") + "');");
            return;
        }

        if (dstInfoKey1.Tables[0].Rows[0][8].ToString().Trim() == eAgentInfo.agent_id)
        {
            // 一KEY與二KEY經辦為同一人
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUserId"));
            ReSetControls(false, false);
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010800_009") + "');");
            return;
        }

        //// 將一Key的收件編號賦值給網頁中的收件編號欄位
        //this.txtReceiveNumber.Text = dstInfoKey1.Tables[0].Rows[0][7].ToString();        

        // 查詢二Key資料
        strSql = "Acc_No, Pay_Way, Acc_ID, bcycle_code, Mobile_Phone, E_Mail, E_Bill, Receive_Number, user_id";
        //DataSet dstInfoKey2 = BRAUTO_PAY.SelectDetail(upperUserID, "0", "Y", "2", strSql);
        DataSet dstInfoKey2 = CSIPKeyInGUI.BusinessRules_new.BRAuto_pay.SelectDetail(upperUserID, sReceiveNumber, "0", "Y", "2", strSql);
        DataTable dtblAchGet2Keyadd = (DataTable)BRAuto_Pay_Popul.SearchQryACH2Key(upperUserID, sReceiveNumber);

        if (dstInfoKey2 == null)
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            ReSetControls(false, false);
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }
        #endregion 查詢一Key、二Key資料結束

        ReSetControls(false, true);
        lblUserNameText.Text = htOutputP4_JCF6["NAME_1"].ToString().Trim();

        #region 將資料庫或主機的資料顯示與網頁中
        // Logging.SaveLog(ELogLayer.UI, "Rows.Count: " + dstInfoKey2.Tables[0].Rows.Count.ToString(), ELogType.Debug);
        Logging.Log("Rows.Count: " + dstInfoKey2.Tables[0].Rows.Count.ToString(), LogLayer.UI);



        // 先將主機姓名顯示出來
        if (dstInfoKey2.Tables[0].Rows.Count > 0)
        {
            DataRow drInfo = dstInfoKey2.Tables[0].Rows[0];
            string sdbfAccNo = drInfo[0].ToString().Trim();

            // 查詢出有資料，將查詢出的資料顯示網頁的欄位中。
            //Logging.SaveLog(ELogLayer.UI, "sdbfAccNo.Length: " + sdbfAccNo.Length.ToString(), ELogType.Debug);
            Logging.Log("sdbfAccNo.Length: " + sdbfAccNo.Length.ToString(), LogLayer.UI);


            if (sdbfAccNo.Length > 3)
            {
                this.txtAccNoBank.Text = sdbfAccNo.Substring(0, 3);// 扣繳帳號(銀行代號)
                txtAccNo.Text = CommonFunction.GetSubString(sdbfAccNo, 4, sdbfAccNo.Length - 4);// 銀行帳號
            }

            this.txtPayWay.Text = drInfo[1].ToString();     // 繳款狀況
            this.txtAccID.Text = drInfo[2].ToString();      // 帳戶ID
            this.txtBcycleCode.Text = drInfo[3].ToString(); // 帳單週期
            this.txtBcycleCodeText.Text = this.txtBcycleCode.Text;
            this.txtMobilePhone.Text = drInfo[4].ToString();// 行動電話
            this.txtEmail.Text = drInfo[5].ToString();      // Email
            this.txtEBill.Text = drInfo[6].ToString();      // EBill

            if (dtblAchGet2Keyadd.Rows.Count > 0)
            {
                txtCaseClass.Text = SelectValue2(dtblAchGet2Keyadd.Rows[0][2].ToString().Trim());   // 推廣代號
                txtPopulEmpNO.Text = dtblAchGet2Keyadd.Rows[0][0].ToString().Trim().PadLeft(8, '0');// 推廣員編
                txtPopulNo.Text = dtblAchGet2Keyadd.Rows[0][1].ToString().Trim();                   // 案件類別
            }
            else
            {
                this.txtCaseClass.Text = SelectValue2(sReceiveNumber.Substring(8, 2));
            }
        }
        else
        {
            // 查詢出無資料，則將viewstate中的訊息顯示與網頁的欄位中
            if (CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 0, 3) == "701" || CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 0, 3) == "042")
            {
                this.txtAccNoBank.Text = CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 0, 3);// 扣繳帳號(銀行代號)
            }
            else
            {
                this.txtAccNoBank.Text = "";
            }

            this.txtAccNo.Text = CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 4, 26);// 銀行帳號
            this.txtPayWay.Text = htOutputP4_JCF6["CO_TAX_ID_TYPE"].ToString();   // 繳款狀況
            this.txtAccID.Text = htOutputP4_JCF6["DD_ID"].ToString();             // 帳戶ID
            this.txtBcycleCode.Text = htOutputP4_JCF6["BILLING_CYCLE"].ToString();// 帳單週期
            this.txtBcycleCodeText.Text = this.txtBcycleCode.Text;
            this.txtMobilePhone.Text = htOutputP4_JCDK["MOBILE_PHONE"].ToString();// 行動電話
            this.txtEmail.Text = htOutputP4_JCDK["EMAIL"].ToString();             // Email
            this.txtEBill.Text = htOutputP4_JCF6["OFF_PHONE_FLAG"].ToString();    // EBill
            this.txtCaseClass.Text = SelectValue2(sReceiveNumber.Substring(8, 2)); // 案件類別
        }

        this.Session["Acct_No_befor"] = CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 4, 26);
        this.Session["AccNoBankt"] = CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 0, 3);
        // Logging.SaveLog(ELogLayer.UI, "AccNoBankt:" + this.Session["AccNoBankt"] + "; Acct_No_befor:" + this.Session["Acct_No_befor"].ToString(), ELogType.Debug);
        Logging.Log("AccNoBankt:" + this.Session["AccNoBankt"] + "; Acct_No_befor:" + this.Session["Acct_No_befor"].ToString(), LogLayer.UI);


        #endregion 將資料庫或主機的資料顯示與網頁中結束

        base.strClientMsg = MessageHelper.GetMessage("01_01010800_005");
        base.sbRegScript.Append(BaseHelper.SetFocus("txtAccNoBank"));
    }

    /// 作者 chenjingxian
    /// 創建日期：2009/09/18
    /// 修改日期：2009/09/18 
    /// <summary>
    /// 提交事件
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (txtCaseClass.Text == string.Empty)
        {
            MessageHelper.GetMessage("01_01010500_023");
            return;
        }

        string upperUserID = this.txtUserId.Text.Trim().ToUpper();
        string receiveNumber = this.txtReceiveNumber.Text.Trim();

        try
        {
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            etMstType = eMstType.Control;
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end
            ReSetControls(false, true);

            Hashtable htOutputP4_JCF6; // 查詢第一卡人檔下行
            Hashtable htInputPCTIP4S;  // 更新第一卡人檔上行
            Hashtable htInputP4_JCDKS = new Hashtable();// 更新第二卡人檔上行
            Hashtable htOutputPCTIP4S; // 更新第一卡人檔下行
            Hashtable htOutputP4_JCDKS;// 更新第二卡人檔下行

            DataTable dtblUpdateData = CommonFunction.GetDataTable();// 異動customer_log記錄的Table

            EntitySet<EntityAUTO_PAY> eAutoPaySetKey1;// Auto_Pay一Key資料
            EntitySet<EntityAUTO_PAY> eAutoPaySetKey2;// Auto_Pay二Key資料

            int intSubmit = 0;// 記錄異動主機成功的次數

            string accNoBank = this.txtAccNoBank.Text.Trim();// 扣繳帳號(銀行代號)

            // 檢核輸入的收件編號和身分證號碼是否符合規則
            if (!CheckInPutInfo())
            {
                return;// 失敗
            }

            //2011-12-25 ADD 檢核條件
            string sAccNoBankt = CommonFunction.GetSubString(accNoBank, 0, 3);
            if (sAccNoBankt == "042")
            {
                if (!checkInfo())
                {
                    return;// 失敗
                }
            }

            // 讀取第一卡人檔
            htOutputP4_JCF6 = (Hashtable)ViewState["HtgInfoP4_JCF6"];
            // 讀取第二卡人檔
            CommonFunction.GetViewStateHt(ViewState["HtgInfoP4_JCDK"], ref htInputP4_JCDKS);

            // 電子帳單(MAIL)值=0或2才可以修改，其他不能異動！
            if (htOutputP4_JCF6["OFF_PHONE_FLAG"].ToString() != this.txtEBill.Text.Trim())
            {
                if (!(htOutputP4_JCF6["OFF_PHONE_FLAG"].ToString() == "0" || htOutputP4_JCF6["OFF_PHONE_FLAG"].ToString() == "2"))
                {
                    base.strClientMsg = MessageHelper.GetMessage("01_01010800_006");
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtEBill"));
                    return;
                }
            }

            ArrayList arrayName = new ArrayList(new object[] { "ACCT_NBR", "CO_OWNER", "CO_TAX_ID_TYPE", "DD_ID", "BILLING_CYCLE", "OFF_PHONE_FLAG" });

            htInputPCTIP4S = new Hashtable();
            MainFrameInfo.ChangeJCF6toPCTI(htOutputP4_JCF6, htInputPCTIP4S, arrayName);// 將異動主機的訊息預設為從主機讀取的訊息

            #region 查詢、更新資料庫
            // 查詢一Key資料
            eAutoPaySetKey1 = GetAutoPaySet(upperUserID, receiveNumber, "0", "Y", "1");
            // 查詢二Key資料
            eAutoPaySetKey2 = GetAutoPaySet(upperUserID, receiveNumber, "0", "Y", "2");

            if (eAutoPaySetKey1 == null || eAutoPaySetKey2 == null)
            {
                base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
                return;
            }

            string accNo = this.txtAccNo.Text.Trim();                      // 銀行帳號
            string payWay = this.txtPayWay.Text.Trim();                    // 扣繳方式
            string accID = this.txtAccID.Text.Trim().ToUpper();            // 帳戶ID
            string bcycleCode = this.txtBcycleCode.Text.Trim();            // 帳單週期
            string mobilePhone = this.txtMobilePhone.Text.Trim();          // 行動電話
            string eMail = this.txtEmail.Text.Trim();                      // E-MAIL
            string eBill = this.txtEBill.Text.Trim();                      // 電子帳單
            string modDate = DateTime.Now.ToString("yyyyMMdd");            // 鍵檔日期
            string sCaseClass = txtCaseClass.Text.Trim().Substring(0, 2);  // 案件類別
            string sPopulEmpNo = txtPopulEmpNO.Text.Trim().PadLeft(8, '0');// 推廣員編
            string sPopulNo = txtPopulNo.Text.Trim();                      // 推廣代號
            string accNoBank_No = accNoBank + "-" + accNo;                 // 扣繳帳號(銀行代號)-銀行帳號
            string cusName = this.lblUserNameText.Text.Trim();

            EntityAUTO_PAY eAutoPay = new EntityAUTO_PAY();
            eAutoPay.Receive_Number = receiveNumber;
            // 扣繳帳號(銀行代號)+銀行帳號
            eAutoPay.Acc_No = (accNoBank != "") ? sAccNoBankt + "-" + accNo : "";
            eAutoPay.Pay_Way = payWay;
            eAutoPay.Acc_ID = accID;
            eAutoPay.bcycle_code = bcycleCode;
            eAutoPay.Mobile_Phone = mobilePhone;
            eAutoPay.E_Mail = eMail;
            eAutoPay.E_Bill = eBill;
            eAutoPay.user_id = eAgentInfo.agent_id.ToString().Trim();
            eAutoPay.mod_date = modDate;

            if (!Compare(eAutoPaySetKey1))
            {
                return;
            }

            // 新增或異動資料庫
            bool updateResult = UpdateAutoPay(eAutoPay, eAutoPaySetKey2, upperUserID, receiveNumber, cusName, accNoBank, accNo, accID, modDate, sCaseClass, sPopulEmpNo, sPopulNo);

            if (!updateResult) return;// 新增或異動資料失敗

            base.strClientMsg = MessageHelper.GetMessage("01_01010800_007");// 新增或異動資料成功

            #endregion 查詢、更新資料庫結束

            #region 比較一Key和二Key的資料是否相同
            if (!Compare(eAutoPaySetKey1))
            {
                return;
            }

            DataTable dtblAchGet1Keyadd = (DataTable)BRAuto_Pay_Popul.SearchQryACH1Key(upperUserID, receiveNumber);
            if (!CompareAdd(dtblAchGet1Keyadd))
            {
                base.strClientMsg = MessageHelper.GetMessage("01_01010800_010");
                return;
            }
            #endregion

            // 比對完以后錄入Auto_Pay_Popul資料，并記錄LOG
            BRAuto_Pay_Popul.Update2KeyInfo(receiveNumber, upperUserID, sCaseClass, sPopulNo, sPopulEmpNo, "2", modDate, eAgentInfo.agent_id.ToString().Trim());
            beforeInfo = (EntityAuto_Pay_Popul)this.Session["beforeInfo"];
            this.CustomerLog_Add(beforeInfo);

            string strAcctNBR = htOutputP4_JCF6["ACCT_NBR"].ToString().Trim();
            string strAcctNO = htOutputP4_JCF6["CO_OWNER"].ToString().Trim();
            string sAccNoBankt1 = this.Session["AccNoBankt"] == null ? " " : this.Session["AccNoBankt"].ToString();
            string strAcctNOStatus = (sAccNoBankt1 == "042") ? this.checkAcc_No_Status() : " ";
            if (strAcctNOStatus == null) return;

            #region 異動主機資料

            EntityAuto_Pay_Status eAuto_Pay_Status = new EntityAuto_Pay_Status();

            #region 記錄銀行帳戶是否被異動以及會由哪種方式異動
            if ((accNoBank == "042" && strAcctNO != "" && strAcctNOStatus == "正常") || (accNoBank == "701" && strAcctNO != ""))
            {
                if (accNoBank_No == htOutputP4_JCF6["CO_OWNER"].ToString().Trim())
                {
                    eAuto_Pay_Status.IsUpdateByTXT = "N";// 上送主機
                }
                else
                {
                    eAuto_Pay_Status.IsUpdateByTXT = "Y";// 主機Temp檔
                    intSubmit++;
                    this.InsertCustomerLog("扣繳帳號", htOutputP4_JCF6["CO_OWNER"].ToString().Trim(), accNoBank_No, eAgentInfo, upperUserID, "TEMP", sPageInfo, receiveNumber);
                }
            }
            else
            {
                eAuto_Pay_Status.IsUpdateByTXT = "N";

                // 異動扣繳帳號和銀行帳號
                CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, accNoBank_No, "BK_ID_AC", BaseHelper.GetShowText("01_01010800_006"));
            }
            #endregion

            #region 異動第一卡人檔

            // 比較主機和畫面中的資料
            CompareHostEdit(intSubmit, htInputPCTIP4S, dtblUpdateData, payWay, accID, bcycleCode, eBill);

            if (dtblUpdateData.Rows.Count > 0)
            {
                // 郵局(701)自扣不上送主機
                if (accNoBank == "042")
                {
                    // 提交修改主機資料
                    htInputPCTIP4S.Add("FUNCTION_ID", "PCMC1");
                    htOutputPCTIP4S = MainFrameInfo.GetMainFrameInfo(HtgType.P4_PCTI, htInputPCTIP4S, false, "2", eAgentInfo);

                    if (!htOutputPCTIP4S.Contains("HtgMsg"))
                    {
                        // 記錄異動欄位
                        RecordChangeColumns(dtblUpdateData, eAgentInfo, upperUserID, "P4", sPageInfo, receiveNumber);

                        intSubmit++;
                        base.strHostMsg += htOutputPCTIP4S["HtgSuccess"].ToString();// 主機返回成功訊息
                    }
                    else
                    {
                        // 異動主機資料失敗
                        if (htOutputPCTIP4S["HtgMsgFlag"].ToString() == "0")// 若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                        {
                            base.strHostMsg += htOutputPCTIP4S["HtgMsg"].ToString();
                            base.strClientMsg = MessageHelper.GetMessage("01_01010800_012");
                        }
                        else
                        {
                            base.strClientMsg += htOutputPCTIP4S["HtgMsg"].ToString();
                        }

                        base.sbRegScript.Append(BaseHelper.SetFocus("txtAccNoBank"));
                        return;
                    }
                }
                else
                {
                    // 記錄異動欄位
                    RecordChangeColumns(dtblUpdateData, eAgentInfo, upperUserID, "P4", sPageInfo, receiveNumber);
                }
            }
            else
            {
                // 無需異動資料
                base.strClientMsg += MessageHelper.GetMessage("01_01010800_011");
            }

            #endregion

            #region 異動第二卡人檔

            dtblUpdateData.Clear();

            // 比較主機和畫面中的資料
            CompareHost(htInputP4_JCDKS, dtblUpdateData, mobilePhone, eMail);

            // 上傳身分證號碼
            htInputP4_JCDKS["ACCT_NBR"] = CommonFunction.GetSubString(upperUserID, 0, 16);

            if (dtblUpdateData.Rows.Count > 0)
            {
                // 郵局(701)自扣不上送主機
                if (accNoBank == "042")
                {
                    // 提交修改主機資料
                    htOutputP4_JCDKS = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCDK, htInputP4_JCDKS, false, "2", eAgentInfo);

                    if (!htOutputP4_JCDKS.Contains("HtgMsg"))
                    {
                        // 記錄異動欄位
                        RecordChangeColumns(dtblUpdateData, eAgentInfo, upperUserID, "P4", sPageInfo, receiveNumber);

                        intSubmit++;
                        base.strHostMsg += htOutputP4_JCDKS["HtgSuccess"].ToString();// 主機返回成功訊息
                    }
                    else
                    {
                        // 異動主機資料失敗
                        if (htOutputP4_JCDKS["HtgMsgFlag"].ToString() == "0")// 若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                        {
                            base.strHostMsg += htOutputP4_JCDKS["HtgMsg"].ToString();
                            base.strClientMsg = MessageHelper.GetMessage("01_01010800_014");
                        }
                        else
                        {
                            base.strClientMsg += htOutputP4_JCDKS["HtgMsg"].ToString();
                        }
                        intSubmit++;
                        base.sbRegScript.Append(BaseHelper.SetFocus("txtAccNoBank"));
                        //20130925 Casper 佑華與主機同意 當第二卡人檔異動失敗 也一樣要丟批次檔出來
                        //return;
                    }
                }
                else
                {
                    // 記錄異動欄位
                    RecordChangeColumns(dtblUpdateData, eAgentInfo, upperUserID, "P4", sPageInfo, receiveNumber);
                }
            }
            else
            {
                // 無需異動資料
                base.strClientMsg += MessageHelper.GetMessage("01_01010800_013");
            }

            #endregion

            #endregion 異動主機資料結束

            if (accNoBank == "042")
            {
                // 異動主機資料後，更新資料庫異動狀態
                if (intSubmit > 0)
                {
                    eAuto_Pay_Status.Receive_Number = receiveNumber;
                    eAuto_Pay_Status.Cus_ID = upperUserID;
                    eAuto_Pay_Status.Cus_Name = cusName;
                    eAuto_Pay_Status.AccNoBank = accNoBank;
                    eAuto_Pay_Status.Acc_No = accNoBank_No;
                    eAuto_Pay_Status.Pay_Way = payWay;
                    eAuto_Pay_Status.IsCTCB = "Y";
                    eAuto_Pay_Status.DateTime = DateTime.Now;
                    eAuto_Pay_Status.Acc_No_O = strAcctNO;

                    // 更新資料庫異動狀態
                    UpdateAutoPayStatus(htOutputP4_JCF6, htInputP4_JCDKS, eAuto_Pay_Status, eAutoPay, receiveNumber, upperUserID,
                                            accNo, eMail, eBill, mobilePhone, modDate, strAcctNBR);
                }
                else
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_015");
                }
            }

            ReSetControls(true, false);
            this.txtReceiveNumber.Text = "";
            this.txtUserId.Text = "";
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
        }
        catch (Exception ex)
        {
            // Logging.SaveLog(ELogLayer.UI, ex.ToString(), ELogType.Error);
            Logging.Log(ex.ToString(), LogLayer.UI);

            return;
        }
    }

    #endregion

    #region 方法區

    /// 作者 chenjingxian
    /// 創建日期：2009/09/23
    /// 修改日期：2009/09/23
    /// <summary>
    /// 設置控件文本
    /// </summary>
    private void SetControlsText()
    {
        btnSelect.Text = BaseHelper.GetShowText("01_01010800_005");
        btnSubmit.Text = BaseHelper.GetShowText("01_01010800_014");
    }

    /// 作者 chenjingxian
    /// 創建日期：2009/09/18
    /// 修改日期：2009/09/18 
    /// <summary>
    /// 加載下拉列表
    /// </summary>
    private void LoadDropDownList()
    {
        DataTable dtblResult = new DataTable();//*記錄查詢Table結果
        if (BaseHelper.GetCommonProperty("01", "14", ref dtblResult))
        {
            for (int i = 0; i < dtblResult.Rows.Count; i++)
            {
                ListItem listItem = new ListItem();
                listItem.Text = dtblResult.Rows[i][1].ToString();
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
    /// 創建日期：2009/09/23
    /// 修改日期：2009/09/23
    /// <summary>
    /// 設置控件是否可用
    /// </summary>
    /// <param name="blnEnabled">True可用，false不可用</param>
    private void ReSetControls(bool bIsClear, bool blnEnabled)
    {
        if (bIsClear)
        {
            this.lblUserNameText.Text = "";
            this.txtAccNo.Text = "";
            this.txtAccNoBank.Text = "";
            this.txtPayWay.Text = "";
            this.txtAccID.Text = "";
            this.txtBcycleCode.Text = "";
            this.txtBcycleCodeText.Text = "";
            this.txtMobilePhone.Text = "";
            this.txtEmail.Text = "";
            this.txtEBill.Text = "";
            this.txtPopulEmpNO.Text = "";
            this.txtPopulNo.Text = "";
            this.txtCaseClass.Text = "";
        }

        this.txtAccNo.Enabled = blnEnabled;
        this.dropAccNo.Enabled = blnEnabled;
        this.txtAccNoBank.Enabled = blnEnabled;
        this.txtPayWay.Enabled = blnEnabled;
        this.txtAccID.Enabled = blnEnabled;
        this.txtBcycleCode.Enabled = blnEnabled;

        this.dropBcycleCode.Enabled = blnEnabled;
        this.txtMobilePhone.Enabled = blnEnabled;
        this.txtEmail.Enabled = blnEnabled;
        this.txtEBill.Enabled = blnEnabled;
        this.btnSubmit.Enabled = blnEnabled;

        this.dropCaseClass.Enabled = blnEnabled;
        //this.txtCaseClass.Enabled = blnEnabled;
        //this.txtPopulEmpNO.Enabled = blnEnabled;
        this.txtPopulNo.Enabled = blnEnabled;

        this.txtReceiveNumber.BackColor = Color.White;
        this.txtUserId.BackColor = Color.White;
        this.txtAccNoBank.BackColor = Color.White;
        this.txtAccNo.BackColor = Color.White;
        this.txtPayWay.BackColor = Color.White;
        this.txtAccID.BackColor = Color.White;
        this.txtBcycleCodeText.BackColor = Color.White;
        this.txtBcycleCode.BackColor = Color.White;
        this.txtMobilePhone.BackColor = Color.White;
        this.txtEmail.BackColor = Color.LightGray;
        this.txtEBill.BackColor = Color.LightGray;

        this.txtCaseClass.BackColor = Color.White;
        this.txtPopulEmpNO.BackColor = Color.White;
        this.txtPopulNo.BackColor = Color.White;
    }

    /// 作者 Carolyn
    /// 創建日期：2009/11/03
    /// 修改日期：2009/11/03
    /// <summary>
    /// 比較一次鍵檔和二次鍵檔的資料
    /// </summary>
    /// <returns></returns>
    private bool Compare(EntitySet<EntityAUTO_PAY> eAutoPaySetKey1)
    {
        int intErrorCount = 0;

        //*檢查銀行3碼代號
        if (this.txtAccNoBank.Text.Trim() != "")
        {
            if (CommonFunction.GetSubString(this.txtAccNoBank.Text.Trim(), 0, 3) != CommonFunction.GetSubString(eAutoPaySetKey1.GetEntity(0).Acc_No.Trim(), 0, 3))
            {
                //*銀行3碼一Key與二Key資料不同
                intErrorCount++;
                this.txtAccNoBank.BackColor = Color.Red;
                if (intErrorCount == 1)
                {
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtAccNoBank"));
                }
            }
        }

        //*扣繳帳號
        if (this.txtAccNo.Text.Trim() != CommonFunction.GetSubString(eAutoPaySetKey1.GetEntity(0).Acc_No.Trim(), 4, eAutoPaySetKey1.GetEntity(0).Acc_No.Trim().Length - 4))
        {
            //*扣繳帳號一Key與二Key資料不同
            intErrorCount++;
            this.txtAccNo.BackColor = Color.Red;
            if (intErrorCount == 1)
            {
                base.sbRegScript.Append(BaseHelper.SetFocus("txtAccNo"));
            }
        }

        //*繳款狀況
        if (this.txtPayWay.Text.Trim() != eAutoPaySetKey1.GetEntity(0).Pay_Way.Trim())
        {
            //*繳款狀況一Key與二Key資料不同
            intErrorCount++;
            this.txtPayWay.BackColor = Color.Red;
            if (intErrorCount == 1)
            {
                base.sbRegScript.Append(BaseHelper.SetFocus("txtPayWay"));
            }
        }

        //*帳戶ID
        if (this.txtAccID.Text.Trim().ToUpper() != eAutoPaySetKey1.GetEntity(0).Acc_ID.Trim())
        {
            //*帳戶ID一Key與二Key資料不同
            intErrorCount++;
            this.txtAccID.BackColor = Color.Red;
            if (intErrorCount == 1)
            {
                base.sbRegScript.Append(BaseHelper.SetFocus("txtAccID"));
            }
        }

        //*帳單週期
        if (this.txtBcycleCode.Text.Trim() != eAutoPaySetKey1.GetEntity(0).bcycle_code.Trim())
        {
            //*帳單週期一Key與二Key資料不同
            intErrorCount++;
            this.txtBcycleCode.BackColor = Color.Red;
            if (intErrorCount == 1)
            {
                base.sbRegScript.Append(BaseHelper.SetFocus("txtBcycleCode"));
            }
        }

        //*行動電話
        if (this.txtMobilePhone.Text.Trim() != eAutoPaySetKey1.GetEntity(0).Mobile_Phone.Trim())
        {
            //*行動電話一Key與二Key資料不同
            intErrorCount++;
            this.txtMobilePhone.BackColor = Color.Red;
            if (intErrorCount == 1)
            {
                base.sbRegScript.Append(BaseHelper.SetFocus("txtMobilePhone"));
            }
        }

        //*e-mail
        if (this.txtEmail.Text.Trim() != eAutoPaySetKey1.GetEntity(0).E_Mail.Trim())
        {
            //*e-mail一Key與二Key資料不同
            intErrorCount++;
            this.txtEmail.BackColor = Color.Red;
            if (intErrorCount == 1)
            {
                base.sbRegScript.Append(BaseHelper.SetFocus("txtEmail"));
            }
        }

        //*電子帳單
        if (this.txtEBill.Text.Trim() != eAutoPaySetKey1.GetEntity(0).E_Bill.Trim())
        {
            //*e-mail一Key與二Key資料不同
            intErrorCount++;
            this.txtEBill.BackColor = Color.Red;
            if (intErrorCount == 1)
            {
                base.sbRegScript.Append(BaseHelper.SetFocus("txtEBill"));
            }
        }

        if (intErrorCount > 0)
        {
            //*若一Key與二Key資料不同，提示錯誤訊息
            base.strClientMsg = MessageHelper.GetMessage("01_01010800_010");
            return false;
        }
        return true;
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

        //GetCompareResult(this.txtPopulEmpNO, dtblAchGet1Keyadd.Rows[0][0].ToString(), ref intCount, ref blnSame);
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

    /// 作者 chenjingxian
    /// 創建日期：2009/09/18
    /// 修改日期：2009/09/18 
    /// <summary>
    /// 檢核收件編號和身分證號輸入是否符合規則
    /// </summary>
    /// <returns>檢核成功或失敗</returns>
    private bool CheckInPutInfo()
    {
        string strSql = ""; // 查詢資料庫的顯示內容
        DataSet dstInfo;// 記錄查詢結果
        string receiveNumber = this.txtReceiveNumber.Text.Trim();

        // 若收件編號已被使用，不能再被使用
        strSql = "Receive_Number, Cus_ID";
        dstInfo = BRAUTO_PAY.Select(receiveNumber, "0", "Y", strSql);
        // 查詢已傳送郵局資料
        DataTable postOfficeTemp = CSIPKeyInGUI.BusinessRules_new.BRPostOffice_Temp.GetPostOffice(receiveNumber);

        if (dstInfo == null || postOfficeTemp == null)
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            ReSetControls(false, false);
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return false;
        }

        if (dstInfo.Tables[0].Rows.Count > 0 || postOfficeTemp.Rows.Count > 0)
        {
            // 此收件編號已被使用，不能再被使用
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            ReSetControls(false, false);
            base.strClientMsg += MessageHelper.GetMessage("01_01010800_001");
            return false;
        }

        // 收件編號重複
        strSql = "Receive_Number, Cus_ID";
        dstInfo = BRAUTO_PAY.SelectFlag(receiveNumber, "0", "Y", strSql);

        if (dstInfo == null)
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            ReSetControls(false, false);
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return false;
        }

        if (dstInfo.Tables[0].Rows.Count > 0)
        {
            string dtbUserId = dstInfo.Tables[0].Rows[0][1].ToString().Trim();

            // 查詢出了資料，比較查詢出的{ Cus_ID }和輸入的【身分證號碼】是否一樣。
            if (dtbUserId != this.txtUserId.Text.Trim().ToUpper())
            {
                base.strClientMsg = MessageHelper.GetMessage("01_01010800_002") + ": 此收件編號已被 " + dtbUserId + " 使用";
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010900_002") + ": 此收件編號已被 " + dtbUserId + " 使用');");

                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                this.txtUserId.BackColor = Color.Red;
                //ReSetControls(true,false);
                return false;
            }

            //2011-12-11 
            //1.比對身分證與帳號且扣款帳號為042
            //if(this.txtAccNoBank.Text.Trim() =="042")
            //{
            //    //  Call to Bancs to Confirm this ID

            //    base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("待填") + ": 此訊息暫未使用');");
            //    return false;
            //}
            //AutoExchange AE = new AutoExchange();

            //2.Call to Credit to Confirm this ID has a Card.
        }

        return true;
    }

    //檢測
    private bool checkInfo()
    {
        //檢測身分證號碼=帳戶ID 且 扣繳帳號 為 042
        if (txtAccID.Text.Trim().ToUpper() == txtUserId.Text.Trim().ToUpper())
        {
            //Call Bancs 確認銀行帳號是否有該身分證號碼
            if (!checkBancs())
            {
                base.strClientMsg = MessageHelper.GetMessage("01_01010700_010");
                //2011-12-28 ADD JS Message
                string[] strArray = new string[] { "01_01010700_010" };
                MessageHelper.ShowMessageWithParms(this, strArray[0], strArray);

                ReSetControls(false, false);
                return false;
            }
        }

        return true;
    }

    //Call Bancs 確認銀行帳號是否有該身分證號碼
    private bool checkBancs()
    {
        string strMsg = string.Empty;
        Hashtable htInputBancs = new Hashtable();
        string[] stringArray = new string[] { "CUST_ID_NO", "ACCT_NO" };
        //銀行帳號-17
        htInputBancs.Add("ACCT_NO", this.txtAccNo.Text.Trim().PadLeft(17, '0'));
        //string acct_no_temp = "";
        //acct_no_temp = this.Session["Acct_No_befor"] == null ? this.txtAccNo.Text.Trim().PadLeft(17, '0') : this.Session["Acct_No_befor"].ToString().Trim().PadLeft(17, '0');
        //htInputBancs.Add("ACCT_NO", acct_no_temp);
        //htInputBancs.Add("branchNo", "0888");

        //*得到主機傳回信息
        //Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P8_000401, htInputBancs, false, "1", eAgentInfo);
        //2011-12-22 讀銀行電文
        Hashtable htReturn = MainFrameInfo.GetMainframeDataLU0(htInputBancs, HtgType.P8_000401, ref strMsg, false, stringArray);

        if (!htReturn.Contains("HtgMsg"))
        {
            //CommonFunction.SetControlsEnabled(pnlText, true);
            string strID = string.Empty;
            if (htReturn["CUST_ID_NO"] != null)
                strID = htReturn["CUST_ID_NO"].ToString();
            else
                strID = "";
            //檢測返回的身分證號碼是否符合
            if (!strID.Trim().ToUpper().Equals(txtUserId.Text.Trim().ToUpper()))
            {
                //Client Message
                base.strClientMsg += MessageHelper.GetMessage("01_01010700_010");
                return false;
            }
            //Host Message
            //base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息
        }
        else
        {
            //Host Message
            base.strHostMsg += htReturn["HtgMsg"].ToString();
            return false;
        }

        base.strHostMsg += "00401 - 查詢成功";
        //base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息
        return true;
    }

    //Call 卡主機 確認是否有正卡
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

    //確認銀行帳號是否为非結清戶(正常)
    private string checkAcc_No_Status()
    {
        string strMsg = string.Empty;
        Hashtable htInputBancs = new Hashtable();
        string[] stringArray = new string[] { "CUST_ID_NO", "ACCT_NO" };
        //htInputBancs.Add("ACCT_NO", this.txtAccNo.Text.Trim().PadLeft(17, '0'));
        string acct_no_temp = "";
        acct_no_temp = this.Session["Acct_No_befor"] == null ? this.txtAccNo.Text.Trim().PadLeft(17, '0') : this.Session["Acct_No_befor"].ToString().Trim().PadLeft(17, '0');
        htInputBancs.Add("ACCT_NO", acct_no_temp);
        htInputBancs.Add("branchNo", "0888");

        Hashtable htReturn = MainFrameInfo.GetMainframeDataLU0(htInputBancs, HtgType.P8_000401, ref strMsg, false, stringArray);
        if (!htReturn.Contains("HtgMsg"))
        {
            string strStatus = string.Empty;
            if (htReturn["ACCT_STATUS_1"] != null)
                strStatus = htReturn["ACCT_STATUS_1"].ToString();
            else
                strStatus = "";

            return strStatus.Trim();
        }
        else
        {
            //Host Message
            base.strHostMsg += htReturn["HtgMsg"].ToString();
            return null;
        }
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

    private void CustomerLog_Add(EntityAuto_Pay_Popul beforeInfo)
    {
        DataTable dtblUpdateData = new DataTable();
        dtblUpdateData.Columns.Add(EntityCUSTOMER_LOG.M_field_name);
        dtblUpdateData.Columns.Add(EntityCUSTOMER_LOG.M_before);
        dtblUpdateData.Columns.Add(EntityCUSTOMER_LOG.M_after);

        //推廣員編
        if (txtPopulEmpNO.Text.Trim().PadLeft(8, '0') != beforeInfo.Popul_EmpNo.ToString().Trim().PadLeft(8, '0'))
        {
            DataRow drowRow = dtblUpdateData.NewRow();
            drowRow[EntityCUSTOMER_LOG.M_field_name] = "推廣員編";
            drowRow[EntityCUSTOMER_LOG.M_before] = beforeInfo.Popul_EmpNo.ToString();
            drowRow[EntityCUSTOMER_LOG.M_after] = txtPopulEmpNO.Text.Trim().PadLeft(8, '0');
            dtblUpdateData.Rows.Add(drowRow);
        }

        //推廣代號
        if (txtPopulNo.Text.Trim() != beforeInfo.Popul_No.ToString().Trim())
        {
            DataRow drowRow = dtblUpdateData.NewRow();
            drowRow[EntityCUSTOMER_LOG.M_field_name] = "推廣代號";
            drowRow[EntityCUSTOMER_LOG.M_before] = beforeInfo.Popul_No.ToString();
            drowRow[EntityCUSTOMER_LOG.M_after] = txtPopulNo.Text.Trim();
            dtblUpdateData.Rows.Add(drowRow);
        }

        //案件類別
        if (txtCaseClass.Text.Trim().Substring(0, 2) != beforeInfo.Case_Class.ToString().Trim())
        {
            DataRow drowRow = dtblUpdateData.NewRow();
            drowRow[EntityCUSTOMER_LOG.M_field_name] = "案件類別";
            drowRow[EntityCUSTOMER_LOG.M_before] = beforeInfo.Case_Class.ToString();
            drowRow[EntityCUSTOMER_LOG.M_after] = txtCaseClass.Text.Trim();
            dtblUpdateData.Rows.Add(drowRow);
        }

        try
        {
            for (int i = 0; i < dtblUpdateData.Rows.Count; i++)
            {
                EntityCUSTOMER_LOG ecustomer_log = new EntityCUSTOMER_LOG();
                ecustomer_log.query_key = this.txtUserId.Text.Trim();
                ecustomer_log.trans_id = ((structPageInfo)this.Session["PageInfo"]).strPageCode.ToString();
                ecustomer_log.field_name = dtblUpdateData.Rows[i][EntityCUSTOMER_LOG.M_field_name].ToString();
                ecustomer_log.before = dtblUpdateData.Rows[i][EntityCUSTOMER_LOG.M_before].ToString();
                ecustomer_log.after = dtblUpdateData.Rows[i][EntityCUSTOMER_LOG.M_after].ToString();
                ecustomer_log.user_id = eAgentInfo.agent_id;
                ecustomer_log.mod_date = DateTime.Now.ToString("yyyyMMdd");
                ecustomer_log.mod_time = DateTime.Now.ToString("HHmmss");
                ecustomer_log.log_flag = "P4";
                ecustomer_log.Receive_Number = this.txtReceiveNumber.Text.Trim();

                BRCustomer_Log.AddNewEntity(ecustomer_log);
            }
        }
        catch (Exception exp)
        {
            // Framework.Common.Logging.Logging.SaveLog(Framework.Common.Logging.ELogLayer.UI, exp);
            Logging.Log(exp, LogLayer.UI);
        }
    }

    /// <summary>
    /// 查詢一、二Key資料
    /// </summary>
    /// <param name="upperUserID"></param>
    /// <param name="addFlag"></param>
    /// <param name="uploadFlag"></param>
    /// <param name="keyInFlag"></param>
    /// <returns></returns>
    private EntitySet<EntityAUTO_PAY> GetAutoPaySet(string upperUserID, string receiveNumber, string addFlag, string uploadFlag, string keyInFlag)
    {
        try
        {
            // SelectEntitySet1Key與SelectEntitySet2Key相同
            //return BRAUTO_PAY.SelectEntitySet1Key(upperUserID, addFlag, uploadFlag, keyInFlag);
            return CSIPKeyInGUI.BusinessRules_new.BRAuto_pay.SelectEntitySet(upperUserID, receiveNumber, addFlag, uploadFlag, keyInFlag);
        }
        catch (Exception ex)
        {
            //Logging.SaveLog(ELogLayer.UI, ex.ToString(), ELogType.Error);
            Logging.Log(ex, LogLayer.UI);
            return null;
        }
    }

    /// <summary>
    /// 新增或異動資料庫
    /// </summary>
    /// <param name="eAutoPay"></param>
    /// <param name="eAutoPaySetKey2"></param>
    /// <param name="cusID"></param>
    /// <param name="receiveNumber"></param>
    /// <param name="cusName"></param>
    /// <param name="accNoBank"></param>
    /// <param name="accNo"></param>
    /// <param name="accID"></param>
    /// <param name="modDate"></param>
    /// <param name="sCaseClass"></param>
    /// <param name="sPopulEmpNo"></param>
    /// <param name="sPopulNo"></param>
    /// <returns></returns>
    private bool UpdateAutoPay(EntityAUTO_PAY eAutoPay, EntitySet<EntityAUTO_PAY> eAutoPaySetKey2, string cusID, string receiveNumber, string cusName,
                                string accNoBank, string accNo, string accID, string modDate, string sCaseClass, string sPopulEmpNo, string sPopulNo)
    {
        try
        {
            string[] autoPayColumns = { EntityAUTO_PAY.M_Receive_Number, EntityAUTO_PAY.M_Acc_No, EntityAUTO_PAY.M_Pay_Way, EntityAUTO_PAY.M_Acc_ID,
                                        EntityAUTO_PAY.M_bcycle_code, EntityAUTO_PAY.M_Mobile_Phone, EntityAUTO_PAY.M_E_Mail, EntityAUTO_PAY.M_E_Bill,
                                        EntityAUTO_PAY.M_user_id, EntityAUTO_PAY.M_mod_date };

            // 郵局(701)自扣寫入PostOffice_Temp
            string accType = (accNo.Length == 14) ? "P" : "G";// 帳戶別(P.存簿 G.劃撥)
            string accDeposit = accNo;                        // 儲金帳號(郵局帳號)
            string cusNo = cusID;                             // 用戶編號(身分證號碼或統一證號)
            string agentID = eAgentInfo.agent_id.ToString().Trim();
            bool result = false;

            // 查詢新增欄位二Key資料
            DataTable dtblAchGet2Keyadd = (DataTable)BRAuto_Pay_Popul.SearchQryACH2Key(cusID, receiveNumber);
            // 查詢郵局自扣資料
            DataTable postOfficeTemp = CSIPKeyInGUI.BusinessRules_new.BRPostOffice_Temp.GetPostOffice(cusID, receiveNumber);

            if (eAutoPaySetKey2.Count > 0)
            {
                // 查詢二Key資料成功,則更新資料
                //bool bUpdate = BRAUTO_PAY.Update(eAutoPay, autoPayColumns, cusID, "0", "Y", "2")
                bool bUpdate = CSIPKeyInGUI.BusinessRules_new.BRAuto_pay.Update(eAutoPay, autoPayColumns, cusID, receiveNumber, "0", "Y", "2");
                if (!bUpdate)
                {
                    base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
                    return false;
                }

                // 查詢出Auto_Pay_Popul有二Key資料，則更新資料,執行Auto_Pay_Popul的資料修改
                if (dtblAchGet2Keyadd != null && dtblAchGet2Keyadd.Rows.Count > 0)
                {
                    if (BRAuto_Pay_Popul.UpdateKeyInfo(receiveNumber, cusID, sCaseClass, sPopulNo, sPopulEmpNo, "2", modDate, agentID, "C", "N") == false)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                        return false;
                    }
                }
                else
                {
                    // Auto_Pay_Popul沒有二Key資料，寫入二Key資料
                    EntityAuto_Pay_Popul eBankadd = new EntityAuto_Pay_Popul();
                    eBankadd.Receive_Number = receiveNumber;
                    eBankadd.Cus_Id = cusID;
                    eBankadd.KeyIn_Flag = "2";
                    eBankadd.Popul_EmpNo = sPopulEmpNo;
                    eBankadd.Popul_No = sPopulNo;
                    eBankadd.Case_Class = sCaseClass;
                    eBankadd.User_Id = agentID;
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

                    // 新增Auto_Pay_Popul新增欄位資訊
                    if (!AddAutoPayPopul(eBankadd))
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                        return false;
                    }
                }

                // 新增或異動郵局自扣資料
                if (accNoBank == "701")
                {
                    if (postOfficeTemp != null && postOfficeTemp.Rows.Count > 0)
                    {
                        // 異動
                        result = CSIPKeyInGUI.BusinessRules_new.BRPostOffice_Temp.UpdatePostOffice(cusID, receiveNumber, cusName, accNoBank, accNo, accID, "1", accType, accDeposit, cusNo, agentID, modDate);
                    }
                    else
                    {
                        // 新增
                        result = CSIPKeyInGUI.BusinessRules_new.BRPostOffice_Temp.InsertIntoPostOffice(cusID, receiveNumber, cusName, accNoBank, accNo, accID, "1", accType, accDeposit, cusNo, agentID, modDate);
                    }

                    if (!result)
                    {
                        base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
                        return false;
                    }
                }
            }
            else
            {
                // 沒有二Key資料，寫入二Key資料
                eAutoPay.Cus_ID = cusID;
                eAutoPay.KeyIn_Flag = "2";
                eAutoPay.Upload_Flag = "N";
                eAutoPay.Add_Flag = "0";

                // 新增Auto_Pay中信及郵局自扣資訊
                if (!AddAutoPay(eAutoPay))
                {
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    return false;
                }

                // 查詢出Auto_Pay_Popul有二Key資料，則更新資料,執行Auto_Pay_Popul的資料修改
                if (dtblAchGet2Keyadd != null && dtblAchGet2Keyadd.Rows.Count > 0)
                {
                    if (BRAuto_Pay_Popul.UpdateKeyInfo(receiveNumber, cusID, sCaseClass, sPopulNo, sPopulEmpNo, "2", modDate, agentID, "C", "A") == false)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                        return false;
                    }
                }
                else
                {
                    // Auto_Pay_Popul沒有二Key資料，寫入二Key資料
                    EntityAuto_Pay_Popul eBankadd = new EntityAuto_Pay_Popul();
                    eBankadd.Receive_Number = receiveNumber;
                    eBankadd.Cus_Id = cusID;
                    eBankadd.KeyIn_Flag = "2";
                    eBankadd.Popul_EmpNo = sPopulEmpNo;
                    eBankadd.Popul_No = sPopulNo;
                    eBankadd.Case_Class = sCaseClass;
                    eBankadd.User_Id = agentID;
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

                    if (!AddAutoPayPopul(eBankadd))
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                        return false;
                    }
                }

                // 新增或異動郵局自扣資料
                if (accNoBank == "701")
                {
                    if (postOfficeTemp != null && postOfficeTemp.Rows.Count > 0)
                    {
                        // 異動
                        result = CSIPKeyInGUI.BusinessRules_new.BRPostOffice_Temp.UpdatePostOffice(cusID, receiveNumber, cusName, accNoBank, accNo, accID, "1", accType, accDeposit, cusNo, agentID, modDate);
                    }
                    else
                    {
                        // 新增
                        result = CSIPKeyInGUI.BusinessRules_new.BRPostOffice_Temp.InsertIntoPostOffice(cusID, receiveNumber, cusName, accNoBank, accNo, accID, "1", accType, accDeposit, cusNo, agentID, modDate);
                    }

                    if (!result)
                    {
                        base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
                        return false;
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            // Logging.SaveLog(ELogLayer.UI, ex.ToString(), ELogType.Error);
            Logging.Log(ex, LogLayer.UI);
            return false;
        }
    }

    /// <summary>
    /// 新增Auto_Pay中信及郵局自扣資訊
    /// </summary>
    /// <param name="eAutoPay"></param>
    /// <returns></returns>
    private bool AddAutoPay(EntityAUTO_PAY eAutoPay)
    {
        try
        {
            return BRAUTO_PAY.AddEntity(eAutoPay);
        }
        catch (Exception ex)
        {
            // Logging.SaveLog(ELogLayer.UI, ex.ToString(), ELogType.Error);
            Logging.Log(ex, LogLayer.UI);
            return false;
        }
    }

    /// <summary>
    /// 新增Auto_Pay_Popul新增欄位資訊
    /// </summary>
    /// <param name="eBankadd"></param>
    /// <returns></returns>
    private bool AddAutoPayPopul(EntityAuto_Pay_Popul eBankadd)
    {
        try
        {
            return BRAuto_Pay_Popul.AddEntity(eBankadd);
        }
        catch (Exception ex)
        {
            // Logging.SaveLog(ELogLayer.UI, ex.ToString(), ELogType.Error);
            Logging.Log(ex, LogLayer.UI);
            return false;
        }
    }

    /// <summary>
    /// 比較主機和畫面中的資料，有異動則修改，無自動則刪除
    /// </summary>
    /// <param name="intSubmit"></param>
    /// <param name="htInputPCTIP4S"></param>
    /// <param name="dtblUpdateData"></param>
    /// <param name="payWay"></param>
    /// <param name="accID"></param>
    /// <param name="bcycleCode"></param>
    /// <param name="eBill"></param>
    private void CompareHostEdit(int intSubmit, Hashtable htInputPCTIP4S, DataTable dtblUpdateData, string payWay, string accID, string bcycleCode, string eBill)
    {
        if (intSubmit == 0)
        {
            // 繳款狀況(扣繳方式)
            CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, payWay, "LAST_CR_LINE_IND", BaseHelper.GetShowText("01_01010800_008"));
            // 帳戶ID
            CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, accID, "DD_ID", BaseHelper.GetShowText("01_01010800_009"));
        }

        // 帳單週期
        CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, bcycleCode, "BILLING_CYCLE", BaseHelper.GetShowText("01_01010800_010"));
        // 電子帳單
        CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, eBill, "MAIL", BaseHelper.GetShowText("01_01010800_013"));
    }

    /// <summary>
    /// 比較主機和畫面中的資料
    /// </summary>
    /// <param name="htInputP4_JCDKS"></param>
    /// <param name="dtblUpdateData"></param>
    /// <param name="mobilePhone"></param>
    /// <param name="eMail"></param>
    private void CompareHost(Hashtable htInputP4_JCDKS, DataTable dtblUpdateData, string mobilePhone, string eMail)
    {
        // 行動電話
        CommonFunction.ContrastData(htInputP4_JCDKS, dtblUpdateData, mobilePhone, "MOBILE_PHONE", BaseHelper.GetShowText("01_01010800_011"));
        // Email
        CommonFunction.ContrastData(htInputP4_JCDKS, dtblUpdateData, eMail, "EMAIL", BaseHelper.GetShowText("01_01010800_012"));
    }

    /// <summary>
    /// 更新資料庫異動狀態
    /// </summary>
    /// <param name="htOutputP4_JCF6"></param>
    /// <param name="htInputP4_JCDKS"></param>
    /// <param name="eAuto_Pay_Status"></param>
    /// <param name="eAutoPay"></param>
    /// <param name="receiveNumber"></param>
    /// <param name="cusID"></param>
    /// <param name="accNo"></param>
    /// <param name="eMail"></param>
    /// <param name="eBill"></param>
    /// <param name="mobilePhone"></param>
    /// <param name="modDate"></param>
    /// <param name="acctNBR"></param>
    private void UpdateAutoPayStatus(Hashtable htOutputP4_JCF6, Hashtable htInputP4_JCDKS, EntityAuto_Pay_Status eAuto_Pay_Status, EntityAUTO_PAY eAutoPay,
                                        string receiveNumber, string cusID, string accNo, string eMail, string eBill, string mobilePhone, string modDate, string acctNBR)
    {
        // 記錄電子賬單是否被異動
        bool bIsEBill_Updated = (htOutputP4_JCF6["OFF_PHONE_FLAG"].ToString().Trim() != eBill);
        // 記錄第二卡人資料是否被異動
        bool bIsP4_JCDK_Updated = ((htInputP4_JCDKS["MOBILE_PHONE"].ToString().Trim() != mobilePhone) || ((htInputP4_JCDKS["EMAIL"].ToString().Trim() != eMail)));

        if (BRAuto_pay_status.AddNewEntity(eAuto_Pay_Status))
        {
            if (eAuto_Pay_Status.IsUpdateByTXT == "Y")
            {
                base.strHostMsg += MessageHelper.GetMessage("01_01010800_018");
            }
        }

        base.strClientMsg += MessageHelper.GetMessage("01_01010800_015");
        if (!BRTRANS_NUM.UpdateTransNum("A14"))
        {
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
        }

        // 更新資料庫異動狀態
        eAutoPay = new EntityAUTO_PAY();
        eAutoPay.Upload_Flag = "Y";
        eAutoPay.mod_date = modDate;

        // 自扣設定(銀行帳號原本為空者，新增帳號後該欄位填入Y)
        if (htOutputP4_JCF6["CO_OWNER"].ToString().Trim() == "" && accNo != "")
            eAutoPay.Auto_Pay_Setting = "Y";
        else
            eAutoPay.Auto_Pay_Setting = "N";

        // 手機及E-MAIL設定(第二卡人檔有被異動時(行動電話、E-MAIL，新增或異動後該欄位填入Y)
        if (bIsP4_JCDK_Updated)
            eAutoPay.CellP_Email_Setting = "Y";
        else
            eAutoPay.CellP_Email_Setting = "N";

        // 電子帳單設定(電子帳單被異動時該欄位填入Y)
        if (bIsEBill_Updated)
            eAutoPay.E_Bill_Setting = "Y";
        else
            eAutoPay.E_Bill_Setting = "N";

        // 記錄持卡人ID
        eAutoPay.Acct_NBR = acctNBR;

        // 週期件註記(二KEY完自扣資料被送至主機TEMP檔者)
        eAutoPay.OutputByTXT_Setting = eAuto_Pay_Status.IsUpdateByTXT;

        string[] updateColumns = { EntityAUTO_PAY.M_Upload_Flag, EntityAUTO_PAY.M_mod_date, EntityAUTO_PAY.M_Auto_Pay_Setting,
                                   EntityAUTO_PAY.M_CellP_Email_Setting, EntityAUTO_PAY.M_E_Bill_Setting, EntityAUTO_PAY.M_OutputByTXT_Setting,
                                   EntityAUTO_PAY.M_Acct_NBR };

        if (!BRAUTO_PAY.UpdateSucc(eAutoPay, updateColumns, cusID, "0", "Y", receiveNumber))
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
            {
                base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
            }
        }
    }

    /// <summary>
    /// 記錄異動欄位
    /// </summary>
    /// <param name="dtblUpdateData"></param>
    /// <param name="eAgentInfo"></param>
    /// <param name="upperUserID"></param>
    /// <param name="htgType"></param>
    /// <param name="sPageInfo"></param>
    /// <param name="receiveNumber"></param>
    private void RecordChangeColumns(DataTable dtblUpdateData, EntityAGENT_INFO eAgentInfo, string upperUserID, string htgType, structPageInfo sPageInfo, string receiveNumber)
    {
        if (!this.InsertCustomerLog(dtblUpdateData, eAgentInfo, upperUserID, htgType, sPageInfo, receiveNumber))
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            }
        }
    }

    #endregion

    /// <summary>
    /// 測試Adapter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnTest_Click(object sender, EventArgs e)
    {
        string userID = this.txtUserId.Text.Trim();
        string receiveNumber = this.txtReceiveNumber.Text.Trim();
        string cusName = this.lblUserNameText.Text.Trim();
        string accNoBank = this.txtAccNoBank.Text.Trim();
        string accNo = this.txtAccNo.Text.Trim();
        string payWay = this.txtPayWay.Text.Trim();
        string accID = this.txtAccID.Text.Trim();
        string bcycleCode = this.txtBcycleCode.Text.Trim();
        string mobilePhone = this.txtMobilePhone.Text.Trim();
        string eMail = this.txtEmail.Text.Trim();
        string eBill = this.txtEBill.Text.Trim();
        EntityAGENT_INFO eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];

        PostToHostAdapter adaptet = new PostToHostAdapter(userID, receiveNumber, cusName, accNoBank, accNo, payWay, accID, bcycleCode, mobilePhone, eMail, eBill, eAgentInfo);
        adaptet.SendToHost();
    }
}
