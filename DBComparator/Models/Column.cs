﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBComparator.Models
{
    public class Column
    {
        public Column()
            : this("", "", false)
        {
        }

        public Column(string dbname, string colname, bool exist)
        {
            this.dbname = dbname;
            this.colname = colname;
            this.exist = exist;
            this.propeties = new List<ColumnPropetie>();
        }
        public string dbname { get; set; }
        public string colname { get; set; }
        public bool exist { get; set; }
        public List<ColumnPropetie> propeties { get; set; }
        public bool Compare(Column column)
        {
            if (this.colname != column.colname) return false;
            if (this.exist != column.exist) return false;
            if (this.propeties.Count() != column.propeties.Count()) return false;
            this.propeties.Sort((a, b) => a.name.CompareTo(b.name));
            column.propeties.Sort((a, b) => a.name.CompareTo(b.name));
            for (int i = 0; i < this.propeties.Count(); i++)
            {
                if (!this.propeties[i].Compare(column.propeties[i])) return false;
            }
            return true;
        }
    }
}