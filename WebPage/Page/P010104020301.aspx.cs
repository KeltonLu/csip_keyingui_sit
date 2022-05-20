//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：特店資料新增- 6063二次鍵檔
//*  創建日期：2009/11/17
//*  修改紀錄：
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************
using System;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CSIPCommonModel.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using Framework.WebControls;
using Framework.Common.Message;
using Framework.Common.Logging;
using Framework.Common.Utility;

public partial class P010104020301 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SetControlsText();
            //*設置GridView分頁顯示的行數
            int intPageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
            this.gpList.PageSize = intPageSize;
            this.gpListTwo.PageSize = intPageSize;
            this.grvShow.PageSize = intPageSize;
            this.grvShowTwo.PageSize = intPageSize;
            this.gpList.Visible = false;
            this.gpListTwo.Visible = false;
            this.btnAdd.Enabled = false;
            
            CommonFunction.SetControlsEnabled(pnlText, true);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtShopNo"));
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        SetRowsForeColor();//*將二次鍵檔資料中行文字設置為黑色
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/17
    /// 修改日期：2009/11/17
    /// <summary>
    /// 一次鍵檔資料分頁事件
    /// </summary>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        BindOneKeyGridView();
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/17
    /// 修改日期：2009/11/17
    /// <summary>
    /// 二次鍵檔資料分頁事件
    /// </summary>
    protected void gpListTwo_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpListTwo.CurrentPageIndex = e.NewPageIndex;
        BindTwoKeyGridView();
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/17
    /// 修改日期：2009/11/17
    /// <summary>
    /// 查詢事件
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        this.gpList.Visible = true;
        this.gpListTwo.Visible = true;
        string strShopNo = "";
        string strShopNoHide = "";
        DataSet dstResult = BRSHOP_6063.SelectInfo(this.txtShopNo.Text.Trim(), "N","1");

        if (dstResult == null)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (dstResult.Tables[0].Rows.Count > 0)
        {
            ViewState["DataBindOneKey"] = dstResult.Tables[0];
            BindOneKeyGridView();
            this.btnAdd.Enabled = true;
        }
        else
        {
            ViewState["DataBindOneKey"] = null;
            this.grvShow.DataSource = null;
            this.grvShow.DataBind();
            this.gpList.Visible = false;
            this.btnAdd.Enabled = false;
            base.strClientMsg += MessageHelper.GetMessage("01_01040203_001");
        }
        this.grvShowTwo.DataSource = null;//*清空【二次鍵檔資料】列表
        //this.gpListTwo.Visible = false;
        this.grvShowTwo.DataBind();
        ViewState["DataBindTwoKey"] = null;

        //*查詢二Key資料  start
        dstResult = BRSHOP_6063.SelectInfo(this.txtShopNo.Text.Trim(), "N", "2");

        if (dstResult == null)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (dstResult.Tables[0].Rows.Count > 0)
        {
            ViewState["DataBindTwoKey"] = dstResult.Tables[0];
            BindTwoKeyGridView();

        }
        else
        {
            ViewState["DataBindTwoKey"] = null;
            this.grvShowTwo.DataSource = null;
            this.grvShowTwo.DataBind();
            this.gpListTwo.Visible = false;
        }

        //*查詢二Key資料  end

        strShopNo = this.txtShopNo.Text.Trim();
        strShopNoHide = this.txtShopNoHide.Text.Trim();
        CommonFunction.SetControlsEnabled(pnlText, true);
        this.txtShopNo.Text = strShopNo;
        this.txtShopNoHide.Text = strShopNoHide;
        
        base.sbRegScript.Append(BaseHelper.SetFocus("txtShopNo"));
    }




    /// 作者 趙呂梁
    /// 創建日期：2009/11/18
    /// 修改日期：2009/11/18
    /// <summary>
    /// 強制事件
    /// </summary>
    protected void btnForce_Click(object sender, EventArgs e)
    {

        for (int i = 0; i < grvShowTwo.Rows.Count; i++)
        {
            if (!UploadHtg("", i))
            {
                break;
            }
        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtShopNo"));
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/18
    /// 修改日期：2009/11/18
    /// <summary>
    /// 新增事件
    /// </summary>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        //*將二Key資料寫入Table

        try
        {
            if (ViewState["DataBindTwoKey"] != null)
            {
                DataTable dtblBind = (DataTable)ViewState["DataBindTwoKey"];
                //*寫入Table  shop_6063
                for (int i = 0; i < dtblBind.Rows.Count; i++)
                {
                    EntitySHOP_6063 eShop = new EntitySHOP_6063();
                    eShop.shop_no = dtblBind.Rows[i][0].ToString();
                    eShop.batch_depict = dtblBind.Rows[i][4].ToString().ToUpper();
                    eShop.card_type = dtblBind.Rows[i][1].ToString().ToUpper();
                    eShop.favour_fee = dtblBind.Rows[i][3].ToString();
                    eShop.identify_no = dtblBind.Rows[i][2].ToString();
                    eShop.first_user = "";
                    eShop.keyin_day = DateTime.Now.ToString("yyyyMMdd");
                    eShop.second_user = eAgentInfo.agent_name;
                    eShop.send3270 = "N";
                    eShop.keyin_flag = "2";
                    eShop.keyin_userID = eAgentInfo.agent_id;
                    try
                    {
                        //*先查詢有無二Key資料
                        DataSet dstResult = BRSHOP_6063.SelectInfo(this.txtShopNo.Text.Trim(), "N", "2", dtblBind.Rows[i][1].ToString().ToUpper(), dtblBind.Rows[i][2].ToString());

                        if (dstResult == null)
                        {
                            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                            return;
                        }
                        if (dstResult.Tables[0].Rows.Count > 0)
                        {
                            //*二Key有資料
                            BRSHOP_6063.UpdateEntity(eShop, dtblBind.Rows[i][0].ToString(), "2", dtblBind.Rows[i][1].ToString().ToUpper(), dtblBind.Rows[i][2].ToString());
                        }
                        else
                        {
                            //*二Key沒有資料
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

            }
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
        }
        //*比對一、二KeyUser是否相同
        if (!CompareUser())
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_00000000_008") + "');");
            return;
        }

        //*比對一、二Key是否相同

        if (Compare())
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01040203_003");
            

            //*循環【二次鍵檔資料】列表中每筆資料上傳主機
            for (int i = 0; i < grvShowTwo.Rows.Count; i++)
            {
                if (!UploadHtg("", i))
                {
                    break;
                }
                this.btnAdd.Enabled = false;
                //*異動成功後異動Table
                if (!BRSHOP_6063.Update2KeyInfo(grvShowTwo.Rows[i].Cells[1].Text, "N",grvShowTwo.Rows[i].Cells[2].Text.ToUpper(), grvShowTwo.Rows[i].Cells[3].Text))
                {
                       ; 
                }
                else
                {
                      ;
                }
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01040203_002");
        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtShopNo"));
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/18
    /// 修改日期：2009/11/18
    /// <summary>
    /// 刪除事件
    /// </summary>
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            //*循環判斷【二次鍵檔資料】列表中選中的資料
            for (int i = 0; i < grvShowTwo.Rows.Count; i++)
            {
                HtmlInputRadioButton radSelect = (HtmlInputRadioButton)grvShowTwo.Rows[i].FindControl("radTwoKey");
                if (radSelect.Checked)
                {
                    DataTable dtblBind = (DataTable)ViewState["DataBindTwoKey"];
                    dtblBind.Rows.RemoveAt(i);
                    BindTwoKeyGridView();
                    break;
                }
            }
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/18
    /// 修改日期：2009/11/18
    /// <summary>
    /// 【BATCH- 描述】欄位按下Enter鍵事件
    /// </summary>
    protected void btnAddHiden_Click(object sender, EventArgs e)
    {

        //*判斷是否一Key已經Key入資料
        DataSet dstResult = BRSHOP_6063.SelectInfo(this.txtShopNo.Text.Trim(), "N", "1", this.txtCardType.Text.Trim().ToUpper(), this.txtIdentifyNo.Text.Trim());
        if (dstResult == null)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }
        if (dstResult.Tables[0].Rows.Count <1)
        {
            //*一Key沒有資料
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_00000000_007") + "');");
            return;
        }

       //*比對一Key和二Key的User是否相同
        if (eAgentInfo.agent_id == dstResult.Tables[0].Rows[0][10].ToString().Trim())
        {
            //相同
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_00000000_008") + "');");
            return;
        }


        string strShopNo = "";
        string strShopNoHide = "";
        this.gpListTwo.Visible = true;
        DataTable dtblBind = (DataTable)ViewState["DataBindTwoKey"];

        if (dtblBind != null)
        {
            //*列表中選擇了資料，移除選中的資料
            for (int i = 0; i < grvShowTwo.Rows.Count; i++)
            {
                HtmlInputRadioButton radSelect = (HtmlInputRadioButton)grvShowTwo.Rows[i].FindControl("radTwoKey");
                if (radSelect.Checked)
                {
                    dtblBind.Rows.RemoveAt(i);
                    break;
                }
            }
        }
        else
        {
            dtblBind = GetDataTable();
        }

        DataRow drowNew = dtblBind.NewRow();
        drowNew[EntitySHOP_6063.M_shop_no] = this.txtShopNo.Text.Trim();
        drowNew[EntitySHOP_6063.M_batch_depict] = this.txtBatchDepict.Text.Trim().ToUpper();
        drowNew[EntitySHOP_6063.M_card_type] = this.txtCardType.Text.Trim().ToUpper();
        drowNew[EntitySHOP_6063.M_favour_fee] = this.txtFavourableFee.Text.Trim();
        drowNew[EntitySHOP_6063.M_identify_no] = this.txtIdentifyNo.Text.Trim();
        drowNew[EntitySHOP_6063.M_keyin_userID] = eAgentInfo.agent_id;
        dtblBind.Rows.Add(drowNew);
        ViewState["DataBindTwoKey"] = dtblBind;
        BindTwoKeyGridView();
        strShopNo = this.txtShopNo.Text.Trim();
        strShopNoHide = this.txtShopNoHide.Text.Trim();
        CommonFunction.SetControlsEnabled(pnlText, true);
        this.txtShopNo.Text = strShopNo;
        this.txtShopNoHide.Text = strShopNoHide;
        base.sbRegScript.Append(BaseHelper.SetFocus("txtCardType"));
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/18
    /// 修改日期：2009/11/18
    /// <summary>
    /// 綁定一次鍵檔資料GridView
    /// </summary>
    private void BindOneKeyGridView()
    {
        try
        {
            DataTable dtblResult = (DataTable)ViewState["DataBindOneKey"];
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
    /// 創建日期：2009/11/18
    /// 修改日期：2009/11/18
    /// <summary>
    /// 綁定二次鍵檔資料GridView
    /// </summary>
    private void BindTwoKeyGridView()
    {
        try
        {
            DataTable dtblResult = (DataTable)ViewState["DataBindTwoKey"];
            this.gpListTwo.RecordCount = dtblResult.Rows.Count;
            this.grvShowTwo.DataSource = CommonFunction.Pagination(dtblResult, this.gpListTwo.CurrentPageIndex, this.gpListTwo.PageSize);
            this.grvShowTwo.DataBind();
            if (dtblResult.Rows.Count == 0)
            {
                gpListTwo.Visible = false;
            }
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
        dtblTemp.Columns.Add(EntitySHOP_6063.M_keyin_userID);
        return dtblTemp;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/17
    /// 修改日期：2009/11/17
    /// <summary>
    /// 設置GridView頁眉
    /// </summary>
    private void SetControlsText()
    {
        grvShow.Columns[1].HeaderText = BaseHelper.GetShowText("01_01040203_002");
        grvShow.Columns[2].HeaderText = BaseHelper.GetShowText("01_01040203_009");
        grvShow.Columns[3].HeaderText = BaseHelper.GetShowText("01_01040203_005");
        grvShow.Columns[4].HeaderText = BaseHelper.GetShowText("01_01040203_010");
        grvShow.Columns[5].HeaderText = BaseHelper.GetShowText("01_01040203_007");

        grvShowTwo.Columns[1].HeaderText = BaseHelper.GetShowText("01_01040203_002");
        grvShowTwo.Columns[2].HeaderText = BaseHelper.GetShowText("01_01040203_009");
        grvShowTwo.Columns[3].HeaderText = BaseHelper.GetShowText("01_01040203_005");
        grvShowTwo.Columns[4].HeaderText = BaseHelper.GetShowText("01_01040203_010");
        grvShowTwo.Columns[5].HeaderText = BaseHelper.GetShowText("01_01040203_007");
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/19
    /// 修改日期：2009/11/19
    /// <summary>
    /// 上傳主機資料
    /// </summary>
    /// <param name="strSend3270">強制執行標識</param>
    /// <param name="intRowIndex">行數</param>
    /// <returns>true成功，false失敗</returns>
    private bool UploadHtg(string strSend3270, int intRowIndex)
    {

        DataTable dtblViewStatus = (DataTable)ViewState["DataBindTwoKey"];
        Hashtable htInput = new Hashtable();

        htInput.Add("FUNCTION_CODE", "A");//*功能別

        //*特店編號
        htInput.Add("MERCH_ACCT", dtblViewStatus.Rows[intRowIndex][0].ToString());

        //*TYPE
        htInput.Add("CARD_TYPE", dtblViewStatus.Rows[intRowIndex][1].ToString());

        //*認同代號
        htInput.Add("AGENT_BANK_NMBR", dtblViewStatus.Rows[intRowIndex][2].ToString());

        //*優惠費率
        int intFee = 0;
        if (dtblViewStatus.Rows[intRowIndex][3].ToString() != "")
        {
            intFee =(int)(float.Parse(grvShowTwo.Rows[intRowIndex].Cells[4].Text)* 1000);
        }
        htInput.Add("DISCOUNT_RATE", intFee.ToString().PadLeft(5, '0'));

        //*Batch-描述
        htInput.Add("AGENT_DESC", dtblViewStatus.Rows[intRowIndex][4].ToString().ToUpper());

        Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGX, htInput, false, "21", eAgentInfo);

        if (!htResult.Contains("HtgMsg"))
        { 
            
            base.strClientMsg += MessageHelper.GetMessage("01_01040203_005") + dtblViewStatus.Rows[intRowIndex][0].ToString();
            base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息;
            return true;
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
            return false;
        }      
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/19
    /// 修改日期：2009/11/19
    /// <summary>
    /// 比較一次鍵檔和二次鍵檔的資料
    /// </summary>
    /// <returns>true相同，false不同</returns>
    private bool Compare()
    {
        bool blnSame = true;
        for (int i = 0; i < grvShowTwo.Rows.Count; i++)
        {
            bool blnRowSame = false;
            for (int j = 0; j < grvShow.Rows.Count; j++)
            {
                if (grvShowTwo.Rows[i].Cells[1].Text.Trim() == grvShow.Rows[j].Cells[1].Text.Trim() && grvShowTwo.Rows[i].Cells[2].Text.Trim().ToUpper() == grvShow.Rows[j].Cells[2].Text.Trim().ToUpper() && grvShowTwo.Rows[i].Cells[3].Text.Trim() == grvShow.Rows[j].Cells[3].Text.Trim() && grvShowTwo.Rows[i].Cells[4].Text.Trim() == grvShow.Rows[j].Cells[4].Text.Trim() && grvShowTwo.Rows[i].Cells[5].Text.Trim().ToUpper() == grvShow.Rows[j].Cells[5].Text.Trim().ToUpper())
                {
                    blnRowSame = true;
                    break;
                }
            }
            if (!blnRowSame)
            {
                grvShowTwo.Rows[i].ForeColor = Color.Red;
                blnSame = false;
            }      
        }
        return blnSame;
    }


    /// 作者 趙呂梁
    /// 創建日期：2009/11/19
    /// 修改日期：2009/11/19
    /// <summary>
    /// 比較一次鍵檔和二次鍵檔的User是否一樣
    /// </summary>
    /// <returns>true相同，false不同</returns>
    private bool CompareUser()
    {
        bool blnSame = true;
        for (int i = 0; i < grvShowTwo.Rows.Count; i++)
        {
            bool blnRowSame = false;
            for (int j = 0; j < grvShow.Rows.Count; j++)
            {
                if (grvShowTwo.Rows[i].Cells[1].Text.Trim() == grvShow.Rows[j].Cells[1].Text.Trim() && grvShowTwo.Rows[i].Cells[2].Text.Trim().ToUpper() == grvShow.Rows[j].Cells[2].Text.Trim().ToUpper() && grvShowTwo.Rows[i].Cells[3].Text.Trim() == grvShow.Rows[j].Cells[3].Text.Trim() && grvShowTwo.Rows[i].Cells[4].Text.Trim() == grvShow.Rows[j].Cells[4].Text.Trim() && grvShowTwo.Rows[i].Cells[5].Text.Trim().ToUpper() == grvShow.Rows[j].Cells[5].Text.Trim().ToUpper())
                {
                    if (grvShowTwo.Rows[i].Cells[6].Text.Trim().ToUpper() == grvShow.Rows[j].Cells[6].Text.Trim().ToUpper())
                    {
                        blnRowSame = true;
                        break;
                    }
                }
            }
            if (blnRowSame)
            {
                grvShowTwo.Rows[i].ForeColor = Color.Red;
                blnSame = false;
            }
        }
        return blnSame;
    }



    /// 作者 趙呂梁
    /// 創建日期：2009/11/19
    /// 修改日期：2009/11/19
    /// <summary>
    /// 設置二次鍵檔資料列表中行的文字設為黑色
    /// </summary>
    private void SetRowsForeColor()
    {
        for (int i = 0; i < grvShowTwo.Rows.Count; i++)
        {
            grvShowTwo.Rows[i].ForeColor = Color.Black;
        }
    }

}
