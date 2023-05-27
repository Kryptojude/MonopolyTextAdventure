using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MonopolyTextAdventure
{
    class Program
    {
        public class Ereignis
        {
            public string text;
            public string action;
            public int cash;
            public int tile;
            public bool losMoney = true;
        }

        public class Tile
        {
            public static int[] houseMultiplier = new int[] { 1, 5, 15, 45, 80, 125 };
            public static int[] trainMultiplier = new int[] { 0, 1, 2, 4, 8 };
            public static int[] factoryMultiplier = new int[] { 0, 4, 10 };

            public int number;
            public string name;
            public ConsoleColor color; //white = no color
            public int autoMoney;
            public int price;
            public int rent;
            public int houses = 0; //5 houses = hotel
            public Player owner;
            public List<Player> onTop = new List<Player>();
            public int samecolor = 0;
            public int housePrice;
            public string mapChar;
            public bool mortgage = false;
        }
        static Tile[] tiles;
        static int eyes;
        static Random random = new Random();



        public class Player
        {
            public int number;
            private Tile position = tiles[0];
            public Tile Position
            {
                get { return position; }
                set
                {
                    value.onTop.Add(this);
                    position = value;
                }
            }
            private int money = 1500;
            public int Money
            {
                get { return money; }
                set
                {
                    money = value;
                    if (money <= -1500)
                    {
                        Console.WriteLine("Geld verzockt, geh zurück in die Villa");
                        gameOver = true;
                    }
                }
            }

            public List<Tile> cards = new List<Tile>();
            public bool isInPrison;
            public int jailCards = 0;
            public int jailTries = 0;
            public int paschCount = 0;
            public bool gameOver = false;
            public int stationCards = 0;
            public int factoryCards = 0;
        }
        static Player[] players;
        static Player currentPlayer;
        static string[] names = new string[] {"Henrietta", "Natalie", "Chantal", "Joline", "Shirley Ann", "Ewald", "Tristan", "Bernd", "Fernando", "UwU"};
        static string Name
        {
            get { return names[random.Next(0, names.Length)]; }
        }
        static Ereignis[] ereigniskarten = new Ereignis[]
        {
            new Ereignis{text = "Marschiere bis zur Seestraße, Wenn du über Los gehst, bekommst du 200€", tile = 11, action = "move"},
            new Ereignis{text ="Du hast bei Fortnite gewonnen, du erhälst 100€", cash = 100, action = "autoMoney"},
            new Ereignis{text ="Du bekommst HartzIV, das Amt zahlt dir 150€", cash = 150, action = "autoMoney"},
            new Ereignis{text ="Dein Hintern verdient 50€", cash = 50, action = "autoMoney"},
            new Ereignis{text ="Du bist besoffen zur Arbeit gekommen, zahle 20€", cash = -20, action = "autoMoney"},
            new Ereignis{text ="Du bist mit 100Km/h durchs Dorf gefahren und du hast den Hund von deinem Nachbarn überfahren zahle 15€" , cash = -15, action = "autoMoney"},
            new Ereignis{text ="Gehe bis zum Los", tile = 0, action = "move"},
            new Ereignis{text ="Gehe auf die Schlossallee" , tile = 39, action = "move"},
            new Ereignis{text ="Gehe zum Opernplatz, wenn du über LOS gehst bekommst du 200€", tile = 24, action = "move"},
            new Ereignis{text ="Du kommst aus dem Gefängnis frei", action = "jailCard"},
            new Ereignis{text ="Du gehst direkt in den Knast, du gehst nicht über Los und du bekommst keine 200€", action = "goToJail"},
            new Ereignis{text ="du steigst in das falsche Taxi ein, gehe 3 Felder zurück.", tile = 3, action = "moveBack"},
            new Ereignis{text =$"Tsunami {Name} überschwemmt deine Bezirke, zahle für jedes Haus 40€ und für jedes Hotel 160€", cash = 40, action = "moneyForEachHouse"},
            new Ereignis{text =$"Hurricane {Name} trifft dich, zahle für jedes Haus 25€ und für jedes Hotel 100€", cash = 25, action = "moneyForEachHouse"},
            new Ereignis{text ="Hallo, ich bin ein Unterstützer des tombusianischen Generals Mahmoud Abdallah II. Leider haben wir nicht das Geld, um unsere Diamanten nach Europa zu schicken. Wenn Sie wirklich mithelfen, uns 500 € zu leihen, dann verspreche ich, dass wir Sie mindestens zweimal bezahlen. Möchtest du Tombusien unterstützen? (ja/nein)", action = "prince"},
            new Ereignis{text =" ゴジラ!!!! er wird alles zerstören, dass ihm im Weg steht"}
        };
        static Ereignis[] gemeinschaftskarten = new Ereignis[]
        {
            new Ereignis{text = "Du überfällst das Altersheim, du bekommst von jedem Spieler 100€", cash=100, action = "moneyForEachPlayer"},
            new Ereignis{text = "Du bekommst 5€ Schmerzensgeld", cash=5, action = "autoMoney"},
            new Ereignis{text = "Einkommenssteuerrückzahlung, du bekommst 20€", cash=20, action="autoMoney"},
            new Ereignis{text = "Du Loser wurdest Zweiter im Loserwettbewerb, weil du so ein Loser bist, deswegen bekommst du Loser 5€... du Loser", cash=5, action="autoMoney"},
            new Ereignis{text = "latsche bis auf Los!", tile = 0, action ="move"},
            new Ereignis{text = "Du bläst den Gefängnisdirektor, du kommst aus dem Gefängnis frei", action="jailCard"},
            new Ereignis{text = "deine verstorbene Großtante mütterlicherseits die dich schon immer kannte, aber für dich war sie eigentlich immer eine betrunkene Frau die auf deinem Geburtstag ins Aquarium gekotzt hat, hat dir 200€ vererbt. Cheers.", cash=200, action="autoMoney"},
            new Ereignis{text = "Ding Dong! hier kommt dein Behindertengeld, 100€ für dich", cash=100, action="autoMoney"},
            new Ereignis{text = "Dein Bitcoin investment zahlt sich aus, hier sind 3€", cash=3, action="autoMoney"},
            new Ereignis{text = "du torkelst zur Badstraße", tile = 1, losMoney = false, action="move"}
        };

        static void Main(string[] args)
        {
            GenerateTiles(); //Fills tile 
            Console.OutputEncoding = Encoding.Unicode;

            int numberOfPlayers = 0;

            Console.WriteLine("Monopoly Text Adventure!");
            Console.WriteLine("Wie viele Kloppis machen mit?");
            while (true)
            {
                bool buffer = int.TryParse(Console.ReadLine(), out numberOfPlayers);
                if (!buffer)
                    Console.WriteLine("Das war keine Zahl du Mongoloid");
                else
                    break;
            }
            Console.WriteLine("\n" + numberOfPlayers + " Spieler spielen mit! \n");
            Console.WriteLine("Spieler 1 ist dran");
            //Spieler erstellen
            players = new Player[numberOfPlayers];
            for (int i = 0; i < numberOfPlayers; i++)
            {
                players[i] = new Player { number = i };
                tiles[0].onTop.Add(players[i]);
            }
            currentPlayer = players[0];

            while (true)
            {
                bool samePlayer = false; //Soll der nächste Spieler nochmal der gleiche sein, weil Pasch?
                //Eingabe
                while (true)
                {
                    if (currentPlayer.isInPrison)
                        Console.WriteLine("Du bist im Knast, du hast 3-Versuche um einen Pasch zu würfeln, oder du bestichst einen Wärter und bezahlst.\nUm die Gefängniskarte zu benutzen, schreibe 'karte', um zu bezahlen schreibe 'blow'");
                    Console.WriteLine("  'würfeln' : Um zu würfeln  \n   'info'   : Um Information zum derzeitigen Stand abzufragen \n   'bauen'  : Um Häuser zu bauen  \n   'print'  : Um dir das Spielbrett anzusehen  \n 'hypothek' : Um eine Hypothek aufzunehmen");
                    string input = Console.ReadLine();
                    if (input == "würfeln")
                    {
                        samePlayer = Dice();
                        if (currentPlayer.isInPrison) //im gefängnis
                        {
                            if (!samePlayer) //Kein pasch im gefängnis
                            {
                                if (currentPlayer.jailTries == 2) //Letzter versuch fehlgeschlagen
                                {
                                    Console.WriteLine("Drei Versuche fehlgeschlagen, du musst 50€ zahlen, aber du bist nicht mehr Knast");
                                    currentPlayer.isInPrison = false;
                                    currentPlayer.jailTries = 0;
                                    currentPlayer.Money -= 50;
                                    eyes = 0; //Keine Bewegung
                                }
                                else //Noch nicht letzter Versuch fehlgeschlagen
                                {
                                    Console.WriteLine("Leider kein Pasch bro, lass nicht die Seife fallen :p");
                                    eyes = 0; //Keine Bewegung
                                    currentPlayer.jailTries++;
                                }
                            }
                            else //Pasch im Gefängnis
                            {
                                samePlayer = false; //nicht nochmal würfeln
                                currentPlayer.isInPrison = false;
                                Console.WriteLine("Pasch! Du kommst aus dem Gefängnis frei");
                                currentPlayer.jailTries = 0;
                            }
                        }
                        else //Nicht im Gefängnis
                        {
                            if (samePlayer) //pasch
                            {
                                currentPlayer.paschCount++;
                                if (currentPlayer.paschCount == 3)
                                {
                                    Console.WriteLine("CHEATER!!111elf Geh ins Gefängnis");
                                    currentPlayer.paschCount = 0;
                                    currentPlayer.isInPrison = true;
                                    currentPlayer.Position = tiles[10];
                                }
                            }
                            else //kein pasch
                            {
                                currentPlayer.paschCount = 0;
                            }
                        }
                        break;
                    }
                    else if (input == "info")
                    {
                        Console.WriteLine("Du hast " + currentPlayer.Money + "€ auf dem Konto");
                        PrintStreets("info");
                    }
                    else if (input == "print")
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        for (int i = 20; i <= 30; i++)// 20 - 30
                            PrintMap(i);

                        Console.WriteLine();

                        for (int i = 0; i < 9; i++)
                        {
                            PrintMap(19 - i);
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write("                  ");
                            PrintMap(31 + i);
                            Console.Write("\n");
                        }

                        for (int i = 10; i >= 0; i--)// 10 - 0
                            PrintMap(i);

                        Console.WriteLine();
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        
                    }
                    else if (input == "bauen")
                    {
                        if (PrintStreets("bauen"))
                        {
                            Tile tile = null;
                            while (true)
                            {
                                Console.WriteLine("Wenn du auf einem Grundstück bauen willst, schreibe den Namen der Straße");
                                string Name = Console.ReadLine();
                                for (int i = 0; i < currentPlayer.cards.Count; i++)
                                {
                                    if (Name == currentPlayer.cards[i].name)
                                    {
                                        tile = currentPlayer.cards[i];
                                        break;
                                    }

                                }
                                if (tile == null)
                                {
                                    Console.WriteLine("Falsche Eingabe");
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (tile.houses < 5)
                            {
                                ConsoleColor color = tile.color; //Alle Grundstücke der selben Farbe?
                                int numberColor = tile.samecolor;
                                int amount = 0;
                                for (int i = 0; i < currentPlayer.cards.Count; i++)
                                {
                                    if (color == currentPlayer.cards[i].color)
                                    {
                                        amount++;
                                    }

                                }
                                if (amount == numberColor)
                                {
                                    tile.houses++;
                                    if (tile.houses <= 4)
                                        Console.WriteLine("Haus gebaut");
                                    else
                                        Console.WriteLine("Hotel gebaut");

                                    currentPlayer.Money -= tile.housePrice;
                                }
                                else
                                {
                                    Console.WriteLine("Sie besitzen nicht alle Grundstücke der gleichen Farbe");
                                }

                            }
                            else
                            {
                                Console.WriteLine("Du hast schon ein Hotel auf dem Grundstück");
                            }
                        }
                    }
                    else if (input == "hypothek")
                    {
                        PrintStreets("bauen");
                        Tile tile = null;
                        while (true)
                        {
                            Console.WriteLine("Wenn du auf ein Grundstück eine Hypothek aufnehmen willst, schreibe den Namen der Straße");
                            string Name = Console.ReadLine();
                            for (int i = 0; i < currentPlayer.cards.Count; i++)
                            {
                                if (Name == currentPlayer.cards[i].name)
                                {
                                    tile = currentPlayer.cards[i];
                                    break;
                                }

                            }
                            if (tile == null)
                            {
                                Console.WriteLine("Falsche Eingabe");
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (!tile.mortgage)
                        {
                            int mortgageMoney = tile.price/2 + tile.houses * tile.housePrice/2;
                            currentPlayer.Money += mortgageMoney;
                            tile.houses = 0;
                            tile.mortgage = true;
                            Console.WriteLine("Hypothek aufgenommen, du hast " + mortgageMoney + "€ erhalten.");
                        }
                        else
                            Console.WriteLine("Das Grundstück hat bereits eine Hypothek");
                    }
                    else if (input == "blow" && currentPlayer.isInPrison)
                    {
                        Console.WriteLine("Du tust dem Wärter einen \"Gefallen\" und er klaut dir 100€.");
                        currentPlayer.Money -= 100;
                        currentPlayer.isInPrison = false;
                        break;
                    }
                    else if (input == "karte" && currentPlayer.isInPrison)
                    {
                        if (currentPlayer.jailCards > 0)
                        {
                            Console.WriteLine("Du setzt eine Gefängnis-Frei-Karte ein, du kommst aus dem Gefängnis frei.");
                            currentPlayer.jailCards--;
                            currentPlayer.isInPrison = false;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Du besitzt keine Gefängnis-Frei-Karte :'(");
                        }
                    }
                    else
                        Console.WriteLine("Falsche Eingabe");
                    
                }
                //Gehen
                currentPlayer.Position.onTop.Remove(currentPlayer);
                if (currentPlayer.Position.number + eyes <= 39)
                    currentPlayer.Position = tiles[currentPlayer.Position.number + eyes];
                else //Über/auf Los hinweg gegangen
                {
                    currentPlayer.Position = tiles[currentPlayer.Position.number + eyes - 40];
                    if (currentPlayer.Position.number != 0)
                    {
                        Console.WriteLine("Du bist über Los gegangen, du kriegst 200€");
                        currentPlayer.Money += 200;
                    }
                    
                }
                
                Console.Write("Du stehst auf ");
                PrintName(currentPlayer.Position);
                Console.WriteLine();
                //Neues Feld überprüfen
                StepOnProperty();

                currentPlayer.Money += currentPlayer.Position.autoMoney;

                NextPlayer(samePlayer);
            }
        }

        private static bool PrintStreets(string type)
        {
            if (currentPlayer.cards.Count > 0) //Hat Grundstücke
            {
                if (type == "bauen")
                {
                    //wie viele Grundstücke sind bebaubar
                    int bebaubar = 0;
                    foreach (Tile card in currentPlayer.cards)
                        if (card.color != ConsoleColor.DarkCyan && card.color != ConsoleColor.DarkMagenta)
                            bebaubar++;
                    if (bebaubar == 0)
                    {
                        Console.WriteLine("Du besitzt keine verfügbaren Grundstücke du bob");
                        return false;
                    }
                }
                Console.WriteLine("Das sind deine verfügbaren Grundstücke: ");
                foreach (Tile tile in tiles)
                    if (currentPlayer.cards.Contains(tile))
                        if (type == "info" || (tile.color != ConsoleColor.DarkCyan && tile.color != ConsoleColor.DarkMagenta))
                        {
                            PrintName(tile);
                            if ((tile.houses >= 2  && tile.houses <5) || tile.houses == 0)
                                Console.Write(": " + tile.houses + " Häuser");
                            else if (tile.houses == 5)
                                Console.Write(": 1 Hotel");
                            else if (tile.houses == 1)
                                Console.Write(": 1 Haus");

                            if (tile.mortgage)
                                Console.Write(", Hypothek");

                            Console.WriteLine("\n");
                        }
                return true;
            }
            else //Keine Grundstücke
            {
                Console.WriteLine("Du besitzt keine Grundstücke :'(");
                return false;
            }
        }

        static private void GenerateTiles()
        {
			string[] streetNames = new string[40] { "Los", "Badstraße", "Gemeinschaftsfeld", "Turmstraße", "Einkommensteuer", "Südbahnhof", "Chausseestraße", "Ereignisfeld", "Elisenstraße", "Poststraße", "Gefängnis", "Seestraße", "Elektrizitätswerk", "Hafenstraße", "Neue Straße", "Westbahnhof", "Münchner Straße", "Gemeinschaftsfeld", "Wiener Straße", "Berliner Straße", "Frei Parken", "Theaterstraße", "Ereignisfeld", "Museumstraße", "Opernplatz", "Nordbahnhof", "Lessingstraße", "Schillerstraße", "Wasserwerk", "Goethestraße", "Geh Gefängnis", "Rathausplatz", "Hauptstraße", "Gemeinschaftsfeld", "Bahnhofstraße", "Hauptbahnhof", "Ereignisfeld", "Parkstraße", "Zusatzsteuer", "Schlossallee" };
			int[] buyPrices = new int[40] { 0, 60, 0, 60, 0, 200, 100, 0, 100, 120, 0, 140, 150, 140, 160, 200, 180, 0, 180, 200, 0, 220, 0, 220, 240, 200, 260, 260, 150, 280, 0, 300, 300, 0, 320, 200, 0, 350, 0, 400 };
            int[] autoMoney = new int[40] { 400, 0, 0, 0, -200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -100, 0 };
            int[] startRent = new int[40] { 0, 2, 0, 4, 0, 0, 6, 0, 6, 8, 0, 10, 0, 10, 12, 0, 14, 0, 14, 16, 0, 18, 0, 18, 20, 0, 22, 22, 0, 24, 0, 26, 26, 0, 28, 0, 0, 35, 0, 50 };
            ConsoleColor[] colors = new ConsoleColor[40] { ConsoleColor.White ,ConsoleColor.DarkGray ,ConsoleColor.White ,ConsoleColor.DarkGray ,ConsoleColor.White ,ConsoleColor.DarkMagenta ,ConsoleColor.Cyan ,ConsoleColor.White ,ConsoleColor.Cyan ,ConsoleColor.Cyan ,ConsoleColor.White ,ConsoleColor.Magenta ,ConsoleColor.DarkCyan ,ConsoleColor.Magenta ,ConsoleColor.Magenta ,ConsoleColor.DarkMagenta ,ConsoleColor.DarkYellow ,ConsoleColor.White ,ConsoleColor.DarkYellow ,ConsoleColor.DarkYellow ,ConsoleColor.White ,ConsoleColor.Red ,ConsoleColor.White ,ConsoleColor.Red ,ConsoleColor.Red ,ConsoleColor.DarkMagenta, ConsoleColor.Yellow ,ConsoleColor.Yellow ,ConsoleColor.DarkCyan ,ConsoleColor.Yellow ,ConsoleColor.White ,ConsoleColor.Green ,ConsoleColor.Green ,ConsoleColor.White ,ConsoleColor.Green ,ConsoleColor.DarkMagenta ,ConsoleColor.White ,ConsoleColor.Blue ,ConsoleColor.White ,ConsoleColor.Blue };
            int[] samecolor = new int[40] { 0, 2, 0, 2, 0, 0, 3, 0, 3, 3, 0, 3, 0, 3, 3, 0, 3, 0, 3, 3, 0, 3, 0, 3, 3, 0, 3, 3, 0, 3, 0, 3, 3, 0, 3, 0, 0, 2, 0, 2 };
            int[] housePrices = new int[40] { 0, 50, 0, 50, 0, 0, 50, 0, 50, 50, 0, 100, 0, 100, 0, 100, 100, 0, 100, 0, 100, 150, 0, 150, 0, 150, 150, 0, 150, 150, 0, 200, 0, 200, 200, 0, 200, 0, 0, 200,};
            string[] Icon = new string[40] { "", "", "!", "", "$", "B", "", "?", "", "", "", "", "", "", "", "B", "", "!", "", "", "", "", "?", "", "", "B", "", "", "", "", "", "", "", "!", "","B","?","", "$",""};

            tiles = new Tile[40];
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = new Tile {number = i, price = buyPrices[i], name = streetNames[i], rent = startRent[i], color = colors[i], autoMoney = autoMoney[i], samecolor = samecolor[i],  housePrice = housePrices[i], mapChar = Icon[i] };
            }
        }

        static public bool Dice()
        {
            eyes = random.Next(1, 7);
            int temp = random.Next(1, 7);
            if (!currentPlayer.isInPrison)
                Console.WriteLine("Geh " + (eyes + temp) + " Felder nach vorne");

            if (temp == eyes)
            {
                Console.WriteLine("Würfel 1: " + eyes + ", Würfel 2: " + temp);
                eyes += temp;
                if (!currentPlayer.isInPrison)
                Console.WriteLine("PASCH!!! Du kannst nochmal würfeln");
                return true;

            }
            else
            {
                Console.WriteLine("Würfel 1: " + eyes + ", Würfel 2: " + temp);
                eyes += temp;
                return false;
            }
        }

        static private void NextPlayer(bool samePlayer)
        {
            if (!samePlayer)
            {
                if (currentPlayer.number + 1 < players.Length)
                    currentPlayer = players[currentPlayer.number + 1];
                else
                    currentPlayer = players[0];
            }

            Console.WriteLine("\nSpieler " + (currentPlayer.number + 1) + " ist dran");
        }

        private static void CallEvent(Ereignis[] karten)
        {
            int number = random.Next(0, karten.Length);
            Ereignis ereignis = karten[number];
            Console.WriteLine(ereignis.text);

            switch (ereignis.action)
            {
                case "autoMoney":
                    currentPlayer.Money += ereignis.cash;
                    break;
                case "move":
                    if (ereignis.losMoney)
                        if (currentPlayer.Position.number > ereignis.tile && ereignis.tile != 0) //Über los gehen
                            currentPlayer.Money += 200;

                    currentPlayer.Position = tiles[ereignis.tile];
                    StepOnProperty();
                        break;
                case "moveBack":
                    currentPlayer.Position = tiles[currentPlayer.Position.number - ereignis.tile];
                    StepOnProperty();
                    break;
                case "goToJail":
                    currentPlayer.isInPrison = true;
                    currentPlayer.Position = tiles[10];
                    break;
                case "jailCard":
                    currentPlayer.jailCards++;
                    break;
                case "prince":;
                    break;
                case "MoneyForEachHouse":
                    int numberOfHouses = 0;
                    int numberOfHotels = 0;
                    foreach (Tile card in currentPlayer.cards)
                    {
                        if (card.houses != 5)
                            numberOfHouses += card.houses;
                        else
                            numberOfHotels++;
                    }
                    currentPlayer.Money -= (ereignis.cash* numberOfHouses) + (ereignis.cash*4* numberOfHotels);
                    break;
            }
        }

        static private void PrintName(Tile tile)
        {
            Console.ForegroundColor = tile.color;
            Console.Write(tile.name);
            Console.ForegroundColor = ConsoleColor.White;
        }

        static public void StepOnProperty()
        {
            switch (currentPlayer.Position.name)
            {
                case "Ereignisfeld":
                    CallEvent(ereigniskarten);
                    break;
                case "Gemeinschaftsfeld":
                    CallEvent(gemeinschaftskarten);
                    break;
                case "Geh Gefängnis":
                    Console.WriteLine("Du musst wegen öffentlichem Urinieren ins Gefängnis");
                    currentPlayer.Position = tiles[10];
                    currentPlayer.isInPrison = true;
                    break;
                case "Los":
                    Console.WriteLine("Du kriegst 400 €");
                    break;
                case "Einkommensteuer":
                case "Zusatzsteuer":
                    Console.WriteLine("Du musst " + (currentPlayer.Position.autoMoney * -1) + "€ zahlen");
                    break;
                default:
                    if (currentPlayer.Position.price != 0) //Feld ist Grundstück (Straße, Bahnhof, Werke)
                    {
                        if (currentPlayer.Position.owner == null) //Grundstück ist frei
                        {
                            Console.Write("Möchtest du das Grundstück ");
                            PrintName(currentPlayer.Position);
                            Console.WriteLine(" dir für " + currentPlayer.Position.price + "€ einverleiben? (klar/ne mann)");
                            bool breaker = true;
                            while (breaker)
                            {
                                breaker = false;
                                switch (Console.ReadLine())
                                {
                                    case "klar":

                                        if (currentPlayer.Money - currentPlayer.Position.price >= 0) //Genug Geld
                                        {
                                            currentPlayer.Money -= currentPlayer.Position.price;
                                            Console.WriteLine("Du hast " + currentPlayer.Position.price + "€ bezahlt");
                                            currentPlayer.cards.Add(currentPlayer.Position);
                                            currentPlayer.Position.owner = currentPlayer;
                                            if (currentPlayer.Position.name.Contains("bahnhof"))
                                            {
                                                currentPlayer.stationCards++;
                                            }
                                            else if (currentPlayer.Position.name.Contains("werk"))
                                            {
                                                currentPlayer.factoryCards++;
                                            }
                                        }
                                        else //Nicht genug Geld
                                        {
                                            Console.WriteLine("Deine Kredikarte wurde rejected\nHast wohl zu viel gekokst");
                                        }
                                        break;
                                    case "ne mann":
                                        Console.WriteLine("Hast wohl kein Geld lel");
                                        break;
                                    default:
                                        Console.WriteLine("Kommst du aus der Villa oder was?");
                                        breaker = true;
                                        break;
                                }
                            }
                        }
                        else if (currentPlayer.Position.owner != currentPlayer)//Grundstück gehört wem anders
                        {
                            int finalRent = 0;
                            if (currentPlayer.Position.name.Contains("bahnhof"))
                            {
                                finalRent = Tile.trainMultiplier[currentPlayer.Position.owner.stationCards] * currentPlayer.Position.rent;
                            }
                            else if (currentPlayer.Position.name.Contains("werk"))
                            {
                                finalRent = Tile.factoryMultiplier[currentPlayer.Position.owner.factoryCards] * eyes;
                            }   
                            else
                            {
                                finalRent = Tile.houseMultiplier[currentPlayer.Position.houses] * currentPlayer.Position.rent;
                            }

                            Console.WriteLine(currentPlayer.Position.name + " gehört Spieler " + (currentPlayer.Position.owner.number + 1) + "\nDu musst " + finalRent + "€ Miete bezahlen");
                            Console.WriteLine("Schreibe \"bezahlen\" um zu bezahlen");
                            while (true)
                            {
                                if (Console.ReadLine() == "bezahlen")
                                {
                                    currentPlayer.Money -= finalRent;
                                    currentPlayer.Position.owner.Money += finalRent;
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Du kannst dich nicht vor der Miete drücken");
                                }
                                
                            }
                        }
                        else //Grundstück gehört dir
                        {
                            Console.WriteLine("Diese Straße gehört dir");
                        }
                        
                    }
                    break;
            }
        }

        static void PrintMap(int i)
        {
            Tile tile = tiles[i];
            string player = " ";
            if (tile.onTop.Count > 0)
                player = (tile.onTop[0].number +1).ToString();
            string mod = " ";
            if (tile.houses > 0 && tile.houses < 5)
                mod = "h";
            else if (tile.houses == 5)
                mod = "H";
            else if (tile.mapChar != "")
                mod = tile.mapChar;

            Console.BackgroundColor = tile.color;
            Console.Write(player + mod);
        }

    }
        
}
