using CookItUniversal.Models;
using CookItUniversal.SQLiteController;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Parse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace CookItUniversal.ViewModels
{
    public class RecipeViewModel : ViewModelBase
    {
        private BitmapImage imageSource;
        private ObservableCollection<Ingredient> ingredients;
        private ICommand saveClick;
        private byte[] imageStream;
        private string path;
        public MediaElement mElement;

        public static Expression<Func<Recipe, RecipeViewModel>> fromRecipeModel
        {
            get
            {
                return model => new RecipeViewModel()
                {
                    RecipeId = model.ObjectId,
                    Title = model.Title,
                    Description = model.Description,
                    Type = model.Type,
                    Rating = model.Rating,
                    Duration = model.Duration,
                    ImageUri = model.RecipeImage != null ? model.RecipeImage.Url : null
                };
            }
        }

        public string RecipeId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public string Type { get; set; }

        public int Rating { get; set; }

        public int Duration { get; set; }

        public Uri ImageUri { get; set; }

        public BitmapImage ImageSource
        {
            get 
            {
                return imageSource;
            }
            set
            {
                imageSource = value;
                RaisePropertyChanged(() => ImageSource);
            }
        }

        public IEnumerable<Ingredient> Ingredients
        {
            get
            {
                if (ingredients == null)
                {
                    this.ingredients = new ObservableCollection<Ingredient>();
                }

                return this.ingredients;
            }
            set
            {
                if (ingredients == null)
                {
                    ingredients = new ObservableCollection<Ingredient>();
                }

                this.ingredients.Clear();
                foreach (var item in value)
                {
                    this.ingredients.Add(item);
                }
            }
        }

        public ICommand SaveClick
        {
            get
            {
                if (saveClick == null)
                {
                    saveClick = new RelayCommand(this.Save);
                }

                return saveClick;
            }
        }

        public async Task LoadImage()
        {
            if (ImageUri == null)
            {
                return;
            }
            imageStream = await new HttpClient().GetByteArrayAsync(ImageUri);
            MemoryStream stream;
            using (stream = new MemoryStream(imageStream))
            {
                BitmapImage bmImage = new BitmapImage();
                await bmImage.SetSourceAsync(stream.AsRandomAccessStream());
                ImageSource = bmImage;
            }
        }

        public async void Save()
        {
            await SaveRecipeInDB();
            await SaveIngredientsInDB();
            await SaveStepsInDB();

            SQLiteController<DBRecipeModel> sqlController = new SQLiteController<DBRecipeModel>();
            SQLiteController<DBIngredientsModel> sqlController2 = new SQLiteController<DBIngredientsModel>();
            SQLiteController<DBStepModel> sqlController3 = new SQLiteController<DBStepModel>();

            List<DBRecipeModel> list = await sqlController.FetchItems() as List<DBRecipeModel>;
            List<DBIngredientsModel> list2 = await sqlController2.FetchItems() as List<DBIngredientsModel>;
            List<DBStepModel> list3 = await sqlController3.FetchItems() as List<DBStepModel>;
        }

        private async Task<string> SaveFileInStorage(string fileName, byte[] stream)
        {
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            StorageFile file = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            try
            {
                if (file != null)
                {
                    await FileIO.WriteBytesAsync(file, stream);
                }
            }
            catch (FileNotFoundException)
            {
            }

            return file.Path;
        }

        private async Task SaveRecipeInDB()
        {
            string imagePath = await SaveFileInStorage(this.RecipeId + "\\reviewImage.jpg", imageStream);

            SQLiteController<DBRecipeModel> sqlController = new SQLiteController<DBRecipeModel>();

            await sqlController.CreateTableIfNotExist();

            DBRecipeModel recipeModel = new DBRecipeModel
            {
                Id = this.RecipeId,
                Title = this.Title,
                Description = this.Description,
                Duration = this.Duration,
                Rating = this.Rating,
                imagePath = imagePath
            };

            DBRecipeModel recipeModelExists = await sqlController.FindItemById(recipeModel.Id);
            if (recipeModelExists == null)
            {
                await sqlController.AddItemAsync(recipeModel);
            }

            //List<DBRecipeModel> list = await sqlController.FetchItems() as List<DBRecipeModel>;
        }

        private async Task SaveIngredientsInDB()
        {
            SQLiteController<DBIngredientsModel> sqlController = new SQLiteController<DBIngredientsModel>();

            await sqlController.CreateTableIfNotExist();

            foreach (var ingredient in Ingredients)
            {
                DBIngredientsModel ingredientModel = new DBIngredientsModel
                {
                    Id = ingredient.ObjectId,
                    RecipeId = ingredient.RecipeId,
                    Name = ingredient.Name,
                    Quantity = ingredient.Quantity
                };

                DBIngredientsModel ingredientModelExists = await sqlController.FindItemById(ingredientModel.Id);
                if (ingredientModelExists == null)
                {
                    await sqlController.AddItemAsync(ingredientModel);
                }
            }  
        }

        private async Task SaveStepsInDB()
        {
            var recipeSteps = await new ParseQuery<Step>()
                    .Where(step => step.RecipeId == RecipeId)
                    .FindAsync();
            var steps = recipeSteps.ToList();

            SQLiteController<DBStepModel> sqlController = new SQLiteController<DBStepModel>();
            await sqlController.CreateTableIfNotExist();

            foreach (var step in steps)
            {
                string startImagePath = "";
                if (step.FirstImage != null)
                {
                    byte[] startImageStream = await new HttpClient().GetByteArrayAsync(step.FirstImage.Url);
                    startImagePath = await SaveFileInStorage(this.RecipeId + "\\startImage" + step.StepNumber +".jpg", startImageStream);
                }
                string endImagePath = "";
                if (step.EndImage != null)
                {
                    byte[] endImageStream = await new HttpClient().GetByteArrayAsync(step.EndImage.Url);
                    endImagePath = await SaveFileInStorage(this.RecipeId + "\\endImage" + step.StepNumber + ".jpg", endImageStream);
                }
                string videoPath = "";
                if (step.Video != null)
                {
                    byte[] videoStream = await new HttpClient().GetByteArrayAsync(step.Video.Url);
                    videoPath = await SaveFileInStorage(this.RecipeId + "\\video" + step.StepNumber + ".mp4", videoStream);
                    mElement.Source = new Uri(videoPath, UriKind.RelativeOrAbsolute);
                }

                DBStepModel stepModel = new DBStepModel
                {
                    Id = step.ObjectId,
                    RecipeId = step.RecipeId,
                    StepNumber = step.StepNumber,
                    Timer = step.Timer,
                    Description = step.Description,
                    FirstImagePath = startImagePath,
                    EndImagePath = endImagePath,
                    VideoPath = videoPath
                };

                DBStepModel stepModelExists = await sqlController.FindItemById(stepModel.Id);
                if (stepModelExists == null)
                {
                    await sqlController.AddItemAsync(stepModel);
                }
            }
        }
        
        public async void LoadData()
        {
            Stream myString;
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            try
            {
                StorageFile sampleFile = await localFolder.GetFileAsync(this.RecipeId + "\\reviewImage.jpg");
                //myString = await FileIO.ReadBufferAsync(sampleFile);
            }
            
            catch (Exception)
            {
                // No file to load or error loading it
            }
            BitmapImage bmm = new BitmapImage();
            bmm.UriSource = new Uri(this.path, UriKind.RelativeOrAbsolute);
            ImageSource = bmm;
            // Data is now in myString
        }
    }
}
