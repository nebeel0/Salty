using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameRules
{
    public class PvE : Base.GameRules
    {
        //Essentially a tower defense game, where you can be a hero/controller/builder etc to grow your system
        public PlayerControlManager pacMan;

        float timer = 5;

        Vector3 bounds = new Vector3(50, 50, 50);
        int seed = 1;

        public override string GetGameDescription()
        {
            return "Essentially a tower defense game with a few twists.";
        }

        public override void Update()
        {
            base.Update();
        }

        public override void PlayersSetup()
        {
            List<PlayerControlManager> players = gameMaster.spawnManager.players.ToList();
            int randomPlayer = Random.Range(0, players.Count);
            for (int i = 0; i < players.Count; i++)
            {
                gameMaster.spawnManager.EquipDefaultPlayer(players[i]);
            }
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