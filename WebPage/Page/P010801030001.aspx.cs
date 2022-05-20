// *****************************************************************
//   作    者：林家賜
//   功能說明：AML資料 總公司資料編輯
//   創建日期：2019/02/03
//   修改記錄：
//   <author>            <time>            <TaskID>                <desc>
//   Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//   Ares Stanley      2021/04/07                                  修改ESB電文路徑
//   Ares Stanley      2021/04/08                                  調整取web.config加解密參數
//   Ares Stanley      2021/04/23                                  調整名單掃描, 電文每次發送十一筆資料
//   Ares Rick           2021/06/03                                  視窗跳轉調整 避免雙開 並新增 返回按鈕
// ******************************************************************
// 2020.06.16 Ray (U) 新增NameCheck發電文抬頭判斷

using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
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
using TIBCO.EMS;//20200423-RQ-2019-030155-005-NameCheck
using log4net;//20200423-RQ-2019-030155-005-NameCheck

public partial class Page_P010801030001 : PageBase
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
    #region  轉換代碼用字典 

    /// <summary>
    /// 通用字典，各項次機能以前兩碼區分
    /// </summary>
    Dictionary<string, string> DCCommonColl;

    #endregion

    //2021/04/07_Ares_Stanley-增加ESB LOG路徑
    private string logPath_ESB = "NameCheckInfoLog";
    private string logPath_ESB_TG = "HtgNameCheckInfoLog";

    private const string thisPageName = "P010801030001.aspx";
    #endregion

    #region 事件區

    protected void txtHCOP_CC_TextChanged(object sender, EventArgs e)
    {
        //設定行業別中文名稱
        setCCName(lblCCName.Text);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string isTEST = UtilHelper.GetAppSettings("isTest");
        if (isTEST == "Y") { isTest = true; } else { isTest = false; }

        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
        if (!IsPostBack)
        {
            //   CommonFunction.SetControlsEnabled(pnlText, false);// 清空網頁中所有的輸入欄位
            //   base.sbRegScript.Append(BaseHelper.SetFocus("txtTaxID"));// 將 總公司/總店統編 設為輸入焦點
            LoadDropDownList();
            //顯示資料，並填至畫面
            showeditDate();
        }
        //2021/03/26_Ares_Stanley-變更風險選項時重新取得中文
        txtHCOP_BUSINESS_ORGAN_TYPE.Text = this.dropSCCDOrganization.SelectedItem.Text;


    }

    /// <summary>
    /// 存檔
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
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

        //20200401 調整至上方
        //追加檢核 Talas 20191017 調整為變色方法
        //ValidVal_Nochange<EntityAML_HQ_Work_edit>(DataObj, ref errMsg, (Page)this, "");
        ValidVal<EntityAML_HQ_Work_edit>(DataObj, ref errMsg, (Page)this, "");

        //先驗證關聯欄位，主要是總公司負責人證件資料,追加 聯絡人長姓名判斷
        LinKedValid(DataObj, ref errMsg, (Page)this, "");
        //逐行取得高階經理人
        //暫存原本電文的勾選長姓名狀態
        Hashtable ManagerLnameFlag = new Hashtable();
        string _lnameFlag = "N";
        for (int i = 1; i < 13; i++)
        {
            _lnameFlag = "N";
            try
            {
                _lnameFlag = DataObj.ManagerColl[i - 1].BENE_LNAM_FLAG;
            }
            catch (Exception)
            {

            }
            ManagerLnameFlag.Add(string.Format("BENE_LNAM_FLAG{0}", i), _lnameFlag);
        }

        for (int i = 1; i < 13; i++)
        {
            EntityAML_HQ_Manager_Work_edit managerObj = null;


            if (DBDataObj.ManagerColl.Count > i - 1)
            {
                DataObj.ManagerColl[i - 1] = DBDataObj.ManagerColl[i - 1]; //DB有找到，用DB抓畫面 
                managerObj = DataObj.ManagerColl[i - 1];
            }
            else
            {
                managerObj = new EntityAML_HQ_Manager_Work_edit();
                //補上以下欄位 ，以DB補入
                managerObj.CASE_NO = DBDataObj.CASE_NO;
                managerObj.FileName = DBDataObj.FileName;
                managerObj.HCOP_BATCH_NO = DBDataObj.HCOP_BATCH_NO;
                managerObj.HCOP_INTER_ID = DBDataObj.HCOP_INTER_ID;
                managerObj.HCOP_SIXM_TOT_AMT = DBDataObj.HCOP_SIXM_TOT_AMT;
                managerObj.HCOP_KEY = DBDataObj.HCOP_KEY;
                managerObj.HCOP_BENE_RESERVED = "";//給予預設值
                managerObj.HCOP_BENE_OTH_CERT = ""; //給予預設值
                managerObj.HCOP_BENE_OTH_CERT_EXP = "";//給予預設值
                managerObj.HCOP_BENE_PASSPORT_EXP = "";//給予預設值,20191209-因畫面已無護照效期，故帶空白，否則會錯 
                managerObj.HCOP_BENE_RESIDENT_EXP = "";//給予預設值,20191209-因畫面已無護照效期，故帶空白，否則會錯 
                managerObj.ID = "";
                managerObj.HCOP_BENE_LNAME = ""; //長姓名
                managerObj.HCOP_BENE_ROMA = ""; //羅馬拼音
                DataObj.ManagerColl[i - 1] = managerObj; //將物件指回指標
            }

            managerObj.LineID = i.ToString();

            this.GetVal<EntityAML_HQ_Manager_Work_edit>(ref managerObj);
            //更新使用者及更新時間
            managerObj.Create_User = Create_User;
            managerObj.Create_Time = CreateTime;
            managerObj.Create_Date = CreateDate;
            bool isAny = false;
            //處理Checkbox
            CustCheckBox ck1 = FindControl("chkHCOP_BENE_JOB_TYPE_" + i.ToString()) as CustCheckBox;
            CustCheckBox ck2 = FindControl("chkHCOP_BENE_JOB_TYPE_2_" + i.ToString()) as CustCheckBox;
            CustCheckBox ck3 = FindControl("chkHCOP_BENE_JOB_TYPE_3_" + i.ToString()) as CustCheckBox;
            CustCheckBox ck4 = FindControl("chkHCOP_BENE_JOB_TYPE_4_" + i.ToString()) as CustCheckBox;
            CustCheckBox ck5 = FindControl("chkHCOP_BENE_JOB_TYPE_5_" + i.ToString()) as CustCheckBox;
            CustCheckBox ck6 = FindControl("chkHCOP_BENE_JOB_TYPE_6_" + i.ToString()) as CustCheckBox;
            managerObj.HCOP_BENE_JOB_TYPE = GetCheckboxval(ck1, ref isAny);
            managerObj.HCOP_BENE_JOB_TYPE_2 = GetCheckboxval(ck2, ref isAny);
            managerObj.HCOP_BENE_JOB_TYPE_3 = GetCheckboxval(ck3, ref isAny);
            managerObj.HCOP_BENE_JOB_TYPE_4 = GetCheckboxval(ck4, ref isAny);
            managerObj.HCOP_BENE_JOB_TYPE_5 = GetCheckboxval(ck5, ref isAny);
            managerObj.HCOP_BENE_JOB_TYPE_6 = GetCheckboxval(ck6, ref isAny);

            CustCheckBox ckLname = FindControl("chkBENE_LNAM_FLAG_" + i.ToString()) as CustCheckBox;
            managerObj.BENE_LNAM_FLAG = GetCheckboxval(ckLname, ref isAny);

            //20200611-RQ-2019-030155-000
            CustCheckBox cb1 = FindControl("chkIDType1_" + i.ToString()) as CustCheckBox;
            CustCheckBox cb3 = FindControl("chkIDType3_" + i.ToString()) as CustCheckBox;
            CustCheckBox cb4 = FindControl("chkIDType4_" + i.ToString()) as CustCheckBox;
            CustCheckBox cb7 = FindControl("chkIDType7_" + i.ToString()) as CustCheckBox;
            CustTextBox txtBENE_ID = FindControl("txtHCOP_BENE_ID_" + i.ToString()) as CustTextBox;
            CustTextBox txtBENE_NAME = FindControl("txtHCOP_BENE_NAME_" + i.ToString()) as CustTextBox;
            CustTextBox txtBENE_NATION = FindControl("txtHCOP_BENE_NATION_" + i.ToString()) as CustTextBox;//20200723 加強檢核

            //先將原資料庫欄位值清空
            managerObj.HCOP_BENE_ID = "";
            managerObj.HCOP_BENE_PASSPORT = "";
            managerObj.HCOP_BENE_RESIDENT_NO = "";
            managerObj.HCOP_BENE_OTH_CERT = "";

            int chekSum = 0;

            if (cb1.Checked)//身分證號
            {
                chekSum++;
                managerObj.HCOP_BENE_ID = txtBENE_ID.Text.Trim();
            }
            if (cb3.Checked)//護照號碼
            {
                chekSum++;
                managerObj.HCOP_BENE_PASSPORT = txtBENE_ID.Text.Trim();
            }
            if (cb4.Checked)//統一証號
            {
                chekSum++;
                managerObj.HCOP_BENE_RESIDENT_NO = txtBENE_ID.Text.Trim();
            }
            if (cb7.Checked)//其他證件號
            {
                chekSum++;
                managerObj.HCOP_BENE_OTH_CERT = txtBENE_ID.Text.Trim();
            }

            //20200723-加強檢核，同AML鍵檔
            //如果是身分證件類型1 or 4，則身分證件號碼一定要是10碼
            if ((cb1.Checked || cb4.Checked) && txtBENE_ID.Text.Trim().Length != 10)
            {
                errMsg.Add("身分證件類型1或4時，身分證件號碼須為10碼，第" + i.ToString() + "行\\n");
            }

            if (txtBENE_NATION.Text.Trim().Equals("TW") && !(cb1.Checked || cb7.Checked))
            {
                errMsg.Add("國籍為TW，身分證件類型須為1或7，第" + i.ToString() + "行\\n");
            }

            //追加驗證 Talas 20191017 調整為變色方法
            // ValidVal_Nochange<EntityAML_HQ_Manager_Work_edit>(managerObj, ref errMsg, (Page)this, managerObj.LineID);
            ValidVal<EntityAML_HQ_Manager_Work_edit>(managerObj, ref errMsg, (Page)this, managerObj.LineID);

            LinKedValidLine(managerObj, ref errMsg, (Page)this, managerObj.LineID);
            //追加國籍與姓名轉換處理
            checkNation(managerObj, ref errMsg, managerObj.LineID);
            //追加 長姓名 檢核
            checkLName(managerObj, ref errMsg, managerObj.LineID);
            //20220104 檢核 國籍TW 身分證為Z999999999, 身分證指定為1 br Ares Dennis
            if (!checkIdNoType(i.ToString()))
            {
                errMsg.Add("高階經理人第 " + i.ToString() + "行身分證類型輸入值有誤\\n");
            }
            //檢核身分類型
            if (!string.IsNullOrEmpty(managerObj.HCOP_BENE_NATION) && !isAny)
            {
                errMsg.Add(MessageHelper.GetMessages("01_01080103_024") + "，第" + i.ToString() + "行\\n");
            }
            //20200611-RQ-2019-030155-000-檢核只能勾選一種身分證件類型
            // 2020.07.02 Ray Add 當有填ID欄位再做判斷
            if (chekSum > 1 && !string.IsNullOrEmpty(txtBENE_ID.Text))
            {
                errMsg.Add(MessageHelper.GetMessages("01_01080103_032") + "，第" + i.ToString() + "行\\n");
            }

            // 2020.07.02 Ray Add 當有填ID欄位卻沒勾身分證件類型
            if (chekSum == 0 && !string.IsNullOrEmpty(txtBENE_ID.Text))
            {
                errMsg.Add(MessageHelper.GetMessages("01_01080103_033") + "，第" + i.ToString() + "行\\n");
            }
            // 2020.07.02 Peggy Add 當有姓名卻沒勾身分證件類型
            if (chekSum == 0 && !string.IsNullOrEmpty(txtBENE_NAME.Text))
            {
                errMsg.Add(MessageHelper.GetMessages("01_01080103_033") + "，第" + i.ToString() + "行\\n");
            }
            //有輸入姓名，身分證號碼不可為空白
            if (!string.IsNullOrEmpty(txtBENE_NAME.Text) && string.IsNullOrEmpty(txtBENE_ID.Text))
            {
                errMsg.Add(MessageHelper.GetMessages("01_01080103_034") + "，第" + i.ToString() + "行\\n");
            }

            //20201102-RQ-2020-021027-003 外國人新式證號檢核
            if (cb4.Checked && !CheckResidentID(txtBENE_ID.Text.Trim()))
            {
                errMsg.Add("統一證號輸入錯誤，第" + i.ToString() + "行\\n");
            }

            //20220106_Ares_Jack_國籍不得輸入無
            if (!checkCountry(i.ToString()))
            {
                errMsg.Add("高階經理人第 " + i.ToString() + "行 國籍不得輸入無\\n");
            }

            //20211029_Ares_Jack_虛擬ID延後上線, 先還原Z999999999↓
            ////20210630_Ares_Stanley-其他身分證檢核
            //if (cb7.Checked && !CheckResidentID_SeniorManager(txtBENE_ID.Text.Trim(), "7")) 
            //{
            //    errMsg.Add("其他身分證件號碼輸入錯誤" + i.ToString() + "行\\n");
            //}

            //if(txtBENE_ID.Text.Trim() != "")// 身分證件號碼不為空
            //{
            //    //身分證類型不是其他(7)開頭不能是CA
            //    if (!cb7.Checked && txtBENE_ID.Text.Trim().Substring(0, 2) == "CA")
            //    {
            //        errMsg.Add("身分證類型為其他才可使用CA開頭的ID" + i.ToString() + "行\\n");
            //    }
            //}
            //20211029_Ares_Jack_虛擬ID延後上線, 先還原Z999999999↑

            /*20200330 因驗證會將底色先還原，故拉至上方先做基本檢核
            //追加驗證 Talas 20191017 調整為變色方法
           // ValidVal_Nochange<EntityAML_HQ_Manager_Work_edit>(managerObj, ref errMsg, (Page)this, managerObj.LineID);
            ValidVal<EntityAML_HQ_Manager_Work_edit>(managerObj, ref errMsg, (Page)this, managerObj.LineID);
            */
        }

        /*20200330 因驗證會將底色先還原，故拉至上方先做基本檢核
        //追加檢核 Talas 20191017 調整為變色方法
        //ValidVal_Nochange<EntityAML_HQ_Work_edit>(DataObj, ref errMsg, (Page)this, "");
        ValidVal<EntityAML_HQ_Work_edit>(DataObj, ref errMsg, (Page)this, "");
        //特殊關聯欄位處理  先保留不進行驗證，原因為非所有欄位均可編輯，會造成邏輯檢核異常
        //ValidLinkedFields(DataObj, ref errMsg);
        */

        //加入SCDD資料收集，及驗證
        //檢查取回畫面值後公司統編與案號是否為空值，若為空，表示為新增，反之為修改
        EntityHQ_SCDD_edit editObj = BRHQ_SCDD.getSCDDData_WOrk(sessionOBJ).toEditMode();
        this.GetVal<EntityHQ_SCDD_edit>(ref editObj);
        //特殊欄位處理--DP
        getDPValue(ref editObj);
        //關聯欄位驗證
        LinKedValidSCDD(editObj, ref errMsg, (Page)this, "");
        //追加驗證 Talas 20191017 調整為變色方法
        //ValidVal_Nochange<EntityHQ_SCDD_edit>(editObj, ref errMsg, (Page)this, "");
        ValidVal<EntityHQ_SCDD_edit>(editObj, ref errMsg, (Page)this, "");


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
        //insObj.ManagerColl = edObjT.toShowMode().ManagerColl;


        EntityAML_HQ_Work insObj = DataObj.toShowMode();
        bool result;

        using (OMTransactionScope ts = new OMTransactionScope())
        {
            //總公司資料寫入
            result = BRAML_HQ_Work.UpdateEdit(insObj);
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
                strAlertMsg = MessageHelper.GetMessages("01_01080105_006");
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
                                                       //JC66OBj["CORP_SEQ"]= "0000";             //資料庫有此欄位
                                                       //上送主機資料
                                                       //Hashtable hstExmsP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC66, JC66OBj, false, "11", eAgentInfo);
            JC66OBj.Add("LAST_UPDATE_DATE", DateTime.Now.ToString("yyyyMMdd"));//資料最後異動日期
            //////測試用模擬資料
            ////  Hashtable hstExmsP4A = getTestData();
            //201090625 Talas 追加檢核，若母公司國別為無，則變成空
            if (JC66OBj.ContainsKey("OVERSEAS_FOREIGN_COUNTRY"))
            {
                if (JC66OBj["OVERSEAS_FOREIGN_COUNTRY"].ToString() == "無")
                {
                    JC66OBj["OVERSEAS_FOREIGN_COUNTRY"] = "";
                }
            }

            //-------------------------------------------------------            
            //在轉成 Hastable 時就已經判斷是否為長姓名, 所以後面就直接拿來用, 不用再拿畫面的值(中文長姓名,羅馬拼音)重新判斷是否為長姓名                        
            List<EntityHTG_JC68> _JC68s = new List<EntityHTG_JC68>();
            EntityHTG_JC68 _tmpJC68 = new EntityHTG_JC68();
            bool _needSendJC68 = true;
            //編輯時, 只有聯絡人, 沒有負責人
            //畫面上有勾選 長姓名時
            if (JC66OBj.ContainsKey("CONTACT_LNAM_FLAG") == true && JC66OBj["CONTACT_LNAM_FLAG"].ToString() == "Y")
            {
                _tmpJC68.LONG_NAME = DataObj.HCOP_CONTACT_LNAME;
            }
            else
            {
                //若沒有勾選聯絡人長姓名時需再判斷原本電文-聯絡人長姓名指標是否為長姓名, 若是的話, 就要送出
                if (DataObj.CONTACT_LNAM_FLAG == "Y")
                {
                    _tmpJC68.LONG_NAME = "";
                    //原本是給原本的姓名欄位值
                    //_tmpJC68.LONG_NAME = ToWide(DataObj.HCOP_CONTACT_NAME);
                }
                else
                {
                    _needSendJC68 = false;
                }
            }
            if (_needSendJC68)
            {
                _tmpJC68.ID = string.Format("{0}{1}", DataObj.HCOP_HEADQUATERS_CORP_NO, DataObj.HCOP_HEADQUATERS_CORP_SEQ);
                _tmpJC68.PINYIN_NAME = DataObj.HCOP_CONTACT_ROMA;
                _JC68s.Add(_tmpJC68);
            }

            //判斷高管為長姓名時才需要送電文
            //原本是判斷為長姓名時才送電文, 但有可能改短, 所以變成只要原本姓名欄有值,就需送電文, 只是再由後面判斷要不要把NAME 放到 LONG_NAME 裡
            string _tmpKey = string.Empty;

            foreach (EntityAML_HQ_Manager_Work_edit item in DataObj.ManagerColl)
            {
                _tmpJC68 = new EntityHTG_JC68();
                _tmpJC68.LONG_NAME = item.HCOP_BENE_LNAME;
                _tmpKey = string.Format("BENE_LNAM_FLAG{0}", item.LineID);
                _needSendJC68 = true;

                //若無勾選長姓名時,需再進一步判斷
                //if (JC66OBj.ContainsKey(_tmpKey) == true && JC66OBj[_tmpKey].ToString() != "Y")//20200507-P修正抓畫面資料
                if (!item.BENE_LNAM_FLAG.Trim().Equals("Y"))
                {
                    //若原本是長姓名, 而現行已不是長姓名,但還是需要更新資料, 所以給予空白送主機,主機會刪除此筆
                    if (ManagerLnameFlag.ContainsKey(_tmpKey) == true && ManagerLnameFlag[_tmpKey].ToString() == "Y")
                    {
                        //_tmpJC68.LONG_NAME = ToWide(item.HCOP_BENE_NAME); //原本姓名欄位
                    }
                    else
                    {
                        _needSendJC68 = false;
                    }
                }

                if (_needSendJC68)
                {
                    _tmpJC68.LineID = item.LineID;
                    _tmpJC68.ID = this.getHQ_ID(item);

                    _tmpJC68.PINYIN_NAME = item.HCOP_BENE_ROMA;

                    _JC68s.Add(_tmpJC68);
                }
            }
            //-------------------------------------------------------

            Hashtable hstExmsP4A;
            if (!isTest)
            {
                //上送主機資料
                hstExmsP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC66, JC66OBj, false, "11", eAgentInfo);

                //當JC66異動無誤時才會更新JC68
                if (hstExmsP4A["MESSAGE_TYPE"].ToString() == "0000")
                {   //長姓名:執行 BRHGT_JC68
                    using (BRHTG_JC68 obj = new BRHTG_JC68(sPageInfo.strPageCode))
                    {
                        EntityResult _Entityresult = new EntityResult();
                        List<string> _jc68Msg = new List<string>();
                        foreach (EntityHTG_JC68 item in _JC68s)
                        {
                            _Entityresult = obj.Update(item, this.eAgentInfo, "12");
                            if (_Entityresult.Success == false)
                            {
                                _jc68Msg.Add(string.Format("第{0}行:{1}", item.LineID, _Entityresult.HostMsg));
                            }
                        }

                        //有異常訊息時
                        if (_jc68Msg.Count > 0)
                        {
                            hstExmsP4A["MESSAGE_TYPE"] = "2068";

                            if (hstExmsP4A.ContainsKey("HtgMsg"))
                            {
                                hstExmsP4A["HtgMsg"] = hstExmsP4A["HtgMsg"].ToString() + string.Join(",", _jc68Msg.ToArray());
                            }
                        }
                    }
                }

            }
            else
            {
                //測試用模擬資料
                hstExmsP4A = BuildTestHashTB();
                //Talas 20190920 增加輸出到D:\比對用
                StringBuilder sb = new StringBuilder();
                foreach (string ekey in JC66OBj.Keys)
                {
                    sb.AppendLine(ekey + "<!>" + JC66OBj[ekey].ToString());
                }
                File.AppendAllText(@"D:\JC66Up.txt", sb.ToString());
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

        string lastPage = Session["P010801010001_Last"].ToString();
        string urlStringA = @"alert('" + strAlertMsg + @"');location.href='" + lastPage + "';";
        base.sbRegScript.Append(urlStringA);

    }
    //產生測試用HSAHTABLE 模擬電文回傳
    private Hashtable BuildTestHashTB()
    {
        Hashtable rtn = new Hashtable();
        rtn.Add("MESSAGE_TYPE", "0000");
        return rtn;
    }
    /// <summary>
    /// 註冊國別變更檢核
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtHCOP_REGISTER_NATION_TextChanged(object sender, EventArgs e)
    {
        //Talas 20190919 修正將顏色還原
        //txtHCOP_REGISTER_NATION.BackColor = Color.White;
        txtHCOP_REGISTER_NATION.BackColor = default(Color);
        DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID("1", txtHCOP_REGISTER_NATION.Text);
        if (result.Rows.Count > 0 && CheckCodeType(result))
        {
        }
        else
        {
            txtHCOP_REGISTER_NATION.BackColor = Color.Red;
        }
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
        if (editObj.Organization_Item == "2" && string.IsNullOrEmpty(txtOrganization_Note.Text))
        {
            strAlertMsg = MessageHelper.GetMessages("01_01080105_010");
            errMsg.Add(strAlertMsg + "\\n");
        }
        //追加看字，若選其他，則NOTE必填
        if (dropNameCheck_Item.SelectedItem.Text == "其他")
        {
            if (string.IsNullOrEmpty(txtNameCheck_Note.Text))
            {
                strAlertMsg = MessageHelper.GetMessages("01_01080105_011");
                errMsg.Add(strAlertMsg + "\\n");
            }
        }

        //Talas 20190613 增加行業別檢核，若為空白，則請重新輸入
        if (lblCCName.Text == "")
        {
            strAlertMsg = MessageHelper.GetMessages("01_01080105_012");
            errMsg.Add(strAlertMsg + "\\n");
        }
        //20190625 Talas 追加檢核，若所在國別為高風險國家，則國家1為必填
        if (dropSCCDIsSanction.SelectedValue == "Y")
        {
            if (string.IsNullOrEmpty(txtIsSanctionCountryCode1.Text))
            {
                strAlertMsg = MessageHelper.GetMessages("01_01080105_013");
                errMsg.Add(strAlertMsg + "\\n");
            }
        }

        //追加檢核 若非高風險國家，但國家1有植，則提示 Talas 20190717
        if (dropSCCDIsSanction.SelectedValue == "N")
        {
            if (!string.IsNullOrEmpty(txtIsSanctionCountryCode1.Text))
            {
                strAlertMsg = MessageHelper.GetMessages("01_01080105_014");
                errMsg.Add(strAlertMsg + "\\n");
            }
        }
        //2020.04.22 -RQ-2019-030155-005-Ray Add 判斷NameCheck訊息顯示為HIT中，則結果不能為請選擇 or 其他
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
        editObj.IsSanction = dropSCCDIsSanction.SelectedValue;
        editObj.Organization_Item = dropOrganization_Item.SelectedValue;
        editObj.Proof_Item = dropProof_Item.SelectedValue;

        //調整為看值 2為高風險，3為中風險，其他或結案都為低風險
        if (dropNameCheck_Item.SelectedIndex != -1)
        {
            //editObj.NameCheck_RiskRanking = rislk[dropNameCheck_Item.SelectedIndex];
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
        //設立日期20200327 日期檢核
        if (!string.IsNullOrEmpty(txtHCOP_BUILD_DATE.Text.Trim()) && !checkDateTime(txtHCOP_BUILD_DATE.Text.Trim(), "V"))
        {
            errMsg.Add("設立日期格式錯誤\\n");
            txtHCOP_BUILD_DATE.BackColor = Color.Red;//20200401
        }
        //出生日期20200327 日期檢核
        if (!string.IsNullOrEmpty(txtHCOP_OWNER_BIRTH_DATE.Text.Trim()) && !checkDateTime(txtHCOP_OWNER_BIRTH_DATE.Text.Trim(), "V"))
        {
            errMsg.Add("總公司負責人出生日期格式錯誤\\n");
            txtHCOP_OWNER_BIRTH_DATE.BackColor = Color.Red;//20200401
        }
        ///國籍為TW時，發證日期，發證地點，領補換別不可空白
        if (txtHCOP_OWNER_NATION.Text.ToUpper() == "TW")
        {
            if (string.IsNullOrEmpty(txtHCOP_OWNER_ID_ISSUE_DATE.Text))
            {
                errMsg.Add(MessageHelper.GetMessages("01_01080103_011") + "\\n");
                txtHCOP_OWNER_ID_ISSUE_DATE.BackColor = Color.Red;//20200401
            }
            else
            {
                //20200327 日期檢核
                if (!checkDateTime(txtHCOP_OWNER_ID_ISSUE_DATE.Text.Trim(), "C"))
                {
                    errMsg.Add("身分證發證日期格式錯誤\\n");
                    txtHCOP_OWNER_ID_ISSUE_DATE.BackColor = Color.Red;//20200401
                }
            }
            if (string.IsNullOrEmpty(txtHCOP_OWNER_ID_ISSUE_PLACE.Text))
            {
                errMsg.Add(MessageHelper.GetMessages("01_01080103_012") + "\\n");
                txtHCOP_OWNER_ID_ISSUE_PLACE.BackColor = Color.Red;//20200401
            }
            if (string.IsNullOrEmpty(dropHCOP_OWNER_ID_REPLACE_TYPE.SelectedValue))
            {
                errMsg.Add(MessageHelper.GetMessages("01_01080103_013") + "\\n");
                dropHCOP_OWNER_ID_REPLACE_TYPE.BackColor = Color.Red;//20200401
            }
        }
        else
        {
            //Talas 20190919 調整護照到期日及居留證號到期日不檢核
            ////有護照號碼，護照到期日不可空白
            //if (!string.IsNullOrEmpty(HQlblHCOP_PASSPORT.Text) && string.IsNullOrEmpty(txtHCOP_PASSPORT_EXP_DATE.Text))
            //{
            //    errMsg.Add(MessageHelper.GetMessages("01_01080103_014") + "\\n");
            //}
            ////有居留證號，居留證到期日不可空白
            //if (!string.IsNullOrEmpty(HQlblHCOP_RESIDENT_NO.Text) && string.IsNullOrEmpty(txtHCOP_RESIDENT_EXP_DATE.Text))
            //{
            //    errMsg.Add(MessageHelper.GetMessages("01_01080103_015") + "\\n");
            //}
            //居留證效期20200327 日期檢核
            if (!string.IsNullOrEmpty(txtHCOP_PASSPORT_EXP_DATE.Text.Trim()) && !checkDateTime(txtHCOP_PASSPORT_EXP_DATE.Text.Trim(), "V"))
            {
                errMsg.Add("護照效期格式錯誤\\n");
                txtHCOP_PASSPORT_EXP_DATE.BackColor = Color.Red;//20200401
            }
            //居留證效期20200327 日期檢核
            if (!string.IsNullOrEmpty(txtHCOP_RESIDENT_EXP_DATE.Text.Trim()) && !checkDateTime(txtHCOP_RESIDENT_EXP_DATE.Text.Trim(), "V"))
            {
                errMsg.Add("統一證號效期格式錯誤\\n");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                txtHCOP_RESIDENT_EXP_DATE.BackColor = Color.Red;//20200401
            }
        }

        //聯絡人的長姓名規範判斷
        if (radHCOP_CONTACT_LNAMEY.Checked == true)
        {
            string tmpMsg = valideLnameRoma(editObj.HCOP_CONTACT_LNAME, editObj.HCOP_CONTACT_ROMA);
            if (string.IsNullOrEmpty(tmpMsg) == false)
            {
                errMsg.Add(string.Format("聯絡人:{0}\\n", tmpMsg));
            }
        }

        //20200117-調整國籍非US時，州別不得有值
        if (!editObj.HCOP_REGISTER_NATION.Trim().Equals("US") && !editObj.HCOP_REGISTER_US_STATE.Trim().Equals(""))
        {
            errMsg.Add(MessageHelper.GetMessages("01_01080103_027") + "\\n");
        }

        //20200330
        if (!ChkOWNER_ID_SreachStatusY.Checked && !ChkOWNER_ID_SreachStatusN.Checked)
        {
            errMsg.Add(MessageHelper.GetMessages("01_01080103_028") + "\\n");
        }

        //20220106_Ares_Jack_國籍不得輸入無
        if (txtHCOP_REGISTER_NATION.Text.Trim().Equals("無"))
        {
            errMsg.Add("註冊國籍不得輸入無\\n");
        }
        if (this.txtHCOP_OWNER_NATION.Text.Trim().Equals("無"))
        {
            errMsg.Add("總公司負責人國籍不得輸入無\\n");
        }
        if (this.txtHCOP_PRIMARY_BUSI_COUNTRY.Text.Trim().Equals("無"))
        {
            errMsg.Add("主要營業處所國別不得輸入無\\n");
        }
    }

    /// <summary>
    /// 驗證連動欄位
    /// </summary>
    /// <param name="editObj"></param>
    /// <param name="errMsg"></param>
    /// <param name="page"></param>
    /// <param name="v"></param>
    private void LinKedValidLine(EntityAML_HQ_Manager_Work_edit editObj, ref List<string> errMsg, Page page, string LinID)
    {
        CustTextBox CBID = FindControl("txtHCOP_BENE_ID_" + LinID) as CustTextBox; //身分證字號
        CustTextBox CBNation = FindControl("txtHCOP_BENE_NATION_" + LinID) as CustTextBox; //國籍
        //CustTextBox CBPASSPORT = FindControl("txtHCOP_BENE_PASSPORT_" + LinID) as CustTextBox; //護照號碼
        //CustTextBox CBPASSPORT_EXP = FindControl("txtHCOP_BENE_PASSPORT_EXP_" + LinID) as CustTextBox; //護照效期
        //CustTextBox CBRESIDENT_NO = FindControl("txtHCOP_BENE_RESIDENT_NO_" + LinID) as CustTextBox; //居留證號碼
        //CustTextBox CBRESIDENT_EXP = FindControl("txtHCOP_BENE_RESIDENT_EXP_" + LinID) as CustTextBox; //居留證效期
        CustTextBox CBBirthDate = FindControl("txtHCOP_BENE_BIRTH_DATE_" + LinID) as CustTextBox; //出生日期,20200330

        //20200327 日期檢核
        if (!string.IsNullOrEmpty(CBBirthDate.Text.Trim()) && !checkDateTime(CBBirthDate.Text.Trim(), "V"))
        {
            errMsg.Add("【高管】第" + LinID + "行，出生日期格式錯誤\\n");
            CBBirthDate.BackColor = Color.Red;
        }

        ///國籍為TW時，身分證字號 不可空白
        if (CBNation.Text.ToUpper() == "TW")
        {
            if (string.IsNullOrEmpty(CBID.Text))
            {
                errMsg.Add(MessageHelper.GetMessages("01_01080103_016") + "第" + LinID + "行\\n");
                CBID.BackColor = Color.Red;
            }
        }

        //Talas 20190919 調整護照到期日及居留證號到期日不檢核
        //else
        //{
        //    //有護照號碼，護照到期日不可空白
        //    if (!string.IsNullOrEmpty(CBPASSPORT.Text) && string.IsNullOrEmpty(CBPASSPORT_EXP.Text))
        //    {
        //        errMsg.Add(MessageHelper.GetMessages("01_01080103_017") + "第" + LinID + "行\\n");
        //    }
        //    //有居留證號，居留證到期日不可空白
        //    if (!string.IsNullOrEmpty(CBRESIDENT_NO.Text) && string.IsNullOrEmpty(CBRESIDENT_EXP.Text))
        //    {
        //        errMsg.Add(MessageHelper.GetMessages("01_01080103_018") + "第" + LinID + "行\\n");
        //    }
        //}
    }
    /// <summary>
    /// 實作字典
    /// </summary>
    private void buiInfoDict()
    {
        DCCommonColl = new Dictionary<string, string>();
        // 通用項目
        DCCommonColl.Add("YN_1", "是");
        DCCommonColl.Add("YN_0", "否");
        DCCommonColl.Add("YN_Y", "是");
        DCCommonColl.Add("YN_N", "否");
        DCCommonColl.Add("HS_1", "有");
        DCCommonColl.Add("HS_0", "無");
        DCCommonColl.Add("HS_Y", "有");
        DCCommonColl.Add("HS_N", "無");

        DCCommonColl.Add("YN1_是", "1");
        DCCommonColl.Add("YN1_否", "0");
        DCCommonColl.Add("YNY_是", "Y");
        DCCommonColl.Add("YNY_否", "N");
        DCCommonColl.Add("HS1_有", "1");
        DCCommonColl.Add("HS1_無", "0");
        DCCommonColl.Add("HSY_有", "Y");
        DCCommonColl.Add("HSY_無", "N");
        //開關 OC_ 
        DCCommonColl.Add("OC_O", "OPEN");
        DCCommonColl.Add("OC_C", "Close");
        DCCommonColl.Add("OC_OPEN", "O");
        DCCommonColl.Add("OC_Close", "C");
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
    /// HQ特殊欄位處理 收集資料
    /// </summary>
    /// <param name="dataObj"></param>
    private void CollectSPField(ref EntityAML_HQ_Work_edit dataObj)
    {
        //20211203_Ares_Jack_ MAKER = 登入者ID, CHECKER = "", BRANCH = "9999"
        dataObj.HCOP_LAST_UPD_MAKER = eAgentInfo.agent_id;//登入者ID
        dataObj.HCOP_LAST_UPD_CHECKER = "";
        dataObj.HCOP_LAST_UPD_BRANCH = "9999";

        //給予預設值
        dataObj.OWNER_ID_SreachStatus = "";
        //負責人ID領補換查詢結果
        if (ChkOWNER_ID_SreachStatusY.Checked == true)
        {
            dataObj.OWNER_ID_SreachStatus = "Y";
        }
        if (ChkOWNER_ID_SreachStatusN.Checked == true)
        {
            dataObj.OWNER_ID_SreachStatus = "N";
        }
        dataObj.HCOP_STATUS = GetDcValue("OC_" + HQlblHCOP_STATUS.Text);
        //複雜股權結構
        if (radHCOP_COMPLEX_STR_CODEY.Checked == true)
        {
            dataObj.HCOP_COMPLEX_STR_CODE = "Y";
        }
        else
        {
            dataObj.HCOP_COMPLEX_STR_CODE = "N";
        }

        //是否可發行無記名股票 
        if (radHCOP_ALLOW_ISSUE_STOCK_FLAGY.Checked == true)
        {
            dataObj.HCOP_ALLOW_ISSUE_STOCK_FLAG = "Y";
        }
        else
        {
            dataObj.HCOP_ALLOW_ISSUE_STOCK_FLAG = "N";
        }
        //是否已發行無記名股票 
        if (radHCOP_ISSUE_STOCK_FLAGY.Checked == true)
        {
            dataObj.HCOP_ISSUE_STOCK_FLAG = "Y";
        }
        else
        {
            dataObj.HCOP_ISSUE_STOCK_FLAG = "N";
        }
        // 僑外資 / 外商 
        if (radHCOP_OVERSEAS_FOREIGNY.Checked == true)
        {
            dataObj.HCOP_OVERSEAS_FOREIGN = "Y";
        }
        else
        {
            dataObj.HCOP_OVERSEAS_FOREIGN = "N";
        }
        //有無照片 特殊規則 0 有照片，1無照片
        if (radHasPhoto.Checked == true)
        {
            dataObj.HCOP_ID_PHOTO_FLAG = "0";
        }
        else
        {
            dataObj.HCOP_ID_PHOTO_FLAG = "1";
        }
        dataObj.HCOP_OWNER_ID_REPLACE_TYPE = dropHCOP_OWNER_ID_REPLACE_TYPE.SelectedValue;

        //轉回西元年
        if (dataObj.HCOP_OWNER_ID_ISSUE_DATE.Length == 7)
        {
            dataObj.HCOP_OWNER_ID_ISSUE_DATE = ConvertToDC(dataObj.HCOP_OWNER_ID_ISSUE_DATE);
        }
        //護照效期轉回西元年
        if (dataObj.HCOP_PASSPORT_EXP_DATE.Length == 7)
        {
            dataObj.HCOP_PASSPORT_EXP_DATE = ConvertToDC(dataObj.HCOP_PASSPORT_EXP_DATE);
        }
        //居留證效期轉回西元年
        if (dataObj.HCOP_RESIDENT_EXP_DATE.Length == 7)
        {
            dataObj.HCOP_RESIDENT_EXP_DATE = ConvertToDC(dataObj.HCOP_RESIDENT_EXP_DATE);
        }

        //20190614 Talas 調整美國州別，法律形式因為修改為中文，所以值要重設為選取VALUE，而非TEXT
        dataObj.HCOP_REGISTER_US_STATE = dropBasicCountryStateCode.SelectedItem.Value;
        dataObj.HCOP_BUSINESS_ORGAN_TYPE = dropSCCDOrganization.SelectedItem.Value;
    }
    /// <summary>
    /// HQ特殊欄位處理
    /// </summary>
    /// <param name="edObj"></param>
    private void setHQSpcial(EntityAML_HQ_Work_edit edObj)
    {
        //建立顯示用字典
        buiInfoDict();
        //公司狀態
        HQlblHCOP_STATUS.Text = GetDcValue("OC_" + edObj.HCOP_STATUS);

        //設定行業別
        if (!string.IsNullOrEmpty(edObj.HCOP_CC))
        {
            setCCName(edObj.HCOP_CC);
        }
        //複雜股權結構
        if (edObj.HCOP_COMPLEX_STR_CODE == "Y")
        {
            radHCOP_COMPLEX_STR_CODEY.Checked = true;
        }
        else
        {
            radHCOP_COMPLEX_STR_CODEN.Checked = true;
        }
        //是否可發行無記名股票
        if (edObj.HCOP_ALLOW_ISSUE_STOCK_FLAG == "Y")
        {
            radHCOP_ALLOW_ISSUE_STOCK_FLAGY.Checked = true;
        }
        else
        {
            radHCOP_ALLOW_ISSUE_STOCK_FLAGN.Checked = true;
        }
        //是否已發行無記名股票
        if (edObj.HCOP_ISSUE_STOCK_FLAG == "Y")
        {
            radHCOP_ISSUE_STOCK_FLAGY.Checked = true;
        }
        else
        {
            radHCOP_ISSUE_STOCK_FLAGN.Checked = true;

        }
        //僑外資/外商 
        if (edObj.HCOP_OVERSEAS_FOREIGN == "Y")
        {
            radHCOP_OVERSEAS_FOREIGNY.Checked = true;
        }
        else
        {
            radHCOP_OVERSEAS_FOREIGNN.Checked = true;

        }
        //有無照片，目前規則為 0有，1無
        if (edObj.HCOP_ID_PHOTO_FLAG == "0")
        {
            radHasPhoto.Checked = true;
        }
        else
        {
            radNoPhoto.Checked = true;
        }
        foreach (ListItem litme in dropHCOP_OWNER_ID_REPLACE_TYPE.Items)
        {
            if (edObj.HCOP_OWNER_ID_REPLACE_TYPE == litme.Value)
            {
                litme.Selected = true;
                break;
            }
        }
        if (edObj.OWNER_ID_SreachStatus == "Y")
        {
            ChkOWNER_ID_SreachStatusY.Checked = true;
        }
        if (edObj.OWNER_ID_SreachStatus == "N")
        {
            ChkOWNER_ID_SreachStatusN.Checked = true;
        }

        //發證日期轉民國年
        if (edObj.HCOP_OWNER_ID_ISSUE_DATE.Length == 8)
        {
            txtHCOP_OWNER_ID_ISSUE_DATE.Text = ConvertToROCYear(txtHCOP_OWNER_ID_ISSUE_DATE.Text);
        }
        ////護照效期轉民國年
        //if (edObj.HCOP_PASSPORT_EXP_DATE.Length == 8)
        //{
        //    txtHCOP_PASSPORT_EXP_DATE.Text = ConvertToROCYear(txtHCOP_PASSPORT_EXP_DATE.Text);
        //}
        ////居留證效期轉民國年
        //if (edObj.HCOP_RESIDENT_EXP_DATE.Length == 8)
        //{
        //    txtHCOP_RESIDENT_EXP_DATE.Text = ConvertToROCYear(txtHCOP_RESIDENT_EXP_DATE.Text);
        //}
        //國籍檢核 
        if (edObj.HCOP_OWNER_NATION.ToUpper() == "TW")
        {
            LockTW(true);
        }
        else
        {
            LockTW(false);
        }

        //20190614 Talas w調整美國州別顯示文字，因為下拉選單看不到
        foreach (ListItem lc in dropBasicCountryStateCode.Items)
        {
            if (lc.Text == "請選擇") { continue; }
            if (lc.Value == edObj.HCOP_REGISTER_US_STATE)
            {
                lc.Selected = true;
                txtHCOP_REGISTER_US_STATE.Text = lc.Text;
            }
        }

        //20190614 Talas 增加對應選項中文，因為下拉選單看不見
        setDropSelected(dropSCCDOrganization, edObj.HCOP_BUSINESS_ORGAN_TYPE);
        txtHCOP_BUSINESS_ORGAN_TYPE.Text = dropSCCDOrganization.SelectedItem.Text;

        //20190817 James 判斷聯絡人是否為長姓名
        if (edObj.CONTACT_LNAM_FLAG == "Y")
        {
            radHCOP_CONTACT_LNAMEY.Checked = true;
            cmpLname_contact.Style["display"] = "";
            cmpRname_contact.Style["display"] = "";
        }
        else
        {
            radHCOP_CONTACT_LNAMEN.Checked = true;
        }

        if (edObj.OWNER_LNAM_FLAG == "Y")
        {
            cmpLname_0.Style["display"] = "";
            cmpRname_0.Style["display"] = "";
        }

    }
    /// <summary>
    /// 頁面鎖定國籍為TW時的方法，反之則解鎖
    /// </summary>
    private void LockTW(bool isTw)
    {
        //20200330
        //如果為國籍為TW，則護照效期/居留證效期不得輸入；反之，若非TW但護照號碼/居留證號為空白亦不得輸入
        //ChangeControlLock("txtHCOP_PASSPORT_EXP_DATE", "CustTextBox", isTw);
        //ChangeControlLock("txtHCOP_RESIDENT_EXP_DATE", "CustTextBox", isTw);
        if (!isTw && string.IsNullOrEmpty(HQlblHCOP_PASSPORT.Text.Trim()))
        {
            ChangeControlLock("txtHCOP_PASSPORT_EXP_DATE", "CustTextBox", !isTw);
        }
        else
        {
            ChangeControlLock("txtHCOP_PASSPORT_EXP_DATE", "CustTextBox", isTw);
        }
        if (!isTw && string.IsNullOrEmpty(HQlblHCOP_RESIDENT_NO.Text.Trim()))
        {
            ChangeControlLock("txtHCOP_RESIDENT_EXP_DATE", "CustTextBox", !isTw);
        }
        else
        {
            ChangeControlLock("txtHCOP_RESIDENT_EXP_DATE", "CustTextBox", isTw);
        }

        ChangeControlLock("txtHCOP_OWNER_ID_ISSUE_DATE", "CustTextBox", !isTw);
        ChangeControlLock("txtHCOP_OWNER_ID_ISSUE_PLACE", "CustTextBox", !isTw);
        ChangeControlLock("dropHCOP_OWNER_ID_REPLACE_TYPE", "CustDropDownList", !isTw);
    }

    /// <summary>
    /// 載入下拉選單內容
    /// </summary>
    private void LoadDropDownList()
    {

        // 設定 州別
        SetDropStateCode("8", dropBasicCountryStateCode, true);
        //// 設定 法律形式
        //SetDropStateCode("2", dropSCCDOrganization, false);
        SetDropLAW();
        //設定初補換
        SetDropStateCode("4", dropHCOP_OWNER_ID_REPLACE_TYPE, true);
        // 設定 風險物件
        SetDropNameChk("5");

        //設定 組織運作
        SetDropOrg("6");

        //設定存在證明
        setProof_Item("7");
        //設定是否空白
        SetDropYNEmpty();
    }
    /// <summary>
    /// 設定 風險物件
    /// </summary>
    /// <param name="type"></param>
    private void SetDropLAW()
    {
        DataTable result = BRPostOffice_CodeType.GetCodeType("2");
        //DataTable result2 = BRPostOffice_CodeType.GetCodeType("14");

        ListItem listItem = new ListItem();
        string listString = string.Empty;
        //this.dropNameCheck_Item.Items.Add(new ListItem("請選擇", ""));//2020.04.22 Ray 已新增過，所以mark
        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                //bool isBreak = false;
                //初始時檢核是否在排除名單，若是，則變更旗標
                //for (int x = 0; x < result2.Rows.Count; x++)
                //{
                //    if (result.Rows[i][0].ToString() == result2.Rows[x][0].ToString())
                //    {
                //        isBreak = true;
                //    }
                //}
                //if (isBreak)
                //{
                //    continue;
                //}
                listItem = new ListItem();
                listItem.Value = result.Rows[i][0].ToString();
                listItem.Text = result.Rows[i][1].ToString();
                this.dropSCCDOrganization.Items.Add(listItem);
            }
        }
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
    }   // 設定 是、否、空白
    private void SetDropYNEmpty()
    {
        ListItem listItem = new ListItem();
        string listString = string.Empty;

        string[] arr = { "是;Y", "否;N" };
        string[] arrs = null;
        this.dropSCCDIsSanction.Items.Add(new ListItem("請選擇", ""));
        for (int i = 0; i < arr.Length; i++)
        {
            arrs = arr[i].Split(';');
            listItem = new ListItem();
            if (arrs.Length == 2)
            {
                listItem.Value = arrs[1].ToString();
                listItem.Text = arrs[0].ToString();
                this.dropSCCDIsSanction.Items.Add(listItem);
            }
        }
    }

    /// <summary>
    /// 設定 組織運作
    /// </summary>
    /// <param name="type"></param>
    private void SetDropOrg(string type)
    {
        DataTable result = BRPostOffice_CodeType.GetCodeType(type);
        ListItem listItem = new ListItem();
        string listString = string.Empty;
        this.dropOrganization_Item.Items.Add(new ListItem("請選擇", ""));
        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                listItem = new ListItem();

                listItem.Value = result.Rows[i][0].ToString();
                listItem.Text = result.Rows[i][1].ToString();
                this.dropOrganization_Item.Items.Add(listItem);
            }
        }
    }
    /// <summary>
    /// 設定 風險物件
    /// </summary>
    /// <param name="type"></param>
    private void setProof_Item(string type)
    {
        DataTable result = BRPostOffice_CodeType.GetCodeType(type);
        ListItem listItem = new ListItem();
        string listString = string.Empty;
        this.dropProof_Item.Items.Add(new ListItem("請選擇", ""));
        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                listItem = new ListItem();
                if (result.Columns.Count > 1)
                {
                    listItem.Value = result.Rows[i][0].ToString();
                    listItem.Text = result.Rows[i][1].ToString();
                    this.dropProof_Item.Items.Add(listItem);
                }
            }
        }
    }
    // 設定 州別
    private void SetDropStateCode(string code, CustDropDownList dropItem, bool isShowKey)
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
                listItem.Text = result.Rows[i][1].ToString();
            }
            else
            {
                listItem.Text = result.Rows[i][0].ToString();

            }
            dropItem.Items.Add(listItem);
        }

    }


    #endregion

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
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

    /// <summary>
    /// 顯示資料，並填至畫面
    /// </summary>
    private void showeditDate()
    {
        //由資料庫讀入，測試用
        //private void ReadFromDb()
        string strAlertMsg = "";
        //填基本物件，由電文讀取
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

        //SCCD物件除風險以外，須由資料庫讀出與回寫
        //讀取SCDD資料，轉換成Edit
        EntityHQ_SCDD sccdObj = BRHQ_SCDD.getSCDDData_WOrk(sessionOBJ);
        EntityHQ_SCDD_edit editObj = sccdObj.toEditMode();


        //建立HASHTABLE
        Hashtable JC66OBj = new Hashtable();
        JC66OBj.Add("FUNCTION_CODE", "I");
        JC66OBj.Add("CORP_NO", sessionOBJ.HCOP_HEADQUATERS_CORP_NO);
        JC66OBj.Add("CORP_SEQ", "0000");

        //上送主機資料
        //  Hashtable hstExmsP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC66, JC66OBj, false, "11", eAgentInfo);
        ////測試用模擬資料
        ////Hashtable hstExmsP4A = GetTestData();
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
        //20190614(U) by Nash 如果無JC66電文權限，不會跳出頁面錯誤訊息 
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
                    //20190614(U) by Nash 如果無JC66電文權限，不會跳出頁面錯誤訊息 
                    if (!string.IsNullOrEmpty(hstExmsP4A["MESSAGE_CHI"].ToString()))
                    {
                        hesRtn = hstExmsP4A["MESSAGE_CHI"].ToString();
                    }
                    strAlertMsg = MessageHelper.GetMessages("01_01080103_020") + hesRtn;
                    string NavigateUrl = "P010801000001.aspx";
                    if (!string.IsNullOrEmpty(Session["P010801010001_Last"].ToString()))
                    {
                        NavigateUrl = Session["P010801010001_Last"].ToString();
                    }
                    string urlString = @"alert('" + strAlertMsg + "');location.href='" + NavigateUrl + "';";
                    base.sbRegScript.Append(urlString);
                    break;
            }
        }
        catch (Exception ex)
        {
            // Logging.SaveLog(ex.Message, ELogType.Error);
            Logging.Log(ex.ToString(), LogState.Error, LogLayer.Util);



            //20200207-修正如果RACF密碼到期，僅秀查詢電文失敗；修改顯示密碼到期之訊息
            /*
            if (!string.IsNullOrEmpty(hstExmsP4A["HtgMsg"].ToString()) && hstExmsP4A["HtgMsg"].ToString().Contains("705"))
            {
                strAlertMsg = MessageHelper.GetMessages("00_00000000_039");
            }
            else
            {
                strAlertMsg = MessageHelper.GetMessages("01_01080103_020") + hesRtn;
            }
            */
            if (!string.IsNullOrEmpty(hstExmsP4A["HtgMsg"].ToString()))//20200410-RQ-2019-030155-005
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
            if (!string.IsNullOrEmpty(Session["P010801010001_Last"].ToString()))
            {
                NavigateUrl = Session["P010801010001_Last"].ToString();
            }
            string urlString = @"alert('" + strAlertMsg + "');location.href='" + NavigateUrl + "';";
            base.sbRegScript.Append(urlString);
        }

    }
    /// <summary>
    /// 模擬測試用JC66電文回傳
    /// </summary>
    /// <returns></returns>
    private Hashtable GetTestData()
    {
        //讀取D:\JC66_Download_TEST.txt做成HSAHTABLE
        string[] lincoll = File.ReadAllLines(@"D:\JC66_Download_TEST.txt", Encoding.Default);
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
                    //暫時用不道
                    //string propName = prop.Name;
                    //string authID = authAttr.ControlID; 
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
                    //暫時用不道
                    //string propName = prop.Name;
                    //string authID = authAttr.ControlID; 
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
        //轉換子公司資料 12行 由1開始(非慣用的0 因為是行號，非索引)
        for (int i = 1; i < 13; i++)
        {
            EntityAML_HQ_Manager_Work_edit sMan = new EntityAML_HQ_Manager_Work_edit();

            Type Stype = sMan.GetType();  //取的型別實體
            PropertyInfo[] Sprops = Stype.GetProperties(); //取出所有公開屬性(可以被外部存取得 
                                                           //處理總公司資料
            foreach (PropertyInfo prop in Sprops)
            {
                object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件
                AttributeRfPage authAttr;
                for (int xi = 0; xi < attrs.Length; xi++)
                {
                    if (attrs[xi] is AttributeRfPage)
                    {
                        authAttr = attrs[xi] as AttributeRfPage;
                        //暫時用不道
                        //string propName = prop.Name;
                        //string authID = authAttr.ControlID; 
                        //未設定欄位不對應
                        if (string.IsNullOrEmpty(authAttr.JC66NAME))
                        {
                            continue;
                        }

                        string Jc66FieldKey = authAttr.JC66NAME + i.ToString();
                        //JC66有值，建立HASTABLE欄位，
                        if (!string.IsNullOrEmpty(Jc66FieldKey))
                        {
                            if (inObj.ContainsKey(Jc66FieldKey))
                            {
                                prop.SetValue(sMan, inObj[Jc66FieldKey], null);

                            }
                        }
                    }
                }
            }

            //會有12筆
            rtnObj.ManagerColl.Add(sMan);

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
                        hs.Add(Jc66FieldKey, exVal);
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

        //逐項處理高階經理人
        //因為寫入不一定是12行，故以12處理
        string _isLnameFlag = "Y";
        foreach (EntityAML_HQ_Manager_Work_edit seMobj in DataObj.ManagerColl)
        {

            //因欄位不多，不用映射 ，但須取得LINEID
            string sLineID = seMobj.LineID;
            //姓名
            hs.Add("BENE_NAME_" + sLineID, seMobj.HCOP_BENE_NAME);
            //生日
            hs.Add("BENE_BIRTH_DATE_" + sLineID, seMobj.HCOP_BENE_BIRTH_DATE);
            //國籍
            hs.Add("BENE_NATION_" + sLineID, seMobj.HCOP_BENE_NATION);
            //處理Checkbox
            CustCheckBox ck1 = FindControl("chkHCOP_BENE_JOB_TYPE_" + sLineID) as CustCheckBox;
            CustCheckBox ck2 = FindControl("chkHCOP_BENE_JOB_TYPE_2_" + sLineID) as CustCheckBox;
            CustCheckBox ck3 = FindControl("chkHCOP_BENE_JOB_TYPE_3_" + sLineID) as CustCheckBox;
            CustCheckBox ck4 = FindControl("chkHCOP_BENE_JOB_TYPE_4_" + sLineID) as CustCheckBox;
            CustCheckBox ck5 = FindControl("chkHCOP_BENE_JOB_TYPE_5_" + sLineID) as CustCheckBox;
            CustCheckBox ck6 = FindControl("chkHCOP_BENE_JOB_TYPE_6_" + sLineID) as CustCheckBox;
            //IDNoType 

            bool isany = false;
            hs.Add("BENE_JOB_TYPE_1_" + sLineID, GetCheckboxval(ck1, ref isany));
            hs.Add("BENE_JOB_TYPE_2_" + sLineID, GetCheckboxval(ck2, ref isany));
            hs.Add("BENE_JOB_TYPE_3_" + sLineID, GetCheckboxval(ck3, ref isany));
            hs.Add("BENE_JOB_TYPE_4_" + sLineID, GetCheckboxval(ck4, ref isany));
            hs.Add("BENE_JOB_TYPE_5_" + sLineID, GetCheckboxval(ck5, ref isany));
            hs.Add("BENE_JOB_TYPE_6_" + sLineID, GetCheckboxval(ck6, ref isany));

            hs.Add("BENE_ID_" + sLineID, seMobj.HCOP_BENE_ID);
            hs.Add("BENE_PASSPORT_" + sLineID, seMobj.HCOP_BENE_PASSPORT);
            //Talas 調整 BENE_PASSPORT_EXP_ 以空白送出
            // hs.Add("BENE_PASSPORT_EXP_" + sLineID, seMobj.HCOP_BENE_PASSPORT_EXP);
            hs.Add("BENE_PASSPORT_EXP_" + sLineID, "");
            hs.Add("BENE_RESIDENT_NO_" + sLineID, seMobj.HCOP_BENE_RESIDENT_NO);
            //Talas 調整 BENE_RESIDENT_EXP  以空白送出
            //hs.Add("BENE_RESIDENT_EXP_" + sLineID, seMobj.HCOP_BENE_RESIDENT_EXP);
            hs.Add("BENE_RESIDENT_EXP_" + sLineID, "");
            //Talas 調整增加 BENE_OTH_CERT 
            hs.Add("BENE_OTH_CERT_" + sLineID, seMobj.HCOP_BENE_OTH_CERT);

            //依長姓名原則來決定值是給Y或N            
            string _id = getHQ_ID(seMobj);
            _isLnameFlag = ValidLongNameFlag(_id, seMobj.HCOP_BENE_LNAME, seMobj.HCOP_BENE_ROMA);
            hs.Add("BENE_LNAM_FLAG" + sLineID, _isLnameFlag);

        }

        //加上美國州別 若為NA，改成空白 ?
        if (hs.ContainsKey("REGISTER_US_STATE"))
        {
            if (hs["REGISTER_US_STATE"].ToString() == "NA")
            {
                hs["REGISTER_US_STATE"] = "";
            }
        }
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
        if (hs.ContainsKey("REG_ADDR2") && hs["REG_ADDR2"].ToString().ToUpper() == "Ｘ　　　　　　　　　　　　　")
        {
            hs["REG_ADDR2"] = "";
            hs["REG_ADDR2"] = hs["REG_ADDR2"].ToString().PadRight(14, '　');
        }

        if (hs.ContainsKey("MAILING_ADDR2") && hs["MAILING_ADDR2"].ToString().ToUpper() == "Ｘ　　　　　　　　　　　　　")
        {
            hs["MAILING_ADDR2"] = "";
            hs["MAILING_ADDR2"] = hs["MAILING_ADDR2"].ToString().PadRight(14, '　');
        }

        if (hs.ContainsKey("OWNER_ADDR2") && hs["OWNER_ADDR2"].ToString().ToUpper() == "Ｘ　　　　　　　　　　　　　")
        {
            hs["OWNER_ADDR2"] = "";
            hs["OWNER_ADDR2"] = hs["OWNER_ADDR2"].ToString().PadRight(14, '　');
        }

        //20200331 為避免因全形半形被擋，故將姓名再重新轉全形送出
        hs["OWNER_CHINESE_NAME"] = ToWide(hs["OWNER_CHINESE_NAME"].ToString()).Trim();

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
        // ShowinPage(edObj);
    }
    /// <summary>
    /// 設定下拉選單選取
    /// </summary>
    /// <param name="editObj"></param>
    private void SetDPValue(EntityHQ_SCDD_edit editObj)
    {
        setDropSelected(dropNameCheck_Item, editObj.NameCheck_Item);
        setDropSelected(dropSCCDIsSanction, editObj.IsSanction);
        setDropSelected(dropOrganization_Item, editObj.Organization_Item);

        setDropSelected(dropProof_Item, editObj.Proof_Item);


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
        this.SetVal<EntityAML_HQ_Work_edit>(edObj, false);

        this.SetVal<EntityHQ_SCDD_edit>(SCDDObj, false);
        SetDPValue(SCDDObj);
        //HQ特殊欄位處理
        setHQSpcial(edObj);

        int resetLineID = 0;//20200409-高管畫面重新排序 by Peggy
        for (int i = 1; i < 13; i++)
        {
            if (edObj.ManagerColl.Count > i - 1)
            {
                EntityAML_HQ_Manager_Work_edit managerObj = edObj.ManagerColl[i - 1];
                //20200409
                if (managerObj.HCOP_BENE_NAME.Trim().Equals(""))
                {
                    continue;
                }
                else
                    resetLineID++;
                //20200409-RQ-2019-030155-005 高管資料重新排序
                /*
                managerObj.LineID = i.ToString();

                //處理Checkbox
                CustCheckBox ck1 = FindControl("chkHCOP_BENE_JOB_TYPE_" + i.ToString()) as CustCheckBox;
                CustCheckBox ck2 = FindControl("chkHCOP_BENE_JOB_TYPE_2_" + i.ToString()) as CustCheckBox;
                CustCheckBox ck3 = FindControl("chkHCOP_BENE_JOB_TYPE_3_" + i.ToString()) as CustCheckBox;
                CustCheckBox ck4 = FindControl("chkHCOP_BENE_JOB_TYPE_4_" + i.ToString()) as CustCheckBox;
                CustCheckBox ck5 = FindControl("chkHCOP_BENE_JOB_TYPE_5_" + i.ToString()) as CustCheckBox;
                CustCheckBox ck6 = FindControl("chkHCOP_BENE_JOB_TYPE_6_" + i.ToString()) as CustCheckBox;
                */

                managerObj.LineID = resetLineID.ToString();
                this.SetVal<EntityAML_HQ_Manager_Work_edit>(managerObj, false);
                CustCheckBox ck1 = FindControl("chkHCOP_BENE_JOB_TYPE_" + managerObj.LineID) as CustCheckBox;
                CustCheckBox ck2 = FindControl("chkHCOP_BENE_JOB_TYPE_2_" + managerObj.LineID) as CustCheckBox;
                CustCheckBox ck3 = FindControl("chkHCOP_BENE_JOB_TYPE_3_" + managerObj.LineID) as CustCheckBox;
                CustCheckBox ck4 = FindControl("chkHCOP_BENE_JOB_TYPE_4_" + managerObj.LineID) as CustCheckBox;
                CustCheckBox ck5 = FindControl("chkHCOP_BENE_JOB_TYPE_5_" + managerObj.LineID) as CustCheckBox;
                CustCheckBox ck6 = FindControl("chkHCOP_BENE_JOB_TYPE_6_" + managerObj.LineID) as CustCheckBox;


                setCheckboxval(ck1, managerObj.HCOP_BENE_JOB_TYPE);
                setCheckboxval(ck2, managerObj.HCOP_BENE_JOB_TYPE_2);
                setCheckboxval(ck3, managerObj.HCOP_BENE_JOB_TYPE_3);
                setCheckboxval(ck4, managerObj.HCOP_BENE_JOB_TYPE_4);
                setCheckboxval(ck5, managerObj.HCOP_BENE_JOB_TYPE_5);
                setCheckboxval(ck6, managerObj.HCOP_BENE_JOB_TYPE_6);
                if (!string.IsNullOrEmpty(managerObj.HCOP_BENE_NATION))
                {
                    LockTW(managerObj.HCOP_BENE_NATION == "TW", managerObj.LineID);
                }

                //------------------------------------------------
                //長姓名區是否顯示的判斷
                if (!string.IsNullOrEmpty(managerObj.HCOP_BENE_LNAME) && managerObj.BENE_LNAM_FLAG == "Y")
                {
                    //20200409-RQ-2019-030155-005 高管資料重新排序
                    //CustCheckBox cklname = FindControl("chkBENE_LNAM_FLAG_" + i.ToString()) as CustCheckBox;
                    CustCheckBox cklname = FindControl("chkBENE_LNAM_FLAG_" + managerObj.LineID) as CustCheckBox;
                    setCheckboxval(cklname, "Y");
                    //20200409-RQ-2019-030155-005 高管資料重新排序
                    //HtmlTableRow _tmpRow1 = FindControl(string.Format("cmpLname_{0}", i.ToString())) as HtmlTableRow;
                    //HtmlTableRow _rmpRow2 = FindControl(string.Format("cmpRname_{0}", i.ToString())) as HtmlTableRow;
                    HtmlTableRow _tmpRow1 = FindControl(string.Format("cmpLname_{0}", managerObj.LineID)) as HtmlTableRow;
                    HtmlTableRow _rmpRow2 = FindControl(string.Format("cmpRname_{0}", managerObj.LineID)) as HtmlTableRow;
                    if (_tmpRow1 != null)
                    {
                        _tmpRow1.Style["display"] = "";
                        _rmpRow2.Style["display"] = "";
                    }
                }
                //20200610-RQ-2019-030155-000-身分證類型
                CustCheckBox cb1 = FindControl("chkIDType1_" + managerObj.LineID) as CustCheckBox;//身分證
                CustCheckBox cb3 = FindControl("chkIDType3_" + managerObj.LineID) as CustCheckBox;//護照
                CustCheckBox cb4 = FindControl("chkIDType4_" + managerObj.LineID) as CustCheckBox;//居留證(統一證號)
                CustCheckBox cb7 = FindControl("chkIDType7_" + managerObj.LineID) as CustCheckBox;//其他證號
                CustTextBox txtBENE_ID = FindControl("txtHCOP_BENE_ID_" + managerObj.LineID) as CustTextBox;//身分證件號碼

                string _IDNO = string.Empty;
                if (!string.IsNullOrEmpty(managerObj.HCOP_BENE_ID))
                {
                    cb1.Checked = true;
                    _IDNO = managerObj.HCOP_BENE_ID;
                }

                if (!string.IsNullOrEmpty(managerObj.HCOP_BENE_PASSPORT))
                {
                    cb3.Checked = true;
                    _IDNO = managerObj.HCOP_BENE_PASSPORT;
                }

                if (!string.IsNullOrEmpty(managerObj.HCOP_BENE_RESIDENT_NO))
                {
                    cb4.Checked = true;
                    _IDNO = managerObj.HCOP_BENE_RESIDENT_NO;
                }

                if (!string.IsNullOrEmpty(managerObj.HCOP_BENE_OTH_CERT))
                {
                    cb7.Checked = true;
                    _IDNO = managerObj.HCOP_BENE_OTH_CERT;
                }

                txtBENE_ID.Text = _IDNO.Trim();

                //------------------------------------------------
            }
        }

        //從經辦頁的session抓取換補領檢查結果
        //追加處理領補換檢查結果
        if (Session["0801_OWNER_ID_SreachStatus"] != null)
        {
            string ID_SreachStatus = Session["0801_OWNER_ID_SreachStatus"].ToString();
            if (ID_SreachStatus == "Y")
            {
                ChkOWNER_ID_SreachStatusY.Checked = true;
                ChkOWNER_ID_SreachStatusN.Checked = false;
            }
            if (ID_SreachStatus == "N")
            {
                ChkOWNER_ID_SreachStatusY.Checked = false;
                ChkOWNER_ID_SreachStatusN.Checked = true;
            }
        }
        //20190625 Talas 修改非僑外資時母公司國別]
        radHCOP_OVERSEAS_FOREIGNY_CheckedChanged(null, null);

    }
    /// <summary>
    /// 處理高階經理人國籍與姓名
    /// </summary>
    /// <param name="dObj"></param>
    /// <param name="errMsg"></param>
    /// <param name="lineID"></param>
    private void checkNation(EntityAML_HQ_Manager_Work_edit dObj, ref List<string> errMsg, string lineID)
    {
        CustLabel lblEnNotice = FindControl("lblEnNotice_" + lineID) as CustLabel; //姓名提示文字 
        //直接不顯示，未來如果要檢核，也加在這
        lblEnNotice.Visible = false;
        string _alertStrFormat = "{0}，高管第{1}行\\n";

        // MessageHelper.GetMessages("01_01080103_019");
        //先檢核 有國籍，姓名不能空
        if (string.IsNullOrEmpty(dObj.HCOP_BENE_NAME) && !string.IsNullOrEmpty(dObj.HCOP_BENE_NATION))
        {
            //errMsg.Add(MessageHelper.GetMessages("01_01080103_021") + "，第" + lineID + "行\\n");
            errMsg.Add(string.Format(_alertStrFormat, MessageHelper.GetMessages("01_01080103_021"), lineID));
        }
        //先檢核 有姓名，國籍不能空
        if (!string.IsNullOrEmpty(dObj.HCOP_BENE_NAME) && string.IsNullOrEmpty(dObj.HCOP_BENE_NATION))
        {
            //errMsg.Add(MessageHelper.GetMessages("01_01080103_022") + "，第" + lineID + "行\\n");
            errMsg.Add(string.Format(_alertStrFormat, MessageHelper.GetMessages("01_01080103_022"), lineID));
        }
        if (!string.IsNullOrEmpty(dObj.HCOP_BENE_NAME) && !string.IsNullOrEmpty(dObj.HCOP_BENE_NATION))
        {
            //先依國籍判斷全半形
            if (dObj.HCOP_BENE_NATION == "TW")
            {
                //全型
                dObj.HCOP_BENE_NAME = ToWide(dObj.HCOP_BENE_NAME);
                if (dObj.HCOP_BENE_NAME.Length > 19)
                {
                    //姓名欄位太長
                    //errMsg.Add(MessageHelper.GetMessages("01_01080103_022") +"，第" + lineID + "行\\n");
                    //errMsg.Add(string.Format(_alertStrFormat, MessageHelper.GetMessages("01_01080103_022"), lineID));//20200525調整訊息
                    //20200701-RQ-2019-030155-000 調整提示訊息
                    //errMsg.Add(string.Format(_alertStrFormat, MessageHelper.GetMessages("01_01080103_029"), lineID));
                    errMsg.Add(string.Format(_alertStrFormat, MessageHelper.GetMessages("01_01080103_030"), lineID));

                }
                else
                {
                    dObj.HCOP_BENE_NAME = dObj.HCOP_BENE_NAME.PadRight(19, '　');
                }
            }
            else
            {
                //半形
                dObj.HCOP_BENE_NAME = ToNarrow(dObj.HCOP_BENE_NAME);
                if (dObj.HCOP_BENE_NAME.Length > 40)
                {
                    //姓名欄位太長
                    //errMsg.Add(MessageHelper.GetMessages("01_01080103_022") + "，第" + lineID + "行\\n");
                    //errMsg.Add(string.Format(_alertStrFormat, MessageHelper.GetMessages("01_01080103_022"), lineID));//20200525調整訊息
                    //20200701-RQ-2019-030155-000 調整提示訊息
                    //errMsg.Add(string.Format(_alertStrFormat, MessageHelper.GetMessages("01_01080103_029"), lineID));
                    errMsg.Add(string.Format(_alertStrFormat, MessageHelper.GetMessages("01_01080103_031"), lineID));

                }
                else
                {
                    dObj.HCOP_BENE_NAME = dObj.HCOP_BENE_NAME.PadRight(40, ' ');
                }
            }
        }
    }
    /// <summary>
    /// 處理高階經理人 的長姓名和羅馬拼音
    /// </summary>
    /// <param name="dObj"></param>
    /// <param name="errMsg"></param>
    /// <param name="lineID"></param>
    private void checkLName(EntityAML_HQ_Manager_Work_edit dObj, ref List<string> errMsg, string lineID)
    {
        CustLabel lblEnNotice = FindControl("lblEnNotice_" + lineID) as CustLabel; //姓名提示文字 
        //直接不顯示，未來如果要檢核，也加在這
        lblEnNotice.Visible = false;

        //檢核 長姓名原則
        if (dObj.BENE_LNAM_FLAG == "Y")
        {
            string _alertStrFormat = "{0}，高管第{1}行\\n";
            string tmpMsg = valideLnameRoma(dObj.HCOP_BENE_LNAME, dObj.HCOP_BENE_ROMA);
            if (string.IsNullOrEmpty(tmpMsg) == false)
            {
                errMsg.Add(string.Format(_alertStrFormat, tmpMsg, lineID));
            }
        }
    }
    /// <summary>
    /// 檢核 國籍TW 身分證為Z999999999, 身分證指定為1
    /// </summary>
    /// <param name="lid">高管畫面LINEID</param>
    /// <returns></returns>
    private bool checkIdNoType(string lid)
    {        
        //國籍
        CustTextBox txtCountryCode = FindControl("txtHCOP_BENE_NATION_" + lid) as CustTextBox;
        //身分證件號碼
        CustTextBox txtSMID = FindControl("txtHCOP_BENE_ID_" + lid) as CustTextBox;
        //身分證件類型
        CustCheckBox cb1 = FindControl("chkIDType1_" + lid) as CustCheckBox;//身分證
        CustCheckBox cb3 = FindControl("chkIDType3_" + lid) as CustCheckBox;//護照
        CustCheckBox cb4 = FindControl("chkIDType4_" + lid) as CustCheckBox;//統一證號
        CustCheckBox cb7 = FindControl("chkIDType7_" + lid) as CustCheckBox;//其他

        if (txtCountryCode.Text.Trim().ToUpper() == "TW" && txtSMID.Text.Trim() == "Z999999999")
        {
            if (cb1.Checked && !cb3.Checked && !cb4.Checked && !cb7.Checked)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    ///作者:Ares_Jack
    ///創建日期:20220106
    ///修改日期:20220106
    /// <summary>
    /// 檢核 高階管理人國籍
    /// </summary>
    /// <param name="lid">高管畫面LINEID</param>
    /// <returns></returns>
    private bool checkCountry(string lid)
    {
        //國籍
        CustTextBox txtCountryCode = FindControl("txtHCOP_BENE_NATION_" + lid) as CustTextBox;
        if (txtCountryCode.Text.Trim() == "無")
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 取回經理人選項
    /// </summary>
    /// <param name="ckitem"></param>   
    /// <returns></returns>
    private string GetCheckboxval(CustCheckBox ckitem, ref bool isany)
    {
        string result = "N";
        if (ckitem == null) { return result; }
        if (ckitem.Checked == true)
        {
            result = "Y";
            isany = true;
        }


        return result;
    }
    /// <summary>
    /// 取回-設定經理人選項
    /// </summary>
    /// <param name="ckitem"></param>
    /// <param name="inKey"></param>
    /// <returns></returns>
    private void setCheckboxval(CustCheckBox ckitem, string inKey)
    {
        if (ckitem == null) { return; }
        if (inKey == "Y")
        {
            ckitem.Checked = true;
        }
        else
        {
            ckitem.Checked = false;
        }
    }
    /// <summary>
    /// 依傳入行業別顯示於畫面
    /// </summary>
    /// <param name="text"></param>
    private void setCCName(string text)
    {
        //Talas 20190919 修正將顏色還原
        // txtAMLCC.BackColor = Color.White;
        txtAMLCC.BackColor = default(Color);
        DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", text);
        if (result.Rows.Count > 0 && CheckCodeType(result))
        {
            lblCCName.Text = result.Rows[0]["CODE_NAME"].ToString();
        }
        //Talas 20190613 增加行業別編號錯誤或不存在時，變成空白
        else
        {
            txtAMLCC.BackColor = Color.Red;
            lblCCName.Text = "";
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
    /// 負責人國籍
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtHCOP_OWNER_NATION_TextChanged(object sender, EventArgs e)
    {
        string strAlertMsg = "";
        txtHCOP_OWNER_NATION.Text = txtHCOP_OWNER_NATION.Text.ToUpper();
        //Talas 20190919 調整國籍檢核不合規則變色
        CustTextBox mConteol = (CustTextBox)sender;
        mConteol.BackColor = default(Color);
        if (string.IsNullOrEmpty(txtHCOP_OWNER_NATION.Text)) //空白不檢核
        {
            return;
        }
        //檢核國籍       
        //不正確的國籍
        if (GetDcValue("CT_1_" + txtHCOP_OWNER_NATION.Text.ToUpper()) == "")
        {
            //Talas 20190919 調整國籍檢核不合規則變色
            //   txtHCOP_OWNER_NATION.BackColor = Color.Red;
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
        LockTW(txtHCOP_OWNER_NATION.Text.ToUpper() == "TW");
    }
    /// <summary>
    /// 高階經理人國籍處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtHCOP_BENE_NATION_TextChanged(object sender, EventArgs e)
    {
        //找出LINEID 以控制像名稱檢核
        CustTextBox mConteol = (CustTextBox)sender;
        //Talas 20190919 不正確的國籍變色
        mConteol.BackColor = default(Color);
        string sname = mConteol.ID;
        string[] namePhase = sname.Split('_');
        string LineID = "";
        if (namePhase.Length == 4)
        {
            LineID = namePhase[3];
        }
        else { return; }


        string strAlertMsg = "";
        mConteol.Text = mConteol.Text.ToUpper();
        //設定國籍變更時，變更Focus
        //CustCheckBox ccBox = FindControl("chkHCOP_BENE_JOB_TYPE_" + LineID) as CustCheckBox;
        //CustTextBox myControlPass = FindControl("txtHCOP_BENE_PASSPORT_" + LineID) as CustTextBox;
        if (mConteol.Text == "TW")
        {

            strIsShow = "document.all['" + "chkHCOP_BENE_JOB_TYPE_" + LineID + "'].focus();";
        }
        else
        {
            //strIsShow = "document.all['" + "txtHCOP_BENE_PASSPORT_" + LineID + "'].focus();";
            strIsShow = "setfocus('" + "txtHCOP_BENE_PASSPORT_" + LineID + "');";
        }
        if (string.IsNullOrEmpty(mConteol.Text)) //空白不檢核
        {
            //開放該行
            UnLockALL(LineID);
            return;
        }

        //檢核國籍       
        //不正確的國籍
        if (GetDcValue("CT_1_" + mConteol.Text) == "")
        {
            //Talas 20190919 不正確的國籍變色
            mConteol.BackColor = Color.Red;
            strAlertMsg = MessageHelper.GetMessages("01_01080103_009") + "第" + LineID + "行\\n";
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
        LockTW(mConteol.Text == "TW", LineID);

        setLabNation();

    }

    //國籍為TW時需變更的欄位
    private void LockTW(bool isTw, string lineID)
    {
        //顯示欄位調整，此處一併調整 
        //ChangeControlLock("txtHCOP_BENE_PASSPORT_EXP_" + lineID, "CustTextBox", isTw);
        //ChangeControlLock("txtHCOP_BENE_RESIDENT_EXP_" + lineID, "CustTextBox", isTw);
        //ChangeControlLock("txtHCOP_BENE_OTH_CERT_" + lineID, "CustTextBox", isTw);
        ChangeControlLock("txtHCOP_BENE_PASSPORT_" + lineID, "CustTextBox", isTw);
        ChangeControlLock("txtHCOP_BENE_RESIDENT_NO_" + lineID, "CustTextBox", isTw);
        ChangeControlLock("lblEnNotice_" + lineID, "CustLabel", !isTw);
    }

    private void UnLockALL(string lineID)
    {
        //顯示欄位調整，此處一併調整 
        //ChangeControlLock("txtHCOP_BENE_PASSPORT_EXP_" + lineID, "CustTextBox", false);
        //ChangeControlLock("txtHCOP_BENE_RESIDENT_EXP_" + lineID, "CustTextBox", false);
        //ChangeControlLock("txtHCOP_BENE_OTH_CERT_" + lineID, "CustTextBox", false);
        ChangeControlLock("txtHCOP_BENE_PASSPORT_" + lineID, "CustTextBox", false);
        ChangeControlLock("txtHCOP_BENE_RESIDENT_NO_" + lineID, "CustTextBox", false);
        ChangeControlLock("lblEnNotice_" + lineID, "CustLabel", null);
    }

    private void setLabNation()
    {
        for (int i = 1; i < 13; i++)
        {
            CustLabel myControl = FindControl("lblEnNotice_" + i.ToString()) as CustLabel;
            CustTextBox myControlNat = FindControl("txtHCOP_BENE_NATION_" + i.ToString()) as CustTextBox;

            if (string.IsNullOrEmpty(myControlNat.Text))
            {
                myControl.Visible = false;
            }
            else
            {
                if (myControlNat.Text.ToUpper() != "TW")
                {
                    myControl.ShowID = "01_01080103_057";
                }
                else
                {
                    myControl.ShowID = "01_01080103_058";
                }
            }
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
                //string twStr = WebHelper.GetShowText("01_01080103_057");
                //string NtwStr = WebHelper.GetShowText("01_01080103_058");
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
    /// 特殊關聯欄位處理
    /// </summary>
    /// <param name="dataObj"></param>
    /// <param name="errMsg"></param>
    //private void ValidLinkedFields(EntityAML_HQ_Work_edit dataObj, ref List<string> errMsg)
    //{

    //    ////國籍非TW，則需填英文名
    //    if (txtHCOP_REGISTER_NATION.Text == "TW")
    //    {

    //        //有身分證字號，須追加檢核
    //        if (!string.IsNullOrEmpty(HQlblHCOP_OWNER_ID.Text))
    //        { 
    //            string eng = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";

    //            //第一碼英文，後9碼數字，一定是台灣人，所以要判斷發證日期、發證地點領補換別、有無照片
    //            if ((eng.Contains(dataObj.HCOP_OWNER_ID.Substring(0, 1)) && isNumeric(dataObj.HCOP_OWNER_ID.Substring(1))) || dataObj.HCOP_OWNER_NATION == "TW")
    //            {
    //                //追加驗證身分證規格 
    //                if (!(eng.Contains(dataObj.HCOP_OWNER_ID.Substring(0, 1)) && isNumeric(dataObj.HCOP_OWNER_ID.Substring(1))))
    //                {
    //                    errMsg.Add("身分證號碼規格不符\\n");

    //                }

    //                //先檢核發證日期空白
    //                if (string.IsNullOrEmpty(dataObj.HCOP_OWNER_ID_ISSUE_DATE))
    //                {
    //                    errMsg.Add("身分證發證日期不可空白\\n");
    //                }
    //                else
    //                {
    //                    //檢核是否為數字
    //                    if (!isNumeric(dataObj.HCOP_OWNER_ID_ISSUE_DATE))
    //                    {
    //                        errMsg.Add("身分證發證日期需為數字\\n");
    //                    }

    //                }
    //                //發證地點，不可空白
    //                if (string.IsNullOrEmpty(dataObj.HCOP_OWNER_ID_ISSUE_PLACE))
    //                {
    //                    errMsg.Add("身分證發證地點不可空白\\n");
    //                }
    //                //領補換別，不可空白
    //                if (string.IsNullOrEmpty(dataObj.HCOP_OWNER_ID_REPLACE_TYPE))
    //                {
    //                    errMsg.Add("身分證領補換別不可空白\\n");
    //                }
    //                //有無照片，不可空白
    //                if (string.IsNullOrEmpty(dataObj.HCOP_ID_PHOTO_FLAG))
    //                {
    //                    errMsg.Add("身分證有無照片不可空白\\n");
    //                }
    //            }
    //        }
    //    }
    //    //護照號碼不為空，護照效期需為數字
    //    if (!string.IsNullOrEmpty(dataObj.HCOP_PASSPORT))
    //    {
    //        //護照效期空白
    //        if (string.IsNullOrEmpty(dataObj.HCOP_PASSPORT_EXP_DATE))
    //        {
    //            errMsg.Add("護照效期不可空白\\n");

    //        }
    //        else
    //        {
    //            //檢核是否為數字
    //            if (!isNumeric(dataObj.HCOP_PASSPORT_EXP_DATE))
    //            {
    //                errMsg.Add("護照效期需為數字\\n");
    //            }

    //        }
    //    }
    //    //居留證號碼不為空，居留證效期需為數字
    //    if (!string.IsNullOrEmpty(dataObj.HCOP_RESIDENT_NO))
    //    {
    //        if (dataObj.HCOP_RESIDENT_NO.Length != 10)
    //        {
    //            errMsg.Add("居留證號須為10碼\\n");
    //        }

    //        //居留證效期空白
    //        if (string.IsNullOrEmpty(dataObj.HCOP_RESIDENT_EXP_DATE))
    //        {

    //            errMsg.Add("居留證效期不可空白\\n");

    //        }
    //        else
    //        {
    //            //檢核是否為數字
    //            if (!isNumeric(dataObj.HCOP_RESIDENT_EXP_DATE))
    //            {
    //                errMsg.Add("居留證效期需為數字\\n");
    //            }

    //        }
    //    }

    //}
    ///// <summary>
    /// 限定單選
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ChkOWNER_ID_SreachStatusY_CheckedChanged(object sender, EventArgs e)
    {
        if (ChkOWNER_ID_SreachStatusY.Checked == true)
        {
            ChkOWNER_ID_SreachStatusN.Checked = false;
        }
    }
    /// <summary>
    /// 限定單選
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ChkOWNER_ID_SreachStatusN_CheckedChanged(object sender, EventArgs e)
    {
        if (ChkOWNER_ID_SreachStatusN.Checked == true)
        {
            ChkOWNER_ID_SreachStatusY.Checked = false;
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
    /// <summary>
    /// 非僑外資時，變更母公司國別
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void radHCOP_OVERSEAS_FOREIGNY_CheckedChanged(object sender, EventArgs e)
    {
        if (radHCOP_OVERSEAS_FOREIGNN.Checked == true)
        {
            txtHCOP_OVERSEAS_FOREIGN_COUNTRY.Text = "無";
        }
        else
        {
            if (txtHCOP_OVERSEAS_FOREIGN_COUNTRY.Text == "無")
            {
                txtHCOP_OVERSEAS_FOREIGN_COUNTRY.Text = "";
            }
        }

    }

    /// <summary>
    /// 增加對應，若選擇非高風險國家，國家1有植，提示 Talas 20190717
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void dropSCCDIsSanction_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (dropSCCDIsSanction.SelectedValue == "N")
        {
            txtIsSanctionCountryCode1.Text = "";
        }
    }

    /// <summary>
    /// 是否顯示長姓名區
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void radHCOP_CONTACT_CheckedChanged(object sender, EventArgs e)
    {
        if (radHCOP_CONTACT_LNAMEN.Checked == true)
        {
            HQlblHCOP_CONTACT_LNAME.Text = "";
            HQlblHCOP_CONTACT_ROMA.Text = "";
            //Talas 2019092 追加除了清空還要關閉不可輸入
            HQlblHCOP_CONTACT_LNAME.Enabled = false;
            HQlblHCOP_CONTACT_ROMA.Enabled = false;
            HQlblHCOP_CONTACT_LNAME.BackColor = Color.LightGray;
            HQlblHCOP_CONTACT_ROMA.BackColor = Color.LightGray;
        }
        else
        {
            cmpLname_contact.Style["display"] = "";
            cmpRname_contact.Style["display"] = "";
            //Talas 2019092 追加可輸入
            HQlblHCOP_CONTACT_LNAME.Enabled = true;
            HQlblHCOP_CONTACT_ROMA.Enabled = true;
            HQlblHCOP_CONTACT_LNAME.BackColor = default(Color);
            HQlblHCOP_CONTACT_ROMA.BackColor = default(Color);
        }

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
    /// 高管的ID :要依判斷原則(身分證字號-> 護照號碼->居留證號碼)取得
    /// </summary>
    /// <returns></returns>
    private string getHQ_ID(EntityAML_HQ_Manager_Work_edit data)
    {
        string _result = string.Empty;
        string ID, PASSPORT, RESIDENT_NO;
        ID = data.HCOP_BENE_ID;
        PASSPORT = data.HCOP_BENE_PASSPORT;
        RESIDENT_NO = data.HCOP_BENE_RESIDENT_NO;
        _result = (string.IsNullOrEmpty(ID) == false ? ID : PASSPORT);
        if (string.IsNullOrEmpty(_result) == true)
        {
            _result = RESIDENT_NO;
        }
        return _result;
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

        //轉換高管資料 12行 由1開始(非慣用的0 因為是行號，非索引)
        string Jc66FieldKey = string.Empty;
        int _index = 0;
        foreach (EntityAML_HQ_Manager_Work_edit item in rtnObj.ManagerColl)
        {
            //判斷是否為長姓名, 若為Y 才需要再去查JC68得對應值
            if (!string.IsNullOrEmpty(item.BENE_LNAM_FLAG) && item.BENE_LNAM_FLAG.Equals("Y"))
            {
                _index++;
                _data = new EntityHTG_JC68();
                tmpID = getHQ_ID(item);
                _data.ID = tmpID;
                _data = obj.getData(_data, eAgentInfo);
                item.HCOP_BENE_LNAME = _data.LONG_NAME;
                item.HCOP_BENE_ROMA = _data.PINYIN_NAME;
                if (!_data.Success)
                {
                    HTGfailMsg.Add(string.Format(msgFormat, tmpID, _data.MESSAGE_TYPE));
                }
            }
        }

        if (HTGfailMsg.Count > 0)
        {
            this.strHostMsg = "查詢長姓名電文時有異常!! " + string.Join("", HTGfailMsg.ToArray()); ;
        }

        return rtnObj;
    }

    /// <summary>
    /// 勾選高管的長姓名時的相關判斷
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void chkBENE_LNAM_FLAG_CheckedChanged(object sender, EventArgs e)
    {
        CustCheckBox _chk = (CustCheckBox)sender;
        string _id = _chk.ID.Replace("chkBENE_LNAM_FLAG_", "");
        //若有勾選時需顯示輸入區塊
        if (_chk.Checked == true)
        {
            HtmlTableRow _tmpRow1 = FindControl(string.Format("cmpLname_{0}", _id)) as HtmlTableRow;
            HtmlTableRow _rmpRow2 = FindControl(string.Format("cmpRname_{0}", _id)) as HtmlTableRow;
            if (_tmpRow1 != null)
            {
                _tmpRow1.Style["display"] = "";
                _rmpRow2.Style["display"] = "";
                //Talas 2019092 追加可輸入
                CustTextBox _tmpTxtbox1 = FindControl(string.Format("txtHCOP_BENE_LNAME_{0}", _id)) as CustTextBox;
                CustTextBox _tmpTxtbox2 = FindControl(string.Format("txtHCOP_BENE_ROMA_{0}", _id)) as CustTextBox;
                _tmpTxtbox1.Enabled = true;
                _tmpTxtbox2.Enabled = true;
                _tmpTxtbox1.BackColor = default(Color);
                _tmpTxtbox2.BackColor = default(Color);
            }
        }
        else
        {
            //若取消勾選時,需把輸入欄位值清空
            CustTextBox _tmpTxtbox1 = FindControl(string.Format("txtHCOP_BENE_LNAME_{0}", _id)) as CustTextBox;
            CustTextBox _tmpTxtbox2 = FindControl(string.Format("txtHCOP_BENE_ROMA_{0}", _id)) as CustTextBox;

            _tmpTxtbox1.Text = "";
            _tmpTxtbox2.Text = "";
            //Talas 2019092 追加除了清空還要關閉不可輸入
            _tmpTxtbox1.Enabled = false;
            _tmpTxtbox2.Enabled = false;
            _tmpTxtbox1.BackColor = Color.LightGray;
            _tmpTxtbox2.BackColor = Color.LightGray;
        }
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
    /// <summary>
    /// 法律形式檢核
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtHCOP_BUSINESS_ORGAN_TYPE_TextChanged(object sender, EventArgs e)
    {
        //取得實體並轉型
        CustTextBox CurTxt = txtHCOP_BUSINESS_ORGAN_TYPE;
        CurTxt.BackColor = default(Color);
        //因為法律形式為中文，故需dropSCCDOrganization.SelectedValue來做判斷
        //判斷文字，若與dropSCCDOrganization.selectText相同，則不須檢核，反之則以文字送出
        string myValidKey = CurTxt.Text;
        if (myValidKey == dropSCCDOrganization.SelectedItem.Text)
        {
            myValidKey = dropSCCDOrganization.SelectedValue;
        }
        else
        {
            DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID("2", myValidKey);
            if (result.Rows.Count > 0 && CheckCodeType(result))
            {
                //須將若與dropSCCDOrganization 調整至與輸入相同，因為驗證為有效，故一定會有項目
                dropSCCDOrganization.SelectedValue = myValidKey;
                CurTxt.Text = dropSCCDOrganization.SelectedItem.Text;
            }
            else
            {
                CurTxt.BackColor = Color.Red;
            }
        }
    }
    /// <summary>
    /// 母公司國別檢核
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtHCOP_OVERSEAS_FOREIGN_COUNTRY_TextChanged(object sender, EventArgs e)
    {  //取得實體並轉型
        CustTextBox CurTxt = (CustTextBox)sender;
        CurTxt.BackColor = default(Color);
        //空白或無則不檢核
        if (CurTxt.Text == "無" || CurTxt.Text == "")
        {
            return;
        }
        DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID("1", CurTxt.Text);
        if (result.Rows.Count > 0 && CheckCodeType(result))
        {
        }
        else
        {
            CurTxt.BackColor = Color.Red;
        }

    }
    /// <summary>
    /// 主要營業處所國別檢核
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtHCOP_PRIMARY_BUSI_COUNTRY_TextChanged(object sender, EventArgs e)
    {  //取得實體並轉型
        CustTextBox CurTxt = (CustTextBox)sender;
        CurTxt.BackColor = default(Color);
        //空白則不檢核
        if (CurTxt.Text == "")
        {
            return;
        }
        DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID("1", CurTxt.Text);
        if (result.Rows.Count > 0 && CheckCodeType(result))
        {
        }
        else
        {
            CurTxt.BackColor = Color.Red;
        }

    }
    /// <summary>
    /// 高風險或制裁國家檢核 (共用)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtIsSanctionCountryCode1_TextChanged(object sender, EventArgs e)
    {
        //取得實體並轉型
        CustTextBox CurTxt = (CustTextBox)sender;
        CurTxt.BackColor = default(Color);
        //空白則不檢核
        if (CurTxt.Text == "")
        {
            return;
        }
        DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID("12", CurTxt.Text); //高風險
        if (result.Rows.Count > 0 && CheckCodeType(result))
        {
        }
        else
        {
            CurTxt.BackColor = Color.Red;
        }
    }

    #region RQ-2019-030155-005 NameCheck
    /// <summary>
    /// ESB_NameCheck 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnESB_NameCheck_Click(object sender, EventArgs e)
    {
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
        List<Entity_ESBNameCheck_DateObj> NameCheck_DateObj = new List<Entity_ESBNameCheck_DateObj>();
        NameCheck_DateObj = GetNameCheck_Item(sessionOBJ.CASE_NO, sessionOBJ.HCOP_CORP_REG_ENG_NAME);

        string strConfirmMsg = string.Empty;
        string strIDMsg = string.Empty;
        string strItemMsg = string.Empty;
        // 2020.06.09 Ray Make 判斷 改用STR提示人工判斷
        // 2020.04.30 Ray Add 新增 是否有分公司資料
        //bool isBRCH_Work = false;//20200811-下次RC若有自動拋查分公司資訊，記得MARK
        string strBrchMsg = string.Empty;
        // 2020.06.24 Ray Add 高階主管是否有身分類型
        string strJobTypeMsg = string.Empty;
        //20200926
        string strBrch_HQ_Msg = string.Empty;
        //20201029-RQ-2020-021027-003 記錄如果分公司設立日期為NULL或00000000的訊息
        string strBrch_BuildDate_Msg = string.Empty;

        int i = 1;
        //用ROW_NO排序 最大值優先
        //顯示資料ROW_NO 小會在最上面
        NameCheck_DateObj.Sort(new NameCheck_DateObj_Comparer_TaD());

        for (int x = NameCheck_DateObj.Count - 1; x >= 0; x--) //(Entity_ESBNameCheck_DateObj reObj in NameCheck_DateObj)
        {
            // 2020.06.09-RQ-2019-030155-000  Ray Add
            // 將DB抓的分公司ITEM替換
            if (NameCheck_DateObj[x].ITEM.IndexOf("分公司負責人") != -1)
            {
                if (NameCheck_DateObj[x].ITEM.IndexOf("#") != -1)
                {
                    // 分公司負責人2.# (分公司登記名稱)
                    NameCheck_DateObj[x].ITEM = "分公司登記名稱" + NameCheck_DateObj[x].ITEM.Split('.')[0].Replace("分公司負責人", "");
                }
                else
                {
                    // 分公司負責人1.1 (分公司負責人短姓名及長姓名)
                    NameCheck_DateObj[x].ITEM = NameCheck_DateObj[x].ITEM.Split('.')[0];
                }
            }

            // 2020.06.10 Ray 新增判斷如是分公司登記資料沒有名字，顯示提醒人工
            // 判斷ID || (chineseN & englishN) 是否空值 
            if (string.IsNullOrEmpty(NameCheck_DateObj[x].ID) || (string.IsNullOrEmpty(NameCheck_DateObj[x].CHINESE_NAME) && string.IsNullOrEmpty(NameCheck_DateObj[x].ENGLISH_NAME)))
            {
                // 紀錄資料錯誤屬於哪個Item
                //20200610-RQ-2019-030155-000
                //strItemMsg = strItemMsg + NameCheck_DateObj[x].ITEM + "、";
                if (NameCheck_DateObj[x].ITEM.IndexOf("分公司登記名稱") != -1)
                {
                    strBrchMsg = strBrchMsg + NameCheck_DateObj[x].ID + "、";
                    // 空白資料刪除ltem
                    NameCheck_DateObj.RemoveAt(x);
                    continue;
                }
                else
                {
                    if (strItemMsg.IndexOf(NameCheck_DateObj[x].ITEM) == -1)
                    {
                        strItemMsg = strItemMsg + NameCheck_DateObj[x].ITEM + "、";
                    }
                }
            }

            //比對，如主機給的分公司檔中的總公司統編，與案件的總公司統編不同時，跳訊息
            if (!string.IsNullOrEmpty(NameCheck_DateObj[x].HCOP_CorpNO) && !(NameCheck_DateObj[x].HCOP_CorpNO.Equals(sessionOBJ.HCOP_HEADQUATERS_CORP_NO)))
            {
                strBrch_HQ_Msg = strBrch_HQ_Msg + NameCheck_DateObj[x].ID + "、";
                // 空白資料刪除ltem
                NameCheck_DateObj.RemoveAt(x);
                continue;
            }
            //20201029-RQ-2020-021027-003
            //比對，如主機給的分公司檔中的設立日期為空白或是NULL時，跳訊息
            if (NameCheck_DateObj[x].ConnectedPartyType.Equals("APP") && (string.IsNullOrEmpty(NameCheck_DateObj[x].BIRTH_DATE) || NameCheck_DateObj[x].BIRTH_DATE.Equals("00000000")))
            {
                strBrch_BuildDate_Msg = strBrch_BuildDate_Msg + NameCheck_DateObj[x].ID + "、";
                // 空白資料刪除ltem
                NameCheck_DateObj.RemoveAt(x);
                continue;
            }
            // 比對NameCheck_DateObj資料是否已拋查過
            //if (BRPNAMECHECKLOG.NameChecklog_Compare(sessionOBJ.CASE_NO, NameCheck_DateObj[x].ID, NameCheck_DateObj[x].CHINESE_NAME, NameCheck_DateObj[x].ENGLISH_NAME))
            // 2020.06.29 增加ConnectedPartySubType 與 ConnectedPartyType判斷
            if (BRPNAMECHECKLOG.NameChecklog_Compare(sessionOBJ.CASE_NO, NameCheck_DateObj[x].ID, NameCheck_DateObj[x].CHINESE_NAME, NameCheck_DateObj[x].ENGLISH_NAME, NameCheck_DateObj[x].ConnectedPartyType, NameCheck_DateObj[x].ConnectedPartySubType))
            {
                // 重複資料刪除ltem
                NameCheck_DateObj.RemoveAt(x);
                continue;
            }



            // ID如為Z999999999時 紀錄strIDMsg後 顯示在最後
            if (NameCheck_DateObj[x].ID.ToUpper().IndexOf("Z999999999") != -1)
            {
                strIDMsg = strIDMsg + NameCheck_DateObj[x].CHINESE_NAME + " / ";
                // 2020.06.24 Ray ID如為Z999999999刪除ltem
                NameCheck_DateObj.RemoveAt(x);
                continue;
            }

            // 2020.06.24 Ray Add 高階主管無身分類型
            if (NameCheck_DateObj[x].ITEM.IndexOf("高階主管") != -1 && string.IsNullOrEmpty(NameCheck_DateObj[x].ConnectedPartyType) && string.IsNullOrEmpty(NameCheck_DateObj[x].ConnectedPartySubType))
            {
                if (strJobTypeMsg.IndexOf(NameCheck_DateObj[x].ID) == -1)
                {
                    strJobTypeMsg = strJobTypeMsg + NameCheck_DateObj[x].ID + "、";
                }
                // 高階主管無身分類型刪除ltem
                NameCheck_DateObj.RemoveAt(x);
                continue;
            }

            //20220125_Ares_Jack_名單掃描國籍不得為無
            if (NameCheck_DateObj[x].NATION.ToString().Trim() == "無")
            {
                base.sbRegScript.Append("alert('國籍不得為 無! ');");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(i + ". ");

            // 2020.06.09 Ray Mark 判斷，以抓DB資料
            // 判斷需要拋查資料中是否有分公司資料
            //if (NameCheck_DateObj[x].ITEM.IndexOf("分公司負責人") != -1)//20200811-下次RC若有自動拋查分公司資訊，記得MARK
            //{
            //    isBRCH_Work = true;
            //}

            sb.Append(NameCheck_DateObj[x].CHINESE_NAME);
            sb.Append(" / " + NameCheck_DateObj[x].ENGLISH_NAME);
            sb.Append(" / " + NameCheck_DateObj[x].ID);
            sb.Append(" / " + NameCheck_DateObj[x].BIRTH_DATE);
            sb.Append(" / " + NameCheck_DateObj[x].NATION);
            // 2020.06.29 Ray Add ConnectedType
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

        if (!string.IsNullOrEmpty(strIDMsg))
            strIDMsg = "【高階管理人: " + strIDMsg.Substring(0, (strIDMsg.Length - 2)) + "為虛擬ID，請人工進行名單掃描】";

        // 2020.06.09 Ray Make 改用strBrchMsg並加入提醒哪些分公司統編進行人工掃描
        //// 2020.04.30 Ray Add 新增是否有分公司資料拋查，加備註
        //if (isBRCH_Work)//20200811-下次RC若有自動拋查分公司資訊，記得MARK
        //    strIDMsg = strIDMsg + "\\n＊分公司統編請人工進行名單掃描";
        if (!string.IsNullOrEmpty(strBrchMsg))
            strIDMsg = strIDMsg + "\\n【分公司統編：" + strBrchMsg.Substring(0, (strBrchMsg.Length - 1)) + "資料有誤，請人工進行名單掃描】";
        //20200926
        if (!string.IsNullOrEmpty(strBrch_HQ_Msg))
        {
            strIDMsg = strIDMsg + "\\n【分公司統編：" + strBrch_HQ_Msg.Substring(0, (strBrch_HQ_Msg.Length - 1)) + "請人工進行名單掃描】";
        }
        //20201029-RQ-2020-021027-003
        if (!string.IsNullOrEmpty(strBrch_BuildDate_Msg))
        {
            strIDMsg = strIDMsg + "\\n【分公司統編：" + strBrch_BuildDate_Msg.Substring(0, (strBrch_BuildDate_Msg.Length - 1)) + "之設立日期有誤，請人工進行名單掃描】";
        }

        //  2020.07.01 Ray Add 新增高階主管身分類型 ,修正取尾碼-1
        if (!string.IsNullOrEmpty(strJobTypeMsg))
            strIDMsg = strIDMsg + "\\n【高階主管：" + strJobTypeMsg.Substring(0, (strJobTypeMsg.Length - 1)) + "無身分類型，請人工進行名單掃描】";

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
        // 2020.05.06 Ray NameCheck 顯示排序名單
        // 總公司統編/總公司負責人/分公司負責人/高階主管及受益人
        // 2020.06.09 新增分公司登記資料
        // 總公司統編/總公司負責人/分公司統編/分公司負責人/高階主管及受益人
        // 2020.06.24 新增ESB_TYPE 總公司統編type=1 其他type=2

        #region 分公司
        // 分公司抓DB資料 Row_no 20開始累加上去
        DataSet Ds = new DataSet();
        BRAML_BRCH_Work.GetToESB_Query(CASE_NO, ref Ds);
        reObj = DataTableConvertor.ConvertCollToObj<Entity_ESBNameCheck_DateObj>(Ds.Tables["AML_BRCH_WORK"]);
        #endregion

        #region 總公司
        // 總公司登記名稱與統編 Row_no 11
        reObjGet = new Entity_ESBNameCheck_DateObj();
        reObjGet.CHINESE_NAME = hlblHCOP_REG_NAME.Text.Trim();
        reObjGet.ENGLISH_NAME = _HCOP_CORP_REG_ENG_NAME.Trim();
        reObjGet.BATCH_NO = "";
        reObjGet.NATION = txtHCOP_REGISTER_NATION.Text.Trim();
        reObjGet.BIRTH_DATE = txtHCOP_BUILD_DATE.Text.Trim();
        reObjGet.ID = hlblHCOP_HEADQUATERS_CORP_NO.Text.Trim();
        reObjGet.ITEM = "總公司登記名稱";
        reObjGet.ROW_NO = "11";
        // 2020.06.24 新增ESB_TYPE ConnectedPartyType
        reObjGet.ESB_TYPE = "1";
        reObjGet.ConnectedPartyType = "";
        reObjGet.ConnectedPartySubType = "";

        reObj.Add(reObjGet);

        // 總公司負責人 短姓名 Row_no 12
        reObjGet = new Entity_ESBNameCheck_DateObj();
        reObjGet.CHINESE_NAME = HQlblHCOP_OWNER_CHINESE_NAME.Text.Trim();
        reObjGet.ENGLISH_NAME = HQlblHCOP_OWNER_ENGLISH_NAME.Text.Trim();
        reObjGet.BIRTH_DATE = txtHCOP_OWNER_BIRTH_DATE.Text.Trim();
        reObjGet.NATION = txtHCOP_OWNER_NATION.Text.Trim();
        // 判斷國籍非TW時，先抓居留證　沒有在抓護照號碼
        if (txtHCOP_OWNER_NATION.Text.ToUpper().Trim() != "TW")
        {
            if (!string.IsNullOrEmpty(HQlblHCOP_RESIDENT_NO.Text.Trim()))
            {
                reObjGet.ID = HQlblHCOP_RESIDENT_NO.Text.Trim();
            }
            else
            {
                reObjGet.ID = HQlblHCOP_PASSPORT.Text.Trim();
            }
        }
        else
        {
            reObjGet.ID = HQlblHCOP_OWNER_ID.Text.Trim();
        }
        reObjGet.BATCH_NO = "";
        reObjGet.ITEM = "總公司負責人";
        reObjGet.ROW_NO = "12";
        // 2020.06.24 新增ESB_TYPE ConnectedPartyType
        reObjGet.ESB_TYPE = "2";
        reObjGet.ConnectedPartyType = "LA";
        reObjGet.ConnectedPartySubType = "LA";
        reObj.Add(reObjGet);

        // 總公司負責人 長姓名 Row_no 13
        // 當長姓名 or 羅馬拼音有值就加入NameCheck拋查資料
        if (!string.IsNullOrEmpty(HQlblHCOP_OWNER_CHINESE_LNAME.Text.Trim()) || !string.IsNullOrEmpty(HQlblHCOP_OWNER_ROMA.Text.Trim()))
        {
            reObjGet = new Entity_ESBNameCheck_DateObj();
            reObjGet.CHINESE_NAME = HQlblHCOP_OWNER_CHINESE_LNAME.Text.Trim();
            reObjGet.ENGLISH_NAME = HQlblHCOP_OWNER_ROMA.Text.Trim();
            reObjGet.BIRTH_DATE = txtHCOP_OWNER_BIRTH_DATE.Text.Trim();
            reObjGet.NATION = txtHCOP_OWNER_NATION.Text.Trim();
            // 判斷國籍非TW時，先抓居留證　沒有在抓護照號碼
            if (txtHCOP_OWNER_NATION.Text.ToUpper().Trim() != "TW")
            {
                if (!string.IsNullOrEmpty(HQlblHCOP_RESIDENT_NO.Text.Trim()))
                {
                    reObjGet.ID = HQlblHCOP_RESIDENT_NO.Text.Trim();
                }
                else
                {
                    reObjGet.ID = HQlblHCOP_PASSPORT.Text.Trim();
                }
            }
            else
            {
                reObjGet.ID = HQlblHCOP_OWNER_ID.Text.Trim();
            }
            reObjGet.BATCH_NO = "";
            reObjGet.ITEM = "總公司負責人";
            reObjGet.ROW_NO = "13";
            // 2020.06.24 新增ESB_TYPE ConnectedPartySubType
            reObjGet.ESB_TYPE = "2";
            reObjGet.ConnectedPartyType = "LA";
            reObjGet.ConnectedPartySubType = "LA";
            reObj.Add(reObjGet);
        }
        #endregion

        #region 高階主管
        // 高階主管 Row_no 60開始累加上去
        int iHCOP_Row_No = 60;
        // 實際上高階主管筆數
        int Rno = 1;

        for (int i = 1; i <= 12; i++)
        {
            TextBox txt_HCOP_BENE_NAME = FindControl("txtHCOP_BENE_NAME_" + i.ToString()) as TextBox; // 姓名
            TextBox txt_HCOP_BENE_BIRTH_DATE = FindControl("txtHCOP_BENE_BIRTH_DATE_" + i.ToString()) as TextBox; // 生日
            TextBox txt_HCOP_BENE_ID = FindControl("txtHCOP_BENE_ID_" + i.ToString()) as TextBox; // ID
            TextBox txt_HCOP_BENE_NATION = FindControl("txtHCOP_BENE_NATION_" + i.ToString()) as TextBox; // 國籍
            TextBox txt_HCOP_BENE_LNAME = FindControl("txtHCOP_BENE_LNAME_" + i.ToString()) as TextBox; // 長姓名
            TextBox txt_HCOP_BENE_ROMA = FindControl("txtHCOP_BENE_ROMA_" + i.ToString()) as TextBox; //羅馬拼音
            //20200611-RQ-2019-030155-000 因高管欄位異動，故ID只需固定抓同一個欄位
            //TextBox txtHCOP_BENE_PASSPORT = FindControl("txtHCOP_BENE_PASSPORT_" + i.ToString()) as TextBox; //護照號碼
            //TextBox txtHCOP_BENE_RESIDENT_NO = FindControl("txtHCOP_BENE_RESIDENT_NO_" + i.ToString()) as TextBox; // 居留證號碼
            //TextBox txtHCOP_BENE_OTH_CERT = FindControl("txtHCOP_BENE_OTH_CERT_" + i.ToString()) as TextBox; // 其他號碼
            TextBox txtHCOP_BENE_ROMA = FindControl("txtHCOP_BENE_ROMA_" + i.ToString()) as TextBox;  // 羅馬拼音
            CheckBox chkBENE_LNAM_FLAG = FindControl("chkBENE_LNAM_FLAG_" + i.ToString()) as CheckBox; //長姓名 checkbox

            //高階主管 短姓名沒有值就跳過
            if (string.IsNullOrEmpty(txt_HCOP_BENE_NAME.Text))
            {
                continue;
            }
            else
            {
                // 2020.06.24 高階主管身分類型有打勾
                string strJob_Type = HcopBene_JobType(i);
                int iForRow = string.IsNullOrEmpty(strJob_Type) ? 1 : strJob_Type.Split(',').Length;

                // 新增身分類型ITEM ROM
                for (int x = 0; x < iForRow; x++)
                {
                    #region 高階主管 短姓名
                    reObjGet = new Entity_ESBNameCheck_DateObj();
                    // 預設ChineseName & EnglishName 值為空值，避免NULL
                    reObjGet.CHINESE_NAME = "";
                    reObjGet.ENGLISH_NAME = "";

                    // 判斷國籍非TW時，先抓居留證　沒有在抓護照號碼 最後抓其他證件號碼                
                    reObjGet.ID = txt_HCOP_BENE_ID.Text.Trim();//20200611-RQ-2019-030155-000 因高管欄位異動，故ID只需固定抓同一個欄位

                    if (txt_HCOP_BENE_NATION.Text.ToUpper().Trim() != "TW")
                    {
                        /*//20200611-RQ-2019-030155-000 因高管欄位異動，故ID只需固定抓同一個欄伅
                        if (!string.IsNullOrEmpty(txtHCOP_BENE_RESIDENT_NO.Text))
                        {
                            reObjGet.ID = txtHCOP_BENE_RESIDENT_NO.Text.Trim();
                        }
                        else if (!string.IsNullOrEmpty(txtHCOP_BENE_PASSPORT.Text))
                        {
                            reObjGet.ID = txtHCOP_BENE_PASSPORT.Text.Trim();
                        }
                        else
                        {
                            reObjGet.ID = txtHCOP_BENE_OTH_CERT.Text.Trim();
                        }
                        */

                        // 高階主管欄位只有中文姓名，國籍非TW時，放入英文欄位
                        reObjGet.ENGLISH_NAME = txt_HCOP_BENE_NAME.Text.Trim();
                    }
                    else
                    {
                        reObjGet.CHINESE_NAME = txt_HCOP_BENE_NAME.Text.Trim();
                        //reObjGet.ID = txt_HCOP_BENE_ID.Text.Trim();
                    }

                    reObjGet.BIRTH_DATE = txt_HCOP_BENE_BIRTH_DATE.Text.Trim();
                    reObjGet.NATION = txt_HCOP_BENE_NATION.Text.Trim();
                    reObjGet.BATCH_NO = "";
                    reObjGet.ITEM = "高階主管" + i.ToString();
                    //20200609-RQ-2019-030155-000
                    //reObjGet.ROW_NO = (50 + Rno).ToString();
                    reObjGet.ROW_NO = (iHCOP_Row_No + Rno).ToString();
                    //2020.06.24 新增ESB_TYPE ConnectedPartySType
                    reObjGet.ESB_TYPE = "2";
                    //
                    JobType_ConnectedType(strJob_Type.Split(',')[x], ref reObjGet);
                    reObj.Add(reObjGet);
                    Rno = Rno + 1;

                    #endregion

                    #region 高階主管 長姓名
                    // 判斷長姓名選項有打勾，就將長姓名列入NameCheck 拋查資料
                    if (chkBENE_LNAM_FLAG.Checked)
                    {
                        reObjGet = new Entity_ESBNameCheck_DateObj();
                        // 預設ChineseName & EnglishName 值為空值，避免NULL
                        reObjGet.CHINESE_NAME = "";
                        reObjGet.ENGLISH_NAME = "";

                        // 判斷國籍非TW時，先抓居留證　沒有在抓護照號碼 最後抓其他證件號碼
                        reObjGet.ID = txt_HCOP_BENE_ID.Text.Trim();//20200611-RQ-2019-030155-000 因高管欄位異動，故ID只需固定抓同一個欄位
                        if (txt_HCOP_BENE_NATION.Text.ToUpper().Trim() != "TW")
                        {
                            /*//20200611-RQ-2019-030155-000 因高管欄位異動，故ID只需固定抓同一個欄位
                            if (!string.IsNullOrEmpty(txtHCOP_BENE_RESIDENT_NO.Text))
                            {
                                reObjGet.ID = txtHCOP_BENE_RESIDENT_NO.Text.Trim();
                            }
                            else if (!string.IsNullOrEmpty(txtHCOP_BENE_PASSPORT.Text))
                            {
                                reObjGet.ID = txtHCOP_BENE_PASSPORT.Text.Trim();
                            }
                            else
                            {
                                reObjGet.ID = txtHCOP_BENE_OTH_CERT.Text.Trim();
                            }
                            */
                            // 高階主管欄位只有中文姓名，國籍非TW時，放入英文欄位
                            reObjGet.ENGLISH_NAME = txt_HCOP_BENE_LNAME.Text.Trim();
                        }
                        else
                        {
                            reObjGet.CHINESE_NAME = txt_HCOP_BENE_LNAME.Text.Trim();
                            //20220118 AML NOVA 功能需求程式碼,註解保留 start by Ares Jack
                            //reObjGet.EnglishName2 = txt_HCOP_BENE_ROMA.Text.Trim();
                            //20220118 AML NOVA 功能需求程式碼,註解保留 end by Ares Jack
                            //reObjGet.ID = txt_HCOP_BENE_ID.Text.Trim();
                        }

                        reObjGet.BIRTH_DATE = txt_HCOP_BENE_BIRTH_DATE.Text.Trim();
                        reObjGet.NATION = txt_HCOP_BENE_NATION.Text.Trim();
                        reObjGet.BATCH_NO = "";
                        reObjGet.ITEM = "高階主管" + i.ToString();
                        //20200609-RQ-2019-030155-000
                        //reObjGet.ROW_NO = (50 + Rno).ToString();
                        reObjGet.ROW_NO = (iHCOP_Row_No + Rno).ToString();
                        // 2020.06.24 新增ESB_TYPE ConnectedPartyType
                        reObjGet.ESB_TYPE = "2";
                        // 2020.06.24 Ray Add 高階主管身分類型需跑多次資料
                        // 無資料塞空白，後續判斷顯示ErrorMsg
                        JobType_ConnectedType(strJob_Type.Split(',')[x], ref reObjGet);
                        reObj.Add(reObjGet);
                        Rno = Rno + 1;
                    }
                    #endregion
                }

            }
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
                    // Logging.SaveLog(ELogLayer.UI, Uat + "：電文TimeOut：\r\n");
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
                // Logging.SaveLog(ELogLayer.UI, Uat + "：ESB連線錯誤\r\n" + ex.ToString());
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
            // Logging.SaveLog(ELogLayer.UI, Uat + "：ESB連線錯誤\r\n" + ex.ToString());
            Logging.Log(Uat + "：ESB連線錯誤\r\n" + ex.ToString(), logPath_ESB, LogLayer.UI);
            msgNull = true;
            return "ESB連線錯誤";
        }
    }


    protected void btnESB_NameCheckAdd_Click(object sender, EventArgs e)
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
        string strESBCustType = "N";//自然人(P) 法人(N)
        string strESBGender = "";
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

        try
        {
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
            if (sessionObjCount > 11)
            {
                //名單資料大於11筆
                #region Entity_ESBNameCheck_DateObj 資料填寫
                for (int i = 0; i < 11; i++)
                {
                    // ID如為Z999999999  該筆資料不拋資料查詢
                    // 2020.06.24 Ray Make 將show Message 時將虛擬帳號移除打電文
                    //if (li.ID.ToUpper().IndexOf("Z999999999") != -1)
                    //{
                    //    continue;
                    //}

                    // 2020.06.24 Ray 更新電文資料
                    strESBType = sessionNCObj[i].ESB_TYPE;
                    strESBConnectedPartyType = sessionNCObj[i].ConnectedPartyType;
                    strESBConnectedPartySubType = sessionNCObj[i].ConnectedPartySubType;


                    strSEQ = ESBSEQ.ToString().PadLeft(3, '0');
                    strESBChinese_Name = sessionNCObj[i].CHINESE_NAME;
                    //20220118 AML NOVA 功能需求程式碼,註解保留 start by Ares Jack
                    //strESBEnglish_Name2 = sessionNCObj[i].EnglishName2;
                    //20220118 AML NOVA 功能需求程式碼,註解保留 end by Ares Jack
                    strESBEnglish_Name = sessionNCObj[i].ENGLISH_NAME;

                    //生日為00000000時，替換成空值
                    if (sessionNCObj[i].BIRTH_DATE == "00000000")
                        strESBBirth_Date = "";
                    else
                        strESBBirth_Date = sessionNCObj[i].BIRTH_DATE.Trim();

                    strESBID = sessionNCObj[i].ID.Trim();
                    strESBNATION = sessionNCObj[i].NATION.Trim();

                    sb.AppendLine("<ns2:InputName> ");
                    sb.AppendLine("<ns2:Nationality>" + strESBNATION + "</ns2:Nationality>");
                    sb.AppendLine("<ns2:EnglishName>" + strESBEnglish_Name + "</ns2:EnglishName>");
                    //20200703 依mail2020/7/3 11:11要求AddressCountry改帶空白
                    //sb.AppendLine("<ns2:AddressCountry>TW</ns2:AddressCountry>");
                    sb.AppendLine("<ns2:AddressCountry></ns2:AddressCountry>");
                    sb.AppendLine("<ns2:BranchNo>" + strESBBatch_No + "</ns2:BranchNo>");
                    sb.AppendLine("<ns2:Type>" + strESBType + "</ns2:Type>");
                    sb.AppendLine("<ns2:NonEnglishName>" + strESBChinese_Name + "</ns2:NonEnglishName>");
                    sb.AppendLine("<ns2:ConnectedPartyType>" + strESBConnectedPartyType + "</ns2:ConnectedPartyType>");
                    sb.AppendLine("<ns2:DOB>" + strESBBirth_Date + "</ns2:DOB>");
                    sb.AppendLine("<ns2:ConnectedPartySubType>" + strESBConnectedPartySubType + "</ns2:ConnectedPartySubType>");
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
                    //20200703 依mail2020/7/3 11:11要求AddressCountry改帶空白
                    //nchecklog.AddressCountry = "TW";
                    nchecklog.AddressCountry = "";
                    nchecklog.ConnectedPartyType = strESBConnectedPartyType;
                    nchecklog.ConnectedPartySubType = strESBConnectedPartySubType;
                    //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
                    //nchecklog.Gender = strESBGender;
                    //nchecklog.CustType = strESBCustType;                    
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
            }
            else
            {
                //名單資料小於等於11筆
                #region Entity_ESBNameCheck_DateObj 資料填寫
                foreach (Entity_ESBNameCheck_DateObj li in sessionNCObj)
                {
                    // ID如為Z999999999  該筆資料不拋資料查詢
                    // 2020.06.24 Ray Make 將show Message 時將虛擬帳號移除打電文
                    //if (li.ID.ToUpper().IndexOf("Z999999999") != -1)
                    //{
                    //    continue;
                    //}

                    // 2020.06.24 Ray 更新電文資料
                    strESBType = li.ESB_TYPE;
                    strESBConnectedPartyType = li.ConnectedPartyType;
                    strESBConnectedPartySubType = li.ConnectedPartySubType;


                    strSEQ = ESBSEQ.ToString().PadLeft(3, '0');
                    strESBChinese_Name = li.CHINESE_NAME;
                    //20220118 AML NOVA 功能需求程式碼,註解保留 start by Ares Jack
                    //strESBEnglish_Name2 = li.EnglishName2;
                    //20220118 AML NOVA 功能需求程式碼,註解保留 end by Ares Jack
                    strESBEnglish_Name = li.ENGLISH_NAME;

                    //生日為00000000時，替換成空值
                    if (li.BIRTH_DATE == "00000000")
                        strESBBirth_Date = "";
                    else
                        strESBBirth_Date = li.BIRTH_DATE.Trim();

                    strESBID = li.ID.Trim();
                    strESBNATION = li.NATION.Trim();

                    sb.AppendLine("<ns2:InputName> ");
                    sb.AppendLine("<ns2:Nationality>" + strESBNATION + "</ns2:Nationality>");
                    sb.AppendLine("<ns2:EnglishName>" + strESBEnglish_Name + "</ns2:EnglishName>");
                    //20200703 依mail2020/7/3 11:11要求AddressCountry改帶空白
                    //sb.AppendLine("<ns2:AddressCountry>TW</ns2:AddressCountry>");
                    sb.AppendLine("<ns2:AddressCountry></ns2:AddressCountry>");
                    sb.AppendLine("<ns2:BranchNo>" + strESBBatch_No + "</ns2:BranchNo>");
                    sb.AppendLine("<ns2:Type>" + strESBType + "</ns2:Type>");
                    sb.AppendLine("<ns2:NonEnglishName>" + strESBChinese_Name + "</ns2:NonEnglishName>");
                    sb.AppendLine("<ns2:ConnectedPartyType>" + strESBConnectedPartyType + "</ns2:ConnectedPartyType>");
                    sb.AppendLine("<ns2:DOB>" + strESBBirth_Date + "</ns2:DOB>");
                    sb.AppendLine("<ns2:ConnectedPartySubType>" + strESBConnectedPartySubType + "</ns2:ConnectedPartySubType>");
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
                    //20200703 依mail2020/7/3 11:11要求AddressCountry改帶空白
                    //nchecklog.AddressCountry = "TW";
                    nchecklog.AddressCountry = "";
                    nchecklog.ConnectedPartyType = strESBConnectedPartyType;
                    nchecklog.ConnectedPartySubType = strESBConnectedPartySubType;
                    //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
                    //nchecklog.Gender = strESBGender;
                    //nchecklog.CustType = strESBCustType;                    
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

            }
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
                // Logging.SaveLog(ELogLayer.UI, " 發查次數：" + i.ToString() + "；ConnESB電文第一組Result：" + strResult);
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
                    // Logging.SaveLog(ELogLayer.UI, " 發查次數：" + i.ToString() + "；ConnESB電文第二組Result：" + connResult);
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

            // 2020.04.29 Ray Add 判斷當電文下行為空白(TimeOut) || 連線錯誤
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
                // 紀錄LogXML 下行資料 已方便上下行對應
                //SaveNameCheckLog(strESBMsg, "RTN", strCASE_NO, logPath_ESB_TG);

                // Logging.SaveLog(ELogLayer.UI, strESBMsg);
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
            //20200703 依mail2020/7/3 11:11要求AddressCountry改帶空白
            //nchecklog2.AddressCountry = "TW";
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
            editObj.NameCheck_No = txtNameCheck_No.Text;
            editObj.NameCheck_Note = txtNameCheck_Note.Text;
            editObj.NameCheck_Item = dropNameCheck_Item.SelectedValue;
            editObj.NameCheck_RiskRanking = "";
            editObj.Organization_Item = "";
            editObj.Proof_Item = "";
            editObj.IsSanction = "";
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
                strAlertMsg = MessageHelper.GetMessages("01_01080105_006");
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
        // string strURL = "window.open('P010801030002.aspx?" + "','','height=500,width=1300,top=300,left=300,status=no,toolbar=no, menubar=no,location=no');";
        // base.sbRegScript.Append(strURL);

        //20210426_Ares_Luke-調整頁面轉向方法(避免造成瀏覽器雙開被剔除)
        string NavigateUrl = "P010801030002.aspx";
        Session["P010801030001_Last"] = thisPageName;
        Response.Redirect(NavigateUrl, false);

    }

    /// <summary>
    /// 高階主管JOB_TYPE打勾 tp String
    /// </summary>
    /// <param name="HcopBeneRow">高階主管列數</param>
    /// <returns></returns>
    public string HcopBene_JobType(int HcopBeneRow)
    {
        string strJobType = string.Empty;
        List<CheckBox> chkHCOP_BENE_JOB_TYPE = new List<CheckBox>();
        chkHCOP_BENE_JOB_TYPE.Add(FindControl("chkHCOP_BENE_JOB_TYPE_" + HcopBeneRow.ToString()) as CheckBox);  //1董/理事、監事/監察人
        chkHCOP_BENE_JOB_TYPE.Add(FindControl("chkHCOP_BENE_JOB_TYPE_2_" + HcopBeneRow.ToString()) as CheckBox);//2總經理
        chkHCOP_BENE_JOB_TYPE.Add(FindControl("chkHCOP_BENE_JOB_TYPE_3_" + HcopBeneRow.ToString()) as CheckBox);//3財務長
        chkHCOP_BENE_JOB_TYPE.Add(FindControl("chkHCOP_BENE_JOB_TYPE_4_" + HcopBeneRow.ToString()) as CheckBox);//4有權簽章人
        chkHCOP_BENE_JOB_TYPE.Add(FindControl("chkHCOP_BENE_JOB_TYPE_5_" + HcopBeneRow.ToString()) as CheckBox);//5實質受益人
        chkHCOP_BENE_JOB_TYPE.Add(FindControl("chkHCOP_BENE_JOB_TYPE_6_" + HcopBeneRow.ToString()) as CheckBox);//6其他關聯人
        for (int i = 0; i < chkHCOP_BENE_JOB_TYPE.Count; i++)
        {
            if (chkHCOP_BENE_JOB_TYPE[i].Checked == true)
                strJobType = strJobType + (i + 1) + ",";
        }

        if (!string.IsNullOrEmpty(strJobType))
        {
            strJobType = strJobType.Substring(0, (strJobType.Length - 1));
        }

        return strJobType;
    }

    /// <summary>
    /// 高階主管JOB_TYPE 對應 ConnectedPartyType、ConnectedPartySubType 關係值
    /// </summary>
    /// <param name="strJob_Type"></param>
    /// <param name="reObjGet"></param>
    public void JobType_ConnectedType(string strJob_Type, ref Entity_ESBNameCheck_DateObj reObjGet)
    {
        // 高階主管 身分類型
        // 1董/理事、監事/監察人 2總經理 3財務長 4有權簽章人 5實質受益人 6其他關聯人 否則空白
        switch (strJob_Type)
        {
            case "1":
            case "2"://20200902-10月RC 變更關連人TYPE
            case "3"://20200902-10月RC 變更關連人TYPE
                reObjGet.ConnectedPartyType = "BOD";
                reObjGet.ConnectedPartySubType = "BOD";
                break;
            //20200902-10月RC 變更關連人TYPE
            /*
            case "2":
                reObjGet.ConnectedPartyType = "MS";
                reObjGet.ConnectedPartySubType = "GM";
                break;
            case "3":
                reObjGet.ConnectedPartyType = "MS";
                reObjGet.ConnectedPartySubType = "GM";
                break;
                */
            case "4":
                reObjGet.ConnectedPartyType = "MS";
                //20200902-10月RC 變更關連人TYPE
                //reObjGet.ConnectedPartySubType = "GM";
                reObjGet.ConnectedPartySubType = "EC";
                break;
            case "5":
                reObjGet.ConnectedPartyType = "MS";
                reObjGet.ConnectedPartySubType = "MS";
                break;
            case "6":
                reObjGet.ConnectedPartyType = "OT";
                reObjGet.ConnectedPartySubType = "OT";
                break;
            default:
                reObjGet.ConnectedPartyType = "";
                reObjGet.ConnectedPartySubType = "";
                break;
        }
    }
    #endregion

}