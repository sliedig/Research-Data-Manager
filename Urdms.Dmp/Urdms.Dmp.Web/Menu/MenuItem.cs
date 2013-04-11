using System.Collections.Generic;
using System.Linq;

namespace Urdms.Dmp.Web.Menu
{
    public class MenuItem
    {
        public string Title { get; set; }
        public string Path { get; set; }
        public bool? Auth { get; set; }
        public IList<string> Roles { get; set; }
        public IList<string> AdditionalPaths { get; set; }
        public IList<MenuItem> Children { get; set; }

        public bool Selected { get; set; }

        public MenuItem()
        {
            Roles = new List<string>();
            AdditionalPaths = new List<string>();
            Children = new List<MenuItem>();
        }

        public MenuItem CopyWithChildrenWhenSelected(IEnumerable<int> selected)
        {
            var isSelected = GetHashCode() == selected.FirstOrDefault();
            return new MenuItem
                       {
                           Title = Title,
                           Path = Path,
                           AdditionalPaths = AdditionalPaths,
                           Auth = Auth,
                           Roles = Roles,
                           Selected = isSelected,
                           Children = !isSelected ? new List<MenuItem>() :
                                                                             Children.Select(i => i.CopyWithChildrenWhenSelected(selected.Skip(1))).ToList()
                       };
        }
    }
}