using Exiled.API.Features;
using System.Collections.Generic;

namespace MurderMystery.API.Features
{
    public class MMPlayer
    {
        internal MMPlayer(Player player) => Player = player;

        public static List<MMPlayer> List { get; } = new List<MMPlayer>();

        public Player Player { get; }

        public static bool Get(Player ply, out MMPlayer player)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].Player == ply)
                {
                    player = List[i];
                    return true;
                }
            }

            player = null;
            return false;
        }

        internal void Verified()
        {
        }
        internal void Destroying()
        {
        }
    }
}
