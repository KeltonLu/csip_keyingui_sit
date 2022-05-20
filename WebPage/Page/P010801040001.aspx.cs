// *****************************************************************
//   作    者：林家賜
//   功能說明：AML資料 分公司資料編輯
//   創建日期：2019/02/03
//   修改記錄：
// <author>            <time>            <TaskID>                <desc>
// ******************************************************************

using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Framework.Common.Utility;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using CSIPNewInvoice.EntityLayer_new;
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.BusinessRules_new;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Common.Logging;
using Framework.WebControls;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using Framework.Data.OM.Transaction;
public partial class Page_P010801040001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    //建立國家字典 檢核用
    /// <summary>
    /// 通用字典，各項次機能以前兩碼區分
    /// </summary>
    Dictionary<string, string> DCCommonColl;
    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        if (!IsPostBack)
        {
            LoadDropDownList();
            //顯示資料，並填至畫面
            showeditDate();
        }


    }


    protected void btnAdd_Click(object sender, EventArgs e)
    {
        string strAlertMsg = "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
        if (sessionOBJ == null)
        {
            string NavigateUrl = "P010801000001.aspx";
            strAlertMsg = MessageHelper.GetMessages("01_01080105_003");
            string urlString = @"alert('" + strAlertMsg + @"');location.href='" + NavigateUrl + "';";
            base.sbRegScript.Append(urlString);
        }

        EntityAML_BRCH_Work_edit editObj = new EntityAML_BRCH_Work_edit();
        //收集頁面資訊
        this.GetVal<EntityAML_BRCH_Work_edit>(ref editObj);
        //賦予ID
        editObj.ID = sessionOBJ.BRCHID;
        //轉換其他資訊
        getOtherInfo(ref editObj);

        List<string> errMsg = new List<string>();
        //連動檢核 
        LinKedValid(editObj, ref errMsg, (Page)this, "");


        //追加驗證
        ValidVal_Nochange<EntityAML_BRCH_Work_edit>(editObj, ref errMsg, (Page)this, "");
        if (errMsg.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            int linC = 0;
            foreach (string oitem in errMsg)
            {
                sb.Append(oitem);
                linC++;
                if (linC > 10)
                {
                    sb.Append("---顯示前十筆，以下省略---");
                    break;
                }
            }
            strAlertMsg = @"『" + MessageHelper.GetMessage("01_01080104_006") + "』" + @"\n" + sb.ToString();
            sbRegScript.Append("alert('" + strAlertMsg + "');");
            return;
        }
        bool result;

        using (OMTransactionScope ts = new OMTransactionScope())
        {
            result = BRAML_BRCH_Work.UpdateAML_BRCH_WOrk(editObj.toShowMode());
            if (!result)
            {
                strAlertMsg = MessageHelper.GetMessages("01_01080104_004");
                sbRegScript.Append("alert('" + strAlertMsg + "');");
                return;
            }
            //修改成功，調整派案
            sessionOBJ.CaseOwner_User = eAgentInfo.agent_id;
            result = BRAML_HQ_Work.Update_Apply(sessionOBJ, "4");
            if (!result)
            {
                strAlertMsg = MessageHelper.GetMessages("01_01080104_007");
                sbRegScript.Append("alert('" + strAlertMsg + "');");
                return;
            }

            //寫入案件歷程
            EntityNoteLog notrLog = new EntityNoteLog();
            notrLog.NL_CASE_NO = sessionOBJ.CASE_NO;
            notrLog.NL_SecondKey = "";
            notrLog.NL_ShowFlag = "1";
            notrLog.NL_DateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            notrLog.NL_User = eAgentInfo.agent_id;
            notrLog.NL_Type = "BranchInfo";
            notrLog.NL_Value = "編輯分公司資料";
            result = BRNoteLog.Insert(notrLog);
            //調整為提示訊息
            if (!result)
            {
                strAlertMsg = MessageHelper.GetMessages("01_01080104_008");
                sbRegScript.Append("alert('" + strAlertMsg + "');");
                return;
            }
            //追加調整排程產出分公司異動檔旗標更新
            result = BRAML_BRCH_Work.Update_BRCH_ExportFileFlagr(sessionOBJ);
            //調整為提示訊息
            if (!result)
            {
                strAlertMsg = MessageHelper.GetMessages("01_01080104_015");
                sbRegScript.Append("alert('" + strAlertMsg + "');"); 
                return;
            }



            ts.Complete();
        }
        strAlertMsg = MessageHelper.GetMessages("01_01080104_003");

        string lastPage = Session["P010801010001_Last"].ToString();
        string urlStringA = @"alert('" + strAlertMsg + @"');location.href='" + lastPage + "';";
        base.sbRegScript.Append(urlStringA);

    }
    /// <summary>
    /// 驗證連動欄位
    /// </summary>
    /// <param name="editObj"></param>
    /// <param name="errMsg"></param>
    /// <param name="page"></param>
    /// <param name="v"></param>
    private void LinKedValid(EntityAML_BRCH_Work_edit editObj, ref List<string> errMsg, Page page, string v)
    {
        ///國籍為TW時，發證日期，發證地點，領補換別不可空白
        if (txtBRCH_NATION.Text.ToUpper() == "TW")
        {
            if (string.IsNullOrEmpty(txtBRCH_OWNER_ID_ISSUE_DATE.Text))
            {
                errMsg.Add(MessageHelper.GetMessages("01_01080104_009") + "\\n");
            }
            if (string.IsNullOrEmpty(txtBRCH_OWNER_ID_ISSUE_PLACE.Text))
            {
                errMsg.Add(MessageHelper.GetMessages("01_01080104_010") + "\\n"); 
            }
            if (string.IsNullOrEmpty(dropHCOP_OWNER_ID_REPLACE_TYPE.SelectedValue))
            {
                errMsg.Add(MessageHelper.GetMessages("01_01080104_011") + "\\n");
            } 
        }
        else
        {
            /*20191218-拿掉效期檢核Ｌ
            //有護照號碼，護照到期日不可空白
            if (!string.IsNullOrEmpty(BrlblBRCH_PASSPORT.Text) && string.IsNullOrEmpty(txtBRCH_PASSPORT_EXP_DATE.Text))
            {
                errMsg.Add(MessageHelper.GetMessages("01_01080104_013") + "\\n");
            }
            //有居留證號，居留證到期日不可空白
            if (!string.IsNullOrEmpty(BrlblBRCH_RESIDENT_NO.Text) && string.IsNullOrEmpty(txtBRCH_RESIDENT_EXP_DATE.Text))
            {
                errMsg.Add(MessageHelper.GetMessages("01_01080104_014") + "\\n");
            }
            */


        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        goLastPage();
    }
    #endregion

    #region 方法區

    /// <summary>
    /// 設定下拉選單
    /// </summary>
    private void LoadDropDownList()
    {
        // 設定 領補換
        SetDropID_REPLACEList("4");
    }

    /// <summary>
    /// 顯示資料，並填至畫面
    /// </summary>
    private void showeditDate()
    {
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
        if (sessionOBJ == null)
        {
            string NavigateUrl = "P010801000001.aspx";
            string urlString = @"alert('" + MessageHelper.GetMessages("01_01080104_011")  + "');location.href='" + NavigateUrl + "';";
            base.sbRegScript.Append(urlString);
        }
        this.SetVal<AML_SessionState>(sessionOBJ, false);

        //讀取SCDD資料，轉換成Edit
        EntityAML_BRCH_Work BrchObj = BRAML_BRCH_Work.getAML_BRCH_WOrk(sessionOBJ);
        EntityAML_BRCH_Work_edit editObj = BrchObj.toEditMode();
        this.SetVal<EntityAML_BRCH_Work_edit>(editObj, false);
        //設定其他資訊
        setOtherInfo(ref editObj);
    }
    /// <summary>
    /// 設定其他資訊
    /// </summary>
    /// <param name="editObj"></param>
    private void setOtherInfo(ref EntityAML_BRCH_Work_edit editObj)
    {
        //有無照片，目前規則為 0有，1無
        if (editObj.BRCH_ID_PHOTO_FLAG == "0")
        {
            radHasPhoto.Checked = true;
        }
        else
        {
            radNoPhoto.Checked = true;
        }
        foreach (ListItem litme in dropHCOP_OWNER_ID_REPLACE_TYPE.Items)
        {
            if (editObj.BRCH_OWNER_ID_REPLACE_TYPE == litme.Value)
            {
                litme.Selected = true;
                break;
            }
        }
        if (editObj.BRCH_ID_SreachStatus == "Y")
        {
            chkSreachStatusY.Checked = true;
        }
        if (editObj.BRCH_ID_SreachStatus == "N")
        {
            chkSreachStatusN.Checked = true;
        }

        //發證日期轉民國年
        if (editObj.BRCH_OWNER_ID_ISSUE_DATE.Length == 8)
        {
            txtBRCH_OWNER_ID_ISSUE_DATE.Text = ConvertToROCYear(txtBRCH_OWNER_ID_ISSUE_DATE.Text);
        }
        ////護照效期轉民國年
        //if (editObj.BRCH_PASSPORT_EXP_DATE.Length == 8)
        //{
        //    txtBRCH_PASSPORT_EXP_DATE.Text = ConvertToROCYear(txtBRCH_PASSPORT_EXP_DATE.Text);
        //}
        ////居留證效期轉民國年
        //if (editObj.BRCH_RESIDENT_EXP_DATE.Length == 8)
        //{
        //    txtBRCH_RESIDENT_EXP_DATE.Text = ConvertToROCYear(txtBRCH_RESIDENT_EXP_DATE.Text);
        //}
        //國籍檢核 
        if (editObj.BRCH_NATION.ToUpper() == "TW")
        {
            LockTW(true);
        }
        else
        {
            LockTW(false);
        }
    }
    /// <summary>
    /// 頁面鎖定國籍為TW時的方法，反之則解鎖
    /// </summary>
    private void LockTW(bool isTw)
    {
        ChangeControlLock("txtBRCH_PASSPORT_EXP_DATE", "CustTextBox", isTw);
        ChangeControlLock("txtBRCH_RESIDENT_EXP_DATE", "CustTextBox", isTw);
        ChangeControlLock("txtBRCH_OWNER_ID_ISSUE_DATE", "CustTextBox", !isTw);
        ChangeControlLock("txtBRCH_OWNER_ID_ISSUE_PLACE", "CustTextBox", !isTw);
        ChangeControlLock("dropHCOP_OWNER_ID_REPLACE_TYPE", "CustDropDownList", !isTw);

    }




    /// <summary>
    /// 轉換其他資訊
    /// </summary>
    /// <param name="editObj"></param>
    private void getOtherInfo(ref EntityAML_BRCH_Work_edit editObj)
    {
        //照片 目前規則為 0 有 1無
        if (radHasPhoto.Checked == true)
        {
            editObj.BRCH_ID_PHOTO_FLAG = "0";
        }
        else
        {
            editObj.BRCH_ID_PHOTO_FLAG = "1";
        }
        //領補換
        editObj.BRCH_OWNER_ID_REPLACE_TYPE = dropHCOP_OWNER_ID_REPLACE_TYPE.SelectedValue;

        //ID換補領查詢結果 可能為空，所以要給預設值空白
        editObj.BRCH_ID_SreachStatus = "";
        //ID適用
        if (chkSreachStatusY.Checked == true)
        {
            editObj.BRCH_ID_SreachStatus = "Y";
        }
        if (chkSreachStatusN.Checked == true)
        {
            editObj.BRCH_ID_SreachStatus = "N";
        }

        //轉回西元年
        if (editObj.BRCH_OWNER_ID_ISSUE_DATE.Length == 7)
        {
            editObj.BRCH_OWNER_ID_ISSUE_DATE = ConvertToDC(editObj.BRCH_OWNER_ID_ISSUE_DATE);
        }
        ////護照效期轉回西元年
        //if (editObj.BRCH_PASSPORT_EXP_DATE.Length == 7)
        //{
        //    editObj.BRCH_PASSPORT_EXP_DATE = ConvertToDC(editObj.BRCH_PASSPORT_EXP_DATE);
        //}
        ////居留證效期轉回西元年
        //if (editObj.BRCH_RESIDENT_EXP_DATE.Length ==7)
        //{
        //    editObj.BRCH_RESIDENT_EXP_DATE = ConvertToDC(editObj.BRCH_RESIDENT_EXP_DATE);
        //}
    }

    // 設定 領補換
    private void SetDropID_REPLACEList(string type)
    {
        DataTable result = BRPostOffice_CodeType.GetCodeType(type);
        string listString = string.Empty;
        ListItem listItem = new ListItem();
        this.dropHCOP_OWNER_ID_REPLACE_TYPE.Items.Add(new ListItem("請選擇", ""));
        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                listItem = new ListItem();

                listItem.Value = result.Rows[i][0].ToString();
                listItem.Text = result.Rows[i][1].ToString();

                //listString += result.Rows[i][0].ToString() + ":";

                dropHCOP_OWNER_ID_REPLACE_TYPE.Items.Add(listItem);
            }
        }
    }
    /// <summary>
    /// 返回上一層
    /// </summary>
    private void goLastPage()
    {
        string lastPage = Session["P010801010001_Last"].ToString();
        //取得上一頁
        if (lastPage != "")
        {
            //20210412_Ares_Stanley-調整轉向方法
            Response.Redirect(lastPage, false);
        }
        else
        {
            //20210412_Ares_Stanley-調整轉向方法
            Response.Redirect("P010801000001.aspx", false);
        }
    }
    #endregion

    /// <summary>
    /// 國籍檢核
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtBRCH_NATION_TextChanged(object sender, EventArgs e)
    {
        string strAlertMsg = "";
        txtBRCH_NATION.Text = txtBRCH_NATION.Text.ToUpper();
        //Talas 20190919 不正確的國籍變色
        CustTextBox mConteol = (CustTextBox)sender;
        mConteol.BackColor = default(Color);
      
        //不正確的國籍
        if (GetDcValue("CT_1_" + txtBRCH_NATION.Text.ToUpper()) == "")
        {
            //Talas 20190919 不正確的國籍變色
            mConteol.BackColor = Color.Red;
            strAlertMsg = MessageHelper.GetMessages("01_01080104_005");
            sbRegScript.Append("alert('" + strAlertMsg + "');");
            return;
        }
        //Talas 20190919 檢核國籍 無效國籍
        DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID("1", mConteol.Text);
        if (result.Rows.Count > 0 && CheckCodeType(result))
        {
        }
        else
        {
            mConteol.BackColor = Color.Red;
        }
        LockTW(txtBRCH_NATION.Text.ToUpper() == "TW");
    }
    private void buiInfoDict()
    {
        DCCommonColl = new Dictionary<string, string>();
        setFromCodeType("1"); //國家

    }
    /// <summary>
    /// 將指定CODETYPE鍵入字典中
    /// </summary>
    /// <param name="codeType"></param>
    private void setFromCodeType(string codeType)
    {
        DataTable result = BRPostOffice_CodeType.GetCodeType(codeType);
        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                string sKey = "CT_" + codeType + "_" + result.Rows[i][0].ToString();
                if (!DCCommonColl.ContainsKey(sKey))
                {
                    DCCommonColl.Add("CT_" + codeType + "_" + result.Rows[i][0].ToString(), result.Rows[i][1].ToString());
                }
            }
        }
    }
    /// <summary>
    /// 取出指定字典的值
    /// </summary>
    /// <param name="inObj"></param>
    /// <param name="inKey"></param>
    /// <returns></returns>
    private string GetDcValue(string inKey)
    {
        if (DCCommonColl == null)
        {
            buiInfoDict();
        }
        string rtnVal = "";
        if (DCCommonColl.ContainsKey(inKey))
        {
            rtnVal = DCCommonColl[inKey];
        }
        return rtnVal;
    }

    private void ChangeControlLock(string controlName, string ctrlType, bool isLock)
    {
        object myControl = FindControl(controlName);
        if (myControl == null) { return; }
        switch (ctrlType)
        {
            case "CustTextBox":
                CustTextBox ct = myControl as CustTextBox;
                if (isLock) //鎖定 值清空，灰色底色
                {
                    ct.Enabled = false;
                    ct.BackColor = Color.LightGray;
                    ct.Text = "";
                }
                else
                {
                    ct.Enabled = true;
                    ct.BackColor = Color.White;
                }
                break;

            case "CustDropDownList":
                CustDropDownList cDr = myControl as CustDropDownList;
                if (isLock) //鎖定 值清空，灰色底色
                {
                    cDr.Enabled = false;
                    cDr.BackColor = Color.LightGray;
                    cDr.SelectedValue = "";
                }
                else
                {
                    cDr.Enabled = true;
                    cDr.BackColor = Color.White;
                }
                break;
            case "CustLabel":
                CustLabel csT = myControl as CustLabel;
                csT.Visible = !isLock;
                break;
        }
    }
    /// <summary>
    /// 限定單選
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void chkSreachStatusY_CheckedChanged(object sender, EventArgs e)
    {
        if (chkSreachStatusY.Checked == true)
        {
            chkSreachStatusN.Checked = false;
        }
    }
    /// <summary>
    /// 限定單選
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void chkSreachStatusN_CheckedChanged(object sender, EventArgs e)
    {
        if (chkSreachStatusN.Checked == true)
        {
            chkSreachStatusY.Checked = false;
        }
    }
    /// <summary>
    /// 依據資料表檢核IsValid欄位
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    private bool CheckCodeType(DataTable dt)
    {
        bool result = true;
        bool? isValid = dt.Rows[0]["IsValid"] as bool?;
        if (isValid == null)
        {
            result = false;
        }
        else
        {
            if (isValid == false)
                result = false;
        }
        return result;
    }
}