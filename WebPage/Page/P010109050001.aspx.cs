// *****************************************************************
//   作    者：Ares Dennis
//   功能說明：自然人收單定審維護
//   創建日期：2021/08/06   
// <author>            <time>            <TaskID>                <desc>
// ******************************************************************

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using CSIPNewInvoice.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules_new;
using Framework.Common.Logging;
using Framework.WebControls;
using System.Drawing;
using System.Xml;
using System.IO;
using System.Reflection;
using Framework.Data.OM.OMAttribute;
using System.Text;
using Framework.Data.OM.Transaction;
using CSIPCommonModel.EntityLayer_new;
using Framework.Common.Utility;
using TIBCO.EMS;//NameCheck

public partial class Page_P010109050001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    /// <summary>
    /// 切換測試電文
    /// </summary>
    private bool isTest;
    private DataTable dtSanctionCountry = new DataTable();
    private DataTable dtRiskCountry = new DataTable();
    private DataTable dtGeneralSanctionCountry = new DataTable();
    private List<string> ID = new List<string>();//關聯案件序號ID
    #region  轉換代碼用字典 

    /// <summary>
    /// 通用字典，各項次機能以前兩碼區分
    /// </summary>
    Dictionary<string, string> DCCommonColl;

    #endregion

    //2021/04/07_Ares_Stanley-增加ESB LOG路徑
    private string logPath_ESB = "NameCheckInfoLog";
    private string logPath_ESB_TG = "HtgNameCheckInfoLog";

    private const string thisPageName = "P010109050001.aspx";
    #endregion

    #region 事件區    
    protected void Page_Load(object sender, EventArgs e)
    {
        string isTEST = UtilHelper.GetAppSettings("isTest");
        if (isTEST == "Y") { isTest = true; } else { isTest = false; }

        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
        if (!IsPostBack)
        {
            //高風險國家 Type = 12
            dtRiskCountry = BRPostOffice_CodeType.GetCodeType("12");
            //高度制裁國家 Type = 13
            dtSanctionCountry = BRPostOffice_CodeType.GetCodeType("13");
            //一般制裁國家 Type = 15
            dtGeneralSanctionCountry = BRPostOffice_CodeType.GetCodeType("15");

            //設定本頁使用GRID頁數
            setGridLine();

            LoadDropDownList();
            //顯示資料，並填至畫面
            showeditDate();
        }
    }

    /// <summary>
    /// 分公司負責人資料命令處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvCardData_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Show")
        {
            LinkButton lnkView = (LinkButton)e.CommandSource;
            GridViewRow rowView = (GridViewRow)lnkView.NamingContainer;
            string[] keys = e.CommandArgument.ToString().Split(';');
            AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
            if (keys.Length != 3) //參數錯誤，不處理
            {
                return;
            }
            sessionOBJ.RMMBatchNo = keys[0];
            sessionOBJ.AMLInternalID = keys[1];
            sessionOBJ.BRCHID = keys[2];
            Session["P010801150001_Last"] = thisPageName;
            Session["P010801150001_SESSION"] = sessionOBJ;
            string NavigateUrl = "P010801040001.aspx";

            Response.Redirect(NavigateUrl, false);

        }
    }

    /// <summary>
    /// 存檔
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            string strAlertMsg = "";
            AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
            if (sessionOBJ == null)
            {
                string NavigateUrl = "P010801000001.aspx";
                strAlertMsg = MessageHelper.GetMessages("01_01080103_019");
                string urlString = @"alert('" + strAlertMsg + "');location.href='" + NavigateUrl + "';";
                base.sbRegScript.Append(urlString);
            }

            //取得現有項目 DB，取得更新用的KEY
            EntityAML_HQ_Work_edit DBDataObj = BRAML_HQ_Work.getHQ_WOrk(sessionOBJ).toEditMode();
            //每次都是由電文更新，所以不能由DB讀取
            EntityAML_HQ_Work_edit DataObj = Session["P010801030001_Data"] as EntityAML_HQ_Work_edit;
            DataObj.ID = DBDataObj.ID;
            DataObj.CASE_NO = DBDataObj.CASE_NO;

            string DtCreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string CreateDate = DateTime.Now.ToString("yyyyMMdd");
            string CreateTime = DateTime.Now.ToString("HHmmss");
            string Create_User = eAgentInfo.agent_id;

            this.GetVal<EntityAML_HQ_Work_edit>(ref DataObj);
            //特殊欄位處理，如EMAIL
            CollectSPField(ref DataObj);
            DataObj.Create_Time = CreateTime;
            DataObj.Create_User = Create_User;
            DataObj.Create_Date = CreateDate;
            List<string> errMsg = new List<string>();

            ValidVal<EntityAML_HQ_Work_edit>(DataObj, ref errMsg, (Page)this, "");

            //先驗證關聯欄位，主要是總公司負責人證件資料,追加 聯絡人長姓名判斷
            LinKedValid(DataObj, ref errMsg, (Page)this, "");

            //加入SCDD資料收集，及驗證
            //檢查取回畫面值後公司統編與案號是否為空值，若為空，表示為新增，反之為修改
            EntityHQ_SCDD_edit editObj = BRHQ_SCDD.getSCDDData_WOrk(sessionOBJ).toEditMode();
            this.GetVal<EntityHQ_SCDD_edit>(ref editObj);
            editObj.Organization_Item = "";//組織運作項目
            editObj.Proof_Item = "";//存在證明
            editObj.IsSanction = "";//營業處所是否在高風險或制裁國家
            //20220107_Ares_Jack 欄位增加預設值,預防執行SQL時 Null 引發錯誤
            editObj.BusinessForeignAddress = "";//台灣以外主要之營業處所地址
            editObj.RiskObject = "";//中高風險客戶交易往來對象
            editObj.Organization_Note = "";//組織運作
            editObj.IsSanctionCountryCode1 = "";
            editObj.IsSanctionCountryCode2 = "";
            editObj.IsSanctionCountryCode3 = "";
            editObj.IsSanctionCountryCode4 = "";
            editObj.IsSanctionCountryCode5 = "";
            //特殊欄位處理--DP
            getDPValue(ref editObj);
            //關聯欄位驗證
            LinKedValidSCDD(editObj, ref errMsg, (Page)this, "");

            //20211123_Ares_Jack_修改追加驗證
            ValidVal_NATURAL_PERSON<EntityHQ_SCDD_edit>(editObj, ref errMsg, (Page)this, "");

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
                strAlertMsg = @"『" + MessageHelper.GetMessage("01_01080103_004") + "』" + @"\n" + sb.ToString();
                sbRegScript.Append("alert('" + strAlertMsg + "');");
                return;
            }

            EntityAML_HQ_Work insObj = DataObj.toShowMode();
            bool result;

            using (OMTransactionScope ts = new OMTransactionScope())
            {
                //總公司自然人收單資料寫入
                result = BRAML_HQ_Work.UpdateEdit_Natural(insObj);
                strAlertMsg = MessageHelper.GetMessages("01_01080103_002");
                if (!result)
                {
                    strAlertMsg = MessageHelper.GetMessages("01_01080103_003");
                    sbRegScript.Append("alert('" + strAlertMsg + "');");
                    return;
                }
                //加入SCDD寫DB 
                if (string.IsNullOrEmpty(editObj.CASE_NO))
                { //以檔頭帶入，新增
                    editObj.CASE_NO = sessionOBJ.CASE_NO;
                    editObj.CORP_NO = sessionOBJ.HCOP_HEADQUATERS_CORP_NO;
                    result = BRHQ_SCDD.Insert(editObj.toShowMode());
                }
                else //更新
                {
                    result = BRHQ_SCDD.Update(editObj.toShowMode());
                }
                if (!result)
                {
                    strAlertMsg = MessageHelper.GetMessages("01_01080105_006");//編輯審查維護2更新失敗
                    sbRegScript.Append("alert('" + strAlertMsg + "');");
                    return;
                }

                //修改成功，調整經辦人
                sessionOBJ.CaseOwner_User = eAgentInfo.agent_id;
                result = BRAML_HQ_Work.Update_Apply(sessionOBJ, "4");
                if (!result)
                {
                    strAlertMsg = MessageHelper.GetMessages("01_01080103_006");
                    sbRegScript.Append("alert('" + strAlertMsg + "');");
                    return;

                }
                //轉換物件至電文，準備上送
                Hashtable JC66OBj = buildJC66Hash(DataObj, editObj);
                JC66OBj.Add("FUNCTION_CODE", "C");         //修改 C 新增A

                JC66OBj.Add("LAST_UPDATE_DATE", DateTime.Now.ToString("yyyyMMdd")); //資料最後異動日期

                 //201090625 Talas 追加檢核，若母公司國別為無，則變成空
                if (JC66OBj.ContainsKey("OVERSEAS_FOREIGN_COUNTRY"))
                {
                    if (JC66OBj["OVERSEAS_FOREIGN_COUNTRY"].ToString() == "無")
                    {
                        JC66OBj["OVERSEAS_FOREIGN_COUNTRY"] = "";
                    }
                }

                JC66OBj["COMP_TEL"] = DataObj.HCOP_COMP_TEL;//連絡電話                

                Hashtable hstExmsP4A;
                if (!isTest)
                {
                    //上送主機資料
                    hstExmsP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC66, JC66OBj, false, "11", eAgentInfo);                    
                }
                else
                {
                    //測試用模擬資料
                    hstExmsP4A = BuildTestHashTB();
                    
                    StringBuilder sb = new StringBuilder();
                    foreach (string ekey in JC66OBj.Keys)
                    {
                        sb.AppendLine(ekey + "<!>" + JC66OBj[ekey].ToString());
                    }
                    File.AppendAllText(@"C:\JC66Up.txt", sb.ToString());
                }

                //檢核電文回傳欄位是否成功
                if (hstExmsP4A["MESSAGE_TYPE"].ToString() != "0000")
                {
                    if (hstExmsP4A.ContainsKey("HtgMsg"))
                    {
                        strAlertMsg = hstExmsP4A["HtgMsg"].ToString();
                    }
                    else
                    {
                        strAlertMsg = MessageHelper.GetMessages("01_01080103_005");
                    }
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
                notrLog.NL_Type = "HeadquartersInfo";
                notrLog.NL_Value = "編輯審查維護";
                result = BRNoteLog.Insert(notrLog);
                //調整為提示訊息
                if (!result)
                {
                    strAlertMsg = MessageHelper.GetMessages("01_01080103_007");
                    sbRegScript.Append("alert('" + strAlertMsg + "');");
                    return;
                }

                ts.Complete();
            }

            string lastPage = Session["P010801150001_Last"].ToString();
            string urlStringA = @"alert('" + strAlertMsg + @"');location.href='" + lastPage + "';";
            base.sbRegScript.Append(urlStringA);
        }
        catch (Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex);
            return;
        }
    }
    //產生測試用HSAHTABLE 模擬電文回傳
    private Hashtable BuildTestHashTB()
    {
        Hashtable rtn = new Hashtable();
        rtn.Add("MESSAGE_TYPE", "0000");
        return rtn;
    }
    /// <summary>
    ///  案件歷程聯絡資料綁定處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdNoteLog_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //取得eAgentInfo
            checkUserAgent();
            System.Data.DataRowView rowView = (DataRowView)e.Row.DataItem;
            string idKey = rowView["NL_Type"].ToString();
            string dTime = rowView["NL_DateTime"].ToString();
            e.Row.Cells[3].Text = GetDcValue("CN_" + idKey);
            DateTime Dt = Convert.ToDateTime(dTime);
            e.Row.Cells[0].Text = Dt.ToString("yyyy/MM/dd");
            e.Row.Cells[1].Text = Dt.ToString("HH:mm:ss");            
        }
    }

    /// <summary>
    /// 換頁方法
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpNoteLogList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpNoteLogList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    /// 作者 Ares Jack
    /// 創建日期：2021/11/24
    /// 修改日期：2021/11/24
    /// <summary>
    /// 使用者輸入行業別自動帶出中文名稱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtBasicAMLCC_TextChanged(object sender, EventArgs e)
    {
        setCC_ChName("txtAMLCC", "HQlblHCOP_CC_CNAME", this.txtAMLCC.Text, "1");
        base.sbRegScript.Append(BaseHelper.SetFocus("txtHCOP_CC_2"));// 將行業別2設為輸入焦點
    }
    protected void txtBasicAMLCC2_TextChanged(object sender, EventArgs e)
    {
        setCC_ChName("txtHCOP_CC_2", "HQlblHCOP_CC2_CNAME", this.txtHCOP_CC_2.Text, "2");
        base.sbRegScript.Append(BaseHelper.SetFocus("txtHCOP_CC_3"));// 將行業別3設為輸入焦點
    }
    protected void txtBasicAMLCC3_TextChanged(object sender, EventArgs e)
    {
        setCC_ChName("txtHCOP_CC_3", "HQlblHCOP_CC3_CNAME", this.txtHCOP_CC_3.Text, "3");
        base.sbRegScript.Append(BaseHelper.SetFocus("txtHCOP_OC"));// 將行業別編號1設為輸入焦點
    }
    /// 作者 Ares Jack
    /// 創建日期：2021/11/24
    /// 修改日期：2021/11/24
    /// <summary>
    /// 使用者輸入職稱自動帶出中文名稱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtBasicAMLOC_TextChanged(object sender, EventArgs e)
    {
        setOC_ChName("txtHCOP_OC", "HQlblHCOP_OC_CNAME", this.txtHCOP_OC.Text);
        base.sbRegScript.Append(BaseHelper.SetFocus("chkIncome1"));// 將職稱編號設為輸入焦點
    }
    #endregion

    #region 方法區
    /// <summary>
    /// 驗證連動欄位
    /// </summary>
    /// <param name="editObj"></param>
    /// <param name="errMsg"></param>
    /// <param name="page"></param>
    /// <param name="v"></param>
    private void LinKedValidSCDD(EntityHQ_SCDD_edit editObj, ref List<string> errMsg, Page page, string v)
    {
        string strAlertMsg = "";
        
        //追加看字，若選其他，則NOTE必填
        if (dropNameCheck_Item.SelectedItem.Text == "其他")
        {
            if (string.IsNullOrEmpty(txtNameCheck_Note.Text))
            {
                strAlertMsg = MessageHelper.GetMessages("01_01080105_011");
                errMsg.Add(strAlertMsg + "\\n");
            }
        }        

        //Add 判斷NameCheck訊息顯示為HIT中，則結果不能為請選擇 or 其他
        if (txtNameCheck_Note.Text.IndexOf("HIT中") != -1)
        {
            if (dropNameCheck_Item.SelectedItem.Text == "其他" || dropNameCheck_Item.SelectedItem.Text == "請選擇" || dropNameCheck_Item.SelectedItem.Text == "NC結果")
            {
                strAlertMsg = MessageHelper.GetMessages("01_01080105_015");
                errMsg.Add(strAlertMsg + "\\n");
            }
        }
    }
    /// <summary>
    /// 將畫面DDL值設置到物件
    /// </summary>
    /// <param name="editObj"></param>
    private void getDPValue(ref EntityHQ_SCDD_edit editObj)
    {
        string[] rislk = new string[] { "高", "中", "低" };

        editObj.NameCheck_Item = dropNameCheck_Item.SelectedValue;        

        //調整為看值 2為高風險，3為中風險，其他或結案都為低風險
        if (dropNameCheck_Item.SelectedIndex != -1)
        {            
            switch (dropNameCheck_Item.SelectedValue)
            {
                case "2":
                    editObj.NameCheck_RiskRanking = rislk[0];
                    break;
                case "3":
                    editObj.NameCheck_RiskRanking = rislk[1];
                    break;
                default:
                    editObj.NameCheck_RiskRanking = rislk[2];
                    break;
            }
        }
    }

    /// <summary>
    /// 總公司負責人驗證連動欄位
    /// </summary>
    /// <param name="editObj"></param>
    /// <param name="errMsg"></param>
    /// <param name="page"></param>
    /// <param name="v"></param>
    private void LinKedValid(EntityAML_HQ_Work_edit editObj, ref List<string> errMsg, Page page, string v)
    {
        //20211209_Ares_Jack_行業別編號檢核
        #region 載入行業別編號
        DataTable result = BRPostOffice_CodeType.GetCodeType("3");
        string Industry = string.Empty;
        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                Industry += result.Rows[i][0].ToString() + ":";
            }
            this.hidAllCC.Value = Industry;
        }
        #endregion
        string[] allAMLIndustry = hidAllCC.Value.Split(':');
        if (txtAMLCC.Text.Trim() != "")
        {
            if (Array.IndexOf(allAMLIndustry, txtAMLCC.Text.Trim()) == -1)//行業別編號1錯誤
            {
                errMsg.Add("行業別編號1錯誤\\n");
                txtAMLCC.BackColor = Color.Red;
            }
        }
        if (txtHCOP_CC_2.Text.Trim() != "")
        {
            if (Array.IndexOf(allAMLIndustry, txtHCOP_CC_2.Text.Trim()) == -1)//行業別編號2錯誤
            {
                errMsg.Add("行業別編號2錯誤\\n");
                txtHCOP_CC_2.BackColor = Color.Red;
            }
        }
        if (txtHCOP_CC_3.Text.Trim() != "")
        {
            if (Array.IndexOf(allAMLIndustry, txtHCOP_CC_3.Text.Trim()) == -1)//行業別編號3錯誤
            {
                errMsg.Add("行業別編號3錯誤\\n");
                txtHCOP_CC_3.BackColor = Color.Red;
            }
        }
        if (txtHCOP_OC.Text.Trim() != "")
        {
            if (txtHCOP_OC.Text.Length != 4)
            {
                errMsg.Add("職稱編號請輸入四碼\\n");
                txtHCOP_OC.BackColor = Color.Red;
            }
            else
            {
                DataTable txtHCOP_OC_result = BRPostOffice_CodeType.GetCodeTypeByCodeID("16", txtHCOP_OC.Text.Trim());
                if (!(txtHCOP_OC_result.Rows.Count > 0) )
                {
                    errMsg.Add("職稱編號錯誤\\n");
                    txtHCOP_OC.BackColor = Color.Red;
                }
            }
        }
        else
        {
            errMsg.Add("職稱編號不可空白\\n");
            txtHCOP_OC.BackColor = Color.Red;
        }

        //20211207_Ares_Jack_第二國籍不得為TW
        if (txtHCOP_COUNTRY_CODE_2.Text.Trim().ToUpper() == "TW")
        {
            errMsg.Add("第二國籍不得為TW\\n");
            txtHCOP_COUNTRY_CODE_2.BackColor = Color.Red;
        }
        //20220105_Ares_Jack_國籍2不得選無
        if (this.txtHCOP_COUNTRY_CODE_2.Text.Trim() == "無")
        {
            errMsg.Add("第二國籍不得選無!\\n");
            txtHCOP_COUNTRY_CODE_2.BackColor = Color.Red;
        }
        //20220114_Ares_Jack_國籍二檢核
        #region 載入國籍二
        DataTable resultCOUNTRY_CODE_2 = BRPostOffice_CodeType.GetCodeType("1");
        string countryCode2 = string.Empty;
        if (resultCOUNTRY_CODE_2 != null && resultCOUNTRY_CODE_2.Rows.Count > 0)
        {
            for (int i = 0; i < resultCOUNTRY_CODE_2.Rows.Count; i++)
            {
                countryCode2 += resultCOUNTRY_CODE_2.Rows[i][0].ToString() + ":";
            }
            this.hidCountry2.Value = countryCode2;
        }
        #endregion
        string[] allCOUNTRY_CODE_2 = hidCountry2.Value.Split(':');
        if (Array.IndexOf(allCOUNTRY_CODE_2, txtHCOP_COUNTRY_CODE_2.Text.Trim().ToUpper()) == -1)
        {
            if (!(txtHCOP_COUNTRY_CODE_2.Text.Trim() == "請選擇" || txtHCOP_COUNTRY_CODE_2.Text.Trim() == ""))
            {
                errMsg.Add("第二國籍輸入錯誤!\\n");
                txtHCOP_COUNTRY_CODE_2.BackColor = Color.Red;
            }
        }

        //20211202_Ares_Jack_畫面日期帶西元年
        if (!string.IsNullOrEmpty(txtHCOP_OWNER_BIRTH_DATE.Text.Trim()) && !checkDateTime(txtHCOP_OWNER_BIRTH_DATE.Text.Trim(), "V"))
        {
            errMsg.Add("總公司負責人出生日期格式錯誤\\n");
            txtHCOP_OWNER_BIRTH_DATE.BackColor = Color.Red;
        }
        
        //20210806 EOS_AML(NOVA) 自然人審查維護國籍1帶TW by Ares Dennis
        //發證日期，發證地點，領補換別不可空白
        if (string.IsNullOrEmpty(txtHCOP_OWNER_ID_ISSUE_DATE.Text.Trim()))
        {
            errMsg.Add(MessageHelper.GetMessages("01_01080103_011") + "\\n");
            txtHCOP_OWNER_ID_ISSUE_DATE.BackColor = Color.Red;
        }
        else
        {
            //20200327 日期檢核
            if (!checkDateTime(txtHCOP_OWNER_ID_ISSUE_DATE.Text.Trim(), "C"))
            {
                errMsg.Add("身分證發證日期格式錯誤\\n");
                txtHCOP_OWNER_ID_ISSUE_DATE.BackColor = Color.Red;
            }
        }
        if (string.IsNullOrEmpty(txtHCOP_OWNER_ID_ISSUE_PLACE.Text.Trim()))
        {
            errMsg.Add(MessageHelper.GetMessages("01_01080103_012") + "\\n");
            txtHCOP_OWNER_ID_ISSUE_PLACE.BackColor = Color.Red;
        }
        if (string.IsNullOrEmpty(dropHCOP_OWNER_ID_REPLACE_TYPE.SelectedValue))
        {
            errMsg.Add(MessageHelper.GetMessages("01_01080103_013") + "\\n");
            dropHCOP_OWNER_ID_REPLACE_TYPE.BackColor = Color.Red;
        }
        // 20211216_Ares_Jack_領補換類別檢核
        string id_replaceType = dropHCOP_OWNER_ID_REPLACE_TYPE.SelectedItem.Value.Trim();
        string[] tempList = { "1", "2", "3", "" };//1：初 2：補 3：換
        if (Array.IndexOf(tempList, id_replaceType) == -1)
        {
            errMsg.Add(MessageHelper.GetMessages("01_01080103_035") + "\\n");//領補換類別代碼錯誤
            dropHCOP_OWNER_ID_REPLACE_TYPE.BackColor = Color.Red;
        }

        //20220223_Ares_Jack_有無照片不可空白
        if (string.IsNullOrEmpty(txtHQlblHCOP_ID_PHOTO_FLAG.Text.Trim()) || txtHQlblHCOP_ID_PHOTO_FLAG.Text.Trim() == "請選擇")
        {
            errMsg.Add(MessageHelper.GetMessages("01_01080103_039") + "\\n");
            txtHQlblHCOP_ID_PHOTO_FLAG.BackColor = Color.Red;
        }
        // 20211216_Ares_Jack_有無照片檢核
        string id_photoflag = dropHCOP_ID_PHOTO_FLAG.SelectedItem.Value.Trim();
        string[] tempList2 = { "0", "1", "" };
        if (Array.IndexOf(tempList2, id_photoflag) == -1)
        {
            errMsg.Add(MessageHelper.GetMessages("01_01080103_036") + "\\n");//有無照片代碼錯誤
            dropHCOP_ID_PHOTO_FLAG.BackColor = Color.Red;
        }

        // 20211224_Ares_Jack_性別檢核
        string id_genderflig = dropGENDER.SelectedItem.Value.Trim();
        string[] tempList3 = { "M", "F", "" };
        if (Array.IndexOf(tempList3, id_genderflig) == -1)
        {
            errMsg.Add(MessageHelper.GetMessages("01_01080103_037") + "\\n");//性別代碼錯誤
            dropGENDER.BackColor = Color.Red;
        }

        if (id_genderflig.Trim() == "M")
        {
            if (HQlblHCOP_OWNER_ID.Text.Substring(1, 1).Trim() != "1")
            {
                errMsg.Add(MessageHelper.GetMessages("01_01080103_038") + "\\n");//請確認性別欄位的正確性
                dropGENDER.BackColor = Color.Red;
            }
        }
        if (id_genderflig.Trim() == "F")
        {
            if (HQlblHCOP_OWNER_ID.Text.Substring(1, 1).Trim() != "2")
            {
                errMsg.Add(MessageHelper.GetMessages("01_01080103_038") + "\\n");//請確認性別欄位的正確性
                dropGENDER.BackColor = Color.Red;
            }
        }

        //20220127_Ares_Jack_出生年月日不得為空
        if (this.txtHCOP_OWNER_BIRTH_DATE.Text.Trim() == "00000000")
        {
            errMsg.Add("出生年月日不得為空\\n");
            txtHCOP_OWNER_BIRTH_DATE.BackColor = Color.Red;
        }

    }


    /// <summary>
    /// 實作字典
    /// </summary>
    private void buiInfoDict()
    {
        DCCommonColl = new Dictionary<string, string>();

        DCCommonColl.Add("HSP_0", "有"); //有無照片是反過來的
        DCCommonColl.Add("HSP_1", "無");

        //身分證領補換類別  編碼ID_        
        setFromCodeType("4", "ID");

        //風險等級 RS_
        DCCommonColl.Add("RS_H", "高");
        DCCommonColl.Add("RS_M", "中");
        DCCommonColl.Add("RS_L", "低");

        //聯絡資訊 CN_         
        DCCommonColl.Add("CN_HeadquartersInfo", "編輯審查維護");
        DCCommonColl.Add("CN_BranchInfo", "編輯分公司資料");
        DCCommonColl.Add("CN_HeadquartersSCDD", "編輯審查維護2");
        DCCommonColl.Add("CN_SCDD", "編輯SCDD頁");
        DCCommonColl.Add("CN_EDD1", "編輯非自然人EDD表單");
        DCCommonColl.Add("CN_EDD2", "編輯非營利EDD表單");
        DCCommonColl.Add("CN_Verify", "結案送審(上呈)");
        DCCommonColl.Add("CN_ReturnCase", "案件退回");
        DCCommonColl.Add("CN_CaseOK", "案件放行");
        DCCommonColl.Add("CN_NotCooperate", "不合作註記");
        DCCommonColl.Add("CN_Note", "連絡註記");
        DCCommonColl.Add("CN_BranchEditStatus", "分公司更新狀態");
        DCCommonColl.Add("CN_ReturnOK", "放行(結案)更新狀態");        
        DCCommonColl.Add("CN_NonCooperated", "【不合作結案】送審上呈");
        DCCommonColl.Add("CN_CaseClosed", "【商店解約結案】送審上呈");
        DCCommonColl.Add("CN_OtherClosed", "【其他結案】送審上呈");
        DCCommonColl.Add("CN_RejectNonCooperated", "【不合作結案】案件退回");
        DCCommonColl.Add("CN_RejectCaseClosed", "【商店解約結案】案件退回");
        DCCommonColl.Add("CN_RejectOtherClosed", "【其他結案】案件退回");
        
        setFromCodeType("5", "RN");

        DCCommonColl.Add("CSTYPE_A", "用戶預定的案件");
        DCCommonColl.Add("CSTYPE_B", " 已到達審查日期");
        DCCommonColl.Add("CSTYPE_C", "客戶的風險等級被升級或者被降級");
        DCCommonColl.Add("CSTYPE_D1", "選擇 X 比率（%）的高風險客戶");
        DCCommonColl.Add("CSTYPE_D2", "選擇高風險分數大過或等於 X 的客戶 ");
        DCCommonColl.Add("CSTYPE_D3", "選擇前 X 的高風險客戶");
        
        setFromCodeType("8", "STTYPE");

        //國籍
        setFromCodeType("1", "CT_1");

        //20211208_Ares_Jack_新增欄位商店狀態 開關 OC_ 
        DCCommonColl.Add("OC_OPEN", "O");
        DCCommonColl.Add("OC_Close", "C");
        //20220105_Ares_Jack_定審維護欄位商店狀態 開關 OC_
        DCCommonColl.Add("OC_O", "OPEN");
        DCCommonColl.Add("OC_C", "Close");
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
    /// HQ特殊欄位處理 收集資料
    /// </summary>
    /// <param name="dataObj"></param>
    private void CollectSPField(ref EntityAML_HQ_Work_edit dataObj)
    {
        //20211208_新增欄位 商店狀態
        dataObj.HCOP_STATUS = GetDcValue("OC_" + HQlblHCOP_STATUS.Text);

        //20211203_Ares_Jack_ MAKER = 登入者ID, CHECKER = "", BRANCH = "9999"
        dataObj.HCOP_LAST_UPD_MAKER = eAgentInfo.agent_id;//登入者ID
        dataObj.HCOP_LAST_UPD_CHECKER = "";
        dataObj.HCOP_LAST_UPD_BRANCH = "9999";

        //EMAIL不能輸入空
        if (string.IsNullOrEmpty(txtHCOP_EMAIL.Text))
        {
            base.sbRegScript.Append("alert('Email欄位為必填欄位! ');$('#txtHCOP_EMAIL').focus();");
            return;
        }
        string mailEnd = string.Empty;
        if (this.radGmail.Checked)
        {
            mailEnd = this.radGmail.Text;
        }
        if (this.radYahoo.Checked)
        {
            mailEnd = this.radYahoo.Text;
        }
        if (this.radHotmail.Checked)
        {
            mailEnd = this.radHotmail.Text;
        }
        if (this.radOther.Checked)
        {
            mailEnd = this.txtEmailOther.Text;
        }
        if (!CommonFunction.CheckMailLength(this.txtHCOP_EMAIL.Text, mailEnd))
        {
            base.sbRegScript.Append("alert('Email總長度不可大於50碼');$('#txtEmailFront').focus();");
            return;
        }
        if (txtHCOP_EMAIL.Text.Length > 0)
        {
            dataObj.HCOP_EMAIL = this.txtHCOP_EMAIL.Text + "@" + mailEnd;
        }

        //任職公司
        hidHCOP_NP_COMPANY_NAME.Value = this.HQlblHCOP_NP_COMPANY_NAME.Text;
        dataObj.HCOP_NP_COMPANY_NAME = hidHCOP_NP_COMPANY_NAME.Value;

        //20211130_Ares_Jack_收入及資產來源
        #region 收入及資產來源
        string value = "000000000";
        for (int i = 1; i < 10; i++)
        {
            string ID = "chkIncome" + i.ToString();
            CheckBox checkBox = this.FindControl(ID) as CheckBox;
            if (checkBox.Checked)
            {
                value = value.Remove(i - 1, 1).Insert(i - 1, "1");
            }
        }
        dataObj.HCOP_INCOME_SOURCE = value;
        #endregion

        //給予預設值
        dataObj.OWNER_ID_SreachStatus = "";

        //20211224_Ares_Jack_定審維護的性別欄位要可維護
        string[] tempListGenderCH = { "男", "女", "請選擇" };
        if (Array.IndexOf(tempListGenderCH, txtGENDER.Text.Trim().ToUpper()) != -1)//有找到tempListGenderCH
        {
            switch (txtGENDER.Text.Trim())
            {
                case "男":
                    dropGENDER.SelectedItem.Value = "M";
                    break;
                case "女":
                    dropGENDER.SelectedItem.Value = "F";
                    break;
                case "請選擇":
                    dropGENDER.SelectedItem.Value = "";
                    break;
            }
        }
        else
        {
            string GENDER = txtGENDER.Text.Trim().ToUpper();//直接輸入代號
            dropGENDER.SelectedItem.Value = GENDER;
        }
        dataObj.HCOP_GENDER = dropGENDER.SelectedItem.Value;

        //20211216_Ares_Jack_有無照片 特殊規則 0 有照片，1無照片
        string[] tempList = { "有", "無", "請選擇" };
        if (Array.IndexOf(tempList, txtHQlblHCOP_ID_PHOTO_FLAG.Text.Trim()) != -1)//有找到tempList
        {
            switch (txtHQlblHCOP_ID_PHOTO_FLAG.Text.Trim())
            {
                case "有":
                    dropHCOP_ID_PHOTO_FLAG.SelectedItem.Value = "0";
                    break;
                case "無":
                    dropHCOP_ID_PHOTO_FLAG.SelectedItem.Value = "1";
                    break;
                case "請選擇":
                    dropHCOP_ID_PHOTO_FLAG.SelectedItem.Value = "";
                    break;
            }
        }
        else
        {
            dropHCOP_ID_PHOTO_FLAG.SelectedItem.Value = txtHQlblHCOP_ID_PHOTO_FLAG.Text.Trim();
        }
        dataObj.HCOP_ID_PHOTO_FLAG = dropHCOP_ID_PHOTO_FLAG.SelectedItem.Value;
        //20211216_Ares_Jack_領補換類別
        string[] tempList2 = { "初", "補", "換", "請選擇" };
        if (Array.IndexOf(tempList2, txtHQlblHCOP_OWNER_ID_REPLACE_TYPE.Text.Trim()) != -1)//有找到tempList2
        {
            switch (txtHQlblHCOP_OWNER_ID_REPLACE_TYPE.Text.Trim())
            {
                case "初":
                    dropHCOP_OWNER_ID_REPLACE_TYPE.SelectedItem.Value = "1";
                    break;
                case "補":
                    dropHCOP_OWNER_ID_REPLACE_TYPE.SelectedItem.Value = "2";
                    break;
                case "換":
                    dropHCOP_OWNER_ID_REPLACE_TYPE.SelectedItem.Value = "3";
                    break;
                case "請選擇":
                    dropHCOP_OWNER_ID_REPLACE_TYPE.SelectedItem.Value = "";
                    break;
            }
        }
        else
        {
            dropHCOP_OWNER_ID_REPLACE_TYPE.SelectedItem.Value = txtHQlblHCOP_OWNER_ID_REPLACE_TYPE.Text.Trim();
        }
        dataObj.HCOP_OWNER_ID_REPLACE_TYPE = dropHCOP_OWNER_ID_REPLACE_TYPE.SelectedItem.Value;

        //連絡電話 //20211208_Ares_Jack_第三欄為空的話串接的 "-" 一併移除 
        string COMP_TEL_3 = ToNarrow(LongNameRomaClean(this.txtContactTel3.Text.Trim())) == "" ? "" : "-" + ToNarrow(LongNameRomaClean(this.txtContactTel3.Text.Trim()));
        hidHCOP_CONTACT_TEL.Value = txtContactTel1.Text.Trim() + "-" + txtContactTel2.Text.Trim() + COMP_TEL_3.Trim();
        dataObj.HCOP_COMP_TEL = hidHCOP_CONTACT_TEL.Value.Trim();

        //20211202_Ares_Jack_身分證發證日期轉回西元年
        if (dataObj.HCOP_OWNER_ID_ISSUE_DATE.Length == 7)
        {
            dataObj.HCOP_OWNER_ID_ISSUE_DATE = ConvertToDC(dataObj.HCOP_OWNER_ID_ISSUE_DATE);
        }
        //20211202_Ares_Jack_出生年月日轉回西元年
        if (dataObj.HCOP_OWNER_BIRTH_DATE.Length == 7)
        {
            dataObj.HCOP_OWNER_BIRTH_DATE = ConvertToDC(dataObj.HCOP_OWNER_BIRTH_DATE);
        }
        //20211213_Ares_Jack_ DB通訊地址第三欄為 x 就清除
        dataObj.HCOP_MAILING_ADDR2 = LongNameRomaClean(txtHCOP_MAILING_ADDR2.Text.Trim());

        if (this.txtHCOP_COUNTRY_CODE_2.Text == "請選擇")
            dataObj.HCOP_COUNTRY_CODE_2 = "";
    }
    /// <summary>
    /// HQ特殊欄位處理
    /// </summary>
    /// <param name="edObj"></param>
    private void setHQSpcial(EntityAML_HQ_Work_edit edObj)
    {
        if (edObj == null)
        {
            return;
        }
        //客戶ID
        HQlblHCOP_HEADQUATERS_CORP_NO.Text = edObj.HCOP_HEADQUATERS_CORP_NO + edObj.HCOP_HEADQUATERS_CORP_SEQ;

        //E-MAIL
        if (!string.IsNullOrEmpty(edObj.HCOP_EMAIL))
        {
            SetEmailValue(edObj.HCOP_EMAIL);
        }

        //戶籍地址
        hidHCOP_REG_ZIP_CODE.Value = edObj.HCOP_REG_ZIP_CODE.Trim();
        HQlblHCOP_REG_ZIP_CODE.Text = hidHCOP_REG_ZIP_CODE.Value;
        hidHCOP_REG_CITY.Value = edObj.HCOP_REG_CITY.Trim();
        HQlblHCOP_REG_CITY.Text = hidHCOP_REG_CITY.Value;
        hidHCOP_REG_ADDR1.Value = edObj.HCOP_REG_ADDR1.Trim();
        HQlblHCOP_REG_ADDR1.Text = hidHCOP_REG_ADDR1.Value;
        hidHCOP_REG_ADDR2.Value = edObj.HCOP_REG_ADDR2.Trim();
        HQlblHCOP_REG_ADDR2.Text = hidHCOP_REG_ADDR2.Value;

        //20211125_Ares_Jack_帶出行業別編號, 職稱編號的中文名稱
        setCC_ChName("txtAMLCC", "HQlblHCOP_CC_CNAME", edObj.HCOP_CC.Trim(), "1");
        setCC_ChName("txtHCOP_CC_2", "HQlblHCOP_CC2_CNAME", edObj.HCOP_CC_2.Trim(), "2");
        setCC_ChName("txtHCOP_CC_3", "HQlblHCOP_CC3_CNAME", edObj.HCOP_CC_3.Trim(), "3");
        setOC_ChName("txtHCOP_OC", "HQlblHCOP_OC_CNAME", edObj.HCOP_OC.Trim());

        //收入及資產來源
        if (!string.IsNullOrEmpty(edObj.HCOP_INCOME_SOURCE))
        {
            string incomeSource = edObj.HCOP_INCOME_SOURCE;
            for (int i = 1; i < 10; i++)
            {
                if (incomeSource.IndexOf('1', i - 1, 1) != -1)//被勾選的是1
                {
                    incomeSource = incomeSource.Remove(i - 1, 1).Insert(i - 1, i.ToString());// ex:101000000 => 1,3
                }
            }
            incomeSource = string.Join(",", incomeSource.Replace("0", "").ToCharArray());//清除0,剩餘的用逗號分開 ex:1,2,3

            string[] incomes = incomeSource.Split(',');
            foreach (string income in incomes)
            {
                for (int i = 1; i <= 9; i++)
                {
                    if (income == i.ToString())
                    {
                        CheckBox checkBox = (CheckBox)FindControl("chkIncome" + i.ToString());
                        checkBox.Checked = true;
                    }
                }
            }
        }

        //性別
        if (!string.IsNullOrEmpty(edObj.HCOP_GENDER))
        {
            //20211018_Ares_Jack_自然人收單 因為性別Value由 M F 代表 所以 HCOP_GENDER 加上判斷 M F
            if (edObj.HCOP_GENDER == "M")
            {
                //男
                this.txtGENDER.Text = "男";
                this.dropGENDER.SelectedValue = "M";
            }
            else if (edObj.HCOP_GENDER == "F")
            {
                //女
                this.txtGENDER.Text = "女";
                this.dropGENDER.SelectedValue = "F";
            }
            else
            {
                //請選擇
                this.txtGENDER.Text = "請選擇";
                this.dropGENDER.SelectedValue = "";
            }
        }

        //有無照片
        if (!string.IsNullOrEmpty(edObj.HCOP_ID_PHOTO_FLAG))
        {
            txtHQlblHCOP_ID_PHOTO_FLAG.Text = GetDcValue("HSP_" + edObj.HCOP_ID_PHOTO_FLAG);
        }

        //領補換類別
        if (!string.IsNullOrEmpty(edObj.HCOP_OWNER_ID_REPLACE_TYPE))
        {
            txtHQlblHCOP_OWNER_ID_REPLACE_TYPE.Text = GetDcValue("ID_" + edObj.HCOP_OWNER_ID_REPLACE_TYPE);
        }

        if (dtRiskCountry != null && dtRiskCountry.Rows.Count > 0)
        {
            //註冊國籍	AML_HQ_Work.HCOP_REGISTER_NATION
            if (!String.IsNullOrEmpty(edObj.HCOP_REGISTER_NATION))
            {
                //客戶 國籍、戶籍地 或 通訊地 或關聯人任一人 國籍 位於位於高風險國家/地區
                if (dtRiskCountry.Select("CODE_ID = '" + edObj.HCOP_REGISTER_NATION + "'").Length > 0)
                {
                    lblIsRisk.Text = "Y";
                }
                else
                {
                    lblIsRisk.Text = "N";
                }

                //客戶 國籍、戶籍地 或 通訊地 位於一般或高度制裁國家/ 地區
                if (dtGeneralSanctionCountry.Select("CODE_ID = '" + edObj.HCOP_REGISTER_NATION + "'").Length > 0 ||
                    dtSanctionCountry.Select("CODE_ID = '" + edObj.HCOP_REGISTER_NATION + "'").Length > 0)
                {
                    lblIsSanction.Text = "Y";
                }
                else
                {
                    lblIsSanction.Text = "N";
                }
            }
            //20220111_Ares_Jack_加上國籍2判斷風險等級
            if (!String.IsNullOrEmpty(edObj.HCOP_COUNTRY_CODE_2))
            {
                //客戶 國籍、戶籍地 或 通訊地 或關聯人任一人 國籍 位於位於高風險國家/地區
                if (dtRiskCountry.Select("CODE_ID = '" + edObj.HCOP_COUNTRY_CODE_2 + "'").Length > 0)
                {
                    lblIsRisk.Text = "Y";
                }

                //客戶 國籍、戶籍地 或 通訊地 位於一般或高度制裁國家/ 地區
                if (dtGeneralSanctionCountry.Select("CODE_ID = '" + edObj.HCOP_COUNTRY_CODE_2 + "'").Length > 0 ||
                    dtSanctionCountry.Select("CODE_ID = '" + edObj.HCOP_COUNTRY_CODE_2 + "'").Length > 0)
                {
                    lblIsSanction.Text = "Y";
                }
            }
        }

        //因其他頁面已使用 EntityAML_HQ_Work_edit 中的 hidHCOP_NP_COMPANY_NAME 欄位自動代值
        //任職公司頁面資料從 hidHCOP_NP_COMPANY_NAME 取得
        HQlblHCOP_NP_COMPANY_NAME.Text = hidHCOP_NP_COMPANY_NAME.Value;

        //20220113_Ares_Jack_主機定義此欄位不會是空白, 修改為8個0時可編輯
        txtHCOP_OWNER_BIRTH_DATE.Enabled = edObj.HCOP_OWNER_BIRTH_DATE == "00000000";

        //連絡電話
        //調整連絡電話帶值, 避免空值造成錯誤 by Ares Stanley 20211203
        //20211207_Ares_Jack_連絡電話 COMP_TEL
        if (edObj.HCOP_COMP_TEL.Trim() != "")
        {
            hidHCOP_CONTACT_TEL.Value = edObj.HCOP_COMP_TEL.Trim();
            string[] txtContactTel = hidHCOP_CONTACT_TEL.Value.Split('-');
            switch (txtContactTel.Length)
            {
                case 1:
                    txtContactTel1.Text = txtContactTel[0];
                    break;
                case 2:
                    txtContactTel1.Text = txtContactTel[0];
                    txtContactTel2.Text = txtContactTel[1];
                    break;
                case 3:
                    txtContactTel1.Text = txtContactTel[0];
                    txtContactTel2.Text = txtContactTel[1];
                    txtContactTel3.Text = txtContactTel[2];
                    break;
            }
        }

        //20211210_Ares_Jack_畫面身分證發證日期轉民國年
        if (edObj.HCOP_OWNER_ID_ISSUE_DATE.Length == 8)
        {
            txtHCOP_OWNER_ID_ISSUE_DATE.Text = ConvertToROCYear(txtHCOP_OWNER_ID_ISSUE_DATE.Text);
        }
        //20211202_Ares_Jack_畫面出生年月日轉民國年
        //畫面出生年月日維持西元年 by Ares Stanley 20211203
        if (edObj.HCOP_OWNER_BIRTH_DATE.Length == 8)
        {
            txtHCOP_OWNER_BIRTH_DATE.Text = txtHCOP_OWNER_BIRTH_DATE.Text;
        }

        //建立顯示用字典
        buiInfoDict();

        //商店狀態 //20211208_Ares_Jack_新增欄位
        HQlblHCOP_STATUS.Text = GetDcValue("OC_" + edObj.HCOP_STATUS);        
    }

    /// <summary>
    /// 填入Email到畫面
    /// </summary>
    /// <param name="email"></param>
    private void SetEmailValue(string email)
    {
        string[] emailArr = email.Split('@');
        if (emailArr.Length > 1)
        {
            this.txtHCOP_EMAIL.Text = emailArr[0];
            switch (emailArr[1].ToLower())
            {
                case "gmail.com":
                    this.radGmail.Checked = true;
                    this.txtEmailOther.Text = "";
                    break;
                case "yahoo.com.tw":
                    this.radYahoo.Checked = true;
                    this.txtEmailOther.Text = "";
                    break;
                case "hotmail.com":
                    this.radHotmail.Checked = true;
                    this.txtEmailOther.Text = "";
                    break;
                default:
                    this.radOther.Checked = true;
                    this.txtEmailOther.Text = emailArr[1];
                    break;
            }
        }
    }


    /// <summary>
    /// 載入下拉選單內容
    /// </summary>
    private void LoadDropDownList()
    {
        //設定 國籍2
        SetDropCode("1", dropCountry2, true);        
        //設定 領補換類別
        SetDropCode("4", dropHCOP_OWNER_ID_REPLACE_TYPE, true);
        //設定 風險物件
        SetDropNameChk("5");
        //設定有無照片
        SetDropPhotoFlag();
        #region 性別
        ListItem listItem = new ListItem();
        listItem.Value = "";
        listItem.Text = "請選擇";
        this.dropGENDER.Items.Add(listItem);
        listItem = new ListItem();
        listItem.Value = "M";
        listItem.Text = "男";
        this.dropGENDER.Items.Add(listItem);
        listItem = new ListItem();
        listItem.Value = "F";
        listItem.Text = "女";
        this.dropGENDER.Items.Add(listItem);
        this.dropGENDER.SelectedValue = "";
        this.txtGENDER.Text = "請選擇";
        #endregion
    }
    /// <summary>
    /// 設定 風險物件
    /// </summary>
    /// <param name="type"></param>
    private void SetDropNameChk(string type)
    {
        DataTable result = BRPostOffice_CodeType.GetCodeType(type);
        ListItem listItem = new ListItem();
        string listString = string.Empty;
        this.dropNameCheck_Item.Items.Add(new ListItem("請選擇", ""));
        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                listItem = new ListItem();

                listItem.Value = result.Rows[i][0].ToString();
                listItem.Text = result.Rows[i][1].ToString();
                this.dropNameCheck_Item.Items.Add(listItem);
            }
        }
    }


    //設定有無照片DropDownList
    private void SetDropPhotoFlag()
    {
        ListItem listItem = new ListItem();
        string listString = string.Empty;

        string[] arr = { "有;0", "無;1" };//有無照片是反過來的
        string[] arrs = null;
        dropHCOP_ID_PHOTO_FLAG.Items.Add(new ListItem("請選擇", ""));
        for (int i = 0; i < arr.Length; i++)
        {
            arrs = arr[i].Split(';');
            listItem = new ListItem();
            if (arrs.Length == 2)
            {
                listItem.Value = arrs[1].ToString();
                listItem.Text = arrs[0].ToString();
                dropHCOP_ID_PHOTO_FLAG.Items.Add(listItem);
            }
        }
    }


    //設定DropDownList
    private void SetDropCode(string code, CustDropDownList dropItem, bool isShowKey)
    {
        ListItem listItem = new ListItem();
        string listString = string.Empty;
        listItem.Text = "請選擇";
        listItem.Value = "";
        DataTable result = BRPostOffice_CodeType.GetCodeType(code);
        dropItem.Items.Add(listItem);
        for (int i = 0; i < result.Rows.Count; i++)
        {
            listItem = new ListItem();
            listItem.Value = result.Rows[i][0].ToString();
            if (isShowKey)
            {
                if (code == "1")
                    listItem.Text = result.Rows[i][0].ToString();//國籍2
                else
                    listItem.Text = result.Rows[i][1].ToString();
            }
            else
            {
                listItem.Text = result.Rows[i][0].ToString();

            }
            dropItem.Items.Add(listItem);
        }

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
                DataTable dtblResult = (DataTable)ViewState["DataBind"];
                if (dtblResult.Rows.Count > 0)
                {
                    this.gpNoteLogList.Visible = true;
                    this.grdNoteLog.Visible = true;
                    this.gpNoteLogList.RecordCount = dtblResult.Rows.Count;
                    this.grdNoteLog.DataSource = CommonFunction.Pagination(dtblResult, this.gpNoteLogList.CurrentPageIndex, this.gpNoteLogList.PageSize);
                    this.grdNoteLog.DataBind();
                }
                else
                {
                    this.grdNoteLog.DataSource = CommonFunction.Pagination(dtblResult, this.gpNoteLogList.CurrentPageIndex, this.gpNoteLogList.PageSize);
                    this.grdNoteLog.DataBind();
                    this.gpNoteLogList.Visible = false;
                    this.grdNoteLog.Visible = false;
                }
            }
        }
        catch (Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex, LogLayer.UI);
        }
    }

    int grvCardDataSubRowCount = 0;
    /// <summary>
    /// 分公司負責人資料綁定處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvCardData_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {            
            //用LIST進來，就不適用 DataRowView 必須用原型別操作資料
            EntityAML_BRCH_Work rowView = (EntityAML_BRCH_Work)e.Row.DataItem;
            //轉換民國年
            string idDate = rowView.BRCH_OWNER_ID_ISSUE_DATE;            

            //長姓名區            
            if (string.IsNullOrEmpty(rowView.BRCH_CHINESE_LNAME) == false || string.IsNullOrEmpty(rowView.BRCH_ROMA) == false)
            {
                ControlCollection _table = e.Row.Parent.Controls; //e.Row.Parent就是Table
                int _columnSpan = 2;
                GridViewRow gvr = new GridViewRow(-1, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
                TableCell cell_label = new TableCell();
                TableCell cell_value = new TableCell();
                cell_label.ColumnSpan = 1;
                cell_label.Text = BaseHelper.GetShowText("01_01080103_059");
                cell_value.ColumnSpan = _columnSpan;
                cell_value.Text = rowView.BRCH_CHINESE_LNAME;
                cell_label.HorizontalAlign = HorizontalAlign.Center;
                gvr.Cells.Add(cell_label);
                gvr.Cells.Add(cell_value);
                grvCardDataSubRowCount += 2;

                _table.AddAt(grvCardDataSubRowCount, gvr);

                gvr = new GridViewRow(-1, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
                cell_label = new TableCell();
                cell_value = new TableCell();
                cell_label.HorizontalAlign = HorizontalAlign.Center;
                cell_label.Text = BaseHelper.GetShowText("01_01080103_060");
                cell_label.ColumnSpan = 1;
                cell_value.ColumnSpan = _columnSpan;
                cell_value.Text = rowView.BRCH_ROMA;
                gvr.Cells.Add(cell_label);
                gvr.Cells.Add(cell_value);
                grvCardDataSubRowCount += 1;
                _table.AddAt(grvCardDataSubRowCount, gvr);
            }
            else
            {
                grvCardDataSubRowCount += 1;
            }

        }
    }

    /// <summary>
    /// 設定本頁各GRID行數
    /// </summary>
    private void setGridLine()
    {
        //*設置GridView分頁顯示的行數
        int intPageSize = 15;
        //聯繫註記
        gpNoteLogList.PageSize = 5;
        grdNoteLog.PageSize = 5;
        //分公司負責人資料
        grvCardData.PageSize = intPageSize;
    }

    /// <summary>
    /// HEAD需要特殊處理的欄位
    /// </summary>
    /// <param name="hqObj"></param>
    private void setSpeciCollHead(AML_SessionState hDObj)
    {
        //風險等級
        hlblOriginalRiskRanking.Text = GetDcValue("RS_" + hDObj.OriginalRiskRanking);
        //新增最新試算後的風險等級
        hlblNewRiskRanking.Text = GetDcValue("RS_" + hDObj.NewRiskRanking);

        if (string.IsNullOrEmpty(hlblCaseExpiryDate.Text))
        {
            //依建案日期計算到期日 cell6 及 寄送不合作信函日期 cell8
            string pDate = hDObj.DataDate;
            string endDate = "";
            string noCropDate = "";
            proceDate(pDate, ref endDate, ref noCropDate);
            hlblCaseExpiryDate.Text = endDate;
        }

        //案件類別轉換
        hlblCaseType.Text = GetDcValue("CSTYPE_" + hlblCaseType.Text);
    }

    /// <summary>
    /// 計算到期日與寄送通知日
    /// </summary>
    /// <param name="endDate"></param>
    /// <param name="noCropDate"></param>
    private void proceDate(string pDate, ref string endDate, ref string noCropDate)
    {
        if (pDate.Length == 8)
        {
            try
            {
                string sD = pDate.Substring(0, 4) + "-" + pDate.Substring(4, 2) + "-" + pDate.Substring(6, 2);
                DateTime Ds = Convert.ToDateTime(sD);
                string eDtmp = Ds.AddMonths(6).ToString("yyyy/MM/01");
                endDate = Convert.ToDateTime(eDtmp).AddDays(-1).ToString("yyyyMMdd");
                //以到期日計算，先減5天，若為假日，(六、日)則依假日遞減回前一個工作日(周五)
                DateTime nCpTemp = Convert.ToDateTime(eDtmp).AddDays(-5);
                switch (nCpTemp.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        nCpTemp = nCpTemp.AddDays(-2);
                        break;
                    case DayOfWeek.Saturday:
                        nCpTemp = nCpTemp.AddDays(-1);
                        break;
                }
                noCropDate = nCpTemp.ToString("yyyyMMdd");
            }
            catch (Exception ex)
            {
                ///不寫LOG
                string res = ex.Message;
            }
        }
    }

    /// <summary>
    /// 檢查user登入物件 初始化字典
    /// </summary>
    protected void checkUserAgent()
    {
        if (eAgentInfo == null)
        {
            //UPDATEPANLE 自動POSTBACK 不會經過PAGE_LOAD 必須重取 eAgentInfo
            eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        }
        ///建立所有字典
        buiInfoDict();
    }
    #endregion

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        string lastPage = Session["P010801150001_Last"].ToString();
        //取得上一頁
        if (lastPage != "")
        {            
            Response.Redirect(lastPage, false);
        }
        else
        {            
            Response.Redirect("P010801000001.aspx", false);
        }

    }

    /// <summary>
    /// 顯示資料，並填至畫面
    /// </summary>
    private void showeditDate()
    {
        try
        {
            //由資料庫讀入，測試用
            //private void ReadFromDb()
            string strAlertMsg = "";
            //填基本物件，由電文讀取
            AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
            if (sessionOBJ == null)
            {
                string NavigateUrl = "P010801150001.aspx";
                strAlertMsg = MessageHelper.GetMessages("01_01080103_019");
                string urlString = @"alert('" + strAlertMsg + "');location.href='" + NavigateUrl + "';";
                base.sbRegScript.Append(urlString);
            }
            this.SetVal<AML_SessionState>(sessionOBJ, false);
            //表頭特殊欄位處理
            setSpeciCollHead(sessionOBJ);

            //SCCD物件除風險以外，須由資料庫讀出與回寫
            //讀取SCDD資料，轉換成Edit
            EntityHQ_SCDD sccdObj = BRHQ_SCDD.getSCDDData_WOrk(sessionOBJ);
            EntityHQ_SCDD_edit editObj = sccdObj.toEditMode();

            //讀取風險資料 Cdata_Work
            EntityAML_Cdata_Work cDobj = BRAML_Cdata_Work.getCData_WOrk(sessionOBJ);
            this.SetVal<EntityAML_Cdata_Work>(cDobj, false);
            if (cDobj.CreditCardBlockCode == null || cDobj.CreditCardBlockCode.Trim() == "")
            {
                CDlblCreditCardBlockCode.Text = "";
            }
            else
            {
                CDlblCreditCardBlockCode.Text = cDobj.CreditCardBlockCode == "0000000000" ? "N" : "Y";//信用卡Block Code 正常戶:10個0
            }


            //20211221_Ares_Jack_商店資訊
            List<EntityAML_BRCH_Work> BRCHColl = BRAML_BRCH_Work.getBRCH_WOrkColl(sessionOBJ);
            grvCardData.DataSource = BRCHColl;
            grvCardData.DataBind();
            gdvBRCHPerAMT.DataSource = BRCHColl;
            gdvBRCHPerAMT.DataBind();

            //讀取SCDD Report
            EntitySCDDReport sccdRObj = BRSCDDReport.getSCDDReport(sessionOBJ);
            this.SetVal<EntitySCDDReport>(sccdRObj, false);
            //轉換日期格式
            if (lblSR_DateTime.Text != "")
            {
                lblSR_DateTime.Text = Convert.ToDateTime(lblSR_DateTime.Text).ToString("yyyyMMdd");
            }

            //繫結MFA資訊
            EntityAML_HQ_MFAF MfafObj = BRAML_HQ_MFAF.getMFAFData_WOrk(sessionOBJ);
            this.SetVal<EntityAML_HQ_MFAF>(MfafObj, false);

            //讀取案件歷程聯絡註記
            DataTable dt = BRNoteLog.getNoteLog(sessionOBJ, "1");
            //將結果設定為ViewState
            ViewState["DataBind"] = dt;
            BindGridView();

            //建立HASHTABLE
            Hashtable JC66OBj = new Hashtable();
            JC66OBj.Add("FUNCTION_CODE", "I");
            JC66OBj.Add("CORP_NO", sessionOBJ.HCOP_HEADQUATERS_CORP_NO.Substring(0, 8).Trim());
            JC66OBj.Add("CORP_SEQ", sessionOBJ.HCOP_HEADQUATERS_CORP_NO.Substring(8, 2).Trim());


            Hashtable hstExmsP4A;
            if (!isTest)
            {
                //上送主機取得資料
                hstExmsP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC66, JC66OBj, false, "11", eAgentInfo);
            }
            else
            {
                //測試用模擬資料取得
                hstExmsP4A = GetTestData();
            }
            //如果無JC66電文權限，不會跳出頁面錯誤訊息 
            string hesRtn = "";

            try
            {
                //檢核若主機回傳無訊息，則提示後離開
                string rMsg = hstExmsP4A["MESSAGE_TYPE"].ToString();
                switch (rMsg)
                {
                    case "0000":

                        //取回後MAP至物件  
                        //取回後MAP至物件 注意!本處為編輯模式，所以HASHTABLE對應欄位物件應用edit，以方便直接對應到頁面
                        EntityAML_HQ_Work_edit edObj = HTconvertToObj(hstExmsP4A);

                        sessionOBJ.HCOP_CORP_REG_ENG_NAME = !string.IsNullOrEmpty(hstExmsP4A["CORP_REG_ENG_NAME"].ToString()) ? hstExmsP4A["CORP_REG_ENG_NAME"].ToString() : "";
                        //追加 長姓名的判斷和取值 
                        edObj = getLongName(edObj);

                        //暫存物件，
                        Session["P010801030001_Data"] = edObj;

                        //追加EntityHQ_SCDD_edit
                        EntityHQ_SCDD_edit SDDObj = SDDconvertToObj(hstExmsP4A);

                        editObj.IsSanction = SDDObj.IsSanction;
                        editObj.IsSanctionCountryCode1 = SDDObj.IsSanctionCountryCode1;
                        editObj.IsSanctionCountryCode2 = SDDObj.IsSanctionCountryCode2;
                        editObj.IsSanctionCountryCode3 = SDDObj.IsSanctionCountryCode3;
                        editObj.IsSanctionCountryCode4 = SDDObj.IsSanctionCountryCode4;
                        editObj.IsSanctionCountryCode5 = SDDObj.IsSanctionCountryCode5;

                        Session["P010801030001_SCDDData"] = editObj;
                        //顯示至頁面
                        ShowinPage(edObj, editObj);

                        break;
                    case "0006":
                    default:
                        // 如果無JC66電文權限，不會跳出頁面錯誤訊息 
                        if (!string.IsNullOrEmpty(hstExmsP4A["MESSAGE_CHI"].ToString()))
                        {
                            hesRtn = hstExmsP4A["MESSAGE_CHI"].ToString();
                        }
                        strAlertMsg = MessageHelper.GetMessages("01_01080103_020") + hesRtn;
                        string NavigateUrl = "P010801000001.aspx";
                        if (!string.IsNullOrEmpty(Session["P010801150001_Last"].ToString()))
                        {
                            NavigateUrl = Session["P010801150001_Last"].ToString();
                        }
                        string urlString = @"alert('" + strAlertMsg + "');location.href='" + NavigateUrl + "';";
                        base.sbRegScript.Append(urlString);
                        break;
                }
            }
            catch (Exception ex)
            {                
                Logging.Log(ex.ToString(), LogState.Error, LogLayer.Util);
                
                if (!string.IsNullOrEmpty(hstExmsP4A["HtgMsg"].ToString()))
                {
                    if (hstExmsP4A["HtgMsg"].ToString().Contains("704"))
                    {
                        strAlertMsg = MessageHelper.GetMessages("00_00000000_040");
                    }
                    else if (hstExmsP4A["HtgMsg"].ToString().Contains("705"))
                    {
                        strAlertMsg = MessageHelper.GetMessages("00_00000000_039");
                    }
                    else if (hstExmsP4A["HtgMsg"].ToString().Contains("799"))
                    {
                        strAlertMsg = MessageHelper.GetMessages("00_00000000_041");
                    }
                    else
                    {
                        strAlertMsg = MessageHelper.GetMessages("01_01080103_020") + hesRtn;
                    }
                }

                string NavigateUrl = "P010801000001.aspx";
                if (!string.IsNullOrEmpty(Session["P010801150001_Last"].ToString()))
                {
                    NavigateUrl = Session["P010801150001_Last"].ToString();
                }
                string urlString = @"alert('" + strAlertMsg + "');location.href='" + NavigateUrl + "';";
                base.sbRegScript.Append(urlString);
            }
        }
        catch (Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex);
            sbRegScript.Append("alert('" + strClientMsg + "');window.location.href = 'P010801000001.aspx';");
            return;
        }
    }
    /// <summary>
    /// 模擬測試用JC66電文回傳
    /// </summary>
    /// <returns></returns>
    private Hashtable GetTestData()
    {
        //讀取D:\JC66_Download_TEST.txt做成HSAHTABLE
        string[] lincoll = File.ReadAllLines(@"C:\JC66_Download_TEST.txt", Encoding.Default);
        Hashtable JC66OBj = new Hashtable();
        foreach (string oitem in lincoll)
        {
            string[] tmpColl = oitem.Split(new string[] { "<!>" }, StringSplitOptions.None);
            if (tmpColl.Length == 2)
            {
                JC66OBj.Add(tmpColl[0], tmpColl[1]);
            }
        }

        return JC66OBj;
    }
    /// <summary>
    /// 轉換HASHTABLE to EntityAML_HeadOffice物件
    /// </summary>
    /// <param name="inObj"></param>
    /// <returns></returns>
    private EntityHQ_SCDD_edit SDDconvertToObj(Hashtable inObj)
    {
        EntityHQ_SCDD_edit rtnObj = new EntityHQ_SCDD_edit();

        //先將主體值找出來，方法是用映射，取出對應欄位
        Type v = rtnObj.GetType();  //取的型別實體
        PropertyInfo[] props = v.GetProperties(); //取出所有公開屬性(可以被外部存取得 
                                                  //處理總公司資料
        foreach (PropertyInfo prop in props)
        {
            object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件
            AttributeRfPage authAttr;
            for (int xi = 0; xi < attrs.Length; xi++)
            {
                if (attrs[xi] is AttributeRfPage)
                {
                    authAttr = attrs[xi] as AttributeRfPage;

                    string Jc66FieldKey = authAttr.JC66NAME;
                    //JC66有值，建立HASTABLE欄位，
                    if (!string.IsNullOrEmpty(Jc66FieldKey))
                    {
                        if (inObj.ContainsKey(Jc66FieldKey))
                        {
                            prop.SetValue(rtnObj, inObj[Jc66FieldKey], null);
                        }
                    }
                }
            }
        }
        return rtnObj;
    }
    /// <summary>
    /// 轉換HASHTABLE to EntityAML_HeadOffice物件
    /// </summary>
    /// <param name="inObj"></param>
    /// <returns></returns>
    private EntityAML_HQ_Work_edit HTconvertToObj(Hashtable inObj)
    {
        EntityAML_HQ_Work_edit rtnObj = new EntityAML_HQ_Work_edit();

        //先將主體值找出來，方法是用映射，取出對應欄位
        Type v = rtnObj.GetType();  //取的型別實體
        PropertyInfo[] props = v.GetProperties(); //取出所有公開屬性(可以被外部存取得 
                                                  //處理總公司資料
        foreach (PropertyInfo prop in props)
        {
            object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件
            AttributeRfPage authAttr;
            for (int xi = 0; xi < attrs.Length; xi++)
            {
                if (attrs[xi] is AttributeRfPage)
                {
                    authAttr = attrs[xi] as AttributeRfPage;

                    string Jc66FieldKey = authAttr.JC66NAME;
                    //JC66有值，建立HASTABLE欄位，
                    if (!string.IsNullOrEmpty(Jc66FieldKey))
                    {
                        if (inObj.ContainsKey(Jc66FieldKey))
                        {
                            prop.SetValue(rtnObj, inObj[Jc66FieldKey], null);
                        }
                    }
                }
            }
        }
        
        return rtnObj;
    }
    /// <summary>
    ///  以畫面上物件產生JC66用HASHTABLE
    /// </summary>
    private Hashtable buildJC66Hash(EntityAML_HQ_Work_edit DataObj, EntityHQ_SCDD_edit editObj)
    {
        //建立HASHTABLE
        Hashtable hs = new Hashtable();
        //先取出所有屬性，其中有JC66對應名稱的 (總公司資料直接用映射)
        Type v = DataObj.GetType();  //取的型別實體
        PropertyInfo[] props = v.GetProperties(); //取出所有公開屬性(可以被外部存取得 
        //處理總公司資料
        foreach (PropertyInfo prop in props)
        {
            object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件
            AttributeRfPage authAttr;
            for (int xi = 0; xi < attrs.Length; xi++)
            {
                if (attrs[xi] is AttributeRfPage)
                {
                    authAttr = attrs[xi] as AttributeRfPage;

                    string Jc66FieldKey = authAttr.JC66NAME;
                    int Jc66Len = authAttr.JC66Len;
                    char Jc66Char = authAttr.JC6LenChar;
                    bool isPadLeft = authAttr.JC6PadLeft;
                    if (string.IsNullOrEmpty(authAttr.JC66NAME))
                    {
                        continue;
                    }
                    //JC66有值，建立HASTABLE欄位，
                    if (!string.IsNullOrEmpty(Jc66FieldKey))
                    {
                        string exVal = prop.GetValue(DataObj, null) as string;
                        if (Jc66Len > 0)
                        {
                            if (isPadLeft)
                            {
                                exVal = exVal.PadLeft(Jc66Len, Jc66Char);
                            }
                            else
                            {
                                exVal = exVal.PadRight(Jc66Len, Jc66Char);
                            }
                        }
                        if (exVal != null)
                        {
                            hs.Add(Jc66FieldKey, exVal);
                        }
                    }
                }
            }
        }
        //SCDD電文處理 只有六個，直接對應
        hs["BUSI_RISK_NATION_FLAG"] = editObj.IsSanction;
        hs["BUSI_RISK_NATION_1"] = editObj.IsSanctionCountryCode1;
        hs["BUSI_RISK_NATION_2"] = editObj.IsSanctionCountryCode2;
        hs["BUSI_RISK_NATION_3"] = editObj.IsSanctionCountryCode3;
        hs["BUSI_RISK_NATION_4"] = editObj.IsSanctionCountryCode4;
        hs["BUSI_RISK_NATION_5"] = editObj.IsSanctionCountryCode5;

        //是否為長姓名flag :聯絡人 =: 需再注意 跟JC68 的互動結果為何.
        string ContactID = string.Format("{0}{1}", DataObj.HCOP_HEADQUATERS_CORP_NO, DataObj.HCOP_HEADQUATERS_CORP_SEQ);
        hs["CONTACT_LNAM_FLAG"] = ValidLongNameFlag(ContactID, DataObj.HCOP_CONTACT_LNAME, DataObj.HCOP_CONTACT_ROMA);
        
        //設立日期 改成西元年
        if (hs.ContainsKey("BUILD_DATE"))
        {
            if (hs["REGISTER_US_STATE"].ToString().Length == 7)
            {
                hs["REGISTER_US_STATE"] = ConvertToDC(hs["REGISTER_US_STATE"].ToString());
            }
        }

        //身分證發證日期 改成西元年
        if (hs.ContainsKey("OWNER_ID_ISSUE_DATE"))
        {
            if (hs["OWNER_ID_ISSUE_DATE"].ToString().Length == 7)
            {
                hs["OWNER_ID_ISSUE_DATE"] = ConvertToDC(hs["OWNER_ID_ISSUE_DATE"].ToString());
            }
        }

        //生日 改成西元年
        if (hs.ContainsKey("OWNER_BIRTH_DATE"))
        {
            if (hs["OWNER_BIRTH_DATE"].ToString().Length == 7)
            {
                hs["OWNER_BIRTH_DATE"] = ConvertToDC(hs["OWNER_BIRTH_DATE"].ToString());
            }
        }

        //當地址第三段打Ｘ，送出主機的資料要把第三段清空
        if (hs.ContainsKey("REG_ADDR2") && hs["REG_ADDR2"].ToString().ToUpper().Trim() == "Ｘ")
        {
            hs["REG_ADDR2"] = "";
            hs["REG_ADDR2"] = hs["REG_ADDR2"].ToString().PadRight(14, '　');
        }

        if (hs.ContainsKey("MAILING_ADDR2") && hs["MAILING_ADDR2"].ToString().ToUpper().Trim() == "Ｘ")
        {
            hs["MAILING_ADDR2"] = "";
            hs["MAILING_ADDR2"] = hs["MAILING_ADDR2"].ToString().PadRight(14, '　');
        }

        if (hs.ContainsKey("OWNER_ADDR2") && hs["OWNER_ADDR2"].ToString().ToUpper().Trim() == "Ｘ")
        {
            hs["OWNER_ADDR2"] = "";
            hs["OWNER_ADDR2"] = hs["OWNER_ADDR2"].ToString().PadRight(14, '　');
        }

        //為避免因全形半形被擋，故將姓名再重新轉全形送出
        hs["OWNER_CHINESE_NAME"] = ToWide(hs["OWNER_CHINESE_NAME"].ToString()).Trim();

        //20220107_Ares_Jack_CORP_NO帶八碼
        hs["CORP_NO"] = hs["CORP_NO"].ToString().Trim().Substring(0, 8);

        return hs;
    }


    //由資料庫讀入，測試用
    private void ReadFromDb()
    {
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
        if (sessionOBJ == null)
        {
            string NavigateUrl = "P010801000001.aspx";
            if (!string.IsNullOrEmpty(Session["P010801010001_Last"].ToString()))
            {
                NavigateUrl = Session["P010801010001_Last"].ToString();
            }
            strAlertMsg = MessageHelper.GetMessages("01_01080103_019");
            string urlString = @"alert('" + strAlertMsg + "');location.href='" + NavigateUrl + "';";
            base.sbRegScript.Append(urlString);
        }
        this.SetVal<AML_SessionState>(sessionOBJ, false);
        //讀取公司資料 HQ_WORK CDATA,EDATA
        EntityAML_HQ_Work hqObj = BRAML_HQ_Work.getHQ_WOrk(sessionOBJ);
        EntityAML_HQ_Work_edit edObj = hqObj.toEditMode();

    }
    /// <summary>
    /// 設定下拉選單選取
    /// </summary>
    /// <param name="editObj"></param>
    private void SetDPValue(EntityHQ_SCDD_edit editObj)
    {
        setDropSelected(dropNameCheck_Item, editObj.NameCheck_Item);//名單掃描結果-結果的下拉選單               
    }
    /// <summary>
    /// 設定下拉選單選取
    /// </summary>
    /// <param name="editObj"></param>
    private void SetDPValue(EntityAML_HQ_Work_edit editObj)
    {
        setDropSelected(dropCountry2, editObj.HCOP_COUNTRY_CODE_2);//國籍2
        setDropSelected(dropHCOP_OWNER_ID_REPLACE_TYPE, editObj.HCOP_OWNER_ID_REPLACE_TYPE);//領補換類別        
        setDropSelected(dropHCOP_ID_PHOTO_FLAG, editObj.HCOP_ID_PHOTO_FLAG);//有無照片
    }
    /// <summary>
    /// 以傳入值設定選取
    /// </summary>
    /// <param name="DP"></param>
    /// <param name="val"></param>
    private void setDropSelected(CustDropDownList DP, string val)
    {
        foreach (ListItem item in DP.Items)
        {
            if (item.Value == val)
            {
                item.Selected = true;
                break;
            }
        }
    }
    /// <summary>
    /// 將電文資料顯示於畫面
    /// </summary>
    /// <param name="edObj"></param>
    private void ShowinPage(EntityAML_HQ_Work_edit edObj, EntityHQ_SCDD_edit SCDDObj)
    {
        //客戶資料、商店資訊
        this.SetVal<EntityAML_HQ_Work_edit>(edObj, false);
        SetDPValue(edObj);
        //名單掃描結果
        this.SetVal<EntityHQ_SCDD_edit>(SCDDObj, false);
        SetDPValue(SCDDObj);
        //HQ特殊欄位處理
        setHQSpcial(edObj);

        //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
        //"新增案件編號"
        //if (!string.IsNullOrEmpty(edObj.GROUP_NO))
        //{
        //    string relatedCaseNo = "";
        //    DataTable dtCaseNo = BRAML_HQ_Work.getRelatedCaseNo(edObj.GROUP_NO);
        //    for (int i = 0; i < dtCaseNo.Rows.Count; i++)
        //    {
        //        if (relatedCaseNo == "")
        //        {
        //            relatedCaseNo = dtCaseNo.Rows[i][0].ToString();
        //        }
        //        else
        //        {
        //            relatedCaseNo += "," + dtCaseNo.Rows[i][0].ToString();
        //        }
        //        ID.Add(dtCaseNo.Rows[i][1].ToString());
        //    }
        //    hlblRelateCaseNo.Text = relatedCaseNo;
        //}
        //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis             
    }
    
    
    /// <summary>
    /// 依傳入行業別顯示於畫面
    /// </summary>
    /// <param name="text"></param>
    private void setCCName(string text)
    {
        //Talas 20190919 修正將顏色還原
        txtAMLCC.BackColor = default(Color);
        DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", text);
        if (result.Rows.Count < 0 || !CheckCodeType(result))
        {
            txtAMLCC.BackColor = Color.Red;
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

    /// <summary>
    /// 行業別變更時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param> 
    protected void txtAMLCC_TextChanged(object sender, EventArgs e)
    {
        setCCName(txtAMLCC.Text);
    }
    /// <summary>
    /// 國籍2 Changed Event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtHCOP_COUNTRY_CODE_2_TextChanged(object sender, EventArgs e)
    {
        string strAlertMsg = "";
        txtHCOP_COUNTRY_CODE_2.Text = txtHCOP_COUNTRY_CODE_2.Text.ToUpper();
        //Talas 20190919 調整國籍檢核不合規則變色
        CustTextBox mConteol = (CustTextBox)sender;
        mConteol.BackColor = default(Color);
        if (string.IsNullOrEmpty(txtHCOP_COUNTRY_CODE_2.Text) || txtHCOP_COUNTRY_CODE_2.Text.Trim() == "請選擇") //空白不檢核
        {
            return;
        }
        //檢核國籍       
        //不正確的國籍
        if (GetDcValue("CT_1_" + txtHCOP_COUNTRY_CODE_2.Text.ToUpper()) == "")
        {
            //Talas 20190919 調整國籍檢核不合規則變色
            mConteol.BackColor = Color.Red;
            strAlertMsg = MessageHelper.GetMessages("01_01080103_008");
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

    }


    /// <summary>
    /// 依傳入值鎖定-解鎖控制項
    /// </summary>
    /// <param name="controlName"></param>
    /// <param name="ctrlType"></param>
    /// <param name="isLock"></param>
    private void ChangeControlLock(string controlName, string ctrlType, object Lock)
    {
        object myControl = FindControl(controlName);
        bool isLock = false;
        if (Lock != null)
        {
            isLock = (bool)Lock;
        }
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
                csT.Visible = true;                
                if (Lock == null)
                {
                    csT.Visible = false;
                }
                else
                {
                    if (isLock)
                    {
                        csT.ShowID = "01_01080103_057";
                    }
                    else
                    {
                        csT.ShowID = "01_01080103_058";
                    }
                }
                break;
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

    private string ValidLongNameFlag(string id, string Lname, string roma)
    {
        string result = "N";
        //若ID(身分證字號或公司統編+流水號)不等於空白時才進一步比對
        if (string.IsNullOrEmpty(id) == false)
        {
            //判斷 是否有符合 長姓名規範, 若為空白就是有符合
            string valideResult = valideLnameRoma(Lname, roma);
            if (string.IsNullOrEmpty(valideResult) == true)
            {
                result = "Y";
            }
        }
        return result;
    }



    /// <summary>
    /// 判斷長姓名Flag 並查詢電文取得對應值
    /// </summary>
    /// <param name="inObj"></param>
    /// <returns></returns>
    private EntityAML_HQ_Work_edit getLongName(EntityAML_HQ_Work_edit inObj)
    {
        EntityAML_HQ_Work_edit rtnObj = inObj;
        BRHTG_JC68 obj = new BRHTG_JC68(sPageInfo.strPageCode);
        EntityHTG_JC68 _data = new EntityHTG_JC68();
        List<string> HTGfailMsg = new List<string>();
        string msgFormat = "{0}:{1}. ";
        string tmpID = string.Empty;
        //主要是判斷長姓名flag 是否為 Y, 若是的話就要再到JC68 去查對應
        if (inObj.CONTACT_LNAM_FLAG.Equals("Y"))
        {
            tmpID = string.Format("{0}{1}", inObj.HCOP_HEADQUATERS_CORP_NO, inObj.HCOP_HEADQUATERS_CORP_SEQ);
            _data.ID = tmpID;
            _data = obj.getData(_data, eAgentInfo);
            rtnObj.HCOP_CONTACT_LNAME = _data.LONG_NAME;
            rtnObj.HCOP_CONTACT_ROMA = _data.PINYIN_NAME;
            if (!_data.Success)
            {
                HTGfailMsg.Add(string.Format(msgFormat, tmpID, _data.MESSAGE_TYPE));
            }
        }

        if (inObj.OWNER_LNAM_FLAG.Equals("Y"))
        {
            _data = new EntityHTG_JC68();
            tmpID = inObj.HCOP_OWNER_ID;
            //判斷要查詢的ID是(身分證字號 -> 護照號 -> 居留證號)
            if (string.IsNullOrEmpty(tmpID) == true)
            {
                tmpID = inObj.HCOP_PASSPORT;
            }
            if (string.IsNullOrEmpty(tmpID) == true)
            {
                tmpID = inObj.HCOP_RESIDENT_NO;
            }

            _data.ID = tmpID;
            _data = obj.getData(_data, eAgentInfo);
            rtnObj.HCOP_OWNER_CHINESE_LNAME = _data.LONG_NAME;
            rtnObj.HCOP_OWNER_ROMA = _data.PINYIN_NAME;
            if (!_data.Success)
            {
                HTGfailMsg.Add(string.Format(msgFormat, tmpID, _data.MESSAGE_TYPE));
            }
        }


        if (HTGfailMsg.Count > 0)
        {
            this.strHostMsg = "查詢長姓名電文時有異常!! " + string.Join("", HTGfailMsg.ToArray()); ;
        }

        return rtnObj;
    }

    /// <summary>
    /// 長中文姓名和羅馬拼音的檢查規範
    /// </summary>
    /// <param name="lname"></param>
    /// <param name="roma"></param>
    /// <returns></returns>
    private string valideLnameRoma(string lname, string roma)
    {
        string _result = string.Empty;

        if (string.IsNullOrEmpty(lname) == true)
        {
            _result = "長中文姓名不得為空白";
        }
        else if ((lname.Length + roma.Length) < 5)
        {
            _result = "長中文姓名+羅馬拼音的長度需大於4";
        }
        else
        {
            //有填羅馬拼音時就需要符合規範
            if (!string.IsNullOrEmpty(roma) && this.ValidRoma(roma) == false)
            {
                _result = MessageHelper.GetMessages("01_01080103_025");
            }
        }

        return _result;
    }

    #region NameCheck
    /// <summary>
    /// ESB_NameCheck 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnESB_NameCheck_Click(object sender, EventArgs e)
    {
        try
        {
            AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
            List<Entity_ESBNameCheck_DateObj> NameCheck_DateObj = new List<Entity_ESBNameCheck_DateObj>();
            NameCheck_DateObj = GetNameCheck_Item(sessionOBJ.CASE_NO, sessionOBJ.HCOP_CORP_REG_ENG_NAME);

            string strConfirmMsg = string.Empty;
            string strIDMsg = string.Empty;
            string strItemMsg = string.Empty;
            // 判斷 改用STR提示人工判斷
            // 新增 是否有分公司資料        
            string strBrchMsg = string.Empty;
            // 高階主管是否有身分類型
            string strJobTypeMsg = string.Empty;
            string strBrch_HQ_Msg = string.Empty;
            // 記錄如果分公司設立日期為NULL或00000000的訊息
            string strBrch_BuildDate_Msg = string.Empty;

            int i = 1;
            //用ROW_NO排序 最大值優先
            //顯示資料ROW_NO 小會在最上面
            NameCheck_DateObj.Sort(new NameCheck_DateObj_Comparer_TaD());

            if (this.txtHCOP_COUNTRY_CODE_2.Text.ToUpper().Trim() == "TW")
            {
                base.sbRegScript.Append("alert('國籍二不得為TW! ');$('#txtHCOP_COUNTRY_CODE_2').focus();");
                return;
            }
            //20220125_Ares_Jack_名單掃描國籍不得為無
            if (this.txtHCOP_COUNTRY_CODE_2.Text.Trim() == "無")
            {
                base.sbRegScript.Append("alert('國籍二不得為 無! ');$('#txtHCOP_COUNTRY_CODE_2').focus();");
                return;
            }
            //20220127_Ares_Jack_出生年月日不得為空
            if (this.txtHCOP_OWNER_BIRTH_DATE.Text.Trim() == "00000000" || this.txtHCOP_OWNER_BIRTH_DATE.Text.Trim() == "")
            {
                base.sbRegScript.Append("alert('出生年月日不得為空! ');$('#txtHCOP_OWNER_BIRTH_DATE').focus();");
                return;
            }

            for (int x = NameCheck_DateObj.Count - 1; x >= 0; x--) //(Entity_ESBNameCheck_DateObj reObj in NameCheck_DateObj)
            {
                // 比對NameCheck_DateObj資料是否已拋查過
                //if (BRPNAMECHECKLOG.NameChecklog_Compare(sessionOBJ.CASE_NO, NameCheck_DateObj[x].ID, NameCheck_DateObj[x].CHINESE_NAME, NameCheck_DateObj[x].ENGLISH_NAME))
                // 2020.06.29 增加ConnectedPartySubType 與 ConnectedPartyType判斷
                //20220111_Ares_Jack_自然人名單掃描加上國籍二 比對NameCheck_DateObj資料是否已拋查過
                if (BRPNAMECHECKLOG.NameChecklog_Compare_Natural_Person(sessionOBJ.CASE_NO, NameCheck_DateObj[x].ID, NameCheck_DateObj[x].CHINESE_NAME, NameCheck_DateObj[x].ENGLISH_NAME, NameCheck_DateObj[x].ConnectedPartyType, NameCheck_DateObj[x].ConnectedPartySubType, NameCheck_DateObj[x].NATION))
                {
                    // 重複資料刪除ltem
                    NameCheck_DateObj.RemoveAt(x);
                    continue;
                }

                if (NameCheck_DateObj.Count == 1)
                {
                    //20220117_Ares_Jack_異動為一筆時, TYPE = 1, ConnectedPartyType = "", ConnectedPartySubType = ""
                    NameCheck_DateObj[x].ConnectedPartyType = "";
                    NameCheck_DateObj[x].ConnectedPartySubType = "";
                }
                StringBuilder sb = new StringBuilder();
                sb.Append(i + ". ");

                sb.Append(NameCheck_DateObj[x].CHINESE_NAME);
                sb.Append(" / " + NameCheck_DateObj[x].ENGLISH_NAME);
                sb.Append(" / " + NameCheck_DateObj[x].ID);
                sb.Append(" / " + NameCheck_DateObj[x].BIRTH_DATE);
                sb.Append(" / " + NameCheck_DateObj[x].NATION);
                sb.Append(" / " + NameCheck_DateObj[x].ConnectedPartyType);
                sb.Append(" / " + NameCheck_DateObj[x].ConnectedPartySubType);
                sb.Append("\\n");
                strConfirmMsg = strConfirmMsg + sb.ToString();
                i = i + 1;
            }

            // 查詢資料有問題
            if (!string.IsNullOrEmpty(strItemMsg))
            {
                strItemMsg = strItemMsg.Substring(0, (strItemMsg.Length - 1));
                base.sbRegScript.Append("alert('" + strItemMsg + "\\n資料有誤，請確認拋查資料是否正確!');");
                return;
            }


            // 判斷是否有新資料需要拋查
            if (string.IsNullOrEmpty(strConfirmMsg))
            {
                // 無資料顯示無異動訊息+高階管理人為Z999999999
                base.sbRegScript.Append("alert('資料無異動 無需執行名單掃描!\\n" + strIDMsg + "');");
            }
            else
            {
                // 彈跳視窗按確認 執行btnESB_NameCheckAdd(按鈕隱藏)
                Session["P010801030001_NAMECHECK_DATA"] = NameCheck_DateObj;
                base.sbRegScript.Append("if(confirm('目前NameCheck資料\\n" + strConfirmMsg + "" + strIDMsg + "')) {$('#btnESB_NameCheckAdd').click();}else{}");
            }
        }
        catch(Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex + "執行名單掃描失敗");
            sbRegScript.Append("alert('" + strClientMsg + "');window.location.href = 'P010109050001.aspx';");
        }
    }

    /// <summary>
    /// Entity_ESBNameCheck_DateObj_Comparer
    /// ESBNameCheck_DateObj 排序ROW_NO 大到小
    /// </summary>
    public class NameCheck_DateObj_Comparer_TaD : IComparer<Entity_ESBNameCheck_DateObj>
    {
        //用ID排序
        public int Compare(Entity_ESBNameCheck_DateObj x, Entity_ESBNameCheck_DateObj y)
        {
            return (y.ROW_NO.CompareTo(x.ROW_NO));
        }
    }

    /// <summary>
    /// Entity_ESBNameCheck_DateObj_Comparer
    /// ESBNameCheck_DateObj 排序ROW_NO 小到大
    /// </summary>
    public class NameCheck_DateObj_Comparer_DaT : IComparer<Entity_ESBNameCheck_DateObj>
    {
        //用ID排序
        public int Compare(Entity_ESBNameCheck_DateObj x, Entity_ESBNameCheck_DateObj y)
        {
            return (x.ROW_NO.CompareTo(y.ROW_NO));
        }
    }

    /// <summary>
    /// NameCheck_Data
    /// 抓取公司資料-發電文資料
    /// </summary>
    /// <param name="CASE_NO"></param>
    /// <returns></returns>
    private List<Entity_ESBNameCheck_DateObj> GetNameCheck_Item(string CASE_NO, string _HCOP_CORP_REG_ENG_NAME)
    {
        List<Entity_ESBNameCheck_DateObj> reObj = new List<Entity_ESBNameCheck_DateObj>();
        Entity_ESBNameCheck_DateObj reObjGet = new Entity_ESBNameCheck_DateObj();        

        #region 總公司
        // 總公司登記名稱與統編 Row_no 11
        reObjGet = new Entity_ESBNameCheck_DateObj();
        reObjGet.CHINESE_NAME = HQlblHCOP_OWNER_CHINESE_NAME.Text.Trim();//20211203 自然人總公司名稱帶負責人名字 by Ares Stanley        
        reObjGet.ENGLISH_NAME = HQlblHCOP_OWNER_ENGLISH_NAME.Text.Trim();//20211203 自然人總公司名稱帶負責人英文名 by Ares Stanley
        reObjGet.BATCH_NO = "";        
        reObjGet.ID = HQlblHCOP_HEADQUATERS_CORP_NO.Text.Trim();
        reObjGet.ITEM = "總公司登記名稱";
        reObjGet.ROW_NO = "11";        
        reObjGet.ESB_TYPE = "1";
        reObjGet.ConnectedPartyType = "";
        reObjGet.ConnectedPartySubType = "";
        reObjGet.BIRTH_DATE = txtHCOP_OWNER_BIRTH_DATE.Text.Trim();//20211207 自然人DOB帶負責人生日 by Ares Stanley
        reObjGet.NATION = txtHCOP_OWNER_NATION.Text.Trim();//20211207 自然人國籍帶負責人國籍 by Ares Stanley

        reObj.Add(reObjGet);

        //自然人-國籍二 名單掃描資料
        if (!string.IsNullOrEmpty(this.txtHCOP_COUNTRY_CODE_2.Text.Trim()) && !this.txtHCOP_COUNTRY_CODE_2.Text.Trim().Equals("請選擇"))
        {
            reObjGet = new Entity_ESBNameCheck_DateObj();
            reObjGet.CHINESE_NAME = HQlblHCOP_OWNER_CHINESE_NAME.Text.Trim();//20211203 自然人總公司名稱帶負責人名字 by Ares Stanley        
            reObjGet.ENGLISH_NAME = HQlblHCOP_OWNER_ENGLISH_NAME.Text.Trim();//20211203 自然人總公司名稱帶負責人英文名 by Ares Stanley
            reObjGet.BATCH_NO = "";
            reObjGet.ID = HQlblHCOP_HEADQUATERS_CORP_NO.Text.Trim();
            reObjGet.ITEM = "總公司登記名稱";
            reObjGet.ROW_NO = "12";
            reObjGet.ESB_TYPE = "2";
            reObjGet.ConnectedPartyType = "LA";
            reObjGet.ConnectedPartySubType = "LA";
            reObjGet.BIRTH_DATE = txtHCOP_OWNER_BIRTH_DATE.Text.Trim();//20211207 自然人DOB帶負責人生日 by Ares Stanley
            reObjGet.NATION = this.txtHCOP_COUNTRY_CODE_2.Text.Trim();//20211207 自然人國籍帶負責人國籍 by Ares Stanley
            reObj.Add(reObjGet);
        }

        #endregion
        return reObj;
    }

    /// <summary>
    /// 發送EMS電文資訊
    /// </summary>
    public string ConnESB(string ServerUrl, string ServerPort, string UserName, string Password, string ESBSendQueueName, string ESBReceiveQueueName, string strXML, ref bool msgNull, ref string ConnEndTime, ref string SendupEndTime, ref string ReceDownEndTime, ref string ConnColseEndTime, string Uat, ref bool timeout)
    {
        //當線路1 連線錯誤　& TimeOut msgNull = ture 跑線路2
        msgNull = false;
        string strResult = string.Empty;
        string _url = string.Empty;
        string _messageid = string.Empty;
        // ESB 設定Timeout秒數
        int ESBTimeout = Convert.ToInt32(UtilHelper.GetAppSettings("ESBTimeout").ToString());
        _url = "tcp://" + ServerUrl + ":" + ServerPort;
        /* 方法二,直接使用QueueConnectionFactory */
        try
        {
            QueueConnectionFactory factory = new TIBCO.EMS.QueueConnectionFactory(_url);
            QueueConnection connection = factory.CreateQueueConnection(UserName, Password);
            try
            {
                QueueSession session = connection.CreateQueueSession(false, TIBCO.EMS.Session.AUTO_ACKNOWLEDGE);

                TIBCO.EMS.Queue queue = session.CreateQueue(ESBSendQueueName);

                QueueSender qsender = session.CreateSender(queue);

                /* send messages */
                TextMessage message = session.CreateTextMessage();
                message.Text = strXML;

                //一定要設定要reply的queue,這樣才收得到
                message.ReplyTo = (TIBCO.EMS.Destination)session.CreateQueue(ESBReceiveQueueName);
                ConnEndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                qsender.Send(message);
                SendupEndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                _messageid = message.MessageID;

                //receive message
                String messageselector = null;
                messageselector = "JMSCorrelationID = '" + _messageid + "'";

                TIBCO.EMS.Queue receivequeue = session.CreateQueue(ESBReceiveQueueName);

                QueueReceiver receiver = session.CreateReceiver(receivequeue, messageselector);

                connection.Start();

                //set up timeout 
                TIBCO.EMS.Message msg = receiver.Receive(ESBTimeout * 1000);

                if (msg == null)
                {
                    msgNull = true;
                    strResult = "";
                    Logging.Log(Uat + "：電文TimeOut：\r\n", logPath_ESB, LogLayer.UI);
                    timeout = true; // 如有TimeOut 設定為True
                }
                else
                {
                    msg.Acknowledge();
                    if (msg is TextMessage)
                    {
                        TextMessage tm = (TextMessage)msg;
                        strResult = tm.Text;
                    }
                    else
                    {
                        strResult = msg.ToString();
                    }
                }
                ReceDownEndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                ConnColseEndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                return strResult;
            }
            catch (Exception ex)
            {
                Logging.Log(Uat + "：ESB連線錯誤\r\n" + ex.ToString(), logPath_ESB, LogLayer.UI);
                msgNull = true;
                return "ESB連線錯誤";
            }
            finally
            {
                try
                {
                    connection.Close();
                }
                catch (Exception)
                {

                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(Uat + "：ESB連線錯誤\r\n" + ex.ToString(), logPath_ESB, LogLayer.UI);
            msgNull = true;
            return "ESB連線錯誤";
        }
    }


    protected void btnESB_NameCheckAdd_Click(object sender, EventArgs e)
    {
        try
        {
            AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
            List<Entity_ESBNameCheck_DateObj> sessionNCObj = (List<Entity_ESBNameCheck_DateObj>)Session["P010801030001_NAMECHECK_DATA"];
            //NameCheckDateObj 用ROW_NO排序 小排大
            // 跑資料ROW_NO 小會在最上面 跟顯示資料順序一致
            sessionNCObj.Sort(new NameCheck_DateObj_Comparer_DaT());
            string strXML = string.Empty;
            const string strHerader1 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            const string strHerader2 = "<ns0:ServiceEnvelope xmlns:ns0=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceEnvelope\">";
            const string strHerader3 = "<ns1:ServiceHeader xmlns:ns1=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceHeader\">";

            // NAMECHECKLOG 參數
            string strCASE_NO = sessionOBJ.CASE_NO;
            string strTransactionID = "CSIP" + System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string strTrnNumD = "";
            string strSEQ = "";
            string strTellerName = eAgentInfo.agent_id;
            string strRspCode = "";
            string strBranchNo = "";
            string strMatchedResult = "";
            string strRCScore = "";
            string strReferenceNumber = "";
            string strAMLReferenceNumber = "";

            // XML InputName 
            string strESBChinese_Name = "";
            string strESBEnglish_Name = "";
            string strESBEnglish_Name2 = "";
            string strESBBirth_Date = "";
            string strESBID = "";
            string strESBNATION = "";
            string strERRORDESC = "";
            string strESBType = "";
            string strESBConnectedPartyType = "";
            string strESBConnectedPartySubType = "";
            string strESBCustType = "P";//自然人(P) 法人(N)
            string strESBGender = "";//性別
            if (txtGENDER.Text == "男")
                strESBGender = "M";
            if (txtGENDER.Text == "女")
                strESBGender = "F";
            string strESBBroadNameSearch = "";


            // ESB 電文指定相關參數
            // 第一組
            string ServerUrl = UtilHelper.GetAppSettings("ESB_ServerUrl").ToString();
            string ServerPort = UtilHelper.GetAppSettings("ESB_ServerPort").ToString();
            string UserName = UtilHelper.GetAppSettings("ESB_UserName").ToString();
            string Password = UtilHelper.GetAppSettings("ESB_Password").ToString();
            string ESBSendQueueName = UtilHelper.GetAppSettings("ESB_SendQueueName").ToString();
            string ESBReceiveQueueName = UtilHelper.GetAppSettings("ESB_ReceiveQueueName").ToString();
            // 第二組
            string ServerUrl_1 = UtilHelper.GetAppSettings("ESB_ServerUrl_1").ToString();
            string ServerPort_1 = UtilHelper.GetAppSettings("ESB_ServerPort_1").ToString();
            string UserName_1 = UtilHelper.GetAppSettings("ESB_UserName_1").ToString();
            string Password_1 = UtilHelper.GetAppSettings("ESB_Password_1").ToString();
            string ESBSendQueueName_1 = UtilHelper.GetAppSettings("ESB_SendQueueName_1").ToString();
            string ESBReceiveQueueName_1 = UtilHelper.GetAppSettings("ESB_ReceiveQueueName_1").ToString();

            string strESBBatch_No = UtilHelper.GetAppSettings("ESBBatch_No").ToString();
            int ESBRetry = Convert.ToInt32(UtilHelper.GetAppSettings("ESBRetry").ToString());
            string ConnEndTime = "";//  連線結束時間
            string SendupEndTime = "";//發送上行結束時間
            string ReceDownEndTime = "";//收到下行結束時間
            string ConnColseEndTime = "";//關閉連接結束時間
            string strResult = string.Empty;//ESB回傳訊息
            string strESBMsg = "";
            bool msgNull = false;
            bool esbtimeout = false; // 紀錄ESB是否Timeout

            string strMsg = string.Empty;
            int ESBSEQ = 1;
            int sessionObjCount = sessionNCObj.Count;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(strHerader1);
            sb.AppendLine(strHerader2);
            sb.AppendLine(strHerader3);
            sb.AppendLine("<ns1:ServiceName>WlfNameCheck</ns1:ServiceName>");
            sb.AppendLine("<ns1:ServiceVersion>02</ns1:ServiceVersion>");
            sb.AppendLine("<ns1:SourceID>CSIP</ns1:SourceID>");
            sb.AppendLine("<ns1:TransactionID>" + strTransactionID + "</ns1:TransactionID>");
            sb.AppendLine("<ns1:RqTimestamp>" + System.DateTime.Now.ToString("yyyy-MM-dd") + "T" + System.DateTime.Now.ToString("HH:mm:ss.ff") + "+08:00" + "</ns1:RqTimestamp>");
            sb.AppendLine("</ns1:ServiceHeader>");
            sb.AppendLine("<ns1:ServiceBody xmlns:ns1=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceBody\">");
            sb.AppendLine("<ns2:WlfNameCheckRq xmlns:ns2=\"http://ns.chinatrust.com.tw/XSD/CTCB/AML/Message/WlfNameCheckRq/02\">");
            sb.AppendLine("<ns2:REQHDR>");
            sb.AppendLine("<ns2:TrnNum>" + strTransactionID + "</ns2:TrnNum>");
            sb.AppendLine("</ns2:REQHDR><ns2:REQBDY>");
            sb.AppendLine("<ns2:ReferenceNumber>" + strTransactionID + "</ns2:ReferenceNumber>");
            sb.AppendLine("<ns2:Channel/>");
            sb.AppendLine("<ns2:BankNo>TW</ns2:BankNo>");
            sb.AppendLine("<ns2:InputNames>");

            
            
            //名單掃描資料
            #region Entity_ESBNameCheck_DateObj 資料填寫
            foreach (Entity_ESBNameCheck_DateObj li in sessionNCObj)
            {
                if (sessionNCObj.Count == 1)
                {
                    //20220117_Ares_Jack_異動為一筆時, TYPE = 1, ConnectedPartyType = "", ConnectedPartySubType = ""
                    li.ESB_TYPE = "1";
                    li.ConnectedPartyType = "";
                    li.ConnectedPartySubType = "";
                }
                strESBType = li.ESB_TYPE;
                strESBConnectedPartyType = li.ConnectedPartyType;
                strESBConnectedPartySubType = li.ConnectedPartySubType;


                strSEQ = ESBSEQ.ToString().PadLeft(3, '0');
                strESBChinese_Name = li.CHINESE_NAME;
                strESBEnglish_Name = li.ENGLISH_NAME;
                //20220118 AML NOVA 功能需求程式碼,註解保留 start by Ares Jack
                //strESBEnglish_Name2 = li.EnglishName2;
                //20220118 AML NOVA 功能需求程式碼,註解保留 end by Ares Jack

                //生日為00000000時，替換成空值
                if (li.BIRTH_DATE == "00000000" || string.IsNullOrEmpty(li.BIRTH_DATE))
                    strESBBirth_Date = "";
                else
                    strESBBirth_Date = li.BIRTH_DATE.Trim();

                strESBID = li.ID.Trim();
                strESBNATION = string.IsNullOrEmpty(li.NATION) ? "" : li.NATION.Trim();

                sb.AppendLine("<ns2:InputName> ");
                sb.AppendLine("<ns2:Nationality>" + strESBNATION + "</ns2:Nationality>");
                sb.AppendLine("<ns2:EnglishName>" + strESBEnglish_Name + "</ns2:EnglishName>");
                // 依mail2020/7/3 11:11要求AddressCountry改帶空白
                sb.AppendLine("<ns2:AddressCountry></ns2:AddressCountry>");

                //20211202 自然NameCheck電文調整 
                sb.AppendLine("<ns2:BranchNo>" + strESBBatch_No + "</ns2:BranchNo>");
                sb.AppendLine("<ns2:Type>" + strESBType + "</ns2:Type>");
                sb.AppendLine("<ns2:NonEnglishName>" + strESBChinese_Name + "</ns2:NonEnglishName>");
                sb.AppendLine("<ns2:ConnectedPartyType>" + strESBConnectedPartyType  + "</ns2:ConnectedPartyType>");
                sb.AppendLine("<ns2:DOB>" + strESBBirth_Date + "</ns2:DOB>");
                sb.AppendLine("<ns2:ConnectedPartySubType>" + strESBConnectedPartySubType + "</ns2:ConnectedPartySubType>");
                //20211202 自然NameCheck電文調整 

                sb.AppendLine("<ns2:ID>" + strESBID + "</ns2:ID>");
                sb.AppendLine("</ns2:InputName>");

                #region 退版機制
                DataTable dt = new DataTable();
                CSIPCommonModel.BusinessRules_new.BRM_PROPERTY_CODE.GetEnableProperty("01", new string[] { "OLD_VERSION_FLAG" }, ref dt);
                string flag = "";
                if (dt.Rows.Count > 0)
                {
                    flag = dt.Rows[0]["PROPERTY_CODE"].ToString();
                }
                #endregion
                if (flag == "N")// 20211130_Ares_Jack_新版程式碼
                {
                    sb.AppendLine("<ns2:EnglishName2>" + strESBEnglish_Name2 + "</ns2:EnglishName2>");//羅馬拼音全名
                    sb.AppendLine("<ns2:Gender>" + strESBGender + "</ns2:Gender>");//性別
                    sb.AppendLine("<ns2:CustType>" + strESBCustType + "</ns2:CustType>");//客戶類別
                    sb.AppendLine("<ns2:BroadNameSearch>" + strESBBroadNameSearch + "</ns2:BroadNameSearch>");//BroadNameSearch
                }

                #region INSERT INTO NAMECHECKLOG KIND = "S"
                Entity_NAMECHECKLOG nchecklog = new Entity_NAMECHECKLOG();
                nchecklog.TellerName = strTellerName;
                nchecklog.CASE_NO = strCASE_NO;
                nchecklog.SEQ = strSEQ;
                nchecklog.REPCODE = "";
                nchecklog.TRNNUM = strTransactionID;
                nchecklog.BankNo = "TW";
                nchecklog.BranchNo = strESBBatch_No;
                nchecklog.ID = strESBID;

                nchecklog.EnglishName = strESBEnglish_Name;
                nchecklog.NonEnglishName = strESBChinese_Name;
                //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
                //nchecklog.EnglishName2 = strESBEnglish_Name2;
                //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis
                nchecklog.DOB = strESBBirth_Date;
                nchecklog.Nationality = strESBNATION;
                nchecklog.Type = strESBType;
                nchecklog.AddressCountry = "";
                nchecklog.ConnectedPartyType = strESBConnectedPartyType;
                nchecklog.ConnectedPartySubType = strESBConnectedPartySubType;
                //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
                //nchecklog.Gender = strESBGender;
                //nchecklog.CustType = strESBCustType;//自然人(P) 法人(N)                    
                //nchecklog.BroadNameSearch = strESBBroadNameSearch;
                //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis
                nchecklog.MatchedResult = "";
                nchecklog.RCScore = "";
                nchecklog.ReferenceNumber = strTransactionID;
                nchecklog.AMLReferenceNumber = "";
                nchecklog.ERRORDESC = "";
                nchecklog.KIND = "S";
                nchecklog.CRE_USER = strTellerName;
                nchecklog.CRE_DATE = System.DateTime.Now.ToShortDateString();
                BRPNAMECHECKLOG.Add(nchecklog, ref strMsg);
                ESBSEQ = ESBSEQ + 1;
                    
                #endregion
            }
            #endregion

            
            sb.AppendLine("</ns2:InputNames>");
            sb.AppendLine("<ns2:TellerName>" + strTellerName + "</ns2:TellerName>");
            sb.AppendLine("</ns2:REQBDY>");
            sb.AppendLine("</ns2:WlfNameCheckRq>");
            sb.AppendLine("</ns1:ServiceBody>");
            sb.AppendLine("</ns0:ServiceEnvelope>");
            strXML = sb.ToString();

            // 紀錄電文上行log
            SaveNameCheckLog(strXML, "REQ", strCASE_NO, logPath_ESB_TG);

            for (int i = 1; i <= ESBRetry; i++)
            {
                strResult = ConnESB(ServerUrl, ServerPort, UserName, Password, ESBSendQueueName, ESBReceiveQueueName, strXML, ref msgNull, ref ConnEndTime, ref SendupEndTime, ref ReceDownEndTime, ref ConnColseEndTime, "UAT1", ref esbtimeout);
                Logging.Log(" 發查次數：" + i.ToString() + "；ConnESB電文第一組Result：" + strResult, logPath_ESB, LogLayer.UI);
                //當線路1 連線錯誤　& TimeOut msgNull = ture 跑線路2
                if (msgNull)
                {
                    //第一組發電文TimeOut時，改查第二組
                    strResult = ConnESB(ServerUrl_1, ServerPort_1, UserName_1, Password_1, ESBSendQueueName_1, ESBReceiveQueueName_1, strXML, ref msgNull, ref ConnEndTime, ref SendupEndTime, ref ReceDownEndTime, ref ConnColseEndTime, "UAT2", ref esbtimeout);
                    string connResult = string.Empty;
                    if (!strResult.Trim().Equals("ESB連線錯誤") && !strResult.Trim().Equals(""))
                    {
                        connResult = "拋查成功";
                    }
                    else
                    {
                        connResult = strResult.Trim().Equals("") ? "TimeOut" : strResult.Trim();
                    }
                    Logging.Log(" 發查次數：" + i.ToString() + "；ConnESB電文第二組Result：" + connResult, logPath_ESB, LogLayer.UI);
                }

                //判斷當回傳訊息是ESB連線錯誤或非空值，直接離開迴圈
                //如果空值代表TimeOut
                if (strResult == "ESB連線錯誤" && esbtimeout == false)
                    break;
                if (!string.IsNullOrEmpty(strResult) && esbtimeout == false)
                    break;
            }

            //調整程式執行順序，先取xml node value by Ares Stanley 20211126
            ESB_RTNXml_SingleNode(strResult, ref strRspCode, ref strTrnNumD, ref strMatchedResult, ref strRCScore, ref strReferenceNumber, ref strAMLReferenceNumber, ref strERRORDESC);

            // 紀錄電文下行log
            // 調整紀錄下行電文執行順序 by Ares Stanley 20211202
            SaveNameCheckLog(strResult, "RTN", strCASE_NO, logPath_ESB_TG);

            // 判斷當電文下行為空白(TimeOut) || 連線錯誤
            // 增加ESB NameCheck判斷，strRspCode為"0000"才繼續執行後續程式 by Ares Stanley 20211126
            if (string.IsNullOrEmpty(strResult) || strResult == "ESB連線錯誤" || strRspCode != "0000")
            {
                if (string.IsNullOrEmpty(strResult))
                {
                    strESBMsg = "拋查名單掃描逾時";
                }
                else
                {
                    strESBMsg = "拋查名單連線異常";
                }

                // 刪除NAMECHECKLOG資料
                BRPNAMECHECKLOG.Delete(strCASE_NO, strTransactionID, ref strMsg);


                Logging.Log(strESBMsg, logPath_ESB, LogLayer.UI);
                base.sbRegScript.Append("alert('" + strESBMsg + "，請洽維護IT人員');");
                return;
            }

            #region INSERT INTO NAMECHECKLOG KIND = "R"
            // 下行Log
            Entity_NAMECHECKLOG nchecklog2 = new Entity_NAMECHECKLOG();
            nchecklog2.CASE_NO = strCASE_NO;
            nchecklog2.SEQ = "000";
            nchecklog2.REPCODE = strRspCode;
            nchecklog2.TRNNUM = strTrnNumD;
            nchecklog2.BankNo = "TW";
            nchecklog2.BranchNo = strBranchNo;
            nchecklog2.ID = "";
            nchecklog2.EnglishName = "";
            nchecklog2.NonEnglishName = "";
            //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
            //nchecklog2.EnglishName2 = "";
            //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis
            nchecklog2.DOB = "";
            nchecklog2.Nationality = "";
            nchecklog2.Type = "1";            
            nchecklog2.AddressCountry = "";
            nchecklog2.ConnectedPartyType = "";
            nchecklog2.ConnectedPartySubType = "";
            //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
            //nchecklog2.Gender = strESBGender;
            //nchecklog2.CustType = strESBCustType;            
            //nchecklog2.BroadNameSearch = strESBBroadNameSearch;
            //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis
            nchecklog2.MatchedResult = strMatchedResult;
            nchecklog2.RCScore = strRCScore;
            nchecklog2.ReferenceNumber = strReferenceNumber;
            nchecklog2.AMLReferenceNumber = strAMLReferenceNumber;
            nchecklog2.ERRORDESC = strERRORDESC;
            nchecklog2.KIND = "R";
            nchecklog2.CRE_USER = strTellerName;
            nchecklog2.CRE_DATE = System.DateTime.Now.ToShortDateString();
            BRPNAMECHECKLOG.Add(nchecklog2, ref strMsg);
            #endregion

            // 電文回電後更新畫面欄位
            if (strMatchedResult == "Y")
            {
                txtNameCheck_Note.Text = "HIT中，請更新名單掃描結果";
            }
            else if (strMatchedResult == "N")
            {
                txtNameCheck_Note.Text = "未hit 中，假警報";
            }
            else if (strMatchedResult == "E")
            {
                txtNameCheck_Note.Text = "資料內容有誤，請修改後重新發查";
            }

            // 塞資料到案件編號欄位
            if (!string.IsNullOrEmpty(strAMLReferenceNumber))
            {
                if (string.IsNullOrEmpty(txtNameCheck_No.Text))
                {
                    txtNameCheck_No.Text = strAMLReferenceNumber;
                }
                else
                {
                    txtNameCheck_No.Text = txtNameCheck_No.Text + "," + strAMLReferenceNumber;
                }
            }
            dropNameCheck_Item.SelectedValue = "5";

            //刪除SESSION
            Session.Remove("P010801030001_NAMECHECK_DATA");

            // NameCheck後 新增or更新 HQ_SCDD
            EntityHQ_SCDD_edit editObj = BRHQ_SCDD.getSCDDData_WOrk(sessionOBJ).toEditMode();
            this.GetVal<EntityHQ_SCDD_edit>(ref editObj);
            editObj.NameCheck_No = txtNameCheck_No.Text;//名單掃描案件編號
            editObj.NameCheck_Note = txtNameCheck_Note.Text;//名單掃描案件說明
            editObj.NameCheck_Item = dropNameCheck_Item.SelectedValue;//名單掃描案件項目
            editObj.NameCheck_RiskRanking = hlblNewRiskRanking.Text;//名單掃描案件風險
            editObj.Organization_Item = "";//組織運作項目
            editObj.Proof_Item = "";//存在證明
            editObj.IsSanction = "";//營業處所是否在高風險或制裁國家
            //20211018_Ares_Jack_以下欄位必須給參數不然 Add by SQL 會報錯 導致名單掃描失敗
            editObj.BusinessForeignAddress = "";//台灣以外主要之營業處所地址
            editObj.RiskObject = "";//中高風險客戶交易往來對象
            editObj.Organization_Note = "";//組織運作
            editObj.IsSanctionCountryCode1 = "";
            editObj.IsSanctionCountryCode2 = "";
            editObj.IsSanctionCountryCode3 = "";
            editObj.IsSanctionCountryCode4 = "";
            editObj.IsSanctionCountryCode5 = "";

            //加入SCDD寫DB 
            bool result;
            if (string.IsNullOrEmpty(editObj.CASE_NO))
            { //以檔頭帶入，新增
                editObj.CASE_NO = sessionOBJ.CASE_NO;
                editObj.CORP_NO = sessionOBJ.HCOP_HEADQUATERS_CORP_NO;
                result = BRHQ_SCDD.Insert(editObj.toShowMode());
            }
            else //更新
            {
                result = BRHQ_SCDD.Update(editObj.toShowMode());
            }
            if (!result)
            {
                strAlertMsg = MessageHelper.GetMessages("01_01080105_006");//編輯審查維護2更新失敗
                return;
            }
            else
            {
                if (sessionObjCount <= 11)
                {
                    base.sbRegScript.Append("alert('" + "名單掃描成功！');");
                }
                else
                {
                    base.sbRegScript.Append("alert('" + "名單掃描成功，名單筆數大於11筆，請繼續執行名單掃描！');");
                }
            }
        }
        catch (Exception ex)
        {
            // Logging.SaveLog(ELogLayer.UI, ex);
            Logging.Log(ex, logPath_ESB, LogLayer.UI);
        }
    }

    protected void btnNOTEEdit_Click(object sender, EventArgs e)
    {
        Response.Redirect("P010801100001.aspx", false);
    }

    /// <summary>
    /// ESB_NameCheck 下行電文XML拆解
    /// </summary>
    /// <param name="strResult"></param>
    /// <param name="strRspCode"></param>
    /// <param name="strTrnNumD"></param>
    /// <param name="strMatchedResult"></param>
    /// <param name="strRCScore"></param>
    /// <param name="strReferenceNumber"></param>
    /// <param name="strAMLReferenceNumber"></param>
    /// <param name="strERRORDESC"></param>
    private void ESB_RTNXml_SingleNode(string strResult, ref string strRspCode, ref string strTrnNumD, ref string strMatchedResult, ref string strRCScore, ref string strReferenceNumber, ref string strAMLReferenceNumber, ref string strERRORDESC)
    {
        // 將電文下行 拆解 String 
        XmlDocument xmldoc = PareXML(strResult);
        if (xmldoc != null)
        {
            // RTN回傳一般訊息
            XmlNamespaceManager mgr = new XmlNamespaceManager(xmldoc.NameTable);
            mgr.AddNamespace("ns0", "http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceEnvelope");
            mgr.AddNamespace("ns1", "http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceBody");
            mgr.AddNamespace("ns2", "http://ns.chinatrust.com.tw/XSD/CTCB/AML/Message/WlfNameCheckRs/02");
            XmlNode node = xmldoc.SelectSingleNode("/ns0:ServiceEnvelope/ns1:ServiceBody/ns2:WlfNameCheckRs/ns2:RESHDR", mgr);
            if (node != null)
            {
                for (int j = 0; j < node.ChildNodes.Count; j++)
                {
                    string strColumnName = node.ChildNodes[j].Name.ToString().ToUpper().Replace("NS2:", "");
                    string strValue = node.ChildNodes[j].InnerText.Trim();
                    if (strColumnName == "RSPCODE")
                    {
                        strRspCode = strValue;
                    }
                    if (strColumnName == "TRNNUM")
                    {
                        strTrnNumD = strValue;
                    }
                }
            }

            node = xmldoc.SelectSingleNode("/ns0:ServiceEnvelope/ns1:ServiceBody/ns2:WlfNameCheckRs/ns2:RESBDY/ns2:BDYREC", mgr);
            if (node != null)
            {
                for (int j = 0; j < node.ChildNodes.Count; j++)
                {
                    string strColumnName = node.ChildNodes[j].Name.ToString().ToUpper().Replace("NS2:", "");
                    string strValue = node.ChildNodes[j].InnerText.Trim();
                    if (strColumnName == "MATCHEDRESULT")
                    {
                        strMatchedResult = strValue;
                    }
                    if (strColumnName == "RCSCORE")
                    {
                        strRCScore = strValue;
                    }
                    if (strColumnName == "REFERENCENUMBER")
                    {
                        strReferenceNumber = strValue;
                    }
                    if (strColumnName == "AMLREFERENCENUMBER")
                    {
                        strAMLReferenceNumber = strValue;
                    }
                }
            }

            if (strMatchedResult.ToUpper().Trim().Equals("E"))
            {
                strERRORDESC = strRCScore + strMatchedResult + strReferenceNumber;
            }
            else
            {
                strERRORDESC = "";
            }

            // RTN回傳ERROR
            mgr = new XmlNamespaceManager(xmldoc.NameTable);
            mgr.AddNamespace("ns0", "http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceEnvelope");
            mgr.AddNamespace("ns1", "http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceHeader");
            node = xmldoc.SelectSingleNode("/ns0:ServiceEnvelope/ns1:ServiceHeader", mgr);
            if (node != null)
            {
                for (int j = 0; j < node.ChildNodes.Count; j++)
                {
                    string strColumnName = node.ChildNodes[j].Name.ToString().ToUpper().Replace("NS1:", "");
                    string strValue = node.ChildNodes[j].InnerText.Trim();
                    if (strColumnName == "TRANSACTIONID")
                    {
                        strTrnNumD = strValue;
                    }
                }
            }
            mgr.RemoveNamespace("ns1", "http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceHeader");
            mgr.AddNamespace("ns1", "http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceError");
            mgr.AddNamespace("ns2", "http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/Common");
            node = xmldoc.SelectSingleNode("/ns0:ServiceEnvelope/ns1:ServiceError/ns2:Error", mgr);
            if (node != null)
            {
                for (int j = 0; j < node.ChildNodes.Count; j++)
                {
                    string strColumnName = node.ChildNodes[j].Name.ToString().ToUpper().Replace("NS2:", "");
                    string strValue = node.ChildNodes[j].InnerText.Trim();
                    if (strColumnName == "ERRORCODE")
                    {
                        strRspCode = strValue;
                    }
                    if (strColumnName == "ERRORMESSAGE")
                    {
                        strERRORDESC = strValue;
                        strMatchedResult = "E";
                    }
                }
            }
        }
    }

    /// <summary>
    /// 將字串轉成XML
    /// </summary>
    /// <param name="strSource"></param>
    /// <returns></returns>
    public static XmlDocument PareXML(string strSource)
    {
        XmlDocument xmldoc = new XmlDocument();
        try
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = null;
            XmlReader reader = XmlReader.Create(new System.IO.StringReader(strSource), settings);
            xmldoc.Load(reader);
            return xmldoc;
        }
        catch (Exception ex)
        {
            // Logging.SaveLog(ELogLayer.UI, ex);
            Logging.Log(ex, LogLayer.UI);
            throw;
        }
    }

    /// <summary>
    /// 記錄NameCheck電文日誌;
    /// </summary>
    /// <param name="strESBSerializ">ESB電文內容</param>
    /// <param name="strNameCheckType">REQ、RTN</param>
    /// <param name="strCase_No">案件編號</param>
    /// <param name="logPath_ESB_TG">ESB電文路徑</param>
    public static void SaveNameCheckLog(string strESBSerializ, string strNameCheckType, string strCase_No, string logPath_ESB_TG)
    {
        string strMsg = "\r\n[" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss:fff") + "] ---------------- " + strNameCheckType + " [NAMECHECK" + strCase_No + "]  --------------------------\r\n";
        strMsg = strMsg + strESBSerializ;
        Logging.Log(strMsg, logPath_ESB_TG, LogState.Info);
    }

    //名單掃描記錄查詢
    protected void btnESB_NameCheck_Detail_Click(object sender, EventArgs e)
    {        
        //調整頁面轉向方法(避免造成瀏覽器雙開被剔除)
        string NavigateUrl = "P010801030002.aspx";
        Session["P010801030001_Last"] = thisPageName;
        Response.Redirect(NavigateUrl, false);

    }
    #endregion
    //20220105_Ares_JAck_修改月請款金額呈現方式
    protected void gdvBRCHPerAMT_PreRender(object sender, EventArgs e)
    {
        try
        {
            if (gdvBRCHPerAMT.DataSource != null)
            {
                AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
                if (sessionOBJ == null || eAgentInfo == null)
                {
                    string strAlertMsg = MessageHelper.GetMessages("01_01080101_002");
                    sbRegScript.Append("alert('" + strAlertMsg + "');window.location.href = 'P010801000001.aspx';");
                    return;
                }

                int objCaseNO = Convert.ToInt16(sessionOBJ.CASE_NO.Substring(4, 2));
                int mon = 0;
                for (int i = 1; i < 13; i++)
                {
                    if (objCaseNO - i > 0)
                    {
                        mon = objCaseNO - i;
                    }
                    else
                    {
                        mon = objCaseNO - i + 12;
                    }
                    if (gdvBRCHPerAMT.HeaderRow != null)
                        gdvBRCHPerAMT.HeaderRow.Cells[i].Text = mon + "月<br />請款金額";

                }
            }
        }
        catch (Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex + "年度請款金額錯誤");
        }

    }
}