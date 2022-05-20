//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：報表-作業量比對報表
//*  創建日期：2009/10/28
//*  修改記錄：2021/01/11_Ares_Stanley-新增NPOI
//*<author>            <time>            <TaskID>                <desc>
//* Ge.Song         2010/05/18          20090023                將現有4個子系統修改爲讀取Common中取得屬性的方法
//* Grezz           20180502 (U)                                [M_PROPERTY_CODE]新增PROPERTY_KEY條件，查詢eTag項目
//* Ares Luke       2020/11/19          20200031-CSIP EOS       調整取web.config加解密參數
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
using System.Text;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BusinessRules_new;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using Framework.Common.Logging;
using Framework.Common.Utility;
using System.Collections.Generic;
using CSIPKeyInGUI.BusinessRules_new;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.HSSF.EventUserModel.DummyRecord;
using NPOI.XSSF.UserModel.Charts;

public partial class P010301000001 : PageBase
{
    #region event

    /// 作者 占偉林
    /// 創建日期：2009/10/28
    /// 修改日期：2009/10/28; 2021/01/11_Ares_Stanley-增加PropertyName儲存
    /// <summary>
    /// 畫面裝載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            //* 【區間】起
            this.dtpSearchStart.Text = DateTime.Now.ToString("yyyy/MM/dd");
            //* 【區間】迄
            this.dtpSearchEnd.Text = DateTime.Now.ToString("yyyy/MM/dd");
            //* 綁定業務項目DropDownList
            BindDropDownList();
            //* 設置光標
            this.dtpSearchStart.Focus();
        }
        this.Session["PropertyName"] = this.dropProperty.SelectedItem.Text;
        base.strHostMsg += "";
        this.sbRegScript.Append("loadSetFocus();");
    }
    
    ///// 作者 占偉林
    ///// 創建日期：2009/11/06
    ///// 修改日期：2009/11/06; 2021/01/11_Ares_Stanley-變更報表產出為NPOI
    /// <summary>
    /// 點選畫面【列印】按鈕時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        StringBuilder sbRegScriptF = new StringBuilder("");
        string strProperty = this.dropProperty.SelectedValue;
        if (strProperty == "01030501" || strProperty == "01030502")
        {
            Dictionary<string, string> dirValues = new Dictionary<string, string>();
            //* 鍵檔起迄日
            dirValues.Add("@DATE_BEGIN", this.dtpSearchStart.Text.Trim().Replace("/", ""));
            dirValues.Add("@DATE_END", this.dtpSearchEnd.Text.Trim().Replace("/", ""));

            //* 匯出到Excel.
            OutputExcel(dirValues, strProperty);
        }
        else
        {
            OutputOtherExcel(strProperty);
        }
        
        
        //* 顯示報表
        this.Session["PropertyName"] = this.dropProperty.SelectedItem.Text;
        this.sbRegScript.Append(sbRegScriptF.ToString());
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.gvbpWorkLoadCompare.Visible = true;
        this.gpList.Visible = true;
        // 綁定GridView表頭訊息
        ShowControlsText();

        BindGridView();
    }
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }
    #endregion event
    #region method
    /// <summary>
    /// 建立日期2021/01/20_Ares_Stanley
    /// 修改紀錄:2021/01/21_Ares_Stanley-調整資料順序
    /// 綁定查詢查詢結果表頭
    /// </summary>
    private void ShowControlsText()
    {
        this.gvbpWorkLoadCompare.Columns.Clear();
        #region common field
        // 序號
        BoundField S_NO = new BoundField();
        S_NO.HeaderText = BaseHelper.GetShowText("01_03010000_006");
        S_NO.DataField = "S_NO";
        // 作業日
        BoundField mod_date = new BoundField();
        mod_date.HeaderText = BaseHelper.GetShowText("01_03010000_007");
        mod_date.DataField = "mod_date";
        // 比對值(Key值)
        BoundField query_key = new BoundField();
        query_key.HeaderText = BaseHelper.GetShowText("01_03010000_008");
        query_key.DataField = "query_key";
        // 鍵一同仁
        BoundField user_1key = new BoundField();
        user_1key.HeaderText = BaseHelper.GetShowText("01_03010000_009");
        user_1key.DataField = "user_1key";
        // 鍵二同仁
        BoundField user_2key = new BoundField();
        user_2key.HeaderText = BaseHelper.GetShowText("01_03010000_010");
        user_2key.DataField = "user_2key";

        #endregion
        #region eTag(001) field
        // 收件編號
        BoundField Receive_Number = new BoundField();
        Receive_Number.HeaderText = BaseHelper.GetShowText("01_03010000_011");
        Receive_Number.DataField = "receive_number";
        // eTag儲值
        BoundField eTag= new BoundField();
        eTag.HeaderText = BaseHelper.GetShowText("01_03010000_012");
        eTag.DataField = "1641";
        // 月租停車
        BoundField Monthly_Rent = new BoundField();
        Monthly_Rent.HeaderText = BaseHelper.GetShowText("01_03010000_013");
        Monthly_Rent.DataField = "1643";
        // 臨時停車
        BoundField Temporary_Parking = new BoundField();
        Temporary_Parking.HeaderText = BaseHelper.GetShowText("01_03010000_014");
        Temporary_Parking.DataField = "1642";
        #endregion
        #region 他行自扣申請書(卡人資料異動) || 中信及郵局自扣(卡人資料異動) || 他行自扣訊息 / 更正單(卡人資料異動) (01010600 || 01010800 || 01011000) field
        // 異動後自扣銀行帳號
        BoundField Acc_No = new BoundField();
        Acc_No.HeaderText = BaseHelper.GetShowText("01_03010000_015");
        Acc_No.DataField = "acc_no";
        // 自扣設定
        BoundField Auto_Pay_Setting = new BoundField();
        Auto_Pay_Setting.HeaderText = BaseHelper.GetShowText("01_03010000_016");
        Auto_Pay_Setting.DataField = "auto_pay_setting";
        // 電子賬單設定
        BoundField E_Bill_Setting = new BoundField();
        E_Bill_Setting.HeaderText = BaseHelper.GetShowText("01_03010000_017");
        E_Bill_Setting.DataField = "E_Bill_Setting";
        // 手機及E-MAIL設定
        BoundField cellP_Email_Setting = new BoundField();
        cellP_Email_Setting.HeaderText = BaseHelper.GetShowText("01_03010000_018");
        cellP_Email_Setting.DataField = "cellP_Email_Setting";
        // 週期件註記
        BoundField OutputByTXT_Setting = new BoundField();
        OutputByTXT_Setting.HeaderText = BaseHelper.GetShowText("01_03010000_019");
        OutputByTXT_Setting.DataField = "OutputByTXT_Setting";
        #endregion
        #region 特店資料異動(請款加批)(01030100) field
        // 商店代號
        BoundField Shop_ID = new BoundField();
        Shop_ID.HeaderText = BaseHelper.GetShowText("01_03010000_020");
        Shop_ID.DataField = "shop_id";
        // 請款加批前YorN
        BoundField Before = new BoundField();
        Before.HeaderText = BaseHelper.GetShowText("01_03010000_021");
        Before.DataField = "before";
        // 請款加批后YorN
        BoundField After = new BoundField();
        After.HeaderText = BaseHelper.GetShowText("01_03010000_022");
        After.DataField = "after";
        // 登記名稱
        BoundField Record_Name = new BoundField();
        Record_Name.HeaderText = BaseHelper.GetShowText("01_03010000_023");
        Record_Name.DataField = "record_name";
        // 營業名稱
        BoundField Business_Name = new BoundField();
        Business_Name.HeaderText = BaseHelper.GetShowText("01_03010000_024");
        Business_Name.DataField = "business_name";
        // 賬單列示名稱
        BoundField Merchant_Name = new BoundField();
        Merchant_Name.HeaderText = BaseHelper.GetShowText("01_03010000_025");
        Merchant_Name.DataField = "merchant_name";
        // 鍵檔人員
        BoundField User_Name = new BoundField();
        User_Name.HeaderText = BaseHelper.GetShowText("01_03010000_026");
        User_Name.DataField = "user_name";
        #endregion
        #region 自扣終止(01011200) field
        // 推廣成績異動
        BoundField Auto_Pay_Setting1 = new BoundField();
        Auto_Pay_Setting1.HeaderText = BaseHelper.GetShowText("01_03010000_028");
        Auto_Pay_Setting1.DataField = "auto_pay_setting1";
        // 鍵檔同仁
        BoundField User_Key = new BoundField();
        User_Key.HeaderText = BaseHelper.GetShowText("01_03010000_030");
        User_Key.DataField = "user_key";
        #endregion
        #region 毀補轉一卡通(01021500) field
        // 協銷公司
        BoundField Co_Marketing_Com = new BoundField();
        Co_Marketing_Com.HeaderText = BaseHelper.GetShowText("01_03010000_031");
        Co_Marketing_Com.DataField = "Co_Marketing_Com";
        // 卡號
        BoundField Card_No = new BoundField();
        Card_No.HeaderText = BaseHelper.GetShowText("01_03010000_032");
        Card_No.DataField = "Card_No";
        // 製卡中文姓名
        BoundField Chinese_Name = new BoundField();
        Chinese_Name.HeaderText = BaseHelper.GetShowText("01_03010000_033");
        Chinese_Name.DataField = "Chinese_Name";
        // 有無異動製卡英文姓名(Y/N)
        BoundField Change_Eng_Name = new BoundField();
        Change_Eng_Name.HeaderText = BaseHelper.GetShowText("01_03010000_034");
        Change_Eng_Name.DataField = "Change_Eng_Name";
        // 異動後製卡英文名
        BoundField Eng_Name = new BoundField();
        Eng_Name.HeaderText = BaseHelper.GetShowText("01_03010000_035");
        Eng_Name.DataField = "Eng_Name";
        // 取卡方式
        BoundField GetCard_code = new BoundField();
        GetCard_code.HeaderText = BaseHelper.GetShowText("01_03010000_036");
        GetCard_code.DataField = "GetCard_code";
        // 會員編號
        BoundField Member_No = new BoundField();
        Member_No.HeaderText = BaseHelper.GetShowText("01_03010000_037");
        Member_No.DataField = "Member_No";
        // 協銷註記
        BoundField Co_Marketing = new BoundField();
        Co_Marketing.HeaderText = BaseHelper.GetShowText("01_03010000_038");
        Co_Marketing.DataField = "Co_Marketing";
        // 鍵一記録(員編 / 鍵檔時間)
        BoundField KeyIn_Flag1 = new BoundField();
        KeyIn_Flag1.HeaderText = BaseHelper.GetShowText("01_03010000_039");
        KeyIn_Flag1.DataField = "KeyIn_Flag1";
        // 鍵二記録(員編 / 鍵檔時間)
        BoundField KeyIn_Flag2 = new BoundField();
        KeyIn_Flag2.HeaderText = BaseHelper.GetShowText("01_03010000_040");
        KeyIn_Flag2.DataField = "KeyIn_Flag2";
        #endregion
        #region 特店資料異動(依統編)鍵檔同仁明細(01030501) field
        // 序號
        BoundField SEQ = new BoundField();
        SEQ.HeaderText = BaseHelper.GetShowText("01_03010000_041");
        SEQ.DataField = "SEQ";
        // 統一編號
        BoundField CORP_NO = new BoundField();
        CORP_NO.HeaderText = BaseHelper.GetShowText("01_03010000_042");
        CORP_NO.DataField = "CORP_NO";
        // 鍵一同仁
        BoundField USER1 = new BoundField();
        USER1.HeaderText = BaseHelper.GetShowText("01_03010000_043");
        USER1.DataField = "USER1";
        // 鍵一同仁
        BoundField USER2 = new BoundField();
        USER2.HeaderText = BaseHelper.GetShowText("01_03010000_044");
        USER2.DataField = "USER2";
        // 放行同仁
        BoundField USER3 = new BoundField();
        USER3.HeaderText = BaseHelper.GetShowText("01_03010000_045");
        USER3.DataField = "USER3";
        #endregion
        #region 特店資料異動(依統編)主機異動明細(01030502)
        // 作業日(BATCH_DATE)
        BoundField BATCH_DATE = new BoundField();
        BATCH_DATE.HeaderText = BaseHelper.GetShowText("01_03010000_047");
        BATCH_DATE.DataField = "BATCH_DATE";
        // 統編序號
        BoundField CORP_SEQ = new BoundField();
        CORP_SEQ.HeaderText = BaseHelper.GetShowText("01_03010000_048");
        CORP_SEQ.DataField = "CORP_SEQ";
        // 商店代號(MERCH_NO)
        BoundField MERCH_NO = new BoundField();
        MERCH_NO.HeaderText = BaseHelper.GetShowText("01_03010000_049");
        MERCH_NO.DataField = "MERCH_NO";
        // 狀態碼
        BoundField STATUS_CODE = new BoundField();
        STATUS_CODE.HeaderText = BaseHelper.GetShowText("01_03010000_050");
        STATUS_CODE.DataField = "STATUS_CODE";
        // 解約原因
        BoundField TERMINATE_CODE = new BoundField();
        TERMINATE_CODE.HeaderText = BaseHelper.GetShowText("01_03010000_051");
        TERMINATE_CODE.DataField = "TERMINATE_CODE";
        // 解約日期
        BoundField TERMINATE_DATE = new BoundField();
        TERMINATE_DATE.HeaderText = BaseHelper.GetShowText("01_03010000_052");
        TERMINATE_DATE.DataField = "TERMINATE_DATE";
        // 更新筆數
        BoundField UPDATE_CNT = new BoundField();
        UPDATE_CNT.HeaderText = BaseHelper.GetShowText("01_03010000_053");
        UPDATE_CNT.DataField = "UPDATE_CNT";
        #endregion
        //20211118_Ares_Jack_特店資料異動(解約)STATUE
        BoundField after = new BoundField();
        after.HeaderText = BaseHelper.GetShowText("01_03010000_054");
        after.DataField = "after";

        switch (this.dropProperty.SelectedItem.Value)
        {
            case "001": // eTag
                this.gvbpWorkLoadCompare.Columns.Add(S_NO); // 序號
                this.gvbpWorkLoadCompare.Columns.Add(mod_date); // 作業日
                this.gvbpWorkLoadCompare.Columns.Add(Receive_Number); // 收件編號
                this.gvbpWorkLoadCompare.Columns.Add(query_key); // 比對值
                this.gvbpWorkLoadCompare.Columns.Add(eTag); // eTag儲值
                this.gvbpWorkLoadCompare.Columns.Add(Monthly_Rent); // 月租停車
                this.gvbpWorkLoadCompare.Columns.Add(Temporary_Parking); // 臨時停車
                this.gvbpWorkLoadCompare.Columns.Add(user_1key); // 鍵一同仁
                this.gvbpWorkLoadCompare.Columns.Add(user_2key); // 鍵二同仁
                break;
            case "01010600": //他行自扣申請書(卡人資料異動)
            case "01010800": //中信及郵局自扣(卡人資料異動)
            case "01011000": //他行自扣訊息/更正單(卡人資料異動)
                // 他行自扣申請書(卡人資料異動) || 中信及郵局自扣(卡人資料異動) || 他行自扣訊息 / 更正單(卡人資料異動)
                this.gvbpWorkLoadCompare.Columns.Add(S_NO);
                this.gvbpWorkLoadCompare.Columns.Add(mod_date);
                this.gvbpWorkLoadCompare.Columns.Add(Receive_Number);
                this.gvbpWorkLoadCompare.Columns.Add(query_key);
                this.gvbpWorkLoadCompare.Columns.Add(Acc_No);
                this.gvbpWorkLoadCompare.Columns.Add(Auto_Pay_Setting);
                this.gvbpWorkLoadCompare.Columns.Add(E_Bill_Setting);
                this.gvbpWorkLoadCompare.Columns.Add(cellP_Email_Setting);
                this.gvbpWorkLoadCompare.Columns.Add(OutputByTXT_Setting);
                this.gvbpWorkLoadCompare.Columns.Add(user_1key);
                this.gvbpWorkLoadCompare.Columns.Add(user_2key);
                break;
            case "01030100": //特店資料異動(請款加批) 
                this.gvbpWorkLoadCompare.Columns.Add(S_NO);
                this.gvbpWorkLoadCompare.Columns.Add(mod_date);
                this.gvbpWorkLoadCompare.Columns.Add(Shop_ID);
                this.gvbpWorkLoadCompare.Columns.Add(Before);
                this.gvbpWorkLoadCompare.Columns.Add(After);
                this.gvbpWorkLoadCompare.Columns.Add(Record_Name);
                this.gvbpWorkLoadCompare.Columns.Add(Business_Name);
                this.gvbpWorkLoadCompare.Columns.Add(Merchant_Name);
                this.gvbpWorkLoadCompare.Columns.Add(User_Name);
                break;
            case "01041100": //特店資料新增(特店延伸性/請款加批)
                this.gvbpWorkLoadCompare.Columns.Add(S_NO);
                this.gvbpWorkLoadCompare.Columns.Add(mod_date);
                this.gvbpWorkLoadCompare.Columns.Add(Shop_ID);
                After.HeaderText = BaseHelper.GetShowText("01_03010000_027");
                this.gvbpWorkLoadCompare.Columns.Add(After);
                this.gvbpWorkLoadCompare.Columns.Add(Record_Name);
                this.gvbpWorkLoadCompare.Columns.Add(Business_Name);
                this.gvbpWorkLoadCompare.Columns.Add(Merchant_Name);
                this.gvbpWorkLoadCompare.Columns.Add(User_Name);
                break;
            case "01011200": //自扣終止
                this.gvbpWorkLoadCompare.Columns.Add(S_NO);
                this.gvbpWorkLoadCompare.Columns.Add(mod_date);
                this.gvbpWorkLoadCompare.Columns.Add(Receive_Number);
                this.gvbpWorkLoadCompare.Columns.Add(query_key);
                this.gvbpWorkLoadCompare.Columns.Add(Acc_No);
                Auto_Pay_Setting.HeaderText= BaseHelper.GetShowText("01_03010000_029");
                this.gvbpWorkLoadCompare.Columns.Add(Auto_Pay_Setting);
                this.gvbpWorkLoadCompare.Columns.Add(Auto_Pay_Setting1);
                this.gvbpWorkLoadCompare.Columns.Add(User_Key);
                break;
            case "01021500": //毀補轉一卡通
                this.gvbpWorkLoadCompare.Columns.Add(S_NO);
                this.gvbpWorkLoadCompare.Columns.Add(Co_Marketing_Com);
                this.gvbpWorkLoadCompare.Columns.Add(Card_No);
                this.gvbpWorkLoadCompare.Columns.Add(Chinese_Name);
                this.gvbpWorkLoadCompare.Columns.Add(Change_Eng_Name);
                this.gvbpWorkLoadCompare.Columns.Add(Eng_Name);
                this.gvbpWorkLoadCompare.Columns.Add(GetCard_code);
                this.gvbpWorkLoadCompare.Columns.Add(Member_No);
                this.gvbpWorkLoadCompare.Columns.Add(Co_Marketing);
                this.gvbpWorkLoadCompare.Columns.Add(KeyIn_Flag1);
                this.gvbpWorkLoadCompare.Columns.Add(KeyIn_Flag2);
                break;
            case "01030501": //特店資料異動(依統編)鍵檔同仁明細
                this.gvbpWorkLoadCompare.Columns.Add(SEQ);
                this.gvbpWorkLoadCompare.Columns.Add(CORP_NO);
                this.gvbpWorkLoadCompare.Columns.Add(USER1);
                this.gvbpWorkLoadCompare.Columns.Add(USER2);
                this.gvbpWorkLoadCompare.Columns.Add(USER3);
                mod_date.HeaderText = BaseHelper.GetShowText("01_03010000_046");
                this.gvbpWorkLoadCompare.Columns.Add(mod_date);
                break;
            case "01030502": //特店資料異動(依統編)主機異動明細
                this.gvbpWorkLoadCompare.Columns.Add(BATCH_DATE);
                this.gvbpWorkLoadCompare.Columns.Add(CORP_NO);
                this.gvbpWorkLoadCompare.Columns.Add(CORP_SEQ);
                this.gvbpWorkLoadCompare.Columns.Add(MERCH_NO);
                this.gvbpWorkLoadCompare.Columns.Add(STATUS_CODE);
                this.gvbpWorkLoadCompare.Columns.Add(TERMINATE_CODE);
                this.gvbpWorkLoadCompare.Columns.Add(TERMINATE_DATE);
                this.gvbpWorkLoadCompare.Columns.Add(UPDATE_CNT);
                break;
            case "01030203": //20211118_Ares_Jack_特店資料異動(解約)
                this.gvbpWorkLoadCompare.Columns.Add(S_NO); // 序號
                this.gvbpWorkLoadCompare.Columns.Add(mod_date); // 作業日
                this.gvbpWorkLoadCompare.Columns.Add(query_key); // 比對值
                this.gvbpWorkLoadCompare.Columns.Add(after);//解約狀態
                this.gvbpWorkLoadCompare.Columns.Add(user_1key); // 鍵一同仁
                this.gvbpWorkLoadCompare.Columns.Add(user_2key); // 鍵二同仁
                break;
            default:
                this.gvbpWorkLoadCompare.Columns.Add(S_NO);
                this.gvbpWorkLoadCompare.Columns.Add(mod_date);
                this.gvbpWorkLoadCompare.Columns.Add(query_key);
                this.gvbpWorkLoadCompare.Columns.Add(user_1key);
                this.gvbpWorkLoadCompare.Columns.Add(user_2key);
                break;
        }
        for (int i = 0; i < this.gvbpWorkLoadCompare.Columns.Count; i++)
        {
            this.gvbpWorkLoadCompare.Columns[i].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            this.gvbpWorkLoadCompare.Columns[i].ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            this.gvbpWorkLoadCompare.Columns[i].HeaderStyle.CssClass = "whiteSpaceNormal";
            this.gvbpWorkLoadCompare.Columns[i].ItemStyle.CssClass = "whiteSpaceNormal";
        }
        if (this.dropProperty.SelectedItem.Value == "05010001")
        {
            this.gvbpWorkLoadCompare.Columns[2].ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        }

        //* 設置每頁顯示記錄最大條數
            this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvbpWorkLoadCompare.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }
    private void BindGridView()
    {
        int intTotalCount = 0;
        Int64 lngTotal = 0;
        string strMsgID = "";
        string strBuildDate = string.Empty;
        Dictionary<string, string> dirValues = new Dictionary<string, string>();
        //* 鍵檔起迄日
        dirValues.Add("@DATE_BEGIN", this.dtpSearchStart.Text.Trim().Replace("/", ""));
        dirValues.Add("@DATE_END", this.dtpSearchEnd.Text.Trim().Replace("/", ""));
        if (this.dropProperty.SelectedItem.Value == "01030501" || this.dropProperty.SelectedItem.Value == "01030502")
        {
            gvbpWorkLoadCompare.DataSource = CSIPKeyInGUI.BusinessRules_new.BRShopChange_LOG.getData_ChangeLog_Search(dirValues, this.dropProperty.SelectedValue, ref strBuildDate, gpList.CurrentPageIndex, gpList.PageSize, ref intTotalCount);
        }
        else
        {
            gvbpWorkLoadCompare.DataSource = CSIPKeyInGUI.BusinessRules_new.BRCompareRpt.SearchRPTData_Search(this.dropProperty.SelectedValue, this.dtpSearchStart.Text.Trim(), this.dtpSearchEnd.Text.Trim(), ref lngTotal, ref strMsgID, gpList.CurrentPageIndex, gpList.PageSize, ref intTotalCount);
        }
        
        gpList.RecordCount = intTotalCount;
        gvbpWorkLoadCompare.DataBind();
    }
    #endregion method
    #region function
    /// 作者 占偉林
    /// 創建日期：2009/10/28
    /// 修改日期：2009/10/28 
    /// <summary>
    /// 綁定業務項目DropDownList控件
    /// </summary>
    private void BindDropDownList()
    {
        try
        {
            //* REQ-20090023 將現有4個子系統修改爲讀取Common中取得屬性的方法 Add by Ge.Song 2010/05/18 Start
            //EntitySet<EntityM_PROPERTY_CODE> eProperty_CodeSet = (EntitySet<EntityM_PROPERTY_CODE>)BRCompareRpt.SelectEntitySet();
            //if (eProperty_CodeSet == null)
            //{
            //    //* 顯示端末訊息
            //    base.strClientMsg += MessageHelper.GetMessage("01_03010000_001");
            //}
            //else
            //{
            //    //* 將取得的【業務項目】訊息顯示到業務項目DropDownList控件
            //    this.dropProperty.Items.Clear();
            //    foreach (EntityM_PROPERTY_CODE ePropertyCode in eProperty_CodeSet)
            //    {
            //        this.dropProperty.Items.Add(new ListItem(ePropertyCode.PROPERTY_NAME, ePropertyCode.PROPERTY_CODE));
            //    }
            //}

            //DataTable dtblProperty = new DataTable();
            //if (!BaseHelper.GetCommonProperty("01", "11", ref dtblProperty))
            //{
            //    //* 顯示端末訊息
            //    base.strClientMsg += MessageHelper.GetMessage("01_03010000_001");
            //}
            //else
            //{
            //    //* 將取得的【業務項目】訊息顯示到業務項目DropDownList控件
            //    this.dropProperty.Items.Clear();
            //    this.dropProperty.DataSource = dtblProperty;
            //    this.dropProperty.DataTextField = EntityM_PROPERTY_CODE.M_PROPERTY_NAME;
            //    this.dropProperty.DataValueField = EntityM_PROPERTY_CODE.M_PROPERTY;
            //    this.dropProperty.DataBind();
            //}
            //* REQ-20090023 將現有4個子系統修改爲讀取Common中取得屬性的方法 Add by Ge.Song 2010/05/18 Start

            #region [M_PROPERTY_CODE]新增PROPERTY_KEY條件，查詢ETAG事業項目

            // [M_PROPERTY_CODE]新增PROPERTY_KEY條件，查詢eTag項目
            DataTable dtblProperty = new DataTable();
            string[] arr = { "11", "40" };
            if (!BRM_PROPERTY_CODE.GetEnableProperty("01", arr, ref dtblProperty))
            {
                // 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("01_03010000_001");
            }
            else
            {
                // 將取得的【業務項目】訊息顯示到業務項目DropDownList控件
                this.dropProperty.Items.Clear();
                this.dropProperty.DataSource = dtblProperty;
                this.dropProperty.DataTextField = EntityM_PROPERTY_CODE.M_PROPERTY_NAME;
                this.dropProperty.DataValueField = EntityM_PROPERTY_CODE.M_PROPERTY;
                this.dropProperty.DataBind();
            }

            #endregion
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_03010000_001");
        }
    }

    /// <summary>
    /// 將查詢結果匯出到Excel文檔。

    /// </summary>
    protected void OutputExcel(Dictionary<string, string> dirValues, string strProperty)
    {
        try
        {
            string _ExcelTemplate = string.Empty;
            string _FileName = string.Empty;
            if (strProperty.Trim().Equals("01030501"))
            {
                _ExcelTemplate = "RptChangeLlog.xls";
                _FileName = "特店資料異動(依統編)鍵檔同仁明細";
            }
            else
            {
                _ExcelTemplate = "BatchMainFrameRtnInfo.xls";
                _FileName = "特店資料異動(依統編)主機異動明細";
            }
            string strMsgID = "";
            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            //* 服務器端，生成Excel文檔
            if (!BRShopChange_LOG.CreateExcelFile(dirValues,
                            ((EntityAGENT_INFO)this.Session["Agent"]).agent_name,
                            _ExcelTemplate,
                            strProperty,
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
            // string urlString = @"ClientMsgShow('" + MessageHelper.GetMessage("01_01030400_003") + "');";
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("01_01030400_003") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += MessageHelper.GetMessage("01_01030400_004");
        }
    }
    /// <summary>
    /// NPOI Excel 報表產出
    /// 創建日期:2021/01/11_Ares_Stanley-新增報表NPOI產出; 2021/01/21_Ares_Stanley-調整報表格式
    /// </summary>
    /// <param name="dirValues"></param>
    /// <param name="strProperty"></param>
    protected void OutputOtherExcel(string strProperty)
    {
        try
        {
            string _ExcelTemplate = string.Empty;
            string _FileName = string.Empty;
            string strSearchStart = "";
            string strSearchEnd = "";
            StringBuilder sbRegScriptF = new StringBuilder("");
            string propertyName = this.Session["PropertyName"].ToString().Trim().Replace("/", ""); // 無底線
            //string propertyName = this.Session["PropertyName"].ToString(); //有底線
            string strMsgID = "";
            Int64 lngTotal = 0;

            switch (strProperty)
            {
                case "001":
                    _ExcelTemplate = "rptUtilities.xls";
                    _FileName = "ETAG業務";
                    break;

                case "01010600":
                    _ExcelTemplate = "rptcompareAutoPay.xls";
                    _FileName = "他行自扣申請書(卡人資料異動)";
                    break;

                case "01010800":
                    _ExcelTemplate = "rptcompareAutoPay.xls";
                    _FileName = "中信及郵局自扣(卡人資料異動)";
                    break;

                case "01011000":
                    _ExcelTemplate = "rptcompareAutoPay.xls";
                    _FileName = "他行自扣訊息/更正單(卡人資料異動)";
                    break;

                case "01030100":
                    _ExcelTemplate = "rptReqApproU.xls";
                    _FileName = "特店資料異動(請款加批)";
                    break;

                case "01041100":
                    _ExcelTemplate = "rptReqApproI.xls";
                    _FileName = "特店資料新增(特店延伸性/請款加批)";
                    break;

                case "01011200":
                    _ExcelTemplate = "rptcompareAutoPayPopul.xls";
                    _FileName = "自扣終止（卡人資料異動）";
                    break;

                case "05010001":
                    _ExcelTemplate = "rptcompare.xls";
                    _FileName = "特店AML資料";
                    break;

                case "01021500":
                    _ExcelTemplate = "rptcompareToiPass.xls";
                    _FileName = "毀補轉一卡通";
                    break;
                //20211122_Ares_Jack_特店資料異動(解約)
                case "01030203":
                    _ExcelTemplate = "rptcompare_Terminate_the_contract.xls";
                    _FileName = "特店資料異動(解約)";
                    break;
                default:
                    _ExcelTemplate = "rptcompare.xls";
                    _FileName = propertyName;
                    break;
            }
            
            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());

            // 檢查目錄，並刪除以前的文檔資料
            CSIPKeyInGUI.BusinessRules_new.BRExcel_File.CheckDirectory(ref strServerPathFile);

            //* 區間起日期沒有輸入
            if (!string.IsNullOrEmpty(this.dtpSearchStart.Text.Trim().Replace("/", "")))
            {
                //* 區間起
                strSearchStart = this.dtpSearchStart.Text.Trim().Replace("/", "");
            }
            else
            {
                sbRegScriptF.Append("alert('" + MessageHelper.GetMessage("01_03010000_003") + "');");
                sbRegScriptF.Append("window.close();");
                this.sbRegScript.Append(sbRegScriptF.ToString());
                return;
            }

            //* 區間迄日期沒有輸入
            if (!string.IsNullOrEmpty(this.dtpSearchEnd.Text.Trim().Replace("/", "")))
            {
                //* 區間迄
                strSearchEnd = this.dtpSearchEnd.Text.Trim().Replace("/", "");
            }
            else
            {
                sbRegScriptF.Append("alert('" + MessageHelper.GetMessage("01_03010000_003") + "');");
                sbRegScriptF.Append("window.close();");
                this.sbRegScript.Append(sbRegScriptF.ToString());
                return;
            }

            // 取得資料
            DataTable dtblSearchResult = CSIPKeyInGUI.BusinessRules_new.BRCompareRpt.SearchRPTData(strProperty, strSearchStart, strSearchEnd, ref lngTotal, ref strMsgID);

            // 寫入Excel
            if (null == dtblSearchResult)
            {
                //* 取報表數據不成功
                sbRegScriptF.Append("alert('" + MessageHelper.GetMessage(strMsgID) + "');");
                sbRegScriptF.Append("window.close();");
                this.sbRegScript.Append(sbRegScriptF.ToString());
            }
            else
            {
                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + _ExcelTemplate.Trim();
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ExportExcelForNPOI(dtblSearchResult, ref wb, 9, "工作表1");
                ISheet sheet = wb.GetSheet("工作表1");
                #region 表頭文字靠左格式
                HSSFCellStyle titleFromat = (HSSFCellStyle)wb.CreateCellStyle();
                //文字置中
                titleFromat.VerticalAlignment = VerticalAlignment.Center;
                titleFromat.Alignment = HorizontalAlignment.Left;
                titleFromat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                HSSFFont titleFont = (HSSFFont)wb.CreateFont();
                //字體尺寸
                titleFont.FontHeightInPoints = 12;
                titleFont.FontName = "新細明體";
                titleFromat.SetFont(titleFont);
                #endregion
                // 起訖日期
                sheet.GetRow(1).CreateCell(1).SetCellValue(strSearchStart + "-" + strSearchEnd);
                sheet.GetRow(1).GetCell(1).CellStyle = titleFromat;
                // 列印經辦
                sheet.GetRow(2).CreateCell(1).SetCellValue(((EntityAGENT_INFO)this.Session["Agent"]).agent_name);
                sheet.GetRow(2).GetCell(1).CellStyle = titleFromat;
                // 列印日期
                sheet.GetRow(3).CreateCell(1).SetCellValue(DateTime.Now.ToString("yyyyMMdd"));
                sheet.GetRow(3).GetCell(1).CellStyle = titleFromat;
                // 業務項目
                sheet.GetRow(4).CreateCell(1).SetCellValue(propertyName);
                sheet.GetRow(4).GetCell(1).CellStyle = titleFromat;
                // 總量
                sheet.GetRow(5).CreateCell(1).SetCellValue(lngTotal.ToString("N0"));
                sheet.GetRow(5).GetCell(1).CellStyle = titleFromat;

                #region 文字靠左對齊樣式
                HSSFCellStyle leftFont = (HSSFCellStyle)wb.CreateCellStyle();
                leftFont.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                leftFont.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                leftFont.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                leftFont.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                //啟動多行文字
                leftFont.WrapText = true;
                //文字置中
                leftFont.VerticalAlignment = VerticalAlignment.Center;
                leftFont.Alignment = HorizontalAlignment.Left;

                HSSFFont font1 = (HSSFFont)wb.CreateFont();
                //字體尺寸
                font1.FontHeightInPoints = 12;
                font1.FontName = "新細明體";
                leftFont.SetFont(font1);
                #endregion
                
                if(strProperty == "05010001")
                {
                    for (int row = 9; row < sheet.LastRowNum + 1; row++)
                    {
                        sheet.GetRow(row).GetCell(2).CellStyle = leftFont;
                    }
                }

                //2021/03/08_Ares_Stanley-ETAG比對值隱碼
                if(strProperty == "001")
                {
                    for (int row = 9; row < sheet.LastRowNum + 1; row++)
                    {
                        string compCellValue = sheet.GetRow(row).GetCell(3).StringCellValue;
                        if (compCellValue != "" || compCellValue != null)
                        {
                            if (compCellValue.Length >= 10)
                            {
                                string compHead = compCellValue.Substring(0, 3);
                                string compLast = compCellValue.Substring(6, compCellValue.Length - 6);
                                sheet.GetRow(row).GetCell(3).SetCellValue(compHead + "XXX" + compLast);
                            }
                        }
                    }
                }

                for (int row = 9; row < sheet.LastRowNum + 1; row++)
                {
                    sheet.GetRow(row).GetCell(0).SetCellValue((int.Parse(sheet.GetRow(row).GetCell(0).StringCellValue)).ToString("N0"));
                }

                strServerPathFile = strServerPathFile + @"\" + strProperty.Trim() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strServerPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();

                //* 將服務器端生成的文檔，下載到本地。
                string strYYYYMMDD = "000" + CSIPCommonModel.BaseItem.Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
                string strFileName = _FileName.Trim().Replace("/","") + strYYYYMMDD + ".xls";

                //* 顯示提示訊息：匯出到Excel文檔資料成功
                this.Session["ServerFile"] = strServerPathFile;
                this.Session["ClientFile"] = strFileName;
                // string urlString = @"ClientMsgShow('" + MessageHelper.GetMessage("01_01030400_003") + "');";
                string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("01_01030400_003") + "' }, '*');";
                urlString += @"location.href='DownLoadFile.aspx';";
                base.sbRegScript.Append(urlString);
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }


    }
    #region 共用NPOI
    /// <summary>
    /// 修改紀錄:2021/01/11_Ares_Stanley-新增共用NPOI
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="wb"></param>
    /// <param name="start"></param>
    /// <param name="sheetName"></param>
    private static void ExportExcelForNPOI(DataTable dt, ref HSSFWorkbook wb, Int32 start, String sheetName)
    {
        try
        {
            HSSFCellStyle cs = (HSSFCellStyle)wb.CreateCellStyle();
            cs.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cs.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cs.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            cs.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;
            cs.Alignment = HorizontalAlignment.Center;

            HSSFFont font1 = (HSSFFont)wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);

            if (dt != null && dt.Rows.Count != 0)
            {
                int count = start;
                ISheet sheet = wb.GetSheet(sheetName);
                int cols = dt.Columns.Count;
                foreach (DataRow dr in dt.Rows)
                {
                    int cell = 0;
                    IRow row = (IRow)sheet.CreateRow(count);
                    row.CreateCell(0).SetCellValue(count.ToString());
                    for (int i = 0; i < cols; i++)
                    {
                        row.CreateCell(cell).SetCellValue(dr[i].ToString());
                        row.GetCell(cell).CellStyle = cs;
                        cell++;
                    }
                    count++;
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    /// <summary>
    /// 修改紀錄: 2021/01/11_Ares_Stanley-新增共用NPOI
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="wb"></param>
    /// <param name="start"></param>
    /// <param name="delColumn"></param>
    /// <param name="sheetName"></param>
    private static void ExportExcelForNPOI_filter(DataTable dt, ref HSSFWorkbook wb, Int32 start, Int32 delColumn, String sheetName)
    {
        try
        {
            HSSFCellStyle cs = (HSSFCellStyle)wb.CreateCellStyle();
            cs.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cs.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cs.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            cs.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;
            cs.Alignment = HorizontalAlignment.Center;
            cs.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
            HSSFFont font1 = (HSSFFont)wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);

            if (dt != null && dt.Rows.Count != 0)
            {
                int count = start;
                ISheet sheet = wb.GetSheet(sheetName);
                int cols = dt.Columns.Count - delColumn;
                foreach (DataRow dr in dt.Rows)
                {
                    int cell = 0;
                    IRow row = (IRow)sheet.CreateRow(count);
                    row.CreateCell(0).SetCellValue(count.ToString());
                    for (int i = 0; i < cols; i++)
                    {
                        row.CreateCell(cell).SetCellValue(dr[i].ToString());
                        row.GetCell(cell).CellStyle = cs;
                        cell++;
                    }
                    count++;
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }
    #endregion

    #endregion function
}
