namespace Ello

open Godot

type BulletFs() =
    inherit Area2D()

    member val OnCollisionFunc = ignore with get, set

    [<Export>]
    member val AttackPower = 1 with get, set

    [<Export>]
    member val Speed = 100f with get, set

    member val Velocity = Vector2.Zero with get, set

    [<Export>]
    member val PlayerPath = "res://Player.tscn" with get, set
    
    [<Export>]
    member val TimeToLive = 5f with get, set

    member val AccumulatedTime = 0f with get, set

    member this.ApplyDamage(target: IHealth) = target.TakeDamage this.AttackPower

    member this.OnBulletBodyShapeEntered(body: PhysicsBody2D) =
        this.OnCollisionFunc(body, this.AttackPower)
        this.QueueFree()

    override this._PhysicsProcess(delta) =
        this.Translate(this.Velocity * this.Speed * delta)
        this.AccumulatedTime <- this.AccumulatedTime + delta
        if this.AccumulatedTime > this.TimeToLive then this.QueueFree()
