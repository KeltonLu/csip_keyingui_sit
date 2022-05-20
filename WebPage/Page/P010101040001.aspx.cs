//******************************************************************
//*  作    者：趙呂梁  

//*  功能說明：卡人資料異動-異動姓名(生日)

//*  創建日期：2009/10/06
//*  修改記錄：
//*  2010/12/21   chaoma      RQ-2010-005537-000      增設(□P4及□P4D)作業選項及【自動翻譯】按鈕 
//*  2021/07/15   Joe                                 增設中文姓名檢核(不可含有數字0~9)
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*  Joe                2021/07/15         RQ-2021-019992-000      增設中文姓名檢核(不可含有數字0~9)
//*  Nash               2021/10/05         RQ-2021-019992-000      增設英文姓名檢核(不可輸入連續7個數字)
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
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Message;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Logging;
using System.Collections.Generic;
using CSIPCommonModel.EntityLayer_new;
using Framework.Common.Utility;
using System.Text.RegularExpressions;

public partial class P010101040001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    /// <summary>
    /// 切換測試電文
    /// </summary>
    private bool isTest;

    /// <summary>
    /// 通用字典，各項次機能以前兩碼區分
    /// </summary>
    //Dictionary<string, string> DCCommonColl;
    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        //---------------------------------------------------------------        
        isTest = (UtilHelper.GetAppSettings("isTest") == "Y");
        //---------------------------------------------------------------

        if (!IsPostBack)
        {
            //*modify by chaoma RQ-2010-005537-000 Start
            if (chkP4.Enabled == true)
            {
                chkP4.Checked = true;
            }
            if (chkP4D.Enabled == true)
            {
                chkP4D.Checked = true;
            }
            //*modify by chaoma RQ-2010-005537-000 End
            SetControlsText();
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUserId"));
            //*設置GridView分頁顯示的行數
            int intPageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
            this.gpList.PageSize = intPageSize;
            this.grvCardData.PageSize = intPageSize;
            this.gpList.Visible = false;
            this.grvCardData.Visible = false;
            CommonFunction.SetControlsEnabled(pnlText, false);
        }
        base.strClientMsg = "";
        base.strHostMsg = "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/06
    /// 修改日期：2009/10/06 
    /// <summary>
    /// 查詢事件
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtUserId.Text;

        //20200109-修改SOC存入條件
        //log.Statement_Text = string.Format("UserId:{0}", log.Customer_Id); //查詢條件內容: 用 | 區隔,要看文件確認
        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔

        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        this.gpList.CurrentPageIndex = 1;
        CommonFunction.SetControlsEnabled(pnlText, false);
        Hashtable htReturnP4D = new Hashtable();
        Hashtable htReturnP4 = new Hashtable();
        Hashtable htReturn = new Hashtable();
        DataTable dtblCard = GetDataTable();

        bool blnP4 = false;
        bool blnP4D = false;

        ViewState["P4_Flag"] = false;
        ViewState["P4D_Flag"] = false;

        ViewState["SelChkP4_Flag"] = false;
        ViewState["SelChkP4D_Flag"] = false;

        if (chkP4.Enabled == false && chkP4D.Enabled == false)
        {
            base.sbRegScript.Append("alert('此角色無 P4 及 P4D 執行權限');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUserId"));
            return;
        }

        //*modify by chaoma RQ-2010-005537-000 Start
        if (chkP4.Checked == true)
        {
            ViewState["SelChkP4_Flag"] = true;
            blnP4 = GetMainframeData(HtgType.P4_JCF6, ref htReturnP4, dtblCard);//*查詢P4環境主機資料
            ViewState["HtgInfoP4"] = htReturnP4;//*存儲P4環境主機資料
            ViewState["P4_Flag"] = blnP4;//*存儲P4環境是否查詢主機資料成功
            if (blnP4)
            {
                base.strClientMsg = MessageHelper.GetMessage("01_00000000_012");
                base.strHostMsg = htReturnP4["HtgSuccess"].ToString();
                htReturn = htReturnP4;
            }
            else
            {
                base.strHostMsg = htReturnP4["HtgMsg"].ToString();
            }
        }

        if (chkP4D.Checked == true)
        {
            ViewState["SelChkP4D_Flag"] = true;
            blnP4D = GetMainframeData(HtgType.P4D_JCF6, ref htReturnP4D, dtblCard);//*查詢P4D環境主機資料  
            ViewState["HtgInfoP4D"] = htReturnP4D;//*存儲P4D環境主機資料
            ViewState["P4D_Flag"] = blnP4D;//*存儲P4D環境是否查詢主機資料成功
            if (blnP4D)
            {
                base.strClientMsg = MessageHelper.GetMessage("01_00000000_012");
                base.strHostMsg += htReturnP4D["HtgSuccess"].ToString();
                if (!blnP4)
                {
                    htReturn = htReturnP4D;
                }
            }
            else
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
                base.strHostMsg += htReturnP4D["HtgMsg"].ToString();
            }
        }

        if ((bool)ViewState["SelChkP4_Flag"] == false && (bool)ViewState["SelChkP4D_Flag"] == false)
        {
            base.sbRegScript.Append("alert('請勾選 P4 或 P4D 主機環境');");
            return;
        }



        //if (blnP4) //*P4環境查詢主機資料成功
        //{
        //    base.strClientMsg = MessageHelper.GetMessage("01_00000000_012");
        //    base.strHostMsg = htReturnP4["HtgSuccess"].ToString();
        //    htReturn = htReturnP4;

        //    if (blnP4D)
        //    {
        //        base.strHostMsg += htReturnP4D["HtgSuccess"].ToString();
        //    }
        //    else
        //    {
        //        base.strHostMsg += htReturnP4D["HtgMsg"].ToString();
        //    }
        //}
        //else
        //{
        //    base.strHostMsg = htReturnP4["HtgMsg"].ToString();

        //    if (blnP4D)//*P4D環境查詢主機資料成功
        //    {
        //        base.strClientMsg = MessageHelper.GetMessage("01_00000000_012");
        //        base.strHostMsg += htReturnP4D["HtgSuccess"].ToString();
        //        htReturn = htReturnP4D;
        //    }
        //    else
        //    {
        //        base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
        //        base.strHostMsg += htReturnP4D["HtgMsg"].ToString();
        //    }
        //}

        //if (chkP4.Checked == true && chkP4D.Checked == true)
        //{
        //    DataView dv = dtblCard.DefaultView;
        //    dv.RowFilter = "[P4/P4D]='P4'";
        //    ViewState["DataBind"] = dv.ToTable();

        //}
        //else
        //{
        //    ViewState["DataBind"] = dtblCard;
        //}

        ViewState["DataBind"] = dtblCard;

        //*modify by chaoma RQ-2010-005537-000 End

        if (blnP4 || blnP4D)
        {
            ViewState["HtgInfo"] = htReturn;
            CommonFunction.SetControlsEnabled(pnlText, true);
            //*頁面欄位賦值
            this.txtName.Text = htReturn["NAME_1"].ToString();
            this.txtName.Enabled = false;
            this.txtEName.Text = htReturn["EMBOSSER_NAME_11"].ToString();
            //------長姓名--------------
            string lName = string.Empty;
            string lRoma = string.Empty;
            lName = this.txtName.Text;
            //需再依指標決定是否要再查 長姓名電文(JC99)                        
            using (BRHTG_JC99 obj = new BRHTG_JC99(sPageInfo.strPageCode))
            {
                if (obj.getIsLongNameFlag(htReturn) == true)
                {
                    EntityHTG_JC99 _jc99Info = new EntityHTG_JC99();                    
                    _jc99Info.IDNO_NO = CommonFunction.GetSubString(this.txtUserId.Text.Trim().ToUpper(), 0, 16);
                    _jc99Info = obj.getData(_jc99Info, eAgentInfo);
                    lName = _jc99Info.NAME;
                    lRoma = _jc99Info.ROMA;
                }
            }
            //------長姓名--------------
            htReturnP4["LNAME"] = lName;
            htReturnP4["ROMA"] = lRoma;
            ViewState["HtgInfoP4"] = htReturnP4;//*存儲P4環境主機資料            
            this.txtLName.Text = lName;
            this.txtRoma.Text = lRoma;

            //string _errMsg = string.Format("{0}. {1}", sPageInfo.strPageCode, MessageHelper.GetMessage("01_00000000_026"));
            //Logging.SaveLog(ELogLayer.UI, _errMsg, ELogType.Error);
            //--------------------

            //P4 只存在主檔 , 無卡片資料時,改抓P4D的英文名稱.
            if (this.txtEName.Text == "")
            {
                if (htReturnP4D.ContainsKey("EMBOSSER_NAME_11") == true)
                {
                    this.txtEName.Text = htReturnP4D["EMBOSSER_NAME_11"].ToString();
                }
            }
            this.txtPostalDistrict.Text = htReturn["MANAGE_ZIP_CODE"].ToString();
            this.txtRemarkOne.Text = htReturn["MEMO_1"].ToString();
            this.txtRemarkTwo.Text = htReturn["MEMO_2"].ToString();
            this.txtBirthday.Text = htReturn["BIRTH_DATE"].ToString();

            base.sbRegScript.Append(BaseHelper.SetFocus("txtLName"));
            BindGridView();
        }
        else
        {
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            etMstType = eMstType.Select;
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end
            this.grvCardData.Visible = false;
            CommonFunction.SetControlsEnabled(pnlText, false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUserId"));
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/06
    /// 修改日期：2009/10/06 
    /// <summary>
    /// 提交事件
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        //Hashtable htReturn = (Hashtable)ViewState["HtgInfo"];
        base.strClientMsg = "";
        base.strHostMsg = "";
        bool blnP4 = (bool)ViewState["P4_Flag"];
        bool blnP4D = (bool)ViewState["P4D_Flag"];
        bool bSelChkP4 = (bool)ViewState["SelChkP4_Flag"];
        bool bSelChkP4D = (bool)ViewState["SelChkP4D_Flag"];


        //*modify by chaoma RQ-2010-005537-000 Start

        if (chkP4.Checked == false && chkP4D.Checked == false)
        {
            base.sbRegScript.Append("alert('請勾選 P4 或 P4D 主機環境');");
            return;
        }

        if ((chkP4.Checked != bSelChkP4) || (chkP4D.Checked != bSelChkP4D))
        {
            base.sbRegScript.Append("alert('查詢與提交，勾選的主機環境不一致，請重新勾選');");
            return;
        }


        //*modify by chaoma RQ-2010-005537-000 End
        bool bSucc = true;//*異動主機成功標識
        bool bAllSucss = true;
        bool bPCMH_P4 = true;
        bool bPCMH_P4D = true;
        bool bP4_JC99 = true;

        string sRemark1 = this.txtRemarkOne.Text.Trim();
        string sRemark2 = this.txtRemarkTwo.Text.Trim();

        if (BaseHelper.GetByteLength(sRemark1) > 52 || BaseHelper.GetByteLength(sRemark2) > 52)
        {
            base.sbRegScript.Append("alert('註記欄位長度過長，請重新輸入');");
            return;
        }

        string engname = this.txtEName.Text.Trim();
        //edit by james 20190806 英文名可包含數字
        //edit by mel 20141203 修改畫面上英文名字若有數字則不能新增
        //if (System.Text.RegularExpressions.Regex.Match(engname, "[0-9]").Success)
        //{
        //    base.sbRegScript.Append("alert('英文姓名不可有數字，請重新輸入');");
        //    return;
        //}
        if (engname.Length > 26)
        {
            base.sbRegScript.Append("alert('英文姓名長度不可超過26，請重新輸入');");
            return;
        }
        if (this.txtLName.Text.Length > 50)
        {
            base.sbRegScript.Append("alert('中文姓名長度不可超過50，請重新輸入');");
            return;
        }
        //*  2021/07/15 Joe 增設中文姓名檢核(不可含有數字0~9)
        string norrowName = ToNarrow(this.txtLName.Text);
        string norrowNameEngname = ToNarrow(this.txtEName.Text);
        if (Regex.IsMatch(norrowName, "[0-9]"))
        {
            base.sbRegScript.Append("alert('中文姓名欄位 不可輸入0~9數字，請重新輸入');");
            return;
        }
        //*  2021/10/05 Nash 增設英文姓名檢核(不可輸入連續7個數字)
        if (Regex.IsMatch(norrowNameEngname, "[0-9]{7}"))
        {
            base.sbRegScript.Append("alert('英文姓名欄位 不可輸入連續7個數字，請重新輸入');");
            return;
        }
        if (this.txtRoma.Text.Length > 50)
        {
            base.sbRegScript.Append("alert('羅馬拼音長度不可超過50，請重新輸入');");
            return;
        }
        if (this.ValidRoma(this.txtRoma.Text.Trim()) == false)
        {
            base.sbRegScript.Append("alert('有未符合規範的羅馬拼音字元，請重新輸入');");
            return;
        }

        //把長姓名的值放回原本的 姓名欄位
        string _lname = txtLName.Text.Trim();
        this.txtName.Text = (_lname.Length > 5) ? _lname.Substring(0, 5) : _lname;

        ArrayList arrayName = new ArrayList(new object[] { "NAME_1", "BIRTH_DATE", "ACCT_NBR", "MEMO_1", "MEMO_2", "MANAGE_ZIP_CODE" });

        if (blnP4)//*P4環境
        {
            Hashtable htReturnP4 = (Hashtable)ViewState["HtgInfoP4"];
            Hashtable htOutputP4 = new Hashtable();//*P4_PCTI主機異動HashTbale

            MainFrameInfo.ChangeJCF6toPCTI(htReturnP4, htOutputP4, arrayName);
            
            DataTable dtblUpdateP4 = PrepareLogFields(htOutputP4, "PCMC", "");
            htOutputP4.Add("FUNCTION_ID", "PCMC1");
           
            //*P4環境下異動主機資料
            if (UpdatePCMC(htOutputP4, HtgType.P4_PCTI, dtblUpdateP4, "P4"))
            {
                //*遍歷GridView中的資料，通過PCMH_P4更新主機資料
                bPCMH_P4 = UpdatePCMH(htReturnP4, HtgType.P4_PCTI, "P4");
                //-----------------------------
                if (bPCMH_P4)
                {
                    //更新 長姓名資料
                    bP4_JC99 = UpdateP4_JC99(htReturnP4);
                }
                //--------------------------------
                //*更新主機國旅卡資料
                UpdateP4_JCHO();
            }
            else
            {
                bSucc = false;
            }
        }

        if (blnP4D)//*P4D環境
        {
            Hashtable htReturnP4D = (Hashtable)ViewState["HtgInfoP4D"];
            Hashtable htOutputP4D = new Hashtable();//*P4D_PCTI主機異動HashTbale
            MainFrameInfo.ChangeJCF6toPCTI(htReturnP4D, htOutputP4D, arrayName);

            DataTable dtblUpdateP4D = PrepareLogFields(htOutputP4D, "PCMC", "");
            htOutputP4D.Add("FUNCTION_ID", "PCMC1");

            //*P4D環境下異動主機資料
            if (UpdatePCMC(htOutputP4D, HtgType.P4D_PCTI, dtblUpdateP4D, "P4D"))
            {
                //*遍歷GridView中的資料，通過PCMH_P4D更新主機資料
                bPCMH_P4D = UpdatePCMH(htReturnP4D, HtgType.P4D_PCTI, "P4D");
            }
            else
            {
                bSucc = false;
            }
        }

        if (bSucc && bPCMH_P4 && bPCMH_P4D && bP4_JC99)
        {
            if (blnP4)
            {
                if (!bPCMH_P4 || !bP4_JC99)
                {
                    bAllSucss = false;
                }                
            }
            if (blnP4D)
            {
                if (!bPCMH_P4D)
                {
                    bAllSucss = false;
                }
            }

            if (bAllSucss)
            {
                ClearFields();
                CommonFunction.SetControlsEnabled(pnlText, false);
            }
        }
        else
        {
            //CommonFunction.SetEnabled(pnlText, false);//*將網頁中的提交按鈕和輸入框disable  
        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtUserId"));
    }


    private void ClearFields()
    {
        this.txtUserId.Text = "";
        this.txtName.Text = "";
        this.txtEName.Text = "";
        this.txtPostalDistrict.Text = "";
        this.txtRemarkOne.Text = "";
        this.txtRemarkTwo.Text = "";
        this.txtBirthday.Text = "";
        this.gpList.Visible = false;
        this.grvCardData.Visible = false;
        this.gpList.RecordCount = 0;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/06
    /// 修改日期：2009/10/06 
    /// <summary>
    /// 分頁事件
    /// </summary>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    /// <summary>
    /// 自動翻譯
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnTranslation_Click(object sender, EventArgs e)
    {
        string strCName = txtLName.Text.Replace(" ", "");
        string strTransWord = "";
        string strTmp = "";
        string strTransFail = "";

        for (int i = 0; i < strCName.Length; i++)
        {
            strTmp = CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnablePropertyName("01", "23", strCName[i].ToString()).Trim();
            if (strTmp == "")
            {
                strTransFail = strTransFail + strCName[i].ToString() + "．";
            }
            else
            {
                strTransWord += " " + strTmp;
            }
        }
        if (strTransFail != "")
        {
            base.sbRegScript.Append("alert('英文姓名【 " + strTransFail + " 】翻譯不完全 或 無法翻譯');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtEName"));
        }
        //英文姓名的最大長度為26
        strTransWord = strTransWord.Trim();
        strTransWord = (strTransWord.Length > 26) ? strTransWord.Substring(0, 26) : strTransWord;
        txtEName.Text = strTransWord;
    }

    #endregion

    #region 方法區

    /// 作者 趙呂梁
    /// 創建日期：2009/10/06
    /// 修改日期：2009/10/06 
    private void SetControlsText()
    {
        grvCardData.Columns[0].HeaderText = BaseHelper.GetShowText("01_01010400_012");
        grvCardData.Columns[1].HeaderText = BaseHelper.GetShowText("01_01010400_013");
        grvCardData.Columns[2].HeaderText = BaseHelper.GetShowText("01_01010400_014");
        grvCardData.Columns[3].HeaderText = BaseHelper.GetShowText("01_01010400_015");
        grvCardData.Columns[4].HeaderText = BaseHelper.GetShowText("01_01010400_016");
        grvCardData.Columns[5].HeaderText = BaseHelper.GetShowText("01_01010400_017");
        grvCardData.Columns[6].HeaderText = BaseHelper.GetShowText("01_01010400_018");
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/06
    /// 修改日期：2009/10/06 
    /// <summary>
    /// 得到主機資料
    /// </summary>
    /// <returns>主機返回的HashTable</returns>
    private bool GetMainframeData(HtgType type, ref Hashtable htResult, DataTable dtblCard)
    {
        int iCount = 0;
        int iRCount = 0;
        int iNum = 0;
        int iBegin = 0;
        int iUpFlag = 0;
        string sUserID = CommonFunction.GetSubString(this.txtUserId.Text.Trim().ToUpper(), 0, 16);

        //*添加上傳主機信息
        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT_NBR", sUserID);
        htInput.Add("LINE_CNT", "0000");
        htInput.Add("FUNCTION_CODE", "1");

        string strType = "";//*查詢電文環境
        string strHtgCondition = "";//*電文環境(P4/P4D)
        if (type == HtgType.P4D_JCF6)
        {
            strType = "12";
            strHtgCondition = "P4D";
        }
        else
        {
            strType = "1";
            strHtgCondition = "P4";
        }
        //*得到主機傳回信息                
        //Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(type, htInput, false, strType, eAgentInfo);        
        Hashtable htReturn;
        if (isTest)
        {
            //測試用模擬資料
            using (BRHTG_JC99 obj = new BRHTG_JC99())
            {
                obj.PageCode = sPageInfo.strPageCode;
                //_tmpData = obj.getData(_tmpData, eAgentInfo);
                htReturn = obj.GetTestMainFrameInfo("P4_JCF6");
            }

        }
        else
        {
            //上送主機資料           
            htReturn = MainFrameInfo.GetMainFrameInfo(type, htInput, false, strType, eAgentInfo);
        }
        //--------------------------------------------------------------

        htResult = htReturn;
        if (!htReturn.Contains("HtgMsg"))
        {
            htResult["ACCT_NBR"] = htInput["ACCT_NBR"];

            if (htReturn["LINE_CNT"].ToString() != "")//*取得卡人資料筆數
            {
                iRCount = Convert.ToInt32(htReturn["LINE_CNT"].ToString());
            }
            iNum = iRCount - iCount * 18;
                        
            if (iNum <= 18)
            {
                iUpFlag = 1;
            }
            else
            {
                iNum = 18;
            }

            while (iNum > 0)
            {
                //*為網頁欄位(list)賦值
                for (int i = 1; i < iNum + 1; i++)
                {
                    if (htReturn.ContainsKey("SHORT_NAME" + i.ToString()))
                    {
                        //*插入卡人資料信息
                        DataRow drowRow = dtblCard.NewRow();
                        drowRow[0] = htReturn["SHORT_NAME" + i.ToString()].ToString();
                        drowRow[1] = htReturn["EMBOSSER_NAME_1" + i.ToString()].ToString();
                        drowRow[2] = htReturn["CARD_NMBR" + i.ToString()].ToString();
                        drowRow[3] = htReturn["CARD_ID" + i.ToString()].ToString();
                        drowRow[4] = htReturn["TYPE" + i.ToString()].ToString();
                        drowRow[5] = strHtgCondition;
                        drowRow[6] = "";
                        dtblCard.Rows.Add(drowRow);
                    }
                }

                if (iUpFlag == 1)
                {
                    return true;
                }

                iBegin = (iCount + 1) * 18 + 1;
                htInput["ACCT_NBR"] = sUserID;
                htInput["LINE_CNT"] = iBegin.ToString().PadLeft(4, '0');
                htInput["FUNCTION_CODE"] = "1";
                htInput["MESSAGE_TYPE"] = "";

                if (isTest)
                {
                    //測試用模擬資料                    
                    using (BRHTG_JC99 obj = new BRHTG_JC99(sPageInfo.strPageCode))
                    {
                        htReturn = obj.GetTestMainFrameInfo("P4_JCF6");
                    }
                }
                else
                {
                    //上送主機資料           
                    htReturn = MainFrameInfo.GetMainFrameInfo(type, htInput, false, strType, eAgentInfo);
                }

                if (htReturn.Contains("HtgMsg"))
                {
                    //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
                    etMstType = eMstType.Select;
                    //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

                    for (int j = 0; j < dtblCard.Rows.Count; j++)
                    {
                        //*主機查詢卡人資料失敗，移除該環境下的資料
                        if (strHtgCondition == dtblCard.Rows[j]["P4/P4D"].ToString().Trim())
                        {
                            dtblCard.Rows.RemoveAt(j);
                        }
                    }

                    return false;
                }
                else
                {
                    iCount++;
                    iRCount = Convert.ToInt32(htReturn["LINE_CNT"].ToString());
                    iNum = iRCount - iCount * 18;
                    if (iNum <= 18)
                    {
                        iUpFlag = 1;
                    }
                    else
                    {
                        iNum = 18;
                    }
                }
            }

            base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息
            return true;
        }
        else
        {

            return false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/06
    /// 修改日期：2009/10/06 
    /// <summary>
    /// 綁定卡人資料的主機信息
    /// </summary>
    /// <param name="htReturn">上傳主機返回的信息</param>
    private void BindGridView()
    {
        try
        {
            if (ViewState["DataBind"] != null)
            {
                DataTable dtblResult = (DataTable)ViewState["DataBind"];
                if (dtblResult.Rows.Count > 0)
                {
                    this.gpList.Visible = true;
                    this.grvCardData.Visible = true;
                    this.gpList.RecordCount = dtblResult.Rows.Count;
                    this.grvCardData.DataSource = CommonFunction.Pagination(dtblResult, this.gpList.CurrentPageIndex, this.gpList.PageSize);
                    this.grvCardData.DataBind();
                }
                else
                {
                    this.grvCardData.DataSource = CommonFunction.Pagination(dtblResult, this.gpList.CurrentPageIndex, this.gpList.PageSize);
                    this.grvCardData.DataBind();
                    this.gpList.Visible = false;
                    this.grvCardData.Visible = false;
                }
            }
        }
        catch (Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex, LogLayer.UI);
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/06
    /// 修改日期：2009/10/06 
    /// <summary>
    /// 得到存儲卡人資料主機信息的表結構
    /// </summary>
    /// <returns>存儲主機信息表</returns>
    public DataTable GetDataTable()
    {
        DataTable dtblCardData = new DataTable();
        dtblCardData.Columns.Add("CName");
        dtblCardData.Columns.Add("EName");
        dtblCardData.Columns.Add("CardNum");
        dtblCardData.Columns.Add("ID");
        dtblCardData.Columns.Add("Type");
        dtblCardData.Columns.Add("P4/P4D");
        dtblCardData.Columns.Add("Status");
        return dtblCardData;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/07
    /// 修改日期：2009/10/07
    /// <summary>
    /// 比對要上送 PCMC/PCMH 的欄位 , 有修改的欄位才送
    /// </summary>
    /// <param name="htReturn">主機返回信息的HashTable</param>
    /// <returns>DataTable(保存異動欄位信息)</returns>
    private DataTable PrepareLogFields(Hashtable htReturn, string TransType, string sPcmhIndex)
    {
        DataTable dtblUpdate = CommonFunction.GetDataTable();

        if (TransType == "PCMH")
        {
            //*比對英文姓名
            CommonFunction.ContrastDataEdit(htReturn, dtblUpdate, this.txtEName.Text.Trim(), "EMBOSSER_NAME_1" + sPcmhIndex, BaseHelper.GetShowText("01_01010400_005"));
        }
        else
        {
            //*比對姓名
            if (CommonFunction.ContrastDataEdit(htReturn, dtblUpdate, this.txtName.Text.Trim().ToUpper(), "NAME_LINE_1", BaseHelper.GetShowText("01_01010400_004")) == 1)
            {
                htReturn.Add("SHORT_NAME", this.txtName.Text.Trim().ToUpper());//*別名
            }
            
            //*比對管理郵區
            CommonFunction.ContrastDataEdit(htReturn, dtblUpdate, this.txtPostalDistrict.Text.Trim(), "MAIL_IND", BaseHelper.GetShowText("01_01010400_006"));

            //*比對註記一
            CommonFunction.ContrastDataEdit(htReturn, dtblUpdate, this.txtRemarkOne.Text.Trim(), "MEMO_1", BaseHelper.GetShowText("01_01010400_007"));

            //*比對註記二
            CommonFunction.ContrastDataEdit(htReturn, dtblUpdate, this.txtRemarkTwo.Text.Trim(), "MEMO_2", BaseHelper.GetShowText("01_01010400_008"));

            //*比對生日
            CommonFunction.ContrastDataEdit(htReturn, dtblUpdate, this.txtBirthday.Text.Trim(), "BIRTH_DATE", BaseHelper.GetShowText("01_01010400_009"));
        }

        return dtblUpdate;
    }


    /// 作者 趙呂梁
    /// 創建日期：2009/10/07
    /// 修改日期：2009/10/07
    /// <summary>
    /// 修改P4D或P4環境 卡人資料
    /// </summary>
    /// <param name="htInput">主機返回信息的HashTable</param>
    /// <param name="type">電文類型</param>
    /// <param name="dtblUpdate">修改主機欄位信息的DataTable</param>
    /// <param name="strLogFlag">操作標識</param>
    /// <returns>true成功，false失敗</returns>
    private bool UpdatePCMC(Hashtable htReturn, HtgType type, DataTable dtblUpdate, string strLogFlag)
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

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
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();

                    if (strLogFlag == "P4D")
                    {
                        base.strClientMsg += MessageHelper.GetMessage("01_01010400_003");
                    }
                    else
                    {
                        base.strClientMsg += MessageHelper.GetMessage("01_01010400_004");
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
                base.strClientMsg += MessageHelper.GetMessage("01_01010400_001");
            }
            else
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01010400_002");
            }
            return true;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/07
    /// 修改日期：2009/10/07
    /// <summary>
    /// 更新資料庫信息
    /// </summary>
    /// <param name="dtblUpdate">修改主機信息的DataTable</param>
    /// <param name="strLogFlag">操作標識</param>
    private void UpdateDataBase(DataTable dtblUpdate, string strLogFlag)
    {
        if (!BRTRANS_NUM.UpdateTransNum("A04"))
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
    /// 創建日期：2009/10/07
    /// 修改日期：2009/10/07
    /// <summary>
    /// 遍歷GridView中的資料，更新主機PCMH_P4D/PCMH資料
    /// </summary>
    /// <param name="type">電文類型</param>
    /// <param name="strHtgCondition">主機環境</param>
    private bool UpdatePCMH(Hashtable htReturn, HtgType type, string strHtgCondition)
    {
        DataTable dtblBand = (DataTable)ViewState["DataBind"];
        string sCName = this.txtName.Text.Trim();
        string sEName = this.txtEName.Text.Trim();
        bool sLogedFlag = false;
        bool bUpdatePCMHOK = true;
        int iPcmhIndex_P4 = 0;
        int iPcmhIndex_P4D = 0;

        for (int i = 0; i < dtblBand.Rows.Count; i++)
        {
            int inCount = 0;//*異動主機欄位數量
            Hashtable htInput = new Hashtable();
            htInput.Add("FUNCTION_ID", "PCMH1");
            if (dtblBand.Rows[i]["P4/P4D"].ToString().Trim() == "P4")
            {
                iPcmhIndex_P4++;
            }
            else if (dtblBand.Rows[i]["P4/P4D"].ToString().Trim() == "P4D")
            {
                iPcmhIndex_P4D++;
            }

            if (dtblBand.Rows[i]["ID"].ToString().Trim().ToUpper() == this.txtUserId.Text.Trim().ToUpper() && dtblBand.Rows[i]["P4/P4D"].ToString().Trim() == strHtgCondition)
            {
                htInput.Add("CARD_DATA", "822" + dtblBand.Rows[i]["Type"].ToString().Trim() + dtblBand.Rows[i]["CardNum"].ToString().Trim());
                if (sCName != dtblBand.Rows[i]["CName"].ToString().Trim())
                {
                    htInput.Add("SHORT_NAME", sCName);
                    inCount++;
                }

                if (sEName != "")
                {
                    if (sEName != dtblBand.Rows[i]["EName"].ToString().Trim())
                    {
                        htInput.Add("E1", sEName);
                        inCount++;
                    }
                }

                if (inCount > 0)//*有無異動
                {
                    Hashtable htResult = MainFrameInfo.GetMainFrameInfo(type, htInput, false, "2", eAgentInfo);
                    if (!htResult.Contains("HtgMsg"))
                    {
                        dtblBand.Rows[i]["CName"] = sCName;
                        dtblBand.Rows[i]["EName"] = sEName;
                        dtblBand.Rows[i]["Status"] = "異動完成";
                        if (!sLogedFlag)
                        {
                            if (strHtgCondition == "P4")
                            {
                                DataTable dtblUpdatePCMH = PrepareLogFields(htReturn, "PCMH", iPcmhIndex_P4.ToString());
                                UpdateDataBase(dtblUpdatePCMH, strHtgCondition);
                            }
                            else if (strHtgCondition == "P4D")
                            {
                                DataTable dtblUpdatePCMH = PrepareLogFields(htReturn, "PCMH", iPcmhIndex_P4D.ToString());
                                UpdateDataBase(dtblUpdatePCMH, strHtgCondition);
                            }
                            sLogedFlag = true;
                        }
                    }
                    else
                    {
                        switch (htResult["ERR_MSG_DATA"].ToString())
                        {
                            case "00301":
                                dtblBand.Rows[i]["Status"] = "ORG/TYPE 不存在";
                                break;
                            case "00303":
                                dtblBand.Rows[i]["Status"] = "卡片類別錯誤";
                                break;
                            case "00304":
                                dtblBand.Rows[i]["Status"] = "卡號不存在";
                                break;
                            case "00305":
                                dtblBand.Rows[i]["Status"] = "資料已存在";
                                break;
                            case "00307":
                                dtblBand.Rows[i]["Status"] = "已換卡";
                                break;
                            case "00310":
                                dtblBand.Rows[i]["Status"] = "主機關檔維護中";
                                break;
                            case "00311":
                                dtblBand.Rows[i]["Status"] = "無調整額度的權限";
                                break;
                            default:
                                dtblBand.Rows[i]["Status"] = "其他錯誤，異動失敗";
                                break;
                        }

                        bUpdatePCMHOK = false;
                    }
                }

            }
        }
        this.grvCardData.DataSource = dtblBand;
        this.grvCardData.DataBind();
        if (bUpdatePCMHOK)
        {
            base.strClientMsg = MessageHelper.GetMessage("01_00000000_014");
        }
        else
        {
            base.strClientMsg = MessageHelper.GetMessage("01_00000000_014") + "，修改結果，請參考 STATUS 欄位";
        }

        return bUpdatePCMHOK;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/07
    /// 修改日期：2009/10/07
    /// <summary>
    /// 更新主機國旅卡資料
    /// </summary>
    private bool UpdateP4_JCHO()
    {
        //*添加上傳主機信息
        Hashtable htInput = new Hashtable();
        htInput.Add("ID", CommonFunction.GetSubString(this.txtUserId.Text.Trim().ToUpper(), 0, 11));
        htInput.Add("FUNCTION_CODE", "I");

        //*得到主機傳回信息
        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCHO, htInput, false, "1", eAgentInfo);

        if (!htReturn.Contains("HtgMsg"))
        {
            htReturn["ID"] = htInput["ID"];//* for_xml_test
            htReturn["MESSAGE_TYPE"] = "";

            DataTable dtblUpdate = CommonFunction.GetDataTable();
            //*比對姓名
            CommonFunction.ContrastData(htReturn, dtblUpdate, this.txtName.Text.Trim(), "NAME", "公務人員姓名");

            if (dtblUpdate.Rows.Count > 0)
            {
                htReturn["FUNCTION_CODE"] = "U";
                Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCHO, htReturn, false, "2", eAgentInfo);
                if (!htResult.Contains("HtgMsg"))
                {
                    base.strClientMsg += HtgType.P4_JCHO.ToString() + MessageHelper.GetMessage("01_00000000_014");
                    base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                    UpdateDataBase(dtblUpdate, "P4");
                    return true;
                }
                else
                {
                    //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
                    etMstType = eMstType.Control;
                    //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end
                    if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                    {
                        base.strHostMsg += htResult["HtgMsg"].ToString();
                        base.strClientMsg += MessageHelper.GetMessage("01_00000000_027");
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
                //base.strClientMsg+= HtgType.P4_JCHO + ":" + MessageHelper.GetMessage("01_00000000_015");
                return true;
            }
        }
        else
        {
            if (htReturn["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                if (htReturn["MESSAGE_TYPE"].ToString() != "8888")
                {
                    base.strHostMsg += htReturn["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_045");
                }
                else
                {
                    //etMstType = eMstType.NoAlert;
                }
            }
            else
            {
                base.strClientMsg += htReturn["HtgMsg"].ToString();
            }
            return false;
        }
    }

    private bool UpdateP4_JC99(Hashtable htReturn)
    {
        bool _result = false;        

        //*更新主機長姓名資料(JC99)                
        using (BRHTG_JC99 obj = new BRHTG_JC99(sPageInfo.strPageCode))
        {
            EntityHTG_JC99 _jc99Info = new EntityHTG_JC99();
            _jc99Info.NAME = txtLName.Text;
            _jc99Info.ROMA = this.txtRoma.Text;
            _jc99Info.IDNO_NO = CommonFunction.GetSubString(this.txtUserId.Text.Trim().ToUpper(), 0, 16);
            //只要原電文值的長姓名flag=true ,其function_code 就是 C,若不是且 (中文長姓名長度大於5 or 羅馬拼音非空白時) 則是 A,否則就是空白不處理
            _jc99Info.FUNCTION_CODE = obj.getFunctionCode(htReturn, _jc99Info);
            if (string.IsNullOrEmpty(_jc99Info.FUNCTION_CODE) == false)
            {
                _jc99Info.IN_CFLAG = "Y";

                EntityResult _JC99result = obj.Update(_jc99Info, eAgentInfo);
                //異常時
                if (_JC99result.Success)
                {
                    DataTable dtblUpdate = CommonFunction.GetDataTable();
                    //*比對差異(長姓名和羅馬拼音)
                    if (htReturn.ContainsKey("LNAME"))
                    {
                        CommonFunction.ContrastDataEdit(htReturn, dtblUpdate, _jc99Info.NAME, "LNAME", BaseHelper.GetShowText("01_01010400_021"));
                    }
                    if (htReturn.ContainsKey("ROMA"))
                    {
                        CommonFunction.ContrastData(htReturn, dtblUpdate, _jc99Info.ROMA, "ROMA", BaseHelper.GetShowText("01_01010400_022"));
                    }
                    if (dtblUpdate.Rows.Count > 0)
                    {
                        UpdateDataBase(dtblUpdate, "P4");
                        //_result = true;
                    }
                    _result = true;
                }
                else
                {
                    base.strClientMsg = string.Format("更新JC99時發生異常:{0}", _JC99result.HostMsg);
                    Logging.Log(base.strClientMsg, LogState.Error, LogLayer.BusinessRule);
                }
            }
            else
            {
                _result = true;
            }
        }
           
        return _result;
    }
    #endregion

}
