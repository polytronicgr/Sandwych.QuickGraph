using System;
using System.Collections.Generic;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Serialization;
using Xunit;


namespace QuickGraph.Algorithms.ShortestPath
{
    public class DijkstraShortestPathAlgorithmTest
    {
        [Fact]
        public void Repro12359()
        {
            var g = TestGraphFactory.LoadGraph("graphml/repro12359.graphml");
            int i = 0;
            foreach (var v in g.Vertices)
            {
                if (i++ > 5) break;
                Dijkstra(g, v);
            }
        }

        [Fact]
        public void DijkstraAll()
        {
            System.Threading.Tasks.Parallel.ForEach(TestGraphFactory.GetAdjacencyGraphs(), g =>
                {
                    foreach (var root in g.Vertices)
                        this.Dijkstra(g, root);
                });
        }

        private void Dijkstra<TVertex, TEdge>(IVertexAndEdgeListGraph<TVertex, TEdge> g, TVertex root)
            where TEdge : IEdge<TVertex>
        {
            var distances = new Dictionary<TEdge, double>(g.EdgeCount);
            foreach (var e in g.Edges)
                distances[e] = g.OutDegree(e.Source) + 1;

            var algo = new DijkstraShortestPathAlgorithm<TVertex, TEdge>(
                g,
                e => distances[e]
                );
            var predecessors = new VertexPredecessorRecorderObserver<TVertex, TEdge>();
            using (predecessors.Attach(algo))
                algo.Compute(root);

            Verify(algo, predecessors);
        }

        private static void Verify<TVertex, TEdge>(
            DijkstraShortestPathAlgorithm<TVertex, TEdge> algo,
            VertexPredecessorRecorderObserver<TVertex, TEdge> predecessors
            )
            where TEdge : IEdge<TVertex>
        {
            // let's verify the result
            foreach (var v in algo.VisitedGraph.Vertices)
            {
                TEdge predecessor;
                if (!predecessors.VertexPredecessors.TryGetValue(v, out predecessor))
                    continue;
                if (predecessor.Source.Equals(v))
                    continue;
                double vd, vp;
                bool found;
                Assert.Equal(
                    found = algo.TryGetDistance(v, out vd),
                    algo.TryGetDistance(predecessor.Source, out vp)
                    );
            }
        }
    }
}
