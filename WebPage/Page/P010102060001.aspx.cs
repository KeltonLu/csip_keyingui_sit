//******************************************************************
//*  作    者：趙呂梁


//*  功能說明：卡片資料異動-毀補一KEY

//*  創建日期：2009/08/12
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
using CSIPCommonModel.EntityLayer;

public partial class P010102060001 : PageBase
{
    #region 變數區
    /// <summary>
    /// 一KEY標識
    /// </summary>
    private string m_OneKeyFlag = "1";

    /// <summary>
    /// 補發種類
    /// </summary>
    private string m_ReIssueType = "2";

    /// <summary>
    ///上傳主機標識
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
            LoadDropDownList();
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
            CommonFunction.SetControlsEnabled(pnlText, false);
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"]; //*記錄網頁訊息
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/08/12
    /// 修改日期：2009/08/12 
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
            //*獲取DB 一KEY 資料
            EntitySet<EntityREISSUE_CARD> eReissueCardSet = null;
            try
            {
                eReissueCardSet = BRREISSUE_CARD.SelectEntitySet(this.txtCardNo.Text.Trim(), m_OneKeyFlag, m_UploadFlag, m_ReIssueType);
            }
            catch (Exception exc)
            {
                base.strClientMsg = MessageHelper.GetMessage("00_00000000_000") + exc;
                return;
            }


            if (eReissueCardSet.Count > 0)
            {
                this.txtEnglishName.Text = eReissueCardSet.GetEntity(0).Eng_Name;
                this.txtCardFashion.Text = eReissueCardSet.GetEntity(0).GetCard_code;
                this.txtMemberNo.Text = eReissueCardSet.GetEntity(0).Member_No;
                this.txtCardPattern.Text = eReissueCardSet.GetEntity(0).Card_Type;
            }

            base.sbRegScript.Append(BaseHelper.SetFocus("txtEnglishName"));
        }
        else
        {
            CommonFunction.SetControlsEnabled(pnlText, false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
        }
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/08/12
    /// 修改日期：2009/08/12 
    /// <summary>
    /// 提交事件
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string strCardNo = "";
        string sCardPattern = this.txtCardPattern.Text.Trim();
        //*檢核【製卡式樣】

        //*製卡式樣:僅能接受原帶出值為「00」者,改為「01」..餘出ERR(顯示”製卡式樣除00改01外,餘不可異動,請確認!” ; 長度限制:2碼數值
        if (ViewState["CardPattern"].ToString().Trim() != sCardPattern)
        {
            if (!(ViewState["CardPattern"].ToString().Trim() == "00" && sCardPattern == "01"))
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01020600_001");
                return;
            }
        }

        //*查詢一Key資料
        EntitySet<EntityREISSUE_CARD> eReissueCardSet = null;
        try
        {
            eReissueCardSet = BRREISSUE_CARD.SelectEntitySet(this.txtCardNo.Text.Trim(), m_OneKeyFlag, m_UploadFlag, m_ReIssueType);
        }
        catch (Exception exc)
        {
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000") + exc;
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
        eReissueCard.Card_Type = sCardPattern;

        string[] strColumns = { EntityREISSUE_CARD.M_Eng_Name, EntityREISSUE_CARD.M_GetCard_code, EntityREISSUE_CARD.M_Member_No, EntityREISSUE_CARD.M_user_id, EntityREISSUE_CARD.M_mod_date, EntityREISSUE_CARD.M_Card_Type };

        if (strCardNo != "")
        {
            //*資料庫中已存在一筆一Key資料，更新Table ReIssue_Card
            if (BRREISSUE_CARD.Update(eReissueCard, this.txtCardNo.Text.Trim(), m_OneKeyFlag, m_UploadFlag, m_ReIssueType, strColumns) == false)
            {
                base.strClientMsg = MessageHelper.GetMessage("01_00000000_021");
                return;
            }
        }
        else
        {
            //*資料庫中沒有一Key資料，寫入Table ReIssue_Card
            eReissueCard.Card_No = this.txtCardNo.Text.Trim();
            eReissueCard.ReIssue_Type = m_ReIssueType;
            eReissueCard.KeyIn_Flag = m_OneKeyFlag;
            eReissueCard.Upload_Flag = "";
            try
            {
                BRREISSUE_CARD.AddEntity(eReissueCard);
            }
            catch (Exception exc)
            {

                base.strClientMsg = MessageHelper.GetMessage("01_00000000_020") + exc;
                return;

            }
        }
        base.strClientMsg = MessageHelper.GetMessage("01_00000000_013");
        CommonFunction.SetControlsEnabled(pnlText, false);
        this.txtCardNo.Text = "";
        ClearControlsText();
        base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
    }
    #endregion

    #region 方法區
    /// 作者 趙呂梁

    /// 創建日期：2009/08/12
    /// 修改日期：2009/08/12 
    /// <summary>
    /// 加載下拉列表
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

    /// 創建日期：2009/08/12
    /// 修改日期：2009/08/12 
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

    /// 創建日期：2009/08/12
    /// 修改日期：2009/08/12 
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

        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT_NBR", strCardNo);
        htInput.Add("FUNCTION_CODE", "I");
        htInput.Add("LINE_CNT", "0000");
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
            base.strClientMsg = MessageHelper.GetMessage("01_00000000_012");
            base.strHostMsg = htOutput["HtgSuccess"].ToString();//*主機返回成功訊息

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
                        //this.lblPA1.Text = htJCATResult["PREV_ACTION"].ToString();//*Layout中無PA
                        //this.lblPA2.Text = htJCATResult["DTE_LST_REQ"].ToString();//*Date Last REQ

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
    #endregion
}
