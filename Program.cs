using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowTest
{
    class Program
    {
        private static Graph graph = new Graph();
        static void Main()
        {
            SimpleGraph(3);
            Run();
        }
        private static void SimpleGraph(int nodeCount)
        {
            //simple chain like graph
            for (int i = 0; i < nodeCount; i++)
            {
                if(i==0) 
                    graph.Nodes.Add(new Source());

                else if(i== nodeCount-1)
                    graph.Nodes.Add(new Sink());

                else
                    graph.Nodes.Add(new Node());

                if (i > 0)
                    graph.Links.Add(new Link() { End = graph.Nodes[i], Start = graph.Nodes[i - 1] });
            }
        }
        private static void Run()
        {
            int step = 0;
            
            while (step < 100)
            {
                for (int i = 0; i < graph.Nodes.Count; i++)
                    Transistion(graph.Nodes[i]);
                
                Console.WriteLine(String.Format("Status at step = {0}", step));
                for (int i = 0; i < graph.Nodes.Count; i++)
                    Console.WriteLine("node " + i + ": " + graph.Nodes[i].Status());

                step++;
            }
        }
        private static void Transistion(INode node)
        {
            //supply node only makes more availble at flow rate
            if (node is Node || node is Sink)
            {
                //get supply nodes
                IEnumerable<INode> suppliers = graph.Links.Where(x => x.End.Equals(node)).Select(x => x.Start);
                //remaining capacity on this node
                double space = node.Capacity - (node.Incoming + node.Outgoing);
                //available to join node
                double available = 0;

                foreach (INode n in suppliers)
                    available += n.Outgoing;

                double incomingNew = Math.Min(space, available);
                // add the incoming
                node.Incoming += incomingNew;
                // remove from suppliers by equal proportions
                foreach (INode n in suppliers)
                    n.Outgoing -= incomingNew / suppliers.Count();
            }
            
            node.Outgoing += node.FlowRate;
            node.Incoming -= node.FlowRate;
        }
        
    }
    public interface INode
    {
        Guid Id { get; set; }
        double Incoming { get; set; }
        double Outgoing { get; set; }
        double Capacity { get; set; }
        double FlowRate { get; set; }
        string Status();
    }
    public class Sink : INode
    {
        //ignore capacity on exit nodes
        public Guid Id { get; set; } = Guid.NewGuid();
        public double Incoming { get; set; } = 0.0;
        public double Outgoing { get; set; } = 0.0;
        public double Capacity { get; set; } = double.MaxValue;
        public double FlowRate { get; set; } = 12.5;
        public string Status()
        {
            return String.Format("Exited: {0}", Outgoing);
        }
    }
    public class Source : INode
    {
        //make large supply at source and ignore capacity
        public Guid Id { get; set; } = Guid.NewGuid();
        public double Incoming { get; set; } = 10000;
        public double Outgoing { get; set; } = 0.0;
        public double Capacity { get; set; } = 0;
        public double FlowRate { get; set; } = 20;

        public string Status()
        {
            return String.Format("Entered: {0}", 10000 - Incoming);
        }
    }
    public class Node : INode
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public double Incoming { get; set; } = 0.0;
        public double Outgoing { get; set; } = 0.0;
        public double Capacity { get; set; } = 45;
        public double FlowRate { get; set; } = 12.5;

        public string Status()
        {
            return String.Format("Incoming: {0}, Outgoing: {1}, Total: {2}", Incoming, Outgoing, Incoming + Outgoing);
        }
    }
    public class Link
    {
        public INode Start { get; set; }
        public INode End { get; set; }
    }
    public class Graph
    {
        public List<INode> Nodes { get; set; } = new List<INode>();
        public List<Link> Links{ get; set; } = new List<Link>();
    }
}
