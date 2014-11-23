using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CookItUniversal.Models
{
    [Table("Recipies")]
    public class DBRecipeModel:DBBaseModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int Rating { get; set; }

        public int Duration { get; set; }

        public string ImagePath { get; set; }

        public string Type { get; set; }
    }
}
