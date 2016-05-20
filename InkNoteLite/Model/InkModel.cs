using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkNoteLite.Model
{
    public class InkModel
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        [NotNull]
        public string Title { get; set; }
        [NotNull]
        public byte[] Content { get; set; }
        [NotNull]
        public DateTime SubTime { get; set; }
        [NotNull]
        public DateTime ModifiedOn { get; set; }
    }
}
