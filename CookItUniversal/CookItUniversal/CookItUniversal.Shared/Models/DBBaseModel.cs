using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CookItUniversal.Models
{
    public abstract class DBBaseModel
    {
        [PrimaryKey]
        public string Id { get; set; }
    }
}
