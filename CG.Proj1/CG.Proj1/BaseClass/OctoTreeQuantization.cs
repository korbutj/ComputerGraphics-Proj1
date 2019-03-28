using System.Collections.Generic;
using System.Linq;

namespace CG.Proj1.BaseClass
{
    public class OctoTreeQuantization
    {
        public OctoNode RootNode { get; set; }
        
        
        public OctoTreeQuantization()
        {
            RootNode = new OctoNode(0);

            
        }
    }
}