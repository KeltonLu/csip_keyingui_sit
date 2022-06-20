// *****************************************************************
//   作    者：林家賜
//   功能說明：AML資料 案件明細(經辦)
//   創建日期：2019/02/03
//   修改記錄：2021/06/03 異常結案調整 異常結案 頁面轉向修正，避免造成瀏覽器雙開異常
// <author>            <time>            <TaskID>                <desc>
// ******************************************************************

using System;
using System.Data;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Framework.Common.Utility;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using CSIPNewInvoice.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules_new;
using Framework.Common.Logging;
using Framework.Data.OM.Transaction;
using System.Text;
using System.Collections;//20191218 為讀主機電文使用
using System.Reflection;
using Framework.Data.OM.OMAttribute;
using CSIPCommonModel.EntityLayer_new;

public partial class Page_P010801010001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    private const string thisPageName = "P010801010001.aspx";
    private List<string> ID = new List<string>();//關聯案件序號ID
    #region  轉換代碼用字典 

    /// <summary>
    /// 通用字典，各項次機能以前兩碼區分
    /// </summary>
    Dictionary<string, string> DCCommonColl;

    #endregion
    private EntityAML_Cdata_Work_S tempcDSobj = new EntityAML_Cdata_Work_S(); // 20220413 暫存查詢到的 C 檔資料，送審時要寫入 AML_Cdata_Work_S By Kelton
    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        checkUserAgent();
        if (!IsPostBack)
        {

            ///設定本頁使用GRID頁數
            setGridLine();
            // CommonFunction.SetControlsEnabled(pnlText, false);// 清空網頁中所有的輸入欄位
            //   base.sbRegScript.Append(BaseHelper.SetFocus("txtTaxID"));// 將 總公司/總店統編 設為輸入焦點
            showBaseData(); //非POSTBACK，則讀取資料
        }

        // eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合

    }



    /// <summary>
    /// 分公司負責人資料命令處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvCardData_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Show")
        {
            LinkButton lnkView = (LinkButton)e.CommandSource;
            GridViewRow rowView = (GridViewRow)lnkView.NamingContainer;
            string[] keys = e.CommandArgument.ToString().Split(';');
            AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
            if (keys.Length != 3) //參數錯誤，不處理
            {
                return;
            }
            sessionOBJ.RMMBatchNo = keys[0];
            sessionOBJ.AMLInternalID = keys[1];
            sessionOBJ.BRCHID = keys[2];
            Session["P010801010001_Last"] = thisPageName;
            Session["P010801010001_SESSION"] = sessionOBJ;
            string NavigateUrl = "P010801040001.aspx";
            //20210412_Ares_Stanley-調整轉向方法
            Response.Redirect(NavigateUrl, false);

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
            //  e.Row.Cells[2].Text = eAgentInfo.agent_name;

        }
    }




    /// <summary>
    /// 總公司資料編輯
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void BtnHQ_Modify_Click(object sender, EventArgs e)
    {
        Session["P010801010001_Last"] = thisPageName;
        //20210412_Ares_Stanley-調整轉向方法
        Response.Redirect("P010801030001.aspx", false);
    }
    /// <summary>
    ///總公司 SCDD 資料編輯 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void BtnHQ_SCCD_Modify_Click(object sender, EventArgs e)
    {
        Session["P010801010001_Last"] = thisPageName;
        //20210412_Ares_Stanley-調整轉向方法
        Response.Redirect("P010801050001.aspx", false);
    }
    /// <summary>
    /// SCDD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Btn_SCCD_Modify_Click(object sender, EventArgs e)
    {
        if (SCDDlblNameCheck_No.Text == "")
        {

            string strAlertMsgR = MessageHelper.GetMessages("01_01080101_008");
            sbRegScript.Append("alert('" + strAlertMsgR + "');");
            return;
        }

        Session["P010801010001_Last"] = thisPageName;
        //20210412_Ares_Stanley-調整轉向方法
        Response.Redirect("P010801060001.aspx", false);
    }

    /// <summary>
    /// 返回
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        //20210412_Ares_Stanley-調整轉向方法
        Response.Redirect("P010801000001.aspx", false);
    }
    //聯絡註記編輯
    protected void btnNOTEEdit_Click(object sender, EventArgs e)
    { //20210412_Ares_Stanley-調整轉向方法
        Response.Redirect("P010801100001.aspx", false);
    }
    /// <summary>
    /// 送審
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnApply_Click(object sender, EventArgs e)
    {
        //檢核  無SCDDReport日期不可送審
        if (string.IsNullOrEmpty(lblSR_DateTime.Text))
        {
            string strAlertMsg = MessageHelper.GetMessages("01_01080101_003");
            sbRegScript.Append("alert('" + strAlertMsg + "');");
            return;
        }
        if (!ValidLowType(HQlblHCOP_BUSINESS_ORGAN_TYPE.Text))
        {
            string strAlertMsg = MessageHelper.GetMessages("01_01080101_004");
            sbRegScript.Append("alert('" + strAlertMsg + "');");
            return;
        }

        //送審要變更 CaseOwner_User 經辦，以目前帶入 MI等級變更為C1
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];
        if (sessionOBJ == null || eAgentInfo == null)
        {
            string strAlertMsg = MessageHelper.GetMessages("01_01080101_002");
            sbRegScript.Append("alert('" + strAlertMsg + "');window.location.href = 'P010801000001.aspx';");
            return;
        }
        ;
        sessionOBJ.CaseOwner_User = eAgentInfo.agent_id;
        sessionOBJ.CaseProcess_User = "C1"; //一級主管
        sessionOBJ.CaseProcess_Status = "0";//處理中
        bool result;
        using (OMTransactionScope ts = new OMTransactionScope())
        {

            result = BRAML_HQ_Work.Update_Apply(sessionOBJ, "2"); //送審不動經辦

            //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
            //2021/7/6 EOS_AML(NOVA) 關聯案件一併送出 by Ares Dennis 
            //foreach (string id in ID)
            //{
            //    sessionOBJ.ID = id;
            //    result = BRAML_HQ_Work.Update_Apply(sessionOBJ, "2"); //送審不動經辦
            //}
            //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis

            if (!result)
            {
                string strAlertMsg = MessageHelper.GetMessages("01_01080101_005");
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
            notrLog.NL_Value = "送審";
            result = BRNoteLog.Insert(notrLog);
            //調整為提示訊息
            if (!result)
            {
                string strAlertMsg = MessageHelper.GetMessages("01_01080101_006");
                sbRegScript.Append("alert('" + strAlertMsg + "');");

                return;
            }

            // 20220413 先將 AML_Cdata_Work_S 已存在的資料刪除，再將 C 檔資料寫入 AML_Cdata_Work_S by Kelton
            BRAML_Cdata_Work_S.Delete(new AML_SessionState { CASE_NO = sessionOBJ.CASE_NO });
            tempcDSobj = (EntityAML_Cdata_Work_S)this.Session["P010801010001_cDs"];
            BRAML_Cdata_Work_S.Insert(tempcDSobj);

            ts.Complete();
        }

        //提示作業完成
        string strAlertMsgR = MessageHelper.GetMessages("01_01080101_007");
        sbRegScript.Append("alert('" + strAlertMsgR + "');window.location.href = 'P010801000001.aspx';");
        return;

    }


    #endregion

    #region 方法區
    /// <summary>
    /// 檢查法律形式是否在排他清單
    /// </summary>
    /// <param name="lawTypeName"></param>
    /// <returns></returns>
    private bool ValidLowType(string lawTypeName)
    {
        bool result = false;
        DataTable dt = BRPostOffice_CodeType.GetCodeType("2");//20210115-修正抓取正確的法律型式設定

        string Fillter = "CODE_NAME = '" + lawTypeName.Trim() + "'";
        DataRow[] dr = dt.Select(Fillter);
        if (dr.Length > 0)
        {
            result = true;
        }
        
        //foreach (DataRow dr in dt.Rows)
        //{
        //    if (dr["CODE_NAME"].ToString() == lawTypeName)
        //    {
        //        result = true;
        //        break;
        //    }
        //}
        return result;
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
        try
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
            EntityAML_HQ_Work hqObj = BRAML_HQ_Work.getHQ_WOrk(sessionOBJ); //待加長姓名相關
            sessionOBJ.ID = hqObj.ID;
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

            //讀取風險資料 Cdata_Work
            EntityAML_Cdata_Work cDobj = BRAML_Cdata_Work.getCData_WOrk(sessionOBJ);
            this.SetVal<EntityAML_Cdata_Work>(cDobj, false);

            // 20220413 將查詢到的 C 檔資料暫存，最後送審時要寫入 AML_Cdata_Work_S By Kelton
            tempcDSobj = cDobj.toSMode();
            tempcDSobj.CASE_NO = sessionOBJ.CASE_NO;
            this.Session["P010801010001_cDs"] = tempcDSobj;

            //讀取分公司明細
            List<EntityAML_BRCH_Work> BRCHColl = BRAML_BRCH_Work.getBRCH_WOrkColl(sessionOBJ); //待加 長姓名相關
            grvCardData.DataSource = BRCHColl;
            grvCardData.DataBind();

            //20191114-RQ-2018-015749-002
            gdvBRCHPerAMT.DataSource = BRCHColl;
            gdvBRCHPerAMT.DataBind();
            //GetBRCHMonthAMT


            //讀取高階管理人暨實質受益人資料
            List<EntityAML_HQ_Manager_Work> HQMAColl = BRAML_HQ_Manager_Work.getHQMA_WorkColl(sessionOBJ);
            //待加 長姓名相關,但如何在列表上怎麼呈現 =>  原本的姓名欄位長度已增加至 nvarchar(100)
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

            //20191114-RQ-2018-015749-002
            DataTable dt_Comp = BRAML_HQ_Work.GetHQMonthAMT(sessionOBJ.CASE_NO); //待加長姓名相關
            gdvHCOPPerAMT.DataSource = dt_Comp;
            gdvHCOPPerAMT.DataBind();

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
        catch (Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex);
            sbRegScript.Append("alert('" + strClientMsg + "');window.location.href = 'P010801000001.aspx';");
            return;
        }
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

        Session["0801_OWNER_ID_SreachStatus"] = hqObj.OWNER_ID_SreachStatus;

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

        // 20220613 調整若 CaseProcess_User 已為 C1 或 C2 時，不顯示 申請送審 按鈕 By Kelton
        string caseProcessUsers = "C1,C2";
        if (caseProcessUsers.Contains(hqObj.CaseProcess_User))
            btnApply.Visible = false;
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
            //20200924-RQ-2020-021027-002
            //e.Row.Cells[4].Text = ConvertToROCYear(idDate);
            //e.Row.Cells[6].Text = GetDcValue("ID_" + rowView.BRCH_OWNER_ID_REPLACE_TYPE);
            //e.Row.Cells[7].Text = GetDcValue("HSP_" + rowView.BRCH_ID_PHOTO_FLAG);
            e.Row.Cells[6].Text = ConvertToROCYear(idDate);
            e.Row.Cells[8].Text = GetDcValue("ID_" + rowView.BRCH_OWNER_ID_REPLACE_TYPE);
            e.Row.Cells[9].Text = GetDcValue("HSP_" + rowView.BRCH_ID_PHOTO_FLAG);

            //長姓名區
            //if (rowView.BRCH_CHINESE_LNAME.Length > 5)
            if (string.IsNullOrEmpty(rowView.BRCH_CHINESE_LNAME) == false || string.IsNullOrEmpty(rowView.BRCH_ROMA) == false)
            {
                ControlCollection _table = e.Row.Parent.Controls; //e.Row.Parent就是Table
                int _columnSpan = 14;
                GridViewRow gvr = new GridViewRow(-1, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
                TableCell cell_label = new TableCell();
                TableCell cell_value = new TableCell();
                cell_label.ColumnSpan = 2;
                cell_label.Text = BaseHelper.GetShowText("01_01080103_059");
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
            else
            {
                grvCardDataSubRowCount += 1;
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
            result += "5合夥人、實質受益人，";//20200921 修改顯示文字
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
            buiInfoDict();
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

    //異常結案-20191104-RQ-2018-015749-002
    protected void btnAbnormal_Click(object sender, EventArgs e)
    {
        //20191104 新流程
        // StringBuilder sbRegScriptF = new StringBuilder("");
        // sbRegScriptF.Append("window.open('P010801010002.aspx?");
        // sbRegScriptF.Append("Property=" + RedirectHelper.GetEncryptParam("C") + "'");
        // sbRegScriptF.Append(",'ReportCompare','height=300px,width=1119px,top=100,left=200,toolbar=no,menubar=no,scrollbars=yes,resizable=yes,location=no,status=no');");
        // this.sbRegScript.Append(sbRegScriptF.ToString());


        //20210426_Ares_Luke-調整頁面轉向方法(避免造成瀏覽器雙開被剔除)
        string NavigateUrl = "P010801010002.aspx";
        Session["P010801010001_Last"] = thisPageName;
        Response.Redirect(NavigateUrl, false);
    }

    //更新特店狀態
    protected void btnUpdateHQWork_Click(object sender, EventArgs e)
    {
        //填基本物件，由電文讀取
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
        if (sessionOBJ == null)
        {
            string NavigateUrl = "P010801000001.aspx";
            if (!string.IsNullOrEmpty(Session["P010801010001_Last"].ToString()))
            {
                NavigateUrl = Session["P010801010001_Last"].ToString();
            }
            strAlertMsg = MessageHelper.GetMessages("01_01080103_019");
            string urlString = @"alert('" + strAlertMsg + "');location.href='" + NavigateUrl + "';";
            base.sbRegScript.Append(urlString);
        }
        this.SetVal<AML_SessionState>(sessionOBJ, false);
        //建立HASHTABLE
        Hashtable JC66OBj = new Hashtable();
        JC66OBj.Add("FUNCTION_CODE", "I");
        JC66OBj.Add("CORP_NO", sessionOBJ.HCOP_HEADQUATERS_CORP_NO);
        JC66OBj.Add("CORP_SEQ", "0000");

        //測試用模擬資料
        Hashtable hstExmsP4A;
        string hesRtn = "";
        bool result;
        // 上送主機取得資料
        hstExmsP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC66, JC66OBj, false, "11", eAgentInfo);
        try
        {
            //檢核若主機回傳無訊息，則提示後離開
            string rMsg = hstExmsP4A["MESSAGE_TYPE"].ToString();
            switch (rMsg)
            {
                case "0000":

                    //取回後MAP至物件  
                    //取回後MAP至物件 注意!本處為編輯模式，所以HASHTABLE對應欄位物件應用edit，以方便直接對應到頁面
                    EntityAML_HQ_Work_edit edObj = HTconvertToObj(hstExmsP4A);
                    EntityAML_HQ_Work insObj = edObj.toShowMode();

                    insObj.ID = sessionOBJ.ID;
                    insObj.Create_Time = DateTime.Now.ToString("HHmmss");
                    insObj.Create_User = eAgentInfo.agent_id;
                    insObj.Create_Date = DateTime.Now.ToString("yyyyMMdd");
                    insObj.HCOP_OWNER_CHINESE_LNAME = "";
                    insObj.HCOP_OWNER_ROMA = "";
                    insObj.HCOP_CONTACT_LNAME = "";
                    insObj.HCOP_CONTACT_ROMA = "";
                    if (!string.IsNullOrEmpty(hstExmsP4A["OWNER_LNAM_FLAG"].ToString()) && hstExmsP4A["OWNER_LNAM_FLAG"].ToString().Trim().Equals("Y"))
                    {
                        EntityHTG_JC68 htReturn_JC68 = GetJC68(hstExmsP4A["OWNER_ID"].ToString().Trim());
                        insObj.HCOP_OWNER_CHINESE_LNAME = htReturn_JC68.LONG_NAME;
                        insObj.HCOP_OWNER_ROMA = htReturn_JC68.PINYIN_NAME;
                    }

                    if (!string.IsNullOrEmpty(hstExmsP4A["CONTACT_LNAM_FLAG"].ToString()) && hstExmsP4A["CONTACT_LNAM_FLAG"].ToString().Trim().Equals("Y"))
                    {
                        EntityHTG_JC68 htReturn_JC68 = GetJC68(sessionOBJ.HCOP_HEADQUATERS_CORP_NO + "0000");
                        insObj.HCOP_CONTACT_LNAME = htReturn_JC68.LONG_NAME;
                        insObj.HCOP_CONTACT_ROMA = htReturn_JC68.PINYIN_NAME;
                    }
                    insObj.HCOP_LAST_UPDATE_DATE = DateTime.Now.ToString("yyyyMMdd");//20220116_Ares_Jack_最後異動日期

                    result = BRAML_HQ_Work.UpdateFromJC66(insObj);
                    
                    if (!result)
                    {
                        strAlertMsg = MessageHelper.GetMessages("01_01080101_010");
                    }
                    else
                    {
                        strAlertMsg = MessageHelper.GetMessages("01_01080101_011");
                    }
                    
                    //寫入案件歷程
                    EntityNoteLog notrLog = new EntityNoteLog();
                    notrLog.NL_CASE_NO = sessionOBJ.CASE_NO;
                    notrLog.NL_SecondKey = "";
                    notrLog.NL_ShowFlag = "1";
                    notrLog.NL_DateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    notrLog.NL_User = eAgentInfo.agent_id;
                    notrLog.NL_Type = "";
                    notrLog.NL_Value = "更新總公司狀態(Button)";
                    result = BRNoteLog.Insert(notrLog);

                    //顯示至頁面
                    showBaseData();

                    break;
                case "0006":
                default:
                    if (!string.IsNullOrEmpty(hstExmsP4A["MESSAGE_CHI"].ToString()))
                    {
                        hesRtn = hstExmsP4A["MESSAGE_CHI"].ToString();
                    }
                    strAlertMsg = MessageHelper.GetMessages("01_01080103_020") + hesRtn;
                    string urlString = @"alert('" + strAlertMsg + "');";
                    break;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            //strAlertMsg = MessageHelper.GetMessages("01_01080103_020") + hesRtn;
            if (!string.IsNullOrEmpty(hstExmsP4A["HtgMsg"].ToString()))//20200410-RQ-2019-030155-005
            {
                if (hstExmsP4A["HtgMsg"].ToString().Contains("704"))
                {
                    strAlertMsg = MessageHelper.GetMessages("00_00000000_040");
                }
                else if (hstExmsP4A["HtgMsg"].ToString().Contains("705"))
                {
                    strAlertMsg = MessageHelper.GetMessages("00_00000000_039");
                }
                else if (hstExmsP4A["HtgMsg"].ToString().Contains("799"))
                {
                    strAlertMsg = MessageHelper.GetMessages("00_00000000_041");
                }
                else
                {
                    strAlertMsg = MessageHelper.GetMessages("01_01080103_020") + hesRtn;
                }
            }

            string urlString = @"alert('" + strAlertMsg + "');";
            this.strHostMsg += strAlertMsg;
        }
        finally
        {
            base.strClientMsg += strAlertMsg;
        }

    }

    //分公司資訊維護
    protected void btnBRCH_Modify_Click(object sender, EventArgs e)
    {
        Session["P010801010001_Last"] = thisPageName;

        string NavigateUrl = "P010801040002.aspx";
        //20210412_Ares_Stanley-調整轉向方法
        Response.Redirect(NavigateUrl, false);
    }

    /// <summary>
    /// 轉換HASHTABLE to EntityAML_HQ_Work_edit物件
    /// </summary>
    /// <param name="inObj"></param>
    /// <returns></returns>
    private EntityAML_HQ_Work_edit HTconvertToObj(Hashtable inObj)
    {
        EntityAML_HQ_Work_edit rtnObj = new EntityAML_HQ_Work_edit();

        //先將主體值找出來，方法是用映射，取出對應欄位
        Type v = rtnObj.GetType();  //取的型別實體
        PropertyInfo[] props = v.GetProperties(); //取出所有公開屬性(可以被外部存取得 
                                                  //處理總公司資料
        foreach (PropertyInfo prop in props)
        {
            object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件
            AttributeRfPage authAttr;
            for (int xi = 0; xi < attrs.Length; xi++)
            {
                if (attrs[xi] is AttributeRfPage)
                {
                    authAttr = attrs[xi] as AttributeRfPage;
                    //暫時用不道
                    //string propName = prop.Name;
                    //string authID = authAttr.ControlID; 
                    string Jc66FieldKey = authAttr.JC66NAME;
                    //JC66有值，建立HASTABLE欄位，
                    if (!string.IsNullOrEmpty(Jc66FieldKey))
                    {
                        if (inObj.ContainsKey(Jc66FieldKey))
                        {
                            prop.SetValue(rtnObj, inObj[Jc66FieldKey], null);
                        }
                    }
                }
            }
        }

        /*
        //轉換子公司資料 12行 由1開始(非慣用的0 因為是行號，非索引)
        for (int i = 1; i < 13; i++)
        {
            EntityAML_HQ_Manager_Work_edit sMan = new EntityAML_HQ_Manager_Work_edit();

            Type Stype = sMan.GetType();  //取的型別實體
            PropertyInfo[] Sprops = Stype.GetProperties(); //取出所有公開屬性(可以被外部存取得 
                                                           //處理總公司資料
            foreach (PropertyInfo prop in Sprops)
            {
                object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件
                AttributeRfPage authAttr;
                for (int xi = 0; xi < attrs.Length; xi++)
                {
                    if (attrs[xi] is AttributeRfPage)
                    {
                        authAttr = attrs[xi] as AttributeRfPage;
                        //暫時用不道
                        //string propName = prop.Name;
                        //string authID = authAttr.ControlID; 
                        //未設定欄位不對應
                        if (string.IsNullOrEmpty(authAttr.JC66NAME))
                        {
                            continue;
                        }
                        string Jc66FieldKey = authAttr.JC66NAME + i.ToString();
                        //JC66有值，建立HASTABLE欄位，
                        if (!string.IsNullOrEmpty(Jc66FieldKey))
                        {
                            if (inObj.ContainsKey(Jc66FieldKey))
                            {
                                prop.SetValue(sMan, inObj[Jc66FieldKey], null);

                            }
                        }
                    }
                }
            }

            //會有12筆
            rtnObj.ManagerColl.Add(sMan);

        }
        */
        return rtnObj;
    }

    //取長姓名的內容值
    private EntityHTG_JC68 GetJC68(string strID)
    {
        EntityHTG_JC68 _result = new EntityHTG_JC68();
        using (BRHTG_JC68 obj = new BRHTG_JC68("P010801010001"))
        {
            EntityHTG_JC68 _data = new EntityHTG_JC68();

            _data.ID = strID;
            _result = obj.getData(_data, eAgentInfo, "11");
        }
        return _result;
    }

    //20200120-修改月請款金額呈現方式
    protected void gdvHCOPPerAMT_PreRender(object sender, EventArgs e)
    {
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
        if (sessionOBJ == null || eAgentInfo == null)
        {
            string strAlertMsg = MessageHelper.GetMessages("01_01080101_002");
            sbRegScript.Append("alert('" + strAlertMsg + "');window.location.href = 'P010801000001.aspx';");
            return;
        }

        int objCaseNO = Convert.ToInt16(sessionOBJ.CASE_NO.Substring(4, 2));
        int mon = 0;
        for (int i = 1; i < 13; i++)
        {
            if (objCaseNO - i > 0)
            {
                mon = objCaseNO - i;
            }
            else
            {
                mon = objCaseNO - i + 12;
            }

            gdvHCOPPerAMT.HeaderRow.Cells[i].Text = mon + "月<br />請款金額";
            
        }
    }
}