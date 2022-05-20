//******************************************************************
//*  作    者：chenjingxian
//*  功能說明：中信及郵局 自扣一KEY
//*  創建日期：2009/09/23
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

public partial class P010101070001 : PageBase
{

    #region 變數區
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息
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
        Hashtable htInputP4_JCF6;//*查詢第一卡人檔上行
        Hashtable htOutputP4_JCF6;//*查詢第一卡人檔下行
        Hashtable htInputP4_JCDK;//*查詢第二卡人檔上行
        Hashtable htOutputP4_JCDK;//*查詢第二卡人檔下行

        string strSql = "";
        string sUserId = CommonFunction.GetSubString(this.txtUserId.Text.Trim().ToUpper(), 0, 16);
        string sReceiveNumber = this.txtReceiveNumber.Text.Trim();
        ReSetControls(true, false);

        //*檢核輸入的收件編號和身分證號碼是否符合規則
        if (!CheckInPutInfo())
        {
            //*失敗
            //return;
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
                base.strClientMsg = MessageHelper.GetMessage("01_01010700_009");
            }
            else
            {
                base.strClientMsg = htOutputP4_JCF6["HtgMsg"].ToString();
            }
            return;
        }

        if (htOutputP4_JCF6["NAME_1"].ToString().Trim() == "")
        {
            base.strClientMsg = MessageHelper.GetMessage("01_01010700_008");
            return;
        }
        base.strHostMsg = htOutputP4_JCF6["HtgSuccess"].ToString();//*主機返回成功訊息
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
                base.strClientMsg = MessageHelper.GetMessage("01_01010700_003");
            }
            else
            {
                base.strClientMsg = htOutputP4_JCDK["HtgMsg"].ToString();
            }
            return;
        }
        base.strHostMsg += htOutputP4_JCDK["HtgSuccess"].ToString();//*主機返回成功訊息
        htOutputP4_JCDK["ACCT_NBR"] = htInputP4_JCDK["ACCT_NBR"];//* for_xml_test  
        ViewState["HtgInfoP4_JCDK"] = htOutputP4_JCDK;


        //2011-12-30 ADD 檢核條件
        if (!checkCredit())
        {
            string[] strArray = new string[] { "01_01010700_011" };
            MessageHelper.ShowMessageWithParms(this, strArray[0], strArray);

            return;
        }


        #endregion 獲取主機資料結束

        //*查詢一Key資料
        strSql = "Acc_No,Pay_Way,Acc_ID,bcycle_code,Mobile_Phone,E_Mail,E_Bill,Receive_Number ,user_id";
        DataSet dstInfo = CSIPKeyInGUI.BusinessRules_new.BRAuto_pay.SelectDetail(this.txtUserId.Text.ToUpper().Trim(), sReceiveNumber, "0", "Y", "1", strSql);

        DataTable dtblAchGet1Keyadd = (DataTable)BRAuto_Pay_Popul.SearchQryACH1Key(this.txtUserId.Text.Trim().ToUpper(), sReceiveNumber);

        if (dstInfo == null)
        {
            //*查詢資料庫時發生錯誤
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            ReSetControls(false, false);
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        ReSetControls(false, true);
        lblUserNameText.Text = htOutputP4_JCF6["NAME_1"].ToString().Trim();

        #region 將資料庫或主機的資料顯示與網頁中

        //*先將主機姓名顯示出來
        if (dstInfo.Tables[0].Rows.Count > 0)
        {
            DataRow drInfo = dstInfo.Tables[0].Rows[0];
            string sdbfAccNo = drInfo[0].ToString().Trim();

            //*查詢出有資料，將查詢出的資料顯示網頁的欄位中。

            //*銀行代號、銀行帳號
            if (sdbfAccNo.Length > 3)
            {
                //if (CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 0, 3) == "701" || CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 0, 3) == "042")
                //{
                //    this.txtAccNoBank.Text = sdbfAccNo.Substring(0, 3);
                //}
                //else
                //{
                //    this.txtAccNoBank.Text = "";
                //}
                this.txtAccNoBank.Text = sdbfAccNo.Substring(0, 3);
                txtAccNo.Text = CommonFunction.GetSubString(sdbfAccNo, 4, sdbfAccNo.Length - 4);
            }

            //*繳款狀況
            this.txtPayWay.Text = drInfo[1].ToString();
            //*帳戶ID
            this.txtAccID.Text = drInfo[2].ToString();
            //*帳單週期
            this.txtBcycleCodeText.Text = drInfo[3].ToString();
            this.txtBcycleCode.Text = this.txtBcycleCodeText.Text;
            //*行動電話
            this.txtMobilePhone.Text = drInfo[4].ToString();
            //*Email
            this.txtEmail.Text = drInfo[5].ToString();
            //*EBill
            this.txtEBill.Text = drInfo[6].ToString();
            //*新增三個欄位的顯示
            if (dtblAchGet1Keyadd.Rows.Count > 0)
            {
                txtCaseClass.Text = SelectValue2(dtblAchGet1Keyadd.Rows[0][2].ToString().Trim());
                txtPopulEmpNO.Text = dtblAchGet1Keyadd.Rows[0][0].ToString().Trim();
                txtPopulNo.Text = dtblAchGet1Keyadd.Rows[0][1].ToString().Trim();
            }
            else
            {
                this.txtCaseClass.Text = SelectValue2(txtReceiveNumber.Text.Substring(8, 2));
            }
        }
        else
        {
            //*銀行代號、銀行帳號
            if (CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 0, 3) == "701" || CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 0, 3) == "042")
            {
                this.txtAccNoBank.Text = CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 0, 3);
            }
            else
            {
                this.txtAccNoBank.Text = "";
            }
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

        #endregion 將資料庫或主機的資料顯示與網頁中結束

        this.Session["Acct_No_befor"] = CommonFunction.GetSubString(htOutputP4_JCF6["CO_OWNER"].ToString(), 4, 26);

        base.strClientMsg = MessageHelper.GetMessage("01_01010700_005");
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
        string sAccNoBank = CommonFunction.GetSubString(this.txtAccNoBank.Text.Trim(), 0, 3);
        string sUserId = this.txtUserId.Text.Trim().ToUpper();
        string sReceiveNumber = this.txtReceiveNumber.Text.Trim();

        //*檢核輸入的收件編號和身分證號碼是否符合規則
        //if (!CheckInPutInfo()) 
        if (txtCaseClass.Text == string.Empty)
        {
            MessageHelper.GetMessage("01_01010500_023");
            return;
        }
        if (!CheckInPutInfo())
        {
            //*失敗
            return;
        }
        //2011-12-20 ADD 檢核條件
        if (sAccNoBank == "042")
        {
            if (!checkInfo())
            {
                //*失敗
                return;
            }
        }


        //edit by mel 20140901 start 
        //新增郵局自扣 週期檢核RQ-2014-014138-000
        if (sAccNoBank.Substring(0, 1) == "7" && !checkBcycleCode())
        {
            //*失敗
            return;
        }
        //edit by mel 20140901 end 


        Hashtable htEBill = (Hashtable)ViewState["HtgInfoP4_JCF6"];

        //*電子帳單(MAIL)值=0或2才可以修改，其他不能異動！
        if (htEBill["OFF_PHONE_FLAG"].ToString().Trim() != this.txtEBill.Text.Trim())
        {
            if (!(htEBill["OFF_PHONE_FLAG"].ToString().Trim() == "0" || htEBill["OFF_PHONE_FLAG"].ToString().Trim() == "2"))
            {
                base.strClientMsg = MessageHelper.GetMessage("01_01010700_006");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtEBill"));
                return;
            }
        }

        //*查詢一Key資料
        EntitySet<EntityAUTO_PAY> eAutoPaySet = null;
        try
        {
            //eAutoPaySet = BRAUTO_PAY.SelectEntitySet1Key(sUserId, "0", "Y", "1");
            eAutoPaySet = CSIPKeyInGUI.BusinessRules_new.BRAuto_pay.SelectEntitySet(sUserId, sReceiveNumber, "0", "Y", "1");
        }
        catch
        {
            //*操作失敗      
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
            return;
        }
        //DataTable dtblAchGet1Keyadd = (DataTable)BRAuto_Pay_Popul.SearchQryACH1Key(eAgentInfo.agent_id.ToString().Trim(), this.txtReceiveNumber.Text.Trim());
        DataTable dtblAchGet1Keyadd = (DataTable)BRAuto_Pay_Popul.SearchQryACH1Key(this.txtUserId.Text.Trim().ToUpper(), sReceiveNumber);
        EntityAUTO_PAY eAutoPay = new EntityAUTO_PAY();
        eAutoPay.Receive_Number = sReceiveNumber;
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

        if (eAutoPaySet.Count > 0)
        {
            //*查詢一Key資料成功,則更新資料
            //bool bUpdate = BRAUTO_PAY.Update(eAutoPay, strColumns, sUserId, "0", "Y", "1");
            bool bUpdate = CSIPKeyInGUI.BusinessRules_new.BRAuto_pay.Update(eAutoPay, strColumns, sUserId, sReceiveNumber, "0", "Y", "1");
            if (!bUpdate)
            {
                //*更新操作失敗
                base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
                return;
            }

            //*查詢出Auto_Pay_Popul有一Key資料，則更新資料,執行Auto_Pay_Popul的資料修改
            if (dtblAchGet1Keyadd != null && dtblAchGet1Keyadd.Rows.Count > 0)
            {
                if (BRAuto_Pay_Popul.UpdateKeyInfo(sReceiveNumber, sUserId, sCaseClass, sPopulNo, sPopulEmpNo, "1", System.DateTime.Now.ToString("yyyyMMdd"), eAgentInfo.agent_id.ToString().Trim(), "C", "N") == false)

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
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    return;
                }

            }
        }
        else
        {
            //*沒有一Key資料，寫入一Key資料
            eAutoPay.Cus_ID = sUserId;
            eAutoPay.KeyIn_Flag = "1";
            eAutoPay.Upload_Flag = "N";
            eAutoPay.Add_Flag = "0";

            try
            {
                BRAUTO_PAY.AddEntity(eAutoPay);
            }
            catch
            {
                //*寫入資料失敗
                base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
                return;
            }


            //*查詢出Auto_Pay_Popul有一Key資料，則更新資料,執行Auto_Pay_Popul的資料修改
            if (dtblAchGet1Keyadd.Rows.Count > 0)
            {
                if (BRAuto_Pay_Popul.UpdateKeyInfo(sReceiveNumber, sUserId, sCaseClass, sPopulNo, sPopulEmpNo, "1", System.DateTime.Now.ToString("yyyyMMdd"), eAgentInfo.agent_id.ToString().Trim(), "A", "N") == false)

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
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    return;
                }

            }
        }
        base.strClientMsg = MessageHelper.GetMessage("01_01010700_007");
        ReSetControls(true, false);
        this.txtReceiveNumber.Text = "";
        this.txtUserId.Text = "";
        base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
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
        btnSelect.Text = BaseHelper.GetShowText("01_01010700_005");
        btnSubmit.Text = BaseHelper.GetShowText("01_01010700_014");
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


        this.txtUserId.BackColor = Color.White;
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
        //20131008 Casper 驗證方式改變
        //dstInfo = BRAUTO_PAY.Select(this.txtReceiveNumber.Text.Trim(), "0", "Y", strSql);
        dstInfo = BRAUTO_PAY.Select(this.txtReceiveNumber.Text.Trim());

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
            string dtbUserIda = dstInfo.Tables[0].Rows[0][1].ToString().Trim();

            //*查詢出了資料，比較查詢出的{ Cus_ID }和輸入的【身分證號碼】是否一樣。
            if (dtbUserIda != this.txtUserId.Text.Trim().ToUpper())
            {
                base.strClientMsg = MessageHelper.GetMessage("01_01010700_002") + ": 此收件編號已被 " + dtbUserIda + " 使用,請確認後再繼續操作~";
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010700_002") + ": 此收件編號已被 " + dtbUserIda + " 使用,請確認後再繼續操作~');");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                this.txtReceiveNumber.BackColor = Color.Red;
                //ReSetControls(false);
                return false;
            }
        }

        /*收件編號重複*/
        //20131029 Casper 暫停使用 使用上面

        strSql = "Receive_Number,Cus_ID";
        dstInfo = BRAUTO_PAY.SelectFlag(this.txtReceiveNumber.Text.Trim(), "0", "Y", strSql);
        /*
        if (dstInfo == null) {
            //*查詢資料庫時發生錯誤
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            ReSetControls(false, false);
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return false;
        }

        if (dstInfo.Tables[0].Rows.Count > 0) {
            string dtbUserId = dstInfo.Tables[0].Rows[0][1].ToString().Trim();

            //*查詢出了資料，比較查詢出的{ Cus_ID }和輸入的【身分證號碼】是否一樣。
            if (dtbUserId != this.txtUserId.Text.Trim().ToUpper()) {
                base.strClientMsg = MessageHelper.GetMessage("01_01010700_002") + ": 此收件編號已被 " + dtbUserId + " 使用";
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010700_002") + ": 此收件編號已被 " + dtbUserId + " 使用');");

                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                this.txtUserId.BackColor = Color.Red;
                //ReSetControls(false);
                return false;
            }
        }
         */
        return true;
    }
    #endregion
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


    /// <summary>
    /// add by mel 20140901
    /// 新增郵局自扣 週期檢核RQ-2014-014138-000
    /// 
    /// </summary>
    /// <returns></returns>
    private bool checkBcycleCode()
    {


        //*檢核他行帳單週期是否為05/10/15/20/27週期(可隨系統維護中的資料維護中的帳單週期變動)，若輸入非系統設定的週期則出”帳單週期錯誤”之錯誤訊息
        DataSet dstInfo = BaseHelper.CheckCommonPropertySet("01", "15", this.txtBcycleCode.Text.Trim());

        if (dstInfo == null)
        {
            //*查詢資料庫時發生錯誤
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            //ReSetControls(false, false);
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("00_00000000_000") + "');");
            return false;
        }
        else
        {
            if (dstInfo.Tables[0].Rows.Count == 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01010500_020");
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01010500_020") + "');");
                //ReSetControls(false, false);
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
        //2013-03-22    add by Mars
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
