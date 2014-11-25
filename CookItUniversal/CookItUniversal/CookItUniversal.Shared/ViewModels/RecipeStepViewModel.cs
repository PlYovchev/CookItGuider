using CookItUniversal.Models;
using GalaSoft.MvvmLight;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Windows.UI.Xaml;
using Windows.Foundation;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Windows.Devices.Sensors;
using System.Diagnostics;
using Windows.UI.Core;

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
        private int currentStepIndex;
        private bool nextButtonEnabled;
        private bool previousButtonEnabled;
        private bool playAlarm = false;
        private ICommand goToNextStep;
        private ICommand goToPreviousStep;
        private DispatcherTimer timer;
        private int currentStepNumber;


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

        public int CurrentStepNumber 
        {
            get
            {
                return this.currentStepNumber;
            }
            set
            {
                this.currentStepNumber = value;
                this.RaisePropertyChanged(() => this.CurrentStepNumber);
            }
        }

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
                if (this.CurrentStartImageUri != null && this.CurrentStartImageUri.OriginalString != "")
                {
                    RaisePropertyChanged(() => this.CurrentStartImageUri);
                }
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
                if (this.CurrentVideoUri != null && this.CurrentVideoUri.OriginalString != "")
                {
                    RaisePropertyChanged(() => this.CurrentVideoUri);
                }
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

        public bool PlayAlarm
        {
            get
            {
                return this.playAlarm;
            }
            set
            {
                this.playAlarm = value;
                this.RaisePropertyChanged(() => this.PlayAlarm);
            }
        }

        public bool isNextButtonEnabled
        {
            get
            {
                return this.nextButtonEnabled;
            }
            set
            {
                this.nextButtonEnabled = value;
                this.RaisePropertyChanged(() => this.isNextButtonEnabled);
            }
        }

        public bool isPreviousButtonEnabled
        {
            get
            {
                return this.previousButtonEnabled;
            }
            set
            {
                this.previousButtonEnabled = value;
                this.RaisePropertyChanged(() => this.isPreviousButtonEnabled);
            }
        }

        public ICommand GoToNextStep
        {
            get
            {
                if (this.goToNextStep == null)
                {
                    this.goToNextStep = new RelayCommand(LoadNextStep);
                }

                return this.goToNextStep;
            }
        }

        public ICommand GoToPreviousStep
        {
            get
            {
                if (this.goToPreviousStep == null)
                {
                    this.goToPreviousStep = new RelayCommand(LoadPreviousStep);
                }

                return this.goToPreviousStep;
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
                this.currentStepIndex = 1;

                CallStepProperties();
            }
        }

        private void CallStepProperties()
        {
            if (this.timer != null)
            {
                this.timer.Stop();
            }

            SetCurrentStep();

            StartTimer();

            SetStepButtonsEnability();
        }

        private void SetStepButtonsEnability()
        {
            if (this.currentStepIndex == 1)
            {
                this.isPreviousButtonEnabled = false;
            }
            else
            {
                this.isPreviousButtonEnabled = true;
            }

            if (this.CurrentTimer != -1)
            {
                this.isNextButtonEnabled = true;
            }
            else
            {
                this.isNextButtonEnabled = false; 
            }
        }

        private void SetCurrentStep()
        {
            RecipeStepViewModel recipeStep = this.RecipeSteps.AsQueryable()
                                                                 .Where(step => step.CurrentStepNumber == this.currentStepIndex)
                                                                 .FirstOrDefault();

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
             else if (this.CurrentStartImageUri != null && this.CurrentStartImageUri.OriginalString != "")
             {
                 this.CurrentVideoVisibility = Visibility.Collapsed;
                 this.CurrentStartImageVisibility = Visibility.Visible;
             }
             else
             {
                 this.CurrentVideoVisibility = Visibility.Collapsed;
                 this.CurrentStartImageVisibility = Visibility.Collapsed;
             }
        }

        private void StartTimer()
        {
            this.IsLargeArc = true;
            Point startPointInitial = new Point(150,90);
            this.StartPoint = startPointInitial;
            Point endPointInitial = new Point(150,91);
            this.EndPoint = endPointInitial;

            int fullTimeSum = this.CurrentTimer * 60;
            double singleAngle = 360d / (double)fullTimeSum;

            //////////
            float startValue = (float)this.CurrentTimer;
            this.CurrentTimerText = startValue.ToString("0.00");

            double currentAngle = singleAngle;
            if (this.CurrentTimer < 0)
            {
                this.CurrentTimerText = "Bon Apette!";
                startPointInitial = new Point(150, 90);
                this.StartPoint = startPointInitial;
                endPointInitial = new Point(150, 90);
                this.EndPoint = endPointInitial;
            }
            if (this.CurrentTimer > 0)
            {
                this.timer = new DispatcherTimer();
                this.timer.Tick += (obj, args) =>
                {
                    startValue -= 0.01f;
                    if (startValue <= 0f)
                    {
                        timer.Stop();
                        startValue = 0.00f;
                        this.CurrentTimerText = startValue.ToString("0.00");
                        this.PlayAlarm = true;
                        return;
                    }

                    float flootStart = (float)Math.Floor(startValue);
                    if (startValue - flootStart > 0.60f && startValue - flootStart < 1.00f)
                    {
                        startValue -= 0.40f;
                    }

                    this.CurrentTimerText = startValue.ToString("0.00");

                    //Arc animation
                    double y = Math.Sin((Math.PI / 180) * currentAngle) * 50;
                    double x = Math.Cos((Math.PI / 180) * currentAngle) * 50;
                    Point newEndPoint = new Point(100 + x, 90 + y);
                    this.EndPoint = newEndPoint;
                    currentAngle += singleAngle;
                    if (currentAngle > 180)
                    {
                        this.IsLargeArc = false;
                    }
                };

                this.timer.Interval = TimeSpan.FromSeconds(1);
                this.timer.Start();
            }
        }

        public void LoadNextStep()
        {
            if (this.isNextButtonEnabled)
            {
                this.currentStepIndex++;

                CallStepProperties();
            }
        }

        public void LoadPreviousStep()
        {
            if (this.isPreviousButtonEnabled)
            {
                this.currentStepIndex--;

                CallStepProperties();
            }
        }
    }
}
