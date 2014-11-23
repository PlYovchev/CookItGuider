using CookItUniversal.Models;
using GalaSoft.MvvmLight;
using Parse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Windows.UI.Xaml.Controls;
using System.Net;
using System.IO;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace CookItUniversal.ViewModels
{
    public class RecipiesViewModel : ViewModelBase
    {
        private ObservableCollection<RecipeViewModel> recipies;

        public RecipiesViewModel()
        {
            LoadRecipies();
        }

        public IEnumerable<RecipeViewModel> Recipies
        {
            get
            {
                if (recipies == null)
                {
                    this.recipies = new ObservableCollection<RecipeViewModel>();
                }

                return this.recipies;
            }
            set
            {
                if (recipies == null)
                {
                    recipies = new ObservableCollection<RecipeViewModel>();
                }

                this.recipies.Clear();
                foreach (var item in value)
                {
                    this.recipies.Add(item);
                }
            }
        }

        private async Task LoadRecipies()
        {
            var recipiesFromParse = await new ParseQuery<Recipe>().FindAsync();
            var recipiesVM = recipiesFromParse.AsQueryable().Select(RecipeViewModel.fromRecipeModel).ToList();
            foreach (var recipe in recipiesVM)
            {
                var recipeIngredients = await new ParseQuery<Ingredient>()
                    .Where(ingredient => ingredient.RecipeId == recipe.RecipeId)
                    .FindAsync();
                var ingredients = recipeIngredients.ToList();
                recipe.Ingredients = ingredients;
                await recipe.LoadImage();
            }

            this.Recipies = recipiesVM;
        }
    }
}
