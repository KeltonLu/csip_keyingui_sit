//******************************************************************
//*  作    者：蘇洺葳
//*  功能說明：歷史資料查詢1645(歷史資料查詢1645)
//*  創建日期：2018/02/013
//*  修改記錄：
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
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Common.Logging;
using Framework.WebControls;
using System.Drawing;
using Framework.Common.Utility;

public partial class P010213030001 : PageBase
{
    #region 變數區

    /// <summary>
    /// Session變數集合
    /// </summary>
    private static EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    /// <summary>
    /// 正卡ID
    /// </summary>
    private static string acctID;

    /// <summary>
    /// 車號
    /// </summary>
    private static string lprNO;

    /// <summary>
    /// 收件編號/信用卡申請書編號
    /// </summary>
    private static string applyNO;

    /// <summary>
    /// 鍵檔日(起)
    /// </summary>
    private static string keyInDateS;

    /// <summary>
    /// 鍵檔日(訖)
    /// </summary>
    private static string keyInDateE;

    /// <summary>
    /// 功能碼：1 => [正卡ID]查詢，2 => [車號]查詢，3 => [鍵檔日]查詢
    /// </summary>
    private static string functionCode;

    /// <summary>
    /// 上傳次數
    /// </summary>
    private int uploadCount = 0;

    /// <summary>
    /// 組成的GridView資料
    /// </summary>
    private DataTable dtP4_JCLE;

    #endregion

    #region

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 綁定GridView列頭訊息
            ShowControlsText();
            this.gpList.RecordCount = 0;
            this.gpList.Visible = false;
            this.gvpbCase.DataSource = null;
            this.gvpbCase.DataBind();
        }

        base.strHostMsg = "";
        base.strClientMsg = "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//*記錄網頁訊息
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtACCT_ID.Text;

        //20200109-修改SOC存入條件
        //log.Statement_Text = string.Format("UserId:{0}", log.Customer_Id); //查詢條件內容: 用 | 區隔,要看文件確認
        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔

        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        try
        {
            acctID = this.txtACCT_ID.Text;
            lprNO = this.txtLPR_NO.Text;
            applyNO = this.txtAPPLY_NO.Text;
            // (異動日) & (鍵檔日) 二者至少有一組有值，即不可無日期條件，區間最長為6個月
            keyInDateS = this.dpKeyInDateS.Text.Replace("/", "");
            keyInDateE = this.dpKeyInDateE.Text.Replace("/", "");
            functionCode = string.Empty;
            dtP4_JCLE = new DataTable();

            #region 獲取主機資料

            Hashtable htInputP4_JCLE = new Hashtable();
            // JCLE上行
            htInputP4_JCLE = GetUploadHtgInfo();
            // JCLE下行
            Hashtable htOutputP4_JCLE = new Hashtable();

            // 重覆查詢電文
            MainFrameInfo.GetHtgOutput(HtgType.P4_JCLE, htInputP4_JCLE, ref htOutputP4_JCLE, ref dtP4_JCLE, ref uploadCount, ref eAgentInfo);

            ViewState["dtP4_JCLE"] = dtP4_JCLE;

            #endregion

            if (htOutputP4_JCLE.Contains("HtgMsg"))
            {
                base.strClientMsg = MessageHelper.GetMessage("01_01011303_002");
                base.strHostMsg = htOutputP4_JCLE["HtgMsg"].ToString();
            }

            this.gpList.CurrentPageIndex = 0;

            this.BindGridView();

            this.gpList.Visible = true;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            // 顯示端末訊息
            base.strClientMsg = MessageHelper.GetMessage("01_01011303_002");
        }
    }

    /// <summary>
    /// 換頁
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }

    #endregion

    #region

    /// <summary>
    /// 綁定GridView列頭訊息
    /// </summary>
    private void ShowControlsText()
    {
        this.gvpbCase.Columns[0].HeaderText = "序號";
        this.gvpbCase.Columns[1].HeaderText = BaseHelper.GetShowText("01_02130300_007");
        this.gvpbCase.Columns[2].HeaderText = BaseHelper.GetShowText("01_02130300_002");
        this.gvpbCase.Columns[3].HeaderText = BaseHelper.GetShowText("01_02130300_008");
        this.gvpbCase.Columns[4].HeaderText = BaseHelper.GetShowText("01_02130300_009");
        this.gvpbCase.Columns[5].HeaderText = BaseHelper.GetShowText("01_02130300_010");
        this.gvpbCase.Columns[6].HeaderText = BaseHelper.GetShowText("01_02130300_011");
        this.gvpbCase.Columns[7].HeaderText = BaseHelper.GetShowText("01_02130300_012");
        this.gvpbCase.Columns[8].HeaderText = BaseHelper.GetShowText("01_02130300_013");
        this.gvpbCase.Columns[9].HeaderText = BaseHelper.GetShowText("01_02130300_014");
        this.gvpbCase.Columns[10].HeaderText = BaseHelper.GetShowText("01_02130300_015");
        this.gvpbCase.Columns[11].HeaderText = BaseHelper.GetShowText("01_02130300_016");
        this.gvpbCase.Columns[12].HeaderText = BaseHelper.GetShowText("01_02130300_017");
        this.gvpbCase.Columns[13].HeaderText = BaseHelper.GetShowText("01_02130300_018");
        this.gvpbCase.Columns[14].HeaderText = BaseHelper.GetShowText("01_02130300_019");
        this.gvpbCase.Columns[15].HeaderText = BaseHelper.GetShowText("01_02130300_020");
        this.gvpbCase.Columns[16].HeaderText = BaseHelper.GetShowText("01_02130300_021");
        this.gvpbCase.Columns[17].HeaderText = BaseHelper.GetShowText("01_02130300_022");
        this.gvpbCase.Columns[18].HeaderText = BaseHelper.GetShowText("01_02130300_023");
        this.gvpbCase.Columns[19].HeaderText = BaseHelper.GetShowText("01_02130300_024");
        this.gvpbCase.Columns[20].HeaderText = BaseHelper.GetShowText("01_02130300_025");
        this.gvpbCase.Columns[21].HeaderText = BaseHelper.GetShowText("01_02130300_026");
        this.gvpbCase.Columns[22].HeaderText = BaseHelper.GetShowText("01_02130300_027");
        //this.gvpbCase.Columns[0].HeaderText = BaseHelper.GetShowText("01_02130300_007");
        //this.gvpbCase.Columns[1].HeaderText = BaseHelper.GetShowText("01_02130300_002");
        //this.gvpbCase.Columns[2].HeaderText = BaseHelper.GetShowText("01_02130300_008");
        //this.gvpbCase.Columns[3].HeaderText = BaseHelper.GetShowText("01_02130300_009");
        //this.gvpbCase.Columns[4].HeaderText = BaseHelper.GetShowText("01_02130300_010");
        //this.gvpbCase.Columns[5].HeaderText = BaseHelper.GetShowText("01_02130300_011");
        //this.gvpbCase.Columns[6].HeaderText = BaseHelper.GetShowText("01_02130300_012");
        //this.gvpbCase.Columns[7].HeaderText = BaseHelper.GetShowText("01_02130300_013");
        //this.gvpbCase.Columns[8].HeaderText = BaseHelper.GetShowText("01_02130300_014");
        //this.gvpbCase.Columns[9].HeaderText = BaseHelper.GetShowText("01_02130300_015");
        //this.gvpbCase.Columns[10].HeaderText = BaseHelper.GetShowText("01_02130300_016");
        //this.gvpbCase.Columns[11].HeaderText = BaseHelper.GetShowText("01_02130300_017");
        //this.gvpbCase.Columns[12].HeaderText = BaseHelper.GetShowText("01_02130300_018");
        //this.gvpbCase.Columns[13].HeaderText = BaseHelper.GetShowText("01_02130300_019");
        //this.gvpbCase.Columns[14].HeaderText = BaseHelper.GetShowText("01_02130300_020");
        //this.gvpbCase.Columns[15].HeaderText = BaseHelper.GetShowText("01_02130300_021");
        //this.gvpbCase.Columns[16].HeaderText = BaseHelper.GetShowText("01_02130300_022");
        //this.gvpbCase.Columns[17].HeaderText = BaseHelper.GetShowText("01_02130300_023");
        //this.gvpbCase.Columns[18].HeaderText = BaseHelper.GetShowText("01_02130300_024");
        //this.gvpbCase.Columns[19].HeaderText = BaseHelper.GetShowText("01_02130300_025");
        //this.gvpbCase.Columns[20].HeaderText = BaseHelper.GetShowText("01_02130300_026");
        //this.gvpbCase.Columns[21].HeaderText = BaseHelper.GetShowText("01_02130300_027");

        // 設置每頁顯示記錄最大條數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbCase.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }

    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    private void BindGridView()
    {
        try
        {
            int pageIndex = this.gpList.CurrentPageIndex;
            DataTable dt = (DataTable)ViewState["dtP4_JCLE"];
            int totalCount = dt.Rows.Count;

            if (pageIndex > 1)
            {
                DataTable newDT = new DataTable();
                MainFrameInfo.AddHtgColumnToDataTable(HtgType.P4_JCLE, ref newDT);
                int endCount = pageIndex * 10;
                int startCount = endCount - 9;

                string expression = string.Format("COUNT >= {0} and COUNT <= {1}", startCount, endCount);
                DataRow[] foundRows = dt.Select(expression);

                foreach (DataRow row in foundRows)
                {
                    newDT.ImportRow(row);
                }

                dt = newDT;
            }

            if (dt == null || dt.Rows.Count == 0)
            {
                // 顯示端末訊息
                this.gpList.RecordCount = 0;
                this.gvpbCase.DataSource = dt;
                this.gvpbCase.DataBind();
            }
            else
            {
                this.gpList.RecordCount = totalCount;
                this.gvpbCase.DataSource = dt;
                this.gvpbCase.DataBind();
                // 顯示端末訊息
                base.strClientMsg = MessageHelper.GetMessage("01_01011303_001");
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            // 顯示端末訊息
            base.strClientMsg = MessageHelper.GetMessage("01_01011303_002");
        }
    }

    /// <summary>
    /// 得到上傳主機信息
    /// </summary>
    /// <returns></returns>
    private static Hashtable GetUploadHtgInfo()
    {
        // 上行
        Hashtable htInput = new Hashtable();
        htInput.Add("TRAN_ID", "JCLE");// 電文交易名稱
        htInput.Add("PROGRAM_ID", "JCGU852");// 電文程式名稱
        htInput.Add("USER_ID", "CSIP");// 電文使用者ID
        htInput.Add("MESSAGE_TYPE", "");// 回覆碼
        htInput.Add("FUNCTION_CODE", "1");// 功能碼
        htInput.Add("LINE_CNT", "0000");// DETAIL起始行數
        htInput.Add("ACCTID", acctID);// 正卡ID
        htInput.Add("LPR_NO", lprNO);// 車號
        htInput.Add("UPDATE_DATE_S", "");// 異動日(起)
        htInput.Add("UPDATE_DATE_E", "");// 異動日(迄)
        htInput.Add("KEYIN_DATE_S", keyInDateS);// 鍵檔日(起)
        htInput.Add("KEYIN_DATE_E", keyInDateE);// 鍵檔日(迄)
        htInput.Add("APPLY_NO", applyNO);// 收件編號/信用卡申請書編號
        htInput.Add("FILLER", "");

        //// 下行
        //htInput.Add("ACTION", "");// 異動代碼 1 => 新增，2 => 修改，3 => 刪除
        //htInput.Add("TYPE", "");// 代扣種類 01 => 儲值，02 => 臨停，03 => 月租
        //htInput.Add("ACCT_ID", "");// 正卡人ID
        ////htInput.Add("LPR_NO", "");// 車號
        //htInput.Add("CUSTID", "");// 車主ID
        //htInput.Add("STATUS", "");// 狀態碼 1 => 申請，2 => 終止
        //htInput.Add("NO", "");// 停車場代碼 代扣類別= 03(月租)時提供
        //htInput.Add("SERVICE_A_DATE", "");// 申請日
        //htInput.Add("SERVICE_D_DATE", "");// 終止日
        //htInput.Add("RESULT_A_CODE", "");// 申請回覆碼
        //htInput.Add("RESULT_D_CODE", "");// 終止回覆碼
        ////htInput.Add("APPLY_NO", "");// 收件編號/信用卡申請書編號 1.若為13碼則為[收件編號]，2.若為16碼則為[信用卡申請書編號]
        ////htInput.Add("KEYIN_DATE", "");// 鍵檔日
        //htInput.Add("UPDATE_USER", "");// 維護員 可能為異動人員OR異動程式編號
        //htInput.Add("UPDATE_DATE", "");// 維護日
        //htInput.Add("UPDATE_TIME", "");// 維護時間
        //htInput.Add("SEND_DATE", "");// 媒體傳送日
        //htInput.Add("SPC", "");// 推廣通路代號
        //htInput.Add("SPC_ID", "");// 推廣員編
        //htInput.Add("INTRODUCER_CARD_ID", "");// 種子
        //htInput.Add("EFF_DATE", "");// 生效日
        //htInput.Add("SOURCE", "");// 申請異動發動來源 1 => 空白: 中信，2 => Y: 遠通
        //htInput.Add("CHANNEL", "");// 進件通路別 01 => EXMS，02 => CSIP，03 => CSI0，4 => 小版，05 => 申請書，06 => 遠通
        //htInput.Add("VENDOR_NAME", "");// 停車場名稱 代扣類別= 03(月租)時提供

        return htInput;
    }

    #endregion
}
