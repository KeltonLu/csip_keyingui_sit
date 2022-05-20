using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Data.OM.OMAttribute
{
    public interface IMutiLineClass
    {
        /// <summary>
        /// 設定Class層級為多LINE 屬性唯讀
        /// </summary>
        bool isMutilime { get; }

        /// <summary>
        /// 設計階段對應至Class中LineID欄位，屬性唯讀
        /// </summary>
        string InterFaceLineID { get; }
    }
}
