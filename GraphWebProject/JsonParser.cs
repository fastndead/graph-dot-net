using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;


namespace GraphWebProject
{
    public class JsonParser
    {
        public static void Parser()//вспомогательный метод для парсинга json в sql запрос и заполнения таблицы
                                    //в самом проекте использоваться вряд ли будет
        {
            using (StreamReader r = new StreamReader("/Users/ASMANandNASTYA/Projects/GraphWebProject/GraphWebProject/wwwroot/json/NODES.json"))
            {
                string json = r.ReadToEnd();
                List<Graph.Node> nodes = JsonConvert.DeserializeObject<List<Graph.Node>>(json);
                var sw = new StreamWriter("/Users/ASMANandNASTYA/Projects/GraphWebProject/GraphWebProject/scripts/nodes-script.sql");
                using (sw)
                {
                    foreach (var node in nodes)
                    {
                        sw.WriteLine(
                            "INSERT INTO graph_scheme.t_nodes (c_id, c_name, c_group) VALUES (nextval('graph_scheme.nodes_seq'), '{0}', {1});", node.id,node.group);
                    } 
                }
            }
            using (StreamReader r = new StreamReader("/Users/ASMANandNASTYA/Projects/GraphWebProject/GraphWebProject/wwwroot/json/LINKS.json"))
            {
                string json = r.ReadToEnd();
                List<Graph.Link> links = JsonConvert.DeserializeObject<List<Graph.Link>>(json);
                var sw = new StreamWriter("/Users/ASMANandNASTYA/Projects/GraphWebProject/GraphWebProject/scripts/links-script.sql");
                using (sw)
                {
                    foreach (var link in links)
                    {
                        sw.WriteLine(
                            "INSERT INTO graph_scheme.t_links (c_id, c_source, c_target, c_value) VALUES (nextval('graph_scheme.links_seq'),'{0}','{1}',{2});", link.source,link.target, link.value);
                    } 
                }
            }
            
        }
    }
}