using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    /// <summary>
    /// The Union Find data structure to maintain disjoint sets.
    /// To be used with clustering.
    /// </summary>
    public class UnionFind
    {
        private Dictionary<int, int> _dictVertexLeader; // Maintains vertices with respective leader.
        private Dictionary<int, int> _dictLeaderSize;   // Maintains a leader's component's size.

        private UnionFind()
        {
            _dictVertexLeader = new Dictionary<int, int>();
            _dictLeaderSize = new Dictionary<int, int>();
        }

        public UnionFind(List<int> vertices)
            : this()
        {
            InitializeComponents(vertices);
        }

        /// <summary>
        /// This constructor initializes the UF by 
        /// intrapolating the sequence between 0 and maxVertex (both inclusive).
        /// </summary>
        /// <param name="maxVertex"></param>
        public UnionFind(int maxVertex)
            : this()
        {
            List<int> vertices = new List<int>();
            for (int i = 0; i <= maxVertex; i++)
            {
                vertices.Add(i);
            }
            InitializeComponents(vertices);
        }



        public int Find(int vertex)
        {
            return _dictVertexLeader[vertex];
        }

        /// <summary>
        /// Adds to Union in O(n).
        /// Max leader pointer changes = n/2
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        public void Union(int u, int v)
        {
            // Compare leaders' size and fuse smaller with greater.
            int uLeader = _dictVertexLeader[u];
            int vLeader = _dictVertexLeader[v];
            if (_dictLeaderSize[uLeader] <= _dictLeaderSize[vLeader])
            {
                // Bring component of 'u' under component of 'v'
                Unite(uLeader, vLeader);
            }
            else
            {
                // Vice-versa
                Unite(vLeader, uLeader);
            }
        }

        /// <summary>
        /// Returns current number of components in the DS.
        /// (Number of leaders = Number of components)
        /// </summary>
        public int Components
        {
            get
            {
                return _dictLeaderSize.Count;
            }
        }

        private void InitializeComponents(List<int> vertices)
        {
            foreach (int vertex in vertices)
            {
                // Each vertex being its own leader with a size of 1
                _dictLeaderSize.Add(vertex, 1);
                _dictVertexLeader.Add(vertex, vertex);
            }
        }

        private void Unite(int oldLeader, int newLeader)    // O(m)
        {
            // Find the followers of the old leader(included) and join them to new leader. O(m)
            List<int> oldLeaderFollowers = _dictVertexLeader.Where(vl => vl.Value == oldLeader).Select(vl => vl.Key).ToList();

            foreach (int oldLeaderFollower in oldLeaderFollowers)   // O(m)
            {
                // Set new leader
                _dictVertexLeader[oldLeaderFollower] = newLeader;
            }
            // Add old leader too under the new leader
            _dictVertexLeader[oldLeader] = newLeader;
            UpdateLeaderSize(oldLeader, newLeader);
        }

        private void UpdateLeaderSize(int oldLeader, int newLeader)
        {
            // Add old leader's size to new leader and remove its entry from leader-size.
            _dictLeaderSize[newLeader] += _dictLeaderSize[oldLeader];   // O(1)
            _dictLeaderSize.Remove(oldLeader);      // O(1)
        }

    }
}
