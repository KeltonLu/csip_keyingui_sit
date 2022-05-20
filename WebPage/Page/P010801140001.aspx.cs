// *****************************************************************
//   作    者：林家賜
//   功能說明：AML資料 結案案件列表
//   創建日期：2019/02/03
//   修改記錄：
//   <author>            <time>            <TaskID>                <desc>
//   Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
// ******************************************************************

using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Framework.Common.Utility;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using CSIPNewInvoice.EntityLayer_new;
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.BusinessRules_new;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Common.Logging;
using Framework.WebControls;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
public partial class Page_P010801140001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;

    #region  轉換代碼用字典
    //風險區分
    Dictionary<string, string> DCRiskRank;

    #endregion

    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //*設置GridView分頁顯示的行數
            int intPageSize = 15;
            gpList.PageSize = intPageSize;
            grvCardData.PageSize = intPageSize;
            gpList.Visible = false;
            grvCardData.Visible = false;


            CommonFunction.SetControlsEnabled(pnlText, false);// 清空網頁中所有的輸入欄位
            base.sbRegScript.Append(BaseHelper.SetFocus("txtTaxID"));// 將 總公司/總店統編 設為輸入焦點
            LoadDropDownList();
            //腳色權限驗證
            if (!validUser())
            {
                string strAlertMsg = MessageHelper.GetMessages("01_01080114_002");
                sbRegScript.Append("alert('" + strAlertMsg + "');window.location.href = 'P010801000001.aspx';");
                return;
            }
            if (Session["P010801140001CASE_QUERY"] != null)
            {
                AML_HQQuery QueryObj = (AML_HQQuery)Session["P010801140001CASE_QUERY"];
                //將查詢條件顯示回畫面
                SetQueryDataToPage(QueryObj);
                QueryData(QueryObj);
            }

        }
        // eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        checkUserAgent();
    }

    /// <summary>
    /// 查詢機能
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtLastUpdateDateBegin.Text) && !DateTimeCheck(txtLastUpdateDateBegin.Text.Trim()))
        {
            base.sbRegScript.Append("alert('結案日期(起)格式錯誤');$('#txtLastUpdateDateBegin').focus();");
            return;
        }
        if (!string.IsNullOrEmpty(txtLastUpdateDateEnd.Text) && !DateTimeCheck(txtLastUpdateDateEnd.Text.Trim()))
        {
            base.sbRegScript.Append("alert('結案日期(迄)格式錯誤');$('#txtLastUpdateDateEnd').focus();");
            return;
        }
        //查詢前先清除畫面上會用到的控制項，避免資料殘留
        this.gpList.CurrentPageIndex = 1;
        //依傳入條件查詢，做成Datatable回傳
        AML_HQQuery QueryObj = new AML_HQQuery();
        GetParamByPage(ref QueryObj);
        Session["P010801140001CASE_QUERY"] = QueryObj;//記下當前查詢條件
        QueryData(QueryObj);
    }
    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="QueryObj"></param>
    private void QueryData(AML_HQQuery QueryObj)
    {

        DataTable dt = BRAML_HQ_Work.MasterQuery(QueryObj);

        //將結果設定為ViewState
        ViewState["DataBind"] = dt;

        ///將查詢結果綁定
        BindGridView();
        if (dt.Rows.Count == 0)
        {
            string strAlertMsg = MessageHelper.GetMessages("01_01080114_003");
            sbRegScript.Append("alert('" + strAlertMsg + "');");

        }
    }
    #endregion
    /// <summary>
    /// 取得頁面上輸入查詢條件
    /// </summary>
    /// <param name="QueryObj"></param>
    private void GetParamByPage(ref AML_HQQuery QueryObj)
    {
        #region "案件類型"

        #endregion
        #region "統編"
        if (txtTaxID.Text != "")
        {
            QueryObj.TaxNo = txtTaxID.Text;
        }
        #endregion
        #region "案號"
        if (txtCASE_NO.Text != "")
        {
            QueryObj.CaseNo = txtCASE_NO.Text;
        }
        #endregion
        #region "開始年月"
        if (txtProjYearBegin.Text != "" && txtProjYearBegin.Text != "請選擇" && txtProjMonthBegin.Text != "" && txtProjMonthBegin.Text != "請選擇")
        {
            QueryObj.yearS = txtProjYearBegin.Text;
            QueryObj.MonthS = txtProjMonthBegin.Text;
        }
        #endregion
        #region "結束年月"
        if (txtProjYearEnd.Text != "" && txtProjYearEnd.Text != "請選擇" && txtProjMonthEnd.Text != "" && txtProjMonthEnd.Text != "請選擇")
        {
            QueryObj.yearE = txtProjYearEnd.Text;
            QueryObj.MonthE = txtProjMonthEnd.Text;
        }

        #endregion
        #region "風險等級"
        if (txtOriginalRiskRanking.Text != "請選擇" && txtOriginalRiskRanking.Text != "")
        {
            QueryObj.RiskRanking = dropOriginalRiskRanking.SelectedItem.Value;
        }
        #endregion

        #region "經辦人員"
        if (txtCaseOwner_User.Text != "請選擇" && txtCaseOwner_User.Text != "")
        {
            QueryObj.Owner_User = dropCaseOwner_User.SelectedItem.Value;
        }
        #endregion

        //20191115-RQ-2018-015749-002-新增查詢條件
        #region 不合作註記
        if (dropIncorporated.SelectedIndex != -1 && !dropIncorporated.SelectedValue.Trim().Equals("X"))
        {
            QueryObj.IncorporatedFlag = dropIncorporated.SelectedValue;
        }
        #endregion

        #region 結案原因
        if (dropCloseType.SelectedIndex != -1 && !dropCloseType.SelectedValue.Trim().Equals("請選擇"))
        {
            QueryObj.CloseType = dropCloseType.SelectedValue;
        }
        #endregion

        #region 結案日期
        // 結案日期(起)
        if (!string.IsNullOrEmpty(txtLastUpdateDateBegin.Text.Trim()))
        {
            
            //if (!DateTimeCheck(txtLastUpdateDateBegin.Text.Trim()))
            //{
            //    base.sbRegScript.Append("alert('結案日期格式錯誤');$('#txtLastUpdateDateBegin').focus();");
            //    return;
            //}
            QueryObj.CloseDateS = txtLastUpdateDateBegin.Text.Trim();
        }

        // 結案日期(迄)
        if (!string.IsNullOrEmpty(txtLastUpdateDateEnd.Text.Trim()))
        {
            //if (!DateTimeCheck(txtLastUpdateDateEnd.Text.Trim()))
            //{
            //    base.sbRegScript.Append("alert('結案日期格式錯誤');$('#txtLastUpdateDateEnd').focus();");
            //    return;
            //}
            QueryObj.CloseDateE = txtLastUpdateDateEnd.Text.Trim();
        }
        #endregion
        
        #region "排序"
        if (txtSort.Text != "請選擇" && txtSort.Text != "")
        {
            QueryObj.OrderBy = dropSort.SelectedItem.Value;
            if (txtAsc.Text != "請選擇" && txtAsc.Text != "")
            {
                QueryObj.OrderASC = dropAsc.SelectedItem.Value;
            }
        }
        #endregion

        //取得當前的角色
        QueryObj.UserRoll = GetqueryRole();
    }


    #region 方法區
    /// <summary>
    /// 依記憶將查詢條件還原至畫面
    /// </summary>
    /// <param name="queryObj"></param>
    private void SetQueryDataToPage(AML_HQQuery QueryObj)
    {
        //全部還原
        txtProjYearBegin.Text = "請選擇";
        txtProjMonthBegin.Text = "請選擇";
        txtProjYearEnd.Text = "請選擇";
        txtProjMonthEnd.Text = "請選擇";
        txtOriginalRiskRanking.Text = "請選擇";
        txtCaseOwner_User.Text = "請選擇";
        txtSort.Text = "請選擇";
        txtAsc.Text = "請選擇";
        //20191025-RQ-2018-015749-002 新增查詢條件 by Peggy
        txtIncorporated.Text = "請選擇";
        txtCloseType.Text = "請選擇";
        txtLastUpdateDateBegin.Text = "";
        txtLastUpdateDateEnd.Text = "";

        #region "統編"
        txtTaxID.Text = QueryObj.TaxNo;
        #endregion
        #region "案號"

        txtCASE_NO.Text = QueryObj.CaseNo;

        #endregion
        #region "開始年月"
        if (!string.IsNullOrEmpty(QueryObj.yearS) && !string.IsNullOrEmpty(QueryObj.MonthS))
        {
            txtProjYearBegin.Text = QueryObj.yearS;
            txtProjMonthBegin.Text = QueryObj.MonthS;
        }
        #endregion
        #region "結束年月"
        if (!string.IsNullOrEmpty(QueryObj.yearE) && !string.IsNullOrEmpty(QueryObj.MonthE))
        {
            txtProjYearEnd.Text = QueryObj.yearE;
            txtProjMonthEnd.Text = QueryObj.MonthE;
        }

        #endregion
        #region "風險等級"
        if (!string.IsNullOrEmpty(QueryObj.RiskRanking))
        {
            foreach (ListItem litem in dropOriginalRiskRanking.Items)
            {
                if (litem.Value == QueryObj.RiskRanking)
                {
                    litem.Selected = true;
                    txtOriginalRiskRanking.Text = litem.Text;
                    break;
                }
            }
        }
        #endregion

        #region "經辦人員"
        if (!string.IsNullOrEmpty(QueryObj.Owner_User))
        {

            foreach (ListItem litem in dropCaseOwner_User.Items)
            {
                if (litem.Value == QueryObj.Owner_User)
                {
                    litem.Selected = true;
                    txtCaseOwner_User.Text = litem.Text;
                    break;
                }
            }
        }
        #endregion
        //20191025-RQ-2018-015749-002 新增查詢條件 by Peggy
        #region 不合作註記
        if (!string.IsNullOrEmpty(QueryObj.IncorporatedFlag))
        {
            foreach (ListItem litem in dropIncorporated.Items)
            {
                if (litem.Value == QueryObj.IncorporatedFlag)
                {
                    litem.Selected = true;
                    txtIncorporated.Text = litem.Text;
                    break;
                }
            }
        }
        #endregion
        #region 結案原因
        if (!string.IsNullOrEmpty(QueryObj.CloseType))
        {
            foreach (ListItem litem in dropCloseType.Items)
            {
                if (litem.Value == QueryObj.CloseType)
                {
                    litem.Selected = true;
                    txtCloseType.Text = litem.Text;
                    break;
                }
            }
        }
        #endregion
        #region "結案日期(起)"
        if (!string.IsNullOrEmpty(QueryObj.CloseDateS))
        {
            txtLastUpdateDateBegin.Text = QueryObj.CloseDateS;
        }
        #endregion
        #region "結案日期(迄)"
        if (!string.IsNullOrEmpty(QueryObj.CloseDateE))
        {
            txtLastUpdateDateEnd.Text = QueryObj.CloseDateE;
        }
        #endregion

        #region "排序"
        if (!string.IsNullOrEmpty(QueryObj.OrderBy))
        {

            foreach (ListItem litem in dropSort.Items)
            {
                if (litem.Value == QueryObj.OrderBy)
                {
                    litem.Selected = true;
                    txtSort.Text = litem.Text;
                    break;
                }
            }
            foreach (ListItem litem in dropAsc.Items)
            {
                if (litem.Value == QueryObj.OrderASC)
                {
                    litem.Selected = true;
                    txtAsc.Text = litem.Text;
                    break;
                }
            }
        }

        #endregion

    }
    /// <summary>
    /// 腳色權限驗證
    /// </summary>
    private bool validUser()
    {
        bool isMaster = false;
        if (eAgentInfo == null)
        {
            checkUserAgent();
        }
        string userRolls = eAgentInfo.roles;
        if (userRolls.Contains("CSIP0123"))
        {
            isMaster = true;
        }
        return isMaster;
    }
    private string GetqueryRole()
    {
        string rtn = "";
        if (eAgentInfo == null)
        {
            checkUserAgent();
        }
        string userRolls = eAgentInfo.roles;
        if (userRolls.Contains("CSIP0120"))
        {
            rtn += "M1,";
        }
        if (userRolls.Contains("CSIP0121"))
        {
            rtn += "C1,";
        }
        if (userRolls.Contains("CSIP0122"))
        {
            rtn += "C2,";
        }
        if (!string.IsNullOrEmpty(rtn))
        {
            rtn = rtn.Remove(rtn.Length - 1, 1);
        }
        return rtn;

    }

    /// <summary>
    /// 換頁方法
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    /// <summary>
    /// 綁定資料
    /// 修改日期: 2021/01/21_Ares_Stanley-設定查詢結果欄位css
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
                    this.gpList.Visible = true;
                    this.grvCardData.Visible = true;
                    this.gpList.RecordCount = dtblResult.Rows.Count;
                    this.grvCardData.DataSource = CommonFunction.Pagination(dtblResult, this.gpList.CurrentPageIndex, this.gpList.PageSize);
                    this.grvCardData.DataBind();
                }
                else
                {
                    this.grvCardData.DataSource = CommonFunction.Pagination(dtblResult, this.gpList.CurrentPageIndex, this.gpList.PageSize);
                    this.grvCardData.DataBind();
                    this.gpList.Visible = false;
                    this.grvCardData.Visible = false;
                }
            }
            for (int i = 0; i < this.grvCardData.Columns.Count; i++)
            {
                this.grvCardData.Columns[i].HeaderStyle.CssClass = "whiteSpaceNormal";
                this.grvCardData.Columns[i].ItemStyle.CssClass = "whiteSpaceNormal";
            }
        }
        catch (Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex, LogLayer.UI);
        }
    }
    /// <summary>
    /// 載入下拉選單內容
    /// </summary>
    private void LoadDropDownList()
    {
        DataTable result = new DataTable();
        //ListItem listItem = null;
        string aMLCC = string.Empty;
        string countryCode = string.Empty;
        string stateCode = string.Empty;
        string organization = string.Empty;
        string yNEmpty = string.Empty;
        string number = string.Empty;

        // 設定 年月
        SetDropNumberYR();

        //設定風險等級
        SetDropRisk();

        ///設定排序
        SetDropOrder();

        //設定經辦
        SetDropCreateUser();

        //20191025 modify by Peggy
        //不合作註記
        txtIncorporated.Text = "請選擇";
        //結案原因
        SetDropCloseType();

    }
    /// <summary>
    /// 設定經辦
    /// </summary>
    private void SetDropCreateUser()
    {
        DataTable dt = BRAML_HQ_Work.getMasterProcUser();
        ListItem listItem1 = new ListItem();
        listItem1.Text = "請選擇";
        dropCaseOwner_User.Items.Add(listItem1);
        foreach (DataRow dr in dt.Rows)
        {
            ListItem listItem = new ListItem();
            listItem.Text = dr[1].ToString();
            if (dr[1].ToString() == "")
            {
                listItem.Text = dr[0].ToString();
            }

            listItem.Value = dr[0].ToString();
            if (!string.IsNullOrEmpty(dr[0].ToString()))
            {
                dropCaseOwner_User.Items.Add(listItem);
            }
        }
        txtCaseOwner_User.Text = "請選擇";

    }
    // 設定風險等級
    private void SetDropRisk()
    {
        ListItem listItem = new ListItem();
        string listString = string.Empty;

        string[] arr = { "高;H", "中;M", "低;L" };
        string[] arrs = null;
        ListItem listItem1 = new ListItem();
        listItem1.Text = "請選擇";
        dropOriginalRiskRanking.Items.Add(listItem1);
        for (int i = 0; i < arr.Length; i++)
        {
            arrs = arr[i].Split(';');
            listItem = new ListItem();
            listItem.Value = arrs[1].ToString();
            listItem.Text = arrs[0].ToString();

            this.dropOriginalRiskRanking.Items.Add(listItem);
        }
        txtOriginalRiskRanking.Text = "請選擇";
    }
    // 設定排序
    private void SetDropOrder()
    {
        ListItem listItem = new ListItem();
        string listString = string.Empty;

        string[] arrSort = { "案件編號; CASE_NO", "統編; HCOP_HEADQUATERS_CORP_NO", "登記名稱; HCOP_REG_NAME", "風險等級; OriginalRiskRanking", "建案日期; DataDate", "派案日期;", "到期日;CaseExpiryDate" };
        string[] arrAsc = { "升冪;ASC", "降冪;DESC" };
        string[] arrs = null;
        ListItem listItem1 = new ListItem();
        listItem1.Text = "請選擇";
        this.dropSort.Items.Add(listItem1);
        this.dropAsc.Items.Add(listItem1);
        //排序條件
        for (int i = 0; i < arrSort.Length; i++)
        {
            arrs = arrSort[i].Split(';');
            listItem = new ListItem();
            listItem.Value = arrs[1].ToString();
            listItem.Text = arrs[0].ToString();
            this.dropSort.Items.Add(listItem);
        }
        //升降冪
        for (int i = 0; i < arrAsc.Length; i++)
        {
            arrs = arrAsc[i].Split(';');
            listItem = new ListItem();
            listItem.Value = arrs[1].ToString();
            listItem.Text = arrs[0].ToString();
            this.dropAsc.Items.Add(listItem);
        }

        txtSort.Text = "請選擇";
        txtAsc.Text = "請選擇";
    }

    // 設定 數值
    private void SetDropNumberYR()
    {
        ListItem listItem = new ListItem();
        string listString = string.Empty;


        // string[] arr = { DateTime.Now.Year.ToString(), DateTime.Now.AddYears(-1).Year.ToString() };
        //建案年
        int startYear = 2019;
        int nowYear = DateTime.Now.Year;
        for (int i = nowYear; i >= startYear; i--)
        {
            string cueYear = i.ToString();
            listItem = new ListItem();
            listItem.Value = cueYear;
            listItem.Text = cueYear;
            dropProjYearEnd.Items.Add(listItem);
            dropProjYearBegin.Items.Add(listItem);
        }

        //建案月
        for (int i = 1; i < 13; i++)
        {
            listItem = new ListItem();
            listItem.Value = i.ToString().PadLeft(2, '0');
            listItem.Text = i.ToString().PadLeft(2, '0');
            dropProjMonthEnd.Items.Add(listItem);
            dropProjMonthBegin.Items.Add(listItem);
        }

        //建案日
        for (int i = 1; i < 32; i++)
        {
            listItem = new ListItem();
            listItem.Value = i.ToString().PadLeft(2, '0');
            listItem.Text = i.ToString().PadLeft(2, '0');
        }

        txtProjYearEnd.Text = "請選擇";
        txtProjYearBegin.Text = "請選擇";
        txtProjMonthBegin.Text = "請選擇";
        txtProjMonthEnd.Text = "請選擇";

        ListItem listItem1 = new ListItem();
        listItem1.Text = "請選擇";
        this.dropProjMonthBegin.Items.Insert(0, listItem1);
        this.dropProjMonthEnd.Items.Insert(0, listItem1);
        this.dropProjYearEnd.Items.Insert(0, listItem1);
        this.dropProjYearBegin.Items.Insert(0, listItem1);
    }

    //結案原因
    private void SetDropCloseType()
    {
        DataTable dt_CodeType = BRPostOffice_CodeType.GetCodeType("14");

        dropCloseType.DataSource = dt_CodeType;
        dropCloseType.DataTextField = "CODE_NAME";
        dropCloseType.DataValueField = "CODE_ID";
        dropCloseType.DataBind();

        dropCloseType.Items.Insert(0, "請選擇");
        txtCloseType.Text = "請選擇";
    }

    protected void grvCardData_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ///確定物件及字典均已建立
            if (eAgentInfo == null)
            {
                checkUserAgent();
            }
            System.Data.DataRowView rowView = (DataRowView)e.Row.DataItem;

            //依建案日期計算到期日 cell6 及 寄送不合作信函日期 cell8

            string pDate = rowView["DataDate"].ToString();
            string AddressLabelTwoMonthFlagTime = rowView["AddressLabelTwoMonthFlagTime"].ToString();

            // string endDate = "";
            //   string noCropDate = "";
            // proceDate(pDate,ref endDate, ref noCropDate);
            //  e.Row.Cells[6].Text = endDate; 建案日期已由SQL計算
            e.Row.Cells[7].Text = Convert.ToDateTime(e.Row.Cells[7].Text).ToString("yyyyMMdd");
            if (!string.IsNullOrEmpty(AddressLabelTwoMonthFlagTime) && AddressLabelTwoMonthFlagTime.Length > 8)
            {
                e.Row.Cells[9].Text = Convert.ToDateTime(AddressLabelTwoMonthFlagTime).ToString("yyyyMMdd");
            }
            //END依建案日期計算到期日
            e.Row.Cells[4].Text = GetDcValue(DCRiskRank, rowView["OriginalRiskRanking"].ToString());

            //顯示經辦人員
            string eName = rowView["CaseOwner_User"].ToString();
            foreach (ListItem litem in dropCaseOwner_User.Items)
            {
                //20200318-RQ-2019-030155-003 修改：修正因帳號大小寫不同而無法顯示問題。
                //if (litem.Value == eName)
                if (litem.Value.Trim().ToUpper() == eName.Trim().ToUpper())
                {
                    e.Row.Cells[11].Text = litem.Text;
                    break;
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
                string eDtmp = Ds.AddMonths(3).ToString("yyyy/MM/01");
                endDate = Convert.ToDateTime(eDtmp).AddDays(-1).ToString("yyyyMMdd");
                //以到期日計算，先減6天，若為假日，(六、日)則依假日遞減回前一+2個工作日(減計6.日)，因為計算工作日，若為周五，周四，周三，表示有跨六日，在減兩天
                //周2 -4天(因為跨周日) 周一-3天 其他2天
                DateTime nCpTemp = Convert.ToDateTime(eDtmp).AddDays(-6);
                switch (nCpTemp.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        nCpTemp = nCpTemp.AddDays(-2);
                        break;
                    case DayOfWeek.Saturday:
                        nCpTemp = nCpTemp.AddDays(-1);
                        break;
                    case DayOfWeek.Tuesday:
                        nCpTemp = nCpTemp.AddDays(-4);
                        break;
                    case DayOfWeek.Monday:
                        nCpTemp = nCpTemp.AddDays(-3);
                        break;
                    default:
                        nCpTemp = nCpTemp.AddDays(-2);
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

    protected void grvCardData_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "VIEW")
        {
            LinkButton lnkView = (LinkButton)e.CommandSource;
            GridViewRow rowView = (GridViewRow)lnkView.NamingContainer;
            string[] keys = e.CommandArgument.ToString().Split(';');
            AML_SessionState sessionOBJ = new AML_SessionState();
            if (keys.Length != 5)
            {
                strAlertMsg = @"『" + MessageHelper.GetMessage("01_01080114_002") + "』" + @"\n";
                sbRegScript.Append("alert('" + strAlertMsg + "');");
                return;
            }
            sessionOBJ.RMMBatchNo = keys[0];
            sessionOBJ.AMLInternalID = keys[1];
            sessionOBJ.CaseProcess_User = keys[2];
            sessionOBJ.CASE_NO = keys[3];
            Session["P010801140001_SESSION"] = sessionOBJ;

            //20211007_EOS_AML(NOVA) by Ares Jack
            string NavigateUrl = "";
            string corpNo = keys[4];
            int intTemp;
            //開頭是數字為統編，否則為自然人ID
            if (Int32.TryParse(corpNo.Substring(0, 1), out intTemp))
            {
                NavigateUrl = "P010801140101.aspx";//法人 結案明細
            }
            else
            {
                NavigateUrl = "P010801170001.aspx";//自然人收單 結案明細
            }
            //string NavigateUrl = "P010801140101.aspx";//法人 結案明細
            //20210412_Ares_Stanley-調整轉向方法
            Response.Redirect(NavigateUrl, false);

        }
    }
    /// <summary>
    /// 檢查user登入物件  初始化字典
    /// </summary>
    protected void checkUserAgent()
    {
        if (eAgentInfo == null)
        {
            //UPDATEPANLE 自動POSTBACK 不會經過PAGE_LOAD 必須重取 eAgentInfo
            eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        }
        ///實作字典
        buiInfoDict();
    }
    /// <summary>
    /// 實作字典
    /// </summary>
    private void buiInfoDict()
    {
        ///風險區分字典
        initDCRiskRank();
    }
    /// <summary>
    /// 建構字典 DCRiskRank
    /// </summary>
    private void initDCRiskRank()
    {
        DCRiskRank = new Dictionary<string, string>();
        DCRiskRank.Add("H", "高");
        DCRiskRank.Add("M", "中");
        DCRiskRank.Add("L", "低");
    }

    /// <summary>
    /// 取出指定字典的值
    /// </summary>
    /// <param name="inObj"></param>
    /// <param name="inKey"></param>
    /// <returns></returns>
    private string GetDcValue(Dictionary<string, string> inObj, string inKey)
    {
        if (inObj == null)
        {
            checkUserAgent();
        }
        string rtnVal = "";
        if (inObj.ContainsKey(inKey))
        {
            rtnVal = inObj[inKey];
        }
        return rtnVal;
    }

    private bool DateTimeCheck(string sDate)
    {
        if (sDate.Trim().Length != 8)
        {
            //日期格式有誤：長度不足8碼
            return false;
        }
        if (!checkDateTime(sDate.Trim()))
        {
            //日期輸入不正確
            return false;
        }
        return true;
    }
    #endregion


    //20191105-RQ-2018-015749-002
    protected void btnDownload_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtLastUpdateDateBegin.Text) && !DateTimeCheck(txtLastUpdateDateBegin.Text.Trim()))
        {
            base.sbRegScript.Append("alert('結案日期(起)格式錯誤');$('#txtLastUpdateDateBegin').focus();");
            return;
        }
        if (!string.IsNullOrEmpty(txtLastUpdateDateEnd.Text) && !DateTimeCheck(txtLastUpdateDateEnd.Text.Trim()))
        {
            base.sbRegScript.Append("alert('結案日期(迄)格式錯誤');$('#txtLastUpdateDateEnd').focus();");
            return;
        }
        //依傳入條件查詢，做成Datatable回傳
        AML_HQQuery QueryObj = new AML_HQQuery();
        GetParamByPage(ref QueryObj);
        Session["P010801010001CASE_QUERY"] = QueryObj;//記下當前查詢條件

        DataTable dt = BRAML_HQ_Work.ReportQuery(QueryObj);

        if (dt.Rows.Count < 0)
        {
            strAlertMsg = "查無這條件下的資料!!";
            return;
        }

        //用執行結果來產excel
        ViewState["DataBind"] = dt;

        //* 匯出到Excel.
        OutputExcel(dt);
    }
    /// <summary>
    /// 將查詢結果匯出到Excel文檔。
    /// </summary>
    protected void OutputExcel(DataTable _dt)
    {
        try
        {
            string _ExcelTemplate = string.Empty;
            string _FileName = string.Empty;
            _ExcelTemplate = "080114CaseClosed.xls";
            _FileName = "結案報表下載";

            string strMsgID = "";
            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            //* 服務器端，生成Excel文檔
            if (!BRAML_HQ_Work.CreateExcelFile(_dt,
                            ((EntityAGENT_INFO)this.Session["Agent"]).agent_name,
                            _ExcelTemplate,
                            ref strServerPathFile,
                            ref strMsgID))
            {
                if (strMsgID != "")
                    base.strClientMsg += MessageHelper.GetMessage(strMsgID);
                else
                    base.strClientMsg += MessageHelper.GetMessage("01_01030400_004");//匯出特店資料異動(依統編)檔案失敗
                return;
            }

            //* 將服務器端生成的文檔，下載到本地。
            string strYYYYMMDD = "000" + CSIPCommonModel.BaseItem.Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
            string strFileName = _FileName.Trim() + strYYYYMMDD + ".xls";

            //* 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = strServerPathFile;
            this.Session["ClientFile"] = strFileName;
            // string urlString = @"ClientMsgShow('" + MessageHelper.GetMessage("01_01080114_009") + "');";
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("01_01080114_009") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += MessageHelper.GetMessage("01_01080114_010");
        }
    }
}