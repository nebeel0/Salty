using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class AI : MonoBehaviour
{
    //Controls input for a playercontroller
    float cooldown = 0.3f;
    float timer = 2;
    bool active;

    PlayerControlManager Player //TODO remove this as it can be any controller
    {
        get { return GetComponent<PlayerControlManager>(); }
    }

    public void Start()
    {
        //active = Random.Range(0, 10) == 0;
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.IsGhost())
        {
            Player.gameMaster.spawnManager.DestroyAI(Player);
        }
        else
        {
            if (active)
            {
                TargetPlayerWalk();
            }
        }
    }

    void RandomWalk()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = cooldown;
            //Vector2 randomPerspective = new Vector2(Random.Range(-20f, 20f), Random.Range(-20f, 20f));
            //Player.GetComponent<Controller.RotationSubController>().LookAt(firstPlayer.transform);
            //Player.GetComponent<Controller.MovementSubController>().OnMove(new Vector2(0, 1));
            //Player.SetLookDelta(randomPerspective);
            //Player.OnHold();
        }
    }

    void TargetPlayerWalk()
    {
        while (Player.GetComponent<Controller.FlyMovementSubController>() == null)
        {
            Player.OnScrollSlot(new Vector2(0, 1));
        }


        if (Player.Character.ParentPlayer == null)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = cooldown;
                PlayerControlManager firstPlayer = Player.gameMaster.spawnManager.players.First();
                if (firstPlayer != null && !firstPlayer.IsGhost())
                {
                    Player.GetComponent<Character.Managers.CharacterTargetManager>().SetTarget(firstPlayer.gameObject);
                    Player.GetComponent<Controller.RotationSubController>().OnTargetLock(1);
                    Player.GetComponent<Controller.MovementSubController>().OnMove(new Vector2(0, 1));
                }
                else
                {
                    Debug.Log("Player doesn't exist yet.");
                }
            }
        }
        else
        {
            Player.GetComponent<Controller.MovementSubController>().OnMove(new Vector2(0, 0));
        }

    }

}
