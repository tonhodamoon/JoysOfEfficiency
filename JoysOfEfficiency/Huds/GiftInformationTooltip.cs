﻿using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace JoysOfEfficiency.Huds
{
    public class GiftInformationTooltip
    {

        private static bool _unableToGift;
        private static string _hoverText;

        public static void UpdateTooltip()
        {
            _hoverText = null;
            if (!Context.IsPlayerFree)
            {
                return;
            }
            Farmer player = Game1.player;
            _unableToGift = false;
            if (Game1.player.CurrentItem == null || !player.CurrentItem.canBeGivenAsGift() || player.currentLocation == null || player.currentLocation.characters.Count == 0)
            {
                return;
            }

            List<NPC> npcList = player.currentLocation.characters.Where(a => a != null && a.isVillager()).ToList();
            foreach (NPC npc in npcList)
            {
                Rectangle npcRect = new Rectangle(
                    (int)npc.position.X,
                    (int)(npc.position.Y - npc.Sprite.getHeight() - Game1.tileSize / 1.5f),
                    (int)(npc.Sprite.getWidth() * 3 + npc.Sprite.getWidth() / 1.5f),
                    (int)(npc.Sprite.getHeight() * 3.5f)
                );

                if (!npcRect.Contains(
                    Game1.getMouseX() + Game1.viewport.X,
                    Game1.getMouseY() + Game1.viewport.Y))
                {
                    continue;
                }

                //Mouse hovered on the NPC
                StringBuilder key = new StringBuilder("taste.");
                if (player.friendshipData.ContainsKey(npc.Name) && Game1.NPCGiftTastes.ContainsKey(npc.Name))
                {
                    Friendship friendship = player.friendshipData[npc.Name];
                    if (friendship.GiftsThisWeek >= 2 && !IsNPCMarriedWithPlayer(npc, player))
                    {
                         key.Append("gavetwogifts.");
                         _unableToGift = true;
                    }
                    else if (friendship.GiftsToday > 0)
                    {
                        //Day restriction
                        key.Append("gavetoday.");
                        _unableToGift = true;
                    }
                    else if (npc.canReceiveThisItemAsGift(player.CurrentItem))
                    {
                        switch (npc.getGiftTasteForThisItem(player.CurrentItem))
                        {
                            case 0:
                                key.Append("love.");
                                break;
                            case 2:
                                key.Append("like.");
                                break;
                            case 4:
                                key.Append("dislike.");
                                break;
                            case 6:
                                key.Append("hate.");
                                break;
                            default:
                                key.Append("neutral.");
                                break;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                switch (npc.Gender)
                {
                    case NPC.female:
                        key.Append("female");
                        break;
                    default:
                        key.Append("male");
                        break;
                }
                _hoverText = Util.Helper.Translation.Get(key.ToString());
            }
        }

        internal static void DrawTooltip(ModEntry modInstance)
        {
            if (Context.IsPlayerFree && !string.IsNullOrEmpty(_hoverText) && Game1.player.CurrentItem != null)
            {
                Util.DrawSimpleTextbox(Game1.spriteBatch, _hoverText, Game1.dialogueFont, modInstance, false, _unableToGift ? null : Game1.player.CurrentItem);
            }
        }

        private static bool IsNPCMarriedWithPlayer(NPC npc, Farmer player)
        {
            return npc.isMarried() && npc.getSpouse().UniqueMultiplayerID == player.UniqueMultiplayerID;
        }
    }
}
