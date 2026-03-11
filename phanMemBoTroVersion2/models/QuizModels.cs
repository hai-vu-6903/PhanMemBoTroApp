using System.Collections.Generic;

namespace phanMemBoTroVersion2.Models
{
    public class Root
    {
        public List<ExamContainer> exams { get; set; }
    }

    public class ExamContainer
    {
        public int id { get; set; }
        public string title { get; set; }
        public List<Question> questions { get; set; }
    }

    public class Question
    {
        public string question { get; set; }
        public List<string> options { get; set; }
        public int answer { get; set; }

        public int selected { get; set; } = -1;
    }
}