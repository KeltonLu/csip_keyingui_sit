//******************************************************************
//*  作    者：杨璐
//*  功能說明：自扣案件處理狀態查询
//*  創建日期：2012/09/20
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
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;

public partial class P010210000001 : PageBase
{

    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息

    #region event

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

    /// 作者 楊璐
    /// 創建日期：2012/09/20
    /// 修改日期：2012/09/20
    /// 20210707_Ares_Stanley-css調整
    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        this.gvpbSearchRecord.Columns[0].HeaderText = BaseHelper.GetShowText("01_02100000_007");
        this.gvpbSearchRecord.Columns[1].HeaderText = BaseHelper.GetShowText("01_02100000_021");
        this.gvpbSearchRecord.Columns[2].HeaderText = BaseHelper.GetShowText("01_02100000_009");
        this.gvpbSearchRecord.Columns[3].HeaderText = BaseHelper.GetShowText("01_02100000_008");
        this.gvpbSearchRecord.Columns[4].HeaderText = BaseHelper.GetShowText("01_02100000_010");
        this.gvpbSearchRecord.Columns[5].HeaderText = BaseHelper.GetShowText("01_02100000_011");
        this.gvpbSearchRecord.Columns[6].HeaderText = BaseHelper.GetShowText("01_02100000_012");
        this.gvpbSearchRecord.Columns[7].HeaderText = BaseHelper.GetShowText("01_02100000_022");
        this.gvpbSearchRecord.Columns[8].HeaderText = BaseHelper.GetShowText("01_02100000_013");
        this.gvpbSearchRecord.Columns[9].HeaderText = BaseHelper.GetShowText("01_02100000_014");
        this.gvpbSearchRecord.Columns[10].HeaderText = BaseHelper.GetShowText("01_02100000_023");
        this.gvpbSearchRecord.Columns[11].HeaderText = BaseHelper.GetShowText("01_02100000_016");
        this.gvpbSearchRecord.Columns[12].HeaderText = BaseHelper.GetShowText("01_02100000_015");
        this.gvpbSearchRecord.Columns[13].HeaderText = BaseHelper.GetShowText("01_02100000_024");
        this.gvpbSearchRecord.Columns[4].HeaderStyle.CssClass = "Grid_Header_whiteSpaceNormal";
        this.gvpbSearchRecord.Columns[6].HeaderStyle.CssClass = "Grid_Header_whiteSpaceNormal";
        this.gvpbSearchRecord.Columns[7].HeaderStyle.CssClass = "Grid_Header_whiteSpaceNormal";
        this.gvpbSearchRecord.Columns[9].HeaderStyle.CssClass = "Grid_Header_whiteSpaceNormal";
        this.gvpbSearchRecord.Columns[12].HeaderStyle.CssClass = "Grid_Header_whiteSpaceNormal";
        this.gvpbSearchRecord.Columns[13].HeaderStyle.CssClass = "Grid_Header_whiteSpaceNormal";
        for (int i = 0; i <13; i++)
        {
            this.gvpbSearchRecord.Columns[i].ItemStyle.CssClass = "Grid_Header_whiteSpaceNormal";
        }

        //* 設置每頁顯示記錄最大條數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbSearchRecord.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }

    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtCust_id.Text;

        //20200109-修改SOC存入條件
        //log.Statement_Text = string.Format("UserId:{0}", log.Customer_Id); //查詢條件內容: 用 | 區隔,要看文件確認
        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔

        BRL_AP_LOG.Add(log);
        //------------------------------------------------------

        this.gpList.CurrentPageIndex = 0;

        this.BindGridView();

        this.gpList.Visible = true;
    }

    protected void btnExc_Click(object sender, EventArgs e)
    {
        try
        {
            string strMsgID = "";
            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());

            string strDateStart = this.dpDateStart.Text.Replace("/", "");
            string strDateEnd = this.dpDateEnd.Text.Replace("/", "");
            // Y. strIsCTCB(radIsCTCB) P. 郵局自扣(radIsPostOffice) N. 他行自扣(radIsNotCTCB) A.本行+郵局+他行自扣(radAll)
            string strIsCTCB = this.radIsCTCB.Checked ? "Y" : (this.radIsPostOffice.Checked ? "P" : (this.radIsNotCTCB.Checked ? "N" : "A"));
            //string strIsCTCB = this.radIsCTCB.Checked ? "Y" : (this.radAll.Checked ? "B" : "N");

            //string strIsUpdateByTXT = this.radIsUpdateByTXT.Checked ? "Y" : "N";
            string strIsUpdateByTXT = this.radIsUpdateByTXT.Checked ? "Y" : (this.radIsUpdateAll.Checked ? "A" : "N");
            string strcust_id = this.txtCust_id.Text;

            DataSet ds = CSIPKeyInGUI.BusinessRules_new.BRAuto_pay_status.GetDataFromtblAuto_Pay_StatusForReportWithoutPaging(strDateStart, strDateEnd, strIsCTCB, strIsUpdateByTXT, strcust_id);
            if (!(ds != null && ds.Tables[0].Rows.Count > 0)) return;

            if (!BRAuto_pay_status.CreateExcelFile(ds,
                            ((EntityAGENT_INFO)this.Session["Agent"]).agent_name,
                            ref strServerPathFile,
                            ref strMsgID))
            {
                if (strMsgID != "")
                    base.strClientMsg += MessageHelper.GetMessage(strMsgID);
                else
                    base.strClientMsg += MessageHelper.GetMessage("01_03050400_004");
                return;
            }

            string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
            string strFileName = BaseHelper.GetShowText("01_02100000_001") + strYYYYMMDD + ".xls";
            this.Session["ServerFile"] = strServerPathFile;
            this.Session["ClientFile"] = strFileName;
            // string urlString = @"ClientMsgShow('" + MessageHelper.GetMessage("01_03050400_005") + "');";
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("01_03050400_005") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);

        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += BaseHelper.ClientMsgShow("01_03050400_004");
        }
    }

    #endregion event

    #region function

    /// 作者 楊璐
    /// 創建日期：2012/09/20
    /// 修改日期：2012/09/20
    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    private void BindGridView()
    {
        try
        {
            int intTotolCount = 0;

            string dateStart = this.dpDateStart.Text.Replace("/", "");
            string dateEnd = this.dpDateEnd.Text.Replace("/", "");
            // Y. 本行自扣(radIsCTCB) P. 郵局自扣(radIsPostOffice) N. 他行自扣(radIsNotCTCB) A.本行+郵局+他行自扣(radAll)
            string isCTCB = this.radIsCTCB.Checked ? "Y" : (this.radIsPostOffice.Checked ? "P" : (this.radIsNotCTCB.Checked ? "N" : "A"));
            //string isCTCB = this.radIsCTCB.Checked ? "Y" : (this.radAll.Checked ? "B" : "N");
            // N. 上送主機(radIsNotUpdateByTXT) Y. 主機Temp檔(radIsUpdateByTXT) A. 上送主機+主機Temp檔(radIsUpdateAll)
            string isUpdateByTXT = this.radIsUpdateByTXT.Checked ? "Y" : (this.radIsUpdateAll.Checked ? "A" : "N");
            string custID = this.txtCust_id.Text;
            Logging.Log(custID, LogLayer.UI);

            DataSet ds = CSIPKeyInGUI.BusinessRules_new.BRAuto_pay_status.GetDataFromtblAuto_Pay_StatusForReport(dateStart, dateEnd, isCTCB, isUpdateByTXT, custID, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotolCount);

            if (ds == null)
            {
                // 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("01_02100000_001");
                this.gpList.RecordCount = 0;
                this.gvpbSearchRecord.DataSource = null;
                this.gvpbSearchRecord.DataBind();
            }
            else
            {
                this.gpList.RecordCount = intTotolCount;
                this.gvpbSearchRecord.DataSource = ds.Tables[0];
                this.gvpbSearchRecord.DataBind();
                // 顯示端末訊息
                if (intTotolCount == 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_0210000_003");
                }
                else
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_02100000_002");
                }
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_02100000_001");
        }
    }

    #endregion function
}
