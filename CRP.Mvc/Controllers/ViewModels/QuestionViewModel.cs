
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRP.Controllers.ViewModels
{
    public class QuestionViewModel
    {
        public QuestionViewModel()
        {
            Options = new List<string>();
            Validators = new List<int>();
        }


        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        public int QuestionTypeId { get; set; }

        [Required]
        public IList<string> Options { get; set; }

        [Required]
        public IList<int> Validators { get; set; }
    }
}
