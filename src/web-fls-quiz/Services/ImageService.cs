﻿using System;
using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;

namespace WebFlsQuiz.Services
{
    public class ImageService : IImageService
    {
        private readonly IDataStorage _dataStorage;

        private readonly Random _random = new Random();

        public ImageService(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        public void LoadIfNeeded(StandardImage image)
        {
            if (image.ImageId != 0)
            {
                image.ImageBase64 = _dataStorage.GetStandardImage(image.ImageId).Result.ImageBase64;
            }
            else
            {
                if (string.IsNullOrEmpty(image.ImageBase64))
                {
                    var standardImagesIds = _dataStorage.GetStandardImagesIds().Result;
                    if (standardImagesIds.Length > 0)
                    {
                        var randomIndex = _random.Next(0, standardImagesIds.Length);
                        var randomImageId = standardImagesIds[randomIndex];
                        image.ImageBase64 = _dataStorage.GetStandardImage(randomImageId).Result.ImageBase64;
                    }
                }
            }
        }
    }
}