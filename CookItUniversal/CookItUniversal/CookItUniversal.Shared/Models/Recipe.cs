using System;
using System.Collections.Generic;
using System.Text;

using Parse;

namespace CookItUniversal.Models
{
    [ParseClassName("Recipies")]
    public class Recipe : ParseObject
    {
        [ParseFieldName("Title")]
        public string Title
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [ParseFieldName("Description")]
        public string Description
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [ParseFieldName("Image")]
        public ParseFile RecipeImage
        {
            get { return GetProperty<ParseFile>(); }
            set { SetProperty<ParseFile>(value); }
        }

        [ParseFieldName("Type")]
        public string Type
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [ParseFieldName("Rating")]
        public int Rating
        {
            get { return GetProperty<int>(); }
            set { SetProperty<int>(value); }
        }

        [ParseFieldName("Duration")]
        public int Duration
        {
            get { return GetProperty<int>(); }
            set { SetProperty<int>(value); }
        }
    }
}
