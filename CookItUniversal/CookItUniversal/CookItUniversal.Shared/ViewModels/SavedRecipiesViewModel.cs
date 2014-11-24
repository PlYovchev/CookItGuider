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
using GalaSoft.MvvmLight.Command;

namespace CookItUniversal.ViewModels
{
    public class SavedRecipiesViewModel : ViewModelBase
    {
        private ObservableCollection<RecipeViewModel> savedRecipies;
        private ICommand deleteCommand;
        private RecipeViewModel selectedRecipe;
        private bool isEnabled;
        private ICommand dismissNotification;
        private ICommand confirmAction;

        public SavedRecipiesViewModel()
        {
            LoadRecipiesFromDb();
        }

        public ICommand DeleteCommand
        {
            get
            {
                if(this.deleteCommand == null)
                {
                    this.deleteCommand = new RelayCommand(this.OnDeleteClick);
                }

                return this.deleteCommand;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                this.isEnabled = value;
                this.RaisePropertyChanged(() => this.IsEnabled);
            }
        }

        public ICommand DismissNotification
        {
            get
            {
                if (this.dismissNotification == null)
                {
                    this.dismissNotification = new RelayCommand(this.OnDismissNotification);
                }

                return this.dismissNotification;
            }
        }

        public ICommand ConfirmAction
        {
            get
            {
                if (this.confirmAction == null)
                {
                    this.confirmAction = new RelayCommand(this.OnConfirmAction);
                }

                return this.confirmAction;
            }
        }

        public RecipeViewModel SelectedRecipe
        {
            get
            {
                return this.selectedRecipe;
            }
            set
            {
                this.selectedRecipe = value;
                this.RaisePropertyChanged(() => this.SelectedRecipe);
            }
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
        }

        private void OnDeleteClick()
        {
            if (this.SelectedRecipe != null)
            {
                this.IsEnabled = true;
            }
        }

        private async void DeleteRecipiesFromDb()
        {
            SQLiteController<DBRecipeModel> sqlController = new SQLiteController<DBRecipeModel>();
            await sqlController.DeleteItemsAsync(SelectedRecipe.RecipeId);

            SQLiteController<DBIngredientsModel> sqlControllerIngredients = new SQLiteController<DBIngredientsModel>();
            await sqlControllerIngredients.DeleteItemsAsync(SelectedRecipe.RecipeId);

            SQLiteController<DBStepModel> sqlControllerSteps = new SQLiteController<DBStepModel>();
            await sqlControllerSteps.DeleteItemsAsync(SelectedRecipe.RecipeId);

            this.SavedRecipies = this.SavedRecipies.Where(recipe => recipe.RecipeId != SelectedRecipe.RecipeId).ToList();
        }

        public void OnConfirmAction()
        {
            DeleteRecipiesFromDb();
            this.IsEnabled = false;
        }

        public void OnDismissNotification()
        {
            this.IsEnabled = false;
        }
    }
}
