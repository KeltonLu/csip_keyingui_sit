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
using Framework.Data.OM.Collections;
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using CSIPCommonModel.EntityLayer;
using System.Drawing;
using Framework.Common.Message;
using Framework.Common.IO;

public partial class P010105020201 : PageBase
{
    #region Variable
    private EntityAGENT_INFO eAgentInfo;
    #endregion Variable

    #region event

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            isEnableAll(false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
        }
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
    }

    protected void btnSelect_Click(object sender, EventArgs e)
    {
        if (!txtReceiveNumber.Enabled)
        {
            isEnableAll(false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            return;
        }

        EntitySet<EntityRedeemUpload> esRU1Key = new EntitySet<EntityRedeemUpload>();
        esRU1Key = BRRedeemUpload.SelectData(txtReceiveNumber.Text.Trim(), "1");

        // 查詢一KEY資料
        if (!(esRU1Key.Count > 0))
        {
            //*沒有一Key資料
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040202_013") + "');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            isEnableAll(false);
            return;
        }

        if ("Y" == esRU1Key.GetEntity(0).SEND3270.ToString().ToUpper())
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040202_001") + "');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            isEnableAll(false);
            return;
        }

        //*比較一Key和二Key的User是否為同一人
        if (esRU1Key.GetEntity(0).USER_ID.ToString().Trim() == eAgentInfo.agent_id.ToString().Trim())
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040202_014") + "');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            isEnableAll(false);
            return;
        }


        EntitySet<EntityRedeemUpload> esRU2Key = new EntitySet<EntityRedeemUpload>();
        esRU2Key = BRRedeemUpload.SelectData(txtReceiveNumber.Text.Trim(), "2");

        if (esRU2Key.Count > 0)
        {
            if ("N" == esRU2Key.GetEntity(0).SEND3270)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01050100_003");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                isEnableAll(false);
                return;
            }
        }

        isEnableAll(true);
        cFUpload.Focus();

        ViewState["esEntityRedeemUpload"] = esRU1Key;
    }

    protected void btnShowData_Click(object sender, EventArgs e)
    {
        ShowData();
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        int iDiffCoutn = 0;

        EntitySet<EntityRedeemUpload> esRU2Key = new EntitySet<EntityRedeemUpload>();
        esRU2Key = BRRedeemUpload.SelectData(txtReceiveNumber.Text.Trim(), "2");

        if (Compare(ref iDiffCoutn))
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01040202_004");

            if (esRU2Key.Count > 0)
            {
                if (BRRedeemUpload.UpdateKey2(txtReceiveNumber.Text.Trim(), CreateES("N", iDiffCoutn)))
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01050100_004");
                    isEnableAll(false);
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                }
                else
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01050100_005");
                }
            }
            else
            {
                if (BRRedeemUpload.AddBatch(CreateES("N", iDiffCoutn)))
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01050100_004");
                    isEnableAll(false);
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                }
                else
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01050100_005");
                }
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01040202_003");

            if (esRU2Key.Count > 0)
            {
                BRRedeemUpload.UpdateKey2(txtReceiveNumber.Text.Trim(), CreateES("", iDiffCoutn));
            }
            else
            {
                BRRedeemUpload.AddBatch(CreateES("", iDiffCoutn));
            }
        }
    }

    protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[0].Text = BaseHelper.GetShowText("01_01050201_005");
            e.Row.Cells[1].Text = BaseHelper.GetShowText("01_01050201_006");
        }

    }
    #endregion event


    #region method

    private void isEnableAll(bool bIsEnable)
    {
        if (bIsEnable)
        {
            ViewState["lStartTime"] = System.Environment.TickCount;  // 開始 keyin 時間
        }

        cFUpload.Enabled = bIsEnable;
        btnShowData.Enabled = bIsEnable;

        txtReceiveNumber.Enabled = !bIsEnable;


        if (!bIsEnable)
        {
            pnList.Visible = bIsEnable;
        }
    }

    private void ShowData()
    {
        if (this.cFUpload.PostedFile.FileName.Trim() == "")
        {
            strAlertMsg = MessageHelper.GetMessage("01_01050200_001");
            return;
        }

        if (cFUpload.PostedFile.ContentLength == 0)
        {
            strAlertMsg = MessageHelper.GetMessage("01_01050200_002");
            return;
        }

        string[] arryFile = FileTools.Read(cFUpload.PostedFile.FileName.Trim());
        string strT = "";   // DataType
        string strD = "";   // 內容
        string strNowT = "";    // 當前列的 DataType

        DataTable dtList = new DataTable();

        dtList.Columns.Add(new DataColumn("DATA_TYPE", typeof(string)));
        dtList.Columns.Add(new DataColumn("UPLOAD_DATA", typeof(string)));

        pnList.Visible = false;

        for (int i = 0; i < arryFile.Length; i++)
        {
            DataRow dr = dtList.NewRow();

            if (!strSplit(arryFile[i], ref strT, ref strD))
            {
                return;
            }

            // 第一列
            if (0 == i)
            {
                if ("S" != strT.ToUpper())
                {
                    strAlertMsg = MessageHelper.GetMessage("01_01050200_003");
                    return;
                }

                if (strD.Length > 9)
                {
                    strAlertMsg = MessageHelper.GetMessage("01_01050200_004");
                    return;
                }
            }

            // 中間內容列
            if (0 != i && i != arryFile.Length - 1)
            {
                if ("T" != strT && "M" != strT)
                {
                    strAlertMsg = MessageHelper.GetMessage("01_01050200_005");
                    return;
                }

                if ("" == strNowT)
                {
                    if ("M" == strT)
                    {
                        strAlertMsg = MessageHelper.GetMessage("01_01050200_006");
                        return;
                    }

                    if (strD.Length > 5)
                    {
                        strAlertMsg = MessageHelper.GetMessage("01_01050200_007");
                        return;
                    }

                    strNowT = "T";
                }
                else if ("T" == strNowT)
                {
                    if ("T" == strT)
                    {
                        if (strD.Length > 5)
                        {
                            strAlertMsg = MessageHelper.GetMessage("01_01050200_007");
                            return;
                        }
                    }
                    else
                    {
                        if (strD.Length > 9)
                        {
                            strAlertMsg = MessageHelper.GetMessage("01_01050200_008");
                            return;
                        }
                        strNowT = "M";
                    }
                }
                else
                {
                    if ("T" == strT)
                    {
                        strAlertMsg = MessageHelper.GetMessage("01_01050200_006");
                        return;
                    }

                    if (strD.Length > 9)
                    {
                        strAlertMsg = MessageHelper.GetMessage("01_01050200_008");
                        return;
                    }
                }

            }

            // 結束列
            if (i == arryFile.Length - 1)
            {
                if ("E" != arryFile[arryFile.Length - 1].Trim().ToUpper())
                {
                    strAlertMsg = MessageHelper.GetMessage("01_01050200_009");
                    return;
                }
            }

            dr["DATA_TYPE"] = strT;
            dr["UPLOAD_DATA"] = strD;
            dtList.Rows.Add(dr);
        }

        pnList.Visible = true;
        gvList.DataSource = dtList;
        gvList.DataBind();
    }

    private bool strSplit(string str, ref string strT, ref string strD)
    {
        str = str.Trim().Trim('\t');

        str = str + "\t";

        strT = str.Split('\t')[0].Trim();
        strD = str.Split('\t')[1].Trim();

        if ("" == strD)
        {
            if (strT != "E")
            {
                strAlertMsg = strT + MessageHelper.GetMessage("01_01050200_010");
                return false;
            }
        }
        else
        {
            if ("E" == strT)
            {
                strAlertMsg = MessageHelper.GetMessage("01_01050200_011");
                return false;
            }
        }

        return true;
    }

    private bool Compare(ref int intDiffCount)
    {
        bool bReturn = true;
        int intCount = 0;//*記錄不相同的數量

        EntitySet<EntityRedeemUpload> esRU = new EntitySet<EntityRedeemUpload>();
        esRU = (EntitySet<EntityRedeemUpload>)ViewState["esEntityRedeemUpload"];

        for (int i = 0; i < esRU.Count; i++)
        {
            EntityRedeemUpload eRU = esRU.GetEntity(i);

            if (eRU.DATA_TYPE.Trim() != gvList.Rows[eRU.LINE_INDEX].Cells[0].Text.Trim().Replace("&nbsp;", ""))
            {
                gvList.Rows[eRU.LINE_INDEX].Cells[0].ForeColor = Color.Red;
                bReturn = false;
                intCount++;
            }
            else
            {
                gvList.Rows[eRU.LINE_INDEX].Cells[0].ForeColor = Color.Black;
            }

            if (eRU.UPLOAD_DATA.Trim() != gvList.Rows[eRU.LINE_INDEX].Cells[1].Text.Trim().Replace("&nbsp;", ""))
            {
                gvList.Rows[eRU.LINE_INDEX].Cells[1].ForeColor = Color.Red;
                bReturn = false;
                intCount++;
            }
            else
            {
                gvList.Rows[eRU.LINE_INDEX].Cells[1].ForeColor = Color.Black;
            }
        }

        if (gvList.Rows.Count > esRU.Count)
        {
            for (int i = esRU.Count; i < gvList.Rows.Count; i++)
            {
                gvList.Rows[i].Cells[0].ForeColor = Color.Red;
                gvList.Rows[i].Cells[1].ForeColor = Color.Red;
                bReturn = false;
                intCount = intCount + 2;
            }
        }

        intDiffCount = intCount;
        return bReturn;
    }

    private EntitySet<EntityRedeemUpload> CreateES(string strSend3270, int iDiffCount)
    {
        EntitySet<EntityRedeemUpload> esRU = new EntitySet<EntityRedeemUpload>();

        for (int i = 0; i < gvList.Rows.Count; i++)
        {
            EntityRedeemUpload eRU = new EntityRedeemUpload();

            eRU.RECEIVE_NUMBER = txtReceiveNumber.Text.Trim().ToUpper();
            eRU.USER_ID = ((EntityAGENT_INFO)Session["Agent"]).agent_id;
            eRU.DATA_TYPE = gvList.Rows[i].Cells[0].Text.Trim().ToUpper().Replace("&nbsp;", "");
            eRU.UPLOAD_DATA = gvList.Rows[i].Cells[1].Text.Trim().Replace("&nbsp;", "");
            eRU.LINE_INDEX = i;
            eRU.KEYIN_DATE = DateTime.Now.ToString("yyyyMMdd");
            eRU.UPDATE_DATE = DateTime.Now.ToString("yyyyMMdd");
            eRU.KEYIN_FLAG = "2";
            eRU.SEND3270 = strSend3270;
            eRU.USE_TIME = (System.Environment.TickCount - (int)ViewState["lStartTime"]) / 1000;
            eRU.ISSAME = 0 == iDiffCount ? "Y" : "N";
            eRU.DIFF_NUM = iDiffCount;
            eRU.EDIT_USER_ID = ((EntityAGENT_INFO)Session["Agent"]).agent_id;

            esRU.Add(eRU);
        }

        return esRU;
    }
    #endregion method


}
