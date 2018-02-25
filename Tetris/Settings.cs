using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
namespace Tetris
{
    
    public class PlayerSettings {
        public Key LeftKey { get; set; }
        public Key RightKey { get; set; }
        public Key RotateKey { get; set; }
        public Key DropKey { get; set; }
        public Pallete.Scheme scheme;
        public String ID;

        public PlayerSettings(){}
        public PlayerSettings(Key leftKey, Key rightKey, Key rotateKey, Key dropKey, Pallete.Scheme scheme, string iD)
        {
            LeftKey = leftKey;
            RightKey = rightKey;
            RotateKey = rotateKey;
            DropKey = dropKey;
            this.scheme = scheme;
            ID = iD;
        }
    };
    
    [Serializable]
    [XmlRoot]
    [XmlInclude(typeof(PlayerSettings))]
    public class GameSettings
    {
        public readonly int rows = 20;
        public readonly int collumns = 10;
        public bool ShowGridLines = false;
        public bool showDropPreview = true;
        public bool flipped = false;
        public bool multiplayerEnabled = false;
        public bool soundEnabled = true;
        public bool controlsReversed = false;
        public float volume = 0.3f;
        public Uri MusicUri => new Uri(uriPath, urikind);
        public UriKind urikind = UriKind.Relative;

#if DEBUG
        public string uriPath = @"../../Sound/Music.wav";
#else
        public string uriPath =  @"Sound/Music.wav";
#endif
        

        public PlayerSettings player1Settings = new PlayerSettings(Key.A, Key.D, Key.W, Key.S, Pallete.Scheme.Solid, "Player1");
        public PlayerSettings player2Settings = new PlayerSettings(Key.Left, Key.Right, Key.Up, Key.Down, Pallete.Scheme.Solid, "Player2");

        private static string filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/Tetris/Settings.xml";

        public void Save()
        {
            try
            {
                if(!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/Tetris"))
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create) + @"/Tetris");
                if(File.Exists(filePath))
                    File.WriteAllText(filePath, String.Empty);        //bacause without this a '>' may somehow be left at the end of the new file, making an error when the next load attempt is made

                using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    XmlSerializer xmls = new XmlSerializer(typeof(GameSettings));
                    xmls.Serialize(stream, this);
                }
            }
            catch (Exception e)
            {
#if DEBUG
                MessageBox.Show(e.Message);
#endif
            }
        }
        public static GameSettings Load()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        XmlSerializer xmls = new XmlSerializer(typeof(GameSettings));
                        return xmls.Deserialize(stream) as GameSettings;
                    }
                }
                catch (Exception e)
                {
#if DEBUG 
                    MessageBox.Show(e.Message);
#endif
                    return new GameSettings();
                }
            }
            return new GameSettings();
        }
    }
}

