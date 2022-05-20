// *****************************************************************
//   作    者：Ares Dennis
//   功能說明：自然人收單 別名報送資料維護
//   創建日期：2021/07/13
//   修改記錄：
// <author>            <time>            <TaskID>                <desc>
// ******************************************************************
using CSIPCommonModel.EntityLayer;
using Framework.Common.Logging;
using Framework.Common.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Framework.WebControls;

public partial class P010105060001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    //20191023 修改：SOC所需資訊  by Peggy
    private structPageInfo sPageInfo;//*記錄網頁訊息
    private Dictionary<string, string> compareDC = new Dictionary<string, string>();
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CommonFunction.SetControlsEnabled(pnlText, false);// 清空網頁中所有的輸入欄位  
            txtNewIdNum.Enabled = false;
            txtChineseName.Enabled = false;
        }

        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//20191023 修改：SOC所需資訊  by Peggy
    }

    /// 作者 Ares Dennis
    /// 創建日期：2021/07/13
    /// 修改日期：2021/07/13
    /// <summary>
    /// 查詢事件
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        // 身分證字號必填
        if (string.IsNullOrEmpty(txtIdNum.Text))
        {
            base.sbRegScript.Append("alert('請輸入身分證字號!');$('#txtIdNum').focus();");
            return;
        }

        // 曾經用過的姓名必填
        if (string.IsNullOrEmpty(txtUsedName.Text))
        {
            base.sbRegScript.Append("alert('請輸入曾經用過的姓名!');$('#txtUsedName').focus();");
            return;
        }

        Hashtable htInput = new Hashtable();
        htInput.Add("FUNCTION_CODE", "I");// 查詢
        htInput.Add("ID", txtIdNum.Text.Trim());// 身分證字號  
        htInput.Add("NAME", txtUsedName.Text.Trim()); //曾使用別名

        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC70, htInput, false, "11", eAgentInfo);

        if (!htReturn.Contains("HtgMsg"))
        {
            base.strHostMsg += htReturn["MESSAGE_CHI"].ToString();// 主機返回成功訊息
            ViewState["DataBind"] = ConvertHashtableRowsToDataTableColumns(htReturn);
            ClearPage(true);
            BindGridView();

            this.txtNewIdNum.Enabled = true;
            this.txtChineseName.Enabled = true;
        }
        else
        {
            if (htReturn["MESSAGE_TYPE"] != null)
            {
                ClearPage(false);
                btnAdd.Enabled = false;
                base.sbRegScript.Append(BaseHelper.SetFocus("txtOWNER_ID"));
                base.strHostMsg += MessageHelper.GetMessage("01_01090100_001"); // 無此筆查詢資料，請重新輸入。                                       

            }
            else
            {
                //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
                etMstType = eMstType.Select;
                //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

                base.sbRegScript.Append(BaseHelper.SetFocus("txtOWNER_ID"));
                // 異動主機資料失敗
                if (htReturn["HtgMsgFlag"].ToString() == "0")// 若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htReturn["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
                }
                else
                {
                    base.strClientMsg += htReturn["HtgMsg"].ToString();
                }

                ClearPage(false);
            }
        }
    }

    /// 作者 Ares Dennis
    /// 創建日期：2021/07/13
    /// 修改日期：2021/07/13
    /// <summary>
    /// 新增事件
    /// </summary>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            if (!checkAddData())
            {
                string msg = MessageHelper.GetMessage("01_01090300_003");
                base.sbRegScript.Append("alert('" + msg + "');");
            }
            Hashtable hstExmsP4A = new Hashtable();
            Hashtable hstExmsP4A2 = new Hashtable();
            if (this.gvpbCodeInfo.Rows.Count > 0)
            {
                CustCheckBox delFlag = (CustCheckBox)this.gvpbCodeInfo.Rows[0].Cells[2].FindControl("delFlag");
                if (delFlag.Checked == true)
                {
                    // 刪除
                    Hashtable JC70Obj = new Hashtable();
                    //狀態只有 I (查詢), C (異動)
                    JC70Obj.Add("FUNCTION_CODE", "C");
                    JC70Obj.Add("ID", this.gvpbCodeInfo.Rows[0].Cells[0].Text.Trim());
                    JC70Obj.Add("NAME", this.gvpbCodeInfo.Rows[0].Cells[1].Text.Trim());
                    //別名狀態（Ａ新增，Ｄ取消）
                    JC70Obj.Add("REC_STS", "D");
                    hstExmsP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC70, JC70Obj, false, "11", eAgentInfo);
                }
                if(!string.IsNullOrEmpty(this.txtChineseName.Text.Trim()))
                {
                    //檢查是否有重複別名
                    string isNameRepeatMsg = isNameRepeat();
                    if (isNameRepeatMsg == "查詢成功，請繼續")
                    {
                        string msg = MessageHelper.GetMessage("01_01090300_002");
                        base.sbRegScript.Append("alert('" + msg + "');");
                        return;
                    }
                    if (isNameRepeatMsg == "")
                    {
                        string msg = MessageHelper.GetMessage("01_01090300_004");
                        base.sbRegScript.Append("alert('" + msg + "');");
                        return;
                    }
                    Hashtable JC70Obj2 = new Hashtable();
                    //狀態只有 I (查詢), C (異動)
                    JC70Obj2.Add("FUNCTION_CODE", "C");
                    JC70Obj2.Add("ID", txtNewIdNum.Text);
                    JC70Obj2.Add("NAME", txtChineseName.Text);
                    //別名狀態（Ａ新增，Ｄ取消）
                    JC70Obj2.Add("REC_STS", "A");
                    hstExmsP4A2 = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC70, JC70Obj2, false, "11", eAgentInfo);
                }
            }
            else
            {
                //檢查是否有重複別名
                string isNameRepeatMsg = isNameRepeat();
                if (isNameRepeatMsg == "查詢成功，請繼續")
                {
                    string msg = MessageHelper.GetMessage("01_01090300_002");
                    base.sbRegScript.Append("alert('" + msg + "');");
                    return;
                }
                if (isNameRepeatMsg == "")
                {
                    string msg = MessageHelper.GetMessage("01_01090300_004");
                    base.sbRegScript.Append("alert('" + msg + "');");
                    return;
                }
                // 新增
                Hashtable JC70Obj2 = new Hashtable();
                //狀態只有 I (查詢), C (異動)
                JC70Obj2.Add("FUNCTION_CODE", "C");
                JC70Obj2.Add("ID", txtNewIdNum.Text);
                JC70Obj2.Add("NAME", txtChineseName.Text);
                //別名狀態（Ａ新增，Ｄ取消）
                JC70Obj2.Add("REC_STS", "A");
                hstExmsP4A2 = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC70, JC70Obj2, false, "11", eAgentInfo);
            }

            if (!hstExmsP4A.Contains("HtgMsg"))
            {
                if (hstExmsP4A.Count > 0)
                    base.strHostMsg += hstExmsP4A["MESSAGE_CHI"].ToString();//*主機返回成功訊息
            }
            else
            {
                ClearPage(false);
                base.strHostMsg += hstExmsP4A["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
            }

            if (!hstExmsP4A2.Contains("HtgMsg"))
            {
                if (hstExmsP4A2.Count > 0)
                    base.strHostMsg += hstExmsP4A2["MESSAGE_CHI"].ToString();//*主機返回成功訊息
            }
            else
            {
                ClearPage(false);
                base.strHostMsg += hstExmsP4A2["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
            }
        }
        catch(Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex, LogLayer.UI);
        }
        finally
        {
            if(checkAddData())
                ClearData();
        }
        
    }

    /// 作者 Ares Stanley
    /// 創建日期：2021/10/07
    /// 修改日期：2021/10/07
    /// <summary>
    /// 取消事件
    /// </summary>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        try
        {
            this.txtIdNum.Text = string.Empty;
            this.txtUsedName.Text = string.Empty;
            ClearPage(false);
            ViewState["DataBind"] = null;
            this.gpList.RecordCount = 0;
            this.gvpbCodeInfo.DataSource = null;
            this.gvpbCodeInfo.DataBind();
            base.sbRegScript.Append("$('#txtIdNum').focus();");
        }
        catch(Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex, LogLayer.UI);
        }
    }

    /// 作者 Ares Dennis
    /// 創建日期：2021/07/13
    /// 修改日期：2021/07/13
    /// <summary>
    /// 清空頁面內容
    /// </summary>
    private void ClearPage(bool blnEnabled)
    {
        CommonFunction.SetControlsEnabled(pnlText, blnEnabled);// 清空網頁中所有的輸入欄位                                  
    }

    /// 作者 Ares Stanley
    /// 創建日期：2021/10/07
    /// 修改日期：2021/10/07
    /// <summary>
    /// 清空表格及上方查詢內容
    /// </summary>
    private void ClearData()
    {
        this.txtIdNum.Text = string.Empty;
        this.txtUsedName.Text = string.Empty;
        ClearPage(false);
        ViewState["DataBind"] = null;
        this.gpList.RecordCount = 0;
        this.gvpbCodeInfo.DataSource = null;
        this.gvpbCodeInfo.DataBind();
        base.sbRegScript.Append("$('#txtIdNum').focus();");
    }

    /// 作者 Ares Stanley
    /// 創建日期：2021/10/07
    /// 修改日期：2021/10/07
    /// <summary>
    /// 檢核是否有勾選刪除或填寫新別名
    /// </summary>
    /// <returns>是否有勾選刪除或填寫新別名</returns>
    private bool checkAddData()
    {
        if(this.gvpbCodeInfo.Rows.Count > 0)
        {
            CustCheckBox delFlag = (CustCheckBox)this.gvpbCodeInfo.Rows[0].Cells[2].FindControl("delFlag");
            return !(string.IsNullOrEmpty(this.txtChineseName.Text.Trim()) && delFlag.Checked == false);
        }
        return (!string.IsNullOrEmpty(this.txtChineseName.Text.Trim()));
        }
    /// <summary>
    /// 綁定資料    
    /// </summary>
    private void BindGridView()
    {
        try
        {
            if (ViewState["DataBind"] != null)
            {
                this.txtNewIdNum.Text = this.txtIdNum.Text;
                DataTable dtblResult = (DataTable)ViewState["DataBind"];
                if (dtblResult.Rows.Count > 0 && dtblResult.Rows[0]["MESSAGE_CHI"].ToString().Trim() != "無此筆查詢資料，請重新輸入") 
                {
                    this.gpList.Visible = true;
                    this.gvpbCodeInfo.Visible = true;
                    this.gpList.RecordCount = dtblResult.Rows.Count;
                    this.gvpbCodeInfo.DataSource = CommonFunction.Pagination(dtblResult, this.gpList.CurrentPageIndex, this.gpList.PageSize);
                    this.gvpbCodeInfo.DataBind();
                }
                else
                {
                    string msg = MessageHelper.GetMessage("01_01090300_001");
                    base.sbRegScript.Append("alert('" + msg + "');");
                    ViewState["DataBind"] = null;
                    this.gpList.RecordCount = 0;
                    this.gvpbCodeInfo.DataSource = null;
                    this.gvpbCodeInfo.DataBind();                    
                }
            }
            for (int i = 0; i < gvpbCodeInfo.Columns.Count; i++)
            {
                this.gvpbCodeInfo.Columns[i].HeaderStyle.CssClass = "whiteSpaceNormal";
                this.gvpbCodeInfo.Columns[i].ItemStyle.CssClass = "whiteSpaceNormal";
            }
        }
        catch (Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex, LogLayer.UI);
        }
    }

    private DataTable ConvertHashtableRowsToDataTableColumns(System.Collections.Hashtable ht)
    {
        //create an instance of DataTable
        var dataTable = new DataTable(ht.GetType().Name);
        //specify the table name		
        dataTable.TableName = "TableName";
        //fill the columns in the DataTable
        foreach (DictionaryEntry entry in ht)
        {
            dataTable.Columns.Add(entry.Key.ToString(), typeof(object));
        }
        //create a new DataRow in the DataTable	
        DataRow dr = dataTable.NewRow();
        //fill the new row in the DataTable
        foreach (DictionaryEntry entry in ht)
        {
            dr[entry.Key.ToString()] = entry.Value.ToString();
        }
        //add the filled up row to the DataTable
        dataTable.Rows.Add(dr);
        //return the DataTable
        return dataTable;
    }

    /// 作者 Ares Stanley
    /// 創建日期：2021/10/07
    /// 修改日期：2021/10/07
    /// <summary>
    /// 查詢電文確認是否有相同別名
    /// </summary>
    /// <returns>是否有相同別名</returns>
    private string isNameRepeat()
    {
        try
        {
            Hashtable htInput = new Hashtable();
            htInput.Add("FUNCTION_CODE", "I");// 查詢
            htInput.Add("ID", txtNewIdNum.Text.Trim());// 身分證字號  
            htInput.Add("NAME", txtChineseName.Text.Trim()); //曾使用別名

            Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC70, htInput, false, "11", eAgentInfo);
            if (!htReturn.Contains("HtgMsg"))
            {
                return htReturn["MESSAGE_CHI"].ToString().Trim();
            }
            return "";
        }
        catch(Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex, LogLayer.UI);
            return "";
        }

    }

}
