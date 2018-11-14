using System;
using Microsoft.AspNetCore.Mvc;

namespace GraphWebProject.Controllers
{
    public class GraphProvider : Controller
    {
        //public Graph MainGraph = new Graph();
        // GET
        public JsonResult Index()
        {
            return Json(Startup.MainGraph);
        }

        [HttpPost]
        public IActionResult Create([FromBody]Graph.Node node)
        {
            try
            {
                Startup.MainGraph.AddNode(node.id, node.group);
            }
            catch 
            {
                return StatusCode(500);
            }

            return NoContent();
        }
        
        [HttpPost]
        public IActionResult Update(string source, string target, int value)
        {
            try
            {
                Startup.MainGraph.AddLink(source, target, value);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }

            return NoContent();
        }

        public JsonResult Sample()
        {
            var graph = new Graph();
            graph.GetFromDb();
            return Json(graph);
        }
    }
}