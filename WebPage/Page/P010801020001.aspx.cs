// *****************************************************************
//   作    者：林家賜
//   功能說明：AML資料 案件明細(主管)
//   創建日期：2019/02/03
//   修改記錄：
// <author>            <time>            <TaskID>                <desc>
// ******************************************************************

using System;
using System.Data;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using CSIPNewInvoice.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules_new;
using Framework.Common.Logging;
using Framework.WebControls;
using Framework.Data.OM.Transaction;
using CSIPCommonModel.EntityLayer_new;

public partial class Page_P010801020001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    private const string thisPageName = "P010801020001.aspx";
    private List<string> ID = new List<string>();//關聯案件序號ID
    #region  轉換代碼用字典 

    /// <summary>
    /// 通用字典，各項次機能以前兩碼區分
    /// </summary>
    Dictionary<string, string> DCCommonColl;

    #endregion
    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        if (!IsPostBack)
        {
            CommonFunction.SetControlsEnabled(pnlText, true);
            ///設定本頁使用GRID頁數
            setGridLine();
            showBaseData(); //非POSTBACK，則讀取資料
        }

        if (!validUser())
        {
            string strAlertMsg = MessageHelper.GetMessages("01_01080102_002");
            sbRegScript.Append("alert('" + strAlertMsg + "');window.location.href = 'P010801000001.aspx';");
            return;
        }

    }
    int grvManagerDataSubRowCount = 0;
    /// <summary>
    /// 高階管理人暨實質受益人資料綁定處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvManagerData_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            EntityAML_HQ_Manager_Work rowView = (EntityAML_HQ_Manager_Work)e.Row.DataItem;
            //轉換身分別文字
            // e.Row.Cells[8].Text = ConvManagerIdTypeToStr(rowView);
            e.Row.Cells[7].Text = ConvManagerIdTypeToStr(rowView);

            //無ID不顯示  調整成無姓名  Talas 20190717
            if (string.IsNullOrEmpty(rowView.HCOP_BENE_NAME))
            {
                e.Row.Visible = false;
            }
            else
            {
                //----------------------------------------------------
                //長姓名區
                //if (rowView.HCOP_BENE_LNAME.Length > 5)
                if (string.IsNullOrEmpty(rowView.HCOP_BENE_LNAME) == false || string.IsNullOrEmpty(rowView.HCOP_BENE_ROMA) == false)
                {
                    ControlCollection _table = e.Row.Parent.Controls; //e.Row.Parent就是Table                    
                    GridViewRow gvr = new GridViewRow(-1, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
                    TableCell cell_label = new TableCell();
                    TableCell cell_value = new TableCell();
                    int _columnSpan = 8;
                    cell_label.ColumnSpan = 1;
                    cell_label.Text = BaseHelper.GetShowText("01_01080103_059");
                    cell_value.ColumnSpan = _columnSpan;
                    cell_value.Text = rowView.HCOP_BENE_LNAME;

                    cell_label.HorizontalAlign = HorizontalAlign.Center;
                    gvr.Cells.Add(cell_label);
                    gvr.Cells.Add(cell_value);
                    //gvr.CssClass = "master" + index + "_" + index;
                    //gvr.Attributes["style"] = "display:none;";
                    grvManagerDataSubRowCount += 2;
                    _table.AddAt(grvManagerDataSubRowCount, gvr);

                    gvr = new GridViewRow(-1, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
                    cell_label = new TableCell();
                    cell_value = new TableCell();
                    cell_label.HorizontalAlign = HorizontalAlign.Center;
                    cell_label.Text = BaseHelper.GetShowText("01_01080103_060");
                    cell_value.ColumnSpan = _columnSpan;
                    cell_value.Text = rowView.HCOP_BENE_ROMA;
                    gvr.Cells.Add(cell_label);
                    gvr.Cells.Add(cell_value);
                    grvManagerDataSubRowCount += 1;
                    _table.AddAt(grvManagerDataSubRowCount, gvr);
                }
            }

            //調整長姓名區或姓名區不顯示的列數(長姓名和羅馬拼音都為空 或 名字為空)
            if ((string.IsNullOrEmpty(rowView.HCOP_BENE_LNAME) == true && string.IsNullOrEmpty(rowView.HCOP_BENE_ROMA) == true)
                || string.IsNullOrEmpty(rowView.HCOP_BENE_NAME))
            {
                grvManagerDataSubRowCount += 1;
            }
        }
    }

    /// <summary>
    ///  案件歷程聯絡資料綁定處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdNoteLog_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //取得eAgentInfo
            checkUserAgent();
            System.Data.DataRowView rowView = (DataRowView)e.Row.DataItem;
            string idKey = rowView["NL_Type"].ToString();
            string dTime = rowView["NL_DateTime"].ToString();
            e.Row.Cells[3].Text = GetDcValue("CN_" + idKey);
            DateTime Dt = Convert.ToDateTime(dTime);
            e.Row.Cells[0].Text = Dt.ToString("yyyy/MM/dd");
            e.Row.Cells[1].Text = Dt.ToString("HH:mm:ss");
            //e.Row.Cells[2].Text = eAgentInfo.agent_name;

        }
    }

    /// <summary>



    ///// <summary>
    ///// EDD_VIEW
    ///// </summary>
    ///// <param name="sender"></param>
    ///// <param name="e"></param>
    //protected void Btn_EDD_View_Click(object sender, EventArgs e)
    //{
    //    Session["P010801010001_Last"] = thisPageName;
    //    Response.Redirect("P010801090001.aspx");
    //}
    /// <summary>
    /// SDD VIEW
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Btn_SCCD_View_Click(object sender, EventArgs e)
    {
        if (SCDDlblNameCheck_No.Text == "")
        {

            string strAlertMsgR = MessageHelper.GetMessages("01_01080102_011");
            sbRegScript.Append("alert('" + strAlertMsgR + "');");
            return;
        }
        Session["P010801010001_Last"] = thisPageName;
        //20210412_Ares_Stanley-調整轉向方法
        Response.Redirect("P010801060001.aspx", false);

    }
    /// <summary>
    /// 回上頁
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        //20210412_Ares_Stanley-調整轉向方法
        Response.Redirect("P010801000001.aspx", false);
    }
    /// <summary>
    /// 送審退回
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReject_Click(object sender, EventArgs e)
    {
        //送審要變更 CaseOwner_User 經辦，以目前帶入 因為是退件 C1等級變更為M1 處理狀態變更為1
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];
        if (sessionOBJ == null || eAgentInfo == null)
        {
            string strAlertMsg = MessageHelper.GetMessages("01_01080101_002");
            sbRegScript.Append("alert('" + strAlertMsg + "');window.location.href = 'P010801000001.aspx';");
            return;
        }
        //sessionOBJ.CaseOwner_User = eAgentInfo.agent_id;  
        sessionOBJ.CaseProcess_User = "M1"; //經辦
        //20191111-RQ-2018-015749-002 modify by Peggy
        //sessionOBJ.CaseProcess_Status = "1";//處理中
        string _ProcessStatus = string.Empty;//記錄案件狀態
        string _NL_Type = string.Empty;//記錄案件歷程顯示文字

        switch (sessionOBJ.CaseProcess_Status.Trim())
        {
            case "3"://不合作結案退件
                _ProcessStatus = "13";
                _NL_Type = "RejectNonCooperated";
                break;
                case "4"://商店解約結案退件
                _ProcessStatus = "14";
                _NL_Type = "RejectCaseClosed";
                break;
                case "5"://其他原因結案退件
                _ProcessStatus = "15";
                _NL_Type = "RejectOtherClosed";
                break;
                default://一般案件退件
                _ProcessStatus = "1";
                _NL_Type = "ReturnCase";
                break;
        }
        sessionOBJ.CaseProcess_Status = _ProcessStatus.Trim();


        bool result;
        using (OMTransactionScope ts = new OMTransactionScope())
        {
            result = BRAML_HQ_Work.Update_Apply(sessionOBJ, "2");

            //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
            //2021/7/6 EOS_AML(NOVA) 關聯案件一併送出  by Ares Dennis 
            //foreach (string id in ID)
            //{
            //    sessionOBJ.ID = id;
            //    result = BRAML_HQ_Work.Update_Apply(sessionOBJ, "2");
            //}
            //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis

            if (!result)
            {
                string strAlertMsg = MessageHelper.GetMessages("01_01080102_004");
                sbRegScript.Append("alert('" + strAlertMsg + "');");
                return;
            }
            //寫入案件歷程
            EntityNoteLog notrLog = new EntityNoteLog();
            notrLog.NL_CASE_NO = sessionOBJ.CASE_NO;
            notrLog.NL_SecondKey = "";
            notrLog.NL_ShowFlag = "1";
            notrLog.NL_DateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            notrLog.NL_User = eAgentInfo.agent_id;
            //20191111-RQ-2018-015749-002 modify by Peggy
            //notrLog.NL_Type = "ReturnCase";
            notrLog.NL_Type = _NL_Type.Trim();
            notrLog.NL_Value = txtNotelogValue.Text;
            result = BRNoteLog.Insert(notrLog);
            //調整為提示訊息
            if (!result)
            {

                string strAlertMsg = MessageHelper.GetMessages("01_01080102_005");
                sbRegScript.Append("alert('" + strAlertMsg + "');");
                return;
            }

            // 20220413 退件時要將 AML_Cdata_Work_S 對應的 C 檔資料刪除 by Kelton
            BRAML_Cdata_Work_S.Delete(new AML_SessionState { CASE_NO = sessionOBJ.CASE_NO });

            ts.Complete();
        }
        //提示作業完成
        string strAlertMsgR = MessageHelper.GetMessages("01_01080102_006");
        sbRegScript.Append("alert('" + strAlertMsgR + "');window.location.href = 'P010801000001.aspx';");
        return;



    }
    /// <summary>
    /// 放行
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnFinish_Click(object sender, EventArgs e)
    { //要變更 CaseOwner_User 經辦，以目前帶入 因為是放行 C1等級變更為M1 處理狀態變更為2
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];
        if (sessionOBJ == null || eAgentInfo == null)
        {
            string strAlertMsg = MessageHelper.GetMessages("01_01080101_002");
            sbRegScript.Append("alert('" + strAlertMsg + "');window.location.href = 'P010801000001.aspx';");
            return;
        }

        using (OMTransactionScope ts = new OMTransactionScope())
        {

            //sessionOBJ.CaseOwner_User = eAgentInfo.agent_id;
            // sessionOBJ.CaseProcess_User = "M1"; //經辦
            //20191112-
            //sessionOBJ.CaseProcess_Status = "2";//放行
            string _ProcessStatus = string.Empty;//記錄案件狀態
            string _NL_Type = string.Empty;//記錄案件歷程顯示文字

            switch (sessionOBJ.CaseProcess_Status.Trim())
            {
                case "3"://不合作結案放行
                    _ProcessStatus = "23";
                    _NL_Type = "NonCooperatedDone";
                    break;
                case "4"://商店解約結案放行
                    _ProcessStatus = "24";
                    _NL_Type = "CaseClosedDone";
                    break;
                case "5"://其他原因結案放行
                    _ProcessStatus = "25";
                    _NL_Type = "OtherClosedDone";
                    break;
                default://一般案件放行
                    _ProcessStatus = "2";
                    _NL_Type = "CaseOK";
                    break;
            }
            sessionOBJ.CaseProcess_Status = _ProcessStatus.Trim();

            bool result = BRAML_HQ_Work.Update_Apply(sessionOBJ, "3");

            //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
            //2021/7/6 EOS_AML(NOVA) 關聯案件一併送出  by Ares Dennis 
            //foreach (string id in ID)
            //{
            //    sessionOBJ.ID = id;
            //    result = BRAML_HQ_Work.Update_Apply(sessionOBJ, "3");
            //}
            //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis

            if (!result)
            {
                string strAlertMsg = MessageHelper.GetMessages("01_01080102_007");
                sbRegScript.Append("alert('" + strAlertMsg + "');");
                return;
            }

            //寫入案件歷程
            EntityNoteLog notrLog = new EntityNoteLog();
            notrLog.NL_CASE_NO = sessionOBJ.CASE_NO;
            notrLog.NL_SecondKey = "";
            notrLog.NL_ShowFlag = "1";
            notrLog.NL_DateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            notrLog.NL_User = eAgentInfo.agent_id;
            //20191112-RQ-2018-015749-002 modify by Peggy
            //notrLog.NL_Type = "CaseOK";
            notrLog.NL_Type = _NL_Type.Trim();
            notrLog.NL_Value = txtNotelogValue.Text;
            result = BRNoteLog.Insert(notrLog);
            //調整為提示訊息
            if (!result)
            {
                string strAlertMsg = MessageHelper.GetMessages("01_01080102_005");
                sbRegScript.Append("alert('" + strAlertMsg + "');");
                return;
            }
            //追加調整排程產總公司異動檔旗標更新 For 放行
            result = BRAML_HQ_Work.Update_AML_ExportFileFlagr(sessionOBJ);
            //調整為提示訊息
            if (!result)
            {
                strAlertMsg = MessageHelper.GetMessages("01_01080102_012");
                sbRegScript.Append("alert('" + strAlertMsg + "');");
                return;
            }

            result = BRAML_CASE_DATA.Update_CaseDataCloseDate(sessionOBJ.CASE_NO); //案件放行時，把AML_CASE_DATA的Close_Date填入Now

            if (!result)
            {
                string strAlertMsg = MessageHelper.GetMessages("01_01080102_013");
                sbRegScript.Append("alert('" + strAlertMsg + "');");
                return;
            }


            ts.Complete();
        }
        //提示作業完成
        string strAlertMsgR = MessageHelper.GetMessages("01_01080102_008");
        sbRegScript.Append("alert('" + strAlertMsgR + "');window.location.href = 'P010801000001.aspx';");
        return;

    }
    /// <summary>
    /// 呈上級主管
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnApply_Click(object sender, EventArgs e)
    {
        //送審要變更 CaseOwner_User 經辦，以目前帶入 MI等級變更為C2
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];
        if (sessionOBJ == null || eAgentInfo == null)
        {
            string strAlertMsg = MessageHelper.GetMessages("01_01080101_002");
            sbRegScript.Append("alert('" + strAlertMsg + "');window.location.href = 'P010801000001.aspx';");
            return;
        }
        //呈上階主管時，調整為僅改CaseProcess_User與CaseProcess_Status，不異動CaseOwner_User
        sessionOBJ.CaseOwner_User = eAgentInfo.agent_id;
        sessionOBJ.CaseProcess_User = "C2"; //二級主管
        //20210204-2021 3月RC需求，稽核議題修正，不合作結案需上送至二階主管核准
        //sessionOBJ.CaseProcess_Status = "0";//處理中
        //if (sessionOBJ.CaseProcess_Status.Trim().Equals("3")//20210422-RQ-2021-004136-003-5RC 應惠文要求，要高風險且不合作結案才需送至二階主管
        //20220329-RQ-2022-002393-000-202204RC調整，異常結案重新試算後風險等級為高風險時，不管以何種方式結案，都需上送二階主管
        /*
        if (sessionOBJ.CaseProcess_Status.Trim().Equals("3") && sessionOBJ.OriginalRiskRanking.Trim().Equals("H"))
        {
            sessionOBJ.CaseProcess_Status = "3";//處理中
        }
        else
        {
            sessionOBJ.CaseProcess_Status = "0";//處理中
        }
        */
        bool result = BRAML_HQ_Work.Update_Apply(sessionOBJ, "2");

        //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
        //2021/7/6 EOS_AML(NOVA) 關聯案件一併送出 by Ares Dennis 
        //foreach (string id in ID)
        //{
        //    sessionOBJ.ID = id;
        //    result = BRAML_HQ_Work.Update_Apply(sessionOBJ, "2");
        //}
        //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis

        if (!result)
        {
            string strAlertMsg = MessageHelper.GetMessages("01_01080102_009");
            sbRegScript.Append("alert('" + strAlertMsg + "');");
            return;
        }

        //寫入案件歷程
        EntityNoteLog notrLog = new EntityNoteLog();
        notrLog.NL_CASE_NO = sessionOBJ.CASE_NO;
        notrLog.NL_SecondKey = "";
        notrLog.NL_ShowFlag = "1";
        notrLog.NL_DateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        notrLog.NL_User = eAgentInfo.agent_id;
        notrLog.NL_Type = "Verify";
        notrLog.NL_Value = txtNotelogValue.Text;
        result = BRNoteLog.Insert(notrLog);
        //調整為提示訊息
        if (!result)
        {
            string strAlertMsg = MessageHelper.GetMessages("01_01080102_005");
            sbRegScript.Append("alert('" + strAlertMsg + "');");
            return;
        }


        //提示作業完成
        string strAlertMsgR = MessageHelper.GetMessages("01_01080102_010");
        sbRegScript.Append("alert('" + strAlertMsgR + "');window.location.href = 'P010801000001.aspx';");
        return;




    }


    #endregion

    #region 方法區

    /// <summary>
    /// 腳色權限驗證
    /// </summary>
    private bool validUser()
    {
        bool isMaster = false;
        string userRolls = eAgentInfo.roles;
        if (userRolls.Contains("CSIP0121") || userRolls.Contains("CSIP0122"))
        {
            isMaster = true;
        }
        if (userRolls.Contains("CSIP0122"))
        {
            //看不到送審
            btnApply.Visible = false;
        }
        //當案件別為C1且角色有121.風險為高 則只能看到送審，關閉放行
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];

        //20191112-RQ-2018-015749-002 modify by Peggy
        /*
            if (sessionOBJ.CaseProcess_User == "C1" && userRolls.Contains("CSIP0121") && lblSR_RiskLevel.Text == "高風險")
            {
                btnApply.Visible = true;
                btnFinish.Visible = false;
            }
         */
        //若為異常結案，即只需上呈到一階主管即可，不考慮其他因素，如高風險
        //20210129 異常結案-不合作結案需上呈至二階主管
        //20210422-RQ-2021-004136-003-5RC 應惠文要求，要高風險且不合作結案才需送至二階主管

        //if (sessionOBJ.CaseProcess_Status.Trim().Equals("3") || sessionOBJ.CaseProcess_Status.Trim().Equals("4") || sessionOBJ.CaseProcess_Status.Trim().Equals("5"))
        //if (sessionOBJ.CaseProcess_Status.Trim().Equals("4") || sessionOBJ.CaseProcess_Status.Trim().Equals("5"))
        //if (sessionOBJ.CaseProcess_Status.Trim().Equals("4") || sessionOBJ.CaseProcess_Status.Trim().Equals("5") || (sessionOBJ.CaseProcess_Status.Trim().Equals("3") && !sessionOBJ.OriginalRiskRanking.Trim().Equals("H")))

        //20220324_Ares_Jack_異常結案不分結案類別, 高風險審核層級應至二階主管審核放行_Start
        //if (!(lblSR_RiskLevel.Text.Trim() == "高風險"))
        //{
        //btnApply.Visible = false;
        //btnFinish.Visible = true;
        //}
        //else
        //{
            //if (sessionOBJ.CaseProcess_User == "C1" && userRolls.Contains("CSIP0121") && (lblSR_RiskLevel.Text == "高風險" || (sessionOBJ.CaseProcess_Status.Trim().Equals("3") && sessionOBJ.OriginalRiskRanking.Trim().Equals("H"))))
            if (sessionOBJ.CaseProcess_User == "C1" && userRolls.Contains("CSIP0121") && lblSR_RiskLevel.Text == "高風險")
            {
                btnApply.Visible = true;
                btnFinish.Visible = false;
            }
        //}
        //20220324_Ares_Jack_異常結案不分結案類別, 高風險審核層級應至二階主管審核放行_End

        return isMaster;
    }

    /// <summary>
    /// 綁定資料
    /// </summary>
    private void BindGridView()
    {
        try
        {
            if (ViewState["DataBind"] != null)
            {
                DataTable dtblResult = (DataTable)ViewState["DataBind"];
                if (dtblResult.Rows.Count > 0)
                {
                    this.gpNoteLogList.Visible = true;
                    this.grdNoteLog.Visible = true;
                    this.gpNoteLogList.RecordCount = dtblResult.Rows.Count;
                    this.grdNoteLog.DataSource = CommonFunction.Pagination(dtblResult, this.gpNoteLogList.CurrentPageIndex, this.gpNoteLogList.PageSize);
                    this.grdNoteLog.DataBind();
                }
                else
                {
                    this.grdNoteLog.DataSource = CommonFunction.Pagination(dtblResult, this.gpNoteLogList.CurrentPageIndex, this.gpNoteLogList.PageSize);
                    this.grdNoteLog.DataBind();
                    this.gpNoteLogList.Visible = false;
                    this.grdNoteLog.Visible = false;
                }
            }
        }
        catch (Exception ex)
        {
            //合併檔案調整
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex, LogLayer.UI);
        }
    }
    /// <summary>
    /// 換頁方法
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpNoteLogList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpNoteLogList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }
    /// <summary>
    /// 檢查user登入物件 初始化字典
    /// </summary>
    protected void checkUserAgent()
    {
        if (eAgentInfo == null)
        {
            //UPDATEPANLE 自動POSTBACK 不會經過PAGE_LOAD 必須重取 eAgentInfo
            eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        }
        ///建立所有字典
        buiInfoDict();
    }
    //顯示基本資料 
    private void showBaseData()
    {
        checkUserAgent();
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
        if (sessionOBJ == null)
        {
            string NavigateUrl = "P010801000001.aspx";
            string urlString = @"alert('查無資料');location.href='" + NavigateUrl + "';";
            base.sbRegScript.Append(urlString);
            return;
        }
        //讀取表頭
        AML_SessionState QueryRes = BRAML_HQ_Work.getProjDetailHeader(sessionOBJ);

        this.SetVal<AML_SessionState>(QueryRes, false);
        //表頭特殊欄位處理
        setSpeciCollHead(QueryRes);


        //讀取公司資料 HQ_WORK CDATA,EDATA
        EntityAML_HQ_Work hqObj = BRAML_HQ_Work.getHQ_WOrk(sessionOBJ);
        sessionOBJ.ID = hqObj.ID;
        //20191111-RQ-2018-015749-002 modify by Peggy
        sessionOBJ.CaseProcess_Status = hqObj.CaseProcess_Status;        
        //更新SESSION 次級編輯用
        Session["P010801000001_SESSION"] = sessionOBJ;
        this.SetVal<EntityAML_HQ_Work>(hqObj, false);
        //HQ特殊欄位處理
        setSpeciCollHQ(hqObj);

        // 2021/7/6 EOS_AML(NOVA) by Ares Dennis
        //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
        //if (!string.IsNullOrEmpty(hqObj.GROUP_NO))
        //{
        //    string relatedCaseNo = "";
        //    DataTable dtCaseNo = BRAML_HQ_Work.getRelatedCaseNo(hqObj.GROUP_NO);
        //    for (int i = 0; i < dtCaseNo.Rows.Count; i++)
        //    {
        //        if (relatedCaseNo == "")
        //        {
        //            relatedCaseNo = dtCaseNo.Rows[i][0].ToString();
        //        }
        //        else
        //        {
        //            relatedCaseNo += "," + dtCaseNo.Rows[i][0].ToString();
        //        }
        //        ID.Add(dtCaseNo.Rows[i][1].ToString());
        //    }
        //    hlblRelateCaseNo.Text = relatedCaseNo;
        //}
        //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis

        //20191210-RQ-2018-015749-002-異常結案的沒做SCDD，所以要ENABLED掉
        if (hqObj.CaseProcess_Status.Trim().Equals("3") || hqObj.CaseProcess_Status.Trim().Equals("4") || hqObj.CaseProcess_Status.Trim().Equals("5"))
        {
            Btn_SCCD_View.Enabled = false;
        }
        else
            Btn_SCCD_View.Enabled = true;

        //20191108-RQ-2018-015749-002 modify by Peggy
        CustLabel myControl = FindControl("CustLabel60") as CustLabel;
        switch (hqObj.CaseProcess_Status.Trim())
        {
            case "3"://不合作結案
            case "13":
                myControl.Text = "不合作結案放行：不合作結案";
                    break;
            case "4"://異常結案
            case "14":
                myControl.Text = "異常結案放行：商店解約結案";
                break;
            case "5"://其他結案
            case "15":
                myControl.Text = "其他結案放行：其他結案";
                break;
            default:
                myControl.Text = BaseHelper.GetShowText("01_01080102_078");//"案件歷程註記";
                break;
        }

        // 20220415 個/法人的SCDD C檔TABLE 主管頁面看到的要是KEEP住經辦送出時的資料，故將原邏輯註解 Start By Kelton
        ////讀取風險資料 Cdata_Work
        //EntityAML_Cdata_Work cDobj = BRAML_Cdata_Work.getCData_WOrk(sessionOBJ);
        //this.SetVal<EntityAML_Cdata_Work>(cDobj, false);
        // 20220415 個/法人的SCDD C檔TABLE 主管頁面看到的要是KEEP住經辦送出時的資料，故將原邏輯註解 Start By Kelton

        // 20220415 先查詢 BRAML_Cdata_Work_S by Kelton
        EntityAML_Cdata_Work_S cDSobj = BRAML_Cdata_Work_S.getCData_WOrk(new AML_SessionState { CASE_NO = sessionOBJ.CASE_NO });

        // 20220415 若 BRAML_Cdata_Work_S 沒有對應資料才查詢 BRAML_Cdata_Work 的 C 檔資料 by Kelton
        if (cDSobj == null)
        {
            //讀取風險資料 Cdata_Work
            EntityAML_Cdata_Work cDobj = BRAML_Cdata_Work.getCData_WOrk(sessionOBJ);
            this.SetVal<EntityAML_Cdata_Work>(cDobj, false);
        }
        else // 20220415 若有 BRAML_Cdata_Work_S 的 C 檔資料，則使用 BRAML_Cdata_Work_S 的資料 by Kelton
        {
            this.SetVal<EntityAML_Cdata_Work_S>(cDSobj, false);
        }

        //讀取分公司明細
        List<EntityAML_BRCH_Work> BRCHColl = BRAML_BRCH_Work.getBRCH_WOrkColl(sessionOBJ);
        grvCardData.DataSource = BRCHColl;
        grvCardData.DataBind();

        //讀取高階管理人暨實質受益人資料
        List<EntityAML_HQ_Manager_Work> HQMAColl = BRAML_HQ_Manager_Work.getHQMA_WorkColl(sessionOBJ);
        grvManagerData.DataSource = HQMAColl;
        grvManagerData.DataBind();

        //讀取名單掃描結果
        EntityHQ_SCDD sccdObj = BRHQ_SCDD.getSCDDData_WOrk(sessionOBJ);
        this.SetVal<EntityHQ_SCDD>(sccdObj, false);
        //名單掃描結果對應
        SCDDlblNameCheck_Item.Text = GetDcValue("RN_" + SCDDlblNameCheck_Item.Text);


        EntitySCDDReport sccdRObj = BRSCDDReport.getSCDDReport(sessionOBJ);
        this.SetVal<EntitySCDDReport>(sccdRObj, false);
        //轉換日期格式
        if (lblSR_DateTime.Text != "")
        {
            lblSR_DateTime.Text = Convert.ToDateTime(lblSR_DateTime.Text).ToString("yyyyMMdd");
        }

        //20191231-RQ-2019-030155-002
        //繫結MFA資訊
        EntityAML_HQ_MFAF MfafObj = BRAML_HQ_MFAF.getMFAFData_WOrk(sessionOBJ);
        this.SetVal<EntityAML_HQ_MFAF>(MfafObj, false);

        //讀取案件歷程聯絡註記
        DataTable dt = BRNoteLog.getNoteLog(sessionOBJ, "1");
        //將結果設定為ViewState
        ViewState["DataBind"] = dt;
        BindGridView();

    }
    /// <summary>
    /// HEAD需要特殊處理的欄位
    /// </summary>
    /// <param name="hqObj"></param>
    private void setSpeciCollHead(AML_SessionState hDObj)
    {
        //風險等級
        hlblOriginalRiskRanking.Text = GetDcValue("RS_" + hDObj.OriginalRiskRanking);
        //20200227-RQ-2019-030155-003 新增最新試算後的風險等級
        hlblNewRiskRanking.Text = GetDcValue("RS_" + hDObj.NewRiskRanking);

        if (string.IsNullOrEmpty(hlblCaseExpiryDate.Text))
        {
            //依建案日期計算到期日 cell6 及 寄送不合作信函日期 cell8
            string pDate = hDObj.DataDate;
            string endDate = "";
            string noCropDate = "";
            proceDate(pDate, ref endDate, ref noCropDate);
            hlblCaseExpiryDate.Text = endDate;
        }
        //案件類別轉換
        hlblCaseType.Text = GetDcValue("CSTYPE_" + hlblCaseType.Text);
    }
    /// <summary>
    /// HQ需要特殊處理的欄位
    /// </summary>
    /// <param name="hqObj"></param>
    private void setSpeciCollHQ(EntityAML_HQ_Work hqObj)
    {
        if (hqObj == null)
        {
            return;
        }
        //設定行業別中文名稱
        DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", hqObj.HCOP_CC);
        if (result.Rows.Count > 0)
        {
            HQlblHCOP_CC_Cname.Text = result.Rows[0]["CODE_NAME"].ToString();
        }
        ///轉換民國年
        if (hqObj.HCOP_OWNER_ID_ISSUE_DATE != null && hqObj.HCOP_OWNER_ID_ISSUE_DATE.Length == 8)
        {
            hqObj.HCOP_OWNER_ID_ISSUE_DATE = ConvertToROCYear(hqObj.HCOP_OWNER_ID_ISSUE_DATE);
            HQlblHCOP_OWNER_ID_ISSUE_DATE.Text = hqObj.HCOP_OWNER_ID_ISSUE_DATE;
        }
        ///負責人身分證件類型         欄位對應  HQlblHCOP_OWNER_ID_Type

        ///暫未處理

        //公司狀態
        HQlblHCOP_STATUS.Text = GetDcValue("OC_" + hqObj.HCOP_STATUS);
        //公司型態 
        HQlblHCOP_CORP_TYPE.Text = GetDcValue("CP_" + hqObj.HCOP_CORP_TYPE);
        //法律形式
        HQlblHCOP_BUSINESS_ORGAN_TYPE.Text = GetDcValue("LW_" + hqObj.HCOP_BUSINESS_ORGAN_TYPE);

        //僑外資 / 外商
        HQlblHCOP_OVERSEAS_FOREIGN.Text = GetDcValue("YN_" + hqObj.HCOP_OVERSEAS_FOREIGN);

        //複雜股權結構
        HQlblHCOP_COMPLEX_STR_CODE.Text = GetDcValue("YN_" + hqObj.HCOP_COMPLEX_STR_CODE);
        //是否可發行無記名股票
        HQlblHCOP_ALLOW_ISSUE_STOCK_FLAG.Text = GetDcValue("YN_" + hqObj.HCOP_ALLOW_ISSUE_STOCK_FLAG);
        //是否已發行無記名股票
        HQlblHCOP_ISSUE_STOCK_FLAG.Text = GetDcValue("YN_" + hqObj.HCOP_ISSUE_STOCK_FLAG);

        if (!string.IsNullOrEmpty(HQlblHCOP_REGISTER_NATION.Text) && HQlblHCOP_REGISTER_NATION.Text == "US")
        {
            //美國州別轉換 國別是美國才轉
            HQlblHCOP_REGISTER_US_STATE.Text = GetDcValue("STTYPE_" + hqObj.HCOP_REGISTER_US_STATE);
        }
        
        //總公司負責人領補換
        HQlblHCOP_OWNER_ID_REPLACE_TYPE.Text = GetDcValue("ID_" + hqObj.HCOP_OWNER_ID_REPLACE_TYPE);
        //總公司負責人有無照片
        HQlblHCOP_ID_PHOTO_FLAG.Text = GetDcValue("HSP_" + hqObj.HCOP_ID_PHOTO_FLAG);
        ////總公司負責人身分證件類型
        //HQlblOWNER_ID_Type.Text = GetDcValue("DT_" + hqObj.OWNER_ID_Type);

        //設立日期轉民國年
        if (HQlblHCOP_BUILD_DATE.Text.Length == 8)
        {
            HQlblHCOP_BUILD_DATE.Text = ConvertToROCYear(HQlblHCOP_BUILD_DATE.Text);
        }
        //公司負責人長姓名顯示
        if (!string.IsNullOrEmpty(HQlblHCOP_OWNER_CHINESE_LNAME.Text) || !string.IsNullOrEmpty(HQlblHCOP_OWNER_ROMA.Text))
        {
            cmpRname.Style["display"] = "";
            cmpLname.Style["display"] = "";
        }
        //聯絡人長姓名顯示
        if (!string.IsNullOrEmpty(HQlblHCOP_CONTACT_LNAME.Text) || !string.IsNullOrEmpty(HQlblHCOP_CONTACT_ROMA.Text))
        {
            cmpRname1.Style["display"] = "";
            cmpLname1.Style["display"] = "";
        }
    }
    int grvCardDataSubRowCount = 0;
    /// <summary>
    /// 分公司負責人資料綁定處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvCardData_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //DataRowView寫法
            //System.Data.DataRowView rowView = (DataRowView)e.Row.DataItem;
            //string idDate = rowView["BRCH_OWNER_ID_ISSUE_DATE"].ToString();

            //用LIST進來，就不適用 DataRowView 必須用原型別操作資料
            EntityAML_BRCH_Work rowView = (EntityAML_BRCH_Work)e.Row.DataItem;
            //轉換民國年
            string idDate = rowView.BRCH_OWNER_ID_ISSUE_DATE;
            e.Row.Cells[4].Text = ConvertToROCYear(idDate);
            e.Row.Cells[6].Text = GetDcValue("ID_" + rowView.BRCH_OWNER_ID_REPLACE_TYPE);
            e.Row.Cells[7].Text = GetDcValue("HSP_" + rowView.BRCH_ID_PHOTO_FLAG);

            //長姓名區
            //if (rowView.BRCH_CHINESE_LNAME.Length > 5)
            if (string.IsNullOrEmpty(rowView.BRCH_CHINESE_LNAME)==false || string.IsNullOrEmpty(rowView.BRCH_ROMA)==false)
            {
                ControlCollection _table = e.Row.Parent.Controls; //e.Row.Parent就是Table
                int _columnSpan = 14;
                GridViewRow gvr = new GridViewRow(-1, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
                TableCell cell_label = new TableCell();
                TableCell cell_value = new TableCell();
                cell_label.Text = BaseHelper.GetShowText("01_01080103_059");
                cell_label.ColumnSpan = 2;
                cell_value.ColumnSpan = _columnSpan;
                cell_value.Text = rowView.BRCH_CHINESE_LNAME;
                cell_label.HorizontalAlign = HorizontalAlign.Center;
                gvr.Cells.Add(cell_label);
                gvr.Cells.Add(cell_value);
                grvCardDataSubRowCount += 2;

                _table.AddAt(grvCardDataSubRowCount, gvr);

                gvr = new GridViewRow(-1, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
                cell_label = new TableCell();
                cell_value = new TableCell();
                cell_label.HorizontalAlign = HorizontalAlign.Center;
                cell_label.Text = BaseHelper.GetShowText("01_01080103_060");
                cell_label.ColumnSpan = 2;
                cell_value.ColumnSpan = _columnSpan;
                cell_value.Text = rowView.BRCH_ROMA;
                gvr.Cells.Add(cell_label);
                gvr.Cells.Add(cell_value);
                grvCardDataSubRowCount += 1;
                _table.AddAt(grvCardDataSubRowCount, gvr);
            }

        }
    }



    /// <summary>
    /// 轉換高階經理人暨實質受益人身分別文字
    /// </summary>
    /// <returns></returns>
    private string ConvManagerIdTypeToStr(EntityAML_HQ_Manager_Work rowView)
    {
        string result = "";
        if (rowView.HCOP_BENE_JOB_TYPE == "Y")
        {
            result += "1董/理事、監事/監察人，";
        }
        if (rowView.HCOP_BENE_JOB_TYPE_2 == "Y")
        {
            result += "2總經理，";
        }
        if (rowView.HCOP_BENE_JOB_TYPE_3 == "Y")
        {
            result += "3財務長，";
        }
        if (rowView.HCOP_BENE_JOB_TYPE_4 == "Y")
        {
            result += "4有權簽章人，";
        }
        if (rowView.HCOP_BENE_JOB_TYPE_5 == "Y")
        {
            result += "5合夥人、實質受益人，";//20200921
        }
        if (rowView.HCOP_BENE_JOB_TYPE_6 == "Y")
        {
            result += "6其他關聯人，";
        }
        if (result.Length > 1)
        {
            //移除最後一個逗號
            result = result.Remove(result.Length - 1, 1);
        }
        return result;
    }
    /// <summary>
    /// 設定本頁各GRID行數
    /// </summary>
    private void setGridLine()
    {
        //*設置GridView分頁顯示的行數
        int intPageSize = 15;
        //聯繫註記
        gpNoteLogList.PageSize = 5;
        grdNoteLog.PageSize = 5;


        //分公司負責人資料
        grvCardData.PageSize = intPageSize;

        //經理人12
        grvManagerData.PageSize = 12;

    }
    /// <summary>
    /// 實作字典
    /// </summary>
    private void buiInfoDict()
    {
        DCCommonColl = new Dictionary<string, string>();
        // 通用項目
        DCCommonColl.Add("YN_1", "是");
        DCCommonColl.Add("YN_0", "否");
        DCCommonColl.Add("YN_Y", "是");
        DCCommonColl.Add("YN_N", "否");
        DCCommonColl.Add("HS_1", "有");
        DCCommonColl.Add("HS_0", "無");
        DCCommonColl.Add("HS_Y", "有");
        DCCommonColl.Add("HS_N", "無");
        DCCommonColl.Add("HSP_0", "有"); //有無照片是反過來的
        DCCommonColl.Add("HSP_1", "無");
        DCCommonColl.Add("HSP_Y", "有");
        DCCommonColl.Add("HSP_N", "無");

        //身分證領補換類別  編碼ID_
        //DCCommonColl.Add("ID_1", "初");
        // DCCommonColl.Add("ID_2", "補");
        // DCCommonColl.Add("ID_3", "換");
        setFromCodeType("4", "ID");

        //身分證件類型  編碼DT_
        DCCommonColl.Add("DT_1", "身分證");
        DCCommonColl.Add("DT_2", "護照");
        DCCommonColl.Add("DT_3", "統一證號");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
        DCCommonColl.Add("DT_4", "其他");


        //法律形式 編碼 LW_
        //DCCommonColl.Add("LW_01", "政府組織");
        //DCCommonColl.Add("LW_02", "金融機構");
        //DCCommonColl.Add("LW_03", "無限公司");
        //DCCommonColl.Add("LW_04", "有限公司");
        //DCCommonColl.Add("LW_05", "兩合公司");
        //DCCommonColl.Add("LW_06", "股份有限公司");
        //DCCommonColl.Add("LW_07", "閉鎖性股份有限公司");
        //DCCommonColl.Add("LW_08", "財團法人");
        //DCCommonColl.Add("LW_09", "社團法人");
        //DCCommonColl.Add("LW_10", "獨資：個人開立之商號");
        //DCCommonColl.Add("LW_11", "有限合夥");
        //DCCommonColl.Add("LW_12", "合夥：二人以上出資合作經營事業");
        //DCCommonColl.Add("LW_13", "信託");
        //DCCommonColl.Add("LW_14", "團體或組織：如公寓大廈管理委員會");
        //DCCommonColl.Add("LW_15", "其他");
        setFromCodeType("2", "LW");

        ///風險等級 RS_
        DCCommonColl.Add("RS_H", "高");
        DCCommonColl.Add("RS_M", "中");
        DCCommonColl.Add("RS_L", "低");
        //開關 OC_

        DCCommonColl.Add("OC_O", "OPEN");
        DCCommonColl.Add("OC_C", "Close");

        //公司型態 CP_
        DCCommonColl.Add("CP_1", "本國公司");
        DCCommonColl.Add("CP_4", "外國公司");

        //聯絡資訊 CN_         
        DCCommonColl.Add("CN_HeadquartersInfo", "編輯審查維護");
        DCCommonColl.Add("CN_BranchInfo", "編輯分公司資料");
        DCCommonColl.Add("CN_HeadquartersSCDD", "編輯審查維護2");
        DCCommonColl.Add("CN_SCDD", "編輯SCDD頁");
        DCCommonColl.Add("CN_EDD1", "編輯非自然人EDD表單");
        DCCommonColl.Add("CN_EDD2", "編輯非營利EDD表單");
        DCCommonColl.Add("CN_Verify", "結案送審(上呈)");
        DCCommonColl.Add("CN_ReturnCase", "案件退回");
        DCCommonColl.Add("CN_CaseOK", "案件放行");
        DCCommonColl.Add("CN_NotCooperate", "不合作註記");
        DCCommonColl.Add("CN_Note", "連絡註記");
        DCCommonColl.Add("CN_BranchEditStatus", "分公司更新狀態");
        DCCommonColl.Add("CN_ReturnOK", "放行(結案)更新狀態");
        //20191104-RQ-2018-015749-002 ADD BY Peggy
        DCCommonColl.Add("CN_NonCooperated", "【不合作結案】送審上呈");
        DCCommonColl.Add("CN_CaseClosed", "【商店解約結案】送審上呈");
        DCCommonColl.Add("CN_OtherClosed", "【其他結案】送審上呈");
        DCCommonColl.Add("CN_RejectNonCooperated", "【不合作結案】案件退回");
        DCCommonColl.Add("CN_RejectCaseClosed", "【商店解約結案】案件退回");
        DCCommonColl.Add("CN_RejectOtherClosed", "【其他結案】案件退回");

        //DCCommonColl.Add("RN_1", "徵提公司章程或類似權力文件");
        //DCCommonColl.Add("RN_2", "未徵提章程或類似權力文件，請說明組織運作及權限決定方式");
        setFromCodeType("5", "RN");

        DCCommonColl.Add("CSTYPE_A", "用戶預定的案件");
        DCCommonColl.Add("CSTYPE_B", " 已到達審查日期");
        DCCommonColl.Add("CSTYPE_C", "客戶的風險等級被升級或者被降級");
        DCCommonColl.Add("CSTYPE_D1", "選擇 X 比率（%）的高風險客戶");
        DCCommonColl.Add("CSTYPE_D2", "選擇高風險分數大過或等於 X 的客戶 ");
        DCCommonColl.Add("CSTYPE_D3", "選擇前 X 的高風險客戶");

        //DCCommonColl.Add("STTYPE_TX", "德克薩斯州(TX)");
        //DCCommonColl.Add("STTYPE_NM", "新墨西哥州(NM)");
        //DCCommonColl.Add("STTYPE_AZ", "亞利桑那州(AZ)");
        //DCCommonColl.Add("STTYPE_CA", "加利福尼亞州(CA)");
        //DCCommonColl.Add("STTYPE_DE", "達拉瓦州(DE)");
        //DCCommonColl.Add("STTYPE_NV", "內華達州(NV)");
        //DCCommonColl.Add("STTYPE_", "非前述州別");
        setFromCodeType("8", "STTYPE");

    }
    /// <summary>
    /// 取出指定字典的值
    /// </summary>
    /// <param name="inObj"></param>
    /// <param name="inKey"></param>
    /// <returns></returns>
    private string GetDcValue(string inKey)
    {
        if (DCCommonColl == null)
        {
            checkUserAgent();
        }
        string rtnVal = "";
        if (DCCommonColl.ContainsKey(inKey))
        {
            rtnVal = DCCommonColl[inKey];
        }
        return rtnVal;
    }
    /// <summary>
    /// 將指定CODETYPE鍵入字典中
    /// </summary>
    /// <param name="codeType"></param>
    private void setFromCodeType(string codeType, string insKey)
    {
        DataTable result = BRPostOffice_CodeType.GetCodeType(codeType);
        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                string sKey = insKey + "_" + result.Rows[i][0].ToString();
                if (!DCCommonColl.ContainsKey(sKey))
                {
                    DCCommonColl.Add(sKey, result.Rows[i][1].ToString());
                }
            }
        }

    }
    /// <summary>
    /// 計算到期日與寄送通知日
    /// </summary>
    /// <param name="endDate"></param>
    /// <param name="noCropDate"></param>
    private void proceDate(string pDate, ref string endDate, ref string noCropDate)
    {
        if (pDate.Length == 8)
        {
            try
            {
                string sD = pDate.Substring(0, 4) + "-" + pDate.Substring(4, 2) + "-" + pDate.Substring(6, 2);
                DateTime Ds = Convert.ToDateTime(sD);
                // 20210819_Are_Stanley-經姵晴確認將日期應調整案件明細到期日+5個月
                string eDtmp = Ds.AddMonths(5).ToString("yyyy/MM/01");
                endDate = Convert.ToDateTime(eDtmp).AddDays(-1).ToString("yyyyMMdd");
                //以到期日計算，先減5天，若為假日，(六、日)則依假日遞減回前一個工作日(周五)
                DateTime nCpTemp = Convert.ToDateTime(eDtmp).AddDays(-5);
                switch (nCpTemp.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        nCpTemp = nCpTemp.AddDays(-2);
                        break;
                    case DayOfWeek.Saturday:
                        nCpTemp = nCpTemp.AddDays(-1);
                        break;
                }
                noCropDate = nCpTemp.ToString("yyyyMMdd");
            }
            catch (Exception ex)
            {
                ///不寫LOG
                string res = ex.Message;
            }
        }
    }
    #endregion


}