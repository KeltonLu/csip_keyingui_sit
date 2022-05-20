//******************************************************************
//*  作    者：chenjingxian
//*  功能說明：訊息/更正單二KEY
//*  創建日期：2009/10/02
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
using Framework.Data;
using Framework.WebControls;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Transactions;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Logging;

public partial class P010101100001 : PageBase
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


        Hashtable htInputP4_JCF6;//*查詢第一卡人檔上行
        Hashtable htOutputP4_JCF6;//*查詢第一卡人檔下行
        Hashtable htInputP4_JCDK;//*查詢第二卡人檔上行
        Hashtable htOutputP4_JCDK;//*查詢第二卡人檔下行

        ReSetControls(true, false);
        string strSql = "";
        string sUserId = CommonFunction.GetSubString(this.txtUserId.Text.Trim().ToUpper(), 0, 16);

        //*檢核輸入的收件編號和身分證號碼是否符合規則
        if (!CheckInPutInfo())
        {
            //*失敗
            return;
        }

        #region 獲取主機資料
        //*讀取第一卡人檔
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

            //主機傳回訊息缺少欄位
            ReSetControls(false, false);
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

        if (htOutputP4_JCF6["NAME_1"].ToString().Trim() == "")
        {
            base.strClientMsg = MessageHelper.GetMessage("01_01011000_018");
            return;
        }

        base.strHostMsg = htOutputP4_JCF6["HtgSuccess"].ToString();//*主機返回成功訊息
        htOutputP4_JCF6["ACCT_NBR"] = htInputP4_JCF6["ACCT_NBR"];//* for_xml_test  
        ViewState["HtgInfoP4_JCF6"] = htOutputP4_JCF6;

        //*讀取第二卡人檔
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

            //主機傳回訊息缺少欄位
            ReSetControls(false, false);
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
        htOutputP4_JCDK["ACCT_NBR"] = htInputP4_JCDK["ACCT_NBR"];//* for_xml_test 
        htOutputP4_JCDK["MESSAGE_TYPE"] = "";
        htOutputP4_JCDK["FUNCTION_CODE"] = "2";
        htOutputP4_JCDK["LINE_CNT"] = "0000";
        ViewState["HtgInfoP4_JCDK"] = htOutputP4_JCDK;

        #endregion 獲取主機資料結束

        #region 查詢一Key、二Key資料

        //*查詢一Key資料
        strSql = "Acc_No,Pay_Way,Acc_ID,bcycle_code,Mobile_Phone,E_Mail,E_Bill,Receive_Number,user_id";
        DataSet dstInfoKey1 = BRAUTO_PAY.SelectDetail(sUserId, "1", "Y", "1", strSql);
        if (dstInfoKey1 == null)
        {
            //*查詢資料庫時發生錯誤
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            ReSetControls(false, false);
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (dstInfoKey1.Tables[0].Rows.Count < 1)
        {
            //*若一Key無資料提示一Key后才能二Key
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUserId"));
            ReSetControls(false, false);
            //base.strClientMsg += MessageHelper.GetMessage("01_01011000_008");
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01011000_008") + "');");
            return;
        }

        if (dstInfoKey1.Tables[0].Rows[0][8].ToString().Trim() == eAgentInfo.agent_id)
        {
            //*一KEY與二KEY經辦為同一人
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUserId"));
            ReSetControls(false, false);
            //base.strClientMsg += MessageHelper.GetMessage("01_01011000_009");
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01011000_009") + "');");
            return;
        }

        //*將一Key的收件編號賦值給網頁中的收件編號欄位
        this.txtReceiveNumber.Text = dstInfoKey1.Tables[0].Rows[0][7].ToString();

        //*查詢二Key資料
        strSql = "Acc_No,Pay_Way,Acc_ID,bcycle_code,Mobile_Phone,E_Mail,E_Bill,Receive_Number,user_id";
        DataSet dstInfoKey2 = BRAUTO_PAY.SelectDetail(sUserId, "1", "Y", "2", strSql);
        DataTable dtblAchGet2Keyadd = (DataTable)BRAuto_Pay_Popul.SearchQryACH2Key(this.txtUserId.Text.Trim().ToUpper(), this.txtReceiveNumber.Text.Trim());
        if (dstInfoKey2 == null)
        {
            //*查詢資料庫時發生錯誤
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            ReSetControls(false, false);
            base.strClientMsg = BaseHelper.GetShowText("00_00000000_000");
            return;
        }

        #endregion 查詢一Key、二Key資料結束
        ReSetControls(false, true);
        lblUserNameText.Text = htOutputP4_JCF6["NAME_1"].ToString().Trim();

        #region 將資料庫或主機的資料顯示與網頁中

        //*先將主機姓名顯示出來
        if (dstInfoKey2.Tables[0].Rows.Count > 0)
        {
            string sdbfAccNo = dstInfoKey2.Tables[0].Rows[0][0].ToString().Trim();

            //*查詢出有資料，將查詢出的資料顯示網頁的欄位中。
            //*銀行代號、銀行帳號
            if (sdbfAccNo.Length > 3)
            {
                /*若查詢出的{ Acc_No }.length>3
                  將【扣繳帳號】顯示為查詢出的{ Acc_No }前3碼,【銀行帳號】顯示為查詢出的{ Acc_No }第5碼以后資料。*/
                this.txtAccNoBank.Text = SelectValue(sdbfAccNo.Substring(0, 3));
                this.txtAccNo.Text = CommonFunction.GetSubString(sdbfAccNo.Trim(), 4, sdbfAccNo.Length - 4);

            }
            //*繳款狀況
            this.txtPayWay.Text = dstInfoKey2.Tables[0].Rows[0][1].ToString();
            //*帳戶ID
            this.txtAccID.Text = dstInfoKey2.Tables[0].Rows[0][2].ToString();
            //*帳單週期
            this.txtBcycleCode.Text = dstInfoKey2.Tables[0].Rows[0][3].ToString();
            this.txtBcycleCodeText.Text = this.txtBcycleCode.Text;
            //*行動電話
            this.txtMobilePhone.Text = dstInfoKey2.Tables[0].Rows[0][4].ToString();
            //*Email
            this.txtEmail.Text = dstInfoKey2.Tables[0].Rows[0][5].ToString();
            //*EBill
            this.txtEBill.Text = dstInfoKey2.Tables[0].Rows[0][6].ToString();
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
        else
        {
            //*查詢出無資料，則將viewstate中的訊息顯示與網頁的欄位中
            //*銀行代號、銀行帳號
            this.txtAccNoBank.Text = SelectValue(CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 0, 3));
            this.txtAccNo.Text = CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 4, 26);
            //*繳款狀況
            this.txtPayWay.Text = htOutputP4_JCF6["CO_TAX_ID_TYPE"].ToString();
            //*帳戶ID
            this.txtAccID.Text = htOutputP4_JCF6["DD_ID"].ToString();
            //*帳單週期
            this.txtBcycleCode.Text = htOutputP4_JCF6["BILLING_CYCLE"].ToString();
            this.txtBcycleCodeText.Text = this.txtBcycleCode.Text;
            //*行動電話
            this.txtMobilePhone.Text = htOutputP4_JCDK["MOBILE_PHONE"].ToString();
            //*Email
            this.txtEmail.Text = htOutputP4_JCDK["EMAIL"].ToString();
            //*EBill
            this.txtEBill.Text = htOutputP4_JCF6["OFF_PHONE_FLAG"].ToString();

            //*案件類別
            //this.txtCaseClass.Text = SelectValue2("03");
            this.txtCaseClass.Text = SelectValue2(txtReceiveNumber.Text.Substring(8, 2));
        }
        this.Session["Acct_No_befor"] = CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 4, 26);

        #endregion 將資料庫或主機的資料顯示與網頁中結束

        base.strClientMsg = MessageHelper.GetMessage("01_01011000_005");
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
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end
        ReSetControls(false, true);

        Hashtable htOutputP4_JCF6;//*查詢第一卡人檔下行
        Hashtable htInputPCTIP4S;//*更新第一卡人檔上行
        Hashtable htInputP4_JCDKS = new Hashtable();//*更新第二卡人檔上行
        Hashtable htOutputPCTIP4S;//*更新第一卡人檔下行
        Hashtable htOutputP4_JCDKS;//*更新第二卡人檔下行

        DataTable dtblUpdateData = CommonFunction.GetDataTable();  //*異動customer_log記錄的Table

        int intSubmit = 0; //*記錄異動主機成功的次數

        EntitySet<EntityAUTO_PAY> eAutoPaySetKey1; //*記錄資料庫一Key資料
        EntitySet<EntityAUTO_PAY> eAutoPaySetKey2; //*記錄資料庫二Key資料

        string sAccNoBank = CommonFunction.GetSubString(this.txtAccNoBank.Text.Trim(), 0, 3);
        string sUserId = this.txtUserId.Text.Trim().ToUpper();

        //*檢核輸入的收件編號和身分證號碼是否符合規則
        if (!CheckInPutInfo())
        {
            //*失敗
            return;
        }

        //*讀取第一卡人檔
        htOutputP4_JCF6 = (Hashtable)ViewState["HtgInfoP4_JCF6"];

        //*讀取第二卡人檔
        CommonFunction.GetViewStateHt(ViewState["HtgInfoP4_JCDK"], ref htInputP4_JCDKS);

        //*電子帳單(MAIL)值=0或2才可以修改，其他不能異動！
        if (htOutputP4_JCF6["OFF_PHONE_FLAG"].ToString() != this.txtEBill.Text.Trim())
        {
            if (!(htOutputP4_JCF6["OFF_PHONE_FLAG"].ToString() == "0" || htOutputP4_JCF6["OFF_PHONE_FLAG"].ToString() == "2"))
            {
                base.strClientMsg = MessageHelper.GetMessage("01_01011000_006");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtEBill"));
                return;
            }
        }

        ArrayList arrayName = new ArrayList(new object[] { "ACCT_NBR", "CO_OWNER", "CO_TAX_ID_TYPE", "DD_ID", "BILLING_CYCLE", "OFF_PHONE_FLAG" });

        htInputPCTIP4S = new Hashtable();
        MainFrameInfo.ChangeJCF6toPCTI(htOutputP4_JCF6, htInputPCTIP4S, arrayName); //*將異動主機的訊息預設為從主機讀取的訊息

        #region 查詢、更新資料庫
        //*查詢一Key資料
        try
        {
            eAutoPaySetKey1 = BRAUTO_PAY.SelectEntitySet1Key(sUserId, "1", "Y", "1");
        }
        catch
        {
            //*操作失敗          
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        //*查詢二Key資料
        try
        {
            eAutoPaySetKey2 = BRAUTO_PAY.SelectEntitySet2Key(sUserId, "1", "Y", "2");
        }
        catch
        {
            //*操作失敗           
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        //*更新資料庫
        DataTable dtblAchGet2Keyadd = (DataTable)BRAuto_Pay_Popul.SearchQryACH2KeyAll(this.txtUserId.Text.Trim().ToUpper(), this.txtReceiveNumber.Text.Trim());
        EntityAUTO_PAY eAutoPay = new EntityAUTO_PAY();
        eAutoPay.Receive_Number = this.txtReceiveNumber.Text.Trim();
        if (this.txtAccNoBank.Text.Trim() != "")
        {
            eAutoPay.Acc_No = sAccNoBank + "-" + this.txtAccNo.Text.Trim();
        }
        else
        {
            eAutoPay.Acc_No = "";
        }

        eAutoPay.Pay_Way = this.txtPayWay.Text.Trim();
        eAutoPay.Acc_ID = this.txtAccID.Text.Trim().ToUpper();
        eAutoPay.bcycle_code = this.txtBcycleCode.Text.Trim();
        eAutoPay.Mobile_Phone = this.txtMobilePhone.Text.Trim();
        eAutoPay.E_Mail = this.txtEmail.Text.Trim();
        eAutoPay.E_Bill = this.txtEBill.Text.Trim();
        eAutoPay.user_id = eAgentInfo.agent_id.ToString().Trim();
        eAutoPay.mod_date = DateTime.Now.ToString("yyyyMMdd");

        string sCaseClass = txtCaseClass.Text.Trim().Substring(0, 2);
        string sPopulEmpNo = txtPopulEmpNO.Text.Trim().PadLeft(8, '0');
        string sPopulNo = txtPopulNo.Text.Trim();

        string[] strColumns = { EntityAUTO_PAY.M_Receive_Number, EntityAUTO_PAY.M_Acc_No, EntityAUTO_PAY.M_Pay_Way, EntityAUTO_PAY.M_Acc_ID, EntityAUTO_PAY.M_bcycle_code, EntityAUTO_PAY.M_Mobile_Phone, EntityAUTO_PAY.M_E_Mail, EntityAUTO_PAY.M_E_Bill, EntityAUTO_PAY.M_user_id, EntityAUTO_PAY.M_mod_date };

        if (eAutoPaySetKey2.Count > 0)
        {
            //*查詢二Key資料成功,則更新資料
            if (!BRAUTO_PAY.Update(eAutoPay, strColumns, sUserId, "1", "Y", "2"))
            {
                //*更新操作失敗
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return;
            }

            //*查詢出Auto_Pay_Popul有二Key資料，則更新資料,執行Auto_Pay_Popul的資料修改
            if (dtblAchGet2Keyadd != null && dtblAchGet2Keyadd.Rows.Count > 0)
            {
                if (BRAuto_Pay_Popul.UpdateKeyInfo(this.txtReceiveNumber.Text.Trim(), sUserId, sCaseClass, sPopulNo, sPopulEmpNo, "2", System.DateTime.Now.ToString("yyyyMMdd"), eAgentInfo.agent_id.ToString().Trim(), "C", "N") == false)

                //* 顯示端末訊息
                {
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    return;
                }
            }
            else
            {
                //*Auto_Pay_Popul沒有一Key資料，寫入一Key資料
                EntityAuto_Pay_Popul eBankadd = new EntityAuto_Pay_Popul();
                eBankadd.Receive_Number = this.txtReceiveNumber.Text.Trim();
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
                    base.strClientMsg += MessageHelper.GetMessage("01_01010500_015");
                    return;
                }

            }
        }
        else
        {
            //*沒有二Key資料，寫入二Key資料
            eAutoPay.Cus_ID = sUserId;
            eAutoPay.KeyIn_Flag = "2";
            eAutoPay.Upload_Flag = "N";
            eAutoPay.Add_Flag = "1";

            try
            {
                BRAUTO_PAY.AddEntity(eAutoPay);
            }
            catch
            {
                //*寫入資料失敗
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return;
            }

            //*查詢出Auto_Pay_Popul有二Key資料，則更新資料,執行Auto_Pay_Popul的資料修改
            if (dtblAchGet2Keyadd != null && dtblAchGet2Keyadd.Rows.Count > 0)
            {
                if (BRAuto_Pay_Popul.UpdateKeyInfo(this.txtReceiveNumber.Text.Trim(), sUserId, sCaseClass, sPopulNo, sPopulEmpNo, "2", System.DateTime.Now.ToString("yyyyMMdd"), eAgentInfo.agent_id.ToString().Trim(), "C", "N") == false)

                //* 顯示端末訊息
                {
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    return;
                }
            }
            else
            {
                //*Auto_Pay_Popul沒有一Key資料，寫入一Key資料
                EntityAuto_Pay_Popul eBankadd = new EntityAuto_Pay_Popul();
                eBankadd.Receive_Number = this.txtReceiveNumber.Text.Trim();
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
                    base.strClientMsg += MessageHelper.GetMessage("01_01010500_015");
                    return;
                }

            }
        }

        base.strClientMsg += MessageHelper.GetMessage("01_01011000_007");

        #endregion 查詢、更新資料庫結束

        #region 比較一Key和二Key的資料是否相同

        if (!Compare(eAutoPaySetKey1))
        {
            return;
        }

        DataTable dtblAchGet1Keyadd = (DataTable)BRAuto_Pay_Popul.SearchQryACH1Key(sUserId, this.txtReceiveNumber.Text.Trim());
        if (!CompareAdd(dtblAchGet1Keyadd))
        {
            base.strClientMsg = MessageHelper.GetMessage("01_01011000_010");
            return;
        }

        #endregion 比較一Key和二Key的資料是否相同結束

        //20130424 Yucheng 訊息更正單也要記錄是否異動Emai,週期件等資訊
        //並寫入到Auto_Pay_Status提供自扣案件查詢使用
        //20130510 Yucheng 
        //eAuto_Pay_Status.IsCTCB 等於S 為訊息更正單異動進入

        //*比對完以后新增欄位錄入Auto_Pay_Popul資料，，并記錄LOG
        BRAuto_Pay_Popul.Update2KeyInfo(this.txtReceiveNumber.Text.Trim(), sUserId, sCaseClass, sPopulNo, sPopulEmpNo, "2", System.DateTime.Now.ToString("yyyyMMdd"), eAgentInfo.agent_id.ToString().Trim());
        beforeInfo = (EntityAuto_Pay_Popul)this.Session["beforeInfo"];
        this.CustomerLog_Add(beforeInfo);

        string strAcctNBR = htOutputP4_JCF6["ACCT_NBR"].ToString().Trim();
        string strAcctNO = htOutputP4_JCF6["CO_OWNER"].ToString().Trim();

        EntityAuto_Pay_Status eAuto_Pay_Status = new EntityAuto_Pay_Status();
        eAuto_Pay_Status.Receive_Number = this.txtReceiveNumber.Text.Trim();
        eAuto_Pay_Status.Cus_ID = this.txtUserId.Text.Trim();
        eAuto_Pay_Status.Cus_Name = this.lblUserNameText.Text.Trim();
        eAuto_Pay_Status.AccNoBank = sAccNoBank;
        eAuto_Pay_Status.Acc_No = sAccNoBank + "-" + this.txtAccNo.Text.Trim();
        eAuto_Pay_Status.Pay_Way = this.txtPayWay.Text.Trim();
        eAuto_Pay_Status.IsCTCB = "S";
        eAuto_Pay_Status.DateTime = DateTime.Now;
        eAuto_Pay_Status.Acc_No_O = strAcctNO;


        //記錄電子賬單是否被異動
        bool bIsEBill_Updated = (htOutputP4_JCF6["OFF_PHONE_FLAG"].ToString().Trim() != txtEBill.Text.Trim());
        //記錄第二卡人資料是否被異動
        bool bIsP4_JCDK_Updated = ((htInputP4_JCDKS["MOBILE_PHONE"].ToString().Trim() != this.txtMobilePhone.Text.Trim()) || ((htInputP4_JCDKS["EMAIL"].ToString().Trim() != this.txtEmail.Text.Trim())));
        //記錄銀行帳戶是否被異動以及會由哪種方式異動

        eAuto_Pay_Status.IsUpdateByTXT = "N";


        #region 異動主機資料

        #region 異動第一卡人檔

        //*異動扣繳帳號和銀行帳號
        CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, sAccNoBank + "-" + this.txtAccNo.Text.Trim(), "BK_ID_AC", BaseHelper.GetShowText("01_01010800_006"));
        //*繳款狀況
        CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, this.txtPayWay.Text.Trim(), "LAST_CR_LINE_IND", BaseHelper.GetShowText("01_01010800_008"));
        //*帳戶ID
        CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, this.txtAccID.Text.Trim().ToUpper(), "DD_ID", BaseHelper.GetShowText("01_01010800_009"));
        //*帳單週期
        CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, this.txtBcycleCode.Text.Trim(), "BILLING_CYCLE", BaseHelper.GetShowText("01_01010800_010"));
        //*電子帳單
        CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, this.txtEBill.Text.Trim(), "MAIL", BaseHelper.GetShowText("01_01010800_013"));

        if (dtblUpdateData.Rows.Count > 0)
        {
            htInputPCTIP4S.Add("FUNCTION_ID", "PCMC1");
            htOutputPCTIP4S = MainFrameInfo.GetMainFrameInfo(HtgType.P4_PCTI, htInputPCTIP4S, false, "2", eAgentInfo);

            if (!htOutputPCTIP4S.Contains("HtgMsg"))
            {
                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtUserId.Text.Trim().ToUpper(), "P4", sPageInfo))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }
                intSubmit++;
                base.strHostMsg += htOutputPCTIP4S["HtgSuccess"].ToString();//*主機返回成功訊息
            }
            else
            {
                //*異動主機資料失敗
                if (htOutputPCTIP4S["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htOutputPCTIP4S["HtgMsg"].ToString();
                    base.strClientMsg = MessageHelper.GetMessage("01_01011000_012");
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
            //*無需異動資料    
            base.strClientMsg = MessageHelper.GetMessage("01_01011000_019");
        }

        #endregion

        #region 異動第二卡人檔


        dtblUpdateData.Clear();

        //*上傳身分證號碼
        htInputP4_JCDKS["ACCT_NBR"] = CommonFunction.GetSubString(this.txtUserId.Text.Trim().ToUpper(), 0, 16);
        //*移動電話
        CommonFunction.ContrastData(htInputP4_JCDKS, dtblUpdateData, this.txtMobilePhone.Text.Trim(), "MOBILE_PHONE", BaseHelper.GetShowText("01_01010800_011"));
        //*Email
        CommonFunction.ContrastData(htInputP4_JCDKS, dtblUpdateData, this.txtEmail.Text.Trim(), "EMAIL", BaseHelper.GetShowText("01_01010800_012"));

        if (dtblUpdateData.Rows.Count > 0)
        {
            //*提交修改主機資料
            htOutputP4_JCDKS = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCDK, htInputP4_JCDKS, false, "2", eAgentInfo);

            if (!htOutputP4_JCDKS.Contains("HtgMsg"))
            {
                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtUserId.Text.Trim().ToUpper(), "P4", sPageInfo))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }
                intSubmit++;
                base.strHostMsg += htOutputP4_JCDKS["HtgSuccess"].ToString();//*主機返回成功訊息
            }
            else
            {
                //*異動主機資料失敗
                if (htOutputP4_JCDKS["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htOutputP4_JCDKS["HtgMsg"].ToString();
                    base.strClientMsg = MessageHelper.GetMessage("01_01011000_014");
                }
                else
                {
                    base.strClientMsg += htOutputP4_JCDKS["HtgMsg"].ToString();
                }

                base.sbRegScript.Append(BaseHelper.SetFocus("txtAccNoBank"));
                return;

            }
        }
        else
        {
            //*無需異動資料
            base.strClientMsg += MessageHelper.GetMessage("01_01011000_013");
        }

        #endregion

        //if (intSubmit > 0)
        //{
        if (BRAuto_pay_status.AddNewEntity(eAuto_Pay_Status))
        {
            if (eAuto_Pay_Status.IsUpdateByTXT == "N")
            {
                base.strHostMsg += MessageHelper.GetMessage("01_01010800_018");
            }
        }
        base.strClientMsg += MessageHelper.GetMessage("01_01011000_015");
        if (!BRTRANS_NUM.UpdateTransNum("A14"))
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            }
        }

        //*更新資料庫異動狀態
        //*更新資料庫
        eAutoPay = new EntityAUTO_PAY();
        eAutoPay.Upload_Flag = "Y";
        //eAutoPay.user_id = eAgentInfo.agent_id.ToString();
        eAutoPay.mod_date = DateTime.Now.ToString("yyyyMMdd");

        //自扣設定(銀行帳號原本為空者，新增帳號後該欄位填入Y)
        if (htOutputP4_JCF6["CO_OWNER"].ToString().Trim() == "" && this.txtAccNo.Text.Trim() != "")
            eAutoPay.Auto_Pay_Setting = "Y";
        else
            eAutoPay.Auto_Pay_Setting = "N";

        //手機及E-MAIL設定(第二卡人檔有被異動時(行動電話、E-MAIL，新增或異動後該欄位填入Y)
        if (bIsP4_JCDK_Updated)
            eAutoPay.CellP_Email_Setting = "Y";
        else
            eAutoPay.CellP_Email_Setting = "N";

        //電子帳單設定(電子帳單被異動時該欄位填入Y)
        if (bIsEBill_Updated)
            eAutoPay.E_Bill_Setting = "Y";
        else
            eAutoPay.E_Bill_Setting = "N";

        //記錄持卡人ID
        eAutoPay.Acct_NBR = strAcctNBR;


        //週期件註記(二KEY完自扣資料被送至主機TEMP檔者)
        eAutoPay.OutputByTXT_Setting = eAuto_Pay_Status.IsUpdateByTXT;

        //string[] strColumnsUpdate = { EntityAUTO_PAY.M_Upload_Flag,  EntityAUTO_PAY.M_mod_date };
        string[] strColumnsUpdate = { EntityAUTO_PAY.M_Upload_Flag, EntityAUTO_PAY.M_mod_date,
                EntityAUTO_PAY.M_Auto_Pay_Setting, EntityAUTO_PAY.M_CellP_Email_Setting, EntityAUTO_PAY.M_E_Bill_Setting, EntityAUTO_PAY.M_OutputByTXT_Setting,  EntityAUTO_PAY.M_Acct_NBR};


        if (!BRAUTO_PAY.UpdateSucc(eAutoPay, strColumnsUpdate, sUserId, "1", "Y", this.txtReceiveNumber.Text.Trim()))
        {
            //*更新操作失敗
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            }
        }
        //}
        //else
        //{
        //    base.strClientMsg += MessageHelper.GetMessage("01_00000000_015");
        //}

        #endregion 異動主機資料結束
        ReSetControls(true, false);
        this.txtReceiveNumber.Text = "";
        this.txtUserId.Text = "";
        base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
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
    /// <param name="strAccNoBank">銀行3碼</param>
    /// <returns>銀行3碼+銀行名稱</returns>
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
    /// 設置控件文本
    /// </summary>
    private void SetControlsText()
    {
        btnSelect.Text = BaseHelper.GetShowText("01_01011000_005");
        btnSubmit.Text = BaseHelper.GetShowText("01_01011000_014");
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
        if (BaseHelper.GetCommonProperty("01", "17", ref dtblResult))
        {
            for (int i = 0; i < dtblResult.Rows.Count; i++)
            {
                ListItem listItem = new ListItem();
                listItem.Text = dtblResult.Rows[i][0].ToString() + " " + dtblResult.Rows[i][1].ToString();
                //listItem.Value = dtblResult.Rows[i][0].ToString() + " " + dtblResult.Rows[i][1].ToString();
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

        //this.dropCaseClass.Enabled = blnEnabled;
        //this.txtCaseClass.Enabled = blnEnabled;
        this.txtPopulEmpNO.Enabled = blnEnabled;
        this.txtPopulNo.Enabled = blnEnabled;

        this.txtReceiveNumber.BackColor = Color.White;
        this.txtUserId.BackColor = Color.White;
        this.txtAccNoBank.BackColor = Color.White;
        this.txtAccNo.BackColor = Color.White;
        this.txtPayWay.BackColor = Color.White;
        this.txtAccID.BackColor = Color.White;

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
        string sAccNoBank = CommonFunction.GetSubString(this.txtAccNoBank.Text.Trim(), 0, 3);

        //*檢查銀行3碼代號
        if (this.txtAccNoBank.Text.Trim() != "")
        {
            if (sAccNoBank != CommonFunction.GetSubString(eAutoPaySetKey1.GetEntity(0).Acc_No.Trim(), 0, 3))
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
            base.strClientMsg = MessageHelper.GetMessage("01_01011000_010");
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
        string strSql = ""; //*查詢資料庫的顯示內容
        DataSet dstInfo;//*記錄查詢結果

        /*若收件編號已被使用，不能再被使用*/
        strSql = "Receive_Number,Cus_ID";
        dstInfo = BRAUTO_PAY.Select(this.txtReceiveNumber.Text.Trim(), "1", "Y", strSql);

        if (dstInfo == null)
        {
            //*查詢資料庫時發生錯誤
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            ReSetControls(false, false);
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return false;
        }

        if (dstInfo.Tables[0].Rows.Count > 0)
        {
            //*此收件編號已被使用，不能再被使用
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            ReSetControls(false, false);
            base.strClientMsg += MessageHelper.GetMessage("01_01011000_001");
            return false;
        }

        /*收件編號重複*/
        strSql = "Receive_Number,Cus_ID";
        dstInfo = BRAUTO_PAY.SelectFlag(this.txtReceiveNumber.Text.Trim(), "1", "Y", strSql);

        if (dstInfo == null)
        {
            //*查詢資料庫時發生錯誤
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            ReSetControls(false, false);
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return false;
        }

        if (dstInfo.Tables[0].Rows.Count > 0)
        {
            string dtbUserId = dstInfo.Tables[0].Rows[0][1].ToString().Trim();

            //*查詢出了資料，比較查詢出的{ Cus_ID }和輸入的【身分證號碼】是否一樣。
            if (dtbUserId != this.txtUserId.Text.Trim().ToUpper())
            {
                base.strClientMsg = MessageHelper.GetMessage("01_01010900_002") + ": 此收件編號已被 " + dtbUserId + " 使用";
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010900_002") + ": 此收件編號已被 " + dtbUserId + " 使用');");

                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                this.txtUserId.BackColor = Color.Red;
                //ReSetControls(true,false);
                return false;
            }
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
        if (txtPopulEmpNO.Text.Trim() != beforeInfo.Popul_EmpNo.ToString().Trim())
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
            //drowRow[EntityCUSTOMER_LOG.M_after] = txtCaseClass.Text.Trim().Substring(0, 2);
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
            Logging.Log(exp, LogLayer.UI);
        }
    }

    #endregion
}
