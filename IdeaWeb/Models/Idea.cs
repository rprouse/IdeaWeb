using System.ComponentModel.DataAnnotations;

namespace IdeaWeb.Models
{
    public class Idea
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You must specify a name for the idea")]
        public string Name { get; set; }

        public string Description { get; set; }

        public int? Rating { get; set; }
    }
}
