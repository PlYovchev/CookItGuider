using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CookItUniversal.Models
{
    [Table("Ingredients")]
    public class DBIngredientsModel : DBBaseModel
    {
        public string Name { get; set; }

        public string Quantity { get; set; }

    }
}
