using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using WebService_Lib.Attributes;
using WebService_Lib.Cards;


namespace WebService_Lib.DB
{
    /// <summary>
    /// <c>Component</c> class that handles message management & storage.
    /// </summary>
    public class Database
    {
        private static string Host = "localhost";
        private static string User = "mtcg";
        private static string DBname = "mtcg";
        private static string Password = "mtcg";
        private static string Port = "5432";
        private static NpgsqlConnection conn;
        public static void setupConnection()
        {
            string connString =
                 String.Format(
                     "Server={0};Username={1};Database={2};Port={3};Password={4}",
                     Host,
                     User,
                     DBname,
                     Port,
                     Password);
            conn = new NpgsqlConnection(connString);
            conn.Open();
        }

        public static void addUser(string username, string password)
        {
            if (conn == null)
                setupConnection();
            using (var command = new NpgsqlCommand("INSERT INTO users (username, password) VALUES (@n1,@q1); INSERT INTO deck (owner,deck) VALUES (@n1,''); INSERT INTO scoreboard (username,rating) VALUES (@n1,100)", conn))
            {
                command.Parameters.AddWithValue("n1", username);
                command.Parameters.AddWithValue("q1", password);
                command.ExecuteNonQuery();
            }
        }

        public static void addBattle(string id, string user1, string user2, string log, string winner)
        {
            if (conn == null)
                setupConnection();
            using (var command = new NpgsqlCommand("INSERT INTO battles (id, user1, user2, log, winner) VALUES (@i1,@u1,@u2,@l1,@w1)", conn))
            {
                command.Parameters.AddWithValue("i1", id);
                command.Parameters.AddWithValue("u1", user1);
                command.Parameters.AddWithValue("u2", user2);
                command.Parameters.AddWithValue("l1", log);
                command.Parameters.AddWithValue("w1", winner);
                command.ExecuteNonQuery();
            }
        }

        public static bool doesUserExist(string username)
        {
            if (conn == null)
                setupConnection();
            using (var command = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username = @n1", conn))
            {
                command.Parameters.AddWithValue("n1", username);
                Int64 count = (Int64)command.ExecuteScalar();
                return count > 0;
            }
        }

        public static bool userAuthenticate(string username, string password)
        {
            if (conn == null)
                setupConnection();
            using (var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @n1 AND password = @p1", conn))
            {
                command.Parameters.AddWithValue("n1", username);
                command.Parameters.AddWithValue("p1", password);
                bool ret = false;
                using (NpgsqlDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        ret = true;
                    }
                }
                return ret;
            }

        }

        public static string userID(string username)
        {
            if (conn == null)
                setupConnection();
            using (var command = new NpgsqlCommand("SELECT id FROM users WHERE username = @n1", conn))
            {
                command.Parameters.AddWithValue("n1", username);
                string id = "";
                using (NpgsqlDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        id = rdr.GetString(0);
                    }
                }
                return id;
            }

        }

        public static Stack getUserStack(string username, List<string> wantedIds = null)
        {
            if (conn == null)
                setupConnection();
            using (var command = new NpgsqlCommand("SELECT id,name,damage FROM cards WHERE owner = @n1", conn))
            {
                command.Parameters.AddWithValue("n1", username);
                Stack stack = new Stack();
                using (NpgsqlDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (wantedIds == null)
                        {
                            Card c = new Card(rdr.GetString(0), rdr.GetString(1), rdr.GetDouble(2));
                            stack.addCard(c);
                        }
                        else
                        {
                            string id = rdr.GetString(0);
                            if (wantedIds.Contains(id))
                            {
                                Card c = new Card(id, rdr.GetString(1), rdr.GetDouble(2));
                                stack.addCard(c);
                            }
                        }
                    }
                }
                return stack;
            }
        }

        public static Dictionary<string, object> getUserProfile(string username)
        {
            if (conn == null)
                setupConnection();
            using (var command = new NpgsqlCommand("SELECT username,name,bio,image FROM users WHERE username = @n1", conn))
            {
                command.Parameters.AddWithValue("n1", username);
                Dictionary<string, object> profile = new Dictionary<string, object>();
                using (NpgsqlDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        profile["username"] = rdr.GetString(0);
                        profile["name"] = rdr.GetString(1);
                        profile["bio"] = rdr.GetString(2);
                        profile["image"] = rdr.GetString(3);
                    }
                }
                return profile;
            }

        }

        public static Dictionary<string, int> getScoreboard()
        {
            if (conn == null)
                setupConnection();
            Dictionary<string, int> board = new Dictionary<string, int>();
            using (var command = new NpgsqlCommand("SELECT username,rating FROM scoreboard ORDER BY rating DESC", conn))
            {
                using (NpgsqlDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        board[rdr.GetString(0)] = rdr.GetInt32(1);
                    }
                }
                return board;
            }

        }

        public static Dictionary<string, object> getUserStats(string username)
        {
            if (conn == null)
                setupConnection();
            Dictionary<string, object> stats = new Dictionary<string, object>();
            using (var command = new NpgsqlCommand("SELECT rating FROM scoreboard WHERE username = @n1", conn))
            {
                command.Parameters.AddWithValue("n1", username);
                using (NpgsqlDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        stats["elo"] = rdr.GetInt32(0);
                    }
                }
            }
            using (var command = new NpgsqlCommand("SELECT winner FROM battles WHERE user1 = @n1 OR user2 = @n1", conn))
            {
                command.Parameters.AddWithValue("n1", username);
                int totalGames = 0;
                int wonGames = 0;
                using (NpgsqlDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        totalGames++;
                        if (rdr.GetString(0) == username)
                            wonGames++;
                    }
                }
                stats["totalBattles"] = totalGames;
                stats["wonBattles"] = wonGames;
                if (totalGames > 0)
                    stats["winrate"] = Math.Round((double)(wonGames / totalGames * 100), 2);
                else
                    stats["winrate"] = "N/A";
                return stats;
            }

        }

        public static void setUserProfile(string username, string name, string bio, string image)
        {
            if (conn == null)
                setupConnection();
            using (var command = new NpgsqlCommand("UPDATE users SET name = @n1, bio = @b1, image = @i1 WHERE username = @u1", conn))
            {
                command.Parameters.AddWithValue("u1", username);
                command.Parameters.AddWithValue("n1", name);
                command.Parameters.AddWithValue("b1", bio);
                command.Parameters.AddWithValue("i1", image);
                command.ExecuteNonQuery();
            }
        }

        public static Stack getUserDeck(string username)
        {
            if (conn == null)
                setupConnection();
            using (var command = new NpgsqlCommand("SELECT deck FROM deck WHERE owner = @n1", conn))
            {
                command.Parameters.AddWithValue("n1", username);
                List<string> wantedIds = new List<string>();
                using (NpgsqlDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string data = rdr.GetString(0);
                        wantedIds = new List<string>(data.Split(","));
                    }
                }
                return Database.getUserStack(username, wantedIds);
            }
        }

        public static string getCardOwner(string cardId)
        {
            if (conn == null)
                setupConnection();
            using (var command = new NpgsqlCommand("SELECT owner FROM cards WHERE id = @n1", conn))
            {
                command.Parameters.AddWithValue("n1", cardId);
                string id = "";
                using (NpgsqlDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        id = rdr.GetString(0);
                    }
                }
                return id;
            }

        }

        public static void changeElo(string username, int amount)
        {
            if (conn == null)
                setupConnection();
            using (var command2 = new NpgsqlCommand("UPDATE scoreboard SET rating = rating + @a1 WHERE username = @u1", conn))
            {
                command2.Parameters.AddWithValue("u1", username);
                command2.Parameters.AddWithValue("a1", amount);
                command2.ExecuteNonQuery();
            }

        }

        public static void setCardOwner(string cardId, string username)
        {
            if (conn == null)
                setupConnection();
            using (var command2 = new NpgsqlCommand("UPDATE cards SET owner = @u1 WHERE id = @i1", conn))
            {
                command2.Parameters.AddWithValue("u1", username);
                command2.Parameters.AddWithValue("i1", cardId);
                command2.ExecuteNonQuery();
            }

        }

        public static bool setUserDeck(string username, Dictionary<string, object> cards)
        {
            List<string> ids = JArray.FromObject(cards["array"]).ToObject<List<string>>();
            if (ids.Count != 4)
                return false;
            foreach (string id in ids)
            {
                if (Database.getCardOwner(id) != username)
                    return false;
            }
            using (var command2 = new NpgsqlCommand("UPDATE deck SET deck = @d1 WHERE owner = @v1", conn))
            {
                command2.Parameters.AddWithValue("v1", username);
                command2.Parameters.AddWithValue("d1", String.Join(",", ids.ToArray()));
                command2.ExecuteNonQuery();
            }
            return true;
        }

        public static void savePackage(Dictionary<string, object> payload)
        {
            string json = JsonConvert.SerializeObject(payload);
            if (conn == null)
                setupConnection();
            using (var command = new NpgsqlCommand("INSERT INTO packages (data, id) VALUES (@v1, @i1)", conn))
            {
                string id = Guid.NewGuid().ToString();
                command.Parameters.AddWithValue("v1", json);
                command.Parameters.AddWithValue("i1", id);
                command.ExecuteNonQuery();
            }
        }

        public static string buyPackage(string username)
        {
            if (conn == null)
                setupConnection();
            using (var command = new NpgsqlCommand("SELECT balance FROM users WHERE username = @n1", conn))
            {
                command.Parameters.AddWithValue("n1", username);
                double count = (double)command.ExecuteScalar();
                if (count < 5.0)
                    return "no_money";
            }
            using (var command = new NpgsqlCommand("SELECT id,data FROM packages WHERE boughtby IS NULL ORDER BY random() LIMIT 1", conn))
            {
                string packageId = null;
                string packageData = null;
                using (NpgsqlDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        packageId = rdr.GetString(0);
                        packageData = rdr.GetString(1);
                    }
                    if (packageId == null)
                        return "no_packages";
                }
                using (var command2 = new NpgsqlCommand("UPDATE users SET balance = balance - 5 WHERE username = @v1; UPDATE packages SET boughtby = @v1 WHERE id = @i1", conn))
                {
                    command2.Parameters.AddWithValue("v1", username);
                    command2.Parameters.AddWithValue("i1", packageId);
                    command2.ExecuteNonQuery();
                }
                Root r = JsonConvert.DeserializeObject<Root>(packageData);
                List<Array> list = r.array;
                foreach (Array card in list)
                {
                    using (var command3 = new NpgsqlCommand("INSERT INTO cards (id,name,damage,owner) VALUES (@i1,@n1,@d1,@o1)", conn))
                    {
                        command3.Parameters.AddWithValue("i1", card.Id);
                        command3.Parameters.AddWithValue("n1", card.Name);
                        command3.Parameters.AddWithValue("d1", card.Damage);
                        command3.Parameters.AddWithValue("o1", username);
                        command3.ExecuteNonQuery();
                    }
                }
                return "bought";
            }
        }
    }
    public class Array
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Damage { get; set; }
    }

    public class Root
    {
        public List<Array> array { get; set; }
    }
}