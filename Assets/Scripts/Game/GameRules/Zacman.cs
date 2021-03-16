using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameRules
{
    public class Zacman : Base.GameRules
    {
        //Players can grow bigger by connecting to other blocks.
        //Largest player becomes pac man, and can no longer grow except by consuming players
        //Other players are incentivized to break apart pac man, or become larger than pac man.
        public PlayerControlManager pacMan;

        float timer = 5;


        Color pacManColor = new Color(1, 0, 0, 0.4f);
        Vector3 bounds = new Vector3(50, 50, 50);
        int seed = 50;

        public override string GetGameDescription()
        {
            return "A game where the biggest player hunts everyone else.";
        }

        public override void Update()
        {
            base.Update();
        }

        void PacManSetUp()
        {
            pacMan.transform.position = Vector3.zero;
            //pacMan.Cluster.SetSize(2);
            //pacMan.Cluster.SetColor(pacManColor);
            //TODO make size twice as big, if no block count;
            //TODO change texture of block
            //TODO disable adding of blocks, except for player blocks.
        }

        public override void PlayersSetup()
        {
            List<PlayerControlManager> players = gameMaster.spawnManager.players.ToList();
            if (pacMan == null)
            {
                int randomPlayer = Random.Range(0, players.Count);
                for (int i = 0; i < players.Count; i++)
                {
                    gameMaster.spawnManager.EquipDefaultPlayer(players[i]);
                    if (i == randomPlayer)
                    {
                        pacMan = players[i];
                    }
                    //players[i].Cluster.trackingBlock.transform.position = Vector3Utils.RandomBoundedVector3(bounds);
                }
            }
            PacManSetUp();
        }

        public override void LevelSetup()
        {
            gameMaster.spawnManager.CreateCage(bounds);
            gameMaster.spawnManager.SpawnQuantumBlocks(seed, bounds);
        }

        public override bool TransitionCondition()
        {
            return false;
        }

        public override bool EndCondition()
        {
            //timer -= Time.deltaTime;
            //if(timer <= 0)
            //{
            //    return true;
            //}
            return false;
        }
    }
}