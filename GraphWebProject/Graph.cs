using System;
using System.Collections.Generic;
using System.Configuration;
using Npgsql;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Newtonsoft.Json;
using System.IO;
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

        public void AddNode(string id, int group)
        {
            this.nodes.Add(new Node(id,group));
        }

        public void AddNode(Node node)
        {
            this.nodes.Add(node);
        }

        public void AddLink(string source, string target, int value)
        {
            this.links.Add(new Link(source, target, value));
        }

        public void AddLink(Link link)
        {
            this.links.Add(link);
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

    }
}