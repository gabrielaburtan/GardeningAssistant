﻿using System;
using System.Collections.Generic;
using System.Text;
using ga_forms.Services;
using ga_forms.Views;
using SkiaSharp;
using Xamarin.Forms;

namespace ga_forms.ViewModels
{
    class HealthSelectionViewModel : ViewModel
    {
        // Commands
        public Command GoBackCommand { get; }
        public Command DoneCommand { get; }
        public Command AutoBackgroundCommand { get; }

        // Services
        private readonly IImageManagerService _imageManagerService;

        // Properties
        public SKPath SelectionPath { get; set; }

        // Ctor
        public HealthSelectionViewModel(IImageManagerService imageManagerService)
        {
            Title = "Health Selection Page";

            GoBackCommand = new Command(OnBack);
            DoneCommand = new Command(OnDone);
            AutoBackgroundCommand = new Command(OnAutoBackground);

            _imageManagerService = imageManagerService;
        }

        public SKBitmap GetCapturedImageBitmap() => _imageManagerService.HealthInitialImageBitmap;

        private void OnAutoBackground(object obj)
        {
            //TODO
        }

        private async void OnBack(object obj)
        {
            await Shell.Current.GoToAsync($"//{nameof(HealthCameraPage)}");
        }

        private async void OnDone(object obj)
        {
            if (SelectionPath == null)
            {
                //TODO: Display dialog box that you first need to select a path
                return;
            }

            _imageManagerService.HealthSelectionPath = SelectionPath;
            await Shell.Current.GoToAsync($"//{nameof(HealthResultsPage)}");
        }
    }
}
