using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public static class World
    {
        public static readonly List<Item> Items = new List<Item>();
        public static readonly List<Monster> Monsters = new List<Monster>();
        public static readonly List<Quest> Quests = new List<Quest>();
        public static readonly List<Location> Locations = new List<Location>();

        public const int ITEM_ID_AK47 = 1;
        public const int ITEM_ID_ZOMBIE_FLESH = 2;
        public const int ITEM_ID_SKULL = 3;
        public const int ITEM_ID_ROTTEN_PIECE_CLOTH = 4;
        public const int ITEM_ID_CONGEALED_BLOOD = 5;
        public const int ITEM_ID_MACE = 6;
        public const int ITEM_ID_HEALING_POTION = 7;
        public const int ITEM_ID_DIVINE_DISTURBANCE = 8;
        public const int ITEM_ID_ACCESS = 9;

        public const int MONSTER_ID_ZOMBIE = 1;
        public const int MONSTER_ID_GHOUL = 2;
        public const int MONSTER_ID_FOUR_HORSEMEN = 3;

        public const int QUEST_ID_CLEAR_NORTHERN_OUTSKIRTS = 1;
        public const int QUEST_ID_WESTERN_WAR = 2;

        public const int LOCATION_ID_PORTAL = 1;
        public const int LOCATION_ID_TOWN_SQUARE = 2;
        public const int LOCATION_ID_GUARD_POST = 3;
        public const int LOCATION_ID_ALCHEMIST_HUT = 4;
        public const int LOCATION_ID_NORTHERN_OUTSKIRTS = 5;
        public const int LOCATION_ID_CASTLE = 6;
        public const int LOCATION_ID_FORTIFICATIONS = 7;
        public const int LOCATION_ID_EASTERN_LINE = 8;
        public const int LOCATION_ID_PLAINS = 9;

        static World()
        {
            PopulateItems();
            PopulateMonsters();
            PopulateQuests();
            PopulateLocations();

        }

        private static void PopulateItems()
        {
            Items.Add(new Weapon(ITEM_ID_AK47, "AK-47", "AK's", 0, 5));
            Items.Add(new Item(ITEM_ID_ZOMBIE_FLESH, "Zombie flesh", "Zombie flesh"));
            Items.Add(new Item(ITEM_ID_SKULL, "Skull", "Skulls"));
            Items.Add(new Item(ITEM_ID_ROTTEN_PIECE_CLOTH, "Rotten piece of cloth", "Rotten pieces of cloth"));
            Items.Add(new Item(ITEM_ID_CONGEALED_BLOOD, "Congealed blood", "Congealed blood"));
            Items.Add(new Weapon(ITEM_ID_MACE, "AK-74", "74s", 3, 10));
            Items.Add(new HealingPotion(ITEM_ID_HEALING_POTION, "Healing potion", "Healing potions", 5));
            Items.Add(new Item(ITEM_ID_DIVINE_DISTURBANCE, "Congealed blood", "Congealed blood"));
            Items.Add(new Item(ITEM_ID_ACCESS, "Access", "Access"));
        }

        private static void PopulateMonsters()
        {
            Monster zombie = new Monster(MONSTER_ID_ZOMBIE, "Zombie", 5, 3, 10, 3, 3);
            zombie.LootTable.Add(new LootItem(ItemByID(ITEM_ID_ZOMBIE_FLESH), 75, false));
            zombie.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SKULL), 75, true));

            Monster ghoul = new Monster(MONSTER_ID_GHOUL, "Ghoul", 5, 3, 10, 3, 3);
            ghoul.LootTable.Add(new LootItem(ItemByID(ITEM_ID_ROTTEN_PIECE_CLOTH), 75, false));
            ghoul.LootTable.Add(new LootItem(ItemByID(ITEM_ID_CONGEALED_BLOOD), 75, true));

            Monster fourHorsemen = new Monster(MONSTER_ID_FOUR_HORSEMEN, "Horseman", 20, 5, 40, 10, 10);
            fourHorsemen.LootTable.Add(new LootItem(
            ItemByID(ITEM_ID_MACE), 75, true));
            fourHorsemen.LootTable.Add(new LootItem(
            ItemByID(ITEM_ID_DIVINE_DISTURBANCE), 25, false));

            Monsters.Add(zombie);
            Monsters.Add(ghoul);
            Monsters.Add(fourHorsemen);
        }


        private static void PopulateQuests()
        {
            Quest clearNorthernOutskirts = new Quest(QUEST_ID_CLEAR_NORTHERN_OUTSKIRTS,
            "Clear the northern outskirts" +
            "",
            "Kill zombies in the northern outskirts of town and bring back 3 pieces of zombie flesh. You will receive a healing potion and 10 gold", 20, 10);

            clearNorthernOutskirts.QuestCompletionItems.Add(new QuestCompletionItem(
            ItemByID(ITEM_ID_ZOMBIE_FLESH), 3));

            clearNorthernOutskirts.RewardItem = ItemByID(ITEM_ID_HEALING_POTION);
            
            

            Quest westernWar = new Quest(QUEST_ID_WESTERN_WAR,
            "Western war" +
            "",
            "Kill ghouls on the western fortifications of the castle and bring back 3 rotten pieces of cloth to prove your contribution to the war effort. You will receive 20 gold pieces, and access to the eastern plains where the Four Horsemen are rapidly approaching", 20, 20);

            westernWar.QuestCompletionItems.Add(new QuestCompletionItem(
            ItemByID(ITEM_ID_ROTTEN_PIECE_CLOTH), 3));

            westernWar.RewardItem = ItemByID(ITEM_ID_ACCESS);
           

            Quests.Add(clearNorthernOutskirts);
            Quests.Add(westernWar);
        }

        private static void PopulateLocations()
        {
            // Create each location
            Location portal = new Location(LOCATION_ID_PORTAL, "Portal" +
                "",
            "Your starting point;  a warping purple gateway which you have come through but cannot come back in.");

            Location townSquare = new Location(LOCATION_ID_TOWN_SQUARE,
            "Town square" +
            "", "Panic is setting in as people are desperately trying to evacuate before disaster strikes.");

            Location alchemistHut = new Location(LOCATION_ID_ALCHEMIST_HUT,
            "Alchemist's hut" +
            "", "An elderly man is frantically flipping through books and filing through shelfs of miscellaneous ingredients, trying to find some sort of solution to the impending problem at hand.");
            alchemistHut.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_NORTHERN_OUTSKIRTS);

            Location northernOutskirts = new Location(LOCATION_ID_NORTHERN_OUTSKIRTS,
            "Northern outskirts", "Zombies meander about; people have already left their homes by now. The undead's only sense of a compass is the faint scent of human flesh and desperation.");
            northernOutskirts.MonsterLivingHere = MonsterByID(MONSTER_ID_ZOMBIE);

            Location castle = new Location(LOCATION_ID_CASTLE,
            "Castle" +
            "", "The king is visibly distressed, sitting in his throne room with a furrowed brow and an intense expression of thought; conjuring up an scenario in his mind where he and his subjects can be annihilated honourably.");
            castle.QuestAvailableHere = QuestByID(QUEST_ID_WESTERN_WAR);

            Location fortifications = new Location(LOCATION_ID_FORTIFICATIONS,
            "Western fortifications" +
            "", "A horde of ghouls attempt to breach the western walls.");
            fortifications.MonsterLivingHere = MonsterByID(MONSTER_ID_GHOUL);

            Location guardPost = new Location(LOCATION_ID_GUARD_POST,
            "Guard post" +
            "", "A timid scout is perched up high on a post. He knows he is near death, and can see the culprits heading towards him at blistering speeds.",
            ItemByID(ITEM_ID_ACCESS));
            Location easternLine = new Location(LOCATION_ID_EASTERN_LINE, "Eastern line", "A line of the most seasoned veterans - the elite few - guard the eastern front. They are ready to die with honour.");

            Location plains = new Location(LOCATION_ID_PLAINS,
             "Plains" +
             "", "A white horse approaches...!!!");
            plains.MonsterLivingHere = MonsterByID(MONSTER_ID_FOUR_HORSEMEN);

            // Link the locations together
            portal.LocationToNorth = townSquare;

            townSquare.LocationToNorth = alchemistHut;
            townSquare.LocationToSouth = portal;
            townSquare.LocationToEast = easternLine;
            townSquare.LocationToWest = castle;

            castle.LocationToEast = townSquare;
            castle.LocationToWest = fortifications;

            fortifications.LocationToEast = castle;

            alchemistHut.LocationToSouth = townSquare;
            alchemistHut.LocationToNorth = northernOutskirts;

            northernOutskirts.LocationToSouth = alchemistHut;

            guardPost.LocationToEast = plains;
            guardPost.LocationToWest = easternLine;
            easternLine.LocationToWest = townSquare;
            easternLine.LocationToEast = guardPost;

            plains.LocationToWest = guardPost;

            // Add the locations to the static list
            Locations.Add(portal);
            Locations.Add(townSquare);
            Locations.Add(guardPost);
            Locations.Add(alchemistHut);
            Locations.Add(northernOutskirts);
            Locations.Add(castle);
            Locations.Add(fortifications);
            Locations.Add(easternLine);
            Locations.Add(plains);
        }

        public static Item ItemByID(int id)
        {
            foreach (Item item in Items)
            {
                if (item.ID == id)
                {
                    return item;
                }
            }
            return null;
        }

        public static Monster MonsterByID(int id)
        {
            foreach (Monster monster in Monsters)

            {
                if (monster.ID == id)
                {
                    return monster;
                }
            }
            return null;
        }

        public static Quest QuestByID(int id)
        {
            foreach (Quest quest in Quests)
            {
                if(quest.ID == id)
                {
                    return quest;
                }
            }
            return null;

        }

        public static Location LocationByID(int id)
        {
            foreach (Location location in Locations)
            {
                if(location.ID == id)
                {
                    return location;
                }
            }
            return null;

            
        }






    }
}
