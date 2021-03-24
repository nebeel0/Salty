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
        float timer = 2;
        Vector3 bounds = new Vector3(50, 50, 50);
        int currentWave = 0;
        float nextWaveCooldownCounter = 0;
        float nextWaveCooldown = 10;

        public override string GetGameDescription()
        {
            return "Essentially a tower defense game with a few twists.";
        }

        public override void Update()
        {
            base.Update();
            List<GameObject> enemies = new List<GameObject>();
            foreach(PlayerControlManager ai in gameMaster.spawnManager.aiPlayers)
            {
                if(!gameMaster.spawnManager.players.Contains(ai.Character.ParentPlayer))
                {
                    enemies.Add(ai.gameObject);
                }
            }
            foreach(PlayerControlManager player in gameMaster.spawnManager.players)
            {
                player.GetComponent<Character.Managers.CharacterTargetManager>().SetTargets(enemies);
            }
            if(enemies.Count == 0)
            {
                NextWave();
            }
        }

        void NextWave()
        {
            if (nextWaveCooldownCounter <= 0)
            {
                nextWaveCooldownCounter = nextWaveCooldown;
                currentWave++;
                gameMaster.spawnManager.SpawnQuantumBlocks(System.Math.Pow(2, currentWave), bounds, antiMatterFlag: true);
            }
            nextWaveCooldownCounter -= Time.deltaTime;
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
        }

        public override bool TransitionCondition()
        {
            return false;
        }

        public override bool EndCondition()
        {
            foreach (PlayerControlManager player in gameMaster.spawnManager.players)
            {
                if (!player.IsGhost())
                {
                    timer = 1; //Reset Timer
                    return false;
                }
            }
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                return true;
            }
            return false;
        }
    }
}