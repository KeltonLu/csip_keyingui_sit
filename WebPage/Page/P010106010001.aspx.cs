//******************************************************************
//*  作    者：Mars
//*  功能說明：收件(請款簽單總表)檔案匯入
//*  創建日期：2014/07/25
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
using Framework.WebControls;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;
using Framework.Data.OM.Transaction;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using Framework.Common.Logging;
using CSIPKeyInGUI.BusinessRules;

public partial class P010106010001 : PageBase
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
        base.strClientMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
    }

    /// <summary>
    /// 匯入事件
    /// </summary>
    protected void btnInsert_Click(object sender, EventArgs e)
    {
        string strPath = fulFilePath.PostedFile.FileName;//*打開文件路徑
        string strMsgID = string.Empty;

        #region 檔案類型檢查
        string strFileType = Path.GetExtension(strPath);
        if (strFileType.ToLower() != ".xls")
        {
            MessageHelper.ShowMessage(this.UpdatePanel1,"01_01060100_003");
            base.strClientMsg += MessageHelper.GetMessage("01_01060100_003");
            return;
        }
        #endregion

        #region 檔名長度檢查
        string strFileName = Path.GetFileNameWithoutExtension(strPath);
        int iFileNameLength = strFileName.Length;
        if (strFileName.Substring(0, 1).Equals("P"))
        {
            //分期簽單檔名長度應為9碼
            if (iFileNameLength != 9)
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060100_007");
                base.strClientMsg += MessageHelper.GetMessage("01_01060100_007");
                return;
            }
        }
        else
        {
            //一般簽單檔名長度應為8碼
            if (iFileNameLength != 8)
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, "01_01060100_007");
                base.strClientMsg += MessageHelper.GetMessage("01_01060100_007");
                return;
            }
        }
        #endregion

        #region 檔案上傳
        string strFilePath = FileUpload(fulFilePath.PostedFile, ref strMsgID);
        if (strFilePath == "")
        {
            base.strClientMsg += MessageHelper.GetMessage(strMsgID);
            return;
        }
        #endregion

        int total = 0;          //匯入檔總筆數
        int insertcount = 0;    //匯入筆數
        int updatecount = 0;    //更新筆數
        int insertfail = 0;     //匯入失敗筆數
        int updatefail = 0;     //更新失敗筆數
        //2021/03/30_Ares_Stanley-改用NPOI匯入Excel檔
        if (BRArtificial_Signing_Batch_Data.Import_NPOI(strFilePath, eAgentInfo.agent_id, ref strMsgID, 
            ref total, ref insertcount, ref updatecount, ref insertfail, ref updatefail))
        {
            DataTable dtlog = CommonFunction.GetDataTable();
            CommonFunction.UpdateLog("新增筆數：" + insertcount.ToString(), "更新筆數：" + updatecount.ToString(), "", dtlog);
            CommonFunction.InsertCustomerLog(dtlog, eAgentInfo, Path.GetFileNameWithoutExtension(strFilePath), "DB", (structPageInfo)Session["PageInfo"]);
        }

        //匯入訊息
        MessageHelper.ShowMessageWithParms(this.UpdatePanel1, strMsgID, total.ToString(), insertcount.ToString(), updatecount.ToString());
        base.strClientMsg += MessageHelper.GetMessage(strMsgID, total.ToString(), insertcount.ToString(), updatecount.ToString());
    }


    #endregion

    #region 方法區
    
    #endregion   
}
