using Parse;
using System;
using System.Collections.Generic;
using System.Text;

namespace CookItUniversal.Models
{
    [ParseClassName("Steps")]
    public class Step : ParseObject
    {
        [ParseFieldName("RecipeId")]
        public string RecipeId
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [ParseFieldName("StepNumber")]
        public int StepNumber
        {
            get { return GetProperty<int>(); }
            set { SetProperty<int>(value); }
        }

        [ParseFieldName("Description")]
        public string Description
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [ParseFieldName("Timer")]
        public int Timer
        {
            get { return GetProperty<int>(); }
            set { SetProperty<int>(value); }
        }

        [ParseFieldName("Image")]
        public ParseFile FirstImage
        {
            get { return GetProperty<ParseFile>(); }
            set { SetProperty<ParseFile>(value); }
        }

        [ParseFieldName("EndImage")]
        public ParseFile EndImage
        {
            get { return GetProperty<ParseFile>(); }
            set { SetProperty<ParseFile>(value); }
        }

        [ParseFieldName("Video")]
        public ParseFile Video
        {
            get { return GetProperty<ParseFile>(); }
            set { SetProperty<ParseFile>(value); }
        }
    }
}
