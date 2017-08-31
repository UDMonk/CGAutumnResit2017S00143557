using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace CGAutumnResitServer
{
    //Changed original planned game into racing game so that I don't have to keep track of projectiles and the like. Might add them back in depending on how confusing this becomes.
    //Keeping track of who's in first is finicky; the most reasonable way to go about it is to keep track of which lap/checkpoint the player has reached
    //and assigning positions based on that. Of course, this becomes a lot more exact once the finish line is crossed. Until then it's entirely possible
    //for the server to consider everyone to be "in first place", if they are all on the same lap and checkpoint.

    // I have no idea how authentication works. More pressingly, I have no idea how to impliment it with the player class.
    // Or even if I should. That sounds like the kind of thing that I assumed last time which made finishing the assignment impossible

    // I know authentication (Individual User Accounts, which I think was the best option upon reading up on it) is "important". Theoretically you'd need it to keep track of
    //stats and achievements as well as a username for theoretical chat implementation.
    // It's when the existence of a 'user' and a 'player' as seperate but linked entities comes up that my brain starts curling up in the corner of my skull in an effort
    //to get away from the program. I just can't keep up with how to link the two, possibly because they both function seperately. Like the account needs to know which
    //Player its user controls in order to record a win/loss, but keeping track of which player belongs to which account is something I just. Cannot. Think my way around.
    //Like would that be done Server-side or Client-side? I just. Agh.

    //Other fun things: PlayerID assignment has literally zero contingencies. It assumes that up to four players will join, and that none of them will leave. If someone loses connection
    //they can't be replaced without restarting.

    

    public class Player
    {
        public string PlayerID;
        public int Avatar; //1-4, corresponds to each player sprite
        public int LapNumber; //1-3
        public int CheckpointNumber; //0-3 (Fourth Checkpoint is a new lap, resets to zero)
        public double PosX;
        public double PosY;
        //rotation variable
    }

    public static class HubState

    {
        public static List<Player> players = new List<Player>()

        {
            new Player { PlayerID = "player1", Avatar = 1, LapNumber = 0, CheckpointNumber = 0, PosX = 5, PosY = 50 }, //X and Y are placeholder values until I build the track and know where the starting line is going to be
            new Player { PlayerID = "player2", Avatar = 2, LapNumber = 0, CheckpointNumber = 0, PosX = (5 + 18), PosY = 50 },
            new Player { PlayerID = "player3", Avatar = 3, LapNumber = 0, CheckpointNumber = 0, PosX = (10 + 36), PosY = 50 },
            new Player { PlayerID = "player4", Avatar = 4, LapNumber = 0, CheckpointNumber = 0, PosX = (15 + 54), PosY = 50 },
        };
    }

    public class GameHub : Hub
    {
        private int PlayerAssignmentInteger = 0;

        public void Hello()
        {
            Clients.All.hello();
        }

        public void sendPlayers()
        {
            Clients.Caller.RecievePlayers(HubState.players);
        }

        public List<Player> getPlayers()
        {
            return HubState.players;
        }

        public void updatePlayer(string playerident, int lap, int check, int posx, int posy)
        {
            foreach(Player x in HubState.players)
                if(playerident == x.PlayerID)
                {
                    x.LapNumber = lap;
                    x.CheckpointNumber = check;
                    x.PosX = posx;
                    x.PosY = posy;
                }
        }

        #region Update Placements Old
        //public List<string> UpdatePlacements()
        //{
        //    //Need to redo all of this, there's no guarantee the placement values will be accurate unless the player order matches the placement order
        //    List<string> placements = new List<string>();
        //    List<int> placementValues = new List<int>();
        //    int comparisonValue = 0;
        //    int playerOrderIndicator = 1;
        //    string placementPlaceholder = "First";

        //    foreach(Player x in HubState.players)
        //    {
        //        int value = 0;
        //value = ((x.LapNumber* 4) + (x.CheckpointNumber));
        //        placementValues.Add(value);
        //    }

    //    //placementValues.OrderByDescending().ToList();

    //    foreach(int pValue in placementValues)
    //    {
    //        if(pValue > comparisonValue)
    //        {
    //            comparisonValue = pValue;
    //            placementPlaceholder = "First";
    //        }
    //        else if(pValue == comparisonValue)
    //        {

    //        }
    //        else if(pValue <= comparisonValue)
    //        {
    //            if (placementPlaceholder == "First")
    //            {
    //                placementPlaceholder = "Second";
    //            }
    //            else if(placementPlaceholder == "Second")
    //            {
    //                placementPlaceholder = "Third";
    //            }
    //            else if (placementPlaceholder == "Third")
    //            {
    //                placementPlaceholder = "Fourth";
    //            }
    //            else
    //            {
    //                placementPlaceholder = "Error";
    //            }
    //        }

    //        if(playerOrderIndicator == 1)
    //        {
    //            placements.Add(String.Format("{0} is in {1}", "player1", placementPlaceholder));
    //        }
    //        else if (playerOrderIndicator == 2)
    //        {
    //            placements.Add(String.Format("{0} is in {1}", "player2", placementPlaceholder));
    //        }
    //        else if (playerOrderIndicator == 3)
    //        {
    //            placements.Add(String.Format("{0} is in {1}", "player3", placementPlaceholder));
    //        }
    //        else if (playerOrderIndicator == 4)
    //        {
    //            placements.Add(String.Format("{0} is in {1}", "player4", placementPlaceholder));
    //        }
    //        placements.Add(String.Format("{0} is in {1}"));
    //    }

    //    foreach(Player x in HubState.players)
    //    {
    //        //int value = 0;
    //        //value = ((x.LapNumber * 4) + (x.CheckpointNumber));
    //        foreach(int y in placementValues)
    //        {

    //        }
    //    }

    //    return placements;
    //}
    #endregion Update Placements Old

        public List<string> UpdatePlacements()
        {
            //What do? Need to return a list of strings corresponding to each player, preferrably in the numerical order. (player1, player2, player3, player4)
            //Need to work out what 'place' each player is in (first, second, third, fourth) (It is possible for everyone to register as 'first' if they are all on the same lap and checkpoint)
            //I need a way to compare all the values and assign places based on who's ahead of who
            List<string> placements = new List<string>();
            List<string> placementPlaceholders = new List<string>();
            int[] placementScore = new int[4];
            int highestScore = 0;
            int secondHighestScore = 0;
            int thirdHighestScore = 0;
            int fourthHighestScore = 0;

            for (int i = 0; i < 4; i++)
            {
                placementScore[i] = ((HubState.players[i].LapNumber * 4) + HubState.players[i].CheckpointNumber);
                if(highestScore == 0)
                {
                    highestScore = placementScore[i];
                }
                else if(placementScore[i] <= highestScore && secondHighestScore == 0)
                {
                    secondHighestScore = placementScore[i];
                }
                else if(placementScore[i] <= highestScore && placementScore[i] <= secondHighestScore && thirdHighestScore == 0)
                {
                    thirdHighestScore = placementScore[i];
                }
                else if (placementScore[i] <= highestScore && placementScore[i] <= secondHighestScore && placementScore[i] <= thirdHighestScore && fourthHighestScore == 0)
                {
                    fourthHighestScore = placementScore[i];
                }
                //if all players have the same score, or if the players happen to follow a descending order of scores, the below if/elses should never fire
                if (placementScore[i] > highestScore)
                {
                    fourthHighestScore = thirdHighestScore;
                    thirdHighestScore = secondHighestScore;
                    secondHighestScore = highestScore;
                    highestScore = placementScore[i];
                }
                else if (placementScore[i] > secondHighestScore)
                {
                    fourthHighestScore = thirdHighestScore;
                    thirdHighestScore = secondHighestScore;
                    secondHighestScore = placementScore[i];
                }
                else if (placementScore[i] > thirdHighestScore)
                {
                    fourthHighestScore = thirdHighestScore;
                    thirdHighestScore = placementScore[i];
                }
                else if (placementScore[i] > fourthHighestScore)
                {
                    //this particular statement should not be reachable if the four player limit is still in effect
                    fourthHighestScore = placementScore[i];
                }
                else
                    break;
            }

            for (int i = 0; i < 4; i++)
            {
                if (placementScore[i] == highestScore)
                {
                    placements.Add(String.Format("player{0} is in First", (i + 1)));
                }
                else if (placementScore[i] == secondHighestScore)
                {
                    placements.Add(String.Format("player{0} is in Second", (i + 1)));
                }
                else if (placementScore[i] == thirdHighestScore)
                {
                    placements.Add(String.Format("player{0} is in Third", (i + 1)));
                }
                else if (placementScore[i] == fourthHighestScore)
                {
                    placements.Add(String.Format("player{0} is in Fourth", (i + 1)));
                }
                else
                {
                    placements.Add("Error in Placement Calculation");
                }
            }

            return placements;
        }

        public void sendPlacements()
        {
            Clients.Caller.RecievePlacements(UpdatePlacements());
        }

        public string requestID()
        {
            //Client Requests ID
            if(PlayerAssignmentInteger < 4)
            {
                PlayerAssignmentInteger += 1;
                return (string.Format("player{0}", PlayerAssignmentInteger));
            }
            else
            {
                return "Error in ID Assignment";
                //placements are going to be drawn in the top left of the screen, if 'Error in ID Assignment is in First/Second/Third/Fourth' pops up you know something's gone wrong here
            }
        }

        public Player requestStartPoint(Player localP)
        {
            //Client Uses ID to Get Start Position
            foreach(Player p in HubState.players)
            {
                if (localP.PlayerID == p.PlayerID)
                {
                    localP.PosX = p.PosX;
                    localP.PosY = p.PosY;
                }
            }
            return localP;
        }

        public void playerUpdate(Player p)
        {
            //Client Sends Player to Server, Includes Position and Current Placement Data (Lap, Checkpoint)
            foreach(Player x in HubState.players)
            {
                if(p.PlayerID == x.PlayerID)
                {
                    x.Avatar = p.Avatar;
                    x.CheckpointNumber = p.CheckpointNumber;
                    x.LapNumber = p.LapNumber;
                    x.PosX = p.PosX;
                    x.PosY = p.PosY;
                }
            }
        }
    }
}