//******************************************************************
//*  作    者：趙呂梁

//*  功能說明：卡片資料異動-毀補二KEY

//*  創建日期：2009/08/18
//*  修改記錄：


//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
//20171020 (U) by Tank, PA值若為3，顯示訊息

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
using System.Text;
using CSIPCommonModel.EntityLayer;

public partial class P010102070001 : PageBase
{
    #region 變數區
    /// <summary>
    /// 一KEY標識
    /// </summary>
    private string m_OneKeyFlag = "1";

    /// <summary>
    /// 二KEY標識
    /// </summary>
    private string m_TwoKeyFlag = "2";

    /// <summary>
    /// 補發種類
    /// </summary>
    private string m_ReIssueType = "2";

    /// <summary>
    /// 上傳主機標識
    /// </summary>
    private string m_UploadFlag = "Y";

    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #endregion

    #region 事件區
    /// <summary>
    /// 頁面加載事件
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
            LoadDropDownList();
            CommonFunction.SetControlsEnabled(pnlText, false);
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"]; //*記錄網頁訊息
        CommonFunction.SetControlsForeColor(this.pnlText, Color.Black);
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/08/18
    /// 修改日期：2009/08/18 
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
        //*獲取主機資料
        if (GetMainframeData())
        {
            //*獲取主機資料成功
            //*查詢一Key資料
            EntitySet<EntityREISSUE_CARD> eReissueCardSet = null;

            try
            {
                eReissueCardSet = BRREISSUE_CARD.SelectEntitySet(this.txtCardNo.Text.Trim(), m_OneKeyFlag, m_UploadFlag, m_ReIssueType);
            }
            catch
            {

                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                }
                return;

            }

            if (eReissueCardSet.Count > 0)
            {
                //*判斷查詢的結果user_id欄位是否和Session["UserId"]相同
                if (eReissueCardSet.GetEntity(0).user_id.Trim() == eAgentInfo.agent_id)
                {
                    //base.strClientMsg += MessageHelper.GetMessage("01_00000000_008");
                    base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_00000000_008") + "');");
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
                    CommonFunction.SetEnabled(pnlText, false);
                    ClearControlsText();
                }
                else
                {
                    //*查詢二Key資料
                    EntitySet<EntityREISSUE_CARD> eReissueCardSetTwoKey = null;
                    try
                    {
                        eReissueCardSetTwoKey = BRREISSUE_CARD.SelectEntitySet(this.txtCardNo.Text.Trim(), m_TwoKeyFlag, m_UploadFlag, m_ReIssueType);
                    }
                    catch
                    {

                        if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                        {
                            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                        }
                        return;

                    }

                    if (eReissueCardSetTwoKey.Count > 0)
                    {
                        //*將資料庫欄位賦值給畫面中的【製卡英文姓名】、【取卡方式】、【會員編號】、【製卡式樣】
                        this.txtEnglishName.Text = eReissueCardSetTwoKey.GetEntity(0).Eng_Name;
                        this.txtCardFashion.Text = eReissueCardSetTwoKey.GetEntity(0).GetCard_code;
                        this.txtMemberNo.Text = eReissueCardSetTwoKey.GetEntity(0).Member_No;
                        this.txtCardPattern.Text = eReissueCardSetTwoKey.GetEntity(0).Card_Type;
                    }
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtEnglishName"));
                }
            }
            else
            {
                //base.strClientMsg += MessageHelper.GetMessage("01_00000000_007");
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_00000000_007") + "');");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
                CommonFunction.SetEnabled(pnlText, false);
            }
        }
        else
        {
            CommonFunction.SetControlsEnabled(pnlText, false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/08/18
    /// 修改日期：2009/08/18 
    /// <summary>
    /// 提交事件
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string strCardNo = "";
        //*製卡式樣:僅能接受原帶出值為「00」者,改為「01」..餘出ERR(顯示”製卡式樣除00改01外,餘不可異動,請確認!” ; 長度限制:2碼數值
        if (ViewState["CardPattern"].ToString().Trim() != this.txtCardPattern.Text.Trim())
        {
            if (!(ViewState["CardPattern"].ToString().Trim() == "00" && this.txtCardPattern.Text.Trim() == "01"))
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01020600_001");
                return;
            }
        }
        EntitySet<EntityREISSUE_CARD> eReissueCardSet = null;
        try
        {
            eReissueCardSet = BRREISSUE_CARD.SelectEntitySet(this.txtCardNo.Text.Trim(), m_TwoKeyFlag, m_UploadFlag, m_ReIssueType);
        }
        catch
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            }
            return;
        }

        if (eReissueCardSet.Count > 0)
        {
            strCardNo = eReissueCardSet.GetEntity(0).Card_No;
        }

        EntityREISSUE_CARD eReissueCard = new EntityREISSUE_CARD();
        eReissueCard.Eng_Name = this.txtEnglishName.Text.Trim();
        eReissueCard.GetCard_code = this.txtCardFashion.Text.Substring(0, 1).Trim();
        eReissueCard.Member_No = this.txtMemberNo.Text.Trim();
        eReissueCard.user_id = eAgentInfo.agent_id;
        eReissueCard.mod_date = DateTime.Now.ToString("yyyyMMdd");
        eReissueCard.Card_Type = this.txtCardPattern.Text.Trim();

        string[] strColumns = { EntityREISSUE_CARD.M_Eng_Name, EntityREISSUE_CARD.M_GetCard_code, EntityREISSUE_CARD.M_Member_No, EntityREISSUE_CARD.M_user_id, EntityREISSUE_CARD.M_mod_date, EntityREISSUE_CARD.M_Card_Type };
        if (strCardNo != "")
        {
            //*資料庫中已存在一筆二Key資料，更新Table ReIssue_Card
            if (BRREISSUE_CARD.Update(eReissueCard, this.txtCardNo.Text.Trim(), m_TwoKeyFlag, m_UploadFlag, m_ReIssueType, strColumns) == false)
            {
                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_021")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_021");
                }
                return;
            }
        }
        else
        {
            //*資料庫中沒有二Key資料，寫入Table ReIssue_Card
            eReissueCard.Card_No = this.txtCardNo.Text.Trim();
            eReissueCard.ReIssue_Type = m_ReIssueType;
            eReissueCard.KeyIn_Flag = m_TwoKeyFlag;
            eReissueCard.Upload_Flag = "";
            try
            {
                BRREISSUE_CARD.AddEntity(eReissueCard);
            }
            catch
            {
                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_020")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_020");
                }
                return;
            }
        }

        //*比較資料庫里的一Key資料是否和畫面中的輸入欄位一樣
        if (CompareKey())
        {
            UpdateHtgData();
        }
    }

    #endregion

    #region 方法區

    /// 作者 趙呂梁
    /// 創建日期：2009/08/18
    /// 修改日期：2009/08/18 
    /// <summary>
    /// 加載DropDownList
    /// </summary>
    private void LoadDropDownList()
    {
        DataTable dtblResult = new DataTable();
        if (BaseHelper.GetCommonProperty("01", "10", ref dtblResult))
        {
            for (int i = 0; i < dtblResult.Rows.Count; i++)
            {
                ListItem listItem = new ListItem();
                listItem.Text = dtblResult.Rows[i][0].ToString() + "     " + dtblResult.Rows[i][1].ToString();
                listItem.Value = dtblResult.Rows[i][0].ToString();
                this.dropCardFashion.Items.Add(listItem);
            }
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/08/18
    /// 修改日期：2009/08/18 
    /// <summary>
    /// 清空控件文本內容
    /// </summary>
    /// <param name="blnEnabled">True可用，false不可用</param>
    private void ClearControlsText()
    {
        this.lblChineseNameText.Text = "";
        this.lblPA1.Text = "";
        this.lblPA2.Text = "";
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/08/18
    /// 修改日期：2009/08/18 
    /// <summary>
    /// 得到上傳主機的信息
    /// </summary>
    /// <returns>主機信息的HashTable</returns>
    private Hashtable GetUploadMainframeData()
    {
        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT_NBR", this.txtCardNo.Text.Trim());
        htInput.Add("FUNCTION_CODE", "I");
        htInput.Add("LINE_CNT", "0000");
        return htInput;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/08/18
    /// 修改日期：2009/08/18 
    /// <summary>
    /// 得到主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool GetMainframeData()
    {
        string strMsg = "";//*主機返回錯誤信息
        string sACCT_NBR = ""; //卡人 ID
        string strCardNo = this.txtCardNo.Text.Trim();
        Hashtable htResult = new Hashtable();//*P4_JCAA主機傳回信息
        Hashtable htJCATResult = new Hashtable();//*P4_JCAT主機傳回信息
        Hashtable htInput = GetUploadMainframeData();
        Hashtable htOutput = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCEM, htInput, false, "1", eAgentInfo);
        ClearControlsText();
        if (!htOutput.Contains("HtgMsg"))
        {
            //htOutput["ACCT_NBR"] = htInput["ACCT_NBR"];//* for_xml_test  
            CommonFunction.SetControlsEnabled(pnlText, true);
            this.lblPA1.Text = htOutput["PREV_ACTION"].ToString();//*Layout中無PA
            this.lblPA2.Text = htOutput["DTE_LST_REQ"].ToString();//*Date Last REQ
            this.lblChineseNameText.Text = htOutput["CARDHOLDER_NAME"].ToString(); //*制卡中文姓名
            this.txtEnglishName.Text = htOutput["EMBOSS_NAME"].ToString();//*製卡英文姓名
            this.txtCardPattern.Text = htOutput["EMBOSS_TYPE"].ToString();//*製卡式樣
            ViewState["CardPattern"] = htOutput["EMBOSS_TYPE"].ToString();
            this.txtCardFashion.Text = htOutput["CARD_TAKE"].ToString();//*取卡方式
            if (this.txtCardFashion.Text == "")
            {
                this.txtCardFashion.Text = "0";
            }
            this.txtMemberNo.Text = htOutput["MEM_NO"].ToString();//*會員編號
            ViewState["HtgInfo"] = htOutput;
            ViewState["HtgJCEM"] = "Y";
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_012");
            base.strHostMsg += htOutput["HtgSuccess"].ToString();//*主機返回成功訊息

            //20171020 (U) by Tank, PA值若為3且毀補日<30天，顯示訊息  PA:0~當日毀補  PA:3~近日內曾毀補
            DateTime dtPA2 = DateTime.ParseExact(this.lblPA2.Text, "MMddyyyy", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
            TimeSpan tsDay = DateTime.Now - dtPA2;
            int dayCount = (int)tsDay.TotalDays;

            if (this.lblPA1.Text == "3" && dayCount < 30)
            {
                base.sbRegScript.Append("alert('PA值為3(近日內曾毀補)，請確認是否要執行!'); ");
            }

            return true;
        }
        else
        {
            if (htOutput["MESSAGE_TYPE"].ToString() == "6019") //若查無資料,則Call JCAT 取得畫面相關資訊
            {
                //根據卡號 , 取得卡人 ID
                if (CommonFunction.GetJCAAMainframeData(strCardNo, ref htResult, eAgentInfo, ref strMsg))
                {
                    sACCT_NBR = htResult["ACCT_NBR"].ToString(); //卡人ID
                    //發 JCAT 電文, 取得畫面相關資訊
                    if (CommonFunction.GetJCATMainframeData(sACCT_NBR, strCardNo, ref htJCATResult, eAgentInfo, ref strMsg))
                    {
                        CommonFunction.SetControlsEnabled(pnlText, true);

                        //20171020 (U) by Tank, add 取第一次發查的值
                        if (htOutput.Contains("PREV_ACTION"))
                        {
                            this.lblPA1.Text = htOutput["PREV_ACTION"].ToString();//*Layout中無PA
                        }
                        if (htOutput.Contains("DTE_LST_REQ"))
                        {
                            this.lblPA2.Text = htOutput["DTE_LST_REQ"].ToString();//*Date Last REQ
                        }

                        this.lblChineseNameText.Text = htJCATResult["CARD_NAME"].ToString(); //*制卡中文姓名
                        this.txtEnglishName.Text = htJCATResult["EMBOSSER_NAME"].ToString();//*製卡英文姓名
                        this.txtCardPattern.Text = htJCATResult["CM_CARD_TYPE"].ToString();//*製卡式樣
                        ViewState["CardPattern"] = htJCATResult["CM_CARD_TYPE"].ToString();
                        this.txtCardFashion.Text = "0";//*取卡方式
                        this.txtMemberNo.Text = htJCATResult["MEMBER_NO"].ToString();//*會員編號
                        ViewState["HtgInfo"] = htJCATResult;
                        ViewState["HtgJCEM"] = "N";
                        base.strClientMsg = MessageHelper.GetMessage("01_00000000_012");
                        base.strHostMsg = htJCATResult["HtgSuccess"].ToString();//*主機返回成功訊息

                        //20171020 (U) by Tank, PA值若為3且毀補日<30天，顯示訊息  PA:0~當日毀補  PA:3~近日內曾毀補
                        DateTime dtPA2 = DateTime.ParseExact(this.lblPA2.Text, "MMddyyyy", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
                        TimeSpan tsDay = DateTime.Now - dtPA2;
                        int dayCount = (int)tsDay.TotalDays;

                        if (this.lblPA1.Text == "3" && dayCount < 30)
                        {
                            base.sbRegScript.Append("alert('PA值為3(近日內曾毀補)，請確認是否要執行!'); ");
                        }

                        return true;
                    }
                    else
                    {
                        etMstType = eMstType.Select;
                        base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));

                        if (htJCATResult["HtgMsgFlag"].ToString() == "0")
                        {
                            base.strHostMsg = strMsg;
                            base.strClientMsg = MessageHelper.GetMessage("01_00000000_026");
                        }
                        else
                        {
                            base.strClientMsg = strMsg;
                        }
                        return false;
                    }

                }
                else
                {
                    etMstType = eMstType.Select;
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));

                    if (htResult["HtgMsgFlag"].ToString() == "0")
                    {
                        base.strHostMsg = strMsg;
                        base.strClientMsg = MessageHelper.GetMessage("01_00000000_026");
                    }
                    else
                    {
                        base.strClientMsg = strMsg;
                    }
                    return false;
                }
            }
            else
            {
                etMstType = eMstType.Select;
                base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
                if (htOutput["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg = htOutput["HtgMsg"].ToString();
                    base.strClientMsg = MessageHelper.GetMessage("01_00000000_026");
                }
                else
                {
                    base.strClientMsg = htOutput["HtgMsg"].ToString();
                }
                return false;
            }
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/08/18
    /// 修改日期：2009/08/18 
    /// <summary>
    /// 比較一、二KEY資料是否相同
    /// </summary>
    /// <returns>true 相同，false 不同</returns>
    private bool CompareKey()
    {
        bool blnSame = true;
        EntitySet<EntityREISSUE_CARD> eReissueCardSet = null;
        try
        {
            eReissueCardSet = BRREISSUE_CARD.SelectEntitySet(this.txtCardNo.Text.Trim(), m_OneKeyFlag, m_UploadFlag, m_ReIssueType);
        }
        catch
        {

            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            }
            return false;

        }
        if (eReissueCardSet.Count > 0)
        {
            int intCount = 0;//*記錄不相同的數量
            //*比較資料庫里的一Key資料是否和畫面中的輸入欄位一樣

            //*比較【製卡英文姓名】
            if (eReissueCardSet.GetEntity(0).Eng_Name.Trim() != this.txtEnglishName.Text.Trim())
            {
                intCount++;
                if (intCount == 1)
                {
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtEnglishName"));
                }
                this.txtEnglishName.ForeColor = Color.Red;
                blnSame = false;
            }

            //*比較【取卡方式】
            if (eReissueCardSet.GetEntity(0).GetCard_code.Trim() != this.txtCardFashion.Text.Substring(0, 1).Trim())
            {
                intCount++;
                if (intCount == 1)
                {
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtCardFashion"));
                }
                this.txtCardFashion.ForeColor = Color.Red;
                blnSame = false;
            }


            //*比較【製卡式樣】

            if (eReissueCardSet.GetEntity(0).Card_Type.Trim() != this.txtCardPattern.Text.Trim())
            {
                intCount++;
                if (intCount == 1)
                {
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtCardPattern"));
                }
                this.txtCardPattern.ForeColor = Color.Red;
                blnSame = false;
            }

            //*比較【會員編號】
            if (eReissueCardSet.GetEntity(0).Member_No.Trim() != this.txtMemberNo.Text.Trim())
            {
                intCount++;
                if (intCount == 1)
                {
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtMemberNo"));
                }
                this.txtMemberNo.ForeColor = Color.Red;
                blnSame = false;
            }

        }

        if (blnSame == false)
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_009");
        }
        return blnSame;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/08/18
    /// 修改日期：2009/08/18 
    /// <summary>
    /// 更新ReIssue_Card資料庫Upload_Flag、mod_date 欄位
    /// </summary>
    private void UpdateUploadFlag()
    {
        EntityREISSUE_CARD eReissueCard = new EntityREISSUE_CARD();
        eReissueCard.Upload_Flag = m_UploadFlag;
        eReissueCard.mod_date = DateTime.Now.ToString("yyyyMMdd");
        string[] strColumns = { EntityREISSUE_CARD.M_Upload_Flag, EntityREISSUE_CARD.M_mod_date };
        if (BRREISSUE_CARD.Update(eReissueCard, this.txtCardNo.Text.Trim(), m_UploadFlag, m_ReIssueType, strColumns) == false)
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_021")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_021");
            }
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/08/18
    /// 修改日期：2009/08/18 
    /// <summary>
    /// 修改主機資料
    /// </summary>
    private void UpdateHtgData()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end
        string strCardNo = this.txtCardNo.Text.Trim();
        Hashtable htOutputData = (Hashtable)ViewState["HtgInfo"];//*P4_JCEM或P4_JCAT主機查詢資料
        DataTable dtblUpdateData = CommonFunction.GetDataTable();
        Hashtable htOutput = new Hashtable();
        if (ViewState["HtgJCEM"].ToString().Trim() == "Y")//*判斷是由P4_JCEM或P4_JCAT獲取資料，以便組JCAW電文
        {
            ArrayList arrayName = new ArrayList(new object[] { "ACCT_NBR", "CARD_TAKE", "EMBOSS_NAME", "EMBOSS_TYPE", "MEM_NO", "EXPIRE_DATE" });//*P4_JCEM欄位名稱
            MainFrameInfo.ChangeJCEMtoJCAW(htOutputData, htOutput, arrayName);
        }
        else
        {
            ArrayList arrayName = new ArrayList(new object[] { "CARD_NMBR", "EMBOSSER_NAME", "CM_CARD_TYPE", "MEMBER_NO", "EXPIR_DATE" });//*P4_JCAT欄位名稱
            MainFrameInfo.ChangeJCATtoJCAW(htOutputData, htOutput, arrayName);
            htOutput.Add("SELF_TAKE", "");
        }
        //*修改主機損毀類型   
        htOutput.Add("FUNCTION_CODE", "2");
        htOutput.Add("LINE_CNT", "0000");
        string strCardFashion = "";
        if (this.txtCardFashion.Text.Trim().Substring(0, 1) == "0")
        {
            //*若畫面中的【取卡方式】的第一位為"0"，上傳主機<取卡方式>為""
            strCardFashion = "";
        }
        else
        {
            //*若畫面中的【取卡方式】的第一位不為"0"，上傳主機<取卡方式>為【取卡方式】的第一位數
            strCardFashion = this.txtCardFashion.Text.Trim().Substring(0, 1);
        }
        DataRow drowRow = dtblUpdateData.NewRow();
        drowRow[EntityCUSTOMER_LOG.M_field_name] = BaseHelper.GetShowText("01_01020700_006");
        drowRow[EntityCUSTOMER_LOG.M_before] = htOutput["SELF_TAKE"].ToString();
        drowRow[EntityCUSTOMER_LOG.M_after] = strCardFashion;
        dtblUpdateData.Rows.Add(drowRow);
        //*取卡方式
        htOutput["SELF_TAKE"] = strCardFashion;

        //*比對會員編號
        CommonFunction.ContrastData(htOutput, dtblUpdateData, this.txtMemberNo.Text.Trim(), "MEMNO", BaseHelper.GetShowText("01_01020700_008"));

        //*比對製卡英文姓名
        CommonFunction.ContrastData(htOutput, dtblUpdateData, this.txtEnglishName.Text.Trim(), "EMBNAME", BaseHelper.GetShowText("01_01020700_005"));

        //*比對制卡式樣
        CommonFunction.ContrastData(htOutput, dtblUpdateData, this.txtCardPattern.Text.Trim(), "EMBTYPE", BaseHelper.GetShowText("01_01020700_007"));
        if (dtblUpdateData.Rows.Count > 0)
        {
            //*提交修改主機資料
            Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCAW, htOutput, false, "2", eAgentInfo);

            if (!htReturn.Contains("HtgMsg"))
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01020700_003");
                base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息
                drowRow = dtblUpdateData.NewRow();
                drowRow[EntityCUSTOMER_LOG.M_field_name] = MessageHelper.GetMessage("01_01020700_004");
                drowRow[EntityCUSTOMER_LOG.M_before] = this.txtCardNo.Text.Trim();
                drowRow[EntityCUSTOMER_LOG.M_after] = MessageHelper.GetMessage("01_01020700_005") + this.txtCardNo.Text.Trim();
                dtblUpdateData.Rows.Add(drowRow);
                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtCardNo.Text.Trim(), "P4", (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }
                if (BRTRANS_NUM.UpdateTransNum("B10") == false)
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }
                UpdateUploadFlag();
                CommonFunction.SetControlsEnabled(pnlText, false);
                this.txtCardNo.Text = "";
                ClearControlsText();
                base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));

            }
            else
            {
                if (htReturn["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htReturn["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_027");
                }
                else
                {
                    base.strClientMsg += htReturn["HtgMsg"].ToString();
                }
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_015");
        }
    }
    #endregion
}
