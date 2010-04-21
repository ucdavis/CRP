using CRP.Core.Domain;

namespace CRP.Core.Abstractions
{
    public interface IQuestionAnswer 
    {
        Transaction Transaction { get; set; }
        QuestionSet QuestionSet { get; set; }
        Question Question { get; set; }
        string Answer { get; set; }
    }
}