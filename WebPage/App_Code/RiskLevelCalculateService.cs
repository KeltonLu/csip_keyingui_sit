//******************************************************************
//*  作    者：Ares Jack
//*  功能說明：計算風險等級
//*  創建日期：2022/03/09
//*  修改紀錄：
//*  <author>            <time>            <TaskID>                <desc>
//*******************************************************************

using System;
using System.Data;
using CSIPCommonModel.BusinessRules;
using System.Text;
using System.IO;
using Framework.Common.Logging;
using Framework.Common.Message;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using CSIPNewInvoice.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules_new;

public class RiskLevelCalculateService
{
    private List<int> iRiskLevel = new List<int>();
    //高風險國家 Type = 12
    DataTable dtRiskCountry = BRPostOffice_CodeType.GetCodeType("12");
    //高度制裁國家 Type = 13
    DataTable dtSanctionCountry = BRPostOffice_CodeType.GetCodeType("13");
    //一般制裁國家 Type = 15
    DataTable dtGeneralSanctionCountry = BRPostOffice_CodeType.GetCodeType("15");

    public RiskLevelCalculateService()
    {
        
    }

    /// <summary>
    /// 法人人計算風險等級
    /// </summary>
    /// <param name="sessionOBJ"></param>
    public int RiskLevelCalculate(AML_SessionState sessionOBJ)
    {
        try
        {
            EntityAML_HQ_Work hqObj = BRAML_HQ_Work.getHQ_WOrk(sessionOBJ);
            EntityHQ_SCDD sccdObj = BRHQ_SCDD.getSCDDData_WOrk(sessionOBJ);
            EntityAML_Cdata_Work cDobj = BRAML_Cdata_Work.getCData_WOrk(sessionOBJ);
            List<EntityAML_BRCH_Work> listBRCH_Work = BRAML_BRCH_Work.getBRCH_WOrkColl(sessionOBJ);
            List<EntityAML_HQ_Manager_Work> listHQ_Manager = BRAML_HQ_Manager_Work.getHQMA_WorkColl(sessionOBJ);
            List<Personnel_Manager> listPersonnel_Manager = new List<Personnel_Manager>();

            iRiskLevel.Add(1);

            if (hqObj != null)
            {
                //20200115-RQ-2019-030155-002 -商店註冊國籍或負責人/高管/實質受益人的國籍位於一般制裁國家/地區->高風險
                if (isGeneralSanctionCountry(hqObj.HCOP_REGISTER_NATION))
                    iRiskLevel.Add(3);
                //20200115-RQ-2019-030155-002 -負責人/高管/實質受益人的國籍為本行所列高度或一般制裁國家->高風險
                if (isSanctionCountry(hqObj.HCOP_OWNER_NATION))
                    iRiskLevel.Add(3);
                if (isGeneralSanctionCountry(hqObj.HCOP_OWNER_NATION))
                    iRiskLevel.Add(3);
                //高風險行業別 Type = 11
                DataTable dtHCOP_CC_RiskLevel = BRPostOffice_CodeType.GetCodeTypeByCodeID("11", hqObj.HCOP_CC);
                //客戶行業為高風險行業->高風險
                if (dtHCOP_CC_RiskLevel != null && dtHCOP_CC_RiskLevel.Rows.Count > 0)
                    iRiskLevel.Add(3);

                // 20220426 移除 位於高風險國家地區->中風險 By Kelton
                ////客戶註冊國籍/通訊地國家/永久地國家/主要營業處所國家/高風險營業處所/僑外資外商母公司國別 位於高風險國家地區->中風險 
                ////通訊地國家/永久地國家??
                //if (dtRiskCountry != null && dtRiskCountry.Rows.Count > 0)
                //{
                //    //註冊國籍	AML_HQ_Work.HCOP_REGISTER_NATION
                //    if (!String.IsNullOrEmpty(hqObj.HCOP_REGISTER_NATION))
                //    {
                //        if (dtRiskCountry.Select("CODE_ID = '" + hqObj.HCOP_REGISTER_NATION + "'").Length > 0)
                //            iRiskLevel.Add(2);
                //    }
                //    //主要營業處所國別	AML_HQ_Work.HCOP_PRIMARY_BUSI_COUNTRY
                //    if (!String.IsNullOrEmpty(hqObj.HCOP_PRIMARY_BUSI_COUNTRY))
                //    {
                //        if (dtRiskCountry.Select("CODE_ID = '" + hqObj.HCOP_PRIMARY_BUSI_COUNTRY + "'").Length > 0)
                //            iRiskLevel.Add(2);
                //    }
                //    //僑外資/外商母公司國別	AML_HQ_Work.HCOP_OVERSEAS_FOREIGN_COUNTRY
                //    if (!String.IsNullOrEmpty(hqObj.HCOP_OVERSEAS_FOREIGN_COUNTRY))
                //    {
                //        if (dtRiskCountry.Select("CODE_ID = '" + hqObj.HCOP_OVERSEAS_FOREIGN_COUNTRY + "'").Length > 0)
                //            iRiskLevel.Add(2);
                //    }
                //}


                //客戶為複雜股權結構或無記名股份有限公司(無記名=Y)->高風險
                if (hqObj.HCOP_COMPLEX_STR_CODE == "Y" || hqObj.HCOP_COMPLEX_STR_CODE == "1")
                    iRiskLevel.Add(3);
                if (hqObj.HCOP_ALLOW_ISSUE_STOCK_FLAG == "Y" || hqObj.HCOP_ALLOW_ISSUE_STOCK_FLAG == "1")
                    iRiskLevel.Add(3);
            }

            if (sccdObj != null)
            {
                //(Name Check)客戶為國外PEP /國外PEP利益關聯人/NN /制裁名單->高風險  
                //(Name Check)國內PEP/國際組織PEP/國內PEP利益關聯人/國際組織PEP利益關聯人->中風險
                // 20211203  依據USER需求調整風險等級  (Name Check)國內PEP/國際組織PEP/國內PEP利益關聯人/國際組織PEP利益關聯人->高風險   Edit by Ares Rick
                if (sccdObj.NameCheck_Item == "2")
                    iRiskLevel.Add(3);
                if (sccdObj.NameCheck_Item == "3")
                    iRiskLevel.Add(3);

                // 20220426 移除 營業處所是否位於高風險國家地區->中風險 By Kelton
                ////營業處所是否在高風險或制裁國家
                //if (dtRiskCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode1 + "'").Length > 0)
                //    iRiskLevel.Add(2);
                //if (dtRiskCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode2 + "'").Length > 0)
                //    iRiskLevel.Add(2);
                //if (dtRiskCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode3 + "'").Length > 0)
                //    iRiskLevel.Add(2);
                //if (dtRiskCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode4 + "'").Length > 0)
                //    iRiskLevel.Add(2);
                //if (dtRiskCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode5 + "'").Length > 0)
                //    iRiskLevel.Add(2);
            }

            if (cDobj != null)
            {
                //(AML C檔)被申報疑似洗錢/信用卡不良註記/AML不合作註記/警示帳戶->高風險
                if (cDobj.GroupInformationSharingNameListflag == "Y")
                    iRiskLevel.Add(3);
                //20191119-RQ-2018-015749-002-因法遵已公告不合作不列入高風險因子，故調整SCDD計算風險等級邏輯。
                //if (cDobj.Incorporated == "Y")//不合作
                //    iRiskLevel.Add(3);
                //警示帳戶
                if (cDobj.WarningFlag == "Y")
                    iRiskLevel.Add(3);
            }

            if (listBRCH_Work != null && listHQ_Manager != null)
            {
                for (int i = 0; i < listBRCH_Work.Count; i++)
                {
                    Personnel_Manager loopObj = new Personnel_Manager();
                    loopObj.Name = listBRCH_Work[i].BRCH_CHINESE_NAME;
                    loopObj.Country = listBRCH_Work[i].BRCH_NATION;
                    loopObj.Type = "BRCH";
                    loopObj.ID_SreachStatus = listBRCH_Work[i].BRCH_ID_SreachStatus;
                    //增加長姓名及羅馬
                    loopObj.Lname = listBRCH_Work[i].BRCH_CHINESE_LNAME;
                    loopObj.Romaname = listBRCH_Work[i].BRCH_ROMA;
                    listPersonnel_Manager.Add(loopObj);
                }

                for (int i = 0; i < listHQ_Manager.Count; i++)
                {
                    if (!String.IsNullOrEmpty(listHQ_Manager[i].HCOP_BENE_NAME) && !String.IsNullOrEmpty(listHQ_Manager[i].HCOP_BENE_NATION))
                    {
                        Personnel_Manager loopObj = new Personnel_Manager();
                        loopObj.Name = listHQ_Manager[i].HCOP_BENE_NAME;
                        loopObj.Country = listHQ_Manager[i].HCOP_BENE_NATION;
                        loopObj.Type = "Manager";
                        loopObj.ID_SreachStatus = String.Empty;
                        //增加長姓名及羅馬
                        loopObj.Lname = listHQ_Manager[i].HCOP_BENE_LNAME;
                        loopObj.Romaname = listHQ_Manager[i].HCOP_BENE_ROMA;
                        listPersonnel_Manager.Add(loopObj);
                    }
                }

                for (int i = 0; i < listPersonnel_Manager.Count; i++)
                {
                    //20200115-RQ-2019-030155-002 -負責人/高管/實質受益人的國籍為本行所列高度或一般制裁國家->高風險                         
                    if (isGeneralSanctionCountry(listPersonnel_Manager[i].Country))
                    {
                        iRiskLevel.Add(3);
                    }
                    //客戶其關聯人為自然人且國籍為本行所列高度制裁國家->高風險，加提示請依指引『風險管控』章節所列核准層級授權
                    if (dtSanctionCountry.Select("CODE_ID = '" + listPersonnel_Manager[i].Country + "'").Length > 0)
                    {
                        iRiskLevel.Add(3);
                    }
                }
            }

            //進行降冪排序後，取第一個最大值進行判斷
            iRiskLevel.Sort();
            iRiskLevel.Reverse();

            return iRiskLevel[0];
        }
        catch (Exception ex)
        {
            Logging.Log("法人計算風險等級 :" + ex);
            return 1;
        }
    }

    /// <summary>
    /// 自然人計算風險等級
    /// </summary>
    /// <param name="sessionOBJ"></param>
    public int NaturalPersonRiskLevelCalculate(AML_SessionState sessionOBJ)
    {
        try
        {
            EntityAML_HQ_Work hqObj = BRAML_HQ_Work.getHQ_WOrk(sessionOBJ);
            EntityHQ_SCDD sccdObj = BRHQ_SCDD.getSCDDData_WOrk(sessionOBJ);
            EntityAML_Cdata_Work cDobj = BRAML_Cdata_Work.getCData_WOrk(sessionOBJ);
            List<EntityAML_BRCH_Work> listBRCH_Work = BRAML_BRCH_Work.getBRCH_WOrkColl(sessionOBJ);
            List<Personnel_Manager> listPersonnel_Manager = new List<Personnel_Manager>();

            iRiskLevel.Add(1);

            if (hqObj != null)
            {
                // 20220426 移除 位於位於高風險國家/地區 SCDD風險試算調整為中風險 By Kelton
                ////客戶 國籍、戶籍地 或 通訊地 或關聯人任一人 國籍 位於位於高風險國家/地區 //20220323_Ares_Jack_自然人GV版本-SCDD風險試算調整為中風險
                //if (dtRiskCountry.Select("CODE_ID = '" + hqObj.HCOP_REGISTER_NATION + "'").Length > 0)
                //{
                //    iRiskLevel.Add(2);
                //}

                //客戶 國籍、戶籍地 或 通訊地 位於一般或高度制裁國家/ 地區
                if (dtGeneralSanctionCountry.Select("CODE_ID = '" + hqObj.HCOP_REGISTER_NATION + "'").Length > 0 ||
                    dtSanctionCountry.Select("CODE_ID = '" + hqObj.HCOP_REGISTER_NATION + "'").Length > 0)
                {
                    iRiskLevel.Add(3);
                }

                // 20220426 移除 位於位於高風險國家/地區 SCDD風險試算調整為中風險 By Kelton
                ////客戶 國籍、戶籍地 或 通訊地 或關聯人任一人 國籍 位於位於高風險國家/地區 //20220323_Ares_Jack_自然人GV版本-SCDD風險試算調整為中風險
                //if (dtRiskCountry.Select("CODE_ID = '" + hqObj.HCOP_COUNTRY_CODE_2 + "'").Length > 0)
                //{
                //    iRiskLevel.Add(2);
                //}

                //客戶 國籍、戶籍地 或 通訊地 位於一般或高度制裁國家/ 地區
                if (dtGeneralSanctionCountry.Select("CODE_ID = '" + hqObj.HCOP_COUNTRY_CODE_2 + "'").Length > 0 ||
                    dtSanctionCountry.Select("CODE_ID = '" + hqObj.HCOP_COUNTRY_CODE_2 + "'").Length > 0)
                {
                    iRiskLevel.Add(3);
                }

                #region 高風險行職業組合
                DataTable dtHCOP_CC_OC_RiskLevel = BRPostOffice_CodeType.GetCodeTypeByCodeID("21", hqObj.HCOP_CC + "_" + hqObj.HCOP_OC);//高風險行職業組合
                DataTable dtHCOP_CC_OC_2_RiskLevel = BRPostOffice_CodeType.GetCodeTypeByCodeID("21", hqObj.HCOP_CC_2 + "_" + hqObj.HCOP_OC);//高風險行職業組合
                DataTable dtHCOP_CC_OC_3_RiskLevel = BRPostOffice_CodeType.GetCodeTypeByCodeID("21", hqObj.HCOP_CC_3 + "_" + hqObj.HCOP_OC);//高風險行職業組合
                if (dtHCOP_CC_OC_RiskLevel != null && dtHCOP_CC_OC_RiskLevel.Rows.Count > 0 )
                    iRiskLevel.Add(3);
                
                if (dtHCOP_CC_OC_2_RiskLevel != null && dtHCOP_CC_OC_2_RiskLevel.Rows.Count > 0 )
                    iRiskLevel.Add(3);
                
                if (dtHCOP_CC_OC_3_RiskLevel != null && dtHCOP_CC_OC_3_RiskLevel.Rows.Count > 0 )
                    iRiskLevel.Add(3);

                #endregion
            }

            if (sccdObj != null)
            {
                //(Name Check)客戶為國外PEP /國外PEP利益關聯人/NN /制裁名單->高風險  
                //(Name Check)國內PEP/國際組織PEP/國內PEP利益關聯人/國際組織PEP利益關聯人->中風險 //20211022_Ares_Jack_調整為高風險
                if (sccdObj.NameCheck_Item == "2")
                    iRiskLevel.Add(3);
                if (sccdObj.NameCheck_Item == "3")
                    iRiskLevel.Add(3);
            }

            if (cDobj != null)
            {
                //(AML C檔)被申報疑似洗錢/信用卡不良註記/AML不合作註記/警示帳戶->高風險
                if (cDobj.GroupInformationSharingNameListflag == "Y")
                    iRiskLevel.Add(3);
                if (cDobj.WarningFlag == "Y")//警示帳戶
                    iRiskLevel.Add(3);
                if (cDobj.CreditCardBlockCode != "0000000000")//信用卡不良註記
                    iRiskLevel.Add(3);
                if (cDobj.NNListHitFlag == "Y")//負面新聞
                    iRiskLevel.Add(3);
            }

            if (listBRCH_Work != null)
            {
                for (int i = 0; i < listBRCH_Work.Count; i++)
                {
                    Personnel_Manager loopObj = new Personnel_Manager();
                    loopObj.Name = listBRCH_Work[i].BRCH_CHINESE_NAME;
                    loopObj.Country = listBRCH_Work[i].BRCH_NATION;
                    loopObj.Type = "BRCH";
                    loopObj.ID_SreachStatus = listBRCH_Work[i].BRCH_ID_SreachStatus;
                    //增加長姓名及羅馬
                    loopObj.Lname = listBRCH_Work[i].BRCH_CHINESE_LNAME;
                    loopObj.Romaname = listBRCH_Work[i].BRCH_ROMA;
                    listPersonnel_Manager.Add(loopObj);

                }

                for (int i = 0; i < listPersonnel_Manager.Count; i++)
                {
                    //負責人/高管/實質受益人的國籍為本行所列高度或一般制裁國家->高風險
                    if (isGeneralSanctionCountry(listPersonnel_Manager[i].Country))
                    {
                        iRiskLevel.Add(3);
                    }

                    //客戶其關聯人為自然人且國籍為本行所列高度制裁國家->高風險，加提示請依指引『風險管控』章節所列核准層級授權
                    if (dtSanctionCountry.Select("CODE_ID = '" + listPersonnel_Manager[i].Country + "'").Length > 0)
                    {
                        iRiskLevel.Add(3);
                    }
                }
            }

            //進行降冪排序後，取第一個最大值進行判斷
            iRiskLevel.Sort();
            iRiskLevel.Reverse();

            return iRiskLevel[0];
        }
        catch (Exception ex)
        {
            Logging.Log("自然人計算風險等級 :" + ex);
            return 1;
        }
    }

    //判斷是否為一般制裁國家
    private bool isGeneralSanctionCountry(string _Nation)
    {
        bool isExist = false;

        if (dtGeneralSanctionCountry != null && dtGeneralSanctionCountry.Rows.Count > 0)
        {
            if (!String.IsNullOrEmpty(_Nation))
            {
                if (dtGeneralSanctionCountry.Select("CODE_ID = '" + _Nation + "'").Length > 0)
                {
                    isExist = true;
                }
            }
        }

        return isExist;
    }
    //判斷是否為高度制裁國家
    private bool isSanctionCountry(string _Nation)
    {
        bool isExist = false;

        if (dtSanctionCountry != null && dtSanctionCountry.Rows.Count > 0)
        {
            if (!String.IsNullOrEmpty(_Nation))
            {
                if (dtSanctionCountry.Select("CODE_ID = '" + _Nation + "'").Length > 0)
                {
                    isExist = true;
                }
            }
        }

        return isExist;
    }

    private class Personnel_Manager
    {
        private string _Country;
        public string Country
        {
            get
            {
                return this._Country;
            }
            set
            {
                this._Country = value;
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }

        private string _Type;
        public string Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
            }
        }

        private string _ID_SreachStatus;
        public string ID_SreachStatus
        {
            get
            {
                return this._ID_SreachStatus;
            }
            set
            {
                this._ID_SreachStatus = value;
            }
        }
        //長姓名
        private string _Lname;
        public string Lname
        {
            get
            {
                return this._Lname;
            }
            set
            {
                this._Lname = value;
            }
        }
        //長ROMA
        private string _Romaname;
        public string Romaname
        {
            get
            {
                return this._Romaname;
            }
            set
            {
                this._Romaname = value;
            }
        }
    }

}