//******************************************************************
//*  作    者：Eric
//*  功能說明：餘額轉置
//*  創建日期：2014/10
//*  修改記錄：
//* <author>            <time>            <TaskID>                <desc>
//* Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//* Ares Stanley       2020/11/19                                 修正新增/刪除按鈕異常
//*******************************************************************
using System;
using System.Data;
using System.Collections;
using System.Web.UI.WebControls;
using Framework.Common.Message;
using Framework.WebControls;
using Framework.Common.Logging;
using CSIPCommonModel.EntityLayer;
using System.IO;
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Utility;

public partial class P010211000001 : PageBase
{
    #region 變數區
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息
    private DateTime tDay = DateTime.Now;   // 次營業日
    private DateTime s_today = DateTime.Now;// 餘轉日期時間
    public string cbtn_disable = "";
    public DataTable dt_hash = new DataTable();
    private string PIDtype = "";
    #endregion

    #region event

    protected void Page_Load(object sender, EventArgs e)
    {
        DataTable dt = new DataTable();

        #region 檢核餘轉日期時間是否錯誤
        if (!BaseHelper.GetCommonProperty("01", "31", ref dt))
        {
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_03100000_001");
        }
        else
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                DateTime DT_try = DateTime.Now;
                string vDay = string.Format("{0} {1}", DateTime.Now.ToString("yyyy/MM/dd"),
                    dt.Rows[0]["PROPERTY_CODE"].ToString().Trim());

                if (!DateTime.TryParse(vDay, out DT_try))
                {
                    cbtn_disable = "Y";
                    base.strClientMsg += MessageHelper.GetMessage("01_02110001_006");
                    MessageHelper.ShowMessage(this.UpdatePanel1, "01_02110001_006");
                    return;
                }
                else
                    s_today = DateTime.Parse(vDay);
            }
            else
            {
                cbtn_disable = "Y";
                base.strClientMsg += MessageHelper.GetMessage("01_02110001_006");
                MessageHelper.ShowMessage(this.UpdatePanel1, "01_02110001_006");
                return;
            }
        }
        #endregion

        if (!IsPostBack)
        {
            //* 綁定GridView列頭訊息
            ShowControlsText();
            //this.gpList.RecordCount = 0;
            //this.gpList.Visible = false;
            //this.gvpbSearchRecord.DataSource = null;
            //this.gvpbSearchRecord.DataBind();

            //this.GridPager1.RecordCount = 0;
            //this.GridPager1.Visible = false;
            this.CustGridView1.DataSource = null;
            this.CustGridView1.DataBind();
        }

        #region 取得次營業日
        dt = BaseHelper.GetWORK_DATE("01", tDay.ToString("yyyyMMdd"));

        if (dt != null && dt.Rows.Count > 0)
        {
            string tempDay = string.Format("{0}/{1}/{2}", dt.Rows[0][0].ToString().Substring(0, 4),
                dt.Rows[0][0].ToString().Substring(4, 2), dt.Rows[0][0].ToString().Substring(6, 2));
            if (DateTime.Parse(tempDay) > tDay)
            {
                tDay = DateTime.Parse(tempDay);
            }
        }
        #endregion

        base.strHostMsg += "";
        base.strClientMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }

    private void ShowControlsText()
    {
        this.CustGridView1.Columns[0].HeaderText = BaseHelper.GetShowText("01_0211000001_008");
        this.CustGridView1.Columns[2].Visible = false;
        this.CustGridView1.Columns[3].HeaderText = BaseHelper.GetShowText("01_0211000001_009");
        this.CustGridView1.Columns[4].HeaderText = BaseHelper.GetShowText("01_0211000001_003");
        this.CustGridView1.Columns[5].HeaderText = BaseHelper.GetShowText("01_0211000001_004");
        this.CustGridView1.Columns[6].HeaderText = BaseHelper.GetShowText("01_0211000001_007");
        this.CustGridView1.Columns[7].HeaderText = BaseHelper.GetShowText("01_0211000001_010");
        this.CustGridView1.Columns[8].HeaderText = BaseHelper.GetShowText("01_0211000001_011");
        this.CustGridView1.Columns[9].HeaderText = BaseHelper.GetShowText("01_0211000001_012");
        this.CustGridView2.Columns[6].Visible = false;

        //* 設置每頁顯示記錄最大條數
        //this.gpList.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
        //this.gvpbSearchRecord.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());

        //this.GridPager1.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
        //this.CustGridView1.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
    }

    //protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    //{
    //    this.gpList.CurrentPageIndex = e.NewPageIndex;
    //    this.BindGridView();
    //}

    #region button click

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Account_Nbr = this.txtKey.Text;

        //20200109-修改SOC存入條件
        //log.Statement_Text = string.Format("AC_NO:{0}", log.Account_Nbr); //查詢條件內容: 用 | 區隔,要看文件確認        
        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔

        BRL_AP_LOG.Add(log);
        //------------------------------------------------------

        PIDtype = "btn";
        //this.gpList.CurrentPageIndex = 0;
        //* 將【信用卡卡號】字段保存到ViewState中，
        this.ViewState["Key"] = this.txtKey.Text.Trim();
        //* 以【信用卡卡號】字段查詢記錄訊息，并顯示到畫面
        //this.BindGridView();
        //* 顯示頁面導航
        //this.gpList.Visible = true;
        //* 將光標設置到【信用卡卡號】欄位
        base.sbRegScript.Append(BaseHelper.SetFocus("txtKey"));
        string CARD_NUM = CommonFunction.GetSubString(this.txtKey.Text.Trim().ToUpper(), 0, 16);
        string PID = this.txtPID.Text.Trim().ToUpper();
        Hashtable htInputP4_JCFA;
        //Hashtable htInputP4D_JCFA;
        //Hashtable htOutputP4D_JCFA;

        #region 獲取主機資料

        //*讀取第一卡人檔
        htInputP4_JCFA = new Hashtable();
        htInputP4_JCFA.Add("CARD_NMBR", CARD_NUM);
        htInputP4_JCFA.Add("FUNCTION_CODE", "2");
        htInputP4_JCFA.Add("SVCP_ID", "");
        htInputP4_JCFA.Add("LINE_CNT", "0000");

        #region 2014/10/03 判斷是否要傳P4D用卡號前四位判斷 by Eric
        HtgType hType = HtgType.P4_JCFA;
        DataTable dt_ProCode = new DataTable(); ;

        if (!BaseHelper.GetCommonProperty("01", "30", ref dt_ProCode))
        {
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_03100000_001");
        }
        else
        {
            for (int i = 0; i < dt_ProCode.Rows.Count; i++)
            {
                if (this.txtKey.Text.Trim().Substring(0, 4).Equals(
                    dt_ProCode.Rows[i]["PROPERTY_CODE"].ToString().Trim()))
                {
                    hType = HtgType.P4D_JCFA;
                    break;
                }
            }
        }

        #endregion

        Search_Click(htInputP4_JCFA, hType, PIDtype);

        #endregion 獲取主機資料結束
    }

    protected void btnPID_Click(object sender, EventArgs e)
    {
        PIDtype = "btn";
        base.sbRegScript.Append(BaseHelper.SetFocus("txtPID"));
        btnPID_Click_Extend(this.txtPID.Text, this.txtKey.Text);
    }

    protected void btnPID_Click_Extend(string refPID, string refCard)
    {
        //this.gpList.CurrentPageIndex = 0;
        //* 將【PID】字段保存到ViewState中，
        this.ViewState["Key"] = refPID.Trim();
        //* 以【PID】字段查詢記錄訊息，并顯示到畫面
        //this.BindGridView();
        //* 顯示頁面導航
        //this.gpList.Visible = true;
        //* 將光標設置到【PID】欄位
        string PID = refPID.Trim().ToUpper();
        //string CARD_NUM = CommonFunction.GetSubString(refCard.Trim().ToUpper(), 0, 16);
        Hashtable htInputP4_JCFA;
        HtgType hType = HtgType.P4_JCFA;

        #region 獲取主機資料

        //*讀取第一卡人檔
        htInputP4_JCFA = new Hashtable();
        htInputP4_JCFA.Add("SVCP_ID", PID);
        htInputP4_JCFA.Add("FUNCTION_CODE", "1");
        htInputP4_JCFA.Add("LINE_CNT", "0000");
        htInputP4_JCFA.Add("CARD_NMBR", "");

        Search_Click(htInputP4_JCFA, hType, PIDtype);

        #endregion 獲取主機資料結束
    }

    protected void BtnAdd_Click(object sender, EventArgs e)
    {
        EntityAuto_Balance_Trans eBalanceAdd = new EntityAuto_Balance_Trans();

        if (this.CustGridView2.Rows.Count > 0)
        {
            for (int i = 0; i < this.CustGridView2.Rows.Count; i++)
            {
                RadioButton radio = (RadioButton)this.CustGridView2.Rows[i].Cells[0]
                    .FindControl("rb_select");

                if (radio != null && radio.Checked)
                {
                    eBalanceAdd.Newid = Guid.NewGuid();
                    eBalanceAdd.CardNo = this.CustGridView2.Rows[i].Cells[1].Text.Replace("&nbsp;", "");
                    eBalanceAdd.PID = this.CustGridView2.Rows[i].Cells[2].Text.Replace("&nbsp;", "");

                    if (BRBalance_Trans.SelectLOG("", eBalanceAdd.PID, DateTime.Now > s_today ? tDay : DateTime.Now))
                    {
                        // 該PID今日已新增，不可重覆新增
                        MessageHelper.ShowMessage(this.UpdatePanel1, "01_02110001_004");
                        for (int j = 0; j < this.CustGridView1.Rows.Count; j++)
                        {
                            if (this.CustGridView1.Rows[j].Cells[8].Text == "刪除")
                                this.CustGridView1.Rows[j].Cells[1].Text = "";
                        }

                        return;
                    }

                    CustDropDownList cddl = (CustDropDownList)this.CustGridView2.Rows[i].Cells[6]
                        .FindControl("cddl_ReasonCode");

                    if (cddl != null)
                        eBalanceAdd.Reason_Code = cddl.SelectedValue;
                    else
                        eBalanceAdd.Reason_Code = this.CustGridView2.Rows[i].Cells[6].Text.Trim();

                    eBalanceAdd.Memo = !string.IsNullOrEmpty(this.txt_Memo.Text) ? this.txt_Memo.Text.Trim() : "";
                    eBalanceAdd.Upload_Flag = "N";
                    eBalanceAdd.Process_Flag = "N";
                    eBalanceAdd.Create_User = eAgentInfo.agent_id;
                    eBalanceAdd.Create_DateTime = DateTime.Now;
                    eBalanceAdd.Modify_User = eBalanceAdd.Create_User;
                    eBalanceAdd.Modify_DateTime = eBalanceAdd.Create_DateTime;
                    eBalanceAdd.Trans_Date = eBalanceAdd.Create_DateTime > s_today ? tDay : eBalanceAdd.Create_DateTime;
                    InsertEntity(eBalanceAdd);
                }
            }
        }
        else
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, "01_04040000_001");
            return;
        }
    }
    /// <summary>
    /// 修改紀錄:2021/01/27_Ares_Stanley-新增卡號與PID不能相同的檢核條件; 2021/04/29_Ares_Stanley-修正陣列長度錯誤
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnImport_Click(object sender, EventArgs e)
    {
        PIDtype = "import";
        if (this.fulFilePath.HasFile)
        {
            string[] subName = this.fulFilePath.FileName.Split('.');

            if (subName[1].ToString().ToLower() == "txt")
            {
                int number = 0;
                int counter = 0;
                string msg = "";
                string f_msg = "";
                string pathName = string.Format("{0}{1}_{2}.txt", Server.MapPath("~/Upload/"),
                    DateTime.Now.ToString("yyyyMMdd_HHmmss"), subName[0]);

                try
                {
                    this.fulFilePath.SaveAs(pathName);
                    if (System.IO.File.Exists(pathName))
                    {
                        System.IO.StreamReader sr = new System.IO.StreamReader(pathName, System.Text.Encoding.Default);
                        DateTime start = DateTime.Now;

                        #region 匯入檢核如PID有重覆、卡號PID相同整批不匯入，訊息需提示重複PID(檔案內重複或當日已鍵檔)
                        string[] file = sr.ReadToEnd().Split(new string[] { Environment.NewLine },
                            StringSplitOptions.RemoveEmptyEntries);
                        string[] arrayPID = new string[file.Length];

                        for (int i = 0; i < arrayPID.Length; i++)
                        {
                            object oPID = file[i].Substring(16, 16);
                            object oCardNo = file[i].Substring(0, 16);
                            if(oPID.ToString() == oCardNo.ToString())
                            {
                                f_msg += string.Format("第{0}筆,PID與卡號相同{1}", i + 1, "\\r\\n");
                            }
                            if (Array.IndexOf(arrayPID, oPID) != -1)
                                f_msg += string.Format("第{0}筆,PID:{1},檔案內重複{2}", i + 1, oPID, "\\r\\n");
                            else
                                arrayPID[i] = file[i].Substring(16, 16);
                        }

                        for (int i = 0; i < arrayPID.Length; i++)
                        {
                            if (arrayPID[i] != null)
                            {
                                if (BRBalance_Trans.SelectLOG("", arrayPID[i], DateTime.Now > s_today ? tDay : DateTime.Now))
                                {
                                    msg += string.Format("第{0}筆,PID:{1},資料庫已存在{2}", i + 1, arrayPID[i],
                                        "\\r\\n");
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(f_msg) || !string.IsNullOrEmpty(msg))
                        {
                            msg = string.Format("<Script language='JavaScript'>alert('{0}{1}');</Script>", f_msg, msg);
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", msg);
                            return;
                        }

                        sr.Close();
                        #endregion

                        msg = "";
                        for (int i = 0; i < file.Length; i++)
                        {
                            if (file[i].Length < 33)
                                msg += string.Format("第{0}筆,資料格式錯誤{1}", i + 1, "\\r\\n");
                        }

                        if (!string.IsNullOrEmpty(msg))
                        {
                            base.strClientMsg += msg;
                            return;
                        }

                        #region 2014/12/08 送主機檢核PID是否正確
                        //bool check_cardNo = false;

                        //msg = "";
                        //for (int j = 0; j < file.Length; j++)
                        //{
                        //    number++;
                        //    dt_hash = new DataTable();
                        //    btnPID_Click_Extend(file[j].Substring(16, 16), PIDtype);

                        //    #region 檢核卡號是否存在主機回傳的資料中
                        //    if (dt_hash != null && dt_hash.Rows.Count > 0)
                        //    {
                        //        for (int i = 0; i < dt_hash.Rows.Count; i++)
                        //        {
                        //            if (dt_hash.Rows[i]["CARD_NMBR"] != null &&
                        //                !string.IsNullOrEmpty(dt_hash.Rows[i]["CARD_NMBR"].ToString()))
                        //            {
                        //                if (dt_hash.Rows[i]["CARD_NMBR"].ToString() == file[j].Substring(0, 16))
                        //                {
                        //                    check_cardNo = true;
                        //                    break;
                        //                }
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        msg += string.Format("第{0}筆,PID:{1},PID不存在{2}", number,
                        //            file[j].Substring(16, 16), "\\r\\n");
                        //        msg = string.Format("<Script language='JavaScript'>alert('{0}');</Script>", msg);
                        //        ClientScript.RegisterStartupScript(this.GetType(), "alert", msg);
                        //        return;
                        //    }

                        //    if (check_cardNo == false)
                        //        msg += string.Format("第{0}筆,卡號:{1},卡號錯誤{2}", number,
                        //            file[j].Substring(0, 16), "\\r\\n");

                        //    #endregion
                        //}

                        //if (!string.IsNullOrEmpty(msg))
                        //{
                        //    msg = string.Format("<Script language='JavaScript'>alert('{0}');</Script>", msg);
                        //    ClientScript.RegisterStartupScript(this.GetType(), "alert", msg);

                        //    return;
                        //}
                        #endregion

                        msg = "";
                        number = 0;
                        for (int i = 0; i < file.Length; i++)
                        {
                            number++;
                            EntityAuto_Balance_Trans balance = new EntityAuto_Balance_Trans();
                            balance.Newid = Guid.NewGuid();
                            balance.CardNo = file[i].Substring(0, 16);
                            balance.PID = file[i].Substring(16, 16);
                            balance.Reason_Code = file[i].Substring(32, 1);
                            balance.Memo = file[i].Substring(33);
                            balance.Upload_Flag = "N";
                            balance.Process_Flag = "N";
                            balance.Create_User = eAgentInfo.agent_id;
                            balance.Create_DateTime = DateTime.Now;
                            balance.Modify_User = balance.Create_User;
                            balance.Modify_DateTime = balance.Create_DateTime;
                            balance.Trans_Date = balance.Create_DateTime > s_today ? tDay : balance.Create_DateTime;

                            if (!BRAuto_Balance_Trans.AddEntity(balance))
                                msg += string.Format("第{0}筆,卡號:{1},匯入失敗{2}", number, balance.CardNo,
                                    "\\r\\n");
                            else
                                counter++;
                        }

                        DateTime end = DateTime.Now;

                        if (!string.IsNullOrEmpty(msg))
                        {
                            msg = string.Format("<Script language='JavaScript'>alert('{0}');</Script>", msg);
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", msg);
                        }

                        string[] ids = new string[this.CustGridView1.Rows.Count];
                        DataTable dt_blance = new DataTable();

                        for (int i = 0; i < this.CustGridView1.Rows.Count; i++)
                        {
                            ids[i] = this.CustGridView1.Rows[i].Cells[2].Text;

                        }

                        dt_blance = (DataTable)BRBalance_Trans.BindImport(start, end, ids);

                        if (dt_blance != null && dt_blance.Rows.Count > 0)
                        {
                            this.CustGridView1.Columns[2].Visible = true;
                            this.CustGridView1.DataSource = dt_blance;
                            this.CustGridView1.DataBind();
                            this.CustGridView1.Columns[2].Visible = false;
                            base.strClientMsg += string.Format("已成功匯入{0}筆資料", counter);
                        }
                        else
                            base.strClientMsg += "無可匯入資料";
                    }
                }
                catch (Exception ex)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_02110001_001");
                    MessageHelper.ShowMessage(this.UpdatePanel1, "01_02110001_001");
                    Logging.Log(ex, LogLayer.DB);
                }
            }
            #region error else
            else
            {
                // 檔案格式錯誤，必須為txt檔案
                MessageHelper.ShowMessage(this.UpdatePanel1, "01_02110001_002");
            }
        }
        else
        {
            // 請選擇檔案
            MessageHelper.ShowMessage(this.UpdatePanel1, "01_02110001_003");
        }
        #endregion
    }

    /// <summary>
    /// 修改日期: 2020/12/04_Ares_Stanley-修正陣列長度不足錯誤, 無範本NPOI報表產出;
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void cbtn_Excel_Click(object sender, EventArgs e)
    {
        if (this.CustGridView1.Rows.Count > 0)
        {
            string[] guidArr = new string[] { };
            DataSet ds = new DataSet();

            this.CustGridView1.Columns[2].Visible = true;
            for (int i = 0; i < this.CustGridView1.Rows.Count; i++)
            {
                if (this.CustGridView1.Rows[i].Cells[2].Text != null)
                {
                    System.Array.Resize(ref guidArr, guidArr.Length + 1);
                    guidArr[i] = this.CustGridView1.Rows[i].Cells[2].Text;
                }
            }
            
            this.CustGridView1.Columns[2].Visible = false;
            ds = (DataSet)BRBalance_Trans.BindNewidsForExcel(guidArr);

            //string templateFileName = this.Server.MapPath(ConfigurationManager.AppSettings["ReportTemplate"].ToString() + "/餘額轉置匯出.xls");
            string templateFileName = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "Balance_Trans.xls";

            if (System.IO.File.Exists(templateFileName))
            {
                string path = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
                BRExcel_File.CheckDirectory(ref path);
                string filename = "/Balance_Trans_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls";
                BRAuto_pay_status bps = new BRAuto_pay_status();

                bps.Excel(ds, templateFileName, path, filename, "", "", "", "", "");

                #region 下載
                FileInfo file = new FileInfo(path + filename);
                this.Session["ServerFile"] = path + filename;
                this.Session["ClientFile"] = filename;
                // string urlString = @"ClientMsgShow('Balance_Trans');";
                string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: 'Balance_Trans' }, '*');";
                urlString += @"location.href='DownLoadFile.aspx';";
                base.sbRegScript.Append(urlString);
                #endregion
            }
        }
        else
        {
            // 您是否遺漏查詢，結果清單無資料可匯出
            MessageHelper.ShowMessage(this.UpdatePanel1, "01_02110001_005");
            return;
        }
    }

    #endregion

    #region GridView Event

    /// <summary>
    /// 功能說明:
    /// 作    者:
    /// 創建時間:
    /// 修改記錄: 2020/11/10 Ares Luke 處理白箱報告SQL Injection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gv_Deleting(object sender, GridViewDeleteEventArgs e)
    {
        string gv_Newid = this.CustGridView1.Rows[e.RowIndex].Cells[2].Text;
        if (BRBalance_Trans.UpdateProcess_Flag(gv_Newid))
        {
            string[] ids = new string[this.CustGridView1.Rows.Count];
            for (int i = 0; i < this.CustGridView1.Rows.Count; i++)
            {
                if (i != e.RowIndex)
                    ids[i] = this.CustGridView1.Rows[i].Cells[2].Text;
            }

            if (ids.Length > 0)
            {
                DataSet ds_blance = (DataSet)BRBalance_Trans.BindNewIds(ids);

                if (ds_blance != null && ds_blance.Tables[0].Rows.Count > 0)
                {
                    this.CustGridView1.Columns[2].Visible = true;
                    this.CustGridView1.DataSource = ds_blance.Tables[0];
                    this.CustGridView1.DataBind();
                    this.CustGridView1.Columns[2].Visible = false;
                    for (int i = 0; i < this.CustGridView1.Rows.Count; i++)
                    {
                        if (this.CustGridView1.Rows[i].Cells[8].Text == "刪除") //狀態欄
                            this.CustGridView1.Rows[i].Cells[1].Text = ""; //刪除按鈕欄
                    }
                }
            }
            else
            {
                if (this.CustGridView1.Rows.Count == 1)
                {
                    this.CustGridView1.Columns[2].Visible = true;
                    this.CustGridView1.DataSource = null;
                    this.CustGridView1.DataBind();
                    this.CustGridView1.Columns[2].Visible = false;
                }
            }
            // 刪除成功
            MessageHelper.ShowMessage(this.UpdatePanel1, "00_00000000_003");
        }
    }

    public void cgv2_Bound(object sender, GridViewRowEventArgs e)
    {
        CustDropDownList cddl = (CustDropDownList)e.Row.FindControl("cddl_ReasonCode");

        if (e.Row.Cells[6].Text.ToString() == "3" || e.Row.Cells[6].Text.ToString() == "4")
        {
            cddl.SelectedValue = e.Row.Cells[6].Text.ToString();
        }

        e.Row.Cells[8].Visible = true;
        if (string.IsNullOrEmpty(e.Row.Cells[1].Text.Trim().Replace("&nbsp;", "")))
        {
            e.Row.Cells[1].Text = e.Row.Cells[8].Text;
        }
        else if (string.IsNullOrEmpty(e.Row.Cells[2].Text.Trim().Replace("&nbsp;", "")))
        {
            e.Row.Cells[2].Text = e.Row.Cells[8].Text;
        }
        e.Row.Cells[8].Visible = false;
    }

    public void cgv1_Bound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex > -1)
        {
            if (e.Row.Cells[8].Text != "" || e.Row.Cells[8].Text != null)
            {
                switch (e.Row.Cells[8].Text)
                {
                    case "Y":
                        e.Row.Cells[8].Text = BaseHelper.GetShowText("01_0212000001_017");
                        break;
                    case "E":
                        e.Row.Cells[8].Text = BaseHelper.GetShowText("01_0212000001_018");
                        break;
                    case "D":
                        e.Row.Cells[1].Text = "";
                        e.Row.Cells[8].Text = "刪除";
                        break;
                    case "N":
                        e.Row.Cells[8].Text = "未處理";
                        break;
                }
            }
            e.Row.Cells[3].Text = DateTime.Parse(e.Row.Cells[3].Text).ToString("yyyy/MM/dd");
            e.Row.Cells[9].Text = DateTime.Parse(e.Row.Cells[9].Text).ToString("yyyy/MM/dd HH:mm:ss");
        }
    }

    #endregion

    protected void Search_Click(Hashtable input, HtgType htType, string type)
    {
        DataTable dt_temp = new DataTable();

        Hashtable output = MainFrameInfo.GetMainFrameInfo(htType, input, false, "1", eAgentInfo);
        if (output.Contains("HtgMsg"))
        {
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            etMstType = eMstType.Select;
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

            //主機傳回訊息缺少欄位
            //ReSetControls(false, false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));

            if (output["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg = output["HtgMsg"].ToString();
                base.strClientMsg = MessageHelper.GetMessage("01_01010700_009");
            }
            else
            {
                base.strClientMsg = output["HtgMsg"].ToString();
            }

            return;
        }

        #region 2014/10/13 隨主機回傳筆數動態產生
        int lineCnt = 0;

        if (output["LINE_CNT"] != null && int.TryParse(output["LINE_CNT"].ToString(), out lineCnt))
            lineCnt = int.Parse(output["LINE_CNT"].ToString());
        else
            lineCnt = 0;

        dt_hash = HashTableToDataTable(output, lineCnt, input);

        if (lineCnt > 20)
        {
            int count = 1;
            for (int i = 1; i <= lineCnt; i += 20)
            {
                int mainfram = count * 20;
                input.Remove("LINE_CNT");
                input.Remove("ISSUE_CNT");
                input.Remove("PURSE_ID");
                input.Add("LINE_CNT", lineCnt.ToString().PadLeft(4, '0'));
                input.Add("ISSUE_CNT", output["ISSUE_CNT"].ToString());
                input.Add("PURSE_ID", output["PURSE_ID"].ToString());

                output = MainFrameInfo.GetMainFrameInfo(htType, input, false, "1", eAgentInfo);
                if (output["LINE_CNT"] != null && int.TryParse(output["LINE_CNT"].ToString(), out lineCnt))
                {
                    lineCnt = int.Parse(output["LINE_CNT"].ToString());
                }
                else
                    lineCnt = 0;

                dt_temp = HashTableToDataTable(output, (lineCnt - mainfram) >= 20 ? 20 : lineCnt - mainfram, input);
                count++;
                if (dt_temp != null && dt_temp.Rows.Count > 0)
                {
                    foreach (DataRow row in dt_temp.Rows)
                    {
                        dt_hash.ImportRow(row);
                    }
                }
            }
        }

        if (dt_hash != null && dt_hash.Rows.Count > 0)
        {
            #region 2014/10/03 只需要顯示外顯註記是1的資料即可 by Eric
            for (int i = 0; i < dt_hash.Rows.Count; i++)
            {
                if (dt_hash.Rows[i]["ID_IND"].ToString() != "1")
                    dt_hash.Rows.RemoveAt(i);
            }
            #endregion
            if (type == "btn")
            {
                this.CustGridView2.Columns[6].Visible = true;
                this.CustGridView2.Columns[8].Visible = true;
                this.CustGridView2.DataSource = dt_hash;
                this.CustGridView2.DataBind();
                this.CustGridView2.Columns[6].Visible = false;
                this.CustGridView2.Columns[8].Visible = false;

                if (this.CustGridView2.Rows.Count > 10)
                    this.Pl_gv1.Height = 325;
            }
        }
        else
        {
            // 查無資料
            MessageHelper.ShowMessage(this.UpdatePanel1, "01_04040000_001");
        }

        if (type == "btn")
        {
            #region 2014/10/20 查詢主機時也要查詢DB
            DataTable dtBalance = (DataTable)BRBalance_Trans.BindSearch(this.txtKey.Text.Trim(), this.txtPID.Text.Trim());

            if (dtBalance != null && dtBalance.Rows.Count > 0)
            {
                this.CustGridView1.Columns[2].Visible = true;
                this.CustGridView1.DataSource = dtBalance;
                //GridPager1.RecordCount = iTotalCount;
                this.CustGridView1.DataBind();
                this.CustGridView1.Columns[2].Visible = false;

                if (this.CustGridView1.Rows.Count > 10)
                    this.pl_cgv1.Height = 325;

                base.strClientMsg += MessageHelper.GetMessage("01_0211000001_003");

            }
            else
            {
                this.CustGridView1.DataSource = null;
                this.CustGridView1.DataBind();
                //this.pl_cgv1.Height = 0;
            }
            #endregion
        }
        #endregion

        base.strHostMsg = output["HtgSuccess"].ToString();//*主機返回成功訊息
        //ViewState["HtgInfoP4_JCF6"] = htOutputP4_JCFA;
    }

    /// <summary>
    /// 功能說明:
    /// 作    者:
    /// 創建時間:
    /// 修改記錄: 2020/11/10 Ares Luke 處理白箱報告SQL Injection
    /// </summary>
    /// <param name="eBalanceAdd"></param>
    protected void InsertEntity(EntityAuto_Balance_Trans eBalanceAdd)
    {
        try
        {
            //新增一筆資料到餘額轉置資料表
            if (BRAuto_Balance_Trans.AddEntity(eBalanceAdd) == true)
            {
                string[] ids = new string[this.CustGridView1.Rows.Count];

                for (int i = 0; i < this.CustGridView1.Rows.Count; i++)
                {
                    ids[i] = this.CustGridView1.Rows[i].Cells[2].Text;
                }

                DataTable dtBalance = (DataTable)BRBalance_Trans.BindAddValue(eBalanceAdd.CardNo,
                    eBalanceAdd.PID, ids);

                if (dtBalance != null && dtBalance.Rows.Count > 0)
                {
                    this.CustGridView1.Columns[2].Visible = true;
                    CustGridView1.DataSource = dtBalance;
                    //GridPager1.RecordCount = iTotalCount;
                    CustGridView1.DataBind();
                    this.CustGridView1.Columns[2].Visible = false;

                    #region 此項輸入作業須可於[記錄查詢]功能中查詢
                    DataTable dtlog = CommonFunction.GetDataTable();
                    CommonFunction.UpdateLog("", eBalanceAdd.CardNo, "CardNo", dtlog);
                    CommonFunction.UpdateLog("", eBalanceAdd.PID, "PID", dtlog);
                    CommonFunction.UpdateLog("", eBalanceAdd.Trans_Date.ToString("yyyy/MM/dd HH:mm:ss"), "Trans_Date", dtlog);
                    CommonFunction.UpdateLog("", eBalanceAdd.Reason_Code, "Reason_Code", dtlog);
                    CommonFunction.UpdateLog("", eBalanceAdd.Memo, "Memo", dtlog);
                    CommonFunction.InsertCustomerLog(dtlog, eAgentInfo, this.ViewState["Key"].ToString(),
                        "DB", (structPageInfo)Session["PageInfo"]);
                    #endregion

                    base.strClientMsg += MessageHelper.GetMessage("01_0211000001_003");
                }
                return;
            }
        }
        catch (Exception ex)
        {
            //*寫入資料失敗
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            base.strClientMsg += ex.ToString();
            Logging.Log(ex, LogLayer.DB);

            return;
        }
    }

    public static DataTable HashTableToDataTable(Hashtable ht, int cnt, Hashtable input)
    {
        if (ht != null && ht.Count > 0)
        {
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataColumn dc1 = dt.Columns.Add("dc1", typeof(string));
            DataColumn dc2 = dt.Columns.Add("dc2", typeof(string));

            try
            {
                #region Hashtable to DataTable

                foreach (DictionaryEntry element in ht)
                {
                    DataRow dr = dt.NewRow();

                    dr["dc1"] = (string)element.Key;
                    dr["dc2"] = (string)element.Value.ToString().Trim();
                    dt.Rows.Add(dr);
                }
                #endregion

                #region create gridview data source
                if (dt != null && dt.Rows.Count > 0)
                {
                    dt2.Columns.Add("CARD_NMBR");
                    dt2.Columns.Add("SVCP_ID");
                    dt2.Columns.Add("ID_IND");
                    dt2.Columns.Add("EXPIRE_DATE");
                    dt2.Columns.Add("BLOCK_CODE");
                    dt2.Columns.Add("ReasonCode");
                    dt2.Columns.Add("KEY_NMBER");

                    for (int line = 1; line <= cnt; line++)
                    {
                        DataRow dr2 = dt2.NewRow();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (dt.Rows[i][0].ToString() == "CARD_NMBR")
                                dr2["CARD_NMBR"] = dt.Rows[i][1].ToString();

                            if (dt.Rows[i][0].ToString() == "SVCP_ID")
                                dr2["SVCP_ID"] = dt.Rows[i][1].ToString().Trim();

                            if (dt.Rows[i][0].ToString() == string.Format("ID_IND{0}", line))
                                dr2["ID_IND"] = dt.Rows[i][1].ToString();

                            if (dt.Rows[i][0].ToString() == string.Format("EXPIRE_DATE{0}", line))
                                dr2["EXPIRE_DATE"] = dt.Rows[i][1].ToString();

                            if (dt.Rows[i][0].ToString() == string.Format("BLOCK_CODE{0}", line))
                            {
                                dr2["BLOCK_CODE"] = dt.Rows[i][1].ToString();

                                if (dt.Rows[i][1].ToString() != "X")
                                    dr2["ReasonCode"] = "3";
                                else
                                    dr2["ReasonCode"] = "4";
                            }

                            if (dt.Rows[i][0].ToString() == "KEY_NMBER" + line)
                                dr2["KEY_NMBER"] = dt.Rows[i][1].ToString();
                        }
                        dt2.Rows.Add(dr2);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt2;
        }
        else
            return null;
    }

    #endregion event
}

#region function
/// 作者 占偉林
/// 創建日期：2009/10/22
/// 修改日期：2009/10/22 
/// <summary>
/// 綁定GridView數據源
/// </summary>
//private void BindGridView()
//{
//    try
//    {
//        string strMsgID = "";
//        int intTotolCount = 0;
//        DataTable dtblCustomer_Log = (DataTable)BRCustomer_Log.SearchCustomer_Log(this.ViewState["Key"].ToString(), this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotolCount, ref strMsgID);
//        if (dtblCustomer_Log == null)
//        {
//            //* 顯示端末訊息
//            base.strClientMsg += MessageHelper.GetMessage(strMsgID);
//            this.gpList.RecordCount = 0;
//            this.gvpbSearchRecord.DataSource = null;
//            this.gvpbSearchRecord.DataBind();
//        }
//        else
//        {
//            this.gpList.RecordCount = intTotolCount;
//            this.gvpbSearchRecord.DataSource = dtblCustomer_Log;
//            this.gvpbSearchRecord.DataBind();
//            //* 顯示端末訊息
//            if (intTotolCount == 0)
//            {
//                base.strClientMsg += MessageHelper.GetMessage("01_02010000_003");    
//            }
//            else
//            {
//                base.strClientMsg += MessageHelper.GetMessage("01_02010000_002");    
//            }
//        }
//    }
//    catch (Exception exp)
//    {
//        Logging.SaveLog(ELogLayer.UI, exp);
//        //* 顯示端末訊息
//        base.strClientMsg += MessageHelper.GetMessage("01_02010000_001");
//    }
//}
#endregion function
//protected void GridPager1_PageChanged(object src, PageChangedEventArgs e)
//{
//    //this.gpList.CurrentPageIndex = e.NewPageIndex;
//}
//protected void gvpbSsearchRecord_RowDataBound(object sender, GridViewRowEventArgs e)
//{
//    if (e.Row.RowType == DataControlRowType.DataRow)
//    {
//        HtmlInputRadioButton radRole = (HtmlInputRadioButton)e.Row.FindControl("radRole");
//        radRole.Checked = false;
//    }
//}

