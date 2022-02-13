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
    public class GameDatabase
    {
        public class KeyValue<T1, T2>
        {
            [XmlElement("Key")]
            public T1 key;
            [XmlElement("Value")]
            public T2 val;
        }

        public class Game
        {
            [XmlIgnore]
            public SortedSet<ulong> players { get; private set; }
            [XmlElement("Capacity")]
            public uint capacity;

            [XmlArray("GameOwners"),XmlArrayItem("PlayerId")]
            public ulong[] serializable_players
            {
                get
                {
                    return players.ToArray();
                }
                set
                {
                    foreach (ulong item in value)
                    {
                        players.Add(item);
                    }
                }
            }

            public Game()
            {
                capacity = 1;
                players = new SortedSet<ulong>();
            }
        }

        public class Player
        {
            [XmlIgnore]
            public SortedSet<string> games { get; private set; }
            [XmlElement("Name")]
            public string name;

            [XmlArray("OwnedGames"), XmlArrayItem("GameName")]
            public string[] serializable_games
            {
                get
                {
                    return games.ToArray();
                }
                set
                {
                    foreach (string item in value)
                    {
                        games.Add(item);
                    }
                }
            }

            public Player()
            {
                name = "DEFAULT_PLAYER_NAME";
                games = new SortedSet<string>();
            }

            public Player(string name)
            {
                this.name = name;
                games = new SortedSet<string>();
            }
        }

        [XmlRoot(ElementName = "Database")]
        public class Database
        {
            [XmlIgnore]
            public SortedDictionary<string, Game> games { get; private set; }
            [XmlIgnore]
            public Dictionary<ulong, Player> players { get; private set; }

            [XmlArray("Games"), XmlArrayItem("Game")]
            public KeyValue<string, Game>[] serializable_games
            {
                get
                {
                    List<KeyValue<string, Game>> temp = new List<KeyValue<string, Game>>();
                    if (games != null)
                    {
                        temp.AddRange(games.Keys.Select(akey => new KeyValue<string, Game>() { key = akey, val = games[akey] }));
                    }
                    return temp.ToArray();
                }
                set
                {
                    foreach (KeyValue<string, Game> item in value)
                    {
                        games.Add(item.key, item.val);
                    }
                }
            }
            [XmlArray("Players"), XmlArrayItem("Player")]
            public KeyValue<ulong, Player>[] serializable_players
            {
                get
                {
                    List<KeyValue<ulong, Player>> temp = new List<KeyValue<ulong, Player>>();
                    if (games != null)
                    {
                        temp.AddRange(players.Keys.Select(akey => new KeyValue<ulong, Player>() { key = akey, val = players[akey] }));
                    }
                    return temp.ToArray();
                }
                set
                {
                    foreach (KeyValue<ulong, Player> item in value)
                    {
                        players.Add(item.key, item.val);
                    }
                }
            }

            public Database()
            {
                games = new SortedDictionary<string, Game>();
                players = new Dictionary<ulong, Player>();
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
            ulong[] owners = db.games[game].players.ToArray();
            foreach (ulong id in owners)
            {
                db.players[id].games.Remove(game);
            }
            db.games.Remove(game);
        }

        public bool gameExists(string game)
        {
            return db.games.ContainsKey(game);
        }

        public void addPlayer(ulong id, string name)
        {
            db.players.Add(id, new Player(name));
        }

        public void removePlayer(ulong id)
        {
            string[] owned = db.players[id].games.ToArray();
            foreach (string game in owned)
            {
                db.games[game].players.Remove(id);
            }
            db.players.Remove(id);
        }

        public bool playerExists(ulong id)
        {
            return db.players.ContainsKey(id);
        }

        public bool playerHasGames(ulong id)
        {
            return !(db.players[id].games.Count == 0);
        }

        public void buyGame(ulong id, string game)
        {
            db.players[id].games.Add(game);
            db.games[game].players.Add(id);
        }

        public void sellGame(ulong id, string game)
        {
            db.players[id].games.Remove(game);
            db.games[game].players.Remove(id);
        }

        public bool hasGame(ulong id, string game)
        {
            return db.players[id].games.Contains(game);
        }

        public string[] listGames()
        {
            return db.games.Keys.ToArray();
        }

        public string[] listGames(ulong id)
        {
            return db.players[id].games.ToArray();
        }

        public string[] intersect(ulong[] ids)
        {
            SortedSet<string> games = new SortedSet<string>(db.players[ids[0]].games);
            bool first = true;
            foreach(ulong id in ids)
            {
                if(first)
                {
                    first = false;
                    continue;
                } 
                else
                {
                    games.IntersectWith(db.players[id].games);
                }
            }
            return games.ToArray();
        }
        
        public void save()
        {
            if(db == null)
            {
                Console.Error.WriteLine("Could not save database, value is null.");
                return;
            }
            IO.saveXML(filename, db);
        }

        public void load()
        {
            db = IO.readXML(filename, new Database(), "Creating a new database.");
        }
    }
}
