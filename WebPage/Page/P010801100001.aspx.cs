//******************************************************************
//*  作    者：趙呂梁  

//*  功能說明：卡人資料異動-異動姓名(生日)

//*  創建日期：2009/10/06
//*  修改記錄：
//*  2010/12/21   chaoma      RQ-2010-005537-000      增設(□P4及□P4D)作業選項及【自動翻譯】按鈕 

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
using CSIPCommonModel.EntityLayer;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Message;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPKeyInGUI.EntityLayer_new;
using CSIPNewInvoice.EntityLayer_new;

public partial class P010801100001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    //private Entity_NoteLog thisEntity_NoteLog;

    private EntityAGENT_INFO eAgentInfo;

    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        if (!IsPostBack)
        {
            ViewState["ReferringURL"] = Request.ServerVariables["HTTP_REFERER"];//btnCancel_Click

            AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
            if (sessionOBJ == null)
            {
                string NavigateUrl = "P010801000001.aspx";
                string urlString = @"alert('查無資料');location.href='" + NavigateUrl + "';";
                base.sbRegScript.Append(urlString);
            }

            if (sessionOBJ != null)
            {
                //讀取公司資料 HQ_WORK CDATA,EDATA
                EntityAML_HQ_Work hqObj = BRAML_HQ_Work.getHQ_WOrk(sessionOBJ);
                lbAML_HQ_Work_CASE_NO.Text = hqObj.CASE_NO;
                hidAML_HQ_Work_CASE_NO.Value = hqObj.CASE_NO;
                lbAML_HQ_Work_HCOP_HEADQUATERS_CORP_NO.Text = hqObj.HCOP_HEADQUATERS_CORP_NO;
                lbAML_HQ_Work_HCOP_REG_NAME.Text = hqObj.HCOP_REG_NAME;
            }

            //Entity_NoteLog objEntity_NoteLog = new Entity_NoteLog();
            //DataTable temp = new DataTable();

            //objEntity_NoteLog.NL_CASE_NO = "0000000000001";
            //objEntity_NoteLog.NL_SecondKey = "0000000000001";
            //objEntity_NoteLog.NL_DateTime = DateTime.Now;
            //objEntity_NoteLog.NL_User = "Test";
            //objEntity_NoteLog.NL_Type = "Note";
            //objEntity_NoteLog.NL_ShowFlag = "1";
            //NoteLog.GetNoteLogDataTable(objEntity_NoteLog,"", ref temp);

            //txtNoteLog_NL_Value.Text = DateTime.Now.ToString("yyyy/MM/dd HHmmss.fff");
        }
        base.strClientMsg = "";
        base.strHostMsg = "";
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/06
    /// 修改日期：2009/10/06 
    /// <summary>
    /// 提交事件
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        bool blSubmitStatus = false;
        //string NavigateUrl = "P010801010001.aspx";
        //string urlString = @"alert('[MSG]');location.href='" + NavigateUrl + "';";
        string urlString = @"alert('[MSG]');";

        if (txtNoteLog_NL_Value.Text.Trim() == String.Empty)
        {
            urlString = @"alert('內容不可為空白');";
            Response.Redirect(ViewState["ReferringURL"].ToString());
        }
        else if (txtNoteLog_NL_Value.Text.Length > 500)
        {
            urlString = @"alert('內容長度過長');";
            Response.Redirect(ViewState["ReferringURL"].ToString());
        }
        else
        {
            try
            {
                eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];

                Entity_NoteLog objEntity_NoteLog = new Entity_NoteLog();
                objEntity_NoteLog.NL_CASE_NO = hidAML_HQ_Work_CASE_NO.Value;
                objEntity_NoteLog.NL_SecondKey = "";
                objEntity_NoteLog.NL_DateTime = DateTime.Now;
                //objEntity_NoteLog.NL_User = "Test";
                objEntity_NoteLog.NL_User = eAgentInfo.agent_id;
                objEntity_NoteLog.NL_Type = "Note";
                // 20220620 調整將文字內容的 \r\n 換行符號替換成頁面顯示換行用的 <br /> By Kelton
                //objEntity_NoteLog.NL_Value = txtNoteLog_NL_Value.Text;
                string noteLog = txtNoteLog_NL_Value.Text;
                if (noteLog.Contains("\r\n"))
                {
                    noteLog = noteLog.Replace("\r\n", "<br />").Trim();
                }
                if (noteLog.Contains("\n"))
                {
                    noteLog = noteLog.Replace("\n", "<br />").Trim();
                }
                objEntity_NoteLog.NL_Value = noteLog;
                objEntity_NoteLog.NL_ShowFlag = "1";
                blSubmitStatus = NoteLog.AddNewEntity(objEntity_NoteLog);
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
            if (blSubmitStatus)
            {
                urlString = urlString.Replace("[MSG]", "更新成功");
                Response.Redirect(ViewState["ReferringURL"].ToString());
            }
            else
            {
                urlString = urlString.Replace("[MSG]", "更新失敗");
                Response.Redirect(ViewState["ReferringURL"].ToString());
            }
        }

        base.sbRegScript.Append(urlString);

        //base.strClientMsg = "";
        //base.strHostMsg = "";
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        /*
         * 需補上案件明細 所需的Session物件(含案件列表的查詢條件)
         */
        //string NavigateUrl = "P010801010001.aspx";
        //string urlString = @"location.href='" + NavigateUrl + "';";
        //base.sbRegScript.Append(urlString);

        //20211005_Ares_Jack_返回上一頁
        if (ViewState["ReferringURL"] != null)
            Response.Redirect(ViewState["ReferringURL"].ToString());
    }
    #endregion

    #region 方法區

    #endregion
}
