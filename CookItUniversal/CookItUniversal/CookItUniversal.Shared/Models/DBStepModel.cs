using System;
using System.Collections.Generic;
using System.Text;

namespace CookItUniversal.Models
{
    public class DBStepModel:DBBaseModel
    {

        public string RecipeId { get; set; }

        public int StepNumber { get; set; }

        public string Description { get; set; }

        public int Timer { get; set; }

        public string FirstImagePath { get; set; }

        public string EndImagePath { get; set; }

        public string VideoPath { get; set; }
    }
}
