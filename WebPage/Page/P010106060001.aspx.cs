//******************************************************************
//*  作    者：
//*  功能說明：錯誤登錄
//*  創建日期：2014/08/20
//*  修改記錄：
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
using System.Drawing;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;

public partial class P010106060001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    #endregion

    #region event
 
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            //* 綁定GridView列頭訊息
            ShowControlsText();
            this.gvpbSearchRecord.DataSource = null;
            this.gvpbSearchRecord.DataBind();
        }
        base.strHostMsg += "";
        base.strClientMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.pnlErrKeyin.Visible = false;
        BindGridView();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        string strBatchDate = this.Session["BatchDate"].ToString();     //編批日期
        string strBatchNO = this.Session["BatchNO"].ToString();         //批號
        string strShopID = this.Session["ShopID"].ToString();           //商店代號
        string strSignType = this.Session["SignType"].ToString();       //簽單類別

        //選定的gridview資料序號
        int selectSN = Convert.ToInt32(this.dropSN.SelectedValue);
        //收件批次
        string strReceiveBatch = ((HiddenField)this.gvpbSearchRecord.Rows[selectSN - 1].Cells[0].FindControl("Receive_Batch")).Value;
        //人工簽單明細檔資料序號
        string strSN = ((HiddenField)this.gvpbSearchRecord.Rows[selectSN - 1].Cells[0].FindControl("DetailSN")).Value;

        //選擇的錯誤欄位於gvpbSearchRecord中的順序號碼
        int iErrCol = Convert.ToInt32(this.dropErrColumn.SelectedValue);
        //錯誤值
        string strErrValue = this.gvpbSearchRecord.Rows[selectSN - 1].Cells[iErrCol].Text;

        if (BRASErrKeyin.Check_Insert_ASErr(strBatchDate, strBatchNO, strShopID, dropErrColumn.SelectedItem.Text, strSN, ref strMsgID))
        {
            //通過重覆資料檢查，寫入錯誤登錄檔
            if (BRASErrKeyin.Insert_ASErr(strBatchDate, strBatchNO, strShopID, strReceiveBatch, strSignType, strSN, dropErrColumn.SelectedItem.Text,
                txtCorrectValue.Text.Trim(), dropReflectSource.SelectedValue, strErrValue, eAgentInfo.agent_id, ref strMsgID))
            {
                string strKey = strBatchDate.Replace("/", "") + strBatchNO + strShopID;
                DataTable dtlog = CommonFunction.GetDataTable();
                CommonFunction.UpdateLog(dropErrColumn.SelectedItem.Text, txtCorrectValue.Text.Trim(), "新增", dtlog);
                CommonFunction.InsertCustomerLog(dtlog, eAgentInfo, strKey, "DB", (structPageInfo)Session["PageInfo"]);
            }
        }

        MessageHelper.ShowMessage(this.UpdatePanel1,strMsgID);
        //* 顯示端末訊息
        base.strClientMsg += MessageHelper.GetMessage(strMsgID);

        Refresh_gvSearchASErr();
    }

    protected void gvpbSearchRecord_RowDataBound(object sender, GridViewRowEventArgs e)
    {
    }

    protected void gvSearchASErr_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CustLinkButton LbtnDelete = (CustLinkButton)e.Row.FindControl("lbtnDelete");
            LbtnDelete.Text = BaseHelper.GetShowText("01_01060600_025");
            LbtnDelete.Attributes.Add("onclick", "return confirm('" + BaseHelper.GetShowText("01_01060600_027") + "')"); 
        }
    }

    /// <summary>
    /// 確定刪除
    /// </summary>
    protected void gvSearchASErr_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        string strMsgID = string.Empty;
        string strBatchDate = this.Session["BatchDate"].ToString();     //編批日期
        string strBatchNO = this.Session["BatchNO"].ToString();         //批號
        string strShopID = this.Session["ShopID"].ToString();           //商店代號
        string strSignType = this.Session["SignType"].ToString();       //簽單類別

        //收件批次
        string strReceiveBatch = ((HiddenField)this.gvSearchASErr.Rows[e.RowIndex].Cells[0].FindControl("Err_Receive_Batch")).Value;
        //明細序號
        string strSN = this.gvSearchASErr.Rows[e.RowIndex].Cells[2].Text;
        //錯誤欄位
        string strErrorColumn = this.gvSearchASErr.Rows[e.RowIndex].Cells[3].Text;
        //正確值
        string strCorrectValue = this.gvSearchASErr.Rows[e.RowIndex].Cells[5].Text;


        if(BRASErrKeyin.Delete_ASErr(strBatchDate, strBatchNO, strShopID, strReceiveBatch, strSignType, strSN, strErrorColumn, ref strMsgID))
        {
            string strKey = strBatchDate.Replace("/", "") + strBatchNO + strShopID;
            DataTable dtlog = CommonFunction.GetDataTable();
            CommonFunction.UpdateLog(strErrorColumn, "", "刪除", dtlog);
            CommonFunction.InsertCustomerLog(dtlog, eAgentInfo, strKey, "DB", (structPageInfo)Session["PageInfo"]);
        }

        MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
        //* 顯示端末訊息
        base.strClientMsg += MessageHelper.GetMessage(strMsgID);

        Refresh_gvSearchASErr();
    }
    
    #endregion event

    #region function
    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        #region 第一個查詢畫面
        this.gvpbSearchRecord.Columns[0].HeaderText = BaseHelper.GetShowText("01_01060600_006");
        this.gvpbSearchRecord.Columns[1].HeaderText = BaseHelper.GetShowText("01_01060600_007");
        this.gvpbSearchRecord.Columns[2].HeaderText = BaseHelper.GetShowText("01_01060600_008");
        this.gvpbSearchRecord.Columns[3].HeaderText = BaseHelper.GetShowText("01_01060600_009");
        this.gvpbSearchRecord.Columns[4].HeaderText = BaseHelper.GetShowText("01_01060600_010");
        this.gvpbSearchRecord.Columns[5].HeaderText = BaseHelper.GetShowText("01_01060600_011");
        this.gvpbSearchRecord.Columns[6].HeaderText = BaseHelper.GetShowText("01_01060600_012");
        this.gvpbSearchRecord.Columns[7].HeaderText = BaseHelper.GetShowText("01_01060600_013");
        this.gvpbSearchRecord.Columns[8].HeaderText = BaseHelper.GetShowText("01_01060600_014");
        this.gvpbSearchRecord.Columns[9].HeaderText = BaseHelper.GetShowText("01_01060600_015");
        this.gvpbSearchRecord.Columns[10].HeaderText = BaseHelper.GetShowText("01_01060600_016");
        #endregion 第一個查詢畫面

        #region 第二個查詢畫面(錯誤登錄資料)
        this.gvSearchASErr.Columns[0].HeaderText = BaseHelper.GetShowText("01_01060600_020");
        this.gvSearchASErr.Columns[1].HeaderText = BaseHelper.GetShowText("01_01060600_004");
        this.gvSearchASErr.Columns[2].HeaderText = BaseHelper.GetShowText("01_01060600_021");
        this.gvSearchASErr.Columns[3].HeaderText = BaseHelper.GetShowText("01_01060600_017");
        this.gvSearchASErr.Columns[4].HeaderText = BaseHelper.GetShowText("01_01060600_022");
        this.gvSearchASErr.Columns[5].HeaderText = BaseHelper.GetShowText("01_01060600_018");
        this.gvSearchASErr.Columns[6].HeaderText = BaseHelper.GetShowText("01_01060600_023");
        this.gvSearchASErr.Columns[7].HeaderText = BaseHelper.GetShowText("01_01060600_024");
        #endregion 第二個查詢畫面(錯誤登錄資料)
    }

    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    private void BindGridView()
    {
        try
        {
            string strBatchNO = this.txtBatchNO.Text.Trim();    //批號
            if (strBatchNO.Length < 3)
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060600_004");
                return;
            }
            string strSignType = strBatchNO.Length == 3 ? "1" : "2";    //簽單類別, 一般簽單,值:1,分期簽單,值:2
            string strShopID = this.txtShopID.Text.Trim();
            string strMsgID = string.Empty;

            string strBatchDate = this.dpBatchDate.Text.Replace("/", "");

            //存入Session後續新增資料使用
            this.Session["BatchDate"] = this.dpBatchDate.Text;
            this.Session["BatchNO"] = strBatchNO;
            this.Session["ShopID"] = strShopID;
            this.Session["SignType"] = strSignType;

            DataSet ds = BRASErrKeyin.SearchASErrData(strBatchDate, strBatchNO, strShopID, strSignType, "ALL", ref strMsgID);
            if (ds == null)
            {
                this.gvpbSearchRecord.DataSource = null;
                this.gvpbSearchRecord.DataBind();
            }
            else
            {
                this.gvpbSearchRecord.DataSource = ds.Tables["ASErr1"];
                this.gvpbSearchRecord.DataBind();

                this.gvSearchASErr.DataSource = ds.Tables["ASErr2"];
                this.gvSearchASErr.DataBind();

                BindDropDownList(ds.Tables["ASErr1"].Rows.Count);
                this.pnlErrKeyin.Visible = true;
            }

            //20141017  超過10筆顯示拉霸
            if (this.gvpbSearchRecord.Rows.Count > 10)
            {
                this.Pl_gv1.Height = 325;
            }
            if (this.gvSearchASErr.Rows.Count > 10)
            {
                this.Pl_gv2.Height = 325;
            }

            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage(strMsgID);
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_01060600_001");
        }
    }

    /// <summary>
    /// 綁定DropDownList控件
    /// </summary>
    private void BindDropDownList(int iDataCount)
    {
        try
        {
            this.dropSN.Items.Clear();
            this.dropErrColumn.Items.Clear();
            this.dropReflectSource.Items.Clear();
            
            //序號
            for (int i = 1; i <= iDataCount; i++)
            {
                this.dropSN.Items.Add(i.ToString());
            }

            //錯誤欄位 (value對應gvpbSearchRecord的欄位)
            this.dropErrColumn.Items.Add(new ListItem("交易卡號", "2"));
            this.dropErrColumn.Items.Add(new ListItem("交易日期", "3"));
            this.dropErrColumn.Items.Add(new ListItem("產品別", "4"));
            this.dropErrColumn.Items.Add(new ListItem("分期期數", "5"));
            this.dropErrColumn.Items.Add(new ListItem("授權號碼", "6"));
            this.dropErrColumn.Items.Add(new ListItem("金額/分期總價", "7"));
            this.dropErrColumn.Items.Add(new ListItem("請退款", "8"));

            //反應來源
            //* 讀取公共屬性
            DataTable dtblProperty = null;
            if (!BaseHelper.GetCommonProperty("01", "AS_REFLECT_SOURCE", ref dtblProperty))
            {
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("01_01060600_008");
            }
            else
            {
                //* 將取得的【報表種類】訊息顯示到報表種類DropDownList控件
                this.dropReflectSource.Items.Clear();
                this.dropReflectSource.DataSource = dtblProperty;
                this.dropReflectSource.DataTextField = EntityM_PROPERTY_CODE.M_PROPERTY_NAME;
                this.dropReflectSource.DataValueField = EntityM_PROPERTY_CODE.M_PROPERTY;
                this.dropReflectSource.DataBind();
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_01060600_008");
        }
    }

    /// <summary>
    /// 刷新gvSearchASErr
    /// </summary>
    private void Refresh_gvSearchASErr()
    {
        try
        {
            string strBatchDate = this.Session["BatchDate"].ToString();
            string strBatchNO = this.Session["BatchNO"].ToString();     //批號
            string strSignType = this.Session["SignType"].ToString();   //簽單類別, 一般簽單,值:1,分期簽單,值:2
            string strShopID = this.Session["ShopID"].ToString();
            string strMsgID = string.Empty;

            DataSet ds = BRASErrKeyin.SearchASErrData(strBatchDate, strBatchNO, strShopID, strSignType, "ASErr2", ref strMsgID);
            if (ds == null)
            {
                this.gvSearchASErr.DataSource = null;
                this.gvSearchASErr.DataBind();
            }
            else
            {
                this.gvSearchASErr.DataSource = ds.Tables["ASErr2"];
                this.gvSearchASErr.DataBind();
            }

            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage(strMsgID);
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_01060600_001");
        }
    }
    #endregion function
}
