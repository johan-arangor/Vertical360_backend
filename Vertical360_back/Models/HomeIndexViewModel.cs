using System.Net.Http.Headers;
using Vertical360_back.Entityes;

namespace Vertical360_back.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Products> Products { get; set; } = new List<Products>();
        public IEnumerable<Countries> Countries { get; set; } = new List<Countries>();
    }
}
