namespace Ello

open Godot

type Ship() =
    inherit KinematicBody2D()

    member this.ShootAudio =
        this.GetNode(new NodePath("Shoot")) :?> AudioStreamPlayer
    
    member this.TakeDamageAudio=
        this.GetNode(new NodePath("TakeDamage")) :?> AudioStreamPlayer
    
    member this.DieAudio=
        this.GetNode(new NodePath("Die")) :?> AudioStreamPlayer

    [<Export(PropertyHint.File, "*.wav")>]
    member val ShootSound = DefaultSoundPaths.PlayerShoot with get,set

    [<Export(PropertyHint.File, "*.wav")>]
    member val TakeDamageSound = DefaultSoundPaths.PlayerShoot with get,set

    [<Export(PropertyHint.File, "*.wav")>]
    member val DeathSound = DefaultSoundPaths.PlayerShoot with get,set
    
    member val HpProvider = HealthProvider(EntityHealth.Default())
    
    [<Export>]
    member val IsShootingEnabled = false with get,set

    member val ShootDirection = Down with get, set

    member val MuzzlePath = "Muzzle" with get, set

    [<Export>]
    member val Acceleration = 10f with get, set

    [<Export>]
    member val Speed = 100f with get, set    

    [<Export>]
    member this.MaxHp
        with get () = this.HpProvider.MaxHp
        and set (value) = 
            this.HpProvider.MaxHp <- value

    [<Export>]
    member this.CurrentHp
        with get () = this.HpProvider.CurrentHp
        and set (value) = this.HpProvider.CurrentHp <- value

    [<Export(PropertyHint.File, "*.tscn")>]
    member val BulletPath: string = Constants.BulletPath with get, set

    abstract member OnBulletCollision : Node * float -> unit

    default this.OnBulletCollision(body: Node, attackPower: float) : unit =
        this.TakeDamageAudio.Play()
        let applyDamage =
            fun () ->
                let e = body :?> Ship                
                e.HpProvider.TakeDamage(attackPower) |> ignore

        match body.Name.ToLower() with
        | "player" -> applyDamage ()
        | name when name.Contains("enemy") -> applyDamage ()
        | name when  name.Contains("anxiety")  -> applyDamage ()
        | _ -> ignore ()       

    abstract member InstantiateBullet : string -> BulletFs

    default this.InstantiateBullet scenePath =
        let bulletType = GD.Load<PackedScene>(scenePath)
        let instance = bulletType.Instance() :?> BulletFs
        instance.OnCollisionFunc <- this.OnBulletCollision
        instance

    abstract member Shoot : unit -> unit
    abstract member GetMuzzle : unit -> Position2D

    default this.GetMuzzle() =
        this.GetNode<Position2D>(new NodePath(this.MuzzlePath))

    default this.Shoot() =
        this.ShootAudio.Play()
        let bulletInstance = this.InstantiateBullet this.BulletPath
        this.AddChild(bulletInstance)
        let muzzle =
            this.GetNode<Position2D>(new NodePath(this.MuzzlePath))
        bulletInstance.SetAsToplevel(true)
        bulletInstance.GlobalPosition <- muzzle.GlobalPosition
        bulletInstance.Velocity <- MoveDirectionUtils.MoveDirToVector(this.ShootDirection)        
    
    abstract member ApplyConstantAcceleration : Vector2 -> Vector2

    default this.ApplyConstantAcceleration(velocity: Vector2) =
        Vector2(velocity.x, velocity.y - this.Acceleration)
