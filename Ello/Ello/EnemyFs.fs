namespace Ello

open Godot

type MoveDirection =
    | Left
    | Right
    | Up 
    | Down

type EnemyFs() =
    inherit KinematicBody2D()

    [<Export>]
    member val Text = "Hello World!" with get, set

    override this._Ready() =
        GD.Print(this.Text)

    [<Export>]
    member val Hp: int = 100 with get, set

    [<Export(PropertyHint.File, "*.tscn")>]
    member val BulletPath: string = "res://bullet.tscn" with get, set

    [<Export>]
    member val CooldownTime: float32 = 2f with get, set

    [<Export>]
    member val MaxMoveTime = 20f with get, set

    member val CurrentMoveDirection= MoveDirection.Left with get, set
    
    member val AccumulatedMoveTime = 0f with get, set

    member this.OnBulletCollision(body: PhysicsBody2D, attackPower) =
        if body.Name = "Enemy" then
            this.TakeDamage(attackPower)

    member this.InstantiateBullet scenePath =
        let bulletType = GD.Load<PackedScene>(scenePath)
        let instance = bulletType.Instance() :?> BulletFs
        instance.OnCollisionFunc <- this.OnBulletCollision
        instance

    member this.Shoot() =
        let bulletInstance = this.InstantiateBullet this.BulletPath
        this.AddChild(bulletInstance)
        let muzzle = this.GetNode<Position2D>("Muzzle")
        bulletInstance.SetAsToplevel(true)
        bulletInstance.GlobalPosition <- muzzle.GlobalPosition
        bulletInstance.Velocity <- Vector2.Up

    member this.GetHealth() = this.Hp

    member this.TakeDamage amt = this.Hp <- this.Hp - amt

    member this.HealDamage amt = this.Hp <- this.Hp + amt

    member this.Die() = 
        this.QueueFree()    

    member this.ChangeDirection() = 
        match this.CurrentMoveDirection with 
        | Left -> Right
        | Right -> Left
        | Up -> Down
        | Down -> Up

    member this.GetVelocityInMoveDirection() =
        match this.CurrentMoveDirection with 
        | Left -> this.currentVelocity.x - this.Speed
        | Right -> this.currentVelocity.x + this.Speed
        | Up -> this.currentVelocity.y - this.Speed
        | Down -> this.currentVelocity.y + this.Speed

    override this._PhysicsProcess(delta) =
        
        this.AccumulatedMoveTime <- this.AccumulatedMoveTime + delta
        if this.AccumulatedMoveTime >= this.MaxMoveTime then 
            this.CurrentMoveDirection <- this.ChangeDirection()
            this.AccumulatedMoveTime <- 0f
