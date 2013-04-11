namespace Urdms.Dmp.Models
{
    public class ScriptProjectsViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ScriptProjectsViewModel(string id, string name)
        {
            Id = id;
            Name = name.Length > 100 ? name.Substring(0,100) + " ..." : name;
        }
    }
}