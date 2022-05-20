//******************************************************************
//*  作    者：chenjingxian
//*  功能說明：特店認同卡優惠費率維護-新增/修改維護作業(特店6063費率新增修改)
//*  創建日期：2010/04/27
//*  修改紀錄：
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************




using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CSIPCommonModel.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Message;
using Framework.Common.Logging;
using Framework.Common.Utility;

public partial class P010104080001 : PageBase
{

    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SetControlsText();
            //*修改按鈕disable
            this.btnSubmit.Enabled = false;
            //*設置GridView分頁顯示的行數

            int intPageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
            //gpList.PageSize = intPageSize;
            //grvShow.PageSize = intPageSize;
            ViewState["DataBind"] = null;
            //this.gpList.Visible = false;
            base.sbRegScript.Append(BaseHelper.SetFocus("txtShopNo"));
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合    

    }

    /// 作者 chenjingxian

    /// 創建日期：2010/04/27
    /// 修改日期：2010/04/27
    /// <summary>
    /// 查詢事件
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {

        int iCount = 0;
        int iRCount = 0;
        int iNum = 0;
        int iBegin = 0;
        int iUpFlag = 0;
        string strKey = "";

        txtHiden.Text = "0";

        DataTable dtblBindView = (DataTable)ViewState["DataBind"];
        int iTotalCount = 0;

        //*將上方的輸入欄位清空
        this.txtCardType.Text = "";
        this.txtIdentifyNo.Text = "";
        this.txtFavourableFee.Text = "";
        this.txtBatchDepict.Text = "";

        //*查詢前先將修改按鈕disable
        this.btnSubmit.Enabled = false;

        //*將下方的list清空
        if (dtblBindView != null)
        {
            dtblBindView.Clear();
            ViewState["DataBind"] = dtblBindView;
            BindGridView();
        }

        //this.gpList.Visible = false;

        Hashtable htInput = new Hashtable();

        htInput.Add("FUNCTION_CODE", "I");//*功能別 查詢
        htInput.Add("MERCH_ACCT", this.txtShopNo.Text.Trim());//*特店代號
        htInput.Add("LINE_CNT", "00");//*LINE_CNT

        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGX, htInput, false, "11", eAgentInfo);

        if (!htReturn.Contains("HtgMsg"))
        {
            //*主機返回正確

            htReturn["MERCH_ACCT"] = htInput["MERCH_ACCT"];

            base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息     


            DataTable dtblBind = GetDataTable();


            if (htReturn["LINE_CNT"].ToString() != "")//*取得卡人資料筆數
            {
                iRCount = Convert.ToInt32(htReturn["LINE_CNT"].ToString());
            }

            iNum = iRCount - iCount * 5;

            if (iNum <= 5)
            {
                iUpFlag = 1;
            }
            else
            {
                iNum = 5;
            }

            while (iNum > 0)
            {
                //*為網頁欄位(list)賦值

                for (int i = 1; i < iNum + 1; i++)
                {

                    iTotalCount++;
                    DataRow drowRow = dtblBind.NewRow();
                    for (int j = 0; j < dtblBind.Columns.Count; j++)
                    {

                        strKey = dtblBind.Columns[j].ToString() + i.ToString();
                        if (j == 0 || j == 3)
                        {
                            drowRow[j] = htReturn[strKey].ToString().Trim().ToUpper();
                        }
                        else if (j == 2)
                        {
                            drowRow[j] = htReturn[strKey].ToString().Trim().PadLeft(5, '0');
                        }
                        else
                        {

                            drowRow[j] = htReturn[strKey].ToString().Trim();

                        }


                    }
                    dtblBind.Rows.Add(drowRow);
                }

                if (iUpFlag == 1)
                {
                    break;
                }

                iBegin = (iCount + 1) * 5 + 1;
                htInput["FUNCTION_CODE"] = "I";
                htInput["LINE_CNT"] = iBegin.ToString().PadLeft(2, '0');
                htInput["MERCH_ACCT"] = this.txtShopNo.Text.Trim();

                htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGX, htInput, false, "11", eAgentInfo);

                if (htReturn.Contains("HtgMsg"))
                {
                    //*查詢主機失敗

                    //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
                    etMstType = eMstType.Select;
                    //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

                    if (htReturn["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                    {
                       
                        base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
                        base.strHostMsg += htReturn["HtgMsg"].ToString();

                     
                    }
                    else
                    {
                        base.strClientMsg += htReturn["HtgMsg"].ToString();
                    }
                    return;
                }
                else
                {


                    iCount++;
                    iRCount = Convert.ToInt32(htReturn["LINE_CNT"].ToString());
                    iNum = iRCount - iCount * 5;
                    if (iNum <= 5)
                    {
                        iUpFlag = 1;
                    }
                    else
                    {
                        iNum = 5;
                    }
                }


            }

            ViewState["HtgInfo"] = htReturn;
            base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_031");
            ViewState["DataBind"] = dtblBind;

            if (dtblBind.Rows.Count > 0)
            {

                //this.gpList.Visible = true;
                //*綁定GridView               
                BindGridView();

            }
            else
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01040501_003");
            }

            base.sbRegScript.Append(BaseHelper.SetFocus("txtShopNo"));

        }
        else
        {
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            etMstType = eMstType.Select;
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

            //*show 錯誤訊息
            if (htReturn["HtgMsgFlag"].ToString() == "0")
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
                base.strHostMsg += htReturn["HtgMsg"].ToString();
            }
            else
            {
                base.strClientMsg += htReturn["HtgMsg"].ToString();
            }

            if (htReturn["MESSAGE_TYPE"] != null && htReturn["MESSAGE_TYPE"].ToString() == "8888")
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01040501_007");
            }

        }
    }

    /// 作者 chenjingxian
    /// 創建日期：2010/04/27
    /// 修改日期：2010/04/27
    /// <summary>
    /// 新增事件
    /// </summary>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        //*先判斷是否要清空list

        if (txtHiden.Text.Trim() == "1")
        {
            txtHiden.Text = "0";

            //*清空list
            DataTable dtblBindView = (DataTable)ViewState["DataBind"];

            if (dtblBindView != null)
            {
                dtblBindView.Clear();
                ViewState["DataBind"] = dtblBindView;
                BindGridView();
            }
        }




        Hashtable htInput = new Hashtable();

        htInput.Add("FUNCTION_CODE", "A");//*功能別


        //*特店編號
        htInput.Add("MERCH_ACCT", txtShopNo.Text.Trim());

        //*TYPE
        htInput.Add("CARD_TYPE", txtCardType.Text.Trim().ToUpper());

        //*認同代號
        htInput.Add("AGENT_BANK_NMBR", txtIdentifyNo.Text.Trim());

        //*優惠費率
        htInput.Add("DISCOUNT_RATE", txtFavourableFee.Text.Trim().PadLeft(5, '0'));

        //*Batch-描述
        htInput.Add("AGENT_DESC", txtBatchDepict.Text.Trim().ToUpper());

        Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGX, htInput, false, "21", eAgentInfo);

        if (!htResult.Contains("HtgMsg"))
        {
            //*新增成功
            //this.gpList.Visible = true;
            DataTable dtblBind = (DataTable)ViewState["DataBind"];

            if (dtblBind == null)
            {
                dtblBind = GetDataTable();
            }

            DataRow drowNew = dtblBind.NewRow();

            drowNew["CARD_TYPE"] = txtCardType.Text.Trim().ToUpper();
            drowNew["AGENT_DESC"] = txtBatchDepict.Text.Trim().ToUpper();
            drowNew["DISCOUNT_RATE"] = txtFavourableFee.Text.Trim().PadLeft(5, '0');
            drowNew["AGENT_BANK_NMBR"] = txtIdentifyNo.Text.Trim();
            dtblBind.Rows.Add(drowNew);
            ViewState["DataBind"] = dtblBind;
            BindGridView();

            //*上方欄位清空
            txtCardType.Text = "";
            txtBatchDepict.Text = "";
            txtFavourableFee.Text = "";
            txtIdentifyNo.Text = "";

            base.strClientMsg += MessageHelper.GetMessage("01_01040501_006");
            base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息;

        }
        else
        {
            //*新增失敗
            if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg += htResult["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_027");
            }
            else
            {
                base.strClientMsg += htResult["HtgMsg"].ToString();
            }

        }
    }

    /// 作者 chenjingxian
    /// 創建日期：2010/04/27
    /// 修改日期：2010/04/27
    /// <summary>
    /// 修改事件
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        if (ViewState["HtgInfo"] != null)
        {
            Hashtable htInput = new Hashtable();
            CommonFunction.GetViewStateHt(ViewState["HtgInfo"], ref htInput);
            htInput["MESSAGE_TYPE"] = "";
            htInput["FUNCTION_CODE"] = "C";
            htInput["LINE_CNT"] = "00";
            htInput["MERCH_ACCT"] = this.txtShopNo.Text.Trim();


            #region 比對主機資料
            //*CARD-TYPE
            CommonFunction.ContrastData(htInput, this.txtCardType.Text.Trim().ToUpper(), "CARD_TYPE");

            //*認同代號
            CommonFunction.ContrastData(htInput, this.txtIdentifyNo.Text.Trim().ToUpper(), "AGENT_BANK_NMBR");

            //*優惠費率
            CommonFunction.ContrastData(htInput, this.txtFavourableFee.Text.Trim().ToUpper().PadLeft(5, '0'), "DISCOUNT_RATE");

            //*BATCH-描述
            CommonFunction.ContrastData(htInput, this.txtBatchDepict.Text.Trim().ToUpper().ToUpper(), "AGENT_DESC");

            #endregion

            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGX, htInput, false, "21", eAgentInfo);

            //*異動主機資料成功
            if (!htResult.Contains("HtgMsg"))
            {

                DataTable dtblBind = (DataTable)ViewState["DataBind"];


                if (dtblBind != null)
                {
                    //*列表中選擇了資料
                    int i = 0;
                    int iCur = 0;
                    int intPageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
                    for (i = 0; i < grvShow.Rows.Count; i++)
                    {
                        HtmlInputRadioButton radSelect = (HtmlInputRadioButton)grvShow.Rows[i].FindControl("radSelect");
                        if (radSelect.Checked)
                        {
                            //dtblBind.Rows.RemoveAt(i);
                            //  iCur = (gpList.CurrentPageIndex - 1) * intPageSize + i;
                            iCur = i;
                            dtblBind.Rows[iCur]["CARD_TYPE"] = txtCardType.Text.Trim().ToUpper();
                            dtblBind.Rows[iCur]["AGENT_DESC"] = txtBatchDepict.Text.Trim().ToUpper();
                            dtblBind.Rows[iCur]["DISCOUNT_RATE"] = txtFavourableFee.Text.Trim().PadLeft(5, '0');
                            dtblBind.Rows[iCur]["AGENT_BANK_NMBR"] = txtIdentifyNo.Text.Trim();

                            break;
                        }

                    }

                    //DataRow drowNew = dtblBind.NewRow();
                    //drowNew["CARD_TYPE"] = txtCardType.Text.Trim().ToUpper();
                    //drowNew["AGENT_DESC"] = txtBatchDepict.Text.Trim().ToUpper();
                    //drowNew["DISCOUNT_RATE"] = txtFavourableFee.Text.Trim().PadLeft(5, '0');
                    //drowNew["AGENT_BANK_NMBR"] = txtIdentifyNo.Text.Trim();
                    //dtblBind.Rows.Add(drowNew);
                    ViewState["DataBind"] = dtblBind;
                    BindGridView();

                    if (i >= 0)
                    {
                        HtmlInputRadioButton radioObj = (HtmlInputRadioButton)grvShow.Rows[i].FindControl("radSelect");
                        radioObj.Checked = true;
                    }
                }

                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_039");
                txtCardType.Text = "";
                txtBatchDepict.Text = "";
                txtFavourableFee.Text = "";
                txtIdentifyNo.Text = "";
                this.btnSubmit.Enabled = false;

            }
            else
            {
                //*異動主機資料失敗
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_027");
                }
                else
                {
                    base.strClientMsg += htResult["HtgMsg"].ToString();
                }

            }
        }


    }


    /// 作者 chenjingxian

    /// 創建日期：2010/04/27
    /// 修改日期：2010/04/27
    /// <summary>
    /// 翻頁事件
    /// </summary>
    //protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    //{
    //    this.gpList.CurrentPageIndex = e.NewPageIndex;
    //    BindGridView();
    //}
    #endregion

    #region 方法區
    /// 作者 趙呂梁


    /// 創建日期：2009/12/01
    /// 修改日期：2009/12/01
    /// <summary>
    /// 設置控件文本
    /// </summary>
    private void SetControlsText()
    {
        grvShow.Columns[0].HeaderText = BaseHelper.GetShowText("01_01040501_018");
        grvShow.Columns[1].HeaderText = BaseHelper.GetShowText("01_01040501_015");
        grvShow.Columns[2].HeaderText = BaseHelper.GetShowText("01_01040501_005");
        grvShow.Columns[3].HeaderText = BaseHelper.GetShowText("01_01040501_016");
        grvShow.Columns[4].HeaderText = BaseHelper.GetShowText("01_01040501_007");

    }

    /// 作者 趙呂梁


    /// 創建日期：2009/12/02
    /// 修改日期：2009/12/02
    /// <summary>
    /// 得到綁定列表的表結構
    /// </summary>
    /// <returns>存儲主機信息表</returns>
    public static DataTable GetDataTable()
    {
        DataTable dtblBind = new DataTable();
        dtblBind.Columns.Add("CARD_TYPE");
        dtblBind.Columns.Add("AGENT_BANK_NMBR");
        dtblBind.Columns.Add("DISCOUNT_RATE");
        dtblBind.Columns.Add("AGENT_DESC");
        return dtblBind;
    }



    /// 作者 趙呂梁


    /// 創建日期：2009/12/02
    /// 修改日期：2009/12/02
    /// <summary>
    /// 費率格式轉換
    /// </summary>
    /// <param name="strValue">優惠費率欄位值</param>
    /// <returns>轉換后的費率值</returns>
    private string GetFeeValue(string strValue)
    {
        if (!string.IsNullOrEmpty(strValue))
        {
            try
            {
                float intTemp = float.Parse(strValue) * 1000;
                return intTemp.ToString().PadLeft(5, '0');
            }
            catch
            {
                return "00000";
            }
        }
        else
        {
            return "00000";
        }
    }

    /// 作者 chenjingxian

    /// 創建日期：2010/04/27
    /// 修改日期：2010/04/27
    ///<summary>
    /// 綁定主機資料
    /// </summary>
    private void BindGridView()
    {
        try
        {
            DataTable dtblResult = (DataTable)ViewState["DataBind"];
            //this.gpList.RecordCount = dtblResult.Rows.Count;
            //this.grvShow.DataSource = CommonFunction.Pagination(dtblResult, this.gpList.CurrentPageIndex, this.gpList.PageSize);
            this.grvShow.DataSource = dtblResult;
            this.grvShow.DataBind();
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
        }
    }
    #endregion
}
