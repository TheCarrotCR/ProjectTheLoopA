using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phantom : MonoBehaviour
{
    public struct MemoryCell
    {
        public MemoryCell(Player.State state, Vector2 transformScale, Vector2 speedVector, bool allowedMoveLeft, bool allowedMoveRight, bool allowedClimbing, bool isNull)
        {
            this.state = state;
            this.transformScale = transformScale;
            this.speedVector = speedVector;
            this.allowedMoveLeft = allowedMoveLeft;
            this.allowedMoveRight = allowedMoveRight;
            this.allowedClimbing = allowedClimbing;
            this.isNull = isNull;
        }

        public MemoryCell(Player.State state, Vector2 transformScale, Vector2 speedVector, bool allowedMoveLeft, bool allowedMoveRight, bool allowedClimbing) 
            : this(state, transformScale, speedVector, allowedMoveLeft, allowedMoveRight, allowedClimbing, false) {}

        public Player.State state { get; }
        public Vector2 speedVector { get; }
        public Vector2 transformScale { get; }
        public bool allowedMoveLeft { get; }
        public bool allowedMoveRight { get; }
        public bool allowedClimbing { get; }
        public bool isNull{ get; }
    }

    public List<MemoryCell> memory;
    //TODO: Pop() работает некорректно при значениях MemorySpeed != 2
    public int memorySpeed;
    public int memoryPosition;

    public void Start()
    {
        memory = new List<MemoryCell>();
        memoryPosition = 0;
        gameObject.SetActive(!GetComponent<Player>().isPhantom);
    }

    public void Remember(Player.State state, Vector2 transformScale, Vector2 speedVector, bool allowedMoveLeft, bool allowedMoveRight, bool allowedClimbing)
    {
        var newCell = new MemoryCell(state, transformScale, speedVector, allowedMoveLeft, allowedMoveRight, allowedClimbing);
        memory.Add(newCell);
    }

    //TODO: сейчас MemorySpeed обязательно должен быть 2
    public MemoryCell Pop() 
    {
        if (memory.Count < 1)
            return new MemoryCell(Player.State.Idle, Vector2.zero, Vector2.zero, true, true, false, true);
        var lastCell = memory[memory.Count - 1];
        memory.RemoveAt(memory.Count - 1);
        return lastCell;
    }

    public void Play(Player p)
    {
        if (memoryPosition > memory.Count - 1)
            return;
        var memento = memory[memoryPosition++];
        if (!memento.isNull)
        {
            p.speedVector = memento.speedVector;
            if (p.speedVector.y != 0 && !p.CanClimb())
                p.speedVector.y = 0;
            if (p.speedVector.x < 0 && !p.CanMoveLeft() || p.speedVector.x > 0 && !p.CanMoveRight())
                p.speedVector.x = 0;
            p.state = p.speedVector.y != 0 ? Player.State.Climb : p.speedVector.x == 0 ? Player.State.Idle : Player.State.Run;
            if (p.speedVector.x != 0)
            {
                var scale = transform.localScale;
                scale.x = Mathf.Abs(transform.localScale.x) * p.speedVector.x / Mathf.Abs(p.speedVector.x);
                transform.localScale = scale;
            }
            GetComponent<Rigidbody2D>().gravityScale = p.state == Player.State.Climb ? 0 : 1;
            transform.Translate(new Vector2(p.speedVector.x, p.speedVector.y));
        }
    }
}
