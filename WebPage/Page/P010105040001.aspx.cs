// *****************************************************************
//   作    者：Ares Dennis
//   功能說明：自然人收單 新增/修改一KEY
//   創建日期：2021/07/13
//   修改記錄：
// <author>            <time>            <TaskID>                <desc>
// ******************************************************************
using System;
using System.Data;
using System.Collections;
using System.Web.UI.WebControls;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.BusinessRules_new;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using System.Drawing;
using CSIPCommonModel.EntityLayer_new;
using System.Collections.Generic;
using CSIPKeyInGUI.EntityLayer_new;
using Framework.Common.Logging;

public partial class P010105040001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    //SOC所需資訊
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CommonFunction.SetControlsEnabled(pnlText, false);// 清空網頁中所有的輸入欄位
            base.sbRegScript.Append(BaseHelper.SetFocus("txtOWNER_ID"));// 將自然人身分證字號設為輸入焦點
            LoadDropDownList();
            this.btnAdd.Enabled = false;
        }

        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//SOC所需資訊
    }

    /// 作者 Ares Dennis
    /// 創建日期：2021/07/13
    /// 修改日期：2021/07/13
    /// <summary>
    /// 查詢事件
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        #region 檢核
        // 自然人身分證字號
        if (string.IsNullOrEmpty(txtOWNER_ID.Text))
        {
            base.sbRegScript.Append("alert('請輸入自然人身分證字號!');$('#txtOWNER_ID').focus();");
            return;
        }
        if (txtOWNER_ID.Text.Length != 10)
        {
            base.sbRegScript.Append("alert('自然人身分證號碼須為10碼');$('#txtOWNER_ID').focus();");
            return;
        }
        if (!CheckNaturePersonID(this.txtOWNER_ID.Text.Trim()))
        {
            base.sbRegScript.Append("alert('自然人身分證字號錯誤!');$('#txtOWNER_ID').focus();");
            return;
        }
        #endregion
        try
        {
            lblNoticeNew.Text = "";//20211028_Ares_Jack_查詢前清空

            //先查本機1KEY資料，有資料直接帶出，若沒有就打電文
            //新增查詢條件, MOD_DATE 須為當天日期(yyyyMMdd) by Ares Stanley 20211217
            string today = DateTime.Now.ToString("yyyyMMdd");
            EntityAML_NATURALPERSON DataObj = BRAML_NATURALPERSON.Query(txtOWNER_ID.Text.Trim(), "", "1", today);

            //刪除 keyin_day 非當日資料 by Ares Stanley 20211217
            BRAML_NATURALPERSON.DeleteNotTodayKData(txtOWNER_ID.Text.Trim(), "1", today);

            if (string.IsNullOrEmpty(DataObj.OWNER_ID))
            {
                #region 查JC66資料
                Hashtable htInput = new Hashtable();
                htInput.Add("FUNCTION_CODE", "I");// 查詢                                              
                                                  //身分證字號 前八碼放 CORP_NO, 後兩碼放 CORP_SEQ  + "  "
                htInput.Add("CORP_NO", this.txtOWNER_ID.Text.Substring(0, 8).Trim());
                htInput.Add("CORP_SEQ", this.txtOWNER_ID.Text.Substring(8, 2).Trim() + "  ");

                Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC66, htInput, false, "11", eAgentInfo);
                #region Test
                //Hashtable htReturn = new Hashtable();
                //htReturn.Add("STATUS", "O");
                //htReturn.Add("OWNER_ID", txtOWNER_ID.Text);
                //htReturn.Add("NameCH", "testtest");
                #endregion
                if (!htReturn.Contains("HtgMsg"))
                {
                    ViewState["HtgInfo"] = htReturn;
                    ClearPage(true);
                    SetValues();//為網頁中的欄位賦值
                    this.txtOWNER_ID2.Enabled = false;
                }
                else
                {
                    //修改鍵檔GUI訊息呈現方式
                    etMstType = eMstType.Select;

                    base.sbRegScript.Append(BaseHelper.SetFocus("txtOWNER_ID"));
                    //P4A_JC66:0006 無此筆查詢資料，請重新輸入
                    if (htReturn["MESSAGE_TYPE"] != null && htReturn["MESSAGE_TYPE"].ToString().Trim() == "0006")
                    {
                        //提示HTG電文回應訊息  無資料
                        string strAlertMsg = MessageHelper.GetMessages("01_01090100_003");
                        sbRegScript.Append("alert('" + strAlertMsg + "');");
                        string ID = this.txtOWNER_ID.Text;//暫存自然人身份証字號
                        ClearPage(true);
                        this.txtOWNER_ID2.Text = ID;
                        this.txtOWNER_ID2.Enabled = false;
                        this.txtGENDER.Text = dropGENDER.Items.FindByValue("").ToString();//性別 預設請選擇
                        this.txtCountryCode2.Text = dropCountry2.Items.FindByValue("").ToString();//國籍2 預設請選擇
                        lblNoticeNew.Text = strAlertMsg;//20211028_Ares_Jack_HTG電文無資料，請手動建檔
                    }
                    else
                    {
                        base.strClientMsg += htReturn["HtgMsg"].ToString();
                        ClearPage(false);
                    }
                }
                #endregion
            }
            else
            {
                ClearPage(true);
                SetValues(DataObj);//為網頁中的欄位賦值
                this.txtOWNER_ID2.Enabled = false;// 身分證字號
            }
            base.sbRegScript.Append(BaseHelper.SetFocus("txtNameCH"));// 將中文姓名設為輸入焦點
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
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
        #region 檢核
        // 身分證號碼
        if (!string.IsNullOrEmpty(txtOWNER_ID2.Text))
        {
            if (txtOWNER_ID2.Text.Length != 10)
            {
                base.sbRegScript.Append("alert('身分證號碼須為10碼');$('#txtOWNER_ID2').focus();");
                return;
            }
        }
        else
        {
            base.sbRegScript.Append("alert('請輸入身分證字號!');$('#txtOWNER_ID2').focus();");
            return;
        }
        if (!CheckNaturePersonID(this.txtOWNER_ID2.Text.Trim()))
        {
            base.sbRegScript.Append("alert('自然人身分證字號錯誤!');$('#txtOWNER_ID2').focus();");
            return;
        }

        //舊身分證字號
        if (this.txtOWNER_ID2.Text == this.txtOWNER_ID_OLD.Text)
        {
            base.sbRegScript.Append("alert('舊身分證字號與身分證字號不得相同!');$('#txtOWNER_ID_OLD').focus();");
            return;
        }

        // 中文姓名不能輸入空
        if (string.IsNullOrEmpty(txtNameCH.Text))
        {
            base.sbRegScript.Append("alert('請輸入中文姓名! ');$('#txtNameCH').focus();");
            return;
        }

        //生日
        if (string.IsNullOrEmpty(txtBIRTH_DATE.Text))
        {
            base.sbRegScript.Append("alert('請輸入生日! ');$('#txtBIRTH_DATE').focus();");
            return;
        }
        else
        {
            if (txtBIRTH_DATE.Text.Length != 7)
            {
                base.sbRegScript.Append("alert('生日請輸入七碼!');$('#txtBIRTH_DATE').focus();");
                return;
            }
            if (!checkDateTime(txtBIRTH_DATE.Text))
            {
                base.sbRegScript.Append("alert('生日格式錯誤');$('#txtBIRTH_DATE').focus();");
                return;
            }
        }
        //20211206_Ares_Jack_當性別有選擇男女時、需增加 身分證字號的的判斷
        string[] allGENDER = { "男", "女", "請選擇", "M", "F", "" };
        if (txtGENDER.Text.Trim() == "男" || txtGENDER.Text.Trim().ToUpper() == "M")
        {
            if (txtOWNER_ID2.Text.Substring(1, 1).Trim() != "1")
            {
                base.sbRegScript.Append("alert('請確認性別欄位的正確性');$('#txtGENDER').focus();");
                return;
            }
        }
        if (txtGENDER.Text.Trim() == "女" || txtGENDER.Text.Trim().ToUpper() == "F")
        {
            if (txtOWNER_ID2.Text.Substring(1, 1).Trim() != "2")
            {
                base.sbRegScript.Append("alert('請確認性別欄位的正確性');$('#txtGENDER').focus();");
                return;
            }
        }
        if (Array.IndexOf(allGENDER, txtGENDER.Text.Trim().ToUpper()) == -1)
        {
            base.sbRegScript.Append("alert('性別錯誤');$('#txtGENDER').focus();");
            return;
        }

        //20211206_Ares_Jack_國籍2不得為TW
        if (txtCountryCode2.Text.Trim().ToUpper() == "TW")
        {
            base.sbRegScript.Append("alert('第二國籍不得為TW');$('#txtCountryCode2').focus();");
            return;
        }
        string[] allCountryCode2 = hidCountry2.Value.Split(':');
        if ( !(txtCountryCode2.Text.Trim() == "" || txtCountryCode2.Text.Trim() == "請選擇")) //空白不檢核
        {
            if (Array.IndexOf(allCountryCode2, this.txtCountryCode2.Text.ToUpper()) == -1)
            {
                base.sbRegScript.Append("alert('第二國籍錯誤!');$('#txtCountryCode2').focus();");
                return;
            }
            //20220105_Ares_Jack_國籍2不得選無
            if (this.txtCountryCode2.Text.Trim() == "無")
            {
                base.sbRegScript.Append("alert('第二國籍不得選無!');$('#txtCountryCode2').focus();");
                return;
            }
        }


        // 發證日期不能輸入空
        if (string.IsNullOrEmpty(txtID_ISSUEDATE.Text))
        {
            base.sbRegScript.Append("alert('請輸入發證日期! ');$('#txtID_ISSUEDATE').focus();");
            return;
        }
        else
        {
            if (txtID_ISSUEDATE.Text.Length != 7)
            {
                base.sbRegScript.Append("alert('發證日期請輸入七碼!');$('#txtID_ISSUEDATE').focus();");
                return;
            }
            if (!checkDateTime(txtID_ISSUEDATE.Text))
            {
                base.sbRegScript.Append("alert('發證日期格式錯誤');$('#txtID_ISSUEDATE').focus();");
                return;
            }
        }

        // 發證地點不能輸入空
        if (string.IsNullOrEmpty(txtID_ISSUEPLACE.Text))
        {
            base.sbRegScript.Append("alert('請輸入發證地點! ');$('#txtID_ISSUEPLACE').focus();");
            return;
        }

        // 領補換類別
        string id_replaceType = txtID_REPLACETYPE.Text.Trim();
        string[] tempList = { "1", "2", "3", "" };//1：初 2：補 3：換
        if (Array.IndexOf(tempList, id_replaceType) == -1)
        {
            base.sbRegScript.Append("alert('領補換類別代碼錯誤!');$('#txtID_REPLACETYPE').focus();");
            return;
        }

        // 有無照片
        if (string.IsNullOrEmpty(txtID_PHOTOFLAG.Text.Trim()))
        {
            base.sbRegScript.Append("alert('有無照片不得為空!');$('#txtID_PHOTOFLAG').focus();");
            return;
        }
        string id_photoflag = txtID_PHOTOFLAG.Text.Trim();
        string[] tempList2 = { "0", "1", "" };
        if (Array.IndexOf(tempList2, id_photoflag) == -1)
        {
            base.sbRegScript.Append("alert('有無照片代碼錯誤!');$('#txtID_PHOTOFLAG').focus();");
            return;
        }

        //戶籍地址不能輸入空
        if (string.IsNullOrEmpty(txtREG_CITY.Text))
        {
            base.sbRegScript.Append("alert('請輸入戶籍地址! ');$('#txtREG_CITY').focus();");
            return;
        }
        if (string.IsNullOrEmpty(txtREG_ADDR1.Text))
        {
            base.sbRegScript.Append("alert('請輸入戶籍地址! ');$('#txtREG_ADDR1').focus();");
            return;
        }
        if (string.IsNullOrEmpty(txtREG_ADDR2.Text))
        {
            base.sbRegScript.Append("alert('請輸入戶籍地址! ');$('#txtREG_ADDR2').focus();");
            return;
        }

        //通訊地址不能輸入空
        if (string.IsNullOrEmpty(txtMAILING_CITY.Text))
        {
            base.sbRegScript.Append("alert('請輸入通訊地址! ');$('#txtMAILING_CITY').focus();");
            return;
        }
        if (string.IsNullOrEmpty(txtMAILING_ADDR1.Text))
        {
            base.sbRegScript.Append("alert('請輸入通訊地址! ');$('#txtMAILING_ADDR1').focus();");
            return;
        }
        if (string.IsNullOrEmpty(txtMAILING_ADDR2.Text))
        {
            base.sbRegScript.Append("alert('請輸入通訊地址! ');$('#txtMAILING_ADDR2').focus();");
            return;
        }

        //EMAIL不能輸入空
        if (string.IsNullOrEmpty(txtEMAIL.Text))
        {
            base.sbRegScript.Append("alert('Email欄位為必填欄位! ');$('#txtEMAIL').focus();");
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
            if (this.txtEmailOther.Text == "")
            {
                base.sbRegScript.Append("alert('請輸入Email Address! ');$('#txtEmailOther').focus();");
                return;
            }

            mailEnd = this.txtEmailOther.Text;
        }
        if (!CommonFunction.CheckMailLength(this.txtEMAIL.Text, mailEnd))
        {
            base.sbRegScript.Append("alert('Email總長度不可大於50碼');$('#txtEmailFront').focus();");
            return;
        }
        if (txtEMAIL.Text.Length > 0)
        {
            this.hidEmailFall.Value = this.txtEMAIL.Text + "@" + mailEnd;
        }

        //連絡電話不能輸入空
        if (string.IsNullOrEmpty(txtCOMP_TEL1.Text))
        {
            base.sbRegScript.Append("alert('請輸入連絡電話! ');$('#txtCOMP_TEL1').focus();");
            return;
        }
        if (string.IsNullOrEmpty(txtCOMP_TEL2.Text))
        {
            base.sbRegScript.Append("alert('請輸入連絡電話! ');$('#txtCOMP_TEL2').focus();");
            return;
        }
        if (string.IsNullOrEmpty(txtCOMP_TEL3.Text))
        {
            base.sbRegScript.Append("alert('請輸入連絡電話! ');$('#txtCOMP_TEL3').focus();");
            return;
        }

        //行動電話不能輸入空
        if (string.IsNullOrEmpty(txtMobilePhone.Text))
        {
            base.sbRegScript.Append("alert('請輸入行動電話! ');$('#txtMobilePhone').focus();");
            return;
        }

        //任職公司名稱不能輸入空
        if (string.IsNullOrEmpty(txtNP_COMPANY_NAME.Text))
        {
            base.sbRegScript.Append("alert('請輸入任職公司名稱! ');$('#txtNP_COMPANY_NAME').focus();");
            return;
        }

        //行業別
        #region 行業別
        if (txtIndustry1.Text.Equals("") && txtIndustry2.Text.Equals("") && txtIndustry3.Text.Equals(""))
        {
            base.sbRegScript.Append("alert('行業別不得為空!');$('#txtIndustry1').focus();");
            return;
        }
        if (txtIndustry1.Text.Trim().Equals(""))
        {
            base.sbRegScript.Append("alert('行業別1不得為空!');$('#txtIndustry1').focus();");
            return;
        }

        if (!string.IsNullOrEmpty(txtIndustry1.Text))
        {
            string result = check_Industry(txtIndustry1.Text.Trim(), "1");
            if (result == "err")
                return;
        }

        if (!string.IsNullOrEmpty(txtIndustry2.Text))
        {
            string result = check_Industry(txtIndustry2.Text.Trim(), "2");
            if (result == "err")
                return;
        }

        if (!string.IsNullOrEmpty(txtIndustry3.Text))
        {
            string result = check_Industry(txtIndustry3.Text.Trim(), "3");
            if (result == "err")
                return;
        }
        #endregion

        // 行業編號
        #region 行業編號
        if (txtCC.Text.Equals("") && txtCC2.Text.Equals("") && txtCC3.Text.Equals(""))
        {
            base.sbRegScript.Append("alert('請輸入行業編號!');$('#txtCC').focus();");
            return;
        }
        if (txtCC.Text.Equals(""))
        {
            base.sbRegScript.Append("alert('行業編號1不得為空!');$('#txtCC').focus();");
            return;
        }
        // 行業編號1
        if (!string.IsNullOrEmpty(txtCC.Text))
        {
            string result = check_CC(txtCC.Text.Trim(), "");
            if (result == "err")
                return;
        }
        // 行業編號2
        if (!string.IsNullOrEmpty(txtCC2.Text))
        {
            string result = check_CC(txtCC2.Text.Trim(), "2");
            if (result == "err")
                return;
        }
        // 行業編號3
        if (!string.IsNullOrEmpty(txtCC3.Text))
        {
            string result = check_CC(txtCC3.Text.Trim(), "3");
            if (result == "err")
                return;
        }
        //【行業別】與【行業別編號】資料需相互對應, 非一對一
        if (!txtCC.Text.Trim().Equals(""))
        {
            string result = check_CC_Industry(txtCC.Text.Trim(), "");
            if (result == "err")
                return;
        }
        if (!txtCC2.Text.Trim().Equals(""))
        {
            string result = check_CC_Industry(txtCC2.Text.Trim(), "2");
            if (result == "err")
                return;
        }
        if (!txtCC3.Text.Trim().Equals(""))
        {
            string result = check_CC_Industry(txtCC3.Text.Trim(), "3");
            if (result == "err")
                return;
        }
        #endregion

        //職稱
        if (!string.IsNullOrEmpty(txtTITLE.Text))
        {
            DataTable TitleDT = BRPostOffice_CodeType.GetCodeType("19");
            ListItem TitleListItem = null;
            string TitleText = string.Empty;
            for (int i = 0; i < TitleDT.Rows.Count; i++)
            {
                TitleListItem = new ListItem();
                TitleListItem.Text = TitleDT.Rows[i][0].ToString();
                TitleText += TitleListItem.Text + ':';
            }

            string[] allAMLTitleText = TitleText.Split(':');
            if (Array.IndexOf(allAMLTitleText, txtTITLE.Text) == -1)
            {
                base.sbRegScript.Append("alert('職稱不存在!');$('#txtTITLE').focus();");
                return;
            }
        }
        else
        {
            base.sbRegScript.Append("alert('職稱不能為空!');$('#txtOC').focus();");
            return;
        }


        // 職稱編號
        if (!string.IsNullOrEmpty(txtOC.Text.Trim()))
        {
            if (txtOC.Text.Length != 4)
            {
                base.sbRegScript.Append("alert('職稱編號請輸入4碼數字!');$('#txtOC').focus();");
                return;
            }
            else
            {
                string[] allAMLOC = hidOC.Value.Split(':');
                if (Array.IndexOf(allAMLOC, txtOC.Text) == -1)
                {
                    base.sbRegScript.Append("alert('職稱編號不存在!');$('#txtOC').focus();");
                    return;
                }
            }
        }
        else
        {
            base.sbRegScript.Append("alert('請輸入職稱編號!');$('#txtOC').focus();");
            return;
        }
        //【職稱】與【職稱編號】資料需相互對應
        if (!txtOC.Text.Trim().Equals(""))
        {
            DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID("16", txtOC.Text.Trim());
            string[] allAMLOC_DESCRIPTION = result.Rows[0]["DESCRIPTION"].ToString().Trim().Split('/');

            if (Array.IndexOf(allAMLOC_DESCRIPTION, txtTITLE.Text) == -1)
            {
                base.sbRegScript.Append("alert('職稱編號輸入有誤');$('#txtOC').focus();");
                return;
            }
        }

        //收入及資產來源
        if (!chkIncome1.Checked && !chkIncome2.Checked && !chkIncome3.Checked && !chkIncome4.Checked && !chkIncome5.Checked && !chkIncome6.Checked && !chkIncome7.Checked && !chkIncome8.Checked && !chkIncome9.Checked)
        {
            base.sbRegScript.Append("alert('請勾選收入及資料來源!');$('#chkIncome1').focus();");
            return;
        }

        //資料最後異動分行不能輸入空
        if (string.IsNullOrEmpty(txtLAST_UPD_BRANCH.Text))
        {
            base.sbRegScript.Append("alert('請輸入資料最後異動分行! ');$('#txtLAST_UPD_BRANCH').focus();");
            return;
        }

        //資料最後異動MAKER不能輸入空
        if (string.IsNullOrEmpty(txtLAST_UPD_MAKER.Text))
        {
            base.sbRegScript.Append("alert('請輸入資料最後異動MAKER! ');$('#txtLAST_UPD_MAKER').focus();");
            return;
        }

        //資料最後異動CHECKER不能輸入空
        if (string.IsNullOrEmpty(txtLAST_UPD_CHECKER.Text))
        {
            base.sbRegScript.Append("alert('請輸入資料最後異動CHECKER! ');$('#txtLAST_UPD_CHECKER').focus();");
            return;
        }

        // EOS_AML(NOVA) 檢查最後異動分行 by Ares Dennis
        string LAST_UPD_BRANCH = this.txtLAST_UPD_BRANCH.Text.Trim();
        if (!string.IsNullOrEmpty(LAST_UPD_BRANCH))
        {
            //20211203_Ares_Jack_異動分行為9999不檢驗BRANCH, MAKER, CHECKER
            if (!(LAST_UPD_BRANCH == "9999"))
            {
                if (!checkLAS_UPD_BRANCH(LAST_UPD_BRANCH))
                {
                    base.sbRegScript.Append("alert('異動分行不存在，請重新填寫');");
                    return;
                }
            }
        }
        //20211122_EOS_AML(NOVA)_Ares_Jack_檢查MAKER
        string LAST_UPD_MAKER = this.txtLAST_UPD_MAKER.Text.Trim();
        if (!string.IsNullOrEmpty(LAST_UPD_MAKER))
        {
            //20211203_Ares_Jack_異動分行為9999不檢驗BRANCH, MAKER, CHECKER
            if (!(LAST_UPD_BRANCH == "9999"))
            {
                if (!checkLAS_UPD_MAKER(LAST_UPD_MAKER))
                {
                    base.sbRegScript.Append("alert('MAKER不存在，請重新填寫');");
                    return;
                }
            }
        }
        //20211122_EOS_AML(NOVA)_Ares_Jack_檢查CHECKER
        string LAST_UPD_CHECKER = this.txtLAST_UPD_CHECKER.Text.Trim();
        if (!string.IsNullOrEmpty(LAST_UPD_CHECKER))
        {
            //20211203_Ares_Jack_異動分行為9999不檢驗BRANCH, MAKER, CHECKER
            if (!(LAST_UPD_BRANCH == "9999"))
            {
                if (!checkLAS_UPD_CHECKER(LAST_UPD_CHECKER))
                {
                    base.sbRegScript.Append("alert('CHECKER不存在，請重新填寫');");
                    return;
                }
            }
        }

        // EOS_AML(NOVA) 檢查郵遞區號 by Ares Dennis
        string address = this.txtREG_CITY.Text.Trim();
        if (!string.IsNullOrEmpty(address) && !checkREG_ZIP_CODE(address))
        {
            base.sbRegScript.Append("alert('地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新');");
            return;
        }

        // 【負責人長姓名】打勾時，中文長姓名與羅馬拼音為必填欄位。
        if (chkisLongName_c.Checked)
        {
            if (txtNameCH_L.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('聯絡人長姓名FLAG勾選時，請輸入中文長姓名');$('#txtNameCH_L').focus();");
                return;
            }

            if (txtNameCH_Pinyin.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('聯絡人長姓名FLAG勾選時，請輸入羅馬拼音');$('#txtNameCH_Pinyin').focus();");
                return;
            }
        }

        // 羅馬拼音
        if (!txtNameCH_Pinyin.Text.Trim().Equals(""))
        {
            if (!ValidRoma(txtNameCH_Pinyin.Text.Trim()))
            {
                base.sbRegScript.Append("alert('負責人羅馬拼音輸入有誤');$('#txtNameCH_Pinyin').focus();");
                return;
            }
        }

        //20211022_Ares_Jack_自然人 如為全新戶, 檢核SCDD是否勾選
        if (!string.IsNullOrEmpty(lblNoticeNew.Text.Trim()))
        {
            if (chkisSCDD.Checked != true)
            {
                base.sbRegScript.Append("alert('請先完成SCDD!');$('#chkisSCDD').focus();");
                return;
            }
        }

        #endregion
        try
        {
            btnAddHiden_Click(sender, e);// 新增或修改資料
            this.txtOWNER_ID.Text = "";//自然人身分證字號 清空
        }
        catch (Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex);
            return;
        }
    }


    /// 作者 趙呂梁
    /// 創建日期：2021/07/13
    /// 修改日期：2021/07/13
    /// <summary>
    /// 新增或修改資料
    /// </summary>
    protected void btnAddHiden_Click(object sender, EventArgs e)
    {
        EntityAML_NATURALPERSON eAMLNaturalPerson = null;
        string strOwnerID = txtOWNER_ID2.Text;
        //新增查詢條件, MOD_DATE 須為當天日期(yyyyMMdd) by Ares Stanley 20211217
        string today = DateTime.Now.ToString("yyyyMMdd");

        try
        {
            eAMLNaturalPerson = CSIPKeyInGUI.BusinessRules_new.BRAML_NATURALPERSON.Query(strOwnerID, "", "1", today);
            //刪除 keyin_day 非當日資料 by Ares Stanley 20211217
            BRAML_NATURALPERSON.DeleteNotTodayKData(strOwnerID, "1", today);
        }
        catch (Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex);
            ClearPage(false);
            return;
        }

        bool blnSucceed = false;// 是否新增或修改成功
        if (eAMLNaturalPerson.OWNER_ID != null)
        {
            if (UpdateData())// 更新
            {
                blnSucceed = true;
            }
        }
        else
        {
            if (AddNewData())// 新增
            {
                blnSucceed = true;
            }
        }

        if (blnSucceed)// 新增或修改成功
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040101_001") + "');$('#txtOWNER_ID').focus();");
            base.strClientMsg += MessageHelper.GetMessage("01_01040101_001");
            ClearPage(false);
        }
    }

    /// <summary>
    /// 使用者輸入的地址解析後系統自動帶入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TextBox_AddrChanged(object sender, EventArgs e)
    {
        string strZipData = this.txtREG_CITY.Text.Trim();

        EntitySet<EntitySZIP> SZIPSet = null;
        try
        {
            SZIPSet = BRSZIP.SelectEntitySet(strZipData);
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (SZIPSet != null && SZIPSet.Count > 0)
        {
            this.txtREG_ZIPCODE.Text = SZIPSet.GetEntity(0).zip_code;
        }
        else
        {
            this.txtREG_ZIPCODE.Text = "";
            if (this.txtREG_CITY.Text.Trim() != "")//20220114_Ares_Jack_不等於空值才跳錯誤檢核
            {
                base.strClientMsg += "地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新";
                base.sbRegScript.Append("alert('地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新');");
            }
        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtREG_ADDR1"));// 將戶籍地址2設為輸入焦點
    }

    /// 作者 Ares Jack
    /// 創建日期：2021/11/22
    /// 修改日期：2021/11/22
    /// <summary>
    /// 使用者輸入行業別自動帶出中文名稱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtBasicAMLIndustry1_TextChanged(object sender, EventArgs e)
    {
        setIndustryChName("txtIndustry1", "HQlblIndustry1", this.txtIndustry1.Text, "1");
        base.sbRegScript.Append(BaseHelper.SetFocus("txtIndustry2"));// 將行業別2設為輸入焦點
    }
    protected void txtBasicAMLIndustry2_TextChanged(object sender, EventArgs e)
    {
        setIndustryChName("txtIndustry2", "HQlblIndustry2", this.txtIndustry2.Text, "2");
        base.sbRegScript.Append(BaseHelper.SetFocus("txtIndustry3"));// 將行業別3設為輸入焦點
    }
    protected void txtBasicAMLIndustry3_TextChanged(object sender, EventArgs e)
    {
        setIndustryChName("txtIndustry3", "HQlblIndustry3", this.txtIndustry3.Text, "3");
        base.sbRegScript.Append(BaseHelper.SetFocus("txtCC"));// 將行業別編號1設為輸入焦點
    }
    /// 作者 Ares Jack
    /// 創建日期：2021/11/22
    /// 修改日期：2021/11/22
    /// <summary>
    /// 使用者輸入職稱自動帶出中文名稱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtBasicAMLTITLE_TextChanged(object sender, EventArgs e)
    {
        setTITLEName("txtTITLE", "HQlblTITLE", this.txtTITLE.Text);
        base.sbRegScript.Append(BaseHelper.SetFocus("txtOC"));// 將職稱編號設為輸入焦點
    }

    /// 作者 Ares Jack
    /// 創建日期：2021/11/16
    /// 修改日期：2021/11/16
    /// <summary>
    /// 使用者輸入行業別編號自動帶出中文名稱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtBasicAMLCC_TextChanged(object sender, EventArgs e)
    {
        setCC_ChName("txtCC", "HQlblHCOP_CC_Cname1", this.txtCC.Text, "1");
        base.sbRegScript.Append(BaseHelper.SetFocus("txtCC2"));// 將行業別編號2設為輸入焦點
    }
    protected void txtBasicAMLCC2_TextChanged(object sender, EventArgs e)
    {
        setCC_ChName("txtCC2", "HQlblHCOP_CC_Cname2", this.txtCC2.Text, "2");
        base.sbRegScript.Append(BaseHelper.SetFocus("txtCC3"));// 將行業別編號3設為輸入焦點
    }
    protected void txtBasicAMLCC3_TextChanged(object sender, EventArgs e)
    {
        setCC_ChName("txtCC3", "HQlblHCOP_CC_Cname3", this.txtCC3.Text, "3");
        base.sbRegScript.Append(BaseHelper.SetFocus("txtTITLE"));// 將職稱設為輸入焦點
    }

    /// 作者 Ares Jack
    /// 創建日期：2021/11/16
    /// 修改日期：2021/11/16
    /// <summary>
    /// 使用者輸入職稱編號自動帶入職稱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtBasicAMLOC_TextChanged(object sender, EventArgs e)
    {
        setOC_ChName("txtOC", "HQlblHCOP_OC_Cname", this.txtOC.Text);
        base.sbRegScript.Append(BaseHelper.SetFocus("chkIncome1"));// 將收入來源設為輸入焦點
    }
    #endregion

    #region 方法區    

    /// <summary>
    /// 載入下拉選單內容
    /// </summary>
    private void LoadDropDownList()
    {
        #region 變數
        DataTable result = new DataTable();
        ListItem listItem = null;
        string countryCode = string.Empty;
        string industry = string.Empty;
        string amlcc = string.Empty;
        string title = string.Empty;
        string oc = string.Empty;
        #endregion

        #region 載入國籍
        result = BRPostOffice_CodeType.GetCodeType("1");
        DataView dv = result.DefaultView;
        dv.Sort = "CODE_ID asc";
        result = dv.ToTable();

        if (result != null && result.Rows.Count > 0)
        {
            listItem = new ListItem();
            listItem.Value = "";
            listItem.Text = "請選擇";
            this.dropCountry2.Items.Add(listItem);

            for (int i = 0; i < result.Rows.Count; i++)
            {
                listItem = new ListItem();
                listItem.Value = result.Rows[i][0].ToString();
                listItem.Text = result.Rows[i][0].ToString();
                this.dropCountry2.Items.Add(listItem);
                countryCode += result.Rows[i][0].ToString() + ":";
            }
            this.hidCountry2.Value = countryCode;
            this.dropCountry2.SelectedValue = "";
            this.txtCountryCode2.Text = "請選擇";
        }
        listItem = new ListItem();
        listItem.Value = "TW";
        listItem.Text = "TW";
        this.dropCountry1.Items.Add(listItem);
        this.dropCountry1.SelectedValue = "TW";
        this.dropCountry1.Enabled = false;
        #endregion        

        #region 載入行業編號
        result = BRPostOffice_CodeType.GetCodeType("3");
        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                amlcc += result.Rows[i][0].ToString() + ":";
            }
            this.hidAMLCC.Value = amlcc;
        }
        #endregion        

        #region 載入職稱編號
        result = BRPostOffice_CodeType.GetCodeType("16");
        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                oc += result.Rows[i][0].ToString() + ":";
            }
            this.hidOC.Value = oc;
        }
        #endregion

        #region 性別
        listItem = new ListItem();
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



    /// 作者 Ares Dennis
    /// 創建日期：2021/07/13
    /// 修改日期：2021/07/13
    /// <summary>
    /// 清空頁面內容
    /// </summary>
    private void ClearPage(bool blnEnabled)
    {
        CommonFunction.SetControlsEnabled(pnlText, blnEnabled);// 清空網頁中所有的輸入欄位        
        this.txtREG_ZIPCODE.Enabled = false;
        this.txtOWNER_ID_OLD.Text = "";//舊身分證字號    
        this.chkisLongName_c.Checked = false;//中文長姓名check box
        this.txtCountryCode.Enabled = false;//國籍1
        this.txtCountryCode.Text = "TW";//國籍1 預設帶TW 且不可修改
        this.dropCountry1.Enabled = false;//國籍1 dropdown list
        // 電子郵件信箱
        this.txtEMAIL.Text = "";
        this.hidEmailFall.Value = "";
        this.txtEmailOther.Text = "";
        this.radGmail.Checked = false;
        this.radHotmail.Checked = false;
        this.radOther.Checked = false;
        this.radYahoo.Checked = false;
        CheckBox_CheckedChanged(chkisLongName_c, null);
        //收入來源
        this.chkIncome1.Checked = false;
        this.chkIncome2.Checked = false;
        this.chkIncome3.Checked = false;
        this.chkIncome4.Checked = false;
        this.chkIncome5.Checked = false;
        this.chkIncome6.Checked = false;
        this.chkIncome7.Checked = false;
        this.chkIncome8.Checked = false;
        this.chkIncome9.Checked = false;
        //行業, 職稱 中文名稱
        this.HQlblIndustry1.Text = "";
        this.HQlblIndustry2.Text = "";
        this.HQlblIndustry3.Text = "";
        this.HQlblHCOP_CC_Cname1.Text = "";
        this.HQlblHCOP_CC_Cname2.Text = "";
        this.HQlblHCOP_CC_Cname3.Text = "";
        this.HQlblTITLE.Text = "";
        this.HQlblHCOP_OC_Cname.Text = "";
        this.txtIndustry1.BackColor = Color.White;
        this.txtIndustry2.BackColor = Color.White;
        this.txtIndustry3.BackColor = Color.White;
        this.txtCC.BackColor = Color.White;
        this.txtCC2.BackColor = Color.White;
        this.txtCC3.BackColor = Color.White;
        this.txtTITLE.BackColor = Color.White;
        this.txtOC.BackColor = Color.White;
        chkisSCDD.Checked = false;//SCDD
        this.txtCountryCode2.BackColor = Color.White;
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
            this.txtEMAIL.Text = emailArr[0];
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

    /// 作者 Ares Dennis
    /// 創建日期：2021/07/13
    /// 修改日期：2021/07/13
    /// <summary>
    /// 向資料庫中新增資料
    /// </summary>
    /// <returns>true成功,false失敗</returns>
    private bool AddNewData()
    {
        CSIPKeyInGUI.EntityLayer_new.EntityAML_NATURALPERSON eAMLNaturalPerson = GetEntity();

        try
        {
            eAMLNaturalPerson.KEYIN_FLAG = "1";
            return CSIPKeyInGUI.BusinessRules_new.BRAML_NATURALPERSON.AddEntity(eAMLNaturalPerson);
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return false;
        }
    }

    /// <summary>
    /// 更新資料庫
    /// </summary>
    /// <returns></returns>
    private bool UpdateData()
    {
        CSIPKeyInGUI.EntityLayer_new.EntityAML_NATURALPERSON eAMLNaturalPerson = GetEntity();

        try
        {
            eAMLNaturalPerson.KEYIN_FLAG = "1";
            return CSIPKeyInGUI.BusinessRules_new.BRAML_NATURALPERSON.Update(eAMLNaturalPerson, eAMLNaturalPerson.OWNER_ID.Trim(), "1");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return false;
        }
    }

    /// 作者 Ares Dennis
    /// 創建日期：2021/07/13
    /// 修改日期：2021/07/13
    /// <summary>
    /// 得到插入或更新資料的Entity
    /// </summary>
    /// <returns>Entity</returns>
    private CSIPKeyInGUI.EntityLayer_new.EntityAML_NATURALPERSON GetEntity()
    {
        CSIPKeyInGUI.EntityLayer_new.EntityAML_NATURALPERSON eAMLNaturalPerson = new CSIPKeyInGUI.EntityLayer_new.EntityAML_NATURALPERSON();

        eAMLNaturalPerson.OWNER_ID = this.txtOWNER_ID2.Text;
        eAMLNaturalPerson.NameCH = this.txtNameCH.Text;
        eAMLNaturalPerson.NameCH_OLD = this.txtNameCH_OLD.Text;
        eAMLNaturalPerson.NameEN = this.txtNameEN.Text;
        eAMLNaturalPerson.NameCH_L = this.txtNameCH_L.Text;
        eAMLNaturalPerson.NameCH_Pinyin = this.txtNameCH_Pinyin.Text;
        eAMLNaturalPerson.BIRTH_DATE = ConvertToDC(this.txtBIRTH_DATE.Text);

        string[] tempList = { "男", "女", "請選擇" };
        string[] tempList2 = { "M", "F", "" };
        if (Array.IndexOf(tempList, txtGENDER.Text.Trim().ToUpper()) != -1)//有找到tempList
        {
            string GENDER_CH = dropGENDER.Items.FindByText(txtGENDER.Text.Trim().ToUpper()).Value;//中文對應的代號
            dropGENDER.SelectedValue = GENDER_CH;
        }
        else if (Array.IndexOf(tempList2, txtGENDER.Text.Trim().ToUpper()) != -1)//有找到tempList2
        {
            string GENDER = dropGENDER.Items.FindByValue(txtGENDER.Text.Trim().ToUpper()).Value;//直接輸入代號
            dropGENDER.SelectedValue = GENDER;
        }
        eAMLNaturalPerson.GENDER = dropGENDER.SelectedValue;

        eAMLNaturalPerson.CountryCode = this.dropCountry1.SelectedValue;
        if (this.txtCountryCode2.Text.Trim() == "請選擇")
                eAMLNaturalPerson.CountryCode2 = "";
        else
            eAMLNaturalPerson.CountryCode2 = this.txtCountryCode2.Text.Trim().ToUpper();

        eAMLNaturalPerson.ID_ISSUEDATE = ConvertToDC(this.txtID_ISSUEDATE.Text);
        eAMLNaturalPerson.ID_ISSUEPLACE = this.txtID_ISSUEPLACE.Text;
        eAMLNaturalPerson.ID_REPLACETYPE = this.txtID_REPLACETYPE.Text;
        eAMLNaturalPerson.ID_PHOTOFLAG = this.txtID_PHOTOFLAG.Text;
        eAMLNaturalPerson.REG_ZIPCODE = this.txtREG_ZIPCODE.Text;
        eAMLNaturalPerson.REG_CITY = this.txtREG_CITY.Text;
        eAMLNaturalPerson.REG_ADDR1 = this.txtREG_ADDR1.Text;
        eAMLNaturalPerson.REG_ADDR2 = this.txtREG_ADDR2.Text;
        eAMLNaturalPerson.MAILING_CITY = this.txtMAILING_CITY.Text;
        eAMLNaturalPerson.MAILING_ADDR1 = this.txtMAILING_ADDR1.Text;
        eAMLNaturalPerson.MAILING_ADDR2 = this.txtMAILING_ADDR2.Text;
        eAMLNaturalPerson.EMAIL = this.hidEmailFall.Value.Trim();
        eAMLNaturalPerson.COMP_TEL1 = this.txtCOMP_TEL1.Text;
        eAMLNaturalPerson.COMP_TEL2 = this.txtCOMP_TEL2.Text;
        eAMLNaturalPerson.COMP_TEL3 = this.txtCOMP_TEL3.Text;
        eAMLNaturalPerson.MOBILE = this.txtMobilePhone.Text;
        eAMLNaturalPerson.NP_COMPANY_NAME = this.txtNP_COMPANY_NAME.Text;
        #region 行業別
        eAMLNaturalPerson.Industry1 = this.txtIndustry1.Text;
        eAMLNaturalPerson.Industry2 = this.txtIndustry2.Text;
        eAMLNaturalPerson.Industry3 = this.txtIndustry3.Text;
        #endregion
        eAMLNaturalPerson.CC = this.txtCC.Text;
        eAMLNaturalPerson.CC2 = this.txtCC2.Text;
        eAMLNaturalPerson.CC3 = this.txtCC3.Text;        
        #region 職稱
        eAMLNaturalPerson.TITLE = this.txtTITLE.Text;
        #endregion
        eAMLNaturalPerson.OC = this.txtOC.Text;
        #region 主要收入來源 以,隔開
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
        eAMLNaturalPerson.INCOME_SOURCE = value;
        #endregion
        eAMLNaturalPerson.OWNER_ID_OLD = this.txtOWNER_ID_OLD.Text;
        eAMLNaturalPerson.LAST_UPD_MAKER = this.txtLAST_UPD_MAKER.Text;
        eAMLNaturalPerson.LAST_UPD_CHECKER = this.txtLAST_UPD_CHECKER.Text;
        eAMLNaturalPerson.LAST_UPD_BRANCH = this.txtLAST_UPD_BRANCH.Text;
        eAMLNaturalPerson.isSCDD = this.chkisSCDD.Checked ? "Y" : "N";
        eAMLNaturalPerson.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
        eAMLNaturalPerson.MOD_USERID = this.eAgentInfo.agent_id;

        return eAMLNaturalPerson;
    }

    /// 作者 Ares Dennis
    /// 創建日期：2021/07/13
    /// 修改日期：2021/07/13
    /// <summary>
    /// 為網頁中的欄位賦值
    /// </summary>
    /// <param name="eAMLNaturalPerson">EntityAML_NATURALPERSON</param>
    private void SetValues(CSIPKeyInGUI.EntityLayer_new.EntityAML_NATURALPERSON eAMLNaturalPerson)
    {
        this.txtOWNER_ID2.Text = eAMLNaturalPerson.OWNER_ID;
        this.txtNameCH.Text = eAMLNaturalPerson.NameCH;
        this.txtNameEN.Text = eAMLNaturalPerson.NameEN;
        this.txtNameCH_OLD.Text = eAMLNaturalPerson.NameCH_OLD;
        this.txtBIRTH_DATE.Text = ConvertToROCYear(eAMLNaturalPerson.BIRTH_DATE);
        this.txtGENDER.Text = dropGENDER.Items.FindByValue(eAMLNaturalPerson.GENDER.Trim()) != null
            ? dropGENDER.Items.FindByValue(eAMLNaturalPerson.GENDER.Trim()).Text
            : eAMLNaturalPerson.GENDER.Trim();
        if (dropGENDER.Items.FindByValue(eAMLNaturalPerson.GENDER.Trim()) != null)//下拉式選單項目的值不為null
        {
            this.dropGENDER.SelectedValue = eAMLNaturalPerson.GENDER.Trim();
        }
        else
        {
            this.dropGENDER.SelectedValue = "";//請選擇
        }
        this.txtCountryCode.Text = eAMLNaturalPerson.CountryCode;
        this.txtCountryCode2.Text = dropCountry2.Items.FindByValue(eAMLNaturalPerson.CountryCode2.Trim()) != null
           ? dropCountry2.Items.FindByValue(eAMLNaturalPerson.CountryCode2.Trim()).Text
           : eAMLNaturalPerson.CountryCode2.Trim();
        if (dropCountry2.Items.FindByValue(eAMLNaturalPerson.CountryCode2.Trim()) != null)//下拉式選單項目的值不為null
        {
            this.dropCountry2.SelectedValue = eAMLNaturalPerson.CountryCode2.Trim();
        }
        else
        {
            this.dropCountry2.SelectedValue = "";//請選擇
        }

        this.txtID_ISSUEDATE.Text = ConvertToROCYear(eAMLNaturalPerson.ID_ISSUEDATE);
        this.txtID_ISSUEPLACE.Text = eAMLNaturalPerson.ID_ISSUEPLACE;
        this.txtID_REPLACETYPE.Text = eAMLNaturalPerson.ID_REPLACETYPE;
        this.txtID_PHOTOFLAG.Text = eAMLNaturalPerson.ID_PHOTOFLAG;
        this.txtREG_ZIPCODE.Text = eAMLNaturalPerson.REG_ZIPCODE;
        this.txtREG_CITY.Text = eAMLNaturalPerson.REG_CITY;
        this.txtREG_ADDR1.Text = eAMLNaturalPerson.REG_ADDR1;
        this.txtREG_ADDR2.Text = eAMLNaturalPerson.REG_ADDR2;
        this.txtMAILING_CITY.Text = eAMLNaturalPerson.MAILING_CITY;
        this.txtMAILING_ADDR1.Text = eAMLNaturalPerson.MAILING_ADDR1;
        this.txtMAILING_ADDR2.Text = eAMLNaturalPerson.MAILING_ADDR2;
        this.hidEmailFall.Value = eAMLNaturalPerson.EMAIL;
        SetEmailValue(eAMLNaturalPerson.EMAIL);// 電子郵件信箱
        this.txtCOMP_TEL1.Text = eAMLNaturalPerson.COMP_TEL1;
        this.txtCOMP_TEL2.Text = eAMLNaturalPerson.COMP_TEL2;
        this.txtCOMP_TEL3.Text = eAMLNaturalPerson.COMP_TEL3;
        this.txtMobilePhone.Text = eAMLNaturalPerson.MOBILE;
        this.txtNP_COMPANY_NAME.Text = eAMLNaturalPerson.NP_COMPANY_NAME;
        this.txtIndustry1.Text = eAMLNaturalPerson.Industry1.Trim();//行業別1
        if (eAMLNaturalPerson.Industry1.Trim() != "")
            setIndustryChName("txtIndustry1", "HQlblIndustry1", eAMLNaturalPerson.Industry1.Trim(), "1");
        this.txtIndustry2.Text = eAMLNaturalPerson.Industry2.Trim();//行業別2
        if (eAMLNaturalPerson.Industry2.Trim() != "")
            setIndustryChName("txtIndustry2", "HQlblIndustry2", eAMLNaturalPerson.Industry2.Trim(), "2");
        this.txtIndustry3.Text = eAMLNaturalPerson.Industry3.Trim();//行業別3
        if (eAMLNaturalPerson.Industry3.Trim() != "")
            setIndustryChName("txtIndustry3", "HQlblIndustry3", eAMLNaturalPerson.Industry3.Trim(), "3");
        this.txtCC.Text = eAMLNaturalPerson.CC;
        if (eAMLNaturalPerson.CC.Trim() != "")
            setCC_ChName("txtCC", "HQlblHCOP_CC_Cname1", eAMLNaturalPerson.CC.Trim(), "1");

        this.txtCC2.Text = eAMLNaturalPerson.CC2;
        if (eAMLNaturalPerson.CC2.Trim() != "")
            setCC_ChName("txtCC2", "HQlblHCOP_CC_Cname2", eAMLNaturalPerson.CC2.Trim(), "2");

        this.txtCC3.Text = eAMLNaturalPerson.CC3;
        if (eAMLNaturalPerson.CC3.Trim() != "")
            setCC_ChName("txtCC3", "HQlblHCOP_CC_Cname3", eAMLNaturalPerson.CC3.Trim(), "3");

        this.txtTITLE.Text = eAMLNaturalPerson.TITLE.Trim();//職稱
        if (eAMLNaturalPerson.TITLE.Trim() != "")
            setTITLEName("txtTITLE", "HQlblTITLE", eAMLNaturalPerson.TITLE.Trim());
        this.txtOC.Text = eAMLNaturalPerson.OC;
        if (eAMLNaturalPerson.OC != "")
            setOC_ChName("txtOC", "HQlblHCOP_OC_Cname", eAMLNaturalPerson.OC);
        #region 主要收入來源
        string incomeSource = eAMLNaturalPerson.INCOME_SOURCE;
        for (int i = 1; i < 10; i++)
        {
            if (incomeSource.IndexOf('1', i - 1, 1) != -1)//被勾選的是1
            {
                incomeSource = incomeSource.Remove(i - 1, 1).Insert(i - 1, i.ToString());
            }
        }
        incomeSource = string.Join(",", incomeSource.Replace("0", "").ToCharArray());//清除0,剩餘的用逗號分開 ex:1,2,3

        string[] incomes = incomeSource.Split(',');
        DataTable dt = BRPostOffice_CodeType.GetCodeType("17");//主要收入來源
        List<string> codes = new List<string>();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            codes.Add(dt.Rows[i][0].ToString());
        }
        foreach (string income in incomes)
        {
            if (codes.Contains(income))
            {
                CheckBox checkBox = FindControl("chkIncome" + income) as CheckBox;
                checkBox.Checked = true;
            }
        }
        #endregion
        this.txtOWNER_ID_OLD.Text = eAMLNaturalPerson.OWNER_ID_OLD;
        this.txtLAST_UPD_MAKER.Text = eAMLNaturalPerson.LAST_UPD_MAKER;
        this.txtLAST_UPD_CHECKER.Text = eAMLNaturalPerson.LAST_UPD_CHECKER;
        this.txtLAST_UPD_BRANCH.Text = eAMLNaturalPerson.LAST_UPD_BRANCH;
        this.chkisSCDD.Checked = eAMLNaturalPerson.isSCDD == "Y";

        //中文長姓名
        this.txtNameCH_L.Text = eAMLNaturalPerson.NameCH_L;
        this.txtNameCH_Pinyin.Text = eAMLNaturalPerson.NameCH_Pinyin;
        //當中文長姓名不為空時，中文長姓名flag要打勾
        if (!eAMLNaturalPerson.NameCH_L.Trim().Equals(""))
            chkisLongName_c.Checked = true;
        CheckBox_CheckedChanged(chkisLongName_c, null);
    }

    /// 作者 Ares Dennis
    /// 創建日期：2021/07/13
    /// 修改日期：2021/07/13
    /// <summary>
    /// 為網頁中的欄位賦值
    /// </summary>
    protected void SetValues()
    {
        Hashtable htReturn = (Hashtable)ViewState["HtgInfo"];
        base.strHostMsg += htReturn["HtgSuccess"].ToString();// 主機返回成功訊息

        #region        
        this.txtOWNER_ID2.Text = htReturn["OWNER_ID"].ToString();//自然人身份ID
        this.txtNameCH.Text = htReturn["OWNER_CHINESE_NAME"].ToString();//中文姓名
        this.txtNameEN.Text = htReturn["OWNER_ENGLISH_NAME"].ToString();//英文姓名        
        this.txtBIRTH_DATE.Text = ConvertToROCYear(htReturn["OWNER_BIRTH_DATE"].ToString());//生日
        if (htReturn["GENDER"].ToString().Trim() == "M" || htReturn["GENDER"].ToString().Trim() == "F")
        {
            this.dropGENDER.SelectedValue = htReturn["GENDER"].ToString();//性別
            this.txtGENDER.Text = dropGENDER.Items.FindByValue(htReturn["GENDER"].ToString().Trim()).ToString();//性別
        }
        else
        {
            this.dropGENDER.SelectedValue = "";//性別
            this.txtGENDER.Text = "請選擇";
        }

        this.txtCountryCode.Text = htReturn["OWNER_NATION"].ToString();//國籍1
        this.txtCountryCode2.Text = htReturn["COUNTRY_CODE_2"].ToString() == "" ? dropCountry2.Items.FindByValue("").ToString() : htReturn["COUNTRY_CODE_2"].ToString();//國籍2
        this.dropCountry2.SelectedValue = htReturn["COUNTRY_CODE_2"].ToString() == "" ? "" : htReturn["COUNTRY_CODE_2"].ToString();//國籍2        
        this.txtID_ISSUEDATE.Text = ConvertToROCYear(htReturn["OWNER_ID_ISSUE_DATE"].ToString());//發證日期
        this.txtID_ISSUEPLACE.Text = htReturn["OWNER_ID_ISSUE_PLACE"].ToString();//發證地點
        this.txtID_REPLACETYPE.Text = htReturn["OWNER_ID_REPLACE_TYPE"].ToString();//領換補類別
        this.txtID_PHOTOFLAG.Text = htReturn["ID_PHOTO_FLAG"].ToString();//有無照片
        this.txtREG_ZIPCODE.Text = htReturn["REG_ZIP_CODE"].ToString();//登記地址郵遞區號
        this.txtREG_CITY.Text = htReturn["REG_CITY"].ToString();//登記地址城市
        this.txtREG_ADDR1.Text = htReturn["REG_ADDR1"].ToString();//登記地址第一段
        this.txtREG_ADDR2.Text = htReturn["REG_ADDR2"].ToString().Trim() == "" ? "Ｘ" : htReturn["REG_ADDR2"].ToString();//登記地址第二段
        this.txtMAILING_CITY.Text = htReturn["MAILING_CITY"].ToString();//通訊地址城市
        this.txtMAILING_ADDR1.Text = htReturn["MAILING_ADDR1"].ToString();//通訊地址第一段
        this.txtMAILING_ADDR2.Text = htReturn["MAILING_ADDR2"].ToString().Trim() == "" ? "Ｘ" : htReturn["MAILING_ADDR2"].ToString();//通訊地址第二段
        this.hidEmailFall.Value = htReturn["EMAIL"].ToString().Trim();
        SetEmailValue(htReturn["EMAIL"].ToString().Trim());//EMAIL_Fall        
        string[] compTel = htReturn["COMP_TEL"].ToString().Split('-');
        this.txtCOMP_TEL1.Text = compTel.Length > 0 ? compTel[0] : "";//連絡電話
        this.txtCOMP_TEL2.Text = compTel.Length > 1 ? compTel[1] : "";//連絡電話2
        this.txtCOMP_TEL3.Text = compTel.Length > 2 ? compTel[2] : "";//連絡電話3
        if (this.txtCOMP_TEL2.Text.Trim() != "" && this.txtCOMP_TEL3.Text.Trim() == "")
        {
            this.txtCOMP_TEL3.Text = "X";//連絡電話3空值帶X
        }
        this.txtMobilePhone.Text = htReturn["MOBILE"].ToString();//行動電話
        this.txtNP_COMPANY_NAME.Text = htReturn["NP_COMPANY_NAME"].ToString();//任職公司名稱
        this.txtCC.Text = htReturn["CC"].ToString().Trim();//行業別編號1
        if (htReturn["CC"].ToString().Trim() != "")
            setCC_ChName("txtCC", "HQlblHCOP_CC_Cname1", htReturn["CC"].ToString().Trim(), "1");
        this.txtCC2.Text = htReturn["CC_2"].ToString().Trim();//行業別編號2
        if (htReturn["CC_2"].ToString().Trim() != "")
            setCC_ChName("txtCC2", "HQlblHCOP_CC_Cname2", htReturn["CC_2"].ToString().Trim(), "2");
        this.txtCC3.Text = htReturn["CC_3"].ToString().Trim();//行業別編號3
        if (htReturn["CC_3"].ToString().Trim() != "")
            setCC_ChName("txtCC3", "HQlblHCOP_CC_Cname3", htReturn["CC_3"].ToString().Trim(), "3");
        if (!string.IsNullOrEmpty(txtCC.Text))//行業編號1有值的話, 行業別1就帶值
        {
            string[] allAMLCC = hidAMLCC.Value.Split(':');
            if (Array.IndexOf(allAMLCC, txtCC.Text) != -1) //行業編號存在
            {
                DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", txtCC.Text.Trim());
                if (result.Rows[0]["DESCRIPTION"].ToString().Trim() != "")//有找到行業別
                {
                    DataTable result2 = BRPostOffice_CodeType.GetCodeTypeByCodeID("18", result.Rows[0]["DESCRIPTION"].ToString());

                    if (result2.Rows[0]["CODE_ID"].ToString().Trim() != this.txtIndustry2.Text && result2.Rows[0]["CODE_ID"].ToString().Trim() != this.txtIndustry3.Text)
                    {
                        this.txtIndustry1.Text = result2.Rows[0]["CODE_ID"].ToString().Trim();
                        if (result2.Rows[0]["CODE_ID"].ToString().Trim() != "")
                            setIndustryChName("txtIndustry1", "HQlblIndustry1", result2.Rows[0]["CODE_ID"].ToString().Trim(), "1");
                    }
                }

            }
        }
        if (!string.IsNullOrEmpty(txtCC2.Text))//行業編號2有值的話, 行業別2就帶值
        {
            if (this.txtCC2.Text != this.txtCC.Text && this.txtCC2.Text != this.txtCC3.Text)
            {
                string[] allAMLCC = hidAMLCC.Value.Split(':');
                if (Array.IndexOf(allAMLCC, txtCC2.Text) != -1) //行業編號存在
                {
                    DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", txtCC2.Text.Trim());
                    if (result.Rows[0]["DESCRIPTION"].ToString().Trim() != "")//有找到行業別
                    {
                        DataTable result2 = BRPostOffice_CodeType.GetCodeTypeByCodeID("18", result.Rows[0]["DESCRIPTION"].ToString());

                        if (result2.Rows[0]["CODE_ID"].ToString().Trim() != this.txtIndustry1.Text && result2.Rows[0]["CODE_ID"].ToString().Trim() != this.txtIndustry3.Text)
                        {
                            this.txtIndustry2.Text = result2.Rows[0]["CODE_ID"].ToString().Trim();
                            if (result2.Rows[0]["CODE_ID"].ToString().Trim() != "")
                                setIndustryChName("txtIndustry2", "HQlblIndustry2", result2.Rows[0]["CODE_ID"].ToString().Trim(), "2");
                        }
                    }
                }
            }
        }
        if (!string.IsNullOrEmpty(txtCC3.Text))//行業編號3有值的話, 行業別3就帶值
        {
            if (this.txtCC3.Text != this.txtCC.Text && this.txtCC3.Text != this.txtCC2.Text)
            {
                string[] allAMLCC = hidAMLCC.Value.Split(':');
                if (Array.IndexOf(allAMLCC, txtCC3.Text) != -1) //行業編號存在
                {
                    DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", txtCC3.Text.Trim());
                    if (result.Rows[0]["DESCRIPTION"].ToString().Trim() != "")//有找到行業別
                    {
                        DataTable result2 = BRPostOffice_CodeType.GetCodeTypeByCodeID("18", result.Rows[0]["DESCRIPTION"].ToString());

                        if (result2.Rows[0]["CODE_ID"].ToString().Trim() != this.txtIndustry1.Text && result2.Rows[0]["CODE_ID"].ToString().Trim() != this.txtIndustry2.Text)
                        {
                            this.txtIndustry3.Text = result2.Rows[0]["CODE_ID"].ToString().Trim();
                            if (result2.Rows[0]["CODE_ID"].ToString().Trim() != "")
                                setIndustryChName("txtIndustry3", "HQlblIndustry3", result2.Rows[0]["CODE_ID"].ToString().Trim(), "3");
                        }
                    }
                }
            }
        }
        this.txtOC.Text = htReturn["OC"].ToString().Trim();//職稱編號
        if (htReturn["OC"].ToString().Trim() != "")
            setOC_ChName("txtOC", "HQlblHCOP_OC_Cname", htReturn["OC"].ToString().Trim());

        if (!string.IsNullOrEmpty(txtOC.Text))//職稱編號有值的話,職稱就帶值
        {
            string[] allAMLOC = hidOC.Value.Split(':');
            if (Array.IndexOf(allAMLOC, txtOC.Text) != -1)//職稱編號存在
            {
                DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID("16", txtOC.Text.Trim());
                DataTable result2 = BRPostOffice_CodeType.GetCodeTypeByCodeID("19", result.Rows[0]["DESCRIPTION"].ToString());
                //20211227_Ares_Jack_職稱編號有多個大類就不自動帶職稱
                string[] allAMLOC_DESCRIPTION = result.Rows[0]["DESCRIPTION"].ToString().Trim().Split('/');
                if (allAMLOC_DESCRIPTION.Length  < 2)
                {
                    this.txtTITLE.Text = result2.Rows[0]["CODE_ID"].ToString().Trim();
                    if (result2.Rows[0]["CODE_ID"].ToString().Trim() != "")
                        setTITLEName("txtTITLE", "HQlblTITLE", result2.Rows[0]["CODE_ID"].ToString().Trim());
                }
            }   
        }
        #region 主要收入來源
        if (!string.IsNullOrEmpty(htReturn["INCOME_SOURCE"].ToString()))
        {
            this.hidIncome.Value = htReturn["INCOME_SOURCE"].ToString();//主要收入及資產來源        
            string incomeSource = htReturn["INCOME_SOURCE"].ToString();
            for (int i = 1; i < 10; i++)
            {
                if (incomeSource.IndexOf('1', i - 1, 1) != -1)//被勾選的是1
                {
                    incomeSource = incomeSource.Remove(i - 1, 1).Insert(i - 1, i.ToString());
                }
            }
            incomeSource = string.Join(",", incomeSource.Replace("0", "").ToCharArray());//清除0,剩餘的用逗號分開 ex:1,2,3

            string[] incomes = incomeSource.Split(',');
            DataTable dt = BRPostOffice_CodeType.GetCodeType("17");//主要收入來源
            List<string> codes = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                codes.Add(dt.Rows[i][0].ToString());
            }
            foreach (string income in incomes)
            {
                if (codes.Contains(income))
                {
                    CheckBox checkBox = FindControl("chkIncome" + income) as CheckBox;
                    checkBox.Checked = true;
                }
            }
        }
        #endregion
        this.txtOWNER_ID_OLD.Text = htReturn["ID_OLD"].ToString();//負責人舊ID
        this.txtLAST_UPD_MAKER.Text = htReturn["LAST_UPD_MAKER"].ToString();//資料最後異動MAKER
        this.txtLAST_UPD_CHECKER.Text = htReturn["LAST_UPD_CHECKER"].ToString();//資料最後異動CHECKER
        this.txtLAST_UPD_BRANCH.Text = htReturn["LAST_UPDATE_BRANCH"].ToString();//資料最後異動分行        
        #endregion

        if (!string.IsNullOrEmpty(htReturn["OWNER_LNAM_FLAG"].ToString()) && htReturn["OWNER_LNAM_FLAG"].ToString().Trim().Equals("Y"))
        {
            chkisLongName_c.Checked = true;
            EntityHTG_JC68 htReturn_JC68 = GetJC68(htReturn["OWNER_ID"].ToString().Trim());
            txtNameCH_L.Text = htReturn_JC68.LONG_NAME;//中文長姓名
            txtNameCH_Pinyin.Text = htReturn_JC68.PINYIN_NAME;//羅馬拼音
        }

        CheckBox_CheckedChanged(chkisLongName_c, null);
    }

    /// <summary>
    /// 檢查下拉選單
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtCodeType_TextChanged(object sender, EventArgs e)
    {
        CustTextBox txt = (CustTextBox)sender;
        string _codeType = string.Empty;
        string _codeName = string.Empty;
        string _dropID = string.Empty;

        switch (txt.ID.Trim())
        {
            case "txtCountryCode"://國籍1
                _dropID = "dropCountry1";
                _codeType = "1";
                break;
            case "txtCountryCode2"://國籍2
                _dropID = "dropCountry2";
                _codeType = "1";
                break;                        
            default:
                break;
        }
        string result = checkCodeType(_codeType, txt.ID, false, _codeName);
        if (result != "error")
        {
            DropDownList drop = FindControl(_dropID) as DropDownList;
            if (txt.Text != "")
                drop.SelectedValue = drop.Items.FindByValue(txt.Text.ToUpper()).Text;
        }
    }

    #endregion    

    #region 長姓名需求
    /// <summary>
    /// 長姓名flag勾選時，checkbox控制事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void CheckBox_CheckedChanged(object sender, EventArgs e)
    {
        string lid = string.Empty;
        CustCheckBox chk = (CustCheckBox)sender;
        string CtrlName1 = string.Empty;//(原)人員名稱
        string CtrlName2 = "txtNameCH_L";//中文長姓名
        string CtrlName3 = "txtNameCH_Pinyin";//羅馬拼音        

        CustTextBox contNameL = this.FindControl(CtrlName2) as CustTextBox;
        CustTextBox contNamePinyin = this.FindControl(CtrlName3) as CustTextBox;

        //修正粗框問題        
        contNameL.Enabled = chk.Checked;
        contNameL.BackColor = chk.Checked ? Color.Empty : Color.LightGray;
        contNamePinyin.Enabled = chk.Checked;
        contNamePinyin.BackColor = chk.Checked ? Color.Empty : Color.LightGray;

        if (!chk.Checked)
        {
            contNameL.Text = "";
            contNamePinyin.Text = "";
        }
        else
            contNameL.Focus();
    }

    //取長姓名的內容值
    private EntityHTG_JC68 GetJC68(string strID)
    {
        EntityHTG_JC68 _result = new EntityHTG_JC68();
        using (BRHTG_JC68 obj = new BRHTG_JC68("P010105040001"))
        {
            EntityHTG_JC68 _data = new EntityHTG_JC68();

            _data.ID = strID;
            _result = obj.getData(_data, eAgentInfo, "11");
        }
        return _result;
    }
    #endregion

    #region 檢核
    /// 作者 Ares Jack
    /// 創建日期：2022/01/18
    /// 修改日期：2022/01/18
    /// <summary>
    /// 檢核【行業別】與【行業別編號】資料需相互對應
    /// </summary>
    /// <param name="txtCC"></param>
    private string check_CC_Industry(string txtCC, string no)
    {
        try
        {
            DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", txtCC.Trim());
            if (result.Rows[0]["DESCRIPTION"].ToString().Trim() != txtIndustry1.Text && result.Rows[0]["DESCRIPTION"].ToString().Trim() != txtIndustry2.Text && result.Rows[0]["DESCRIPTION"].ToString().Trim() != txtIndustry3.Text || result.Rows[0]["DESCRIPTION"].ToString().Trim() == "")
            {
                base.sbRegScript.Append("alert('行業別編號" + (no.Trim() == "" ? "1" : no) + "錯誤!!');$('#txtCC" + no.Trim() + "').focus();");
                return "err";
            }
            return "";
        }
        catch (Exception ex)
        {

            Logging.Log(ex);
            return "";
        }
    }

    /// 作者 Ares Jack
    /// 創建日期：2022/01/18
    /// 修改日期：2022/01/18
    /// <summary>
    /// 檢核【行業別】
    /// </summary>
    /// <param name="txtIndustry"></param>
    private string check_Industry(string txtIndustry, string no)
    {
        try
        {
            DataTable industryDT = BRPostOffice_CodeType.GetCodeType("18");
            ListItem listItem = null;

            string industryText = string.Empty;
            for (int i = 0; i < industryDT.Rows.Count; i++)
            {
                listItem = new ListItem();
                listItem.Text = industryDT.Rows[i][0].ToString();
                industryText += listItem.Text + ':';
            }

            string[] allAMLIndustryText = industryText.Split(':');

            if (Array.IndexOf(allAMLIndustryText, txtIndustry) == -1)
            {
                base.sbRegScript.Append("alert('行業別" + no.Trim() + "不存在!');$('#txtIndustry" + no.Trim() + "').focus();");
                return "err";
            }
            return "";
        }
        catch (Exception ex)
        {

            Logging.Log(ex);
            return "";
        }
    }

    /// 作者 Ares Jack
    /// 創建日期：2022/01/18
    /// 修改日期：2022/01/18
    /// <summary>
    /// 檢核【行業別編號】
    /// </summary>
    /// <param name="txtCC"></param>
    private string check_CC(string txtCC, string no)
    {
        try
        {
            if (txtCC.Length != 7)
            {
                base.sbRegScript.Append("alert('行業編號" + (no.Trim() == "" ? "1" : no) + "請輸入7碼數字!');$('#txtCC" + no.Trim() + "').focus();");
                return "err";
            }
            else
            {
                string[] allAMLCC = hidAMLCC.Value.Split(':');
                if (Array.IndexOf(allAMLCC, txtCC) == -1)
                {
                    base.sbRegScript.Append("alert('行業編號" + (no.Trim() == "" ? "1" : no) + "不存在!');$('#txtCC" + no.Trim() + "').focus();");
                    return "err";
                }

            }
            return "";
        }
        catch (Exception ex)
        {

            Logging.Log(ex);
            return "";
        }
    }
    #endregion

}
