using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Models
{
    public class ChildReferenceList : List<IChildReference>, IChildReferenceList
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select CASE WHEN sum(cnt) IS NULL THEN 0 ELSE sum(cnt) END count FROM(");
            string union = string.Empty;
            foreach (IChildReference child in this)
            {
                sb.AppendLine(union);
                sb.AppendLine(child.ToString());

                union = " union ";
            }
            sb.AppendLine(") tab");
            //return base.ToString();
            return sb.ToString();
        }
    }
}
