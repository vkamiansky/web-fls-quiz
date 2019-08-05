﻿namespace HolyJsQuiz2019.Models
{
    public class Question
    {
        public int Id { get; set; }

        public string ImageBase64 { get; set; }

        public string Text { get; set; }

        public Answer[] Answers { get; set; }
    }
}
