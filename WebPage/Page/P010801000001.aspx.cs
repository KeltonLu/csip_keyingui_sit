// *****************************************************************
//   作    者：林家賜
//   功能說明：AML資料 案件列表
//   創建日期：2019/02/03
//   修改記錄：
// <author>            <time>            <TaskID>                <desc>
// ******************************************************************

using System;
using System.Data;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Logging;
using System.Configuration;

public partial class Page_P010801000001 : PageBase
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
        validUser();
            if (Session["P010801010001CASE_QUERY"] != null)
            {
                AML_HQQuery QueryObj = (AML_HQQuery)Session["P010801010001CASE_QUERY"];
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
        //查詢前先清除畫面上會用到的控制項，避免資料殘留
        this.gpList.CurrentPageIndex = 1;
        //依傳入條件查詢，做成Datatable回傳
        AML_HQQuery QueryObj = new AML_HQQuery();
        GetParamByPage(ref QueryObj);
        Session["P010801010001CASE_QUERY"] = QueryObj;//記下當前查詢條件
        QueryData( QueryObj);
    }
    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="QueryObj"></param>
    private void QueryData(AML_HQQuery QueryObj)
    {

        DataTable dt = BRAML_HQ_Work.Query(QueryObj);
    
        //將結果設定為ViewState
        ViewState["DataBind"] = dt;

        ///將查詢結果綁定
        BindGridView();
        if (dt.Rows.Count == 0)
        {
            string strAlertMsg = MessageHelper.GetMessages("01_01080100_003");
            sbRegScript.Append("alert('" + strAlertMsg + "');");
       
        }
    }

    /// <summary>
    /// 取得頁面上輸入查詢條件
    /// </summary>
    /// <param name="QueryObj"></param>
    private void GetParamByPage(ref AML_HQQuery QueryObj)
    {
        #region "案件類型"
        if (radBasicNewproj.Checked == true)
        {
            QueryObj.CaseType = "new";
        }
        if (radBasicProcing.Checked == true)
        {
            QueryObj.CaseType = "procing";
        }
        if (radBasicMaster.Checked == true)
        {
            QueryObj.CaseType = "Master";
        }
        if (radBasicReject.Checked == true)
        {
            QueryObj.CaseType = "Reject";
        }
        if (radBasicALL.Checked == true)
        {
            QueryObj.CaseType = "ALL";
        }
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

        //20191025-RQ-2018-015749-002 modify by Peggy
        #region 不合作註記
        if (dropIncorporated.SelectedIndex != -1)
        {
            QueryObj.IncorporatedFlag = dropIncorporated.SelectedValue;
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

    protected void btnAdd_Click(object sender, EventArgs e)
    {

    }

    protected void btnJob_Click(object sender, EventArgs e)
    {

        bool result = false;

        EntityAGENT_INFO eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];

        result = new BatchJob_SendTwoMonthAddressLebel().ExecuteManual();

        if (result)
        {
            strAlertMsg = MessageHelper.GetMessage("01_01080100_004");
            base.strClientMsg += MessageHelper.GetMessage("01_01080100_004");
        }
        else
        {
            strAlertMsg = MessageHelper.GetMessage("01_01080100_005");
            base.strClientMsg += MessageHelper.GetMessage("01_01080100_005");
        }


    }
    #endregion

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
        //20191025-RQ-2018-015749-002 by Peggy
        txtIncorporated.Text = "請選擇";

        #region "案件類型"
        switch (QueryObj.CaseType)
        {
            case "new":
                radBasicNewproj.Checked = true;
                break;
            case "procing":
                radBasicProcing.Checked = true;
                break;
            case "Master":
                radBasicMaster.Checked = true;
                break;
            case "Reject":
                radBasicReject.Checked = true;
                break;
            case "ALL":
                radBasicALL.Checked = true;
                break;
        }
        #endregion
        #region "統編"
        txtTaxID.Text = QueryObj.TaxNo;
        #endregion
        #region "案號"

        txtCASE_NO.Text = QueryObj.CaseNo  ;
     
        #endregion
        #region "開始年月"
        if (!string.IsNullOrEmpty(QueryObj.yearS)  && !string.IsNullOrEmpty(QueryObj.MonthS))
        {
            txtProjYearBegin.Text = QueryObj.yearS;
            txtProjMonthBegin.Text = QueryObj.MonthS;
        }
      
        #endregion
        #region "結束年月"
        if (!string.IsNullOrEmpty(QueryObj.yearE) && !string.IsNullOrEmpty(QueryObj.MonthE) )
        {
            txtProjYearEnd.Text = QueryObj.yearE ;
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

        //20191025-RQ-2018-015749-002 by Peggy
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

        #region "排序"
        if (!string.IsNullOrEmpty(QueryObj.OrderBy ))
        {
           
            foreach (ListItem litem in dropSort.Items)
            {
                if (litem.Value ==  QueryObj.OrderBy)
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
        //20191205-RQ-2018-015749-002-新增主管看的到經辦的案件
        //沒有主管權限就不看到主管放行
        if (!(userRolls.Contains("CSIP0121") || userRolls.Contains("CSIP0122")))
        {
            radBasicMaster.Checked = false;
            radBasicMaster.Visible = false;
        }
        else
        {
            radBasicMaster.Visible = true;
            radBasicMaster.Checked = true;
            isMaster = true;
        }

        //如果還是有經辦M1，則還是要把新案加回來
        if (userRolls.Contains("CSIP0120"))
        {
            radBasicMaster.Checked = false;
            radBasicNewproj.Checked = true; //新案變為選取
        }
        /*
        if (userRolls.Contains("CSIP0121") || userRolls.Contains("CSIP0122"))
        {
            radBasicNewproj.Visible = false;
            radBasicProcing.Visible = false;
            radBasicProcing.Checked = false;
            radBasicMaster.Checked = true;//看不到新案，經辦處理，主管放行變為選取
            radBasicMaster.Visible = true;
            isMaster = true;
        }
        //如果還是有經辦M1，則還是要把新案加回來{
        if (userRolls.Contains("CSIP0120"))
        {
            radBasicNewproj.Visible = true;
            radBasicProcing.Visible = true;
            radBasicMaster.Checked = false; 
            radBasicNewproj.Checked = true; //新案變為選取
        }
        //送審退件22不可見
        if (userRolls.Contains("CSIP0122"))
        {
            radBasicReject.Visible = false;           
        }
        //送審退件22不可見 如果還是有經辦M1，或C1 則還是要把送審退件加回來{
        if (userRolls.Contains("CSIP0120") || userRolls.Contains("CSIP0121"))
        {
            radBasicReject.Visible = true; 
        }

        //沒有主管權限就不看到主管放行
        if (!(userRolls.Contains("CSIP0121") || userRolls.Contains("CSIP0122")))
        {
            radBasicMaster.Checked = false;
            radBasicMaster.Visible = false;
        }
        */

        //追加數計 
        string urows = GetqueryRole();
        int vcnt = 0;
        if (radBasicNewproj.Visible == true)  //new 
        {
          int r1  =  BRAML_HQ_Work.getProjectCount("new", urows, "");
         cntNew.InnerText = "(" + r1.ToString() + ")";
            vcnt += r1;
        }
        if (radBasicProcing.Visible == true)  //procing
        {
             //int r2 = BRAML_HQ_Work.getProjectCount("procing", urows, "");
            int r2 = BRAML_HQ_Work.getProjectCount("procing", urows, eAgentInfo.agent_id);
            cntprocing.InnerText = "(" + r2.ToString() + ")";
            vcnt += r2;
        }
        if (radBasicMaster.Visible == true)  //Master
        { 
            int r3 = BRAML_HQ_Work.getProjectCount("Master", urows, "");
            cntMaster.InnerText = "(" + r3.ToString() + ")";
            vcnt += r3;

        }
        if (radBasicReject.Visible == true)  // Reject
        {        
            int r4 = BRAML_HQ_Work.getProjectCount("Reject", urows, "");
            cntReject.InnerText = "(" + r4.ToString() + ")";
            vcnt += r4;
        }
       
            cntALL.InnerText = "(" + vcnt.ToString() + ")";
       
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
        //20191209-RQ-2018-015749-002 主管權限要能看到經辦的案件
        if (userRolls.Contains("CSIP0120"))//經辦
        {
            //rtn += "M1,";
            rtn = "M1,";
        }
        if (userRolls.Contains("CSIP0121"))//一階主管
        {
            //rtn += "C1,";
            rtn = "M1,C1,";
        }
        if (userRolls.Contains("CSIP0122"))//二階主管
        {
            //rtn += "C2,";
            rtn += "M1,C1,C2,";
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
    /// 修改紀錄:2021/01/21_Ares_Stanley-設定查詢結果欄位css
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
            for (int i = 0; i < grvCardData.Columns.Count; i++)
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

        //20191025-RQ-2018-015749-002 modify by Peggy
        //不合作註記
        txtIncorporated.Text = "請選擇";
    }
    /// <summary>
    /// 設定經辦
    /// </summary>
    private void SetDropCreateUser()
    {
        DataTable dt = BRAML_HQ_Work.getProcUser();
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

        int startYear = 2019;
        int nowYear =  DateTime.Now.Year;
        for (int i = nowYear; i >= startYear; i--)
        {
            string cueYear =i.ToString();
            listItem = new ListItem();
            listItem.Value = cueYear;
            listItem.Text = cueYear;
            dropProjYearEnd.Items.Add(listItem);
            dropProjYearBegin.Items.Add(listItem);
        }


        for (int i = 1; i < 13; i++)
        {
            listItem = new ListItem();
            listItem.Value = i.ToString().PadLeft(2, '0');
            listItem.Text = i.ToString().PadLeft(2, '0');
            dropProjMonthEnd.Items.Add(listItem);
            dropProjMonthBegin.Items.Add(listItem);
        }
        txtProjMonthBegin.Text = "請選擇";
        txtProjMonthEnd.Text = "請選擇";
        txtProjYearEnd.Text = "請選擇";
        txtProjYearBegin.Text = "請選擇";
        ListItem listItem1 = new ListItem();
        listItem1.Text = "請選擇";
        this.dropProjMonthBegin.Items.Insert(0,listItem1);
        this.dropProjMonthEnd.Items.Insert(0, listItem1);
        this.dropProjYearEnd.Items.Insert(0, listItem1);
        this.dropProjYearBegin.Items.Insert(0, listItem1);

    }

    #endregion


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
            if (!string.IsNullOrEmpty(AddressLabelTwoMonthFlagTime) && AddressLabelTwoMonthFlagTime.Length > 8 )
            {
                e.Row.Cells[9].Text = Convert.ToDateTime(AddressLabelTwoMonthFlagTime).ToString("yyyyMMdd");
            }
            //END依建案日期計算到期日
            e.Row.Cells[4].Text = GetDcValue(DCRiskRank, rowView["OriginalRiskRanking"].ToString());

            //顯示經辦人員
            string eName = rowView["CaseOwner_User"].ToString();
            foreach(ListItem litem in dropCaseOwner_User.Items)
            {
                //20200318-RQ-2019-030155-003 修改：修正因帳號大小寫不同而無法顯示問題。
                //if (litem.Value == eName)
                if (litem.Value.Trim().ToUpper() ==eName.Trim().ToUpper())
                {
                    e.Row.Cells[11].Text = litem.Text;
                    break;
                }
            }

            //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
            // 2021/7/6 EOS_AML(NOVA) by Ares Dennis
            //if (rowView["CASE_NO"].ToString() != rowView["GROUP_NO"].ToString() && rowView["CASE_NO"].ToString().Substring(6, 1) != "8" && rowView["CASE_NO"].ToString().Substring(6, 1) != "9")
            //{
            //    e.Row.Cells[1].Enabled = false;
            //}
            //// 2021/9/7 EOS_AML(NOVA) by jack  GroupNo等於空白的時候要可以點
            //if (rowView["GROUP_NO"].ToString() == "")
            //{
            //    e.Row.Cells[1].Enabled = true;
            //}
            //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis
        }
    }
    /// <summary>
    /// 計算到期日與寄送通知日
    /// </summary>
    /// <param name="endDate"></param>
    /// <param name="noCropDate"></param>
    private void proceDate(string pDate,ref string endDate, ref string noCropDate)
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
                strAlertMsg = @"『" + MessageHelper.GetMessage("01_01080100_002") + "』" + @"\n" ;
                sbRegScript.Append("alert('" + strAlertMsg + "');");
                return;
            }
            sessionOBJ.RMMBatchNo =keys[0];
            sessionOBJ.AMLInternalID = keys[1];
            sessionOBJ.CaseProcess_User = keys[2];
            sessionOBJ.CASE_NO = keys[3];            
            Session["P010801000001_SESSION"] = sessionOBJ;

            //20210806 EOS_AML(NOVA) by Ares Dennis
            string NavigateUrl = "";
            string corpNo = keys[4];
            int intTemp;
            //開頭是數字為統編，否則為自然人ID
            if(Int32.TryParse(corpNo.Substring(0, 1), out intTemp))
            {
                NavigateUrl = "P010801010001.aspx";
                if (validUser() && sessionOBJ.CaseProcess_User != "M1") //主管 且案件非M1
                {
                    NavigateUrl = "P010801020001.aspx";
                }
            }
            else
            {
                NavigateUrl = "P010801150001.aspx";//自然人收單定審明細
                //20211004_Ares_Jack_自然人收單定審案件明細(主管)
                if (validUser() && sessionOBJ.CaseProcess_User != "M1") //主管 且案件非M1
                {
                    NavigateUrl = "P010801160001.aspx";
                }
            }

            
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
    private string GetDcValue(Dictionary<string, string> inObj,string inKey)
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
}