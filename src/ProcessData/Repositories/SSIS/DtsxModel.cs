using System.ComponentModel.DataAnnotations;

namespace ProcessData.Repositories.SSIS
{
    public class DtsxModel
    {
        [Key]
        public int Id { get; set; }

        public string message { get; set; }
        public long? reference_id { get; set; }
        public int status { get; set; }
        
    }
}
