//******************************************************************
//*  作    者：chenjingxian
//*  功能說明：卡人資料異動-其他
//*  創建日期：2009/09/15
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*Ares_Stanley      20210420                                      電文回應學歷無法對應下拉選單時, 即時新增下拉選單選項
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

public partial class P010101020001 : PageBase
{

    #region 變數區
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息
    private static string newItemIndex = ""; //記錄根據電文新增的下拉選單項目
    #endregion

    #region 事件區

    /// 作者 chenjingxian
    /// 創建日期：2009/09/18
    /// 修改日期：2009/09/18 
    /// <summary>
    /// 加載網頁
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            SetControlsText();
            LoadDropDownList();
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUserId"));
            SetControlsEnabled(false);
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }

    /// 作者 chenjingxian
    /// 創建日期：2009/09/18
    /// 修改日期：2009/09/18 
    /// <summary>
    /// 查詢事件
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        //若先前有根據電文新增下拉選單選項, 先移除
        if (this.dropStudy.Items.FindByValue(newItemIndex) != null)
        {
            ListItem removeItem = this.dropStudy.Items.FindByValue(newItemIndex);
            this.dropStudy.Items.Remove(removeItem);
        }
        //-----------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtUserId.Text;

        //20200109-修改SOC存入條件
        //log.Statement_Text = string.Format("UserId:{0}", log.Customer_Id);
        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔

        BRL_AP_LOG.Add(log);
        //-----------------------------------------------

        SetControlsEnabled(false);
        Hashtable htInput = GetUploadMainframeData();//*查詢主機訊息Hashtable
        Hashtable htOutput = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCF6, htInput, false, "1", eAgentInfo);//*存儲主機返回訊息

        if (!htOutput.Contains("HtgMsg"))
        {
            //*獲取主機資料成功
            SetControlsEnabled(true);
            htOutput["ACCT_NBR"] = htInput["ACCT_NBR"]; //* for_xml_test  
            this.lblUserNameText.Text = htOutput["NAME_1"].ToString();
            this.txtStudy.Text = htOutput["EU_CUSTOMER_CLASS"].ToString().Trim();
            //this.dropStudy.SelectedIndex = this.dropStudy.Items.IndexOf(this.dropStudy.Items.FindByValue(htOutput["EU_CUSTOMER_CLASS"].ToString()));
            //檢查電文學歷是否在下拉選單內, 若無則新增選項至下拉選單
            if (this.dropStudy.Items.FindByValue(htOutput["EU_CUSTOMER_CLASS"].ToString().Trim()) != null)
            {
                this.txtStudy.Text = this.dropStudy.Items.FindByValue(htOutput["EU_CUSTOMER_CLASS"].ToString().Trim()).ToString();
            }
            else
            {
                ListItem listItem = new ListItem();
                listItem.Text = htOutput["EU_CUSTOMER_CLASS"].ToString().Trim();
                listItem.Value = htOutput["EU_CUSTOMER_CLASS"].ToString().Trim();
                this.dropStudy.Items.Add(listItem);
                newItemIndex = htOutput["EU_CUSTOMER_CLASS"].ToString().Trim();
                this.txtStudy.Text = this.dropStudy.Items.FindByValue(htOutput["EU_CUSTOMER_CLASS"].ToString().Trim()).ToString();
            }

            this.txtYear.Text = CommonFunction.GetSubString(htOutput["GRADUATE_YYMM"].ToString().Trim(), 0, 2);
            this.txtMonth.Text = CommonFunction.GetSubString(htOutput["GRADUATE_YYMM"].ToString().Trim(), 3, 2);

            ViewState["InfoOne"] = htOutput;
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_012");
            base.strHostMsg += htOutput["HtgSuccess"].ToString();//*主機返回成功訊息
            base.sbRegScript.Append(BaseHelper.SetFocus("txtStudy"));
        }
        else
        {
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            etMstType = eMstType.Select;
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

            //*若"HtgMsg"不為空，則HTG通訊錯誤
            if (htOutput["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg += htOutput["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
            }
            else
            {
                base.strClientMsg += htOutput["HtgMsg"].ToString();
            }
            SetControlsEnabled(false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUserId"));
        }
    }

    /// 作者 chenjingxian
    /// 創建日期：2009/09/18
    /// 修改日期：2009/09/18 
    /// <summary>
    /// 提交事件
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        DataTable dtblUpdateData = CommonFunction.GetDataTable();  //*異動customer_log記錄的Table
        ArrayList arrayName = new ArrayList(new object[] { "EU_CUSTOMER_CLASS", "GRADUATE_YYMM", "ACCT_NBR" });

        Hashtable htOutputData = new Hashtable();
        MainFrameInfo.ChangeJCF6toPCTI((Hashtable)ViewState["InfoOne"], htOutputData, arrayName); //*將異動主機的訊息預設為從主機讀取的訊息

        string strYearMonthHost; //*記錄主機原有的值年月
        string strYearMonthWeb; //*輸入年月

        //*異動學歷
        CommonFunction.ContrastDataEdit(htOutputData, dtblUpdateData, CommonFunction.GetSubString(this.txtStudy.Text.Trim(), 0, 1), "EDUCATION_CODE", BaseHelper.GetShowText("01_01010200_005"));

        //*畢業西元年月
        strYearMonthHost = htOutputData["GRADUATION_DATE"].ToString();
        strYearMonthWeb = this.txtYear.Text.Trim() + "/" + this.txtMonth.Text.Trim();

        if (strYearMonthHost != strYearMonthWeb)
        {
            htOutputData["GRADUATION_DATE"] = this.txtYear.Text.Trim() + this.txtMonth.Text.Trim();
            CommonFunction.UpdateLog(strYearMonthHost, strYearMonthWeb, BaseHelper.GetShowText("01_01010200_009"), dtblUpdateData);
        }
        else
        {
            htOutputData.Remove("GRADUATION_DATE");
        }

        if (dtblUpdateData.Rows.Count > 0)
        {
            //*提交修改主機資料
            htOutputData.Add("FUNCTION_ID", "PCMC1");
            Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4_PCTI, htOutputData, false, "2", eAgentInfo);

            if (!htReturn.Contains("HtgMsg"))
            {
                //*異動主機資料成功
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_014");
                base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息
                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtUserId.Text.Trim().ToUpper(), "P4", sPageInfo))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                if (!BRTRANS_NUM.UpdateTransNum("A11"))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }
                SetControlsEnabled(false);
                this.txtUserId.Text = "";
                base.sbRegScript.Append(BaseHelper.SetFocus("txtUserId"));
            }
            else
            {
                //*異動主機資料失敗
                if (htReturn["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htReturn["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_024");
                }
                else
                {
                    base.strClientMsg += htReturn["HtgMsg"].ToString();
                }
                base.sbRegScript.Append(BaseHelper.SetFocus("txtStudy"));
            }
        }
        else
        {
            //*無需異動資料
            base.strHostMsg += MessageHelper.GetMessage("01_00000000_015");
        }
        //若先前有根據電文新增下拉選單選項, 先移除
        if (this.dropStudy.Items.FindByValue(newItemIndex) != null)
        {
            ListItem removeItem = this.dropStudy.Items.FindByValue(newItemIndex);
            this.dropStudy.Items.Remove(removeItem);
        }
    }

    #endregion

    #region 方法區

    /// 作者 chenjingxian
    /// 創建日期：2009/09/18
    /// 修改日期：2009/09/18 
    /// <summary>
    /// 設置控件文本
    /// </summary>
    private void SetControlsText()
    {
        btnSelect.Text = BaseHelper.GetShowText("01_01010200_003");
        btnSubmit.Text = BaseHelper.GetShowText("01_01010200_008");
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
        if (BaseHelper.GetCommonProperty("01", "13", ref dtblResult))
        {
            for (int i = 0; i < dtblResult.Rows.Count; i++)
            {
                ListItem listItem = new ListItem();
                listItem.Text = dtblResult.Rows[i][0].ToString() + " " + dtblResult.Rows[i][1].ToString();
                listItem.Value = dtblResult.Rows[i][0].ToString();
                //listItem.Attributes.Add("onclick", "showOptionValue( this );");
                this.dropStudy.Items.Add(listItem);
            }
        }
    }


    /// 作者 chenjingxian
    /// 創建日期：2009/09/18
    /// 修改日期：2009/09/18 
    /// <summary>
    /// 設置控件是否可用
    /// </summary>
    /// <param name="blnEnabled">True可用，false不可用</param>
    private void SetControlsEnabled(bool blnEnabled)
    {
        this.lblUserNameText.Text = "";
        this.txtStudy.Text = "";
        this.txtYear.Text = "";
        this.txtMonth.Text = "";

        this.txtStudy.Enabled = blnEnabled;
        this.dropStudy.Enabled = blnEnabled;
        this.txtYear.Enabled = blnEnabled;
        this.txtMonth.Enabled = blnEnabled;
        this.btnSubmit.Enabled = blnEnabled;
    }






    /// 作者 chenjingxian
    /// 創建日期：2009/09/18
    /// 修改日期：2009/09/18 
    /// <summary>
    /// 組合異動主機訊息Hashtable
    /// </summary>
    /// <param name="type">主機類型</param>
    /// <returns>異動主機訊息的HashTable</returns>
    private Hashtable GetUploadMainframeData()
    {
        Hashtable htInput = new Hashtable();//*存儲主機上的資料
        htInput.Add("ACCT_NBR", CommonFunction.GetSubString(this.txtUserId.Text.Trim().ToUpper(), 0, 16));
        htInput.Add("FUNCTION_CODE", "1");
        htInput.Add("LINE_CNT", "0000");
        return htInput;
    }



    #endregion
}
