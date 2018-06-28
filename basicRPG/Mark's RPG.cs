﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using Engine;

namespace basicRPG
{
    public partial class basicRPG : Form
    {
        private Player _player;
        private Monster _currentMonster;

        public basicRPG()
        {
            InitializeComponent();

            
            _player = new Player(10, 10, 20, 0, 1);
            MoveTo(World.LocationByID(World.LOCATION_ID_PORTAL));
            _player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_AK47), 1));

            rtbMessages.Text += "You are Mohandas Karamchand Gandhi, also known as Mahatma Gandhi. Normally an extreme advocate of peace, you have been brainwashed into thinking that war and violence are the answer. Sent through a portal to the Middle Ages equipped with an AK-47, you find yourself by a medieval city which is about to be destroyed by zombies, ghouls, and the 4 Horsemen of the apocalypse; the perfect environment for a bloodthirsty, battle-ready peace advocate." + Environment.NewLine;
            rtbMessages.Text += Environment.NewLine;
            

            lblHealth.Text = _player.CurrentHealth.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();

        }

        private void btnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToNorth);
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToEast);
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToSouth);
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToWest);
        }

        private void MoveTo(Location newLocation)
        {
            //Does the location have any required items
            if (!_player.HasRequiredItemToEnterThisLocation(newLocation))
            {
                rtbMessages.Text += "You must have " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                ScrollToBottomOfMessages();
                return;
            }

            // Update the player's current location
            _player.CurrentLocation = newLocation;

            changeBackgroundImage(newLocation);

            // Show/hide available movement buttons
            btnNorth.Visible = (newLocation.LocationToNorth != null);
            btnEast.Visible = (newLocation.LocationToEast != null);
            btnSouth.Visible = (newLocation.LocationToSouth != null);
            btnWest.Visible = (newLocation.LocationToWest != null);

            // Display current location name and description
            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;

            // Completely heal the player
            _player.CurrentHealth = _player.MaximumHealth;

            // Update Hit Points in UI
            lblHealth.Text = _player.CurrentHealth.ToString();

            // Does the location have a quest?
            if (newLocation.QuestAvailableHere != null)
            {
                // See if the player already has the quest, and if they've completed it
                bool playerAlreadyHasQuest = _player.HasThisQuest(newLocation.QuestAvailableHere);
                bool playerAlreadyCompletedQuest = _player.CompletedThisQuest(newLocation.QuestAvailableHere);

                // See if the player already has the quest
                if (playerAlreadyHasQuest)
                {
                    // If the player has not completed the quest yet
                    if (!playerAlreadyCompletedQuest)
                    {
                        // See if the player has all the items needed to complete the quest
                        bool playerHasAllItemsToCompleteQuest = _player.HasAllQuestCompletionItems(newLocation.QuestAvailableHere);

                        // The player has all items required to complete the quest
                        if (playerHasAllItemsToCompleteQuest)
                        {
                            // Display message
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You complete the '" + newLocation.QuestAvailableHere.Name + "' quest." + Environment.NewLine;

                            // Remove quest items from inventory
                            _player.RemoveQuestCompletionItems(newLocation.QuestAvailableHere);

                            // Give quest rewards
                            rtbMessages.Text += "You receive: " + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + " gold" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                            rtbMessages.Text += Environment.NewLine;
                            ScrollToBottomOfMessages();
                            _player.ExperiencePoints += newLocation.QuestAvailableHere.RewardExperiencePoints;
                            _player.Gold += newLocation.QuestAvailableHere.RewardGold;

                            // Add the reward item to the player's inventory
                            _player.AddItemToInventory(newLocation.QuestAvailableHere.RewardItem);

                            // Mark the quest as completed
                            _player.MarkQuestCompleted(newLocation.QuestAvailableHere);
                        }
                    }
                }
                else
                {
                    // The player does not already have the quest

                    // Display the messages
                    rtbMessages.Text += "You receive the " + newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
                    rtbMessages.Text += "To complete it, return with:" + Environment.NewLine;
                    foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        if (qci.Quantity == 1)
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine;
                        }
                        else
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.NamePlural + Environment.NewLine;
                        }
                    }
                    rtbMessages.Text += Environment.NewLine;
                    ScrollToBottomOfMessages();
                    // Add the quest to the player's quest list
                    _player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }

            // Does the location have a monster?
            if (newLocation.MonsterLivingHere != null)
            {
                rtbMessages.Text += "You see a " + newLocation.MonsterLivingHere.Name + Environment.NewLine;
                ScrollToBottomOfMessages();
                // Make a new monster, using the values from the standard monster in the World.Monster list
                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);

                _currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaximumDamage,
                    standardMonster.RewardExperiencePoints, standardMonster.RewardGold, standardMonster.CurrentHealth, standardMonster.MaximumHealth);

                foreach (LootItem lootItem in standardMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(lootItem);
                }

                cboWeapons.Visible = true;
                cboPotions.Visible = true;
                btnUseWeapon.Visible = true;
                btnUsePotion.Visible = true;
            }
            else
            {
                _currentMonster = null;

                cboWeapons.Visible = false;
                cboPotions.Visible = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;
            }

            // Refresh player's inventory list
            UpdateInventoryListInUI();

            // Refresh player's quest list
            UpdateQuestListInUI();

            // Refresh player's weapons combobox
            UpdateWeaponListInUI();

            // Refresh player's potions combobox
            UpdatePotionListInUI();
        }

        private void changeBackgroundImage(Location newLocation)
        {
            Size horsemanSize = new Size(400, 325);
            Size humanoidSize = new Size(285, 380);
           
            

            if (newLocation == (World.LocationByID(World.LOCATION_ID_PORTAL)))
            {
                picBackground.Image = Properties.Resources.portal1;
                picEntity.Visible = false;
            }
            if (newLocation == (World.LocationByID(World.LOCATION_ID_TOWN_SQUARE)))
            {
                picBackground.Image = Properties.Resources.town_centre;

                picEntity.Visible = false;
                lblEnemyDamage.Visible = false;
                lblPlayerDamage.Visible = false;
            }
            if (newLocation == (World.LocationByID(World.LOCATION_ID_GUARD_POST)))
            {
                picBackground.Image = Properties.Resources.post;
                picEntity.Visible = false;
                lblEnemyDamage.Visible = false;
                lblPlayerDamage.Visible = false;
            }
            if (newLocation == (World.LocationByID(World.LOCATION_ID_ALCHEMIST_HUT)))
            {
                picBackground.Image = Properties.Resources.hut;

                picEntity.Visible = true;
            }
            if (newLocation == (World.LocationByID(World.LOCATION_ID_NORTHERN_OUTSKIRTS)))
            {
                picBackground.Image = Properties.Resources.outskirt;

                picEntity.Visible = true;
                picEntity.Image = Properties.Resources.zombie;
                picEntity.Size = humanoidSize;
                lblEnemyDamage.Visible = true;
                lblPlayerDamage.Visible = true;
            }
            if (newLocation == (World.LocationByID(World.LOCATION_ID_CASTLE)))
            {
                picBackground.Image = Properties.Resources.castle;

                picEntity.Visible = false;
                lblEnemyDamage.Visible = false;
                lblPlayerDamage.Visible = false;
            }
            if (newLocation == (World.LocationByID(World.LOCATION_ID_FORTIFICATIONS)))
            {
                picBackground.Image = Properties.Resources.fortifications;

                picEntity.Visible = true;
                picEntity.Image = Properties.Resources.ghoul;
                picEntity.Size = humanoidSize;
                lblEnemyDamage.Visible = true;
                lblPlayerDamage.Visible = true;
            }
            if (newLocation == (World.LocationByID(World.LOCATION_ID_EASTERN_LINE)))
            {
                picBackground.Image = Properties.Resources.eastern_line;
                picEntity.Visible = false;
                lblEnemyDamage.Visible = false;
                lblPlayerDamage.Visible = false;
            }
            if (newLocation == (World.LocationByID(World.LOCATION_ID_PLAINS)))
            {
                picBackground.Image = Properties.Resources.plain;

                picEntity.Visible = true;
                picEntity.Image = Properties.Resources.horseman;
                picEntity.Size = horsemanSize;
                lblEnemyDamage.Visible = true;
                lblPlayerDamage.Visible = true;
            }
        }


        private void UpdateInventoryListInUI()
        {
            dgvInventory.RowHeadersVisible = false;

            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Name";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "Quantity";

            dgvInventory.Rows.Clear();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[] { inventoryItem.Details.Name, inventoryItem.Quantity.ToString() });
                }
            }
        }

        private void UpdateQuestListInUI()
        {
            dgvQuests.RowHeadersVisible = false;

            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "Name";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "Done?";

            dgvQuests.Rows.Clear();

            foreach (PlayerQuest playerQuest in _player.Quests)
            {
                dgvQuests.Rows.Add(new[] { playerQuest.Details.Name, playerQuest.IsCompleted.ToString() });
            }
        }

        private void UpdateWeaponListInUI()
        {
            List<Weapon> weapons = new List<Weapon>();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Details is Weapon)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        weapons.Add((Weapon)inventoryItem.Details);
                    }
                }
            }

            if (weapons.Count == 0)
            {
                // The player doesn't have any weapons, so hide the weapon combobox and "Use" button
                cboWeapons.Visible = false;
                btnUseWeapon.Visible = false;
            }
            else
            {
                cboWeapons.DataSource = weapons;
                cboWeapons.DisplayMember = "Name";
                cboWeapons.ValueMember = "ID";

                cboWeapons.SelectedIndex = 0;
            }
        }

        private void UpdatePotionListInUI()
        {
            List<HealingPotion> healingPotions = new List<HealingPotion>();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Details is HealingPotion)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        healingPotions.Add((HealingPotion)inventoryItem.Details);
                    }
                }
            }

            if (healingPotions.Count == 0)
            {
                // The player doesn't have any potions, so hide the potion combobox and "Use" button
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
            else
            {
                cboPotions.DataSource = healingPotions;
                cboPotions.DisplayMember = "Name";
                cboPotions.ValueMember = "ID";

                cboPotions.SelectedIndex = 0;
            }
        }

        private void btnUseWeapon_Click(object sender, EventArgs e)
        {
            // Get the currently selected weapon from the cboWeapons ComboBox
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;

            // Determine the amount of damage to do to the monster
            int damageToMonster = RandomNumberGenerator.NumberBetween(currentWeapon.MinimumDamage, currentWeapon.MaximumDamage);

            // Apply the damage to the monster's CurrentHealth
            _currentMonster.CurrentHealth -= damageToMonster;

            // Display message
            rtbMessages.Text += "You hit the " + _currentMonster.Name + " for " + damageToMonster.ToString() + " points." + Environment.NewLine;
            lblEnemyDamage.Text = damageToMonster.ToString();

            ScrollToBottomOfMessages();
            // Check if the monster is dead
            if (_currentMonster.CurrentHealth <= 0)
            {
                // Monster is dead
                rtbMessages.Text += Environment.NewLine;
                rtbMessages.Text += "You defeated the " + _currentMonster.Name + Environment.NewLine;

                // Give player experience points for killing the monster
                _player.ExperiencePoints += _currentMonster.RewardExperiencePoints;
                rtbMessages.Text += "You receive " + _currentMonster.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;

                // Give player gold for killing the monster 
                _player.Gold += _currentMonster.RewardGold;
                rtbMessages.Text += "You receive " + _currentMonster.RewardGold.ToString() + " gold" + Environment.NewLine;
                ScrollToBottomOfMessages();
                // Get random loot items from the monster
                List<InventoryItem> lootedItems = new List<InventoryItem>();

                // Add items to the lootedItems list, comparing a random number to the drop percentage
                foreach (LootItem lootItem in _currentMonster.LootTable)
                {
                    if (RandomNumberGenerator.NumberBetween(1, 100) <= lootItem.DropPercentage)
                    {
                        lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                    }
                }

                // If no items were randomly selected, then add the default loot item(s).
                if (lootedItems.Count == 0)
                {
                    foreach (LootItem lootItem in _currentMonster.LootTable)
                    {
                        if (lootItem.IsDefaultItem)
                        {
                            lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                        }
                    }
                }

                // Add the looted items to the player's inventory
                foreach (InventoryItem inventoryItem in lootedItems)
                {
                    _player.AddItemToInventory(inventoryItem.Details);

                    if (inventoryItem.Quantity == 1)
                    {
                        rtbMessages.Text += "You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.Name + Environment.NewLine;
                        ScrollToBottomOfMessages();
                    }
                    else
                    {
                        rtbMessages.Text += "You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.NamePlural + Environment.NewLine;
                        ScrollToBottomOfMessages();
                    }
                }

                // Refresh player information and inventory controls
                lblHealth.Text = _player.CurrentHealth.ToString();
                lblGold.Text = _player.Gold.ToString();
                lblExperience.Text = _player.ExperiencePoints.ToString();
                lblLevel.Text = _player.Level.ToString();

                UpdateInventoryListInUI();
                UpdateWeaponListInUI();
                UpdatePotionListInUI();

                // Add a blank line to the messages box, just for appearance.
                rtbMessages.Text += Environment.NewLine;

                // Move player to current location (to heal player and create a new monster to fight)
                MoveTo(_player.CurrentLocation);
            }
            else
            {
                // Monster is still alive

                // Determine the amount of damage the monster does to the player
                int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentMonster.MaximumDamage);

                // Display message
                rtbMessages.Text += "The " + _currentMonster.Name + " did " + damageToPlayer.ToString() + " points of damage." + Environment.NewLine;
               lblPlayerDamage.Text = damageToPlayer.ToString();
                ScrollToBottomOfMessages();
                // Subtract damage from player
                _player.CurrentHealth -= damageToPlayer;

                // Refresh player data in UI
                lblHealth.Text = _player.CurrentHealth.ToString();

                if (_player.CurrentHealth <= 0)
                {
                    // Display message
                    rtbMessages.Text += "The " + _currentMonster.Name + " killed you." + Environment.NewLine;
                    rtbMessages.Text += "You're teleported back to the cathedral; a divine force has given you another chance." + Environment.NewLine;
                    ScrollToBottomOfMessages();
                    // Move player to "Home"
                    MoveTo(World.LocationByID(World.LOCATION_ID_PORTAL));
                }
            }
        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {
            // Get the currently selected potion from the combobox
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;

            // Add healing amount to the player's current hit points
            _player.CurrentHealth = (_player.CurrentHealth + potion.AmountToHeal);

            // CurrentHealth cannot exceed player's MaximumHealth
            if (_player.CurrentHealth > _player.MaximumHealth)
            {
                _player.CurrentHealth = _player.MaximumHealth;
            }

            // Remove the potion from the player's inventory
            foreach (InventoryItem ii in _player.Inventory)
            {
                if (ii.Details.ID == potion.ID)
                {
                    ii.Quantity--;
                    break;
                }
            }

            // Display message
            rtbMessages.Text += "You drink a " + potion.Name + Environment.NewLine;
            ScrollToBottomOfMessages();
            // Monster gets their turn to attack

            // Determine the amount of damage the monster does to the player
            int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentMonster.MaximumDamage);

            // Display message
            rtbMessages.Text += "The " + _currentMonster.Name + " did " + damageToPlayer.ToString() + " points of damage." + Environment.NewLine;
            ScrollToBottomOfMessages();
            // Subtract damage from player
            _player.CurrentHealth -= damageToPlayer;

            if (_player.CurrentHealth <= 0)
            {
                // Display message
                rtbMessages.Text += "The " + _currentMonster.Name + " killed you." + Environment.NewLine;
                ScrollToBottomOfMessages();
                // Move player to "Home"
                MoveTo(World.LocationByID(World.LOCATION_ID_PORTAL));
            }

            // Refresh player data in UI
            lblHealth.Text = _player.CurrentHealth.ToString();
            UpdateInventoryListInUI();
            UpdatePotionListInUI();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void btnShowQuests_Click(object sender, EventArgs e)
        {

            if (dgvQuests.Visible == false)
            {
                dgvQuests.Visible = true;
            }
            else
            {
                dgvQuests.Visible = false;
            }
        }

        private void btnShowInventory_Click(object sender, EventArgs e)
        {


            if (dgvInventory.Visible == false)
            {
                dgvInventory.Visible = true;
            }
            else
            {
                dgvInventory.Visible = false;
            }

        }

        private void mapToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            
                WorldMap mapScreen = new WorldMap();
                mapScreen.StartPosition = FormStartPosition.CenterParent;
                mapScreen.ShowDialog(this);
            
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void tutorialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tutorial gameTutorial = new Tutorial();
            gameTutorial.ShowDialog(this);
        }
        private void ScrollToBottomOfMessages()
        {
            rtbMessages.SelectionStart = rtbMessages.Text.Length;
            rtbMessages.ScrollToCaret();
        }


    }
}