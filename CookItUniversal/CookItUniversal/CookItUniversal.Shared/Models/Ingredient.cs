using Parse;
using System;
using System.Collections.Generic;
using System.Text;

namespace CookItUniversal.Models
{
    [ParseClassName("Ingredients")]
    public class Ingredient : ParseObject
    {
        [ParseFieldName("RecipeId")]
        public string RecipeId
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [ParseFieldName("Name")]
        public string Name
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [ParseFieldName("Quantity")]
        public string Quantity
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }
    }
}
