// *****************************************************************
//   作    者：陳香琦
//   功能說明：AML資料 分公司資料編輯
//   創建日期：2020/01/02
//   修改記錄：需求單：RQ-2019-030155-002 要求 分公司編輯變成整批編輯
// <author>            <time>            <TaskID>                <desc>
// ******************************************************************

using System;
using System.Data;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using CSIPNewInvoice.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules_new;
using Framework.WebControls;
using System.Drawing;
using System.Text;
using Framework.Data.OM.Transaction;
public partial class Page_P010801040002 : PageBase
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
            //LoadDropDownList();
            //顯示資料，並填至畫面
            showeditDate();
            BindData();
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        DataTable dt = (DataTable)Session["BRCH_WORK"];
        string strAlertMsg = "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
        if (sessionOBJ == null)
        {
            string NavigateUrl = "P010801000001.aspx";
            strAlertMsg = MessageHelper.GetMessages("01_01080105_003");//作業逾時，請重試
            string urlString = @"alert('" + strAlertMsg + @"');location.href='" + NavigateUrl + "';";
            base.sbRegScript.Append(urlString);
        }

        if (grvBRCHData.EditIndex != -1)
        {
            strAlertMsg = MessageHelper.GetMessages("01_01080104_016");
            string urlString = @"alert('" + strAlertMsg + @"');";//資料尚在編輯中，無法存檔
            base.sbRegScript.Append(urlString);
            return;
        }
        EntityAML_BRCH_Work editObj = new EntityAML_BRCH_Work();

        bool result;
        bool UpdateCaseProcessResult;
        using (OMTransactionScope ts = new OMTransactionScope())
        {
            //賦予ID
            foreach (DataRow row in dt.Rows)
            {
                //20200327 用更新flag來判斷是否該更新該筆db資料
                //if (!row["BRCH_ExportFileFlag"].ToString().Trim().Equals("1"))
                if (!row["UP_FLAG"].ToString().Trim().Equals("1"))
                {
                    //break;
                    continue;
                }
                else
                {
                    sessionOBJ.RMMBatchNo = row["BRCH_BATCH_NO"].ToString().Trim();
                    sessionOBJ.AMLInternalID = row["BRCH_INTER_ID"].ToString().Trim();
                    sessionOBJ.BRCHID = row["ID"].ToString().Trim();

                    editObj.ID = sessionOBJ.BRCHID;
                    editObj.BRCH_OWNER_ID_ISSUE_DATE = row["BRCH_OWNER_ID_ISSUE_DATE"].ToString().Trim();//身分證發證日期 
                    editObj.BRCH_OWNER_ID_ISSUE_PLACE = row["BRCH_OWNER_ID_ISSUE_PLACE"].ToString().Trim();//發證地點
                    editObj.BRCH_OWNER_ID_REPLACE_TYPE = row["BRCH_OWNER_ID_REPLACE_TYPE"].ToString().Trim();//領補換類別
                    editObj.BRCH_ID_PHOTO_FLAG = row["BRCH_ID_PHOTO_FLAG"].ToString().Trim();//有無照片
                    editObj.BRCH_NATION = row["BRCH_NATION"].ToString().Trim();//國籍(必填)
                    editObj.BRCH_PASSPORT_EXP_DATE = row["BRCH_PASSPORT_EXP_DATE"].ToString().Trim();//護照效期(西元) 
                    editObj.BRCH_RESIDENT_EXP_DATE = row["BRCH_RESIDENT_EXP_DATE"].ToString().Trim();//居留證效期(西元) 
                    editObj.BRCH_BRCH_NO = row["BRCH_BRCH_NO"].ToString().Trim();
                    editObj.BRCH_ID_SreachStatus = row["BRCH_ID_SreachStatus"].ToString().Trim();
                    editObj.BRCH_BIRTH_DATE = row["BRCH_BIRTH_DATE"].ToString().Trim();//出生日期 
                    editObj.BRCH_ExportFileFlag = row["BRCH_ExportFileFlag"].ToString().Trim();
                    //20210908 by jack 異動分行 = 9999, maker = 登入人ID, checker = 空白
                    editObj.LAST_UPD_BRANCH = row["LAST_UPD_BRANCH"].ToString().Trim();// 異動分行
                    editObj.LAST_UPD_MAKER = row["LAST_UPD_MAKER"].ToString().Trim();// 異動MAKER
                    editObj.LAST_UPD_CHECKER = row["LAST_UPD_CHECKER"].ToString().Trim();// 異動CHECKER

                    result = BRAML_BRCH_Work.UpdateAML_BRCH_WOrk(editObj);
                    if (!result)
                    {
                        strAlertMsg = MessageHelper.GetMessages("01_01080104_004");
                        sbRegScript.Append("alert('" + strAlertMsg + "');");
                        return;
                    }

                    //修改成功，調整派案
                    sessionOBJ.CaseOwner_User = eAgentInfo.agent_id;
                    UpdateCaseProcessResult = BRAML_HQ_Work.Update_Apply(sessionOBJ, "4");
                    if (!UpdateCaseProcessResult)
                    {
                        strAlertMsg = MessageHelper.GetMessages("01_01080104_007");//更新派案資訊失敗
                        sbRegScript.Append("alert('" + strAlertMsg + "');");
                        return;
                    }

                    if (result)
                    {
                        //寫入案件歷程
                        EntityNoteLog notrLog = new EntityNoteLog();
                        notrLog.NL_CASE_NO = sessionOBJ.CASE_NO;

                        //20200325 修正bug
                        //notrLog.NL_SecondKey = "";
                        notrLog.NL_SecondKey = row["BRCH_BRCH_NO"].ToString().Trim();
                        notrLog.NL_ShowFlag = "1";
                        notrLog.NL_DateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                        notrLog.NL_User = eAgentInfo.agent_id;
                        notrLog.NL_Type = "BranchInfo";
                        notrLog.NL_Value = "編輯分公司資料";
                        result = BRNoteLog.Insert(notrLog);
                        //調整為提示訊息
                        if (!result)
                        {
                            strAlertMsg = MessageHelper.GetMessages("01_01080104_008");//寫入案件歷程存檔失敗
                            sbRegScript.Append("alert('" + strAlertMsg + "');");
                            return;
                        }
                    }
                }

                /*20200113 依各別執行的判斷是否有修改
                //追加調整排程產出分公司異動檔旗標更新 
                result = BRAML_BRCH_Work.Update_BRCH_ExportFileFlagr(sessionOBJ);

                //調整為提示訊息
                if (!result)
                {
                    strAlertMsg = MessageHelper.GetMessages("01_01080104_015");//更新排程產出分公司異動檔旗標失敗
                    sbRegScript.Append("alert('" + strAlertMsg + "');");
                    return;
                }
                */
            }

            ts.Complete();
        }

        strAlertMsg = MessageHelper.GetMessages("01_01080104_003");//分公司資料更新完成
        //20200327-更新完原，將異動的session清空
        Session["BRCH_WORK"] = null;

        string lastPage = Session["P010801010001_Last"].ToString();
        string urlStringA = @"alert('" + strAlertMsg + @"');location.href='" + lastPage + "';";
        base.sbRegScript.Append(urlStringA);

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        goLastPage();
    }
    #endregion

    #region 方法區
    /// <summary>
    /// 顯示資料，並填至畫面
    /// </summary>
    private void showeditDate()
    {
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
        if (sessionOBJ == null)
        {
            string NavigateUrl = "P010801000001.aspx";
            string urlString = @"alert('" + MessageHelper.GetMessages("01_01080104_011") + "');location.href='" + NavigateUrl + "';";
            base.sbRegScript.Append(urlString);
        }
        this.SetVal<AML_SessionState>(sessionOBJ, false);

        //讀取分公司明細
        DataTable oDT = new DataTable();
        oDT = BRAML_BRCH_Work.GetCaseNOBrch(sessionOBJ.CASE_NO).Tables[0];
        Session["BRCH_WORK"] = oDT;
    }

    /// <summary>
    /// 頁面鎖定國籍為TW時的方法，反之則解鎖
    /// </summary>
    private void LockTW(bool isTw, int eIndex)
    {
        ChangeControlLock("txtBRCH_PASSPORT_EXP_DATE", "CustTextBox", isTw, eIndex);
        ChangeControlLock("txtBRCH_RESIDENT_EXP_DATE", "CustTextBox", isTw, eIndex);
        ChangeControlLock("txtBRCH_OWNER_ID_ISSUE_DATE", "CustTextBox", !isTw, eIndex);
        ChangeControlLock("txtBRCH_OWNER_ID_ISSUE_PLACE", "CustTextBox", !isTw, eIndex);
        ChangeControlLock("ddlBRCH_OWNER_ID_REPLACE_TYPE", "CustDropDownList", !isTw, eIndex);
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
        CustTextBox mConteol = (CustTextBox)sender;
        GridViewRow row = (GridViewRow)mConteol.Parent.Parent;

        CustTextBox txtNATION = (CustTextBox)row.FindControl("txtBRCH_NATION");
        txtNATION.Text = txtNATION.Text.ToUpper();
        //不正確的國籍變色
        mConteol.BackColor = default(Color);

        string strAlertMsg = "";

        //不正確的國籍
        if (GetDcValue("CT_1_" + mConteol.Text.ToUpper()) == "")
        {
            //Talas 20190919 不正確的國籍變色
            mConteol.BackColor = Color.Red;
            strAlertMsg = MessageHelper.GetMessages("01_01080104_005");//分公司負責人國籍輸入錯誤
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
        LockTW(mConteol.Text.Trim().ToUpper() == "TW", row.DataItemIndex);
    }

    private void buiInfoDict()
    {
        DCCommonColl = new Dictionary<string, string>();
        setFromCodeType("1", "CT_1"); //國家
        DCCommonColl.Add("HSP_0", "有"); //有無照片是反過來的
        DCCommonColl.Add("HSP_1", "無");
        //身分證領補換類別  編碼ID_
        setFromCodeType("4", "ID");
        //ID換補領查詢結果
        DCCommonColl.Add("IDC_Y", "正常");
        DCCommonColl.Add("IDC_N", "不適用");

    }
    /// <summary>
    /// 將指定CODETYPE鍵入字典中
    /// </summary>
    /// <param name="codeType"></param>
    private void setFromCodeType(string codeType, string insKey)
    {
        DataTable result = BRPostOffice_CodeType.GetCodeType(codeType);
        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                string sKey = insKey + "_" + result.Rows[i][0].ToString();
                if (!DCCommonColl.ContainsKey(sKey))
                {
                    DCCommonColl.Add(sKey, result.Rows[i][1].ToString());
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

    private void ChangeControlLock(string controlName, string ctrlType, bool isLock, int eIndex)
    {
        object myControl = this.grvBRCHData.Rows[eIndex].Cells[1].FindControl(controlName);
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
        CheckBox chk = (CheckBox)sender;
        GridViewRow row = (GridViewRow)chk.Parent.Parent;

        CustCheckBox chkY = (CustCheckBox)row.FindControl("chkSreachStatusY");//正常
        CustCheckBox chkN = (CustCheckBox)row.FindControl("chkSreachStatusN");//不適用

        if (chkY.Checked == true)
        {
            chkN.Checked = false;
        }
    }
    /// <summary>
    /// 限定單選
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void chkSreachStatusN_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chk = (CheckBox)sender;
        GridViewRow row = (GridViewRow)chk.Parent.Parent;

        CustCheckBox chkY = (CustCheckBox)row.FindControl("chkSreachStatusY");//正常
        CustCheckBox chkN = (CustCheckBox)row.FindControl("chkSreachStatusN");//不適用

        if (chkN.Checked == true)
        {
            chkY.Checked = false;
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

    protected void grvBRCHData_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //非編輯狀態顯示
        GridViewRow row = (GridViewRow)e.Row;
        if (e.Row.RowType == DataControlRowType.DataRow
            && (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate))
        {
            CustLabel clBRCH_OWNER_ID_REPLACE_TYPE = (CustLabel)row.FindControl("lblBRCH_OWNER_ID_REPLACE_TYPE");//領補換類別
            CustLabel clBRCH_ID_PHOTO_FLAG = (CustLabel)row.FindControl("lblBRCH_ID_PHOTO_FLAG");//有無照片
            CustLabel clBRCH_OWNER_ID_ISSUE_DATE = (CustLabel)row.FindControl("lblBRCH_OWNER_ID_ISSUE_DATE");//身分證發證日期
            CustLabel clBRCH_ID_SreachStatus = (CustLabel)row.FindControl("lblBRCH_ID_SreachStatus");//ID換補領查詢結果

            clBRCH_OWNER_ID_ISSUE_DATE.Text = ConvertToROCYear(clBRCH_OWNER_ID_ISSUE_DATE.Text);
            clBRCH_OWNER_ID_REPLACE_TYPE.Text = GetDcValue("ID_" + clBRCH_OWNER_ID_REPLACE_TYPE.Text.Trim());
            clBRCH_ID_PHOTO_FLAG.Text = GetDcValue("HSP_" + clBRCH_ID_PHOTO_FLAG.Text.Trim());
            clBRCH_ID_SreachStatus.Text = GetDcValue("IDC_" + clBRCH_ID_SreachStatus.Text.Trim());

        }

        //編輯狀態
        if (e.Row.RowState == DataControlRowState.Edit || ((int)e.Row.RowState) == 5)
        {
            CustTextBox ctbBRCH_NATION = (CustTextBox)row.FindControl("txtBRCH_NATION");//國籍
            CustTextBox ctbBRCH_OWNER_ID_ISSUE_DATE = (CustTextBox)row.FindControl("txtBRCH_OWNER_ID_ISSUE_DATE");//身分證發證日期
            ctbBRCH_OWNER_ID_ISSUE_DATE.Text = ConvertToROCYear(ctbBRCH_OWNER_ID_ISSUE_DATE.Text);
            CustTextBox ctbBRCH_OWNER_ID_ISSUE_PLACE = (CustTextBox)row.FindControl("txtBRCH_OWNER_ID_ISSUE_PLACE");//發證地點
            CustLabel clBRCH_OWNER_ID_REPLACE_TYPEE = (CustLabel)row.FindControl("lblBRCH_OWNER_ID_REPLACE_TYPEE");//有無照片
            CustDropDownList cddBRCH_OWNER_ID_REPLACE_TYPE = (CustDropDownList)row.FindControl("ddlBRCH_OWNER_ID_REPLACE_TYPE");//領補換類別
            cddBRCH_OWNER_ID_REPLACE_TYPE.SelectedIndex = cddBRCH_OWNER_ID_REPLACE_TYPE.Items.IndexOf(cddBRCH_OWNER_ID_REPLACE_TYPE.Items.FindByValue(clBRCH_OWNER_ID_REPLACE_TYPEE.Text.Trim()));
            CustTextBox ctbBRCH_PASSPORT_EXP_DATE = (CustTextBox)row.FindControl("txtBRCH_PASSPORT_EXP_DATE");//護照效期
            CustTextBox ctbBRCH_RESIDENT_EXP_DATE = (CustTextBox)row.FindControl("txtBRCH_RESIDENT_EXP_DATE");//居留證效期
            CustLabel clHasPHOTO = (CustLabel)row.FindControl("lblHasPHOTO");//有無照片
            CustRadioButton crbHasPhoto = (CustRadioButton)row.FindControl("radHasPhoto");
            CustRadioButton crbNoPhoto = (CustRadioButton)row.FindControl("radNoPhoto");
            CustLabel lblSreachStatus = (CustLabel)row.FindControl("lblBRCH_ID_SreachStatusE");//ID換補領查詢結果
            CustCheckBox chkY = (CustCheckBox)row.FindControl("chkSreachStatusY");
            CustCheckBox chkN = (CustCheckBox)row.FindControl("chkSreachStatusN");

            CustLabel lblBRCH_PASSPORT = (CustLabel)row.FindControl("lblBRCH_PASSPORT");//護照號碼
            CustLabel lblBRCH_RESIDENT_NO = (CustLabel)row.FindControl("lblBRCH_RESIDENT_NO");//居留證號

            //國籍檢核 

            if (ctbBRCH_NATION.Text.Trim().ToUpper().Equals("TW"))
            {
                //LockTW(true, row.RowIndex);

                ctbBRCH_OWNER_ID_ISSUE_DATE.Enabled = true;
                ctbBRCH_OWNER_ID_ISSUE_PLACE.Enabled = true;
                cddBRCH_OWNER_ID_REPLACE_TYPE.Enabled = true;
                ctbBRCH_PASSPORT_EXP_DATE.Enabled = false;
                ctbBRCH_RESIDENT_EXP_DATE.Enabled = false;
            }
            else
            {
                //LockTW(false, row.RowIndex);

                ctbBRCH_OWNER_ID_ISSUE_DATE.Enabled = false;
                ctbBRCH_OWNER_ID_ISSUE_PLACE.Enabled = false;
                cddBRCH_OWNER_ID_REPLACE_TYPE.Enabled = false;
                
                //20200330
                //如果為國籍為TW，則護照效期/居留證效期不得輸入；反之，若非TW但護照號碼/居留證號為空白亦不得輸入
                //ctbBRCH_PASSPORT_EXP_DATE.Enabled = true;
                //ctbBRCH_RESIDENT_EXP_DATE.Enabled = true;
                if (string.IsNullOrEmpty(lblBRCH_PASSPORT.Text.Trim()))
                {
                    ctbBRCH_PASSPORT_EXP_DATE.Enabled = false;
                }
                else
                {
                    ctbBRCH_PASSPORT_EXP_DATE.Enabled = true;
                }
                if (string.IsNullOrEmpty(lblBRCH_RESIDENT_NO.Text.Trim()))
                {
                    ctbBRCH_RESIDENT_EXP_DATE.Enabled = false;
                }
                else
                {
                    ctbBRCH_RESIDENT_EXP_DATE.Enabled = true;
                }
            }
            
            if (clHasPHOTO.Text.Trim().Equals("0"))
            {
                crbHasPhoto.Checked = true;
            }
            else
            {
                crbNoPhoto.Checked = true;
            }

            if (lblSreachStatus.Text.Trim().Equals("Y"))
            {
                chkY.Checked = true;
            }
            if (lblSreachStatus.Text.Trim().Equals("N"))
            {
                chkN.Checked = true;
            }
        }
    }

    private void BindData()
    {
        grvBRCHData.DataSource = Session["BRCH_WORK"];
        grvBRCHData.DataBind();
    }

    protected void grvBRCHData_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int index = Convert.ToInt32(e.CommandArgument);
        switch (e.CommandName)
        {
            case "Edit2"://編輯
                grvBRCHData.EditIndex = index;
                BindData();
                break;
            case "Update2"://更新
                DataUpdate(index);
                break;
            case "Cancel2"://取消
                grvBRCHData.EditIndex = -1;
                BindData();
                break;
            default:
                break;
        }

        //Bind data to the GridView control.
        //BindData();
    }

    private void DataUpdate(int RowIndex)
    {
        DataTable dt = (DataTable)Session["BRCH_WORK"];

        //Update the values.
        GridViewRow row = grvBRCHData.Rows[RowIndex];

        //20200327 記錄錯誤訊息
        string strAlertMsg = string.Empty;

        #region 連動欄位檢核

        // 檢查出生日期
        //20200327 日期檢核
        if (!string.IsNullOrEmpty(((CustTextBox)(row.Cells[3].Controls[1])).Text.Trim()) && !checkDateTime(((CustTextBox)(row.Cells[3].Controls[1])).Text.Trim(), "V"))
        {
            strAlertMsg += @"『出生日期格式錯誤』\n";
        }

        //身分證發證日期
        //20200327 日期檢核
        if (!string.IsNullOrEmpty(((CustTextBox)(row.Cells[5].Controls[1])).Text.Trim()) && !checkDateTime(((CustTextBox)(row.Cells[5].Controls[1])).Text.Trim(), "C") && ((CustTextBox)(row.Cells[9].Controls[1])).Text.Trim().Equals("TW"))
        {
            strAlertMsg += @"『身分證發證日期格式錯誤』\n";
        }

        //護照效期
        //20200327 日期檢核
        if (!string.IsNullOrEmpty(((CustTextBox)(row.Cells[11].Controls[1])).Text.Trim()) && !checkDateTime(((CustTextBox)(row.Cells[11].Controls[1])).Text.Trim(), "V"))
        {
            strAlertMsg += @"『護照效期格式錯誤』\n";
        }

        //居留證效期
        //20200327 日期檢核
        if (!string.IsNullOrEmpty(((CustTextBox)(row.Cells[13].Controls[1])).Text.Trim()) && !checkDateTime(((CustTextBox)(row.Cells[13].Controls[1])).Text.Trim(), "V"))
        {
            strAlertMsg += @"『統一證號效期格式錯誤』\n";//20200410-RQ-2019-030155-005-居留證號更名為統一證號
        }

        // 20200330
        if (!((CustCheckBox)row.Cells[15].FindControl("chkSreachStatusY")).Checked && !((CustCheckBox)row.Cells[15].FindControl("chkSreachStatusN")).Checked)
        {
            strAlertMsg += @"" + MessageHelper.GetMessage("01_01080103_028")  + @"\n";//『ID換補領查詢結果』未勾選
        }

        //國籍為TW時，發證日期，發證地點，領補換別不可空白        
        if (((CustTextBox)(row.Cells[9].Controls[1])).Text.Trim().ToUpper() == "TW")
        {
            if (string.IsNullOrEmpty(((CustTextBox)(row.Cells[5].Controls[1])).Text))
            {
                strAlertMsg += @"『" + MessageHelper.GetMessage("01_01080104_009") + "』" + @"\n";//發證日期不可空白
                ((CustTextBox)(row.Cells[5].Controls[1])).Focus();
            }
            if (string.IsNullOrEmpty(((CustTextBox)(row.Cells[6].Controls[1])).Text))
            {
                strAlertMsg += @"『" + MessageHelper.GetMessage("01_01080104_010") + "』" + @"\n";//發證地點不可空白
                ((CustTextBox)(row.Cells[6].Controls[1])).Focus();
            }
            if (string.IsNullOrEmpty(((CustDropDownList)(row.Cells[7].Controls[1])).SelectedValue))
            {
                strAlertMsg += @"『" + MessageHelper.GetMessage("01_01080104_011") + "』" + @"\n";//領補換別不可空白
                ((CustDropDownList)(row.Cells[7].Controls[1])).Focus();
            }
        }

        //20220106_Ares_Jack_國籍不得為空
        if (((CustTextBox)(row.Cells[9].Controls[1])).Text.Trim() == "")
        {
            strAlertMsg += @"『 國籍不得為空 』" + @"\n";
            ((CustTextBox)(row.Cells[9].Controls[1])).Focus();
        }
        //20220106_Ares_Jack_國籍不得輸入無
        if (((CustTextBox)(row.Cells[9].Controls[1])).Text.Trim() == "無")
        {
            strAlertMsg += @"『 國籍不得輸入無 』" + @"\n";
            ((CustTextBox)(row.Cells[9].Controls[1])).Focus();
        }

        /*20200401
        if (dt.Rows[row.DataItemIndex]["BRCH_PASSPORT"].ToString().Trim().Equals("")
            && !((CustTextBox)(row.Cells[11].Controls[1])).Text.Trim().Equals(""))
        {
            strAlertMsg += @"『" + MessageHelper.GetMessage("01_01080104_017") + "』" + @"\n";//無護照號碼，護照到期日請留空白
            ((CustTextBox)(row.Cells[11].Controls[1])).Focus();
        }
        if (dt.Rows[row.DataItemIndex]["BRCH_RESIDENT_NO"].ToString().Trim().Equals("")
            && !((CustTextBox)(row.Cells[13].Controls[1])).Text.Trim().Equals(""))
        {
            strAlertMsg += @"『" + MessageHelper.GetMessage("01_01080104_018") + "』" + @"\n";//無居留證號，居留證到期日請留空白
            ((CustTextBox)(row.Cells[13].Controls[1])).Focus();
        }
        */
        #endregion

        if (!string.IsNullOrEmpty(strAlertMsg))
        {
            sbRegScript.Append("alert('" + strAlertMsg + "');");
            
            return;
        }
        else
        {
            #region 更新DataTable Value

            // 檢查出生日期
            string BRCH_BIRTH_DATE = ((CustTextBox)(row.Cells[3].Controls[1])).Text.Trim();
            dt.Rows[row.DataItemIndex]["BRCH_BIRTH_DATE"] = BRCH_BIRTH_DATE.Trim();//出生日期
            dt.Rows[row.DataItemIndex]["BRCH_ExportFileFlag"] = "1"; //更新排程產出分公司異動檔旗標
            dt.Rows[row.DataItemIndex]["UP_FLAG"] = "1"; //20200327-判別是否更新DB資料

            //身分證發證日期        
            string strISSUE_DATE = ((CustTextBox)(row.Cells[5].Controls[1])).Text;
            if (strISSUE_DATE.Trim().Length == 7)//轉回西元年
            {
                dt.Rows[row.DataItemIndex]["BRCH_OWNER_ID_ISSUE_DATE"] = ConvertToDC(strISSUE_DATE);
            }
            else
            {
                dt.Rows[row.DataItemIndex]["BRCH_OWNER_ID_ISSUE_DATE"] = strISSUE_DATE.Trim();
            }

            dt.Rows[row.DataItemIndex]["BRCH_OWNER_ID_ISSUE_PLACE"] = ((CustTextBox)(row.Cells[6].Controls[1])).Text;//發證地點
            dt.Rows[row.DataItemIndex]["BRCH_OWNER_ID_REPLACE_TYPE"] = ((CustDropDownList)(row.Cells[7].Controls[1])).SelectedValue;//領補換類別
            CustRadioButton rbl = (CustRadioButton)row.Cells[8].FindControl("radHasPhoto");
            if (rbl.Checked)
            {
                dt.Rows[row.DataItemIndex]["BRCH_ID_PHOTO_FLAG"] = "0";//有無照片
            }
            else
            {
                dt.Rows[row.DataItemIndex]["BRCH_ID_PHOTO_FLAG"] = "1";//有無照片
            }
            dt.Rows[row.DataItemIndex]["BRCH_NATION"] = ((CustTextBox)(row.Cells[9].Controls[1])).Text;//國籍

            //護照效期
            string strPASSPORT_EXP_DATE = ((CustTextBox)(row.Cells[11].Controls[1])).Text.Trim();
            dt.Rows[row.DataItemIndex]["BRCH_PASSPORT_EXP_DATE"] = strPASSPORT_EXP_DATE.Trim();//護照效期

            //居留證效期
            string strRESIDENT_EXP_DATE = ((CustTextBox)(row.Cells[13].Controls[1])).Text;
            dt.Rows[row.DataItemIndex]["BRCH_RESIDENT_EXP_DATE"] = strRESIDENT_EXP_DATE.Trim();//居留證效期
            CustCheckBox chkY = (CustCheckBox)row.Cells[15].FindControl("chkSreachStatusY");
            CustCheckBox chkN = (CustCheckBox)row.Cells[15].FindControl("chkSreachStatusN");
            if (chkY.Checked)
            {
                dt.Rows[row.DataItemIndex]["BRCH_ID_SreachStatus"] = "Y";//正常
            }
            if (chkN.Checked)
            {
                dt.Rows[row.DataItemIndex]["BRCH_ID_SreachStatus"] = "N";//不適用
            }

            dt.Rows[row.DataItemIndex]["LAST_UPD_BRANCH"] = "9999";//異動分行
            dt.Rows[row.DataItemIndex]["LAST_UPD_MAKER"] = eAgentInfo.agent_id;// 異動MAKER
            dt.Rows[row.DataItemIndex]["LAST_UPD_CHECKER"] = "";//異動CHECKER
            #endregion

            //修改成功後，提示：完成分公司資料編輯後，請按確定鍵進行存檔
            strAlertMsg = MessageHelper.GetMessages("01_01080104_019");
            sbRegScript.Append("alert('" + strAlertMsg + "');");

            dt.AcceptChanges();

            //Reset the edit index.
            grvBRCHData.EditIndex = -1;

            //將異動的datatable內容給session
            Session["BRCH_WORK"] = dt;

            BindData();
        }
    }

}