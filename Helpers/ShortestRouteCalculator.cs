using MapAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapAPI.Helpers
{
    public class ShortestRouteCalculator
    {
        public static string FindShortestRoute(Graph graph, string startNode, string endNode)
        {
            // Handle the case where the nodes do not exist
            if (!graph.Nodes.ContainsKey(startNode) || !graph.Nodes.ContainsKey(endNode))
            {
                throw new ArgumentException($"One or both of the nodes '{startNode}' and '{endNode}' do not exist in the graph.");
            }

            var distances = new Dictionary<string, int>();
            var previousNodes = new Dictionary<string, string>();
            var nodes = new List<string>(graph.Nodes.Keys);

            foreach (var node in nodes)
            {
                distances[node] = int.MaxValue;
                previousNodes[node] = null;
            }
            distances[startNode] = 0;

            var priorityQueue = new SortedSet<Tuple<int, string>>();
            priorityQueue.Add(new Tuple<int, string>(0, startNode));

            while (priorityQueue.Count > 0)
            {
                var currentNode = priorityQueue.Min.Item2;
                priorityQueue.Remove(priorityQueue.Min);

                // Log current node being processed
                Console.WriteLine($"Processing node: {currentNode}");

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
                        previousNodes[edge.To] = currentNode;
                        priorityQueue.Add(new Tuple<int, string>(newDist, edge.To));
                    }
                }
            }

            // Check if we reached the destination node, otherwise throw an error
            if (previousNodes[endNode] == null)
            {
                throw new ArgumentException("No valid path found.");
            }

            var path = new Stack<string>();
            var current = endNode;
            while (current != null)
            {
                path.Push(current);
                current = previousNodes[current];
            }

            return string.Join("", path);  // This will return the path like "ABCD"
        }
    }
}
