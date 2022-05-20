//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：特店資料更改- 6063
//*  創建日期：2009/12/01
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

public partial class P010104030301 : PageBase
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
            CommonFunction.SetControlsEnabled(pnlText, true);
            //*設置GridView分頁顯示的行數

            int intPageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
            //this.gpList.PageSize = intPageSize;
            //this.grvShow.PageSize = 100;
            this.grvShow.AllowPaging = false;
            //this.gpList.Visible = false;
            base.sbRegScript.Append(BaseHelper.SetFocus("txtShopNo"));
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合     
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/12/01
    /// 修改日期：2009/12/01
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


        int iTotalCount = 0;


        //*依據EXMS 6063 P4A 查詢主機資料
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
            this.txtCardType.Text = htReturn["CARD_TYPE"].ToString();
            this.txtIdentifyNo.Text = htReturn["AGENT_BANK_NMBR"].ToString();
            this.txtFavourableFee.Text = htReturn["DISCOUNT_RATE"].ToString();
            this.txtBatchDepict.Text = htReturn["AGENT_DESC"].ToString();

            //this.gpList.Visible = true;
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
                        if (j != 3)
                        {

                            strKey = dtblBind.Columns[j].ToString() + i.ToString();
                            drowRow[j] = htReturn[strKey].ToString();
                        }
                        else
                        {
                            //*列表中第一筆資料的BATCH-描述為網頁中的【BATCH- 描述】

                            if (iTotalCount == 1)
                            {
                                drowRow[j] = this.txtBatchDepict.Text.Trim();
                            }
                            else
                            {
                                //*除第一筆資料外的BATCH-描述填寫空

                                drowRow[j] = "";
                            }
                        }
                    }
                    dtblBind.Rows.Add(drowRow);
                }

                if (iUpFlag == 1)
                {
                    break ;
                }

                iBegin = (iCount + 1) * 5 + 1;
                htInput["FUNCTION_CODE"] = "I";
                htInput["LINE_CNT"] = iBegin.ToString().PadLeft(2, '0');
                htInput["MERCH_ACCT"] = this.txtShopNo.Text.Trim();

                htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGX, htInput, false, "11", eAgentInfo);

                if (htReturn.Contains("HtgMsg"))
                {

                    if (htReturn["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                    {
                        base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
                        base.strHostMsg += htReturn["HtgMsg"].ToString();
                    }
                    else
                    {
                        base.strClientMsg += htReturn["HtgMsg"].ToString();
                    }
                    return  ;
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

            base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_031");
            ViewState["DataBind"] = dtblBind;
            //*綁定GridView               
            BindGridView();

            base.sbRegScript.Append(BaseHelper.SetFocus("txtShopNo"));

        }
        else
        {
            if (htReturn["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
                base.strHostMsg += htReturn["HtgMsg"].ToString();
            }
            else
            {
                base.strClientMsg += htReturn["HtgMsg"].ToString();
            }

        }
        
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/12/01
    /// 修改日期：2009/12/01
    /// <summary>
    /// 更新事件
    /// </summary>
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        //*依據EXMS 6063 P4A 查詢主機資料
        Hashtable htReturn = GetHtgInfo();
        if (!htReturn.Contains("HtgMsg"))
        {
            base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_031");
            //*比對< CARD-TYPE>
            CommonFunction.ContrastData(htReturn, this.txtCardType.Text.Trim().ToUpper(), "CARD_TYPE");
            //*比對<認同代號>
            CommonFunction.ContrastData(htReturn, this.txtIdentifyNo.Text.Trim(), "AGENT_BANK_NMBR");
            //*比對<優惠費率>
            CommonFunction.ContrastData(htReturn, GetFeeValue(this.txtFavourableFee.Text.Trim()), "DISCOUNT_RATE");
            //*比對<Batch-描述>
            CommonFunction.ContrastData(htReturn, this.txtBatchDepict.Text.Trim().ToUpper(), "AGENT_DESC");

            htReturn["FUNCTION_CODE"] = "C";//*功能別 更新
            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGX, htReturn, false, "21", eAgentInfo);
            if (!htResult.Contains("HtgMsg"))
            {
                //*將【主機特店資料】列表中的優惠費率異動為提交至主機上的費率

                for (int i = 0; i < grvShow.Rows.Count; i++)
                {
                    this.grvShow.Rows[i].Cells[3].Text = GetFeeValue(this.txtFavourableFee.Text.Trim());
                }

                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_039");
            }
            else
            {
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
            base.sbRegScript.Append(BaseHelper.SetFocus("txtShopNo"));
        }
        else
        {
            if (htReturn["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg += htReturn["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
            }
            else
            {
                base.strClientMsg += htReturn["HtgMsg"].ToString();
            }
        }
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/12/02
    /// 修改日期：2009/12/02
    /// <summary>
    /// 刪除事件
    /// </summary>
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        DataTable dtblBind = (DataTable)ViewState["DataBind"];
        if (dtblBind != null && dtblBind.Rows.Count > 0) 
        {
            Hashtable htInput = new Hashtable();
            htInput.Add("MERCH_ACCT", this.txtShopNo.Text.Trim());//*提交<特店編號>

            string strCardType = "";//*列表選中行的< CARD-TYPE>
            string strIdentifyNo = "";//*列表選中行的< 認同代號>
            string strFee = "";//*列表選中行的<優惠費率>
            string strDesc = "";//*列表選中行的<優惠費率>

            int intSelectIndex = 0;
            for (int i = 0; i < dtblBind.Rows.Count; i++)
            {
                HtmlInputRadioButton radSelect = (HtmlInputRadioButton)grvShow.Rows[i].FindControl("radSelect");
                if (radSelect.Checked)
                {
                    strCardType = dtblBind.Rows[i][0].ToString().Trim();
                    strIdentifyNo = dtblBind.Rows[i][1].ToString().Trim();
                    strFee = dtblBind.Rows[i][2].ToString().Trim();
                    strDesc = dtblBind.Rows[i][3].ToString().Trim();
                    intSelectIndex = i;
                    break;
                }
            }

            htInput.Add("CARD_TYPE", strCardType.ToUpper());//*提交< CARD-TYPE>
            htInput.Add("AGENT_BANK_NMBR", strIdentifyNo);//*提交<認同代號>
            htInput.Add("DISCOUNT_RATE", GetFeeValue(strFee));//*提交<優惠費率>
            htInput.Add("AGENT_DESC", strDesc);//*提交<BATCH-描述>
            htInput.Add("FUNCTION_CODE", "D");//*提交<異動類型>為”D”

            //*依據EXMS 6063 P4A Submit 異動主機資料
            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGX, htInput, false, "2", eAgentInfo);

            if (!htResult.Contains("HtgMsg"))
            {
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_039");
                dtblBind.Rows.RemoveAt(intSelectIndex);
                ViewState["DataBind"] = dtblBind;
                BindGridView();
                //this.gpList.Visible = false;

                //*刪除資料庫資料
                try
                {
                    BRSHOP_6063.Delete(this.txtShopNo.Text.Trim(), strCardType, strIdentifyNo);
                }
                catch 
                {
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                }
            }
            else
            {
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

    /// 作者 趙呂梁

    /// 創建日期：2009/12/02
    /// 修改日期：2009/12/02
    /// <summary>
    /// 分頁事件
    /// </summary>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        //this.gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }
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
        grvShow.Columns[1].HeaderText = BaseHelper.GetShowText("01_01040303_009");
        grvShow.Columns[2].HeaderText = BaseHelper.GetShowText("01_01040303_005");
        grvShow.Columns[3].HeaderText = BaseHelper.GetShowText("01_01040303_010");
        grvShow.Columns[4].HeaderText = BaseHelper.GetShowText("01_01040303_007");
        grvShow.Columns[5].HeaderText = BaseHelper.GetShowText("01_01040303_011");
        grvShow.Columns[6].HeaderText = BaseHelper.GetShowText("01_01040303_012");
        grvShow.Columns[7].HeaderText = BaseHelper.GetShowText("01_01040303_013");
        grvShow.Columns[8].HeaderText = BaseHelper.GetShowText("01_01040303_014");
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
        dtblBind.Columns.Add("CHANGE_ID");
        dtblBind.Columns.Add("CHANGE_DATE");
        dtblBind.Columns.Add("CHANGE_ID_B");
        dtblBind.Columns.Add("CHANGE_DATE_B");
        return dtblBind;
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/12/02
    /// 修改日期：2009/12/02
    /// <summary>
    /// 依據EXMS 6063 P4A 查詢主機資料
    /// </summary>
    /// <returns>主機資料</returns>
    private Hashtable GetHtgInfo()
    {     
        Hashtable htInput = new Hashtable();

        htInput.Add("FUNCTION_CODE", "I");//*功能別 查詢
        //*特店編號
        htInput.Add("MERCH_ACCT", this.txtShopNo.Text.Trim());

        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGX, htInput, false, "11", eAgentInfo);
        htReturn["MERCH_ACCT"] = htInput["MERCH_ACCT"];//* for_xml_test  
        htReturn["FUNCTION_CODE"] = "I";//* for_xml_test  
        htReturn["MESSAGE_TYPE"] = "";
        return htReturn;

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

    /// 作者 趙呂梁

    /// 創建日期：2009/12/02
    /// 修改日期：2009/12/02
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
        catch(Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex, LogLayer.UI);
        }
    }
    #endregion
}
