using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Urdms.Dmp.Models
{
	public class CsvDumpViewModel
	{
		[Display(Name = "Data Management Plans")]
		public IList<DmpListViewModel> Projects { get; set; }
        [Display(Name = "Data Collections")]
        public IList<CollectionListViewModel> DataCollections { get; set; }
	}
}