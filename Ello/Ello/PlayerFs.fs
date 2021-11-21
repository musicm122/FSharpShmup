namespace Ello

open Godot
open Ello.GDUtils

type PlayerFs() =
    inherit KinematicBody2D()

    [<Export>]
    member val RateOfFire = 1f with get, set

    [<Export>]
    member val Speed = 100f with get, set

    [<Export>]
    member val Hp: int = 100 with get, set

    [<Export(PropertyHint.File, "*.tscn")>]
    member val BulletPath: string = "res://bullet.tscn" with get, set

    //[<Export>]
    //member val MuzzlePath: string = "Muzzle" with get, set

    [<Export>]
    member val CooldownTime: float32 = 2f with get, set

    member this.OnBulletCollision(body: PhysicsBody2D, attackPower) =
        if body.Name = "player" then
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
        bulletInstance.GlobalPosition <- muzzle.GlobalPosition

    member this.GetHealth() = this.Hp

    member this.TakeDamage amt = this.Hp <- this.Hp - amt

    member this.HealDamage amt = this.Hp <- this.Hp + amt

    member this.Die() = GD.Print("You Died")

    member this.GetInputMovemet(currentVelocity: Vector2) : Vector2 =
        let mutable x = 0f
        let mutable y = 0f

        if Input.IsActionPressed("right") then
            x <- currentVelocity.x + this.Speed
        elif Input.IsActionPressed("left") then
            x <- currentVelocity.x - this.Speed

        if Input.IsActionPressed("up") then
            y <- currentVelocity.y - this.Speed
        elif Input.IsActionPressed("down") then
            y <- currentVelocity.y + this.Speed

        new Vector2(x, y)

    member this.ShootCheck() : bool = Input.IsActionPressed("shoot")

    override this._PhysicsProcess(delta) =
        let mutable currentVelocity = Vector2.Zero
        currentVelocity <- this.GetInputMovemet(currentVelocity)
        this.MoveAndSlide(currentVelocity) |> ignore

        if Input.IsActionJustPressed("shoot") then
            this.Shoot()
