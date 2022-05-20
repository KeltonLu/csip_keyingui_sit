//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：特店資料新增- 6063一次鍵檔
//*  創建日期：2009/11/12
//*  修改紀錄：
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
using Framework.WebControls;
using Framework.Common.Logging;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.Collections.Generic;
using Framework.Common.Utility;


public partial class P010104010301 : PageBase
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
        if(!IsPostBack)
        {
            CommonFunction.SetControlsEnabled(pnlText, true);//*清空網頁中的輸入欄位
            base.sbRegScript.Append(BaseHelper.SetFocus("txtShopNo"));
            SetControlsText();
            int intPageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
            gpList.PageSize = intPageSize;
            grvShow.PageSize = intPageSize;
            ViewState["DataBind"] = null;
            this.gpList.Visible = false;
        }
        base.strClientMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/11/12
    /// 修改日期：2009/11/12
    /// <summary>
    /// 新增事件
    /// </summary>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            if (ViewState["DataBind"] != null)
            {
                DataTable dtblBind = (DataTable)ViewState["DataBind"];
                //*寫入Table  shop_6063
                for (int i = 0; i < dtblBind.Rows.Count; i++)
                {
                    EntitySHOP_6063 eShop = new EntitySHOP_6063();
                    eShop.shop_no = dtblBind.Rows[i][0].ToString();
                    eShop.batch_depict = dtblBind.Rows[i][4].ToString().ToUpper();
                    eShop.card_type = dtblBind.Rows[i][1].ToString().ToUpper();
                    eShop.favour_fee = dtblBind.Rows[i][3].ToString();
                    eShop.identify_no = dtblBind.Rows[i][2].ToString();
                    eShop.first_user = eAgentInfo.agent_name;
                    eShop.keyin_day = DateTime.Now.ToString("yyyyMMdd");
                    eShop.second_user = "";
                    eShop.send3270 = "N";
                    eShop.keyin_flag="1";
                    eShop.keyin_userID=eAgentInfo.agent_id;
                    try
                    {
                        //*先查詢有無一Key資料
                        DataSet dstResult = BRSHOP_6063.SelectInfo(dtblBind.Rows[i][0].ToString(), "N", "1", dtblBind.Rows[i][1].ToString().ToUpper(), dtblBind.Rows[i][2].ToString());

                        if (dstResult == null)
                        {
                            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                            return;
                        }
                        if (dstResult.Tables[0].Rows.Count > 0)
                        {
                            //*一Key有資料
                            BRSHOP_6063.UpdateEntity(eShop, dtblBind.Rows[i][0].ToString(), "1", dtblBind.Rows[i][1].ToString().ToUpper(), dtblBind.Rows[i][2].ToString());
                        }
                        else
                        {
                            //*一Key沒有資料
                            BRSHOP_6063.AddEntity(eShop);
                        }


                        
                    }
                    catch
                    {
                        if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_020")) < 0)
                        {
                            base.strClientMsg += MessageHelper.GetMessage("01_00000000_020");
                        }

                        return;
                    }
                }
                base.strClientMsg += MessageHelper.GetMessage("01_01040103_002");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtShopNo"));
                ViewState["DataBind"] = null;
                grvShow.DataSource = null;
                grvShow.DataBind();
            }
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
        }
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/11/12
    /// 修改日期：2009/11/12
    /// <summary>
    /// 刪除事件
    /// </summary>
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            DataTable dtblBind = (DataTable)ViewState["DataBind"];
            if (dtblBind != null && dtblBind.Rows.Count > 0)
            {
                for (int i = 0; i < dtblBind.Rows.Count; i++)
                {
                    HtmlInputRadioButton radSelect = (HtmlInputRadioButton)grvShow.Rows[i].FindControl("radSelect");
                    if (radSelect.Checked)
                    {
                        if (BRSHOP_6063.DeleteInfo(dtblBind.Rows[i][0].ToString(), dtblBind.Rows[i][1].ToString(), dtblBind.Rows[i][2].ToString()))
                        {
                            base.strClientMsg += MessageHelper.GetMessage("01_01040103_001");
                            
                            dtblBind.Rows.RemoveAt(i);
                            ViewState["DataBind"] = dtblBind;
                            BindGridView();
                        }
                        break;
                    }
                }
            }
        }
        catch 
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
        }
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/11/12
    /// 修改日期：2009/11/12
    /// <summary>
    /// CARD-TYPE欄位按下Enter鍵查詢事件

    /// </summary>
    protected void btnSelectHiden_Click(object sender, EventArgs e)
    {
        //*查詢資料，綁定在GridView中

        string strShopNo = "";

        DataSet dstResult = BRSHOP_6063.SelectInfo(this.txtShopNo.Text.Trim(), "N","1");

        if(dstResult == null)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }
        this.gpList.Visible = true;

        if (dstResult.Tables[0].Rows.Count > 0)
        {
            ViewState["DataBind"] = dstResult.Tables[0];
            BindGridView();
        }
        else
        {
            ViewState["DataBind"] = null;
            this.grvShow.DataSource = null;
            this.grvShow.DataBind();
            base.strClientMsg += MessageHelper.GetMessage("01_01040103_003");
        }

        strShopNo = this.txtShopNo.Text.Trim();
        CommonFunction.SetControlsEnabled(pnlText, true);//*清空網頁中的輸入欄位
        this.txtShopNo.Text = strShopNo;
        base.sbRegScript.Append(BaseHelper.SetFocus("txtShopNo"));   
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/11/13
    /// 修改日期：2009/11/13
    /// <summary>
    /// BATCH- 描述欄位按下Enter鍵事件

    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAddHiden_Click(object sender, EventArgs e)
    {
        this.gpList.Visible = true;
        DataTable dtblBind = (DataTable)ViewState["DataBind"];
        string strShopNo = "";

        if (dtblBind != null)
        {
            //*列表中選擇了資料，移除選中的資料
            for (int i = 0; i < grvShow.Rows.Count; i++)
            {
                HtmlInputRadioButton radSelect = (HtmlInputRadioButton)grvShow.Rows[i].FindControl("radSelect");
                if (radSelect.Checked)
                {
                    dtblBind.Rows.RemoveAt(i);
                    break;
                }
                //dtblBind.Rows.RemoveAt(i);
            }
        }
        else
        {
            dtblBind = GetDataTable();
        }


        //dtblBind = GetDataTable();
        DataRow drowNew = dtblBind.NewRow();
        drowNew[EntitySHOP_6063.M_shop_no] = this.txtShopNo.Text.Trim();
        drowNew[EntitySHOP_6063.M_batch_depict] = this.txtBatchDepict.Text.Trim().ToUpper();
        drowNew[EntitySHOP_6063.M_card_type] = this.txtCardType.Text.Trim().ToUpper();
        drowNew[EntitySHOP_6063.M_favour_fee] = this.txtFavourableFee.Text.Trim();
        drowNew[EntitySHOP_6063.M_identify_no] = this.txtIdentifyNo.Text.Trim();
        dtblBind.Rows.Add(drowNew);
        ViewState["DataBind"] = dtblBind;
        BindGridView();

        strShopNo = this.txtShopNo.Text.Trim();
        CommonFunction.SetControlsEnabled(pnlText, true);
        this.txtShopNo.Text = strShopNo;
        base.sbRegScript.Append(BaseHelper.SetFocus("txtCardType"));
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/11/12
    /// 修改日期：2009/11/12
    /// <summary>
    /// 翻頁事件
    /// </summary>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }
    #endregion

    #region 方法區
    /// 作者 趙呂梁

    /// 創建日期：2009/11/12
    /// 修改日期：2009/11/12
    /// <summary>
    /// 設置控件文本
    /// </summary>
    private void SetControlsText()
    {
        grvShow.Columns[1].HeaderText = BaseHelper.GetShowText("01_01040103_002");
        grvShow.Columns[2].HeaderText = BaseHelper.GetShowText("01_01040103_009");
        grvShow.Columns[3].HeaderText = BaseHelper.GetShowText("01_01040103_005");
        grvShow.Columns[4].HeaderText = BaseHelper.GetShowText("01_01040103_010");
        grvShow.Columns[5].HeaderText = BaseHelper.GetShowText("01_01040103_007");
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/11/13
    /// 修改日期：2009/11/13
   /// <summary>
    /// 綁定GridView
   /// </summary>
    private void BindGridView()
    {
        try
        {     
            DataTable dtblResult = (DataTable) ViewState["DataBind"];
            this.gpList.RecordCount = dtblResult.Rows.Count;
            this.grvShow.DataSource = CommonFunction.Pagination(dtblResult, this.gpList.CurrentPageIndex, this.gpList.PageSize);
            this.grvShow.DataBind();
        }
        catch  
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
        }
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/11/13
    /// 修改日期：2009/11/13
    /// <summary>
    /// 得到綁定GridView的表結構
    /// </summary>
    /// <returns>DataTable</returns>
    private DataTable GetDataTable()
    {
        DataTable dtblTemp = new DataTable();
        dtblTemp.Columns.Add(EntitySHOP_6063.M_shop_no);
        dtblTemp.Columns.Add(EntitySHOP_6063.M_card_type);
        dtblTemp.Columns.Add(EntitySHOP_6063.M_identify_no);
        dtblTemp.Columns.Add(EntitySHOP_6063.M_favour_fee);
        dtblTemp.Columns.Add(EntitySHOP_6063.M_batch_depict);
        return dtblTemp;
    }
    #endregion
}
