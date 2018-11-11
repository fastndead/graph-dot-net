using Microsoft.AspNetCore.Mvc;

namespace GraphWebProject.Controllers
{
    public class GraphProvider : Controller
    {
        // GET
        public JsonResult Index()
        {
            var graph = new Graph();
            graph.GetFromDb();
            return Json(graph);
        }
    }
}