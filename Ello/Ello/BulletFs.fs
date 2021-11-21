namespace Ello

open Godot

type BulletFs() =
    inherit Area2D()

    member val OnCollisionFunc = ignore with get, set

    [<Export>]
    member val AttackPower = 1 with get, set

    [<Export>]
    member val Speed = 100f with get, set

    [<Export>]
    member val PlayerPath = "res://Player.tscn" with get, set
    
    [<Export>]
    member val TimeToLive = 5f with get, set

    member val AccumulatedTime = 0f with get, set

    override this._Ready() =
        GD.Print(
            "Bullet instantiated with attack power "
            + this.AttackPower.ToString()
            + "and speed "
            + this.Speed.ToString()
        )

    member this.ApplyDamage(target: IHealth) = target.TakeDamage this.AttackPower

    member this.OnBulletBodyShapeEntered(body: PhysicsBody2D) =
        GD.Print("OnBulletBodyShapeEntered called")
        this.OnCollisionFunc(body, this.AttackPower)
        this.QueueFree()

    override this._PhysicsProcess(delta) =
        //this.Position <- this.Position - this.Transform.y * this.Speed * delta
        this.Position <- this.Position - this.Transform.y * this.Speed * delta
        this.AccumulatedTime <- this.AccumulatedTime + delta
        if this.AccumulatedTime > this.TimeToLive then this.QueueFree()
