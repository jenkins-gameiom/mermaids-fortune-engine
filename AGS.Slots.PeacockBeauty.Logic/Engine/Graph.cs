using System.Collections.Generic;

namespace AGS.Slots.MermaidsFortune.Logic
{
    public class Graph<T>
    {
        public Dictionary<Vertex<T>, List<Vertex<T>>> Edges { get; set; }
        public Dictionary<T, Vertex<T>> AllNodes { get; set; }
        public List<Vertex<T>> Roots { get; set; }

        public Graph()
        {
            Edges = new Dictionary<Vertex<T>, List<Vertex<T>>>();
            Roots = new List<Vertex<T>>();
            AllNodes = new Dictionary<T, Vertex<T>>();
        }

        public void AddEdge(T from, T to)
        {
            Vertex<T> vfrom = null; //
            Vertex<T> vto = null; //new Vertex<T>(to);
            if (!AllNodes.ContainsKey(from))
            {
                vfrom = new Vertex<T>(from);
                AllNodes.Add(from, vfrom);
            }
            if (!AllNodes.ContainsKey(to))
            {
                vto = new Vertex<T>(to);
                AllNodes.Add(to, vto);
            }

            if (!Edges.ContainsKey(AllNodes[from]))
            {
                Edges.Add(AllNodes[from], new List<Vertex<T>>());
            }
            if (!Edges[AllNodes[from]].Contains(AllNodes[to]))
            {
                Edges[AllNodes[from]].Add(AllNodes[to]);
                AllNodes[to].RefCount++;
            }
        }

        public void SetRoots()
        {
            foreach (var key in AllNodes.Values)
            {
                if (key.RefCount == 0)
                    Roots.Add(key);
            }
        }

        public bool IsLeaf(Vertex<T> node) => !Edges.ContainsKey(node);

        public bool IsRoot(Vertex<ItemOnReel> node)
        {
            return node.RefCount == 0;
        }


        
    }
}
