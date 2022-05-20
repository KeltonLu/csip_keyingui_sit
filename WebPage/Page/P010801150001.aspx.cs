// *****************************************************************
//   作    者：Ares Dennis
//   功能說明：自然人收單定審案件明細
//   創建日期：2021/08/06   
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
using System.Collections;
using System.Reflection;
using Framework.Data.OM.OMAttribute;
using CSIPCommonModel.EntityLayer_new;
using Framework.WebControls;

public partial class Page_P010801150001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    private const string thisPageName = "P010801150001.aspx";
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
    private EntityAML_Cdata_Work_S tempcDSobj = new EntityAML_Cdata_Work_S(); // 20220413 暫存查詢到的 C 檔資料，送審時要寫入 AML_Cdata_Work_S By Kelton
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

        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合

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
            Session["P010801150001_Last"] = thisPageName;
            Session["P010801150001_SESSION"] = sessionOBJ;
            string NavigateUrl = "P010801040001.aspx";
            
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
        }
    }

    /// <summary>
    /// 總公司資料編輯
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void BtnHQ_Modify_Click(object sender, EventArgs e)
    {
        Session["P010801150001_Last"] = thisPageName;
        
        Response.Redirect("P010109050001.aspx", false);
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

        Session["P010801150001_Last"] = thisPageName;
        
        Response.Redirect("P010109060001.aspx", false);
    }

    /// <summary>
    /// 返回
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        
        Response.Redirect("P010801000001.aspx", false);
    }
    //聯絡註記編輯
    protected void btnNOTEEdit_Click(object sender, EventArgs e)
    { 
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
            tempcDSobj = (EntityAML_Cdata_Work_S)this.Session["P010801150001_cDs"];
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
            //若無風險資料則CDlblCreditCardBlockCode為空 by Ares Stanley 20220106
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

            // 20220413 將查詢到的 C 檔資料暫存，最後送審時要寫入 AML_Cdata_Work_S By Kelton
            tempcDSobj = cDobj.toSMode();
            tempcDSobj.CASE_NO = sessionOBJ.CASE_NO;
            this.Session["P010801150001_cDs"] = tempcDSobj;

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
        if(!string.IsNullOrEmpty(hqObj.HCOP_GENDER))
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
        else
        {
            HCOP_GENDER.Text = "";
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

    //異常結案
    protected void btnAbnormal_Click(object sender, EventArgs e)
    {        
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
        //20211005_Ares_Jack_身分證字號 前八碼放 CORP_NO, 後兩碼放 CORP_SEQ  + "  "
        JC66OBj.Add("CORP_NO", sessionOBJ.HCOP_HEADQUATERS_CORP_NO.Substring(0, 8).ToString().Trim());
        JC66OBj.Add("CORP_SEQ", sessionOBJ.HCOP_HEADQUATERS_CORP_NO.Substring(8, 2).ToString().Trim() + "  ");

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
                    insObj.HCOP_HEADQUATERS_CORP_NO = JC66OBj["CORP_NO"].ToString().Trim() + JC66OBj["CORP_SEQ"].ToString().Trim();// 取完整身分證字號;
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
                    notrLog.NL_CASE_NO = JC66OBj["CORP_NO"].ToString().Trim() + JC66OBj["CORP_SEQ"].ToString().Trim();// 取完整身分證字號;
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
            if (!string.IsNullOrEmpty(hstExmsP4A["HtgMsg"].ToString()))
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
        
        return rtnObj;
    }

    //取長姓名的內容值
    private EntityHTG_JC68 GetJC68(string strID)
    {
        EntityHTG_JC68 _result = new EntityHTG_JC68();
        using (BRHTG_JC68 obj = new BRHTG_JC68("P010801150001"))
        {
            EntityHTG_JC68 _data = new EntityHTG_JC68();

            _data.ID = strID;
            _result = obj.getData(_data, eAgentInfo, "11");
        }
        return _result;
    }

    //20220105_Ares_JAck_修改月請款金額呈現方式
    protected void gdvBRCHPerAMT_PreRender(object sender, EventArgs e)
    {
        try
        {
            if (gdvBRCHPerAMT.DataSource != null)
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