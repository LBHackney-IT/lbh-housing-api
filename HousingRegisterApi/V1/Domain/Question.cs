using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace HousingRegisterApi.V1.Domain
{
    public class Question
    {
        // eg. section-name/question
        public string Id { get; set; }

        // eg. yes/no
        public string Answer { get; set; }
    }

    public static class QuestionExtensions
    {
        public static string GetAnswer(this IEnumerable<Question> questions, string name)
        {
            var question = questions?.FirstOrDefault(x => x.Id == name);
            if (question != null && !string.IsNullOrEmpty(question.Answer))
            {
                try
                {
                    var jsonData = JsonSerializer.Deserialize<string>(question.Answer);

                    return jsonData;
                }
                catch (JsonException)
                {
                    return "ERROR in question:" + question.Answer;
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
