using CookItUniversal.Models;
using CookItUniversal.SQLiteController;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CookItUniversal.ViewModels
{
    public class SavedRecipiesViewModel : ViewModelBase
    {
        private ObservableCollection<RecipeViewModel> savedRecipies;

        public SavedRecipiesViewModel()
        {
            LoadRecipiesFromDb();
        }

        public IEnumerable<RecipeViewModel> SavedRecipies
        {
            get
            {
                if (savedRecipies == null)
                {
                    this.savedRecipies = new ObservableCollection<RecipeViewModel>();
                }

                return this.savedRecipies;
            }
            set
            {
                if (savedRecipies == null)
                {
                    savedRecipies = new ObservableCollection<RecipeViewModel>();
                }

                this.savedRecipies.Clear();
                foreach (var item in value)
                {
                    this.savedRecipies.Add(item);
                }
            }
        }

        private async Task LoadRecipiesFromDb()
        {

            SQLiteController<DBRecipeModel> sqlController = new SQLiteController<DBRecipeModel>();
            await sqlController.CreateTableIfNotExist();

            List<DBRecipeModel> recipiesList = await sqlController.FetchItems() as List<DBRecipeModel>;

            var recipiesViewModel = recipiesList.AsQueryable().Select(RecipeViewModel.fromRecipeModelDB).ToList();

            foreach (var savedRecipe in recipiesViewModel)
            {
                await savedRecipe.LoadImageFromDb();
            }

            this.SavedRecipies = recipiesViewModel;

           

            //var recipiesFromParse = await new ParseQuery<Recipe>().FindAsync();
            //var recipiesVM = recipiesFromParse.AsQueryable().Select(RecipeViewModel.fromRecipeModel).ToList();
            //foreach (var recipe in recipiesVM)
            //{
            //    var recipeIngredients = await new ParseQuery<Ingredient>()
            //        .Where(ingredient => ingredient.RecipeId == recipe.RecipeId)
            //        .FindAsync();
            //    var ingredients = recipeIngredients.ToList();
            //    recipe.Ingredients = ingredients;
            //    await recipe.LoadImage();
            //}

            //this.Recipies = recipiesVM;
        }
    }
}
