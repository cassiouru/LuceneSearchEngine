using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucene.Net.Models
{
    public class Book
    {
        public string Isbn { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public int PageNum { get; set; }
    }
}
