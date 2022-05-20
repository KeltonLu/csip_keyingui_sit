// *****************************************************************
//   作    者：Ares Dennis
//   功能說明：自然人收單 新增/修改二KEY
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
using System.Text;
using Framework.Data.OM.Transaction;
using Framework.Common.Logging;

public partial class P010105050001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    //SOC所需資訊
    private structPageInfo sPageInfo;//*記錄網頁訊息
    private Dictionary<string, string> compareDC = new Dictionary<string, string>();
    bool sendJC68 = false;// 中文長姓名電文JC68
    bool sendJC70 = false;// 送別名電文JC70
    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CommonFunction.SetControlsEnabled(pnlText, false);// 清空網頁中所有的輸入欄位            
            LoadDropDownList();
            this.btnAdd.Enabled = false;
            base.sbRegScript.Append(BaseHelper.SetFocus("txtOWNER_ID"));// 將自然人身分證字號設為輸入焦點
        }

        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//SOC所需資訊
    }

    /// 作者 Ares Dennis
    /// 創建日期：2021/07/13
    /// 修改日期：2021/07/13
    /// 修改紀錄：新增查詢條件, MOD_DATE 須為當天日期(yyyyMMdd) by Ares Stanley 20211217
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
        #endregion

        // 查詢資料庫
        EntitySet<CSIPKeyInGUI.EntityLayer_new.EntityAML_NATURALPERSON> eAMLNaturalPersonSet = null;
        string today = DateTime.Now.ToString("yyyyMMdd");
        try
        {
            eAMLNaturalPersonSet = CSIPKeyInGUI.BusinessRules_new.BRAML_NATURALPERSON.SelectEntitySet(this.txtOWNER_ID.Text.Trim(), "2", today);//先讀取二key資料
            
            //刪除 keyin_day 非當日資料 by Ares Stanley 20211217
            BRAML_NATURALPERSON.DeleteNotTodayKData(this.txtOWNER_ID.Text.Trim(), "2", today);
            
            if (eAMLNaturalPersonSet.Count == 0)
            {
                eAMLNaturalPersonSet = CSIPKeyInGUI.BusinessRules_new.BRAML_NATURALPERSON.SelectEntitySet(this.txtOWNER_ID.Text.Trim(), "1", today);
                
                //刪除 keyin_day 非當日資料 by Ares Stanley 20211217
                BRAML_NATURALPERSON.DeleteNotTodayKData(this.txtOWNER_ID.Text.Trim(), "1", today);
            }

            //若無資料，提示1KEY無資料後進入一key畫面
            if (eAMLNaturalPersonSet.Count == 0)
            {
                string sAlertMsg = MessageHelper.GetMessages("01_01090200_001");
                sbRegScript.Append("alert('" + sAlertMsg + "');window.location.href = 'P010105040001.aspx';");
                return;
            }
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            ClearPage(false);
            return;
        }

        if (eAMLNaturalPersonSet != null && eAMLNaturalPersonSet.Count > 0)
        {
            ClearPage(true);
            btnAdd.Enabled = true;
            ViewState["EntityAMLNaturalPerson"] = CSIPKeyInGUI.BusinessRules_new.BRAML_NATURALPERSON.SelectEntitySet(this.txtOWNER_ID.Text.Trim(), "1", today).GetEntity(0);//一key資料
            SetValues(eAMLNaturalPersonSet.GetEntity(0));// 為網頁中的欄位賦值  
            this.txtOWNER_ID2.Enabled = false;// 身分證字號
            this.txtCountryCode.Enabled = false;
            this.dropCountry1.Enabled = false;
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01090200_001");
            ClearPage(false);
        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtNameCH"));// 將中文姓名設為輸入焦點
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
        //行業別1不得為空
        if (txtIndustry1.Text.Trim().Equals(""))
        {
            base.sbRegScript.Append("alert('行業別1不得為空!');$('#txtIndustry1').focus();");
            return;
        }
        //行業編號1不得為空
        if (txtCC.Text.Equals(""))
        {
            base.sbRegScript.Append("alert('行業編號1不得為空!');$('#txtCC').focus();");
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

            if (txtNameCH_Pinyin.Text.Trim().Equals("X"))
            {
                sendJC68 = true;
            }
        }

        // 曾用過的姓名
        // 曾用過姓名改查JC70, 若有資料則不異動, 若無資料則透過JC70新增 by Ares Stanley 20211210
        if (!txtNameCH_OLD.Text.Trim().Equals(""))
        {
            Hashtable htInput = new Hashtable();
            htInput.Add("FUNCTION_CODE", "I");// 查詢
            htInput.Add("ID", txtOWNER_ID2.Text.Trim());// 身分證字號  
            htInput.Add("NAME", txtNameCH_OLD.Text.Trim()); //曾使用別名

            Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC70, htInput, false, "11", eAgentInfo);
            if (!htReturn.Contains("HtgMsg"))
            {
                if(htReturn["MESSAGE_CHI"].ToString().Trim() != "無此筆查詢資料，請重新輸入")
                {
                    //如果有曾使用姓名則不異動
                }
                else
                {
                    //如果沒有曾使用姓名, 則新增
                    sendJC70 = true;
                }
            }
            else
            {
                //JC70查詢錯誤
                base.sbRegScript.Append(string.Format("alert('別名查詢失敗：{0}');$('#txtNameCH_OLD').focus();", htReturn["HtgMsg"].ToString()));
                return;
            }      
        }

        //收入及資產來源
        if (!chkIncome1.Checked && !chkIncome2.Checked && !chkIncome3.Checked && !chkIncome4.Checked && !chkIncome5.Checked && !chkIncome6.Checked && !chkIncome7.Checked && !chkIncome8.Checked && !chkIncome9.Checked)
        {
            base.sbRegScript.Append("alert('請勾選收入及資料來源!');$('#chkIncome1').focus();");
            return;
        }

        // 內容檢核
        if (!chkConfirm.Checked)
        {
            base.sbRegScript.Append("alert('請審核並勾選以上資料確認無誤');$('#chkConfirm').focus();");
            return;
        }
        #endregion

        try
        {
            // 新增或修改資料 
            if (UpdateDBData())
            {
                //比對12K差異用
                List<string> errMsgT = new List<string>();
                //比對所有欄位，是否有差異
                bool retCompare = Compare(ref errMsgT);
                //有差異則提示後離開
                if (!retCompare)
                {
                    StringBuilder sb = new StringBuilder();
                    int linC = 0;
                    foreach (string oitem in errMsgT)
                    {
                        sb.Append(oitem);
                        linC++;
                        if (linC > 10)
                        {
                            sb.Append("---顯示前十筆，以下省略---");
                            break;
                        }
                    }
                    strAlertMsg = @"『" + MessageHelper.GetMessage("01_01050101_006") + "』" + "\\n" + sb.ToString();
                    //sbRegScript.Append("alert('" + strAlertMsg + "');");
                    return;
                }
                else
                {
                    // 更新主機JC66
                    if (UpdateJC66())
                    {
                        #region 增送JC70
                        if (sendJC70)
                        {
                            Hashtable JC70Obj = new Hashtable();
                            //狀態只有 I (查詢), C (異動)
                            JC70Obj.Add("FUNCTION_CODE", "C");
                            JC70Obj.Add("ID", txtOWNER_ID2.Text.Trim());
                            JC70Obj.Add("NAME", txtNameCH_OLD.Text.Trim());
                            //別名狀態（Ａ新增，Ｄ取消）
                            JC70Obj.Add("REC_STS", "A");
                            Hashtable htJC70Return = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC70, JC70Obj, false, "11", eAgentInfo);

                            if (!htJC70Return.Contains("HtgMsg"))
                            {
                                base.strHostMsg += htJC70Return["MESSAGE_CHI"].ToString();//*主機返回成功訊息 
                            }
                            else
                            {
                                base.strClientMsg += htJC70Return["HtgMsg"].ToString();
                                base.strHostMsg += htJC70Return["HtgMsg"].ToString();
                            }
                        }
                        #endregion

                        ///使用交易
                        using (OMTransactionScope ts = new OMTransactionScope())
                        {
                            bool dRet = true;

                            //清除資料庫
                            dRet = BRAML_NATURALPERSON.DeleteKData(txtOWNER_ID2.Text.Trim());

                            ts.Complete();
                            if (!dRet)
                            {
                                strAlertMsg = @"『" + MessageHelper.GetMessage("01_01050101_004") + "』";
                                return;
                            }
                        }

                        base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_00000000_005") + "');$('#txtOWNER_ID').focus();");
                        base.strClientMsg += MessageHelper.GetMessage("01_00000000_005");
                        ClearPage(false);
                        this.txtOWNER_ID.Text = "";//自然人身分證字號 清空
                    }
                }
            }
        }
        catch (Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex);
            return;
        }
    }

    /// 作者 Ares Dennis
    /// 創建日期：2021/07/13
    /// 修改日期：2021/07/13
    /// 修改紀錄：新增查詢條件, MOD_DATE 須為當天日期(yyyyMMdd) by Ares Stanley 20211217
    /// <summary>
    /// 新增或修改資料
    /// </summary>
    protected bool UpdateDBData()
    {
        EntitySet<CSIPKeyInGUI.EntityLayer_new.EntityAML_NATURALPERSON> eAMLNaturalPerson = null;
        string strOwnerID = txtOWNER_ID2.Text;
        string today = DateTime.Now.ToString("yyyyMMdd");

        try
        {
            eAMLNaturalPerson = CSIPKeyInGUI.BusinessRules_new.BRAML_NATURALPERSON.SelectEntitySet(strOwnerID, "2", today);
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            ClearPage(false);
            return false;
        }

        bool blnSucceed = false;// 是否新增或修改成功
        if (eAMLNaturalPerson != null && eAMLNaturalPerson.Count > 0)
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
        return blnSucceed;
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
    /// 使用者輸入職稱編號自動帶入職稱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtBasicAMLOC_TextChanged(object sender, EventArgs e)
    {
        setOC_ChName("txtOC", "HQlblHCOP_OC_Cname", this.txtOC.Text);
        base.sbRegScript.Append(BaseHelper.SetFocus("chkIncome1"));// 將收入來源設為輸入焦點
    }

    /// 作者 Ares Jack
    /// 創建日期：2021/11/16
    /// 修改日期：2021/11/16
    /// <summary>
    /// 使用者輸入行業別編號自動帶入行業別
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

        #region 載入職業編號
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
        this.chkConfirm.Checked = false;//內容檢核 清空
        //Email
        this.hidEmailFall.Value = "";
        this.txtEMAIL.Text = "";
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
            eAMLNaturalPerson.KEYIN_FLAG = "2";
            return CSIPKeyInGUI.BusinessRules_new.BRAML_NATURALPERSON.AddEntity(eAMLNaturalPerson);
        }
        catch(Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex);
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
            eAMLNaturalPerson.KEYIN_FLAG = "2";
            return CSIPKeyInGUI.BusinessRules_new.BRAML_NATURALPERSON.Update(eAMLNaturalPerson, eAMLNaturalPerson.OWNER_ID.Trim(), "2");
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
    /// 得到插入或更新資料的Entity
    /// </summary>
    /// <returns>Entity</returns>
    private Hashtable BuildJC66Hashtable()
    {
        Hashtable JC66Obj = new Hashtable();

        JC66Obj.Add("OWNER_ID", this.txtOWNER_ID2.Text);
        JC66Obj.Add("OWNER_CHINESE_NAME", this.txtNameCH.Text);
        JC66Obj.Add("OWNER_ENGLISH_NAME", this.txtNameEN.Text);
        JC66Obj.Add("OWNER_BIRTH_DATE", ConvertToDC(this.txtBIRTH_DATE.Text));
        JC66Obj.Add("GENDER", this.dropGENDER.Text.Trim().ToUpper());
        JC66Obj.Add("OWNER_NATION", this.txtCountryCode.Text);
        if (this.txtCountryCode2.Text.Trim() == "請選擇")
            JC66Obj.Add("COUNTRY_CODE_2", "");
        else
            JC66Obj.Add("COUNTRY_CODE_2", this.txtCountryCode2.Text.Trim().ToUpper());
        JC66Obj.Add("OWNER_ID_ISSUE_DATE", ConvertToDC(this.txtID_ISSUEDATE.Text));
        JC66Obj.Add("OWNER_ID_ISSUE_PLACE", this.txtID_ISSUEPLACE.Text);
        JC66Obj.Add("OWNER_ID_REPLACE_TYPE", this.txtID_REPLACETYPE.Text);
        JC66Obj.Add("ID_PHOTO_FLAG", this.txtID_PHOTOFLAG.Text);
        JC66Obj.Add("REG_ZIP_CODE", this.txtREG_ZIPCODE.Text);
        JC66Obj.Add("REG_CITY", this.txtREG_CITY.Text);
        JC66Obj.Add("REG_ADDR1", this.txtREG_ADDR1.Text);        
        //20211206_Ares_Jack_判斷輸入的戶籍地址若為x 則清空欄位值
        JC66Obj.Add("REG_ADDR2", LongNameRomaClean(this.txtREG_ADDR2.Text));
        JC66Obj.Add("OWNER_CITY", this.txtREG_CITY.Text);//登記地址 = 戶籍地址
        JC66Obj.Add("OWNER_ADDR1", this.txtREG_ADDR1.Text);//登記地址 = 戶籍地址        
        JC66Obj.Add("OWNER_ADDR2", LongNameRomaClean(this.txtREG_ADDR2.Text));//登記地址 = 戶籍地址
        JC66Obj.Add("MAILING_CITY", this.txtMAILING_CITY.Text);
        JC66Obj.Add("MAILING_ADDR1", this.txtMAILING_ADDR1.Text);        
        //20211206_Ares_Jack_判斷輸入的通訊地址若為x 則清空欄位值
        JC66Obj.Add("MAILING_ADDR2", LongNameRomaClean(this.txtMAILING_ADDR2.Text));
        JC66Obj.Add("EMAIL", this.hidEmailFall.Value.Trim());
        //20211206_Ares_Jack_判斷輸入的連絡電話第三攔若為x 則清空欄位值 //20211208_Ares_Jack_第三欄為空的話串接的 "-" 一併移除 
        string COMP_TEL_3 = ToNarrow(LongNameRomaClean(this.txtCOMP_TEL3.Text.Trim())) == "" ? "" : "-" + ToNarrow(LongNameRomaClean(this.txtCOMP_TEL3.Text.Trim()));
        JC66Obj.Add("COMP_TEL", this.txtCOMP_TEL1.Text + "-" + this.txtCOMP_TEL2.Text + COMP_TEL_3);
        JC66Obj.Add("MOBILE", this.txtMobilePhone.Text);
        JC66Obj.Add("NP_COMPANY_NAME", this.txtNP_COMPANY_NAME.Text);
        JC66Obj.Add("CC", this.txtCC.Text);
        JC66Obj.Add("CC_2", this.txtCC2.Text);
        JC66Obj.Add("CC_3", this.txtCC3.Text);        
        JC66Obj.Add("TITLE", "");// 因沒有對應的職稱，所以就放空白
        JC66Obj.Add("OC", this.txtOC.Text);
        #region 主要收入來源 
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
        JC66Obj.Add("INCOME_SOURCE",value);
        #endregion
        JC66Obj.Add("ID_OLD", this.txtOWNER_ID_OLD.Text);
        JC66Obj.Add("LAST_UPD_MAKER", this.txtLAST_UPD_MAKER.Text);
        JC66Obj.Add("LAST_UPD_CHECKER", this.txtLAST_UPD_CHECKER.Text);
        JC66Obj.Add("LAST_UPDATE_BRANCH", this.txtLAST_UPD_BRANCH.Text);

        if (chkisLongName_c.Checked)
        {
            JC66Obj.Add("OWNER_LNAME_FLAG", "Y");
            sendJC68 = true;//負責人長姓名打勾 增送JC68
        }

        //20211207_Ares_Jack_增加欄位
        JC66Obj.Add("EXAMINE_FLAG", this.chkisSCDD.Checked == true ? "A" : ""); //SCDD勾選判斷

        return JC66Obj;
    }

    /// 作者 Ares Jack
    /// 創建日期：2021/12/01
    /// 修改日期：2021/12/01
    /// <summary>
    /// JC68上送主機
    /// </summary>
    /// <returns>Entity</returns>
    private Hashtable BuildJC68Hashtable()
    {
        Hashtable JC68Obj = new Hashtable();

        JC68Obj.Add("ID", this.txtOWNER_ID2.Text);
        JC68Obj.Add("LONG_NAME", this.txtNameCH_L.Text);
        //判斷輸入的羅馬姓名若為x 則清空欄位值
        JC68Obj.Add("PINYIN_NAME", LongNameRomaClean(this.txtNameCH_Pinyin.Text));

        return JC68Obj;
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
        if(dropGENDER.Items.FindByValue(eAMLNaturalPerson.GENDER.Trim()) != null)//下拉式選單項目的值不為null
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
        if(dropCountry2.Items.FindByValue(eAMLNaturalPerson.CountryCode2.Trim()) != null)//下拉式選單項目的值不為null
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
        this.txtIndustry1.Text = eAMLNaturalPerson.Industry1.Trim();
        if (eAMLNaturalPerson.Industry1.Trim() != "")
            setIndustryChName("txtIndustry1", "HQlblIndustry1", eAMLNaturalPerson.Industry1.Trim(), "1");       
        this.txtIndustry2.Text = eAMLNaturalPerson.Industry2.Trim();
        if (eAMLNaturalPerson.Industry2.Trim() != "")
            setIndustryChName("txtIndustry2", "HQlblIndustry2", eAMLNaturalPerson.Industry2.Trim(), "2");        
        this.txtIndustry3.Text = eAMLNaturalPerson.Industry3.Trim();
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
        setOC_ChName("txtOC", "HQlblHCOP_OC_Cname", eAMLNaturalPerson.OC);
        #region 主要收入來源
        string incomeSource = eAMLNaturalPerson.INCOME_SOURCE;
        for (int i = 1; i < 10; i++)
        {
            if (incomeSource.IndexOf('1', i - 1, 1 ) != -1)//被勾選的是1
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

    private bool UpdateJC66()
    {
        Hashtable JC66QueryOBj = new Hashtable();
        JC66QueryOBj.Add("FUNCTION_CODE", "I");        
        //身分證字號 前八碼放 CORP_NO, 後兩碼放 CORP_SEQ  + "  "
        JC66QueryOBj.Add("CORP_NO", this.txtOWNER_ID2.Text.Substring(0, 8).Trim());
        JC66QueryOBj.Add("CORP_SEQ", this.txtOWNER_ID2.Text.Substring(8, 2).Trim() + "  ");
        JC66QueryOBj.Add("CORP_TYPE", "6");// 總公司類型

        // 主機資料
        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC66, JC66QueryOBj, false, "11", eAgentInfo);

        // 上送主機資料
        Hashtable JC66Obj = BuildJC66Hashtable();
        if(!htReturn.Contains("HtgMsg"))
        {
            JC66Obj.Add("FUNCTION_CODE", "C");
        }
        else if (htReturn["MESSAGE_TYPE"] != null && htReturn["MESSAGE_TYPE"].ToString().Trim() == "0006")
        {
            JC66Obj.Add("FUNCTION_CODE", "A");
        }
        else
        {

        }
        //身分證字號 前八碼放 CORP_NO, 後兩碼放 CORP_SEQ  + "  "
        JC66Obj.Add("CORP_NO", this.txtOWNER_ID2.Text.Substring(0, 8).Trim());
        JC66Obj.Add("CORP_SEQ", this.txtOWNER_ID2.Text.Substring(8, 2).Trim() + "  ");

        //預設欄位
        JC66Obj.Add("CORP_TYPE", "6");// 總公司類型
        JC66Obj.Add("REGISTER_NATION", txtCountryCode.Text);//註冊國籍
        JC66Obj.Add("REG_NAME", txtNameCH.Text);//總公司登記名稱
        JC66Obj.Add("COMPLEX_STR_CODE", "N");//複雜股權結構
        JC66Obj.Add("ISSUE_STOCK_FLAG", "N");//總公司是否發行無記名股票
        JC66Obj.Add("ALLOW_ISSUE_STOCK_FLAG", "N");//是否可發行無記名股票
        JC66Obj.Add("LAST_UPDATE_DATE", DateTime.Now.ToString("yyyyMMdd"));//資料最後異動日期
        JC66Obj.Add("OWNER_LNAM_FLAG", chkisLongName_c.Checked ? "Y" : "N");//中文長姓名checkbox

        Hashtable hstExmsP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC66, JC66Obj, false, "11", eAgentInfo);

        //20210824 EOS_AML(NOVA) by Ares Jack 若查無主機資料(新的一筆資料)也需比對異動欄位並紀錄到DB
        if (!hstExmsP4A.Contains("HtgMsg") || (hstExmsP4A["MESSAGE_TYPE"] != null && hstExmsP4A["MESSAGE_TYPE"].ToString().Trim() == "0124"))
        {
            base.strHostMsg += hstExmsP4A["HtgSuccess"].ToString();//*主機返回成功訊息

            List<string> changedFileds = new List<string>();
            List<string> changedFileds_jc68 = new List<string>();

            // 20210527 EOS_AML(NOVA) by Ares Dennis
            #region 異動記錄                
            bool isChanged = false;
            bool isChanged_LNameCompare = false;
            bool isChanged_PinyinCompare = false;
            bool isChanged_LnameFlag = false;

            #region 檢核欄位是否異動
            compareForAMLCheckLog(htReturn, JC66Obj, "OWNER_ID", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "OWNER_CHINESE_NAME", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "OWNER_ENGLISH_NAME", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "OWNER_BIRTH_DATE", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "GENDER", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "OWNER_NATION", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "COUNTRY_CODE_2", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "REGISTER_US_STATE", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "OWNER_ID_ISSUE_DATE", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "OWNER_ID_ISSUE_PLACE", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "OWNER_ID_REPLACE_TYPE", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "ID_PHOTO_FLAG", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "REG_ZIP_CODE", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "REG_CITY", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "REG_ADDR1", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "REG_ADDR2", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "MAILING_CITY", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "MAILING_ADDR1", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "MAILING_ADDR2", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "EMAIL", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "COMP_TEL", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "MOBILE", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "NP_COMPANY_NAME", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "CC", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "CC_2", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "CC_3", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "TITLE", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "OC", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "INCOME_SOURCE", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "ID_OLD", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "LAST_UPD_MAKER", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "LAST_UPD_CHECKER", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "LAST_UPDATE_BRANCH", ref isChanged, ref changedFileds);
            compareForAMLCheckLog(htReturn, JC66Obj, "EXAMINE_FLAG", ref isChanged, ref changedFileds);
            #endregion

            if (isChanged)
            {
                EntityAML_CHECKLOG eAMLCheckLog = new EntityAML_CHECKLOG();
                eAMLCheckLog.CORP_NO = JC66Obj["CORP_NO"].ToString().Trim() + JC66Obj["CORP_SEQ"].ToString().Trim();// 取完整身分證字號
                eAMLCheckLog.TRANS_ID = "CSIPJC66";
                eAMLCheckLog.LAST_UPD_BRANCH = txtLAST_UPD_BRANCH.Text.Trim();
                eAMLCheckLog.LAST_UPD_CHECKER = txtLAST_UPD_CHECKER.Text.Trim();
                eAMLCheckLog.LAST_UPD_MAKER = txtLAST_UPD_MAKER.Text.Trim();
                eAMLCheckLog.MOD_USERID = eAgentInfo.agent_id;
                eAMLCheckLog.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
                eAMLCheckLog.MOD_TIME = DateTime.Now.ToString("HHmmss");

                InsertAMLCheckLog(eAMLCheckLog);

                foreach(string field in changedFileds)
                {

                    //20211214_Ares_Jack_ field改成中文名稱
                    string fieldChName = string.Empty;
                    switch (field)
                    {
                        case "OWNER_ID":
                            fieldChName = BaseHelper.GetShowText("01_01090200_002");//自然人身分證字號
                            break;
                        case "OWNER_CHINESE_NAME":
                            fieldChName = BaseHelper.GetShowText("01_01090200_006");//中文姓名
                            break;
                        case "OWNER_ENGLISH_NAME":
                            fieldChName = BaseHelper.GetShowText("01_01090200_008");//英文名稱
                            break;
                        case "OWNER_BIRTH_DATE":
                            fieldChName = BaseHelper.GetShowText("01_01090200_009");//出生年月日
                            break;
                        case "GENDER":
                            fieldChName = BaseHelper.GetShowText("01_01090200_010");//性別
                            break;
                        case "OWNER_NATION":
                            fieldChName = BaseHelper.GetShowText("01_01090200_011");//國籍1
                            break;
                        case "COUNTRY_CODE_2":
                            fieldChName = BaseHelper.GetShowText("01_01090200_012");//國籍2
                            break;
                        case "OWNER_ID_ISSUE_DATE":
                            fieldChName = BaseHelper.GetShowText("01_01090200_013");//身分證發證日期
                            break;
                        case "OWNER_ID_ISSUE_PLACE":
                            fieldChName = BaseHelper.GetShowText("01_01090200_014");//發證地點
                            break;
                        case "OWNER_ID_REPLACE_TYPE":
                            fieldChName = BaseHelper.GetShowText("01_01090200_015");//領補換類別
                            break;
                        case "ID_PHOTO_FLAG":
                            fieldChName = BaseHelper.GetShowText("01_01090200_016");//有無照片
                            break;
                        case "REG_ZIP_CODE":
                            fieldChName = BaseHelper.GetShowText("01_01090200_059");//郵遞區號
                            break;
                        case "REG_CITY":
                            fieldChName = BaseHelper.GetShowText("01_01090200_017");//戶籍地址一
                            break;
                        case "REG_ADDR1":
                            fieldChName = BaseHelper.GetShowText("01_01090200_017");//戶籍地址二
                            break;
                        case "REG_ADDR2":
                            fieldChName = BaseHelper.GetShowText("01_01090200_017");//戶籍地址三
                            break;
                        case "MAILING_CITY":
                            fieldChName = BaseHelper.GetShowText("01_01090200_018");//通訊地址一
                            break;
                        case "MAILING_ADDR1":
                            fieldChName = BaseHelper.GetShowText("01_01090200_018");//通訊地址二
                            break;
                        case "MAILING_ADDR2":
                            fieldChName = BaseHelper.GetShowText("01_01090200_018");//通訊地址三
                            break;
                        case "EMAIL":
                            fieldChName = BaseHelper.GetShowText("01_01090200_019");//E-mail
                            break;
                        case "COMP_TEL":
                            fieldChName = BaseHelper.GetShowText("01_01090200_020");//連絡電話
                            break;
                        case "MOBILE":
                            fieldChName = BaseHelper.GetShowText("01_01090200_021");//行動電話
                            break;
                        case "NP_COMPANY_NAME":
                            fieldChName = BaseHelper.GetShowText("01_01090200_032");//任職公司
                            break;
                        case "CC":
                            fieldChName = BaseHelper.GetShowText("01_01090200_036");//行業別編號1
                            break;
                        case "CC_2":
                            fieldChName = BaseHelper.GetShowText("01_01090200_037");//行業別編號2
                            break;
                        case "CC_3":
                            fieldChName = BaseHelper.GetShowText("01_01090200_038");//行業別編號3
                            break;
                        case "TITLE":
                            fieldChName = BaseHelper.GetShowText("01_01090200_024");//職稱
                            break;
                        case "OC":
                            fieldChName = BaseHelper.GetShowText("01_01090200_025");//職稱編號
                            break;
                        case "INCOME_SOURCE":
                            fieldChName = BaseHelper.GetShowText("01_01090200_026");//收入及資產來源(複選)
                            break;
                        case "ID_OLD":
                            fieldChName = BaseHelper.GetShowText("01_01090200_004");//舊身分證字號
                            break;
                        case "LAST_UPD_MAKER":
                            fieldChName = BaseHelper.GetShowText("01_01090200_028");//資料最後異動MAKER
                            break;
                        case "LAST_UPD_CHECKER":
                            fieldChName = BaseHelper.GetShowText("01_01090200_029");//資料最後異動CHECKER
                            break;
                        case "LAST_UPDATE_BRANCH":
                            fieldChName = BaseHelper.GetShowText("01_01090200_027");//資料最後異動分行
                            break;
                        case "EXAMINE_FLAG":
                            fieldChName = BaseHelper.GetShowText("01_01090100_053");//是否已完成SCDD表
                            break;
                        default:
                            break;
                    }

                    //寫入customer_log
                    DataTable dtlog = CommonFunction.GetDataTable();
                    if (field.Trim() == "INCOME_SOURCE")
                    {
                        //20211214_Ares_Jack_客戶收入及資產主要收入來源 中文名稱
                        CommonFunction.UpdateLog(IncomeSourceChName(htReturn[field].ToString()), IncomeSourceChName(JC66Obj[field].ToString()), fieldChName, dtlog); //主機值, 輸入值, 異動欄位的中文名稱
                    }
                    else if(field.Trim() == "OWNER_BIRTH_DATE" || field.Trim() == "OWNER_ID_ISSUE_DATE")
                    {
                        //20211222_Ares_Jack_生日取民國年, 身分證發證日期取民國年
                        CommonFunction.UpdateLog(ConvertToROCYear(htReturn[field].ToString()), ConvertToROCYear(JC66Obj[field].ToString()), fieldChName, dtlog);
                    }
                    else
                    {
                        CommonFunction.UpdateLog(htReturn[field].ToString(), JC66Obj[field].ToString(), fieldChName, dtlog); //主機值, 輸入值, 異動欄位的中文名稱   
                    }
                                    
                    CommonFunction.InsertCustomerLog(dtlog, eAgentInfo, this.txtOWNER_ID2.Text, "P4A", (structPageInfo)Session["PageInfo"]);
                }                
            }
            #endregion            

            #region 增送JC68
            // 比對中文長姓名FLAG
            if (htReturn.Contains("OWNER_LNAM_FLAG") && string.IsNullOrEmpty(htReturn["OWNER_LNAM_FLAG"].ToString().Trim()))
            {
                htReturn["OWNER_LNAM_FLAG"] = "N";
            }
            compareForAMLCheckLog(htReturn, JC66Obj, "OWNER_LNAM_FLAG", ref isChanged_LnameFlag, ref changedFileds);

            Hashtable JC68QueryObj = new Hashtable();
            JC68QueryObj.Add("FUNCTION_CODE", "I"); //I:查詢, C:異動, A:新增
            JC68QueryObj.Add("ID", txtOWNER_ID2.Text);
            Hashtable htJC68QueryReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC68, JC68QueryObj, false, "21", eAgentInfo);

            Hashtable JC68Obj = BuildJC68Hashtable();//JC68上送主機
            if (chkisLongName_c.Checked || isChanged_LnameFlag == true)//假設現在為長姓名，或為現在FLAG狀態與主機不同時，才做異動比對與異動長姓名
            {
                //負責人長姓名
                compareForAMLCheckLog(htJC68QueryReturn, JC68Obj, "LONG_NAME", ref isChanged_LNameCompare, ref changedFileds_jc68);
                //負責人羅馬拼音
                compareForAMLCheckLog(htJC68QueryReturn, JC68Obj, "PINYIN_NAME", ref isChanged_PinyinCompare, ref changedFileds_jc68);
            }

            if (isChanged_LNameCompare == true || isChanged_PinyinCompare == true)//有差異才需打JC68
            {
                if (sendJC68)
                {
                    if (!htJC68QueryReturn.Contains("HtgMsg"))
                    {
                        JC68Obj.Add("FUNCTION_CODE", "C");
                    }
                    else if (htJC68QueryReturn["MESSAGE_TYPE"] != null && htJC68QueryReturn["MESSAGE_TYPE"].ToString().Trim() == "0006")
                    {
                        JC68Obj.Add("FUNCTION_CODE", "A"); //I:查詢, C:異動, A:新增
                    }

                    Hashtable htJC68Return = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC68, JC68Obj, false, "21", eAgentInfo);

                    if (!htJC68Return.Contains("HtgMsg"))
                    {
                        base.strHostMsg += htJC68Return["HtgSuccess"].ToString();//*主機返回成功訊息 
                    }
                    else
                    {
                        base.strClientMsg += htJC68Return["HtgMsg"].ToString();
                        base.strHostMsg += htJC68Return["HtgMsg"].ToString();
                    }
                }
                //20211223_Ares_Jack_寫入customer_log
                DataTable dtlog = CommonFunction.GetDataTable();
                CommonFunction.UpdateLog(htJC68QueryReturn["LONG_NAME"].ToString(), JC68Obj["LONG_NAME"].ToString(), "中文長姓名", dtlog);
                CommonFunction.UpdateLog(htJC68QueryReturn["PINYIN_NAME"].ToString(), JC68Obj["PINYIN_NAME"].ToString(), "羅馬拼音", dtlog);
                CommonFunction.InsertCustomerLog(dtlog, eAgentInfo, this.txtOWNER_ID2.Text, "P4A", (structPageInfo)Session["PageInfo"]);
            }
            #endregion

            return true;
        }
        else
        {
            base.strHostMsg += hstExmsP4A["HtgMsg"].ToString();
            return false;
        }
    }

    /// 作者 Ares Dennis
    /// 創建日期：2021/07/13
    /// 修改日期：2021/07/13
    /// <summary>
    /// 比較一次鍵檔和二次鍵檔的資料
    /// </summary>
    /// <returns></returns>
    private bool Compare(ref List<string> errMsgT)
    {
        bool blnSame = true;
        if (compareDC.Count == 0)
        {
            initcompareDC();
        }

        if (ViewState["EntityAMLNaturalPerson"] != null)
        {
            // 一key資料
            CSIPKeyInGUI.EntityLayer_new.EntityAML_NATURALPERSON eAMLNaturalPerson = (CSIPKeyInGUI.EntityLayer_new.EntityAML_NATURALPERSON)ViewState["EntityAMLNaturalPerson"];

            #region 比對資料
            CompareValueToUpper(txtOWNER_ID2, eAMLNaturalPerson.OWNER_ID, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtNameCH, eAMLNaturalPerson.NameCH, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtNameEN, eAMLNaturalPerson.NameEN, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtNameCH_L, eAMLNaturalPerson.NameCH_L, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtNameCH_Pinyin, eAMLNaturalPerson.NameCH_Pinyin, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtNameCH_OLD, eAMLNaturalPerson.NameCH_OLD, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtBIRTH_DATE, ConvertToROCYear(eAMLNaturalPerson.BIRTH_DATE), ref blnSame, ref errMsgT);
            string gender = dropGENDER.Items.FindByValue(eAMLNaturalPerson.GENDER.Trim()).Text;//性別中文選項
            string[] tempList2 = { "M", "F", "" };
            if (Array.IndexOf(tempList2, txtGENDER.Text.Trim().ToUpper()) != -1)//有找到tempList2
            {
                string gender2 = dropGENDER.Items.FindByValue(txtGENDER.Text.Trim().ToUpper()).Text;//直接輸入代號
                CompareValueToUpper(gender2, gender, ref blnSame, ref errMsgT);
            }
            else
            {
                CompareValueToUpper(txtGENDER, gender, ref blnSame, ref errMsgT);
            }
            CompareValueToUpper(txtCountryCode, eAMLNaturalPerson.CountryCode, ref blnSame, ref errMsgT);

            if (this.txtCountryCode2.Text.Trim() == "請選擇")
            {
                this.txtCountryCode2.Text = "";
                CompareValueToUpper(txtCountryCode2, eAMLNaturalPerson.CountryCode2.Trim(), ref blnSame, ref errMsgT);
            } 
            else
            {
                CompareValueToUpper(txtCountryCode2, eAMLNaturalPerson.CountryCode2.Trim(), ref blnSame, ref errMsgT);
            }
                

            CompareValueToUpper(txtID_ISSUEDATE, ConvertToROCYear(eAMLNaturalPerson.ID_ISSUEDATE), ref blnSame, ref errMsgT);
            CompareValueToUpper(txtID_ISSUEPLACE, eAMLNaturalPerson.ID_ISSUEPLACE, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtID_REPLACETYPE, eAMLNaturalPerson.ID_REPLACETYPE, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtID_PHOTOFLAG, eAMLNaturalPerson.ID_PHOTOFLAG, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtREG_ZIPCODE, eAMLNaturalPerson.REG_ZIPCODE, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtREG_CITY, eAMLNaturalPerson.REG_CITY, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtREG_ADDR1, eAMLNaturalPerson.REG_ADDR1, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtREG_ADDR2, eAMLNaturalPerson.REG_ADDR2, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtMAILING_CITY, eAMLNaturalPerson.MAILING_CITY, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtMAILING_ADDR1, eAMLNaturalPerson.MAILING_ADDR1, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtMAILING_ADDR2, eAMLNaturalPerson.MAILING_ADDR2, ref blnSame, ref errMsgT);            
            CompareValueToUpper(hidEmailFall.Value, txtEMAIL, eAMLNaturalPerson.EMAIL, ref blnSame, ref errMsgT); // E-Mail
            CompareValueToUpper(txtCOMP_TEL1, eAMLNaturalPerson.COMP_TEL1, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtCOMP_TEL2, eAMLNaturalPerson.COMP_TEL2, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtCOMP_TEL3, eAMLNaturalPerson.COMP_TEL3, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtNP_COMPANY_NAME, eAMLNaturalPerson.NP_COMPANY_NAME, ref blnSame, ref errMsgT);            
            CompareValueToUpper(txtIndustry1, eAMLNaturalPerson.Industry1.Trim(), ref blnSame, ref errMsgT);            
            CompareValueToUpper(txtIndustry2, eAMLNaturalPerson.Industry2.Trim(), ref blnSame, ref errMsgT);            
            CompareValueToUpper(txtIndustry3, eAMLNaturalPerson.Industry3.Trim(), ref blnSame, ref errMsgT);
            CompareValueToUpper(txtCC, eAMLNaturalPerson.CC, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtCC2, eAMLNaturalPerson.CC2, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtCC3, eAMLNaturalPerson.CC3, ref blnSame, ref errMsgT);            
            CompareValueToUpper(txtTITLE, eAMLNaturalPerson.TITLE, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtOC, eAMLNaturalPerson.OC, ref blnSame, ref errMsgT);
            #region 收入來源
            string inComeValue = "000000000";
            for (int i = 1; i < 10; i++)
            {
                string ID = "chkIncome" + i.ToString();
                CheckBox checkBox = this.FindControl(ID) as CheckBox;
                if (checkBox.Checked)
                {
                    inComeValue = inComeValue.Remove(i - 1, 1).Insert(i - 1, "1");
                }
            }
            CompareValueToUpper(inComeValue, eAMLNaturalPerson.INCOME_SOURCE, ref blnSame, ref errMsgT);//收入來源
            #endregion
            CompareValueToUpper(txtOWNER_ID_OLD, eAMLNaturalPerson.OWNER_ID_OLD, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtLAST_UPD_MAKER, eAMLNaturalPerson.LAST_UPD_MAKER, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtLAST_UPD_CHECKER, eAMLNaturalPerson.LAST_UPD_CHECKER, ref blnSame, ref errMsgT);
            CompareValueToUpper(txtLAST_UPD_BRANCH, eAMLNaturalPerson.LAST_UPD_BRANCH, ref blnSame, ref errMsgT);
            string SCDDisChecked = chkisSCDD.Checked ? "Y" : "N";
            CompareValueToUpper_SCDD(SCDDisChecked, eAMLNaturalPerson.isSCDD, ref blnSame, ref errMsgT);
            #endregion

            return blnSame;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 比較輸入欄位值與資料庫欄位值(轉大寫比較)
    /// </summary>
    /// <param name="txtBox">TextBox</param>
    /// <param name="strValue">資料庫欄位值</param>
    /// <param name="blnSame">是否相同</param>
    /// <param name="errMsgT">比較數量</param>
    private void CompareValueToUpper(CustTextBox txtBox, string strValue, ref bool blnSame, ref List<string> errMsgT)
    {
        if (txtBox.Text.Trim().ToUpper() != NullToString(strValue).Trim().ToUpper())
        {
            string tmp = "名稱: {2} 差異值: 1KEY:{0}, 2KEY:{1} \\n";

            string fieldName = compareDC[txtBox.ID.Substring(3)];
            errMsgT.Add(String.Format(tmp, strValue, txtBox.Text.Trim(), fieldName));

            txtBox.BackColor = Color.Red;
            blnSame = false;
        }
        else if (txtBox.ReadOnly == false)
        {
            txtBox.BackColor = Color.Empty;
        }
    }

    private void CompareValueToUpper(string inputValue, CustTextBox txtBox, string strValue, ref bool blnSame, ref List<string> errMsgT)
    {
        if (inputValue.Trim().ToUpper() != NullToString(strValue.ToUpper()).Trim())
        {
            string tmp = "名稱: {2} 差異值: 1KEY:{0}, 2KEY:{1} \\n";

            string fieldName = compareDC[txtBox.ID.Substring(3)];
            errMsgT.Add(String.Format(tmp, strValue, inputValue.Trim(), fieldName));            

            txtBox.BackColor = Color.Red;
            blnSame = false;
        }
        else if (txtBox.ReadOnly == false)
        {            
            txtBox.BackColor = Color.Empty;
        }
    }
    //20210929_Ares_Jack_比對收入來源
    private void CompareValueToUpper(string inputValue, string strValue, ref bool blnSame, ref List<string> errMsgT)
    {
        if (inputValue.Trim().ToUpper() != NullToString(strValue).Trim().ToUpper())
        {
            string tmp = "名稱: {2} 差異值: 1KEY:{0}, 2KEY:{1} \\n";

            string fieldName = compareDC["INCOME_SOURCE"];
            errMsgT.Add(String.Format(tmp, strValue, inputValue.Trim(), fieldName));
            
            blnSame = false;
        }
    }
    //20211025_Ares_Jack_比對SCDD
    private void CompareValueToUpper_SCDD(string inputValue, string strValue, ref bool blnSame, ref List<string> errMsgT)
    {
        if (inputValue.Trim().ToUpper() != NullToString(strValue).Trim().ToUpper())
        {
            string tmp = "名稱: {2} 差異值: 1KEY:{0}, 2KEY:{1} \\n";

            string fieldName = compareDC["isSCDD"];
            errMsgT.Add(String.Format(tmp, strValue, inputValue.Trim(), fieldName));

            blnSame = false;
        }
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

    private void initcompareDC()
    {
        compareDC.Add("OWNER_ID2", "自然人身份ID");
        compareDC.Add("NameCH", "中文姓名");
        compareDC.Add("NameEN", "英文姓名");
        compareDC.Add("NameCH_L", "中文長姓名");
        compareDC.Add("NameCH_Pinyin", "羅馬拼音");
        compareDC.Add("NameCH_OLD", "別名");
        compareDC.Add("BIRTH_DATE", "生日");
        compareDC.Add("GENDER", "性別");
        compareDC.Add("CountryCode", "國籍1");
        compareDC.Add("CountryCode2", "國籍2");        
        compareDC.Add("ID_ISSUEDATE", "發證日期");
        compareDC.Add("ID_ISSUEPLACE", "發證地點");
        compareDC.Add("ID_REPLACETYPE", "領換補類別");
        compareDC.Add("ID_PHOTOFLAG", "有無照片");
        compareDC.Add("REG_ZIPCODE", "登記地址郵遞區號");
        compareDC.Add("REG_CITY", "登記地址城市");
        compareDC.Add("REG_ADDR1", "登記地址第一段");
        compareDC.Add("REG_ADDR2", "登記地址第二段");
        compareDC.Add("MAILING_CITY", "通訊地址城市");
        compareDC.Add("MAILING_ADDR1", "通訊地址第一段");
        compareDC.Add("MAILING_ADDR2", "通訊地址第二段");
        compareDC.Add("EMAIL", "EMAIL");
        compareDC.Add("COMP_TEL1", "公司電話");
        compareDC.Add("NP_COMPANY_NAME", "任職公司名稱");
        compareDC.Add("Industry1", "行業別(大類)1");
        compareDC.Add("Industry2", "行業別(大類)2");
        compareDC.Add("Industry3", "行業別(大類)3");
        compareDC.Add("CC", "行業別編號");
        compareDC.Add("CC2", "行業別編號2");
        compareDC.Add("CC3", "行業別編號3");
        compareDC.Add("TITLE", "職稱");
        compareDC.Add("OC", "職業別");
        compareDC.Add("INCOME_SOURCE", "主要收入及資產來源");
        compareDC.Add("OWNER_ID_OLD", "負責人舊ID");
        compareDC.Add("LAST_UPD_MAKER", "資料最後異動MAKER");
        compareDC.Add("LAST_UPD_CHECKER", "資料最後異動CHECKER");
        compareDC.Add("LAST_UPDATE_BRANCH", "資料最後異動分行");
        compareDC.Add("isSCDD", "是否已完成SCDD");
    }

    private string NullToString(string strValue)
    {
        if (strValue == null)
        {
            return strValue = "";
        }
        return strValue;
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
        using (BRHTG_JC68 obj = new BRHTG_JC68("P010109020001"))
        {
            EntityHTG_JC68 _data = new EntityHTG_JC68();

            _data.ID = strID;
            _result = obj.getData(_data, eAgentInfo, "11");
        }
        return _result;
    }
    #endregion

    /// 作者 Ares Jack
    /// 創建日期：2021/12/14
    /// 修改日期：2021/12/14
    /// <summary>
    /// 收入及資產來源轉中文名稱
    /// </summary>
    /// <param name="incomeSource">收入及資產來源</param>
    private string IncomeSourceChName(string incomeSource)
    {
        if (incomeSource.Trim() != "")
        {
            for (int i = 1; i < 10; i++)
            {
                if (incomeSource.IndexOf('1', i - 1, 1) != -1)//被勾選的是1
                {
                    incomeSource = incomeSource.Remove(i - 1, 1).Insert(i - 1, i.ToString());
                }
            }
            incomeSource = string.Join(",", incomeSource.Replace("0", "").ToCharArray());//清除0,剩餘的用逗號分開 ex:1,2,3

            string[] incomeSourceAll = incomeSource.Split(',');
            string htReturnResult = string.Empty;
            List<string> temp = new List<string>();
            if (Array.IndexOf(incomeSourceAll, "1") != -1)
                temp.Add(BaseHelper.GetShowText("01_01090600_035"));//薪資
            if (Array.IndexOf(incomeSourceAll, "2") != -1)
                temp.Add(BaseHelper.GetShowText("01_01090600_036"));//經營事業收入
            if (Array.IndexOf(incomeSourceAll, "3") != -1)
                temp.Add(BaseHelper.GetShowText("01_01090600_037"));//退休(職)資金
            if (Array.IndexOf(incomeSourceAll, "4") != -1)
                temp.Add(BaseHelper.GetShowText("01_01090600_038"));//遺產繼承(含贈與)
            if (Array.IndexOf(incomeSourceAll, "5") != -1)
                temp.Add(BaseHelper.GetShowText("01_01090600_039"));//買賣房地產
            if (Array.IndexOf(incomeSourceAll, "6") != -1)
                temp.Add(BaseHelper.GetShowText("01_01090600_040"));//投資理財
            if (Array.IndexOf(incomeSourceAll, "7") != -1)
                temp.Add(BaseHelper.GetShowText("01_01090600_041"));//租金收入
            if (Array.IndexOf(incomeSourceAll, "8") != -1)
                temp.Add(BaseHelper.GetShowText("01_01090600_042"));//存款
            if (Array.IndexOf(incomeSourceAll, "9") != -1)
                temp.Add(BaseHelper.GetShowText("01_01090600_043"));//其他

            htReturnResult = string.Join("、", temp);

            return htReturnResult;
        }
        else
        {
            return "";
        }
    }
}
