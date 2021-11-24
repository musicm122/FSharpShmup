namespace Ello

open Godot

type EnemyFs() =
    inherit KinematicBody2D()

    [<Export>]
    member val ShootDirection = Down with get,set

    [<Export>]
    member val Speed = 100f with get, set

    [<Export>]
    member val CurrentVelocity = Vector2.Zero with get, set

    [<Export>]
    member val Hp: int = 100 with get, set

    [<Export(PropertyHint.File, "*.tscn")>]
    member val BulletPath: string = Constants.BulletPath with get, set

    [<Export>]
    member val CooldownTime: float32 = 2f with get, set

    [<Export>]
    member val MaxMoveTime = 20f with get, set

    member val CurrentMoveDirection = MoveDirection.Left with get, set

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

        let muzzle =
            this.GetNode<Position2D>(new NodePath("Muzzle"))

        bulletInstance.SetAsToplevel(true)
        bulletInstance.GlobalPosition <- muzzle.GlobalPosition
        bulletInstance.Velocity <- MoveDirectionUtils.MoveDirToVector(this.ShootDirection)

    member this.GetHealth() = this.Hp

    member this.TakeDamage amt = this.Hp <- this.Hp - amt

    member this.HealDamage amt = this.Hp <- this.Hp + amt

    member this.Die() = this.QueueFree()

    member this.ChangeDirection() =
        match this.CurrentMoveDirection with
        | Left -> Right
        | Right -> Left
        | Up -> Down
        | Down -> Up

    member this.GetVelocityInMoveDirection() =
        match this.CurrentMoveDirection with
        | Left -> new Vector2(this.CurrentVelocity.x - this.Speed,0f)
        | Right -> new Vector2(this.CurrentVelocity.x + this.Speed,0f)
        | Up -> new Vector2(0f,this.CurrentVelocity.y - this.Speed)
        | Down -> new Vector2(0f,this.CurrentVelocity.y + this.Speed)

    override this._PhysicsProcess(delta) =
        this.AccumulatedMoveTime <- this.AccumulatedMoveTime + delta
        this.CurrentVelocity<-Vector2.Zero
        this.CurrentVelocity <- this.GetVelocityInMoveDirection()
        this.MoveAndSlide(this.CurrentVelocity) |> ignore
        if this.AccumulatedMoveTime >= this.MaxMoveTime then
            this.CurrentMoveDirection <- this.ChangeDirection()
            this.Shoot();
            GD.Print("ChangeDirection to " + this.CurrentMoveDirection.ToString() )
            this.AccumulatedMoveTime <- 0f