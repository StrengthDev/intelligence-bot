using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace intelligence_bot
{
    class GameDatabase
    {
        [Serializable]
        internal class Game
        {
            public HashSet<string> players { get; private set; }

            public Game()
            {
                players = new HashSet<string>();
            }
        }

        [Serializable]
        internal class Player
        {
            public SortedSet<string> games { get; private set; }
            public string name;

            public Player(string name)
            {
                this.name = name;
                games = new SortedSet<string>();
            }
        }

        [Serializable]
        private class Database
        {
            public SortedDictionary<string, Game> games;
            public Dictionary<string, Player> players;

            public Database()
            {
                games = new SortedDictionary<string, Game>();
                players = new Dictionary<string, Player>();
            }
        }

        public string filename { get; private set; }
        private Database db;

        public GameDatabase(string filename)
        {
            this.filename = filename;
        }

        public void addGame(string game)
        {
            db.games.Add(game, new Game());
        }

        public void removeGame(string game)
        {
            //TODO
        }

        public bool gameExists(string game)
        {
            return db.games.ContainsKey(game);
        }

        public void addPlayer(string id, string name)
        {
            db.players.Add(id, new Player(name));
        }

        public void removePlayer(string id)
        {
            //TODO
        }

        public bool playerExists(string id)
        {
            return db.players.ContainsKey(id);
        }

        public void buyGame(string id, string game)
        {
            db.players.TryGetValue(id, out Player user);
            user.games.Add(game);
            db.games.TryGetValue(id, out Game go);
            go.players.Add(id);
        }

        public void sellGame(string id, string game)
        {
            db.players.TryGetValue(id, out Player user);
            user.games.Remove(game);
            db.games.TryGetValue(id, out Game go);
            go.players.Remove(id);
        }

        public bool hasGame(string id, string game)
        {
            db.players.TryGetValue(id, out Player user);
            return user.games.Contains(game);
        }

        public string[] listGames()
        {
            return db.games.Keys.ToArray();
        }

        public string[] listGames(string id)
        {
            db.players.TryGetValue(id, out Player user);
            return user.games.ToArray();
        }
        //TODO: use IO file isntead of loading files here
        public void save()
        {
            if(db == null)
            {
                return;
            }

            try
            {
                XmlDocument xml = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(db.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, db);
                    stream.Position = 0;
                    xml.Load(stream);
                    xml.Save(filename);
                }
            } catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void load()
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(filename);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type type = typeof(Database);

                    XmlSerializer serializer = new XmlSerializer(type);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        db = (Database)serializer.Deserialize(reader);
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine("Creating new game database.");
                db = new Database();
            }
        }
    }
}
