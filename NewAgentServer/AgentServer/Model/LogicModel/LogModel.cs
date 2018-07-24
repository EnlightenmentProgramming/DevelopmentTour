using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.LogicModel
{
    public class LogModel
    {
        private int? _pagesize;
        private int? _curepage;
        public string L_OperatorID { get; set; }
        public string L_Operator { get; set; }
        public int? PageSize
        {
            get { return _pagesize; }
            set
            {
                if(value != null && value <0)
                {
                    _pagesize = 20;
                }
                else
                {
                    _pagesize = value;
                }
            }
        }
        public int? CurePage
        {
            get { return _curepage; }
            set
            {
                if (value != null && value < 0)
                {
                    _curepage = 1;
                }
                else
                {
                    _curepage = value;
                }
            }
        }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string L_SourceUser { get; set; }
        public string L_User { get; set; }
        public string L_PAgent { get; set; }
    }
}
