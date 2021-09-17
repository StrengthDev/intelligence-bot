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
                    if (value != null)
                    {
                        foreach (ulong item in value)
                        {
                            players.Add(item);
                        }
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
                    if (value != null)
                    {
                        foreach (string item in value)
                        {
                            games.Add(item);
                        }
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
                    if (value != null)
                    {
                        foreach (KeyValue<ulong, Player> item in value)
                        {
                            players.Add(item.key, item.val);
                        }
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
            //TODO
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
            //TODO
        }

        public bool playerExists(ulong id)
        {
            return db.players.ContainsKey(id);
        }

        public bool playerHasGames(ulong id)
        {
            db.players.TryGetValue(id, out Player user);
            return !(user.games.Count == 0);
        }

        public void buyGame(ulong id, string game)
        {
            db.players.TryGetValue(id, out Player user);
            user.games.Add(game);
            db.games.TryGetValue(game, out Game go);
            go.players.Add(id);
        }

        public void sellGame(ulong id, string game)
        {
            db.players.TryGetValue(id, out Player user);
            user.games.Remove(game);
            db.games.TryGetValue(game, out Game go);
            go.players.Remove(id);
        }

        public bool hasGame(ulong id, string game)
        {
            db.players.TryGetValue(id, out Player user);
            return user.games.Contains(game);
        }

        public string[] listGames()
        {
            return db.games.Keys.ToArray();
        }

        public string[] listGames(ulong id)
        {
            db.players.TryGetValue(id, out Player user);
            return user.games.ToArray();
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
