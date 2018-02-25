using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tetris
{
    public  class SingleKeyBinding      
    {
        private Key key;
        private PlayerSettings settings;
        public Action Command { get; internal set; }  
        public Key Key
        {
            get => key;
            internal set
            {
                if (settings.LeftKey == key)
                    settings.LeftKey = value;
                else if (settings.RightKey == key)              
                    settings.RightKey = value;
                else if (settings.DropKey == key)
                    settings.DropKey = value;
                else if (settings.RotateKey == key)
                    settings.RotateKey = value;
                key = value;
            }
        }
        
        public SingleKeyBinding(Key key, Action action, PlayerSettings settings)
        {
            this.key = key;
            this.Command = action;
            this.settings = settings;
        }
    }
    public class MultiKeyBinding
    {
        public List<Key> Keys { get; private set; }
        public Action Command { get; private set; }
        public MultiKeyBinding(List<Key> keys, Action command)
        {
            Keys = keys;
            Command = command;
        }
    }


}

