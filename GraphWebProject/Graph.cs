using System;
using System.Collections.Generic;
using System.Configuration;
using Npgsql;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using Microsoft.IdentityModel.Protocols;

namespace GraphWebProject
{
    public class Graph//главный класс сущности графа
    {
        public class Node
        {
            // public int index { get; set; }
            public string id { get; set; }
            public int group { get; set; }

            public Node( string id, int group)
            {
                //this.index = index;
                this.id = id;
                this.group = group;
            }

            public Node()
            {}
        }

        public class Link
        {
          //  public int index { get; set; }
            public string source { get; set; }
            public string target { get; set; }
            public int value { get; set; }

            public Link(string source, string target, int value)
            {
              //  this.index = index;
                this.source = source;
                this.target = target;
                this.value = value;
            }

            public Link()
            {}
        }

        public List<Node> nodes;
        public List<Link> links;
        
        public Graph()
        {
            nodes = new List<Node>();
            links = new List<Link>();
        }

        public void DeleteNode(Node node)
        {
            bool found = false;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].id == node.id)
                {
                    found = true;
                    nodes.Remove(nodes[i]);
                    i--;
                }
            }

            if (!found)
            {
                throw new Exception("No such node!");
            }
            
            foreach (var link in this.links)
            {
                if (link.source == node.id || link.target == node.id)
                {
                    this.links.Remove(link);
                }
            }
        }
        
        public void DeleteLink(Link link)
        {
            bool found = false;
            for (int i = 0; i < links.Count; i++)
            {
                if (links[i].source == link.source && links[i].target == link.target)
                {
                    found = true;
                    links.Remove(links[i]);
                    i--;
                }
            }

            if (!found)
            {
                throw new Exception("No such link!");
            }
        }



        public void Reset()
        {
            nodes.Clear();
            links.Clear();
        }

        public void AddNode(string id, int group)
        {
            foreach (var node in nodes)
            {
                if(node.id == id)
                    return;
            }
            this.nodes.Add(new Node(id,group));
        }

        public void AddNode(Node newNode)
        {
            foreach (var node in nodes)
            {
                if(node.id == newNode.id)
                    return;
            }
            this.nodes.Add(newNode);
        }

        public void AddNodes(Node[] nodes)
        {
            foreach (var node in nodes)
            {
                this.AddNode(node);
            }
        }
        public void AddLinks(Link[] links)
        {
            foreach (var link in links)
            {
                this.AddLink(link);
            }
        }

        public void AddLink(string source, string target, int value)
        {
            if (source == target)
            {
                return;
            }
            foreach (var link in links)
            {
                if(link.source == source && link.target == target || link.source == target && link.target == source)
                    return;
            }
            this.links.Add(new Link(source, target, value));
        }

        public void AddLink(Link newLink)
        {
            if (newLink.source == newLink.target)
            {
                return;
            }
            foreach (var link in links)
            {
                if(link.source == newLink.source && link.target == newLink.target || 
                   link.source == newLink.target && link.target == newLink.source)
                    return;
            }
            this.links.Add(newLink);
        }

        public bool CheckIfComplete()
        {
            if (links.Count != nodes.Count * (nodes.Count - 1)/2)
            {
                return false;
            }

            foreach (var node in nodes)
            {
                var linkCount = 0;
                foreach (var link in links)
                {
                    if (link.source == node.id || link.target == node.id)
                    {
                        linkCount++;
                    }
                }

                if (linkCount != nodes.Count - 1)
                {
                    return false;
                }
            }

            return true;
        }

        public void GetFromDb()//метод заполнения сущности графа с базы данных
        {
            var connString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;

            using (var conn = new NpgsqlConnection(connString))//считывание узлов с базы данных
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT * from graph_scheme.t_nodes", conn))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        nodes.Add(new Node(reader.GetString(1),reader.GetInt32(2)));
                    }
            }
            
            using (var conn = new NpgsqlConnection(connString))//считывание связей с базы данных
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT * from graph_scheme.t_links", conn))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        links.Add(new Link(reader.GetString(1),reader.GetString(2),reader.GetInt32(3)));
                    }
            }
            
        }

        public bool CheckIfCycles()
        {
            var color = new Dictionary<string, int>();
            foreach (var node in nodes)
            {
                color.Add(node.id, 0);
            }

            if (nodes.Count > 0)
            {
                return dfs(nodes[0].id, nodes[0].id);
            }
            return false;
            

            bool dfs(string curNode, string prevNode)
            {
                color[curNode] = 1;
                foreach (var connection in findLinkFromNode(curNode))
                {
                    if(connection == prevNode)
                        continue;
                    
                    if (color[connection] == 0)
                    {
                        if (dfs(connection, curNode))
                        {
                            return true;
                        }
                    }
                    else if(color[connection] == 1)
                    {
                        return true;
                    }
                }

                color[curNode] = 2;
                return false;
            }

            List<string> findLinkFromNode(string nodeId)
            {
                var listOfConnections = new List<string>();
                foreach (var item in links.Select((value, i) => new { i, value }))
                {
                    if (item.value.source == nodeId)
                        listOfConnections.Add(item.value.target);
                    if(item.value.target == nodeId)
                        listOfConnections.Add(item.value.source);
                }

                return listOfConnections;
            }

        }

    }
}