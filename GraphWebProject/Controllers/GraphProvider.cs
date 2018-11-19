using System;
using Microsoft.AspNetCore.Mvc;

namespace GraphWebProject.Controllers
{
    public class GraphProvider : Controller
    {
        public JsonResult Index()
        {
            return Json(Startup.MainGraph);
        }
        
        [HttpGet]
        public JsonResult GetAllNodes()
        {
            if (Startup.MainGraph.nodes.Count != 0)
            {
                return Json(Startup.MainGraph.nodes); 
            }

            return Json("");
        }

        [HttpPost]
        public IActionResult AddNode([FromBody]Graph.Node node)
        {
            try
            {
                Startup.MainGraph.AddNode(node);
            }
            catch 
            {
                return StatusCode(500);
            }

            return NoContent();
        }
        
        [HttpPost]
        public IActionResult AddLink([FromBody]Graph.Link link)
        {
            try
            {
                Startup.MainGraph.AddLink(link);
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