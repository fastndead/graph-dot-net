using System;
using System.Collections.Generic;
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

        [HttpGet]
        public JsonResult CheckForCycles()
        {
            return Json(Startup.MainGraph.CheckIfCycles());
        }

        [HttpDelete]
        public IActionResult Reset()
        {
            try
            {
                Startup.MainGraph.Reset();
                Startup.Task = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }

            return NoContent();
        }


        [HttpGet]
        public JsonResult GetAllConnectionsString()
        {
            if (Startup.MainGraph.nodes.Count != 0)
            {
                return Json(Startup.MainGraph.links);
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
        [HttpDelete]
        public IActionResult DeleteLink([FromBody] Graph.Link link)
        {
            try
            {
                Startup.MainGraph.DeleteLink(link);
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
        
        [HttpGet]
        public JsonResult CheckTask()
        {
            return Json(Startup.Task);
        }

        [HttpGet]
        public JsonResult CheckCompleteGraph()
        {
            return Json(Startup.MainGraph.CheckIfComplete());
        }

        [HttpPost]
        public IActionResult SetTask([FromBody]int task)
        {
            if (task == 0)
            {
                Startup.MainGraph.Reset();
            }
            Startup.Task = task;
            return NoContent();
        }

        [HttpPost]
        public IActionResult AddNodes([FromBody]Graph.Node[] nodes)
        {
            try
            {
                Startup.MainGraph.AddNodes(nodes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }

            return NoContent();
        }
        [HttpPost]
        public IActionResult AddLinks([FromBody]Graph.Link[] links)
        {
            try
            {
                Startup.MainGraph.AddLinks(links);
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