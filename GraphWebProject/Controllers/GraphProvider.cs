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

        [HttpDelete]
        public IActionResult DeleteNode([FromBody] Graph.Node node)
        {
            try
            {
                Startup.MainGraph.DeleteNode(node);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }

            return NoContent();
        }

        [HttpPost]
        public IActionResult AddNode([FromBody]Graph.Node node)
        {
            try
            {
                Startup.MainGraph.AddNode(node);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
                Console.WriteLine(e);
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