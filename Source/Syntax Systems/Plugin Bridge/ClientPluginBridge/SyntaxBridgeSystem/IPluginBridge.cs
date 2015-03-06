using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SyntaxBridgeSystem
{
    public interface IBridge
    {
        void InitializePlugin();
        
    }

    abstract public class IPluginBridge : IBridge
    {
        public IPluginBridge()
        {
            init();
        }

        private void init()
        {

        }

        virtual public void InitializePlugin() { }
    }
    
}
