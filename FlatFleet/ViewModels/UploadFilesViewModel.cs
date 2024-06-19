﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using Camera.MAUI;
using Firebase.Storage;
using FlatFleet.Models;
using FlatFleet.Pages;
using Microsoft.Maui.Controls;

namespace FlatFleet.ViewModels
{
    public class UploadFilesViewModel : ViewModelBase
    {
        private bool _isCameraOn = false;

        public bool IsCameraOn
        {
            get { return _isCameraOn; }
            set
            {
                if (_isCameraOn != value)
                {
                    _isCameraOn = value;
                    OnPropertyChanged(nameof(IsCameraOn));
                }
            }
        }

        private int _filesCount = 0;

        public int FilesCount
        {
            get { return _filesCount; }
            set
            {
                if (_filesCount != value)
                {
                    _filesCount = value;
                    OnPropertyChanged(nameof(FilesCount));
                }
            }
        }

        private FirebaseStorage _storage;

        public UploadFilesPage CurrPage {get; set;}

        public ICommand CameraOnCommand { get; }
        public ICommand CameraOffCommand { get; }
        public ICommand SaveFilePic { get; }
        public ICommand UploadFileCommand {  get; }

        public event EventHandler<List<FileItem>>? FilesLoaded;
        
        public UploadFilesViewModel(FirebaseStorage storage)
        {
            LoadFiles();
            _storage = storage;
            UploadFileCommand = new Command(Upload);
            CameraOnCommand = new Command(CameraOnFunc);
            CameraOffCommand = new Command(CameraOffFunc);
            SaveFilePic = new Command(SaveVoid);
        }
        public async void SaveVoid()
        {

            // Отримати зображення як ImageSource
            ImageSource imageSource = CurrPage.cameraView.GetSnapShot(Camera.MAUI.ImageFormat.PNG);

            // Перетворити ImageSource у байти
            byte[] imageBytes = await ConvertImageSourceToBytes(imageSource);

            // Шлях до папки, де буде збережено зображення (змініть на ваш варіант)
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Images");

            // Переконайтеся, що папка існує
            Directory.CreateDirectory(folderPath);

            // Генерувати унікальне ім'я файлу
            string fileName = $"snapshot_{DateTime.Now:yyyyMMddHHmmss}.png";
            string filePath = Path.Combine(folderPath, fileName);

            // Записати байти зображення у файл
            File.WriteAllBytes(filePath, imageBytes);
            double imageSizeInKB = imageBytes.Length / 1024.0;

            CurrPage.lblImageSize.Text = $"Розмір зображення: {imageSizeInKB:F2} KB";

            FileItem file = new FileItem()
            {
                Title = $"IMG-{DateTime.Now:yyyyMMddHHmmss}",
                Size = imageSizeInKB.ToString()
            };

            FilesLoaded?.Invoke(this, new List<FileItem>() { file });

            FilesCount++;
        }
        private async Task<byte[]> ConvertImageSourceToBytes(ImageSource imageSource)
        {
            if (imageSource is FileImageSource fileImageSource && fileImageSource.File.StartsWith("file:", StringComparison.Ordinal))
            {
                var fileBytes = await File.ReadAllBytesAsync(fileImageSource.File.Substring(5));
                return fileBytes;
            }
            else if (imageSource is StreamImageSource streamImageSource)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    var stream = await streamImageSource.Stream(CancellationToken.None);
                    await stream.CopyToAsync(ms);
                    return ms.ToArray();
                }
            }
            else if (imageSource is UriImageSource uriImageSource)
            {
                using (var client = new HttpClient())
                {
                    var stream = await client.GetStreamAsync(uriImageSource.Uri);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await stream.CopyToAsync(ms);
                        return ms.ToArray();
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Unsupported ImageSource type.");
            }
        }

        private async void CameraOnFunc()
        {
            if (CurrPage == null)
                throw new Exception("Page is null");

            if (CurrPage.cameraView == null)
                throw new Exception("CameraView is null");

            if (CurrPage.FrameName == null)
                throw new Exception("FrameName is null");

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (!IsCameraOn)
                {
                    await CurrPage.cameraView.StartCameraAsync();
                    IsCameraOn = true;
                }
            });
        }

        private async void CameraOffFunc()
        {
            if (_isCameraOn)
            {
                await (Application.Current.MainPage as UploadFilesPage).cameraView.StopCameraAsync();
                _isCameraOn = false;
            }
            (Application.Current.MainPage as UploadFilesPage).cameraView.IsVisible = false;
            (Application.Current.MainPage as UploadFilesPage).FrameName.IsVisible = false;
        }
        private async void Upload()
        {
            LoadFiles();
        }
        private async void LoadFiles()
        {
            var files = await FilePicker.PickAsync();
            var filesToUpload = await files.OpenReadAsync();
            var FilesToUploadList = new List<FileItem>()
            {
                new FileItem { Title = files.FileName, Size = filesToUpload.Length.ToString() },
            };
            // Console.WriteLine(filesToUpload);

            FilesLoaded?.Invoke(this, FilesToUploadList);

            await _storage
                .Child("documents/" + files.FileName)
                .PutAsync(filesToUpload);
        }   
       
    }

}
