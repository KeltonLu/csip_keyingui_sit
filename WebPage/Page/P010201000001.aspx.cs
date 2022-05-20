//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：其他作業-記錄查詢
//*  創建日期：2009/10/22
//*  修改紀錄：
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
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
using System.Drawing;
//using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.BusinessRules_new;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPCommonModel.EntityLayer;
public partial class P010201000001 : PageBase
{
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #region event
    /// 作者 占偉林
    /// 創建日期：2009/10/22
    /// 修改日期：2009/10/22 
    /// <summary>
    /// 畫面裝載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //* 綁定GridView列頭訊息
            ShowControlsText();
            this.gpList.RecordCount = 0;
            this.gpList.Visible = false;
            this.gvpbSearchRecord.DataSource = null;
            this.gvpbSearchRecord.DataBind();
        }
        base.strHostMsg += "";
        base.strClientMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"]; //*記錄網頁訊息
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/22
    /// 修改日期：2009/10/22 
    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        this.gvpbSearchRecord.Columns[0].HeaderText = BaseHelper.GetShowText("01_02010000_004");
        this.gvpbSearchRecord.Columns[1].HeaderText = BaseHelper.GetShowText("01_02010000_005");
        this.gvpbSearchRecord.Columns[2].HeaderText = BaseHelper.GetShowText("01_02010000_006");
        this.gvpbSearchRecord.Columns[3].HeaderText = BaseHelper.GetShowText("01_02010000_007");
        this.gvpbSearchRecord.Columns[4].HeaderText = BaseHelper.GetShowText("01_02010000_008");
        this.gvpbSearchRecord.Columns[5].HeaderText = BaseHelper.GetShowText("01_02010000_009");
        this.gvpbSearchRecord.Columns[6].HeaderText = BaseHelper.GetShowText("01_02010000_010");
        this.gvpbSearchRecord.Columns[7].HeaderText = BaseHelper.GetShowText("01_02010000_011");

        //* 設置每頁顯示記錄最大條數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbSearchRecord.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/22
    /// 修改日期：2009/10/22 
    /// <summary>
    /// 頁導航
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/22
    /// 修改日期：2009/10/22 
    /// <summary>
    /// 點選【查詢】按鈕時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);

        //特殊處理，不知道輸入的是卡號還是ID，以長度判斷，輸入長度大於10，以卡號處理，反之以ID處理
        if (this.txtKey.Text.Trim().Length > 10)
        {
            log.Account_Nbr = this.txtKey.Text;
            //20200109-修改SOC存入條件
            //log.Statement_Text = string.Format("AC_NO:{0}", log.Account_Nbr); //查詢條件內容: 用 | 區隔,要看文件確認        
        }
        else
        {
            log.Customer_Id = this.txtKey.Text;
            //20200109-修改SOC存入條件
            //log.Statement_Text = string.Format("UserId:{0}", log.Customer_Id); //查詢條件內容: 用 | 區隔,要看文件確認    
        }

        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔

        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        this.gpList.CurrentPageIndex = 0;
        //* 將【關鍵字】字段保存到ViewState中，
        this.ViewState["Key"] = this.txtKey.Text.Trim();
        //* 以【關鍵字】字段查詢記錄訊息，并顯示到畫面
        this.BindGridView();
        //* 顯示頁面導航
        this.gpList.Visible = true;
        //* 將光標設置到【關鍵字】欄位
        base.sbRegScript.Append(BaseHelper.SetFocus("txtKey"));
    }
    #endregion event

    #region function
    /// 作者 占偉林
    /// 創建日期：2009/10/22
    /// 修改日期：2009/10/22 
    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    private void BindGridView()
    {
        try
        {
            string strMsgID = "";
            int intTotolCount = 0;
            DataTable dtblCustomer_Log = (DataTable)BRCustomer_Log.SearchCustomer_Log(this.ViewState["Key"].ToString(), this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotolCount, ref strMsgID);
            if (dtblCustomer_Log == null)
            {
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage(strMsgID);
                this.gpList.RecordCount = 0;
                this.gvpbSearchRecord.DataSource = null;
                this.gvpbSearchRecord.DataBind();
            }
            else
            {
                this.gpList.RecordCount = intTotolCount;
                this.gvpbSearchRecord.DataSource = dtblCustomer_Log;
                this.gvpbSearchRecord.DataBind();
                //* 顯示端末訊息
                if (intTotolCount == 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_02010000_003");
                }
                else
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_02010000_002");
                }
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_02010000_001");
        }
    }
    #endregion function
}
