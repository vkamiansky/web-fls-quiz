using System;
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
        public IOperationResult LoadIfNeeded(StandardImage image)
        {
            return (image.Id != 0
            ? _dataStorage.GetStandardImage(image.Id)
            : _dataStorage.GetStandardImagesIds(ImageType.General)
                    .Bind(ids => OperationResult.Try(() =>
                    {
                        if (ids.Length == 0)
                            return OperationResult.Failure<StandardImage>(new Exception("No images found in collection."));
                        var randomId = ids[_random.Next(0, ids.Length)];
                        return _dataStorage.GetStandardImage(randomId);
                    })))
                .Bind(storedImage =>
                {
                    image.ImageBase64 = storedImage.ImageBase64;
                    return OperationResult.Success();
                });
        }
    }
}
