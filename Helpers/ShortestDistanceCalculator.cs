using MapAPI.Models;

namespace MapAPI.Helpers
{
    public class ShortestDistanceCalculator
    {
        public static int FindShortestDistance(Graph graph, string startNode, string endNode)
        {
            // Handle the case where the nodes do not exist
            if (!graph.Nodes.ContainsKey(startNode) || !graph.Nodes.ContainsKey(endNode))
            {
                throw new ArgumentException($"One or both of the nodes '{startNode}' and '{endNode}' do not exist in the graph.");
            }

            var distances = new Dictionary<string, int>();
            var nodes = new List<string>(graph.Nodes.Keys);

            foreach (var node in nodes)
            {
                distances[node] = int.MaxValue;
            }
            distances[startNode] = 0;

            var priorityQueue = new SortedSet<Tuple<int, string>>();
            priorityQueue.Add(new Tuple<int, string>(0, startNode));

            while (priorityQueue.Count > 0)
            {
                var currentNode = priorityQueue.Min.Item2;
                priorityQueue.Remove(priorityQueue.Min);

                if (currentNode == endNode)
                {
                    break;
                }

                foreach (var edge in graph.Nodes[currentNode])
                {
                    int newDist = distances[currentNode] + edge.Distance;
                    if (newDist < distances[edge.To])
                    {
                        distances[edge.To] = newDist;
                        priorityQueue.Add(new Tuple<int, string>(newDist, edge.To));
                    }
                }
            }

            // Check if we could find the destination node
            if (distances[endNode] == int.MaxValue)
            {
                throw new ArgumentException("No valid path found.");
            }

            return distances[endNode];  // Return the shortest distance
        }
    }
}
