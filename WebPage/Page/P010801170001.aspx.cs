// *****************************************************************
//   作    者：Ares Jack
//   功能說明：自然人收單 結案案件明細
//   創建日期：2021/10/05   
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
using CSIPCommonModel.EntityLayer_new;

public partial class Page_P010801170001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    private const string thisPageName = "P010801170001.aspx";
    private List<string> ID = new List<string>();//關聯案件序號ID
    private DataTable dtSanctionCountry = new DataTable();
    private DataTable dtRiskCountry = new DataTable();
    private DataTable dtGeneralSanctionCountry = new DataTable();
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
        checkUserAgent();
        if (!IsPostBack)
        {
            //高風險國家 Type = 12
            dtRiskCountry = BRPostOffice_CodeType.GetCodeType("12");
            //高度制裁國家 Type = 13
            dtSanctionCountry = BRPostOffice_CodeType.GetCodeType("13");
            //一般制裁國家 Type = 15
            dtGeneralSanctionCountry = BRPostOffice_CodeType.GetCodeType("15");

            //設定本頁使用GRID頁數
            setGridLine();
            //非POSTBACK，則讀取資料
            showBaseData();
        }

        // eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合

        if (!validUser())
        {
            string strAlertMsg = MessageHelper.GetMessages("01_01080102_002");
            sbRegScript.Append("alert('" + strAlertMsg + "');window.location.href = 'P010801140001.aspx';");
            return;
        }

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
            AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801140001_SESSION"];
            if (keys.Length != 3) //參數錯誤，不處理
            {
                return;
            }
            sessionOBJ.RMMBatchNo = keys[0];
            sessionOBJ.AMLInternalID = keys[1];
            sessionOBJ.BRCHID = keys[2];
            Session["P010801170001_Last"] = thisPageName;
            Session["P010801170001_SESSION"] = sessionOBJ;
            string NavigateUrl = "P010801040001.aspx";
            //20210412_Ares_Stanley-調整轉向方法
            Response.Redirect(NavigateUrl, false);

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

        Session["P010801140001_Last"] = thisPageName;
        //20210412_Ares_Stanley-調整轉向方法
        Response.Redirect("P010801170101.aspx", false);
    }

    /// <summary>
    /// 返回
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        //20210412_Ares_Stanley-調整轉向方法
        Response.Redirect("P010801140001.aspx", false);
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
        if (userRolls.Contains("CSIP0123"))
        {
            isMaster = true;
        }

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
            AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801140001_SESSION"];
            if (sessionOBJ == null)
            {
                string NavigateUrl = "P010801140001.aspx";
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

            sessionOBJ.CaseProcess_Status = hqObj.CaseProcess_Status;
            //更新SESSION 次級編輯用
            Session["P010801140001_SESSION"] = sessionOBJ;
            this.SetVal<EntityAML_HQ_Work>(hqObj, false);
            //HQ特殊欄位處理
            setSpeciCollHQ(hqObj);

            //異常結案的沒做SCDD，所以要ENABLED掉
            if (hqObj.CaseProcess_Status.Trim().Equals("23") || hqObj.CaseProcess_Status.Trim().Equals("24") || hqObj.CaseProcess_Status.Trim().Equals("25"))
            {
                Btn_SCCD_Modify.Enabled = false;
            }
            else
                Btn_SCCD_Modify.Enabled = true;

            //20211221_Ares_Jack_商店資訊
            List<EntityAML_BRCH_Work> BRCHColl = BRAML_BRCH_Work.getBRCH_WOrkColl(sessionOBJ);
            grvCardData.DataSource = BRCHColl;
            grvCardData.DataBind();
            gdvBRCHPerAMT.DataSource = BRCHColl;
            gdvBRCHPerAMT.DataBind();

            //"新增案件編號"
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
            if (cDobj.CreditCardBlockCode == null || cDobj.CreditCardBlockCode.Trim() == "")
            {
                CDlblCreditCardBlockCode.Text = "";
            }
            else
            {
                CDlblCreditCardBlockCode.Text = cDobj.CreditCardBlockCode == "0000000000" ? "N" : "Y";//信用卡Block Code 正常戶:10個0
            }

            if (string.IsNullOrEmpty(cDobj.Incorporated) || cDobj.Incorporated.Trim().Equals("N"))
            {
                btnBeCooperate.Enabled = false;
                btnRemand.Enabled = true;
            }
            else
            {
                btnBeCooperate.Enabled = true;//系統要檢核不合作註記=Y才能讓經辦點「解除不合作」
                btnRemand.Enabled = false;
            }

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
            sbRegScript.Append("alert('" + strClientMsg + "');window.location.href = 'P010801140001.aspx';");
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
        //新增最新試算後的風險等級
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

        //公司狀態 //20211208_Ares_Jack_新增欄位
        HQlblHCOP_STATUS.Text = GetDcValue("OC_" + hqObj.HCOP_STATUS);

        //發證日期轉民國年
        if (hqObj.HCOP_OWNER_ID_ISSUE_DATE.Length == 8)
        {
            HQlblHCOP_OWNER_ID_ISSUE_DATE.Text = ConvertToROCYear(HQlblHCOP_OWNER_ID_ISSUE_DATE.Text);
        }

        //20211124_Ares_Jack_行業別編號, 職稱編號 帶出中文名稱
        #region 行業別編號
        DataTable dtCC_Item = new DataTable();
        DataTable dtCC_2_Item = new DataTable();
        DataTable dtCC_3_Item = new DataTable();
        List<string> temp1 = new List<string>();
        List<string> temp2 = new List<string>();
        List<string> temp3 = new List<string>();
        //行業編號 Type=3 
        dtCC_Item = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", hqObj.HCOP_CC);
        dtCC_2_Item = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", hqObj.HCOP_CC_2);
        dtCC_3_Item = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", hqObj.HCOP_CC_3);
        //CC
        if (dtCC_Item != null && dtCC_Item.Rows.Count > 0)
        {
            temp1.Add(hqObj.HCOP_CC + "(" + dtCC_Item.Rows[0]["CODE_NAME"].ToString() + ")");
        }
        else if (!string.IsNullOrEmpty(hqObj.HCOP_CC))
        {
            temp1.Add(hqObj.HCOP_CC);
        }
        //CC_2
        if (dtCC_2_Item != null && dtCC_2_Item.Rows.Count > 0)
        {
            temp2.Add(hqObj.HCOP_CC_2 + "(" + dtCC_2_Item.Rows[0]["CODE_NAME"].ToString() + ")");
        }
        else if (!string.IsNullOrEmpty(hqObj.HCOP_CC_2))
        {
            temp2.Add(hqObj.HCOP_CC_2);
        }
        //CC_3
        if (dtCC_3_Item != null && dtCC_3_Item.Rows.Count > 0)
        {
            temp3.Add(hqObj.HCOP_CC_3 + "(" + dtCC_3_Item.Rows[0]["CODE_NAME"].ToString() + ")");
        }
        else if (!string.IsNullOrEmpty(hqObj.HCOP_CC_3))
        {
            temp3.Add(hqObj.HCOP_CC_3);
        }
        HQlblHCOP_CC.Text = string.Join(",", temp1);
        HQlblHCOP_CC_2.Text = string.Join(",", temp2);
        HQlblHCOP_CC_3.Text = string.Join(",", temp3);
        dtCC_Item.Clear();
        dtCC_Item.Dispose();
        dtCC_2_Item.Clear();
        dtCC_2_Item.Dispose();
        dtCC_3_Item.Clear();
        dtCC_3_Item.Dispose();
        #endregion
        #region 職稱編號
        DataTable dtOC_Item = new DataTable();
        //職業別 Type=16
        dtOC_Item = BRPostOffice_CodeType.GetCodeTypeByCodeID("16", hqObj.HCOP_OC);
        if (dtOC_Item != null && dtOC_Item.Rows.Count > 0)
            HQlblHCOP_OC.Text = hqObj.HCOP_OC + "(" + dtOC_Item.Rows[0]["CODE_NAME"].ToString() + ")";
        else
            HQlblHCOP_OC.Text = hqObj.HCOP_OC;
        dtOC_Item.Clear();
        dtOC_Item.Dispose();
        #endregion

        //收入及資產來源
        if (!string.IsNullOrEmpty(hqObj.HCOP_INCOME_SOURCE) )
        {
            string incomeSource = hqObj.HCOP_INCOME_SOURCE;
            for (int i = 1; i < 10; i++)
            {
                if (incomeSource.IndexOf('1', i - 1, 1) != -1)//被勾選的是1
                {
                    incomeSource = incomeSource.Remove(i - 1, 1).Insert(i - 1, i.ToString());// ex:101000000 => 1,3
                }
            }
            incomeSource = string.Join(",", incomeSource.Replace("0", "").ToCharArray());//清除0,剩餘的用逗號分開 ex:1,2,3
            
            string[] incomes = incomeSource.Split(',');
            foreach(string income in incomes)
            {
                for(int i = 1; i <= 9; i++)
                {
                    if(income == i.ToString())
                    {
                        CheckBox checkBox = (CheckBox)FindControl("chkIncome" + i.ToString());
                        checkBox.Checked = true;
                    }
                }
            }
        }

        //性別
        if (!string.IsNullOrEmpty(hqObj.HCOP_GENDER))
        {
            //20211018_Ares_Jack_自然人收單 因為性別Value由 M F 代表 所以 HCOP_GENDER 加上判斷 M F
            if (hqObj.HCOP_GENDER == "M")
            {
                HCOP_GENDER.Text = "男";//男
            }
            if (hqObj.HCOP_GENDER == "F")
            {
                HCOP_GENDER.Text = "女";//女
            }            
        }

        //有無照片
        if (!string.IsNullOrEmpty(hqObj.HCOP_ID_PHOTO_FLAG))
        {
            HQlblHCOP_ID_PHOTO_FLAG.Text = GetDcValue("HSP_" + hqObj.HCOP_ID_PHOTO_FLAG);            
        }

        //領補換類別
        if (!string.IsNullOrEmpty(hqObj.HCOP_OWNER_ID_REPLACE_TYPE))
        {
            HQlblHCOP_OWNER_ID_REPLACE_TYPE.Text = GetDcValue("ID_" + hqObj.HCOP_OWNER_ID_REPLACE_TYPE);
        }

        if (dtRiskCountry != null && dtRiskCountry.Rows.Count > 0)
        {
            //註冊國籍 AML_HQ_Work.HCOP_REGISTER_NATION
            if (!String.IsNullOrEmpty(hqObj.HCOP_REGISTER_NATION))
            {
                //客戶 國籍、戶籍地 或 通訊地 或關聯人任一人 國籍 位於位於高風險國家/地區
                if (dtRiskCountry.Select("CODE_ID = '" + hqObj.HCOP_REGISTER_NATION + "'").Length > 0)
                {
                    lblIsRisk.Text = "Y";
                }
                else
                {
                    lblIsRisk.Text = "N";
                }

                //客戶 國籍、戶籍地 或 通訊地 位於一般或高度制裁國家/ 地區
                if (dtGeneralSanctionCountry.Select("CODE_ID = '" + hqObj.HCOP_REGISTER_NATION + "'").Length > 0 ||
                    dtSanctionCountry.Select("CODE_ID = '" + hqObj.HCOP_REGISTER_NATION + "'").Length > 0)
                {
                    lblIsSanction.Text = "Y";
                }
                else
                {
                    lblIsSanction.Text = "N";
                }
            }
            //20220111_Ares_Jack_加上國籍2判斷風險等級
            if (!String.IsNullOrEmpty(hqObj.HCOP_COUNTRY_CODE_2))
            {
                //客戶 國籍、戶籍地 或 通訊地 或關聯人任一人 國籍 位於位於高風險國家/地區
                if (dtRiskCountry.Select("CODE_ID = '" + hqObj.HCOP_COUNTRY_CODE_2 + "'").Length > 0)
                {
                    lblIsRisk.Text = "Y";
                }

                //客戶 國籍、戶籍地 或 通訊地 位於一般或高度制裁國家/ 地區
                if (dtGeneralSanctionCountry.Select("CODE_ID = '" + hqObj.HCOP_COUNTRY_CODE_2 + "'").Length > 0 ||
                    dtSanctionCountry.Select("CODE_ID = '" + hqObj.HCOP_COUNTRY_CODE_2 + "'").Length > 0)
                {
                    lblIsSanction.Text = "Y";
                }
            }
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
            //用LIST進來，就不適用 DataRowView 必須用原型別操作資料
            EntityAML_BRCH_Work rowView = (EntityAML_BRCH_Work)e.Row.DataItem;
            //轉換民國年
            string idDate = rowView.BRCH_OWNER_ID_ISSUE_DATE;
            
            //長姓名區            
            if (string.IsNullOrEmpty(rowView.BRCH_CHINESE_LNAME) == false || string.IsNullOrEmpty(rowView.BRCH_ROMA) == false)
            {
                ControlCollection _table = e.Row.Parent.Controls; //e.Row.Parent就是Table
                int _columnSpan = 2;
                GridViewRow gvr = new GridViewRow(-1, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
                TableCell cell_label = new TableCell();
                TableCell cell_value = new TableCell();
                cell_label.ColumnSpan = 1;
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
                cell_label.ColumnSpan = 1;
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
    }
    /// <summary>
    /// 實作字典
    /// </summary>
    private void buiInfoDict()
    {
        DCCommonColl = new Dictionary<string, string>();

        DCCommonColl.Add("HSP_0", "有"); //有無照片是反過來的
        DCCommonColl.Add("HSP_1", "無");

        //身分證領補換類別  編碼ID_        
        setFromCodeType("4", "ID");

        //風險等級 RS_
        DCCommonColl.Add("RS_H", "高");
        DCCommonColl.Add("RS_M", "中");
        DCCommonColl.Add("RS_L", "低");
        //開關 OC_
        DCCommonColl.Add("OC_O", "OPEN");
        DCCommonColl.Add("OC_C", "Close");

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
        DCCommonColl.Add("CN_NonCooperated", "【不合作結案】送審上呈");
        DCCommonColl.Add("CN_CaseClosed", "【商店解約結案】送審上呈");
        DCCommonColl.Add("CN_OtherClosed", "【其他結案】送審上呈");
        DCCommonColl.Add("CN_RejectNonCooperated", "【不合作結案】案件退回");
        DCCommonColl.Add("CN_RejectCaseClosed", "【商店解約結案】案件退回");
        DCCommonColl.Add("CN_RejectOtherClosed", "【其他結案】案件退回");
        //20220110_Ares_Jack_顯示作業類別
        DCCommonColl.Add("CN_NonCooperatedDone", "【不合作結案】案件放行");
        DCCommonColl.Add("CN_CaseClosedDone", "【商店解約結案】案件放行");
        DCCommonColl.Add("CN_OtherClosedDone", "【其他結案】案件放行");
        setFromCodeType("5", "RN");

        DCCommonColl.Add("CSTYPE_A", "用戶預定的案件");
        DCCommonColl.Add("CSTYPE_B", " 已到達審查日期");
        DCCommonColl.Add("CSTYPE_C", "客戶的風險等級被升級或者被降級");
        DCCommonColl.Add("CSTYPE_D1", "選擇 X 比率（%）的高風險客戶");
        DCCommonColl.Add("CSTYPE_D2", "選擇高風險分數大過或等於 X 的客戶 ");
        DCCommonColl.Add("CSTYPE_D3", "選擇前 X 的高風險客戶");
        
        setFromCodeType("8", "STTYPE");

        //20211208_Ares_Jack_新增欄位商店狀態 開關 OC_ 
        DCCommonColl.Add("OC_OPEN", "O");
        DCCommonColl.Add("OC_Close", "C");
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
                // 20211210_Ares_Jack-經姵晴確認將日期應調整案件明細到期日+5個月
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

    //案件重審
    protected void btnRemand_Click(object sender, EventArgs e)
    {
        //取得今天的案件重審件數
        int _Cnt = BRAML_HQ_Work.GetRemandCaseNo("8");
        string _CaseNo = DateTime.Now.ToString("yyyyMMdd").Trim() + "8" + _Cnt.ToString().Trim().PadLeft(5, '0');
        GetOrigenalData(_CaseNo, "R");
    }
    //解除不合作
    protected void btnBeCooperate_Click(object sender, EventArgs e)
    {
        int _Cnt = BRAML_HQ_Work.GetRemandCaseNo("9");
        string _CaseNo = DateTime.Now.ToString("yyyyMMdd").Trim() + "9" + _Cnt.ToString().Trim().PadLeft(5, '0');
        GetOrigenalData(_CaseNo, "D");
    }
    protected void GetOrigenalData(string _newCaseNo, string _CaseType)
    {
        bool isSucc = false;
        string strAlertMsg = string.Empty;
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801140001_SESSION"];
        if (sessionOBJ == null)
        {
            string NavigateUrl = "P010801140001.aspx";
            string msg = MessageHelper.GetMessage("01_01080114_003");
            string urlString = @"alert('" + msg + "');location.href='" + NavigateUrl + "';";
            base.sbRegScript.Append(urlString);
            return;
        }

        try
        {
            //讀取公司資料 HQ_WORK CDATA,EDATA
            EntityAML_HQ_Work hqObj = BRAML_HQ_Work.getHQ_WOrk(sessionOBJ);
            hqObj.CASE_NO = _newCaseNo.Trim();            
            hqObj.CaseOwner_User = eAgentInfo.agent_id;
            hqObj.CaseProcess_Status = "0";
            hqObj.CaseProcess_User = "M1";
            hqObj.ReviewCompletedDate = "";
            hqObj.Create_Time = DateTime.Now.ToString("HHmmss");
            hqObj.Create_User = eAgentInfo.agent_id;
            hqObj.Create_Date = DateTime.Now.ToString("yyyyMMdd");
            //20211221 AML NOVA 功能需求程式碼,註解保留 by Ares Dennis
            //hqObj.GROUP_NO = _newCaseNo.Trim();//2021/7/6 EOS_AML(NOVA) GROUP_NO存新的案件編號 by Ares Dennis
            isSucc = BRAML_HQ_Work.Insert(hqObj);

            //讀取分公司明細            
            List<EntityAML_BRCH_Work> BRCH_Coll = BRAML_BRCH_Work.getBRCH_WOrkCollList(sessionOBJ);
            foreach (EntityAML_BRCH_Work BRobj in BRCH_Coll)
            {
                if (!string.IsNullOrEmpty(BRobj.FileName))
                {
                    BRobj.CASE_NO = _newCaseNo.Trim();
                    BRobj.Create_Time = DateTime.Now.ToString("HHmmss");
                    BRobj.Create_User = eAgentInfo.agent_id;
                    BRobj.Create_Date = DateTime.Now.ToString("yyyyMMdd");
                    isSucc = BRAML_BRCH_Work.Insert(BRobj);
                }
            }

            EntityAML_Edata_Work eWork = BRAML_Edata_Work.getAML_Edata_Work(sessionOBJ);
            if (!string.IsNullOrEmpty(eWork.FileName))
            {
                eWork.CASE_NO = _newCaseNo.Trim();
                eWork.Create_Time = DateTime.Now.ToString("HHmmss");
                eWork.Create_User = eAgentInfo.agent_id;
                eWork.Create_Date = DateTime.Now.ToString("yyyyMMdd");
                isSucc = BRAML_Edata_Work.InsertBySingle(eWork);
            }

            //寫入案件歷程
            EntityNoteLog notrLog = new EntityNoteLog();
            notrLog.NL_CASE_NO = _newCaseNo.Trim();
            notrLog.NL_SecondKey = "";
            notrLog.NL_ShowFlag = "1";
            notrLog.NL_DateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            notrLog.NL_User = eAgentInfo.agent_id;
            notrLog.NL_Type = "Note";
            notrLog.NL_Value = "新增" + (_CaseType.Trim() == "D" ? " 解除不合作案件" : " 案件重審案件") + "  原案件編號：" + sessionOBJ.CASE_NO;
            isSucc = BRNoteLog.Insert(notrLog);

            //案件重審時，AML_CASE_DATA是否需要新增一筆
            Entity_AML_CASE_DATA eCaseData = BRAML_CASE_DATA.getAML_CASE_DATA(sessionOBJ);
            eCaseData.CASE_NO = _newCaseNo.Trim();
            eCaseData.Create_YM = DateTime.Now.ToString("yyyyMM");
            eCaseData.Create_Date = DateTime.Now.ToString("yyyyMMdd");
            eCaseData.Due_Date = DateTime.Now.ToString("yyyyMMdd");
            isSucc = BRAML_CASE_DATA.InsertBySingle(eCaseData);

            if (isSucc)
            {
                strAlertMsg = (_CaseType.Trim() == "D" ? MessageHelper.GetMessages("01_01080114_007") : MessageHelper.GetMessages("01_01080114_005")) + _newCaseNo.Trim();
                sbRegScript.Append("alert('" + strAlertMsg + "');window.location.href = 'P010801140001.aspx';");
            }
            else
            {
                strAlertMsg = _CaseType.Trim() == "D" ? MessageHelper.GetMessages("01_01080114_008") : MessageHelper.GetMessages("01_01080114_006");
                sbRegScript.Append("alert('" + strAlertMsg + "');");
            }
        }
        catch (Exception e)
        {
            Logging.Log(e, LogLayer.UI);
        }
        finally
        {
        }
    }

    //20220105_Ares_JAck_修改月請款金額呈現方式
    protected void gdvBRCHPerAMT_PreRender(object sender, EventArgs e)
    {
        try
        {
            if (gdvBRCHPerAMT.DataSource != null)
            {
                AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801140001_SESSION"];
                if (sessionOBJ == null || eAgentInfo == null)
                {
                    string strAlertMsg = MessageHelper.GetMessages("01_01080101_002");
                    sbRegScript.Append("alert('" + strAlertMsg + "');window.location.href = 'P010801140001.aspx';");
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
                    if (gdvBRCHPerAMT.HeaderRow != null)
                        gdvBRCHPerAMT.HeaderRow.Cells[i].Text = mon + "月<br />請款金額";

                }
            }

        }
        catch (Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex + "年度請款金額錯誤");
        }
    }
}