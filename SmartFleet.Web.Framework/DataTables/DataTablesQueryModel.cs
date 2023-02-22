using System.Collections.Generic;

namespace SmartFleet.Web.Framework.DataTables
{
    public class DataTablesQueryModel
    {
       
            /// <summary>
            /// Gets or sets the draw.
            /// </summary>
            /// <value>
            /// The draw.
            /// </value>
            public int Draw { get; set; }
            /// <summary>
            /// Gets or sets the start.
            /// </summary>
            /// <value>
            /// The start.
            /// </value>
            public int Start { get; set; }
            /// <summary>
            /// Gets or sets the length.
            /// </summary>
            /// <value>
            /// The length.
            /// </value>
            public int Length { get; set; }
            /// <summary>
            /// Gets or sets the search.
            /// </summary>
            /// <value>
            /// The search.
            /// </value>
            public DataTablesSearchModel Search { get; set; }
            /// <summary>
            /// Gets or sets the orders.
            /// </summary>
            /// <value>
            /// The orders.
            /// </value>
            public IEnumerable<DataTablesOrderModel> Orders { get; set; }
            /// <summary>
            /// Gets or sets the columns.
            /// </summary>
            /// <value>
            /// The columns.
            /// </value>
            public List<DataTablesColumnModel> Columns { get; set; }

        
    }
}