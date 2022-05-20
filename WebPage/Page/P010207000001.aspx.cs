//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：系統維護-匯入銀行代碼
//*  創建日期：2009/10/22
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
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;
using Framework.Data.OM.Transaction;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using Framework.Common.Logging;
using Framework.Common.Utility;

public partial class P010207000001 : PageBase
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
            SetControlsText();
            this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
            this.grvUpload.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
            this.gpList.Visible = false;
        }
        base.strClientMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/22
    /// 修改日期：2009/10/22
    /// <summary>
    ///分頁顯示
    /// </summary>
    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/23
    /// 修改日期：2009/10/23
    /// <summary>
    /// 匯入事件
    /// </summary>
    protected void btnInsert_Click(object sender, EventArgs e)
    {
        string strPath =  fulFilePath.PostedFile.FileName;//*打開文件路徑
               
        DataTable dtblInsert = GetInsertData(strPath);
        if (dtblInsert != null && dtblInsert.Rows.Count > 0)
        {
            ViewState["DataSource"] = dtblInsert;
            //*綁定匯入數據
            BindGridView();

            //*匯入資料
            if (Insert(dtblInsert))
            {
                //*更新M_PROPERTY_KEY資料庫信息
                BaseHelper.UpdateBankCodeLog(eAgentInfo.agent_id);
                base.strClientMsg += MessageHelper.GetMessage("01_04060000_004");
            }
            else
            {
                base.strClientMsg += MessageHelper.GetMessage("01_04060000_005");
            }
        }
        else
        {
            this.grvUpload.DataSource = null;
            this.gpList.Visible = false;
            this.grvUpload.DataBind();
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/23
    /// 修改日期：2009/10/23
    /// <summary>
    /// 打開事件
    /// </summary>
    protected void btnOpen_Click(object sender, EventArgs e)
    {
        string strPath = fulFilePath.PostedFile.FileName;//*打開文件路徑
        
        DataTable dtblInsert = GetInsertData(strPath);
        if (dtblInsert != null && dtblInsert.Rows.Count > 0)
        {
            ViewState["DataSource"] = dtblInsert;
            //*綁定匯入數據
            BindGridView();
        }
        else
        {
            this.grvUpload.DataSource = null;
            this.gpList.Visible = false;
            this.grvUpload.DataBind();
        }
    }
    #endregion

    #region 方法區

    /// 作者 趙呂梁
    /// 創建日期：2009/10/26
    /// 修改日期：2009/10/26
    /// <summary>
    /// 設置控件文本
    /// </summary>
    private void SetControlsText()
    {
        grvUpload.Columns[0].HeaderText = BaseHelper.GetShowText("01_04060000_005");
        grvUpload.Columns[1].HeaderText = BaseHelper.GetShowText("01_04060000_006");
        grvUpload.Columns[2].HeaderText = BaseHelper.GetShowText("01_04060000_007");
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/26
    /// 修改日期：2009/10/26
    /// <summary>
    /// 得到匯入數據
    /// </summary>
    /// <param name="strPath">路徑</param>
    /// <returns>匯入數據的DataTable</returns>
    private DataTable GetInsertData(string strPath)
    {
        this.gpList.Visible = true;
        this.gpList.CurrentPageIndex = 1;//*設置起始頁
   
        string strFileType = Path.GetExtension(strPath);
        if (strFileType.ToLower() != ".txt")
        {
            base.strClientMsg += MessageHelper.GetMessage("01_04060000_007");
            return null;
        }

        //*錯誤信息ID
        string strMsgID = "";
        
        string strFilePath = FileUpload(fulFilePath.PostedFile, ref strMsgID);
        if (strFilePath == "")
        {
            base.strClientMsg += MessageHelper.GetMessage(strMsgID);
            return null;
        }

        StreamReader streamReader = new StreamReader(strFilePath, System.Text.Encoding.Default);
        DataTable dtblInsert = GetInsertTable();
        int intCount = 0;//*讀取的行數
        try
        {
            while (streamReader.Peek() != -1)
            {
                string strTemp = streamReader.ReadLine().Trim();        
                intCount++;

                if (strTemp.Length >= 34)//(!streamReader.EndOfStream)
                {
                    if (CheckString(strTemp))
                    {

                        DataRow drowInsert = dtblInsert.NewRow();

                        drowInsert[0] = strTemp.Substring(0, 3);//*截取的銀行3碼
                        drowInsert[1] = strTemp.Substring(3, 7);//*截取的銀行7碼

                        char b = (char)30;//*銀行的名稱會用Char(30)、Char(31)兩個特殊字符一頭一尾標示出來
                        char c = (char)31;

                        if (strTemp.IndexOf(b.ToString()) > -1 && strTemp.IndexOf(c.ToString()) > -1)
                        {
                            drowInsert[2] = strTemp.Remove(strTemp.Length - 22).Remove(0, 12).Replace(b.ToString(), " ").Replace(c.ToString(), " ").Trim();
                        }
                        else
                        {
                            drowInsert[2] = strTemp.Substring(12, 10).Trim();
                        }
                        dtblInsert.Rows.Add(drowInsert);
                    }
                    else
                    {
                        base.strClientMsg += String.Format(MessageHelper.GetMessage("01_04060000_003"), intCount);
                        return null;
                    }
                }
                else
                {
                    if (!streamReader.EndOfStream && strTemp.Length != 1)//*判斷是否為最后一行
                    {
                        base.strClientMsg += String.Format(MessageHelper.GetMessage("01_04060000_003"), intCount);
                        return null;
                    }
                }
            }
            return dtblInsert;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += MessageHelper.GetMessage("01_04060000_008");
            return null;
        }
        finally
        {
            streamReader.Close();
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/26
    /// 修改日期：2009/10/26
    /// <summary>
    /// 綁定匯入數據
    /// </summary>
    private void BindGridView()
    {
        DataTable dtblSource = (DataTable)ViewState["DataSource"];

        this.grvUpload.DataSource = CommonFunction.Pagination(dtblSource, this.gpList.CurrentPageIndex, this.gpList.PageSize);
        this.gpList.RecordCount = dtblSource.Rows.Count;
        this.grvUpload.DataBind();
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/26
    /// 修改日期：2009/10/26
    /// <summary>
    /// 匯入
    /// </summary>
    /// <param name="dtblSource">匯入數據表</param>
    /// <returns>rue查詢成功,False查詢失敗</returns>
    private bool Insert(DataTable dtblSource)
    {
        try
        {
            using (OMTransactionScope ts = new OMTransactionScope("Connection_CSIP"))
            {
                if (!BaseHelper.DeleBankCode())
                {
                    return false;
                }

                for (int i = 0; i < dtblSource.Rows.Count; i++)
                {
                    //*匯入銀行7碼
                    if (!BaseHelper.InsertBankCode("16", dtblSource.Rows[i][0].ToString(), dtblSource.Rows[i][1].ToString(), i + 1, eAgentInfo.agent_id))
                    {
                        return false;
                    }

                    //*匯入銀行名稱
                    if (!BaseHelper.InsertBankCode("17", dtblSource.Rows[i][0].ToString(), dtblSource.Rows[i][2].ToString(), i + 1, eAgentInfo.agent_id))
                    {
                        return false;
                    }

                    //*匯入銀行3碼
                    if (!BaseHelper.InsertBankCode("18", dtblSource.Rows[i][0].ToString(), dtblSource.Rows[i][0].ToString(), i + 1, eAgentInfo.agent_id))
                    {
                        return false;
                    }
                }
                ts.Complete();
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/23
    /// 修改日期：2009/10/23
    /// <summary>
    /// 檢核讀取的字符串
    /// </summary>
    /// <param name="strReader">讀取的字符串</param>
    /// <returns>true成功，false失敗</returns>
    private bool CheckString(string strReader)
    {
            Regex reg = new Regex("^[0-9]*$");
            if (!reg.IsMatch(strReader.Substring(0, 10)))
            {
                return false;
            }

            //char b = (char)30;//*銀行的名稱會用Char(30)、Char(31)兩個特殊字符一頭一尾標示出來
            //char c = (char)31;
            //if (strReader.IndexOf(b) < 0 || strReader.IndexOf(c) < 0)
            //{
            //    return false;
            //}
            return true;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/23
    /// 修改日期：2009/10/23
    /// <summary>
    /// 得到匯入的表結構
    /// </summary>
    /// <returns>DataTable</returns>
    private DataTable GetInsertTable()
    {
        DataTable dtblInsert = new DataTable();
        dtblInsert.Columns.Add("BankCodeS");
        dtblInsert.Columns.Add("BankCodeL");
        dtblInsert.Columns.Add("BankName");
        return dtblInsert;
    }
    #endregion   
}
