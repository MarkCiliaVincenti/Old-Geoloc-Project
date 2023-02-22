using System.Collections.Generic;
using System.Linq;

namespace SmartFleet.Customer.Domain.Common.Dtos
{
    public class DataTablesModel <T>
    {
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
        public IEnumerable<T> data { get; set; }
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

        public int lenght { get; set; }
    }
}
