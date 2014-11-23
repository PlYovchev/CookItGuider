using CookItUniversal.Models;
using GalaSoft.MvvmLight;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Windows.UI.Xaml;
using Windows.Foundation;

namespace CookItUniversal.ViewModels
{
    public class RecipeStepViewModel :ViewModelBase
    {
        private Uri currentStartImageUri;
        private Uri currentEndImageUri;
        private Uri currentVideoUri;
        private string currentDescription;
        private Visibility startImageVisibility;
        private Visibility videoVisibility;
        private string currentTimerText;
        private Point startPoint;
        private Point endPoint;
        private bool isLargeArc;
        public static Expression<Func<DBStepModel, RecipeStepViewModel>> fromStepModelDB
        {
            get
            {
                return model => new RecipeStepViewModel()
                {
                    RecipeId = model.RecipeId,
                    CurrentDescription = model.Description,
                    CurrentTimer = model.Timer,
                    CurrentStartImageUri = new Uri(model.FirstImagePath,UriKind.RelativeOrAbsolute),
                    CurrentEndImageUri = new Uri(model.EndImagePath, UriKind.RelativeOrAbsolute),
                    CurrentVideoUri = new Uri(model.VideoPath, UriKind.RelativeOrAbsolute),
                    CurrentStepNumber = model.StepNumber
                };
            }
        }

        public string RecipeId { get; set; }

        public int CurrentStepNumber { get; set; }

        public string CurrentDescription
        {
            get
            {
                return this.currentDescription;
            }
            set
            {
                this.currentDescription = value;
                this.RaisePropertyChanged(() => this.CurrentDescription);
            }
        }

        public int CurrentTimer { get; set; }

        public string CurrentTimerText
        {
            get
            {
                return this.currentTimerText;
            }
            set
            {
                this.currentTimerText = value;
                this.RaisePropertyChanged(() => this.CurrentTimerText);
            }
        }

        public Uri CurrentStartImageUri
        {
            get
            {
                return this.currentStartImageUri;
            }
            set
            {
                this.currentStartImageUri = value;
                RaisePropertyChanged(() => this.CurrentStartImageUri);
            }
        }

        public Visibility CurrentStartImageVisibility
        {
            get
            {
                return this.startImageVisibility;
            }
            set
            {
                this.startImageVisibility = value;
                this.RaisePropertyChanged(() => this.CurrentStartImageVisibility);
            }
        }

        public Uri CurrentEndImageUri
        {
            get
            {
                return this.currentEndImageUri;
            }
            set
            {
                this.currentEndImageUri = value;
                RaisePropertyChanged(() => this.CurrentEndImageUri);
            }
        }

        public Uri CurrentVideoUri
        {
            get
            {
                return this.currentVideoUri;
            }
            set
            {
                this.currentVideoUri = value;
                RaisePropertyChanged(() => this.CurrentVideoUri);
            }
        }

        public Visibility CurrentVideoVisibility
        {
            get
            {
                return this.videoVisibility;
            }
            set
            {
                this.videoVisibility = value;
                this.RaisePropertyChanged(() => this.CurrentVideoVisibility);
            }
        }

        public Point StartPoint
        {
            get
            {
                return this.startPoint;
            }
            set
            {
                this.startPoint = value;
                this.RaisePropertyChanged(() => this.StartPoint);
            }
        }

        public Point EndPoint
        {
            get
            {
                return this.endPoint;
            }
            set
            {
                this.endPoint = value;
                this.RaisePropertyChanged(() => this.EndPoint);
            }
        }

        public bool IsLargeArc
        {
            get
            {
                return this.isLargeArc;
            }
            set
            {
                this.isLargeArc = value;
                this.RaisePropertyChanged(() => this.IsLargeArc);
            }
        }

        public IList<RecipeStepViewModel> RecipeSteps { get; set; }

        public RecipeStepViewModel()
        {

        }

        public RecipeStepViewModel(IList<RecipeStepViewModel> recipeSteps)
        {
            this.RecipeSteps = recipeSteps;
            if (this.RecipeSteps != null && this.RecipeSteps.Count > 0)
            {
                SetCurrentStep(this.RecipeSteps.AsQueryable().Where(step => step.CurrentStepNumber == 2).FirstOrDefault());
            }
        }

        private void SetCurrentStep(RecipeStepViewModel recipeStep)
        {
             this.CurrentDescription = recipeStep.CurrentDescription;
             this.CurrentTimer = recipeStep.CurrentTimer;
             this.CurrentStartImageUri = recipeStep.CurrentStartImageUri;
             this.CurrentEndImageUri = recipeStep.CurrentEndImageUri;
             this.CurrentVideoUri = recipeStep.CurrentVideoUri;
             this.CurrentStepNumber = recipeStep.CurrentStepNumber;

             if (this.CurrentVideoUri != null && this.CurrentVideoUri.OriginalString != "")
             {
                 this.CurrentVideoVisibility = Visibility.Visible;
                 this.CurrentStartImageVisibility = Visibility.Collapsed;
             }
             else if (this.CurrentStartImageUri !=null && this.CurrentStartImageUri.OriginalString != "")
             {
                 this.CurrentVideoVisibility = Visibility.Collapsed;
                 this.CurrentStartImageVisibility = Visibility.Visible;
             }
             else
             {
                 this.CurrentVideoVisibility = Visibility.Collapsed;
                 this.CurrentStartImageVisibility = Visibility.Collapsed;
             }

             StartTimer();
        }

        private void StartTimer()
        {
            this.IsLargeArc = true;
            Point startPointInitial = new Point(150,90);
            this.StartPoint = startPointInitial;
            Point endPointInitial = new Point(150,91);
            this.EndPoint = endPointInitial;

            int fullTimeSum = 1 * 60;
            double singleAngle = 360d / (double)fullTimeSum;

            //////////
            float startValue = (float)1;
            this.CurrentTimerText = startValue.ToString("0.00");

            double currentAngle = singleAngle;

            var timer = new DispatcherTimer();
            timer.Tick += (obj, args) =>
            {
                if (startValue <= 0f)
                {
                    timer.Stop();
                    startValue = 0.00f;
                    this.CurrentTimerText = startValue.ToString("0.00");
                    return;
                }
                startValue -= 0.01f;
                float flootStart = (float)Math.Floor(startValue);
                if (startValue - flootStart > 0.60f)
                {
                    startValue -= 0.40f;
                }

                this.CurrentTimerText = startValue.ToString("0.00");

                //Arc animation
                double y = Math.Sin((Math.PI / 180) * currentAngle) * 70;
                double x = Math.Cos((Math.PI / 180) * currentAngle) * 70;
                Point newEndPoint = new Point(80 + x, 90 + y);
                this.EndPoint = newEndPoint;
                currentAngle += singleAngle;
                if (currentAngle > 180)
                {
                    this.IsLargeArc = false;
                }
            };

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }
    }
}
