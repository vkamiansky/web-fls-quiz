using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebFlsQuiz.Models;

namespace WebFlsQuiz.Interfaces
{
    public interface IImageService
    {
        void LoadIfNeeded(StandardImage image);
    }
}
