using System.Linq;

namespace SmartFleet.Web.Framework.DataTables
{
    public class DataTablesResultModel<T>
    {
        /// <summary>
        /// Gets or sets the draw.
        /// </summary>
        /// <value>
        /// The draw.
        /// </value>
        public int draw { get; set; }
        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        public int start { get; set; }
        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int length { get; set; }
        /// <summary>
        /// let the data Iqueryable in order to filter it again 
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public IQueryable<T> data { get; set; }
        /// <summary>
        /// Gets or sets the records total.
        /// </summary>
        /// <value>
        /// The records total.
        /// </value>
        public int recordsTotal { get; set; }

        /// <summary>
        /// Gets or sets the records filtered.
        /// </summary>
        /// <value>
        /// The records filtered.
        /// </value>
        public int recordsFiltered { get; set; }
    }
}