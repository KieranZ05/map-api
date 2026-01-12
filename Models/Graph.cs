namespace MapAPI.Models
{
    public class Graph
    {
        public Dictionary<string, List<Edge>> Nodes { get; set; }

        public Graph()
        {
            Nodes = new Dictionary<string, List<Edge>>();
        }

        public void AddEdge(string from, string to, int distance)
        {
            if (!Nodes.ContainsKey(from))
            {
                Nodes[from] = new List<Edge>();
            }

            if (!Nodes.ContainsKey(to))
            {
                Nodes[to] = new List<Edge>();
            }

            Nodes[from].Add(new Edge(to, distance));
            Nodes[to].Add(new Edge(from, distance)); // Since the graph is bi-directional
        }
    }

    public class Edge
    {
        public string To { get; set; }
        public int Distance { get; set; }

        public Edge(string to, int distance)
        {
            To = to;
            Distance = distance;
        }
    }
}
