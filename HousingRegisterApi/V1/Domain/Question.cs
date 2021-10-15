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
            return JsonSerializer.Deserialize<string>(question?.Answer) ?? string.Empty;
        }
    }
}
